using System;
using kcm.ch.EventSite.Common;
using pbHelpers=playboater.gallery.commons.Helpers;

namespace kcm.ch.EventSite.Web
{
	/// <summary>
	/// Summary description for Register.
	/// </summary>
	public class SendPass : PageBase
	{
		protected System.Web.UI.WebControls.TextBox Email;
		protected System.Web.UI.HtmlControls.HtmlGenericControl pageTitle;
		protected System.Web.UI.WebControls.Button SendButton;
		protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator2;
		protected System.Web.UI.WebControls.HyperLink LoginLink;

		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			LoginLink.NavigateUrl = "Login.aspx?mid=" + BLL.Mandator.MandatorId;
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
			this.SendButton.Click += new System.EventHandler(this.SendButton_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new EventHandler(Register_PreRender);
		}
		#endregion

		private void SendButton_Click(object sender, System.EventArgs e)
		{
			if(IsValid)
			{
				if(!pbHelpers.ValidateEmail(Email.Text.Trim()))
				{
					RegisterStartupScriptIfNeeded("emailInvalid", string.Format(pbHelpers.JavaScriptAlertString, "Die angegebene Email-Adresse ist ungültig!"));
					return;
				}

				try
				{
					Contact c = BLL.GetContact(Email.Text.Trim());
					bool isSuccess = BLL.SendPassword(c);
					string userInfo = isSuccess ? "Email erfolgreich gesendet" : "Fehler beim senden des Passworts!";

					RegisterStartupScriptIfNeeded("userInfo", string.Format(pbHelpers.JavaScriptAlertString, userInfo));
				}
				catch(EventSiteException ex)
				{
					RegisterStartupScriptIfNeeded("addExc", ex.JavaScriptAlertString);
				}
			}
		}

		private void Register_PreRender(object sender, EventArgs e)
		{
		}
	}
}
