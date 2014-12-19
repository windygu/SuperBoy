using SuperBoy.Database.Realize;
using SuperBoy.Dynamic.Interface;
using SuperBoy.Dynamic.Realize;
using SuperBoy.YSQL.Interface;
using SuperBoy.YSQL.Realize;

namespace SuperBoy.Dynamic.Control
{

    /// <summary>
    /// 该表存储着自动调用的所有方法
    /// </summary>
    public class DynamicControl : IDynamicControl
    {
        private readonly IControlYSQL _ysql = new YsqlControl();

        /// <summary>
        /// 自动调用函数
        /// </summary>
        public void AutoCallDataTableAndField()
        {
            var database = new DatabaseControl();
            var dictionary = database.AutoCallDatabaseInfo();
            ISerializationDynamic interDynamic = new SerializationDynamic();
            var json = interDynamic.AnalyticalJson(dictionary);
            //可用，非常棒
            //string deserializedProduct = JsonConvert.SerializeObject(dictionary);
            //存储到ysql数据库中
            //_ysql.InsterInto("AutoCallDataTableAndField", json);
        }
    }
}