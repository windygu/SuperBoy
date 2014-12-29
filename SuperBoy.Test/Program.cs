using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using ServiceStack.Redis;
using SuperBoy.YSQL.Model;

namespace SuperBoy.Test
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            Main2();
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }

        private static void Main2()
        {
            MongoDb();
        }

        //连接Mongodb
        private static void MongoDb()
        {
            // MongoDB服务器 连接串
            //const string connectionString = "mongodb://120.27.51.150";
            var mcsb = new MongoConnectionStringBuilder
            {
                Server = new MongoServerAddress("120.27.51.150", 27017),
                //ConnectTimeout = new TimeSpan(30000),
                //MaxConnectionLifeTime = new TimeSpan(300000),
                //MinConnectionPoolSize = 8,
                //MaxConnectionPoolSize = 2000,
                Password = "shenmemima..01",
                Username = "jyf"
            };

            var server = MongoServer.Create(mcsb);

            // 连接到 mongodb_c_demo 数据库
            var db = server.GetDatabase("mongodbDems");
            // 获取集合 fruit
            MongoCollection collection = db.GetCollection("wolaopo");

            var item = new BsonDocument
            {
                {"name","jyf"},
                {"age","15"}
            };

            collection.Insert(item);
        }

        private static void Redis()
        {
            var client = new RedisClient("120.27.51.150", 63333)
            {
                Password = "ShengEr..01KONGque00shenmemima..01"
            };
            //存储用户名和密码  
            client.Set<string>("username", "postmaster@nnnni.cn");
            client.Set<int>("pwd", 123456);
            //获取
            var username = client.Get<string>("username");
            var pwd = client.Get<int>("pwd");
            client.Set<decimal>("price", 12.10M);
            var price = client.Get<decimal>("price");
            //输出
            Console.WriteLine(price.ToString(CultureInfo.InvariantCulture));
            Console.WriteLine("用户名:" + username + ",密码:" + pwd.ToString());
        }
    }
}
