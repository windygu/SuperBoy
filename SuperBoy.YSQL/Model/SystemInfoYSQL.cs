using System;
using System.Collections.Generic;

namespace SuperBoy.YSQL.Model
{
    // ReSharper disable once InconsistentNaming
    public class SystemInfoYSQL
    {

        /*
         head:[type:systemInfo]
#是否正常关闭
IsNormalClose:true
#上次在处理的工作
control:none
#最后一次关闭时间
lastCloseDatatime:datatime.now
#最后一次启动时间
lastStartDatatime:datatime.now
#版本号
versions:1.0
#所有者
possessor:system
         */

        public bool Control { get; set; }//控制器启动状态
        public int Port { get; set; }//端口
        public bool IsBak { get; set; }//是否备份
        public string[] BakFile { get; set; }//是否备份
        public bool AdminiOpen { get; set; }//管理员账户
        public bool IsAuto { get; set; }//是否自动
        public Dictionary<string, string> DatabasePathAll { get; set; }
        //数据库地址
        public bool IsNormalClose { get; set; }//上次关闭是否正常
        public string[] LastControl { get; set; }//最后操作的文件属性
        public DateTime LastCloseDatatime { get; set; }//最后关闭时间
        public DateTime LastStartDatatime { get; set; }//最后启动时间
        public string Versions { get; set; }//版本号
        public string[] Owner { get; set; }//所有者

        public SystemInfoYSQL(bool boo)
        {
            if (!boo) return;
            // Dictionary<string, string> dic;
            this.Control = true;
            this.Port = 10255;
            this.IsBak = true;
            this.BakFile = new[] { string.Empty };
            this.IsAuto = false;

            this.DatabasePathAll = new Dictionary<string, string>
            {
                {"SysDatabase", Lib.DirectoryUtil.GetCurrentDirectory() + "\\dbInfo\\SysDatabase.ydbc"}
            };

            this.IsNormalClose = true;
            this.LastControl = new[] { string.Empty };
            this.LastStartDatatime = DateTime.Now;
            this.Versions = "1.0";
            this.Owner = new[] { "Sys", "admin", "jyf" };
        }
    }
}