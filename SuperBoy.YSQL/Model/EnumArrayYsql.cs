namespace SuperBoy.YSQL.Model
{
    // ReSharper disable once InconsistentNaming
    public static class EnumArrayYSQL
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
            SysDatabase,
            SystemBak

        }
        public enum ReadType
        {
            SystemInfo,
            SysDatabase,
            SystemBak
        }
        //字段属性
        public enum FieldType
        {
            Int,
            Char,
            Text,
            Object,
            Datetime,
            Local,
            Trigger,
            String,
            Table,
            FileAddress
        }
        public enum Identity
        {
            StartIndex,
            EndIndex,
            Increment
        }

        public enum TableHead
        {
            DatabaseName,
            TableName,
            Namespace,
            Subjection,
            Modifier
        }
        public enum Modifier
        {
            Public,
            Private,
            Lock,
            Seal
        }
    }
}