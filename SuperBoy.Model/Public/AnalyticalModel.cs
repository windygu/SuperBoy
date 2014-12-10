using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using SuperBoy.Model;
using System.Text;
using SuperBoy.Model.Public;
using SuperBoy.Model.Interface;

namespace SuperBoy.Model.Public
{
    /// <summary>
    /// This class is often used to control the program to the class
    /// </summary>
    public class AnalyticalModel : IAnalyticalModel
    {

        Dictionary<EnumArryModel.Database, object> Dic = new Dictionary<EnumArryModel.Database, object>();
        public string SELECT(Model.Public.DatabseSend database)
        {

            Dic = database.Dic;
            //通过权限表
            //权限审核通过后执行拆分并组装
            StringBuilder SQLStr = new StringBuilder();
            SQLStr.Append("[" + getNoJson(database) + ",");
            SQLStr.Append("{\"Type\":\"SELECT\"");
            //行数
            if (Dic.ContainsKey(EnumArryModel.Database.TOP))
            {
                SQLStr.Append(",\"TOP\":\"" + Dic[EnumArryModel.Database.TOP] + "\"");
            }
            else
            {
                SQLStr.Append(",\"TOP\":\"false\"");
            }


            //字段名
            if (Dic.ContainsKey(EnumArryModel.Database.Field))
            {
                SQLStr.Append(",\"Field\":\"");
                string[] str = (string[])Dic[EnumArryModel.Database.Field];
                if (str[0].Trim() != "*")
                {

                    string item = string.Join(",", str);
                    SQLStr.Append(item + "\"");
                }
                else
                {
                    SQLStr.Append("*\"");
                }
            }
            else
            {
                SQLStr.Append(",\"Field\":*");
            }

            //表名
            SQLStr.Append(",\"From\":\"" + Dic[EnumArryModel.Database.TableName] + "\"");
            //条件,如果条件存在，并且条件不等于false
            if (Dic.ContainsKey(EnumArryModel.Database.WHERE) && Convert.ToBoolean(Dic[EnumArryModel.Database.WHERE]))
            {
                /*
                 public static Object Parse(Type enumType,string value)
                    例如：(Colors)Enum.Parse(typeof(Colors), "Red")
                 */
                //对比
                EnumArryModel.WhereType whereType = (EnumArryModel.WhereType)Enum.Parse(typeof(EnumArryModel.WhereType), Dic[EnumArryModel.Database.WHERETYPE].ToString());
                SQLStr.Append(",\"Key\":" + "\"" + Dic[EnumArryModel.Database.Key] + "\"");
                SQLStr.Append(",\"Value\":" + "\"" + Dic[EnumArryModel.Database.Value] + "\"");
                SQLStr.Append(",\"Where\":\"true\"");
                SQLStr.Append(",\"WhereType\":");
                switch (whereType)
                {
                    case EnumArryModel.WhereType.Equal:
                        SQLStr.Append("\"Equal\"");
                        break;
                    case EnumArryModel.WhereType.Greater:
                        SQLStr.Append("\"Greater\"");
                        break;
                    case EnumArryModel.WhereType.Less:
                        SQLStr.Append("\"Less\"");
                        break;
                    case EnumArryModel.WhereType.Like:
                        SQLStr.Append("\"Like\"");
                        break;
                    case EnumArryModel.WhereType.LiftLike:
                        SQLStr.Append("\"LiftLike\"");
                        break;
                    case EnumArryModel.WhereType.RightLike:
                        SQLStr.Append("\"RightLike\"");
                        break;
                    default:
                        break;
                }
            }
            switch (database.ReturnType)
            {
                case EnumArryModel.ReturnType.JSON:
                    SQLStr.Append(",\"ReturnType\":\"JSON\"");
                    break;
                case EnumArryModel.ReturnType.XML:

                    SQLStr.Append(",\"ReturnType\":\"XML\"");
                    break;
                case EnumArryModel.ReturnType.KeyValue:

                    SQLStr.Append(",\"ReturnType\":\"KeyValue\"");
                    break;
                case EnumArryModel.ReturnType.LIST:

                    SQLStr.Append(",\"ReturnType\":\"LIST\"");
                    break;
                case EnumArryModel.ReturnType.DICT:

                    SQLStr.Append(",\"ReturnType\":\"DICT\"");
                    break;
                default:
                    SQLStr.Append(",\"ReturnType\":\"KeyValue\"");
                    break;
            }

            SQLStr.Append("}]");
            return SQLStr.ToString();
        }

        public string INSTER(Model.Public.DatabseSend database)
        {
            throw new NotImplementedException();
        }

        public string UPDATE(Model.Public.DatabseSend database)
        {
            throw new NotImplementedException();
        }

        public string DELTE(Model.Public.DatabseSend database)
        {
            throw new NotImplementedException();
        }


        public string ONLINE(DatabseSend database)
        {
            throw new NotImplementedException();
        }


        public string MANAGE(DatabseSend database)
        {
            throw new NotImplementedException();
        }

        public string getNoJson(DatabseSend database)
        {
            computModel com = database.No;
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (System.Reflection.PropertyInfo p in com.GetType().GetProperties())
            {
                sb.Append("\"" + p.Name + "\":\"" + p.GetValue(com, null) + "\",");
            }

            return sb.ToString().TrimEnd(',') + "}";

        }
    }
}
