using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using kcm.ch.EventSite.BusinessLayer;

namespace kcm.ch.EventSite.Web
{
	public partial class Index : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			MandatorList.DataSource = EventSiteBL.GetAllMandators();
			MandatorList.DataBind();
	}

	protected override void OnDataBinding(EventArgs e)
		{
			base.OnDataBinding(e);
		}
	}
}