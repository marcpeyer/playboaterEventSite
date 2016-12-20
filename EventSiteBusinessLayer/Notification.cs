using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using kcm.ch.EventSite.BusinessLayer;
using kcm.ch.EventSite.Common;
using playboater.gallery.ClickatellApi;
using playboater.gallery.commons;

namespace kcm.ch.EventSite.Notifications
{
	/// <summary>
	/// Summary description for Notification.
	/// </summary>
	public class Notification
	{
		#region declarations

		private static readonly object notificationLock = new object();
		private static int instCount = 0;

		private const string eventAddNotificationMessageFormat = "Unter {0} wurde ein neuer Anlass erstellt.";
		private const string eventEditNotificationMessageFormat = "Unter {0} wurde ein bestehender Anlass ge&auml;ndert.";
		private const string eventAddNotificationSmsMessageFormat = "Neuer Anlass unter {0}: ";
		private const string eventEditNotificationSmsMessageFormat = "Geänderter Anlass unter {0}: ";

		private const string subscriptionAddNotificationMessageFormat =
			"{0} hat sich mit dem Status {1} für den Anlass {2} am {3} um {4} in {5} eingetragen.\nKommentar: {6}";

		private const string subscriptionEditNotificationMessageFormat =
			"{0} hat sein/ihr Eintrag für den Anlass {2} am {3} um {4} in {5} geändert. Status: {1}.\nKommentar: {6}";

		private const string subscriptionDelNotificationMessageFormat =
			"{0} hat sein/ihr Eintrag für den Anlass {2} am {3} um {4} in {5} gelöscht. Status war: {1}";

		private const string subscriptionAddNotificationSmsMessageFormat =
			"{0} hat sich mit dem Status {1} für den Anlass {2} am {3} um {4} in {5} eingetragen. Kommentar: {6}";

		private const string subscriptionEditNotificationSmsMessageFormat =
			"{0} hat den Eintrag für den Anlass {2} am {3} um {4} in {5} geändert. Status: {1}. Kommentar: {6}";

		private const string subscriptionDelNotificationSmsMessageFormat =
			"{0} hat den Eintrag für den Anlass {2} am {3} um {4} in {5} gelöscht. Status war: {1}";

		private const string journeyChangeMessageFormat =
			"{0} hat die Route, bei der du mitfährst geändert. Neu: Route:\r\n{1}\r\nAbfahrtszeit: {2}";

		private const string liftSaveMessageFormat = "Auf deinem Eintrag für den Anlass \"{0}\" am \"{1}\" wurde {2} {3}";

		private const string subscriptionNotificationTemplate =
			@"
	<style type=""text/css"">
	p, tr
	{{
		FONT-FAMILY: Verdana, Arial, Helvetica;
		FONT-SIZE: 10px;
		FONT-WEIGHT: normal;
		COLOR: #000000;
	}}
	th
	{{
		FONT-WEIGHT: bold;
	}}
	</style>
	<p>{0}</p>
	<p>Momentan sind folgende Eintr&auml;ge f&uuml;r diesen Anlass vorhanden:</p>
	<table border=""1"" cellspacing=""0"">
	{1}
	</table>
	<p>See: <a href=""{2}"">{2}</a> for details!</p>";

		private const string eventNotificationTemplate =
			@"
	<style type=""text/css"">
	p, tr
	{{
		FONT-FAMILY: Verdana, Arial, Helvetica;
		FONT-SIZE: 10px;
		FONT-WEIGHT: normal;
		COLOR: #000000;
	}}
	th
	{{
		FONT-WEIGHT: bold;
	}}
	.field
	{{
		FONT-WEIGHT: bold;
	}}
	</style>
	<p>{0}</p>
	<p>Anlass-Daten:</p>
	<table border=""1"" cellspacing=""0"">
		<tr><td class=""field"">Kategorie</td><td>{14}</td></tr>
		<tr><td class=""field"">Anlass</td><td>{1}</td></tr>
		<tr><td class=""field"">Location</td><td title=""{11}"">{9}</td></tr>
		<tr><td class=""field"">Datum</td><td>{2}</td></tr>
		<tr><td class=""field"">Zeit</td><td>{3}</td></tr>
		<tr><td class=""field"">Dauer</td><td>{4}</td></tr>
		<tr><td class=""field"">Min&nbsp;Teilnehmer</td><td>{5}</td></tr>
		<tr><td class=""field"">Max&nbsp;Teilnehmer</td><td>{6}</td></tr>
		<tr><td class=""field"">Beschreibung</td><td>{7}</td></tr>
		<tr><td class=""field"">Web-Adresse</td><td><a href=""{10}"">{10}</a></td></tr>
		<tr><td class=""field"">Anlass-Ersteller</td><td><a href=""{13}"">{12}</a></td></tr>
	</table>
	<p>See: <a href=""{8}"">{8}</a> for details!</p>";

		private readonly string mandatorId;
		private Event eventToNotify;
		private Subscription subscriptionToNotify;
		private List<Subscription> existingSubscriptions;
		private Contact contactToNotify;
		private Contact liftContact;

		private EventSiteBL eventSiteBL
		{
			get { return esBL ?? (esBL = new EventSiteBL(mandatorId)); }
		}

