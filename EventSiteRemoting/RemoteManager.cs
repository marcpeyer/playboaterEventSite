using System;
using System.Collections;
using System.Collections.Generic;
using kcm.ch.EventSite.BusinessLayer;
using kcm.ch.EventSite.Common;
using playboater.gallery.ClickatellApi;

namespace kcm.ch.EventSite.Remoting
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class RemoteManager : IDisposable
	{
		private readonly EventSiteBL bll;
		private readonly Contact senderContact;
		private readonly Event contextEvent;

		/// <summary>
		/// Instantiates a new RemoteManager without a context Event and with the configured default mandator
		/// </summary>
		/// <param name="mobileNumber"></param>
		public RemoteManager(string mobileNumber)
		{
			bll = new EventSiteBL(EventSiteBL.GetDefaultMandator());
			if(!mobileNumber.StartsWith("+"))
			{
				mobileNumber = "+" + mobileNumber;
			}
			senderContact = bll.GetContactByMobileNumber(mobileNumber);
		}

		/// <summary>
		/// The constructor of RemoteManager which instantiates a new bll and a new contact
		/// </summary>
		/// <param name="eventId">The event id to subscribe</param>
		/// <param name="mobileNumber">The subscriber's mobile number</param>
		public RemoteManager(string mobileNumber, int eventId)
		{
			contextEvent = EventSiteBL.GetEventById(eventId);
			Mandator mand = contextEvent.EventCategory.Mandator;
			bll = new EventSiteBL(mand);
			if (!mobileNumber.StartsWith("+"))
			{
				mobileNumber = "+" + mobileNumber;
			}
			senderContact = bll.GetContactByMobileNumber(mobileNumber);
		}

		public List<Subscription> ListSubscriptions()
		{
			return EventSiteBL.ListSubscriptions(contextEvent);
		}

		/// <summary>
		/// Subscibes a user to an event.
		/// </summary>
		/// <param name="subscriptionStateCode">An Integer that indicates the state of the subscription, where 1 = yes, 2 = later/beer, 3 = no</param>
		/// <param name="subscriptionTime">The time for the subscription (can be null).</param>
		/// <param name="comment">Any comment sent by the subscriber (can be null).</param>
		public bool AddSubscription( int subscriptionStateCode, string subscriptionTime, string comment)
		{
			try
			{
				DictionaryEntry subscriptionState = bll.GetSubscriptionStateByCode(subscriptionStateCode, contextEvent.EventCategory);

				Subscription s = new Subscription(
					contextEvent, 
					senderContact, 
					(int)subscriptionState.Key,
					(string)subscriptionState.Value,
					subscriptionTime,
					comment ?? String.Empty,
					null,
					null);

				bll.AddSubscription(s);
			}
			catch(Exception ex)
			{
				Helpers.TrySendErrorMail(ex);
				throw;
			}
			return true;
		}

		public void ConfigureSms(bool eventMgmtSmsOn)
		{
			senderContact.EventMgmtSmsOn = eventMgmtSmsOn;
			bll.EditContact(senderContact);
		}

		internal void SendSuccessSms(string message)
		{
			if (EventSiteConfiguration.Current.NotificationConfiguration.SendTwoWaySuccessNotificationsOn)
			{
				SendSms(message);
			}
			LoggerManager.GetLogger().Trace(message);
		}

		internal void SendSms(string text)
		{
			if (EventSiteConfiguration.Current.NotificationConfiguration.SendSmsOn)
			{
				try
				{
					if (!EventSiteConfiguration.Current.OfflineMode)
					{
						using (WsManager wsManager = new WsManager(
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
							EventSiteConfiguration.Current.ClickatellApiConfiguration.TextCol))
						{
							wsManager.SendMessage(text, new string[] { senderContact.MobilePhone }, bll.Mandator.MandatorShortName, MessageType.SMS_TEXT);
						}
					}
					else
					{
						Simulation.SimulateSms(text, bll.Mandator.MandatorShortName, senderContact.MobilePhone);
					}

					bll.LogSms(senderContact, typeof(Contact));
				}
				catch (Exception ex)
				{
					LoggerManager.GetLogger().ErrorException("Error occured while sending SMS.", ex);
					Helpers.TrySendErrorMail(ex);
					throw;
				}
			}
		}

		public void Dispose()
		{
			bll.Dispose();
		}
	}
}
