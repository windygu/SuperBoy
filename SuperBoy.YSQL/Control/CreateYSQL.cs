using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using SuperBoy.YSQL.Model;
using SuperBoy.YSQL.Realize;

namespace SuperBoy.YSQL.Control
{
    /// <summary>
    /// 创建类库
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class CreateYSQL
    {
        public delegate void DeleCreate(bool boo);
        static readonly Interface.IReadAndWriteYSQL Writes = new ReadAndWrite();

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
            var sysin = new SystemInfoYSQL(true);
            //获取配置文件初始化参数
            var deserializedProduct = JsonConvert.SerializeObject(sysin);
            //初始化数据库信息为默认
            ServiceYSQL.SystemInfoYsql = sysin;
            //存储系统信息到本地
            Writes.WriteSys(deserializedProduct, EnumArrayYSQL.WriteType.SystemInfo);
        }
        /// <summary>
        /// 创建系统[数据库表信息表]
        /// </summary>
        public static void AutoDatabase()
        {
            //创建系统表表
            var sys = new SysDatabaseYSQL(true);
            //序列化
            var deserializedProduct = JsonConvert.SerializeObject(sys);
            //加载默认数据库信息
            ServiceYSQL.SysDatabaseYsql = sys;
            //存储系统表
            Writes.WriteSys(deserializedProduct, EnumArrayYSQL.WriteType.SysDatabase);
        }

        /// <summary>
        /// 创建系统备份文件
        /// </summary>
        public static void AutoDatabaseBak()
        {

        }
        //如果有表信息可以恢复其他表

    }
}