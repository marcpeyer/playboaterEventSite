using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using playboater.gallery.commons;

namespace kcm.ch.EventSite.Common
{

	public enum LiftState
	{
		undefined = 0, //gray
		drives = 1, //green
		full = 2, //red
		lift = 3 //blue
	}

	/// <summary>
	/// Summary description for Subscription.
	/// </summary>
	[Serializable]
	public class Subscription
	{
		public Subscription(int subscriptionId, Event pEvent, Contact contact, int subscriptionStateId, string subscriptionStateText, bool subscriptionStateIsDeletable, bool isUnsubscribable, string subscriptionTime, string fontcolor, string comment, NInt32 numLifts, NInt32 liftSubscriptionJourneyStationId, LiftState subscriptionLiftState)
		{
			this.SubscriptionId = subscriptionId;
			this._Event = pEvent;
			this.Contact = contact;
			this.SubscriptionStateId = subscriptionStateId;
			this.SubscriptionStateText = subscriptionStateText;
			this.subscriptionStateIsDeletable = subscriptionStateIsDeletable;
			this.IsUnsubscribable = isUnsubscribable;
			this.SubscriptionTime = subscriptionTime;
			this.Fontcolor = fontcolor;
			this.Comment = comment;
			this.NumLifts = numLifts;
			this.LiftSubscriptionJourneyStationId = liftSubscriptionJourneyStationId;
			this.SubscriptionLiftState = subscriptionLiftState;

			journeyStations = new ArrayList();
		}

		public Subscription(Event pEvent, Contact contact, int subscriptionStateId, string subscriptionStateText, string subscriptionTime, string comment, NInt32 numLifts, NInt32 liftSubscriptionJourneyStationId)
		{
			this.SubscriptionId = 0;
			this._Event = pEvent;
			this.Contact = contact;
			this.SubscriptionStateId = subscriptionStateId;
			this.SubscriptionStateText = subscriptionStateText;
			this.SubscriptionTime = subscriptionTime;
			this.Comment = comment;
			this.NumLifts = numLifts;
			this.LiftSubscriptionJourneyStationId = liftSubscriptionJourneyStationId;
			this.SubscriptionLiftState = LiftState.undefined;

			journeyStations = new ArrayList();
		}

		public void AddStation(JourneyStation station)
		{
			journeyStations.Add(station);
		}

		public void RemoveStation(JourneyStation station)
		{
			foreach (JourneyStation journeyStation in JourneyStations)
			{
				if(journeyStation.SortOrder > station.SortOrder)
				{
					journeyStation.SortOrder--;
				}
			}
			if(station.JourneyStationId == 0)
			{
				journeyStations.Remove(station);
			}
			else
			{
				station.SortOrder = -1;
//				station.JourneyStationId = -1;
			}
		}

		public void ReOrderStations(JourneyStation station, bool moveUp)
		{
			if(station.SortOrder == 1 && moveUp)
			{
				throw new EventSiteException("Dieser Routenpunkt ist bereits zuoberst!", -1);
			}
			if(station.SortOrder == JourneyStations.Count && !moveUp)
			{
				throw new EventSiteException("Dieser Routenpunkt ist bereits zuunterst!", -1);
			}

			int stationSortOrder = station.SortOrder;

			foreach (JourneyStation journeyStation in journeyStations)
			{
				if(moveUp)
				{
					if(journeyStation.SortOrder == stationSortOrder - 1)
					{
						journeyStation.SortOrder++;
					}
					else if(journeyStation.SortOrder == stationSortOrder)
					{
						journeyStation.SortOrder--;
					}
				}
				else
				{
					if(journeyStation.SortOrder == stationSortOrder + 1)
					{
						journeyStation.SortOrder--;
					}
					else if(journeyStation.SortOrder == stationSortOrder)
					{
						journeyStation.SortOrder++;
					}
				}
			}
		}

		public void SetJourneyStartTime(string startTime)
		{
			foreach (JourneyStation journeyStation in journeyStations)
			{
				if(journeyStation.SortOrder == 1)
				{
					journeyStation.StationTime = startTime;
				}
				else
				{
					journeyStation.StationTime = "";
				}
			}
		}

