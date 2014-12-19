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

        //创建系统信息初始化
        public static void AutoInitSysDatabase()
        {
            //初始化系统信息
            var sysin = new SystemInfoYSQL(true);
            //获取配置文件初始化参数
            var deserializedProduct = JsonConvert.SerializeObject(sysin);
            //初始化数据库
            //加入SystemInfo.conf
            //存储系统信息
            Writes.WriteSys(deserializedProduct, EnumArrayYSQL.WriteType.SystemInfo);
        }
        /// <summary>
        /// 创建系统数据库
        /// </summary>
        /// <param name="boo"></param>
        public static void AutoDatabase(bool boo)
        {
            //创建系统表表
            var sys = new SysDatabaseYSQL(boo);
            //序列化
            var deserializedProduct = JsonConvert.SerializeObject(sys);
            //存储系统表
            Writes.WriteSys(deserializedProduct, EnumArrayYSQL.WriteType.SysDatabase);
        }
        //如果有表信息可以恢复其他表

    }
}