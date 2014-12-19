using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;

namespace SuperBoy.Database.Parameter
{

    public class DatabaseControl : Interface.IDatabaseControl
    {

        public Dictionary<string, string> AutoCallDatabaseInfo()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            var sql = "SELECT TABLE_NAME,column_name FROM INFORMATION_SCHEMA.COLUMNS";
            string oldKey = string.Empty;
            dic.Add("", "CW100_develop");
            DataSet ds = DBHelper.Query(sql);
            //循环行
            for (int index = 0; index < ds.Tables[0].Rows.Count; index++)
            {

                //如果键存在则追加值，
                if (dic.ContainsKey(ds.Tables[0].Rows[index][0].ToString()))
                {
                first:
                    string item = ds.Tables[0].Rows[index][1].ToString() + ",";
                    dic[ds.Tables[0].Rows[index][0].ToString()] += item;
                }
                //否则追加键
                else
                {
                    dic.Add(ds.Tables[0].Rows[index][0].ToString(), string.Empty);
                    dic[oldKey] = dic[oldKey].Trim(',');
                    oldKey = ds.Tables[0].Rows[index][0].ToString();

                }
                goto first;

            }
            return dic;
        }

    }


}
