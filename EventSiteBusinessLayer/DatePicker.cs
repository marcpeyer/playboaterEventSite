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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;


namespace AJAXDatePicker
{

    /// <summary>
    /// Summary description for DatePicker
    /// </summary>
    public class DatePicker : WebControl, INamingContainer
    {
        public TextBox _innerTbx;
        private string errorText = null;
        private bool _panelvisible = false;
        private string _beginDate = DateTime.Now.ToString("d", new System.Globalization.CultureInfo("en-US").DateTimeFormat);

        public DatePicker()
            : base(HtmlTextWriterTag.Div)
        {
			ViewState["DatePicker_ImageDir"] = "images";
		}

        public System.DateTime SelectedDate
        {
            get
            {
                EnsureChildControls();
                System.DateTime d = System.DateTime.Now;
                try
                {
                    d = System.DateTime.Parse(_innerTbx.Text, new System.Globalization.CultureInfo("de-CH").DateTimeFormat);
                    errorText = null;
                    _beginDate = d.ToString("d", new System.Globalization.CultureInfo("en-US").DateTimeFormat);
                }
                catch
                {
                    errorText = "Date needs to be specified as dd.mm.yyyy";
                }
                return d;
            }
            set
            {
                EnsureChildControls();
                _innerTbx.Text = value.ToString("d", new System.Globalization.CultureInfo("de-CH").DateTimeFormat);
            }
        }

        public string ImageDirectory
        {
            get { return (string)ViewState["DatePicker_ImageDir"]; }
            set { ViewState["DatePicker_ImageDir"] = value; }
        }

    	public string TitleText
    	{
    		get { return (string)ViewState["DatePicker_TitleText"]; }
    		set { ViewState["DatePicker_TitleText"] = value; }
    	}

    	protected override void CreateChildControls()
        {
            base.CreateChildControls();
            _innerTbx = new TextBox();
			_innerTbx.MaxLength = 10;
			_innerTbx.Attributes.Add("title", TitleText);
            _innerTbx.ID = "txtSelectedDate";
            this.Controls.Add(_innerTbx);

        }

        protected override System.Web.UI.HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        protected override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer)
        {
            if (this.Width.IsEmpty)
            {
                //this.Width = new Unit(150);
            }
            base.AddAttributesToRender(writer);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            _innerTbx.Attributes.Add("Align", "AbsMiddle");
            _innerTbx.Attributes.Add("onBlur", "CheckIsClickableDate('" + _innerTbx.ClientID.Replace("$", "_") + "');");
            _innerTbx.Style.Add("width", "90px");
            _innerTbx.RenderControl(writer);

			string innerid = this.ClientID + "_inner";
            string PeriodsDivID = this.ClientID + "_Periods";
            string CurrDateID = this.ClientID + "_CurrDate";
            string MonthSpanID = this.ClientID + "_CurrMonth";

            writer.AddAttribute("Align", "AbsMiddle");
            writer.AddAttribute("src", ((string)ViewState["DatePicker_ImageDir"]) + "/dropdownbtn.gif");
            string RenderPeriodMethod = "RenderPeriodMonths('" + CurrDateID.Replace("$", "_") + "','" + PeriodsDivID + "','" + _innerTbx.ClientID.Replace("$", "_") + "','" + innerid + "','" + MonthSpanID + "','sfdg');";
            writer.AddAttribute("onClick", RenderPeriodMethod + "__datepicker_showpopup('" + innerid + "'); ");
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();

            if (errorText != null)
            {
                writer.AddStyleAttribute("color", "red");
                writer.AddStyleAttribute("display", "block");
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.Write(errorText);
                writer.RenderEndTag();
            }

            writer.AddStyleAttribute("position", "relative");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute("class", "drop_down_panel");

            string panelvisible = _panelvisible ? "block" : "none";

            writer.AddStyleAttribute("display", panelvisible);
            writer.AddAttribute("id", innerid);
            writer.AddAttribute("onfocusout", "__popup_losefocus(this)");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            HtmlInputHidden CurrDate = new HtmlInputHidden();
            CurrDate.ID = CurrDateID;
            CurrDate.Value = _beginDate;
            CurrDate.RenderControl(writer);

            writer.AddAttribute("class", "calendar_header");
            writer.RenderBeginTag(HtmlTextWriterTag.Table);

            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.AddAttribute("colspan", "3");
            writer.AddStyleAttribute("text-align", "right");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            HtmlAnchor CloseCalendar = new HtmlAnchor();
            CloseCalendar.InnerHtml = "<img class=\"calendar_close_button\" src='" + ((string)ViewState["DatePicker_ImageDir"]) + "/close_button.gif' border='0'>";
            CloseCalendar.Attributes.Add("onclick", "document.getElementById('" + innerid + "').style.display='none'");
            CloseCalendar.RenderControl(writer);
            writer.RenderEndTag();
            writer.RenderEndTag();

            writer.RenderBeginTag(HtmlTextWriterTag.Tr);

            writer.AddStyleAttribute("text-align", "left");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);

