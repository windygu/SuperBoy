using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Json;
using Newtonsoft.Json;

namespace ResearchAndDevelopment
{
    class TheNewMains
    {
        #region
        private void importTxtNoAdd()
        {
            //string line;
            //string sFileName = "";
            ////if (openFileDialog1.ShowDialog() == DialogResult.OK)
            ////{
            ////    sFileName = openFileDialog1.FileName;

            //    dtTemp.Rows.Clear();
            //    iXH = 0;

            //    System.IO.StreamReader file = new System.IO.StreamReader(sFileName);
            //    while ((line = file.ReadLine()) != null)
            //    {
            //        // if(line.Length==11 )
            //        DataRow dr = dtTemp.NewRow();
            //        dr[0] = iXH + 1;
            //        dr[1] = "临时用户";
            //        dr[2] = line;
            //        dtTemp.Rows.Add(dr);
            //        iXH++;
            //    }

            //    file.Close();
            //    System.Console.ReadLine();
            //    //bindGrid("");
            //}

            /*
                      HeadMessModel obj = new HeadMessModel(DateTime.Now, "v1.13");
                      //StuInfoEntity obj = new StuInfoEntity();
                      Type t = typeof(HeadMessModel);

                      PropertyInfo[] myPropertyInfo;
                      // Get the properties of 'Type' class object.
                      myPropertyInfo = t.GetProperties();
                      for (int i = 0; i < myPropertyInfo.Length; i++)
                      {

                          myPropertyInfo[i].SetValue(obj, i.ToString(), null);
                          Console.WriteLine(myPropertyInfo[i].GetValue(obj, null));


                      }
           
              //获取当前程序集
              Assembly assembly = Assembly.GetExecutingAssembly();
              //获取全名
              object obj = assembly.CreateInstance("SuperBoy.HeadMessModel");
              //相当于实例化New一个类型
              Type type = obj.GetType();
              //数组类型，将所有属性装进一个数组里面
              System.Reflection.PropertyInfo[] finfo = type.GetProperties();
              //循环这个数组，取值
              foreach (PropertyInfo prop in finfo)
              {
                
                  Console.WriteLine("属性名{0}, CanRead={1}, CanWrite={2}, 数据类型={3}",prop.Name, prop.CanRead, prop.CanWrite, prop.PropertyType.Name);
                  Console.WriteLine("数据类型是否是抽象类{0}, 是否是类{1}", prop.PropertyType.IsAbstract, prop.PropertyType.IsClass);
                  Console.WriteLine("数据类型是否是数组{0}, 是否是枚举{1}", prop.PropertyType.IsArray, prop.PropertyType.IsEnum);
                  Console.WriteLine();
               
                  //Console.WriteLine(prop.Name+"--->");
                  Console.WriteLine(prop.GetValue(obj, null));
              }
          */

            /*
           //string itemString;
           //int Int;
           //bool itemBool;
           //double itemDouble;
           //object itemObject;
           //float itemFloat;
           // Mina(out itemString, out Int, out itemBool, out itemDouble, out itemObject, out itemFloat);
           //Console.WriteLine(itemString);

           System.Diagnostics.Stopwatch stopwatch = new Stopwatch();
           stopwatch.Start(); //  开始监视代码运行时间
           //  you code ....
           //out函数
           int vars;
           mains(out vars);
           //传统返回值
           //int item = mains();
           stopwatch.Stop(); //  停止监视
           TimeSpan timespan = stopwatch.Elapsed; //  获取当前实例测量得出的总时间
           double hours = timespan.TotalHours; // 总小时
           double minutes = timespan.TotalMinutes;  // 总分钟
           double seconds = timespan.TotalSeconds;  //  总秒数
           double milliseconds = timespan.TotalMilliseconds;  //  总毫秒数
           Console.WriteLine("总秒数:" + seconds + "总毫秒数:" + milliseconds);
            */
            /*
public static void Mina(out string itemString, out int itemInt, out bool itemBool, out double itemDouble, out object itemObject, out float itemFloat)
{
    itemString = "string";
    itemInt = 0xA;
    itemBool = false;
    itemDouble = 14.56;
    itemObject = 151635.5666F;
    itemFloat = 456.1111111111111111F;
}
*/
            /*
            public static void mains(out int items)
            {
                items = 0;
                for (int item = 0; item < 10000000; item++)
                {
                    items += 1;
                }
            }
            public static int mains()
            {
                int items = 0;
                for (int item = 0; item < 10000000; item++)
                {
                    items += 1;
                }
                return items;
            }
             */




        }
        #endregion
        public static Dictionary<string, object> JsonT(string refs)
        {
            refs = @"{'item':'items'}";
            refs = @"{'item':'items'},'items':'items'";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            return null;
        }
    }
}
