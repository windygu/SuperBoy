using System.Data;

namespace SuperBoyView
{
    /// <summary>
    /// 
    /// </summary>
    class SQLServer : Idatabase
    {
        /// <summary>
        /// According count request database ALL value
        /// </summary>
        /// <param name="count">count</param>
        /// <returns></returns>
        public DataSet SelectALL(int count)
        {
            //The future "value" will be in a configuration file
            string sqlStr = "SELECT TOP " + count + @" [Id]
      ,[CommentTime]
      ,[ProductUserHeart]
      ,[UserID]
      ,[Label]
      ,[Score]
      ,[ReplyNum]
      ,[Praise]
      ,[Wname]
  FROM[CW100_develop].[dbo].[CW100_Comment]";
            return DBHelp.Query(sqlStr);
        }
        public DataSet SelectALL(string Where, int count)
        {
            //The future "value" will be in a configuration file
            string sqlStr = "SELECT TOP " + count + @" [Id]
      ,[CommentTime]
      ,[ProductUserHeart]
      ,[UserID]
      ,[Label]
      ,[Score]
      ,[ReplyNum]
      ,[Praise]
      ,[Wname]
  FROM[CW100_develop].[dbo].[CW100_Comment] " + Where;
            return DBHelp.Query(sqlStr);
        }
        public int Update(string Where)
        {
            string SqlServer = Where;
            int Count = DBHelp.CUD(SqlServer, null);
            return Count;
        }
    }
}
