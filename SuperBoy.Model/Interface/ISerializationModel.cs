using SuperBoy.Model.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperBoy.Model.Interface
{
    public interface ISerializationModel
    {
        /// <summary>
        /// 发送字符串拼接
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        string SuperBoyAnalytical(Model.Public.DatabseSend database);
        /// <summary>
        /// 返回字符串序列
        /// </summary>
        /// <param name="returnValue">序列类型</param>
        /// <param name="Datatxt">序列字符串</param>
        /// <returns></returns>
        object DataSuperBoyAnalytical(EnumArryModel.ReturnType returnValue, object txt);
    }
}
