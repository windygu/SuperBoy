using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using SuperBoy.YSQL.Interface;
using SuperBoy.YSQL.Model;
using SuperBoy.YSQL.Realize;

namespace SuperBoy.YSQL.Control
{
    /// <summary>
    /// 创建类库
    /// </summary>
    // ReSharper disable once InconsistentNaming
    internal static class CreateYSQL
    {
        public delegate void DeleCreate(bool boo);
        static readonly IReadAndWriteYSQL Writes = new ReadAndWrite();

        /// <summary>
        /// 调用数据库方法
        /// </summary>
        /// <param name="delege"></param>
        /// <param name="boo"></param>
        public static void CallbackDelegate(DeleCreate delege, bool boo)
        {
            delege(boo);
        }

        //创建系统[信息初始化]
        public static void AutoInitSysDatabase()
        {
            //初始化系统信息
            var sysin = new SystemInfo(true);
            //获取配置文件初始化参数
            var deserializedProduct = JsonConvert.SerializeObject(sysin);
            //初始化数据库信息为默认
            ServiceYSQL.SystemInfoYsql = sysin;
            //存储系统信息到本地
            Writes.WriteSys(deserializedProduct, EnumArray.WriteType.SystemInfo);
        }
        /// <summary>
        /// 创建系统[数据库表信息表]
        /// </summary>
        public static void AutoDatabase()
        {
            //创建系统表表
            var sys = new SysDatabase(true);
            //序列化
            var deserializedProduct = JsonConvert.SerializeObject(sys);
            //加载默认数据库信息
            ServiceYSQL.SysDatabaseYsql = sys;
            //存储系统表
            Writes.WriteSys(deserializedProduct, EnumArray.WriteType.SysDatabase);
        }


        internal static void CreateBasetable(string address)
        {
            const string underDatabaseName = "SysDatabse";
            const string tableName = "SysTable";
            const string Namespace = "SysNamespace";
            var userName = new List<string>() { "Iner", "guest" };
            var fieldTypes = new Dictionary<string, string>
            {
                {"No", EnumArray.fieldType(EnumArray.FieldType.Int)},                 //数字
                {"Name", EnumArray.fieldType(EnumArray.FieldType.String)},            //普通字符串
                {"Remark", EnumArray.fieldType(EnumArray.FieldType.Text)},            //备注
                {"UpdateDatatime", EnumArray.fieldType(EnumArray.FieldType.Object)},  //修改时间
                {"CreateTime", EnumArray.fieldType(EnumArray.FieldType.Datetime)},    //创建时间
                {"Status", EnumArray.fieldType(EnumArray.FieldType.Local)},           //局部，有限制值
                {"Close", EnumArray.fieldType(EnumArray.FieldType.Trigger)},          //关闭的时候指向一个触发器或事件
                {"File", EnumArray.fieldType(EnumArray.FieldType.FileAddress)}        //指向一个地址，这个地址可以指向一个表，也可以指向一个数据库，一个命名空间，一个文件等在取出数据的时候其实就是将这个数据一同取出
            };
            const string modifier = "private";
            var json = CreateBasetable(fieldTypes, tableName, underDatabaseName, modifier, Namespace, userName);
            ServiceYSQL.ReadAndWrite.Write(json, address);
        }

        //创建数据库实体类
        private static string CreateBasetable(Dictionary<string, string> fieldTypes, string tableName, string databaseName, string modifier, string sysNamespace, IEnumerable<string> userName)
        {
            /*
             Dictionary<string, EnumArrayYSQL.FieldType> fieldTypes, string tableName, string databaseName, string modifier, string sysNamespace, IEnumerable<string> userName
             */
            var entityDatabase = new EntityTable(fieldTypes, tableName, databaseName, modifier, sysNamespace, userName);
            var json = JsonConvert.SerializeObject(entityDatabase);
            return json;
        }
    }
}