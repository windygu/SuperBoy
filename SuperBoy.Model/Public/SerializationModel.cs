using SuperBoy.Model.Interface;

namespace SuperBoy.Model.Public
{
    public class SerializationModel : ISerializationModel
    {
        public delegate object SerializationReturn(object obj);
        public string SuperBoyAnalytical(DatabseSend database)
        {
            // Model.Public.DatabseSend database = new Model.Public.DatabseSend();
            IAnalyticalModel analytical = new AnalyticalModel();
            var sendStr = "";
            switch (database.SendType)
            {
                case EnumArryModel.SendType.Select:
                    sendStr = analytical.Select(database);
                    break;
                case EnumArryModel.SendType.Update:
                    sendStr = analytical.Update(database);
                    break;
                case EnumArryModel.SendType.Delete:
                    sendStr = analytical.Delete(database);
                    break;
                case EnumArryModel.SendType.Inster:
                    sendStr = analytical.Inster(database);
                    break;
                case EnumArryModel.SendType.Online:
                    sendStr = analytical.Online(database);
                    break;
                case EnumArryModel.SendType.Manage:
                    sendStr = analytical.Manage(database);
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
                case EnumArryModel.ReturnType.Json:
                    return obj = retV.AnalyticalJson(txt);
                    break;
                case EnumArryModel.ReturnType.Xml:
                    return obj = retV.AnalyticalXml(txt);
                    break;
                case EnumArryModel.ReturnType.KeyValue:

                    return obj = retV.AnalyticalKeyValue(txt);
                    break;
                case EnumArryModel.ReturnType.List:

                    return obj = retV.AnalyticalList(txt);
                    break;
                case EnumArryModel.ReturnType.Dict:

                    return obj = retV.AnalyticalDict(txt);
                    break;
                default:
                    break;
            }
            return obj;
        }
    }
}
