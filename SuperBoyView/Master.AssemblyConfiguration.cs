using System.Collections.Generic;

namespace SuperBoy
{
    class AssemblyConfiguration : EnumArry
    {
        /// <summary>
        ///创建一个全局系统配置文件
        /// </summary>
        public static Dictionary<Master, object> MasterDiction = new Dictionary<Master, object>();

        static AssemblyConfiguration()
        {
            MasterDiction.Add(Master.ConfigurationProtection, false);       //默认关闭配置文件保护
            MasterDiction.Add(Master.DefaultAdministrator, "Administrator");//默认管理员账户为Administrator
            MasterDiction.Add(Master.ISkeyProtection, false);               //默认关闭密钥保护
            MasterDiction.Add(Master.Jurisdiction, 3);                      //默认管理员等级
            MasterDiction.Add(Master.MasterPath, "\\Master\\");             //默认文件夹名字为Master
            MasterDiction.Add(Master.TextFormat, ConfigFormat.Array);       //键值对集合
            DefaultAdminiName[] Administrators = { DefaultAdminiName.Admini, DefaultAdminiName.System, DefaultAdminiName.Users};
            MasterDiction.Add(Master.AdministratorName, Administrators);    //管理员名字
            string[] item = new string[3];
            MasterDiction.Add(Master.configAtrry, item);                  //配置文件集合
            //System.Windows.Forms.Form forms = new Mains();
            //MasterDiction.Add(Master.StartForm, forms);
            MasterDiction.Add(Master.HeadText, "#*,*#|/*,*/");//头文件标识字符串
        }
    }
}
