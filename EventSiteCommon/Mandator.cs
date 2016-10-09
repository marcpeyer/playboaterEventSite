using System;
using playboater.gallery.commons;

namespace kcm.ch.EventSite.Common
{
	/// <summary>
	/// Summary description for Mandator.
	/// </summary>
	[Serializable]
	public class Mandator : INullable
	{
		public Mandator(string mandatorId, string mandatorName, string mandatorShortName, string mandatorMail, string entryPointUrl, string siteTitle, string eventName, string featureAssembly, string featureAssemblyClassName, string eventNotificationAddressesDefault, bool showEventsAsList, bool useEventCategories, bool useEventUrl, bool useMinMaxSubscriptions, bool useSubscriptions, bool smsNotifications, bool onNewEventNotifyContacts, bool onEditEventNotifyContacts, bool onNewSubscriptionNotifyContacts, bool onEditSubscriptionNotifyContacts, bool onDeleteSubscriptionNotifyContacts, bool isLiftManagementEnabled, bool notifyDeletableSubscriptionStates, string helpText, NInt32 unsubscribeAllowedFromNumSubscriptions, NInt32 unsubscribeAllowedTillNumSubscriptions, int smsLog, int smsPurchased, bool noSmsCreditNotified, bool useExternalAuth, string authTable, string authIdColumn, string authLoginColumn, string authPasswordColumn)
		{
			this.MandatorId = mandatorId;
			this.MandatorName = mandatorName;
			this.MandatorShortName = mandatorShortName;
			this.MandatorMail = mandatorMail;
			this.EntryPointUrl = entryPointUrl;
			this.SiteTitle = siteTitle;
			this.EventName = eventName;
			this.FeatureAssembly = featureAssembly;
			this.FeatureAssemblyClassName = featureAssemblyClassName;
			this.EventNotificationAddressesDefault = eventNotificationAddressesDefault;
			this.ShowEventsAsList = showEventsAsList;
			this.UseEventCategories = useEventCategories;
			this.UseEventUrl = useEventUrl;
			this.UseMinMaxSubscriptions = useMinMaxSubscriptions;
			this.UseSubscriptions = useSubscriptions;
			this.SmsNotifications = smsNotifications;
			this.OnNewEventNotifyContacts = onNewEventNotifyContacts;
			this.OnEditEventNotifyContacts = onEditEventNotifyContacts;
			this.OnNewSubscriptionNotifyContacts = onNewSubscriptionNotifyContacts;
			this.OnEditSubscriptionNotifyContacts = onEditSubscriptionNotifyContacts;
			this.OnDeleteSubscriptionNotifyContacts = onDeleteSubscriptionNotifyContacts;
			this.IsLiftManagementEnabled = isLiftManagementEnabled;
			this.NotifyDeletableSubscriptionStates = notifyDeletableSubscriptionStates;
			this.HelpText = helpText;
			this.UnsubscribeAllowedFromNumSubscriptions = unsubscribeAllowedFromNumSubscriptions;
			this.UnsubscribeAllowedTillNumSubscriptions = unsubscribeAllowedTillNumSubscriptions;
			this.SmsLog = smsLog;
			this.SmsPurchased = smsPurchased;
			this.NoSmsCreditNotified = noSmsCreditNotified;
			UseExternalAuth = useExternalAuth;
			AuthTable = authTable;
			AuthIdColumn = authIdColumn;
			AuthLoginColumn = authLoginColumn;
			AuthPasswordColumn = authPasswordColumn;

			isNull = false;
		}

		#region Properties
		public string MandatorId
		{
			get { return mandatorId; }
			set { mandatorId = value.Trim(); }
		}
		private string mandatorId;

		public string MandatorName
		{
			get { return mandatorName; }
			set { mandatorName = value.Trim(); }
		}
		private string mandatorName;

		public string MandatorShortName
		{
			get { return mandatorShortName; }
			set { mandatorShortName = value.Trim(); }
		}
		private string mandatorShortName;

