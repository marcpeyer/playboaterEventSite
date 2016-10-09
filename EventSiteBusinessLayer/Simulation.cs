using System;
using System.IO;
using System.Threading;
using kcm.ch.EventSite.Common;
using playboater.gallery.commons;

namespace kcm.ch.EventSite.BusinessLayer
{
	/// <summary>
	/// Summary description for Simulation.
	/// </summary>
	public class Simulation
	{
		private const string baseDir = "C:\\temp\\EventSiteSimulation\\";
		private static readonly object simulationLock = new object();

		public static void SimulateSms(string message, string sender, string recipientNumber)
		{
			try
			{
				lock (simulationLock)
				{
					string destinationDir = baseDir + "SMS\\" + recipientNumber;
					if (!Directory.Exists(destinationDir))
					{
						Directory.CreateDirectory(destinationDir);
					}

					TextWriter writer = File.CreateText(destinationDir + "\\Msg" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + DateTime.Now.Millisecond + ".txt");
					writer.WriteLine("SMS From: {0}", sender);
					writer.WriteLine("To: {0}", recipientNumber);
					writer.WriteLine("Date: {0}", DateTime.Now);
					writer.WriteLine("-------------------------");
					writer.WriteLine(message);
					writer.Flush();
					writer.Close();

					Thread.Sleep(1000);
				}
			}
			catch (Exception ex)
			{
				LoggerManager.GetLogger().ErrorException("Exception while SimulateSms()", ex);
			}
		}

		public static void SimulateEmail(string fromAddress, string toAddresses, string subject, string body, EmailMessage.EmailMessageFormat mailFormat)
		{
			try
			{
				lock (simulationLock)
				{
					foreach (string toAddress in toAddresses.Split(';'))
					{
						if (!toAddress.Trim().Equals(string.Empty))
						{
							string destinationDir = baseDir + "Email\\" + toAddress.Replace("@", "AT");
							if (!Directory.Exists(destinationDir))
							{
								Directory.CreateDirectory(destinationDir);
							}

							TextWriter writer = File.CreateText(destinationDir + "\\Msg" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + DateTime.Now.Millisecond + (mailFormat == EmailMessage.EmailMessageFormat.Html ? ".htm" : ".txt"));
							if (mailFormat == EmailMessage.EmailMessageFormat.Html)
							{
								writer.WriteLine("<html><body>");
								writer.WriteLine("<p>Email From: {0}<br>", fromAddress);
								writer.WriteLine("To: {0}<br>", toAddress);
								writer.WriteLine("Date: {0}<br>", DateTime.Now);
								writer.WriteLine("Subject: {0}<br>", subject);
								writer.WriteLine("-------------------------<br></p>");
								writer.WriteLine("<p>{0}</p>", body);
								writer.WriteLine("</body></html>");
							}
							else
							{
								writer.WriteLine("Email From: {0}", fromAddress);
								writer.WriteLine("To: {0}", toAddress);
								writer.WriteLine("Date: {0}", DateTime.Now);
								writer.WriteLine("Subject: {0}", subject);
								writer.WriteLine("-------------------------");
								writer.WriteLine(body);
							}
							writer.Flush();
							writer.Close();

							Thread.Sleep(1000);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LoggerManager.GetLogger().ErrorException("Exception while SimulateEmail()", ex);
			}
		}
	}
}