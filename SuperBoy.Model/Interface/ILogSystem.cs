using SuperBoy.Model.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperBoy.Model.Interface
{
    interface ILogSystem
    {
        //the Common Log + data time
        bool CommonLog(string systemInformation);
        //the are tese log 
        bool CommonLogs(EnumArry.ConfigFormat format, string SystemInformation);
    }
}
