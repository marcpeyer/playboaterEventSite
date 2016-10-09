using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Security;
using kcm.ch.EventSite.Common;
using pbHelpers=playboater.gallery.commons.Helpers;

namespace kcm.ch.EventSite.Web
{
	/// <summary>
	/// Summary description for Login.
	/// </summary>
	public class RemoteLogin : PageBase
	{
		protected System.Web.UI.HtmlControls.HtmlGenericControl pageTitle;
		protected System.Web.UI.WebControls.Label MandatorLabel;
		protected System.Web.UI.WebControls.HyperLink RegisterLink;
		protected System.Web.UI.WebControls.Label Label2;
		protected System.Web.UI.WebControls.Label Label1;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			string customUserIdQryStr = Request.QueryString["cuid"];
			string hashQryStr = Request.QueryString["hash"];
			string contactIdQryStr = Request.QueryString["cid"];
			string eventIdQryStr = Request.QueryString["eid"];
			string remoteSubscribeQryStr = Request.QueryString["rs"];

			if(IsNullOrEmpty(hashQryStr))
			{
				throw new EventSiteException("Invalid Parameters!", 900);
			}

			if(IsNullOrEmpty(customUserIdQryStr))
			{
				//try if there are parameters for remote subscription
				if(IsNullOrEmpty(contactIdQryStr) || IsNullOrEmpty(eventIdQryStr) || IsNullOrEmpty(remoteSubscribeQryStr))
				{
					throw new EventSiteException("Invalid Parameters!", 900);
				}
				else
				{
					LoggerManager.GetLogger().Trace("Remote subscription with parameters: conactId={0} eventId={1}", contactIdQryStr, eventIdQryStr);
					//try to do a remote subscription
					int contactId, eventId;
					bool remoteSubscription;
					try
					{
						contactId = Int32.Parse(contactIdQryStr);
						eventId = Int32.Parse(eventIdQryStr);
						remoteSubscription = Boolean.Parse(remoteSubscribeQryStr);
					}
					catch
					{
						throw new EventSiteException("Invalid Parameters!", 900);
					}

					CheckAndProceedRemoteSubscription(contactId, eventId, hashQryStr, remoteSubscription);
					LoggerManager.GetLogger().Trace("Remote subscription done");
				}
			}
			else
			{
				LoggerManager.GetLogger().Trace("Remote login with params: customUserId={0}", customUserIdQryStr);
				int customUserId; 
				try
				{
					customUserId = int.Parse(customUserIdQryStr);
				}
				catch
				{
					throw new EventSiteException("Invalid Parameters!", 900);
				}

				CheckRemoteLogin(customUserId, hashQryStr);
				LoggerManager.GetLogger().Trace("Remote login done");
			}
		}

		private bool IsNullOrEmpty(string s)
		{
			return (s==null || s.Length==0);
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.RemoteLogin_PreRender);

		}
		#endregion
		
		private static string GetApplicationRootUrl()
		{
			string suffix = EventSiteConfiguration.Current.AppRootUrl.EndsWith("/") ? String.Empty : "/";
			return EventSiteConfiguration.Current.AppRootUrl + suffix;
		}

		private void CheckAndProceedRemoteSubscription(int contactId, int eventId, string hash, bool doRemoteSubscription)
		{
			Contact c = null;
			try
			{
				HttpCookie cookie = BLL.Login(contactId, hash, ref c);
				// Add the cookie to the outgoing cookies collection.
				Response.Cookies.Add(cookie);
			}
			catch(EventSiteException ex)
			{
				RegisterStartupScriptIfNeeded("loginError", ex.JavaScriptAlertString);
			}

			StringBuilder url = new StringBuilder(GetApplicationRootUrl());
			url.AppendFormat("default.aspx?mid={0}&eid={1}", BLL.Mandator.MandatorId, eventId);
				
			try
			{
				if(doRemoteSubscription)
				{
					Event evnt = BLL.GetEvent(eventId);
					DictionaryEntry subscriptionState = BLL.GetDefaultSubscriptionState(evnt.EventCategory);
					Subscription s = new Subscription(evnt, c, (int)subscriptionState.Key, (string)subscriptionState.Value, null, String.Empty, null, null);
					BLL.AddSubscription(s);
				}

				// Redirect the user to the home page
				Response.Redirect(url.ToString());
			}
			catch(EventSiteException ex)
			{
				string redirectScript = String.Format("document.location.href = '{0}';", url.ToString());
				RegisterStartupScriptIfNeeded("loginError", ex.JavaScriptAlertString + String.Format(pbHelpers.JavaScriptString, redirectScript));
			}
		}

		private void CheckRemoteLogin(int customUserId, string hash)
		{
			Label1.Text = "";
			Label2.Text = "";
			try
			{
				string userName = null;
				HttpCookie cookie = BLL.Login(customUserId, hash, userName);
				LoggerManager.GetLogger().Trace("CheckRemoteLogin() BLL.Login succeeded, adding cookie");
				// Add the cookie to the outgoing cookies collection.
				Response.Cookies.Add(cookie);

				// Redirect the user to the home page
				LoggerManager.GetLogger().Trace("Request.ApplicationPath: {0}", Request.ApplicationPath);
				string url = GetApplicationRootUrl();
				url += "default.aspx?mid=" + BLL.Mandator.MandatorId;
				LoggerManager.GetLogger().Trace("redirecting to url: {0}", url);
				Response.Redirect(url);
			}
			catch(EventSiteException ex)
			{
				LoggerManager.GetLogger().TraceException("CheckRemoteLogin() failed", ex);
				RegisterStartupScriptIfNeeded("loginError", ex.JavaScriptAlertString);
			}
		}

		private void RemoteLogin_PreRender(object sender, EventArgs e)
		{
			MandatorLabel.Text = BLL.Mandator.SiteTitle;
			RegisterLink.NavigateUrl = "Register.aspx?mid=" + BLL.Mandator.MandatorId;
		}
	}
}
