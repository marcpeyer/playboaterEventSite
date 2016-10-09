using System.Web;
using System.Web.UI;

namespace kcm.ch.EventSite.EventSiteAPI
{
	/// <summary>
	/// Implement the interface IFeature to write an assembly for the feature functionality of EventSite.
	/// </summary>
	public interface IFeature
	{
		void AddContent(Control destinationControl, HttpRequest request);
	}
}
