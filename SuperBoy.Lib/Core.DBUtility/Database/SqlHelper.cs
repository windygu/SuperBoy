/**********************************************
 * 类作用：   MSSQL访问基础类
 * 建立人：   abaal
 * 建立时间： 2008-09-03 
 * Copyright (C) 2007-2008 abaal
 * All rights reserved
 * http://blog.csdn.net/abaal888
 ***********************************************/

using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;


/// <summary>
/// 数据访问基础类(基于SQLServer)
/// </summary>
public abstract class SqlHelper
{
    //数据库连接字符串(web.config来配置)
    //<add key="ConnectionString" value="Data Source=CHF;Initial Catalog=JGXT;User ID=sa" />  
    //public static readonly string connectionString = ConfigurationSettings.AppSettings["JGXTConnectionString"];
    public static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["strProvider"].ConnectionString;
    public SqlHelper()
    {

    }

    /// <summary>
    /// 获取表某个字段的最大值(增加于2006年9月13日)
    /// </summary>
    /// <param name="fieldName"></param>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public static int GetMaxId(string fieldName, string tableName)
    {
        //string strSql = "select count(" + FieldName + ") from " + TableName;
        var strSql = "select max(" + fieldName + ") from " + tableName;
        using (var connection = new SqlConnection(ConnectionString))
        {
            using (var cmd = new SqlCommand(strSql, connection))
            {
                try
                {
                    connection.Open();
                    var obj = cmd.ExecuteScalar();
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {

                        return 1;

                    }

                    else
                    {

                        return Convert.ToInt32(obj.ToString()) + 1;

                    }
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    connection.Close();
                    throw new Exception(e.Message);
                }
            }
        }
    }


    #region 执行带参数的SQL语句

