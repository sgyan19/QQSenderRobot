using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace QQRobot
{
    class SenderApi
    {
        [DllImport(@"CliboardQQSender.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Send(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, int lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        private static extern int GetClassNameW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);
        public static void sendQQ()
        {
            Send((IntPtr)0x00150060);
        }

        public static uint findQQHwnd()
        {
            EnumWindows(new WNDENUMPROC(onEnumproc), 0);

            return 0;
        }
        public delegate bool WNDENUMPROC(IntPtr hWnd, int lParam);


        public static bool onEnumproc(IntPtr hWnd, int lParam)
        {

            return false;
        }
    }
}
