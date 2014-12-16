using System.Collections.Generic;

namespace SuperBoy.YSQL.Model
{
    public class TriggerModel
    {
        public Dictionary<EnumArrayYsql.BehaviorTrigger,string> BehaviorTrigger { get; set; }
        public Dictionary<EnumArrayYsql.Coltrigger,string> Coltrigger { get; set; }

        public TriggerModel()
        {
        }
    }
}