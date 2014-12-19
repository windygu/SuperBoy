using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Web.CacheManage
{
    /// <summary>
    /// 缓存管理器
    /// <para>
    ///     请在Config文件中配置要使用哪种缓存类型,对于非WEB应用程序来说,只能使用APP类型
    /// </para>
    /// </summary>
    public class CacheProvider
    {
        private volatile static ICache _iwebcache;

        private volatile static ICache _imemcached;

        public static ICache Cache
        {
            get
            {
                if (_iwebcache == null)
                {
                    _iwebcache = WebCache.GetCacheService(true);
                }
                return _iwebcache;
            }
        }

        public static ICache MemCached
        {
            get
            {
                if (_imemcached == null)
                {
                    _imemcached = MemCache.GetCacheService(true);
                }
                return _imemcached;
            }
        }
    }

    /// <summary>
    /// 缓存类型
    /// </summary>
    public enum CacheType
    {
        /// <summary>
        /// 基于WEB的缓存
        /// </summary>
        Web,

        /// <summary>
        /// 基于MemCached
        /// </summary>
        MemCached
    }
}