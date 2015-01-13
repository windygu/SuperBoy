using System;
using System.Collections;
using System.Threading;
using System.Web;
using System.Web.Caching;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using MemcachedProviders.Cache;

namespace SuperBoy.Test
{
    public abstract class Memcacheds
    {

        /// <summary>实现了 缓存操作接口的  HttpRuntime.Cache 操作类
        ///
        /// </summary>
        public static void MemcachedsMe()
        {
            using (var mc = new MemcachedClient())
            {
                //发送
                mc.Store(StoreMode.Set, "mytime", "abcdefghijklmn");
                //获取
                var temp = mc.Get<string>("mytime");
                Console.Write(temp + "\r\n");

            }
        }

        public static void MemcacheMe()
        {
            //申明一个缓存对象
            //create a instance of MemcachedClient
            var mc = new MemcachedClient();

            //存储到缓存里面
            // store a string in the cache
            mc.Store(StoreMode.Set, "MyKey", "Hello World");
            //从缓存中查找这个东西
            // retrieve the item from the cache
            //string temp = mc.Get<string>("MyKey");
            Console.WriteLine(mc.Get("MyKey"));
        }
    }
}