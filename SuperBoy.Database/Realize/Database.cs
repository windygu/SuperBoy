using System.Collections.Generic;
using System.Data;
using SuperBoy.Database.Interface;

namespace SuperBoy.Database.Realize
{

    public class DatabaseControl : IDatabaseControl
    {

        //自动获取表名表列
        public Dictionary<string, string> AutoCallDatabaseInfo()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            var sql = "SELECT TABLE_NAME,column_name FROM INFORMATION_SCHEMA.COLUMNS";
            string oldKey = string.Empty;
            dic.Add("", "CW100_develop");
            DataSet ds = DbHelper.Query(sql);
            //循环行
            for (int index = 0; index < ds.Tables[0].Rows.Count; index++)
            {

                //如果键存在则追加值，
                if (dic.ContainsKey(ds.Tables[0].Rows[index][0].ToString()))
                {
                    string item = ds.Tables[0].Rows[index][1].ToString() + ",";
                    dic[ds.Tables[0].Rows[index][0].ToString()] += item;
                }
                //否则追加键
                else
                {
                    dic.Add(ds.Tables[0].Rows[index][0].ToString(), ds.Tables[0].Rows[index][1].ToString() + ",");
                    dic[oldKey] = dic[oldKey].Trim(',');
                    oldKey = ds.Tables[0].Rows[index][0].ToString();
                }

            }
            return dic;
        }

    }


}
