using SimpleJSON;
using SocketWin32Api.Define;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketWin32Api
{
    public class SocketHelper
    {

        public static void request(Socket socket, byte[] buffer, string code, ref string bakeCode, ref string backData, params string[] args)
        {
            JSONClass request = new JSONClass();
            request.Add(RequestKey.Code, code);
            JSONArray array = new JSONArray();
            foreach (var item in args)
            {
                array.Add(item);
            }
            request.Add(RequestKey.Args, array);
            socket.Send(Encoding.UTF8.GetBytes(request.ToString()));
            int receiveNumber = socket.Receive(buffer);
            JSONClass response = JSON.Parse(Encoding.UTF8.GetString(buffer, 0, receiveNumber)) as JSONClass;
            bakeCode = response[ResponseKey.Code];
            backData = response[ResponseKey.Data];
        }

        public static string makeResponseJson(string code, string data, string requestId)
        {
            JSONClass response = new JSONClass();
            response.Add(ResponseKey.Code, code);
            response.Add(ResponseKey.Data, data);
            response.Add(ResponseKey.RequestId, requestId);
            return response.ToString();
        }

        public static string responseJson(Socket socket, string code, string data, string requestId)
        {
            JSONClass response = new JSONClass();
            response.Add(ResponseKey.Code, code);
            response.Add(ResponseKey.Data, data);
            response.Add(ResponseKey.RequestId, requestId);
            string responseJson = response.ToString();
            socket.Send(HeaderCode.BYTES_JSON);
            sendTextFrame(socket, responseJson);
            return responseJson;
        }

        public static void sendTextFrame(Socket socket, string text)
        {
            byte[] forSend = Encoding.UTF8.GetBytes(text);
            byte[] size = BitConverter.GetBytes(forSend.Length);
            socket.Send(size);
            socket.Send(forSend);
        }

        public static void sendRawFrame(Socket socket, byte[] buf, string dir, string name)
        {
            string path = dir + "\\" + name;
            FileInfo info = new FileInfo(path);
            int size = (int)info.Length;
            socket.Send(BitConverter.GetBytes(size));
            int len;
            using (FileStream stream = new FileStream(path,
                FileMode.Open))
            {
                while ((len = stream.Read(buf, 0, buf.Length)) > 0)
                {
                    socket.Send(buf, 0, len, SocketFlags.None);
                }
                stream.Close();
            }
        }

        public static string receiveTextFrame(Socket socket, byte[] buf)
        {
            socket.ReceiveTimeout = 2000;
            int len = socket.Receive(buf, 4, SocketFlags.None);
            Int32 size = BitConverter.ToInt32(buf, 0);
            if (size > buf.Length)
            {
                throw new SocketException();
            }
            int offset = 0;
            while ((len = socket.Receive(buf, offset, size, SocketFlags.None)) > 0)
            {
                offset += len;
                size = size - len;
            }
            socket.ReceiveTimeout = -1;
            return Encoding.UTF8.GetString(buf, 0, offset);
        }

        public static int receiveRawFrame(Socket socket, byte[] buf, string dirPath, string name = null)
        {
            socket.ReceiveTimeout = 2000;
            int len = socket.Receive(buf, 4, SocketFlags.None);
            Int32 size = BitConverter.ToInt32(buf, 0);
            int result = size;
            if (string.IsNullOrEmpty(name))
            {
                name = "server." + DateTime.UtcNow.ToString();
            }
            string path = dirPath + "\\" + name;
            using (FileStream stream = new FileStream(path,
                FileMode.Create))
            {
                while (size > 0)
                {
                    len = socket.Receive(buf, 0, size > buf.Length ? buf.Length : size, SocketFlags.None);
                    if (len == 0) break;
                    size = size - len;
                    stream.Write(buf, 0, len);
                }
                stream.Close();
            }
            socket.ReceiveTimeout = -1;
            return result;
        }
    }
}
