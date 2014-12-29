using System;
using SuperBoy.YSQL.Model;

namespace SuperBoy.YSQL.Control
{
    /// <summary>
    /// 控制类库
    /// </summary>
    /// ReSharper disable once InconsistentNaming
    public static class ControllerYSQL
    {
        public static void Main()
        {
            //读取json某个键
            //JObject json = (JObject)JsonConvert.DeserializeObject(deserializedProduct);
            //JArray array = (JArray)json["Address"];

            //循环某个键里面的数组
            //var json = (JObject)JsonConvert.DeserializeObject(deserializedProduct);
            //var array = (JArray)json["LastUpdateCount"];
            //foreach (var jObject in array)
            //{
            //    //赋值属性
            //}

            //循环json里面的键和值
            //JsonReader reader = new JsonTextReader(new StringReader(jsons));
            //while (reader.Read())
            //{
            //    Console.WriteLine(reader.Value);
            //    // Console.WriteLine(reader.TokenType + "\t\t" + reader.ValueType + "\t\t" + reader.Value);
            //}

            //将json读取到list但不支持对象读取
            //JavaScriptSerializer json = new JavaScriptSerializer();
            //List<TableInfoModel> list1 = json.Deserialize<List<TableInfoModel>>(jsons);


        }
        /// <summary>
        /// 更改当前运行状态
        /// </summary>
        /// <param name="statusHead"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string[] CurrentStatus(EnumArray.TableHead statusHead, string status)
        {
            // 数据库当前状态，0为命名空间，1为数据库名，2为表名，3为查询过程名，地址，或查询语句地址
            if (status == null) return ServiceYSQL.CurrentStatusget;
            switch (statusHead)
            {
                case EnumArray.TableHead.DatabaseName:
                    ServiceYSQL.CurrentStatusget[1] = status;
                    ServiceYSQL.CurrentStatusget[2] = "SysTable";
                    ServiceYSQL.CurrentStatusget[3] = "Null";
                    break;
                case EnumArray.TableHead.TableName:
                    ServiceYSQL.CurrentStatusget[2] = status;
                    break;
                case EnumArray.TableHead.Namespace:
                    ServiceYSQL.CurrentStatusget[0] = status;
                    break;
                case EnumArray.TableHead.server:
                    ServiceYSQL.CurrentStatusget[4] = status;
                    break;
                case EnumArray.TableHead.Subjection:
                    break;
                case EnumArray.TableHead.Modifier:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("statusHead");
            }
            return ServiceYSQL.CurrentStatusget;
        }
    }
}