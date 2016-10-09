using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using kcm.ch.EventSite.BusinessLayer;
using kcm.ch.EventSite.Common;
using kcm.ch.EventSite.EventSiteAPI;
using kcm.ch.EventSite.Web.modules;
using pbHelpers=playboater.gallery.commons.Helpers;

namespace kcm.ch.EventSite.Web
{
	/// <summary>
	/// Summary description for Default.
	/// </summary>
	public class Default : PageBase
	{
		#region Declarations

		protected DropDownList events;
		protected ListBox eventList;
		protected TextBox email;
		protected RequiredFieldValidator reqSubscriptEmail;
		protected Label lblSubscriptEmail;
		protected Panel pnlSubscriptions;
		protected DataGrid dgrSubscriptions;
		protected Label lblName;
		protected TextBox txtName;
		protected Label lblEmail;
		protected TextBox txtEmail;
		protected Label lblMobile;
		protected TextBox txtMobile;
		protected Label lblSubscript;
		protected Label lblSubscriptions;
		protected Label lblAddContact;
		protected RequiredFieldValidator reqAddContactName;
		protected Panel pnlAddContact;
		protected Label lblSmsBalanceNotifier;
		protected CheckBox chkNotifyBySms;
		protected Label lblComment;
		protected TextBox txtComment;
		protected Label lblAddSubscriptionDay;
		protected Label lblSubscriptionTime;
		protected TextBox subscriptionTime;
		protected CheckBox chkNotifyByEmail;
		protected HtmlGenericControl pageTitle;
		protected HtmlGenericControl title;
		protected Panel pnlAddSubscription;
		protected Label lblAddSubscriptionEmail;
		protected RadioButtonList subscriptionStates;
		protected Literal helptext;
//		protected HyperLink eventDetailsLink;
		protected Panel pnlEvent;
//		protected Label lblMaxNotifications;
//		protected TextBox maxNotifications;
		protected HtmlGenericControl notifSubscription;
		protected Button AddSubscription;
		protected Button AddContact;
		protected Navigation PageNavigation;
		protected HtmlGenericControl legendDiv;
		protected HtmlTableCell AddSubscriptionCell;
		protected HtmlGenericControl subscriptionMailContainer;
		protected HtmlGenericControl numSmsNotificationsLabel;
		protected ImageButton refreshEventButton;
		protected ImageButton showOldEventsButton;
		protected Panel pnlEventDetails;
		protected Label lblNotifSubscription;
		protected Label lblNofifSubscriptionDisabled;
		protected CheckBox chkNotifSubscription;
		protected Label lblEvent;
		protected TextBox txtNumSmsNotifications;

		protected Label EventCategoryLabel;
		protected Label TitleLabel;
		protected Label LocationLabel;
		protected Label StartDateLabel;
		protected Label DurationLabel;
		protected HyperLink UrlLink;
		protected ContactControl EventCreator;
		protected Label MinSubscriptionsLabel;
		protected Label MaxSubscriptionsLabel;
		protected Label DescriptionLabel;
		protected Panel pnlFeature;
		protected PopupControl EventCategoryFilterPopup;

		protected HtmlTableCell EventDetails_EventUrlLabel;
		protected HtmlTableCell EventDetails_EventUrlControl;
		protected HtmlTableRow EventDetails_MinMaxSubscriptions;
		protected HtmlTableRow EventDetails_EventCategory;
		
//		private RiverLevelHandler levelHandler;
		private DateTime currentDate;
		private static readonly Color COLOR1 = Color.Beige;
		private static readonly Color COLOR2 = Color.WhiteSmoke;
		private Color currColor = COLOR1;
		private const string curEventViewStateKey = "EventSite.CurrentEvent";
		private const string showOldEventsViewStateKey = "EventSite.ShowOldEvents";
		private const string isCategoryFilterOnViewStateKey = "EventSite.IsCategoryFilterOn";
		protected DropDownList EventCategories;
		protected CheckBox chkCategoryFilterOn;
		#endregion

		#region Server Events

		#region Page Load

