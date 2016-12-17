using System;
using System.Collections;
using System.Drawing;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AJAXDatePicker;
using kcm.ch.EventSite.Common;
using kcm.ch.EventSite.Web.modules;

namespace kcm.ch.EventSite.Web
{
	/// <summary>
	/// Summary description for Events.
	/// </summary>
	public class Events : PageBase
	{
		#region Declarations
		protected Label lblEvent;
		protected HtmlGenericControl pageTitle;
		protected TextBox TitleTextBox;
		protected DropDownList LocationDropDown;
		protected TextBox DescriptionTextBox;
		protected TextBox UrlTextBox;
		protected TextBox MinSubscriptionsTextBox;
		protected TextBox MaxSubscriptionsTextBox;
		protected TextBox DurationTextBox;
		protected Button SaveButton;
		protected Button ResetButton;
		protected Panel pnlEvent;
		protected DropDownList EventList;
		protected HyperLink LocationMgrLink;
		protected Button LocationMgrButton;
		protected AJAXDatePicker.AJAXDatePickerControl StartDate;
		protected System.Web.UI.WebControls.RadioButtonList NotifyChange;
		protected System.Web.UI.HtmlControls.HtmlGenericControl NotifyChangeArea;
		protected System.Web.UI.WebControls.RequiredFieldValidator reqTitle;
		protected System.Web.UI.WebControls.RequiredFieldValidator reqNotify;
		protected System.Web.UI.WebControls.ValidationSummary validationSummary;
		protected System.Web.UI.WebControls.RequiredFieldValidator reqLocation;
		protected System.Web.UI.WebControls.ImageButton NewEventButton;
		protected System.Web.UI.WebControls.ImageButton CopyEventButton;
		protected System.Web.UI.WebControls.Image CopyEventInactive;

		protected HtmlTableRow EventUrlRow;
		protected HtmlTableRow EventCategoryRow;
		protected HtmlTableRow MinMaxSubscriptionsRow;
		protected System.Web.UI.WebControls.Label lblEventCategory;
		protected System.Web.UI.WebControls.Label lblCategoryDesc;
		protected System.Web.UI.WebControls.DropDownList EventCategories;

