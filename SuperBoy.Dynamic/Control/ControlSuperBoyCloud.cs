using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using SuperBoy.Model;
using System.Text;
using SuperBoy.Model.Public;
using SuperBoy.Model.Interface;

namespace SuperBoy.Cloud.Control
{
    /// <summary>
    /// This class is often used to control the program to the class
    /// </summary>
    public class ControlSuperBoyAnalytical : IControlSuperBoyAnalytical
    {
        Dictionary<EnumArry.Database, object> Dic = new Dictionary<EnumArry.Database, object>();
        public object SELECT(Model.Public.DatabseSend database)
        {
            Dic = database.Dic;
            //通过权限表
            //权限审核通过后执行拆分并组装
            StringBuilder SQLStr = new StringBuilder();
            SQLStr.Append("SELECT ");
            //行数
            if (Dic.ContainsKey(EnumArry.Database.TOP))
            {
                SQLStr.Append("TOP " + Dic[EnumArry.Database.TOP] + " ");
            }
            //表名
            SQLStr.Append(Dic[EnumArry.Database.TableName]);
            //条件,如果条件存在，并且条件不等于false
            if (Dic.ContainsKey(EnumArry.Database.WHERE) && Convert.ToBoolean(Dic[EnumArry.Database.WHERE]))
            {
                /*
                 public static Object Parse(Type enumType,string value)
                    例如：(Colors)Enum.Parse(typeof(Colors), "Red")
                 */
                //对比
                EnumArry.WhereType whereType = (EnumArry.WhereType)Enum.Parse(typeof(EnumArry.WhereType), Dic[EnumArry.Database.WHERETYPE].ToString());

                switch (whereType)
                {
                    //相等
                    case EnumArry.WhereType.Equal:
                        SQLStr.Append(" " + Dic[EnumArry.Database.Key].ToString() + " = " + Dic[EnumArry.Database.Value].ToString() + " ");
                        break;
                    //大于参数
                    case EnumArry.WhereType.Greater:
                        SQLStr.Append(" " + Dic[EnumArry.Database.Key].ToString() + " = " + Dic[EnumArry.Database.Value].ToString() + " ");
                        break;
                    //小于参数
                    case EnumArry.WhereType.Less: 
                        SQLStr.Append(" " + Dic[EnumArry.Database.Key].ToString() + " = " + Dic[EnumArry.Database.Value].ToString() + " ");
                        break;
                    //全模糊查询参数
                    case EnumArry.WhereType.Like:
                        SQLStr.Append(" " + Dic[EnumArry.Database.Key].ToString() + " like '" + Dic[EnumArry.Database.Value].ToString() + "' ");
                        break;
                    //左模糊查询
                    case EnumArry.WhereType.LiftLike:
                        SQLStr.Append(" " + Dic[EnumArry.Database.Key].ToString() + " like '%" + Dic[EnumArry.Database.Value].ToString() + "' ");
                        break;
                    //右模糊查询
                    case EnumArry.WhereType.RightLike:
                        SQLStr.Append(" " + Dic[EnumArry.Database.Key].ToString() + " = '" + Dic[EnumArry.Database.Value].ToString() + "%' ");
                        break;
                    default:
                        break;
                }

            }
            return "";
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
    }
}