		private void Page_Load(object sender, EventArgs e)
		{
			if(BLL.IsEventCreator() && !BLL.IsUser() && !BLL.IsReader() && !BLL.IsManager())
			{
				Response.Redirect("Events.aspx?mid=" + BLL.Mandator.MandatorId);
			}

			Ajax.Utility.RegisterTypeForAjax(typeof(EventSiteBL));

			EventCategories = new DropDownList();
			EventCategories.DataSource = BLL.ListEventCategories();
			EventCategories.DataTextField = "Category";
			EventCategories.DataValueField = "EventCategoryId";
			EventCategories.DataBind();
			EventCategories.Items.Insert(0, "");
			EventCategoryFilterPopup.PopupBody.Controls.Add(EventCategories);

			if(!IsPostBack)
			{
				ShowOldEvents = false;
				IsCategoryFilterOn = false;

				subscriptionStates.DataTextField = "StateText";
				subscriptionStates.DataValueField = "subscriptionStateId";

				EventCategoryFilterPopup.PopupDisabledScriptCall = String.Format("document.getElementById('{0}').click();", chkCategoryFilterOn.ClientID);

				events.DataSource = BLL.ListFutureEvents(Event.AscendingComparer);
				events.DataTextField = "EventTitleAndDate";
				events.DataValueField = "EventId";
				events.DataBind();
				eventList.DataSource = events.DataSource;
				eventList.DataTextField = events.DataTextField;
				eventList.DataValueField = events.DataValueField;
				eventList.DataBind();

				if(events.Items.Count > 0)
				{
					if(Request.QueryString["eid"] != null && Request.QueryString["eid"] != string.Empty)
					{
						try
						{
							events.SelectedValue = Request.QueryString["eid"];
							eventList.SelectedValue = Request.QueryString["eid"];
						}
						catch{}
					}
					else
					{
						events.SelectedIndex = 0;
						eventList.SelectedIndex = 0;
					}
					CurrentEvent = BLL.GetEvent(Convert.ToInt32(events.SelectedValue));
//					eventDetailsLink.Enabled = true;
//					eventDetailsLink.NavigateUrl = String.Format("EventDetails.aspx?mid={0}&evid={1}", BLL.Mandator.MandatorId, CurrentEvent.EventId);
//					eventDetailsLink.Target = "_blank";

					FillEventDetailFields();
					UpdateSubscriptionList();
				}
				else
				{
					CurrentEvent = null;
//					eventDetailsLink.Enabled = false;
				}
			}
		}

		#endregion

		#region grid item databound
		private void DgrSubscriptions_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if(e.Item.ItemType!=ListItemType.Header && e.Item.ItemType!=ListItemType.Footer)
			{
				Subscription subscr = (Subscription)e.Item.DataItem;
				if(subscr.Fontcolor != null)
				{
					Color c = subscr.GetFontcolor();
					e.Item.ForeColor = c;
				}
				if(subscr.SubscriptionTime != null && subscr.SubscriptionTime != string.Empty)
				{
					((Label)e.Item.FindControl("SubscriptionTimeLabel")).Text = subscr.SubscriptionTime;
				}
				if(currentDate!=subscr.Event.StartDate)
				{
					currentDate = subscr.Event.StartDate;
					currColor = (currColor==COLOR1 ? COLOR2 : COLOR1);
				}
				e.Item.BackColor = currColor;
			}
		}
		#endregion

		#region AddSubscription Click

		private void AddSubscription_Click(object sender, EventArgs e)
		{
			List<Subscription> subscriptions = null;
			Contact contact;

			//easteregg ;-)
			if(email.Text == "ThrowEventSiteTestException")
			{
				//throw a test exception
				throw new EventSiteException("Das ist eine Test-Fehlermeldung von der EventSite.", -1);
			}

			if(BLL.IsAdministrator() || BLL.IsManager())
			{
				if(email.Text.Trim() == string.Empty)
				{
					contact = BLL.CurrentContact;
				}
				else
				{
					if(pbHelpers.ValidateEmail(email.Text.Trim()))
					{
						try
						{
							contact = BLL.GetContact(email.Text.Trim());
							if(contact.IsDeleted)
							{
								throw new EventSiteException("IsDeleted", 101);
							}
						}
						catch (EventSiteException ex)
						{
							switch(ex.ErrorNumber)
							{
								case 101:
									ShowAddNewContact();
									return;
								default:
									RegisterStartupScriptIfNeeded("ErrAlert", ex.JavaScriptAlertString);
									return;
							}
						}
					}
					else
					{
						RegisterStartupScriptIfNeeded("invalidMail", string.Format(pbHelpers.JavaScriptAlertString, "Angegebene Email-Adresse ist ungültig."));
						return;
					}
				}
			}
			else
			{
				contact = BLL.CurrentContact;
			}
			
#if(DEBUG)
			System.Threading.Thread.Sleep(1500);
#endif
			try
			{
				subscriptions = BLL.AddSubscription(new Subscription(CurrentEvent, contact, Convert.ToInt32(subscriptionStates.SelectedValue),
					subscriptionStates.SelectedItem.Text, subscriptionTime.Text, txtComment.Text, null, null));
			}
			catch (EventSiteException ex)
			{
				RegisterStartupScriptIfNeeded("ErrAlert", ex.JavaScriptAlertString);
				return;
			}
			finally
			{
				if(subscriptions != null)
				{
					UpdateSubscriptionList(subscriptions);
				}
				else
				{
					UpdateSubscriptionList();
				}
			}
		}

