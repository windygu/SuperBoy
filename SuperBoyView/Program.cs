using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SuperBoyView
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SuperBoys());

        }
    }
}
