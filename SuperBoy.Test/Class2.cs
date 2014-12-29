using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ConsoleApplication1
{
    internal class Program
    {
        private static void Main1(string[] args)
        {
            //连接信息
            const string conn = "mongodb://localhost";
            const string database = "demoBase";
            const string collection = "demoCollection";

            var mongodb = MongoServer.Create(conn); //连接数据库
            var mongoDataBase = mongodb.GetDatabase(database); //选择数据库名
            MongoCollection mongoCollection = mongoDataBase.GetCollection(collection); //选择集合，相当于表

            mongodb.Connect();

            //普通插入
            var o = new {Uid = 123, Name = "xixiNormal", PassWord = "111111"};
            mongoCollection.Insert(o);

            //对象插入
            var p = new Person {Uid = 124, Name = "xixiObject", PassWord = "222222"};
            mongoCollection.Insert(p);

            //BsonDocument 插入
            var b = new BsonDocument();
            b.Add("Uid", 125);
            b.Add("Name", "xixiBson");
            b.Add("PassWord", "333333");
            mongoCollection.Insert(b);

            Console.ReadLine();
        }
    }

    internal class Person
    {
        public string Name;
        public string PassWord;
        public int Uid;
    }
}