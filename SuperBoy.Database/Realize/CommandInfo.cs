using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace SuperBoy.Database.Realize
{
    public enum EffentNextType
    {
        /// <summary>
        ///     对其他语句无任何影响
        /// </summary>
        None,

        /// <summary>
        ///     当前语句必须为"select count(1) from .."格式，如果存在则继续执行，不存在回滚事务
        /// </summary>
        WhenHaveContine,

        /// <summary>
        ///     当前语句必须为"select count(1) from .."格式，如果不存在则继续执行，存在回滚事务
        /// </summary>
        WhenNoHaveContine,

        /// <summary>
        ///     当前语句影响到的行数必须大于0，否则回滚事务
        /// </summary>
        ExcuteEffectRows,

        /// <summary>
        ///     引发事件-当前语句必须为"select count(1) from .."格式，如果不存在则继续执行，存在回滚事务
        /// </summary>
        SolicitationEvent
    }

    public class CommandInfo
    {
        public string CommandText;
        public EffentNextType EffentNextType = EffentNextType.None;
        public object OriginalData = null;
        public DbParameter[] Parameters;
        public object ShareObject = null;

        public CommandInfo()
        {
        }

        public CommandInfo(string sqlText, SqlParameter[] para)
        {
            CommandText = sqlText;
            Parameters = para;
        }

        public CommandInfo(string sqlText, DbParameter[] para, EffentNextType type)
        {
            CommandText = sqlText;
            Parameters = para;
            EffentNextType = type;
        }

        private event EventHandler _solicitationEvent;

        public event EventHandler SolicitationEvent
        {
            add { _solicitationEvent += value; }
            remove { _solicitationEvent -= value; }
        }

        public void OnSolicitationEvent()
        {
            if (_solicitationEvent != null)
            {
                _solicitationEvent(this, new EventArgs());
            }
        }
    }
}