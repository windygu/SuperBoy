using SuperBoy.Database.Interface;
using SuperBoy.Dynamic.Interface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SuperBoy.Database.Realize;

namespace SuperBoy.Dynamic
{
    class AnalyticalDynamic : IAnalyticalDynamic
    {
        /// <summary>
        /// 在线
        /// </summary>
        /// <param name="Dic"></param>
        /// <returns></returns>
        public string OnLine(Dictionary<string, string> Dic)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="Dic"></param>
        /// <returns></returns>
        public string Select(Dictionary<string, string> Dic)
        {
            List<SqlParameter> Para = new List<SqlParameter>();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ");
            if (Dic.ContainsKey("top"))
            {
                sb.Append("top " + Dic["top"] + " ");
            }
            if (Dic.ContainsKey("Field"))
            {
                string[] itemField = Dic["Dic"].Split(',');
                //查询该数据库有无该字段
                if (connectionDatabaseQueryIsNullCount(itemField))
                {

                }


            }
            else
            {
                sb.Append("top " + Dic["top"] + " ");

            }
            return "";
        }

        private bool connectionDatabaseQueryIsNullCount(string[] itemField)
        {
            return true;
        }

        public string Inster(Dictionary<string, string> Dic)
        {
            throw new NotImplementedException();
        }

        public string Delete(Dictionary<string, string> Dic)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 修改操作
        /// </summary>
        /// <param name="Dic"></param>
        /// <returns></returns>
        public string Update(Dictionary<string, string> Dic)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 管理层
        /// </summary>
        /// <param name="Dic"></param>
        /// <returns></returns>
        public string MANAGE(Dictionary<string, string> Dic)
        {
            throw new NotImplementedException();
        }

        public static void AutoCheckUpdate()
        {
            //获取数据库执行类库
            IDatabaseControl inter = new DatabaseControl();
            //获取字典，写入文本
            inter.AutoCallDatabaseInfo();

        }
    }
}
