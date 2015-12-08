using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace QQRobot
{
    class SenderApi
    {
        [DllImport(@"CliboardQQSender.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void PasteAndSumbit(IntPtr hwnd);

        [DllImport(@"CliboardQQSender.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Paste(IntPtr hwnd);

        [DllImport(@"CliboardQQSender.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pasteln(IntPtr hwnd);

        [DllImport(@"CliboardQQSender.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Sumbit(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, int lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "IsWindow")]
        public static extern bool IsWindow(IntPtr hWnd);
        
        private static Regex WndNameRegex;

        private static IntPtr FindHwnd = (IntPtr)0x0;

        private static Object lockObj = new Object();

        public static void QQPaste(IntPtr ptr)
        {
            if (IsWindow(ptr))
            {
                lock (lockObj)
                {
                    Paste(ptr);
                }
            }
        }

        public static void QQPasteln(IntPtr ptr)
        {
            if (IsWindow(ptr))
            {
                lock (lockObj)
                {
                    Pasteln(ptr);
                }
            }
        }

        public static void QQPasteAndSumbit(IntPtr ptr)
        {
            if (IsWindow(ptr))
            {
                lock (lockObj)
                {
                    PasteAndSumbit(ptr);
                }
            }
        }

        public static void QQSumbit(IntPtr ptr)
        {
            if (IsWindow(ptr))
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
            EnumWindows(new WNDENUMPROC(onEnumWindow), 0);
        }

        public static IntPtr GetFindHwnd()
        {
            return FindHwnd;
        }

        public delegate bool WNDENUMPROC(IntPtr hWnd, int lParam);

        private static bool onEnumWindow(IntPtr hWnd, int lParam)
        {
            bool result = true;
            StringBuilder name = new StringBuilder(256);//动态的字符串
            GetWindowTextW(hWnd, name, name.Capacity);
            
            Match match = WndNameRegex.Match(name.ToString());
            if (match.Success)
            {
                result = false;
                FindHwnd = hWnd;
            }
            return result;
        }
    }
}
