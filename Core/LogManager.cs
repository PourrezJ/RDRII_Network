using System;
using System.IO;

namespace RDRN_Core
{
	internal enum LogLevel
	{
		Trace,
		Debug,
		Information,
		Warning,
		Error,
		Critical
	};

	internal static class LogManager
    {
		private static object lockObj = new object();

		static LogLevel MinLevel = LogLevel.Trace;

        internal static void WriteLog(LogLevel logLevel, string args)
		{
			lock (lockObj)
			{
				if (MinLevel > logLevel)
					return;

				var writerPath = Path.Combine(Main.RDRNetworkPath, "..//logs//RDRN_Core.log");
				var writer = new System.IO.StreamWriter(writerPath, true);

				var text = $"[{DateTime.Now.ToString("HH:mm:ss.fff")}] {logLevel}: {args}";
				writer.WriteLine(text);
				Console.WriteLine(text);
				writer.Close();
			}
		}

		internal static void Exception(Exception ex, string args = "")
		{
			lock (lockObj)
			{
				var writerPath = Path.Combine(Main.RDRNetworkPath, "..//logs//Exception.log");
				var writer = new System.IO.StreamWriter(writerPath, true);

				var text = ($"[{ DateTime.Now.ToString("HH:mm:ss.fff")}] || {args} {ex.ToString()}");
				writer.WriteLine(text);
				Console.WriteLine(text);
				writer.Close();
			}
		}

		internal static void Exception(string args)
		{
			lock(lockObj)
			{
				var writerPath = Path.Combine(Main.RDRNetworkPath, "..//logs//Exception.log");
				var writer = new System.IO.StreamWriter(writerPath, true);

				var text = ($"[{DateTime.Now.ToString("HH:mm:ss.fff")}] : {args}");
				writer.WriteLine(text);
				Console.WriteLine(text);
				writer.Close();
			}
		}
	}
}