		#endregion

		#region AddContact Click

		private void AddContact_Click(object sender, EventArgs e)
		{
			try
			{
				Contact newContact = new Contact(
					BLL.Mandator,
					txtName.Text,
					txtEmail.Text,
					txtMobile.Text,
					false,
					false,
					false,
					0,
					0,
					false);

				//save new contact to db
				string userInfoText;
				userInfoText = BLL.AddContact(newContact);
				if(userInfoText != null)
				{
					RegisterStartupScriptIfNeeded("AddContactUserInfo", string.Format(pbHelpers.JavaScriptAlertString, userInfoText));
				}
				
				Subscription subscription = new Subscription(CurrentEvent, newContact, Convert.ToInt32(subscriptionStates.SelectedValue), subscriptionStates.SelectedItem.Text, subscriptionTime.Text, txtComment.Text, null, null);
				dgrSubscriptions.DataSource = BLL.AddSubscription(subscription);
			}
			catch (EventSiteException ex)
			{
				RegisterStartupScriptIfNeeded("ErrAlert", ex.JavaScriptAlertString);
				return;
			}
			string successMsg = String.Format(pbHelpers.JavaScriptAlertString, "Kontakt und Anmeldung wurden erfolgreich gespeichert.");
			RegisterStartupScriptIfNeeded("successMsg", successMsg);

			pnlAddSubscription.Visible = true;
			pnlSubscriptions.Visible = true;
			pnlAddContact.Visible = false;

#if(DEBUG)
			System.Threading.Thread.Sleep(1500);
#endif
		}

		#endregion

		#region EventsSelectedIndex Changed
		private void events_SelectedIndexChanged(object sender, EventArgs e)
		{
			CurrentEvent = BLL.GetEvent(Convert.ToInt32(((ListControl)sender).SelectedValue));

//			eventDetailsLink.Enabled = true;
//			eventDetailsLink.NavigateUrl = String.Format("EventDetails.aspx?mid={0}&evid={1}", BLL.Mandator.MandatorId, CurrentEvent.EventId);

			UpdateSubscriptionList();
			FillEventDetailFields();
		}
		#endregion

		#region ShowOldEventsButton Click
		private void showOldEventsButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			ShowOldEvents = !ShowOldEvents;

