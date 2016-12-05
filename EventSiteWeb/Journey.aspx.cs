using System;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using kcm.ch.EventSite.BusinessLayer;
using kcm.ch.EventSite.Common;
using pbHelpers=playboater.gallery.commons.Helpers;

namespace kcm.ch.EventSite.Web
{
	/// <summary>
	/// Summary description for Journey.
	/// </summary>
	public class Journey : PageBase
	{
		protected CheckBox IsDriving;
		protected TextBox NumLifts;
		protected DropDownList PastJourneys;
		protected LinkButton ApplyPastJourney;
		protected RangeValidator NumLiftsValidator;
		protected ValidationSummary ValSummary;
		//protected TextBox NewJourneyStationTime;
		protected TextBox NewJourneyStation;
		protected Button AddJourneyStation;
		protected Button SaveButton;
		protected DataGrid JourneyStations;
		protected TextBox JourneyStartTime;
		protected RequiredFieldValidator JourneyStartTimeValidator;
		protected CustomValidator JourneyStationsValidator;
		protected HtmlGenericControl pageTitle;

		private void Page_Load(object sender, EventArgs e)
		{
			//reponse.exires to avoid caching in modal dialog
			Response.Expires = -1;
			//Response.Cache.SetNoStore();

			if(!BLL.Mandator.IsLiftManagementEnabled)
			{
				throw new EventSiteException("Dieses Feature ist auf diesem Mandanten ausgeschaltet!", -1);
			}

			if (!IsPostBack)
			{
				string sid = Request.QueryString["sid"];
				if (sid == null || sid == string.Empty)
				{
					throw new EventSiteException("QueryString Parameter 'sid' not found!", 900);
				}
				JourneySubscription = BLL.GetSubscription(Convert.ToInt32(sid));
				EventSiteBL.SetJourneyStations(JourneySubscription);

				pageTitle.InnerText = string.Format("Route von {0} definieren", JourneySubscription.Contact.Name);

				IsDriving.Checked = JourneySubscription.SubscriptionLiftState == LiftState.drives ||
					JourneySubscription.SubscriptionLiftState == LiftState.full;
				if (IsDriving.Checked)
				{
					NumLifts.Text = JourneySubscription.NumLifts.ToString();
					if(JourneySubscription.JourneyStations.Count > 0)
					{
						JourneyStartTime.Text = ((JourneyStation)JourneySubscription.JourneyStations[0]).StationTime;
					}
				}
				
				ContactsPastJourneys = BLL.GetPastJourneys(JourneySubscription.Contact);
				FillPastJourneys();
			}

			if(JourneySubscription.SubscriptionLiftState == LiftState.lift)
			{
				RegisterStartupScriptIfNeeded("LiftShouldNotDrive", string.Format(pbHelpers.JavaScriptString, string.Format("alert('Da {0} schon bei jemandem mitfährt, kann nicht\\nnoch eine Route definiert werden. Bei Bedarf muss\\ndie Mitfahrt zuerst entfernt werden.');fClose();", JourneySubscription.Contact.Name)));
			}
		}

		private void IsDriving_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void ApplyPastJourney_Click(object sender, EventArgs e)
		{
			JourneyStation[] stations = (JourneyStation[])ContactsPastJourneys[PastJourneys.SelectedValue];
			JourneySubscription.journeyStations.Clear();
			JourneySubscription.journeyStations.AddRange(stations);
		}

		private void AddJourneyStation_Click(object sender, EventArgs e)
		{
			if(NewJourneyStation.Text.Trim() == string.Empty)
			{
				RegisterStartupScriptIfNeeded("StationEmpty", string.Format(pbHelpers.JavaScriptAlertString, "Der Routenpunkt darf nicht leer sein!"));
				return;
			}
			Subscription subscription = JourneySubscription;
			subscription.AddStation(new JourneyStation(NewJourneyStation.Text.Trim(), "", JourneySubscription.JourneyStations.Count + 1));
			NewJourneyStation.Text = string.Empty;
		}

		private void JourneyStations_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			Subscription subscription = JourneySubscription;
			try
			{
				switch (e.CommandName)
				{
					case "MoveUp":
						subscription.ReOrderStations((JourneyStation) subscription.JourneyStations[e.Item.ItemIndex], true);
						break;
					case "MoveDown":
						subscription.ReOrderStations((JourneyStation) subscription.JourneyStations[e.Item.ItemIndex], false);
						break;
					case "Delete":
						if(BLL.HasJourneyStationLiftsSet((JourneyStation) subscription.JourneyStations[e.Item.ItemIndex]))
						{
							throw new EventSiteException("Da auf diesem Routenpunkt schon Mitfahrten gesetzt\\nsind, kann er nicht gelöscht werden.", -1);
						}
						subscription.RemoveStation((JourneyStation) subscription.JourneyStations[e.Item.ItemIndex]);
						break;
					default:
						throw new EventSiteException("Dieser CommandName ist unbekannt: \"" + e.CommandName + "\"", -1);
				}
			}
			catch (EventSiteException ex)
			{
				RegisterStartupScriptIfNeeded("JourneyStations_Rearrange_Error", ex.JavaScriptAlertString);
			}
		}

