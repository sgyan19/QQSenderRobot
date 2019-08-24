using SocketWin32Api;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace QQRobot
{
    /// <summary>
    /// QQ发送器，将字符串和图片分步设置到剪切板，调用senderApi发送到指定句柄的qq窗口
    /// </summary>
    class QQSender : Sender
    {
        private string mWndNameRegex;   // 窗口名正则，用于查找
        private IntPtr mHwnd;   // 目标窗口句柄
        private string mWndName;    // 窗口名字
        private Form form;      // 操作剪贴板需要ui线程中，因此必须要程序窗口invoke
        private static System.Timers.Timer heartbeatTimer; // 心跳计时器，每间隔一定时间向QQ 窗口发送空消息，避免长时间无抓取结果，QQ进入离开状态
        private static IntPtr HeartbeatHwnd;    // 心跳消息窗口句柄

        /// <summary>
        /// 工厂方法，创建sender时，必须明确需要发送到哪些窗口。工厂函数负责依据提供的窗口名查找当前系统的QQ窗口。
        /// </summary>
        /// <param name="wndNameRegex">窗口名正则串</param>
        /// <param name="form">程序主窗口</param>
        /// <returns></returns>
        public static QQSender CreateSender(string wndNameRegex, Form form)
        {
            QQSender instance = null;
            SenderApi.StartQQFindHwnd(wndNameRegex);
            IntPtr hwnd = SenderApi.GetFindHwnd();
            string wndName = SenderApi.GetFindWndName();
            if (Win32Api.IsWindow(hwnd))
            {
                instance = new QQSender();
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
        /// <summary>
        /// 心跳计时器回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void HeartbeatTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SenderApi.QQHeartbeat(HeartbeatHwnd);
        }
        /// <summary>
        /// 获取窗口名
        /// </summary>
        /// <returns></returns>
        public string getWndName()
        {
            return mWndName;
        }

        /// <summary>
        /// 复写基类发送方法，向ui线程发送代理
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="imgs"></param>
        public override void send(string msg, Image[] imgs)
        {
            object[] args = new object[2];
            args[0] = msg;
            args[1] = imgs;
            form.Invoke(new Send(mainThreadSend), args);
        }
        /// <summary>
        /// 核心发送方法
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="imgs"></param>
        public void mainThreadSend(string msg,Image[] imgs)
        {
            ClipboardWrapper.Clear();
            ClipboardWrapper.SetText(msg);
            SenderApi.QQPasteln(mHwnd);
            if (imgs != null)
            {
                foreach (Image img in imgs)
                {
                    if (img != null)
                    {
                        ClipboardWrapper.SetImage(img);
                        Thread.Sleep(1000);
                        SenderApi.QQPaste(mHwnd);
                    }
                }
            }
            SenderApi.QQSumbit(mHwnd);
        }
        /// <summary>
        /// 心跳代理
        /// </summary>
        private delegate void Heartbeat();
        public void heartbeat()
        {
            SenderApi.QQHeartbeat(mHwnd);
        }

        public override void sendWithUser(string userName, Image userHeader, string source, string msg, Image[] imgs, string longImgPath)
        {
            object[] args = new object[6];
            args[0] = userName;
            args[1] = userHeader;
            args[2] = source;
            args[3] = msg;
            args[4] = imgs;
            args[5] = longImgPath;
            form.Invoke(new SendWithUser(mainThreadSendWithUser), args);
        }

        /// <summary>
        /// 核心发送方法
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="imgs"></param>
        public void mainThreadSendWithUser(string userName, Image userHeader, string msg, Image[] imgs, string longImgPath)
        {
            ClipboardWrapper.Clear();
            if(userHeader != null)
            {
                ClipboardWrapper.SetImage(userHeader);
                SenderApi.QQPaste(mHwnd);
            }
            if (!string.IsNullOrEmpty(userName))
            {
                ClipboardWrapper.SetText(userName);
                SenderApi.QQPasteln(mHwnd);
                ClipboardWrapper.Clear();
            }
            ClipboardWrapper.SetText(msg);
            SenderApi.QQPasteln(mHwnd);
            foreach (Image img in imgs)
            {
                if (img != null)
                {
                    ClipboardWrapper.SetImage(img);
                    Thread.Sleep(2000);
                    SenderApi.QQPasteln(mHwnd);
                }
            }
            Thread.Sleep(1000);
            SenderApi.QQSumbit(mHwnd);
        }

        /// <summary>
        /// 核心发送方法
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="imgs"></param>
        public void mainThreadSendWithUser(string userName, Image userHeader, string source, string msg, Image[] imgs, string longImgPath)
        {
            ClipboardWrapper.Clear();
            if (userHeader != null)
            {
                ClipboardWrapper.SetImage(userHeader);
                SenderApi.QQPaste(mHwnd);
            }
            if (!string.IsNullOrEmpty(userName))
            {
                ClipboardWrapper.SetText(userName);
                SenderApi.QQPasteln(mHwnd);
                ClipboardWrapper.Clear();
            }
            ClipboardWrapper.SetText(msg);
            SenderApi.QQPasteln(mHwnd);
            foreach (Image img in imgs)
            {
                if (img != null)
                {
                    ClipboardWrapper.SetImage(img);
                    Thread.Sleep(2000);
                    SenderApi.QQPasteln(mHwnd);
                }
            }
            Thread.Sleep(1000);
            if (!string.IsNullOrEmpty(source))
            {
                ClipboardWrapper.Clear();
                ClipboardWrapper.SetText(source);
                SenderApi.QQPasteAndSumbit(mHwnd);
            }
            else
            {
                SenderApi.QQSumbit(mHwnd);
            }
        }

        public override string getName()
        {
            return getWndName();
        }

        ~QQSender()
        {
            heartbeatTimer.Stop();
        }
    }
}
