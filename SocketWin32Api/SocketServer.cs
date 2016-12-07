using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SocketWin32Api
{
    public class SocketServer
    {
        private int RunningFlag;
        private Task mListenerTask;
        private Task mAutoCheckTask;
        private Socket mServerSocket;
        private int mPort;
        private static byte[] buffer = new byte[1024];

        public void start(int port = 19190)
        {
            RunningFlag = 1;
            socketListenWork();
            mPort = port;
        }

        public void stop()
        {
            if (mServerSocket != null)
            {
                try
                {
                    mServerSocket.Shutdown(SocketShutdown.Both);
                    mServerSocket.Close();
                }
                catch (Exception) { }
            }
        }

        private void socketListenWork()
        {
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
            }
        }

        private void socketAcceptWork()
        {
            while (RunningFlag == 1)
            {
                Socket clientSocket = mServerSocket.Accept();
                clientSocket.Send(Encoding.UTF8.GetBytes("Server Say Hello"));
                Task.Factory.StartNew(new Action<object>(socketReceiveWork), clientSocket);
            }
        }

        private void socketReceiveWork(object accept)
        {
            Socket s = accept as Socket;
            string json = "";
            try
            {
                //通过clientSocket接收数据  
                int receiveNumber = s.Receive(buffer);
                json = Encoding.UTF8.GetString(buffer, 0, receiveNumber);

                JSONNode node = null;
                try
                {
                    node = JSON.Parse(json);
                }
                catch { }
                string end = handleCommand(node);
                JSONClass response = new JSONClass();
                response.Add("code", "0");
                response.Add("data", end);
                s.Send(Encoding.UTF8.GetBytes(response.ToString()));
            }
            catch (Exception ex)
            {
            }
            finally
            {
                s.Shutdown(SocketShutdown.Both);
                s.Close();
            }

        }

        private string handleCommand(JSONNode json)
        {
            int command = -1;
            string back = "success";
            try
            {
                command = int.Parse(json["command"]);
            }
            catch (Exception) { }
            switch (command)
            {
                case 0:

                    break;
                case 1:
                    break;
                case 2:
                    if (json != null)
                    {
                        int sec = int.Parse(json["sec"]);
                        back = loginAfterSec(sec);
                    }
                    break;
                case 3:
                    if (json != null)
                    {
                        int sec = int.Parse(json["window_name"]);
                        back = loginAfterSec(sec);
                    }
                    break;
                default:
                    break;
            }
            return back;
        }

        private string loginAfterSec(int sec)
        {
            Thread.Sleep(sec * 1000);
            return Win32Api.getInstance().FindWindow("四人帮").ToString();
        }

        private static IntPtr Hwnd;
        private static Regex WndNameRegex;
        private string findWindowHwnd(string name)
        {
            Hwnd = IntPtr.Zero;
            WndNameRegex = new Regex(name);
            Win32Api.EnumWindows(onEnumWindow, 0);
            return Hwnd.ToString();
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
                Hwnd = hWnd;
            }
            return result;
        }
    }
}
