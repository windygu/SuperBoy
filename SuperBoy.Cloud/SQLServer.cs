using System.Data;

namespace SuperBoy.Cloud
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
            string sqlStr = "SELECT TOP " + count + @" c.Id,p.[productName],c.[ProductID],c.[CommentTime],c.[CommentTitle],c.[ProductUserHeart],c.[Label],c.[Score] ,c.[ReplyNum],c.[Praise],c.[Wname]
FROM [CW100_develop].[dbo].[CW100_Comment] c inner JOIN [dbo].[CW100_Product] p on p.pID = c.productid";
            return DBHelp.Query(sqlStr);
        }
        public DataSet SelectALL(string Where, int count)
        {
            //The future "value" will be in a configuration file
            string sqlStr = "SELECT TOP " + count + @" c.Id,p.[productName],c.[ProductID],c.[CommentTime],c.[CommentTitle],c.[ProductUserHeart],c.[Label],c.[Score] ,c.[ReplyNum],c.[Praise],c.[Wname]
FROM [CW100_develop].[dbo].[CW100_Comment] c inner JOIN [dbo].[CW100_Product] p on p.pID = c.productid " + Where;
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
