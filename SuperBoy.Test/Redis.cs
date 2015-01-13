using System;
using System.Globalization;
using ServiceStack.Redis;

namespace SuperBoy.Test
{
    public class Redis
    {

        private static void RedisMe()
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