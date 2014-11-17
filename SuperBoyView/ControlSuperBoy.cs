using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace SuperBoyView
{
    /// <summary>
    /// This class is often used to control the program to the class
    /// </summary>
    public class ControlSuperBoy
    {


        /// <summary>
        /// this is a self Test method , If return Value is true is run!
        /// </summary>
        /// <returns></returns>
        public static Boolean SelfTest()
        {
            return true;
        }
        /// <summary>
        /// the first is to load .config,if this file is null ,
        /// Use the program to set the default configuration file
        /// According to the configuration file to change the assembly
        /// </summary>
        /// <returns></returns>
        public static Boolean load()
        {
            //load in master config
            Dictionary<EnumArry.Master, object> DicConfig = AssemblyConfiguration.MasterDiction;
            //read in start first configuration file
            string[] fileAddress = (string[])DicConfig[EnumArry.Master.configAtrry];
            string masterFileAddress = fileAddress[0].ToString();
            //if this file exists ,load in the file
            if (System.IO.File.Exists(masterFileAddress))
            {
                //load method
            }
            return true;
        }

        public static DataSet select()
        {

            return null;
        }

        /// <summary>
        /// this is a current workspace
        /// </summary>
        /// <returns>return Current index</returns>
        public static string current()
        {
            return null;
        }



    }
}
