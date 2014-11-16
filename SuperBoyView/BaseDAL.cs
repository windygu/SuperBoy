using System.Data;

namespace SuperBoyView
{
    /// <summary>
    /// 
    /// </summary>
    class BaseDAL
    {


        /// <summary>
        /// 主动绑定方法
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int Mains(string sql)
        {

            return 0;
        }
        /// <summary>
        /// 加载方法
        /// </summary>
        /// <returns></returns>
        public static DataTable Loads()
        {
            return DBHelp.Query("select  top 30 * from China_News").Tables[0];
        }
    }
}
