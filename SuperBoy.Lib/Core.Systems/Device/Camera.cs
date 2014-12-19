using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Core.Systems
{
    /// <summary>
    /// 摄像头操作辅助类，包括开启、关闭、抓图、设置等功能
    /// </summary>
    public class Camera
    {
        private IntPtr _lwndC;
        private IntPtr _mControlPtr;
        private int _mWidth;
        private int _mHeight;

        // 构造函数
        public Camera(IntPtr handle, int width, int height)
        {
            _mControlPtr = handle;
            _mWidth = width;
            _mHeight = height;
        }

        // 帧回调的委托
        public delegate void RecievedFrameEventHandler(byte[] data);
        public event RecievedFrameEventHandler RecievedFrame;
        private AviCapture.FrameEventHandler _mFrameEventHandler;

        /// <summary>
        /// 关闭摄像头
        /// </summary>
        public void CloseWebcam()
        {
            this.CapDriverDisconnect(this._lwndC);
        }

        /// <summary>
        /// 开启摄像头
        /// </summary>
        public void StartWebCam()
        {
            var lpszName = new byte[100];
            var lpszVer = new byte[100];

            AviCapture.capGetDriverDescriptionA(1, lpszName, 100, lpszVer, 100);
            this._lwndC = AviCapture.capCreateCaptureWindowA(lpszName, AviCapture.WsVisible + AviCapture.WsChild, 0, 0, _mWidth, _mHeight, _mControlPtr, 0);

            if (this.CapDriverConnect(this._lwndC, 0))
            {
                this.CapPreviewRate(this._lwndC, 66);

                this.CapPreview(this._lwndC, true);
                this.CapOverlay(this._lwndC, true);
                var bitmapinfo = new AviCapture.Bitmapinfo();
                bitmapinfo.bmiHeader.biSize = AviCapture.SizeOf(bitmapinfo.bmiHeader);
                bitmapinfo.bmiHeader.biWidth = this._mWidth;
                bitmapinfo.bmiHeader.biHeight = this._mHeight;
                bitmapinfo.bmiHeader.biPlanes = 1;
                bitmapinfo.bmiHeader.biBitCount = 24;
                this.CapSetVideoFormat(this._lwndC, ref bitmapinfo, AviCapture.SizeOf(bitmapinfo));

                this._mFrameEventHandler = new AviCapture.FrameEventHandler(FrameCallBack);
                this.CapSetCallbackOnFrame(this._lwndC, this._mFrameEventHandler);
                AviCapture.SetWindowPos(this._lwndC, 0, 0, 0, _mWidth, _mHeight, 6);
            }
        }

        /// <summary>
        /// 抓图到文件
        /// </summary>
        /// <param name="path"></param>
        public void GrabImage(string path)
        {
            var hBmp = Marshal.StringToHGlobalAnsi(path);
            AviCapture.SendMessage(_lwndC, AviCapture.WmCapSavedib, 0, hBmp.ToInt32());
        }

        /// <summary>
        /// 抓图到剪切板
        /// </summary>
        /// <returns></returns>
        public bool GrabImageToClipBoard()
        {
            return AviCapture.SendMessage(_lwndC, AviCapture.WmCapEditCopy, 0, 0);
        }

        /// <summary>
        /// 弹出色彩设置对话框
        /// </summary>
        public void SetCaptureSource()
        {
            var caps = new AviCapture.Capdrivercaps();
            AviCapture.SendMessage(_lwndC, AviCapture.WmCapGetCaps, AviCapture.SizeOf(caps), ref  caps);
            if (caps.fHasDlgVideoSource)
            {
                AviCapture.SendMessage(_lwndC, AviCapture.WmCapDlgVideosource, 0, 0);
            }
        }

        /// <summary>
        /// 弹出视频格式设置对话框
        /// </summary>
        public void SetCaptureFormat() 
        {
            var caps = new AviCapture.Capdrivercaps();
            AviCapture.SendMessage(_lwndC, AviCapture.WmCapGetCaps, AviCapture.SizeOf(caps), ref  caps);
            if (caps.fHasDlgVideoSource)
            {
                AviCapture.SendMessage(_lwndC, AviCapture.WmCapDlgVideoformat, 0, 0);
            }
        }


        #region 以下为私有函数
        private bool CapDriverConnect(IntPtr lwnd, short i)
        {
            return AviCapture.SendMessage(lwnd, AviCapture.WmCapDriverConnect, i, 0);
        }

        private bool CapDriverDisconnect(IntPtr lwnd)
        {
            return AviCapture.SendMessage(lwnd, AviCapture.WmCapDriverDisconnect, 0, 0);
        }

        private bool CapPreview(IntPtr lwnd, bool f)
        {
            return AviCapture.SendMessage(lwnd, AviCapture.WmCapSetPreview, f, 0);
        }

        private bool CapPreviewRate(IntPtr lwnd, short wMs)
        {
            return AviCapture.SendMessage(lwnd, AviCapture.WmCapSetPreviewrate, wMs, 0);
        }

        private bool CapSetCallbackOnFrame(IntPtr lwnd, AviCapture.FrameEventHandler lpProc)
        {
            return AviCapture.SendMessage(lwnd, AviCapture.WmCapSetCallbackFrame, 0, lpProc);
        }

        private bool CapSetVideoFormat(IntPtr hCapWnd, ref AviCapture.Bitmapinfo bmpFormat, int capFormatSize)
        {
            return AviCapture.SendMessage(hCapWnd, AviCapture.WmCapSetVideoformat, capFormatSize, ref bmpFormat);
        }

        private void FrameCallBack(IntPtr lwnd, IntPtr lpVHdr)
        {
            var videoHeader = new AviCapture.Videohdr();
            byte[] videoData;
            videoHeader = (AviCapture.Videohdr)AviCapture.GetStructure(lpVHdr, videoHeader);
            videoData = new byte[videoHeader.dwBytesUsed];
            AviCapture.Copy(videoHeader.lpData, videoData);
            if (this.RecievedFrame != null)
                this.RecievedFrame(videoData);
        }
        private bool CapOverlay(IntPtr lwnd, bool f)
        {
            return AviCapture.SendMessage(lwnd, AviCapture.WmCapSetOverlay, f, 0);
        } 
        #endregion

    }

    /// <summary>
    /// 视频辅助类
    /// </summary>
    internal class AviCapture
    {
        //通过调用acicap32.dll进行读取摄像头数据
        [DllImport("avicap32.dll")]
        public static extern IntPtr capCreateCaptureWindowA(byte[] lpszWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, int nId);
        [DllImport("avicap32.dll")]
        public static extern bool capGetDriverDescriptionA(short wDriver, byte[] lpszName, int cbName, byte[] lpszVer, int cbVer);
        [DllImport("User32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);
        [DllImport("User32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, int wMsg, short wParam, int lParam);
        [DllImport("User32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, int wMsg, short wParam, FrameEventHandler lParam);
        [DllImport("User32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, int wMsg, int wParam, ref Bitmapinfo lParam);
        [DllImport("User32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, int wMsg, int wParam, ref Capdrivercaps lParam);
        [DllImport("User32.dll")]
        public static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);
        [DllImport("avicap32.dll")]
        public static extern int capGetVideoFormat(IntPtr hWnd, IntPtr psVideoFormat, int wSize);

        //部分常量
        public const int WmUser = 0x400;
        public const int WsChild = 0x40000000;
        public const int WsVisible = 0x10000000;
        public const int SwpNomove = 0x2;
        public const int SwpNozorder = 0x4;
        public const int WmCapDriverConnect = WmUser + 10;
        public const int WmCapDriverDisconnect = WmUser + 11;
        public const int WmCapSetCallbackFrame = WmUser + 5;
        public const int WmCapSetPreview = WmUser + 50;
        public const int WmCapSetPreviewrate = WmUser + 52;
        public const int WmCapSetVideoformat = WmUser + 45;
        public const int WmCapSavedib = WmUser + 25;
        public const int WmCapSetOverlay = WmUser + 51;
        public const int WmCapGetCaps = WmUser + 14;
        public const int WmCapDlgVideoformat = WmUser + 41;
        public const int WmCapDlgVideosource = WmUser + 42;
        public const int WmCapDlgVideodisplay = WmUser + 43;
        public const int WmCapEditCopy = WmUser + 30;
        public const int WmCapSetSequenceSetup = WmUser + 64;
        public const int WmCapGetSequenceSetup = WmUser + 65;


        // 结构
        [StructLayout(LayoutKind.Sequential)]
        //VideoHdr
        public struct Videohdr
        {
            [MarshalAs(UnmanagedType.I4)]
            public int lpData;
            [MarshalAs(UnmanagedType.I4)]
            public int dwBufferLength;
            [MarshalAs(UnmanagedType.I4)]
            public int dwBytesUsed;
            [MarshalAs(UnmanagedType.I4)]
            public int dwTimeCaptured;
            [MarshalAs(UnmanagedType.I4)]
            public int dwUser;
            [MarshalAs(UnmanagedType.I4)]
            public int dwFlags;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public int[] dwReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        //BitmapInfoHeader
        public struct Bitmapinfoheader
        {
            [MarshalAs(UnmanagedType.I4)]
            public Int32 biSize;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 biWidth;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 biHeight;
            [MarshalAs(UnmanagedType.I2)]
            public short biPlanes;
            [MarshalAs(UnmanagedType.I2)]
            public short biBitCount;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 biCompression;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 biSizeImage;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 biXPelsPerMeter;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 biYPelsPerMeter;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 biClrUsed;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        //BitmapInfo
        public struct Bitmapinfo
        {
            [MarshalAs(UnmanagedType.Struct, SizeConst = 40)]
            public Bitmapinfoheader bmiHeader;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public Int32[] bmiColors;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Capdrivercaps
        {
            [MarshalAs(UnmanagedType.U2)]
            public UInt16 wDeviceIndex;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fHasOverlay;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fHasDlgVideoSource;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fHasDlgVideoFormat;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fHasDlgVideoDisplay;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fCaptureInitialized;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fDriverSuppliesPalettes;
            [MarshalAs(UnmanagedType.I4)]
            public int hVideoIn;
            [MarshalAs(UnmanagedType.I4)]
            public int hVideoOut;
            [MarshalAs(UnmanagedType.I4)]
            public int hVideoExtIn;
            [MarshalAs(UnmanagedType.I4)]
            public int hVideoExtOut;
        }


        public delegate void FrameEventHandler(IntPtr lwnd, IntPtr lpVHdr);

        // 公共函数
        public static object GetStructure(IntPtr ptr, ValueType structure)
        {
            return Marshal.PtrToStructure(ptr, structure.GetType());
        }

        public static object GetStructure(int ptr, ValueType structure)
        {
            return GetStructure(new IntPtr(ptr), structure);
        }

        public static void Copy(IntPtr ptr, byte[] data)
        {
            Marshal.Copy(ptr, data, 0, data.Length);
        }

        public static void Copy(int ptr, byte[] data)
        {
            Copy(new IntPtr(ptr), data);
        }

        public static int SizeOf(object structure)
        {
            return Marshal.SizeOf(structure);
        }
    }
}
