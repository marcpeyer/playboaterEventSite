using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using kcm.ch.EventSite.Common;
using playboater.gallery.commons;
using playboater.gallery.DAL;
using pbHelpers=playboater.gallery.DAL.Helpers;

namespace kcm.ch.EventSite.DataAccessLayer
{
	/// <summary>
	/// EventSite data access layer. Provides several methods for data access.
	/// </summary>
	public class EventSiteDA
	{
		private const string contactCacheKey = "ES_Contact_{0}";
		private const string realSubscrCacheKey = "ES_RealSubscriptions_{0}";
		private const string subscriptionStatesCacheKey = "ES_SubscriptionStates_{0}";
		private const string eventCacheKey = "ES_Event_{0}";
		private static string connStr;
		private static readonly int defaultCacheDuration = 5;

		private static string connectionString
		{
			get { return connStr ?? (connStr = EventSiteConfiguration.Current.SqlConnectionString); }
		}

		#region Mandator methods
		/// <summary>
		/// Lists all mandators in the database.
		/// </summary>
		/// <returns>Returns a list of each <see cref="Mandator">Mandator</see> in the db.</returns>
		public static List<Mandator> ListMandators()
		{
			List<Mandator> mandators = new List<Mandator>();

			/*mandators.Add(new Mandator("1", "name 1", "short 1", "mail@mail", "www entry", "", "", null, null, null,
				false, true, false, false, false, false, false, false, false, false, false, false, false, null, null, null,
				0, 0, false, false, null, null, null, null));
			mandators.Add(new Mandator("2", "name 2", "short 2", "mail@mail", "www entry", "", "", null, null, null,
				false, true, false, false, false, false, false, false, false, false, false, false, false, null, null, null,
				0, 0, false, false, null, null, null, null));
			mandators.Add(new Mandator("3", "name 3", "short 3", "mail@mail", "www entry", "", "", null, null, null,
				false, true, false, false, false, false, false, false, false, false, false, false, false, null, null, null,
				0, 0, false, false, null, null, null, null));
			return mandators;*/

			using (StoredProcedure sp = new StoredProcedure(connectionString, "ES_ListMandators"))
			{
				SqlDataReader data = sp.ExecuteDataReader();

				while (data.Read())
				{
					Mandator mandator = new Mandator(
						pbHelpers.GetString(data, "MandatorId"),
						pbHelpers.GetString(data, "MandatorName"),
						pbHelpers.GetString(data, "MandatorShortName"),
						pbHelpers.GetString(data, "MandatorMail"),
						pbHelpers.GetString(data, "EntryPointUrl"),
						pbHelpers.GetString(data, "SiteTitle"),
						pbHelpers.GetString(data, "EventName"),
						pbHelpers.GetString(data, "FeatureAssembly"),
						pbHelpers.GetString(data, "FeatureAssemblyClassName"),
						pbHelpers.GetString(data, "EventNotificationAddressesDefault"),
						pbHelpers.GetBool(data, "ShowEventsAsList"),
						pbHelpers.GetBool(data, "UseEventCategories"),
						pbHelpers.GetBool(data, "UseEventUrl"),
						pbHelpers.GetBool(data, "UseMinMaxSubscriptions"),
						pbHelpers.GetBool(data, "UseSubscriptions"),
						pbHelpers.GetBool(data, "SmsNotifications"),
						pbHelpers.GetBool(data, "OnNewEventNotifyContacts"),
						pbHelpers.GetBool(data, "OnEditEventNotifyContacts"),
						pbHelpers.GetBool(data, "OnNewSubscriptionNotifyContacts"),
						pbHelpers.GetBool(data, "OnEditSubscriptionNotifyContacts"),
						pbHelpers.GetBool(data, "OnDeleteSubscriptionNotifyContacts"),
						pbHelpers.GetBool(data, "IsLiftManagementEnabled"),
						pbHelpers.GetBool(data, "NotifyDeletableSubscriptionStates"),
						pbHelpers.GetString(data, "HelpText"),
						pbHelpers.GetNInt32(data, "UnsubscribeAllowedFromNumSubscriptions"),
						pbHelpers.GetNInt32(data, "UnsubscribeAllowedTillNumSubscriptions"),
						pbHelpers.GetInt32(data, "SmsLog"),
						pbHelpers.GetInt32(data, "SmsPurchased"),
						pbHelpers.GetBool(data, "NoSmsCreditNotified"),
						pbHelpers.GetBool(data, "UseExternalAuth"),
						pbHelpers.GetString(data, "AuthTable"),
						pbHelpers.GetString(data, "AuthIdColumn"),
						pbHelpers.GetString(data, "AuthLoginColumn"),
						pbHelpers.GetString(data, "AuthPasswordColumn"));
							
					mandators.Add(mandator);
				}

				data.Close();

				int retVal = -1;
				retVal = sp.ReturnValue;
				switch (retVal)
				{
					case 0:
						//all ok
						return mandators;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Selektieren der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Gets an existing mandator from the database.
		/// </summary>
		/// <param name="mandatorId">Mandator's id</param>
		/// <returns>Returns the <see cref="Mandator">Mandator</see> from the db.</returns>
		public static Mandator GetMandator(string mandatorId)
		{
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_GetMandator"))
			{
				sp.AddParameter("@MandatorId", SqlDbType.Char, mandatorId, 32);
				SqlDataReader data = sp.ExecuteDataReader();

				Mandator mandator = null;
				if(data.Read())
				{
					mandator = new Mandator(
						pbHelpers.GetString(data, "MandatorId"),
						pbHelpers.GetString(data, "MandatorName"),
						pbHelpers.GetString(data, "MandatorShortName"),
						pbHelpers.GetString(data, "MandatorMail"),
						pbHelpers.GetString(data, "EntryPointUrl"),
						pbHelpers.GetString(data, "SiteTitle"),
						pbHelpers.GetString(data, "EventName"),
						pbHelpers.GetString(data, "FeatureAssembly"),
						pbHelpers.GetString(data, "FeatureAssemblyClassName"),
						pbHelpers.GetString(data, "EventNotificationAddressesDefault"),
						pbHelpers.GetBool(data, "ShowEventsAsList"), 
						pbHelpers.GetBool(data, "UseEventCategories"), 
						pbHelpers.GetBool(data, "UseEventUrl"),
						pbHelpers.GetBool(data, "UseMinMaxSubscriptions"),
						pbHelpers.GetBool(data, "UseSubscriptions"),
						pbHelpers.GetBool(data, "SmsNotifications"),
						pbHelpers.GetBool(data, "OnNewEventNotifyContacts"),
						pbHelpers.GetBool(data, "OnEditEventNotifyContacts"),
						pbHelpers.GetBool(data, "OnNewSubscriptionNotifyContacts"),
						pbHelpers.GetBool(data, "OnEditSubscriptionNotifyContacts"),
						pbHelpers.GetBool(data, "OnDeleteSubscriptionNotifyContacts"),
						pbHelpers.GetBool(data, "IsLiftManagementEnabled"),
						pbHelpers.GetBool(data, "NotifyDeletableSubscriptionStates"),
						pbHelpers.GetString(data, "HelpText"),
						pbHelpers.GetNInt32(data, "UnsubscribeAllowedFromNumSubscriptions"),
						pbHelpers.GetNInt32(data, "UnsubscribeAllowedTillNumSubscriptions"),
						pbHelpers.GetInt32(data, "SmsLog"),
						pbHelpers.GetInt32(data, "SmsPurchased"),
						pbHelpers.GetBool(data, "NoSmsCreditNotified"),
						pbHelpers.GetBool(data, "UseExternalAuth"),
						pbHelpers.GetString(data, "AuthTable"),
						pbHelpers.GetString(data, "AuthIdColumn"),
						pbHelpers.GetString(data, "AuthLoginColumn"),
						pbHelpers.GetString(data, "AuthPasswordColumn")
						);
				}

				data.Close();

				int retVal = -1;
				retVal = sp.ReturnValue;
				switch(retVal)
				{
					case 100:
						throw new EventSiteException("Der gewünschte Mandant wurde nicht gefunden.", 100);
					case 0:
						//all ok
						if(mandator != null)
						{
							return mandator;
						}
						else
						{
							throw new EventSiteException("Unbekannter Fehler beim Abrufen des Mandanten.", 900);
						}
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Selektieren der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Modifies the default event notification mail addresses of a mandator in the database.
		/// </summary>
		/// <param name="mandator">the mandator object with all information</param>
		public static void EditMandatorDefaultNotificationAddresses(Mandator mandator)
		{
			int retVal = -1;
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_EditMandatorDefaultNotificationAddresses"))
			{
				sp.AddParameter("@MandatorId", SqlDbType.Char, mandator.MandatorId, 32);
				sp.AddParameter("@MailAddresses", SqlDbType.VarChar, mandator.EventNotificationAddressesDefault, 800);
				sp.Execute();
				retVal = sp.ReturnValue;

				switch(retVal)
				{
					case 100:
						throw new EventSiteException("Unbekannter Fehler beim Updaten der Daten", 100);
					case 0:
						//all ok
						return;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Editieren der Daten", 900);
				}
			}
		}


        /// <summary>
        /// Modifies the field NoSmsCreditNotified of a mandator in the database.
        /// </summary>
        /// <param name="mandator">the mandator object with all information</param>
        public static void EditMandatorAlertMailSettings(Mandator mandator)
        {
            int retVal = -1;
            using (StoredProcedure sp = new StoredProcedure(connectionString, "ES_EditMandatorAlertMailSettings"))
            {
                sp.AddParameter("@MandatorId", SqlDbType.Char, mandator.MandatorId, 32);
                sp.AddParameter("@NoSmsCreditNotified", SqlDbType.Bit, mandator.NoSmsCreditNotified);
                sp.Execute();
                retVal = sp.ReturnValue;

                switch (retVal)
                {
                    case 100:
                        throw new EventSiteException("Unbekannter Fehler beim Updaten der Daten", 100);
                    case 0:
                        //all ok
                        return;
                    default:
                        throw new EventSiteException("Unbekannter ReturnValue nach dem Editieren der Daten", 900);
                }
            }
        }
        #endregion

		#region Contact methods

		#region Contact
		public static Contact Login(string login, string password, Mandator mandator)
		{
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_Login"))
			{
				string userIp = String.Empty;
				try
				{
					userIp = System.Web.HttpContext.Current.Request.UserHostAddress;
				}
				catch{}
				
				sp.AddParameter("@UserIp", SqlDbType.VarChar, userIp, 15);
				sp.AddParameter("@Login", SqlDbType.VarChar, login, 50);
				sp.AddParameter("@Password", SqlDbType.VarChar, password, 50);
				sp.AddParameter("@MandatorId", SqlDbType.Char, mandator.MandatorId, 32);
				string authTable, authIdCol, authLoginCol, authPwCol;
				if(mandator.UseExternalAuth)
				{
					authTable = mandator.AuthTable;
					authIdCol = mandator.AuthIdColumn;
					authLoginCol = mandator.AuthLoginColumn;
					authPwCol = mandator.AuthPasswordColumn;
				}
				else
				{
					authTable = Constants.InternalAuthTable;
					authIdCol = Constants.InternalAuthIdCol;
					authLoginCol = Constants.InternalAuthLoginCol;
					authPwCol = Constants.InternalAuthPasswordCol;
				}
				sp.AddParameter("@CustTable", SqlDbType.VarChar, authTable, 50);
				sp.AddParameter("@CustIdCol", SqlDbType.VarChar, authIdCol, 50);
				sp.AddParameter("@CustLoginCol", SqlDbType.VarChar, authLoginCol, 50);
				sp.AddParameter("@CustPwCol", SqlDbType.VarChar, authPwCol, 50);
				SqlDataReader data = sp.ExecuteDataReader();

				Contact contact = null;
				EventSiteException exception = null;
				if(data.Read())
				{
					try
					{
						contact = new Contact(
							pbHelpers.GetInt32(data, "ContactId"),
							mandator,
							pbHelpers.GetString(data, "Name"),
							pbHelpers.GetString(data, "Email"),
							pbHelpers.GetString(data, "MobilePhone"),
							pbHelpers.GetBool(data, "LiftMgmtSmsOn"),
							pbHelpers.GetBool(data, "EventMgmtSmsOn"),
							pbHelpers.GetBool(data, "UseTwoWaySms"),
							pbHelpers.GetInt32(data, "SmsLog"),
							pbHelpers.GetInt32(data, "SmsPurchased"),
							pbHelpers.GetBool(data, "IsDeleted"),
							pbHelpers.GetBool(data, "NoSmsCreditNotified"),
							pbHelpers.GetString(data, "Login"));
					}
					catch(Exception e)
					{
						exception = new EventSiteException("Bei der Erstellung des Contacts ist ein Fehler aufgetreten.", 900, e);
						LoggerManager.GetLogger().ErrorException("Bei der Erstellung des Contacts ist ein Fehler aufgetreten.", e);
					}
				}

				data.Close();

				int retVal = -1;
				retVal = sp.ReturnValue;
				switch(retVal)
				{
					case 100:
						throw new EventSiteException("Fehler beim Insert in ES_Log.", 100);
					case 101:
						throw new EventSiteException("In den letzten 30 Minuten gibt es von dieser IP-Adresse zu viele fehlgeschlagene Login-Versuche. Bitte später wieder versuchen.", 101);
					case 102:
						throw new EventSiteException("Login / Passwort falsch. Bitte erneut probieren.", 102);
					case 103:
						throw new EventSiteException("Unter diesem Mandanten bist du nicht registriert.\\nDu bist jedoch bereits auf einem anderen Mandanten\\nregistriert. Wechsle den Mandanten oder registriere\\ndich neu unter diesem Mandanten (Link auf dieser\\nSeite \"hier neu registrieren\").", 103);
					case 0:
						//all ok
						if(exception != null)
						{
							throw exception;
						}
						else if(contact != null)
						{
							return contact;
						}
						else
						{
							throw new EventSiteException("Unbekannter Fehler beim Login.", 900);
						}
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Login.", 900);
				}
			}			
		}

		public static Contact RemoteLogin(int customUserId, int contactId, Mandator mandator, string userName)
		{
			using (StoredProcedure sp = new StoredProcedure(connectionString, "ES_RemoteLogin"))
			{
				string userIp = String.Empty;
				try
				{
					userIp = System.Web.HttpContext.Current.Request.UserHostAddress;
				}
				catch { }
				
				sp.AddParameter("@UserIp", SqlDbType.VarChar, userIp, 15);
				sp.AddParameter("@CustomUserId", SqlDbType.Int, customUserId == 0 ? System.DBNull.Value : (object)customUserId);
				sp.AddParameter("@ContactId", SqlDbType.Int, contactId ==0 ? System.DBNull.Value : (object)contactId);
				sp.AddParameter("@MandatorId", SqlDbType.Char, mandator.MandatorId, 32);
				string authTable, authIdCol, authLoginCol, authPwCol;
				if (mandator.UseExternalAuth)
				{
					authTable = mandator.AuthTable;
					authIdCol = mandator.AuthIdColumn;
					authLoginCol = mandator.AuthLoginColumn;
					authPwCol = mandator.AuthPasswordColumn;
				}
				else
				{
					authTable = Constants.InternalAuthTable;
					authIdCol = Constants.InternalAuthIdCol;
					authLoginCol = Constants.InternalAuthLoginCol;
					authPwCol = Constants.InternalAuthPasswordCol;
				}
				sp.AddParameter("@CustTable", SqlDbType.VarChar, authTable, 50);
				sp.AddParameter("@CustIdCol", SqlDbType.VarChar, authIdCol, 50);
				sp.AddParameter("@CustLoginCol", SqlDbType.VarChar, authLoginCol, 50);
				sp.AddParameter("@CustPwCol", SqlDbType.VarChar, authPwCol, 50);
				SqlDataReader data = sp.ExecuteDataReader();

				Contact contact = null;
				EventSiteException exception = null;
				if(data.Read())
				{
					try
					{
						userName = pbHelpers.GetString(data, mandator.UseExternalAuth ? mandator.AuthLoginColumn : Constants.InternalAuthLoginCol);
						contact = new Contact(
							pbHelpers.GetInt32(data, "ContactId"),
							mandator,
							pbHelpers.GetString(data, "Name"),
							pbHelpers.GetString(data, "Email"),
							pbHelpers.GetString(data, "MobilePhone"),
							pbHelpers.GetBool(data, "LiftMgmtSmsOn"),
							pbHelpers.GetBool(data, "EventMgmtSmsOn"),
							pbHelpers.GetBool(data, "UseTwoWaySms"),
							pbHelpers.GetInt32(data, "SmsLog"),
							pbHelpers.GetInt32(data, "SmsPurchased"),
							pbHelpers.GetBool(data, "IsDeleted"),
							pbHelpers.GetBool(data, "NoSmsCreditNotified"),
							pbHelpers.GetString(data, "Login"));
					}
					catch(Exception e)
					{
						exception = new EventSiteException("Bei der Erstellung des Contacts ist ein Fehler aufgetreten.", 900, e);
						LoggerManager.GetLogger().ErrorException("Bei der Erstellung des Contacts ist ein Fehler aufgetreten.", e);
					}
				}

				data.Close();

				int retVal = -1;
				retVal = sp.ReturnValue;
				switch(retVal)
				{
					case 100:
						throw new EventSiteException("Fehler beim Insert in ES_Log.", 100);
					case 101:
						throw new EventSiteException("In den letzten 30 Minuten gibt es von dieser IP-Adresse zu viele fehlgeschlagene Login-Versuche. Bitte später wieder versuchen.", 101);
					case 102:
						throw new EventSiteException("Login verweigert!", 102);
					case 104:
						throw new EventSiteException("Ungültige Parameter wurden dem Datenbank Request übergeben.", 104);
					case 0:
						//all ok
						if(exception != null)
						{
							throw exception;
						}
						else if(contact != null)
						{
							return contact;
						}
						else
						{
							throw new EventSiteException("Unbekannter Fehler beim RemoteLogin.", 900);
						}
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem RemoteLogin.", 900);
				}
			}			
		}

		public static string GetContactLogin(int contactId)
		{
			int retVal = -1;
			using (StoredProcedure sp = new StoredProcedure(connectionString, "ES_GetContactLogin"))
			{
				sp.AddParameter("@ContactId", SqlDbType.Int, contactId);
				string login = sp.ExecuteScalar() as string;
				retVal = sp.ReturnValue;

				switch (retVal)
				{
					case 100:
						throw new EventSiteException("Die angegebene ContactId wurde nicht gefunden. Evtl. wurde der Kontakt inzwischen gelöscht.", 100);
					case 0:
						//all ok
						return login;
					default:
						throw new EventSiteException("Unbekannter ReturnValue beim lesen des Logins", 900);
				}
			}
		}

		public static void ChangePassword(int contactId, string newLogin, string newPassword)
		{
			int retVal = -1;
			using (StoredProcedure sp = new StoredProcedure(connectionString, "ES_ChangePassword"))
			{
				sp.AddParameter("@ContactId", SqlDbType.Int, contactId);
				sp.AddParameter("@NewLogin", SqlDbType.VarChar, newLogin, 50);
				sp.AddParameter("@NewPassword", SqlDbType.VarChar, newPassword, 50);
				sp.Execute();
				retVal = sp.ReturnValue;

				switch (retVal)
				{
					case 100:
						throw new EventSiteException("Die angegebene ContactId wurde nicht gefunden. Evtl. wurde der Kontakt inzwischen gelöscht.", 100);
					case 101:
						throw new EventSiteException("Unbekannter Fehler beim Updaten der Daten", 101);
					case 0:
						//all ok
						return;
					default:
						throw new EventSiteException("Unbekannter ReturnValue beim Ändern des Passworts", 900);
				}
			}
		}

		public static string GetPassword(int contactId)
		{
			int retVal = -1;
			using (StoredProcedure sp = new StoredProcedure(connectionString, "ES_GetPassword"))
			{
				sp.AddParameter("@ContactId", SqlDbType.Int, contactId);
				string pw = sp.ExecuteScalar() as string;
				retVal = sp.ReturnValue;

				switch (retVal)
				{
					case 100:
						throw new EventSiteException("Die angegebene ContactId wurde nicht gefunden. Evtl. wurde der Kontakt inzwischen gelöscht.", 100);
					case 0:
						//all ok
						return pw;
					default:
						throw new EventSiteException("Unbekannter ReturnValue beim lesen des Logins", 900);
				}
			}
		}

		/// <summary>
		/// Gets an existing contact from the database.
		/// </summary>
		/// <param name="email">The contact's email address</param>
		/// <param name="mandator">The mandator on which the desired contact is stored</param>
		/// <returns>Returns the corresponding <see cref="Contact">Contact</see> from db.</returns>
		public static Contact GetContact(string email, Mandator mandator)
		{
			using (StoredProcedure sp = new StoredProcedure(connectionString, "ES_GetContact"))
			{
				sp.AddParameter("@Email", SqlDbType.VarChar, email, 50);
				sp.AddParameter("@MandatorId", SqlDbType.Char, mandator.MandatorId, 32);
				SqlDataReader data = sp.ExecuteDataReader();

				Contact contact = null;
				if(data.Read())
				{
					contact = new Contact(
						pbHelpers.GetInt32(data, "ContactId"),
						mandator,
						pbHelpers.GetString(data, "Name"),
						pbHelpers.GetString(data, "Email"),
						pbHelpers.GetString(data, "MobilePhone"),
						pbHelpers.GetBool(data, "LiftMgmtSmsOn"),
						pbHelpers.GetBool(data, "EventMgmtSmsOn"),
						pbHelpers.GetBool(data, "UseTwoWaySms"),
						pbHelpers.GetInt32(data, "SmsLog"),
						pbHelpers.GetInt32(data, "SmsPurchased"),
						pbHelpers.GetBool(data, "IsDeleted"),
						pbHelpers.GetBool(data, "NoSmsCreditNotified"),
						pbHelpers.GetString(data, "Login"));
				}

				data.Close();

				int retVal = -1;
				retVal = sp.ReturnValue;
				switch(retVal)
				{
					case 100:
						throw new EventSiteException("Weder Email noch ContactId wurden als Parameter übergeben.", 100);
					case 101:
						throw new EventSiteException("Der gewünschte Kontakt wurde nicht gefunden.", 101);
					case 0:
						//all ok
						if(contact != null)
						{
							//cache this contact
							Cache.Set(String.Format(contactCacheKey, contact.ContactId), contact, TimeSpan.FromSeconds(30));
							return contact;
						}
						else
						{
							throw new EventSiteException("Unbekannter Fehler beim Abrufen des Kontakts.", 900);
						}
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Selektieren der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Gets an existing contact from the database by it's mobile number
		/// </summary>
		/// <param name="mobileNumber">The contact's mobile number</param>
		/// <param name="mandator">The mandator on which the desired contact is stored</param>
		/// <returns>Returns the corresponding <see cref="Contact">Contact</see> from db.</returns>
		public static Contact GetContactByMobileNumber(string mobileNumber, Mandator mandator)
		{
			using (StoredProcedure sp = new StoredProcedure(connectionString, "ES_GetContactByMobileNumber"))
			{
				sp.AddParameter("@MobileNumber", SqlDbType.VarChar, mobileNumber, 20);
				sp.AddParameter("@MandatorId", SqlDbType.Char, mandator.MandatorId, 32);
				SqlDataReader data = sp.ExecuteDataReader();

				Contact contact = null;
				if(data.Read())
				{
					contact = new Contact(
						pbHelpers.GetInt32(data, "ContactId"),
						mandator,
						pbHelpers.GetString(data, "Name"),
						pbHelpers.GetString(data, "Email"),
						pbHelpers.GetString(data, "MobilePhone"),
						pbHelpers.GetBool(data, "LiftMgmtSmsOn"),
						pbHelpers.GetBool(data, "EventMgmtSmsOn"),
						pbHelpers.GetBool(data, "UseTwoWaySms"),
						pbHelpers.GetInt32(data, "SmsLog"),
						pbHelpers.GetInt32(data, "SmsPurchased"),
						pbHelpers.GetBool(data, "IsDeleted"),
						pbHelpers.GetBool(data, "NoSmsCreditNotified"),
						pbHelpers.GetString(data, "Login"));
				}

				data.Close();

				int retVal = -1;
				retVal = sp.ReturnValue;
				switch(retVal)
				{
					case 100:
						throw new EventSiteException("Parameter MobileNumber fehlt oder ist leer.", 100);
					case 101:
						throw new EventSiteException("Der gewünschte Kontakt wurde nicht gefunden.", 101);
					case 0:
						//all ok
						if(contact != null)
						{
							//cache this contact
							Cache.Set(String.Format(contactCacheKey, contact.ContactId), contact, TimeSpan.FromSeconds(30));
							return contact;
						}
						else
						{
							throw new EventSiteException("Unbekannter Fehler beim Abrufen des Kontakts.", 900);
						}
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Selektieren der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Gets the roles in which a contact is member of.
		/// </summary>
		/// <param name="contact">the contact to which the roles belong</param>
		/// <returns>Returns the corresponding roles from db.</returns>
		public static ArrayList GetContactsRoles(Contact contact)
		{
			ArrayList roles = new ArrayList();
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_GetContactsRoles"))
			{
				sp.AddParameter("@ContactId", SqlDbType.Int, contact.ContactId);
				SqlDataReader data = sp.ExecuteDataReader();

				while(data.Read())
				{
						roles.Add(pbHelpers.GetString(data, "Role"));
				}

				data.Close();

				int retVal = -1;
				retVal = sp.ReturnValue;
				switch(retVal)
				{
					case 0:
						//all ok
						return roles;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Selektieren der Rollen eines Contacts", 900);
				}
			}
		}

		/// <summary>
		/// Gets an existing contact from the database.
		/// </summary>
		/// <param name="contactId">The contact's id</param>
		/// <param name="mandator">The mandator on which the desired contact is stored</param>
		/// <returns>Returns the corresponding <see cref="Contact">Contact</see> from db.</returns>
		public static Contact GetContact(int contactId, Mandator mandator)
		{
			return GetContact(contactId, mandator, false);
		}

		/// <summary>
		/// Gets an existing contact from the database.
		/// </summary>
		/// <param name="contactId">The contact's id</param>
		/// <param name="mandator">The mandator on which the desired contact is stored</param>
		/// <returns>Returns the corresponding <see cref="Contact">Contact</see> from db.</returns>
		public static Contact GetContact(int contactId, Mandator mandator, bool forceReload)
		{
			string cacheKey = String.Format(contactCacheKey, contactId);
			Contact contact = null;

			if(!forceReload)
			{
				if(Cache.TryGet(cacheKey, out contact)) return contact;
			}

			using (StoredProcedure sp = new StoredProcedure(connectionString, "ES_GetContact"))
			{
				sp.AddParameter("@ContactId", SqlDbType.Int, contactId);
				sp.AddParameter("@MandatorId", SqlDbType.Char, mandator.MandatorId, 32);
				SqlDataReader data = sp.ExecuteDataReader();

				if(data.Read())
				{
					contact = new Contact(
						pbHelpers.GetInt32(data, "ContactId"),
						mandator,
						pbHelpers.GetString(data, "Name"),
						pbHelpers.GetString(data, "Email"),
						pbHelpers.GetString(data, "MobilePhone"),
						pbHelpers.GetBool(data, "LiftMgmtSmsOn"),
						pbHelpers.GetBool(data, "EventMgmtSmsOn"),
						pbHelpers.GetBool(data, "UseTwoWaySms"),
						pbHelpers.GetInt32(data, "SmsLog"),
						pbHelpers.GetInt32(data, "SmsPurchased"),
						pbHelpers.GetBool(data, "IsDeleted"),
						pbHelpers.GetBool(data, "NoSmsCreditNotified"),
						pbHelpers.GetString(data, "Login"));
				}

				data.Close();

				int retVal = -1;
				retVal = sp.ReturnValue;
				switch(retVal)
				{
					case 100:
						throw new EventSiteException("Weder Email noch ContactId wurden als Parameter übergeben.", 100);
					case 101:
						throw new EventSiteException("Der gewünschte Kontakt wurde nicht gefunden.", 101);
					case 0:
						//all ok
						if(contact != null)
						{
							//cache this contact
							Cache.Set(cacheKey, contact, TimeSpan.FromSeconds(30));
							return contact;
						}
						else
						{
							throw new EventSiteException("Unbekannter Fehler beim Abrufen des Kontakts.", 900);
						}
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Selektieren der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Stores the given contact into the database.
		/// </summary>
		/// <param name="contact">The <see cref="Contact">Contact</see> to store.</param>
		/// <returns>A <c>string</c> with a user info. Can be null!</returns>
		public static string AddContact(Contact contact)
		{
			int retVal = -1;
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_AddContact"))
			{
				sp.AddParameter("@MandatorId", SqlDbType.Char, contact.Mandator.MandatorId, 32);
				sp.AddParameter("@Name", SqlDbType.VarChar, contact.Name, 50);
				sp.AddParameter("@Email", SqlDbType.VarChar, contact.Email, 50);
				sp.AddParameter("@MobilePhone", SqlDbType.VarChar, contact.MobilePhone, 20);
				sp.AddParameter("@LiftMgmtSmsOn", SqlDbType.Bit, contact.LiftMgmtSmsOn);
				sp.AddParameter("@EventMgmtSmsOn", SqlDbType.Bit, contact.EventMgmtSmsOn);
				sp.AddParameter("@SmsLog", SqlDbType.Int, contact.SmsLog);
				sp.AddParameter("@SmsPurchased", SqlDbType.Int, contact.SmsPurchased);
				object data = sp.ExecuteScalar();
				retVal = sp.ReturnValue;

				switch(retVal)
				{
					case 100:
						throw new EventSiteException("Ein Kontakt mit dieser E-Mail Adresse besteht bereits in der Datenbank!", 100);
					case 101:
						throw new EventSiteException("Unbekannter Fehler beim Einfügen der Daten", 101);
					case 102:
						throw new EventSiteException("Unbekannter Fehler beim Updaten der Daten. Es wurde versucht einen gelöschten Kontakt wiederherzustellen.", 102);
					case 0:
					case 10:
						//all ok
						int contactId = Convert.ToInt32(data);
						contact.ContactId = contactId;

						if(retVal == 10)
						{
							return "Da ein gelöschter Kontakt mit dieser Email-Adresse gefunden wurde,\\nwurde dieser wiederhergestellt.";
						}
						return "Neuer Account erfolgreich erstellt.";
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Einfügen der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Updates the given contact in the database.
		/// </summary>
		/// <param name="contact">The <see cref="Contact">Contact</see> to update.</param>
		public static void EditContact(Contact contact)
		{
			int retVal = -1;
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_EditContact"))
			{
				sp.AddParameter("@ContactId", SqlDbType.Int, contact.ContactId);
				sp.AddParameter("@Name", SqlDbType.VarChar, contact.Name, 50);
				sp.AddParameter("@Email", SqlDbType.VarChar, contact.Email, 50);
				sp.AddParameter("@MobilePhone", SqlDbType.VarChar, contact.MobilePhone, 20);
				sp.AddParameter("@LiftMgmtSmsOn", SqlDbType.Bit, contact.LiftMgmtSmsOn);
				sp.AddParameter("@EventMgmtSmsOn", SqlDbType.Bit, contact.EventMgmtSmsOn);
				sp.AddParameter("@SmsPurchased", SqlDbType.Int, contact.SmsPurchased);
				sp.AddParameter("@NoSmsCreditNotified", SqlDbType.Bit, contact.NoSmsCreditNotified);
				sp.Execute();
				retVal = sp.ReturnValue;

				switch(retVal)
				{
					case 100:
						throw new EventSiteException("Ein Kontakt mit der neuen E-Mail Adresse besteht bereits in der Datenbank!", 100);
					case 102:
						throw new EventSiteException("Die angegebene ContactId wurde nicht gefunden. Evtl. wurde der Kontakt inzwischen gelöscht.", 102);
					case 101:
						throw new EventSiteException("Unbekannter Fehler beim Updaten der Daten", 101);
					case 0:
						//all ok
						return;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Editieren der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Deletes the desired contact in the database.
		/// </summary>
		/// <param name="contactId">Contact's contactId to delete</param>
		public static void DelContact(int contactId)
		{
			int retVal = -1;
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_DelContact"))
			{
				sp.AddParameter("@ContactId", SqlDbType.Int, contactId);
				sp.Execute();
				retVal = sp.ReturnValue;

				switch(retVal)
				{
					case 101:
						throw new EventSiteException("Unbekannter Fehler beim Löschen der Daten", 101);
					case 0:
						return;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Löschen der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Lists all contacts on the given mandator.
		/// </summary>
		/// <param name="mandator">The mandator for which the contacts should be retrieved</param>
		/// <returns>Returns an <see cref="ArrayList">ArrayList</see> with the contacts.</returns>
		public static ArrayList ListContacts(Mandator mandator)
		{
			ArrayList contacts = new ArrayList();
			using (StoredProcedure sp = new StoredProcedure(connectionString, "ES_ListContacts"))
			{
				sp.AddParameter("@MandatorId", SqlDbType.Char, mandator.MandatorId, 32);
				SqlDataReader data = sp.ExecuteDataReader();

				while(data.Read())
				{
					Contact contact = new Contact(
						pbHelpers.GetInt32(data, "ContactId"),
						mandator,
						pbHelpers.GetString(data, "Name"),
						pbHelpers.GetString(data, "Email"),
						pbHelpers.GetString(data, "MobilePhone"),
						pbHelpers.GetBool(data, "LiftMgmtSmsOn"),
						pbHelpers.GetBool(data, "EventMgmtSmsOn"),
						pbHelpers.GetBool(data, "UseTwoWaySms"),
						pbHelpers.GetInt32(data, "SmsLog"),
						pbHelpers.GetInt32(data, "SmsPurchased"),
						pbHelpers.GetBool(data, "IsDeleted"),
						pbHelpers.GetBool(data, "NoSmsCreditNotified"),
						pbHelpers.GetString(data, "Login"));

					contacts.Add(contact);
				}

				data.Close();

				int retVal = -1;
				retVal = sp.ReturnValue;
				switch(retVal)
				{
					case 0:
						//all ok
						return contacts;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Selektieren der Daten", 900);
				}
			}
		}
		#endregion

		#region ContactSettings

		/// <summary>
		/// Loads all settings for a given contact.
		/// </summary>
		/// <param name="contact">The contact for which the settings should be retrieved</param>
		/// <remarks>Sets the contact's ContactSettings property to a <see cref="Hashtable">Hashtable</see> with the <see cref="ContactSetting">settings</see> as value and the <see cref="EventCategory">category</see> as key.</remarks>
		public static void LoadContactSettings(Contact contact)
		{
			Hashtable settings = new Hashtable();
			
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_ListContactSettings"))
			{
				sp.AddParameter("@MandatorId", SqlDbType.Char, contact.Mandator.MandatorId, 32);
				sp.AddParameter("@ContactId", SqlDbType.Int, contact.ContactId);
				SqlDataReader data = sp.ExecuteDataReader();

				while(data.Read())
				{
					ContactSetting setting;
					if(pbHelpers.GetNInt32(data, "ContactSettingId").IsNull)
					{
						//create new EventSetting
						EventCategory cat = GetEventCategory(pbHelpers.GetInt32(data, "EventCategoryId"), contact.Mandator);
						setting = new ContactSetting(
							contact,
							cat,
							true,
							cat.FreeEventSmsNotifications,
							true,
							new NInt32(0, true));
					}
					else
					{
						//create existing EventSetting
						setting = new ContactSetting(
							pbHelpers.GetInt32(data, "ContactSettingId"),
							contact,
							GetEventCategory(pbHelpers.GetInt32(data, "EventCategoryId"), contact.Mandator),
							pbHelpers.GetBool(data, "NotifyByEmail"),
							pbHelpers.GetBool(data, "NotifyBySms"),
							pbHelpers.GetBool(data, "SmsNotifSubscriptionsOn"),
							pbHelpers.GetNInt32(data, "AutoNotifSubscription"));
					}
					settings.Add(setting.Category.EventCategoryId, setting);
				}

				data.Close();

				int retVal = -1;
				retVal = sp.ReturnValue;
				switch(retVal)
				{
					case 0:
						//all ok
						contact.ContactSettings = settings;
						return;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Selektieren der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Saves all the settings for a given contact
		/// </summary>
		/// <param name="contact">The contact</param>
		public static void SaveContactSettings(Contact contact)
		{
			foreach (DictionaryEntry entry in contact.ContactSettings)
			{
				ContactSetting setting = (ContactSetting)entry.Value;
				//if no change --> skip setting
				if(setting.ContactSettingId == 0)
				{
					continue;
				}
				int retVal = -1;
				using(StoredProcedure sp = new StoredProcedure())
				{
					sp.ConnectionString = connectionString;
					sp.SPName = setting.ContactSettingId == -1 ? "ES_AddContactSetting" : "ES_EditContactSetting";

					if(setting.ContactSettingId == -1)
					{
						sp.AddParameter("@ContactId", SqlDbType.Int, contact.ContactId);
						sp.AddParameter("@EventCategoryId", SqlDbType.Int, setting.Category.EventCategoryId);
					}
					else
					{
						sp.AddParameter("@ContactSettingId", SqlDbType.Int, setting.ContactSettingId);
					}
					sp.AddParameter("@NotifyBySMS", SqlDbType.Bit, setting.NotifyBySms);
					sp.AddParameter("@NotifyByEmail", SqlDbType.Bit, setting.NotifyByEmail);
					sp.AddParameter("@SmsNotifSubscriptionsOn", SqlDbType.Bit, setting.SmsNotifSubscriptionsOn);
					pbHelpers.AddNInt32Param(sp, "@AutoNotifSubscription", setting.AutoNotifSubscription);
					sp.Execute();
					retVal = sp.ReturnValue;

					switch(retVal)
					{
						case 100:
							throw new EventSiteException("Beim Einfügen der Daten ist ein Fehler aufgetreten!", 100);
						case 101:
							throw new EventSiteException("Das angegebene Setting wurde nicht gefunden.", 101);
						case 102:
							throw new EventSiteException("Unbekannter Fehler beim Updaten der Daten", 102);
						case 0:
							//all ok
							continue;
						default:
							throw new EventSiteException("Unbekannter ReturnValue nach dem Editieren der Daten", 900);
					}
				}
			}

			LoadContactSettings(contact);
		}
		#endregion

		#endregion

		#region SubscriptionState methods
		/// <summary>
		/// Gets all subscriptionStates on the given mandator.
		/// </summary>
		/// <param name="category">The category on which the subscriptionStates are stored</param>
		/// <returns>Returns a <see cref="DataSet">DataSet</see> with the subscriptionStates.</returns>
		public static DataSet ListSubscriptionStates(EventCategory category)
		{
			string cacheKey = String.Format(subscriptionStatesCacheKey, category.EventCategoryId);
			DataSet subscriptionStates;
			if(Cache.TryGet(cacheKey, out subscriptionStates))
			{
				return subscriptionStates;
			}

			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_ListSubscriptionStates"))
			{
				sp.AddParameter("@EventCategoryId", SqlDbType.Int, category.EventCategoryId);
				subscriptionStates = sp.ExecuteDataSet();
			}

			Cache.Set(cacheKey, subscriptionStates, TimeSpan.FromMinutes(30));
			return subscriptionStates;
		}
		#endregion

		#region Location methods
		/// <summary>
		/// Gets an existing location from the database.
		/// </summary>
		/// <param name="locationId">Location's id</param>
		/// <param name="category">The location's category</param>
		/// <returns>Returns the <see cref="Location">Location</see> from the db.</returns>
		public static Location GetLocation(int locationId, EventCategory category)
		{
			Hashtable locations = GetCachedLocations(category);

			Location location;
			location = (Location)locations[locationId];
			if(location != null)
			{
				return location;
			}
			else
			{
				throw new EventSiteException("Die gewünschte Location wurde nicht gefunden.", 100);
			}
		}

		/// <summary>
		/// Stores the given location into the database.
		/// </summary>
		/// <param name="location">The <see cref="Location">Location</see> to store</param>
		/// <returns>A <c>string</c> with a user info. Can be null!</returns>
		public static string AddLocation(Location location)
		{
			int retVal = -1;
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_AddLocation"))
			{
				sp.AddParameter("@EventCategoryId", SqlDbType.Int, location.EventCategory.EventCategoryId);
				sp.AddParameter("@LocationText", SqlDbType.VarChar, location.LocationText, 50);
				sp.AddParameter("@LocationShort", SqlDbType.VarChar, location.LocationShort, 15);
				sp.AddParameter("@LocationDescription", SqlDbType.VarChar, location.LocationDescription, 200);
				object data = sp.ExecuteScalar();
				retVal = sp.ReturnValue;

				switch(retVal)
				{
					case 100:
						throw new EventSiteException("Einen Ort mit dieser Bezeichnung in dieser Kategorie besteht bereits in der Datenbank!", 100);
					case 101:
						throw new EventSiteException("Unbekannter Fehler beim Einfügen der Daten", 101);
					case 102:
						throw new EventSiteException("Unbekannter Fehler beim Updaten der Daten. Es wurde versucht einen gelöschten Ort wiederherzustellen.", 102);
					case 0:
					case 10:
						//all ok
						int locationId = Convert.ToInt32(data);
						location.LocationId = locationId;
						CacheLocations(location.EventCategory);

						if(retVal == 10)
						{
							return "Da ein gelöschter Ort mit dieser Bezeichnung gefunden wurde,\\nwurde dieser wiederhergestellt.";
						}
					return null;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Einfügen der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Updates the given location in the database.
		/// </summary>
		/// <param name="location">The <see cref="Location">Location</see> to update</param>
		public static void EditLocation(Location location)
		{
			int retVal = -1;
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_EditLocation"))
			{
				sp.AddParameter("@LocationId", SqlDbType.Int, location.LocationId);
				sp.AddParameter("@LocationText", SqlDbType.VarChar, location.LocationText, 50);
				sp.AddParameter("@LocationShort", SqlDbType.VarChar, location.LocationShort, 15);
				sp.AddParameter("@LocationDescription", SqlDbType.VarChar, location.LocationDescription, 200);
				sp.Execute();
				retVal = sp.ReturnValue;

				switch(retVal)
				{
					case 100:
						throw new EventSiteException("Ein Ort mit der neuen Bezeichnung besteht bereits in dieser Kategorie in der Datenbank!\\nFalls er gelöscht ist, erstelle einen neuen mit demselben Namen, um ihn wiederherzustellen.", 100);
					case 101:
						throw new EventSiteException("Der Ort mit dieser Id wurde nicht gefunden.", 101);
					case 102:
						throw new EventSiteException("Unbekannter Fehler beim Updaten der Daten.", 102);
					case 0:
						//all ok
						CacheLocations(location.EventCategory);
						return;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Editieren der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Deletes the given location in the database.
		/// </summary>
		/// <param name="locationId">The location's id to delete</param>
		/// <param name="category">The location's mandator</param>
		public static void DelLocation(int locationId, EventCategory category)
		{
			int retVal = -1;
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_DelLocation"))
			{
				sp.AddParameter("@LocationId", SqlDbType.Int, locationId);
				sp.Execute();
				retVal = sp.ReturnValue;

				switch(retVal)
				{
					case 101:
						throw new EventSiteException("Unbekannter Fehler beim Löschen der Daten", 101);
					case 0:
						CacheLocations(category);
						return;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Löschen der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Lists all locations on the given category.
		/// </summary>
		/// <param name="category">The category on which the locations are stored</param>
		/// <returns>Returns an ArrayList with the locations.</returns>
		public static ArrayList ListLocations(EventCategory category)
		{
			Hashtable locations = GetCachedLocations(category);

			ArrayList locationList = new ArrayList();
			foreach (DictionaryEntry location in locations)
			{
				locationList.Add(location.Value);
			}
			return locationList;
		}

		private static Hashtable CacheLocations(EventCategory category)
		{
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_ListLocations"))
			{
				sp.AddParameter("@EventCategoryId", SqlDbType.Int, category.EventCategoryId);
				SqlDataReader data = sp.ExecuteDataReader();

				Hashtable locations = new Hashtable();
				while(data.Read())
				{
					Location location = new Location(
						pbHelpers.GetInt32(data, "LocationId"),
						category,
						pbHelpers.GetString(data, "LocationText"),
						pbHelpers.GetString(data, "LocationShort"),
						pbHelpers.GetString(data, "LocationDescription"));

					locations.Add(location.LocationId, location);
				}

				data.Close();

				int retVal = -1;
				retVal = sp.ReturnValue;
				switch(retVal)
				{
					case 0:
						//all ok
						Cache.Set("ES_Locations_" + category.EventCategoryId, locations);
						return locations;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Selektieren der Daten", 900);
				}
			}
		}

		private static Hashtable GetCachedLocations(EventCategory category)
		{
			Hashtable locations;
			if(Cache.TryGet("ES_Locations_" + category.EventCategoryId, out locations))
			{
				return locations;
			}

			return CacheLocations(category);
		}
		#endregion

		#region Event methods
		/// <summary>
		/// Gets an existing event from the database. If possible use overload with mandator!
		/// </summary>
		/// <param name="eventId">The event's id</param>
		/// <returns>Returns the corresponding <see cref="Event">Event</see> from the db.</returns>
		public static Event GetEvent(int eventId)
		{
			return GetEvent(eventId, null);
		}

		/// <summary>
		/// Gets an existing event from the database.
		/// </summary>
		/// <param name="eventId">The event's id</param>
		/// <param name="mandator">The event's mandator</param>
		/// <returns>Returns the corresponding <see cref="Event">Event</see> from the db.</returns>
		public static Event GetEvent(int eventId, Mandator mandator)
		{
			string cacheKey = string.Format(eventCacheKey, eventId);

			Event requestedEvent;
			if(Cache.TryGet(cacheKey, out requestedEvent))
			{
				return requestedEvent;
			}

			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_GetEvent"))
			{
				sp.AddParameter("@EventId", SqlDbType.Char, eventId, 32);
				SqlDataReader data = sp.ExecuteDataReader();

				if(data.Read())
				{
					Mandator m = (mandator != null ? mandator : GetMandator(pbHelpers.GetString(data, "MandatorId")));
					EventCategory ec = GetEventCategory(pbHelpers.GetInt32(data, "EventCategoryId"), m);
					requestedEvent = new Event(
						pbHelpers.GetInt32(data, "EventId"),
						ec,
						GetContact(pbHelpers.GetInt32(data, "EventCreatorId"), m),
						GetLocation(pbHelpers.GetInt32(data, "LocationId"), ec),
						pbHelpers.GetDateTime(data, "StartDate"),
						pbHelpers.GetString(data, "Duration"),
						pbHelpers.GetString(data, "EventTitle"),
						pbHelpers.GetString(data, "EventDescription"),
						pbHelpers.GetString(data, "EventUrl"),
						pbHelpers.GetNInt32(data, "MinSubscriptions"),
						pbHelpers.GetNInt32(data, "MaxSubscriptions"));
				}

				data.Close();

				int retVal = -1;
				retVal = sp.ReturnValue;
				switch(retVal)
				{
					case 100:
						throw new EventSiteException("Der gewünschte Anlass wurde nicht gefunden.", 100);
					case 0:
						//all ok
						if(requestedEvent != null)
						{
							Cache.Set(cacheKey, requestedEvent, TimeSpan.FromSeconds(30));
							return requestedEvent;
						}
						else
						{
							throw new EventSiteException("Unbekannter Fehler beim Abrufen des Mandanten.", 900);
						}
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Selektieren der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Adds the given event into the database.
		/// </summary>
		/// <param name="newEvent">The event to add to the db</param>
		public static void AddEvent(Event newEvent)
		{
			int retVal = -1;
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_AddEvent"))
			{
				sp.AddParameter("@MandatorId", SqlDbType.Char, newEvent.EventCategory.Mandator.MandatorId, 32);
				sp.AddParameter("@LocationId", SqlDbType.Int, newEvent.Location.LocationId);
				sp.AddParameter("@EventCreatorId", SqlDbType.Int, newEvent.EventCreator.ContactId);
				sp.AddParameter("@EventCategoryId", SqlDbType.Int, newEvent.EventCategory.EventCategoryId);
				sp.AddParameter("@StartDate", SqlDbType.VarChar, newEvent.StartDate.ToString("dd.MM.yyyy HH:mm:ss"), 19);
				sp.AddParameter("@Duration", SqlDbType.VarChar, newEvent.Duration, 50);
				sp.AddParameter("@EventTitle", SqlDbType.VarChar, newEvent.EventTitle, 50);
				sp.AddParameter("@EventDescription", SqlDbType.VarChar, newEvent.EventDescription, 8000);
				sp.AddParameter("@EventUrl", SqlDbType.VarChar, newEvent.EventUrl, 100);
				pbHelpers.AddNInt32Param(sp, "@MinSubscriptions", newEvent.MinSubscriptions);
				pbHelpers.AddNInt32Param(sp, "@MaxSubscriptions", newEvent.MaxSubscriptions);
				object data = sp.ExecuteScalar();
				retVal = sp.ReturnValue;

				switch(retVal)
				{
					case 100:
						throw new EventSiteException("Es existiert bereits ein Anlass in dieser Kategorie, mit diesem Titel an diesem Datum", 100);
					case 101:
						throw new EventSiteException("Unbekannter Fehler beim Einfügen der Daten", 101);
					case 0:
						//all ok
						int eventId = Convert.ToInt32(data);
						newEvent.EventId = eventId;
						Cache.Set(string.Format(eventCacheKey, eventId), newEvent, TimeSpan.FromSeconds(30));
						return;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Einfügen der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Edits an existing event in the database.
		/// </summary>
		/// <param name="editEvent">The event to edit in the db</param>
		public static void EditEvent(Event editEvent)
		{
			int retVal = -1;
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_EditEvent"))
			{
				sp.AddParameter("@EventId", SqlDbType.Int, editEvent.EventId);
				sp.AddParameter("@LocationId", SqlDbType.Int, editEvent.Location.LocationId);
				sp.AddParameter("@StartDate", SqlDbType.VarChar, editEvent.StartDate.ToString("dd.MM.yyyy HH:mm:ss"), 19);
				sp.AddParameter("@Duration", SqlDbType.VarChar, editEvent.Duration, 50);
				sp.AddParameter("@EventTitle", SqlDbType.VarChar, editEvent.EventTitle, 50);
				sp.AddParameter("@EventDescription", SqlDbType.VarChar, editEvent.EventDescription, 8000);
				sp.AddParameter("@EventUrl", SqlDbType.VarChar, editEvent.EventUrl, 100);
				pbHelpers.AddNInt32Param(sp, "@MinSubscriptions", editEvent.MinSubscriptions);
				pbHelpers.AddNInt32Param(sp, "@MaxSubscriptions", editEvent.MaxSubscriptions);
				sp.Execute();
				retVal = sp.ReturnValue;

				switch(retVal)
				{
					case 100:
						throw new EventSiteException("Ein Anlass mit diesem Titel an diesem Datum besteht bereits in der Datenbank!", 100);
					case 101:
						throw new EventSiteException("Unbekannter Fehler beim Updaten der Daten", 101);
					case 0:
						//all ok
						Cache.Set(string.Format(eventCacheKey, editEvent.EventId), editEvent, TimeSpan.FromSeconds(30));
						return;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Editieren der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Deletes an event in the database.
		/// </summary>
		/// <param name="eventId">The event's id to delete</param>
		public static void DelEvent(int eventId)
		{
			throw new NotImplementedException("Das Löschen von Events wird nicht unterstützt.");
			int retVal = -1;
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_DelEvent"))
			{
				sp.AddParameter("@EventId", SqlDbType.Int, eventId);
				sp.Execute();
				retVal = sp.ReturnValue;

				switch(retVal)
				{
					case 101:
						throw new EventSiteException("Unbekannter Fehler beim Löschen der Daten", 101);
					case 0:
						return;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Löschen der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Lists all events stored on the given mandator.
		/// </summary>
		/// <param name="mandator">The mandator on which the events are stored</param>
		/// <returns>Returns an <see cref="ArrayList">ArrayList</see> with the events.</returns>
		public static List<Event> ListEvents(Mandator mandator)
		{
			return ListEvents(mandator, false);
		}

		/// <summary>
		/// Lists all events stored on the given mandator.
		/// </summary>
		/// <param name="mandator">The mandator on which the events are stored</param>
		/// <param name="onlyFuture">Defines if events in the past should be returned or not.</param>
		/// <returns>Returns an <see cref="ArrayList">ArrayList</see> with the events.</returns>
		public static List<Event> ListEvents(Mandator mandator, bool onlyFuture)
		{
			List<Event> events = new List<Event>();
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_ListEvents"))
			{
				sp.AddParameter("@MandatorId", SqlDbType.Char, mandator.MandatorId, 32);
				if(onlyFuture)
				{
					sp.AddParameter("@OnlyFuture", SqlDbType.Bit, 1);
				}
				SqlDataReader data = sp.ExecuteDataReader();

				while(data.Read())
				{
					EventCategory ec = GetEventCategory(pbHelpers.GetInt32(data, "EventCategoryId"), mandator);
					Event dbEvent = new Event(
						pbHelpers.GetInt32(data, "EventId"),
						ec,
						GetContact(pbHelpers.GetInt32(data, "EventCreatorId"), mandator),
						GetLocation(pbHelpers.GetInt32(data, "LocationId"), ec),
						pbHelpers.GetDateTime(data, "StartDate"),
						pbHelpers.GetString(data, "Duration"),
						pbHelpers.GetString(data, "EventTitle"),
						pbHelpers.GetString(data, "EventDescription"),
						pbHelpers.GetString(data, "EventUrl"),
						pbHelpers.GetNInt32(data, "MinSubscriptions"),
						pbHelpers.GetNInt32(data, "MaxSubscriptions"));

					events.Add(dbEvent);
				}

				data.Close();

				int retVal = -1;
				retVal = sp.ReturnValue;
				switch(retVal)
				{
					case 0:
						//all ok
						return events;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Selektieren der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Lists all events stored on the given category.
		/// </summary>
		/// <param name="category">The category on which the events are stored</param>
		/// <returns>Returns an <see cref="ArrayList">ArrayList</see> with the events.</returns>
		public static List<Event> ListEvents(EventCategory category)
		{
			return ListEvents(category, false);
		}

		/// <summary>
		/// Lists all events stored on the given category.
		/// </summary>
		/// <param name="category">The category on which the events are stored</param>
		/// <param name="onlyFuture">Defines if events in the past should be returned or not.</param>
		/// <returns>Returns an <see cref="ArrayList">ArrayList</see> with the events.</returns>
		public static List<Event> ListEvents(EventCategory category, bool onlyFuture)
		{
			List<Event> events = new List<Event>();
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_ListEventsByCategory"))
			{
				sp.AddParameter("@EventCategoryId", SqlDbType.Int, category.EventCategoryId);
				if(onlyFuture)
				{
					sp.AddParameter("@OnlyFuture", SqlDbType.Bit, 1);
				}
				SqlDataReader data = sp.ExecuteDataReader();

				while(data.Read())
				{
					Event dbEvent = new Event(
						pbHelpers.GetInt32(data, "EventId"),
						category,
						GetContact(pbHelpers.GetInt32(data, "EventCreatorId"), category.Mandator),
						GetLocation(pbHelpers.GetInt32(data, "LocationId"), category),
						pbHelpers.GetDateTime(data, "StartDate"),
						pbHelpers.GetString(data, "Duration"),
						pbHelpers.GetString(data, "EventTitle"),
						pbHelpers.GetString(data, "EventDescription"),
						pbHelpers.GetString(data, "EventUrl"),
						pbHelpers.GetNInt32(data, "MinSubscriptions"),
						pbHelpers.GetNInt32(data, "MaxSubscriptions"));

					events.Add(dbEvent);
				}

				data.Close();

				int retVal = -1;
				retVal = sp.ReturnValue;
				switch(retVal)
				{
					case 0:
						//all ok
						return events;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Selektieren der Daten", 900);
				}
			}
		}
		#endregion

		#region EventCategory methods
		public static EventCategory GetEventCategory(int eventCategoryId, Mandator mandator)
		{
			Hashtable categories = GetCachedEventCategories(mandator);
			return (EventCategory)categories[eventCategoryId];
		}

		/// <summary>
		/// Lists all categories on the given mandator.
		/// </summary>
		/// <param name="mandator">The mandator on which the categories are stored</param>
		/// <returns>Returns an ArrayList with the categories.</returns>
		public static ArrayList ListEventCategories(Mandator mandator)
		{
			Hashtable categories = GetCachedEventCategories(mandator);

			ArrayList categoryList = new ArrayList();
			foreach (DictionaryEntry category in categories)
			{
				categoryList.Add(category.Value);
			}
			return categoryList;
		}

		private static Hashtable CacheEventCategories(Mandator mandator)
		{
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_ListEventCategories"))
			{
				sp.AddParameter("@MandatorId", SqlDbType.Char, mandator.MandatorId, 32);
				SqlDataReader data = sp.ExecuteDataReader();

				Hashtable categories = new Hashtable();
				while(data.Read())
				{
					EventCategory category = new EventCategory(
						pbHelpers.GetInt32(data, "EventCategoryId"),
						mandator,
						pbHelpers.GetString(data, "EventCategory"),
						pbHelpers.GetString(data, "CategoryDescription"),
						pbHelpers.GetString(data, "FeatureAssembly"),
						pbHelpers.GetString(data, "FeatureAssemblyClassName"),
						pbHelpers.GetBool(data, "FreeEventSmsNotifications"));

					categories.Add(category.EventCategoryId, category);
				}

				data.Close();

				int retVal = -1;
				retVal = sp.ReturnValue;
				switch(retVal)
				{
					case 0:
						//all ok

						Cache.Set("ES_EventCategories_" + mandator.MandatorId, categories);
						return categories;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Selektieren der Daten", 900);
				}
			}
		}

		private static Hashtable GetCachedEventCategories(Mandator mandator)
		{
			Hashtable categories;
			if (Cache.TryGet("ES_EventCategories_" + mandator.MandatorId, out categories))
			{
				return categories;
			}

			return CacheEventCategories(mandator);
		}
		#endregion

		#region Subscription methods

		/// <summary>
		/// returns the number of subscriptions on an event with an undeletable subscription state
		/// </summary>
		private static int GetRealSubscriptions(Event ev)
		{
			string cacheKey = String.Format(realSubscrCacheKey, ev.EventId);
			int realSubscriptions = 0;

			if (Cache.TryGet(cacheKey, out realSubscriptions)) return realSubscriptions;

			//call ListSubscriptions() to get numRealSubscriptions cached
			List<Subscription> subscriptions = ListSubscriptions(ev);

			if (Cache.TryGet(cacheKey, out realSubscriptions)) return realSubscriptions;

			foreach (Subscription subscription in subscriptions)
			{
				if (!subscription.SubscriptionStateIsDeletable)
				{
					realSubscriptions++;
				}
			}

			Cache.Set(cacheKey, realSubscriptions, TimeSpan.FromSeconds(15));

			return realSubscriptions;
		}

		#region Subscription
		/// <summary>
		/// Gets a subscription from the given subscriptionId.
		/// </summary>
		/// <param name="subscriptionId">The id for which the subscription should be returned.</param>
		/// <returns>Returns a <see cref="Subscription">Subscription</see> from the db or null if no
		/// subscription is found for the given id.</returns>
		public static Subscription GetSubscription(int subscriptionId, Mandator mandator)
		{
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_GetSubscription"))
			{
				sp.AddParameter("@SubscriptionId", SqlDbType.Int, subscriptionId);
				SqlDataReader data = sp.ExecuteDataReader();

				Subscription subscription = null;
				Event subscrEvent;
				if(data.Read())
				{
					subscription = new Subscription(
						pbHelpers.GetInt32(data, "SubscriptionId"),
						(subscrEvent = GetEvent(pbHelpers.GetInt32(data, "EventId"), mandator)),
						GetContact(pbHelpers.GetInt32(data, "ContactId"), mandator),
						pbHelpers.GetInt32(data, "SubscriptionStateId"),
						pbHelpers.GetString(data, "SubscriptionStateText"),
						pbHelpers.GetBool(data, "SubscriptionStateIsDeletable"),
						pbHelpers.GetBool(data, "SubscriptionStateIsDeletable") ||
							mandator.UnsubscribeAllowedTillNumSubscriptions.IsNull || GetRealSubscriptions(subscrEvent) <= mandator.UnsubscribeAllowedTillNumSubscriptions ||
							mandator.UnsubscribeAllowedFromNumSubscriptions.IsNull || GetRealSubscriptions(subscrEvent) >= mandator.UnsubscribeAllowedFromNumSubscriptions,
						pbHelpers.GetString(data, "SubscriptionTime"),
						pbHelpers.GetString(data, "Fontcolor"),
						pbHelpers.GetString(data, "Comment"),
						pbHelpers.GetNInt32(data, "NumLifts"),
						pbHelpers.GetNInt32(data, "LiftSubscriptionJourneyStationId"),
						(LiftState)pbHelpers.GetInt32(data, "LiftState"));
				}

				data.Close();

				int retVal = -1;
				retVal = sp.ReturnValue;
				switch(retVal)
				{
					case 0:
					case 10:
						//all ok
						return subscription;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Selektieren der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Gets a subscription from the given contact on the given event.
		/// </summary>
		/// <param name="contact">The contact for which the subscription should be returned.</param>
		/// <param name="ev">The event for which the subscription should be returned.</param>
		/// <returns>Returns a <see cref="Subscription">Subscription</see> from the db or null if no
		/// subscription is found for the given contact on the given event.</returns>
		public static Subscription GetSubscriptionFromContact(Event ev, Contact contact)
		{
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_GetSubscriptionFromContact"))
			{
				sp.AddParameter("@EventId", SqlDbType.Int, ev.EventId);
				sp.AddParameter("@ContactId", SqlDbType.Int, contact.ContactId);
				SqlDataReader data = sp.ExecuteDataReader();

				Subscription subscription = null;
				if (data.Read())
				{
					subscription = new Subscription(
						pbHelpers.GetInt32(data, "SubscriptionId"),
						ev,
						contact,
						pbHelpers.GetInt32(data, "SubscriptionStateId"),
						pbHelpers.GetString(data, "SubscriptionStateText"),
						pbHelpers.GetBool(data, "SubscriptionStateIsDeletable"),
						pbHelpers.GetBool(data, "SubscriptionStateIsDeletable") ||
							contact.Mandator.UnsubscribeAllowedTillNumSubscriptions.IsNull || GetRealSubscriptions(ev) <= contact.Mandator.UnsubscribeAllowedTillNumSubscriptions ||
							contact.Mandator.UnsubscribeAllowedFromNumSubscriptions.IsNull || GetRealSubscriptions(ev) >= contact.Mandator.UnsubscribeAllowedFromNumSubscriptions,
						pbHelpers.GetString(data, "SubscriptionTime"),
						pbHelpers.GetString(data, "Fontcolor"),
						pbHelpers.GetString(data, "Comment"),
						pbHelpers.GetNInt32(data, "NumLifts"),
						pbHelpers.GetNInt32(data, "LiftSubscriptionJourneyStationId"),
						(LiftState)pbHelpers.GetInt32(data, "LiftState"));
				}

				data.Close();

				int retVal = -1;
				retVal = sp.ReturnValue;
				switch(retVal)
				{
					case 0:
					case 10:
						//all ok
						return subscription;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Selektieren der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Stores the given subscription into the database.
		/// </summary>
		/// <param name="subscription">The <see cref="Subscription">Subscription</see> to store</param>
		public static Subscription AddSubscription(Subscription subscription)
		{
			int retVal = -1;
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_AddSubscription"))
			{
				sp.AddParameter("@EventId", SqlDbType.Int, subscription.Event.EventId);
				sp.AddParameter("@ContactId", SqlDbType.Int, subscription.Contact.ContactId);
				sp.AddParameter("@SubscriptionStateId", SqlDbType.Int, subscription.SubscriptionStateId);
				sp.AddParameter("@SubscriptionTime", SqlDbType.VarChar, subscription.SubscriptionTime, 50);
				sp.AddParameter("@Comment", SqlDbType.VarChar, subscription.Comment, 8000);
				sp.AddParameter("@NumLifts", SqlDbType.Int, System.DBNull.Value);
				object data = sp.ExecuteScalar();
				retVal = sp.ReturnValue;

				switch(retVal)
				{
					case 100:
						throw new EventSiteException("Es existiert bereits einen Eintrag für diesen Anlass von diesem Kontakt.", 100);
					case 101:
						throw new EventSiteException("Unbekannter Fehler beim Einfügen der Daten", 101);
					case 0:
						//all ok
						subscription = GetSubscription(Convert.ToInt32(data), subscription.Contact.Mandator);
//						int subscriptionId = Convert.ToInt32(data);
//						subscription.SubscriptionId = subscriptionId;
						return subscription;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Einfügen der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Updates the given subscription in the database.
		/// </summary>
		/// <param name="subscription">The <see cref="Subscription">subscription</see> to update.</param>
		public static void EditSubscription(Subscription subscription)
		{
			int retVal = -1;
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_EditSubscription"))
			{
				sp.AddParameter("@SubscriptionId", SqlDbType.Int, subscription.SubscriptionId);
				sp.AddParameter("@SubscriptionStateId", SqlDbType.Int, subscription.SubscriptionStateId);
				sp.AddParameter("@SubscriptionTime", SqlDbType.VarChar, subscription.SubscriptionTime, 50);
				sp.AddParameter("@Comment", SqlDbType.VarChar, subscription.Comment, 8000);
				pbHelpers.AddNInt32Param(sp, "@NumLifts", subscription.NumLifts);
				pbHelpers.AddNInt32Param(sp, "@LiftSubscriptionJourneyStationId", subscription.LiftSubscriptionJourneyStationId);
				sp.Execute();
				retVal = sp.ReturnValue;

				switch(retVal)
				{
					case 100:
						throw new EventSiteException("Der gegebene Anlass wurde in der Datenbank nicht gefunden.", 100);
					case 101:
						throw new EventSiteException("Fehler beim Ändern der bestehenden LiftSubscription", 101);
					case 102:
						throw new EventSiteException("Fehler beim hinzufügen einer neuen LiftSubscription", 102);
					case 103:
						throw new EventSiteException("Unbekannter Fehler beim Updaten der Daten", 103);
					case 0:
						//all ok
						return;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Editieren der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Lists all subscriptions on the given event.
		/// </summary>
		/// <param name="selEvent">The event on which the subscriptions are stored</param>
		/// <returns>Returns an ArrayList with the subscriptions.</returns>
		public static List<Subscription> ListSubscriptions(Event selEvent)
		{
			List<Subscription> subscriptions = new List<Subscription>();
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_ListSubscriptions"))
			{
				sp.AddParameter("@EventId", SqlDbType.Int, selEvent.EventId);
				SqlDataReader data = sp.ExecuteDataReader();

				Mandator mandator = selEvent.EventCategory.Mandator;
				Event ev;

				int numRealSubscriptions;
				if(data.Read())
				{
					string cacheKey = String.Format(realSubscrCacheKey, selEvent.EventId);
					numRealSubscriptions = pbHelpers.GetInt32(data, "NumRealSubscriptions");
					Cache.Set(cacheKey, numRealSubscriptions, TimeSpan.FromSeconds(15));
				}
				else
				{
					throw new EventSiteException("Fehler: Anzahl Eintragungen konnte nicht ermittelt werden!", 899);
				}

				data.NextResult();

				while (data.Read())
				{
					Subscription subscription = new Subscription(
						pbHelpers.GetInt32(data, "SubscriptionId"),
						(ev = GetEvent(pbHelpers.GetInt32(data, "EventId"), mandator)),
						GetContact(pbHelpers.GetInt32(data, "ContactId"), mandator),
						pbHelpers.GetInt32(data, "SubscriptionStateId"),
						pbHelpers.GetString(data, "SubscriptionStateText"),
						pbHelpers.GetBool(data, "SubscriptionStateIsDeletable"),
						pbHelpers.GetBool(data, "SubscriptionStateIsDeletable") ||
							mandator.UnsubscribeAllowedTillNumSubscriptions.IsNull || numRealSubscriptions <= mandator.UnsubscribeAllowedTillNumSubscriptions ||
							mandator.UnsubscribeAllowedFromNumSubscriptions.IsNull || numRealSubscriptions >= mandator.UnsubscribeAllowedFromNumSubscriptions,
						pbHelpers.GetString(data, "SubscriptionTime"),
						pbHelpers.GetString(data, "Fontcolor"),
						pbHelpers.GetString(data, "Comment"),
						pbHelpers.GetNInt32(data, "NumLifts"),
						pbHelpers.GetNInt32(data, "LiftSubscriptionJourneyStationId"),
						(LiftState)pbHelpers.GetInt32(data, "LiftState"));

					subscriptions.Add(subscription);
				}

				data.Close();

				int retVal = -1;
				retVal = sp.ReturnValue;
				switch(retVal)
				{
					case 0:
						//all ok
						return subscriptions;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Selektieren der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Returns the number of lifts that are defined on the given subscriptions journey
		/// </summary>
		public static int GetNumDefinedLifts(Subscription subscription)
		{
			int retVal = -1;
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_GetNumDefinedLifts"))
			{
				sp.AddParameter("@SubscriptionId", SqlDbType.Int, subscription.SubscriptionId);
				object data = sp.ExecuteScalar();
				retVal = sp.ReturnValue;

				switch(retVal)
				{
					case 100:
					case 101:
						throw new EventSiteException("Dieser Eintrag wurde in der Datenbank nicht gefunden. Evtl. wurde er zwischenzeitlich gelöscht.", 100);
					case 0:
						//all ok
						int numLifts = Convert.ToInt32(data);
						return numLifts;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Selektieren der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Returns the number of lifts that are defined on the given subscriptions journey
		/// </summary>
		public static int GetNumDefinedLifts(JourneyStation journeyStation)
		{
			int retVal = -1;
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_GetNumDefinedLifts"))
			{
				sp.AddParameter("@JourneyStationId", SqlDbType.Int, journeyStation.JourneyStationId);
				object data = sp.ExecuteScalar();
				retVal = sp.ReturnValue;

				switch(retVal)
				{
					case 100:
					case 101:
						throw new EventSiteException("Dieser Eintrag wurde in der Datenbank nicht gefunden. Evtl. wurde er zwischenzeitlich gelöscht.", 100);
					case 0:
						//all ok
						int numLifts = Convert.ToInt32(data);
						return numLifts;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach Selektieren der Daten", 900);
				}
			}
		}
		#endregion

		#region Journey
		/// <summary>
		/// Sets all JourneyStations on the given subscription.
		/// </summary>
		/// <param name="subscription">The subscription on which the JourneyStations are stored</param>
		public static void SetJourneyStations(Subscription subscription)
		{
			subscription.journeyStations.Clear();
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_ListJourneyStations"))
			{
				sp.AddParameter("@SubscriptionId", SqlDbType.Int, subscription.SubscriptionId);
				SqlDataReader data = sp.ExecuteDataReader();

				while(data.Read())
				{
					JourneyStation js = new JourneyStation(
						pbHelpers.GetInt32(data, "JourneyStationId"),
						pbHelpers.GetString(data, "Station"),
						pbHelpers.GetString(data, "StationTime"),
						pbHelpers.GetInt32(data, "SortOrder"));

					subscription.journeyStations.Add(js);
				}

				data.Close();

				int retVal = -1;
				retVal = sp.ReturnValue;
				switch(retVal)
				{
					case 0:
						//all ok
						return;
					default:
						subscription.journeyStations.Clear();
						throw new EventSiteException("Unbekannter ReturnValue nach dem Selektieren der Daten", 900);
				}
			}
		}

		/// <summary>
		/// Saves the JourneyStations in the given subscription to the database.
		/// Inserts new stations, deletes old stations and updates all other stations.
		/// </summary>
		/// <param name="subscription">The subscription from which the JourneyStations are taken into the database.</param>
		public static void SaveJourneyStations(Subscription subscription)
		{
			int retVal = -1;
			using(StoredProcedure spAdd = new StoredProcedure(connectionString, "ES_AddJourneyStation"),
					  spEdit = new StoredProcedure(connectionString, "ES_EditJourneyStation"),
					  spDel = new StoredProcedure(connectionString, "ES_DelJourneyStation"))
			{
				spAdd.AddParameter("@SubscriptionId", SqlDbType.Int, null);
				spAdd.AddParameter("@Station", SqlDbType.VarChar, null, 100);
				spAdd.AddParameter("@StationTime", SqlDbType.VarChar, null, 50);
				spAdd.AddParameter("@SortOrder", SqlDbType.Int, null);

				spEdit.AddParameter("@JourneyStationId", SqlDbType.Int, null);
				spEdit.AddParameter("@Station", SqlDbType.VarChar, null, 100);
				spEdit.AddParameter("@StationTime", SqlDbType.VarChar, null, 50);
				spEdit.AddParameter("@SortOrder", SqlDbType.Int, null);

				spDel.AddParameter("@JourneyStationId", SqlDbType.Int, null);

				ArrayList stationsToRemove = new ArrayList();
				foreach (JourneyStation journeyStation in subscription.journeyStations)
				{
					if(journeyStation.JourneyStationId == 0 && journeyStation.SortOrder > 0)
					{
						//add journeyStation to database
						spAdd.Parameters["@SubscriptionId"].Value = subscription.SubscriptionId;
						spAdd.Parameters["@Station"].Value = journeyStation.Station;
						spAdd.Parameters["@StationTime"].Value = journeyStation.StationTime;
						spAdd.Parameters["@SortOrder"].Value = journeyStation.SortOrder;
						object data = spAdd.ExecuteScalar();
						retVal = spAdd.ReturnValue;

						switch(retVal)
						{
							case 100:
								throw new EventSiteException("Unbekannter Fehler beim Einfügen der Daten", 101);
							case 0:
								//all ok
								int journeyStationId = Convert.ToInt32(data);
								journeyStation.JourneyStationId = journeyStationId;
								break;
							default:
								throw new EventSiteException("Unbekannter ReturnValue nach dem Einfügen der Daten", 900);
						}
						spAdd.Close();
					}
					else if(journeyStation.JourneyStationId != 0 && journeyStation.SortOrder == -1)
					{
						//delete journeyStation in database
						spDel.Parameters["@JourneyStationId"].Value = journeyStation.JourneyStationId;
						spDel.Execute();
						retVal = spDel.ReturnValue;

						switch(retVal)
						{
							case 100:
								throw new EventSiteException("Unbekannter Fehler beim Löschen der Daten", 101);
							case 0:
								//all ok
								break;
							default:
								throw new EventSiteException("Unbekannter ReturnValue nach dem Löschen der Daten", 900);
						}

						//add this station to the array with the stations to be removed after the loop
						stationsToRemove.Add(journeyStation);
						spDel.Close();
					}
					else if(journeyStation.JourneyStationId != 0 && journeyStation.SortOrder > 0)
					{
						//update journeyStation in database
						spEdit.Parameters["@JourneyStationId"].Value = journeyStation.JourneyStationId;
						spEdit.Parameters["@Station"].Value = journeyStation.Station;
						spEdit.Parameters["@StationTime"].Value = journeyStation.StationTime;
						spEdit.Parameters["@SortOrder"].Value = journeyStation.SortOrder;
						spEdit.Execute();
						retVal = spEdit.ReturnValue;

						switch(retVal)
						{
							case 100:
								throw new EventSiteException("Unbekannter Fehler beim Updaten der Daten", 101);
							case 0:
								//all ok
								break;
							default:
								throw new EventSiteException("Unbekannter ReturnValue nach dem Editieren der Daten", 900);
						}
						spEdit.Close();
					}
					else
					{
						throw new EventSiteException("Fehler beim Updaten der JourneyStations. Unbekannter Fall ist eingetreten.", 1001);
					}
				}

				//remove journeyStations from collection
				foreach (JourneyStation journeyStation in stationsToRemove)
				{
					subscription.journeyStations.Remove(journeyStation);
				}
			}
		}

		/// <summary>
		/// Lists all journeys of a given contact.
		/// </summary>
		/// <param name="contact">The contact on which the journeys are stored</param>
		/// <returns>Returns a <see cref="SqlDataReader">SqlDataReader</see> with the journeys.</returns>
		public static SqlDataReader ListContactsJourneys(Contact contact)
		{
			SqlDataReader journeys;
			StoredProcedure sp = new StoredProcedure(connectionString, "ES_ListContactsJourneys");
			
			sp.AddParameter("@ContactId", SqlDbType.Int, contact.ContactId);
			journeys = sp.ExecuteDataReader();
			
			return journeys;
		}
		#endregion
		#endregion

		#region NotifSubscription methods
		public static void SetSmsNotifSubscriptions(Event eventToSet)
		{
			eventToSet.SmsNotifSubscriptions = new List<SmsNotifSubscription>();
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_ListNotifSubscriptions"))
			{
				sp.AddParameter("@EventId", SqlDbType.Int, eventToSet.EventId);
				SqlDataReader data = sp.ExecuteDataReader();

				while(data.Read())
				{
					SmsNotifSubscription smsNotifSubscription = new SmsNotifSubscription(
						pbHelpers.GetInt32(data, "NotifSubscriptionId"),
						eventToSet,
						GetContact(pbHelpers.GetInt32(data, "ContactId"), eventToSet.EventCategory.Mandator),
						pbHelpers.GetNInt32(data, "NumSmsNotifications"));

					eventToSet.SmsNotifSubscriptions.Add(smsNotifSubscription);
				}

				data.Close();

				int retVal = -1;
				retVal = sp.ReturnValue;
				switch(retVal)
				{
					case 0:
						//all ok
						return;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Selektieren der Daten", 900);
				}
			}
		}

		public static void AddSmsNotifSubscription(SmsNotifSubscription smsNotifSubscription)
		{
			int retVal = -1;
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_AddNotifSubscription"))
			{
				sp.AddParameter("@EventId", SqlDbType.Int, smsNotifSubscription.NotifEvent.EventId);
				sp.AddParameter("@ContactId", SqlDbType.Int, smsNotifSubscription.Contact.ContactId);
				pbHelpers.AddNInt32Param(sp, "@NumSmsNotifications", smsNotifSubscription.MaxNotifications);
				object data = sp.ExecuteScalar();
				retVal = sp.ReturnValue;

				switch(retVal)
				{
					case 100:
						throw new EventSiteException("Dieser Kontakt hat bereits ein Abo dieses Anlasses.", 100);
					case 101:
						throw new EventSiteException("Unbekannter Fehler beim Einfügen der Daten", 101);
					case 0:
						//all ok
						int notifSubscriptionId = Convert.ToInt32(data);
						smsNotifSubscription.NotifSubscriptionId = notifSubscriptionId;
						return;// notifSubscription;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Einfügen der Daten", 900);
				}
			}
		}

		public static void EditSmsNotifSubscription(SmsNotifSubscription smsNotifSubscription)
		{
			int retVal = -1;
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_EditNotifSubscription"))
			{
				sp.AddParameter("@NotifSubscriptionId", SqlDbType.Int, smsNotifSubscription.NotifSubscriptionId);
				pbHelpers.AddNInt32Param(sp, "@NumSmsNotifications", smsNotifSubscription.MaxNotifications);
				sp.Execute();
				retVal = sp.ReturnValue;

				switch(retVal)
				{
					case 100:
						throw new EventSiteException("Die Abonnierung wurde in der Datenbank nicht gefunden.", 100);
					case 101:
						throw new EventSiteException("Unbekannter Fehler beim Updaten der Daten", 101);
					case 0:
						//all ok
						return;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Editieren der Daten", 900);
				}
			}
		}

		public static void DeleteSmsNotifSubscription(SmsNotifSubscription smsNotifSubscription)
		{
			int retVal = -1;
			using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_DelNotifSubscription"))
			{
				sp.AddParameter("@NotifSubscriptionId", SqlDbType.Int, smsNotifSubscription.NotifSubscriptionId);
				sp.Execute();
				retVal = sp.ReturnValue;

				switch(retVal)
				{
					case 0:
						//all ok
						return;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem Löschen der Daten.", 900);
				}
			}
		}
		#endregion

		#region Caching methods

		public static string GetCache(string cacheKey)
		{
			using (StoredProcedure sp = new StoredProcedure(connectionString, "ES_GetSimpleCache"))
			{
				sp.AddParameter("@CacheKey", SqlDbType.VarChar, cacheKey, 255);
				object val = sp.ExecuteScalar();
				int retVal = sp.ReturnValue;

				switch (retVal)
				{
					case 0:
						//all ok
						return (string)val;
					case 1:
						return null;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach dem ermitteln des CacheValues", 900);
				}
			}
		}

		public static void SetCache(string cacheKey, string cacheVal)
		{
			SetCache(cacheKey, cacheVal, defaultCacheDuration);
		}

		public static void SetCache(string cacheKey, string cacheVal, int cacheDuration)
		{
			using (StoredProcedure sp = new StoredProcedure(connectionString, "ES_AddSimpleCache"))
			{
				sp.AddParameter("@CacheKey", SqlDbType.VarChar, cacheKey, 255);
				sp.AddParameter("@CacheVal", SqlDbType.VarChar, cacheVal, 8000);
				sp.AddParameter("@CacheDuration", SqlDbType.Int, cacheDuration);
				sp.Execute();
				int retVal = sp.ReturnValue;

				switch (retVal)
				{
					case 0:
						//all ok
						return;
					default:
						throw new EventSiteException("Unbekannter ReturnValue nach hinzufügen eines Cache Eintrages", 900);
				}
			}
		}
		#endregion

		#region Logging
		/// <summary>
		/// Logs a sent sms in the db on the notifsubscription's contact or mandator and decreases MaxNotifications.
		/// </summary>
		/// <param name="smsNotifSubscription">The <see cref="SmsNotifSubscription">NotifSubscription</see> to log the sms.</param>
		/// <param name="typeToBill">The Type to bill the sms message (can be Mandator or Contact)</param>
		public static void LogSms(SmsNotifSubscription smsNotifSubscription, Type typeToBill)
		{
			LogSms(smsNotifSubscription.Contact, typeToBill);

			if(!smsNotifSubscription.MaxNotifications.IsNull)
			{
				smsNotifSubscription.MaxNotifications = smsNotifSubscription.MaxNotifications - 1;
				EditSmsNotifSubscription(smsNotifSubscription);
			}
		}

		/// <summary>
		/// Logs a sent sms in the db on the given contact or mandator corresponding to the typeToBill parameter.
		/// </summary>
		/// <param name="contact">The <see cref="Contact">Contact</see> to log the sms.</param>
		/// <param name="typeToBill">The Type to bill the sms message (can be Mandator or Contact)</param>
		public static void LogSms(Contact contact, Type typeToBill)
		{
			int retVal = -1;
			if(typeToBill == typeof(Mandator))
			{
				using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_LogMandatorSms"))
				{
					sp.AddParameter("@MandatorId", SqlDbType.Char, contact.Mandator.MandatorId);
					sp.Execute();
					retVal = sp.ReturnValue;

#if(DEBUG)
					switch(retVal)
					{
						case 100:
							throw new EventSiteException("Die angegebene MandatorId wurde nicht gefunden.", 102);
						case 0:
							//all ok
							return;
						default:
							throw new EventSiteException("Unbekannter ReturnValue nach dem loggen eines SMS auf dem Mandanten. ReturnValue: " + retVal, 900);
					}
#endif
				}
			}
			else
			{
				using(StoredProcedure sp = new StoredProcedure(connectionString, "ES_LogSms"))
				{
					sp.AddParameter("@ContactId", SqlDbType.Int, contact.ContactId);
					sp.Execute();
					retVal = sp.ReturnValue;

#if(DEBUG)
					switch(retVal)
					{
						case 100:
							throw new EventSiteException("Die angegebene ContactId wurde nicht gefunden. Evtl. wurde der Kontakt inzwischen gelöscht.", 102);
						case 0:
							//all ok
							return;
						default:
							throw new EventSiteException("Unbekannter ReturnValue nach dem loggen eines SMS. ReturnValue: " + retVal, 900);
					}
#endif
				}
			}
		}

		#endregion
	}
}
