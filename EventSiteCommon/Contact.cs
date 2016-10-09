using System;
using System.Collections;
using pbHelpers=playboater.gallery.commons.Helpers;

namespace kcm.ch.EventSite.Common
{
	/// <summary>
	/// Summary description for Contact.
	/// </summary>
	[Serializable]
	public class Contact : IComparable
	{
		public Contact(int contactId, Mandator mandator, string name, string email, string mobilePhone, bool liftMgmtSmsOn, bool eventMgmtSmsOn, bool useTwoWaySms, int smsLog, int smsPurchased, bool isDeleted, bool noSmsCreditNotified)
		{
			ContactId = contactId;
			Mandator = mandator;
			Name = name;
			Email = email;
			MobilePhone = mobilePhone;
			LiftMgmtSmsOn = liftMgmtSmsOn;
			EventMgmtSmsOn = eventMgmtSmsOn;
			UseTwoWaySms = useTwoWaySms;
			SmsLog = smsLog;
			SmsPurchased = smsPurchased;
			IsDeleted = isDeleted;
			NoSmsCreditNotified = noSmsCreditNotified;
		}

		public Contact(Mandator mandator, string name, string email, string mobilePhone, bool liftMgmtSmsOn, bool eventMgmtSmsOn, bool useTwoWaySms, int smsLog, int smsPurchased, bool noSmsCreditNotified)
		{
			ContactId = 0;
			Mandator = mandator;
			Name = name;
			Email = email;
			MobilePhone = mobilePhone;
			LiftMgmtSmsOn = liftMgmtSmsOn;
			EventMgmtSmsOn = eventMgmtSmsOn;
			UseTwoWaySms = useTwoWaySms;
			SmsLog = smsLog;
			SmsPurchased = smsPurchased;
			IsDeleted = false;
			NoSmsCreditNotified = noSmsCreditNotified;
		}

		public Contact()
		{
		}

		public override string ToString()
		{
			return Name;
		}


//		public static bool ValidateContactData(Contact contact, out string errorMsg)
//		{
//			errorMsg = "Bei der Validierung der Eingaben ist folgender Fehler aufgetreten:";
//			if(Helpers.ValidateEmail(contact.Email))
//			{
//				if(contact.Name.Length>2)
//				{
//					if(contact.NotifyBySms)
//					{
//						if(contact.MobilePhone.Length>=10)
//						{
//							return true;
//						}
//						else
//						{
//							errorMsg += "\n- Angegebene Handy-Nummer ist ungültig!";
//							return false;
//						}
//					}
//					else
//					{
//						return true;
//					}
//				}
//				else
//				{
//					errorMsg += "\n- Eingegebener Name ist ungültig (min. 3 Zeichen)";
//					return false;
//				}
//			}
//			else
//			{
//				errorMsg += "\n- Angegebene Email-Adresse ist ungültig!";
//				return false;
//			}
//		}

		#region Properties
		public int ContactId
		{
			get { return contactId; }
			set { contactId = value; }
		}
		private int contactId;

		public Mandator Mandator
		{
			get { return mandator; }
			set { mandator = value; }
		}
		private Mandator mandator;

		public string Name
		{
			get { return name; }
			set
			{
				if(value.Trim().Length>2)
				{
					name = value.Trim();
				}
				else
				{
					throw new EventSiteException("Eingegebener Name ist ungültig (min. 3 Zeichen)", -1);
				}
			}
		}
		private string name;

		public string Email
		{
			get { return email; }
			set
			{
				if(pbHelpers.ValidateEmail(value.Trim()))
				{
					email = value.Trim();
				}
				else
				{
					throw new EventSiteException("Angegebene Email-Adresse ist ungültig!", -1);
				}
			}
		}
		private string email;

		public string MobilePhone
		{
			get { return mobilePhone; }
			set
			{
				if(value!=null && (value=value.Trim()) != String.Empty)
				{
					if((value=value
						.Replace(" ", "")
						.Replace("(", "")
						.Replace(")", "")
						.Replace("/", "")
						.Replace("\\", "")).Length>=10 && value.StartsWith("+"))
					{
						mobilePhone = value;
					}
					else
					{
						throw new EventSiteException("Angegebene Handy Nummer ist ungültig!\\n(Bitte internationales Format mit führendem '+' verwenden)", -1);
					}
				}
				else
				{
					mobilePhone = null;
				}
			}
		}
		private string mobilePhone;

		public bool LiftMgmtSmsOn
		{
			get { return liftMgmtSmsOn; }
			set
			{
				if(value)
				{
					if(MobilePhone == null)
					{
						throw new EventSiteException("Um die SMS Benachrichtigung einzuschalten, muss eine Handy-Nummer angegeben werden!", -1);
					}
				}
				liftMgmtSmsOn = value;
			}
		}
		private bool liftMgmtSmsOn;

		public bool EventMgmtSmsOn
		{
			get { return eventMgmtSmsOn; }
			set
			{
				if (value)
				{
					if (MobilePhone == null)
					{
						throw new EventSiteException("Um die SMS Benachrichtigung einzuschalten, muss eine Handy-Nummer angegeben werden!", -1);
					}
				}
				eventMgmtSmsOn = value;
			}
		}
		private bool eventMgmtSmsOn;

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

		/// <summary>
		/// Returns the contact's EventCategory settings
		/// </summary>
		/// <exception cref="PropertyNotLoadedException">If the property is not loaded</exception>
		public Hashtable ContactSettings
		{
			get
			{
				if(contactSettings == null)
				{
					throw new PropertyNotLoadedException("The property ContactSettings is not loaded yet. Please load first!", -1);
				}
				return contactSettings;
			}
			set { contactSettings = value; }
		}
		private Hashtable contactSettings = null;

		public bool IsDeleted
		{
			get { return isDeleted; }
			set { isDeleted = value; }
		}
		private bool isDeleted;

		public bool NoSmsCreditNotified
		{
			get { return noSmsCreditNotified; }
			set { noSmsCreditNotified = value; }
		}
		private bool noSmsCreditNotified = false;

		public bool UseTwoWaySms
		{
			get { return useTwoWaySms; }
			set { useTwoWaySms = value; }
		}
		private bool useTwoWaySms = false;

		#endregion

		#region IComparable Members

		public int CompareTo(object obj)
		{
			Contact c = (Contact)obj;
			return (Name.CompareTo(c.Name));
		}

		#endregion
	}
}
