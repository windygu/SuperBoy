using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperBoy.Model.Interface;
using SuperBoy.Model.Public;

namespace SuperBoy.Model.Controller
{
    class SystemNeed : ISystemControl
    {
        public bool StartProgram()
        {

            return true;
        }

        public bool DynamicProgram()
        {

            return true;
        }

        public bool CloseProgram()
        {

            return true;
        }
        public static void Append(object Method, EnumArry.ProgramStatus status)
        {
            switch (status)
            {
                case EnumArry.ProgramStatus.StartProgram:

                    break;
                case EnumArry.ProgramStatus.DynamicProgram:

                    break;
                case EnumArry.ProgramStatus.CloseProgram:

                    break;
                default:

                    break;
            }
        }
    }
}
