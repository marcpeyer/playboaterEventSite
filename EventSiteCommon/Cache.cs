using System;
using System.Collections.Generic;

namespace kcm.ch.EventSite.Common
{
	public class Cache<T>
	{
		class CacheItem
		{
			public readonly DateTime ExpirationTime;
			public readonly T Target;

			public CacheItem(T target, DateTime expirationTime)
			{
				Target = target;
				ExpirationTime = expirationTime;
			}
		}
		
		public int MaxSize = 1000;
		public TimeSpan DefaultExpiration = new TimeSpan(0, 5, 0);
		
		private Dictionary<object, CacheItem> cacheItems = new Dictionary<object, CacheItem>();
		private volatile object lockObj = new object();

		
		public Cache()
		{
		}
				
		public Cache(int maxSize)
		{
			MaxSize = maxSize;
		}

		public Cache(int maxSize, TimeSpan defaultExpiration)
		{
			MaxSize = maxSize;
			DefaultExpiration = defaultExpiration;
		}
		
		public void Set(object key, T item)
		{
			Set(key, item, DefaultExpiration);	
		}

		public void Set(object key, T item, TimeSpan expiration)
		{
			lock (lockObj)
			{
				cacheItems[key] = new CacheItem(item, DateTime.Now + expiration);
				if (cacheItems.Count > MaxSize)
				{
					List<object> keysToRemove = new List<object>();					
					DateTime minExpiration = DateTime.MaxValue;					
					object minKey = null;
					DateTime now = DateTime.Now;

					foreach (KeyValuePair<object, CacheItem> kvp in cacheItems)
					{
						// find minimal key if no expired cacheItems where found
						if (kvp.Value.ExpirationTime < minExpiration)
						{
							minExpiration = kvp.Value.ExpirationTime;
							minKey = kvp.Key;
						}
						// find expired cacheItems
						if (kvp.Value.ExpirationTime < now)
						{
							keysToRemove.Add(kvp.Key);
						}
					}

					if (keysToRemove.Count == 0)
					{
						cacheItems.Remove(minKey);
					}
					else
					{
						// remove all expired cacheItems
						foreach (object k in keysToRemove) cacheItems.Remove(k);						
					}
				}
			}
		}
		
		public bool TryGet(object key, out T obj)
		{			
			lock(lockObj)
			{
				CacheItem ci;
				if (cacheItems.TryGetValue(key, out ci))
				{
					if (ci.ExpirationTime > DateTime.Now)
					{
						obj = ci.Target;
						return true;
					}
					else
					{
						cacheItems.Remove(key);
					}
				}
				
				obj = default(T);
				return false;
			}
		}
		
		public void Remove(object key)
		{
			lock (lockObj)
			{
				cacheItems.Remove(key);
			}
		}

		public void Clear()
		{
			lock(lockObj)
			{
				cacheItems.Clear();
			}
		}						
	}
	
	
	public class Cache
	{
		private static volatile object cacheMapSyncRoot = new object();
		private static Dictionary<Type, object> cacheMap = new Dictionary<Type, object>();	

		public static bool TryGet<T>(object key, out T obj)
		{
			return GetCache<T>().TryGet(key, out obj);			
		}

		private static Cache<T> GetCache<T>()
		{
			lock (cacheMapSyncRoot)
			{
				object o;				
				if (cacheMap.TryGetValue(typeof(T), out o)) return (Cache<T>)o;
								
				Cache<T> c = new Cache<T>();
				cacheMap.Add(typeof(T), c);

				return c;
			}
		}

		public static void Clear<T>()
		{
			GetCache<T>().Clear();			
		}

		public static void Remove<T>(object key)
		{
			GetCache<T>().Remove(key);						
		}

		public static void Set<T>(object key, T obj, TimeSpan expiration)
		{
			GetCache<T>().Set(key, obj, expiration);						
		}
		
		public static void Set<T>(object key, T obj)
		{
			GetCache<T>().Set(key, obj);						
		}
	}
}
