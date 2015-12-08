using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QQRobot
{
    class Sender
    {
        private String mWndNameRegex;
        private IntPtr mHwnd;

        public static Sender CreateSender(string wndNameRegex)
        {
            Sender instance = null;
            SenderApi.StartQQFindHwnd(wndNameRegex);
            IntPtr hwnd = SenderApi.GetFindHwnd();
            if (SenderApi.IsWindow(hwnd))
            {
                instance = new Sender();
                instance.mHwnd = hwnd;
                instance.mWndNameRegex = wndNameRegex;
            }
            return instance;
        }

        private Sender() { }

        public void send(string msg,Image img)
        {
            Clipboard.SetText(msg);
            SenderApi.QQPasteln(mHwnd);
            Clipboard.SetImage(img);
            SenderApi.QQPasteAndSumbit(mHwnd);
        }

    }
}
