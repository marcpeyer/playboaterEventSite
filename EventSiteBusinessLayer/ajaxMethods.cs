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
using System.Globalization;
using System.Text.RegularExpressions;

namespace AJAXDatePicker
{
    /// <summary>
    /// Summary description for Class1
    /// </summary>
    public class ajaxMethods
    {
        [Ajax.AjaxMethod]
        public Dates GetNextMonthPeriods(string CurrDate)
        {
            DateTime NextMonth = DateTime.Parse(CurrDate, new CultureInfo("en-US").DateTimeFormat).AddMonths(1);
            DateTime FromDate = new DateTime(NextMonth.Year, NextMonth.Month, 1);
            DateTime ToDate = FromDate.AddMonths(1).AddDays(-1);
            return GetMonth(FromDate, ToDate);
        }

        [Ajax.AjaxMethod]
        public Dates GetPrevMonthPeriods(string CurrDate)
        {
            DateTime PrevMonth = DateTime.Parse(CurrDate, new CultureInfo("en-US").DateTimeFormat).AddMonths(-1);
            DateTime FromDate = new DateTime(PrevMonth.Year, PrevMonth.Month, 1);
            DateTime ToDate = FromDate.AddMonths(1).AddDays(-1);
            return GetMonth(FromDate, ToDate);
        }

        [Ajax.AjaxMethod]
        public Dates GetThisMonthPeriod(string CurrDate)
        {
            DateTime ThisMonth = DateTime.Parse(CurrDate, new CultureInfo("en-US").DateTimeFormat).AddMonths(0);
            DateTime FromDate = new DateTime(ThisMonth.Year, ThisMonth.Month, 1);
            DateTime ToDate = FromDate.AddMonths(1).AddDays(-1);
            return GetMonth(FromDate, ToDate);
        }

        [Ajax.AjaxMethod]
        public bool IsClickableDate(string CurrDate)
        {
			CurrDate = CurrDate.Trim();
			if(CurrDate == string.Empty)
			{
				return true;
			}
			else
			{
				try
				{
					Regex dateRegex = new Regex("[0-9]{1,2}\\.[0-9]{1,2}\\.[0-9]{4}");
					if(!dateRegex.IsMatch(CurrDate))
					{
						return false;
					}
					DateTime.Parse(CurrDate, new CultureInfo("de-CH").DateTimeFormat);
					return true;
				}
				catch
				{
					return false;
				}
			}

//            Dates p = GetThisMonthPeriod(CurrDate);
//
//            foreach (string ClickableDate in p.ClickableDates)
//            {
//                if (ClickableDate == CurrDate) { return true; }
//            }
//
//            return false; //return false if cannot find any dates
        }

		[Ajax.AjaxMethod]
		public bool IsValidTime(string time)
		{
			time = time.Trim();
			if(time != string.Empty)
			{
				Regex timeRegex;
				switch(time.Length)
				{
					case 4:
						//only numbers expected
						timeRegex = new Regex("[0-2][0-9][0-5][0-9]");
						if(timeRegex.IsMatch(time) && Convert.ToInt32(time.Substring(0, 2)) <= 24)
						{
							return true;
						}
						else
						{
							return false;
						}
					case 5:
						//two numbers a separator char and two numbers expected
						timeRegex = new Regex("[0-2][0-9][^0-9][0-5][0-9]");
						if(timeRegex.IsMatch(time) && Convert.ToInt32(time.Substring(0, 2)) <= 24)
						{
							return true;
						}
						else
						{
							return false;
						}
					default:
						//all other cases are not expected
						return false;
				}
			}
			return true;
		}

        private Dates GetMonth(DateTime FromDate, DateTime ToDate)
        {
            Dates p = new Dates();

//			p.FromDate = FromDate.ToString("d", new System.Globalization.CultureInfo("de-CH").DateTimeFormat);
//			p.ToDate = ToDate.ToString("d", new System.Globalization.CultureInfo("de-CH").DateTimeFormat);
			p.FromDate = FromDate.ToString("d", new System.Globalization.CultureInfo("en-US").DateTimeFormat);
			p.ToDate = ToDate.ToString("d", new System.Globalization.CultureInfo("en-US").DateTimeFormat);

            //populating the special dates and holidays. You can call an external source such as a database or web service to gather the data.



            //--------Sample Implementation---------------//


            p.ClickableDates = new string[] { "1/04/2006", "1/12/2005", "3/24/2005", "4/13/2005", "12/25/2005" };
            p.Holidays = new string[] { "12/25/2005", "12/26/2005", "01/01/2006", "01/02/2006" };

            //simulate long running process (maybe you might be calling a web service or a database query)
            //System.Threading.Thread.Sleep(800);

            //--------------------------------------------//


            //--------Database Call Implementation--------//

            /*
   
            List<string> holidays = new List<string>();
            List<string> clickable_days = new List<string>();
     
            SqlConnection Conn = new SqlConnection("Persist Security Info=False;Integrated Security=SSPI;database=northwind;server=mySQLServer;Connect Timeout=30");
            SqlCommand SqlCmd = new SqlCommand("select * from Calendar where [Date] >= '" + FromDate.ToShortDateString() + "' and [Date] <= '" + ToDate.ToShortDateString() + "'", Conn);
            Conn.Open();
            SqlDataReader RS = SqlCmd.ExecuteReader();
            while (RS.Read())
            {
              clickable_days.Add(DateTime.Parse(RS["Date"].ToString()).ToShortDateString());
            }
            Conn.Close();
            RS.Close();
     
            p.ClickableDates = clickable_days.ToArray();
            p.Holidays = holidays.ToArray(); 
    
            */

            //--------------------------------------------//   

            return p;
        }
    }

    [Serializable]
    public class Dates
    {
        public string FromDate;
        public string ToDate;
        public string[] ClickableDates = null;
        public string[] Holidays = null;
        //you can create other members here to pass to the client.
    }

}
