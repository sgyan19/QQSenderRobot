using log4net;
using SimpleJSON;
using SocketWin32Api.Define;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel.Channels;
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
        private BufferManager mBufferManager;
        private Hashtable mSocketMap = new Hashtable();
        private LogHelper mLogHelper = new LogHelper();
        private string mRawFolder = "raw";

        public void setLoger(ILog log)
        {
            mLogHelper.CoreLog = log;
        }

        public void start(int port = 19190)
        {
            RunningFlag = 1;
            mPort = port;
            if (!Directory.Exists(mRawFolder))
            {
                Directory.CreateDirectory(mRawFolder);
            }
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
                    s.ReceiveTimeout = -1;
                    int receiveNumber = s.Receive(buffer, 1, SocketFlags.None);
                    if (receiveNumber <= 0)
                    {
                        mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, Receive: null, ByteCount:{3}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, json, receiveNumber);
                        break;
                    }

                    if (buffer[0] == HeaderCode.ASK)
                    {
                        mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, Receive: HeaderCode.ASK, ByteCount:{2}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, receiveNumber);
                        s.Send(new byte[] { HeaderCode.ANS });
                        mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, response: HeaderCode.ANS", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId);
                        continue;
                    }
                    else if (buffer[0] == HeaderCode.BIN)
                    {
                        string rawName = SocketHelper.receiveTextFrame(s, buffer);
                        int size = SocketHelper.receiveRawFrame(s, buffer, mRawFolder, rawName);
                        mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, Receive: HeaderCode.BIN, ByteCount:{2} name:{3}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, size, rawName);
                    }
                    else if(buffer[0] == HeaderCode.JSON)
                    {
                        json = SocketHelper.receiveTextFrame(s, buffer);
                        mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, Receive: HeaderCode.JSON, ByteCount:{2} json:{3}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, json);
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
                        if (request.Code == (int)RequestCode.DisConnect)
                        {
                            break;
                        }
                        string end;
                        int code = (int)ResponseCode.Success;
                        try
                        {
                            end = handleJson(s, json, request, ref code);
                        }
                        catch (Exception ex)
                        {
                            end = ex.Message;
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);
                        }
                        if (end != null)
                        {
                            string response = SocketHelper.responseJson(s, code.ToString(), end, request.RequestId);
                            mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, response:{2}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, response);
                        }
                        else
                        {
                            mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, no response", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId);
                        }
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

        private string handleCode(Socket socket, byte code)
        {
            return null;
        }

        private string handleJson(Socket socket, string requestStr, Request request, ref int code)
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
                    mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, Conversation Link", ((IPEndPoint)socket.RemoteEndPoint).Address.ToString(), request.DeviceId);
                    break;
                case (int)RequestCode.ConversationNote:
                case (int)RequestCode.ConversationNoteRing:
                    int count = ConvsationManager.getInstance().broadcast(socket, SocketHelper.makeResponseJson(((int)ResponseCode.Success).ToString(), requestStr, request.RequestId));
                    mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, Conversation broadcast:{2}", ((IPEndPoint)socket.RemoteEndPoint).Address.ToString(), request.DeviceId, count);
                    back = requestStr;
                    break;
                case (int)RequestCode.ConversationNoteImage:
                    receiveImage(socket, requestStr, request, ref code);
                    break;
                case (int)RequestCode.ConversationNoteBuffer:
                    int len = int.Parse(request.Args[0]);
                    break;
                case (int)RequestCode.ConversationDisconnect:
                    mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, Conversation unlink", ((IPEndPoint)socket.RemoteEndPoint).Address.ToString(), request.DeviceId);
                    ConvsationManager.getInstance().removeSocket(socket);
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

        private string PasteToWindow(IntPtr hwnd, string text)
        {
            //WindowSendApi.WindowPasteln(hwnd);
            //WindowSendApi.WindowsSend(hwnd, "哈哈");
            WindowSendApi.WindowsSend(hwnd, text);
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

        private string receiveImage(Socket socket, string requestStr, Request request, ref int code)
        {
            int size = -1;
            try
            {
                size = int.Parse(request.Args[0]);
            }
            catch (Exception) { }
            if (size <= 0 || size > Config.SocketBufferSize)
            {
                code = (int)ResponseCode.ErrorOverBuffer;
                return "not support image size =" + size;
            }

            String answerHeader = SocketHelper.responseJson(socket, ((int)ResponseCode.Success).ToString(), requestStr, request.RequestId);
            mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, Conversation img header size:{2} response:{3}", ((IPEndPoint)socket.RemoteEndPoint).Address.ToString(), request.DeviceId, size, answerHeader);
            int count = ConvsationManager.getInstance().broadcast(socket, SocketHelper.makeResponseJson(((int)ResponseCode.Success).ToString(), requestStr, request.RequestId));
            mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, Conversation img header size:{2} broadcast:{3}", ((IPEndPoint)socket.RemoteEndPoint).Address.ToString(), request.DeviceId, size, count);
            int len = socket.Receive(buffer);
            count = ConvsationManager.getInstance().broadcast(socket, buffer, 0, len);
            mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, Conversation img body len:{2} broadcast:{3}", ((IPEndPoint)socket.RemoteEndPoint).Address.ToString(), request.DeviceId, len, count);
            return "";
        }



        private void sendJson(Socket socket, string json)
        {

        }
    }
}
