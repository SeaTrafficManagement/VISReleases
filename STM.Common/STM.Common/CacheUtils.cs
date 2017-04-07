using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Runtime.Caching;

namespace STM.Common
{
    /// <summary>
    /// This class contains utils for Cache (using the Cache implementation from HttpRuntime).
    /// For example, it handles timer expiring cache entries.
    /// </summary>
    public class CacheUtils
    {
        /// <summary>
        /// Add an object to a cache that expires after a given number of seconds.
        /// </summary>
        /// <param name="key">a string to use as key</param>
        /// <param name="objectToCache">the object to put in the cache</param>
        /// <param name="secondsToExpire">the number of seconds until the cache expires, or 0 for never</param>
        public static void AddToTimerCache(string key, object objectToCache, int secondsToExpire = 0)
        {
            ObjectCache cache = MemoryCache.Default;

            CacheItemPolicy policy = null;

            if (secondsToExpire != 0)
            {
                policy = new CacheItemPolicy();

                // Construct a time to use for cache expiration
                DateTime absoluteExpiration = DateTime.UtcNow.AddSeconds((double)secondsToExpire);

                policy.AbsoluteExpiration = absoluteExpiration;
            }

            // Insert with the given key and object
            cache.Add(key, objectToCache, policy);
        }

        /// <summary>
        /// Add an object to a cache that never expires
        /// </summary>
        /// <param name="key">a string to use as key</param>
        /// <param name="objectToCache">the object to put in the cache</param>
        public static void AddToCache(string key, object objectToCache)
        {
            AddToTimerCache(key, objectToCache, 0);
        }


        /// <summary>
        /// Get an object from the cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetFromCache<T>(string key)
        {
            ObjectCache cache = MemoryCache.Default;

            return (T)cache[key];
        }

        /// <summary>
        /// Remove all items in the Cache that contains a given key search
        /// string. Note that items placed in cache are supposed to have key names
        /// that are unique over different domains! E.g: "MyDomainPrefix_cachedItemX".
        /// If not, a call to this method may lead to unpredicted results...
        /// </summary>
        /// <param name="keySearchString">A string to search for in cached item key names</param>
        public static void ClearFromCache(string keySearchString)
        {
            ObjectCache cache = MemoryCache.Default;

            // Get an enumerator for all objects in the cache...
            IEnumerator<KeyValuePair<string, object>> cacheEnum =
                ((IEnumerable<KeyValuePair<string, object>>)cache).GetEnumerator();

            while (cacheEnum.MoveNext())
            {
                // Check if the key name contains the search string
                if (cacheEnum.Current.Key.ToString().Contains(keySearchString))
                {
                    // Yes it does! Remove this cached item!
                    cache.Remove(cacheEnum.Current.Key.ToString());
                }
            }
        }
    }
}
