using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperBoyView
{
    public class ModelHead
    {
        public string DataBase { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string VersionNumber { get; set; }
        /// <summary>
        /// 版本类型
        /// </summary>
        public EnumArry.HeadType VersionType { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreationDate { get; set; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public string LastUpdateDate { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateMan { get; set; }
        /// <summary>
        /// 命名空间
        /// </summary>
        public string Namespace { get; set; }
        /// <summary>
        /// 修改次数
        /// </summary>
        public string UpdateCode { get; set; }
        /// <summary>
        /// 备份还原命名空间
        /// </summary>
        public string BackUp { get; set; }
        /// <summary>
        /// 数据格式
        /// </summary>
        public EnumArry.ConfigFormat DateType { get; set; }
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public ModelHead()
        {
            this.DataBase = "TableName";
            this.VersionNumber = "VersionNumber";
            this.VersionType = EnumArry.HeadType.bak;
            this.CreationDate = "CreationDate";
            this.LastUpdateDate = "LastUpdateDate";
            this.CreateMan = "CreateMan";
            this.Namespace = "Namespace";
            this.UpdateCode = "UpdateCode";
            this.BackUp = "BackUp";
        }

        /// <summary>
        /// 无参构造函数,默认创建时候调用默认
        /// </summary>
        public ModelHead(bool IsCreate)
        {
            if (IsCreate)
            {
                this.DataBase = "TableName";
                this.VersionNumber = "Default ysxl_1.0";
                this.VersionType = EnumArry.HeadType.Plain;
                this.CreationDate = DateTime.Now.ToString();
                this.CreateMan = "MasterUser";
                this.LastUpdateDate = DateTime.Now.ToString();
                this.Namespace = "SuperBoy";
                this.UpdateCode = "0";
            }
        }
        /*
        /// <summary>
        /// 构造函数全部重写
        /// </summary>
        /// <param name="versionNumber">版本号</param>
        /// <param name="versionType">文本属性</param>
        /// <param name="creationDate">创建时间</param>
        /// <param name="createMan">创建人</param>
        /// <param name="lastUpdateDate">最后修改时间</param>
        /// <param name="Namespace">命名空间</param>
        public HeadMessModel(string versionNumber, EnumArry.HeadType versionType, DateTime creationDate, string createMan, string nameSpace, int updateCode)
        {
            this.VersionNumber = versionNumber;
            this.VersionType = versionType;
            this.CreationDate = creationDate.ToString();
            this.CreateMan = createMan;
            this.LastUpdateDate = DateTime.Now.ToString();
            this.Namespace = nameSpace;
            this.UpdateCode = updateCode;
        }
        */
        /// <summary>
        /// 普通文本创建与修改
        /// </summary>
        /// <param name="creationDate">创建的时间</param>
        /// <param name="versionNumber">版本号</param>
        /// <param name="nameSpace">命名空间</param>
        public ModelHead(string tableName, DateTime creationDate, string versionNumber)
            : this(false)
        {
            this.DataBase = tableName;
            this.CreationDate = creationDate.ToString();
            this.VersionNumber = versionNumber;
        }
    }
}
