using SuperBoy.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperBoy.Model.Public
{
    public class SerializationModel : ISerializationModel
    {
        public delegate object SerializationReturn(object obj);
        public string SuperBoyAnalytical(Model.Public.DatabseSend database)
        {
            // Model.Public.DatabseSend database = new Model.Public.DatabseSend();
            IAnalyticalModel analytical = new AnalyticalModel();
            string sendStr = "";
            switch (database.SendType)
            {
                case SuperBoy.Model.Public.EnumArryModel.SendType.SELECT:
                    sendStr = analytical.SELECT(database);
                    break;
                case SuperBoy.Model.Public.EnumArryModel.SendType.UPDATE:
                    sendStr = analytical.UPDATE(database);
                    break;
                case SuperBoy.Model.Public.EnumArryModel.SendType.DELETE:
                    sendStr = analytical.DELTE(database);
                    break;
                case SuperBoy.Model.Public.EnumArryModel.SendType.INSTER:
                    sendStr = analytical.INSTER(database);
                    break;
                case SuperBoy.Model.Public.EnumArryModel.SendType.ONLINE:
                    sendStr = analytical.ONLINE(database);
                    break;
                case SuperBoy.Model.Public.EnumArryModel.SendType.MANAGE:
                    sendStr = analytical.MANAGE(database);
                    break;
                default:
                    break;
            }

            return sendStr;
        }



        public object DataSuperBoyAnalytical(EnumArryModel.ReturnType returnValue, object txt)
        {
            object obj = "Null";
            IReturnValueAnalytical retV = new ReturnValueAnalytical();
            switch (returnValue)
            {
                case EnumArryModel.ReturnType.JSON:
                    return obj = retV.AnalyticalJson(txt);
                    break;
                case EnumArryModel.ReturnType.XML:
                    return obj = retV.AnalyticalXml(txt);
                    break;
                case EnumArryModel.ReturnType.KeyValue:

                    return obj = retV.AnalyticalKeyValue(txt);
                    break;
                case EnumArryModel.ReturnType.LIST:

                    return obj = retV.AnalyticalList(txt);
                    break;
                case EnumArryModel.ReturnType.DICT:

                    return obj = retV.AnalyticalDict(txt);
                    break;
                default:
                    break;
            }
            return obj;
        }
    }
}
