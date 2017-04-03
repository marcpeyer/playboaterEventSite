using System;
using System.IO;
using System.Reflection;
using System.Web.UI.HtmlControls;
using kcm.ch.EventSite.Web.modules;

namespace kcm.ch.EventSite.Web
{
	/// <summary>
	/// Summary description for About.
	/// </summary>
	public class About : PageBase
	{
		protected HtmlGenericControl pageTitle;
		protected HtmlGenericControl title;
		protected HtmlTableRow MandatorSmsCreditRow;

		protected string version = "";
		protected string buildDate = "";
		protected string assemblyName = "";
		protected string mandatorName = "";
		protected string mandatorSiteTitle = "";
		protected string mandatorSmsCredit = "";
	
		private void Page_Load(object sender, EventArgs e)
		{
			mandatorName = BLL.Mandator.MandatorName;
			mandatorSiteTitle = BLL.Mandator.SiteTitle;
			mandatorSmsCredit = (BLL.Mandator.SmsPurchased - BLL.Mandator.SmsLog).ToString();
			MandatorSmsCreditRow.Visible = BLL.Mandator.SmsNotifications;

			// version of web assembly
			Assembly assembly = Assembly.GetExecutingAssembly();
			Version assemblyVersion = assembly.GetName().Version;
			version = String.Format("{0}.{1}.{2}", assemblyVersion.Major, assemblyVersion.Minor, assemblyVersion.Build);
			assemblyName = assembly.GetName().Name;

			DateTime dt = DateTime.Parse("01.01.2000");
			dt = dt.AddDays(assemblyVersion.Build);
			dt = dt.AddSeconds(assemblyVersion.Revision * 2);
			if(TimeZone.IsDaylightSavingTime(dt, TimeZone.CurrentTimeZone.GetDaylightChanges(dt.Year)))
			{
				dt = dt.AddHours(1);
			}
			if(dt > DateTime.Now || assemblyVersion.Build < 730 || assemblyVersion.Revision == 0)
			{
				dt = File.GetLastWriteTime(assembly.Location);
			}

			buildDate = dt.ToLongDateString() + " " + dt.ToLongTimeString();
		}

		private void About_PreRender(object sender, EventArgs e)
		{
			title.InnerText = BLL.Mandator.SiteTitle + " - " + "Über Event Site";
			pageTitle.InnerText = BLL.Mandator.SiteTitle + " - " + "Über Event Site";
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
			this.PreRender += new EventHandler(About_PreRender);
		}
		#endregion
	}
}
