using System;
using System.Collections;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using kcm.ch.EventSite.BusinessLayer;
using kcm.ch.EventSite.Common;
using kcm.ch.EventSite.Web.modules;
using playboater.gallery.commons;
using pbHelpers=playboater.gallery.commons.Helpers;

namespace kcm.ch.EventSite.Web
{
	/// <summary>
	/// Summary description for MyDetails.
	/// </summary>
	public class MyDetails : PageBase
	{
		protected HtmlGenericControl pageTitle;
		protected HtmlGenericControl title;
		protected Navigation PageNavigation;

		protected string userName = "";
		protected string userEmail = "";
		protected string userRoles = "";
		protected HtmlGenericControl ChangePasswordToggle;
		protected TextBox newLogin;
		protected TextBox newPassword;
		protected TextBox newPasswordRepeat;
		protected TextBox Name;
		protected TextBox Email;
		protected TextBox MobilePhone;
		protected DataList EventCategoryList;
		protected CheckBox LiftMgmtSmsOn;
		protected CheckBox EventMgmtSmsOn;
		protected Button LogoutButton;
		protected Button ChangePasswordButton;
		protected HtmlTableRow SmsCreditRow;
		protected HtmlGenericControl NotifyBySmsDesc;
		protected HtmlGenericControl NotifyByMailDesc;
		protected HtmlGenericControl NotifSubscriptionDesc;
		protected HtmlGenericControl AutoNotifSubscriptionDesc;
		protected HtmlTable GlobalSmsSettings;
		protected HtmlTable ChangePasswordTable;
		protected HtmlTableRow LiftMgmtSettings;
		protected HtmlTableRow EventMgmtSettings;

		protected int userSmsCredit = 0;

		private void Page_Load(object sender, EventArgs e)
		{
			if(!IsPostBack)
			{
				FillFieldValues();
			}
		}

