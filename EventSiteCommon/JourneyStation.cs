using System;

namespace kcm.ch.EventSite.Common
{
	/// <summary>
	/// Summary description for JourneyStation.
	/// </summary>
	[Serializable]
	public class JourneyStation : IComparable
	{
		public JourneyStation(int journeyStationId, string station, string stationTime, int sortOrder)
		{
			JourneyStationId = journeyStationId;
			Station = station;
			StationTime = stationTime;
			SortOrder = sortOrder;
		}

		public JourneyStation(string station, string stationTime, int sortOrder)
		{
			Station = station;
			StationTime = stationTime;
			SortOrder = sortOrder;
		}

		#region Properties
		/// <summary>
		/// If id is 0 this JourneyStation is not yet in db, if id is -1 this JourneyStation must be deleted in db
		/// </summary>
		public int JourneyStationId
		{
			get { return journeyStationId; }
			set { journeyStationId = value; }
		}
		private int journeyStationId;

		public string Station
		{
			get { return station; }
			set { station = value; }
		}
		private string station;

		public string StationTime
		{
			get { return stationTime; }
			set { stationTime = value; }
		}
		private string stationTime;

		public int SortOrder
		{
			get { return sortOrder; }
			set { sortOrder = value; }
		}
		private int sortOrder;
		#endregion

		#region IComparable Members
		public int CompareTo(object obj)
		{
			JourneyStation js = (JourneyStation)obj;
			return (this.SortOrder.CompareTo(js.SortOrder));
		}
		#endregion
	}
}
