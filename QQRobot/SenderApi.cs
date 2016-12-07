using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using SocketWin32Api;

namespace QQRobot
{
    class SenderApi
    {
        [DllImport(@"ClipboardQQSender.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void PasteAndSumbit(IntPtr hwnd);

        [DllImport(@"ClipboardQQSender.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Paste(IntPtr hwnd);

        [DllImport(@"ClipboardQQSender.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pasteln(IntPtr hwnd);

        [DllImport(@"ClipboardQQSender.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Heartbeat(IntPtr hwnd);

        [DllImport(@"ClipboardQQSender.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Sumbit(IntPtr hwnd);
        
        private static Regex WndNameRegex;

        private static IntPtr FindHwnd = (IntPtr)0x0;

        private static string FindWndName = null;

        private static Object lockObj = new Object();

        public static void QQPaste(IntPtr ptr)
        {
            if (Win32Api.IsWindow(ptr))
            {
                lock (lockObj)
                {
                    Paste(ptr);
                }
            }
        }

        public static void QQPasteln(IntPtr ptr)
        {
            if (Win32Api.IsWindow(ptr))
            {
                lock (lockObj)
                {
                    Pasteln(ptr);
                }
            }
        }

        public static void QQPasteAndSumbit(IntPtr ptr)
        {
            if (Win32Api.IsWindow(ptr))
            {
                lock (lockObj)
                {
                    PasteAndSumbit(ptr);
                }
            }
        }

        public static void QQHeartbeat(IntPtr ptr)
        {
            if (Win32Api.IsWindow(ptr))
            {
                lock (lockObj)
                {
                    Heartbeat(ptr);
                }
            }
        }

        public static void QQSumbit(IntPtr ptr)
        {
            if (Win32Api.IsWindow(ptr))
            {
                lock (lockObj)
                {
                    Sumbit(ptr);
                }
            }
        }

        public static void StartQQFindHwnd(string titleRegex)
        {
            FindHwnd = (IntPtr)0x0;
            WndNameRegex = new Regex(titleRegex);
            Win32Api.EnumWindows(new Win32Api.WNDENUMPROC(onEnumWindow), 0);
        }

        public static IntPtr GetFindHwnd()
        {
            return FindHwnd;
        }

        public static string GetFindWndName()
        {
            return FindWndName;
        }

        private static bool onEnumWindow(IntPtr hWnd, int lParam)
        {
            bool result = true;
            StringBuilder name = new StringBuilder(256);//动态的字符串
            Win32Api.GetWindowTextW(hWnd, name, name.Capacity);
            
            Match match = WndNameRegex.Match(name.ToString());
            if (match.Success)
            {
                result = false;
                FindHwnd = hWnd;
                FindWndName = name.ToString();
            }
            return result;
        }
    }
}
