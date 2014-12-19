using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace Core.IO
{
    public class ExtensionAttachUtil
    {
        /// <summary>
        /// 关联文件
        /// </summary>
        public static void SaveReg(string filePathString, string pFileTypeName)
        {
            var regKey = Registry.ClassesRoot.OpenSubKey("", true);              //打开注册表
            if (regKey == null) return;
            var vrPkey = regKey.OpenSubKey(pFileTypeName, true);
            if (vrPkey != null)
            {
                regKey.DeleteSubKey(pFileTypeName, true);
            }

            regKey.CreateSubKey(pFileTypeName);
            vrPkey = regKey.OpenSubKey(pFileTypeName, true);
            if (vrPkey != null) vrPkey.SetValue("", "Exec");
            vrPkey = regKey.OpenSubKey("Exec", true);
            if (vrPkey != null) regKey.DeleteSubKeyTree("Exec");         //如果等于空就删除注册表DSKJIVR

            regKey.CreateSubKey("Exec");
            vrPkey = regKey.OpenSubKey("Exec", true);
            if (vrPkey == null) return;
            vrPkey.CreateSubKey("shell");
            vrPkey = vrPkey.OpenSubKey("shell", true);                      //写入必须路径
            if (vrPkey == null) return;
            vrPkey.CreateSubKey("open");
            vrPkey = vrPkey.OpenSubKey("open", true);
            if (vrPkey == null) return;
            vrPkey.CreateSubKey("command");
            vrPkey = vrPkey.OpenSubKey("command", true);
            var pathString = "\"" + filePathString + "\" \"%1\"";
            if (vrPkey != null) vrPkey.SetValue("", pathString);                                    //写入数据
        }

        /// <summary>
        /// 取消文件关联
        /// </summary>
        public static void DelReg(string pFileTypeName)
        {
            var regkey = Registry.ClassesRoot.OpenSubKey("", true);
            if (regkey == null) return;
            var vrPkey = regkey.OpenSubKey(pFileTypeName);
            if (vrPkey != null)
            {
                regkey.DeleteSubKey(pFileTypeName, true);
            }
            if (vrPkey != null)
            {
                regkey.DeleteSubKeyTree("Exec");
            }
        }
    }
}
