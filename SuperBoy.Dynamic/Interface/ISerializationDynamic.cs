using System.Collections.Generic;

namespace SuperBoy.Dynamic.Interface
{
    public interface ISerializationDynamic
    {
        string AnalyticalMain(string analyticalString);
        /// <summary>
        /// 此方法用于将dic集合序列化成json数组
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        string AnalyticalJson(Dictionary<string, string> dictionary);
    }
}
