using System;
using playboater.gallery.commons;

namespace kcm.ch.EventSite.Common
{
	/// <summary>
	/// Summary description for NotifSubscription.
	/// </summary>
	[Serializable]
	public class SmsNotifSubscription
	{
		public SmsNotifSubscription(int notifSubscriptionId, Event notifEvent, Contact contact, NInt32 maxNotifications)
		{
			this.NotifSubscriptionId = notifSubscriptionId;
			this.NotifEvent = notifEvent;
			this.Contact = contact;
			this.MaxNotifications = maxNotifications;
		}

		public SmsNotifSubscription(Event notifEvent, Contact contact, NInt32 maxNotifications)
		{
			this.NotifEvent = notifEvent;
			this.Contact = contact;
			this.MaxNotifications = maxNotifications;
		}

		public SmsNotifSubscription(Contact contact)
		{
			this.Contact = contact;
			this.MaxNotifications = new NInt32(0, false);
		}

		public int NotifSubscriptionId
		{
			get { return notifSubscriptionId; }
			set { notifSubscriptionId = value; }
		}
		private int notifSubscriptionId;

		public Event NotifEvent
		{
			get { return notifEvent; }
			set { notifEvent = value; }
		}
		private Event notifEvent;

		public Contact Contact
		{
			get { return contact; }
			set { contact = value; }
		}
		private Contact contact;

		public NInt32 MaxNotifications
		{
			get { return maxNotifications; }
			set { maxNotifications = value; }
		}
		private NInt32 maxNotifications;
	}
}
