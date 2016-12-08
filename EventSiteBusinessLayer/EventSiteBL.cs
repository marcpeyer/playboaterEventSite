using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using Ajax;
using kcm.ch.EventSite.Common;
using kcm.ch.EventSite.DataAccessLayer;
using playboater.gallery.ClickatellApi;
using playboater.gallery.commons;
using Constants=kcm.ch.EventSite.Common.Constants;
using Helpers = playboater.gallery.DAL.Helpers;
using pbHelpers = playboater.gallery.commons.Helpers;

namespace kcm.ch.EventSite.BusinessLayer
{
	/// <summary>
	/// Summary description for EventSiteBL.
	/// </summary>
	public class EventSiteBL : IDisposable
	{
		#region Declarations

		private static readonly List<Mandator> mandatorInstances = new List<Mandator>();

		public readonly Mandator Mandator;
		public static double SmsCredits = -1;

		private readonly string smtpServer;
		private readonly bool smtpUseSSL;
		private readonly int smtpPort;
		private readonly string smtpUser;
		private readonly string smtpPass;

		#endregion

		public EventSiteBL(Mandator mandator)
		{
			smtpServer = EventSiteConfiguration.Current.MailConfiguration.SmtpServer;
			smtpUseSSL = EventSiteConfiguration.Current.MailConfiguration.UseSSL;
			smtpUser = EventSiteConfiguration.Current.MailConfiguration.SmtpUser;
			smtpPass = EventSiteConfiguration.Current.MailConfiguration.SmtpPass;
			smtpPort = EventSiteConfiguration.Current.MailConfiguration.SmtpPort;

			Mandator = mandator;
			mandatorInstances.Add(Mandator);
			SmsCredits = -1;
		}

		public EventSiteBL(string mandatorId)
		{
			smtpServer = EventSiteConfiguration.Current.MailConfiguration.SmtpServer;
			smtpUseSSL = EventSiteConfiguration.Current.MailConfiguration.UseSSL;
			smtpUser = EventSiteConfiguration.Current.MailConfiguration.SmtpUser;
			smtpPass = EventSiteConfiguration.Current.MailConfiguration.SmtpPass;
			smtpPort = EventSiteConfiguration.Current.MailConfiguration.SmtpPort;

			Mandator = EventSiteDA.GetMandator(mandatorId);
			mandatorInstances.Add(Mandator);
			SmsCredits = -1;
		}

		public static Mandator GetDefaultMandator()
		{
			return EventSiteDA.GetMandator(EventSiteConfiguration.Current.DefaultMandatorId);
		}
		public static List<Mandator> GetAllMandators()
		{
			return EventSiteDA.ListMandators();
		}

		public void RenewCurrentContact()
		{
			try
			{
				HttpContext.Current.Session.Remove(Constants.SESSION_KEY_AUTH_CONTACT);
			}
			catch
			{
			}
		}

		public Hashtable CurrentContactSettings
		{
			get
			{
				if (HttpContext.Current.User.Identity.IsAuthenticated)
				{
					Contact c = GetSessionContact();
					try
					{
						return c.ContactSettings;
					}
					catch (PropertyNotLoadedException)
					{
						EventSiteDA.LoadContactSettings(c);
					}
					return c.ContactSettings;
				}
				else
				{
					FormsAuthentication.SignOut();
					return null;
				}
			}
		}

		public string CurrentContactLogin
		{
			get
			{
				if(currentContactLogin == null)
				{
					currentContactLogin = EventSiteDA.GetContactLogin(CurrentContact.ContactId);
				}
				return currentContactLogin;
			}
		}
		private string currentContactLogin;

		public Contact CurrentContact
		{
			get
			{
				if (HttpContext.Current.User.Identity.IsAuthenticated)
				{
					return GetSessionContact();
				}
				else
				{
					FormsAuthentication.SignOut();
					return null;
				}
			}
		}

		private Contact GetSessionContact()
		{
			Contact c = (Contact) HttpContext.Current.Session[Constants.SESSION_KEY_AUTH_CONTACT];
			if (c == null || !HttpContext.Current.User.Identity.Name.Split('|')[0].Equals(c.Email))
			{
				c = GetContact(HttpContext.Current.User.Identity.Name.Split('|')[0]);
				HttpContext.Current.Session[Constants.SESSION_KEY_AUTH_CONTACT] = c;
			}
			return c;
		}

		#region Mandator methods

		public void EditMandatorAlertMailSettings()
		{
			EventSiteDA.EditMandatorAlertMailSettings(Mandator);
			ReloadMandators();
		}

		#endregion

		#region Security methods

		public HttpCookie Login(int contactId, string hash, ref Contact c)
		{
			string userName = String.Empty;
			LoggerManager.GetLogger().Trace("RemoteLogin for id {0}...", contactId);
			c = EventSiteDA.RemoteLogin(0, contactId, Mandator, userName);

			LoggerManager.GetLogger().Trace("RemoteLogin for {0} succeeded. checking hash...", c.Name);
			string actualHash;
			bool match = false;
			DateTime now = DateTime.Now;
			//check if hash matches in last five hours
			for (int i = 0; i > -301; i--)
			{
				actualHash = Common.Helpers.GetRemoteSubscrHash(contactId, now.AddMinutes(i), Mandator.MandatorId);
				match = actualHash.ToUpper().Equals(hash.ToUpper());
				if (match)
				{
					break;
				}
			}

			if (match)
			{
				//Login trusted
				LoggerManager.GetLogger().Trace("HashCheck succeeded - RemoteLogin trusted.");

				// Create the authentication ticket
				FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
					1, // version
					c.Email + "|" + Mandator.MandatorId, // user name
					DateTime.Now, // creation
					DateTime.Now.AddMinutes(240), //Expiration
					false, // Persistent
					GetRoles(c)); // User data

				// Now encrypt the ticket.
				string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
				// Create a cookie and add the encrypted ticket to the
				// cookie as data.
				HttpCookie authCookie =
					new HttpCookie(FormsAuthentication.FormsCookieName,
					               encryptedTicket);

				HttpContext.Current.Session[Constants.SESSION_KEY_AUTH_CONTACT] = c;

				return authCookie;
			}
			else
			{
				//Login untrusted
				LoggerManager.GetLogger().Trace("HashCheck failed! - RemoteLogin untrusted!");
				throw new EventSiteException(
					"Der Loginprozess ist fehlgeschlagen. Evtl. ist dein Login abgelaufen.\\nVersuche es nochmals indem du dich abmeldest und neu anmeldest.",
					900);
			}
		}

