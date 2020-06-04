using System;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;
using WinForms = System.Windows.Forms;

namespace RDRN_Core
{
	internal abstract class Script
	{
		#region Fields
		internal int _interval = 0;
		internal bool _running = false;
		internal Thread _thread;
		internal SemaphoreSlim _waitEvent = new SemaphoreSlim(0);
		internal SemaphoreSlim _continueEvent = new SemaphoreSlim(0);
		internal ConcurrentQueue<Tuple<bool, WinForms.KeyEventArgs>> _keyboardEvents = new ConcurrentQueue<Tuple<bool, WinForms.KeyEventArgs>>();
		#endregion

		public Script()
		{

		}

		public event EventHandler Tick;

		public event WinForms.KeyEventHandler KeyUp;

		public event WinForms.KeyEventHandler KeyDown;

		public event EventHandler Aborted;

		protected int Interval
		{
			get
			{
				return _interval;
			}
			set
			{
				if (value < 0)
				{
					value = 0;
				}

				_interval = value;
			}
		}

		/// <summary>
		/// Starts execution of this <see cref="Script"/>.
		/// </summary>
		internal void Start()
		{
			ThreadStart threadDelegate = new ThreadStart(MainLoop);
			_thread = new Thread(threadDelegate);
			_thread.Start();

			//ScriptDomain.OnStartScript(this);
		}
		/// <summary>
		/// Aborts execution of this <see cref="Script"/>.
		/// </summary>
		public void Abort()
		{
			try
			{
				Aborted?.Invoke(this, EventArgs.Empty);
			}
			catch (Exception ex)
			{
				//ScriptDomain.HandleUnhandledException(this, new UnhandledExceptionEventArgs(ex, true));
			}

			_running = false;
			_waitEvent.Release();

			if (_thread != null)
			{
				_thread.Abort();
				_thread = null;

				//ScriptDomain.OnAbortScript(this);
			}
		}

		/// <summary>
		/// Pauses execution of the <see cref="Script"/> for a specific amount of time.
		/// Must be called inside the main script loop (the <see cref="Tick"/> event or any sub methods called from it).
		/// </summary>
		/// <param name="ms">The time in milliseconds to pause for.</param>
		public static void Wait(int ms)
		{
			/*
			Script script = ScriptDomain.ExecutingScript;

			if (ReferenceEquals(script, null) || !script._running)
			{
				throw new InvalidOperationException("Illegal call to 'Script.Wait()' outside main loop!");
			}

			var resume = DateTime.UtcNow + TimeSpan.FromMilliseconds(ms);

			do
			{
				script._waitEvent.Release();
				script._continueEvent.Wait();
			}
			while (DateTime.UtcNow < resume);*/
		}
		/// <summary>
		/// Yields the execution of the script for 1 frame.
		/// </summary>
		public static void Yield()
		{
			Wait(0);
		}

		/// <summary>
		/// The main execution logic of all scripts.
		/// </summary>
		internal void MainLoop()
		{
			_running = true;

			// Wait for domain to run scripts
			_continueEvent.Wait();

			// Run main loop
			while (_running)
			{
				Tuple<bool, WinForms.KeyEventArgs> keyevent = null;

				// Process events
				while (_keyboardEvents.TryDequeue(out keyevent))
				{
					try
					{
						if (keyevent.Item1)
						{
							KeyDown?.Invoke(this, keyevent.Item2);
						}
						else
						{
							KeyUp?.Invoke(this, keyevent.Item2);
						}
					}
					catch (Exception ex)
					{
						//ScriptDomain.HandleUnhandledException(this, new UnhandledExceptionEventArgs(ex, false));
						break;
					}
				}

				try
				{
					Tick?.Invoke(this, EventArgs.Empty);
				}
				catch (Exception ex)
				{
					//ScriptDomain.HandleUnhandledException(this, new UnhandledExceptionEventArgs(ex, true));

					Abort();
					break;
				}

				// Yield execution to next tick
				Wait(_interval);
			}
		}
	}
}