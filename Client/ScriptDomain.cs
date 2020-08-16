//
// Copyright (C) 2015 crosire
//
// This software is  provided 'as-is', without any express  or implied  warranty. In no event will the
// authors be held liable for any damages arising from the use of this software.
// Permission  is granted  to anyone  to use  this software  for  any  purpose,  including  commercial
// applications, and to alter it and redistribute it freely, subject to the following restrictions:
//
//   1. The origin of this software must not be misrepresented; you must not claim that you  wrote the
//      original  software. If you use this  software  in a product, an  acknowledgment in the product
//      documentation would be appreciated but is not required.
//   2. Altered source versions must  be plainly  marked as such, and  must not be  misrepresented  as
//      being the original software.
//   3. This notice may not be removed or altered from any source distribution.
//

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using WinForms = System.Windows.Forms;

namespace RDRN_Core
{
	interface IScriptTask
	{
		void Run();
	}

	internal class ScriptDomain : MarshalByRefObject, IDisposable
	{
		#region Fields
		private int _executingThreadId;
		private Script _executingScript;
		private List<Script> _runningScripts = new List<Script>();
		private Queue<IScriptTask> _taskQueue = new Queue<IScriptTask>();
		private List<IntPtr> _pinnedStrings = new List<IntPtr>();
		private List<Type> _scriptTypes = new List<Type>();
		private bool _recordKeyboardEvents = true;
		private bool[] _keyboardState = new bool[256];
		private bool disposed = false;
		#endregion

		public ScriptDomain(AppDomain app)
		{
			AppDomain = app;
			AppDomain.AssemblyResolve += new ResolveEventHandler(HandleResolve);
			AppDomain.UnhandledException += new UnhandledExceptionEventHandler(HandleUnhandledException);

			CurrentDomain = this;

			_executingThreadId = Thread.CurrentThread.ManagedThreadId;

			LogManager.WriteLog("[INFO]", "Created new script domain with v", typeof(ScriptDomain).Assembly.GetName().Version.ToString(3), ".");

			Console = new ConsoleScript();
		}
		~ScriptDomain()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
				return;

			CleanupStrings();

			disposed = true;
		}

		public static Script ExecutingScript => CurrentDomain != null ? CurrentDomain._executingScript : null;
		public static ScriptDomain CurrentDomain { get; private set; }
		public string Name => AppDomain.FriendlyName;
		public AppDomain AppDomain { get; private set; }
		public ConsoleScript Console { get; private set; }
		public Script[] RunningScripts => _runningScripts.ToArray();

		public static ScriptDomain Load(string path)
		{
			// Create AppDomain
			var setup = new AppDomainSetup();
			setup.ApplicationBase = path;
			setup.ShadowCopyFiles = "true";
			setup.ShadowCopyDirectories = path;

			var appdomain = AppDomain.CreateDomain("ScriptDomain_" + (path.GetHashCode() * Environment.TickCount).ToString("X"), null, setup, new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted));
			appdomain.InitializeLifetimeService();

			LogManager.WriteLog("[DEBUG]", "Path ", path);
			LogManager.WriteLog("[DEBUG]", "Location ", typeof(ScriptDomain).Assembly.Location);
			LogManager.WriteLog("[DEBUG]", "FullName ", typeof(ScriptDomain).FullName);
			try
			{
				//scriptdomain = (ScriptDomain)(appdomain.CreateInstanceFromAndUnwrap(typeof(ScriptDomain).Assembly.Location, typeof(ScriptDomain).FullName));

				//var obj = appdomain.CreateInstance(typeof(ScriptDomain).Assembly.Location, typeof(ScriptDomain).FullName);
				//scriptdomain = (ScriptDomain)obj.Unwrap();

				CurrentDomain = new ScriptDomain(appdomain);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog("[ERROR]", "Failed to create script domain':", Environment.NewLine, ex.ToString());

				AppDomain.Unload(appdomain);

				return null;
			}
			
			LogManager.WriteLog("[INFO]", "Loading scripts from '", path, "' ...");

