using System.Collections.Generic;

namespace SuperBoy.Dynamic.Interface
{
    public interface ISerializationDynamic
    {
        string AnalyticalMain(string analyticalString);
        string AnalyticalJson(Dictionary<string, string> dictionary);
    }
}
