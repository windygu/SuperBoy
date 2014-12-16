namespace SuperBoy.YSQL.Model
{
    public class EnumArrayYsql
    {
        public enum Trigger
         {
            BehaviorTrigger,
            Coltrigger
         }
        public enum BehaviorTrigger
        {
            Inster,
            Update,
            Select,
            Delete,

        }
        public enum Coltrigger
        {
            Inster,
            Update,
            Select,
            Delete,

        }
    }
}