		public string MandatorMail
		{
			get { return mandatorMail; }
			set { mandatorMail = value.Trim(); }
		}
		private string mandatorMail;

		public string EntryPointUrl
		{
			get { return entryPointUrl; }
			set { entryPointUrl = (value==null ? null : value.Trim()); }
		}
		private string entryPointUrl;

		public string SiteTitle
		{
			get { return siteTitle; }
			set { siteTitle = value.Trim(); }
		}
		private string siteTitle;

		public string EventName
		{
			get { return eventName; }
			set { eventName = value.Trim(); }
		}
		private string eventName;

		public string FeatureAssembly
		{
			get { return featureAssembly; }
			set { featureAssembly = (value==null ? null : value.Trim()); }
		}
		private string featureAssembly;

		public string FeatureAssemblyClassName
		{
			get { return featureAssemblyClassName; }
			set { featureAssemblyClassName = value; }
		}
		private string featureAssemblyClassName;

		public string EventNotificationAddressesDefault
		{
			get { return eventNotificationAddressesDefault; }
			set { eventNotificationAddressesDefault = (value==null ? null : value.Trim()); }
		}
		private string eventNotificationAddressesDefault;

		public bool ShowEventsAsList
		{
			get { return showEventsAsList; }
			set { showEventsAsList = value; }
		}
		private bool showEventsAsList;

		public bool UseEventCategories
		{
			get { return useEventCategories; }
			set { useEventCategories = value; }
		}
		private bool useEventCategories;

		public bool UseEventUrl
		{
			get { return useEventUrl; }
			set { useEventUrl = value; }
		}
		private bool useEventUrl;

		public bool UseMinMaxSubscriptions
		{
			get { return useMinMaxSubscriptions; }
			set { useMinMaxSubscriptions = value; }
		}
		private bool useMinMaxSubscriptions;

		public bool UseSubscriptions
		{
			get { return useSubscriptions; }
			set { useSubscriptions = value; }
		}
		private bool useSubscriptions;

		public bool SmsNotifications
		{
			get { return smsNotifications; }
			set { smsNotifications = value; }
		}
		private bool smsNotifications;

		public bool OnNewEventNotifyContacts
		{
			get { return onNewEventNotifyContacts; }
			set { onNewEventNotifyContacts = value; }
		}
		private bool onNewEventNotifyContacts;

		public bool OnEditEventNotifyContacts
		{
			get { return onEditEventNotifyContacts; }
			set { onEditEventNotifyContacts = value; }
		}
		private bool onEditEventNotifyContacts;

		public bool OnNewSubscriptionNotifyContacts
		{
			get { return onNewSubscriptionNotifyContacts; }
			set { onNewSubscriptionNotifyContacts = value; }
		}
		private bool onNewSubscriptionNotifyContacts;

		public bool OnEditSubscriptionNotifyContacts
		{
			get { return onEditSubscriptionNotifyContacts; }
			set { onEditSubscriptionNotifyContacts = value; }
		}
		private bool onEditSubscriptionNotifyContacts;

		public bool OnDeleteSubscriptionNotifyContacts
		{
			get { return onDeleteSubscriptionNotifyContacts; }
			set { onDeleteSubscriptionNotifyContacts = value; }
		}
		private bool onDeleteSubscriptionNotifyContacts;

		public bool IsLiftManagementEnabled
		{
			get { return isLiftManagementEnabled; }
			set { isLiftManagementEnabled = value; }
		}
		private bool isLiftManagementEnabled;

		public bool NotifyDeletableSubscriptionStates
		{
			get { return notifyDeletableSubscriptionStates; }
			set { notifyDeletableSubscriptionStates = value; }
		}
		private bool notifyDeletableSubscriptionStates;

		public string HelpText
		{
			get { return helpText; }
			set { helpText = (value==null ? null : value.Trim()); }
		}
		private string helpText;

