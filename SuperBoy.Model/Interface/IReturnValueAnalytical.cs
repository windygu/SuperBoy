using System.Collections.Generic;

namespace SuperBoy.Model.Interface
{
    interface IReturnValueAnalytical
    {
        /// <summary>
        /// key["","","",""]
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        object AnalyticalJson(object txt);

        object AnalyticalXml(object txt);
        /// <summary>
        /// 简单键值对组合，键盘为列名，值为内容,
        /// $列名
        /// 内容2 内容3 内容4 内容5
        /// $列名
        /// 内容1 内容2 内容3 内容4
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns> 
        object AnalyticalKeyValue(object txt);
        /// <summary>
        /// 该方法自动封装并建造属性，例如要三个字段，自动建造三个字段的Model并封装
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        List<string> AnalyticalList(object txt);

        Dictionary<string, string> AnalyticalDict(object txt);
    }
}
