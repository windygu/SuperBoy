using System.Collections.Generic;

namespace SuperBoy.YSQL.Model
{// ReSharper disable once InconsistentNaming
    public class SysDatabase
    {
        /// <summary>
        /// 表集合
        /// </summary>
        public TableMaster TableInfoModel { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public NameSpace NameSpaceModel { get; set; }
        //初始化的时候加载所有的属性
        public SysDatabase(bool boo)
        {
            if (!boo) return;
            var table = new TableMaster();
            this.NameSpaceModel = new NameSpace();
            this.TableInfoModel = new TableMaster();
        }
    }
}