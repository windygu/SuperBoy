using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperBoy.Model.Public
{
    /// <summary>
    /// reques head
    /// </summary>
    public class DatabseSend
    {

        public EnumArryModel.SendType SendType { get; set; }
        public Dictionary<EnumArryModel.Database, object> Dic { get; set; }
        public computModel No { get; set; }
        public EnumArryModel.ReturnType ReturnType { get; set; }
        /// <summary>
        /// testinfo
        /// </summary>
        public DatabseSend()
        {
            //auto No
            this.No = new computModel();
            this.SendType = EnumArryModel.SendType.ONLINE;
            this.ReturnType = EnumArryModel.ReturnType.KeyValue;
        }

        /// <summary>
        /// datainfo
        /// </summary>
        /// <param name="sendType"></param>
        /// <param name="dic"></param>
        /// <param name="returnType"></param>
        public DatabseSend(EnumArryModel.SendType sendType, Dictionary<EnumArryModel.Database, object> dic, EnumArryModel.ReturnType returnType)
            : this()
        {
            //No是软件类型+软件类型编号+地区+许可证或临时许可证+随机字符串（每次发送都不相同）
            //例如mac电脑桌面应用程序就是 [CI][MacDesk]{China|beijing}{Kty03051157}{00035252}
            //如果要查询某个数据库
            //例如查询product表中的前十行 

            this.SendType = sendType;
            this.Dic = dic;
            this.ReturnType = returnType;
        }
    }

}
