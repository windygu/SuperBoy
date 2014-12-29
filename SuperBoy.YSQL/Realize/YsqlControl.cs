using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SuperBoy.YSQL.Interface;
using SuperBoy.YSQL.Model;

namespace SuperBoy.YSQL.Realize
{
    public class YsqlControl : IControlYSQL
    {
        public bool IsTable()
        {
            //读取该系统信息表，查看有无该表
            return true;
        }

        /// <summary>
        ///     自动更新并备份到数据库
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string UpdateMasterInfo(string key, string value)
        {
            return "";
        }

        /// <summary>
        ///     解析系统信息，json到list集合
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public List<MasterTable> AnalysisTableInfo(string txt)
        {
            return null;
        }

        /// <summary>
        ///     从list集合变成json
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public string AnalysisTojson(IEnumerable<MasterTable> list)
        {
            var strLongBuilder = new StringBuilder();
            strLongBuilder.Append("[");
            //循环集合
            foreach (var tableInfo in list)
            {
                strLongBuilder.Append("{");
                foreach (var property in tableInfo.GetType().GetProperties())
                {
                    //string name = property.Name;
                    var value = property.GetValue(tableInfo, null);
                    if (value == null || value.Equals(0)) continue;
                    strLongBuilder.Append("\"" + property.Name + "\":");
                    var type = value.GetType();
                    switch (type.ToString())
                    {
                        case "System.String":
                            strLongBuilder.Append("\"" + value + "\",");
                            break;
                        case "System.String[]":
                            var values = value as string[];
                            if (values != null) strLongBuilder.Append("[\"" + string.Join("\",\"", values) + "\"],");
                            break;
                        case "System.DateTime":
                            strLongBuilder.Append("\"" + value + "\",");
                            break;
                        case "System.Int32[]":
                            var intValue = value as int[];
                            Debug.Assert(intValue != null, "intValue != null");
                            var result = "[" + string.Join(",", intValue.Select(i => i.ToString()).ToArray()) + "],";
                            strLongBuilder.Append(result);
                            break;
                        case "System.Boolean":
                            strLongBuilder.Append("\"" + value + "\",");
                            break;
                        case "System.Int32":
                            strLongBuilder.Append(value + ",");
                            break;
                        case "System.":
                            strLongBuilder.Append(value + ",");
                            break;
                        default:
                            throw new Exception("No Type");
                    }
                }
                strLongBuilder.Remove(strLongBuilder.Length - 1, 1);
                strLongBuilder.Append("}");
            }
            strLongBuilder.Append("]");
            return strLongBuilder.ToString();
        }
    }
}