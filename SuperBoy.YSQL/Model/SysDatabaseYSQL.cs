using System.Collections.Generic;

namespace SuperBoy.YSQL.Model
{// ReSharper disable once InconsistentNaming
    public class SysDatabaseYSQL
    {
        /// <summary>
        /// 表集合
        /// </summary>
        public TableInfoModelYSQL TableInfoModel { get; set; }

        public NameSpaceModelYSQL NameSpaceModel { get; set; }
        //初始化的时候加载所有的属性
        public SysDatabaseYSQL(bool boo)
        {
            if (boo) return;
            var table = new TableInfoModelYSQL();
            this.NameSpaceModel = new NameSpaceModelYSQL();
            this.TableInfoModel = new TableInfoModelYSQL();
        }
    }
}