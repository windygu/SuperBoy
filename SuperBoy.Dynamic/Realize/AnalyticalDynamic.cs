using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using SuperBoy.Database.Interface;
using SuperBoy.Database.Realize;
using SuperBoy.Dynamic.Interface;

namespace SuperBoy.Dynamic
{
    class AnalyticalDynamic : IAnalyticalDynamic
    {
        /// <summary>
        /// 在线
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string OnLine(Dictionary<string, string> dic)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string Select(Dictionary<string, string> dic)
        {
            var para = new List<SqlParameter>();
            var sb = new StringBuilder();
            sb.Append("SELECT ");
            if (dic.ContainsKey("top"))
            {
                sb.Append("top " + dic["top"] + " ");
            }
            if (dic.ContainsKey("Field"))
            {
                var itemField = dic["Dic"].Split(',');
                //查询该数据库有无该字段
                if (ConnectionDatabaseQueryIsNullCount(itemField))
                {

                }
            }
            else
            {
                sb.Append("top " + dic["top"] + " ");

            }
            return "";
        }
        /// <summary>
        /// 查询数据库有无该字段
        /// </summary>
        /// <param name="itemField"></param>
        /// <returns></returns>
        private static bool ConnectionDatabaseQueryIsNullCount(string[] itemField)
        {
            //调用本地存储结构

            return true;
        }

        public string Inster(Dictionary<string, string> dic)
        {
            throw new NotImplementedException();
        }

        public string Delete(Dictionary<string, string> dic)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 修改操作
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string Update(Dictionary<string, string> dic)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 管理层
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string Manage(Dictionary<string, string> dic)
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