		private void EventCategoryList_ItemDataBound(object sender, DataListItemEventArgs e)
		{
			if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				bool showSmsCols = BLL.Mandator.SmsNotifications;
				bool showEmailCols = BLL.Mandator.OnNewEventNotifyContacts
					|| BLL.Mandator.OnEditEventNotifyContacts
					|| BLL.Mandator.OnNewSubscriptionNotifyContacts
					|| BLL.Mandator.OnEditSubscriptionNotifyContacts
					|| BLL.Mandator.OnDeleteSubscriptionNotifyContacts;

				Hashtable categoryControls = new Hashtable();
				EventCategory cat = (EventCategory)e.Item.DataItem;
//				e.Item.AccessKey = cat.EventCategoryId.ToString();
				e.Item.Controls.AddAt(0, new Label());
				string formatString = cat.CategoryDescription == String.Empty || cat.CategoryDescription == null
					? "{0}:" : "{0} ({1}):";
				((Label)e.Item.Controls[0]).Text = String.Format(formatString, cat.Category, cat.CategoryDescription);
				((Label)e.Item.Controls[0]).ToolTip = cat.CategoryDescription;
				((Label)e.Item.Controls[0]).Style.Add("color", "#1E6EC3");

				if(cat.FreeEventSmsNotifications)
				{
					Label freeSmsLbl = new Label();
					e.Item.Controls.Add(freeSmsLbl);
					freeSmsLbl.Text = " [Gratis SMS beim Erstellen von Anlässen]";
					freeSmsLbl.Style.Add("color", "#666666");
				}

				ContactSetting setting;
				setting = (ContactSetting)BLL.CurrentContactSettings[cat.EventCategoryId];

				Panel pnl = new Panel();
				pnl.Wrap = false;
				pnl.Style.Add("margin-top", "3px");
				pnl.Style.Add("margin-bottom", "12px");
				e.Item.Controls.Add(pnl);
				pnl.Style.Add("PADDING-LEFT", "20px");
				CheckBox chk;

				if(showEmailCols)
				{
					chk = new CheckBox();
					chk.ID = String.Format("chkNotifyByEmail_{0}", cat.EventCategoryId);
					chk.Style.Add("MARGIN-RIGHT", "10px");
					chk.Text = "Email Benachrichtigung";
					chk.Checked = setting.NotifyByEmail;
					pnl.Controls.Add(chk);
					categoryControls.Add(chk.ID, chk.UniqueID);
				}

				if(showSmsCols)
				{
					chk = new CheckBox();
					chk.ID = String.Format("chkNotifyBySms_{0}", cat.EventCategoryId);
					chk.Style.Add("MARGIN-RIGHT", "10px");
					chk.Text = "SMS Benachrichtigung";
					chk.Checked = setting.NotifyBySms;
					pnl.Controls.Add(chk);
					categoryControls.Add(chk.ID, chk.UniqueID);

					chk = new CheckBox();
					chk.ID = String.Format("chkSmsNotifSubscription_{0}", cat.EventCategoryId);
					chk.Style.Add("MARGIN-RIGHT", "10px");
					chk.Text = "SMS Abonnierungsfunktion verwenden";
					chk.Checked = setting.SmsNotifSubscriptionsOn;
					pnl.Controls.Add(chk);
					categoryControls.Add(chk.ID, chk.UniqueID);

					//auto notification control
					TextBox tb = new TextBox();
					tb.ID = String.Format("tbAutoNotifSubscription_{0}", cat.EventCategoryId);
					tb.Style.Add("MARGIN-RIGHT", "3px");
					tb.Style.Add("WIDTH", "20px");
					tb.Text = setting.AutoNotifSubscription.IsNull ? String.Empty : setting.AutoNotifSubscription.ToString();
					pnl.Controls.Add(tb);
					HtmlGenericControl autoNotifLbl = new HtmlGenericControl("LABEL");
//					Label autoNotifLbl = new Label();
//					autoNotifLbl.Text = "Automatische Abonnierung";
					autoNotifLbl.InnerText = "Automatische Abonnierung";
					autoNotifLbl.Attributes.Add("for", tb.ClientID);
					autoNotifLbl.Attributes.Add("id", tb.ClientID + "_label");
					pnl.Controls.Add(autoNotifLbl);
					categoryControls.Add(tb.ID, tb.UniqueID);
					if(!setting.SmsNotifSubscriptionsOn)
					{
						tb.Enabled = false;
						autoNotifLbl.Attributes.Add("disabled", "");
					}

					//client script to disable autoNotif control when SmsNotifSubscriptionsOn is turned off
					chk.Attributes.Add("onclick", String.Format("fSetAutoNotifSubscrDisabledState(this, '{0}', '{1}');", tb.ClientID, tb.ClientID + "_label"));
				}

				ViewState[String.Format("ES_CategoryControls_{0}", cat.EventCategoryId)] = categoryControls;
			}
		}

		private void EventCategoryList_ItemCommand(object source, DataListCommandEventArgs e)
		{
			if(e.CommandName == "Save")
			{
				Save();
			}
		}

		private void LogoutButton_Click(object sender, EventArgs e)
		{
			FormsAuthentication.SignOut();
			Response.Redirect("Login.aspx?mid=" + BLL.Mandator.MandatorId);
		}

		private void ChangePasswordButton_Click(object sender, EventArgs e)
		{
			try
			{
				if(BLL.Mandator.UseExternalAuth)
				{
					throw new PlayboaterException("Der Mandant verwendet externe Authentifizierung. Ändern des Passwortes nur in externem System möglich.", 190);
				}
				if(newPassword.Text != newPasswordRepeat.Text)
				{
					throw new PlayboaterException("Die beiden Passwörter stimmen nicht überein.", 191);
				}

				BLL.ChangePassword(BLL.CurrentContact, newLogin.Text, newPassword.Text);
				BLL.RenewCurrentContact();
				FillFieldValues();

				RegisterStartupScriptIfNeeded("changePwdSuccess", String.Format(playboater.gallery.commons.Helpers.JavaScriptAlertString, "Zugangsdaten erfolgreich gespeichert."));
			}
			catch (PlayboaterException ex)
			{
				RegisterStartupScriptIfNeeded("errorChangePwd", ex.JavaScriptAlertString);
				return;
			}
		}

