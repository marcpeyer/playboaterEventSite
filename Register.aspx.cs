using System;
using kcm.ch.EventSite.Common;
using pbHelpers=playboater.gallery.commons.Helpers;

namespace kcm.ch.EventSite.Web
{
	/// <summary>
	/// Summary description for Register.
	/// </summary>
	public class Register : PageBase
	{
		protected System.Web.UI.WebControls.TextBox Name;
		protected System.Web.UI.WebControls.TextBox Email;
		protected System.Web.UI.WebControls.TextBox MobilePhone;
		protected System.Web.UI.HtmlControls.HtmlGenericControl pageTitle;
		protected System.Web.UI.WebControls.Button RegisterButton;
		protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator1;
		protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator2;
		protected System.Web.UI.WebControls.HyperLink LoginLink;
		protected System.Web.UI.WebControls.CheckBox LiftMgmtSmsOn;
		protected System.Web.UI.WebControls.CheckBox EventMgmtSmsOn;
		protected System.Web.UI.HtmlControls.HtmlTableRow LiftMgmtRow;
		protected System.Web.UI.HtmlControls.HtmlTableRow EventMgmtRow;
		protected System.Web.UI.HtmlControls.HtmlGenericControl SendPassLink;

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
			this.RegisterButton.Click += new System.EventHandler(this.RegisterButton_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new EventHandler(Register_PreRender);
		}
		#endregion

		private void RegisterButton_Click(object sender, System.EventArgs e)
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
					Contact c = new Contact(BLL.Mandator, Name.Text, Email.Text, MobilePhone.Text, LiftMgmtSmsOn.Checked, EventMgmtSmsOn.Checked, false, 0, 0, false, "");

					string userInfo = BLL.AddContact(c);
					if(userInfo != null)
					{
						RegisterStartupScriptIfNeeded("userInfo", string.Format(pbHelpers.JavaScriptAlertString, userInfo));
					}
				}
				catch(EventSiteException ex)
				{
					RegisterStartupScriptIfNeeded("addExc", ex.JavaScriptAlertString);
				}
			}
		}

		private void Register_PreRender(object sender, EventArgs e)
		{
			LiftMgmtRow.Visible = BLL.Mandator.IsLiftManagementEnabled;
			EventMgmtRow.Visible = BLL.Mandator.SmsNotifications;
			SendPassLink.Visible = !BLL.Mandator.UseExternalAuth;
		}
	}
}
