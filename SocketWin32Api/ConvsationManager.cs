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
    class ConvsationManager
    {
        private const int ConvsationCacheCount = 10;

        private LinkedList<ConvsationRequest> convastionCaches = new LinkedList<ConvsationRequest>();

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

        public int broadcast(Socket sender, string response)
        {
            if (!ConvsationSockets.Contains(sender))
            {
                ConvsationSockets.Add(sender);
            }
            HashSet<Socket>.Enumerator en = ConvsationSockets.GetEnumerator();
            ConvsationSockets.RemoveWhere(socket => (socket == null || !socket.Connected));
            foreach (Socket item in ConvsationSockets)
            {
                if(item != sender)
                {
                    item.Send(HeaderCode.BYTES_JSON);
                    SocketHelper.sendTextFrame(item, response);
                }
            }
            return ConvsationSockets.Count() - 1;
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

        public void saveConvsationCache(string id, string cvs)
        {
            convastionCaches.AddLast(new ConvsationRequest
            {
                Id = id,
                Content = cvs
            });
            int s = convastionCaches.Count - ConvsationCacheCount;
            for (int i = 0; i < s; i++)
            {
                convastionCaches.RemoveFirst();
            }
        }

        public List<string> getNewConvasation(string id, bool defaultAll)
        {
            List<string> result = new List<string>();
            bool hasFind = false;
            foreach (ConvsationRequest item in convastionCaches)
            {
                if (hasFind)
                {
                    result.Add(item.Content);
                }
                else if(item.Id.Equals(id))
                {
                    hasFind = true;
                }
            }
            if (defaultAll && !hasFind)
            {
                result.AddRange(convastionCaches.Select((m) => (m.Content)));
                //result.AddRange();
            }
            return result;
        }
    }

    class ConvsationRequest
    {
        public string Id { set; get; }
        public string Content { set; get; }
    }
}
