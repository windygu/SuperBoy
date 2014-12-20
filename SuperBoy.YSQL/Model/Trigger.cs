using System.Collections.Generic;

namespace SuperBoy.YSQL.Model
{
    // ReSharper disable once InconsistentNaming
    public class Trigger
    {
        public Dictionary<EnumArray.BehaviorTrigger,string> BehaviorTrigger { get; set; }
        public Dictionary<EnumArray.Coltrigger,string> Coltrigger { get; set; }

    }
}