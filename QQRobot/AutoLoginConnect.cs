using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace QQRobot
{
    class AutoLoginConnect
    {
        private static AutoLoginConnect instance;

        public static AutoLoginConnect getInstance()
        {
            if (instance == null)
            {
                instance = new AutoLoginConnect();
            }
            return instance;
        }
        
        public void connect()
        {
            //设定服务器IP地址  
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(new IPEndPoint(ip, 19190)); //配置服务器IP与端口  
                Console.WriteLine("连接服务器成功");
            }
            catch
            {
                Console.WriteLine("连接服务器失败，请按回车键退出！");
                return;
            }
            try
            {
                var node = new JSONClass();
                node["command"].AsInt = 2;
                node["arg_sec"].AsInt = 90;
                clientSocket.Send(Encoding.UTF8.GetBytes(node.ToString()));
            }
            finally
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            Console.WriteLine("发送完毕，按回车键退出");
            Console.ReadLine();
        }
    }
}
