using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using SuperBoy.Model;
using SuperBoy.Model.Interface;
using SuperBoy.Model.Public;

namespace SuperBoy.Cloud
{
    /// <summary>
    /// This class is often used to control the program to the class
    /// </summary>
    public static class ControlSuperBoy
    {

        public static int CurrentCount = 0;
        public static string CurrentWhere = "";


        //New Idata
        public static Idatabase Idata = new SQLServer();

        public static DataSet CurrentFresh()
        {
            if (CurrentWhere.Length.Equals(0))
            {
                return select(CurrentCount);
            }
            else
            {
                return select(CurrentWhere, CurrentCount);
            }
        }
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
            Dictionary<EnumArry.Master, object> DicConfig = ProgramConfiguration.MasterDiction;
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

        /// <summary>
        /// this method is the query ALL
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static DataSet select(int count)
        {
            CurrentWhere = "";
            return Idata.SelectALL(count);
        }

        /// <summary>
        /// this method is the query ALL (Where)
        /// </summary>
        /// <param name="Where">Where</param>
        /// <param name="count">Count</param>
        /// <returns></returns>
        public static DataSet select(string Where, int count)
        {
            CurrentWhere = Where;
            CurrentCount = count;
            return Idata.SelectALL(Where, count);
        }

        public static int Update(string sql)
        {
            int count = Idata.Update(sql);
            return count;
        }
        /// <summary>
        /// this is a current workspace
        /// </summary>
        /// <returns>return Current index</returns>
        public static string current()
        {

            return null;
        }
        [DllImport("wininet.dll", EntryPoint = "InternetGetConnectedState")]

        //Is Inter
        public extern static bool InternetGetConnectedState(out int conState, int reder);

        public static Boolean IsIneter()
        {
            int n = 0;
            return (InternetGetConnectedState(out n, 0));
        }

        public static void ControlSuperBoys()
        {
            // super
        }

    }
}