            HtmlAnchor PrevMonth = new HtmlAnchor();
            PrevMonth.InnerText = "< Prev";
            PrevMonth.Attributes.Add("class", "prev_month_link");
            PrevMonth.Attributes.Add("onclick", "RenderPeriodMonths('" + CurrDate.ID.Replace("$", "_") + "','" + PeriodsDivID + "','" + _innerTbx.ClientID.Replace("$", "_") + "','" + innerid + "','" + MonthSpanID + "','prev');");
            PrevMonth.RenderControl(writer);

            writer.RenderEndTag();

            writer.AddAttribute("class", "month_label");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);

            writer.AddAttribute("id", MonthSpanID);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write("&nbsp;"); //put some space as a default
            writer.RenderEndTag();

            writer.RenderEndTag();

            writer.AddStyleAttribute("text-align", "right");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);

            HtmlAnchor NextMonth = new HtmlAnchor();
            NextMonth.InnerText = "Next >";
            NextMonth.Attributes.Add("class", "next_month_link");
            NextMonth.Attributes.Add("onclick", "RenderPeriodMonths('" + CurrDate.ID.Replace("$", "_") + "','" + PeriodsDivID + "','" + _innerTbx.ClientID.Replace("$", "_") + "','" + innerid + "','" + MonthSpanID + "','next');");
            NextMonth.RenderControl(writer);

            writer.RenderEndTag();

            writer.RenderEndTag();
            writer.RenderEndTag();

            writer.AddAttribute("id", PeriodsDivID);
            writer.AddAttribute("class", "period_days");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderEndTag();

            //render the panel controls

            writer.RenderEndTag();
            writer.RenderEndTag();

        }

        protected override void OnPreRender(System.EventArgs e)
        {
            base.OnPreRender(e);
            commonScript.WritePopupRoutines(Page);
            StringBuilder sb = new StringBuilder();
			sb.Append("<script language=\"javascript\" type=\"text/javascript\">\r\n");
			if (_panelvisible)
            {
                sb.Append("__popup_panel = '" + this.ClientID + "_inner';\r\n");
            }

            sb.Append("function __datepicker_showpopup(name)\r\n");
            sb.Append("{\r\n");
            sb.Append(" if (__popup_panel != null)\r\n");
            sb.Append(" {\r\n");
            sb.Append(" document.getElementById(__popup_panel).style.display='none';\r\n");
            sb.Append(" }\r\n");
            sb.Append(" __popup_panel=name;\r\n");
            sb.Append(" var panel=document.getElementById(__popup_panel);\r\n");
            sb.Append(" panel.style.display='block';\r\n");
            sb.Append(" var links=panel.getElementsByTagName('A');\r\n");
            sb.Append(" links[0].focus();\r\n");

			sb.Append(" try {");
            sb.Append(" window.event.cancelBubble=true;\r\n");
			sb.Append(" } catch(e){ }");
            sb.Append("}\r\n");
			sb.Append("</script>");
			Page.RegisterClientScriptBlock("popup", sb.ToString());
//            Page.SmartNavigation = true;
        }

    }

    public class commonScript
    {

        public static void WritePopupRoutines(System.Web.UI.Page Page)
        {
            StringBuilder sb = new StringBuilder();
            sb = new StringBuilder();
			sb.Append("<script language=\"javascript\" type=\"text/javascript\">\r\n");
            sb.Append("var __popup_panel;\r\n");

            sb.Append("function __popup_clear() {\r\n");
            sb.Append(" if (__popup_panel != null ) \r\n");
            sb.Append(" {\r\n");
            sb.Append(" document.getElementById(__popup_panel).style.display='none';\r\n");
            sb.Append(" __popup_panel=null;\r\n");
            sb.Append(" }\r\n");
            sb.Append("}\r\n");
            sb.Append("function __popup_losefocus(panel)\r\n");
            sb.Append("{\r\n");
            sb.Append(" if (!panel.contains(document.activeElement))\r\n");
            sb.Append(" {\r\n");
            sb.Append(" panel.style.display='none';\r\n");
            sb.Append(" }\r\n");
            sb.Append("}\r\n");
			sb.Append("</script>");

            Page.RegisterClientScriptBlock("PopupRoutines", sb.ToString());
        }
    }

}