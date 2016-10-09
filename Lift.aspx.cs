using System;
using System.Collections;
using System.Collections.Generic;
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
	public class Lift : PageBase
	{
		protected CheckBox TakesLift;
		protected ValidationSummary ValSummary;
		protected Button SaveButton;
		protected DropDownList LiftChoice;
		protected DropDownList JourneyStationChoice;
		protected HtmlGenericControl JourneyStartTime;
		protected System.Web.UI.WebControls.RequiredFieldValidator LiftChoiceValitator;
		protected System.Web.UI.WebControls.RequiredFieldValidator JourneyStationChoiceValidator;
		protected System.Web.UI.WebControls.TextBox Comment;
		protected System.Web.UI.WebControls.CustomValidator CommentValidator;
		protected HtmlGenericControl pageTitle;

		private enum SaveCase
		{
			saves_new_lift,
			saves_lift_but_no_change,
			saves_lift_change_same_subscription,
			saves_lift_change_other_subscription,
			saves_nolift_but_no_change,
			saves_remove_lift
		}

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

				pageTitle.InnerText = string.Format("Mitfahrt von {0} definieren", JourneySubscription.Contact.Name);

				try
				{
					FillFormFields();
				}
				catch(EventSiteException ex)
				{
					RegisterStartupScriptIfNeeded("FillFormFieldsError", ex.JavaScriptAlertString);
				}
			}

			if(BLL.HasSubscriptionLiftsSet(JourneySubscription))
			{
				RegisterStartupScriptIfNeeded("LiftShouldNotDrive", string.Format(pbHelpers.JavaScriptString, string.Format("alert('Da mit {0} schon Personen mitfahren, kann keine\\nMitfahrt definiert werden.');fClose();", JourneySubscription.Contact.Name)));
			}
		}

		private void LiftChoice_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				Subscription subscr = Subscription.GetSubscriptionById(SubscriptionsWithLifts, Convert.ToInt32(LiftChoice.SelectedValue));
				EventSiteBL.SetJourneyStations(subscr);

				JourneyStartTime.InnerText = ((JourneyStation)subscr.JourneyStations[0]).StationTime;

				JourneyStationChoice.DataSource = subscr.JourneyStations;
				JourneyStationChoice.DataTextField = "Station";
				JourneyStationChoice.DataValueField = "JourneyStationId";
				JourneyStationChoice.DataBind();

				JourneyStationChoice.Items.Insert(0, new ListItem("", ""));
			}
			catch{}
		}

		private void SaveButton_Click(object sender, EventArgs e)
		{
			if(Page.IsValid)
			{
				Subscription oldSubscription = (JourneySubscription.LiftSubscriptionJourneyStationId.IsNull ? null : GetSubscriptionByJourneyStationId(JourneySubscription.LiftSubscriptionJourneyStationId));
				Subscription newSubscription = (TakesLift.Checked ? GetSubscriptionByJourneyStationId(Convert.ToInt32(JourneyStationChoice.SelectedValue)) : null);

				SaveCase scase = SaveCase.saves_new_lift;
				if (TakesLift.Checked)
				{
					if(JourneySubscription.LiftSubscriptionJourneyStationId.IsNull)
					{
						//saves new lift
						scase = SaveCase.saves_new_lift;
					}
					else
					{
						if(JourneySubscription.LiftSubscriptionJourneyStationId == Convert.ToInt32(JourneyStationChoice.SelectedValue))
						{
							//saves but no change
							scase = SaveCase.saves_lift_but_no_change;
						}
						else
						{
							if(oldSubscription == newSubscription)
							{
								//saves a change same subscription
								scase = SaveCase.saves_lift_change_same_subscription;
							}
							else
							{
								//saves a change other subscription
								scase = SaveCase.saves_lift_change_other_subscription;
							}
						}
					}
					JourneySubscription.NumLifts = null;
					foreach (JourneyStation journeyStation in JourneySubscription.journeyStations)
					{
						journeyStation.SortOrder = -1;
					}
					JourneySubscription.LiftSubscriptionJourneyStationId = Convert.ToInt32(JourneyStationChoice.SelectedValue);
				}
				else
				{
					if(JourneySubscription.LiftSubscriptionJourneyStationId.IsNull)
					{
						//saves but no change
						scase = SaveCase.saves_nolift_but_no_change;
					}
					else
					{
						//saves remove lift
						scase = SaveCase.saves_remove_lift;
					}
					JourneySubscription.LiftSubscriptionJourneyStationId = null;
				}

				try
				{
					BLL.EditSubscription(JourneySubscription, false);
				}
				catch (EventSiteException ex)
				{
					RegisterStartupScriptIfNeeded("SaveLiftExc", ex.JavaScriptAlertString);
					JourneySubscription = BLL.GetSubscription(JourneySubscription.SubscriptionId);
					EventSiteBL.SetJourneyStations(JourneySubscription);
					
					return;
				}

				string action = "";
				string definition = "";
				string successMessage = "Der Fahrer wurde darüber informiert.";
				switch(scase)
				{
					case SaveCase.saves_new_lift:
						action = "eine neue Mitfahrt definiert:";
						definition = string.Format("{0} fährt ab {1} mit. Bemerkung: {2}",
							JourneySubscription.Contact.Name,
							JourneyStationChoice.SelectedItem.Text,
							Comment.Text);
						BLL.NotifyLiftSave(action, definition, JourneySubscription.Event, newSubscription.Contact, JourneySubscription.Contact);
						break;
					case SaveCase.saves_lift_change_same_subscription:
						action = "eine Mitfahrt geändert. Neu:";
						definition = string.Format("{0} fährt ab {1} mit. Bemerkung: {2}",
							JourneySubscription.Contact.Name,
							JourneyStationChoice.SelectedItem.Text,
							Comment.Text);
						BLL.NotifyLiftSave(action, definition, JourneySubscription.Event, newSubscription.Contact, JourneySubscription.Contact);
						break;
					case SaveCase.saves_lift_change_other_subscription:
						action = "eine Mitfahrt entfernt:";
						definition = string.Format("{0} fährt nicht mit.",
							JourneySubscription.Contact.Name);
						BLL.NotifyLiftSave(action, definition, JourneySubscription.Event,  oldSubscription.Contact, JourneySubscription.Contact);

						action = "eine neue Mitfahrt definiert:";
						definition = string.Format("{0} fährt ab {1} mit. Bemerkung: {2}",
							JourneySubscription.Contact.Name,
							JourneyStationChoice.SelectedItem.Text,
							Comment.Text);
						BLL.NotifyLiftSave(action, definition, JourneySubscription.Event, newSubscription.Contact, JourneySubscription.Contact);
						break;
					case SaveCase.saves_remove_lift:
						action = "eine Mitfahrt entfernt:";
						definition = string.Format("{0} fährt nicht mit.",
							JourneySubscription.Contact.Name);
						BLL.NotifyLiftSave(action, definition, JourneySubscription.Event,  oldSubscription.Contact, JourneySubscription.Contact);
						break;
					case SaveCase.saves_nolift_but_no_change:
					case SaveCase.saves_lift_but_no_change:
						//no notification needed
						successMessage = "Der Fahrer wurde nicht informiert, da keine Änderung gemacht wurde.";
						break;
				}

				RegisterStartupScriptIfNeeded("JourneySaveSuccess", string.Format(pbHelpers.JavaScriptString, "alert('Mitfahr-Einstellung wurde erfolgreich gespeichert.\\n\\n" + successMessage + "');fClose();"));

#if(DEBUG)
				System.Threading.Thread.Sleep(2500);
#endif
			}
		}

		private void CommentValidator_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			args.IsValid = Comment.Text.Length <= 160;
			CommentValidator.ErrorMessage = "Bemerkung darf maximal 160 Zeichen enthalten. Aktuell: " + Comment.Text.Length;
		}

		private void Journey_PreRender(object sender, EventArgs e)
		{
			if(BLL.IsUser() && !BLL.IsCurrentUser(JourneySubscription.Contact.ContactId) || BLL.IsReader())
			{
				//disable all fields
				TakesLift.Enabled = false;
				LiftChoice.Enabled = false;
				JourneyStationChoice.Enabled = false;
				LiftChoiceValitator.Enabled = false;
				JourneyStationChoiceValidator.Enabled = false;
				Comment.Enabled = false;
				CommentValidator.Enabled = false;
				SaveButton.Enabled = false;
			}
			else
			{
				if(TakesLift.Checked)
				{
					LiftChoice.Enabled = true;
					JourneyStationChoice.Enabled = true;
					LiftChoiceValitator.Enabled = true;
					JourneyStationChoiceValidator.Enabled = true;
					Comment.Enabled = true;
					CommentValidator.Enabled = true;
				}
				else
				{
					LiftChoice.Enabled = false;
					JourneyStationChoice.Enabled = false;
					JourneyStartTime.InnerText = " ";
					LiftChoiceValitator.Enabled = false;
					JourneyStationChoiceValidator.Enabled = false;
					Comment.Enabled = false;
					Comment.Text = "";
					CommentValidator.Enabled = false;
				}
			}
		}

		private void FillFormFields()
		{
			TakesLift.Checked = !JourneySubscription.LiftSubscriptionJourneyStationId.IsNull;

			LiftChoice.DataSource = SubscriptionsWithLifts;
			LiftChoice.DataTextField = "Contact";
			LiftChoice.DataValueField = "SubscriptionId";
			LiftChoice.DataBind();
			LiftChoice.Items.Insert(0, new ListItem("", ""));

			if(!JourneySubscription.LiftSubscriptionJourneyStationId.IsNull)
			{
				Subscription subscription = GetSubscriptionByJourneyStationId(Convert.ToInt32(JourneySubscription.LiftSubscriptionJourneyStationId));
				if(subscription == null)
				{
					throw new EventSiteException("Fehler beim Abfüllen der bestehenden Daten.\\nFehler: Subscription with JourneyStationId " + JourneySubscription.LiftSubscriptionJourneyStationId + " not found in SubscriptionsWithLifts", 900);
				}

				try
				{
					LiftChoice.SelectedValue = subscription.SubscriptionId.ToString();
				}
				catch{}
			}

			LiftChoice_SelectedIndexChanged(null, null);

			if(!JourneySubscription.LiftSubscriptionJourneyStationId.IsNull)
			{
				try
				{
					JourneyStationChoice.SelectedValue = JourneySubscription.LiftSubscriptionJourneyStationId.ToString();
				}
				catch{}
			}
		}

		private Subscription GetSubscriptionByJourneyStationId(int journeyStationId)
		{
			return Subscription.GetSubscriptionByJourneyStationId(SubscriptionsWithLifts, journeyStationId);
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

		public List<Subscription> SubscriptionsWithLifts
		{
			get
			{
				List<Subscription> tmp;
				try
				{
					tmp = (List<Subscription>)ViewState["EventSite_Journey_SubscriptionsWithLifts"];
					if(tmp == null)
					{
						tmp = BLL.ListSubscriptionsWithLifts(JourneySubscription);
						ViewState["EventSite_Journey_SubscriptionsWithLifts"] = tmp;
					}
				}
				catch
				{
					throw new NullReferenceException("Property \"SubscriptionsWithLifts\" is not set!");
				}
				return tmp;
			}
			set { ViewState["EventSite_Journey_SubscriptionsWithLifts"] = value; }
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
			this.LiftChoice.SelectedIndexChanged += new System.EventHandler(this.LiftChoice_SelectedIndexChanged);
			this.CommentValidator.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.CommentValidator_ServerValidate);
			this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Journey_PreRender);

		}

		#endregion
	}
}