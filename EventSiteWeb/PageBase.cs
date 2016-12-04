using System;
using System.Configuration;
using System.Web.UI;
using kcm.ch.EventSite.BusinessLayer;
using kcm.ch.EventSite.Common;

namespace kcm.ch.EventSite.Web
{
	/// <summary>
	/// Summary description for PageBase.
	/// </summary>
	public class PageBase : Page
	{
		public EventSiteBL BLL;

		protected override void OnInit(EventArgs e)
		{
			string mandQryStr = Request.QueryString["mid"];
			if(mandQryStr==null || mandQryStr==string.Empty)
			{
				throw new EventSiteException("QueryString Parameter 'mid' not found!", 900);
			}
			BLL = new EventSiteBL(mandQryStr);

			base.OnInit (e);
		}

		protected override void OnUnload(EventArgs e)
		{
			if(BLL != null)
			{
				BLL.Dispose();
			}

			base.OnUnload (e);
		}

		public void RegisterStartupScriptIfNeeded(string key, string script)
		{
			if(!ClientScript.IsStartupScriptRegistered(key))
			{
				ClientScript.RegisterStartupScript(GetType(), key, script);
			}
		}
	}
}
