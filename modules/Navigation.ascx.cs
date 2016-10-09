using System;
using System.Drawing;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using kcm.ch.EventSite.BusinessLayer;
using playboater.gallery.commons;
using playboater.gallery.webcontrols;

namespace kcm.ch.EventSite.Web.modules
{
	/// <summary>
	///		Summary description for Navigation.
	/// </summary>
	public class Navigation : UserControl
	{
		protected ExtendedHyperLink ContactManagementLink;
		protected ExtendedHyperLink HomeLink;
		protected ExtendedHyperLink HelpLink;
		protected ExtendedHyperLink AboutLink;
		protected ExtendedHyperLink MyDetailsLink;
		protected LinkButton LogoutButton;
		protected HtmlGenericControl HomeContainer;
		protected HtmlGenericControl ContactMgtContainer;
		protected HtmlGenericControl EventMgtContainer;
//		protected HtmlGenericControl NewEventContainer;
		protected HtmlGenericControl HelpContainer;
		protected HtmlGenericControl MyDetailsContainer;
		protected HtmlGenericControl AboutContainer;
		protected ExtendedHyperLink EventManagementLink;
//		protected ExtendedHyperLink NewEventLink;
		private EventSiteBL bll;

		private void Page_Load(object sender, EventArgs e)
		{
			bll = ((PageBase)base.Page).BLL;
		}

		private void LogoutButton_Click(object sender, System.EventArgs e)
		{
			FormsAuthentication.SignOut();
			Response.Redirect("Login.aspx?mid=" + bll.Mandator.MandatorId);
		}

		private void Navigation_PreRender(object sender, EventArgs e)
		{
			string pagename = base.Page.Request.FilePath;
			pagename = pagename.Substring(pagename.LastIndexOf('/')+1).ToLower();
//			((PageBase)base.Page).RegisterStartupScriptIfNeeded("sitename", string.Format(Helpers.JavaScriptAlertString, pagename));
			ExtendedHyperLink[] links = new ExtendedHyperLink[] {HomeLink, ContactManagementLink, /*NewEventLink,*/ EventManagementLink, MyDetailsLink, HelpLink, AboutLink};
			foreach (ExtendedHyperLink link in links)
			{
				if(link.BaseLinkPage.ToLower() == pagename)
				{
//					if(link.ID == "NewEventLink")
//					{
//						NewEventContainer.Visible = false;
//						continue;
//					}
					link.NavigateUrl = "";
					link.ForeColor = Color.Black;
					link.Style.Add("text-decoration", "none");
				}
				else
				{
					link.NavigateUrl = "../" + link.BaseLinkPage + "?mid=" + bll.Mandator.MandatorId;
					if(link.ID == "NewEventLink")
					{
						link.NavigateUrl += "&mode=new";
					}
				}
			}

			HelpContainer.Visible = false;
			HomeContainer.Visible = !(bll.IsEventCreator() && !bll.IsUser() && !bll.IsReader() && !bll.IsManager());
			ContactMgtContainer.Visible = (bll.IsUser() || bll.IsManager() || bll.IsAdministrator());
			EventMgtContainer.Visible = (bll.IsEventCreator() || bll.IsAdministrator());
//			NewEventContainer.Visible = NewEventContainer.Visible && (bll.IsEventCreator() || bll.IsAdministrator());
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.LogoutButton.Click += new System.EventHandler(this.LogoutButton_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Navigation_PreRender);

		}
		#endregion
	}
}