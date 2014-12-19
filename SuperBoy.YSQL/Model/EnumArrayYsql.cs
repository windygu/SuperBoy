namespace SuperBoy.YSQL.Model
{
    // ReSharper disable once InconsistentNaming
    public class EnumArrayYSQL
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
        public enum Jurisdiction
        {

            Read,
            Write,
            Delete,
            Create,
            User,
            Edit,
        }
        public enum WriteType
        {
            SystemInfo,
            SysDatabase
        }
        public enum ReadType
        {
            SystemInfo,
            SysDatabase
        }
    }
}