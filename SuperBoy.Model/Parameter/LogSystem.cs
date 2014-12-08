using SuperBoy.Model.Interface;
using SuperBoy.Model.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperBoy.Model.Controller;

namespace SuperBoy.Model.Parameter
{
    public class LogSystem : SuperBoy.Model.Interface.ILogSystem
    {
        IReadAndWrite log = new ReadAndWrite();
        public bool CommonLog(string systemInformation)
        {

            try
            {

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool CommonLog(EnumArry.LogType format, string SystemInformation)
        {
            try
            {
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        void ILogSystem.CommonLog(string txt)
        {
            throw new NotImplementedException();
        }

        void ILogSystem.CommonLog(EnumArry.LogType logType, string txt)
        {
            throw new NotImplementedException();
        }
    }
}
