using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace kcm.ch.EventSite.Remoting
{
	public class ReflectionHelper
	{
		private static List<Type> _types;
		private static readonly Dictionary<Type, ReadOnlyCollection<Type>> typeMap = new Dictionary<Type, ReadOnlyCollection<Type>>();

//		public static T GetAttribute<T>(MemberInfo mi) where T : Attribute
//		{
//			return (T)Attribute.GetCustomAttribute(mi, typeof(T));
//		}
//
//		public static string GetMemberName(MemberInfo mi)
//		{
//			FriendlyNameAttribute attribute = GetAttribute<FriendlyNameAttribute>(mi);
//			if (attribute != null)
//			{
//				return attribute.get_FriendlyName();
//			}
//			return mi.Name;
//		}

		public static ReadOnlyCollection<Type> GetNonAbstractSubclasses(Type type)
		{
			if (type == null)
			{
				return new ReadOnlyCollection<Type>(new List<Type>());
			}
			if (!typeMap.ContainsKey(type))
			{
				List<Type> list = new List<Type>();
				foreach (Type assemblyType in types)
				{
					if (!assemblyType.IsAbstract && assemblyType.IsSubclassOf(type))
					{
						list.Add(assemblyType);
					}
				}
				ReadOnlyCollection<Type> subTypes = new ReadOnlyCollection<Type>(list);
				typeMap[type] = subTypes;
				return subTypes;
			}
			return typeMap[type];
		}

//		public static Type GetType(string fullName)
//		{
//			foreach (Type type in types)
//			{
//				if (type.FullName == fullName)
//				{
//					return type;
//				}
//			}
//			return null;
//		}

		private static List<Type> types
		{
			get
			{
				if (_types == null)
				{
					_types = new List<Type>();
//					foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
//					{
//						_types.AddRange(assembly.GetTypes());
//					}
					_types.AddRange(Assembly.GetExecutingAssembly().GetTypes());
				}
				return _types;
			}
		}
	}
}