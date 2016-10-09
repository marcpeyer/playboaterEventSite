using System;
using playboater.gallery.commons;

namespace kcm.ch.EventSite.Common
{
	/// <summary>
	/// Summary description for NotifSubscription.
	/// </summary>
	[Serializable]
	public class NotifSubscription
	{
		public NotifSubscription(int notifSubscriptionId, Event notifEvent, Contact contact, NInt32 maxNotifications)
		{
			this.NotifSubscriptionId = notifSubscriptionId;
			this.NotifEvent = notifEvent;
			this.Contact = contact;
			this.MaxNotifications = maxNotifications;
		}

		public NotifSubscription(Event notifEvent, Contact contact, NInt32 maxNotifications)
		{
			this.NotifEvent = notifEvent;
			this.Contact = contact;
			this.MaxNotifications = maxNotifications;
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
