using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Net.Sockets;
using System.Net;
using SimpleJSON;
using System.Threading;
using QQRobot;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace AutoLogin
{
    public partial class AutoLoginService : ServiceBase
    {
        private int RunningFlag;
        private ILog mLoger;
        private Task mListenerTask;
        private Task mAutoCheckTask;
        private Socket mServerSocket;
        private IntPtr mWnd;

        private static byte[] buffer = new byte[1024];

        public AutoLoginService()
        {
            InitializeComponent();
            mLoger = LogManager.GetLogger("AutoLogin.logging");
            mLoger.Info("init");
        }

        protected override void OnStart(string[] args)
        {
            mLoger.Info("OnStart");
            mLoger.Info("Bind port 19190");
            RunningFlag = 1;
            socketListenWork();
            mLoger.Info("OnStart end");
        }

        protected override void OnStop()
        {
            mLoger.Info("OnStop");
            if(mServerSocket != null)
            {
                try
                {
                    mServerSocket.Shutdown(SocketShutdown.Both);
                    mServerSocket.Close();
                }
                catch (Exception) { }
            }
            RunningFlag = 0;
        }

        private void taskWork()
        {
            while (RunningFlag == 1)
            {
                
            }
        }

        private void socketListenWork()
        {
            mLoger.Info("socketListenWork");
            mListenerTask = new Task(socketAcceptWork);
            try
            {
                mServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                mServerSocket.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 19190));
                mListenerTask.Start();
                mServerSocket.Listen(3);
            }
            catch (SocketException e)
            {
                mLoger.Error("socket exception:" + e.Message);
            }
            mLoger.Info("socketListenWork end");
        }

        private void socketAcceptWork()
        {
            mLoger.Info("socketAcceptWork");
            while (RunningFlag == 1)
            {
                mLoger.Info("socketAcceptWork Accept");
                Socket clientSocket = mServerSocket.Accept();
                clientSocket.Send(Encoding.UTF8.GetBytes("Server Say Hello"));
                Task.Factory.StartNew(new Action<object>(socketReceiveWork), clientSocket);
            }
            mLoger.Info("socketAcceptWork end");
        }

        private void socketReceiveWork(object accept)
        {
            Socket s = accept as Socket;
            string json = "" ;
            try
            {
                    //通过clientSocket接收数据  
                int receiveNumber = s.Receive(buffer);
                json = Encoding.UTF8.GetString(buffer, 0, receiveNumber);
                mLoger.Info(string.Format("接收客户端{0}消息{1}", s.RemoteEndPoint.ToString(), json));
                s.Send(Encoding.UTF8.GetBytes("end"));
            }
            catch (Exception ex)
            {
                mLoger.Info(ex.Message);
            }
            finally
            {
                s.Shutdown(SocketShutdown.Both);
                s.Close();
            }
            JSONNode node = null;
            try
            {
                node = JSON.Parse(json);
            }
            catch  { }
            switchCommand(node);
            
        }

        private void switchCommand(JSONNode json)
        {
            int command = -1;
            try
            {
                command = int.Parse(json["command"]);
            }
            catch (Exception) { }
            mLoger.Info("switch command:" + command);
            switch (command)
            {
                case 0:
                   
                    break;
                case 1:
                    break;
                case 2:
                    if(json != null)
                    {
                        int sec = int.Parse(json["sec"]);
                        loginAfterSec(sec);
                    }
                    break;
                case 3:
                    mLoger.Info("isLockedWindow = " + Win32Api.getInstance().isLockedWindow());
                    break;
                default:
                    break;
            }
        }

        private void loginAfterSec(int sec)
        {
            mLoger.Info("loginAfterSec:" + sec);
            Thread.Sleep(sec * 1000);
            try
            {
                Win32Api.getInstance().initWindowsDesktop();
            }catch(Exception e)
            {
                mLoger.Error(e.Message);
                mLoger.Error(e.StackTrace);
            }
            if(mWnd == IntPtr.Zero)
            {
                SenderApi.StartQQFindHwnd("四人帮");
                mWnd = SenderApi.GetFindHwnd();
            }
            SenderApi.pushStringToClipboard(mWnd, "guoyao19");
            SenderApi.QQPaste(mWnd);
            /*
            try
            {
               IntPtr rel = Win32Api.getInstance().Login("guoyao", "guoyao19");
                if(rel == IntPtr.Zero)
                {
                    uint err = Win32Api.GetLastError();
                    mLoger.Error("loginAfterSec Login err rel == null, err code = " + err);
                }
                else
                {
                    mLoger.Info("loginAfterSec Login success:" + rel);
                }
            }
            catch (Exception e)
            {
                mLoger.Info("loginAfterSec Login exc :" + e.Message);
            }
            */
        }

        private void stopAuto()
        {
            RunningFlag = 0;
        }
    }
}