			if (Directory.Exists(path))
			{

				
				var filenameAssemblies = new List<string>();

				try
				{
					filenameAssemblies.AddRange(Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories).Where(x => IsManagedAssembly(x)));

					foreach (string filename in filenameAssemblies)
					{
						try
						{
							Assembly.Load(File.ReadAllBytes(filename));
						}
						catch { }
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog("[ERROR]", "Failed to reload scripts:", Environment.NewLine, ex.ToString());

					AppDomain.Unload(appdomain);

					return null;
				}

				uint count = 0;

				try
				{
					foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsSubclassOf(typeof(Script))))
					{
						count++;
						CurrentDomain._scriptTypes.Add(type);
					}
				}
				catch (ReflectionTypeLoadException ex)
				{
				}

				LogManager.WriteLog("[INFO]", "Found ", count.ToString(), " script(s).");
			}
			else
			{
				LogManager.WriteLog("[ERROR]", "Failed to reload scripts because the directory is missing.");
			}
			
			return CurrentDomain;
		}

		public static bool IsManagedAssembly(string fileName)
		{
			using (Stream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			using (BinaryReader binaryReader = new BinaryReader(fileStream))
			{
				if (fileStream.Length < 64)
				{
					return false;
				}

				//PE Header starts @ 0x3C (60). Its a 4 byte header.
				fileStream.Position = 0x3C;
				uint peHeaderPointer = binaryReader.ReadUInt32();
				if (peHeaderPointer == 0)
				{
					peHeaderPointer = 0x80;
				}

				// Ensure there is at least enough room for the following structures:
				//     24 byte PE Signature & Header
				//     28 byte Standard Fields         (24 bytes for PE32+)
				//     68 byte NT Fields               (88 bytes for PE32+)
				// >= 128 byte Data Dictionary Table
				if (peHeaderPointer > fileStream.Length - 256)
				{
					return false;
				}

				// Check the PE signature.  Should equal 'PE\0\0'.
				fileStream.Position = peHeaderPointer;
				uint peHeaderSignature = binaryReader.ReadUInt32();
				if (peHeaderSignature != 0x00004550)
				{
					return false;
				}

				// skip over the PEHeader fields
				fileStream.Position += 20;

				const ushort PE32 = 0x10b;
				const ushort PE32Plus = 0x20b;

				// Read PE magic number from Standard Fields to determine format.
				var peFormat = binaryReader.ReadUInt16();
				if (peFormat != PE32 && peFormat != PE32Plus)
				{
					return false;
				}

				// Read the 15th Data Dictionary RVA field which contains the CLI header RVA.
				// When this is non-zero then the file contains CLI data otherwise not.
				ushort dataDictionaryStart = (ushort)(peHeaderPointer + (peFormat == PE32 ? 232 : 248));
				fileStream.Position = dataDictionaryStart;

				uint cliHeaderRva = binaryReader.ReadUInt32();
				if (cliHeaderRva == 0)
				{
					return false;
				}

				return true;
			}
		}
		public static void Unload(ref ScriptDomain domain)
		{
			LogManager.WriteLog("[INFO]", "Unloading script domain ...");

			domain.Abort();

			AppDomain appdomain = domain.AppDomain;

			domain.Dispose();

			try
			{
				AppDomain.Unload(appdomain);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog("[ERROR]", "Failed to unload deleted script domain:", Environment.NewLine, ex.ToString());
			}

			domain = null;
		}
		private Script InstantiateScript(Type scriptType)
		{
			if (!scriptType.IsSubclassOf(typeof(Script)) || scriptType.IsAbstract)
			{
				return null;
			}

			LogManager.WriteLog("[INFO]", "Instantiating script '", scriptType.FullName, "' ...");

			try
			{
				return (Script)(Activator.CreateInstance(scriptType));
			}
			catch (MissingMethodException)
			{
				LogManager.WriteLog("[ERROR]", "Failed to instantiate script '", scriptType.FullName, "' because no public default constructor was found.");
			}
			catch (TargetInvocationException ex)
			{
				LogManager.WriteLog("[ERROR]", "Failed to instantiate script '", scriptType.FullName, "' because constructor threw an exception:", Environment.NewLine, ex.InnerException.ToString());
			}
			catch (Exception ex)
			{
				LogManager.WriteLog("[ERROR]", "Failed to instantiate script '", scriptType.FullName, "':", Environment.NewLine, ex.ToString());
			}

			return null;
		}

		internal static bool SortScripts(ref List<Type> scriptTypes)
		{
			var graph = new Dictionary<Tuple<string, Type>, List<Type>>();

			foreach (var scriptType in scriptTypes)
			{
				var dependencies = new List<Type>();
				/*
				foreach (RequireScript attribute in (scriptType.Item2).GetCustomAttributes<RequireScript>(true))
				{
					//dependencies.Add(attribute._dependency);
				}

				graph.Add(scriptType, dependencies);*/
			}
			/*
			var result = new List<Tuple<string, Type>>(graph.Count);

			while (graph.Count > 0)
			{
				Tuple<string, Type> scriptType = null;

				foreach (var item in graph)
				{
					if (item.Value.Count == 0)
					{
						scriptType = item.Key;
						break;
					}
				}

				if (scriptType == null)
				{
					LogManager.WriteLog("[ERROR]", "Detected a circular script dependency. Aborting ...");
					return false;
				}

				result.Add(scriptType);
				graph.Remove(scriptType);

				foreach (var item in graph)
				{
					item.Value.Remove(scriptType.Item2);
				}
			}

			scriptTypes = result;
			*/
			return true;
		}
		public void Start()
		{
			if (_runningScripts.Count != 0)
			{
				return;
			}

			// Start console
			//Console.Start();

			// Start script threads
			LogManager.WriteLog("[INFO]", "Starting ", _scriptTypes.Count.ToString(), " script(s) ...");

			if (_scriptTypes.Count == 0 || !SortScripts(ref _scriptTypes))
			{
				return;
			}

			foreach (var script in _scriptTypes.Select(x => InstantiateScript(x)).Where(x => x != null))
			{
				script.Start();
			}
		}
		/*
		public void StartScript(string filename)
		{
			filename = Path.GetFullPath(filename);

			int offset = _scriptTypes.Count;
			string extension = Path.GetExtension(filename);

			if (extension.Equals(".dll", StringComparison.OrdinalIgnoreCase))
			{
				if (!(IsManagedAssembly(filename) && LoadAssembly(filename)))
				{
					return;
				}
			}

			LogManager.WriteLog("[INFO]", "Starting ", (_scriptTypes.Count - offset).ToString(), " script(s) ...");

			for (int i = offset; i < _scriptTypes.Count; i++)
			{
				Script script = InstantiateScript(_scriptTypes[i].Item2);

				if (Object.ReferenceEquals(script, null))
				{
					continue;
				}

				script.Start();
			}
		}
		public void StartAllScripts()
		{
			string basedirectory = CurrentDomain.AppDomain.BaseDirectory;

			if (Directory.Exists(basedirectory))
			{
				var filenameScripts = new List<string>();

				try
				{
					filenameScripts.AddRange(Directory.GetFiles(basedirectory, "*.vb", SearchOption.AllDirectories));
					filenameScripts.AddRange(Directory.GetFiles(basedirectory, "*.cs", SearchOption.AllDirectories));
					filenameScripts.AddRange(Directory.GetFiles(basedirectory, "*.dll", SearchOption.AllDirectories).Where(x => IsManagedAssembly(x)));
				}
				catch (Exception ex)
				{
					LogManager.WriteLog("[ERROR]", "Failed to reload scripts:", Environment.NewLine, ex.ToString());
				}

				int offset = _scriptTypes.Count;

				foreach (var filename in filenameScripts)
				{
					string extension = Path.GetExtension(filename).ToLower();

					if (extension.Equals(".dll", StringComparison.OrdinalIgnoreCase) && !LoadAssembly(filename))
					{
						continue;
					}
				}

				int TotalScriptCount = _scriptTypes.Count;

				LogManager.WriteLog("[INFO]", "Starting ", (TotalScriptCount - offset).ToString(), " script(s) ...");

				for (int i = offset; i < TotalScriptCount; i++)
				{
					Script script = InstantiateScript(_scriptTypes[i].Item2);

					if (ReferenceEquals(script, null))
					{
						continue;
					}

					script.Start();
				}
			}
		}*/
		public void Abort()
		{
			_runningScripts.Remove(Console);

			LogManager.WriteLog("[INFO]", "Stopping ", _runningScripts.Count.ToString(), " script(s) ...");

			foreach (Script script in _runningScripts)
			{
				script.Abort();
			}

			Console.Abort();

			_scriptTypes.Clear();
			_runningScripts.Clear();
		}
		public void AbortScript(string filename)
		{
			filename = Path.GetFullPath(filename);

			foreach (Script script in _runningScripts.Where(x => filename.Equals(x.Filename, StringComparison.OrdinalIgnoreCase)))
			{
				script.Abort();
			}
		}
		public void AbortAllScriptsExceptConsole()
		{
			foreach (Script script in _runningScripts.Where(x => x != Console))
			{
				script.Abort();
			}

			_scriptTypes.Clear();
			_runningScripts.RemoveAll(x => x != Console);
		}
		public static void OnStartScript(Script script)
		{
			ScriptDomain domain = script._scriptdomain;

			domain._runningScripts.Add(script);

			if (ReferenceEquals(script, domain.Console))
			{
				return;
			}

			domain.Console.RegisterCommands(script.GetType());

			LogManager.WriteLog("[INFO]", "Started script '", script.Name, "'.");
		}
		public static void OnAbortScript(Script script)
		{
			ScriptDomain domain = script._scriptdomain;

			if (ReferenceEquals(script, domain.Console))
			{
				return;
			}

			domain.Console.UnregisterCommands(script.GetType());

			LogManager.WriteLog("[INFO]", "Aborted script '", script.Name, "'.");
		}

		public void DoTick()
		{
			// Execute scripts
			for (int i = 0; i < _runningScripts.Count; i++)
			{
				Script script = _runningScripts[i];

				if (!script._running)
				{
					continue;
				}

				_executingScript = script;

				while ((script._running = SignalAndWait(script._continueEvent, script._waitEvent, 5000)) && _taskQueue.Count > 0)
				{
					_taskQueue.Dequeue().Run();
				}

				_executingScript = null;

				if (!script._running)
				{
					LogManager.WriteLog("[ERROR]", "Script '", script.Name, "' is not responding! Aborting ...");

					OnAbortScript(script);
					continue;
				}
			}

			// Clean up pinned strings
			CleanupStrings();
		}
		public void DoKeyboardMessage(WinForms.Keys key, bool status, bool statusCtrl, bool statusShift, bool statusAlt)
		{
			int keycode = (int)key;

			if (keycode < 0 || keycode >= _keyboardState.Length)
			{
				return;
			}

			_keyboardState[keycode] = status;

			if (_recordKeyboardEvents)
			{
				if (statusCtrl)
				{
					key = key | WinForms.Keys.Control;
				}
				if (statusShift)
				{
					key = key | WinForms.Keys.Shift;
				}
				if (statusAlt)
				{
					key = key | WinForms.Keys.Alt;
				}

				var args = new WinForms.KeyEventArgs(key);
				var eventinfo = new Tuple<bool, WinForms.KeyEventArgs>(status, args);

				if (!ReferenceEquals(Console, null) && Console.IsOpen)
				{
					// Do not send keyboard events to other running scripts when console is open
					Console._keyboardEvents.Enqueue(eventinfo);
				}
				else
				{
					foreach (Script script in _runningScripts)
					{
						script._keyboardEvents.Enqueue(eventinfo);
					}
				}
			}
		}

		public void PauseKeyboardEvents(bool pause)
		{
			_recordKeyboardEvents = !pause;
		}
		public void ExecuteTask(IScriptTask task)
		{
			task.Run();
			/*
			if (Thread.CurrentThread.ManagedThreadId == _executingThreadId)
			{
				task.Run();
			}
			else
			{
				_taskQueue.Enqueue(task);

				SignalAndWait(ExecutingScript._waitEvent, ExecutingScript._continueEvent);
			}*/
		}
		
		public IntPtr PinString(string str)
		{
			IntPtr handle = NativeMemory.StringToCoTaskMemUTF8(str);

			if (handle == IntPtr.Zero)
			{
				return NativeMemory.NullString;
			}
			else
			{
				_pinnedStrings.Add(handle);

				return handle;
			}
		}
		private void CleanupStrings()
		{
			foreach (IntPtr handle in _pinnedStrings)
			{
				Marshal.FreeCoTaskMem(handle);
			}

			_pinnedStrings.Clear();
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}

		internal bool IsKeyPressed(WinForms.Keys key)
		{
			return _keyboardState[(int)(key)];
		}

		static private void SignalAndWait(SemaphoreSlim toSignal, SemaphoreSlim toWaitOn)
		{
			toSignal.Release();
			toWaitOn.Wait();
		}
		static private bool SignalAndWait(SemaphoreSlim toSignal, SemaphoreSlim toWaitOn, int timeout)
		{
			toSignal.Release();
			return toWaitOn.Wait(timeout);
		}

		static private Assembly HandleResolve(Object sender, ResolveEventArgs args)
		{
			var assembly = typeof(Script).Assembly;
			var assemblyName = new AssemblyName(args.Name);

			if (assemblyName.Name.StartsWith("ScriptHookVDotNet", StringComparison.OrdinalIgnoreCase))
			{
				if (assemblyName.Version.Major != assembly.GetName().Version.Major)
				{
					LogManager.WriteLog("[WARNING]", "A script references v", assemblyName.Version.ToString(3), " which may not be compatible with the current v" + assembly.GetName().Version.ToString(3), " and was therefore ignored.");
				}
				else
				{
					return assembly;
				}
			}

			return null;
		}
		static internal void HandleUnhandledException(object sender, UnhandledExceptionEventArgs args)
		{
			if (!args.IsTerminating)
			{
				LogManager.WriteLog("[ERROR]", "Caught unhandled exception:", Environment.NewLine, args.ExceptionObject.ToString());
			}
			else
			{
				LogManager.WriteLog("[ERROR]", "Caught fatal unhandled exception:", Environment.NewLine, args.ExceptionObject.ToString());
			}

			if (sender == null || !typeof(Script).IsInstanceOfType(sender))
			{
				return;
			}

			var scriptType = sender.GetType();

			LogManager.WriteLog("[INFO]", "The exception was thrown while executing the script '", scriptType.FullName, "'.");
		}
	}
}
