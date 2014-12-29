using System.Collections.Generic;

namespace SuperBoy.YSQL.Model
{// ReSharper disable once InconsistentNaming
    public class NameSpaceInfo
    {
        private readonly List<string> _tableNameArray;

        /// <summary>
        /// 该数据库所有者
        /// </summary>
        public List<string> Owner { get; set; }

        /// <summary>
        /// 权限集合（该表的使用者权限）
        /// </summary>
        public Dictionary<EnumArray.Jurisdiction, string[]> JurisdictionArrayList { get; set; }

        /// <summary>
        /// 命名空间集合
        /// </summary>
        private List<string> TableNameArray
        {
            get { return _tableNameArray; }
        }

        public NameSpaceInfo()
        {
            this._tableNameArray = new List<string> {"SysTable"};
            this.Owner = new List<string> {"Sys,Admin,jyf"};
            this.JurisdictionArrayList = new Dictionary<EnumArray.Jurisdiction, string[]>
            {
                {EnumArray.Jurisdiction.Create, new[] {"Sys", "Admin", "jyf"}},
                {EnumArray.Jurisdiction.User, new[] {"Sys", "Admin", "jyf"}},
                {EnumArray.Jurisdiction.Read, new[] {"Guest", "read"}}
            };

        }
        public NameSpaceInfo(string nameSpaceName)
            : this()
        {
            this.TableNameArray.Add(nameSpaceName);
        }
    }
}