using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperBoy.Model;
using SuperBoy.Model.Public;

namespace SuperBoy.Cloud
{
    /// <summary>
    /// The program initialization class
    /// </summary>
    class Initialization : EnumArry
    {

        /// <summary>
        /// 配置文件初始化字典，此字典将运行的集合整合在一起
        /// </summary>
        public static Dictionary<configAtrry, Boolean> ConfiguraDic = new Dictionary<configAtrry, Boolean>();
        /// <summary>
        /// 配置类型初始化字典，此字典将配置属性集合整合在一起
        /// </summary>
        public static Dictionary<Config, object> FunctionDic = new Dictionary<Config, object>();
        /// <summary>
        /// 配置类型地址字典，此字典将配置地址集合整合在一起
        /// </summary>
        public static Dictionary<configAtrry, string[]> FunctionDicFile = new Dictionary<configAtrry, string[]>();

        /// <summary>
        /// 系统默认配置将会启用的配置文件
        /// </summary>
        static Initialization()
        {
            //启动时候的配置文件，地址
            string[] StartConfigFile = new string[3];
            ConfiguraDic.Add(configAtrry.StartConfig, true);
            if (ConfiguraDic[configAtrry.StartConfig])
                FunctionDicFile.Add(configAtrry.StartConfigFile, StartConfigFile);

            //皮肤等跟启动一起出现的配置文件，地址
            string[] SkinConfigFile = new string[3];
            ConfiguraDic.Add(configAtrry.SkinConfig, true);
            if (ConfiguraDic[configAtrry.SkinConfig])
                FunctionDicFile.Add(configAtrry.SkinConfigFile, SkinConfigFile);

            //运行时候读取的配置文件，地址
            string[] FuncConfigFile = new string[3];
            ConfiguraDic.Add(configAtrry.FuncConfig, true);
            if (ConfiguraDic[configAtrry.FuncConfig])
                FunctionDicFile.Add(configAtrry.FuncConfigFile, FuncConfigFile);

            //关闭时的配置文件，地址
            string[] CloseConfigFile = new string[3];
            ConfiguraDic.Add(configAtrry.CloseConfig, true);
            if (ConfiguraDic[configAtrry.CloseConfig])
                FunctionDicFile.Add(configAtrry.CloseConfigFile, CloseConfigFile);

            //临时的配置文件，地址
            string[] TemporaryConfigFile = new string[3];
            ConfiguraDic.Add(configAtrry.TemporaryConfig, true);
            if (ConfiguraDic[configAtrry.TemporaryConfig])
                FunctionDicFile.Add(configAtrry.TemporaryConfigFile, TemporaryConfigFile);

            //读取属性
        }



        /// <summary>
        /// 程序集关闭的时候调用
        /// </summary>
        public static void ApplicationClose()
        {
            //删除未结束操作

        }
        ///// <summary>
        ///// 前景色背景色共四色
        ///// </summary>
        ///// <param name="item"></param>
        ///// <returns></returns>
        //public static System.Drawing.Color[] Color()
        //{
        //    System.Drawing.Color[] Color = new System.Drawing.Color[4];
        //    ReadAndWrite.MessBoxSpeak("");
        //    return "";
        //}
    }
}
