<%@ Control Language="c#" AutoEventWireup="false" Inherits="AJAXDatePicker.AJAXDatePickerControl" CodeBehind="AJAXDatePickerControl.ascx.cs" %>
<%@ Register TagPrefix="ajax" Namespace="AJAXDatePicker" Assembly="EventSiteBusinessLayer" %>
<link type="text/css" rel="Stylesheet" href="pages/DatePicker.css">
	<script type="text/javascript" language="javascript">

var currdate_control_id;
var perioddiv_control_id; 
var txtSelectedDate_id; 
var control_id; 
var month_span_id; 
var direction; 
var currdate_control;

function callback_RenderPeriodMonths(res)
{
  var m_names = new Array("Januar", "Februar", "März", "April", "Mai", "Juni", "Juli", "August", "September", "Oktober", "November", "Dezember");
  currdate_control = document.getElementById(currdate_control_id);

	var thisdate = new Date(res.value.FromDate);
	altercontent(month_span_id, m_names[thisdate.getMonth()] + " " + thisdate.getFullYear()); 
	
	var s = "";
	
	for(var i=0; i<res.value.ClickableDates.length; i++)
		s += "<br/><a href='#' onclick=\"selectperiod('" + txtSelectedDate_id + "','" + control_id + "','" + res.value.ClickableDates[i] + "')\">" + res.value.ClickableDates[i] + "</a>"; 
		
	currdate_control.value =  res.value.FromDate;
		
	altercontent(perioddiv_control_id, GetCalendarHTML(res));
}

function IsClickable(res, month, day, year)
{
	//feature not used --> always return true
	return true;
  for(var i=0; i<res.value.ClickableDates.length; i++)
  {
    var PeriodDay = new Date(res.value.ClickableDates[i]);
    if (PeriodDay.getMonth() == month && PeriodDay.getDate() == day && PeriodDay.getFullYear() == year)
    {
      return true; 
    }
  }
  return false;
}

function IsHoliday(res, month, day, year)
{
	//feature not used --> always return false
	return false;
  for(var j=0; j<res.value.Holidays.length; j++)
  {
    var Holiday = new Date(res.value.Holidays[j]);
    if (Holiday.getMonth() == month && Holiday.getDate() == day && Holiday.getFullYear() == year)
    {
      return true; 
    }
  }
  return false;
}

function GetCalendarHTML(res)
{
//  SET ARRAYS
var day_of_week = new Array('Mo','Di','Mi','Do','Fr','Sa', 'So');
var month_of_year = new Array('Januar','Februar','März','April','Mai','Juni','Juli','August','September','Oktober','November','Dezember');

//  DECLARE AND INITIALIZE VARIABLES
var Calendar = new Date(res.value.FromDate);

var year = Calendar.getFullYear(); // Returns year

var month = Calendar.getMonth();    // Returns month (0-11)
var today = Calendar.getDate();    // Returns day (1-31)
var weekday = Calendar.getDay();    // Returns day (1-31)

var DAYS_OF_WEEK = 7;    // "constant" for number of days in a week
var DAYS_OF_MONTH = 31;    // "constant" for number of days in a month
var cal;    // Used for printing

Calendar.setDate(1);    // Start the calendar day at '1'
Calendar.setMonth(month);    // Start the calendar month at now

var HolidayCssClass = "";

/* BEGIN CODE FOR CALENDAR
NOTE: You can format the 'BORDER', 'BGCOLOR', 'CELLPADDING', 'BORDERCOLOR'
tags to customize your calendar's look.*/


cal = '<table class="drop_down_calendar">';
cal += '<tr class="days_of_week">';


//   DO NOT EDIT BELOW THIS POINT  //

// LOOPS FOR EACH DAY OF WEEK
for(index=0; index < DAYS_OF_WEEK; index++)
{

// BOLD TODAY'S DAY OF WEEK
if(weekday == index)
cal += '<td>' + '' + day_of_week[index] + '' + '</td>';

// PRINTS DAY
else
cal += '<td>' + day_of_week[index] + '</td>';
}

cal += '</td></tr>';
cal += '<tr>';

// FILL IN BLANK GAPS UNTIL TODAY'S DAY
var startDay = (Calendar.getDay()-1);
if(startDay<0) startDay += 7;
for(index=0; index < startDay; index++)
cal += '<td>' + '  ' + '</td>';

// LOOPS FOR EACH DAY IN CALENDAR
for(index=0; index < DAYS_OF_MONTH; index++)
{
if( Calendar.getDate() > index )
{
  // RETURNS THE NEXT DAY TO PRINT
  week_day =Calendar.getDay();

  // START NEW ROW FOR FIRST DAY OF WEEK
  if(week_day == 1)
  cal += '<tr>';

  if(week_day != DAYS_OF_WEEK)
  {

  // SET VARIABLE INSIDE LOOP FOR INCREMENTING PURPOSES
  var day  = Calendar.getDate();
  var now = new Date();
  
  //var day_str = (month+1) + "/" + day + "/" + year;
  var day_str = day + "." + (month+1) + "." + year;
  
  if (IsHoliday(res, month, day, year)) { HolidayCssClass = "holiday_date"; }
  else { HolidayCssClass = ""; }

  // HIGHLIGHT TODAY'S DATE
  if(month == now.getMonth()&& day == now.getDate() && year == now.getFullYear() )
  {
    if (IsClickable(res,month,day,year))
    {
      cal += "<td class=\"regular_date todays_date " + HolidayCssClass + "\"><a class=\"clickable_date\" onclick=\"selectperiod('" + txtSelectedDate_id + "','" + control_id + "','" + day_str + "')\">" + day + "</a></td>"; 
    }    
    else
    {
      cal += "<td class=\"regular_date todays_date " + HolidayCssClass + "\">" + day + "</td>"; 
    } 
  }
  // PRINTS DAY
  else
  {
    if (IsClickable(res,month,day,year))
    {
      cal += "<td class=\"regular_date " + HolidayCssClass + "\"><a class=\"clickable_date\" onclick=\"selectperiod('" + txtSelectedDate_id + "','" + control_id + "','" + day_str + "')\">" + day + "</a>" + "</td>"; 
    } 
    else
    {  
      cal += "<td class=\"regular_date " + HolidayCssClass + "\">" + day + "</td>"; 
    }
  }
  }

  // END ROW FOR LAST DAY OF WEEK
  if(week_day == DAYS_OF_WEEK)
  cal += '</tr>';
  }

  // INCREMENTS UNTIL END OF THE MONTH
  Calendar.setDate(Calendar.getDate()+1);

}// end for loop

cal += '</table>';

//  PRINT CALENDAR
return cal;
}

