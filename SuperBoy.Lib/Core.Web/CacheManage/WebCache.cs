using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Collections.ObjectModel;

namespace Core.Web.CacheManage
{
    /// <summary>
    /// 网站缓存管理类
    /// </summary>
    public class WebCache : ICache
    {
        private static readonly object LockObj = new object();
        /// <summary>
        /// 当前的缓存是否可用
        /// </summary>
        private bool _enable = false;
        /// <summary>
        /// 默认实例
        /// </summary>
        private static WebCache _instance = null;
        /// <summary>
        /// 返回默认WebCache缓存实例
        /// </summary>
        /// <param name="enable">是否可用最好放到配置项里配置下</param>
        public static WebCache GetCacheService(bool enable)
        {
            if (_instance == null)
            {
                lock (LockObj)
                {
                    if (_instance == null)
                    {
                        _instance = new WebCache(enable);
                    }
                }
            }
            return _instance;
        }
        /// <summary>
        /// 构造方法
        /// </summary>
        private WebCache(bool enable)
        {
            this._enable = enable;
        }
        /// <summary>
        /// 获取一个值,指示当前的缓存是否可用
        /// </summary>
        public bool EnableCache
        {
            get
            {
                return this._enable;
            }
        }
        /// <summary>
        /// 获取缓存的类型
        /// </summary>
        public CacheType Type
        {
            get
            {
                return CacheType.Web;
            }
        }
        /// <summary>
        /// 检查缓存中是否存在指定的键
        /// </summary>
        /// <param name="key">要检查的键</param>
        /// <returns>返回一个值,指示检查的键是否存在</returns>
        public bool Contains(string key)
        {
            if (this._enable)
            {
                return HttpRuntime.Cache[key] != null;
            }
            return false;
        }
        /// <summary>
        /// 检查系统中是否存在指定的缓存
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <returns>返回这个类型的值是否存在</returns>
        public bool Contains<T>(string key)
        {
            var value = HttpRuntime.Cache[key];
            if (value is T)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 从缓存中获取指定键的值
        /// </summary>
        /// <param name="key">要获取的键</param>
        /// <returns>返回指定键的值</returns>
        public T Get<T>(string key)
        {
            if (this._enable)
            {
                return (T)HttpRuntime.Cache[key];
            }
            return default(T);
        }
        /// <summary>
        /// 获取缓存中键值的数量
        /// </summary>
        public int Count
        {
            get
            {
                if (this._enable)
                {
                    return HttpRuntime.Cache.Count;
                }
                return 0;
            }
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">缓存值</param>
        public void Add<T>(string key, T value)
        {
            if (this._enable)
            {
                HttpRuntime.Cache.Insert(key, value);
            }
            return;
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">缓存值</param>
        /// <param name="absoluteExpiration">过期时间</param>
        public void Add<T>(string key, T value, DateTime absoluteExpiration)
        {
            if (this._enable)
            {
                HttpRuntime.Cache.Insert(key, value, null, absoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return;
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">缓存值</param>
        /// <param name="slidingExpiration">保存时间</param>
        public void Add<T>(string key, T value, TimeSpan slidingExpiration)
        {
            if (this._enable)
            {
                HttpRuntime.Cache.Insert(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, slidingExpiration);
            }
            return;
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">缓存值</param>
        /// <param name="minutes">保存时间(分钟)</param>
        public void Add<T>(string key, T value, int minutes)
        {
            if (this._enable)
            {
                HttpRuntime.Cache.Insert(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, minutes, 0));
            }
            return;
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">缓存值</param>
        /// <param name="priority">优先级</param>
        /// <param name="slidingExpiration">保存时间</param>
        public void Add<T>(string key, T value, CachePriority priority, TimeSpan slidingExpiration)
        {
            if (this._enable)
            {
                HttpRuntime.Cache.Insert(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, slidingExpiration, CacheItemPriorityConvert(priority), null);
            }
            return;
        }
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">缓存值</param>
        /// <param name="priority">优先级</param>
        /// <param name="absoluteExpiration">过期时间</param>
        public void Add<T>(string key, T value, CachePriority priority, DateTime absoluteExpiration)
        {
            if (this._enable)
            {
                HttpRuntime.Cache.Insert(key, value, null, absoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriorityConvert(priority), null);
            }
            return;
        }
        /// <summary>
        /// 尝试返回指定的缓存
        /// </summary>
        /// <typeparam name="T">缓存内容的类型</typeparam>
        /// <param name="key">缓存的key</param>
        /// <param name="value">缓存的内容</param>
        /// <returns>是否存在这个缓存</returns>
        public bool TryGetValue<T>(string key, out T value)
        {
            var temp = HttpRuntime.Cache[key];
            if (temp != null && temp is T)
            {
                value = (T)temp;
                return true;
            }
            value = default(T);
            return false;
        }
        /// <summary>
        /// 移除键中某关键字的缓存并返回相应的值
        /// </summary>
        /// <param name="key">关键字</param>
        public void Remove(string key)
        {
            object result = null;
            if (this._enable)
            {
                if (HttpRuntime.Cache[key] != null)
                {
                    result = HttpRuntime.Cache.Remove(key);
                }
            }
            return;
        }
        /// <summary>
        /// 移除键中带某关键字的缓存
        /// </summary>
        /// <param name="key">关键字</param>
        public int RemoveContains(string key)
        {
            var result = 0;
            if (this._enable)
            {
                var cacheEnum = HttpRuntime.Cache.GetEnumerator();
                while (cacheEnum.MoveNext())
                {
                    if (cacheEnum.Key.ToString().Contains(key))
                    {
                        HttpRuntime.Cache.Remove(cacheEnum.Key.ToString());
                        result++;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 移除键中以某关键字开头的缓存
        /// </summary>
        /// <param name="key">关键字</param>
        public int RemoveStartWith(string key)
        {
            var result = 0;
            if (this._enable)
            {
                var cacheEnum = HttpRuntime.Cache.GetEnumerator();
                while (cacheEnum.MoveNext())
                {
                    if (cacheEnum.Key.ToString().StartsWith(key))
                    {
                        HttpRuntime.Cache.Remove(cacheEnum.Key.ToString());
                        result++;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 移除键中以某关键字结尾的缓存
        /// </summary>
        /// <param name="key">关键字</param>
        public int RemoveEndWith(string key)
        {
            var result = 0;
            if (this._enable)
            {
                var cacheEnum = HttpRuntime.Cache.GetEnumerator();
                while (cacheEnum.MoveNext())
                {
                    if (cacheEnum.Key.ToString().EndsWith(key))
                    {
                        HttpRuntime.Cache.Remove(cacheEnum.Key.ToString());
                        result++;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 移除键中所有的缓存
        /// </summary>
        public int Clear()
        {
            var result = 0;
            if (this._enable)
            {
                var cacheEnum = HttpRuntime.Cache.GetEnumerator();
                while (cacheEnum.MoveNext())
                {
                    HttpRuntime.Cache.Remove(cacheEnum.Key.ToString());
                    result++;
                }
                _keys.Clear();
            }
            return result;
        }
        private List<string> _keys = new List<string>();
        /// <summary>
        /// 缓存中所有的键列表
        /// </summary>
        public ReadOnlyCollection<string> Keys
        {
            get
            {
                if (this._enable)
                {
                    lock (_keys)
                    {
                        _keys.Clear();
                        var cacheEnum = HttpRuntime.Cache.GetEnumerator();
                        while (cacheEnum.MoveNext())
                        {
                            _keys.Add(cacheEnum.Key.ToString());
                        }
                    }
                }
                return new ReadOnlyCollection<string>(_keys);
            }
        }
        /// <summary>
        /// 对缓存优先级做一个默认的转换
        /// </summary>
        /// <param name="priority">原始的优先级</param>
        /// <returns>目标优先级</returns>
        private CacheItemPriority CacheItemPriorityConvert(CachePriority priority)
        {
            var p = CacheItemPriority.Default;
            switch (priority)
            {
                case CachePriority.Low:
                    {
                        p = CacheItemPriority.Low;
                        break;
                    }
                case CachePriority.Normal:
                    {
                        p = CacheItemPriority.Normal;
                        break;
                    }
                case CachePriority.High:
                    {
                        p = CacheItemPriority.High;
                        break;
                    }
                case CachePriority.NotRemovable:
                    {
                        p = CacheItemPriority.NotRemovable;
                        break;
                    }
            }
            return p;
        }
    }
}