			UpdateEventList();
			UpdateSubscriptionList();
			FillEventDetailFields();
		}
		#endregion

		#region RefreshEventButton Click
		private void refreshEventButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			BLL.RenewCurrentContact();

			// ToDo: use correct control
			string selBefore = (BLL.Mandator.ShowEventsAsList ? eventList.SelectedValue : events.SelectedValue);

			events.DataSource = (ShowOldEvents ? BLL.ListEvents(Event.AscendingComparer) : BLL.ListFutureEvents(Event.AscendingComparer));
			events.DataBind();
			eventList.DataSource = events.DataSource;
			eventList.DataBind();
			try
			{
				events.SelectedValue = selBefore;
				eventList.SelectedValue = selBefore;
			}
			catch{}
			UpdateSubscriptionList();
			FillEventDetailFields();
		}
		#endregion

		public void ChkCategoryFilterOn_CheckedChanged(object sender, EventArgs e)
		{
			IsCategoryFilterOn = ((CheckBox)sender).Checked;

			EventCategoryFilterPopup.PopupEnabled = !IsCategoryFilterOn;

			if(!IsCategoryFilterOn)
			{
				EventCategories.SelectedIndex = 0;
			}

			UpdateEventList();
			UpdateSubscriptionList();
			FillEventDetailFields();
		}

		#region PreRender

		private void Default_PreRender(object sender, EventArgs e)
		{
			#region Features
			string featureAssembly;
			string featureAssemblyClassName;
			if(IsCategoryFilterOn && CurrentEventCategory.FeatureAssembly != null)
			{
				featureAssembly = CurrentEventCategory.FeatureAssembly;
				featureAssemblyClassName = CurrentEventCategory.FeatureAssemblyClassName;
			}
			else
			{
				featureAssembly = BLL.Mandator.FeatureAssembly;
				featureAssemblyClassName = BLL.Mandator.FeatureAssemblyClassName;
			}
			if(featureAssembly != null)
			{
				try
				{
					Assembly a = Assembly.LoadFrom(featureAssembly);
					IFeature feature = (IFeature)a.CreateInstance(featureAssemblyClassName);
					//					IFeature feature = (IFeature)a.CreateInstance(BLL.Mandator.FeatureAssemblyClassName, false, BindingFlags.CreateInstance, null, new object[]{Request}, null, null);
									
					feature.AddContent(pnlFeature, Request);
				}
				catch(Exception ex)
				{
					Helpers.TrySendErrorMail(ex);

					Label lbl = new Label();
					lbl.Text = "Beim Versuch das FeatureAssembly auszuführen ist folgender Fehler aufgetreten: ";
					lbl.Text += ex.ToString();
					pnlFeature.Controls.Clear();
					pnlFeature.Controls.Add(lbl);
				}
			}
			#endregion


			EventCategories.Attributes.Add("onchange", String.Format("document.getElementById('{0}').checked = false;document.getElementById('{0}').click();", chkCategoryFilterOn.ClientID));
			EventCategoryFilterPopup.PopupText = IsCategoryFilterOn ? "Filter deaktivieren" : "Nach Kategorie filtern";
			EventCategoryFilterPopup.PopupIconSrc = IsCategoryFilterOn ? "images\\icons\\filter_on.gif" : "images\\icons\\filter_off.gif";

			if(BLL.Mandator.UseSubscriptions)
			{
				dgrSubscriptions.DataBind();
//				events.DataBind();

				pnlAddSubscription.Visible = !BLL.IsReader();
				AddSubscriptionCell.Visible = !BLL.IsReader();
				if(!pnlAddContact.Visible)
				{
					pnlSubscriptions.Visible = true;
				}
				notifSubscription.Visible = true;

//				string styleBlock = @"
//<style type=""text/css"">
//<!--
//#pnlEventContainer > #pnlEvent
//{
//	 width: 10%;
//}
//
//#eventList
//{
//	vertical-align: bottom;
//}
//#eventChooser > #eventList
//{
//	vertical-align: baseline;
//}
//-->
//</style>";
//				if(!IsClientScriptBlockRegistered("StyleHack"))
//					RegisterClientScriptBlock("StyleHack", styleBlock);
				pnlEvent.Style["FLOAT"] = "left";
				pnlEvent.Style["MARGIN-BOTTOM"] = "auto";
				pnlEventDetails.Style["MARGIN-RIGHT"] = "auto";

				string clientScript = string.Format(pbHelpers.JavaScriptString, @"
				fResetEventDetailSize();
				
				/*Überwachung von Veränderung der Fenstergrösse initialisieren*/
				window.onresize = fResetEventDetailSize;
");
				RegisterStartupScriptIfNeeded("EventDetailHandling", clientScript);

				if(IsOldEvent)
				{
					pnlAddSubscription.Visible = false;
					subscriptionStates.Enabled = false;
					email.Enabled = false;
					email.BackColor = Color.FromKnownColor(KnownColor.LightGray);
									subscriptionTime.Enabled = false;
					subscriptionTime.BackColor = Color.FromKnownColor(KnownColor.LightGray);
//					maxNotifications.Enabled = false;
//					maxNotifications.BackColor = Color.FromKnownColor(KnownColor.LightGray);
					txtComment.Enabled = false;
					txtComment.BackColor = Color.FromKnownColor(KnownColor.LightGray);
					AddSubscription.Enabled = false;
					dgrSubscriptions.Columns[1].Visible = false;
					dgrSubscriptions.Columns[9].Visible = false;
				}
				else
				{
					subscriptionStates.Enabled = true;
					email.Enabled = true;
					email.BackColor = Color.White;
					subscriptionTime.Enabled = true;
					subscriptionTime.BackColor = Color.White;
//					maxNotifications.Enabled = true;
//					maxNotifications.BackColor = Color.White;
					txtComment.Enabled = true;
					txtComment.BackColor = Color.White;
					AddSubscription.Enabled = true;
					dgrSubscriptions.Columns[1].Visible = true;
					dgrSubscriptions.Columns[9].Visible = true;
				}

				dgrSubscriptions.Columns[9].Visible = BLL.Mandator.IsLiftManagementEnabled && dgrSubscriptions.Columns[9].Visible;
			}
			else
			{
//				string styleBlock = @"
//<style type=""text/css"">
//<!--
//#eventDetails
//{
//	width: 100%;
//}
//
//TABLE#pnlEventDetails DIV
//{
//	 width: auto;
//}
//
//#eventList
//{
//	vertical-align: bottom;
//}
//					#eventChooser > #eventList
//					{
//	vertical-align: baseline;
//}
//-->
//</style>";
//				if(!IsClientScriptBlockRegistered("StyleHack"))
//					RegisterClientScriptBlock("StyleHack", styleBlock);

				pnlAddSubscription.Visible = false;
				AddSubscriptionCell.Visible = false;
				pnlSubscriptions.Visible = false;
				notifSubscription.Visible = false;
				
				pnlEvent.Style["FLOAT"] = "none";
				pnlEvent.Style["MARGIN-BOTTOM"] = "20px";
				pnlEventDetails.Style["MARGIN-RIGHT"] = "5px";
			}

			string titleAdd;
			if(CurrentEvent == null)
			{
				titleAdd = "Kein(e) " + BLL.Mandator.EventName + " ausgewählt";
				lblSubscriptions.Text = "Es ist kein Anlass ausgewählt.";
				dgrSubscriptions.Visible = false;
				legendDiv.Visible = false;
			}
			else
			{
				titleAdd = CurrentEvent.EventTitle;
				lblSubscriptions.Text = "Zur Zeit sind für diesen Anlass folgende Einträge vorhanden:";
				dgrSubscriptions.Visible = true;
				legendDiv.Visible = BLL.Mandator.IsLiftManagementEnabled;
			}
			title.InnerText = BLL.Mandator.SiteTitle + " - " + titleAdd;
			pageTitle.InnerText = BLL.Mandator.SiteTitle + " - " + titleAdd;
			helptext.Text = BLL.Mandator.HelpText;

			if(pnlAddSubscription.Visible)
			{
				string previousValue = null;
				if(IsPostBack)
				{
					previousValue = subscriptionStates.SelectedValue;
				}

				subscriptionStates.DataSource = BLL.ListSubscriptionStates(CurrentEvent.EventCategory);
				subscriptionStates.DataBind();
				if(!IsPostBack && subscriptionStates.Items.Count > 0)
				{
					subscriptionStates.Items[0].Selected = true;
				}
				else if(IsPostBack && subscriptionStates.Items.Count > 0)
				{
					//try to set previous value
					try
					{
						subscriptionStates.SelectedValue = previousValue;
					}
					catch
					{
						subscriptionStates.Items[0].Selected = true;
					}
				}

				subscriptionMailContainer.Visible = BLL.IsAdministrator() || BLL.IsManager();
			}

			if(BLL.IsReader())
			{
				chkNotifSubscription.Visible = false;
				txtNumSmsNotifications.Visible = false;
				numSmsNotificationsLabel.Visible = false;

				lblNofifSubscriptionDisabled.Text = "Da du nur Leserechte besitzt, kannst<br>du diese Option nicht nutzen.";
				lblNofifSubscriptionDisabled.Visible = true;
			}
			else
			{
				if(CurrentEvent != null && EventSiteBL.GetContactSetting(BLL.CurrentContact, CurrentEvent.EventCategory).SmsNotifSubscriptionsOn)
				{
					chkNotifSubscription.Visible = true;
					txtNumSmsNotifications.Visible = true;
					numSmsNotificationsLabel.Visible = true;

					lblNofifSubscriptionDisabled.Visible = false;

					SetNotifSubscriptionFields();

					string notifSubscriptionChangeScript = string.Format(@"
<SCRIPT type=""text/javascript"" language=""javascript"">
	
	function UpdateNotifSubscriptionChange()
	{{
		var nofifSubscriptionElem;
		var maxNotificationsElem;
		nofifSubscriptionElem = document.getElementById('{0}'); 
		maxNotificationsElem = document.getElementById('{1}'); 

		document.getElementById('{4}').style.backgroundColor = 'yellow'; 

		var notifSubscriptionOn;
		var maxNotifications;
		notifSubscriptionOn = nofifSubscriptionElem.checked;

		if(notifSubscriptionOn)
		{{
			maxNotifications = maxNotificationsElem.value;
		}}
		else
		{{
			maxNotificationsElem.value = '';
			maxNotifications = '';
		}}
		//alert(notifSubscriptionOn);
		//alert(maxNotifications);

		//ajax postback to update notifSubscription
		EventSiteBL.UpdateNotifSubscription('{2}', {3}, notifSubscriptionOn, maxNotifications, UpdateNotifSubscription_CallBack);
	}}

	function UpdateNotifSubscription_CallBack(response)
	{{
		if (response.error != null)
		{{
			alert(response.error.description + '\nDie Einstellung konnte nicht gespeichert werden!');
		}}
		else
		{{
			document.getElementById('{4}').style.backgroundColor = '#eeeeee'; 
		}}
	}}
</SCRIPT>
",
						chkNotifSubscription.ClientID,
						txtNumSmsNotifications.ClientID,
						BLL.Mandator.MandatorId,
						CurrentEvent == null ? "" : CurrentEvent.EventId.ToString(),
						notifSubscription.ClientID);
					RegisterStartupScriptIfNeeded("notifSubscriptionChangeScript", notifSubscriptionChangeScript);
					chkNotifSubscription.Attributes.Add("onclick", "UpdateNotifSubscriptionChange();");
					txtNumSmsNotifications.Attributes.Add("onchange", "UpdateNotifSubscriptionChange();");
					txtNumSmsNotifications.Attributes.Add("onkeyup", "UpdateNotifSubscriptionChange();");
				}
				else if(CurrentEvent != null)
				{
					chkNotifSubscription.Visible = false;
					txtNumSmsNotifications.Visible = false;
					numSmsNotificationsLabel.Visible = false;

					if(BLL.Mandator.UseEventCategories)
					{
						lblNofifSubscriptionDisabled.Text = "Du hast die Abonnierungsfunktion auf dieser<br>Kategorie zur Zeit ausgeschaltet";
					}
					else
					{
						lblNofifSubscriptionDisabled.Text = "Du hast die Abonnierungsfunktion<br>zur Zeit ausgeschaltet";
					}
					lblNofifSubscriptionDisabled.Visible = true;
				}
				else
				{
					chkNotifSubscription.Checked = false;
					txtNumSmsNotifications.Text = string.Empty;
					chkNotifSubscription.Visible = false;
					txtNumSmsNotifications.Visible = false;
					numSmsNotificationsLabel.Visible = false;

					lblNofifSubscriptionDisabled.Text = "Momentan ist kein Anlass ausgewählt";
					lblNofifSubscriptionDisabled.Visible = true;
				}
			}

			events.Visible = !BLL.Mandator.ShowEventsAsList;
			eventList.Visible = BLL.Mandator.ShowEventsAsList;

			EventDetails_EventUrlLabel.Visible = BLL.Mandator.UseEventUrl;
			EventDetails_EventUrlControl.Visible = BLL.Mandator.UseEventUrl;
			EventDetails_MinMaxSubscriptions.Visible = BLL.Mandator.UseMinMaxSubscriptions;
			EventDetails_EventCategory.Visible = BLL.Mandator.UseEventCategories;
			EventCategoryFilterPopup.Visible = BLL.Mandator.UseEventCategories;
		}

		#endregion
		#endregion

		#region Properties
		private Event CurrentEvent
		{
			get { return (Event)ViewState[curEventViewStateKey]; }
			set
			{
				//set notifSubscriptions to null because they don't need to be in viewstate
				if(value != null)
				{
					value.SmsNotifSubscriptions = null;
				}
				ViewState[curEventViewStateKey] = value;
			}
		}

		private bool IsCategoryFilterOn
		{
			get { return (bool)ViewState[isCategoryFilterOnViewStateKey]; }
			set { ViewState[isCategoryFilterOnViewStateKey] = value; }
		}

		private bool ShowOldEvents
		{
			get { return (bool)ViewState[showOldEventsViewStateKey]; }
			set { ViewState[showOldEventsViewStateKey] = value; }
		}

		private bool IsOldEvent
		{
			get { return (CurrentEvent == null ? true : CurrentEvent.StartDate.Date < DateTime.Now.Date); }
		}
		#endregion

		#region Private Methods

		private void UpdateEventList()
		{
			string selBefore = (BLL.Mandator.ShowEventsAsList ? eventList.SelectedValue : events.SelectedValue);

			if(ShowOldEvents)
			{
				showOldEventsButton.AlternateText = "Vergangene Anlässe ausblenden";
				showOldEventsButton.ToolTip = "Vergangene Anlässe ausblenden";
				showOldEventsButton.ImageUrl = "images/icons/show_new.gif";

				if(IsCategoryFilterOn)
				{
					events.DataSource = BLL.ListEvents(CurrentEventCategory, Event.AscendingComparer);
				}
				else
				{
					events.DataSource = BLL.ListEvents(Event.AscendingComparer);
				}
				events.DataBind();
				eventList.DataSource = events.DataSource;
				eventList.DataBind();
			}
			else
			{
				showOldEventsButton.AlternateText = "Vergangene Anlässe auch anzeigen";
				showOldEventsButton.ToolTip = "Vergangene Anlässe auch anzeigen";
				showOldEventsButton.ImageUrl = "images/icons/show_old.gif";

				if(IsCategoryFilterOn)
				{
					events.DataSource = BLL.ListFutureEvents(CurrentEventCategory, Event.AscendingComparer);
				}
				else
				{
					events.DataSource = BLL.ListFutureEvents(Event.AscendingComparer);
				}
				events.DataBind();
				eventList.DataSource = events.DataSource;
				eventList.DataBind();
			}

			if(events.Items.Count > 0)
			{
				events.SelectedIndex = 0;
				eventList.SelectedIndex = 0;
				try
				{
					events.SelectedValue = selBefore;
					eventList.SelectedValue = selBefore;
				}
				catch
				{
					events_SelectedIndexChanged((BLL.Mandator.ShowEventsAsList ? (ListControl)eventList : (ListControl)events), null);
					return;
				}
			}
			else
			{
				CurrentEvent = null;
			}
		}

		private EventCategory CurrentEventCategory
		{
			get
			{
				if(!IsCategoryFilterOn)
				{
					return null;
				}
				if(currentEventCategory == null)
				{
					currentEventCategory = BLL.GetEventCategory(Int32.Parse(EventCategories.SelectedValue));
				}
				return currentEventCategory;
			}
		}
		private EventCategory currentEventCategory = null;

		private	void ShowAddNewContact()
		{
			pnlAddSubscription.Visible = false;
			pnlSubscriptions.Visible = false;
			pnlAddContact.Visible = true;
			txtEmail.Text = email.Text;

			chkNotifyBySms.Visible = BLL.Mandator.SmsNotifications;
			chkNotifyByEmail.Visible = BLL.Mandator.OnNewEventNotifyContacts
							|| BLL.Mandator.OnEditEventNotifyContacts
							|| BLL.Mandator.OnNewSubscriptionNotifyContacts
							|| BLL.Mandator.OnEditSubscriptionNotifyContacts
							|| BLL.Mandator.OnDeleteSubscriptionNotifyContacts;
		}

		private void UpdateSubscriptionList()
		{
			UpdateSubscriptionList((CurrentEvent != null ? EventSiteBL.ListSubscriptions(CurrentEvent) : null));
		}

		private void UpdateSubscriptionList(List<Subscription> subscriptions)
		{
			dgrSubscriptions.DataSource = subscriptions;
		}

		protected bool ModifyAllowed(int contactId)
		{
			return !BLL.IsReader() && (BLL.IsManager() || BLL.IsAdministrator() || BLL.IsCurrentUser(contactId));
		}

		private void FillEventDetailFields()
		{
			if(CurrentEvent == null)
			{
				EventCategoryLabel.Text = "&nbsp;";
				TitleLabel.Text = "&nbsp;";
				DescriptionLabel.Text = "&nbsp;";
				UrlLink.NavigateUrl = string.Empty;
				UrlLink.Text = string.Empty;
				UrlLink.Target = "_blank";
				LocationLabel.Text = "&nbsp;";
				LocationLabel.ToolTip = string.Empty;
				StartDateLabel.Text = "&nbsp;";
				DurationLabel.Text = "&nbsp;";
				MinSubscriptionsLabel.Text = "&nbsp;";
				MaxSubscriptionsLabel.Text = "&nbsp;";
			}
			else
			{
				EventCategoryLabel.Text = HttpUtility.HtmlEncode(CurrentEvent.EventCategory.Category);
				EventCategoryLabel.ToolTip = CurrentEvent.EventCategory.CategoryDescription;
				TitleLabel.Text = HttpUtility.HtmlEncode(CurrentEvent.EventTitle);
				DescriptionLabel.Text = HttpUtility.HtmlEncode(CurrentEvent.EventDescription ?? String.Empty).Replace("\r\n", "\n").Replace("\n", "<br/>") + "&nbsp;";
				UrlLink.NavigateUrl = CurrentEvent.EventUrl;
				UrlLink.Text = CurrentEvent.EventUrl;
				UrlLink.Target = "_blank";
				EventCreator.Contact = CurrentEvent.EventCreator;
				LocationLabel.Text = HttpUtility.HtmlEncode(CurrentEvent.Location.LocationText);
				LocationLabel.ToolTip = CurrentEvent.Location.LocationDescription;
				if(Request.UserAgent.IndexOf("MSIE") > -1)
				{
					LocationLabel.ToolTip = LocationLabel.ToolTip.Replace(", ", ",").Replace(",", ",\r\n");
				}
				StartDateLabel.Text = CurrentEvent.StartDate.ToString("dd.MM.yyyy HH:mm");
				DurationLabel.Text = CurrentEvent.Duration + "&nbsp;";
				MinSubscriptionsLabel.Text = CurrentEvent.MinSubscriptions.IsNull ? "&nbsp;" : CurrentEvent.MinSubscriptions.ToString();
				MaxSubscriptionsLabel.Text = CurrentEvent.MaxSubscriptions.IsNull ? "&nbsp;" : CurrentEvent.MaxSubscriptions.ToString();
			}
		}

		private void SetNotifSubscriptionFields()
		{
			EventSiteBL.SetSmsNotifSubscriptions(CurrentEvent);
			foreach (SmsNotifSubscription notifSubscription in CurrentEvent.SmsNotifSubscriptions)
			{
				if(notifSubscription.Contact.ContactId.Equals(BLL.CurrentContact.ContactId))
				{
					chkNotifSubscription.Checked = true;
					txtNumSmsNotifications.Text = notifSubscription.MaxNotifications.IsNull ? "" : notifSubscription.MaxNotifications.ToString();
					return;
				}
			}
			chkNotifSubscription.Checked = false;
			txtNumSmsNotifications.Text = string.Empty;
		}
		#endregion

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
			this.events.SelectedIndexChanged += new System.EventHandler(this.events_SelectedIndexChanged);
			this.eventList.SelectedIndexChanged += new System.EventHandler(this.events_SelectedIndexChanged);
			this.refreshEventButton.Click += new System.Web.UI.ImageClickEventHandler(this.refreshEventButton_Click);
			this.showOldEventsButton.Click += new System.Web.UI.ImageClickEventHandler(this.showOldEventsButton_Click);
			this.dgrSubscriptions.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DgrSubscriptions_ItemDataBound);
			this.AddContact.Click += new System.EventHandler(this.AddContact_Click);
			this.AddSubscription.Click += new System.EventHandler(this.AddSubscription_Click);
			this.chkCategoryFilterOn.CheckedChanged += new System.EventHandler(this.ChkCategoryFilterOn_CheckedChanged);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Default_PreRender);

		}

		#endregion
	}
}
