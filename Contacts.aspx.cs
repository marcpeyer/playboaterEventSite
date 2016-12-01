using System;
using System.Collections;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using kcm.ch.EventSite.Common;
using kcm.ch.EventSite.Web;
using kcm.ch.EventSite.Web.modules;
using pbHelpers=playboater.gallery.commons.Helpers;

namespace kcm.ch.EventSite.Web
{
	/// <summary>
	/// Summary description for Contacts.
	/// </summary>
	public class Contacts : PageBase
	{
		protected DataGrid dgrContacts;
		protected HtmlInputHidden hdnDeleteConfiremd;
		protected Button DeleteContact;
		protected HtmlGenericControl pageTitle;
		protected HtmlGenericControl title;
		protected Navigation PageNavigation;
		protected System.Web.UI.WebControls.Button LogoutButton;
		protected HtmlGenericControl NameDesc;
		protected HtmlGenericControl MailDesc;
		protected HtmlGenericControl MobileDesc;
		protected HtmlGenericControl NotifyBySmsDesc;
		protected HtmlGenericControl NotifyByMailDesc;
		protected HtmlGenericControl NotifSubscriptionDesc;
		protected HtmlGenericControl SmsCreditDesc;

		private const string idToDelViewStateKey = "EventSite.Id2Del";

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack)
			{
			}
		}

		private void dgrContacts_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgrContacts.EditItemIndex = e.Item.ItemIndex;
		}

		private void dgrContacts_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			ViewState[idToDelViewStateKey] = Convert.ToInt32(((Label)e.Item.FindControl("ContactIdLabel")).Text); //contactId, nicht index!!!
