using log4net;
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
        private LogHelper mLogHelper = new LogHelper();

        public void setLoger(ILog log)
        {
            mLogHelper.CoreLog = log;
        }

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
                mLogHelper.InfoFormat("accept addr:{0}", ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString());
                Task.Factory.StartNew(new Action<object>(socketReceiveWork), clientSocket);
            }
        }

        private void socketReceiveWork(object accept)
        {
            Socket s = accept as Socket;
            string json = "";
            Request request = new Request();
            request.DeviceId = "";
            try
            {
                while (true)
                {
                    //通过clientSocket接收数据  
                    int receiveNumber = s.Receive(buffer);
                    json = Encoding.UTF8.GetString(buffer, 0, receiveNumber);
                    mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, Receive:{2}, ByteCount:{3}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, json, receiveNumber);
                    if (receiveNumber <= 0)
                    {
                        break;
                    }
                    request.Code = -1;
                    try
                    {
                        JSONNode node = JSON.Parse(json);
                        request.Code = int.Parse(node[RequestKey.Code]);
                        request.Args = (node[RequestKey.Args] as JSONArray).Childs.Select((m) => ((string)m)).ToArray();
                        request.RequestId = node[RequestKey.RequestId];
                        if (request.RequestId == null) request.RequestId = "";
                        string deviceId = node[RequestKey.DeviceId];
                        if (!string.IsNullOrEmpty(deviceId))
                        {
                            request.DeviceId = deviceId;
                        }
                    }
                    catch
                    {
                        request.Args = new string[0];
                    }
                    if(request.Code == (int)RequestCode.DisConnect)
                    {
                        break;
                    }
                    string end;
                    try
                    {
                        end = handleCommand(s, json, request);
                    }
                    catch (Exception ex)
                    {
                        end = ex.Message;
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                    if (end != null)
                    { 
                        JSONClass response = new JSONClass();
                        response.Add(ResponseKey.Code, "0");
                        response.Add(ResponseKey.Data, end);
                        response.Add(ResponseKey.RequestId, request.RequestId);
                        string responseJson = response.ToString();
                        mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, response:{2}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, responseJson);
                        s.Send(Encoding.UTF8.GetBytes(responseJson));
                    }
                    else
                    {
                        mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, no response", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId);
                    }
                }
            }
            catch (Exception ex)
            {
                mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, while Receivere Exception:{2}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, ex.Message);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, socket close", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId);
                s.Shutdown(SocketShutdown.Both);
                s.Disconnect(true);
                s.Close();
            }

        }

        private string handleCommand(Socket socket, string requestStr, Request request)
        {
            string back = "success";
            switch (request.Code)
            {
                case (int)RequestCode.RunCmd:
                    back = runCmd(request.Args[0], request.Args[1]);
                    break;
                case (int)RequestCode.FindWindow:
                    back = findWindowHwnd(request.Args[0]);
                    break;
                case (int)RequestCode.SendWindowInfo:
                    back = PasteToWindow((IntPtr)int.Parse(request.Args[0]), request.Args[1]);
                    break;
                case (int)RequestCode.RemoteFindWindow:
                    var socketClient = getConnectedSocketClient(request.Args[0], request.Args[1]);
                    back = socketClient.remoteFindWindow(request.Args[2]).ToString();
                    break;
                case (int)RequestCode.ConversationLongLink:
                    ConvsationManager.getInstance().addSocket(socket);
                    mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, Conversation Link", ((IPEndPoint)socket.RemoteEndPoint).Address.ToString(),request.DeviceId);
                    break;
                case (int)RequestCode.ConversationNote:
                    int count = ConvsationManager.getInstance().broadcast(socket, requestStr);
                    mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, Conversation broadcast:{2}", ((IPEndPoint)socket.RemoteEndPoint).Address.ToString(), request.DeviceId, count);
                    back = requestStr;
                    break;
                default:
                    back = null;
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
