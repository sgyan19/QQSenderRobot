using SimpleJSON;
using SocketWin32Api.Define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketWin32Api
{
    public class Utils
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
    }
}
