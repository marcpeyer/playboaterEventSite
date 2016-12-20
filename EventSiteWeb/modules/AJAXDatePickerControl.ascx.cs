//-------------------------------------------------------------------------------
// AJAX Date Picker for ASP.NET
//    written by: Maynard Cuellar
//    email: CuellarMail@aol.com
//    website: www.maynardcuellar.com
//
// This control is free along with the source code. Feel free to modify the code 
// to suit your needs. The only thing is just give me a little credit by keeping 
// this comment. Thanks and happy coding.
//-------------------------------------------------------------------------------

using System;

namespace AJAXDatePicker
{

	public class AJAXDatePickerControl : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.LinkButton lnkRaiseSelectDate;
		protected System.Web.UI.HtmlControls.HtmlTableCell LabelCell;
		protected AJAXDatePicker.DatePicker dp1;
		protected System.Web.UI.WebControls.TextBox tbTime;
			   
		#region properties
		public System.DateTime selectedDateTime
		{
			get
			{
				DateTime selDateTime = dp1.SelectedDate;
				if(TimeBoxVisible)
				{
					string time = tbTime.Text.Trim();
					if(time != string.Empty)
					{
						if(time.Length == 4)
						{
							selDateTime = selDateTime.AddHours(Convert.ToDouble(time.Substring(0, 2)));
							selDateTime = selDateTime.AddMinutes(Convert.ToDouble(time.Substring(2)));
						}
						else
						{
							selDateTime = selDateTime.AddHours(Convert.ToDouble(time.Substring(0, 2)));
							selDateTime = selDateTime.AddMinutes(Convert.ToDouble(time.Substring(3)));
						}
					}
				}
				return selDateTime;
			}
			set
			{
				DateTime selDateTime = value;
				if(TimeBoxVisible)
				{
					string time = selDateTime.TimeOfDay.Hours.ToString().PadLeft(2, '0') + ":" + selDateTime.TimeOfDay.Minutes.ToString().PadLeft(2, '0');
					tbTime.Text = time;
					dp1.SelectedDate = selDateTime.Date;
				}
				else
				{
					dp1.SelectedDate = selDateTime;
				}
			}
		}

		public String ImageDirectory
		{
			get
			{
				return dp1.ImageDirectory;
			}
			set
			{
				dp1.ImageDirectory = value;
			}
		}

		public string LabelText
		{
			get { return labelText; }
			set { labelText = value; }
		}
		private string labelText;

		public string TitleText
		{
			get { return dp1.TitleText; }
			set { dp1.TitleText = value; }
		}

		public bool TimeBoxVisible
		{
			get
			{
				if(ViewState["DatePicker_TimeBoxVisible"] == null)
				{
					return false;
				}
				else
				{
					return (bool)ViewState["DatePicker_TimeBoxVisible"];
				}
			}
			set { ViewState["DatePicker_TimeBoxVisible"] = value; }
		}
		#endregion

		protected override void OnLoad(EventArgs e)
		{
			Ajax.Utility.RegisterTypeForAjax(typeof(AJAXDatePicker.ajaxMethods));

			base.OnLoad (e);

			if(!IsPostBack)
			{
				LabelCell.InnerText = LabelText;
				tbTime.Attributes.Add("onBlur", "CheckIsValidTime('" + tbTime.ClientID.Replace("$", "_") + "');");
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender (e);

			tbTime.Visible = TimeBoxVisible;
		}



		//events

		public event SelectDateEventHandler SelectDate;

		protected virtual void OnSelectDate(EventArgs e)
		{
			if (SelectDate != null) { SelectDate(this, e); }
		}

		protected void lnkRaiseSelectDate_Click(object sender, EventArgs e)
		{
			OnSelectDate(e);
		}
	}

	public delegate void SelectDateEventHandler(object sender, EventArgs e);
}