		public string GetJourneyStartTime()
		{
			foreach (JourneyStation journeyStation in journeyStations)
			{
				if(journeyStation.SortOrder == 1)
				{
					return journeyStation.StationTime;
				}
			}
			return null;
		}

		public string[] GetJourneyStationArr()
		{
			string[] stations = new string[JourneyStations.Count];
			int i = -1;
			foreach (JourneyStation journeyStation in JourneyStations)
			{
				stations[++i] = journeyStation.Station;
			}
			return stations;
		}

		public Color GetFontcolor()
		{
			if(Fontcolor == null)
			{
				throw new EventSiteException("Fontcolor property is null! Please check Fontcolor property to null first.", 900);
			}

			Color c = ColorTranslator.FromHtml(Fontcolor);
			return c;
		}

		#region Properties
		public int SubscriptionId
		{
			get { return subscriptionId; }
			set { subscriptionId = value; }
		}
		private int subscriptionId;

		public Event Event
		{
			get { return _Event; }
			set { _Event = value; }
		}
		private Event _Event;

		public Contact Contact
		{
			get { return contact; }
			set { contact = value; }
		}
		private Contact contact;

		public int SubscriptionStateId
		{
			get { return subscriptionStateId; }
			set { subscriptionStateId = value; }
		}
		private int subscriptionStateId;

		public string SubscriptionStateText
		{
			get { return subscriptionStateText; }
			set { subscriptionStateText = value.Trim(); }
		}
		private string subscriptionStateText;

		public bool SubscriptionStateIsDeletable
		{
			get { return subscriptionStateIsDeletable; }
			set { subscriptionStateIsDeletable = value; }
		}
		private bool subscriptionStateIsDeletable;

		public bool IsUnsubscribable
		{
			get { return isUnsubscribable; }
			set { isUnsubscribable = value; }
		}
		private bool isUnsubscribable;

		public string Fontcolor
		{
			get { return fontcolor; }
			set
			{
				fontcolor = (value == null || value.Trim() == string.Empty ? null : value.Trim());
			}
		}
		private string fontcolor;

		public string SubscriptionTime
		{
			get { return subscriptionTime; }
			set { subscriptionTime = (value != null ? value.Trim() : value) ; }
		}
		private string subscriptionTime;

		public string Comment
		{
			get { return comment; }
			set { comment = (value != null ? value.Trim() : value) ; }
		}
		private string comment;

		public NInt32 NumLifts
		{
			get { return numLifts; }
			set { numLifts = value; }
		}
		private NInt32 numLifts;

//		public string JourneyStart
//		{
//			get { return journeyStart; }
//			set { journeyStart = value.Trim(); }
//		}
//		private string journeyStart;
//
//		public string JourneyStartTime
//		{
//			get { return journeyStartTime; }
//			set { journeyStartTime = value.Trim(); }
//		}
//		private string journeyStartTime;

		public NInt32 LiftSubscriptionJourneyStationId
		{
			get { return liftSubscriptionJourneyStationId; }
			set { liftSubscriptionJourneyStationId = value; }
		}
		private NInt32 liftSubscriptionJourneyStationId;

		public LiftState SubscriptionLiftState
		{
			get { return subscriptionLiftState; }
			set { subscriptionLiftState = value; }
		}
		private LiftState subscriptionLiftState;

		public ArrayList JourneyStations
		{
			get
			{
				ArrayList tmp = new ArrayList();
				foreach(JourneyStation js in journeyStations)
				{
					if(js.SortOrder != -1)
					{
						tmp.Add(js);
					}
				}
				tmp.Sort();
				return tmp;
			}
		}
		public ArrayList journeyStations;
		#endregion

		public static Subscription GetSubscriptionById(List<Subscription> subscriptions, int subscriptionId)
		{
			foreach (Subscription subscription in subscriptions)
			{
				if(subscription.SubscriptionId == subscriptionId)
					return subscription;
			}
			return null;
		}

		public static Subscription GetSubscriptionByJourneyStationId(List<Subscription> subscriptions, int journeyStationId)
		{
			foreach (Subscription subscription in subscriptions)
			{
				foreach (JourneyStation js in subscription.JourneyStations)
				{
					if(js.JourneyStationId == journeyStationId)
					{
						return subscription;
					}
				}
			}
			return null;
		}
	}
}
