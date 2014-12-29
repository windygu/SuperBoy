using System;

namespace SuperBoy.YSQL.Model
{
    // ReSharper disable once InconsistentNaming
    public static class EnumArray
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
            DatabaseName,//数据库名
            TableName,//表名
            Namespace,//命名空间名
            server,//服务器地址（实例）
            Subjection,//奴属
            Modifier,
            DataType,//数据类型
            LocalAddress,//自己所在的地址
            HostAddress,//信息所在地址
        }
        public enum Modifier
        {
            Public,//公开的（所有人都能修改）
            Private,//私有的（创建者拥有所有权）
            Lock,//上锁的（只有上锁的人才能开启与更改，上锁者不是创建者）
            Seal,//密封的（谁都不能更改，只定义一次）
        }
        public static string fieldType(EnumArray.FieldType type)
        {
            switch (type)
            {
                case FieldType.Int:
                    return "Int";
                case FieldType.Char:
                    return "Char";
                case FieldType.Text:
                    return "Text";
                case FieldType.Object:
                    return "Object";
                case FieldType.Datetime:
                    return "Datetime";
                case FieldType.Local:
                    return "Local";
                case FieldType.Trigger:
                    return "Trigger";
                case FieldType.String:
                    return "String";
                case FieldType.Table:
                    return "Table";
                case FieldType.FileAddress:
                    return "FileAddress";
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

    }
}