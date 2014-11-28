
namespace SuperBoy.Cloud
{
    /// <summary>
    /// enumeration class
    /// </summary>
    public class EnumArry
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
            IsAuto
        }

        /// <summary>
        /// config file format
        /// </summary>
        public enum ConfigFormat
        {
            /// <summary>
            /// common data List（Key:Value)
            /// </summary>
            Array,
            //json or HTML or Array
            /// <summary>
            /// json format for data([{key:Value},{key,Value}])
            /// </summary>
            Json,
            /// <summary>
            /// XML format of data storage(<Key>Value<Key>）
            /// </summary>
            XML

        }
        /// <summary>
        /// administrator Name（custom）
        /// </summary>
        public enum DefaultAdminiName
        {
            Admini,
            System,
            Users
        }

        /// <summary>
        /// configuration file List（System）
        /// </summary>
        public enum configAtrry
        {
            /// <summary>
            /// start configuration (don't allow the user to change the read only once)
            /// </summary>
            StartConfig,
            /// <summary>
            /// start config address(don't allow the user to change the read only once)
            /// </summary>
            StartConfigFile,

            /// <summary>
            /// Skin configuration（read once , can be change)
            /// </summary>
            SkinConfig,
            /// <summary>
            /// Skin ConfigFile（read once , can be change）
            /// </summary>
            SkinConfigFile,

            /// <summary>
            /// read config（repeatedly）
            /// </summary>
            FuncConfig,
            /// <summary>
            /// read config address（repeatedly）
            /// </summary>
            FuncConfigFile,

            /// <summary>
            /// closed config（current used read）
            /// </summary>
            CloseConfig,
            /// <summary>
            /// closed config address（current used read）
            /// </summary>
            CloseConfigFile,

            /// <summary>
            /// Temporary config file（Use up to delete）
            /// </summary>
            TemporaryConfig,
            /// <summary>
            /// Temporary config file address（Use up to delete）
            /// </summary>
            TemporaryConfigFile
        }
        /// <summary>
        /// Super Boy
        /// </summary>
        public enum HeadType
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

        #region language
        /// <summary>
        /// language
        /// </summary>
        public enum Language
        {
            /// <summary>
            /// 确定
            /// </summary>
            Ok,
            /// <summary>
            /// 不能
            /// </summary>
            No,
            /// <summary>
            /// 错误
            /// </summary>
            Error,
            /// <summary>
            /// 页面错误
            /// </summary>
            PageError,
            /// <summary>
            /// 添加成功
            /// </summary>
            AddSuccess,
            /// <summary>
            /// 添加错误
            /// </summary>
            AddError,
            /// <summary>
            /// 添加失败
            /// </summary>
            AddFailure,
            /// <summary>
            /// 错误操作
            /// </summary>
            ErrorFun,
            /// <summary>
            /// 欢迎来到
            /// </summary>
            Welcome,
            /// <summary>
            /// 设置
            /// </summary>
            Setup
        }
        #endregion

        #region 该软件所属配置（自定义|系统默认）
        /// <summary>
        /// 配置文件（自定义|系统默认）
        /// </summary>
        public enum Config
        {
            /// <summary>
            /// 是否开启语音
            /// </summary>
            IsLanguage,
            /// <summary>
            /// 颜色配置
            /// </summary>
            colorsArray,
            /// <summary>
            /// 标题栏
            /// </summary>
            ItemTitle,
            /// <summary>
            /// 亮度
            /// </summary>
            OpacityItem,
            /// <summary>
            /// 文件夹地址
            /// </summary>
            ConfigsAddress
        }
        #endregion

        #region 数据库操作（自定义）
        /// <summary>
        /// 数据库查询（自定义|系统默认）
        /// </summary>
        enum sql
        {
            /// <summary>
            /// 查询某些条数
            /// </summary>
            SELECT,
            /// <summary>
            /// 查询所有
            /// </summary>
            SELECTALL,
            /// <summary>
            /// 修改某个字段
            /// </summary>
            UPDATE,
            /// <summary>
            /// 修改整条
            /// </summary>
            UPDATES,
            /// <summary>
            /// 修改选择条数
            /// </summary>
            UPDATESELECT,
            /// <summary>
            /// 插入一条
            /// </summary>
            INSERT,
            /// <summary>
            /// 插入多条（插入一个序列）
            /// </summary>
            INSERTS,
        }
        #endregion

        #region 结构化查询语句枚举

        #region Y-SQL

        public enum T_sql
        {
            select,//查询
            insert,//插入
            into,//插入现在
            update,//修改
            set,//传
            values,//值的集合
            inner,//链接
            join,//连接
            create,//创建
            where,//条件
        }

        #endregion

        public enum _ysxl
        {
            conmmit,//提交
            want,//弱转换,链接
            to,//去，插入
            Query,//查询
            Key,//键
            Value,//值
            Delete,//删除
            Modify,//修改
            each,//修改
            control,//控制
            appoint,//指定
            table,//表
            tables,//表集合
            DateSet,//数据集合
        }
        #endregion


        #region Ysxl解析操作符
        public enum ysxlOperator
        {
            Exclude,//排除符

        }

        public enum ysxlAttribute
        {

        }
        #endregion
    }
}
