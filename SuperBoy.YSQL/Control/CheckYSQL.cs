using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SuperBoy.YSQL.Interface;
using SuperBoy.YSQL.Lib;
using SuperBoy.YSQL.Model;
using SuperBoy.YSQL.Realize;

namespace SuperBoy.YSQL.Control
{
    // ReSharper disable once InconsistentNaming

    public class CheckYSQL
    {
        //检查该类有没有配置文件，配置文件是否正常
        public CheckYSQL()
        {
            //检查系统配置文件是否存在
            IReadAndWriteYSQL read = new ReadAndWrite();
            //获取当前系统下所有的文件夹
            var current = DirectoryUtil.GetCurrentDirectory();
            var item = DirectoryUtil.GetDirectoryItems(current);
            //如果该目录下一个文件夹都没有则创建
            if (item.Any())
            {
                //15901492612
                //判断系统文件是否存在
                var paths = DirectoryUtil.GetCurrentDirectory();
                if (!File.Exists(paths + "\\conf\\SystemInfo.conf")) return;
                var jsonStr = read.ReadSys(EnumArrayYSQL.ReadType.SystemInfo);
                try
                {
                    //检测系统文件是否异常
                    var sysin = new SystemInfoYSQL(false);
                    //获取配置文件初始化参数
                    var deserializedProduct = JsonConvert.DeserializeObject<SystemInfoYSQL>(jsonStr);

                }
                catch (Exception)
                {
                    //创建所有系统信息
                    CreateYSQL.AutoInitSysDatabase();
                    //系统表初始化
                    CreateYSQL.AutoDatabase(false);
                }

            }
            else
            {
                //创建所有系统信息
                CreateYSQL.AutoInitSysDatabase();
                //系统表初始化
                CreateYSQL.AutoDatabase(false);
            }
        }
    }
}