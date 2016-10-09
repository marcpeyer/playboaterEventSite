using System;
using System.Reflection;

namespace kcm.ch.EventSite.Remoting
{
	public abstract class Operation
	{
		public readonly string[] Keys;
		public readonly Parameter[] Parameters;

		public Operation(string key, string[] keys, params Parameter[] parameters)
		{
			Keys = keys;
			Parameters = parameters;
			if(key == null)
			{
				return;
			}
			if (Array.IndexOf(Keys, key.Trim().ToUpper()) == -1)
			{
				throw new NotSupportedException("This operation doesn't match the given key.");
			}
		}

		public static Operation GetDefaultOperation()
		{
			return new EventSubscriptionOperation(null);
		}

		public static bool TryParse(string key, ref Operation op)
		{
			foreach (Type type in ReflectionHelper.GetNonAbstractSubclasses(typeof(Operation)))
			{
				try
				{
					op = (Operation) Assembly.GetExecutingAssembly().CreateInstance(
						type.FullName, false, BindingFlags.CreateInstance, null,
						new object[] {key}, null, null);
					return true;
				}
				catch (TargetInvocationException ex)
				{
					if (ex.InnerException != null && ex.InnerException.GetType() == typeof(NotSupportedException))
					{
						continue;
					}
					throw;
				}
			}
			return false;
		}

		public void CheckParameters()
		{
			foreach (Parameter parameter in Parameters)
			{
				if(parameter is ParameterOrGroup)
				{
					ParameterOrGroup orParameter = (ParameterOrGroup)parameter;
					if(orParameter.IsSet && orParameter.IsValid)
					{
						continue;
					}
					else
					{
						throw new ArgumentException("Oder-Parameter ungültig.", orParameter.ToString());
					}
				}
				else
				{
					if(parameter.IsRequired && !parameter.IsSet)
					{
						throw new ArgumentException("Parameter fehlt.", parameter.ToString());
					}
				}
			}
		}
	}

	public class HelpOperation : Operation
	{
		public HelpOperation(string key) : base(
			key,
			new string[] { "HILFE", "HELP", "HLP" })
		{
		}
	}

	public class SmsConfigOperation : Operation
	{
		public SmsConfigOperation(string key) : base(
			key,
			new string[] { "SMS" },
			new ParameterOrGroup( 
				new KeyParameter<bool>(true, "AN", "EIN"),
				new KeyParameter<bool>(false, "AUS")))
		{
		}
	}

	public class EventInfoOperation : Operation
	{
		public EventInfoOperation(string key) : base(
			key,
			new string[] { "EINFO", "AINFO" },
			new ParameterOrGroup(new KeyParameter<string>("TEILN", "TEILN")),
			new ValueParameter<int>())
		{
		}
	}

	public class EventSubscriptionOperation : Operation
	{
		public EventSubscriptionOperation(string key) : base(
			key, new string[] { },
			new ParameterOrGroup( 
				new KeyParameter<int>(1, "KOMME"),
				new KeyParameter<int>(3, "KOMMENICHT", "NICHT"),
				new KeyParameter<int>(2, "SPÄTER", "SPAETER")),
			new ValueParameter<int>(true),
			new PrefixValueParameter<string>("ZEIT:"))
		{
		}
	}

//
//	public enum Operation_
//	{
//		[OperationInfo(OperationType.ConfigCommand, "SMS")]
//		[ParameterInfo("AN", "EIN")]
//		[ParameterInfo("AUS")]
//		SmsConfig,
//		[OperationInfo(OperationType.EventCommand, "EINFO", "AINFO")]
//		EventInfo,
//		[OperationInfo(OperationType.EventCommand)]
//		[ParameterInfo("TEILN")]
//		EventSubscriptions,
//		[OperationInfo(OperationType.HelpCommand, "HELP", "HILFE", "HILF")]
//		Help,
//		Undefined
//	}
//
//	public enum OperationType
//	{
//		EventCommand,
//		ConfigCommand,
//		HelpCommand
//	}
}