using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace SuperBoyView
{
    /// <summary>
    /// 程序集数据库帮助类
    /// </summary>
    class DBHelp
    {
        //链接配置文件
        //private static string connectionString = ConfigurationManager.ConnectionStrings[""].ConnectionString;

        #region 返回一个表格
        public static DataSet Query(string SQLString, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection("server=.;database=CW100_develop;uid=sa;pwd=123;"))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
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
                foreach (SqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }
        #endregion

        #region 返回受影响的行数

        public static int CUD(string sql, List<SqlParameter> list)
        {
            SqlConnection con = new SqlConnection("");
            SqlCommand com = new SqlCommand(sql, con);
            if (list != null && list.Count > 0)
            {
                com.Parameters.Add(list.ToArray());
            }
            con.Open();
            return com.ExecuteNonQuery();
        }
        #endregion


    }
}
