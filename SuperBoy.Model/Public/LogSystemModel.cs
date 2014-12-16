using System;
using SuperBoy.Model.Interface;

namespace SuperBoy.Model.Public
{
    public class LogSystemModel : ILogSystemModel
    {
        readonly IReadAndWriteModel _log = new ReadAndWriteModel();

        readonly LoadModel _model = new LoadModel();
        public bool CommonLog(string systemInformation)
        {

            try
            {
                _log.write(systemInformation, _model.Address);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CommonLog(EnumArryModel.LogType format, string systemInformation)
        {
            try
            {
                var formatTxt = "";
                switch (format)
                {
                    case EnumArryModel.LogType.Plain:
                        formatTxt = "Plain";
                        break;
                    case EnumArryModel.LogType.Log:
                        formatTxt = "Log";
                        break;
                    case EnumArryModel.LogType.System:
                        formatTxt = "System";
                        break;
                    case EnumArryModel.LogType.Master:
                        formatTxt = "Master";
                        break;
                    case EnumArryModel.LogType.Bak:
                        formatTxt = "bak";
                        break;
                    case EnumArryModel.LogType.Odb:
                        formatTxt = "odb";
                        break;
                    default:
                        break;
                }

                _log.write("[" + formatTxt + "]\r\n" + systemInformation, _model.Address);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