		private void SaveButton_Click(object sender, EventArgs e)
		{
			if(Page.IsValid)
			{
				try
				{
					bool notifyChange = false;
					if (IsDriving.Checked)
					{
						JourneySubscription.NumLifts = Convert.ToInt32(NumLifts.Text);
						JourneySubscription.SetJourneyStartTime(JourneyStartTime.Text);
						JourneySubscription.LiftSubscriptionJourneyStationId = null;
						notifyChange = BLL.HasSubscriptionLiftsSet(JourneySubscription);
					}
					else
					{
						if(BLL.HasSubscriptionLiftsSet(JourneySubscription))
						{
							RegisterStartupScriptIfNeeded("JourneySaveFailed", string.Format(pbHelpers.JavaScriptAlertString, "Diese Route kann nicht gelöscht werden,\\nda schon Personen definiert sind,\\nwelche mitfahren."));
							return;
						}
						JourneySubscription.NumLifts = null;
						foreach (JourneyStation journeyStation in JourneySubscription.journeyStations)
						{
							journeyStation.SortOrder = -1;
						}
					}

					BLL.EditSubscription(JourneySubscription, false);

					if(notifyChange)
					{
						//notify all lifts about the change
						BLL.NofityJourneyChange(JourneySubscription);
					}
#if(DEBUG)
					System.Threading.Thread.Sleep(2500);
#endif

					RegisterStartupScriptIfNeeded("JourneySaveSuccess", string.Format(pbHelpers.JavaScriptString, "alert('Routendefinition erfolgreich gespeichert.');fClose();"));
				}
				catch (EventSiteException ex)
				{
					RegisterStartupScriptIfNeeded("SaveJourneyExc", ex.JavaScriptAlertString);
					JourneySubscription = BLL.GetSubscription(JourneySubscription.SubscriptionId);
					EventSiteBL.SetJourneyStations(JourneySubscription);
				}
			}
		}

		private void Journey_PreRender(object sender, EventArgs e)
		{
			if(BLL.IsUser() && !BLL.IsCurrentUser(JourneySubscription.Contact.ContactId) || BLL.IsReader())
			{
				//disable all fields
				IsDriving.Enabled = false;
				NumLifts.Enabled = false;
				JourneyStartTimeValidator.Enabled = false;
				JourneyStationsValidator.Enabled = false;
				PastJourneys.Enabled = false;
				ApplyPastJourney.Enabled = false;
				AddJourneyStation.Enabled = false;
				JourneyStartTime.Enabled = false;
				NewJourneyStation.Enabled = false;
				JourneyStations.Enabled = false;
				SaveButton.Enabled = false;
			}
			else
			{
				if(IsDriving.Checked)
				{
					NumLifts.Enabled = true;
					JourneyStartTimeValidator.Enabled = true;
					JourneyStationsValidator.Enabled = true;
					PastJourneys.Enabled = true;
					ApplyPastJourney.Enabled = true;
					AddJourneyStation.Enabled = true;
					JourneyStartTime.Enabled = true;
					NewJourneyStation.Enabled = true;
					JourneyStations.Enabled = true;

					if(PastJourneys.Items.Count == 0)
					{
						ApplyPastJourney.Enabled = false;
					}
				}
				else
				{
					NumLifts.Enabled = false;
					JourneyStartTimeValidator.Enabled = false;
					JourneyStationsValidator.Enabled = false;
					PastJourneys.Enabled = false;
					ApplyPastJourney.Enabled = false;
					AddJourneyStation.Enabled = false;
					JourneyStartTime.Enabled = false;
					NewJourneyStation.Enabled = false;
					JourneyStations.Enabled = false;
				}
			}

			JourneyStations.DataSource = JourneySubscription.JourneyStations;
			JourneyStations.DataBind();
		}

		private void JourneyStationsValidator_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			args.IsValid = JourneySubscription.JourneyStations.Count > 0;
		}

		public Subscription JourneySubscription
		{
			get
			{
				Subscription tmp;
				try
				{
					tmp = (Subscription) ViewState["EventSite_Journey_Subscription"];
					if (tmp == null)
						throw new Exception();
				}
				catch
				{
					throw new NullReferenceException("Property \"Subscription\" is not set!");
				}
				return tmp;
			}
			set { ViewState["EventSite_Journey_Subscription"] = value; }
		}

		public Hashtable ContactsPastJourneys
		{
			get
			{
				Hashtable tmp;
				try
				{
					tmp = (Hashtable)ViewState["EventSite_Journey_ContactsPastJourneys"];
					if(tmp == null)
						throw new Exception();
				}
				catch
				{
					throw new NullReferenceException("Property \"ContactsPastJourneys\" is not set!");
				}
				return tmp;
			}
			set { ViewState["EventSite_Journey_ContactsPastJourneys"] = value; }
		}

		private void FillPastJourneys()
		{
			PastJourneys.DataSource = ContactsPastJourneys.Keys;
			PastJourneys.DataBind();
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
			this.IsDriving.CheckedChanged += new System.EventHandler(this.IsDriving_CheckedChanged);
			this.ApplyPastJourney.Click += new System.EventHandler(this.ApplyPastJourney_Click);
			this.JourneyStations.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.JourneyStations_ItemCommand);
			this.JourneyStationsValidator.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.JourneyStationsValidator_ServerValidate);
			this.AddJourneyStation.Click += new System.EventHandler(this.AddJourneyStation_Click);
			this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Journey_PreRender);

		}

		#endregion
	}
}