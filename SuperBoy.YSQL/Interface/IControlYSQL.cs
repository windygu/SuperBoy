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
    /// ReSharper disable once InconsistentNaming
    public interface IControlYSQL
    {
        //修改系统信息并更新
        string UpdateMasterInfo(string key, string value);
        //系统信息
        List<TableMaster> AnalysisTableInfo(string txt);
        //解析json方法
        string AnalysisTojson(IEnumerable<TableMaster> list);

    }
}