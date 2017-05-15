using SimpleJSON;
using SocketWin32Api.Define;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketWin32Api
{
    class MobileTerminalManager
    {
        private const int ConvsationCacheCount = 10;

        private LinkedList<string> convastionCaches = new LinkedList<string>();

        private HashSet<Socket> ConvsationSockets = new HashSet<Socket>();

        private static MobileTerminalManager instance;
        private static object locker = new object();


        public static MobileTerminalManager getInstance()
        {
            if(instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = new MobileTerminalManager();
                    }
                }
            }
            return instance;
        }

        public bool addSocket(Socket socket)
        {
            return ConvsationSockets.Add(socket);
        }

        public int broadcast(string response)
        {
            int count = 0;
            ConvsationSockets.RemoveWhere(socket => (socket == null || !socket.Connected));
            foreach (Socket item in ConvsationSockets)
            {
                if(item != null)
                {
                    try
                    {
                        item.Send(HeaderCode.BYTES_JSON);
                        SocketHelper.sendTextFrame(item, response);
                        count++;
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);
                    }
                }
            }
            saveConvsationCache(response);
            return count;
        }

        public int broadcast(Socket sender, byte[] buffer, int offset, int len)
        {
            if (!ConvsationSockets.Contains(sender))
            {
                ConvsationSockets.Add(sender);
            }
            HashSet<Socket>.Enumerator en = ConvsationSockets.GetEnumerator();
            ConvsationSockets.RemoveWhere(socket => (socket == null || !socket.Connected));
            foreach (Socket item in ConvsationSockets)
            {
                if (item != sender)
                {
                    item.Send(buffer, offset, len, SocketFlags.None);
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

        public void saveConvsationCache(string cvsJson)
        {
            convastionCaches.AddLast(cvsJson);
            int s = convastionCaches.Count - ConvsationCacheCount;
            for (int i = 0; i < s; i++)
            {
                convastionCaches.RemoveFirst();
            }
        }

        public ICollection<string> getConvastationCash()
        {
            return convastionCaches;
        }
    }

    class ConvsationRequest
    {
        public string Id { set; get; }
        public string Content { set; get; }
    }
}
