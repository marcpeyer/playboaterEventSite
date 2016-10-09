using System;
using System.Collections;
using playboater.gallery.commons;

namespace kcm.ch.EventSite.Common
{
	/// <summary>
	/// Summary description for Contact.
	/// </summary>
	[Serializable]
	public class ContactSetting
	{
		public ContactSetting(int contactSettingId, Contact Contact, EventCategory category, bool notifyByEmail, bool notifyBySms, bool smsNotifSubscriptionsOn, NInt32 autoNotifSubscription)
		{
			this.ContactSettingId = contactSettingId;
			this.Contact = Contact;
			this.Category = category;
			this.NotifyBySms = notifyBySms;
			this.SmsNotifSubscriptionsOn = smsNotifSubscriptionsOn;
			this.NotifyByEmail = notifyByEmail;
			this.AutoNotifSubscription = autoNotifSubscription;
		}

		public ContactSetting(Contact contact, EventCategory category, bool notifyByEmail, bool notifyBySms, bool smsNotifSubscriptionsOn, NInt32 autoNotifSubscription)
		{
			//0 means no change needed, -1 means new setting --> insert into db
			this.ContactSettingId = -1;
			this.Contact = contact;
			this.Category = category;
			//use internal private field to set sms notification, for that no mobilephone validation is processed (this constructor is only called by the dataadapter object)
			this.notifyBySms = notifyBySms;
			this.SmsNotifSubscriptionsOn = smsNotifSubscriptionsOn;
			this.NotifyByEmail = notifyByEmail;
			this.AutoNotifSubscription = autoNotifSubscription;
		}

		public ContactSetting()
		{
		}

		#region Properties
		public int ContactSettingId
		{
			get { return contactSettingId; }
			set { contactSettingId = value; }
		}
		private int contactSettingId;

		public Contact Contact
		{
			get { return contact; }
			set { contact = value; }
		}
		private Contact contact;

		public EventCategory Category
		{
			get { return category; }
			set { category = value; }
		}
		private EventCategory category;

		public bool NotifyByEmail
		{
			get { return notifyByEmail; }
			set { notifyByEmail = value; }
		}
		private bool notifyByEmail;

		public bool NotifyBySms
		{
			get { return notifyBySms; }
			set { notifyBySms = value;}
		}
		private bool notifyBySms;

		public bool SmsNotifSubscriptionsOn
		{
			get { return smsNotifSubscriptionsOn; }
			set { smsNotifSubscriptionsOn = value; }
		}
		private bool smsNotifSubscriptionsOn;

		public NInt32 AutoNotifSubscription
		{
			get { return autoNotifSubscription; }
			set
			{
				if(value.IsNull || value > 0)
				{
					autoNotifSubscription = value;
				}
				else
				{
					throw new EventSiteException("Der angegebene Wert für \\'Automatische Abonnierung\\' ist\\nungültig. Erlaubt: Ganze positive Zahl oder leer.", -1);
				}
			}
		}
		private NInt32 autoNotifSubscription;
		#endregion

		#region IComparable Members

		public int CompareTo(object obj)
		{
			Contact c = (Contact)obj;
			return (this.Category.CompareTo(c.Name));
		}

		#endregion
	}
}
