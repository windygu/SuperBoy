
using SuperBoy.YSQL.Interface;
using SuperBoy.YSQL.Model;
using SuperBoy.YSQL.Realize;
using Newtonsoft.Json;


namespace SuperBoy.YSQL.Control
{

    /// <summary>
    /// 服务类库
    /// </summary>
    /// ReSharper disable once InconsistentNaming
    public static class ServiceYSQL
    {
        private static SystemInfoYSQL SysInfo { get; set; }
        static ServiceYSQL()
        {
            //初始化读取系统信息
            var systemJson = ReadAndWrite.ReadSys(EnumArrayYSQL.ReadType.SystemInfo);
            var sysin = JsonConvert.DeserializeObject<SystemInfoYSQL>(systemJson);
        }

        //调用数据库总空间
        //初始化调用参数
        private static readonly IReadAndWriteYSQL ReadAndWrite = new ReadAndWrite();

        // ReSharper disable once UnusedAutoPropertyAccessor.Local

        public static void AutoMainAllDatabase()
        {
            //调用其他的所有数据库
            //SysInfo.AllDatabasePath
            foreach (var dict in SysInfo.AllDatabasePath)
            {

            }
        }
    }
}