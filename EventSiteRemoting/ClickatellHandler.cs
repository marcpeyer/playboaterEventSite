using System;
using System.Collections.Generic;
using System.Text;
using kcm.ch.EventSite.Common;
using playboater.gallery.ClickatellApi;

namespace kcm.ch.EventSite.Remoting
{
	public class ClickatellHandler : IClickatellHandler
	{
		public void HandleInbound(Inbound inbound)
		{
			try
			{
				LoggerManager.GetLogger().Trace("Handling new inbound: \"{0}\"", inbound.Text);

				MessageParser parser = new MessageParser(inbound.Text);
				LoggerManager.GetLogger().Trace("Inbound message parsed. Detected operation: {0}", parser.Operation.GetType().Name);
				ExecuteOperation(inbound.From, parser.Operation, parser.CleanMessage);
			}
			catch (Exception ex)
			{
				LoggerManager.GetLogger().ErrorException("Error occured handling inbound message!", ex);

				try
				{
					using (RemoteManager mgr = new RemoteManager(inbound.From))
					{
						mgr.SendSms(String.Format("Fehler beim Bearbeiten des SMS: {0}\r\n{1}", ex.Message, Constants.HelpMsg));
					}
				}
				catch(Exception ex2)
				{
					LoggerManager.GetLogger().ErrorException("Error while notifying user about failure.", ex2);
				}
			}
		}

		private static void ExecuteOperation(string sender, Operation operation, string cleanMessage)
		{
			if(operation is HelpOperation)
			{
				using(RemoteManager mgr = new RemoteManager(sender))
				{
					try
					{
						mgr.SendSms(Constants.HelpMsg);
					}
					catch(Exception ex)
					{
						HandleExecuteOperationException(operation, ex);
					}
				}
			}
			else if (operation is SmsConfigOperation)
			{
				SmsConfigOperation op = (SmsConfigOperation)operation;
				using (RemoteManager mgr = new RemoteManager(sender))
				{
					try
					{
						KeyParameter<bool> smsConfigParam = (KeyParameter<bool>) ((ParameterOrGroup) op.Parameters[0]).GetParameterSet();
						mgr.ConfigureSms(smsConfigParam.Value);

						mgr.SendSuccessSms("SMS Konfiguration erfolgreich gespeichert.");
					}
					catch (Exception ex)
					{
						HandleExecuteOperationException(op, ex);
						mgr.SendSms("Fehler beim setzen der SMS Konfiguration: " + ex.Message);
					}
				}
			}
			else if (operation is EventInfoOperation)
			{
				EventInfoOperation op = (EventInfoOperation) operation;
				try
				{
					int eventId = ((ValueParameter<int>) op.Parameters[1]).Value;

					using (RemoteManager mgr = new RemoteManager(sender, eventId))
					{
						try
						{
							KeyParameter<string> eventInfoParam =
								(KeyParameter<string>) ((ParameterOrGroup) op.Parameters[0]).GetParameterSet();
							switch (eventInfoParam.Value)
							{
								case "TEILN":
									List<Subscription> subscriptions = mgr.ListSubscriptions();
									string smsText;
									if (subscriptions.Count > 0)
									{
										StringBuilder sb =
											new StringBuilder(String.Format("Eintragungen für \"{0}\":\r\n", subscriptions[0].Event.EventTitle));
										foreach (Subscription subscription in subscriptions)
										{
											sb.AppendLine(String.Format("{0} '{1}'", subscription.Contact.Name, subscription.SubscriptionStateText));
										}
										smsText = sb.ToString();
									}
									else
									{
										smsText = "Dieser Anlass hat zur Zeit noch keine Eintragungen.";
									}
									mgr.SendSms(smsText);
									break;
								default:
									throw new ArgumentException("Unknown EventInfo parameter given", eventInfoParam.Value);
							}
						}
						catch(Exception ex)
						{
							HandleExecuteOperationException(op, ex);
							mgr.SendSms(String.Format("Fehler beim ermitteln der Anlass Information: {0}", ex.Message));
						}
					}
				}
				catch (Exception ex)
				{
					HandleExecuteOperationException(op, ex);
					try
					{
						using (RemoteManager mgr = new RemoteManager(sender))
						{
							mgr.SendSms(String.Format("Fehler beim ermitteln der Anlass Information: {0}", ex.Message));
						}
					}
					catch (Exception ex2)
					{
						LoggerManager.GetLogger().ErrorException("Exception while sending a failure sms to the sender:", ex2);
					}
				}
			}
			else if (operation is EventSubscriptionOperation)
			{
				EventSubscriptionOperation op = (EventSubscriptionOperation)operation;
				try
				{
					using (RemoteManager mgr = new RemoteManager(sender, ((ValueParameter<int>) op.Parameters[1]).Value))
					{
						try
						{
							KeyParameter<int> subscrStateCode = (KeyParameter<int>) ((ParameterOrGroup) op.Parameters[0]).GetParameterSet();
							PrefixValueParameter<string> subscrTime = (PrefixValueParameter<string>) op.Parameters[2];
							if (mgr.AddSubscription(subscrStateCode.Value, subscrTime.Value, cleanMessage))
							{
								mgr.SendSuccessSms("Eintragung erfolgreich erstellt.");
							}
							else
							{
								mgr.SendSms("Fehler beim eintragen.");
							}
						}
						catch(Exception ex)
						{
							HandleExecuteOperationException(op, ex);
							mgr.SendSms(String.Format("Fehler beim eintragen: {0}", ex.Message));
						}
					}
				}
				catch(Exception ex)
				{
					HandleExecuteOperationException(op, ex);
					try
					{
						using (RemoteManager mgr = new RemoteManager(sender))
						{
							mgr.SendSms(String.Format("Fehler beim eintragen: {0}", ex.Message));
						}
					}
					catch (Exception ex2)
					{
						LoggerManager.GetLogger().ErrorException("Exception while sending a failure sms to the sender:", ex2);
					}
				}
			}
			else
			{
				throw new InvalidOperationException(String.Format("Given Operation '{0}' is unknown!", operation.GetType().Name));
			}
		}

		private static void HandleExecuteOperationException(Operation operation, Exception ex)
		{
			LoggerManager.GetLogger().ErrorException(String.Format("Error occured while executing {0} operation:", operation.GetType().Name), ex);
		}

		public void HandleCallback(Callback callback)
		{
			throw new NotImplementedException();
		}
	}
}