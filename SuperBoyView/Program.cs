﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SuperBoy.View
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] arge)
        {
            //Not connected to the Internet
            //ControlSuperBoy.IsIneter();
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new SuperBoys());
            SuperBoy.Model.Interface.IReadAndWrite read = new SuperBoy.Model.Parameter.ReadAndWrite();
            read.write("中国风", 5, "d:\\a.txt");

        }
    }
}
