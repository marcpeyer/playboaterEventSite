using System;
using System.Web.UI.HtmlControls;
using kcm.ch.EventSite.Web.modules;

namespace kcm.ch.EventSite.Web
{
	/// <summary>
	/// Summary description for Help.
	/// </summary>
	public class Help : PageBase
	{
		protected HtmlGenericControl pageTitle;
		protected HtmlGenericControl title;
		protected Navigation PageNavigation;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
		}

		private void Help_PreRender(object sender, EventArgs e)
		{
			title.InnerText = BLL.Mandator.SiteTitle + " - " + "Hilfe";
			pageTitle.InnerText = BLL.Mandator.SiteTitle + " - " + "Hilfe";
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
			this.PreRender += new EventHandler(Help_PreRender);
		}
		#endregion
	}
}
