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
    class ConvsationManager
    {

        private HashSet<Socket> ConvsationSockets = new HashSet<Socket>();

        private static ConvsationManager instance;
        private static object locker = new object();


        public static ConvsationManager getInstance()
        {
            if(instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new ConvsationManager();
                    }
                }
            }
            return instance;
        }

        public void addSocket(Socket socket)
        {
            ConvsationSockets.Add(socket);
        }

        public int broadcast(Socket sender, string noteRequest)
        {
            if (!ConvsationSockets.Contains(sender))
            {
                ConvsationSockets.Add(sender);
            }
            HashSet<Socket>.Enumerator en = ConvsationSockets.GetEnumerator();
            ConvsationSockets.RemoveWhere(socket => (socket == null || !socket.Connected));
            JSONClass response = new JSONClass();
            response.Add(ResponseKey.Code, "0");
            response.Add(ResponseKey.Data, noteRequest);
            string responseStr = response.ToString();
            foreach (Socket item in ConvsationSockets)
            {
                if(item != sender)
                {
                    item.Send(Encoding.UTF8.GetBytes(responseStr));
                }
            }
            return ConvsationSockets.Count() - 1;
        }

        public int clientCount()
        {
            return ConvsationSockets.Count();
        }

        public void removeSocket(Socket socket)
        {
            ConvsationSockets.Remove(socket);
        }
    }
}
