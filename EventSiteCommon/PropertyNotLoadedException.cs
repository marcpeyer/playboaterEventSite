using System;

namespace kcm.ch.EventSite.Common
{
	/// <summary>
	/// Summary description for PropertyNotLoadedException.
	/// </summary>
	public class PropertyNotLoadedException : EventSiteException
	{
		public PropertyNotLoadedException(string message, int errorNumber, Exception innerException) : base(message, errorNumber, innerException)
		{
		}

		public PropertyNotLoadedException(string message, int errorNumber) : base(message, errorNumber)
		{
		}
	}
}
