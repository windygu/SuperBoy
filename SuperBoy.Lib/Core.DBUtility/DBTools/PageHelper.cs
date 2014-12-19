using System;
using System.Data;
using System.Data.Common;

namespace Core.DBUtility
{
    /// <summary> 
    /// 根据各种不同数据库生成不同分页语句的辅助类 PagerHelper
    /// </summary> 
    public class PagerHelper
    {
        #region 成员变量

        private string _tableName;//待查询表或自定义查询语句
        private string _fieldsToReturn = "*";//需要返回的列
        private string _fieldNameToSort = string.Empty;//排序字段名称
        private int _pageSize = 10;//页尺寸,就是一页显示多少条记录
        private int _pageIndex = 1;//当前的页码
        private bool _isDescending = false;//是否以降序排列
        private string _strwhere = string.Empty;//检索条件(注意: 不要加 where)

        #endregion

        #region 属性对象

        /// <summary>
        /// 待查询表或自定义查询语句
        /// </summary>
        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        /// <summary>
        /// 需要返回的列
        /// </summary>
        public string FieldsToReturn
        {
            get { return _fieldsToReturn; }
            set { _fieldsToReturn = value; }
        }

        /// <summary>
        /// 排序字段名称
        /// </summary>
        public string FieldNameToSort
        {
            get { return _fieldNameToSort; }
            set { _fieldNameToSort = value; }
        }

        /// <summary>
        /// 页尺寸,就是一页显示多少条记录
        /// </summary>
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        /// <summary>
        /// 当前的页码
        /// </summary>
        public int PageIndex
        {
            get { return _pageIndex; }
            set { _pageIndex = value; }
        }

        /// <summary>
        /// 是否以降序排列结果
        /// </summary>
        public bool IsDescending
        {
            get { return _isDescending; }
            set { _isDescending = value; }
        }

        /// <summary>
        /// 检索条件(注意: 不要加 where)
        /// </summary>
        public string StrWhere
        {
            get { return _strwhere; }
            set { _strwhere = value; }
        }

