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

        public EnumArry.SendType SendType { get; set; }
        public Dictionary<EnumArry.Database, object> Dic { get; set; }
        public comput No { get; set; }

        public DatabseSend()
        {

        }
        public DatabseSend(EnumArry.SendType sendType, Dictionary<EnumArry.Database, object> dic)
        {
            //No是软件类型+软件类型编号+地区+许可证或临时许可证+随机字符串（每次发送都不相同）
            //例如mac电脑桌面应用程序就是 [CI][MacDesk]{China|beijing}{Kty03051157}{00035252}
            //如果要查询某个数据库
            //例如查询product表中的前十行 

            this.SendType = sendType;
            this.Dic = dic;
            this.No = new comput();
        }
    }

}