		private EventSiteBL esBL = null;

		private Mandator mandator
		{
			get { return eventSiteBL.Mandator; }
		}

		private readonly bool smtpUseSSL;
		private readonly string smtpServer;
		private readonly int smtpPort;
		private readonly string smtpUser;
		private readonly string smtpPass;
		private readonly bool sendSmsOn;
		private readonly bool offlineMode;

		#endregion

		#region constructor

		public Notification(string mandatorId)
		{
			lock (notificationLock)
			{
				instCount++;
				LoggerManager.GetLogger().Trace("---> Num instances of Notification: {0}", instCount);
			}

			this.mandatorId = mandatorId;

			smtpUseSSL = EventSiteConfiguration.Current.MailConfiguration.UseSSL;
			smtpServer = EventSiteConfiguration.Current.MailConfiguration.SmtpServer;
			smtpPort = EventSiteConfiguration.Current.MailConfiguration.SmtpPort;
			smtpUser = EventSiteConfiguration.Current.MailConfiguration.SmtpUser;
			smtpPass = EventSiteConfiguration.Current.MailConfiguration.SmtpPass;
			sendSmsOn = EventSiteConfiguration.Current.NotificationConfiguration.SendSmsOn;
			offlineMode = EventSiteConfiguration.Current.OfflineMode;

			LoggerManager.GetLogger().Trace("constructor of 'Notification' finishing");
		}

		~Notification()
		{
			lock (notificationLock)
			{
				instCount--;
			}
		}

		#endregion

		#region notification start methods

		public void BeginAddEventNotification(int eventIdToNotify)
		{
			SetEventNotificationInfo(eventIdToNotify);

			DoEventNotification(String.Format(eventAddNotificationMessageFormat,
			                                  eventToNotify.EventCategory.Category), eventAddNotificationSmsMessageFormat);
		}

		public void BeginEditEventNotification(int eventIdToNotify)
		{
			SetEventNotificationInfo(eventIdToNotify);

			DoEventNotification(String.Format(eventEditNotificationMessageFormat,
			                                  eventToNotify.EventCategory.Category), eventEditNotificationSmsMessageFormat);
		}

		public void BeginAddSubscriptionNotification(int subscriptionIdToNotify)
		{
			SetSubscriptionNotificationInfo(subscriptionIdToNotify);

			DoSubscriptionNotification(subscriptionAddNotificationMessageFormat, subscriptionAddNotificationSmsMessageFormat);
		}

		public void BeginEditSubscriptionNotification(int subscriptionIdToNotify)
		{
			SetSubscriptionNotificationInfo(subscriptionIdToNotify);

			DoSubscriptionNotification(subscriptionEditNotificationMessageFormat, subscriptionEditNotificationSmsMessageFormat);
		}

