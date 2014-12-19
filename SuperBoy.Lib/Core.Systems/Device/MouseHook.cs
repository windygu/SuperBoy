using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Core.Systems
{
    /// <summary>
    /// The global mouse hook. This can be used to globally capture mouse input.
    /// </summary>
    public static class MouseHook
    {
        // The handle to the hook (used for installing/uninstalling it).
        private static IntPtr _hHook = IntPtr.Zero;

        //Delegate that points to the filter function
        private static Hooks.HookProc _hookproc = new Hooks.HookProc(Filter);

        /// <summary>
        /// Delegate for handling mouse input.
        /// </summary>
        /// <param name="button">The mouse button that was pressed.</param>
        /// <returns>True if you want the key to pass through
        /// (be recognized for the app), False if you want it
        /// to be trapped (app never sees it).</returns>
        public delegate bool MouseButtonHandler(MouseButtons button);

        public delegate bool MouseMoveHandler(Point point);

        public delegate bool MouseScrollHandler(int delta);

        public static MouseButtonHandler ButtonDown;
        public static MouseButtonHandler ButtonUp;
        public static MouseMoveHandler Moved;
        public static MouseScrollHandler Scrolled;

        /// <summary>
        /// Keep track of the hook state.
        /// </summary>
        private static bool _enabled;

        /// <summary>
        /// Start the mouse hook.
        /// </summary>
        /// <returns>True if no exceptions.</returns>
        public static bool Enable()
        {
            if (_enabled == false)
            {
                try
                {
                    using (var curProcess = Process.GetCurrentProcess())
                    using (var curModule = curProcess.MainModule)
                        _hHook = Hooks.SetWindowsHookEx((int)Hooks.HookType.WhMouseLl, _hookproc, Hooks.GetModuleHandle(curModule.ModuleName), 0);
                    _enabled = true;
                    return true;
                }
                catch
                {
                    _enabled = false;
                    return false;
                }
            }
            else
                return false;
        }

        /// <summary>
        /// Disable mouse hooking.
        /// </summary>
        /// <returns>True if disabled correctly.</returns>
        public static bool Disable()
        {
            if (_enabled == true)
            {
                try
                {
                    Hooks.UnhookWindowsHookEx(_hHook);
                    _enabled = false;
                    return true;
                }
                catch
                {
                    _enabled = true;
                    return false;
                }
            }
            else
                return false;
        }

        private static IntPtr Filter(int nCode, IntPtr wParam, IntPtr lParam)
        {
            var result = true;

            if (nCode >= 0)
            {
                var info = (Hooks.MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(Hooks.MouseHookStruct));
                switch ((int)wParam)
                {
                    case Hooks.WmLbuttondown:
                        result = OnMouseDown(MouseButtons.Left);
                        break;
                    case Hooks.WmLbuttonup:
                        result = OnMouseUp(MouseButtons.Left);
                        break;
                    case Hooks.WmRbuttondown:
                        result = OnMouseDown(MouseButtons.Right);
                        break;
                    case Hooks.WmRbuttonup:
                        result = OnMouseUp(MouseButtons.Right);
                        break;
                    case Hooks.WmMbuttondown:
                        result = OnMouseDown(MouseButtons.Middle);
                        break;
                    case Hooks.WmMbuttonup:
                        result = OnMouseUp(MouseButtons.Middle);
                        break;
                    case Hooks.WmXbuttondown:
                        if (info.Data >> 16 == Hooks.Xbutton1)
                            result = OnMouseDown(MouseButtons.XButton1);
                        else if (info.Data >> 16 == Hooks.Xbutton2)
                            result = OnMouseDown(MouseButtons.XButton2);
                        break;
                    case Hooks.WmXbuttonup:
                        if (info.Data >> 16 == Hooks.Xbutton1)
                            result = OnMouseUp(MouseButtons.XButton1);
                        else if (info.Data >> 16 == Hooks.Xbutton2)
                            result = OnMouseUp(MouseButtons.XButton2);
                        break;
                    case Hooks.WmMousemove:
                        result = OnMouseMove(new Point(info.Point.X, info.Point.Y));
                        break;
                    case Hooks.WmMousewheel:
                        result = OnMouseWheel((info.Data >> 16) & 0xffff);
                        break;
                }
            }

            return result ? Hooks.CallNextHookEx(_hHook, nCode, wParam, lParam) : new IntPtr(1);
        }

        private static bool OnMouseDown(MouseButtons button)
        {
            if (ButtonDown != null)
                return ButtonDown(button);
            else
                return true;
        }

        private static bool OnMouseUp(MouseButtons button)
        {
            if (ButtonUp != null)
                return ButtonUp(button);
            else
                return true;
        }

        private static bool OnMouseMove(Point point)
        {
            if (Moved != null)
                return Moved(point);
            else
                return true;
        }

        private static bool OnMouseWheel(int delta)
        {
            if (Scrolled != null)
                return Scrolled(delta);
            else
                return true;
        }
    }

    public static class Hooks
    {
        #region Interop

        internal const int WmSize = 0x5;
        internal const int WmKeydown = 0x0100;
        internal const int WmKeyup = 0x0101;
        internal const int WmChar = 0x0102;
        internal const int WmSyskeydown = 0x0104;
        internal const int WmSyskeyup = 0x0105;

        //This is the Import for the SetWindowsHookEx function.
        //Use this function to install a thread-specific hook.
        [DllImport("user32.dll", CharSet = CharSet.Auto,
         CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn,
        IntPtr hInstance, int threadId);

        //This is the Import for the UnhookWindowsHookEx function.
        //Call this function to uninstall the hook.
        [DllImport("user32.dll", CharSet = CharSet.Auto,
         CallingConvention = CallingConvention.StdCall)]
        internal static extern bool UnhookWindowsHookEx(IntPtr idHook);

        //This is the Import for the CallNextHookEx function.
        //Use this function to pass the hook information to the next hook procedure in chain.
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion

        // The type of method used as a handler (Filter)
        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        // Different types of hooks.
        internal enum HookType : int
        {
            WhKeyboard = 2,
            WhGetmessage = 3,
            WhCallwndproc = 4,
            WhCbt = 5,
            WhSysmsgfilter = 6,
            WhMouse = 7,
            WhHardware = 8,
            WhDebug = 9,
            WhShell = 10,
            WhForegroundidle = 11,
            WhCallwndprocret = 12,
            WhKeyboardLl = 13,
            WhMouseLl = 14
        }

        //Mouse stuff
        internal const int WmMousemove = 0x200;
        internal const int WmMousewheel = 0x020A;
        internal const int WmLbuttondown = 0x201;
        internal const int WmRbuttondown = 0x204;
        internal const int WmMbuttondown = 0x207;
        internal const int WmXbuttondown = 0x20B;
        internal const int WmLbuttonup = 0x202;
        internal const int WmRbuttonup = 0x205;
        internal const int WmMbuttonup = 0x208;
        internal const int WmXbuttonup = 0x20C;

        internal const int Xbutton1 = 0x1;
        internal const int Xbutton2 = 0x2;

        [StructLayout(LayoutKind.Sequential)]
        internal struct MouseHookStruct
        {
            public Point Point;
            public int Data;
            public int Flags;
            public int Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Point
        {
            public int X;
            public int Y;
        }
    }
}
