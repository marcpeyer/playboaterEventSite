using System;

namespace kcm.ch.EventSite.Common
{
	/// <summary>
	/// Summary description for EventCategory.
	/// </summary>
	[Serializable]
	public class EventCategory : IComparable
	{
		public EventCategory(int eventCategoryId, Mandator mandator, string category, string categoryDescription, string featureAssembly, string featureAssemblyClassName, bool freeEventSmsNotifications)
		{
			this.EventCategoryId = eventCategoryId;
			this.Mandator = mandator;
			this.Category = category;
			this.CategoryDescription = categoryDescription;
			this.FeatureAssembly = featureAssembly;
			this.FeatureAssemblyClassName = featureAssemblyClassName;
			this.FreeEventSmsNotifications = freeEventSmsNotifications;
		}

		#region Properties
		public int EventCategoryId
		{
			get { return eventCategoryId; }
			set { eventCategoryId = value; }
		}
		private int eventCategoryId;

		public Mandator Mandator
		{
			get { return mandator; }
			set { mandator = value; }
		}
		private Mandator mandator;

		public string Category
		{
			get { return category; }
			set { category = value.Trim(); }
		}
		private string category;

		public string CategoryDescription
		{
			get { return categoryDescription; }
			set { categoryDescription = (value==null ? null : value.Trim()); }
		}
		private string categoryDescription;

		public string FeatureAssembly
		{
			get { return featureAssembly; }
			set { featureAssembly = (value==null ? null : value.Trim()); }
		}
		private string featureAssembly;

		public string FeatureAssemblyClassName
		{
			get { return featureAssemblyClassName; }
			set { featureAssemblyClassName = value; }
		}
		private string featureAssemblyClassName;

		public bool FreeEventSmsNotifications
		{
			get { return freeEventSmsNotifications; }
			set { freeEventSmsNotifications = value; }
		}
		private bool freeEventSmsNotifications;
		#endregion

		#region IComparable Members

		public int CompareTo(object obj)
		{
			EventCategory cat = (EventCategory)obj;
			return this.Category.CompareTo(cat.Category);
		}

		#endregion
	}
}
