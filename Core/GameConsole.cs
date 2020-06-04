using RDRN_Core.Native;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace RDRN_Core
{
	internal class GameConsole : Script
	{
		int cursorPos = 0;
		int commandPos = -1;
		int currentPage = 1;
		bool isOpen = false;
		string input = string.Empty;
		List<string> lineHistory = new List<string>();
		List<string> commandHistory = new List<string>();
		ConcurrentQueue<string[]> outputQueue = new ConcurrentQueue<string[]>();
		Dictionary<string, List<ConsoleCommand>> commands = new Dictionary<string, List<ConsoleCommand>>();
		DateTime lastClosed;
		Task<MethodInfo> compilerTask;
		const int BASE_WIDTH = 1280;
		const int BASE_HEIGHT = 720;
		const int CONSOLE_WIDTH = BASE_WIDTH;
		const int CONSOLE_HEIGHT = BASE_HEIGHT / 3;
		const int INPUT_HEIGHT = 20;
		const int LINES_PER_PAGE = 16;

		static readonly Color InputColor = Color.White;
		static readonly Color InputColorBusy = Color.DarkGray;
		static readonly Color OutputColor = Color.White;
		static readonly Color PrefixColor = Color.FromArgb(255, 189, 216, 216);
		static readonly Color BackgroundColor = Color.FromArgb(120, Color.Black);
		static readonly Color AltBackgroundColor = Color.FromArgb(120, 180, 15, 15);

		[DllImport("user32.dll")]
		static extern int ToUnicode(
			uint virtualKeyCode, uint scanCode, byte[] keyboardState,
			[Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)] StringBuilder receivingBuffer, int bufferSize, uint flags);

		/// <summary>
		/// Gets or sets whether the console is open.
		/// </summary>
		public bool IsOpen
		{
			get => isOpen;
			set
			{
				isOpen = value;
				DisableControlsThisFrame();
				if (!isOpen)
					lastClosed = DateTime.UtcNow.AddMilliseconds(200); // Hack so the input gets blocked long enough
			}
		}

		/// <summary>
		/// Register the specified method as a console command.
		/// </summary>
		/// <param name="command">The command attribute of the method.</param>
		/// <param name="methodInfo">The method information.</param>
		public void RegisterCommand(ConsoleCommand command, MethodInfo methodInfo)
		{
			command.MethodInfo = methodInfo;

			if (!commands.ContainsKey(command.Namespace))
				commands[command.Namespace] = new List<ConsoleCommand>();
			commands[command.Namespace].Add(command);
		}
		/// <summary>
		/// Register all methods with a <see cref="ConsoleCommand"/> attribute in the specified type as console commands.
		/// </summary>
		/// <param name="type">The type to search for console command methods.</param>
		public void RegisterCommands(Type type)
		{
			foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
			{
				try
				{
					foreach (var attribute in method.GetCustomAttributes<ConsoleCommand>(true))
					{
						RegisterCommand(attribute, method);
					}
				}
				catch (Exception ex)
				{
					//LogManager.WriteLog(LogLevel.Error, "Failed to search for console commands in ", type.FullName, ".", method.Name, ": ", ex.ToString());
				}
			}
		}
		/// <summary>
		/// Unregister all methods with a <see cref="ConsoleCommand"/> attribute that were previously registered.
		/// </summary>
		/// <param name="type">The type to search for console command methods.</param>
		public void UnregisterCommands(Type type)
		{
			foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
			{
				string space = method.DeclaringType.FullName;

				if (commands.ContainsKey(space))
				{
					commands[space].RemoveAll(x => x.MethodInfo == method);

					if (commands[space].Count == 0)
						commands.Remove(space);
				}
			}
		}

		/// <summary>
		/// Add text lines to the console. This call is thread-safe.
		/// </summary>
		/// <param name="prefix">The prefix for each line.</param>
		/// <param name="messages">The lines to add to the console.</param>
		void AddLines(string prefix, string[] messages)
		{
			AddLines(prefix, messages, "~w~");
		}
		/// <summary>
		/// Add colored text lines to the console. This call is thread-safe.
		/// </summary>
		/// <param name="prefix">The prefix for each line.</param>
		/// <param name="messages">The lines to add to the console.</param>
		/// <param name="color">The color of those lines.</param>
		void AddLines(string prefix, string[] messages, string color)
		{
			for (int i = 0; i < messages.Length; i++) // Add proper styling
				messages[i] = $"~c~[{DateTime.Now.ToString("HH:mm:ss")}] ~w~{prefix} {color}{messages[i]}";

			outputQueue.Enqueue(messages);
		}
		/// <summary>
		/// Add text to the console input line.
		/// </summary>
		/// <param name="text">The text to add.</param>
		void AddToInput(string text)
		{
			if (string.IsNullOrEmpty(text))
				return;

			input = input.Insert(cursorPos, text);
			cursorPos++;
		}
		/// <summary>
		/// Paste clipboard content into the console input line.
		/// </summary>
		void AddClipboardContent()
		{
			string text = Clipboard.GetText();
			text = text.Replace("\n", string.Empty); // TODO Keep this?

			AddToInput(text);
		}

		/// <summary>
		/// Clear the console input line.
		/// </summary>
		void ClearInput()
		{
			input = string.Empty;
			cursorPos = 0;
		}
		/// <summary>
		/// Clears the console output.
		/// </summary>
		public void Clear()
		{
			lineHistory.Clear();
			currentPage = 1;
		}

		/// <summary>
		/// Writes an info message to the console.
		/// </summary>
		/// <param name="msg">The composite format string.</param>
		/// <param name="args">The formatting arguments.</param>
		public void PrintInfo(string msg, params object[] args)
		{
			if (args.Length > 0)
				msg = String.Format(msg, args);
			AddLines("[~b~INFO~w~] ", msg.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries));
		}
		/// <summary>
		/// Writes an error message to the console.
		/// </summary>
		/// <param name="msg">The composite format string.</param>
		/// <param name="args">The formatting arguments.</param>
		public void PrintError(string msg, params object[] args)
		{
			if (args.Length > 0)
				msg = String.Format(msg, args);
			AddLines("[~r~ERROR~w~] ", msg.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries));
		}
		/// <summary>
		/// Writes a warning message to the console.
		/// </summary>
		/// <param name="msg">The composite format string.</param>
		/// <param name="args">The formatting arguments.</param>
		public void PrintWarning(string msg, params object[] args)
		{
			if (args.Length > 0)
				msg = String.Format(msg, args);
			AddLines("[~o~WARNING~w~] ", msg.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries));
		}

		/// <summary>
		/// Writes the help text for all commands to the console.
		/// </summary>
		public void PrintHelpText()
		{
			StringBuilder help = new StringBuilder();
			foreach (var space in commands.Keys)
			{
				help.AppendLine($"[{space}]");
				foreach (var command in commands[space])
				{
					help.Append("    ~h~" + command.Name + "(");
					foreach (var arg in command.MethodInfo.GetParameters())
						help.Append(arg.ParameterType.Name + " " + arg.Name + ",");
					if (command.MethodInfo.GetParameters().Length > 0)
						help.Length--; // Remove trailing comma
					if (command.Help.Length > 0)
						help.AppendLine(")~h~: " + command.Help);
					else
						help.AppendLine(")~h~");
				}
			}

			PrintInfo(help.ToString());
		}
		/// <summary>
		/// Writes the help text for the specified command to the console.
		/// </summary>
		/// <param name="commandName">The command name to check.</param>
		public void PrintHelpText(string commandName)
		{
			foreach (var space in commands.Keys)
			{
				foreach (var command in commands[space])
				{
					if (command.Name == commandName)
					{
						PrintInfo(command.Name + ": " + command.Help);
						return;
					}
				}
			}
		}


		void PageUp()
		{
			if (currentPage < ((lineHistory.Count + LINES_PER_PAGE - 1) / LINES_PER_PAGE))
				currentPage++;
		}
		void PageDown()
		{
			if (currentPage > 1)
				currentPage--;
		}
		void GoUpCommandList()
		{
			if (commandHistory.Count == 0 || commandPos >= commandHistory.Count - 1)
				return;

			commandPos++;
			input = commandHistory[commandHistory.Count - commandPos - 1];
			// Reset cursor position to end of input text
			cursorPos = input.Length;
		}
		void GoDownCommandList()
		{
			if (commandHistory.Count == 0 || commandPos <= 0)
				return;

			commandPos--;
			input = commandHistory[commandHistory.Count - commandPos - 1];
			cursorPos = input.Length;
		}

		void ForwardWord()
		{
			var regex = new Regex(@"[^\W_]+");
			Match match = regex.Match(input, cursorPos);
			cursorPos = match.Success ? match.Index + match.Length : input.Length;
		}
		void BackwardWord()
		{
			var regex = new Regex(@"[^\W_]+");
			MatchCollection matches = regex.Matches(input);
			cursorPos = matches.Cast<Match>().Where(x => x.Index < cursorPos).Select(x => x.Index).LastOrDefault();
		}
		void RemoveCharLeft()
		{
			if (input.Length > 0 && cursorPos > 0)
			{
				input = input.Remove(cursorPos - 1, 1);
				cursorPos--;
			}
		}
		void RemoveCharRight()
		{
			if (input.Length > 0 && cursorPos < input.Length)
			{
				input = input.Remove(cursorPos, 1);
			}
		}

		void MoveCursorLeft()
		{
			if (cursorPos > 0)
				cursorPos--;
		}
		void MoveCursorRight()
		{
			if (cursorPos < input.Length)
				cursorPos++;
		}
		void MoveCursorToBegOfLine()
		{
			cursorPos = 0;
		}
		void MoveCursorToEndOfLine()
		{
			cursorPos = input.Length;
		}

		static unsafe void DrawRect(float x, float y, int width, int height, Color color)
		{
			float w = (float)(width) / BASE_WIDTH;
			float h = (float)(height) / BASE_HEIGHT;

			Function.Call(Hash.DRAW_RECT,
				(x / BASE_WIDTH) + w * 0.5f,
				(y / BASE_HEIGHT) + h * 0.5f,
				w, h,
				color.R, color.G, color.B, color.A, true, 0);
		}
		static unsafe void DrawText(float x, float y, string text, Color color)
		{
			float fX = x / (float)1280;
			float fY = y / (float)720;
			Function.Call(Hash.SET_TEXT_SCALE, 0.35f, 0.35f);
			Function.Call(Hash._SET_TEXT_COLOR, color.R, color.G, color.B, color.A);
			var res = Function.Call<long>(Hash._CREATE_VAR_STRING, 10, "LITERAL_STRING", text);
			Function.Call(Hash._DISPLAY_TEXT, res, fX, fY);

		}

		static unsafe void DisableControlsThisFrame()
		{
			Function.Call(Hash.DISABLE_ALL_CONTROL_ACTIONS, 0);

			// LookLeftRight .. LookRightOnly
			for (ulong i = 1; i <= 6; i++)
				Function.Call(Hash.ENABLE_CONTROL_ACTION, 0, i, 0);
		}

		static unsafe float GetTextLength(string text)
		{
			Function.Call(Hash.SET_TEXT_SCALE, 0.35f, 0.35f);
			//RDRN_Module.Native.Func.AddStringPool(text);
			return (float)text.Length;
		}

		internal void OnTick()
		{
			DateTime now = DateTime.UtcNow;

			// Execute compiled input line script
			if (compilerTask != null && compilerTask.IsCompleted)
			{
				if (compilerTask.Result != null)
				{
					try
					{
						var result = compilerTask.Result.Invoke(null, null);
						if (result != null)
							PrintInfo($"[Return Value]: {result}");
					}
					catch (TargetInvocationException ex)
					{
						PrintError($"[Exception]: {ex.InnerException.ToString()}");
					}
				}

				ClearInput();

				// Reset compiler task
				compilerTask = null;
			}

			// Add lines from concurrent queue to history
			if (outputQueue.TryDequeue(out string[] lines))
				foreach (string line in lines)
					lineHistory.Add(line);

			if (!IsOpen)
			{
				// Hack so the input gets blocked long enough
				if (lastClosed > now)
					DisableControlsThisFrame();
				return; // Nothing more to do here when the console is not open
			}

			// Disable controls while the console is open
			DisableControlsThisFrame();

			// Draw background
			DrawRect(0, 0, CONSOLE_WIDTH, CONSOLE_HEIGHT, BackgroundColor);
			// Draw input field
			DrawRect(0, CONSOLE_HEIGHT, CONSOLE_WIDTH, INPUT_HEIGHT, AltBackgroundColor);
			DrawRect(0, CONSOLE_HEIGHT + INPUT_HEIGHT, 80, INPUT_HEIGHT, AltBackgroundColor);
			// Draw input prefix
			DrawText(0, CONSOLE_HEIGHT, "$>", PrefixColor);
			// Draw input text
			DrawText(25, CONSOLE_HEIGHT, input, compilerTask == null ? InputColor : InputColorBusy);
			// Draw page information
			DrawText(5, CONSOLE_HEIGHT + INPUT_HEIGHT, "Page " + currentPage + "/" + System.Math.Max(1, ((lineHistory.Count + (LINES_PER_PAGE - 1)) / LINES_PER_PAGE)), InputColor);

			// Draw blinking cursor
			if (now.Millisecond < 500)
			{
				float length = GetTextLength(input.Substring(0, cursorPos));
				DrawText(25 + (length * CONSOLE_WIDTH) - 4, CONSOLE_HEIGHT, "~w~~h~|~w~", InputColor);
			}

			// Draw console history text
			int historyOffset = lineHistory.Count - (LINES_PER_PAGE * currentPage);
			int historyLength = historyOffset + LINES_PER_PAGE;
			for (int i = System.Math.Max(0, historyOffset); i < historyLength; ++i)
			{
				DrawText(2, (float)((i - historyOffset) * 14), lineHistory[i], OutputColor);
			}
		}

		internal void OnKeyDown(KeyEventArgs e)
		{
			if (!IsOpen && e.KeyCode == Keys.F12)
			{
				IsOpen = true;
			}
			else if (!IsOpen)
				return;

			if (e.KeyCode == Keys.PageUp)
			{
				PageUp();
				return;
			}
			if (e.KeyCode == Keys.PageDown)
			{
				PageDown();
				return;
			}

			switch (e.KeyCode)
			{
				case Keys.Back:
					RemoveCharLeft();
					break;
				case Keys.Delete:
					RemoveCharRight();
					break;
				case Keys.Left:
					if (e.Control)
						BackwardWord();
					else
						MoveCursorLeft();
					break;
				case Keys.Right:
					if (e.Control)
						ForwardWord();
					else
						MoveCursorRight();
					break;
				case Keys.Home:
					MoveCursorToBegOfLine();
					break;
				case Keys.End:
					MoveCursorToEndOfLine();
					break;
				case Keys.Up:
					GoUpCommandList();
					break;
				case Keys.Down:
					GoDownCommandList();
					break;
				case Keys.Enter:
					//CompileExpression();
					break;
				case Keys.Escape:
					IsOpen = false;
					break;
				case Keys.B:
					if (e.Control)
						MoveCursorLeft();
					else if (e.Alt)
						BackwardWord();
					else
						goto default;
					break;
				case Keys.F:
					if (e.Control)
						MoveCursorRight();
					else if (e.Alt)
						ForwardWord();
					else
						goto default;
					break;
				case Keys.A:
					if (e.Control)
						MoveCursorToBegOfLine();
					else
						goto default;
					break;
				case Keys.E:
					if (e.Control)
						MoveCursorToEndOfLine();
					else
						goto default;
					break;
				case Keys.P:
					if (e.Control)
						GoUpCommandList();
					else
						goto default;
					break;
				case Keys.N:
					if (e.Control)
						GoDownCommandList();
					else
						goto default;
					break;
				case Keys.L:
					if (e.Control)
						Clear();
					else
						goto default;
					break;
				case Keys.V:
					if (e.Control)
						AddClipboardContent();
					else
						goto default;
					break;
				default:
					var buf = new StringBuilder(256);
					var keyboardState = new byte[256];
					keyboardState[(int)Keys.Menu] = e.Alt ? (byte)0xff : (byte)0;
					keyboardState[(int)Keys.ShiftKey] = e.Shift ? (byte)0xff : (byte)0;
					keyboardState[(int)Keys.ControlKey] = e.Control ? (byte)0xff : (byte)0;

					// Translate key event to character for text input
					ToUnicode((uint)e.KeyCode, 0, keyboardState, buf, 256, 0);
					AddToInput(buf.ToString());
					break;
			}
		}
	}

	public class ConsoleCommand : Attribute
	{
		public ConsoleCommand() : this(string.Empty)
		{
		}
		public ConsoleCommand(string help)
		{
			Help = help;
		}

		public string Help { get; }

		public string Name => MethodInfo.Name;
		public string Namespace => MethodInfo.DeclaringType.FullName;
		public MethodInfo MethodInfo { get; set; }
	}
}