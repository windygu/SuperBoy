namespace SuperBoy.YSQL.Interface
{
    // ReSharper disable once InconsistentNaming
    interface IServerYSQL
    {
        /// <summary>
        /// 系统加载方法，加载系统的时候检查所有配置文件
        /// </summary>
        /// <returns></returns>
        string LoadSysclick();

    }
}
