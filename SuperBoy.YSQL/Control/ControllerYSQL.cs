namespace SuperBoy.YSQL.Control
{
    /// <summary>
    /// 控制类库
    /// </summary>
    /// ReSharper disable once InconsistentNaming
    internal static class ControllerYSQL
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
    }
}