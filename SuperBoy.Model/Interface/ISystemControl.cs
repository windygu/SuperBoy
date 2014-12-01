using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperBoy.Model.Interface
{
    public interface ISystemControl
    {
        /// <summary>
        /// this is a start program
        /// </summary>
        /// <returns></returns>
        bool StartProgram();
        /// <summary>
        /// this is a dynamic program
        /// </summary>
        /// <returns></returns>
        bool DynamicProgram();
        /// <summary>
        /// this is a Close Program
        /// </summary>
        /// <returns></returns>
        bool CloseProgram();
    }
}
