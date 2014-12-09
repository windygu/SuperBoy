using SuperBoy.Model.Control;
using SuperBoy.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperBoy.Model.Public
{
    public class Serialization : ISerialization
    {
        public string SuperBoyAnalytical(Model.Public.DatabseSend database)
        {
            // Model.Public.DatabseSend database = new Model.Public.DatabseSend();
            IControlSuperBoyAnalytical iCloud = new ControlSuperBoyAnalytical();

            switch (database.SendType)
            {
                case SuperBoy.Model.Public.EnumArry.SendType.SELECT:
                    iCloud.SELECT(database);
                    break;
                case SuperBoy.Model.Public.EnumArry.SendType.UPDATE:
                    iCloud.UPDATE(database);
                    break;
                case SuperBoy.Model.Public.EnumArry.SendType.DELETE:
                    iCloud.DELTE(database);
                    break;
                case SuperBoy.Model.Public.EnumArry.SendType.INSTER:
                    iCloud.INSTER(database);
                    break;
                default:
                    break;
            }
            return "";
        }
    }
}
