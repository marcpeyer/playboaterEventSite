using System;
using kcm.ch.EventSite.BusinessLayer;
using kcm.ch.EventSite.Common;
using Nito.AspNetBackgroundTasks;

namespace kcm.ch.EventSite.BusinessLayer.Notifications
{
	public static class NotificationStarter
	{
		public static void StartAddEventNotification(string mandatorId, int eventId)
		{
			LoggerManager.GetLogger().Trace("Calling async method to perform notifications");
			BackgroundTaskManager.Run(() =>
			{
				try
				{
					LoggerManager.GetLogger().Trace("EventSite notifications started.");
					Notification notification = new Notification(mandatorId.ToString());

					notification.BeginAddEventNotification(eventId);
				}
				catch (Exception ex)
				{
					LoggerManager.GetLogger().ErrorException("Error occured while executing async notification for AddEvent", ex);
				}
			});
			LoggerManager.GetLogger().Trace("Called async method to perform notifications");
		}

		public static void StartEditEventNotification(string mandatorId, int eventId)
		{
			LoggerManager.GetLogger().Trace("Calling async method to perform notifications");
			BackgroundTaskManager.Run(() =>
			{
				try
				{
					LoggerManager.GetLogger().Trace("EventSite notifications started.");
					Notification notification = new Notification(mandatorId.ToString());

					notification.BeginEditEventNotification(eventId);
				}
				catch (Exception ex)
				{
					LoggerManager.GetLogger().ErrorException("Error occured while executing async notification for EditEvent", ex);
				}
			});
			LoggerManager.GetLogger().Trace("Called async method to perform notifications");
		}

		static void Main(string mandatorId, NotificationOperation operation, int? eventId, int? subscrId, int? journeySubscrId,
			string action, string definition, int? contactIdToNotify, int? liftContactId)
		{
			try
			{
				LoggerManager.GetLogger().Trace("EventSite notifications started.");
				Notification notification = new Notification(mandatorId);

				switch (operation)
				{
					case NotificationOperation.AddEventNotification:
						if (eventId.HasValue)
						{
							notification.BeginAddEventNotification(eventId.Value);
						}
						else
						{
							//TODO: HANDLE
							return;
						}
						break;
					case NotificationOperation.EditEventNotification:
						if (eventId.HasValue)
						{
							notification.BeginEditEventNotification(eventId.Value);
						}
						else
						{
							//TODO: HANDLE
							return;
						}
						break;
					case NotificationOperation.AddSubscriptionNotification:
						if(subscrId.HasValue)
						{
							notification.BeginAddSubscriptionNotification(subscrId.Value);
						}
						else
						{
							//TODO: HANDLE
							return;
						}
						break;
					case NotificationOperation.EditSubscriptionNotification:
						if (subscrId.HasValue)
						{
							notification.BeginEditSubscriptionNotification(subscrId.Value);
						}
						else
						{
							//TODO: HANDLE
							return;
						}
						break;
					case NotificationOperation.DelSubscriptionNotification:
						if (subscrId.HasValue)
						{
							notification.BeginDelSubscriptionNotification(subscrId.Value);
						}
						else
						{
							//TODO: HANDLE
							return;
						}
						break;
					case NotificationOperation.JourneyChangeNotification:
						if (journeySubscrId.HasValue)
						{
							notification.BeginJourneyChangeNotification(journeySubscrId.Value);
						}
						else
						{
							//TODO: HANDLE
							return;
						}
						break;
					case NotificationOperation.LiftSaveNotification:
						if(!String.IsNullOrEmpty(action) && !String.IsNullOrEmpty(definition) &&
							eventId.HasValue && contactIdToNotify.HasValue && liftContactId.HasValue)
						{
							notification.BeginLiftSaveNotification(action, definition, eventId.Value, contactIdToNotify.Value, liftContactId.Value);
						}
						else
						{
							//TODO: HANDLE
							return;
						}
						break;
					default:
						throw new NotSupportedException(String.Format("Unknown NotificationOperation given {0}", operation));
				}
			}
			catch(Exception ex)
			{
				LoggerManager.GetLogger().ErrorException("Exception occured in EventSiteNotifications NotificationStarter.cs:", ex);
				Console.WriteLine("Exception occured in EventSiteNotifications NotificationStarter.cs: {0}", ex);
			}
		}

		private static void HandleExtendedParameterParsingError(Exception ex)
		{
			Console.WriteLine("following error occured while parsing extended parameters: {0}", ex);
			LoggerManager.GetLogger().Error("following error occured while parsing extended parameters: {0}", ex);
		}
	}
}