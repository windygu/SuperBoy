using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperBoy.Model;
using SuperBoy.Model.Public;

namespace SuperBoy.Cloud
{
    /// <summary>
    /// run time class library
    /// the run time class library used for data calls after software startup
    /// the class at the time of initialization , at the end of the closed
    /// </summary>
    class Dynamic : EnumArry
    {


        /// <summary>
        /// Speech class library
        /// </summary>
        /// <param name="language"></param>
        public static void LangUageConfig(Language language)
        {
            //string[] item = Loads().Split('|');
            switch (language)
            {
                case Language.Ok:
                    //  languaeSpeak(MessBoxSpeak(item, 1));
                    break;
                case Language.No:
                    // languaeSpeak(MessBoxSpeak(item, 2));
                    break;
                case Language.Error:
                    //  languaeSpeak(MessBoxSpeak(item, 3));
                    break;
                case Language.PageError:
                    //  languaeSpeak(MessBoxSpeak(item, 4));
                    break;
                case Language.AddSuccess:
                    // languaeSpeak(MessBoxSpeak(item, 5));
                    break;
                case Language.AddError:
                    //  languaeSpeak(MessBoxSpeak(item, 6));
                    break;
                case Language.AddFailure:
                    // languaeSpeak(MessBoxSpeak(item, 7));
                    break;
                case Language.ErrorFun:
                    //  languaeSpeak(MessBoxSpeak(item, 8));
                    break;
                case Language.Welcome:
                    //  languaeSpeak(MessBoxSpeak(item, 9));
                    break;
                case Language.Setup:
                    //  languaeSpeak(MessBoxSpeak(item, 10));
                    break;
                default:
                    break;
            }
        }


    }
}
