using System;
using System.Runtime.InteropServices;
using System.Security;

namespace DotNet.Utilities
{
    /// <summary>
    /// 执行需要调用 <b>Win32</b> API 的操作辅助类。
    /// </summary>
    [SuppressUnmanagedCodeSecurity()]
	public static partial class Win32
	{
        /// <summary>
        /// 执行当前类在使用前的初始化操作。
        /// </summary>
        static Win32()
        {
            CurrentOs = GetCurrentPlatform();
        }


        /// <summary>
        /// 获取当前用户物理磁盘的性能信息。
        /// </summary>
        /// <returns>一个 <see cref="HDiskInfo"/> 结构，它保存了物理硬盘的性能数据。</returns>
        public static HDiskInfo GetHddInformation()
        {
            switch(CurrentOs)
            {
                case(Platform.Windows95) :
                case(Platform.Windows98):
                case(Platform.WindowsCe) :
                case(Platform.WindowsNt351) :
                case(Platform.WindowsNt40) :
                default :
                    throw new PlatformNotSupportedException(string.Format(ResourcesApi.Win32CurrentPlatformNotSupport, CurrentOs.ToString()));
                case(Platform.UnKnown):
                    throw new PlatformNotSupportedException(ResourcesApi.Win32CurrentPlatformUnknown);
                case(Platform.Windows982NdEdition):
                case(Platform.WindowsMe) :
                    return GetHddInfo9X(0);
                case(Platform.Windows2000) :
                case(Platform.WindowsXp) :
                case(Platform.Windows2003) :
                case (Platform.WindowsVista ):
                    return GetHddInfoNt(0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nXAmount"></param>
        /// <param name="nYAmount"></param>
        /// <param name="rectScrollRegion"></param>
        /// <param name="rectClip"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool ScrollWindow(HandleRef hWnd, int nXAmount, int nYAmount, ref Rect rectScrollRegion, ref Rect rectClip);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="nBar"></param>
        /// <param name="nPos"></param>
        /// <param name="bRedraw"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int SetScrollPos(HandleRef hWnd, int nBar, int nPos, bool bRedraw);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="fnBar"></param>
        /// <param name="si"></param>
        /// <param name="redraw"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int SetScrollInfo(HandleRef hWnd, int fnBar, Scrollinfo si, bool redraw);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hDc"></param>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int GetDeviceCaps(HandleRef hDc, int nIndex);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int LOWORD(int n)
        {
            return (n & 0xffff);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int LOWORD(IntPtr n)
        {
            return LOWORD((int)((long)n));
        }

        public static int HIWORD(int n)
        {
            return ((n >> 0x10) & 0xffff);
        }

        public static int HIWORD(IntPtr n)
        {
            return HIWORD((int)(long)n);
        }
	}
}
