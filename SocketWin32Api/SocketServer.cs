using SimpleJSON;
using SocketWin32Api.Define;
using System;
using System.Collections;
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
        private static byte[] buffer = new byte[Config.SocketBufferSize];

        private Hashtable mSocketMap = new Hashtable();

        public void start(int port = 19190)
        {
            RunningFlag = 1;
            mPort = port;
            socketListenWork();
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
                mServerSocket.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), mPort));
                mServerSocket.Listen(10);
                mListenerTask.Start();
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
                //clientSocket.Send(Encoding.UTF8.GetBytes("Server Say Hello"));
                Task.Factory.StartNew(new Action<object>(socketReceiveWork), clientSocket);
            }
        }

        private void socketReceiveWork(object accept)
        {
            Socket s = accept as Socket;
            string json = "";
            try
            {
                while (true)
                {
                    //通过clientSocket接收数据  
                    int receiveNumber = s.Receive(buffer);
                    json = Encoding.UTF8.GetString(buffer, 0, receiveNumber);

                    int code = -1;
                    string[] args;
                    string requestId;
                    try
                    {
                        JSONNode node = JSON.Parse(json);
                        code = int.Parse(node[RequestKey.Code]);
                        args = (node[RequestKey.Args] as JSONArray).Childs.Select((m) => ((string)m)).ToArray();
                        requestId = node[RequestKey.RequestId];
                        if (requestId == null) requestId = "";
                    }
                    catch
                    {
                        args = new string[0];
                        requestId = "";
                    }
                    if(code == (int)RequestCode.DisConnect)
                    {
                        break;
                    }
                    string end;
                    try
                    {
                        end = handleCommand(s, json, code, args);
                    }
                    catch (Exception ex)
                    {
                        end = ex.Message;
                        code = 1;
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                    JSONClass response = new JSONClass();
                    response.Add(ResponseKey.Code, "0");
                    response.Add(ResponseKey.Data, end);
                    response.Add(ResponseKey.RequestId, requestId);
                    s.Send(Encoding.UTF8.GetBytes(response.ToString()));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                s.Shutdown(SocketShutdown.Both);
                s.Disconnect(true);
                s.Close();
            }

        }

        private string handleCommand(Socket socket, string request, int code, params string[] args)
        {
            string back = "success";
            switch (code)
            {
                case (int)RequestCode.RunCmd:
                    back = runCmd(args[0], args[1]);
                    break;
                case (int)RequestCode.FindWindow:
                    back = findWindowHwnd(args[0]);
                    break;
                case (int)RequestCode.SendWindowInfo:
                    back = PasteToWindow((IntPtr)int.Parse(args[0]), args[1]);
                    break;
                case (int)RequestCode.RemoteFindWindow:
                    var socketClient = getConnectedSocketClient(args[0], args[1]);
                    back = socketClient.remoteFindWindow(args[2]).ToString();
                    break;
                case (int)RequestCode.ConversationLongLink:
                    ConvsationManager.getInstance().addSocket(socket);
                    break;
                case (int)RequestCode.ConversationNote:
                    ConvsationManager.getInstance().broadcast(socket, request);
                    back = request;
                    break;
                default:
                    break;
            }
            return back;
        }

        private string runCmd(string exe, string args)
        {
            return Win32Api.getInstance().RunCmd(exe, args) ? "success" : "failed";
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

        private string PasteToWindow(IntPtr hwnd,string text)
        {
            //WindowSendApi.WindowPasteln(hwnd);
            //WindowSendApi.WindowsSend(hwnd, "哈哈");
            WindowSendApi.WindowsSend(hwnd,text);
            WindowSendApi.WindowSumbit(hwnd);
            //WindowSendApi.WindowPaste(hwnd);
            //WindowSendApi.WindowSumbit(hwnd);
            return "success";
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

        private SocketClient getConnectedSocketClient(string ip, string port)
        {
            string addr = ip + ":" + port;
            var socket = mSocketMap.ContainsKey(addr) ? (SocketClient)mSocketMap[addr] : null;
            if (socket == null)
            {
                socket = new SocketClient();
                socket.connect(ip, int.Parse(port));
                mSocketMap.Add(addr, socket);
            }
            else if (!socket.isConnected())
            {
                socket.Close();
                socket.connect(ip, int.Parse(port));
                mSocketMap.Add(addr, socket);
            }
            return socket;
        }
    }
}
