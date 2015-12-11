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
        private string mWndName;
        private Form form;
        private static System.Timers.Timer heartbeatTimer;
        private static IntPtr HeartbeatHwnd;

        public static Sender CreateSender(string wndNameRegex, Form form)
        {
            Sender instance = null;
            SenderApi.StartQQFindHwnd(wndNameRegex);
            IntPtr hwnd = SenderApi.GetFindHwnd();
            string wndName = SenderApi.GetFindWndName();
            if (SenderApi.IsWindow(hwnd))
            {
                instance = new Sender();
                instance.mHwnd = hwnd;
                instance.form = form;
                instance.mWndNameRegex = wndNameRegex;
                instance.mWndName = wndName;
                if(heartbeatTimer == null)
                {
                    heartbeatTimer = new System.Timers.Timer(5 * 60 * 1000);
                    heartbeatTimer.Elapsed += HeartbeatTimer_Elapsed;
                    HeartbeatHwnd = hwnd;
                    heartbeatTimer.Start();
                }
            }
            return instance;
        }

        private static void HeartbeatTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SenderApi.QQHeartbeat(HeartbeatHwnd);
        }

        public string getWndName()
        {
            return mWndName;
        }

        public void threadHeartbeat()
        {
            form.Invoke(new Heartbeat(heartbeat));
        }

        public void threadSend(string msg, Image[] imgs)
        {
            object[] args = new object[2];
            args[0] = msg;
            args[1] = imgs;
            form.Invoke(new Send(send), args);
        }
        private delegate void Send(string msg, Image[] imgs);
        public void send(string msg,Image[] imgs)
        {
            Clipboard.SetText(msg);
            SenderApi.QQPasteln(mHwnd);
            foreach(Image img in imgs)
            {
                Clipboard.SetImage(img);
                Thread.Sleep(1000);
                SenderApi.QQPaste(mHwnd);
            }
            SenderApi.QQSumbit(mHwnd);
        }
        private delegate void Heartbeat();
        public void heartbeat()
        {
            SenderApi.QQHeartbeat(mHwnd);
        }

        ~Sender()
        {
            heartbeatTimer.Stop();
        }
    }
}
