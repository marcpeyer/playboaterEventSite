using System;
using System.Collections.Generic;
using System.Globalization;
using kcm.ch.EventSite.Common;
using playboater.gallery.commons;

namespace kcm.ch.EventSite.Common
{
	/// <summary>
	/// Summary description for Event.
	/// </summary>
	[Serializable]
	public class Event : IComparable
	{
		public List<SmsNotifSubscription> SmsNotifSubscriptions;

		public Event(int eventId, EventCategory eventCategory, Contact eventCreator, Location location, DateTime startDate, string duration, string eventTitle, string eventDescription, string eventUrl, NInt32 minSubscriptions, NInt32 maxSubscriptions)
		{
			this.EventId = eventId;
			this.EventCategory = eventCategory;
			this.EventCreator = eventCreator;
			this.Location = location;
			this.StartDate = startDate;
			this.Duration = duration;
			this.EventTitle = eventTitle;
			this.EventDescription = eventDescription;
			this.EventUrl = eventUrl;
			this.MinSubscriptions = minSubscriptions;
			this.MaxSubscriptions = maxSubscriptions;

			SmsNotifSubscriptions = null;
		}

		public Event(EventCategory category, Contact eventCreator)
		{
			this.EventCategory = category;
			this.EventCreator = eventCreator;
			this.EventTitle = "Neuer Anlass";
			this.StartDate = DateTime.Now.Date;
			this.EventId = -1;
			this.MinSubscriptions = null;
			this.MaxSubscriptions = null;

			SmsNotifSubscriptions = null;
		}

		#region Properties
		public int EventId
		{
			get { return eventId; }
			set { eventId = value; }
		}
		private int eventId;

		public EventCategory EventCategory
		{
			get { return eventCategory; }
			set { eventCategory = value; }
		}
		private EventCategory eventCategory;

		public Contact EventCreator
		{
			get { return eventCreator; }
			set { eventCreator = value; }
		}
		private Contact eventCreator;

		public Location Location
		{
			get { return location; }
			set { location = value; }
		}
		private Location location;

		public DateTime StartDate
		{
			get { return startDate; }
			set { startDate = value; }
		}
		private DateTime startDate;

		public string Duration
		{
			get { return duration; }
			set { duration = (value==null ? null : value.Trim()); }
		}
		private string duration;

		public string EventTitle
		{
			get { return eventTitle; }
			set { eventTitle = value.Trim(); }
		}
		private string eventTitle;

		public string EventTitleAndDate
		{
			get { return string.Format("{0} ({1})", eventTitle, startDate.Date.ToString("dd.MM.yyyy")); }
		}

		public string EventDescription
		{
			get { return eventDescription; }
			set { eventDescription = (value==null ? null : value.Trim()); }
		}
		private string eventDescription;

		public string EventUrl
		{
			get { return eventUrl; }
			set { eventUrl = (value==null ? null : value.Trim()); }
		}
		private string eventUrl;

		public NInt32 MinSubscriptions
		{
			get { return minSubscriptions; }
			set { minSubscriptions = value; }
		}
		private NInt32 minSubscriptions;

		public NInt32 MaxSubscriptions
		{
			get { return maxSubscriptions; }
			set { maxSubscriptions = value; }
		}
		private NInt32 maxSubscriptions;
		#endregion

		#region IComparable Members

		public int CompareTo(object obj)
		{
			Event e = (Event)obj;
			return e.StartDate.CompareTo(this.StartDate);
		}

		public static IComparer<Event> AscendingComparer = new AscComparer();

		public static IComparer<Event> DescendingComparer = new DescComparer();

		#endregion

		internal class AscComparer : IComparer<Event>
		{
			public int Compare(Event x, Event y)
			{
				return x.StartDate.CompareTo(y.StartDate);
			}
		}

		internal class DescComparer : IComparer<Event>
		{
			public int Compare(Event x, Event y)
			{
				return y.StartDate.CompareTo(x.StartDate);
			}
		}
	}
}