		private const string curEventViewStateKey = "EventSite.CurrentEvent";
		#endregion
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack)
			{
				StartDate.ImageDirectory = "images/datepicker";
				StartDate.LabelText = "";
				StartDate.TitleText = "Format: [dd.mm.yyyy]";
				StartDate.TimeBoxVisible = true;

				EventCategories.DataSource = BLL.ListEventCategories();
				EventCategories.DataTextField = "Category";
				EventCategories.DataValueField = "EventCategoryId";
				EventCategories.DataBind();

				LocationMgrLink.NavigateUrl = "JavaScript:showLocationMgr('" + BLL.Mandator.MandatorId + "');";

				EventCategory curCat = (EventCategory)((ArrayList)EventCategories.DataSource)[EventCategories.SelectedIndex];

				LocationDropDown.DataSource = BLL.ListLocations(curCat);
				LocationDropDown.DataTextField = "LocationText";
				LocationDropDown.DataValueField = "LocationId";
				LocationDropDown.DataBind();

				EventList.DataSource = BLL.ListEvents(curCat, Event.DescendingComparer);
				EventList.DataTextField = "EventTitleAndDate";
				EventList.DataValueField = "EventId";
				EventList.DataBind();
				EventList.Items.Insert(0, new ListItem("", "-1"));

				lblCategoryDesc.Text = String.Format(" {0}", curCat.CategoryDescription);
				lblCategoryDesc.ToolTip = curCat.CategoryDescription;
				lblCategoryDesc.Style.Add("text-overflow", "ellipsis");

				NotifyChange.Items.Add(new ListItem("Ja", "1"));
				NotifyChange.Items.Add(new ListItem("Nein", "-1"));

//				if(EventList.Items.Count > 0)
//				{
//					CurrentEvent = BLL.GetEvent(Convert.ToInt32(EventList.SelectedValue));
//					FillFormFields();
//				}
//				else
//				{
					CurrentEvent = null;
//				}

				string mode = Request.QueryString["mode"];
				if(mode != null && mode == "new")
				{
					NewEventButton_Click(null, null);
				}
			}
		}

		private void EventCategories_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			EventCategory selCategory = BLL.GetEventCategory(Int32.Parse(EventCategories.SelectedValue));
			LocationDropDown.DataSource = BLL.ListLocations(selCategory);
			LocationDropDown.DataTextField = "LocationText";
			LocationDropDown.DataValueField = "LocationId";
			LocationDropDown.DataBind();

			EventList.DataSource = BLL.ListEvents(selCategory, Event.DescendingComparer);
			EventList.DataTextField = "EventTitleAndDate";
			EventList.DataValueField = "EventId";
			EventList.DataBind();
			EventList.Items.Insert(0, new ListItem("", "-1"));

			lblCategoryDesc.Text = String.Format(" {0}", selCategory.CategoryDescription);
			lblCategoryDesc.ToolTip = selCategory.CategoryDescription;

			//if not new event
			if(CurrentEvent == null || CurrentEvent.EventId != -1)
			{
				CurrentEvent = null;
				FillFormFields();
			}
		}

		private void NewEventButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			Event newEvent = new Event(BLL.GetEventCategory(Int32.Parse(EventCategories.SelectedValue)), BLL.CurrentContact);

			CurrentEvent = newEvent;
			if(NotifyChange.SelectedItem != null) NotifyChange.SelectedItem.Selected = false;

			EventList.SelectedValue = "-1";
			EventList.Enabled = false;
			EventCategories.Enabled = false;

			FillFormFields();
		}

		private void CopyEventButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			CurrentEvent.EventId = -1;
			CurrentEvent.StartDate = DateTime.Now.Date + CurrentEvent.StartDate.TimeOfDay;
			CurrentEvent.EventCreator = BLL.CurrentContact;

			EventList.SelectedValue = "-1";
			EventList.Enabled = false;
			EventCategories.Enabled = false;

			FillFormFields();
		}

		private void EventList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int eventId = Convert.ToInt32(EventList.SelectedValue);
			if(eventId != -1)
			{
				CurrentEvent = BLL.GetEvent(eventId);
			}
			else
			{
				CurrentEvent = null;
			}
			
			if(NotifyChange.SelectedItem != null) NotifyChange.SelectedItem.Selected = false;
			FillFormFields();
		}

		private void LocationMgrButton_Click(object sender, System.EventArgs e)
		{
			LocationDropDown.DataSource = BLL.ListLocations(BLL.GetEventCategory(Int32.Parse(EventCategories.SelectedValue)));
			LocationDropDown.DataTextField = "LocationText";
			LocationDropDown.DataValueField = "LocationId";
			LocationDropDown.DataBind();
		}

		private void ResetButton_Click(object sender, System.EventArgs e)
		{
			if(NotifyChange.SelectedItem != null) NotifyChange.SelectedItem.Selected = false;
			EventCategories.Enabled = true;
			EventList.Enabled = true;
			EventList.SelectedIndex = 0;
			CurrentEvent = null;
			FillFormFields();
		}

		private void SaveButton_Click(object sender, System.EventArgs e)
		{
			if(Page.IsValid)
			{
				Event ev = CurrentEvent;
				try
				{
					ev.EventTitle = TitleTextBox.Text;
//					ev.EventDescription = DescriptionTextBox.Text.Replace("\r\n", "\n").Replace("\n", "<br>");
					ev.EventDescription = DescriptionTextBox.Text;
					ev.EventUrl = UrlTextBox.Text == String.Empty || UrlTextBox.Text == "http://" ? null : UrlTextBox.Text;
					ev.Location = BLL.GetLocation(Int32.Parse(LocationDropDown.SelectedValue), BLL.GetEventCategory(Int32.Parse(EventCategories.SelectedValue)));
					if(StartDate.selectedDateTime.Date < DateTime.Now.Date)
					{
						throw new EventSiteException("Das Startdatum kann nicht in der Vergangenheit liegen!", -1);
					}
					ev.StartDate = StartDate.selectedDateTime;
					ev.Duration = DurationTextBox.Text;
					if(MinSubscriptionsTextBox.Text == string.Empty)
					{
						ev.MinSubscriptions = null;
					}
					else
					{
						ev.MinSubscriptions =  Convert.ToInt32(MinSubscriptionsTextBox.Text);
					}
					if(MaxSubscriptionsTextBox.Text == string.Empty)
					{
						ev.MaxSubscriptions = null;
					}
					else
					{
						ev.MaxSubscriptions = Convert.ToInt32(MaxSubscriptionsTextBox.Text);
					}

					if(NotifyChange.SelectedItem != null)
					{
						BLL.SaveEvent(ev, NotifyChange.SelectedItem.Value == "1");
					}
					else
					{
						BLL.SaveEvent(ev);
					}
					
				}
				catch(EventSiteException ex)
				{
					RegisterStartupScriptIfNeeded("SaveEventError", ex.JavaScriptAlertString);
					return;
				}
				finally
				{
					try
					{
						ev = BLL.GetEvent(ev.EventId);
					}
					catch{}
				}

				//save is successful --> redirect to home
				//Server.Execute(string.Format("Default.aspx?mid={0}&eid={1}", BLL.Mandator.MandatorId, ev.EventId));
//				Response.Redirect(string.Format("Default.aspx?mid={0}&eid={1}", BLL.Mandator.MandatorId, ev.EventId), false);
				Response.Redirect(string.Format("Default.aspx?mid={0}&eid={1}", BLL.Mandator.MandatorId, ev.EventId), true);

/*				EventList.DataSource = BLL.ListEvents();
				EventList.DataTextField = "EventTitleAndDate";
				EventList.DataValueField = "EventId";
				EventList.DataBind();
				EventList.Items.Insert(0, new ListItem("", "-1"));

				try
				{
					EventList.SelectedValue = ev.EventId.ToString();
					CurrentEvent = ev;

					FillFormFields();
				}
				catch{}

				if(NotifyChange.SelectedItem != null) NotifyChange.SelectedItem.Selected = false;*/
			}
		}

		private void Events_PreRender(object sender, EventArgs e)
		{
			string titleAdd;

			if(CurrentEvent == null)
			{
				TitleTextBox.Enabled = false;
				TitleTextBox.BackColor = Color.FromKnownColor(KnownColor.LightGray);
				DescriptionTextBox.Enabled = false;
				DescriptionTextBox.BackColor = Color.FromKnownColor(KnownColor.LightGray);
				UrlTextBox.Enabled = false;
				UrlTextBox.BackColor = Color.FromKnownColor(KnownColor.LightGray);
				LocationDropDown.ClearSelection();
				LocationDropDown.Enabled = false;
				LocationDropDown.BackColor = Color.FromKnownColor(KnownColor.LightGray);
				DurationTextBox.Enabled = false;
				DurationTextBox.BackColor = Color.FromKnownColor(KnownColor.LightGray);
				MinSubscriptionsTextBox.Enabled = false;
				MinSubscriptionsTextBox.BackColor = Color.FromKnownColor(KnownColor.LightGray);
				MaxSubscriptionsTextBox.Enabled = false;
				MaxSubscriptionsTextBox.BackColor = Color.FromKnownColor(KnownColor.LightGray);

				ResetButton.Enabled = false;
				SaveButton.Enabled = false;

				titleAdd = ": Kein " + BLL.Mandator.EventName + " ausgewählt";

				CopyEventButton.Visible = false;
				NotifyChangeArea.Visible = false;
			}
			else
			{
				TitleTextBox.Enabled = true;
				TitleTextBox.BackColor = Color.White;
				DescriptionTextBox.Enabled = true;
				DescriptionTextBox.BackColor = Color.White;
				UrlTextBox.Enabled = true;
				UrlTextBox.BackColor = Color.White;
				LocationDropDown.Enabled = true;
				LocationDropDown.BackColor = Color.White;
				DurationTextBox.Enabled = true;
				DurationTextBox.BackColor = Color.White;
				MinSubscriptionsTextBox.Enabled = true;
				MinSubscriptionsTextBox.BackColor = Color.White;
				MaxSubscriptionsTextBox.Enabled = true;
				MaxSubscriptionsTextBox.BackColor = Color.White;

				ResetButton.Enabled = true;
				SaveButton.Enabled = true;

				titleAdd = ": " + CurrentEvent.EventTitle;

				CopyEventButton.Visible = (CurrentEvent.EventId != -1);
				NotifyChangeArea.Visible = (CurrentEvent.EventId != -1) && (BLL.Mandator.OnNewEventNotifyContacts
					|| BLL.Mandator.OnEditEventNotifyContacts
					|| BLL.Mandator.OnNewSubscriptionNotifyContacts
					|| BLL.Mandator.OnEditSubscriptionNotifyContacts
					|| BLL.Mandator.OnDeleteSubscriptionNotifyContacts);
				SaveButton.Text = (CurrentEvent.EventId == -1 ? "Neuer Anlass speichern" : "Änderungen speichern");
			}

			Page.Title = $"{BLL.Mandator.EventName}{titleAdd} - {BLL.Mandator.SiteTitle}";
			pageTitle.InnerText = BLL.Mandator.EventName + titleAdd;
			CopyEventInactive.Visible = !CopyEventButton.Visible;

			EventCategoryRow.Visible = BLL.Mandator.UseEventCategories;
			EventUrlRow.Visible = BLL.Mandator.UseEventUrl;
			MinMaxSubscriptionsRow.Visible = BLL.Mandator.UseMinMaxSubscriptions;

		}

		private void FillFormFields()
		{
			Event ev = CurrentEvent;
			if(ev != null)
			{
				TitleTextBox.Text = ev.EventTitle;
//				DescriptionTextBox.Text = ev.EventDescription == null ? "" : ev.EventDescription.Replace("<br>", "\r\n");
				DescriptionTextBox.Text = ev.EventDescription;
				UrlTextBox.Text = ev.EventUrl == null || ev.EventUrl == String.Empty ? "http://" : ev.EventUrl;
				if(ev.Location != null)
				{
					LocationDropDown.SelectedValue = ev.Location.LocationId.ToString();
				}
				StartDate.selectedDateTime = ev.StartDate;
				DurationTextBox.Text = ev.Duration;
				MinSubscriptionsTextBox.Text = ev.MinSubscriptions.IsNull ? string.Empty : ev.MinSubscriptions.ToString();
				MaxSubscriptionsTextBox.Text = ev.MaxSubscriptions.IsNull ? string.Empty : ev.MaxSubscriptions.ToString();
			}
			else
			{
				TitleTextBox.Text = string.Empty;
				DescriptionTextBox.Text = string.Empty;
				UrlTextBox.Text = string.Empty;
				if(LocationDropDown.Items.Count > 0)
				{
					LocationDropDown.Items[0].Selected = true;
				}
				StartDate.selectedDateTime = DateTime.Now;
				DurationTextBox.Text = string.Empty;
				MinSubscriptionsTextBox.Text = string.Empty;
				MaxSubscriptionsTextBox.Text = string.Empty;
			}
		}

		private Event CurrentEvent
		{
			get { return (Event)ViewState[curEventViewStateKey]; }
			set { ViewState[curEventViewStateKey] = value; }
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
			this.EventCategories.SelectedIndexChanged += new System.EventHandler(this.EventCategories_SelectedIndexChanged);
			this.EventList.SelectedIndexChanged += new System.EventHandler(this.EventList_SelectedIndexChanged);
			this.NewEventButton.Click += new System.Web.UI.ImageClickEventHandler(this.NewEventButton_Click);
			this.CopyEventButton.Click += new System.Web.UI.ImageClickEventHandler(this.CopyEventButton_Click);
			this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
			this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
			this.LocationMgrButton.Click += new System.EventHandler(this.LocationMgrButton_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Events_PreRender);

		}
		#endregion
	}
}