        /// <summary>
        /// 表或Sql语句包装属性
        /// </summary>
        internal string TableOrSqlWrapper
        {
            get
            {
                var isSql = _tableName.ToLower().Contains("from");
                if (isSql)
                {
                    return string.Format("({0})", _tableName);//如果是Sql语句，则加括号后再使用
                }
                else
                {
                    return _tableName;//如果是表名，则直接使用
                }
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认构造函数，其他通过属性设置
        /// </summary>
        public PagerHelper()
        {
        }

        /// <summary>
        /// 完整的构造函数,可以包含条件,返回记录字段等条件
        /// </summary>
        /// <param name="tableName">自定义查询语句</param>
        /// <param name="fieldsToReturn">需要返回的列</param>
        /// <param name="fieldNameToSort">排序字段名称</param>
        /// <param name="pageSize">页尺寸</param>
        /// <param name="pageIndex">当前的页码</param>
        /// <param name="isDescending">是否以降序排列</param>
        /// <param name="strwhere">检索条件</param>
        /// <param name="connectionString">连接字符串</param>
        public PagerHelper(string tableName, string fieldsToReturn, string fieldNameToSort,
            int pageSize, int pageIndex, bool isDescending, string strwhere)
        {
            this._tableName = tableName;
            this._fieldsToReturn = fieldsToReturn;
            this._fieldNameToSort = fieldNameToSort;
            this._pageSize = pageSize;
            this._pageIndex = pageIndex;
            this._isDescending = isDescending;
            this._strwhere = strwhere;
        }

        #endregion

        /// <summary>
        /// 不依赖于存储过程的分页(Oracle)
        /// </summary>
        /// <param name="isDoCount">如果isDoCount为True，返回总数统计Sql；否则返回分页语句Sql</param>
        /// <returns></returns>
        private string GetOracleSql(bool isDoCount)
        {
            var sql = "";
            if (string.IsNullOrEmpty(this._strwhere))
            {
                this._strwhere = " (1=1) ";
            }

            if (isDoCount)//执行总数统计
            {
                sql = string.Format("select count(*) as Total from {0} Where {1} ", this.TableOrSqlWrapper, this._strwhere);
            }
            else
            {
                var strOrder = string.Format(" order by {0} {1}", this._fieldNameToSort, this._isDescending ? "DESC" : "ASC");

                var minRow = _pageSize * (_pageIndex - 1);
                var maxRow = _pageSize * _pageIndex;
                var selectSql = string.Format("select {0} from {1} Where {2} {3}", _fieldsToReturn, this.TableOrSqlWrapper, this._strwhere, strOrder);
                sql = string.Format(@"select b.* from
                           (select a.*, rownum as rowIndex from({2}) a) b
                           where b.rowIndex > {0} and b.rowIndex <= {1}", minRow, maxRow, selectSql);
            }

            return sql;
        }

        /// <summary>
        /// 不依赖于存储过程的分页(SqlServer)
        /// </summary>
        /// <param name="isDoCount">如果isDoCount为True，返回总数统计Sql；否则返回分页语句Sql</param>
        /// <returns></returns>
        private string GetSqlServerSql(bool isDoCount)
        {
            var sql = "";
            if (string.IsNullOrEmpty(this._strwhere))
            {
                this._strwhere = " (1=1) ";
            }

            if (isDoCount)//执行总数统计
            {
                sql = string.Format("select count(*) as Total from {0} Where {1} ", this.TableOrSqlWrapper, this._strwhere);
            }
            else
            {
                var strTemp = string.Empty;
                var strOrder = string.Empty;
                if (this._isDescending)
                {
                    strTemp = "<(select min";
                    strOrder = string.Format(" order by [{0}] desc", this._fieldNameToSort);
                }
                else
                {
                    strTemp = ">(select max";
                    strOrder = string.Format(" order by [{0}] asc", this._fieldNameToSort);
                }

                sql = string.Format("select top {0} {1} from {2} ", this._pageSize, this._fieldsToReturn, this.TableOrSqlWrapper);

                //如果是第一页就执行以上代码，这样会加快执行速度
                if (this._pageIndex == 1)
                {
                    sql += string.Format(" Where {0} ", this._strwhere);
                    sql += strOrder;
                }
                else
                {
                    sql += string.Format(" Where [{0}] {1} ([{0}]) from (select top {2} [{0}] from {3} where {5} {4} ) as tblTmp) and {5} {4}",
                        this._fieldNameToSort, strTemp, (this._pageIndex - 1) * this._pageSize, this.TableOrSqlWrapper, strOrder, this._strwhere);
                }
            }

            return sql;
        }

        /// <summary>
        /// 不依赖于存储过程的分页(Access)
        /// </summary>
        /// <param name="isDoCount">如果isDoCount为True，返回总数统计Sql；否则返回分页语句Sql</param>
        /// <returns></returns>
        private string GetAccessSql(bool isDoCount)
        {
            return GetSqlServerSql(isDoCount);
        }

        /// <summary>
        /// 不依赖于存储过程的分页(MySql)
        /// </summary>
        /// <param name="isDoCount">如果isDoCount为True，返回总数统计Sql；否则返回分页语句Sql</param>
        /// <returns></returns>
        private string GetMySqlSql(bool isDoCount)
        {
            var sql = "";
            if (string.IsNullOrEmpty(this._strwhere))
            {
                this._strwhere = " (1=1) ";
            }

            if (isDoCount)//执行总数统计
            {
                sql = string.Format("select count(*) as Total from {0} Where {1} ", this.TableOrSqlWrapper, this._strwhere);
            }
            else
            {
                //SELECT * FROM 表名称 LIMIT M,N 
                var strOrder = string.Format(" order by {0} {1}", this._fieldNameToSort, this._isDescending ? "DESC" : "ASC");

                var minRow = _pageSize * (_pageIndex - 1);
                var maxRow = _pageSize * _pageIndex;
                sql = string.Format("select {0} from {1} Where {2} {3} LIMIT {4},{5}",
                    _fieldsToReturn, this.TableOrSqlWrapper, this._strwhere, strOrder, maxRow, minRow);
            }

            return sql;
        }

        /// <summary>
        /// 获取对应数据库的分页语句
        /// </summary>
        /// <param name="dbType">数据库类型枚举</param>
        /// <param name="isDoCount">如果isDoCount为True，返回总数统计Sql；否则返回分页语句Sql</param>
        public string GetPagingSql(DatabaseType dbType, bool isDoCount)
        {
            var sql = "";
            switch (dbType)
            {
                case DatabaseType.Access:
                    sql = GetAccessSql(isDoCount);
                    break;
                case DatabaseType.SqlServer:
                    sql = GetSqlServerSql(isDoCount);
                    break;
                case DatabaseType.Oracle:
                    sql = GetOracleSql(isDoCount);
                    break;
                case DatabaseType.MySql:
                    sql = GetMySqlSql(isDoCount);
                    break;
            }
            return sql;
        }
    }

    public enum DatabaseType { SqlServer, Oracle, Access, MySql }
} 
