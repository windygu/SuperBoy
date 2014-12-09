using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperBoy.Model.Public
{
    public class comput
    {
        public string randomNo { get; set; }
        public string Address { get; set; }
        public string controlType { get; set; }
        public string conputNo { set; get; }
        /// <summary>
        /// 该数据中部分从配置文件读取
        /// </summary>
        public comput()
        {
            randomNo = "ABC123456";
            Address = "China|BeiJing";
            controlType = "DataBase";
            conputNo = "Administrator";
        }
    }
}
