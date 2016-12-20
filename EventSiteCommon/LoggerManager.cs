using System.Diagnostics;
using System.Runtime.CompilerServices;
using NLog;
using NLog.Config;
using System.Xml;
using System.IO;
using System;

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
			catch(Exception ex)
			{
        Debug.Write(ex.ToString());
				return false;
			}
		}

		private static XmlLoggingConfiguration GetXmlConfiguration()
		{
			StringReader reader = new StringReader(EventSiteConfiguration.Current.LoggingConfiguration.OuterXml);
			return new XmlLoggingConfiguration(XmlReader.Create(reader), null);
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