﻿using System.Collections.Generic;
using SuperBoy.Model.Public;

namespace SuperBoy.Cloud
{
    class ProgramConfiguration : EnumArryModel
    {
        /// <summary>
        ///create a application configuration files List
        /// </summary>
        public static Dictionary<Master, object> MasterDiction = new Dictionary<Master, object>();

        static ProgramConfiguration()
        {
            MasterDiction.Add(Master.ConfigurationProtection, false);       //default closed configuration files protect
            MasterDiction.Add(Master.DefaultAdministrator, "Administrator");//The default administrator account for the administrator
            MasterDiction.Add(Master.SkeyProtection, false);               //Off by default the secret key protection
            MasterDiction.Add(Master.Jurisdiction, 3);                      //default administrator grade
            MasterDiction.Add(Master.MasterPath, "\\Master\\");             //default by Master
            MasterDiction.Add(Master.DataDefaultCount, 30);                  //data default count
            var item = new string[3];
            item[1] = "load";
            MasterDiction.Add(Master.ConfigAtrry, item);                  //configuration files list
            MasterDiction.Add(Master.DataTimeFormat, "yyyy-MM-dd HH-mm-ss");//dataTime forMater
            //System.Windows.Forms.Form forms = new Mains();
            //MasterDiction.Add(Master.StartForm, forms);
            MasterDiction.Add(Master.HeadText, "#*,*#");//head file mark file string
            //is auto control，if the value equ false not auto.
            MasterDiction.Add(Master.IsAuto, false);
        }
    }
}
