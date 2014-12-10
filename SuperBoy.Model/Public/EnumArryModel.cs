
namespace SuperBoy.Model.Public
{
    /// <summary>
    /// enumeration class
    /// </summary>
    public class EnumArryModel
    {
        #region the programs configuration (system)

        public enum Master
        {
            /// <summary>
            /// programs master path(string)
            /// </summary>
            MasterPath,
            /// <summary>
            /// programs config file format(json or HTML or Array)
            /// </summary>
            TextFormat,
            /// <summary>
            /// the programs access level(int)
            /// </summary>
            Jurisdiction,
            /// <summary>
            /// default（Administrator）
            /// </summary>
            DefaultAdministrator,
            /// <summary>
            /// Is key protect（false）
            /// </summary>
            ISkeyProtection,
            /// <summary>
            /// is open config protect（false）
            /// </summary>
            ConfigurationProtection,
            /// <summary>
            /// administrator Name（Array）
            /// </summary>
            AdministratorName,
            /// <summary>
            /// Key protect List（Array）
            /// </summary>
            keyProtectionS,
            /// <summary>
            /// config file List
            /// </summary>
            configAtrry,
            /// <summary>
            /// start windows
            /// </summary>
            StartForm,
            /// <summary>
            /// head file mark
            /// </summary>
            HeadText,
            /// <summary>
            /// default file count
            /// </summary>
            DataDefaultCount,
            /// <summary>
            /// Is Auto
            /// </summary>
            IsAuto,
            /// <summary>
            /// DataTimeFormat
            /// </summary>
            DataTimeFormat,
        }


        /// <summary>
        /// Super Boy
        /// </summary>

        public enum LogType
        {
            /// <summary>
            /// common text
            /// </summary>
            Plain,
            /// <summary>
            /// log text
            /// </summary>
            Log,
            /// <summary>
            /// system text
            /// </summary>
            System,
            /// <summary>
            /// system master text
            /// </summary>
            Master,
            /// <summary>
            /// backups test
            /// </summary>
            bak,
            /// <summary>
            /// temp test
            /// </summary>
            odb,
        }
        #endregion
        public enum SendType
        {
            SELECT,
            UPDATE,
            DELETE,
            INSTER,
            ONLINE,
            MANAGE,
        }
        public enum ReturnType
        {
            JSON,
            XML,
            KeyValue,
            LIST,
            DICT
        }
        public enum Database
        {
            DatabaseName,
            TOP,
            WHERE,
            WHERETYPE,
            DATA,
            Field,
            Value,
            Key,
            UpdateKey,
            UpdateValue,
            TableName,
        }
        public enum WhereType
        {
            /// <summary>
            ///等于参数
            /// </summary>
            Equal,
            /// <summary>
            ///大于参数
            /// </summary>
            Greater,
            /// <summary>
            /// 小于参数
            /// </summary>
            Less,
            /// <summary>
            /// 类似参数
            /// </summary>
            Like,
            /// <summary>
            /// 左类似
            /// </summary>
            LiftLike,
            /// <summary>
            /// 右类似
            /// </summary>
            RightLike
        }


    }
}
