using System.Collections.Generic;

namespace SuperBoy.YSQL.Model
{// ReSharper disable once InconsistentNaming
    public class DatabaseInfo
    {
        /// <summary>
        /// 表集合
        /// </summary>
        public MasterTable TableInfoModel { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public NameSpaceInfo NameSpaceModel { get; set; }
        //初始化的时候加载所有的属性
        public DatabaseInfo(bool boo)
        {
            if (!boo) return;
            var table = new MasterTable();
            this.NameSpaceModel = new NameSpaceInfo();
            this.TableInfoModel = new MasterTable();
        }
    }
}