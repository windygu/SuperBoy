namespace SuperBoy.Database.Realize
{

    public class PubConstant
    {
        /// <summary>
        /// 获取连接字符串
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                // string _connectionString = ConfigurationManager.AppSettings["ConnectionString"];
                const string connectionString = "server=192.168.1.5;database=cw100_develop;uid=jyf;pwd=123;";
                //string ConStringEncrypt = ConfigurationManager.AppSettings["ConStringEncrypt"];
                //if (ConStringEncrypt == "true")
                //{
                //    _connectionString = DESEncrypt.Decrypt(_connectionString);
                //}
                return connectionString;
            }
        }

    }
}
