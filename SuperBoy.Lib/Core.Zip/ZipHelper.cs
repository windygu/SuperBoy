using System;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;

namespace Core.Zip
{
    /// <summary>
    /// 用系统WinRar进行压缩和解压缩
    /// </summary>
    public class ZipHelper
    {
        #region 私有变量
        static String _theRar; //WinRAR.exe 的完整路径 
        static RegistryKey _theReg; //注册表键 
        static Object _theObj; //键值 
        static String _theInfo;  //cmd命令值
        static ProcessStartInfo _theStartInfo;
        static Process _theProcess;
        #endregion

        //64位系统
        //此时会提示：未将对象引用设置为对象的实例 
        //解决办法：修改注册表，添加如下项：
        //HKEY_CLASSES_ROOT\Applications\WinRAR.exe\Shell\Open\Command 
        //值为："C:\Program Files (x86)\WinRAR\WinRAR.exe" "%1"

        #region 调用外部RAR解压缩
        private static string _rarRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe";

        /// <summary>
        /// 是否安装了Winrar
        /// </summary>
        /// <returns></returns>
        public static bool Exists()
        {
            var theReg = Registry.LocalMachine.OpenSubKey(_rarRegPath);
            return !string.IsNullOrEmpty(theReg.GetValue("").ToString());
        }



        #endregion
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="path">要压缩文件路径</param>
        /// <param name="rarPath">要压缩的文件名</param>
        /// <param name="rarName">压缩的文件路径</param>
        public static void EnZip(string path, string rarName, string rarPath)
        {
            //   bool flag = false;
            try
            {
                _theReg = Registry.ClassesRoot.OpenSubKey(@"Applications\WinRAR.exe\Shell\Open\Command");
                _theObj = _theReg.GetValue("");
                _theRar = _theObj.ToString();
                _theReg.Close();
                _theRar = _theRar.Substring(1, _theRar.Length - 7);
                // Directory.CreateDirectory(path);
                //
                //the_Info = " a    " + rarName + "  " + @"C:Test70821.txt"; //文件压缩
                _theInfo = " a   " + rarPath + "  " + path;
                #region 命令参数
                //// 1
                ////压缩即文件夹及其下文件
                //the_Info = " a    " + rarName + "  " + path + "  -r";              
                //// 2
                ////压缩即文件夹及其下文件 设置压缩方式为 .zip
                //the_Info = " a -afzip  " + rarName + "  " + path;  
                //// 3
                ////压缩文件夹及其下文件 直接设定为free.zip
                //the_Info = " a -r  " + rarName + "  " + path;
                //// 4
                ////搬迁压缩即文件夹及其下文件原文件将不存在
                //the_Info = " m  " + rarName + "  " + path;
                //// 5
                ////压缩即文件  直接设定为free.zip 只有文件 而没有文件夹
                //the_Info = " a -ep  " + rarName + "  " + path;
                //// 6
                ////加密压缩即文件夹及其下文件 密码为123456 注意参数间不要空格
                //the_Info = " a -p123456  " + rarName + "  " + path;
                #endregion


                _theStartInfo = new ProcessStartInfo();
                _theStartInfo.FileName = _theRar;
                _theStartInfo.Arguments = _theInfo;
                _theStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //打包文件存放目录
                _theStartInfo.WorkingDirectory = rarName;
                _theProcess = new Process();
                _theProcess.StartInfo = _theStartInfo;
                _theProcess.Start();
                _theProcess.WaitForExit();
                //if (the_Process.HasExited)
                //{
                //    flag = true;
                //}

                _theProcess.Close();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            // return flag;
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="zipname">要解压的文件名</param>
        /// <param name="zippath">要解压的文件路径</param>
        public static void DeZip(string zipname, string zippath)
        {
            try
            {
                _theReg = Registry.ClassesRoot.OpenSubKey(@"Applications\WinRar.exe\Shell\Open\Command");
                _theObj = _theReg.GetValue("");
                _theRar = _theObj.ToString();
                _theReg.Close();
                _theRar = _theRar.Substring(1, _theRar.Length - 7);
                _theInfo = " X " + zipname + " " + zippath;
                _theStartInfo = new ProcessStartInfo();
                _theStartInfo.FileName = _theRar;
                _theStartInfo.Arguments = _theInfo;
                _theStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                _theProcess = new Process();
                _theProcess.StartInfo = _theStartInfo;
                _theProcess.Start();
                _theProcess.WaitForExit();
  
                _theProcess.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



    }


}