		private void MyDetails_PreRender(object sender, EventArgs e)
		{
			string titleFormat = "{0}'s Details";
			title.InnerText = String.Format(titleFormat, userName);
			pageTitle.InnerText = String.Format(titleFormat, userName);

			bool showSmsCols = BLL.Mandator.SmsNotifications;
			bool showEmailCols = BLL.Mandator.OnNewEventNotifyContacts
							|| BLL.Mandator.OnEditEventNotifyContacts
							|| BLL.Mandator.OnNewSubscriptionNotifyContacts
							|| BLL.Mandator.OnEditSubscriptionNotifyContacts
							|| BLL.Mandator.OnDeleteSubscriptionNotifyContacts;

			if(!showSmsCols && !showEmailCols)
			{
				EventCategoryList.HeaderStyle.CssClass = "hidden";
				EventCategoryList.ItemStyle.CssClass = "hidden";
				EventCategoryList.AlternatingItemStyle.CssClass = "hidden";
			}

			LiftMgmtSettings.Visible = BLL.Mandator.IsLiftManagementEnabled;
			EventMgmtSettings.Visible = showSmsCols;
			GlobalSmsSettings.Visible = LiftMgmtSettings.Visible || EventMgmtSettings.Visible;

			SmsCreditRow.Visible = showSmsCols;
			NotifyByMailDesc.Visible = showEmailCols;
			NotifyBySmsDesc.Visible = showSmsCols;
			NotifSubscriptionDesc.Visible = showSmsCols;
			AutoNotifSubscriptionDesc.Visible = showSmsCols;

			string autoNotifSubscrDisabledStateScript = String.Format(pbHelpers.JavaScriptString,
				@"function fSetAutoNotifSubscrDisabledState(chkElem, elem1Id, elem2Id)
				{
					var elem1 = document.getElementById(elem1Id);
					var elem2 = document.getElementById(elem2Id);
					elem1.disabled = !chkElem.checked;
					elem2.disabled = !chkElem.checked;
				}");
			RegisterStartupScriptIfNeeded("AutoNotifSubscrDisabledStateScript", autoNotifSubscrDisabledStateScript);

			ChangePasswordTable.Visible = !BLL.Mandator.UseExternalAuth;
			ChangePasswordToggle.Visible = !BLL.Mandator.UseExternalAuth;
		}

		private static bool EvaluateCheckBox(string val)
		{
			if(val == null)
				return false;
			val = val.ToLower();
			return val != null && (
				val == "on" ||
				val == "an" ||
				val == "wahr" ||
				val == "true" ||
				val == "ja" ||
				val == "yes");	
		}

		private static NInt32 EvaluateNInt32TextBox(string val)
		{
			NInt32 v = new NInt32(0, true);
			if(val == null || val == String.Empty)
				return v;
			try
			{
				v = Convert.ToInt32(val);
			}
			catch
			{
				throw new EventSiteException("Der angegebene Wert für \\'Automatische Abonnierung\\' ist\\nungültig. Erlaubt: Ganze positive Zahl oder leer.", -1);
			}
			return v;
		}

		private void Save()
		{
			Contact contact = BLL.CurrentContact;
			bool showSmsCols = BLL.Mandator.SmsNotifications;
			bool showEmailCols = BLL.Mandator.OnNewEventNotifyContacts
				|| BLL.Mandator.OnEditEventNotifyContacts
				|| BLL.Mandator.OnNewSubscriptionNotifyContacts
				|| BLL.Mandator.OnEditSubscriptionNotifyContacts
				|| BLL.Mandator.OnDeleteSubscriptionNotifyContacts;

			string emailBefore = contact.Email;

			try
			{
				contact.Name = Name.Text;
				contact.Email = Email.Text;
				contact.MobilePhone = MobilePhone.Text;
				contact.LiftMgmtSmsOn = LiftMgmtSmsOn.Checked;
				contact.EventMgmtSmsOn = EventMgmtSmsOn.Checked;

				foreach (EventCategory category in BLL.ListEventCategories())
				{
					ContactSetting setting = EventSiteBL.GetContactSetting(contact, category);
					Hashtable categoryControls = (Hashtable)ViewState[String.Format("ES_CategoryControls_{0}", category.EventCategoryId)];

					//check if contactsettings have changed --> if not changed set id to 0 to ignore saving this item
					bool notifByEmail = false, notifBySms = false, notifSubscr = false;
					NInt32 autoNotifSubscr = new NInt32(0, true);
					if(showEmailCols)
					{
						notifByEmail = EvaluateCheckBox(Request.Form[(string)categoryControls[String.Format("chkNotifyByEmail_{0}", category.EventCategoryId)]]);
					}
					if(showSmsCols)
					{
						notifBySms = EvaluateCheckBox(Request.Form[(string)categoryControls[String.Format("chkNotifyBySms_{0}", category.EventCategoryId)]]);
						notifSubscr = EvaluateCheckBox(Request.Form[(string)categoryControls[String.Format("chkSmsNotifSubscription_{0}", category.EventCategoryId)]]);
						autoNotifSubscr = EvaluateNInt32TextBox(Request.Form[(string)categoryControls[String.Format("tbAutoNotifSubscription_{0}", category.EventCategoryId)]]);
					}

					if(showEmailCols && setting.NotifyByEmail != notifByEmail
						|| showSmsCols && (setting.NotifyBySms != notifBySms
						|| setting.SmsNotifSubscriptionsOn != notifSubscr
						|| setting.AutoNotifSubscription.IsNull != autoNotifSubscr.IsNull
						|| !setting.AutoNotifSubscription.IsNull && setting.AutoNotifSubscription != autoNotifSubscr))
					{
						if(showEmailCols)
						{
							setting.NotifyByEmail = notifByEmail;
						}
						if(showSmsCols)
						{
							setting.NotifyBySms = notifBySms;
							setting.SmsNotifSubscriptionsOn = notifSubscr;
							setting.AutoNotifSubscription = autoNotifSubscr;
						}
					}
					else
					{
						setting.ContactSettingId = 0;
					}
					//set NotifyBySms in any case, for validation if mobilephone is empty
					setting.NotifyBySms = notifBySms;
				}
			}
			catch (EventSiteException ex)
			{
				RegisterStartupScriptIfNeeded("invalContactData", ex.JavaScriptAlertString);
				BLL.RenewCurrentContact();
				FillFieldValues();
				return;
			}

			BLL.EditContact(contact);
			BLL.SaveContactSettings(contact);

			if(!contact.Email.Equals(emailBefore))
			{
				//current contact's email has changed --> signout after update
				string signoutScript = string.Format(pbHelpers.JavaScriptString, @"
	alert('Da die eigene Email-Adresse geändert wurde, musst Du dich neu anmelden.');
	document.getElementById('" + LogoutButton.ClientID + @"').click();
");
				RegisterStartupScriptIfNeeded("signoutScript", signoutScript);

				return;
			}
			else
			{
				RegisterStartupScriptIfNeeded("saveSuccess", String.Format(playboater.gallery.commons.Helpers.JavaScriptAlertString, "Einstellungen erfolgreich gespeichert."));
				BLL.RenewCurrentContact();
			}

			FillFieldValues();
		}

		private void FillFieldValues()
		{
			userName = BLL.CurrentContact.Name;
			userEmail = BLL.CurrentContact.Email;
			userRoles = BLL.GetUserRoles();
			userSmsCredit = BLL.CurrentContact.SmsPurchased - BLL.CurrentContact.SmsLog;

			Name.Text = userName;
			Email.Text = userEmail;
			MobilePhone.Text = BLL.CurrentContact.MobilePhone;

			if(!BLL.Mandator.UseExternalAuth)
			{
				newLogin.Text = BLL.CurrentContactLogin;
			}

			LiftMgmtSmsOn.Checked = BLL.CurrentContact.LiftMgmtSmsOn;
			EventMgmtSmsOn.Checked = BLL.CurrentContact.EventMgmtSmsOn;

			EventCategoryList.DataSource = BLL.ListEventCategories();
			EventCategoryList.DataBind();
		}
//
//		private DataListItem GetItemByAccessKey(string key, DataListItemCollection items)
//		{
//			foreach (DataListItem item in items)
//			{
//				if(item.AccessKey == key)
//					return item;
//			}
//			return null;
//		}

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
			this.EventCategoryList.ItemCommand += new System.Web.UI.WebControls.DataListCommandEventHandler(this.EventCategoryList_ItemCommand);
			this.EventCategoryList.ItemDataBound += new System.Web.UI.WebControls.DataListItemEventHandler(this.EventCategoryList_ItemDataBound);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.MyDetails_PreRender);
			this.LogoutButton.Click += new EventHandler(LogoutButton_Click);
			this.ChangePasswordButton.Click += new EventHandler(ChangePasswordButton_Click);
		}
		#endregion
	}
}
