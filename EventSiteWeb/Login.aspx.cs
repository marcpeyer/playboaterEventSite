using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using kcm.ch.EventSite.Common;

namespace kcm.ch.EventSite.Web
{
	/// <summary>
	/// Summary description for Login.
	/// </summary>
	public class Login : PageBase
	{
		protected System.Web.UI.HtmlControls.HtmlGenericControl pageTitle;
		protected System.Web.UI.WebControls.TextBox LoginBox;
		protected System.Web.UI.WebControls.TextBox PasswordBox;
		protected System.Web.UI.WebControls.Label MandatorLabel;
		protected System.Web.UI.WebControls.HyperLink RegisterLink;
		protected System.Web.UI.WebControls.Button LoginButton;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			if(!IsPostBack)
			{
				IPrincipal u = HttpContext.Current.User;
				if(u != null && u.Identity != null && u.Identity.IsAuthenticated)
				{
					Response.Redirect("AccessDenied.aspx?mid=" + BLL.Mandator.MandatorId, true);
				}
			}
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
			this.LoginButton.Click += new System.EventHandler(this.LoginButton_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Login_PreRender);

		}
		#endregion

		private void LoginButton_Click(object sender, System.EventArgs e)
		{
			try
			{
				HttpCookie cookie = BLL.Login(LoginBox.Text, PasswordBox.Text);
				// Add the cookie to the outgoing cookies collection.
				Response.Cookies.Add(cookie);

				// Redirect the user to the originally requested page
				string url = FormsAuthentication.GetRedirectUrl(LoginBox.Text, false);
				if(url.IndexOf("mid=") == -1)
				{
					if(url.EndsWith(".aspx"))
					{
						url += "?";
					}
					url += "&mid=" + BLL.Mandator.MandatorId;
				}
				Response.Redirect(url);
			}
			catch(EventSiteException ex)
			{
				RegisterStartupScriptIfNeeded("loginError", ex.JavaScriptAlertString);
			}
		}

		private void Login_PreRender(object sender, EventArgs e)
		{
			MandatorLabel.Text = BLL.Mandator.SiteTitle;
			RegisterLink.NavigateUrl = "Register.aspx?mid=" + BLL.Mandator.MandatorId;
		}
	}
}
