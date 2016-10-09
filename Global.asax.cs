using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mail;
using System.Web.Security;
using System.Web.SessionState;
using kcm.ch.EventSite.Common;

namespace kcm.ch.EventSite.Web 
{
	/// <summary>
	/// Summary description for Global.
	/// </summary>
	public class Global : System.Web.HttpApplication
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		public Global()
		{
			InitializeComponent();
		}	
		
		protected void Application_Start(Object sender, EventArgs e)
		{
		}
 
		protected void Session_Start(Object sender, EventArgs e)
		{
		}

		protected void Application_BeginRequest(Object sender, EventArgs e)
		{
			if(Path.GetFileName(Request.Url.LocalPath) != "MaintenanceMode.aspx"
				&& EventSiteConfiguration.Current.MaintenanceMode)
			{
				Response.Redirect(String.Format("MaintenanceMode.aspx?{0}&tit=Wartungsmodus&txt=Die Event Site befindet sich momentan im Wartungsmodus. Bitte in ein paar Minuten erneut versuchen.", Request.Url.Query).Replace("??", "?"), true);
			}
		}

		protected void Application_EndRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{
			// Extract the forms authentication cookie
			string cookieName = FormsAuthentication.FormsCookieName;
			HttpCookie authCookie = Context.Request.Cookies[cookieName];

			if(null == authCookie)
			{
				// There is no authentication cookie.
				return;
			}

			FormsAuthenticationTicket authTicket = null;
			authTicket = FormsAuthentication.Decrypt(authCookie.Value);

			if (null == authTicket)
			{
				// Cookie failed to decrypt.
				return; 
			}

			// When the ticket was created, the UserData property was assigned a
			// pipe delimited string of role names.
			string[] roles = authTicket.UserData.Split(new char[]{'|'});

			// Create an Identity object
			FormsIdentity id = new FormsIdentity(authTicket);

			string mandQryStr = Request.QueryString["mid"];
			if(!String.IsNullOrEmpty(mandQryStr))
			{
				if(id.Name.Split('|').Length == 2)
				{
					if(!id.Name.Split('|')[1].Equals(mandQryStr))
					{
						//mandator has changed on the fly --> logout
						FormsAuthentication.SignOut();
						return;
					}
				}
				else
				{
					//invalid id.Name property --> logout
					FormsAuthentication.SignOut();
					return;
				}
			}


			// This principal will flow throughout the request.
			GenericPrincipal principal = new GenericPrincipal(id, roles);
			// Attach the new principal object to the current HttpContext object
			Context.User = principal;
		}

		/// <summary>
		/// Callback when an exception was thrown. 
		/// </summary>
		/// <param name="sender">sender of event</param>
		/// <param name="e">event arguments</param>
		protected void Application_Error(Object sender, EventArgs e) 
		{

			Exception ex = Server.GetLastError();
			Server.ClearError();

			// unwrap once in any case
			if (ex.InnerException != null) ex = ex.InnerException;

			// now send a mail
			string msg = "";

			LoggerManager.GetLogger().ErrorException("EventSite: Application Error", ex);
			if(Helpers.TrySendErrorMail(ex))
			{
				msg += "Der Webmaster wurde per E-Mail informiert.";
			}
			else
			{
				msg += "Es konnte kein E-Mail an den Webmaster geschickt werden. Bitte Fehlermeldung mailen an: webmaster@playboater.ch";
			}

			// write error message
			Response.Write("<p style=\"color:red;\">EventSite - Es ist ein unerwarteter Fehler aufgetreten!<br><br>" + msg + "<br><br>Fehlerdetails:<br>" + ex.ToString() + "</p>");
		}

		protected void Session_End(Object sender, EventArgs e)
		{

		}

		protected void Application_End(Object sender, EventArgs e)
		{

		}
			
		#region Web Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.components = new System.ComponentModel.Container();
		}
		#endregion
	}
}

