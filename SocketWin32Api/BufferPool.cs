using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketWin32Api
{
    public class BufferPool
    {
        private byte[][] coreBuffers;
        private HashSet<byte[]> usingSet = new HashSet<byte[]>();
        private int size;
        private static object lockObj = new object();
        public BufferPool(int count,int size)
        {
            coreBuffers = new byte[count][];
            this.size = size;
        }

        public byte[] borrow()
        {
            byte[] result = null;
            lock (lockObj)
            {
                if (usingSet.Count <= coreBuffers.Length)
                {
                    for (int i = 0; i < coreBuffers.Length; i++)
                    {
                        if (coreBuffers[i] == null)
                        {
                            coreBuffers[i] = new byte[size];
                            usingSet.Add(coreBuffers[i]);
                            result = coreBuffers[i];
                            break;
                        }
                        else if (!usingSet.Contains(coreBuffers[i]))
                        {
                            result = coreBuffers[i];
                            break;
                        }
                    }
                }
            }
            
            return result;
        }

        public void giveBack(byte[] obj)
        {
            usingSet.Remove(obj);
        }       
    }
}
