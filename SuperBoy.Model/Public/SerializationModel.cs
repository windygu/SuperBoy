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
            IAnalyticalModel Analytical = new AnalyticalModel();
            string sendStr = "";
            switch (database.SendType)
            {
                case SuperBoy.Model.Public.EnumArryModel.SendType.SELECT:
                    sendStr = Analytical.SELECT(database);
                    break;
                case SuperBoy.Model.Public.EnumArryModel.SendType.UPDATE:
                    sendStr = Analytical.UPDATE(database);
                    break;
                case SuperBoy.Model.Public.EnumArryModel.SendType.DELETE:
                    sendStr = Analytical.DELTE(database);
                    break;
                case SuperBoy.Model.Public.EnumArryModel.SendType.INSTER:
                    sendStr = Analytical.INSTER(database);
                    break;
                case SuperBoy.Model.Public.EnumArryModel.SendType.ONLINE:
                    sendStr = Analytical.ONLINE(database);
                    break;
                case SuperBoy.Model.Public.EnumArryModel.SendType.MANAGE:
                    sendStr = Analytical.MANAGE(database);
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
                    obj = retV.AnalyticalJson(txt);
                    break;
                case EnumArryModel.ReturnType.XML:
                    obj = retV.AnalyticalXML(txt);
                    break;
                case EnumArryModel.ReturnType.KeyValue:

                    obj = retV.AnalyticalKeyValue(txt);
                    break;
                case EnumArryModel.ReturnType.LIST:

                    obj = retV.AnalyticalLIST(txt);
                    break;
                case EnumArryModel.ReturnType.DICT:

                    obj = retV.AnalyticalDICT(txt);
                    break;
                default:
                    break;
            }
            return obj;
        }
    }
}
