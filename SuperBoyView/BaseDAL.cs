using System.Data;

namespace SuperBoyView
{
    /// <summary>
    /// 
    /// </summary>
    class BaseDAL
    {


        /// <summary>
        /// band 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int Mains(string sql)
        {

            return 0;
        }
        /// <summary>
        /// load
        /// </summary>
        /// <returns></returns>
        public static DataTable Loads()
        {
            // return DBHelp.Query("select  top 30 * from China_News").Tables[0];
            return null;
        }
    }
}
