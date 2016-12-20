using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Ajax;
using pbHelpers=playboater.gallery.commons.Helpers;

namespace kcm.ch.EventSite.Web.modules
{
	/// <summary>
	///		Summary description for PopupControl.
	/// </summary>
	public class PopupControl : DragableControl
	{
		protected Image PopupIcon;
		protected HyperLink PopupLink;
		protected Panel pnlPopup;
		public HtmlContainerControl PopupBody;
		protected Label CharCounter;
		protected System.Web.UI.WebControls.HyperLink saveLink;
		protected System.Web.UI.WebControls.HyperLink closeLink;

		private const string popupLinkViewStateKey = "EventSite.PopupControl.PopupLink";
		private const string saveCloseLinksViewStateKey = "EventSite.PopupControl.SaveCloseLinks";
		private const string scriptCallBackViewStateKey = "EventSite.PopupControl.ScriptCallBack";
		private const string popupIconSrcViewStateKey = "EventSite.PopupControl.PopupIconSrc";
		private const string popupTextViewStateKey = "EventSite.PopupControl.PopupText";
		private const string popupEnabledViewStateKey = "EventSite.PopupControl.Enabled";
		private const string popupDisabledScriptCallViewStateKey = "EventSite.PopupControl.DisabledScriptCall";

		private void Page_Load(object sender, System.EventArgs e)
		{
//			Ajax.Utility.RegisterTypeForAjax(typeof(EventSite.modules.ContactControl));

			//register needed scripts
			((PageBase)base.Page).RegisterStartupScriptIfNeeded("PopupControlScripts", string.Format(pbHelpers.JavaScriptString, GetShowHideScript()));
		}

		private void ContactControl_PreRender(object sender, EventArgs e)
		{
			PopupIcon.ImageUrl = String.Concat("..\\", PopupIconSrc);

			PopupLink.Visible = ShowPopupLink;
			PopupIcon.Visible = !ShowPopupLink;
			saveLink.Visible = ShowSaveCloseLinks;
			closeLink.Visible = ShowSaveCloseLinks;

			PopupLink.Text = PopupText;
			PopupLink.NavigateUrl = "Javascript:";

			PopupIcon.AlternateText = PopupText;
			PopupIcon.ToolTip = PopupText;
			PopupIcon.Style.Add("cursor", "pointer");

			string onclickScript;
			if(PopupEnabled)
			{
				onclickScript = String.Format("ShowHidePopupPanel(event, '{0}');", pnlPopup.ClientID);
			}
			else
			{
				onclickScript = PopupDisabledScriptCall;
			}
			PopupLink.Attributes.Add("onclick", onclickScript);
			PopupIcon.Attributes.Add("onclick", onclickScript);

			closeLink.Attributes.Add("onclick", String.Format("ShowHidePopupPanel(event, '{0}');", pnlPopup.ClientID));
			closeLink.NavigateUrl = "Javascript:";
			saveLink.Attributes.Add("onclick", String.Format(@"
				{0};
				ShowHidePopupPanel(event, '{1}');
				",
				ScriptCallBackOnSave,
				pnlPopup.ClientID));
			saveLink.NavigateUrl = "Javascript:";
		}

		#region Properties
		public bool PopupEnabled
		{
			get
			{
				try
				{
					return (bool)ViewState[popupEnabledViewStateKey];
				}
				catch
				{
					ViewState[popupEnabledViewStateKey] = true;
					return true;
				}
			}
			set
			{
				ViewState[popupEnabledViewStateKey] = value;
			}
		}

		/// <summary>
		/// If true displays the PopupLink to open the popup, otherwise displays the PopupIcon.
		/// </summary>
		public bool ShowPopupLink
		{
			get
			{
				try
				{
					return (bool)ViewState[popupLinkViewStateKey];
				}
				catch
				{
					ViewState[popupLinkViewStateKey] = false;
					return false;
				}
			}
			set
			{
				ViewState[popupLinkViewStateKey] = value;
				PopupLink.Visible = value;
				PopupIcon.Visible = !value;
			}
		}

		public bool ShowSaveCloseLinks
		{
			get
			{
				try
				{
					return (bool)ViewState[saveCloseLinksViewStateKey];
				}
				catch
				{
					ViewState[saveCloseLinksViewStateKey] = false;
					return false;
				}
			}
			set
			{
				ViewState[saveCloseLinksViewStateKey] = value;
				saveLink.Visible = value;
				closeLink.Visible = value;
			}
		}

		public string ScriptCallBackOnSave
		{
			get { return (string)ViewState[scriptCallBackViewStateKey]; }
			set { ViewState[scriptCallBackViewStateKey] = value; }
		}

		public string PopupIconSrc
		{
			get { return (string)ViewState[popupIconSrcViewStateKey]; }
			set { ViewState[popupIconSrcViewStateKey] = value; }
		}

		
		public string PopupText
		{
			get { return (string)ViewState[popupTextViewStateKey]; }
			set { ViewState[popupTextViewStateKey] = value; }
		}

		public string PopupDisabledScriptCall
		{
			get { return (string)ViewState[popupDisabledScriptCallViewStateKey]; }
			set { ViewState[popupDisabledScriptCallViewStateKey] = value; }
		}
		#endregion
		
		#region private Methods

		private string GetShowHideScript()
		{
			string script = @"
	function ShowHidePopupPanel(evt, panelCtrlId)
	{
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
		#endregion

//		#region Ajax Methods
//		[AjaxMethod(HttpSessionStateRequirement.Read)]
//		public static bool SendSms(string mandatorId, int contactId, string smsText)
//		{
//			smsText = smsText.Trim();
//			if(smsText.Equals(string.Empty))
//			{
//				throw new EventSiteException("Du kannst keine leeren Nachrichten verschicken.\nBitte einen Text eingeben.", 900);
//			}
//
//			EventSiteBL bll = new EventSiteBL(mandatorId);
//
//			Contact recipient = bll.GetContact(contactId);
//
////			try
////			{
//				bll.SendSms(recipient, smsText);
////			}
////			catch(EventSiteException ex)
////			{
////				try { Helpers.SendErrorMail(ex); }
////				catch {}
////				return false;
////			}
//
//			return true;
//		}
//		#endregion

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
