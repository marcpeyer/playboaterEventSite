using System.Diagnostics;
using System.Runtime.CompilerServices;
using NLog;
using NLog.Config;

namespace kcm.ch.EventSite.Common
{
	public class LoggerManager
	{
		private static LogFactory _logFactory = null;

		private static LogFactory logFactory
		{
			get
			{
				if(_logFactory == null)
				{
					TryGetLogFactory();
				}
				return _logFactory;
			}
		}

		private static bool TryGetLogFactory()
		{
			try
			{
				_logFactory = new LogFactory(GetXmlConfiguration());
				return true;
			}
			catch
			{
				return false;
			}
		}

		private static XmlLoggingConfiguration GetXmlConfiguration()
		{
			return new XmlLoggingConfiguration(EventSiteConfiguration.Current.LoggingConfiguration, null);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Logger GetLogger()
		{
			if (logFactory == null) return LogManager.CreateNullLogger();
			StackFrame frame = new StackFrame(1, false);
			return logFactory.GetLogger(frame.GetMethod().DeclaringType.FullName);
		}

	}
}