		public NInt32 UnsubscribeAllowedFromNumSubscriptions
		{
			get { return unsubscribeAllowedFromNumSubscriptions; }
			set { unsubscribeAllowedFromNumSubscriptions = value; }
		}
		private NInt32 unsubscribeAllowedFromNumSubscriptions;

		public NInt32 UnsubscribeAllowedTillNumSubscriptions
		{
			get { return unsubscribeAllowedTillNumSubscriptions; }
			set { unsubscribeAllowedTillNumSubscriptions = value; }
		}
		private NInt32 unsubscribeAllowedTillNumSubscriptions;

		public int SmsLog
		{
			get { return smsLog; }
			set { smsLog = value; }
		}
		private int smsLog;

		public int SmsPurchased
		{
			get { return smsPurchased; }
			set { smsPurchased = value; }
		}
		private int smsPurchased;
	    
    public bool NoSmsCreditNotified
    {
        get { return noSmsCreditNotified; }
        set { noSmsCreditNotified = value; }
    }
    private bool noSmsCreditNotified = false;

		public bool UseExternalAuth 
		{ get; set; }

		public string AuthTable
		{ get; set; }

		public string AuthIdColumn
		{ get; set; }
	
		public string AuthLoginColumn
		{ get; set; }

		public string AuthPasswordColumn
		{ get; set; }
		#endregion

		public bool IsNull
		{
			get { return isNull; }
			set { isNull = value; }
		}
		private bool isNull = true;

		
		public void Clone(Mandator mandator)
		{
			MandatorName = mandator.MandatorName;
			MandatorShortName = mandator.MandatorShortName;
			MandatorMail = mandator.MandatorMail;
			EntryPointUrl = mandator.EntryPointUrl;
			SiteTitle = mandator.SiteTitle;
			EventName = mandator.EventName;
			FeatureAssembly = mandator.FeatureAssembly;
			FeatureAssemblyClassName = mandator.FeatureAssemblyClassName;
			EventNotificationAddressesDefault = mandator.EventNotificationAddressesDefault;
			ShowEventsAsList = mandator.ShowEventsAsList;
			UseEventCategories = mandator.UseEventCategories;
			UseEventUrl = mandator.UseEventUrl;
			UseMinMaxSubscriptions = mandator.UseMinMaxSubscriptions;
			UseSubscriptions = mandator.UseSubscriptions;
			SmsNotifications = mandator.SmsNotifications;
			OnNewEventNotifyContacts = mandator.OnNewEventNotifyContacts;
			OnEditEventNotifyContacts = mandator.OnEditEventNotifyContacts;
			OnNewSubscriptionNotifyContacts = mandator.OnNewSubscriptionNotifyContacts;
			OnEditSubscriptionNotifyContacts = mandator.OnEditSubscriptionNotifyContacts;
			OnDeleteSubscriptionNotifyContacts = mandator.OnDeleteSubscriptionNotifyContacts;
			IsLiftManagementEnabled = mandator.IsLiftManagementEnabled;
			NotifyDeletableSubscriptionStates = mandator.NotifyDeletableSubscriptionStates;
			HelpText = mandator.HelpText;
			UnsubscribeAllowedFromNumSubscriptions = mandator.UnsubscribeAllowedFromNumSubscriptions;
			UnsubscribeAllowedTillNumSubscriptions = mandator.UnsubscribeAllowedTillNumSubscriptions;
			SmsLog = mandator.SmsLog;
			SmsPurchased = mandator.SmsPurchased;
			NoSmsCreditNotified = mandator.NoSmsCreditNotified;
			UseExternalAuth = mandator.UseExternalAuth;
			AuthTable = mandator.AuthTable;
			AuthIdColumn = mandator.AuthIdColumn;
			AuthLoginColumn = mandator.AuthLoginColumn;
			AuthPasswordColumn = mandator.AuthPasswordColumn;
			isNull = false;
		}

		public override bool Equals(object obj)
		{
			return mandatorId == ((Mandator) obj).MandatorId;
		}
	}
}
