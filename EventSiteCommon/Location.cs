using System;

namespace kcm.ch.EventSite.Common
{
	/// <summary>
	/// Summary description for Location.
	/// </summary>
	[Serializable]
	public class Location : IComparable
	{
		public Location(int locationId, EventCategory eventCategory, string locationText, string locationShort, string locationDescription)
		{
			this.LocationId = locationId;
			this.EventCategory = eventCategory;
			this.LocationText = locationText;
			this.LocationShort = locationShort;
			this.LocationDescription = locationDescription;
		}

		public Location(EventCategory eventCategory, string locationText, string locationShort, string locationDescription)
		{
			this.LocationId = 0;
			this.EventCategory = eventCategory;
			this.LocationText = locationText;
			this.LocationShort = locationShort;
			this.LocationDescription = locationDescription;
		}

		#region Properties
		public int LocationId
		{
			get { return locationId; }
			set { locationId = value; }
		}
		private int locationId;

		public EventCategory EventCategory
		{
			get { return eventCategory; }
			set { eventCategory = value; }
		}
		private EventCategory eventCategory;

		public string LocationText
		{
			get { return locationText; }
			set { locationText = value.Trim(); }
		}
		private string locationText;

		public string LocationShort
		{
			get { return locationShort; }
			set { locationShort = value.Trim(); }
		}
		private string locationShort;

		public string LocationDescription
		{
			get { return locationDescription; }
			set { locationDescription = value.Trim(); }
		}
		private string locationDescription;
		#endregion

		#region IComparable Members

		public int CompareTo(object obj)
		{
			Location loc = (Location)obj;
			return (this.LocationText.CompareTo(loc.LocationText));
		}

		#endregion
	}
}