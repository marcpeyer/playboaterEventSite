using System;
using System.Web;
using System.Web.SessionState;
using kcm.ch.EventSite.Common;
using kcm.ch.EventSite.DataAccessLayer;

namespace kcm.ch.EventSite.BusinessLayer
{
	/// <summary>
	/// Avoids processing tasks multiple times per session by setting session vars.
	/// </summary>
	public class MultipleStepsAvoidance
	{
		private static readonly string globCreditKey = "ES_GlobalSmsCreditLowNotified";
		private static readonly string userCreditKeyFormat = "ES_User{0}SmsCreditLowNotified";

		public static bool DoGlobalSmsCreditNotif()
		{
			return CheckKey(globCreditKey);
		}

		public static void GlobalSmsCreditNotified()
		{
			SetKey(globCreditKey);
		}
	
		public static bool DoUserSmsCreditNotif(int contactId)
		{
			return CheckKey(String.Format(userCreditKeyFormat, contactId));
		}

		public static void UserSmsCreditNotified(int contactId)
		{
			SetKey(String.Format(userCreditKeyFormat, contactId));
		}

		private static bool CheckKey(string key)
		{
			bool notified;
			if (Boolean.TryParse(EventSiteDA.GetCache(key), out notified))
			{
				return !notified;
			}

			return true;
		}

		private static void SetKey(string key)
		{
			EventSiteDA.SetCache(key, true.ToString(), 120);
		}
	}
}
