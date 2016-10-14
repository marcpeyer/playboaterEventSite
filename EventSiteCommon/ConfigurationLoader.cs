using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using System.Xml.Serialization;

namespace kcm.ch.EventSite.Common
{
	public static class ConfigurationLoader
	{
		private class CacheItem
		{
			public readonly object Configuration;
			public readonly DateTime LoadTime;

			public CacheItem(DateTime loadTime, object config)
			{
				LoadTime = loadTime;
				Configuration = config;
			}
		}

		public static string Mid;

		private static readonly Dictionary<string, CacheItem> cache = new Dictionary<string, CacheItem>();

		public static readonly string EventSiteConfigurationFolder =
			String.Format(@"{0}\..\conf\playboater.EventSite\{1}.{2}.{3}",
				System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath,
				Assembly.GetExecutingAssembly().GetName().Version.Major,
				Assembly.GetExecutingAssembly().GetName().Version.Minor,
				Assembly.GetExecutingAssembly().GetName().Version.Build);

		public static object Load(string fileName, Type configType, out bool modified)
		{
			modified = false;

			CacheItem item;
			if (cache.TryGetValue(fileName, out item))
			{
				if (File.GetLastWriteTime(fileName) < item.LoadTime)
				{
					return item.Configuration;
				}
			}

			object configuration;
			using (StreamReader sr = new StreamReader(fileName))
			{
				configuration = new XmlSerializer(configType).Deserialize(sr);
			}

			modified = true;
			cache[fileName] = new CacheItem(DateTime.Now, configuration);
			return configuration;
		}

		public static T Load<T>(string fileNameFormat)
		{
			bool modified;
			return Load<T>(fileNameFormat, out modified);
		}

		public static T Load<T>(string fileNameFormat, out bool modified)
		{
			string path;
			string fileNameSpecificMandator = null;
			string fileNameUnspecific = String.Format(fileNameFormat, String.Empty);
			if(HttpContext.Current != null)
			{
				string mid = HttpContext.Current.Request.QueryString["mid"];
				if(!String.IsNullOrEmpty(mid))
				{
					fileNameSpecificMandator = String.Format(fileNameFormat, "." + mid);
				}
			}
			else if(!String.IsNullOrEmpty(Mid))
			{
				fileNameSpecificMandator = String.Format(fileNameFormat, "." + Mid);
			}

			if (!Path.IsPathRooted(fileNameFormat))
			{
				path = String.Format(@"{0}\{1}", EventSiteConfigurationFolder, fileNameSpecificMandator);

				if(!File.Exists(path))
				{
					path = String.Format(@"{0}\{1}", EventSiteConfigurationFolder, fileNameUnspecific);
				}
			}
			else
			{
				path = fileNameSpecificMandator != null && File.Exists(fileNameSpecificMandator) ?
					fileNameSpecificMandator : fileNameUnspecific;
			}

			return (T)Load(path, typeof(T), out modified);
		}

	}
}