//			string script = string.Format(Helpers.JavaScriptString, "if(confirm('Das Löschen von Kontakten entfernt auch\\nalle Anmeldungen dieses Kontaktes!\\nFortfahren?'))\n\tdocument.getElementById('"+DeleteContact.ClientID+"').click();");
			string script = string.Format(pbHelpers.JavaScriptString, "if(confirm('Bist du sicher, dass du diesen Kontakt löschen willst?\\nSomit werden keine Benachrichtigungen mehr verschickt,\\nweder bei neuen Events noch bei neuen Anmeldungen.'))\n\tdocument.getElementById('"+DeleteContact.ClientID+"').click();");
			RegisterStartupScriptIfNeeded("deleteConfirm", script);
		}

		private void DeleteContact_Click(object sender, System.EventArgs e)
		{
//			ArrayList contacts = null;
			try
			{
//				Contact contactToDel = null;
//				foreach (Contact contact in contacts)
//				{
//					if(contact.ContactId==(int)ViewState[idToDelViewStateKey])
//					{
//						contactToDel = contact;
//					}
//				}
//				if(contactToDel == null)
//				{
//					throw new EventSiteException("Contact to delete not found!", 900);
//				}
//				else
//				{
//					BLL.DelContact(contactToDel.ContactId);
//				}
				BLL.DelContact((int)ViewState[idToDelViewStateKey]);
				BLL.RenewCurrentContact();
//				contacts = BLL.ListContacts();
			}
			catch (EventSiteException ex)
			{
				RegisterStartupScriptIfNeeded("ErrAlert", ex.JavaScriptAlertString);
				return;
			}
			dgrContacts.EditItemIndex = -1;
		}

		private void dgrContacts_CancelCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgrContacts.EditItemIndex = -1;
		}

		private void dgrContacts_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			string editName = ((TextBox)e.Item.FindControl("txtName")).Text;
			string editMail = ((TextBox)e.Item.FindControl("txtEmail")).Text;
			string editMobile = ((TextBox)e.Item.FindControl("txtMobilePhone")).Text;

			ArrayList contacts = BLL.ListContacts();

			Contact contactToUpdate = null;
			contactToUpdate = (Contact)contacts[e.Item.ItemIndex];

			if(contactToUpdate == null)
			{
				throw new EventSiteException("Contact to update not found!", 900);
			}
			string signoutScript = null;
			if(contactToUpdate.ContactId.Equals(BLL.CurrentContact.ContactId) && !contactToUpdate.Email.Equals(editMail))
			{
				//current contact will be updated --> signout after update
				signoutScript = string.Format(pbHelpers.JavaScriptString, @"
	alert('Da die eigene Email-Adresse geändert wurde, musst Du dich neu anmelden.');
	document.getElementById('" + LogoutButton.ClientID + @"').click();
");
			}
			try
			{
				contactToUpdate.Name = editName;
				contactToUpdate.Email = editMail;
				contactToUpdate.MobilePhone = editMobile;
			}
			catch (EventSiteException ex)
			{
				RegisterStartupScriptIfNeeded("invalContactData", ex.JavaScriptAlertString);
				return;
			}
			//save contact in db
			try
			{
				BLL.EditContact(contactToUpdate);

				if(signoutScript != null)
				{
					RegisterStartupScriptIfNeeded("signoutScript", signoutScript);
				}
			}
			catch (EventSiteException ex)
			{
				RegisterStartupScriptIfNeeded("ErrAlert", ex.JavaScriptAlertString);
				return;
			}
			BLL.RenewCurrentContact();
			dgrContacts.EditItemIndex = -1;
		}

		private void dgrContacts_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			if(e.CommandName.Equals("Insert"))
			{
				string newName = ((TextBox)e.Item.FindControl("newName")).Text;
				string newMail = ((TextBox)e.Item.FindControl("newMail")).Text;
				string newMobile = ((TextBox)e.Item.FindControl("newMobile")).Text;
				Contact newContact = null;
				try
				{
					//create contact
					newContact = new Contact(BLL.Mandator, newName, newMail, newMobile, false, false, false, 0, 0, false, "");
				}
				catch (EventSiteException ex)
				{
					RegisterStartupScriptIfNeeded("invalContactData", ex.JavaScriptAlertString);
					return;
				}
				//save new contact to db
				try
				{
					string userInfoText;
					userInfoText = BLL.AddContact(newContact);
					if(userInfoText != null)
					{
						RegisterStartupScriptIfNeeded("AddContactUserInfo", string.Format(pbHelpers.JavaScriptAlertString, userInfoText));
					}
				}
				catch (EventSiteException ex)
				{
					RegisterStartupScriptIfNeeded("ErrAlert", ex.JavaScriptAlertString);
					return;
				}
				dgrContacts.EditItemIndex = -1;
			}
		}

		private void LogoutButton_Click(object sender, System.EventArgs e)
		{
			FormsAuthentication.SignOut();
			Response.Redirect("Login.aspx?mid=" + BLL.Mandator.MandatorId);
		}

		private void Contacts_PreRender(object sender, EventArgs e)
		{
			dgrContacts.DataSource = BLL.ListContacts();
			dgrContacts.DataBind();

			title.InnerText = BLL.Mandator.SiteTitle + " - " + "Kontakt-Administration";
			pageTitle.InnerText = BLL.Mandator.SiteTitle + " - " + "Kontakt-Administration";

			dgrContacts.ShowFooter = BLL.IsManager() || BLL.IsAdministrator();

			bool showSmsCols = BLL.Mandator.SmsNotifications;
//			bool showEmailCols = BLL.Mandator.OnNewEventNotifyContacts
//							|| BLL.Mandator.OnEditEventNotifyContacts
//							|| BLL.Mandator.OnNewSubscriptionNotifyContacts
//							|| BLL.Mandator.OnEditSubscriptionNotifyContacts
//							|| BLL.Mandator.OnDeleteSubscriptionNotifyContacts;
			foreach (DataGridColumn column in dgrContacts.Columns)
			{
				switch(column.HeaderText)
				{
//					case "Notifizierung<br>als SMS":
					case "SMS Kredit":
						column.Visible = showSmsCols;
						break;
//					case "Notifizierungs-<br>abonnierung":
//					case "Notifizierung<br>als E-Mail":
//						column.Visible = showEmailCols;
//						break;
				}
			}
//			NotifyByMailDesc.Visible = showEmailCols;
//			NotifyBySmsDesc.Visible = showSmsCols;
//			NotifSubscriptionDesc.Visible = showSmsCols;
			SmsCreditDesc.Visible = showSmsCols;

			RegisterStartupScriptIfNeeded("disableEnterScript", String.Format(pbHelpers.JavaScriptString, @"document.onkeydown=function (evnt) {
		var keycode;
		if (!evnt)
			evnt = window.event;
		if (evnt.which) {
			keycode = evnt.which;
		} else if (evnt.keyCode) {
			keycode = evnt.keyCode;
		} 
		if(keycode==13) return false;

		return true;
  }"));
		}

		protected bool ModifyAllowed(int contactId)
		{
			bool allowed = false;
			try
			{
				allowed = (BLL.IsManager() || BLL.IsAdministrator() || BLL.IsCurrentUser(contactId));
			}
			catch{}
			return allowed;
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
			this.dgrContacts.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgrContacts_ItemCommand);
			this.dgrContacts.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgrContacts_CancelCommand);
			this.dgrContacts.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgrContacts_EditCommand);
			this.dgrContacts.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgrContacts_UpdateCommand);
			this.dgrContacts.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgrContacts_DeleteCommand);
			this.DeleteContact.Click += new System.EventHandler(this.DeleteContact_Click);
			this.LogoutButton.Click += new System.EventHandler(this.LogoutButton_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.Contacts_PreRender);

		}
		#endregion
	}
}
