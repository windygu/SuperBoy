using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using MongoDB.Bson;
using MongoDB.Driver;
using ServiceStack.Redis;
using SuperBoy.YSQL.Model;

namespace SuperBoy.Test
{
    internal static class Controls
    {
        private static void Main(string[] args)
        {
            var sw = new Stopwatch();
            //启动计时器
            sw.Start();
            //启动主方法
            Mains();
            //关闭计时器
            sw.Stop();
            //打印时间
            Console.WriteLine(sw.Elapsed);
        }
        /// <summary>
        /// 启动主方法
        /// </summary>
        private static void Mains()
        {

        }
    }
}
