using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using SocketWin32Api;
using System.Threading;

namespace SocketWin32Api
{
    public class WindowSendApi
    {
        public static int WM_SETTEXT = 0x000C;
        public static int WM_IME_CHAR = 0x0286;
        public static int WM_KEYDOWN = 0x0100;
        public static int WM_KEYUP = 0x0101;
        public static uint VK_CONTROL = 0x11;
        public static uint VK_RETURN = 0x0D;

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

        [DllImport(@"ClipboardQQSender.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Sumbit2(IntPtr hwnd);

        [DllImport("user32.dll", EntryPoint = "PostMessage", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        private static extern bool PostMessage(IntPtr hwnd, int msg, uint wParam, uint lParam);
        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        private static extern bool SendMessage(IntPtr hwnd, int msg, uint wParam, uint lParam);
        private static Object lockObj = new Object();

        public static void WindowPaste(IntPtr ptr)
        {
            if (Win32Api.IsWindow(ptr))
            {
                Paste(ptr);
            }
        }

        public static void WindowPasteln(IntPtr ptr)
        {
            if (Win32Api.IsWindow(ptr))
            {
                Pasteln(ptr);
            }
        }

        public static void WindowPasteAndSumbit(IntPtr ptr)
        {
            if (Win32Api.IsWindow(ptr))
            {
                PasteAndSumbit(ptr);
            }
        }

        public static void WindowHeartbeat(IntPtr ptr)
        {
            if (Win32Api.IsWindow(ptr))
            {
                Heartbeat(ptr);
            }
        }

        public static void WindowSumbit(IntPtr ptr)
        {
            if (Win32Api.IsWindow(ptr))
            {
                Thread.Sleep(1000);
                Sumbit(ptr);
                //SendMessage(ptr, WM_KEYDOWN, VK_CONTROL, 0);
                //Thread.Sleep(200);
                //SendMessage(ptr, WM_KEYDOWN, VK_RETURN, 0);
                //Thread.Sleep(200);
                //SendMessage(ptr, WM_KEYDOWN, VK_RETURN, 0);
                //Thread.Sleep(200);
                //SendMessage(ptr, WM_KEYDOWN, VK_CONTROL, 0);
            }
        }

        public static void WindowsSend(IntPtr ptr, string text)
        {
            if (Win32Api.IsWindow(ptr))
            {
                char[] cc = text.ToCharArray();
                foreach (var chr in cc)
                {
                    PostMessage(ptr, WM_IME_CHAR, chr, 0);
                }
            }
        }
    }
}
