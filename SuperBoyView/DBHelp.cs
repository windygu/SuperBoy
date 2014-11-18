using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace SuperBoyView
{
    /// <summary>
    /// the project database helper class
    /// </summary>
    class DBHelp
    {
        //connection config
        private static string connectionString = "server=182.92.228.4;database=CW100_develop;uid=cw100New;pwd=txdkj@@abc128(##)(##.)ABa";

        #region return a table
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SQLString"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static DataSet Query(string SQLString, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
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

        #region return the affected rows

        public static int CUD(string sql, List<SqlParameter> list)
        {
            SqlConnection con = new SqlConnection(connectionString);
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
