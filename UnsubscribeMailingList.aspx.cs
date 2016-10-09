using System;
using System.Drawing;
using System.Web.UI;
using kcm.ch.EventSite.Common;
using pbHelpers=playboater.gallery.commons.Helpers;

namespace kcm.ch.EventSite.Web
{
	/// <summary>
	/// Summary description for UnsubscribeMailingList.
	/// </summary>
	public class UnsubscribeMailingList : PageBase
	{
		protected System.Web.UI.WebControls.Label EmailLabel;
		protected System.Web.UI.WebControls.Button UnsubscribeButton;
		protected System.Web.UI.WebControls.Panel StatusPanel;
		protected System.Web.UI.WebControls.TextBox EmailTextbox;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			StatusPanel.Visible = false;
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
			this.UnsubscribeButton.Click += new System.EventHandler(this.UnsubscribeButton_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void UnsubscribeButton_Click(object sender, System.EventArgs e)
		{
			string email = EmailTextbox.Text.Trim();
			if(pbHelpers.ValidateEmail(email))
			{
				//unsubscribe this email address
				if(BLL.RemoveMailFromDefaultNotificationAddresses(email))
				{
					SetSuccessStatus("Die Email-Adresse wurde erfolgreich entfernt. Änderung wird ab sofort berücksichtigt.");
				}
				else
				{
					try
					{
						Contact c = BLL.GetContact(email);
						if(c.IsDeleted)
						{
							throw new EventSiteException("", -1);
						}
						SetErrorStatus("Die Email-Adresse wurde nicht gefunden.<br>" +
							"Es existiert jedoch ein Kontakt mit dieser Email-Adresse.<br>" +
							"Somit kann in der Kontakt-Administration das Versenden von<br>" +
							"Benachrichtigungs Mails für diesen Kontakt ausgeschaltet werden.");
					}
					catch
					{
						SetErrorStatus("Die Email-Adresse wurde nicht gefunden.");
					}
				}
			}
			else
			{
				SetErrorStatus("Die angegebene Email-Adresse ist ungültig.<br>Bitte gültige Adresse eingeben!");
			}
		}

		private void SetErrorStatus(string msg)
		{
			StatusPanel.Visible = true;
			StatusPanel.ForeColor = Color.Red;
			StatusPanel.Controls.Clear();
			StatusPanel.Controls.Add(new LiteralControl(msg));
		}

		private void SetSuccessStatus(string msg)
		{
			StatusPanel.Visible = true;
			StatusPanel.ForeColor = Color.Green;
			StatusPanel.Controls.Clear();
			StatusPanel.Controls.Add(new LiteralControl(msg));
		}
	}
}