    /// <summary>
    /// 执行SQL语句，返回是否存在该记录
    /// </summary>
    /// <param name="sqlString">SQL语句</param>
    /// <returns>是否存在</returns>
    public static bool Exists(string sqlString, params SqlParameter[] cmdParms)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            using (var cmd = new SqlCommand())
            {
                try
                {
                    PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                    var obj = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return false;
                    }
                    else
                    {
                        var i = int.Parse(obj.ToString());
                        if (i > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw new Exception(e.Message);
                }
            }
        }
    }

    /// <summary>
    /// 执行SQL语句，返回影响的记录数
    /// </summary>
    /// <param name="sqlString">SQL语句</param>
    /// <returns>影响的记录数</returns>
    public static int ExecuteSql(string sqlString, params SqlParameter[] cmdParms)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            using (var cmd = new SqlCommand())
            {
                try
                {
                    PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                    var rows = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return rows;
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw new Exception(e.Message);
                }
            }
        }
    }


    /// <summary>
    /// 执行多条SQL语句，实现数据库事务。
    /// </summary>
    /// <param name="sqlStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
    public static void ExecuteSqlTran(Hashtable sqlStringList)
    {
        using (var conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            using (var trans = conn.BeginTransaction())
            {
                var cmd = new SqlCommand();
                try
                {
                    //循环
                    foreach (DictionaryEntry myDe in sqlStringList)
                    {
                        var cmdText = myDe.Key.ToString();
                        var cmdParms = (SqlParameter[])myDe.Value;
                        PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                        var val = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        trans.Commit();
                    }
                }
                catch
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
    }


    /// <summary>
    /// 执行一条计算查询结果语句，返回查询结果（object）。
    /// </summary>
    /// <param name="sqlString">计算查询结果语句</param>
    /// <returns>查询结果（object）</returns>
    public static object GetSingle(string sqlString, params SqlParameter[] cmdParms)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            using (var cmd = new SqlCommand())
            {
                try
                {
                    PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                    var obj = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw new Exception(e.Message);
                }
            }
        }
    }

    /// <summary>
    /// 执行查询语句，返回SqlDataReader
    /// </summary>
    /// <param name="strSQL">查询语句</param>
    /// <returns>SqlDataReader</returns>
    public static SqlDataReader ExecuteReader(string sqlString, params SqlParameter[] cmdParms)
    {
        var connection = new SqlConnection(ConnectionString);
        var cmd = new SqlCommand();
        try
        {
            PrepareCommand(cmd, connection, null, sqlString, cmdParms);
            var myReader = cmd.ExecuteReader();
            cmd.Parameters.Clear();
            return myReader;
        }
        catch (System.Data.SqlClient.SqlException e)
        {
            throw new Exception(e.Message);
        }
    }

    /// <summary>
    /// 执行查询语句，返回DataSet
    /// </summary>
    /// <param name="sqlString">查询语句</param>
    /// <returns>DataSet</returns>
    public static DataSet Query(string sqlString, params SqlParameter[] cmdParms)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand();
            PrepareCommand(cmd, connection, null, sqlString, cmdParms);
            using (var da = new SqlDataAdapter(cmd))
            {
                var ds = new DataSet();
                try
                {
                    da.Fill(ds, "ds");
                    cmd.Parameters.Clear();
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }
    }


    private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
    {
        if (conn.State != ConnectionState.Open)
            conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = cmdText;
        if (trans != null)
            cmd.Transaction = trans;
        cmd.CommandType = CommandType.Text;//cmdType;
        if (cmdParms != null)
        {
            foreach (var parm in cmdParms)
                cmd.Parameters.Add(parm);
        }
    }

    #endregion


    #region  执行简单SQL语句

    /// <summary>
    /// 执行SQL语句，返回影响的记录数
    /// </summary>
    /// <param name="sqlString">SQL语句</param>
    /// <returns>影响的记录数</returns>
    public static int ExecuteSql(string sqlString)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            using (var cmd = new SqlCommand(sqlString, connection))
            {
                try
                {
                    connection.Open();
                    var rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    connection.Close();
                    throw new Exception(e.Message);
                }
            }
        }
    }

    /// <summary>
    /// 执行多条SQL语句，实现数据库事务。
    /// </summary>
    /// <param name="sqlStringList">多条SQL语句</param>  
    public static void ExecuteSqlTran(ArrayList sqlStringList)
    {
        using (var conn = new SqlConnection(ConnectionString))
        {
            conn.Open();
            var cmd = new SqlCommand();
            cmd.Connection = conn;
            var tx = conn.BeginTransaction();
            cmd.Transaction = tx;
            try
            {
                for (var n = 0; n < sqlStringList.Count; n++)
                {
                    var strsql = sqlStringList[n].ToString();
                    if (strsql.Trim().Length > 1)
                    {
                        cmd.CommandText = strsql;
                        cmd.ExecuteNonQuery();
                    }
                }
                tx.Commit();
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                tx.Rollback();
                throw new Exception(e.Message);
            }
        }
    }
    /// <summary>
    /// 执行带一个存储过程参数的的SQL语句。
    /// </summary>
    /// <param name="sqlString">SQL语句</param>
    /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
    /// <returns>影响的记录数</returns>
    public static int ExecuteSql(string sqlString, string content)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand(sqlString, connection);
            var myParameter = new System.Data.SqlClient.SqlParameter("@content", SqlDbType.NText);
            myParameter.Value = content;
            cmd.Parameters.Add(myParameter);
            try
            {
                connection.Open();
                var rows = cmd.ExecuteNonQuery();
                return rows;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                cmd.Dispose();
                connection.Close();
            }
        }
    }
    /// <summary>
    /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
    /// </summary>
    /// <param name="strSql">SQL语句</param>
    /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
    /// <returns>影响的记录数</returns>
    public static int ExecuteSqlInsertImg(string strSql, byte[] fs)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand(strSql, connection);
            var myParameter = new System.Data.SqlClient.SqlParameter("@fs", SqlDbType.Image);
            myParameter.Value = fs;
            cmd.Parameters.Add(myParameter);
            try
            {
                connection.Open();
                var rows = cmd.ExecuteNonQuery();
                return rows;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                cmd.Dispose();
                connection.Close();
            }
        }
    }

    /// <summary>
    /// 执行一条计算查询结果语句，返回查询结果（object）。
    /// </summary>
    /// <param name="sqlString">计算查询结果语句</param>
    /// <returns>查询结果（object）</returns>
    public static object GetSingle(string sqlString)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            using (var cmd = new SqlCommand(sqlString, connection))
            {
                try
                {
                    connection.Open();
                    var obj = cmd.ExecuteScalar();
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    connection.Close();
                    throw new Exception(e.Message);
                }
            }
        }
    }
    /// <summary>
    /// 执行查询语句，返回SqlDataReader
    /// </summary>
    /// <param name="strSql">查询语句</param>
    /// <returns>SqlDataReader</returns>
    public static SqlDataReader ExecuteReader(string strSql)
    {
        var connection = new SqlConnection(ConnectionString);
        var cmd = new SqlCommand(strSql, connection);
        try
        {
            connection.Open();
            var myReader = cmd.ExecuteReader();
            return myReader;
        }
        catch (System.Data.SqlClient.SqlException e)
        {
            throw new Exception(e.Message);
        }

    }
    /// <summary>
    /// 执行查询语句，返回DataSet
    /// </summary>
    /// <param name="sqlString">查询语句</param>
    /// <returns>DataSet</returns>
    public static DataSet Query(string sqlString)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var ds = new DataSet();
            try
            {
                connection.Open();
                var command = new SqlDataAdapter(sqlString, connection);
                command.Fill(ds, "ds");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return ds;
        }
    }


    #endregion

    #region 存储过程操作

    /// <summary>
    /// 执行存储过程
    /// </summary>
    /// <param name="storedProcName">存储过程名</param>
    /// <param name="parameters">存储过程参数</param>
    /// <returns>SqlDataReader</returns>
    public static SqlDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
    {
        var connection = new SqlConnection(ConnectionString);
        SqlDataReader returnReader;
        connection.Open();
        var command = BuildQueryCommand(connection, storedProcName, parameters);
        command.CommandType = CommandType.StoredProcedure;
        returnReader = command.ExecuteReader();
        return returnReader;
    }


    /// <summary>
    /// 执行存储过程
    /// </summary>
    /// <param name="storedProcName">存储过程名</param>
    /// <param name="parameters">存储过程参数</param>
    /// <param name="tableName">DataSet结果中的表名</param>
    /// <returns>DataSet</returns>
    public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            var dataSet = new DataSet();
            connection.Open();
            var sqlDa = new SqlDataAdapter();
            sqlDa.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
            sqlDa.Fill(dataSet, tableName);
            connection.Close();
            return dataSet;
        }
    }


    /// <summary>
    /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
    /// </summary>
    /// <param name="connection">数据库连接</param>
    /// <param name="storedProcName">存储过程名</param>
    /// <param name="parameters">存储过程参数</param>
    /// <returns>SqlCommand</returns>
    private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
    {
        var command = new SqlCommand(storedProcName, connection);
        command.CommandType = CommandType.StoredProcedure;
        foreach (SqlParameter parameter in parameters)
        {
            command.Parameters.Add(parameter);
        }
        return command;
    }

    /// <summary>
    /// 执行存储过程，返回影响的行数  
    /// </summary>
    /// <param name="storedProcName">存储过程名</param>
    /// <param name="parameters">存储过程参数</param>
    /// <param name="rowsAffected">影响的行数</param>
    /// <returns></returns>
    public static int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
    {
        using (var connection = new SqlConnection(ConnectionString))
        {
            int result;
            connection.Open();
            var command = BuildIntCommand(connection, storedProcName, parameters);
            rowsAffected = command.ExecuteNonQuery();
            result = (int)command.Parameters["ReturnValue"].Value;
            //Connection.Close();
            return result;
        }
    }

    /// <summary>
    /// 创建 SqlCommand 对象实例(用来返回一个整数值) 
    /// </summary>
    /// <param name="storedProcName">存储过程名</param>
    /// <param name="parameters">存储过程参数</param>
    /// <returns>SqlCommand 对象实例</returns>
    private static SqlCommand BuildIntCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
    {
        var command = BuildQueryCommand(connection, storedProcName, parameters);
        command.Parameters.Add(new SqlParameter("ReturnValue",
            SqlDbType.Int, 4, ParameterDirection.ReturnValue,
            false, 0, 0, string.Empty, DataRowVersion.Default, null));
        return command;
    }
    #endregion

}


