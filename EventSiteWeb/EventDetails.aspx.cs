using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI.HtmlControls;
using kcm.ch.EventSite.Common;

namespace kcm.ch.EventSite.Web
{
	/// <summary>
	/// Summary description for Events.
	/// </summary>
	public class EventDetails : PageBase
	{
		#region Declarations

		protected HtmlGenericControl pageTitle;
		protected HtmlGenericControl title;
		protected System.Web.UI.WebControls.Label TitleLabel;
		protected System.Web.UI.WebControls.Label LocationLabel;
		protected System.Web.UI.WebControls.Label StartDateLabel;
		protected System.Web.UI.WebControls.Label DurationLabel;
		protected System.Web.UI.WebControls.HyperLink UrlLink;
		protected System.Web.UI.WebControls.Label MinSubscriptionsLabel;
		protected System.Web.UI.WebControls.Label MaxSubscriptionsLabel;
		protected System.Web.UI.WebControls.Label DescriptionLabel;

		private const string curEventViewStateKey = "EventSite.CurrentEvent";
		#endregion
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack)
			{
				string eventQryStr = Request.QueryString["evid"];
				if(eventQryStr==null || eventQryStr==string.Empty)
				{
					throw new EventSiteException("QueryString Parameter 'evid' not found!", 900);
				}

				CurrentEvent = BLL.GetEvent(Convert.ToInt32(eventQryStr));
				FillFormFields();
			}
		}

		private void Events_PreRender(object sender, EventArgs e)
		{
			string titleAdd;

			titleAdd = CurrentEvent.EventTitle;

			title.InnerText = BLL.Mandator.SiteTitle + "Anlass: " + titleAdd;
			pageTitle.InnerText = "Anlass: " + titleAdd;
		}

		private void FillFormFields()
		{
			TitleLabel.Text = CurrentEvent.EventTitle;
			DescriptionLabel.Text = HttpUtility.HtmlEncode(CurrentEvent.EventDescription == null ? "" : CurrentEvent.EventDescription).Replace("\r\n", "\n").Replace("\n", "<br/>") + "&nbsp;";
			UrlLink.NavigateUrl = CurrentEvent.EventUrl;
			UrlLink.Text = CurrentEvent.EventUrl;
			UrlLink.Target = "_blank";
			LocationLabel.Text = CurrentEvent.Location.LocationText;
			StartDateLabel.Text = CurrentEvent.StartDate.ToString("dd.MM.yyyy HH:mm");
			DurationLabel.Text = CurrentEvent.Duration + "&nbsp;";
			MinSubscriptionsLabel.Text = CurrentEvent.MinSubscriptions.IsNull ? "&nbsp;" : CurrentEvent.MinSubscriptions.ToString();
			MaxSubscriptionsLabel.Text = CurrentEvent.MaxSubscriptions.IsNull ? "&nbsp;" : CurrentEvent.MaxSubscriptions.ToString();
		}

		public Event CurrentEvent
		{
			get { return currentEvent; }
			set { currentEvent = value; }
		}
		private Event currentEvent;

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
			this.PreRender += new System.EventHandler(this.Events_PreRender);

		}
		#endregion
	}
}