		public void BeginDelSubscriptionNotification(int subscriptionIdToNotify)
		{
			throw new NotImplementedException(@"This method is not implemented anymore.
You must not delete subscriptions.
Thange the subscriptionstate instead.");
//			SetSubscriptionNotificationInfo(subscriptionIdToNotify);
//
//			DoSubscriptionNotification(subscriptionDelNotificationMessageFormat, subscriptionDelNotificationSmsMessageFormat);
		}

		public void BeginJourneyChangeNotification(int journeySubscriptionId)
		{
			SetJourneyNotificationInfo(journeySubscriptionId);

			DoJourneyChangeNotification();
		}

		public void BeginLiftSaveNotification(string action, string definition, int eventId, int contactIdToNotify, int liftContactId)
		{
			SetLiftNotificationInfo(eventId, contactIdToNotify, liftContactId);

			DoLiftSaveNotification(String.Format(liftSaveMessageFormat,
			                                     eventToNotify.EventTitle,
			                                     eventToNotify.StartDate.ToString("dd.MM.yyyy"),
			                                     action,
			                                     definition));
		}

		#endregion

		#region set info methods

		private void SetEventNotificationInfo(int eventIdToNotify)
		{
			eventToNotify = eventSiteBL.GetEvent(eventIdToNotify);
		}

		private void SetSubscriptionNotificationInfo(int subscriptionIdToNotify)
		{
			subscriptionToNotify = eventSiteBL.GetSubscription(subscriptionIdToNotify);
			existingSubscriptions = EventSiteBL.ListSubscriptions(subscriptionToNotify.Event);
			eventToNotify = subscriptionToNotify.Event;
		}

		private void SetLiftNotificationInfo(int eventId, int contactIdToNotify, int liftContactId)
		{
			eventToNotify = eventSiteBL.GetEvent(eventId);
			contactToNotify = eventSiteBL.GetContact(contactIdToNotify);
			liftContact = eventSiteBL.GetContact(liftContactId);
		}

		private void SetJourneyNotificationInfo(int journeySubscriptionId)
		{
			subscriptionToNotify = eventSiteBL.GetSubscription(journeySubscriptionId);
			EventSiteBL.SetJourneyStations(subscriptionToNotify);
			eventToNotify = subscriptionToNotify.Event;
		}

		#endregion

		#region event notification method

		private void DoEventNotification(string eventNote, string smsBodyFormat)
		{
			try
			{
				LoggerManager.GetLogger().Trace("DoEventNotification() started.");
				string body = String.Format(eventNotificationTemplate,
				                            eventNote,
				                            HttpUtility.HtmlEncode(eventToNotify.EventTitle),
				                            eventToNotify.StartDate.ToString("dd.MM.yyyy"),
				                            eventToNotify.StartDate.ToString("HH:mm"),
				                            eventToNotify.Duration,
				                            eventToNotify.MinSubscriptions.IsNull
				                            	? " - "
				                            	: eventToNotify.MinSubscriptions.ToString(),
				                            eventToNotify.MaxSubscriptions.IsNull
				                            	? " - "
				                            	: eventToNotify.MaxSubscriptions.ToString(),
				                            HttpUtility.HtmlEncode(eventToNotify.EventDescription ?? String.Empty).Replace("\r\n", "\n").Replace(
				                            	"\n", "<br/>"),
				                            mandator.EntryPointUrl,
				                            HttpUtility.HtmlEncode(eventToNotify.Location.LocationText),
				                            eventToNotify.EventUrl,
				                            eventToNotify.Location.LocationDescription,
				                            eventToNotify.EventCreator.Name,
				                            "mailto:" + eventToNotify.EventCreator.Email,
				                            eventToNotify.EventCategory.Category);

				string smsBody = String.Format(smsBodyFormat
				                               + "{1}[{6}] am {2} um {3} in {4}. Beschreibung: {5}",
				                               eventToNotify.EventCategory.Category,
				                               eventToNotify.EventTitle,
				                               eventToNotify.StartDate.ToString("dd.MM.yyyy"),
				                               eventToNotify.StartDate.ToString("HH:mm"),
				                               eventToNotify.Location.LocationShort,
				                               eventToNotify.EventDescription,
				                               eventToNotify.EventId);
				//					newEvent.EventDescription.Replace("<br>", "\r\n"));
				if (smsBody.Length > 160)
					smsBody = smsBody.Substring(0, 160);

				string subject = String.Format("{0} - {1} in der Kategorie '{2}': {3}",
				                               mandator.MandatorName,
				                               mandator.EventName,
				                               eventToNotify.EventCategory.Category,
				                               eventToNotify.EventTitle);
				Notify(subject, body, smsBody, mandator.EventNotificationAddressesDefault);
				LoggerManager.GetLogger().Trace("DoEventNotification() completed.");
			}
			catch (Exception ex)
			{
				LoggerManager.GetLogger().ErrorException("Error occured in DoEventNotification()", ex);
				Common.Helpers.TrySendErrorMail(ex);
			}
		}

		#endregion

		#region subscription notification method

		private void DoSubscriptionNotification(string subscriptionNotificationMessageFormat,
		                                        string subscriptionNotificationSmsMessageFormat)
		{
			try
			{
				LoggerManager.GetLogger().Trace("DoSubscriptionNotification() started.");
				string subscriptionList = String.Empty;
				subscriptionList += GetSubscriptionList4Notification(existingSubscriptions);
				string subscriptionNote = String.Format(subscriptionNotificationMessageFormat,
				                                        subscriptionToNotify.Contact.Name,
				                                        subscriptionToNotify.SubscriptionStateText,
				                                        eventToNotify.EventTitle,
				                                        eventToNotify.StartDate.ToString("dd.MM.yyyy"),
				                                        (subscriptionToNotify.SubscriptionTime != null &&
				                                         subscriptionToNotify.SubscriptionTime != String.Empty
				                                         	? subscriptionToNotify.SubscriptionTime
				                                         	: eventToNotify.StartDate.ToString("HH:mm")),
				                                        eventToNotify.Location.LocationText,
				                                        subscriptionToNotify.Comment);

				string body = String.Format(subscriptionNotificationTemplate,
				                            HttpUtility.HtmlEncode(subscriptionNote).Replace("\r\n", "\n").Replace("\n", "<br/>"),
				                            subscriptionList,
				                            mandator.EntryPointUrl);

				string smsBody = String.Format(subscriptionNotificationSmsMessageFormat,
				                               subscriptionToNotify.Contact.Name,
				                               subscriptionToNotify.SubscriptionStateText,
				                               eventToNotify.EventTitle,
				                               eventToNotify.StartDate.ToString("dd.MM.yyyy"),
				                               (subscriptionToNotify.SubscriptionTime != null &&
				                                subscriptionToNotify.SubscriptionTime != String.Empty
				                                	? subscriptionToNotify.SubscriptionTime
				                                	: eventToNotify.StartDate.ToString("HH:mm")),
				                               eventToNotify.Location.LocationShort,
				                               subscriptionToNotify.Comment);
				if (smsBody.Length > 160)
					smsBody = smsBody.Substring(0, 160);

				if (mandator.NotifyDeletableSubscriptionStates || !subscriptionToNotify.SubscriptionStateIsDeletable)
				{
					string subject = String.Format("{0} - {1} in der Kategorie '{2}': {3}",
					                               mandator.MandatorName,
					                               mandator.EventName,
					                               eventToNotify.EventCategory.Category,
					                               eventToNotify.EventTitle);
					Notify(subject, body, smsBody, subscriptionToNotify.Contact);
				}
				LoggerManager.GetLogger().Trace("DoSubscriptionNotification() completed.");
			}
			catch (Exception ex)
			{
				LoggerManager.GetLogger().ErrorException("Error occured in DoSubscriptionNotification()", ex);
				Common.Helpers.TrySendErrorMail(ex);
			}
		}

		#endregion

		#region event/subscription notification helper methods

		/// <summary>
		/// Notification method to notify all contacts of a given mandator without additional recipients
		/// and except the contact specified with the c parameter.
		/// </summary>
		private void Notify(string subject, string emailBody, string smsBody, Contact c)
		{
			Notify(subject, emailBody, smsBody, c, null, false);
		}

		/// <summary>
		/// Notification method to notify all contacts of a given mandator with additional recipients
		/// without an any exception. This is an event notification which is sent whether notif subscription is set or not.
		/// </summary>
		private void Notify(string subject, string emailBody, string smsBody, string aditionalRecepients)
		{
			Notify(subject, emailBody, smsBody, new Contact(), aditionalRecepients, true);
		}

		/// <summary>
		/// General notification method to notify all contacts on this mandator
		/// except the contact specified with the c parameter.
		/// </summary>
		/// <param name="subject">the subject for the notification</param>
		/// <param name="emailBody">the notification's mail body</param>
		/// <param name="smsBody">the notification's sms body</param>
		/// <param name="c">the contact which issued this notification operation</param>
		/// <param name="aditionalRecipients">a comma separated list of email addresses for aditional notification</param>
		/// <param name="isEventNotification">specifies if this is an event notification and all contacts should get a notification, or if it is another notification and notif subscriptions should be considered.</param>
		private void Notify(string subject, string emailBody, string smsBody, Contact c, string aditionalRecipients,
		                    bool isEventNotification)
		{
			LoggerManager.GetLogger().Trace("Notify() called for subject '{0}' - isEventNotification '{1}'", subject, isEventNotification);
			if (EventSiteConfiguration.Current.NotificationConfiguration.SendNotificationsOn)
			{
				//set correct type to bill. if it is not an event notivication then always Contact is billed
				//if it is an event notivication set it according the category setting.
				Type typeToBill;
				if (isEventNotification)
				{
					typeToBill = eventToNotify.EventCategory.FreeEventSmsNotifications ? typeof (Mandator) : typeof (Contact);
				}
				else
				{
					typeToBill = typeof (Contact);
				}
				string sentMailAddrs = String.Empty;
				EventSiteException instancingExc = null;
				EventSiteException sendingExc = null;
				EmailMessage email = (!String.IsNullOrEmpty(smtpPass)
				                      	? new EmailMessage(smtpServer, smtpUseSSL, smtpPort, smtpUser, smtpPass)
				                      	: new EmailMessage(smtpServer));
				WsManager wsManager = null;
				try
				{
					if (sendSmsOn && !offlineMode)
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
				}
				catch (Exception ex)
				{
					instancingExc = new EventSiteException(ex.Message, 999);
				}

				ArrayList contacts = eventSiteBL.ListContacts();
				if (isEventNotification)
				{
					foreach (Contact contact in contacts)
					{
						try
						{
							if (!c.ContactId.Equals(contact.ContactId))
							{
								string directSubscriptionLink =
									String.Format(
										"<p>Mit einem Klick f&uuml;r diesen Anlass anmelden: <a href=\"{0}\">anmelden</a> (aus sicherheitstechnischen Gr&uuml;nden nur 5 Stunden g&uuml;ltig!)</p>",
										GetDirectSubscriptionLink(contact));
								NotifyContact(c, contact, EventSiteBL.GetContactSetting(contact, eventToNotify.EventCategory), subject, emailBody + directSubscriptionLink, null, typeToBill, wsManager, smsBody, isEventNotification, email, instancingExc, ref sendingExc, ref sentMailAddrs);

								LoggerManager.GetLogger().Trace("sent for {0}", contact.Name);
							}
						}
						catch (Exception ex)
						{
							LoggerManager.GetLogger().ErrorException(String.Format("Error occured in Notify() while notifying contact '{0}' (ID:{1})", contact.Email, contact.ContactId), ex);
							Common.Helpers.TrySendErrorMail(ex);
						}
					}
				}
				else
				{
					EventSiteBL.SetSmsNotifSubscriptions(eventToNotify);
					foreach (Contact contact in contacts)
					{
						try
						{
							if (!c.ContactId.Equals(contact.ContactId))
							{
								ContactSetting contactSet = EventSiteBL.GetContactSetting(contact, eventToNotify.EventCategory);
								if (contactSet.SmsNotifSubscriptionsOn)
								{
									bool found = false;
									foreach (SmsNotifSubscription notifSubscription in eventToNotify.SmsNotifSubscriptions)
									{
										if (contact.ContactId.Equals(notifSubscription.Contact.ContactId))
										{
											NotifyContact(c, contact, contactSet, subject, emailBody, notifSubscription, typeToBill, wsManager, smsBody, isEventNotification, email,
											              instancingExc, ref sendingExc, ref sentMailAddrs);
											found = true;
											break;
										}
									}
									//BUGFIX: if no notif subscription found call notifycontact to send the email!
									if (!found)
									{
										NotifyContact(c, contact, contactSet, subject, emailBody, email, ref sendingExc, ref sentMailAddrs);
									}
								}
								else
								{
									NotifyContact(c, contact, contactSet, subject, emailBody, null, typeToBill, wsManager, smsBody, isEventNotification, email, instancingExc,
									              ref sendingExc, ref sentMailAddrs);
								}
							}
						}
						catch (Exception ex)
						{
							LoggerManager.GetLogger().ErrorException(String.Format("Error occured in Notify() while notifying contact '{0}' (ID:{1})", contact.Email, contact.ContactId), ex);
							Common.Helpers.TrySendErrorMail(ex);
						}
					}
				}

				if (aditionalRecipients != null)
				{
					string aditionalMailAddrs = String.Empty;
					foreach (string aditionalRecipient in aditionalRecipients.Split(';'))
					{
						if (sentMailAddrs.IndexOf(aditionalRecipient) == -1)
						{
							aditionalMailAddrs += aditionalRecipient + ";";
						}
					}
					emailBody +=
						string.Format("<p>Wenn du keine solchen Benachrichtigungs Mails mehr bekommen m&ouml;chtest, kannst " +
						              "du dich unter <a href=\"{0}UnsubscribeMailingList.aspx?mid={1}\">diesem Link</a> austragen.</p>",
						              GetApplicationRootUrl(),
						              mandator.MandatorId);

					if (aditionalMailAddrs != String.Empty)
					{
						if (!offlineMode)
						{
							email.SendMail(c.Email == null ? mandator.MandatorName : c.Name,
							               c.Email ?? mandator.MandatorMail, aditionalMailAddrs, subject, emailBody,
							               EmailMessage.EmailMessageFormat.Html);
						}
						else
						{
							Simulation.SimulateEmail(c.Email ?? mandator.MandatorMail, aditionalMailAddrs, subject,
							                         emailBody, EmailMessage.EmailMessageFormat.Html);
						}
					}
				}

				if (sendSmsOn && instancingExc == null && !offlineMode)
				{
					try
					{
						EventSiteBL.SmsCredits = wsManager.GetCredit();
					}
					catch (Exception ex)
					{
						throw new EventSiteException(ex.Message, 999);
					}
				}

				if (wsManager != null)
				{
					wsManager.Dispose();
				}

				if (instancingExc != null)
				{
					throw instancingExc;
				}
				if (sendingExc != null)
				{
					throw sendingExc;
				}
			}
		}

		/// <summary>
		/// Internal method to notify a contact -> Sends no sms! Sends only Email.
		/// </summary>
		private void NotifyContact(Contact sender, Contact recipient, ContactSetting recipientContactSet, string subject,
		                           string emailBody, EmailMessage mailObj, ref EventSiteException sendingExc, ref string sentMailAddrs)
		{
			NotifyContact(sender, recipient, recipientContactSet, subject, emailBody, new SmsNotifSubscription(recipient), null,
			              null, null, false, mailObj, null, ref sendingExc, ref sentMailAddrs);
		}

		/// <summary>
		/// Internal method to notify a contact -> Sends sms and email only to the recipient <see cref="Contact">Contact</see>.
		/// </summary>
		/// <param name="typeToBill">The Type to bill the sms messag onto. Can be Contact or Mandator</param>
		private void NotifyContact(Contact sender, Contact recipient, ContactSetting recipientContactSet, string mailSubject,
		                           string mailBody, SmsNotifSubscription smsNotifSubscription, Type typeToBill, WsManager wsManager,
		                           string smsBody, bool isEventNotification, EmailMessage mailObj, EventSiteException instancingExc,
		                           ref EventSiteException sendingExc, ref string sentMailAddrs)
		{
			try
			{
				LoggerManager.GetLogger().Trace("NotifyContact() called - Recipient '{0}' - Subject '{1}'", recipient.Email, mailSubject);

				if (recipientContactSet.NotifyByEmail)
				{
					try
					{
						if (!offlineMode)
						{
							mailObj.SendMail(sender.Email == null ? mandator.MandatorName : sender.Name,
							                 sender.Email ?? mandator.MandatorMail, recipient.Email, mailSubject,
							                 mailBody, EmailMessage.EmailMessageFormat.Html);
						}
						else
						{
							Simulation.SimulateEmail(sender.Email ?? mandator.MandatorMail, recipient.Email,
							                         mailSubject, mailBody, EmailMessage.EmailMessageFormat.Html);
						}

						sentMailAddrs += recipient.Email + ";";
					}
					catch (Exception ex)
					{
						LoggerManager.GetLogger().ErrorException("Error while sending email.", ex);
						Common.Helpers.TrySendErrorMail(ex);
					}
				}

				if (sendSmsOn && recipient.EventMgmtSmsOn && recipientContactSet.NotifyBySms && recipient.MobilePhone != null
				    && instancingExc == null && sendingExc == null
				    && (typeToBill == typeof (Contact)
				        && recipient.SmsPurchased > recipient.SmsLog
				        || typeToBill == typeof (Mandator)
				           && mandator.SmsPurchased > mandator.SmsLog)
				    && (smsNotifSubscription == null || (smsNotifSubscription.MaxNotifications.IsNull
				                                         || smsNotifSubscription.MaxNotifications > 0)))
				{
					try
					{
						if (!offlineMode)
						{
							if (EventSiteConfiguration.Current.NotificationConfiguration.UseTwoWayMessaging
								&& isEventNotification
								&& recipient.UseTwoWaySms)
							{
								wsManager.SendTwoWayMessage(
									smsBody, new string[] {recipient.MobilePhone},
									EventSiteConfiguration.Current.ClickatellConfiguration.VirtualMobileNumber, MessageType.SMS_TEXT);
							}
							else
							{
								wsManager.SendMessage(
									smsBody, new string[] {recipient.MobilePhone},
									eventSiteBL.Mandator.MandatorShortName, MessageType.SMS_TEXT);
							}
						}
						else
						{
                            if (EventSiteConfiguration.Current.NotificationConfiguration.UseTwoWayMessaging
                                && isEventNotification
                                && recipient.UseTwoWaySms)
                            {
                                Simulation.SimulateSms("TWO-WAY\r\n" + smsBody, EventSiteConfiguration.Current.ClickatellConfiguration.VirtualMobileNumber, recipient.MobilePhone);
                            }
                            else
                            {
                                Simulation.SimulateSms(smsBody, eventSiteBL.Mandator.MandatorShortName, recipient.MobilePhone);
                            }
						}
						if (smsNotifSubscription != null)
						{
							eventSiteBL.LogSms(smsNotifSubscription, typeToBill);
							EventSiteBL.ReloadMandators();
						}
						else
						{
							eventSiteBL.LogSms(recipient, typeToBill);
							EventSiteBL.ReloadMandators();
						}

						//reload contact to get current sms credit
						recipient = eventSiteBL.GetContact(recipient.ContactId, recipient.Mandator, true);

						eventSiteBL.CheckUserSmsCredit(recipient);
						eventSiteBL.CheckMandatorSmsCredit();
					}
					catch (PlayboaterException pex)
					{
						//if not no credit left
						if (pex.ErrorNumber != 301) //113
						{
							sendingExc = new EventSiteException(pex.Message, pex.ErrorNumber);
							LoggerManager.GetLogger().ErrorException("Error while sending sms.", pex);
							Common.Helpers.TrySendErrorMail(pex);
						}
					}
					catch (Exception ex)
					{
						LoggerManager.GetLogger().ErrorException("Error while sending sms.", ex);
						Common.Helpers.TrySendErrorMail(ex);
					}
				}
				else if (sendSmsOn && recipient.EventMgmtSmsOn && recipientContactSet.NotifyBySms && recipient.MobilePhone != null
				         && instancingExc == null && sendingExc == null
				         && (typeToBill == typeof (Contact)
				             && recipient.SmsPurchased <= recipient.SmsLog)
				         && (smsNotifSubscription == null || (smsNotifSubscription.MaxNotifications.IsNull
				                                              || smsNotifSubscription.MaxNotifications > 0)))
				{
					SendNoSmsCreditLeftMail(recipient);
				}
				else if (sendSmsOn && recipient.EventMgmtSmsOn && recipientContactSet.NotifyBySms && recipient.MobilePhone != null
				         && instancingExc == null && sendingExc == null
				         && (typeToBill == typeof (Mandator)
				             && mandator.SmsPurchased <= mandator.SmsLog)
				         && (smsNotifSubscription == null || (smsNotifSubscription.MaxNotifications.IsNull
				                                              || smsNotifSubscription.MaxNotifications > 0)))
				{
					SendNoSmsCreditLeftMail();
				}
			}
			catch (Exception ex)
			{
				LoggerManager.GetLogger().ErrorException("Error occured in NotifyContact()", ex);
				Common.Helpers.TrySendErrorMail(ex);
			}
		}

		#endregion

		#region LiftManagement notification methods

		/// <summary>
		/// Notifies a journey change to all users, which enroled for a lift.
		/// </summary>
		private void DoJourneyChangeNotification()
		{
			try
			{
				string notificationText = string.Format(journeyChangeMessageFormat,
				                                        subscriptionToNotify.Contact.Name,
				                                        string.Join(" - ", subscriptionToNotify.GetJourneyStationArr()),
				                                        subscriptionToNotify.GetJourneyStartTime());
				List<Subscription> subscriptions = EventSiteBL.ListSubscriptions(subscriptionToNotify.Event);
				foreach (Subscription subscription in subscriptions)
				{
					EventSiteBL.SetJourneyStations(subscription);
				}

				foreach (Subscription subscription in subscriptions)
				{
					if (!subscription.LiftSubscriptionJourneyStationId.IsNull)
					{
						Subscription su =
							Subscription.GetSubscriptionByJourneyStationId(subscriptions, subscription.LiftSubscriptionJourneyStationId);
						if (su != null && su.SubscriptionId == subscriptionToNotify.SubscriptionId)
						{
							//notify
							NotifyLiftMgt(subscription.Contact, subscription.Contact, notificationText, subscriptionToNotify.Contact.Name,
							              subscriptionToNotify.Contact.Email);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Common.Helpers.TrySendErrorMail(ex);
			}
		}

		/// <summary>
		/// Notifies the driver about a lift enrolement
		/// </summary>
		private void DoLiftSaveNotification(string notificationText)
		{
			NotifyLiftMgt(contactToNotify, liftContact, notificationText, liftContact.Name, liftContact.Email);
		}

		/// <summary>
		/// General lift management notification method.
		/// </summary>
		private void NotifyLiftMgt(Contact contactToNotify, Contact contactToLog, string notificationText, string senderName,
		                           string senderEmail)
		{
			if (EventSiteConfiguration.Current.NotificationConfiguration.SendNotificationsOn)
			{
				EventSiteException smsInstancingExc = null;
				EventSiteException smsSendingExc = null;
				EventSiteException emailSendingExc = null;
				EmailMessage email = (!String.IsNullOrEmpty(smtpPass)
				                      	? new EmailMessage(smtpServer, smtpUseSSL, smtpPort, smtpUser, smtpPass)
				                      	: new EmailMessage(smtpServer));
				WsManager wsManager = null;
				try
				{
					if (sendSmsOn && !offlineMode)
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
				}
				catch (Exception ex)
				{
					smsInstancingExc = new EventSiteException(ex.Message, 999);
				}

				try
				{
					if (!offlineMode)
					{
						email.SendMail(senderName, senderEmail, contactToNotify.Email, mandator.SiteTitle + " - Mitfahrt",
						               notificationText, EmailMessage.EmailMessageFormat.Text);
					}
					else
					{
						Simulation.SimulateEmail(senderEmail, contactToNotify.Email, mandator.SiteTitle + " - Mitfahrt", notificationText,
						                         EmailMessage.EmailMessageFormat.Text);
					}
				}
				catch (Exception ex)
				{
					emailSendingExc =
						new EventSiteException(
							"Fehler beim Senden des Benachrichtigungs Emails!\\n\\n" +
							playboater.gallery.commons.Helpers.javascriptEncode(ex.ToString()), 200);
				}

				if (sendSmsOn && contactToNotify.LiftMgmtSmsOn && smsInstancingExc == null
				    && (contactToLog.SmsPurchased > contactToLog.SmsLog || contactToNotify.SmsPurchased > contactToNotify.SmsLog))
				{
					try
					{
						if (!offlineMode)
						{
							wsManager.SendMessage((notificationText.Length > 306 ? notificationText.Substring(0, 306) : notificationText),
								new string[] { contactToNotify.MobilePhone }, eventSiteBL.Mandator.MandatorShortName, MessageType.SMS_TEXT);
						}
						else
						{
							Simulation.SimulateSms((notificationText.Length > 306 ? notificationText.Substring(0, 306) : notificationText),
																		 eventSiteBL.Mandator.MandatorShortName, contactToNotify.MobilePhone);
						}

						Contact contactToReallyLog = contactToLog;
						if (contactToLog.SmsPurchased <= contactToLog.SmsLog)
						{
							contactToReallyLog = contactToNotify;
						}
						eventSiteBL.LogSms(contactToReallyLog, typeof (Contact));
						//if double length sms, log again
						if (notificationText.Length > 160)
						{
							eventSiteBL.LogSms(contactToReallyLog, typeof (Contact));
						}

						//reload contact to get current sms credit
						contactToLog = eventSiteBL.GetContact(contactToLog.ContactId, contactToLog.Mandator, true);

						eventSiteBL.CheckUserSmsCredit(contactToLog);
					}
					catch (Exception ex)
					{
						//TODO: check to get this info out of WsManager
						//if not no credit left
//						if (pex.ErrorNumber != 301) //113
//							smsSendingExc = new EventSiteException(pex.Message, pex.ErrorNumber);
							smsSendingExc = new EventSiteException(ex.Message, 999);
					}
				}

				if (sendSmsOn && contactToNotify.LiftMgmtSmsOn && contactToLog.SmsPurchased <= contactToLog.SmsLog)
				{
					SendNoSmsCreditLeftMail(contactToLog);
				}

				if (wsManager != null)
				{
					wsManager.Dispose();
				}

				if (emailSendingExc != null)
				{
					throw emailSendingExc;
				}
				if (smsInstancingExc != null)
				{
					throw smsInstancingExc;
				}
				if (smsSendingExc != null)
				{
					throw smsSendingExc;
				}
			}
		}

		#endregion

		#region notification helper methods

		private static string GetApplicationRootUrl()
		{
			string suffix = EventSiteConfiguration.Current.AppRootUrl.EndsWith("/") ? String.Empty : "/";
			return EventSiteConfiguration.Current.AppRootUrl + suffix;
		}

		private string GetDirectSubscriptionLink(Contact contact)
		{
			string link = null;
			try
			{
				link = String.Format("{0}RemoteLogin.aspx?mid={1}&hash={2}&eid={3}&cid={4}&rs={5}",
				                     GetApplicationRootUrl(),
				                     mandatorId,
				                     Common.Helpers.GetRemoteSubscrHash(contact.ContactId, DateTime.Now, mandatorId),
				                     eventToNotify.EventId,
				                     contact.ContactId,
				                     true);
			}
			catch (Exception ex)
			{
				Common.Helpers.TrySendErrorMail(ex);
			}
			return link;
		}

		/// <summary>
		/// Internal method to get the list of all the subscriptions for an event as raw html table rows
		/// </summary>
		private static string GetSubscriptionList4Notification(IEnumerable<Subscription> subscriptions)
		{
			string subscriptionList =
				"<tr><th>Datum</th><th>Zeit</th><th>Dauer</th><th>Name</th><th>Handy</th><th>Status</th><th>Kommentar</th></tr>";

			foreach (Subscription s in subscriptions)
			{
				subscriptionList +=
					String.Format(
						"<tr valign=\"top\"><td nowrap>{0}</td><td nowrap>{1}&nbsp;</td><td nowrap>{2}&nbsp;</td><td nowrap>{3}&nbsp;</td><td nowrap>{4}&nbsp;</td><td nowrap>{5}</td><td nowrap>{6}&nbsp;</td></tr>",
						s.Event.StartDate.ToString("dd.MM.yyyy"),
						(s.SubscriptionTime != null && s.SubscriptionTime != "" ? s.SubscriptionTime : s.Event.StartDate.ToString("HH:mm")),
						s.Event.Duration,
						HttpUtility.HtmlEncode(s.Contact.Name),
						s.Contact.MobilePhone,
						s.SubscriptionStateText,
						HttpUtility.HtmlEncode(s.Comment).Replace("\r\n", "\n").Replace("\n", "<br/>"));
			}
			return subscriptionList;
		}

		/// <summary>
		/// sends a mail to the given contact, that notifies about no sms credit left
		/// </summary>
		private void SendNoSmsCreditLeftMail(Contact contact)
		{
			if (!contact.NoSmsCreditNotified)
			{
				EmailMessage noSmsCreditMail = (!String.IsNullOrEmpty(smtpPass)
				                                	? new EmailMessage(smtpServer, smtpUseSSL, smtpPort, smtpUser, smtpPass)
				                                	: new EmailMessage(smtpServer));
				string body =
					string.Format(
						@"Hallo {0}

Dein persönlicher SMS Kredit auf ""{1}"" ist aufgebraucht! Setze dich mit dem Webmaster in Verbindung, um wieder SMS Kredit zu erhalten.
Achtung: Diese Benachrichtigungs-Mail erhälst du nur dieses eine Mal!

Automatisch generierte E-Mail.",
						contact.Name, mandator.SiteTitle);
				if (!offlineMode)
				{
					try
					{
						noSmsCreditMail.SendMail(mandator.MandatorName, mandator.MandatorMail, contact.Email,
						                         mandator.SiteTitle + " - SMS Kredit aufgebraucht!", body,
						                         EmailMessage.EmailMessageFormat.Text);
						contact.NoSmsCreditNotified = true;
						eventSiteBL.EditContact(contact);
					}
					catch (Exception ex)
					{
						LoggerManager.GetLogger().ErrorException("Exception while notify the user of no sms credit left.", ex);
					}
				}
				else
				{
					Simulation.SimulateEmail(mandator.MandatorMail, contact.Email, mandator.SiteTitle + " - SMS Kredit aufgebraucht!",
					                         body, EmailMessage.EmailMessageFormat.Text);
					contact.NoSmsCreditNotified = true;
					eventSiteBL.EditContact(contact);
				}
			}
		}

		/// <summary>
		/// sends a mail to the mandator email, that notifies about no sms credit left.
		/// </summary>
		private void SendNoSmsCreditLeftMail()
		{
			if (!mandator.NoSmsCreditNotified)
			{
				EmailMessage noSmsCreditMail = (!String.IsNullOrEmpty(smtpPass)
				                                	? new EmailMessage(smtpServer, smtpUseSSL, smtpPort, smtpUser, smtpPass)
				                                	: new EmailMessage(smtpServer));
				string body =
					string.Format(
						@"Hallo {0}

Der SMS Kredit des Mandanten ""{1}"" ist aufgebraucht! Somit werden keine gratis SMS mehr verschickt. Setze dich mit dem Webmaster in Verbindung, um neuen SMS Kredit zu erhalten.

Automatisch generierte E-Mail.",
						mandator.MandatorName, mandator.MandatorShortName);
				if (!offlineMode)
				{
					try
					{
						noSmsCreditMail.SendMail(mandator.MandatorName, mandator.MandatorMail, mandator.MandatorMail,
						                         mandator.SiteTitle + " - SMS Kredit aufgebraucht!", body,
						                         EmailMessage.EmailMessageFormat.Text);
						mandator.NoSmsCreditNotified = true;
						eventSiteBL.EditMandatorAlertMailSettings();
					}
					catch (Exception ex)
					{
						LoggerManager.GetLogger().ErrorException("Exception while notify the mandator of no sms credit left.", ex);
					}
				}
				else
				{
					Simulation.SimulateEmail(mandator.MandatorMail, mandator.MandatorMail,
					                         mandator.SiteTitle + " - SMS Kredit aufgebraucht!", body,
					                         EmailMessage.EmailMessageFormat.Text);
					mandator.NoSmsCreditNotified = true;
					eventSiteBL.EditMandatorAlertMailSettings();
				}
			}
		}

		#endregion
	}
}
