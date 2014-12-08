using SuperBoy.Model.Interface;
using SuperBoy.Model.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperBoy.Model.Public
{
    public class LogSystem : ILogSystem
    {
        IReadAndWrite log = new ReadAndWrite();

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

        public bool CommonLog(EnumArry.LogType format, string systemInformation)
        {
            try
            {
                string formatTxt = "";
                switch (format)
                {
                    case EnumArry.LogType.Plain:
                        formatTxt = "Plain";
                        break;
                    case EnumArry.LogType.Log:
                        formatTxt = "Log";
                        break;
                    case EnumArry.LogType.System:
                        formatTxt = "System";
                        break;
                    case EnumArry.LogType.Master:
                        formatTxt = "Master";
                        break;
                    case EnumArry.LogType.bak:
                        formatTxt = "bak";
                        break;
                    case EnumArry.LogType.odb:
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
