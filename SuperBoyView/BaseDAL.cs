using System.Data;

namespace SuperBoyView
{
    /// <summary>
    /// 
    /// </summary>
    class BaseDAL : Idatabase
    {
        /// <summary>
        /// According count request database ALL value
        /// </summary>
        /// <param name="count">count</param>
        /// <returns></returns>
        public DataSet SelectALL(int count)
        {
            //The future "value" will be in a configuration file
            string sqlStr = "SELECT TOP " + count + " * FROM CW100_COMMENT";
            return DBHelp.Query(null, sqlStr);
        }
        public DataSet SelectALL(string Where, int count)
        {
            //The future "value" will be in a configuration file
            string sqlStr = "SELECT TOP " + count + " * FROM CW100_COMMENT";
            return DBHelp.Query(null, sqlStr);
        }
        public int Update(string Where)
        {

            return 0;
        }
    }
}
