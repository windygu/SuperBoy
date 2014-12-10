using SuperBoy.Model.Interface;
using SuperBoy.Model.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperBoy.Model.Public
{
    public class LogSystemModel : ILogSystemModel
    {
        IReadAndWriteModel log = new ReadAndWriteModel();

        Model.Public.LoadModel model = new Public.LoadModel();
        public bool CommonLog(string systemInformation)
        {

            try
            {
                log.write(systemInformation, model.Address);
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
                string formatTxt = "";
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
                    case EnumArryModel.LogType.bak:
                        formatTxt = "bak";
                        break;
                    case EnumArryModel.LogType.odb:
                        formatTxt = "odb";
                        break;
                    default:
                        break;
                }

                log.write("[" + formatTxt + "]\r\n" + systemInformation, model.Address);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
