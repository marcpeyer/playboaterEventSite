using System;

namespace kcm.ch.EventSite.Remoting
{
	public class ParameterInfoAttribute : Attribute
	{
		private readonly string[] keys = null;
		private readonly string prefix = null;

		public ParameterInfoAttribute(params string[] keys)
		{
			this.keys = keys;
		}

		public ParameterInfoAttribute(string prefix)
		{
			this.prefix = prefix;
		}
	}
}