using SuperBoy.Model.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperBoy.Model.Parameter
{
    public class LogSystem : SuperBoy.Model.ILogSystem
    {

        public bool CommonLog(string systemInformation)
        {

            try
            {
                //call read
               
               

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CommonLogs(EnumArry.ConfigFormat format, string SystemInformation)
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
    }
}
