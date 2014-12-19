using System.Collections.Generic;

namespace SuperBoy.YSQL.Model
{
    // ReSharper disable once InconsistentNaming
    public class TriggerModelYSQL
    {
        public Dictionary<EnumArrayYSQL.BehaviorTrigger,string> BehaviorTrigger { get; set; }
        public Dictionary<EnumArrayYSQL.Coltrigger,string> Coltrigger { get; set; }

    }
}