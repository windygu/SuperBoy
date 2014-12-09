using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SuperBoy.Model.Public;
using System.Data;
using SuperBoyView.SuperBoyControl;
using System.IO;
using System.Xml.Serialization;

namespace SuperBoy.View
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] arge)
        {
            #region 单数据查询
            Dictionary<EnumArry.Database, object> DicSelect = new Dictionary<EnumArry.Database, object>();
            //带条件
            DicSelect.Add(EnumArry.Database.WHERE, true);
            //小于参数
            DicSelect.Add(EnumArry.Database.WHERETYPE, EnumArry.WhereType.Less);
            //显示10条
            DicSelect.Add(EnumArry.Database.TOP, 10);
            //参数名
            DicSelect.Add(EnumArry.Database.Key, "BrandID");
            //值
            DicSelect.Add(EnumArry.Database.Value, 25);
            //表名
            DicSelect.Add(EnumArry.Database.TableName, "CW100_Product");

            Model.Public.DatabseSend sendSelect = new Model.Public.DatabseSend(EnumArry.SendType.SELECT, DicSelect);

            string item = Serialize(sendSelect);

            SuperBoyICloudClient client = new SuperBoyICloudClient();
           // string item = client.ToString();
            client.SuperBoyC(sendSelect);

            #endregion

            /*

            #region 单数据修改
            Dictionary<EnumArry.Database, object> DicUPdate = new Dictionary<EnumArry.Database, object>();
            //带条件
            DicUPdate.Add(EnumArry.Database.WHERE, true);
            //小于参数
            DicUPdate.Add(EnumArry.Database.WHERETYPE, EnumArry.WhereType.Equal);
            //参数名
            DicUPdate.Add(EnumArry.Database.Key, "PID");
            //参数名值
            DicUPdate.Add(EnumArry.Database.Value, "1031");
            //值
            DicUPdate.Add(EnumArry.Database.UpdateKey, "ProductName");
            //修改值
            DicUPdate.Add(EnumArry.Database.UpdateValue, "555555");
            //表名
            DicUPdate.Add(EnumArry.Database.TableName, "CW100_Product");


            Model.Public.DatabseSend sendUpdate = new Model.Public.DatabseSend(EnumArry.SendType.SELECT, DicUPdate);
            #endregion
            #region 单数据删除
            Dictionary<EnumArry.Database, object> DicDelete = new Dictionary<EnumArry.Database, object>();
            //带条件
            DicDelete.Add(EnumArry.Database.WHERE, true);
            //小于参数
            DicDelete.Add(EnumArry.Database.WHERETYPE, EnumArry.WhereType.Less);
            //参数名
            DicDelete.Add(EnumArry.Database.Key, "PID");
            //值
            DicDelete.Add(EnumArry.Database.Value, 1000);
            //表名
            DicDelete.Add(EnumArry.Database.TableName, "CW100_Product");

            Model.Public.DatabseSend sendDelete = new Model.Public.DatabseSend(EnumArry.SendType.SELECT, DicDelete);
            #endregion
            #region 单数据增加
            /*
            Dictionary<EnumArry.Database, object> DicInster = new Dictionary<EnumArry.Database, object>();
            //带条件
            DicInster.Add(EnumArry.Database.WHERE, true);
            //小于参数
            DicInster.Add(EnumArry.Database.WHERETYPE, EnumArry.WhereType.Less);
            //显示10条
            DicInster.Add(EnumArry.Database.TOP, 10);
            //参数名
            DicInster.Add(EnumArry.Database.Key, "BrandID");
            //值
            DicInster.Add(EnumArry.Database.Value, 25);

            Model.Public.DatabseSend sendInster = new Model.Public.DatabseSend(EnumArry.SendType.SELECT, DicInster);
             
            #endregion
            */
        }

        /// <summary>

        /// 序列化成xml字符串

        /// </summary>

        /// <param name="obj"></param>

        /// <returns>序列化后的字符串</returns>
        public static string Serialize(object obj)
        {

            XmlSerializer xs = new XmlSerializer(obj.GetType());

            using (MemoryStream ms = new MemoryStream())
            {

                System.Xml.XmlTextWriter xtw = new System.Xml.XmlTextWriter(ms, System.Text.Encoding.UTF8);

                xtw.Formatting = System.Xml.Formatting.Indented;

                xs.Serialize(xtw, obj);

                ms.Seek(0, SeekOrigin.Begin);

                using (StreamReader sr = new StreamReader(ms))
                {

                    string str = sr.ReadToEnd();

                    xtw.Close();

                    ms.Close();

                    return str;

                }

            }

        }
    }
}
