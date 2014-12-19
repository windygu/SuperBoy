using System;

namespace Core.Web.CacheManage.Memcached
{
	internal class LogAdapter {
		public static LogAdapter GetLogger(Type type) {
			return new LogAdapter(type);
		}

		public static LogAdapter GetLogger(string name) {
			return new LogAdapter(name);
		}

		private string _loggerName;
		private LogAdapter(string name) { _loggerName = name; }
		private LogAdapter(Type type) { _loggerName = type.FullName; }
		public void Debug(string message) { Console.Out.WriteLine(DateTime.Now + " DEBUG " + _loggerName + " - " + message); }
		public void Info(string message) { Console.Out.WriteLine(DateTime.Now + " INFO " + _loggerName + " - " + message); }
		public void Warn(string message) { Console.Out.WriteLine(DateTime.Now + " WARN " + _loggerName + " - " + message); }
		public void Error(string message) { Console.Out.WriteLine(DateTime.Now + " ERROR " + _loggerName + " - " + message); }
		public void Fatal(string message) { Console.Out.WriteLine(DateTime.Now + " FATAL " + _loggerName + " - " + message); }
		public void Debug(string message, Exception e) { Console.Out.WriteLine(DateTime.Now + " DEBUG " + _loggerName + " - " + message + "\n" + e.Message + "\n" + e.StackTrace); }
		public void Info(string message, Exception e) { Console.Out.WriteLine(DateTime.Now + " INFO " + _loggerName + " - " + message + "\n" + e.Message + "\n" + e.StackTrace); }
		public void Warn(string message, Exception e) { Console.Out.WriteLine(DateTime.Now + " WARN " + _loggerName + " - " + message + "\n" + e.Message + "\n" + e.StackTrace); }
		public void Error(string message, Exception e) { Console.Out.WriteLine(DateTime.Now + " ERROR " + _loggerName + " - " + message + "\n" + e.Message + "\n" + e.StackTrace); }
		public void Fatal(string message, Exception e) { Console.Out.WriteLine(DateTime.Now + " FATAL " + _loggerName + " - " + message + "\n" + e.Message + "\n" + e.StackTrace); }

	
	}
}
