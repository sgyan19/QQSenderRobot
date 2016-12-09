using SimpleJSON;
using SocketWin32Api.Define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketWin32Api
{
    public class SocketClient
    {
        private static byte[] buffer = new byte[1024];
        private Socket mSocket;
        private Exception mLastException;
        public bool connect(string ip = "127.0.0.1", int port = (int)Port.Service)
        {
            try
            {
                mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                mSocket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
            }
            catch(Exception e)
            {
                mLastException = e;
                return false;
            }
            return true;
        }

        private void request(string code, ref string bakeCode, ref string backData,params string[] args)
        {
            JSONClass request = new JSONClass();
            request.Add(RequestKey.Code, code);
            JSONArray array = new JSONArray();
            foreach (var item in args)
            {
                array.Add(item);
            }
            request.Add(RequestKey.Args, array);
            mSocket.Send(Encoding.UTF8.GetBytes(request.ToString()));
            int receiveNumber = mSocket.Receive(buffer);
            JSONClass response = JSON.Parse(Encoding.UTF8.GetString(buffer, 0, receiveNumber)) as JSONClass;
            bakeCode = response[ResponseKey.Code];
            backData = response[ResponseKey.Data];
        }

        public IntPtr remoteFindWindow(string window)
        {
            IntPtr rlt = IntPtr.Zero;
            try
            {
                string code = "";
                string data = "";
                request(((int)RequestCode.FindWindow).ToString(), ref code, ref data, window);
                rlt = (IntPtr)int.Parse(data);
                //mSocket.Send()
            }
            catch(Exception e)
            {
                mLastException = e;
            }
            return rlt;
        }

        public bool isConnected()
        {
            if (mSocket != null)
            {
                return mSocket.Connected;
            }
            return false;
        }

        public void Close()
        {
            if(mSocket != null)
            {
                try
                {
                    mSocket.Shutdown(SocketShutdown.Both);
                    mSocket.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    mLastException = e;
                }
            }
        }
    }
}
