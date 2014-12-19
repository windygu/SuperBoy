using System.Collections.Generic;

namespace SuperBoy.YSQL.Model
{// ReSharper disable once InconsistentNaming
    public class NameSpaceModelYSQL
    {
        private readonly List<string> _tableNameArray;

        /// <summary>
        /// 该数据库所有者
        /// </summary>
        public List<string> Owner { get; set; }

        /// <summary>
        /// 权限集合（该表的使用者权限）
        /// </summary>
        public Dictionary<EnumArrayYSQL.Jurisdiction, string[]> JurisdictionArrayList { get; set; }

        /// <summary>
        /// 命名空间集合
        /// </summary>
        private List<string> TableNameArray
        {
            get { return _tableNameArray; }
        }

        public NameSpaceModelYSQL()
        {
            this._tableNameArray = new List<string> {"SysTable"};
            this.Owner = new List<string> {"Sys,Admin,jyf"};
            this.JurisdictionArrayList = new Dictionary<EnumArrayYSQL.Jurisdiction, string[]>
            {
                {EnumArrayYSQL.Jurisdiction.Create, new[] {"Sys", "Admin", "jyf"}},
                {EnumArrayYSQL.Jurisdiction.User, new[] {"Sys", "Admin", "jyf"}},
                {EnumArrayYSQL.Jurisdiction.Read, new[] {"Guest", "read"}}
            };

        }
        public NameSpaceModelYSQL(string nameSpaceName)
            : this()
        {
            this.TableNameArray.Add(nameSpaceName);
        }
    }
}