using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace kcm.ch.EventSite.Web
{
	/// <summary>
	/// Summary description for MaintenanceMode.
	/// </summary>
	public class MaintenanceMode : Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			string title = Request.QueryString["tit"];
			string text = Request.QueryString["txt"];
			LiteralControl lit = new LiteralControl(String.Format("<h1 style=\"color:red\">{0}</h1><h3 style=\"color:red\">{1}</h3>", title, text));
			Controls.Add(lit);
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
		}
		#endregion
	}
}
