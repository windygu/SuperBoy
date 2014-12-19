using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SuperBoy.Database.Interface;

namespace SuperBoy.Database.Realize
{

    public class DatabaseControl : IDatabaseControl
    {
        public DatabaseControl()
        {
            //每次新建的时候都会刷新值
            _builder = new StringBuilder();
        }

        //自动获取表名表列
        public Dictionary<string, string> AutoCallDatabaseInfo()
        {
            var dic = new Dictionary<string, string>();
            const string sql = "SELECT TABLE_NAME,column_name FROM INFORMATION_SCHEMA.COLUMNS";
            var oldKey = string.Empty;
            dic.Add("", "CW100_develop");
            var ds = DbHelper.Query(sql);

            //循环行
            for (var index = 0; index < ds.Tables[0].Rows.Count; index++)
            {

                //如果键存在则追加值，
                if (dic.ContainsKey(ds.Tables[0].Rows[index][0].ToString()))
                {
                    var item = ds.Tables[0].Rows[index][1].ToString() + ",";
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

        /// <summary>
        /// 自动压缩序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        private readonly StringBuilder _builder;

        public void AutoCompressSerialize<T>(T value)
        {
            string getType = value.GetType().ToString();

            switch (getType)
            {
                case "System.String":
                    _builder.Append("\"" + value + "\",");
                    break;
                case "System.String[]":
                    var values = value as string[];
                    if (values != null) _builder.Append("[\"" + string.Join("\",\"", values) + "\"],");
                    break;
                case "System.DateTime":
                    _builder.Append("\"" + value + "\",");
                    break;
                case "System.Int32[]":
                    var intValue = value as int[];
                    Debug.Assert(intValue != null, "intValue != null");
                    var result = "[" + string.Join(",", intValue.Select(i => i.ToString()).ToArray()) + "],";
                    break;
                case "System.Boolean":
                    _builder.Append("\"" + value + "\",");
                    break;
                case "System.Int32":
                    _builder.Append(value + ",");
                    break;

                default:
                    //System.Collections.Generic.Dictionary//`2[System.String,System.String]
                    //System.Collections.Generic.List`1[System.String]
                    //可能是键值对或list
                    //判断是否是常见类型
                    if (getType.IndexOf("Generic", StringComparison.Ordinal) != -1)
                    {
                        var item = getType.Substring(getType.IndexOf("Generic", StringComparison.Ordinal) + 8);
                        //分拆属性
                        var valueCount = item.Substring(item.IndexOf('`')+1, 1);//获取键值个数
                        var genre = item.Substring(0, item.IndexOf('`'));//获取类型
                        switch (genre)
                        {
                            case "Dictionary":

                                break;
                            case "List":

                                break;
                        }
                    }
                    break;
            }
        }
    }

}
