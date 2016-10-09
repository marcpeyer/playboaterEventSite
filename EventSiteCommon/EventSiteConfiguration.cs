using System;
using System.Xml;
using System.Xml.Serialization;

namespace kcm.ch.EventSite.Common
{
	[XmlRoot("EventSiteConfiguration")]
	public class EventSiteConfiguration
	{
		private const string configFileNameFormat = "playboater.EventSite{0}.config";

		[XmlElement]
		public string SqlConnectionString;
		[XmlElement]
		public bool OfflineMode;
		[XmlElement]
		public bool MaintenanceMode;
		[XmlElement]
		public string DefaultMandatorId;
		[XmlElement]
		public string AppRootUrl;

		[XmlElement("Mail")]
		public MailConfiguration MailConfiguration;

		[XmlElement("Notification")]
		public NotificationConfiguration NotificationConfiguration;

		[XmlElement("Clickatell")]
		public ClickatellConfiguration ClickatellConfiguration;

		[XmlElement("ClickatellApi")]
		public ClickatellApiConfiguration ClickatellApiConfiguration;

		[XmlElement("ErrorHandling")]
		public ErrorHandlingConfiguration ErrorHandlingConfiguration;

		[XmlElement("Logging")]
		public XmlElement LoggingConfiguration;

		[XmlIgnore]
		public static EventSiteConfiguration Current
		{
			get { return ConfigurationLoader.Load<EventSiteConfiguration>(configFileNameFormat); }
		}
	}

	public class MailConfiguration
	{
		[XmlElement]
		public string SmtpServer;
		[XmlElement]
		public bool UseSSL;
		[XmlElement]
		public int SmtpPort;
		[XmlElement]
		public string SmtpUser;
		[XmlElement]
		public string SmtpPass;

	}

	public class NotificationConfiguration
	{
		[XmlElement]
		public string NotificationAppPath;
		[XmlElement]
		public bool SendSmsOn;
		[XmlElement]
		public bool SendNotificationsOn;
		[XmlElement]
		public int SmsCreditReminderFrom;
		[XmlElement]
		public int MandantorSmsCreditReminderFrom;
		[XmlElement]
		public bool UseTwoWayMessaging;
		[XmlElement]
		public bool SendTwoWaySuccessNotificationsOn;
	}

	public class ClickatellConfiguration
	{
		[XmlElement("API_ID")]
		public int ApiId;
		[XmlElement("Username")]
		public string User;
		[XmlElement]
		public string Password;
		[XmlElement]
		public string VirtualMobileNumber;
	}

	public class ClickatellApiConfiguration
	{
		[XmlElement]
		public string SqlConnectionString;
		[XmlElement]
		public bool EnableDbLogging;
		[XmlElement]
		public string SmsTable;
		[XmlElement]
		public string StatusCol;
		[XmlElement]
		public string ApiMsgIdCol;
		[XmlElement]
		public string ChargeCol;
		[XmlElement]
		public string ClientIdCol;
		[XmlElement]
		public string ToCol;
		[XmlElement]
		public string TextCol;
	}

	public class ErrorHandlingConfiguration
	{
		[XmlElement]
		public bool SendErrorMails;
		[XmlElement]
		public string MailRecipient;
	}
}