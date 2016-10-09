using System;

namespace kcm.ch.EventSite.Remoting
{
	public class OperationInfoAttribute : Attribute
	{
		private readonly string[] keys;

		public OperationInfoAttribute(params string[] keys)
		{
			this.keys = keys;
		}

		public string[] Keys
		{
			get { return keys; }
		}
	}
}