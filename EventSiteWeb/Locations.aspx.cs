using System;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using kcm.ch.EventSite.Common;
using pbHelpers=playboater.gallery.commons.Helpers;

namespace kcm.ch.EventSite.Web
{
	/// <summary>
	/// Summary description for Locations.
	/// </summary>
	public class Locations : PageBase
	{
		protected ListBox ExistingLocations;
		protected Button RenameLocation;
		protected Button DeleteLocation;
		protected System.Web.UI.WebControls.TextBox LocationTextBox;
		protected System.Web.UI.WebControls.TextBox LocationShortTextBox;
		protected System.Web.UI.WebControls.TextBox LocationDescription;
		protected System.Web.UI.WebControls.Button CreateLocation;
		protected HtmlGenericControl pageTitle;

		private const string LocationsViewStateKey = "EventSite.Locations";
		protected System.Web.UI.WebControls.RequiredFieldValidator reqLocationText;
		protected System.Web.UI.WebControls.RequiredFieldValidator reqLocationShort;
		protected System.Web.UI.WebControls.ValidationSummary valSummary;
		protected System.Web.UI.WebControls.DropDownList EventCategories;
		private const string NoSelectedLocationAlert = "Kein selektierter Ort gefunden.\\nBitte wähle einen Ort in der Liste.";
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			//reponse.exires to avoid caching in modal dialog
			Response.Expires = -1;
			//Response.Cache.SetNoStore();

			if(!IsPostBack)
			{
				EventCategories.DataSource = BLL.ListEventCategories();
				EventCategories.DataTextField = "Category";
				EventCategories.DataValueField = "EventCategoryId";
				EventCategories.DataBind();

				FillExistingLocations();
			}
		}

		private void EventCategories_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			FillExistingLocations();
		}

		private void ExistingLocations_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			Location location = GetSelectedLocation();
			if(location == null)
			{
				//no selected location found
				RegisterStartupScriptIfNeeded("noSelLoc", String.Format(pbHelpers.JavaScriptAlertString, "Kein selektierter Ort gefunden.\nBitte wähle einen Ort in der Liste."));
				return;
			}
			LocationTextBox.Text = location.LocationText;
			LocationShortTextBox.Text = location.LocationShort;
			LocationDescription.Text = location.LocationDescription;
		}

		private void CreateLocation_Click(object sender, System.EventArgs e)
		{
			Location loc = new Location(BLL.GetEventCategory(Int32.Parse(EventCategories.SelectedValue)), LocationTextBox.Text, LocationShortTextBox.Text, LocationDescription.Text);
			string userInfoText;
			try
			{
				userInfoText = BLL.AddLocation(loc);
				if(userInfoText != null)
				{
					RegisterStartupScriptIfNeeded("AddLocationUserInfo", string.Format(pbHelpers.JavaScriptAlertString, userInfoText));
				}
			}
			catch(EventSiteException ex)
			{
				RegisterStartupScriptIfNeeded("logicError", ex.JavaScriptAlertString);
				return;
			}
			FillExistingLocations();
		}

		private void DeleteLocation_Click(object sender, System.EventArgs e)
		{
			int locationId;
			try
			{
				locationId = Convert.ToInt32(ExistingLocations.SelectedValue);
			}
			catch(FormatException ex)
			{
				//no selected location found
				RegisterStartupScriptIfNeeded("noSelLoc", String.Format(pbHelpers.JavaScriptAlertString, NoSelectedLocationAlert));
				return;
			}

			if(locationId == 0)
			{
				//no selected location found
				RegisterStartupScriptIfNeeded("noSelLoc", String.Format(pbHelpers.JavaScriptAlertString, NoSelectedLocationAlert));
				return;
			}
			else
			{
				try
				{
					BLL.DelLocation(locationId, BLL.GetEventCategory(Int32.Parse(EventCategories.SelectedValue)));
				}
				catch(EventSiteException ex)
				{
					RegisterStartupScriptIfNeeded("logicError", ex.JavaScriptAlertString);
					return;
				}
			}
			FillExistingLocations();
		}

		private void RenameLocation_Click(object sender, System.EventArgs e)
		{
			Location location = GetSelectedLocation();
			if(location == null)
			{
				//no selected location found
				RegisterStartupScriptIfNeeded("noSelLoc", String.Format(pbHelpers.JavaScriptAlertString, NoSelectedLocationAlert));
				return;
			}
			else
			{
				location.LocationText = LocationTextBox.Text;
				location.LocationShort = LocationShortTextBox.Text;
				location.LocationDescription = LocationDescription.Text;
				try
				{
					BLL.EditLocation(location);
				}
				catch(EventSiteException ex)
				{
					RegisterStartupScriptIfNeeded("logicError", ex.JavaScriptAlertString);
					return;
				}
				FillExistingLocations();
			}
		}

		private void FillExistingLocations()
		{
			ArrayList locations;
			try
			{
				locations = BLL.ListLocations(BLL.GetEventCategory(Int32.Parse(EventCategories.SelectedValue)));
			}
			catch(EventSiteException ex)
			{
				RegisterStartupScriptIfNeeded("logicError", ex.JavaScriptAlertString);
				return;
			}
			ViewState[LocationsViewStateKey] = locations;
			ExistingLocations.DataSource = locations;
			ExistingLocations.DataTextField = "LocationText";
			ExistingLocations.DataValueField = "LocationId";
			ExistingLocations.DataBind();

			LocationTextBox.Text = "";
			LocationShortTextBox.Text = "";
			LocationDescription.Text = "";
		}

		private Location GetSelectedLocation()
		{
			ArrayList locations = (ArrayList)ViewState[LocationsViewStateKey];
			int selectedId;
			try
			{
				selectedId = Convert.ToInt32(ExistingLocations.SelectedValue);
			}
			catch(FormatException ex)
			{
				return null;
			}

			foreach (Location location in locations)
			{
				if(location.LocationId == selectedId)
				{
					return location;
				}
			}
			return null;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.EventCategories.SelectedIndexChanged += new System.EventHandler(this.EventCategories_SelectedIndexChanged);
			this.ExistingLocations.SelectedIndexChanged += new System.EventHandler(this.ExistingLocations_SelectedIndexChanged);
			this.RenameLocation.Click += new System.EventHandler(this.RenameLocation_Click);
			this.DeleteLocation.Click += new System.EventHandler(this.DeleteLocation_Click);
			this.CreateLocation.Click += new System.EventHandler(this.CreateLocation_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
	}
}