		public HttpCookie Login(int customUserId, string hash, string userName)
		{
			LoggerManager.GetLogger().Trace("RemoteLogin for custid {0}, userName {1}...", customUserId, userName);
			Contact c = EventSiteDA.RemoteLogin(customUserId, 0, Mandator, userName);
			LoggerManager.GetLogger().Trace("RemoteLogin for {0} succeeded. Checking hash...", c.Name);

			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			string actualHash;
			bool match = false;
			DateTime now = DateTime.Now;
			//check if hash matches in last twenty minutes
			for (int i = 0; i > -21; i--)
			{
				actualHash =
					pbHelpers.ToHexString(
						md5.ComputeHash(
							Encoding.Default.GetBytes(Mandator.EntryPointUrl + customUserId + HttpContext.Current.Request.UserHostAddress +
							                          now.AddMinutes(i).ToString("dd.MM.yyyy HH:mm"))));
				match = actualHash.ToUpper().Equals(hash.ToUpper());
				if (match)
				{
					break;
				}
			}

			if (match)
			{
				//Login trusted
				LoggerManager.GetLogger().Trace("HashCheck succeeded - RemoteLogin trusted.");

				// Create the authentication ticket
				FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
					1, // version
					c.Email + "|" + Mandator.MandatorId, // user name
					DateTime.Now, // creation
					DateTime.Now.AddMinutes(240), //Expiration
					false, // Persistent
					GetRoles(c)); // User data

				// Now encrypt the ticket.
				string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
				// Create a cookie and add the encrypted ticket to the
				// cookie as data.
				HttpCookie authCookie =
					new HttpCookie(FormsAuthentication.FormsCookieName,
					               encryptedTicket);

				HttpContext.Current.Session[Constants.SESSION_KEY_AUTH_CONTACT] = c;

				return authCookie;
			}
			else
			{
				//Login untrusted
				LoggerManager.GetLogger().Trace("HashCheck failed! - RemoteLogin untrusted!");
				throw new EventSiteException(
					"Der Loginprozess ist fehlgeschlagen. Evtl. ist dein Login abgelaufen.\\nVersuche es nochmals indem du dich abmeldest und neu anmeldest.",
					900);
			}
		}

		public HttpCookie Login(string login, string password)
		{
			try
			{
				LoggerManager.GetLogger().Trace("Login attempt for user {0}...", login);
				Contact c = EventSiteDA.Login(login, password, Mandator);

				// Create the authentication ticket
				FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
					1, // version
					c.Email + "|" + Mandator.MandatorId, // user name
					DateTime.Now, // creation
					DateTime.Now.AddMinutes(240), //Expiration
					false, // Persistent
					GetRoles(c)); // User data

				// Now encrypt the ticket.
				string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
				// Create a cookie and add the encrypted ticket to the
				// cookie as data.
				HttpCookie authCookie =
					new HttpCookie(FormsAuthentication.FormsCookieName,
					               encryptedTicket);

				HttpContext.Current.Session[Constants.SESSION_KEY_AUTH_CONTACT] = c;

				LoggerManager.GetLogger().Trace("Login succeeded");
				return authCookie;
			}
			catch (EventSiteException ex)
			{
				LoggerManager.GetLogger().Trace("Login failed: {0}", ex.ToString());
				throw;
			}
		}

		private string GetRoles(Contact contact)
		{
			// This GetRoles method returns a pipe delimited string containing
			// roles rather than returning an array, because the string format
			// is convenient for storing in the authentication ticket /
			// cookie, as user data
			//			return "Reader";
			//			return "EventCreator|User";
			ArrayList roleList = EventSiteDA.GetContactsRoles(contact);
			string[] roles = new string[roleList.Count];
			roleList.CopyTo(roles);
			return string.Join("|", roles);
		}

		public string GetUserRoles()
		{
			ArrayList userRoles = new ArrayList();
			if (HttpContext.Current.User.IsInRole("Reader"))
				userRoles.Add("Reader");
			if (HttpContext.Current.User.IsInRole("User"))
				userRoles.Add("User");
			if (HttpContext.Current.User.IsInRole("EventCreator"))
				userRoles.Add("EventCreator");
			if (HttpContext.Current.User.IsInRole("Manager"))
				userRoles.Add("Manager");
			if (HttpContext.Current.User.IsInRole("Administrator"))
				userRoles.Add("Administrator");
			string[] roles = new string[userRoles.Count];
			userRoles.CopyTo(roles);
			return string.Join(", ", roles);
		}

		public bool IsReader()
		{
			return HttpContext.Current.User.IsInRole("Reader") &&
			       !HttpContext.Current.User.IsInRole("User") &&
//				!HttpContext.Current.User.IsInRole("EventCreator") &&
			       !HttpContext.Current.User.IsInRole("Manager") &&
			       !HttpContext.Current.User.IsInRole("Administrator");
		}

		public bool IsUser()
		{
			return HttpContext.Current.User.IsInRole("User") &&
//				!HttpContext.Current.User.IsInRole("EventCreator") &&
			       !HttpContext.Current.User.IsInRole("Manager") &&
			       !HttpContext.Current.User.IsInRole("Administrator");
		}

		public bool IsEventCreator()
		{
			return HttpContext.Current.User.IsInRole("EventCreator") &&
			       !HttpContext.Current.User.IsInRole("Administrator");
		}

		public bool IsManager()
		{
			return HttpContext.Current.User.IsInRole("Manager") &&
			       !HttpContext.Current.User.IsInRole("Administrator");
		}

		public bool IsAdministrator()
		{
			return HttpContext.Current.User.IsInRole("Administrator");
		}

		public bool IsCurrentUser(int contactId)
		{
			return CurrentContact.ContactId.Equals(contactId);
		}

		#endregion

		#region Contact methods

		public Contact GetContact(string email)
		{
			return EventSiteDA.GetContact(email, Mandator);
		}

		public Contact GetContact(int contactId)
		{
			return EventSiteDA.GetContact(contactId, Mandator);
		}

		public Contact GetContact(int contactId, Mandator mandator, bool forceReload)
		{
			return EventSiteDA.GetContact(contactId, mandator, forceReload);
		}

		public Contact GetContactByMobileNumber(string mobileNumber)
		{
			if (mobileNumber != null && (mobileNumber = mobileNumber.Trim()) != String.Empty)
			{
				if ((mobileNumber = mobileNumber
				                    	.Replace(" ", "")
				                    	.Replace("(", "")
				                    	.Replace(")", "")
				                    	.Replace("/", "")
				                    	.Replace("\\", "")).Length >= 10 && mobileNumber.StartsWith("+"))
				{
					return EventSiteDA.GetContactByMobileNumber(mobileNumber, Mandator);
				}
			}
			throw new EventSiteException(
				"Angegebene Handy Nummer ist ungültig!\\n(Bitte internationales Format mit führendem '+' verwenden)", 900);
		}

		public string AddContact(Contact newContact)
		{
			string userInfo = EventSiteDA.AddContact(newContact);

			RemoveMailFromDefaultNotificationAddresses(newContact.Email);

			try
			{
				EmailMessage email = (!String.IsNullOrEmpty(smtpPass)
				                      	? new EmailMessage(smtpServer, smtpUseSSL, smtpPort, smtpUser, smtpPass)
				                      	: new EmailMessage(smtpServer));
				string body =
					string.Format(
						@"Auf der EventSite wurde ein neuer Kontakt erstellt.

Id: {3}
Name: {0}
Email: {1}
Mandant: {2}

--> TODO: Mit KCM Admin Login-Daten verknüpfen!",
						newContact.Name, newContact.Email, Mandator.SiteTitle, newContact.ContactId);
				email.SendMail(Mandator.MandatorName, Mandator.MandatorMail, "marc@kcm.ch",
				               "Neuer Kontakt auf der EventSite", body, EmailMessage.EmailMessageFormat.Text);
			}
			catch
			{
				if (userInfo == null)
				{
					userInfo = String.Empty;
				}
				userInfo += "\\nBeim Informieren des Administrators trat ein Fehler auf!\\n"
				            + "Bitte sende ihm eine Nachricht an marc@kcm.ch,\\n"
				            + "damit er den Account vervollständigen kann.";
				LoggerManager.GetLogger().Warn(userInfo);
			}

			return userInfo;
		}

		public void EditContact(Contact contact)
		{
			EventSiteDA.EditContact(contact);
			return;
		}

		public void DelContact(int contactId)
		{
			EventSiteDA.DelContact(contactId);
			return;
		}

		public void ChangePassword(Contact contact, string newLogin, string newPassword)
		{
			EventSiteDA.ChangePassword(contact.ContactId, newLogin, newPassword);
		}

		public bool SendPassword(Contact contact)
		{
			try
			{
				string password = EventSiteDA.GetPassword(contact.ContactId);

				bool smtpUseSSL = EventSiteConfiguration.Current.MailConfiguration.UseSSL;
				string smtpServer = EventSiteConfiguration.Current.MailConfiguration.SmtpServer;
				int smtpPort = EventSiteConfiguration.Current.MailConfiguration.SmtpPort;
				string smtpUser = EventSiteConfiguration.Current.MailConfiguration.SmtpUser;
				string smtpPass = EventSiteConfiguration.Current.MailConfiguration.SmtpPass;
				bool sendSmsOn = EventSiteConfiguration.Current.NotificationConfiguration.SendSmsOn;
				bool offlineMode = EventSiteConfiguration.Current.OfflineMode;

				EmailMessage mailObj = (!String.IsNullOrEmpty(smtpPass)
																? new EmailMessage(smtpServer, smtpUseSSL, smtpPort, smtpUser, smtpPass)
																: new EmailMessage(smtpServer));

				string mailSubject = "Passwort für EventSite";
				string mailBody = String.Format(@"<p>Die Login-Daten für die EventSite (Mandant '{0}') wurden angefordert.</p>
<p>Login: <b>{1}</b><br>Passwort: <b>{2}</b></p>", contact.Mandator.MandatorName, contact.Login, password);
				mailObj.SendMail(contact.Mandator.MandatorMail, contact.Email, mailSubject,
												 mailBody, EmailMessage.EmailMessageFormat.Html);
			}
			catch(Exception ex)
			{
				LoggerManager.GetLogger().ErrorException("Error while sending password email.", ex);
				return false;
			}
			return true;
		}

		public ArrayList ListContacts()
		{
			ArrayList c = EventSiteDA.ListContacts(Mandator);
			c.Sort();
			return c;
		}

		public void LoadContactSettings(Contact contact)
		{
			EventSiteDA.LoadContactSettings(contact);
		}

		public void SaveContactSettings(Contact contact)
		{
			EventSiteDA.SaveContactSettings(contact);
		}

		#endregion

		#region SmsNotifSubscription methods

		public void SaveSmsNotifSubscription(SmsNotifSubscription smsNotifSubscription)
		{
			if (smsNotifSubscription.NotifSubscriptionId == 0)
			{
				//add new
				EventSiteDA.AddSmsNotifSubscription(smsNotifSubscription);
			}
			else if (!smsNotifSubscription.MaxNotifications.IsNull && smsNotifSubscription.MaxNotifications == -1)
			{
				//delete existing
				EventSiteDA.DeleteSmsNotifSubscription(smsNotifSubscription);
			}
			else
			{
				//update existing
				EventSiteDA.EditSmsNotifSubscription(smsNotifSubscription);
			}
		}

		public static void SetSmsNotifSubscriptions(Event eventToSet)
		{
			if (eventToSet != null)
			{
				EventSiteDA.SetSmsNotifSubscriptions(eventToSet);
			}
		}

		#endregion

		#region SubscriptionState methods

		public DataSet ListSubscriptionStates(EventCategory category)
		{
			return EventSiteDA.ListSubscriptionStates(category);
		}

		/// <summary>
		/// Returns the category's default SubscriptionState represented as a DictionaryEntry where 
		/// the key is the SubscriptionStateId and the value is the SubscriptionStateText.
		/// </summary>
		public DictionaryEntry GetDefaultSubscriptionState(EventCategory eventCategory)
		{
			return GetSubscriptionStateByCode(1, eventCategory);
		}

		/// <summary>
		/// Returns a SubscriptionState represented as a DictionaryEntry where the key is the
		/// SubscriptionStateId and the value is the SubscriptionStateText.
		/// </summary>
		public DictionaryEntry GetSubscriptionStateByCode(int code, EventCategory eventCategory)
		{
			DataSet subscriptionStates = EventSiteDA.ListSubscriptionStates(eventCategory);
			foreach (DataRow row in subscriptionStates.Tables[0].Rows)
			{
				if (row["SubscriptionStateCode"] != DBNull.Value && Convert.ToInt32((Byte) row["SubscriptionStateCode"]) == code)
				{
					return new DictionaryEntry(row["SubscriptionStateId"], row["StateText"]);
				}
			}

			throw new EventSiteException("Invalid code parameter supplied!", 900);
		}

		#endregion

		#region Location methods

		public Location GetLocation(int locationId, EventCategory category)
		{
			return EventSiteDA.GetLocation(locationId, category);
		}

		public string AddLocation(Location location)
		{
			return EventSiteDA.AddLocation(location);
		}

		public void EditLocation(Location location)
		{
			EventSiteDA.EditLocation(location);
			return;
		}

		public void DelLocation(int locationId, EventCategory category)
		{
			EventSiteDA.DelLocation(locationId, category);
			return;
		}

		public ArrayList ListLocations(EventCategory category)
		{
			ArrayList locations = EventSiteDA.ListLocations(category);
			locations.Sort();
			return locations;
		}

		#endregion

		#region EventCategory methods

		public ArrayList ListEventCategories()
		{
			ArrayList categories = EventSiteDA.ListEventCategories(Mandator);
			categories.Sort();
			return categories;
		}

		public EventCategory GetEventCategory(int eventCategoryId)
		{
			return EventSiteDA.GetEventCategory(eventCategoryId, Mandator);
		}

		#endregion

		#region Event methods

		public static Event GetEventById(int eventId)
		{
			return EventSiteDA.GetEvent(eventId);
		}

		public Event GetEvent(int eventId)
		{
			return EventSiteDA.GetEvent(eventId, Mandator);
		}

		public void SaveEvent(Event ev)
		{
			SaveEvent(ev, true);
		}

		public void SaveEvent(Event ev, bool notifyContacts)
		{
			if (ev.EventId == -1)
			{
				AddEvent(ev);
			}
			else
			{
				EditEvent(ev, notifyContacts);
			}
		}

		private void AddEvent(Event newEvent)
		{
			EventSiteDA.AddEvent(newEvent);

			//add auto subscriptions
			ArrayList contacts = ListContacts();
			foreach (Contact c in contacts)
			{
				EventSiteDA.LoadContactSettings(c);
				ContactSetting setting = (ContactSetting) c.ContactSettings[newEvent.EventCategory.EventCategoryId];
				if (!setting.AutoNotifSubscription.IsNull)
				{
					SmsNotifSubscription subscr = new SmsNotifSubscription(newEvent, c, setting.AutoNotifSubscription);
					SaveSmsNotifSubscription(subscr);
				}
			}

			if (Mandator.OnNewEventNotifyContacts)
			{
				StartNotificationProcess(NotificationOperation.AddEventNotification, newEvent.EventId.ToString());
			}
			return;
		}

		private void EditEvent(Event editEvent, bool notifyContacts)
		{
			EventSiteDA.EditEvent(editEvent);
			if (notifyContacts && Mandator.OnEditEventNotifyContacts)
			{
				StartNotificationProcess(NotificationOperation.EditEventNotification, editEvent.EventId.ToString());
			}
			return;
		}

		public void DelEvent(int eventId)
		{
			EventSiteDA.DelEvent(eventId);
			return;
		}

		public List<Event> ListFutureEvents(IComparer<Event> sorting)
		{
			List<Event> e = EventSiteDA.ListEvents(Mandator, true);
			e.Sort(sorting);
			return e;
		}

		public List<Event> ListFutureEvents(EventCategory category, IComparer<Event> sorting)
		{
			List<Event> e = EventSiteDA.ListEvents(category, true);
			e.Sort(sorting);
			return e;
		}

		public List<Event> ListEvents(IComparer<Event> sorting)
		{
			List<Event> e = EventSiteDA.ListEvents(Mandator);
			e.Sort(sorting);
			return e;
		}

		public List<Event> ListEvents(EventCategory category, IComparer<Event> sorting)
		{
			List<Event> e = EventSiteDA.ListEvents(category);
			e.Sort(sorting);
			return e;
		}

		#endregion

		#region Subscription methods

		#region subscription

		public List<Subscription> AddSubscription(Subscription subscription)
		{
			subscription = EventSiteDA.AddSubscription(subscription);

			List<Subscription> subscriptions = ListSubscriptions(subscription.Event);

			if (Mandator.OnNewSubscriptionNotifyContacts)
			{
				StartNotificationProcess(NotificationOperation.AddSubscriptionNotification, subscription.SubscriptionId.ToString());
			}
			return subscriptions;
		}

//		public void SaveJourneyStations(Subscription subscription)
//		{
//			EventSiteDA.SaveJourneyStations(subscription);
//		}

		public List<Subscription> EditSubscription(Subscription subscription, bool doNotify)
		{
			EventSiteDA.EditSubscription(subscription);
			EventSiteDA.SaveJourneyStations(subscription);

			List<Subscription> subscriptions = ListSubscriptions(subscription.Event);

			if (Mandator.OnEditSubscriptionNotifyContacts && doNotify)
			{
				StartNotificationProcess(NotificationOperation.EditSubscriptionNotification, subscription.SubscriptionId.ToString());
			}
			return subscriptions;
		}

		public Subscription GetSubscription(int subscriptionId)
		{
			//EventSiteDA da = new EventSiteDA();
			return EventSiteDA.GetSubscription(subscriptionId, Mandator);
		}

		#endregion

		public bool HasSubscriptionLiftsSet(Subscription subscription)
		{
			int numLifts = EventSiteDA.GetNumDefinedLifts(subscription);

			return numLifts > 0;
		}

		public bool HasJourneyStationLiftsSet(JourneyStation journeyStation)
		{
			if (journeyStation.JourneyStationId < 1)
			{
				return false;
			}
			else
			{
				int numLifts = EventSiteDA.GetNumDefinedLifts(journeyStation);

				return numLifts > 0;
			}
		}

		#region journey

		public static void SetJourneyStations(Subscription subscription)
		{
			EventSiteDA.SetJourneyStations(subscription);
		}

		public static List<Subscription> ListSubscriptions(Event selEvent)
		{
			return EventSiteDA.ListSubscriptions(selEvent);
		}

		/// <summary>
		/// Gets all subscriptions that have free lifts and the liftsubscription of subscr
		/// </summary>
		public List<Subscription> ListSubscriptionsWithLifts(Subscription subscr)
		{
			List<Subscription> subscriptions = ListSubscriptions(subscr.Event);
			foreach (Subscription subscription in subscriptions)
			{
				SetJourneyStations(subscription);
			}
			List<Subscription> subscriptionsWithLifts = new List<Subscription>();

			Subscription liftSubscription = null;
			if (!subscr.LiftSubscriptionJourneyStationId.IsNull)
			{
				liftSubscription =
					Subscription.GetSubscriptionByJourneyStationId(subscriptions, subscr.LiftSubscriptionJourneyStationId);
			}

			foreach (Subscription subscription in subscriptions)
			{
				if (subscription.SubscriptionLiftState == LiftState.drives && subscr.SubscriptionId != subscription.SubscriptionId
				    || liftSubscription != null && subscription.SubscriptionId == liftSubscription.SubscriptionId)
				{
					subscriptionsWithLifts.Add(subscription);
				}
			}
			return subscriptionsWithLifts;
		}

		public Hashtable GetPastJourneys(Contact contact)
		{
			Hashtable pastJourneys = new Hashtable();

			using (SqlDataReader contactsJourneys = EventSiteDA.ListContactsJourneys(contact))
			{
				int currentSubscriptionId = 0;
				ArrayList journeyStations = new ArrayList();
				string journeyString = string.Empty;
				while (contactsJourneys.Read())
				{
					if (currentSubscriptionId != Helpers.GetInt32(contactsJourneys, "SubscriptionId"))
					{
						if (currentSubscriptionId > 0)
						{
							journeyString = journeyString.Substring(0, journeyString.Length - 3);
							if (!pastJourneys.ContainsKey(journeyString))
							{
								JourneyStation[] stations = new JourneyStation[journeyStations.Count];
								journeyStations.CopyTo(stations);
								pastJourneys.Add(journeyString, stations);
							}
						}

						journeyStations = new ArrayList();
						journeyString = string.Empty;
						currentSubscriptionId = Helpers.GetInt32(contactsJourneys, "SubscriptionId");
					}

					JourneyStation js = new JourneyStation(
						Helpers.GetString(contactsJourneys, "Station"),
						Helpers.GetString(contactsJourneys, "StationTime"),
						Helpers.GetInt32(contactsJourneys, "SortOrder"));

					journeyStations.Add(js);
					journeyString += Helpers.GetString(contactsJourneys, "Station") + " - ";
					//subscription.journeyStations.Add(js);
				}

				if (currentSubscriptionId > 0)
				{
					journeyString = journeyString.Substring(0, journeyString.Length - 3);
					if (!pastJourneys.ContainsKey(journeyString))
					{
						JourneyStation[] stations = new JourneyStation[journeyStations.Count];
						journeyStations.CopyTo(stations);
						pastJourneys.Add(journeyString, stations);
					}
				}

				contactsJourneys.Close();
			}

			return pastJourneys;
		}

		#endregion

		#endregion

		#region Notification methods

		/// <summary>
		/// Gets the base arguments for notification.
		/// </summary>
		private string GetNotificationBaseEventArgs()
		{
			return GetArgumentString(Mandator.MandatorId);
		}
		
		/// <summary>
		/// Returns the given parameters joined and space separated.
		/// </summary>
		public string GetArgumentString(params object[] args)
		{
			string[] arguments = new string[args.Length];
			
			for(int i = 0; i < args.Length; i++)
			{
				string arg = args[i].ToString();
				if(arg.IndexOf(' ') != -1)
				{
					arg = String.Format("\"{0}\"", arg);
				}
				arguments[i] = arg;
			}
			return String.Join(" ", arguments);
		}

		/// <summary>
		/// Calls the external notification application with the given parameters.
		/// </summary>
		private void StartNotificationProcess(NotificationOperation operation, string specialArgs)
		{
			LoggerManager.GetLogger().Trace("StartNotificationProcess() begin");
			
			string baseArgs = GetNotificationBaseEventArgs();

			string notificationAppPath = Environment.ExpandEnvironmentVariables(EventSiteConfiguration.Current.NotificationConfiguration.NotificationAppPath);
			LoggerManager.GetLogger().Trace("Starting notification process from this location: {0}", notificationAppPath);
			
			string applicationArguments = String.Format("{0} {1} {2}", operation, baseArgs, specialArgs);
			LoggerManager.GetLogger().Trace("calling notificatin app with this params: {0}", applicationArguments);

			// Create a new process object
			// dispose the object so that the webrequest doesn't have a reference and
			// doesn't throw ThreadAbortException therefore.
			using (Process ProcessObj = new Process())
			{
				// StartInfo contains the startup information of the new process
				ProcessObj.StartInfo.FileName = notificationAppPath;
				ProcessObj.StartInfo.Arguments = applicationArguments;

				// These two optional flags ensure that no DOS window appears
				ProcessObj.StartInfo.UseShellExecute = false;
				ProcessObj.StartInfo.CreateNoWindow = true;

				ProcessObj.Start();
			}

			LoggerManager.GetLogger().Trace("StartNotificationProcess() end");
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Method to send a contact a text message
		/// </summary>
		public void SendSms(Contact sender, Contact recipient, string smsBody)
		{
			if (EventSiteConfiguration.Current.NotificationConfiguration.SendSmsOn)
			{
				WsManager wsManager = null;
				try
				{
					if (!EventSiteConfiguration.Current.OfflineMode)
					{
						wsManager = new WsManager(
							EventSiteConfiguration.Current.ClickatellConfiguration.ApiId,
							EventSiteConfiguration.Current.ClickatellConfiguration.User,
							EventSiteConfiguration.Current.ClickatellConfiguration.Password,
							EventSiteConfiguration.Current.ClickatellApiConfiguration.EnableDbLogging,
							EventSiteConfiguration.Current.ClickatellApiConfiguration.SqlConnectionString,
							EventSiteConfiguration.Current.ClickatellApiConfiguration.SmsTable,
							EventSiteConfiguration.Current.ClickatellApiConfiguration.StatusCol,
							EventSiteConfiguration.Current.ClickatellApiConfiguration.ApiMsgIdCol,
							EventSiteConfiguration.Current.ClickatellApiConfiguration.ChargeCol,
							EventSiteConfiguration.Current.ClickatellApiConfiguration.ClientIdCol,
							EventSiteConfiguration.Current.ClickatellApiConfiguration.ToCol,
							EventSiteConfiguration.Current.ClickatellApiConfiguration.TextCol);
					}

					if (sender.SmsPurchased > sender.SmsLog)
					{
						if (!EventSiteConfiguration.Current.OfflineMode)
						{
							wsManager.SendMessage(
								smsBody,
								new string[] {recipient.MobilePhone},
								sender.MobilePhone ?? Mandator.MandatorShortName,
								MessageType.SMS_TEXT);
						}
						else
						{
							Simulation.SimulateSms(smsBody, sender.MobilePhone ?? Mandator.MandatorShortName, recipient.MobilePhone);
						}
						EventSiteDA.LogSms(sender, typeof (Contact));

						if (HttpContext.Current != null && HttpContext.Current.Session != null)
						{
							//reload contact to get current sms credit
							RenewCurrentContact();

							CheckUserSmsCredit(CurrentContact);
						}
					}
					else
					{
						throw new EventSiteException("Dein SMS Kredit ist aufgebraucht!", 900);
					}
				}
				catch(EventSiteException)
				{
					throw;
				}
				catch (Exception ex)
				{
					LoggerManager.GetLogger().ErrorException("Beim Senden einer SMS ist ein Fehler aufgetreten.", ex);
					throw new EventSiteException(ex.Message, 999);
				}
				finally
				{
					if(wsManager != null)
					{
						wsManager.Dispose();
					}
				}
			}
		}

		/// <summary>
		/// Removes a mail address from the default notification addresses list on the mandator
		/// </summary>
		/// <param name="mailAddr">the mail address to remove</param>
		/// <returns>true if found and removed, otherwise false</returns>
		public bool RemoveMailFromDefaultNotificationAddresses(string mailAddr)
		{
			if (Mandator.EventNotificationAddressesDefault != null)
			{
				ArrayList defaultMailAddrs = new ArrayList();
				bool foundInDefaultAddrs = false;
				mailAddr = mailAddr.ToLower();
				foreach (string defaultMailAddr in Mandator.EventNotificationAddressesDefault.Split(';'))
				{
					if (defaultMailAddr.Trim().ToLower() != mailAddr)
					{
						defaultMailAddrs.Add(defaultMailAddr);
					}
					else
					{
						foundInDefaultAddrs = true;
					}
				}
				if (foundInDefaultAddrs)
				{
					if (defaultMailAddrs.Count >= 1)
					{
						string[] mailAddrs = new string[defaultMailAddrs.Count];
						defaultMailAddrs.CopyTo(mailAddrs);
						Mandator.EventNotificationAddressesDefault = string.Join(";", mailAddrs);
					}
					else
					{
						Mandator.EventNotificationAddressesDefault = null;
					}
					EventSiteDA.EditMandatorDefaultNotificationAddresses(Mandator);
					return true;
				}
			}
			return false;
		}

		public void CheckMandatorSmsCredit()
		{
			int credit = Mandator.SmsPurchased - Mandator.SmsLog;

			const int creditModulo = 10;
			if (MultipleStepsAvoidance.DoGlobalSmsCreditNotif() && credit > 0 && credit <= EventSiteConfiguration.Current.NotificationConfiguration.MandantorSmsCreditReminderFrom && credit % creditModulo == 0)
			{
				try
				{
					string body =
						String.Format(
							@"Hallo {0}
Der SMS Kredit des Mandanten ""{1}"" ist bald aufgebraucht. Es können noch ca. {2} SMS Nachrichten verschickt werden. Bitte mit dem Webmaster in Verbindung setzen, um für den Mandanten neuen Kredit zu erhalten.

Automatisch generierte Nachricht.",
							Mandator.MandatorName,
							Mandator.MandatorShortName,
							Mandator.SmsPurchased - Mandator.SmsLog);
					if (!EventSiteConfiguration.Current.OfflineMode)
					{
						EmailMessage email = (!String.IsNullOrEmpty(smtpPass)
						                      	? new EmailMessage(smtpServer, smtpUseSSL, smtpPort, smtpUser, smtpPass)
						                      	: new EmailMessage(smtpServer));
						email.SendMail(Mandator.MandatorName, Mandator.MandatorMail, Mandator.MandatorMail,
						               "SMS Kredit des Mandanten fast aufgebraucht", body, EmailMessage.EmailMessageFormat.Text);
					}
					else
					{
						Simulation.SimulateEmail(Mandator.MandatorMail, Mandator.MandatorMail,
						                         "SMS Kredit des Mandanten fast aufgebraucht", body, EmailMessage.EmailMessageFormat.Text);
					}

					MultipleStepsAvoidance.GlobalSmsCreditNotified();
				}
				catch (Exception ex)
				{
					LoggerManager.GetLogger().ErrorException("Error while checking Mandator sms credit.",
					                                                                     ex);
				}
			}
		}

		public void CheckUserSmsCredit(Contact contact)
		{
			int credit = contact.SmsPurchased - contact.SmsLog;

			const int creditModulo = 5;
			if (MultipleStepsAvoidance.DoUserSmsCreditNotif(contact.ContactId) && credit > 0 && credit <= EventSiteConfiguration.Current.NotificationConfiguration.SmsCreditReminderFrom && credit % creditModulo == 0)
			{
				try
				{
					string body =
						String.Format(
							@"Hallo {0}
Dein Kredit zum Empfangen von SMS Nachrichten auf ""{1}"" ist bald aufgebraucht. Du kannst noch ca. {2} SMS Nachrichten erhalten. Bitte setze dich mit dem Webmaster in Verbindung um neuen Kredit zu erhalten.

Automatisch generierte Nachricht.",
							contact.Name,
							contact.Mandator.MandatorShortName,
							contact.SmsPurchased - contact.SmsLog);
					if (!EventSiteConfiguration.Current.OfflineMode)
					{
						EmailMessage email = (!String.IsNullOrEmpty(smtpPass)
						                      	? new EmailMessage(smtpServer, smtpUseSSL, smtpPort, smtpUser, smtpPass)
						                      	: new EmailMessage(smtpServer));
						email.SendMail(contact.Mandator.MandatorName, contact.Mandator.MandatorMail, contact.Email,
						               "SMS Kredit fast aufgebraucht", body, EmailMessage.EmailMessageFormat.Text);
					}
					else
					{
						Simulation.SimulateEmail(contact.Mandator.MandatorMail, contact.Email, "SMS Kredit fast aufgebraucht", body,
						                         EmailMessage.EmailMessageFormat.Text);
					}
					
					MultipleStepsAvoidance.UserSmsCreditNotified(contact.ContactId);
				}
				catch (Exception ex)
				{
					LoggerManager.GetLogger().ErrorException("Error while checking user sms credit.", ex);
				}
			}
		}

		public static ContactSetting GetContactSetting(Contact contact, EventCategory category)
		{
			try
			{
				return (ContactSetting) contact.ContactSettings[category.EventCategoryId];
			}
			catch (PropertyNotLoadedException)
			{
				EventSiteDA.LoadContactSettings(contact);
				return (ContactSetting) contact.ContactSettings[category.EventCategoryId];
			}
		}

		public static void ReloadMandators()
		{
			for (int i = 0; i < mandatorInstances.Count; i++)
			{
				Mandator mandator = mandatorInstances[i];
				mandatorInstances[i].Clone(EventSiteDA.GetMandator(mandator.MandatorId));
			}
		}

		#endregion

		#region Ajax Methods

		[AjaxMethod(HttpSessionStateRequirement.Read)]
		public static bool UpdateNotifSubscription(string mandatorId, int eventId, bool notifSubscriptionOn,
		                                           string maxNotifications)
		{
			using (EventSiteBL bll = new EventSiteBL(mandatorId))
			{
				Event currentEvent = bll.GetEvent(eventId);
				SetSmsNotifSubscriptions(currentEvent);
				SmsNotifSubscription mainSmsNotifSubscription = null;
				foreach (SmsNotifSubscription notifSubscription in currentEvent.SmsNotifSubscriptions)
				{
					if (notifSubscription.Contact.ContactId.Equals(bll.CurrentContact.ContactId))
					{
						mainSmsNotifSubscription = notifSubscription;
						break;
					}
				}
				if (notifSubscriptionOn)
				{
					NInt32 maxNotifics;
					maxNotifics = GetMaxNotifications(maxNotifications);

					if (mainSmsNotifSubscription == null)
					{
						mainSmsNotifSubscription = new SmsNotifSubscription(currentEvent, bll.CurrentContact, maxNotifics);
					}
					else
					{
						mainSmsNotifSubscription.MaxNotifications = maxNotifics;
					}
				}
				else
				{
					if (mainSmsNotifSubscription != null)
					{
						//MaxNotifications to -1 means delete
						mainSmsNotifSubscription.MaxNotifications = -1;
					}
				}

				if (mainSmsNotifSubscription != null)
				{
					bll.SaveSmsNotifSubscription(mainSmsNotifSubscription);
				}
			}
			return true;
		}

		private static NInt32 GetMaxNotifications(string input)
		{
			NInt32 maxNotif = new NInt32(0, true);
			try
			{
				input = input.Trim();
				if (input != String.Empty)
				{
					maxNotif = Convert.ToInt32(input);
					if (maxNotif < 0)
					{
						throw new EventSiteException("\"Anzahl SMS Benachrichtigungen\" darf nicht kleiner als 0 sein.", 900);
					}
				}
			}
			catch (EventSiteException ex)
			{
				throw ex;
			}
			catch
			{
				throw new EventSiteException("Fehler bei der Konvertierung von \"Anzahl SMS Benachrichtigungen\" in eine Zahl.", 900);
			}
			return maxNotif;
		}
		[AjaxMethod()]
		public static bool ApplySubscriptionChange(string mandatorId, int subscriptionId, int subscriptionStateId, string comment, string time)
		{
			using (EventSiteBL bll = new EventSiteBL(mandatorId))
			{
				LoggerManager.GetLogger().Trace("ApplySubscriptionChange({0}, {1}, {2}, {3}, {4})", mandatorId, subscriptionId, subscriptionStateId, comment, time);
				Subscription s = bll.GetSubscription(subscriptionId);
				s.SubscriptionStateId = subscriptionStateId;
				s.Comment = comment;
				s.SubscriptionTime = (time == String.Empty ? null : time);

				try
				{
					bll.EditSubscription(s, true);
				}
				catch(Exception ex)
				{
					LoggerManager.GetLogger().ErrorException("Error while applying subscription change.", ex);
					return false;
				}

				return true;
			}
		}

		[AjaxMethod()]
		public static string GetSubscriptionChangeHtml(string mandatorId, int subscriptionId, int leftPos, int topPos)
		{
			using(EventSiteBL bll = new EventSiteBL(mandatorId))
			{
				try
				{
					Subscription s = bll.GetSubscription(subscriptionId);

					StringBuilder sb = new StringBuilder();
					sb.AppendLine("<div style='width:400px;padding: 5px;'>");
					sb.AppendLine("<h3>Eintragung &Auml;ndern</h3>");
					sb.AppendLine("<p style='color: black;'>");
					sb.AppendLine("Status:<br>");
					sb.AppendLine("<table border='0' style='font-size:X-Small;'>");

					DataTable subscrStates = bll.ListSubscriptionStates(s.Event.EventCategory).Tables[0];
					string[] subscrStateIds = new string[subscrStates.Rows.Count];

					string disabledAttr = s.IsUnsubscribable ? String.Empty : " disabled";
					for(int i = 0; i < subscrStateIds.Length; i++)
					{
						DataRow row = subscrStates.Rows[i];
						string inpId = String.Format("subscriptionStates_{0}_{1}", s.SubscriptionId, row["SubscriptionStateId"]);
						sb.AppendFormat(
							"<tr><td><input type='radio' id='{5}' name='subscriptionStates_{0}' value='{1}'{3}{4} /><label for='{5}'>{2}</label></td></tr>",
							subscriptionId,
							row["SubscriptionStateId"],
							row["StateText"],
							s.SubscriptionStateId == (int)row["SubscriptionStateId"] ? " checked='checked'" : String.Empty,
							disabledAttr,
							inpId);
						subscrStateIds[i] = inpId;
					}
					sb.AppendLine("</table>");

					string subscrTimeFieldId = String.Format("subscriptionTime_{0}", s.SubscriptionId);
					string subscrCommentFieldId = String.Format("subscriptionComment_{0}", s.SubscriptionId);

					sb.AppendLine("<br>Zeit (falls anders als im Anlass):<br>");
					sb.AppendFormat("<input type='text' id='{0}' value='{1}' maxlength='20'{2} {3}/><br>\r\n", subscrTimeFieldId, s.SubscriptionTime ?? String.Empty, disabledAttr, (!s.IsUnsubscribable ? "style=\"background-color:#D4D0C8;\"" : String.Empty));
					sb.AppendLine("<br>Kommentar:<br>");
					sb.AppendFormat("<textarea rows='4' style='width: 100%;' id='{0}'>{1}</textarea><br>\r\n", subscrCommentFieldId, s.Comment.Replace("<br>", "\r\n"));
					sb.AppendFormat("<button style='margin-top:10px;' onclick=\"fSaveSubscriptionChange('{0}', {1}, new Array('{2}'), '{3}', '{4}'); return false;\">Speichern</button>\r\n",
						bll.Mandator.MandatorId,
						s.SubscriptionId,
						String.Join("','", subscrStateIds),
						subscrTimeFieldId,
						subscrCommentFieldId);
					sb.AppendLine("</p>");
					sb.AppendLine("</div>");

					return String.Format(
							"<div style='background-color:#efffff; border: 1px solid black; padding: 2px; line-height: 18px; position:absolute; left:{0}px; top:{1}px;' onclick='event.cancelBubble=true;'>{2}</div>",
							leftPos,
							topPos,
							sb.ToString());
				}
				catch (Exception ex)
				{
					LoggerManager.GetLogger().ErrorException("Error while generating subscription change code.", ex);
					string debugInfo = String.Empty;
#if(DEBUG)
					debugInfo = "<br><br>Exception:<br>" + ex.ToString();
#endif
					return String.Format(
							"<div style=\"background-color:#efffff; border: 1px solid black; padding: 2px; line-height: 18px; position:absolute; left:{0}px; top:{1}px;\"><span style='color: red; font-weight:bold'>Fehler beim generieren des Htmls{2}</span></div>",
							leftPos,
							topPos,
							debugInfo);
				}
			}
		}

		[AjaxMethod()]
		public static string GetLiftMgtChoiceHtml(string mandatorId, int subscriptionId, int leftPos, int topPos)
		{
			string defineJourneyLink =
				string.Format(
					"<a class=\"choiceMenuLink\" href=\"JavaScript:showEventSiteModalWindow('Journey.aspx?mid={0}&sid={1}', 'EventSite_DefineJourney', 600, 800, 'refreshEventButton');\">Route definieren</a>",
					mandatorId,
					subscriptionId);

			string takeLiftLink =
				string.Format(
					"<a class=\"choiceMenuLink\" href=\"JavaScript:showEventSiteModalWindow('Lift.aspx?mid={0}&sid={1}', 'EventSite_TakeLift', 600, 800, 'refreshEventButton');\">Mitfahrt definieren</a>",
					mandatorId,
					subscriptionId);

			return
				string.Format(
					"<div style=\"background-color:#efffff; border: 1px solid black; padding: 2px; line-height: 18px; position:absolute; left:{0}px; top:{1}px;\">{2}<br>{3}</div>",
					leftPos,
					topPos,
					defineJourneyLink,
					takeLiftLink);
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if (!Mandator.IsNull && mandatorInstances.Count > 0)
			{
				mandatorInstances.Remove(Mandator);
			}
			Mandator.IsNull = true;
		}

		#endregion

		public void NotifyLiftSave(string action, string definition, Event evnt, Contact contactToNotify, Contact liftContact)
		{
			StartNotificationProcess(NotificationOperation.LiftSaveNotification, GetArgumentString(action, definition, evnt.EventId, contactToNotify.ContactId, liftContact.ContactId));
		}

		public void NofityJourneyChange(Subscription journeySubscription)
		{
			StartNotificationProcess(NotificationOperation.JourneyChangeNotification, journeySubscription.SubscriptionId.ToString());
		}

		public void LogSms(SmsNotifSubscription smsNotifSubscription, Type typeToBill)
		{
			EventSiteDA.LogSms(smsNotifSubscription, typeToBill);
		}

		public void LogSms(Contact contact, Type typeToBill)
		{
			EventSiteDA.LogSms(contact, typeToBill);
		}
	}

	public enum NotificationOperation
	{
		AddEventNotification,
		EditEventNotification,
		AddSubscriptionNotification,
		EditSubscriptionNotification,
		DelSubscriptionNotification,
		JourneyChangeNotification,
		LiftSaveNotification
	}
}