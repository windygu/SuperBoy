using System;
using System.Collections.Generic;

namespace SuperBoy.YSQL.Model
{
    // ReSharper disable once InconsistentNaming
    public class TableInfoModelYSQL
    {
        /*
         "Address": "D:\\tdb\\table\\table.tdb",
           "OtherAddress": "D:\\tdb\\table\\table1.tdb",
           "lastupdateTime": "2014-12-15 16:50",
           "IsBak": "true",
           "bakAddress": "address",
           "TableCount": "10000",
           "lastUpdateCount": "12345",
           "lastUpdateSumCount": "50",
           "Speediness": "true",
           "Rollback": "true",
           "RollbackAddress": "D:\\tdb\\table\\Rollback.bak",
           "trigger": {
               "coltrigger": {
               },
               "behaviorTrigger": {}
           },
           "JointQuery": [
               "true"
           ]
         */
        public string Address { get; set; }//地址
        public string[] OtherAddress { get; set; }//其他地址
        public DateTime? LastupdateTime { get; set; }//最后修改时间
        public bool IsBak { get; set; }//是否备份
        public string[] BakAddress { get; set; }//备份地址
        public int TableCount { get; set; }//表一共有多少条数据
        public int[] LastUpdateCount { get; set; }//最后修改的列
        public int LastUpdateSumCount { get; set; }//最后影响的总条数
        public bool Speediness { get; set; }//快速加载技术
        public Boolean Rollback { get; set; }//是否回滚
        public string[] RollbackAddress { get; set; }//回滚地址
        public int RollbackCount { get; set; }//回滚次数/表(一个物理表记录多少次回滚)
        public int LastRollbackCount { get; set; }//最后回滚条数
        public Dictionary<EnumArrayYSQL.Trigger, TriggerModelYSQL> Trigger { get; set; }
        public string[] JointQuery { get; set; }//链接查询
        public string[] Owner { get; set; }//所有者
        public string[] User { get; set; }//用户权限
        public string[] PrimaryKey { get; set; }//主键
        public string[] Foreignkey { get; set; }//外键
        public string[] Uniqueness { get; set; }//唯一键
        public string[] Fields { get; set; }//字段


        public TableInfoModelYSQL()
        {
            this.Address = Lib.DirectoryUtil.GetCurrentDirectory() + "\\tdb";
            this.BakAddress = new[] { Lib.DirectoryUtil.GetCurrentDirectory() + "\\bak" };
            this.IsBak = true;
            this.LastUpdateSumCount = 0;
            this.LastRollbackCount = 0;
            this.Owner = new[] { "sys", "admin", "jyf" };
            this.Rollback = true;
            this.RollbackCount = 1000;
            this.Speediness = true;
            this.TableCount = 1000;
            this.User = new string[] { "jyf=rw", "admin=rw", "sys=rw" };
            var triggerMo = new TriggerModelYSQL();
            this.LastUpdateCount = new int[] { 0 };
            //triggerMo.BehaviorTrigger.Add(EnumArrayYsql.BehaviorTrigger.Inster, "delete:none");
            //this.Trigger.Add(EnumArrayYsql.Trigger.BehaviorTrigger, triggerMo);
        }
    }
}