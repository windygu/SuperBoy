using System.Collections.Generic;

namespace SuperBoy.Dynamic.Interface
{
    interface IAnalyticalDynamic
    {

        string OnLine(Dictionary<string, string> dic);
        string Select(Dictionary<string, string> dic);
        string Inster(Dictionary<string, string> dic);
        string Delete(Dictionary<string, string> dic);
        string Update(Dictionary<string, string> dic);
        string Manage(Dictionary<string, string> dic);
    }
}
