using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using SocketWin32Api;
using System.Threading;

namespace QQRobot
{
    /*
     * 面向enter发送窗口 
     */
    class SenderApi2
    {
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        private static Regex WndNameRegex;

        private static IntPtr FindHwnd = (IntPtr)0x0;

        private static string FindWndName = null;

        private static Object lockObj = new Object();

        private static int WM_PASTE = 0x0302; //<WinUser.h> #define WM_PASTE                        0x0302
        private static int WM_KEYDOWN = 0x0100; //#define WM_KEYDOWN                      0x0100
        private static int WM_KEYUP = 0x0101;//#define WM_KEYUP                        0x0101
        private static int VK_RETURN = 0x0D; //#define WM_KEYUP                        0x0101
        private static int VK_CONTROL = 0x11; //#define VK_CONTROL        0x11

        private static int WM_SYSKEYDOWN = 0x0104; //#define WM_SYSKEYDOWN                   0x0104
        private static int WM_SYSKEYUP = 0x0105; //#define WM_SYSKEYUP                     0x0105

        private static int MK_CONTROL = 0x0008; // #define MK_CONTROL          0x0008
        private static void Paste(IntPtr hwnd)
        {
            SendMessage(hwnd, WM_PASTE, 0, 0);

        }
        private static void Pasteln(IntPtr hwnd)
        {
            SendMessage(hwnd, WM_PASTE, 0, 0);
            ClipboardWrapper.Clear();
            ClipboardWrapper.SetText("\n");
            SendMessage(hwnd, WM_PASTE, 0, 0);
        }

        private static void Sumbit(IntPtr hwnd)
        {
            //SendMessage(hwnd, VK_CONTROL, 0, 0);WM_SYSKEYDOWN
            //SendMessage(hwnd, WM_SYSKEYDOWN, VK_CONTROL, 0);
            SendMessage(hwnd, WM_KEYDOWN, VK_RETURN, 0);
            Thread.Sleep(100);
            SendMessage(hwnd, WM_KEYUP, VK_RETURN, 0);
            //SendMessage(hwnd, WM_SYSKEYUP, VK_CONTROL, 0);

        }

        private static void PasteAndSumbit(IntPtr hwnd)
        {
            Paste(hwnd);
            Thread.Sleep(100);
            Sumbit(hwnd);
        }

        private static void Heartbeat(IntPtr hwnd)
        {
            Thread.Sleep(500);
            SendMessage(hwnd, WM_SYSKEYDOWN, 0x00000011, 0x20380001);
            Thread.Sleep(100);
            SendMessage(hwnd, WM_SYSKEYUP, 0x00000011, 0);
        }

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
            FindWndName = null;
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
            /*
            Console.Write("onEnumWindow:" + name.ToString());
            Console.WriteLine();
            */
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
