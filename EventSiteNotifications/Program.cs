using System;
using kcm.ch.EventSite.BusinessLayer;
using kcm.ch.EventSite.Common;

namespace kcm.ch.EventSite.Notifications
{
	public class Program
	{
		static void Main(string[] args)
		{
			try
			{
				const int baseArgumentsLength = 2;

				if (args.Length < baseArgumentsLength)
				{
					Console.WriteLine("invalid parameters given. (only " + args.Length + ", but at least " + baseArgumentsLength +
					                  " are needed).");
					if (args.Length > 0)
						Console.WriteLine("1st parameter given is: " + args[0]);
					return;
				}

				NotificationOperation operation;
				string mandatorId;

				try
				{
					mandatorId = args[1];
					ConfigurationLoader.Mid = mandatorId.Trim();

					//ConfigurationLoader mandator set. LoggerManager should get correcto xml config now
					LoggerManager.GetLogger().Trace("EventSiteNotifications app started.");

					operation = (NotificationOperation)Enum.Parse(typeof(NotificationOperation), args[0]);
				}
				catch (Exception ex)
				{
					LoggerManager.GetLogger().ErrorException("an error occcured while parsing parameters", ex);
					Console.WriteLine("following error occcured while parsing parameters: {0}", ex);
					return;
				}

				Notification notification = new Notification(mandatorId);

				switch (operation)
				{
					case NotificationOperation.AddEventNotification:
						int addEventId;
						try
						{
							addEventId = Int32.Parse(args[baseArgumentsLength]);
						}
						catch (Exception ex)
						{
							HandleExtendedParameterParsingError(ex);
							return;
						}
						notification.BeginAddEventNotification(addEventId);
						break;
					case NotificationOperation.EditEventNotification:
						int editEventId;
						try
						{
							editEventId = Int32.Parse(args[baseArgumentsLength]);
						}
						catch (Exception ex)
						{
							HandleExtendedParameterParsingError(ex);
							return;
						}
						notification.BeginEditEventNotification(editEventId);
						break;
					case NotificationOperation.AddSubscriptionNotification:
						int addSubscrId;
						try
						{
							addSubscrId = Int32.Parse(args[baseArgumentsLength]);
						}
						catch (Exception ex)
						{
							HandleExtendedParameterParsingError(ex);
							return;
						}
						notification.BeginAddSubscriptionNotification(addSubscrId);
						break;
					case NotificationOperation.EditSubscriptionNotification:
						int editSubscrId;
						try
						{
							editSubscrId = Int32.Parse(args[baseArgumentsLength]);
						}
						catch (Exception ex)
						{
							HandleExtendedParameterParsingError(ex);
							return;
						}
						notification.BeginEditSubscriptionNotification(editSubscrId);
						break;
					case NotificationOperation.DelSubscriptionNotification:
						int delSubscrId;
						try
						{
							delSubscrId = Int32.Parse(args[baseArgumentsLength]);
						}
						catch (Exception ex)
						{
							HandleExtendedParameterParsingError(ex);
							return;
						}
						notification.BeginDelSubscriptionNotification(delSubscrId);
						break;
					case NotificationOperation.JourneyChangeNotification:
						int journeySubscrId;
						try
						{
							journeySubscrId = Int32.Parse(args[baseArgumentsLength]);
						}
						catch (Exception ex)
						{
							HandleExtendedParameterParsingError(ex);
							return;
						}
						notification.BeginJourneyChangeNotification(journeySubscrId);
						break;
					case NotificationOperation.LiftSaveNotification:
						string action;
						string definition;
						int eventId;
						int contactIdToNotify;
						int liftContactId;
						try
						{
							action = args[baseArgumentsLength];
							definition = args[baseArgumentsLength + 1];
							eventId = Int32.Parse(args[baseArgumentsLength + 2]);
							contactIdToNotify = Int32.Parse(args[baseArgumentsLength + 3]);
							liftContactId = Int32.Parse(args[baseArgumentsLength + 4]);
						}
						catch (Exception ex)
						{
							HandleExtendedParameterParsingError(ex);
							return;
						}
						notification.BeginLiftSaveNotification(action, definition, eventId, contactIdToNotify, liftContactId);
						break;
					default:
						throw new NotSupportedException(String.Format("Unknown NotificationOperation given {0}", operation));
				}
			}
			catch(Exception ex)
			{
				LoggerManager.GetLogger().ErrorException("Exception occured in EventSiteNotifications Program.cs:", ex);
				Console.WriteLine("Exception occured in EventSiteNotifications Program.cs: {0}", ex);
			}
		}

		private static void HandleExtendedParameterParsingError(Exception ex)
		{
			Console.WriteLine("following error occured while parsing extended parameters: {0}", ex);
			LoggerManager.GetLogger().Error("following error occured while parsing extended parameters: {0}", ex);
		}
	}
}