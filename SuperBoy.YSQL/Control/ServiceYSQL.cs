
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
        internal static readonly IReadAndWriteYSQL ReadAndWrite = new ReadAndWrite();

        //系统信息【整个系统数据库信息】
        public static SystemInfo SystemInfoYsql;

        //系统数据库信息【指向实体数据的信息】
        public static SysDatabase SysDatabaseYsql;

        //系统数据库实体【实体数据是存在这个里面的】
        // ReSharper disable once MemberCanBePrivate.Global
        public static EntityTable EntityDatabase;

        //当前数据库【当前系统所指向的一个数据库】
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public static string CurrentDatabase { internal get; set; }

        //当前表【当前系统指向的表】
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public static string CurrentTable { get; set; }
        //自检并加载库文件
        static ServiceYSQL()
        {
            //自检完成后加载第一个库
            // ReSharper disable once ObjectCreationAsStatement
            new CheckYSQL();
            //加载系统数据库
            LoadDatabaseYsql.LoadDatabase(SysDatabaseYsql.TableInfoModel.Address);
        }
        //插入一组数据
        public static void Inster()
        {

        }
    }
}