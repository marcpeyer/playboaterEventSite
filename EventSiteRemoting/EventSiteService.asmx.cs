using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;

namespace kcm.ch.EventSite.Remoting
{
	/// <summary>
	/// Summary description for EventSiteService.
	/// </summary>
	[WebService(Namespace="http://kcm.ch/EventSite/Remoting")]
	public class EventSiteService : WebService
	{
		private static string lastErrorMessage = null;

		public EventSiteService()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

		// WEB SERVICE EXAMPLE
		// The HelloWorld() example service returns the string Hello World
		// To build, uncomment the following lines then save and build the project
		// To test this web service, press F5

		[WebMethod]
		public string HelloWorld()
		{
			return "Hello World";
		}

		/// <summary>
		/// Subscribes a user identified with a mobile number to an event.
		/// </summary>
		/// <param name="eventId">The event's id as Integer to subscribe to.</param>
		/// <param name="mobileNumber">The subscriber's mobile number for identification (as String).</param>
		/// <param name="subscriptionStateCode">An Integer that indicates the state of the subscription, where 1 = yes, 2 = later/beer, 3 = no.</param>
		/// <param name="subscriptionTime">The time for the subscription (can be null) (as String).</param>
		/// <param name="comment">Any comment sent by the subscriber (can be null) (as String).</param>
		/// <returns>A Boolean whether the operation was successful or not.</returns>
		[WebMethod]
		public bool AddSubscription(int eventId, string mobileNumber, int subscriptionStateCode, string subscriptionTime, string comment)
		{
			//TODO: remove this special ws input for dani as soon as twsms works live

			bool success;
			try
			{
				RemoteManager rm = new RemoteManager(mobileNumber, eventId);
				success =  rm.AddSubscription(subscriptionStateCode, subscriptionTime, comment);
			}
			catch(Exception ex)
			{
#if(DEBUG)
				lastErrorMessage = ex.ToString();
#else
				lastErrorMessage = ex.Message;
#endif
				return false;
			}

			return success;
		}

//		[WebMethod]
//		public bool SendSms(string senderNumber, string recipientNumber, string text, string mandatorId)
//		{
//			bool success;
//			try
//			{
//				success = RemoteManager.SendSms(senderNumber, recipientNumber, text, mandatorId);
//			}
//			catch(Exception ex)
//			{
//#if(DEBUG)
//				lastErrorMessage = ex.ToString();
//#else
//				lastErrorMessage = ex.Message;
//#endif
//				return false;
//			}
//
//			return success;
//		}

		[WebMethod]
		public string GetLastErrorMessage()
		{
			return lastErrorMessage;
		}
	}
}
