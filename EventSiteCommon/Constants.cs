using System;

namespace kcm.ch.EventSite.Common
{
	/// <summary>
	/// Summary description for Constants.
	/// </summary>
	public class Constants
	{
		public const string ROLE_ADMINISTRATOR = "Administrator";
		public const string ROLE_MANAGER = "Manager";
		public const string ROLE_EVENT_CREATOR = "EventCreator";
		public const string ROLE_USER = "User";
		public const string ROLE_READER = "Reader";

		public const string SESSION_KEY_AUTH_CONTACT = "AuthenticatedContact";

		public const string InternalAuthTable = "ES_Contacts";
		public const string InternalAuthIdCol = "ContactId";
		public const string InternalAuthLoginCol = "Login";
		public const string InternalAuthPasswordCol = "Password";
	}
}
