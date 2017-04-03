using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mail;
using playboater.gallery.commons;
using pbHelpers = playboater.gallery.commons.Helpers;

namespace kcm.ch.EventSite.Common
{
	/// <summary>
	/// Summary description for Helpers.
	/// </summary>
	public class Helpers
	{

		/// <summary>
		/// Build an eeror description, displaying form variables, requestor, Url and
		/// exception.
		/// </summary>
		/// <param name="ex">thrown exception</param>
		/// <returns>the error description</returns>
		public static string ErrorDescription(Exception ex) 
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("Server: " + HttpContext.Current.Server.MachineName + "\n");
			sb.Append("Current User: " + HttpContext.Current.User.Identity.Name + "\n");
			sb.Append("Request Url:  " + HttpContext.Current.Request.RawUrl + "\n");
			sb.AppendFormat("Request Ip:  {0} - http://ipinfo.io/{0}\n", GetIp());
			sb.Append("Form variables:\n");
			foreach (string key in HttpContext.Current.Request.Form) 
			{
				if (key != "__VIEWSTATE") 
				{
					sb.Append("  ");
					sb.Append(key);
					sb.Append(":\t");
					sb.Append(HttpContext.Current.Request.Form[key]);
					sb.Append("\n");
				}
			}
			sb.Append("\n");
			sb.Append(ex.ToString());
			return sb.ToString();
		}

		public static string GetIp()
		{
			string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

			if (String.IsNullOrEmpty(ip))
			{
				ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
			}

			return ip;
		}

		public static bool TrySendErrorMail(Exception ex)
		{
			try
			{
				string mailRecipient = EventSiteConfiguration.Current.ErrorHandlingConfiguration.MailRecipient;

				if (mailRecipient != null) 
				{
					string subject = "EventSite: Application Error";
					string smtpServer = EventSiteConfiguration.Current.MailConfiguration.SmtpServer;
					SmtpMail.SmtpServer = smtpServer;
					bool smtpUseSSL = EventSiteConfiguration.Current.MailConfiguration.UseSSL;
					string smtpPass = EventSiteConfiguration.Current.MailConfiguration.SmtpPass;
					EmailMessage email = (!String.IsNullOrEmpty(smtpPass)
						? new EmailMessage(smtpServer, smtpUseSSL, EventSiteConfiguration.Current.MailConfiguration.SmtpPort, EventSiteConfiguration.Current.MailConfiguration.SmtpUser, smtpPass)
						: new EmailMessage(smtpServer));
					email.SendMail("EventSite Error", mailRecipient, mailRecipient, subject, ErrorDescription(ex), EmailMessage.EmailMessageFormat.Text);
				}

				return true;
			}
			catch
			{
				return false;
			}
		}

		public static string GetRemoteSubscrHash(int contactId, DateTime date, string mandatorId)
		{
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			return  pbHelpers.ToHexString(md5.ComputeHash(Encoding.Default.GetBytes(mandatorId + contactId + "_RemoteSubscription_" + date.ToString("dd.MM.yyyy HH:mm"))));
		}
	}
}
