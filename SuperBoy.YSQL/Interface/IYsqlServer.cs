namespace SuperBoy.YSQL.Interface
{
    interface IYsqlServer
    {
        /// <summary>
        /// 系统加载方法，加载系统的时候检查所有配置文件
        /// </summary>
        /// <returns></returns>
        string LoadSysclick();

    }
}
