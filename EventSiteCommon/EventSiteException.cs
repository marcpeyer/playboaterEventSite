using System;
using pbHelpers=playboater.gallery.commons.Helpers;

namespace kcm.ch.EventSite.Common
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class EventSiteException : Exception
	{
		public int ErrorNumber;
		private readonly string javaScriptAlertString;

		public EventSiteException(string message, int errorNumber):base(message)
		{
			ErrorNumber = errorNumber;
			javaScriptAlertString = String.Format(pbHelpers.JavaScriptAlertString, pbHelpers.javascriptEncode(message));
		}

		public EventSiteException(string message, int errorNumber, Exception innerException):base(message, innerException)
		{
			ErrorNumber = errorNumber;
			javaScriptAlertString = String.Format(pbHelpers.JavaScriptAlertString, pbHelpers.javascriptEncode(message));
		}

		public string JavaScriptAlertString
		{
			get
			{
				return javaScriptAlertString;
			}
		}
	}
}
