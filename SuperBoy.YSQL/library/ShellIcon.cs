using System;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;

namespace SuperBoy.YSQL.Lib
{
    /// <summary>
    /// 获取系统文件类信息
    /// </summary>
    public class ShellIcon
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct Shfileinfo
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };


        class Win32
        {
            public const uint ShgfiIcon = 0x000000100;// get icon
            public const uint ShgfiDisplayname = 0x000000200;// get display name
            public const uint ShgfiTypename = 0x000000400;// get type name
            public const uint ShgfiAttributes = 0x000000800;// get attributes
            public const uint ShgfiIconlocation = 0x000001000;// get icon location
            public const uint ShgfiExetype = 0x000002000;// return exe type
            public const uint ShgfiSysiconindex = 0x000004000;// get system icon index
            public const uint ShgfiLinkoverlay = 0x000008000;// put a link overlay on icon
            public const uint ShgfiSelected = 0x000010000;// show icon in selected state
            public const uint ShgfiAttrSpecified = 0x000020000;// get only specified attributes
            public const uint ShgfiLargeicon = 0x000000000;// get large icon
            public const uint ShgfiSmallicon = 0x000000001;// get small icon
            public const uint ShgfiOpenicon = 0x000000002;// get open icon
            public const uint ShgfiShelliconsize = 0x000000004;// get shell size icon
            public const uint ShgfiPidl = 0x000000008;// pszPath is a pidl
            public const uint ShgfiUsefileattributes = 0x000000010;// use passed dwFileAttribute

            public const uint FileAttributeReadonly = 0x00000001;
            public const uint FileAttributeHidden = 0x00000002;
            public const uint FileAttributeSystem = 0x00000004;
            public const uint FileAttributeDirectory = 0x00000010;
            public const uint FileAttributeArchive = 0x00000020;
            public const uint FileAttributeDevice = 0x00000040;
            public const uint FileAttributeNormal = 0x00000080;
            public const uint FileAttributeTemporary = 0x00000100;
            public const uint FileAttributeSparseFile = 0x00000200;
            public const uint FileAttributeReparsePoint = 0x00000400;
            public const uint FileAttributeCompressed = 0x00000800;
            public const uint FileAttributeOffline = 0x00001000;
            public const uint FileAttributeNotContentIndexed = 0x00002000;
            public const uint FileAttributeEncrypted = 0x00004000;

            [DllImport("shell32.dll")]
            public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref Shfileinfo psfi, uint cbSizeFileInfo, uint uFlags);

            [DllImport("User32.dll")]
            public static extern int DestroyIcon(System.IntPtr hIcon);
        }


        public ShellIcon()
        {
        }

        /// <summary>
        /// 获取扩展名信息
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static string GetTypeInfo(string ext)
        {
            Shfileinfo shinfo = new Shfileinfo();
            Win32.SHGetFileInfo(ext, Win32.FileAttributeNormal, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.ShgfiTypename | Win32.ShgfiUsefileattributes);
            return shinfo.szTypeName;
        }

        /// <summary>
        /// 获取扩展名的图标
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static Icon GetSmallTypeIcon(string ext)
        {
            Shfileinfo shinfo = new Shfileinfo();
            Win32.SHGetFileInfo(ext, Win32.FileAttributeNormal, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.ShgfiIcon | Win32.ShgfiSmallicon | Win32.ShgfiUsefileattributes);
            if (shinfo.hIcon.ToInt32() == 0) return null;
            Icon shellIcon = (Icon)System.Drawing.Icon.FromHandle(shinfo.hIcon).Clone();
            Win32.DestroyIcon(shinfo.hIcon);
            return shellIcon;
        }

        /// <summary>
        /// 获取文件的小图标
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Icon GetSmallIcon(string fileName)
        {
            Shfileinfo shinfo = new Shfileinfo();
            Win32.SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.ShgfiIcon | Win32.ShgfiSmallicon);
            if (shinfo.hIcon.ToInt32() == 0) return null;
            Icon shellIcon = (Icon)System.Drawing.Icon.FromHandle(shinfo.hIcon).Clone();
            Win32.DestroyIcon(shinfo.hIcon);
            return shellIcon;
        }

        /// <summary>
        /// 获取文件的大图标
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Icon GetLargeIcon(string fileName)
        {
            Shfileinfo shinfo = new Shfileinfo();
            Win32.SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.ShgfiIcon | Win32.ShgfiLargeicon);
            if (shinfo.hIcon.ToInt32() == 0) return null;
            Icon shellIcon = (Icon)System.Drawing.Icon.FromHandle(shinfo.hIcon).Clone();
            Win32.DestroyIcon(shinfo.hIcon);
            return shellIcon;
        }
    }
}
