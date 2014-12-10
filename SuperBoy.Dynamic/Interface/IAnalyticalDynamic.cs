using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperBoy.Dynamic.Interface
{
    interface IAnalyticalDynamic
    {

        string OnLine(Dictionary<string, string> Dic);
        string Select(Dictionary<string, string> Dic);
        string Inster(Dictionary<string, string> Dic);
        string Delete(Dictionary<string, string> Dic);
        string Update(Dictionary<string, string> Dic);
        string MANAGE(Dictionary<string, string> Dic);
    }
}
