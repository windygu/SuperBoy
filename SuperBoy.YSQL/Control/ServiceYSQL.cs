
using System;
using System.Collections.Generic;
using System.IO;
using SuperBoy.YSQL.Interface;
using SuperBoy.YSQL.Model;
using SuperBoy.YSQL.Realize;
using Newtonsoft.Json;


namespace SuperBoy.YSQL.Control
{

    /// <summary>
    /// 服务类库
    /// </summary>
    /// ReSharper disable once InconsistentNaming
    public static class ServiceYSQL
    {
        //调用数据库总空间 
        //初始化调用参数
        private static readonly IReadAndWriteYSQL ReadAndWrite = new ReadAndWrite();
        //系统信息
        public static SystemInfoYSQL SystemInfoYsql;
        //系统数据库信息
        public static SysDatabaseYSQL SysDatabaseYsql;
        //当前数据库
        public static string CurrentDatabase;
        //当前表
        public static string CurrentTable;
        static ServiceYSQL()
        {
            //自检完成后加载第一个库
            CheckYSQL checkYsql = new CheckYSQL();
        }
        // ReSharper disable once UnusedAutoPropertyAccessor.Local


        //自动加载所有数据库
        public static void AutoMainAllDatabase()
        {
            //调用其他的所有数据库
            //加载数据库
            LoadDatabase(SystemInfoYsql);

        }

        private static void LoadDatabase(IEnumerable<string> address)
        {
            foreach (var addres in address)
            {
                //创建和加载实体数据库文件
                //判断是否存在
                if (File.Exists(addres))
                {
                    //存在则加载
                    try
                    {
                        var json = ReadAndWrite.Read(addres);
                        //当前数据库信息
                        var entity = JsonConvert.DeserializeObject<EntityDatabaseModel>(json);
                        //取当前数据库名
                        CurrentDatabase = entity.TableHead[EnumArrayYSQL.TableHead.DatabaseName].ToString();
                        //取当前表
                        CurrentTable = entity.TableHead[EnumArrayYSQL.TableHead.TableName].ToString();
                    }
                    catch (Exception)
                    {
                        //数据库不符合规定
                        throw;
                    }
                }
                else
                {
                    //创建数据库
                    CreateBasetable(addres);
                }
            }
        }
        //创建数据库实体
        private static void CreateBasetable(string address, Dictionary<string, EnumArrayYSQL.FieldType> fieldTypes, string tableName, string databaseName, string modifier, string sysNamespace, IEnumerable<string> userName)
        {
            /*
             Dictionary<string, EnumArrayYSQL.FieldType> fieldTypes, string tableName, string databaseName, string modifier, string sysNamespace, IEnumerable<string> userName
             */
            var entityDatabase = new EntityDatabaseModel(fieldTypes, tableName, databaseName, modifier, sysNamespace, userName);
            var json = JsonConvert.SerializeObject(entityDatabase);
            //加入存储
            ReadAndWrite.Write(json, address);
        }
        //创建数据库空实体
        private static void CreateBasetable(string address)
        {
            const string underDatabaseName = "SysDatabse";
            const string tableName = "Systable";
            const string Namespace = "SysNamespace";
            var userName = new List<string>() { "Iner", "guest" };
            var fieldTypes = new Dictionary<string, EnumArrayYSQL.FieldType>
            {
                {"No", EnumArrayYSQL.FieldType.Int},                 //数字
                {"Name", EnumArrayYSQL.FieldType.String},            //普通字符串
                {"Remark", EnumArrayYSQL.FieldType.Text},            //备注
                {"UpdateDatatime", EnumArrayYSQL.FieldType.Object},  //修改时间
                {"CreateTime", EnumArrayYSQL.FieldType.Datetime},    //创建时间
                {"Status", EnumArrayYSQL.FieldType.Local},           //局部，有限制值
                {"Close", EnumArrayYSQL.FieldType.Trigger},          //关闭的时候指向一个触发器或事件
                {"File", EnumArrayYSQL.FieldType.FileAddress}        //指向一个地址，这个地址可以指向一个表，也可以指向一个数据库，一个命名空间，一个文件等在取出数据的时候其实就是将这个数据一同取出
            };
            const string modifier = "private";
            CreateBasetable(address, fieldTypes, tableName, underDatabaseName, modifier, Namespace, userName);
        }
    }
}