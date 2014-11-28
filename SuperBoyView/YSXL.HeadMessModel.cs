using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperBoy.View
{
    public class ModelHead
    {
        public string DataBase { get; set; }
        /// <summary>
        /// versions number
        /// </summary>
        public string VersionNumber { get; set; }
        /// <summary>
        /// versions type
        /// </summary>
        public EnumArry.HeadType VersionType { get; set; }
        /// <summary>
        /// create time
        /// </summary>
        public string CreationDate { get; set; }
        /// <summary>
        /// last update time
        /// </summary>
        public string LastUpdateDate { get; set; }
        /// <summary>
        /// create time
        /// </summary>
        public string CreateMan { get; set; }
        /// <summary>
        /// name space 
        /// </summary>
        public string Namespace { get; set; }
        /// <summary>
        /// Modify the number of times 
        /// </summary>
        public string UpdateCode { get; set; }
        /// <summary>
        /// Backup restore namespace 
        /// </summary>
        public string BackUp { get; set; }
        /// <summary>
        /// data format
        /// </summary>
        public EnumArry.ConfigFormat DateType { get; set; }
        /// <summary>
        /// not parameter
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
        /// not parameter create function ,default create time call default
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
        /// create function all override
        /// </summary>
        /// <param name="versionNumber">versions number</param>
        /// <param name="versionType">text type</param>
        /// <param name="creationDate">create time</param>
        /// <param name="createMan">create person</param>
        /// <param name="lastUpdateDate">last update time</param>
        /// <param name="Namespace">Namespace</param>
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
        /// Plain text creation and modification
        /// </summary>
        /// <param name="tableName">table Name</param>
        /// <param name="creationDate">create time</param>
        /// <param name="versionNumber">versions number</param>
        public ModelHead(string tableName, DateTime creationDate, string versionNumber)
            : this(false)
        {
            this.DataBase = tableName;
            this.CreationDate = creationDate.ToString();
            this.VersionNumber = versionNumber;
        }
    }
}
