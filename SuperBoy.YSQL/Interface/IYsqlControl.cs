using System;
using System.Collections.Generic;
using System.Data;
using SuperBoy.YSQL.Model;

namespace SuperBoy.YSQL.Interface
{
    /// <summary>
    /// 默认命名空间为Ysql 默认表名为Table
    /// 加载数据库的同时提交到内存中，异步存储在本地
    /// </summary>
    public interface IYsqlControl
    {
        //是否有这个表
        bool IsTable();
        //是否有这个命名空间
        bool IsNamespace();
        //是否有这个Key
        bool IsKey(string keyName);
        //根据命名空间Key查询Value值
        string Select(string key, string table, string namespaceStr);
        /// <summary>
        /// 根据命名空间和表返回表
        /// </summary>
        /// <param name="table"></param>
        /// <param name="namespaceStr"></param>
        /// <returns></returns>
        DataTable Select(string table, string namespaceStr);
        /// <summary>
        /// 插入默认表名的Key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        string InsterInto(string key, String value);
        //修改系统信息并更新
        string UpdateMasterInfo(string key, string value);

        List<TableInfoModel> AnalysisTableInfo(string txt);

        string AnalysisTojson(List<TableInfoModel> list);

    }
}