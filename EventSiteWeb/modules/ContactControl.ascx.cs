using System;
using System.Web.UI.WebControls;
using Ajax;
using kcm.ch.EventSite.BusinessLayer;
using kcm.ch.EventSite.Common;
using pbHelpers = playboater.gallery.commons.Helpers;

namespace kcm.ch.EventSite.Web.modules
{
	/// <summary>
	///		Summary description for ContactControl.
	/// </summary>
	public class ContactControl : DragableControl
	{
		protected Image MobileIcon;
		protected HyperLink NameLink;
		protected Panel pnlSendSms;
		protected TextBox SmsBody;
		protected Label CharCounter;
		protected System.Web.UI.WebControls.HyperLink sendLink;
		protected System.Web.UI.WebControls.HyperLink closeLink;

		private const string contactViewStateKey = "EventSite.ContactControl.Contact";

		private void Page_Load(object sender, System.EventArgs e)
		{
			Ajax.Utility.RegisterTypeForAjax(typeof(ContactControl));

			//register needed scripts
			((PageBase)base.Page).RegisterStartupScriptIfNeeded("ContactControlScripts", string.Format(pbHelpers.JavaScriptString, GetCharCountScript() + GetShowHideScript()));
		}

		private void ContactControl_PreRender(object sender, EventArgs e)
		{
			if(Contact == null)
			{
//				throw new EventSiteException("Please set the property \"Contact\"!", 900);
				this.Visible = false;
				return;
			}
			else
			{
				this.Visible = true;
			}

			SmsBody.Attributes.Add("onkeyup", string.Format("UpdateCharCounter('{0}', '{1}');", SmsBody.ClientID, CharCounter.ClientID));
			SmsBody.Attributes.Add("onchange", string.Format("UpdateCharCounter('{0}', '{1}');", SmsBody.ClientID, CharCounter.ClientID));

			NameLink.Text = Contact.Name;
			NameLink.NavigateUrl = "mailto:" + Contact.Email;
			NameLink.ToolTip = Contact.Name + " ein Email senden";

			MobileIcon.Visible = ((PageBase)base.Page).BLL.Mandator.SmsNotifications;
			MobileIcon.AlternateText = Contact.MobilePhone + " ein SMS schicken";
			MobileIcon.ToolTip = Contact.MobilePhone + " ein SMS schicken";
			MobileIcon.Visible = Contact.MobilePhone != null;
			MobileIcon.Attributes.Add("onclick", string.Format("ShowHideSmsPanel(event, '{0}', '{1}', '{2}');", pnlSendSms.ClientID, SmsBody.ClientID, CharCounter.ClientID));
			MobileIcon.Style.Add("cursor", "pointer");

			closeLink.Attributes.Add("onclick", string.Format("ShowHideSmsPanel(event, '{0}', '{1}', '{2}');", pnlSendSms.ClientID, SmsBody.ClientID, CharCounter.ClientID));
			//bind ajax request to onclick event
			sendLink.Attributes.Add("onclick", string.Format(@"
				var response = ContactControl.SendSms('{0}', {1}, document.getElementById('{2}').value);
				if(response.error != null)
				{{
					alert('Die Nachricht konnte nicht verschickt werden!\nDu kannst es evtl. nochmals versuchen.\nFehler: ' + response.error.description);
				}}
				else
				{{
					alert('Mitteilung erfolgreich verschickt.');
					ShowHideSmsPanel(event, '{3}', '{2}', '{4}');
				}}",
				Contact.Mandator.MandatorId,
				Contact.ContactId,
				SmsBody.ClientID,
				pnlSendSms.ClientID,
				CharCounter.ClientID));
		}

		public Contact Contact
		{
			get { return (Contact)ViewState[contactViewStateKey]; }
			set
			{
				ViewState[contactViewStateKey] = value;
				this.Visible = value != null;
			}
		}

		#region private Methods

		private string GetShowHideScript()
		{
			string script = @"
	function ShowHideSmsPanel(evt, panelCtrlId, smsBodyCtrlId, charCounterCtrlId)
	{
		//clear sms body text
		document.getElementById(smsBodyCtrlId).value = '';

		//update char counter label
		UpdateCharCounter(smsBodyCtrlId, charCounterCtrlId);

		var panel = document.getElementById(panelCtrlId);
		panel.style.display = (panel.style.display == 'none' ? 'block' : 'none');

		//detect mouse position
		var top = 0;
		var left = 0;
		if(!is.ie)
		{
			top = evt.pageY;
			left = evt.pageX;
		}
		else
		{
			top = window.event.clientY + document.body.scrollTop;
			left = window.event.clientX + document.body.scrollLeft;
		}

		panel.style.top = top + 'px';
		panel.style.left = left + 'px';
	}";
			return script;
		}

		private string GetCharCountScript()
		{
			string script = @"
	function UpdateCharCounter(smsBodyCtrlId, charCounterCtrlId)
	{
		var numSms = 0;
		var numCharsLeft = 0;
		var typedText = document.getElementById(smsBodyCtrlId).value;

		if(typedText.length <= 160)
		{
			numSms = 1;
			numCharsLeft = 160 - typedText.length;
		}
		else if(typedText.length <= 306)
		{
			numSms = 2;
			numCharsLeft = 306 - typedText.length;
		}
		else
		{
			document.getElementById(smsBodyCtrlId).value = document.getElementById(smsBodyCtrlId).value.substring(0, 306);
			numSms = 2;
			numCharsLeft = 0;
		}
		var charCounter = numCharsLeft + '/' + numSms;

		document.getElementById(charCounterCtrlId).innerHTML = charCounter;
	}";
			return script;
		}
		#endregion

		#region Ajax Methods
		[AjaxMethod(HttpSessionStateRequirement.Read)]
		public static bool SendSms(string mandatorId, int contactId, string smsText)
		{
			smsText = smsText.Trim();
			if(smsText.Equals(string.Empty))
			{
				throw new EventSiteException("Du kannst keine leeren Nachrichten verschicken.\nBitte einen Text eingeben.", 900);
			}

			using(EventSiteBL bll = new EventSiteBL(mandatorId))
			{
				Contact recipient = bll.GetContact(contactId);
				Contact sender = bll.CurrentContact;

				bll.SendSms(sender, recipient, smsText);
			}
			return true;
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
			this.PreRender += new System.EventHandler(this.ContactControl_PreRender);

		}
		#endregion
	}
}
