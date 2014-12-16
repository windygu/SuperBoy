using System;
using System.Collections.Generic;
using System.Text;
using SuperBoy.Model.Interface;

namespace SuperBoy.Model.Public
{
    /// <summary>
    ///     This class is often used to control the program to the class
    /// </summary>
    public class AnalyticalModel : IAnalyticalModel
    {
        private Dictionary<EnumArryModel.Database, object> _dic = new Dictionary<EnumArryModel.Database, object>();

        public string Select(DatabseSend database)
        {
            _dic = database.Dic;
            //通过权限表
            //权限审核通过后执行拆分并组装
            var sqlStr = new StringBuilder();
            sqlStr.Append("[" + GetNoJson(database) + ",");
            sqlStr.Append("{\"Type\":\"SELECT\"");
            //行数
            if (_dic.ContainsKey(EnumArryModel.Database.Top))
            {
                sqlStr.Append(",\"TOP\":\"" + _dic[EnumArryModel.Database.Top] + "\"");
            }
            else
            {
                sqlStr.Append(",\"TOP\":\"false\"");
            }


            //字段名
            if (_dic.ContainsKey(EnumArryModel.Database.Field))
            {
                sqlStr.Append(",\"Field\":\"");
                var str = (string[]) _dic[EnumArryModel.Database.Field];
                if (str[0].Trim() != "*")
                {
                    var item = string.Join(",", str);
                    sqlStr.Append(item + "\"");
                }
                else
                {
                    sqlStr.Append("*\"");
                }
            }
            else
            {
                sqlStr.Append(",\"Field\":*");
            }

            //表名
            sqlStr.Append(",\"From\":\"" + _dic[EnumArryModel.Database.TableName] + "\"");
            //条件,如果条件存在，并且条件不等于false
            if (_dic.ContainsKey(EnumArryModel.Database.Where) && Convert.ToBoolean(_dic[EnumArryModel.Database.Where]))
            {
                /*
                 public static Object Parse(Type enumType,string value)
                    例如：(Colors)Enum.Parse(typeof(Colors), "Red")
                 */
                //对比
                var whereType =
                    (EnumArryModel.WhereType)
                        Enum.Parse(typeof (EnumArryModel.WhereType), _dic[EnumArryModel.Database.Wheretype].ToString());
                sqlStr.Append(",\"Key\":" + "\"" + _dic[EnumArryModel.Database.Key] + "\"");
                sqlStr.Append(",\"Value\":" + "\"" + _dic[EnumArryModel.Database.Value] + "\"");
                sqlStr.Append(",\"Where\":\"true\"");
                sqlStr.Append(",\"WhereType\":");
                switch (whereType)
                {
                    case EnumArryModel.WhereType.Equal:
                        sqlStr.Append("\"Equal\"");
                        break;
                    case EnumArryModel.WhereType.Greater:
                        sqlStr.Append("\"Greater\"");
                        break;
                    case EnumArryModel.WhereType.Less:
                        sqlStr.Append("\"Less\"");
                        break;
                    case EnumArryModel.WhereType.Like:
                        sqlStr.Append("\"Like\"");
                        break;
                    case EnumArryModel.WhereType.LiftLike:
                        sqlStr.Append("\"LiftLike\"");
                        break;
                    case EnumArryModel.WhereType.RightLike:
                        sqlStr.Append("\"RightLike\"");
                        break;
                    default:
                        break;
                }
            }
            switch (database.ReturnType)
            {
                case EnumArryModel.ReturnType.Json:
                    sqlStr.Append(",\"ReturnType\":\"JSON\"");
                    break;
                case EnumArryModel.ReturnType.Xml:

                    sqlStr.Append(",\"ReturnType\":\"XML\"");
                    break;
                case EnumArryModel.ReturnType.KeyValue:

                    sqlStr.Append(",\"ReturnType\":\"KeyValue\"");
                    break;
                case EnumArryModel.ReturnType.List:

                    sqlStr.Append(",\"ReturnType\":\"LIST\"");
                    break;
                case EnumArryModel.ReturnType.Dict:

                    sqlStr.Append(",\"ReturnType\":\"DICT\"");
                    break;
                default:
                    sqlStr.Append(",\"ReturnType\":\"KeyValue\"");
                    break;
            }

            sqlStr.Append("}]");
            return sqlStr.ToString();
        }

        public string Inster(DatabseSend database)
        {
            throw new NotImplementedException();
        }

        public string Update(DatabseSend database)
        {
            throw new NotImplementedException();
        }

        public string Delete(DatabseSend database)
        {
            throw new NotImplementedException();
        }

        public string Online(DatabseSend database)
        {
            throw new NotImplementedException();
        }

        public string Manage(DatabseSend database)
        {
            throw new NotImplementedException();
        }

        public string GetNoJson(DatabseSend database)
        {
            var com = database.No;
            var sb = new StringBuilder();
            sb.Append("{");
            foreach (var p in com.GetType().GetProperties())
            {
                sb.Append("\"" + p.Name + "\":\"" + p.GetValue(com, null) + "\",");
            }

            return sb.ToString().TrimEnd(',') + "}";
        }
    }
}