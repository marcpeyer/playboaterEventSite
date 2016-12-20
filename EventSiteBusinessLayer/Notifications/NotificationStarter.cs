using System;
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
					Notification notification = new Notification(mandatorId);
					notification.BeginAddEventNotification(eventId);
				}
				catch (Exception ex)
				{
					LoggerManager.GetLogger().ErrorException($"Error occured while executing async notification for {nameof(StartAddEventNotification)}", ex);
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
					Notification notification = new Notification(mandatorId);
					notification.BeginEditEventNotification(eventId);
				}
				catch (Exception ex)
				{
					LoggerManager.GetLogger().ErrorException($"Error occured while executing async notification for {nameof(StartEditEventNotification)}", ex);
				}
			});
			LoggerManager.GetLogger().Trace("Called async method to perform notifications");
		}

		public static void StartAddSubscriptionNotification(string mandatorId, int subscriptionId)
		{
			LoggerManager.GetLogger().Trace("Calling async method to perform notifications");
			BackgroundTaskManager.Run(() =>
			{
				try
				{
					LoggerManager.GetLogger().Trace("EventSite notifications started.");
					Notification notification = new Notification(mandatorId);
					notification.BeginAddSubscriptionNotification(subscriptionId);
				}
				catch (Exception ex)
				{
					LoggerManager.GetLogger().ErrorException($"Error occured while executing async notification for {nameof(StartAddSubscriptionNotification)}", ex);
				}
			});
			LoggerManager.GetLogger().Trace("Called async method to perform notifications");
		}

		public static void StartEditSubscriptionNotification(string mandatorId, int subscriptionId)
		{
			LoggerManager.GetLogger().Trace("Calling async method to perform notifications");
			BackgroundTaskManager.Run(() =>
			{
				try
				{
					LoggerManager.GetLogger().Trace("EventSite notifications started.");
					Notification notification = new Notification(mandatorId);
					notification.BeginEditSubscriptionNotification(subscriptionId);
				}
				catch (Exception ex)
				{
					LoggerManager.GetLogger().ErrorException($"Error occured while executing async notification for {nameof(StartEditSubscriptionNotification)}", ex);
				}
			});
			LoggerManager.GetLogger().Trace("Called async method to perform notifications");
		}

		public static void StartDelSubscriptionNotification(string mandatorId, int subscriptionId)
		{
			LoggerManager.GetLogger().Trace("Calling async method to perform notifications");
			BackgroundTaskManager.Run(() =>
			{
				try
				{
					LoggerManager.GetLogger().Trace("EventSite notifications started.");
					Notification notification = new Notification(mandatorId);
					notification.BeginDelSubscriptionNotification(subscriptionId);
				}
				catch (Exception ex)
				{
					LoggerManager.GetLogger().ErrorException($"Error occured while executing async notification for {nameof(StartDelSubscriptionNotification)}", ex);
				}
			});
			LoggerManager.GetLogger().Trace("Called async method to perform notifications");
		}

		public static void StartJourneyChangeNotification(string mandatorId, int journeySubscriptionId)
		{
			LoggerManager.GetLogger().Trace("Calling async method to perform notifications");
			BackgroundTaskManager.Run(() =>
			{
				try
				{
					LoggerManager.GetLogger().Trace("EventSite notifications started.");
					Notification notification = new Notification(mandatorId);
					notification.BeginJourneyChangeNotification(journeySubscriptionId);
				}
				catch (Exception ex)
				{
					LoggerManager.GetLogger().ErrorException($"Error occured while executing async notification for {nameof(StartJourneyChangeNotification)}", ex);
				}
			});
			LoggerManager.GetLogger().Trace("Called async method to perform notifications");
		}

		public static void StartLiftSaveNotification(string mandatorId, string action, string definition, int eventId, int contactIdToNotify, int liftContactId)
		{
			LoggerManager.GetLogger().Trace("Calling async method to perform notifications");
			BackgroundTaskManager.Run(() =>
			{
				try
				{
					LoggerManager.GetLogger().Trace("EventSite notifications started.");
					Notification notification = new Notification(mandatorId);
					notification.BeginLiftSaveNotification(action, definition, eventId, contactIdToNotify, liftContactId);
				}
				catch (Exception ex)
				{
					LoggerManager.GetLogger().ErrorException($"Error occured while executing async notification for {nameof(StartLiftSaveNotification)}", ex);
				}
			});
			LoggerManager.GetLogger().Trace("Called async method to perform notifications");
		}
	}
}