function RenderPeriodMonths(Pcurrdate_control_id, Pperioddiv_control_id, PtxtSelectedDate_id, Pcontrol_id, Pmonth_span_id, Pdirection)
{
//  currdate_control_id = Pcurrdate_control_id;
  currdate_control_id = Pcurrdate_control_id.replace(':', '_');
  perioddiv_control_id = Pperioddiv_control_id;
  txtSelectedDate_id = PtxtSelectedDate_id;
  control_id = Pcontrol_id;
  month_span_id = Pmonth_span_id; 
  direction = Pdirection;
  
  altercontent(perioddiv_control_id, "<div style='text-align:center'>Loading</div><div class='loading_icon'>&nbsp;</div>");
  currdate_control = document.getElementById(currdate_control_id);
  if (direction == 'next') { ajaxMethods.GetNextMonthPeriods(currdate_control.value, callback_RenderPeriodMonths); }
	else if (direction == 'prev') { ajaxMethods.GetPrevMonthPeriods(currdate_control.value, callback_RenderPeriodMonths); }
	else { ajaxMethods.GetThisMonthPeriod(currdate_control.value, callback_RenderPeriodMonths); }
//__datepicker_showpopup('StartDate_dp1_inner');
}

function CheckIsClickableDate(PtxtSelectedDate_id)
{
  txtSelectedDate_control = document.getElementById(PtxtSelectedDate_id);
  var res = ajaxMethods.IsClickableDate(txtSelectedDate_control.value)
  if (!res.value)
  {
    alert("Das eingegebene Datum ist ungültig!\nBitte in diesem Format eingeben: [dd.mm.yyyy]");
    //txtSelectedDate_control.value = "";
    txtSelectedDate_control.focus();
  }
}

function CheckIsValidTime(PtxtTime_id)
{
  txtTime_control = document.getElementById(PtxtTime_id);
  var res = ajaxMethods.IsValidTime(txtTime_control.value);
  if (!res.value)
  {
    alert("Die eingegebene Uhrzeit ist ungültig!\nBitte in diesem Format eingeben: [HH:MM]");
    //txtTime_control.value = "";
    txtTime_control.focus();
  }
}

function altercontent(elementid, content)
{
  //if IE 4+
  if (document.all) document.getElementById(elementid).innerHTML=content;
  //else if NS 6 (supports new DOM)
  else if (document.getElementById)
  {
    rng = document.createRange();
    el = document.getElementById(elementid);
    rng.setStartBefore(el);
    htmlFrag = rng.createContextualFragment(content);
    while (el.hasChildNodes()) el.removeChild(el.lastChild);
    el.appendChild(htmlFrag);
  }
}

function selectperiod(txtSelectedDate_id, control_id, txtValue)
{
  document.getElementById(txtSelectedDate_id).value= txtValue;
  document.getElementById(control_id).style.display='none'; 

  //__doPostBack('ctl00$MainContentPlaceholder$Picker1$lnkRaiseSelectDate','');
  var MyArray = control_id.split("$");
  var event_str = "";
  for (var i=0; i < MyArray.length - 1; i++)
  {
    event_str += MyArray[i] + "$";
  }
  event_str += "lnkRaiseSelectDate";
  //__doPostBack(event_str,''); // comment this out if you do not want to auto postback when the user selects the date.
}

	</script>
	<div class="controlblock">
		<table cellpadding="0" cellspacing="0">
			<tr>
				<td id="LabelCell" runat="server">
					Date:
				</td>
				<td>
					<ajax:DatePicker id="dp1" Runat="server" />
				</td>
				<td style="padding-left: 3px;">
					<asp:TextBox id="tbTime" runat="server" style="width:42px" MaxLength="5"></asp:TextBox>
				</td>
			</tr>
		</table>
		<asp:LinkButton ID="lnkRaiseSelectDate" runat="Server" OnClick="lnkRaiseSelectDate_Click" Visible="false"></asp:LinkButton>
	</div>
