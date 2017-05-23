using log4net;
using SimpleJSON;
using SocketWin32Api.Define;
using System;
using System.Collections;
using System.Collections.Generic;
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
        //private static byte[] buffer = new byte[Config.SocketBufferSize];
        private static BufferPool bufferPool;
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
                bufferPool = new BufferPool(10, Config.SocketBufferSize);
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
            byte[] buffer = bufferPool.borrow();
            mLogHelper.InfoFormat("addr:{0}, buffer:{1}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), buffer.GetHashCode());
            try
            {
                while (true)
                {
                    //通过clientSocket接收数据  
                    s.ReceiveTimeout = -1;
                    int receiveNumber = s.Receive(buffer, 1, SocketFlags.None);
                    if (receiveNumber <= 0)
                    {
                        mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, Receive: null, ByteCount:{2}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, receiveNumber);
                        break;
                    }

                    if (buffer[0] == HeaderCode.ASK)
                    {
                        mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, Receive: HeaderCode.ASK, ByteCount:{2}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, receiveNumber);
                        s.Send(HeaderCode.BYTES_ANS);
                        mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, response: HeaderCode.ANS", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId);
                        continue;
                    }
                    else if (buffer[0] == HeaderCode.RAW)
                    {
                        try
                        {
                            string rawName = SocketHelper.receiveTextFrame(s, buffer, mLogHelper);
                            int size = SocketHelper.receiveRawFrame(s, buffer, mRawFolder, rawName, mLogHelper);
                            string response = SocketHelper.responseJson(s, ((int)ResponseCode.Success).ToString(), "", rawName);
                            mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, Receive: HeaderCode.RAW, ByteCount:{2}, name:{3}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, size, rawName);
                            mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, response{2}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, response);
                        }
                        catch(Exception e)
                        {
                            string response = SocketHelper.responseJson(s, ((int)ResponseCode.ErrorSocketRecive).ToString(), "", format(e));
                        }
                    }
                    else if (buffer[0] == HeaderCode.CKRAW)
                    {
                        try
                        {
                            string rawName = SocketHelper.receiveTextFrame(s, buffer, mLogHelper);
                            int size = SocketHelper.receiveRawFrame(s, buffer, mLogHelper);
                            mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, Receive: HeaderCode.CKRAW, MD5 ByteCount:{2}, name:{3}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, size, rawName);
                            byte[] oldMd5 = SocketHelper.getRawMd5(s, mRawFolder, rawName);
                            bool checkRight = false;
                            if(size == oldMd5.Length)
                            {
                                checkRight = true;
                                for (int i = 0; i< size;i++)
                                {
                                    if(buffer[i] != oldMd5[i])
                                    {
                                        checkRight = false;
                                        break;
                                    }
                                }
                            }
                            if (checkRight)
                            {
                                s.Send(HeaderCode.BYTES_CK_SUC_RAW);
                                mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, Send: HeaderCode.BYTES_CK_SUC_RAW, name:{2}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, rawName);
                                string response = SocketHelper.responseJson(s, ((int)ResponseCode.Success).ToString(), "", rawName);
                                mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, response{2}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, response);
                            }
                            else
                            {
                                s.Send(HeaderCode.BYTES_CK_FAIL_RAW);
                                mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, Send: HeaderCode.BYTES_CK_FAIL_RAW,name:{2}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, rawName);
                                size = SocketHelper.receiveRawFrame(s, buffer, mRawFolder, rawName, mLogHelper);
                                string response = SocketHelper.responseJson(s, ((int)ResponseCode.Success).ToString(), "", rawName);
                                mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, receiveRawFrame: HeaderCode.BYTES_CK_FAIL_RAW, ByteCount:{2}, name:{3}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, size, rawName);
                                mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, response{2}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, response);
                            }
                        }
                        catch (Exception e)
                        {
                            mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, HeaderCode.CKRAW Exception:{2} {3}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, e.Message, e.StackTrace);
                            //string response = SocketHelper.responseJson(s, ((int)ResponseCode.ErrorSocketRecive).ToString(), "", format(e));
                        }
                    }
                    else if(buffer[0] == HeaderCode.JSON)
                    {
                        string end;
                        int code = (int)ResponseCode.Success;
                        try
                        {
                            json = SocketHelper.receiveTextFrame(s, buffer ,mLogHelper);
                        }
                        catch (Exception e)
                        {
                            code = (int)ResponseCode.ErrorSocketRecive;
                            end = SocketHelper.responseJson(s, ((int)ResponseCode.ErrorSocketRecive).ToString(), request.RequestId, format(e));
                        }
                        mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, Receive: HeaderCode.JSON, json:{2}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, json);
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
                        try
                        {
                            end = handleJson(s, buffer, json, request, request.Args.Length, ref code);
                        }
                        catch (Exception ex)
                        {
                            end = ex.StackTrace;
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);
                        }
                        if (end != null)
                        {
                            string response = SocketHelper.responseJson(s, code.ToString(), request.RequestId, end);
                            mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, response:{2}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, response);
                        }
                        else
                        {
                            mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, no response", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId);
                        }
                    }else
                    {
                        mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, unkown code {2}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, buffer[0]);
                        s.ReceiveTimeout = 3000;
                        try
                        {
                            int len;
                            while ((len = s.Receive(buffer)) >= 0)
                            {
                                mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, Receive Trash len{2}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, len);
                            }
                        }catch(Exception e)
                        {
                            mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, Receive Trash over e:{2} {3}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, e.Message, e.StackTrace);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, while Receivere Exception:{2}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, ex.Message);
                mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, while Receivere Exception:{2}", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId, ex.StackTrace);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, socket close", ((IPEndPoint)s.RemoteEndPoint).Address.ToString(), request.DeviceId);
                s.Shutdown(SocketShutdown.Both);
                s.Disconnect(true);
                s.Close();
                bufferPool.giveBack(buffer);
            }

        }

        private string handleJson(Socket socket, byte[] buffer, string requestStr, Request request,int argCount, ref int code)
        {
            string back = "success";
            switch (request.Code)
            {
                case (int)RequestCode.RunCmd:
                    if(argCount > 2)
                    {
                        back = runCmd(request.Args[0], request.Args[1]);
                    }
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
                case (int)RequestCode.MobileTerminalJson:
                    if (MobileTerminalManager.getInstance().addSocket(socket))
                    {
                        foreach(var item in MobileTerminalManager.getInstance().getConvastationCash())
                        {
                            SocketHelper.responseJson(socket, item);
                            mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, send new:{2}", ((IPEndPoint)socket.RemoteEndPoint).Address.ToString(), request.DeviceId, item);
                        }
                    }
                    if (request.Args != null && request.Args.Length >= 1)
                    {
                        int count = MobileTerminalManager.getInstance().broadcast(SocketHelper.makeResponseJson(((int)ResponseCode.Success).ToString(), request.RequestId, request.Args));
                        mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, Conversation broadcast:{2}", ((IPEndPoint)socket.RemoteEndPoint).Address.ToString(), request.DeviceId, count);
                    }

                    // just for log out
                    //string response1 = SocketHelper.responseJson(socket, code.ToString(), request.Args[0], request.RequestId, request.Args[1]);
                    //mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, response:{2}", ((IPEndPoint)socket.RemoteEndPoint).Address.ToString(), request.DeviceId, response1);
                    back = null;
                    break;
                case (int)RequestCode.MoboleTerminalRaw:
                    socket.Send(HeaderCode.BYTES_RAW);
                    if(File.Exists(mRawFolder + "\\" + request.Args[0]))
                    {
                        try
                        {
                            int size = SocketHelper.responseRaw(socket, buffer, mRawFolder, request.Args[0]);
                            mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, download image over size:{2}", ((IPEndPoint)socket.RemoteEndPoint).Address.ToString(), request.DeviceId, size);
                            back = null;
                        }
                        catch (Exception e)
                        {
                            back = format(e);
                            code = (int)ResponseCode.ErrorRawSend;
                            mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, download image exception:{2}", ((IPEndPoint)socket.RemoteEndPoint).Address.ToString(), request.DeviceId, back);
                        }
                        mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, download image suc", ((IPEndPoint)socket.RemoteEndPoint).Address.ToString(), request.DeviceId);
                    }
                    else
                    {
                        code = (int)ResponseCode.ErrorRawNotExist;
                        back = "server don not find file:" + request.Args[0];
                        mLogHelper.InfoFormat("addr:{0}, deviceId:{1}, download image request server don not find file", ((IPEndPoint)socket.RemoteEndPoint).Address.ToString(), request.DeviceId);
                    }
                    if(back != null)
                    {
                        SocketHelper.sendTextFrame(socket, request.Args[0]);
                        SocketHelper.sendTextFrame(socket, back);
                        back = null;
                    }
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

        private string format(Exception e)
        {
            return string.Format("{0}:{1}", e.Message, e.StackTrace);
        }
    }
}
