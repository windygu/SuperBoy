using System;
using System.Collections.Generic;
using System.Web;
using System.Collections;
using System.Web.Caching;

namespace Core.Web
{
    /// <summary>
    /// 全局统一的缓存类
    /// </summary>
    public class CacheHelper
    {
        #region  获取数据缓存
       
        /// <summary>
        /// 获取数据缓存
        /// </summary>
        /// <param name="cacheKey">键</param>
        public static object GetCache(string cacheKey)
        {
            var objCache = HttpRuntime.Cache;
            return objCache[cacheKey];
        }
        #endregion

        #region  设置数据缓存

       
        /// <summary>
        /// 设置数据缓存
        /// </summary>
        public static void SetCache(string cacheKey, object objObject)
        {
            var objCache = HttpRuntime.Cache;
            objCache.Insert(cacheKey, objObject);
        }
      
  
        /// <summary>
        /// 设置数据缓存
        /// </summary>
        public static void SetCache(string cacheKey, object objObject, TimeSpan timeout)
        {
            var objCache = HttpRuntime.Cache;
            objCache.Insert(cacheKey, objObject, null, DateTime.MaxValue, timeout, System.Web.Caching.CacheItemPriority.NotRemovable, null);
        }
       
        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        public static void SetCache(string cacheKey, object objObject, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            var objCache = HttpRuntime.Cache;
            objCache.Insert(cacheKey, objObject, null, absoluteExpiration, slidingExpiration);
        }
        #endregion

        #region   移除缓存

       
        /// <summary>
        /// 移除指定数据缓存
        /// </summary>
        public static void RemoveAllCache(string cacheKey)
        {
            var cache = HttpRuntime.Cache;
            cache.Remove(cacheKey);
        }

        /// <summary>
        /// 移除全部缓存
        /// </summary>
        public static void RemoveAllCache()
        {
            var cache = HttpRuntime.Cache;
            var cacheEnum = cache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                cache.Remove(cacheEnum.Key.ToString());
            }
        }
        #endregion
    
        private SortedDictionary<string, object> _dic = new SortedDictionary<string, object>();
        private static volatile Cache _instance = null;
        private static object _lockHelper = new object();


        public void Add(string key, object value)
        {
            _dic.Add(key, value);
        }


        public object this[string index]
        {
            get
            {
                if (_dic.ContainsKey(index))
                    return _dic[index];
                else
                    return null;
            }
            set { _dic[index] = value; }
        }

        public static Cache Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockHelper)
                    {
                        if (_instance == null)
                        {
                            _instance = new Cache();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}