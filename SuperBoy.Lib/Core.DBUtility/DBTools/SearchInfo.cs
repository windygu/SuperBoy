﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DBUtility
{
    /// <summary>
    /// 查询信息实体类
    /// </summary>
    public class SearchInfo
    {
        public SearchInfo() {}

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="sqlOperator">字段的Sql操作符号</param>
        public SearchInfo(string fieldName, object fieldValue, SqlOperator sqlOperator)
            : this(fieldName, fieldValue, sqlOperator, true)
        { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="sqlOperator">字段的Sql操作符号</param>
        /// <param name="excludeIfEmpty">如果字段为空或者Null则不作为查询条件</param>
        public SearchInfo(string fieldName, object fieldValue, SqlOperator sqlOperator, bool excludeIfEmpty)
            : this(fieldName, fieldValue, sqlOperator, excludeIfEmpty, null)
        { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldValue">字段的值</param>
        /// <param name="sqlOperator">字段的Sql操作符号</param>
        /// <param name="excludeIfEmpty">如果字段为空或者Null则不作为查询条件</param>
        /// <param name="groupName">分组的名称，如需构造一个括号内的条件 ( Test = "AA1" OR Test = "AA2"), 定义一个组名集中条件</param>
        public SearchInfo(string fieldName, object fieldValue, SqlOperator sqlOperator, bool excludeIfEmpty, string groupName) 
        {
            this._fieldName = fieldName;
            this._fieldValue = fieldValue;
            this._sqlOperator = sqlOperator; 
            this._excludeIfEmpty = excludeIfEmpty;
            this._groupName = groupName;
        }

        #region 字段属性
        private string _fieldName;
        private object _fieldValue;
        private SqlOperator _sqlOperator;
        private bool _excludeIfEmpty = true;
        private string _groupName;

        /// <summary>
        /// 分组的名称，如需构造一个括号内的条件 ( Test = "AA1" OR Test = "AA2"), 定义一个组名集中条件
        /// </summary>
        public string GroupName
        {
            get { return _groupName; }
            set { _groupName = value; }
        }


        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName
        {
            get { return _fieldName; }
            set { _fieldName = value; }
        }

        /// <summary>
        /// 字段的值
        /// </summary>
        public object FieldValue
        {
            get { return _fieldValue; }
            set { _fieldValue = value; }
        }

        /// <summary>
        /// 字段的Sql操作符号
        /// </summary>
        public SqlOperator SqlOperator
        {
            get { return _sqlOperator; }
            set { _sqlOperator = value; }
        }

        /// <summary>
        /// 如果字段为空或者Null则不作为查询条件
        /// </summary>
        public bool ExcludeIfEmpty
        {
            get { return _excludeIfEmpty; }
            set { _excludeIfEmpty = value; }
        } 

        #endregion
    }

    /// <summary>
    /// Sql的查询符号
    /// </summary>
    public enum SqlOperator
    {
        /// <summary>
        /// Like 模糊查询
        /// </summary>
        Like,

        /// <summary>
        /// Not LiKE 模糊查询
        /// </summary>
        NotLike,

        /// <summary>
        /// Like 开始匹配模糊查询，如Like 'ABC%'
        /// </summary>
        LikeStartAt,

        /// <summary>
        /// ＝ is equal to 等于号 
        /// </summary>
        Equal,

        /// <summary>
        /// <> (≠) is not equal to 不等于号
        /// </summary>
        NotEqual,

        /// <summary>
        /// ＞ is more than 大于号
        /// </summary>
        MoreThan,

        /// <summary>
        /// ＜ is less than 小于号 
        /// </summary>
        LessThan,

        /// <summary>
        /// ≥ is more than or equal to 大于或等于号 
        /// </summary>
        MoreThanOrEqual,

        /// <summary>
        /// ≤ is less than or equal to 小于或等于号
        /// </summary>
        LessThanOrEqual,

        /*       
        /// <summary>
        /// 在某个值的中间，拆成两个符号 >= 和 <=
        /// </summary>
        Between,
        */

        /// <summary>
        /// 在某个字符串值中
        /// </summary>
        In
    }
}
