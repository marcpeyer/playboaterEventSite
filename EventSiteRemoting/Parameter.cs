using System;

namespace kcm.ch.EventSite.Remoting
{
	public abstract class Parameter
	{
		public readonly bool IsRequired;

		protected Parameter()
		{
			IsRequired = false;
		}

		protected Parameter(bool isRequired)
		{
			IsRequired = isRequired;
		}

		public abstract bool IsSet
		{
			get;
		}

		public abstract bool TrySet(string command, string nextCommand, out bool nextCommandUsed);
	}

	public class ParameterOrGroup : Parameter
	{
		public readonly Parameter[] Parameters;

		public ParameterOrGroup(params Parameter[] parameters)
			: base(true)
		{
			Parameters = parameters;
		}

		private bool isValid = false;
		public bool IsValid
		{
			get { return isValid; }
		}

		public override string ToString()
		{
			foreach (Parameter parameter in Parameters)
			{
				if(parameter.IsSet)
				{
					return parameter.ToString();
				}
			}
			return null;
		}

		public override bool IsSet
		{
			get
			{
				bool isSet = false;
				foreach (Parameter p in Parameters)
				{
					if (p.IsSet)
					{
						if (!isSet)
						{
							isSet = true;
							isValid = true;
						}
						else
						{
							//set isvalid to false because multiple Parameters are set
							//but only one is allowed
							isValid = false;
						}
					}
				}
				return isValid && isSet;
			}
		}

		/// <summary>
		/// Returns the only one Parameter which is set in this OR group
		/// </summary>
		public Parameter GetParameterSet()
		{
			if (IsValid)
			{
				foreach (Parameter p in Parameters)
				{
					if(p.IsSet)
					{
						return p;
					}
				}
			}
			throw new Exception("No set parameter found!");
		}

		public override bool TrySet(string command, string nextCommand, out bool nextCommandUsed)
		{
			throw new InvalidOperationException("TrySet is not valid on a ParameterGroup!");
		}
	}

	public abstract class Parameter<T> : Parameter
	{
		protected T value;

		protected Parameter()
		{
			value = default(T);
		}

		protected Parameter(bool isRequired)
			: base(isRequired)
		{
			value = default(T);
		}

		public override bool IsSet
		{
			get { return !Equals(value, default(T)); }
		}

		public T Value
		{
			get { return value; }
		}
	}

	public class KeyParameter<T> : Parameter<T>
	{
		public readonly string[] Keys;
		private bool isSet = false;

		/// <summary>
		/// Creates a new instance of KeyParameter with IsRequired=false
		/// </summary>
		public KeyParameter(T value, params string[] keys)
		{
			Keys = keys;
			this.value = value;
		}

		/// <summary>
		/// Creates a new instance of KeyParameter with the possibility to set IsRequired
		/// </summary>
		public KeyParameter(bool isRequired, T value, params string[] keys)
			: base(isRequired)
		{
			Keys = keys;
			this.value = value;
		}

		public override string ToString()
		{
			return value.ToString();
		}

		public override bool TrySet(string command, string nextCommand, out bool nextCommandUsed)
		{
			nextCommandUsed = false;
			foreach (string key in Keys)
			{
				if (key.Equals(command, StringComparison.InvariantCultureIgnoreCase))
				{
					isSet = true;
					return true;
				}
			}
			return false;
		}

		public override bool IsSet
		{
			get
			{
				return isSet;
			}
		}
	}

	public class PrefixValueParameter<T> : Parameter<T>
	{
		public readonly string Prefix;

		/// <summary>
		/// Creates a new instance of PrefixValueParameter with IsRequired=false
		/// </summary>
		public PrefixValueParameter(string prefix)
		{
			Prefix = prefix;
		}

		/// <summary>
		/// Creates a new instance of PrefixValueParameter with the possibility to set IsRequired
		/// </summary>
		public PrefixValueParameter(bool isRequired, string prefix)
			: base(isRequired)
		{
			Prefix = prefix;
		}

		public override string ToString()
		{
			return Prefix + value;
		}
		
		public override bool TrySet(string command, string nextCommand, out bool nextCommandUsed)
		{
			nextCommandUsed = false;
			try
			{
				if (command.ToUpper().StartsWith(Prefix))
				{
					string val;
					if(command.Length > Prefix.Length)
					{
						val = command.Substring(Prefix.Length);
					}
					else
					{
						val = nextCommand;
						nextCommandUsed = true;
					}
					value = (T)Convert.ChangeType(val, typeof(T));
					return true;
				}
				return false;
			}
			catch
			{
				return false;
			}
		}
	}

	public class ValueParameter<T> : Parameter<T>
	{
		/// <summary>
		/// Creates a new instance of ValueParameter with IsRequired=false
		/// </summary>
		public ValueParameter()
		{
		}

		/// <summary>
		/// Creates a new instance of ValueParameter with the possibility to set IsRequired
		/// </summary>
		public ValueParameter(bool isRequired)
			: base(isRequired)
		{
		}

		public override string ToString()
		{
			return value.ToString();
		}

		public override bool TrySet(string command, string nextCommand, out bool nextCommandUsed)
		{
			nextCommandUsed = false;
			try
			{
				value = (T)Convert.ChangeType(command, typeof(T));
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}