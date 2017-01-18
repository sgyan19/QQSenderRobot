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
        public BufferPool(int count,int size)
        {
            coreBuffers = new byte[count][];
            this.size = size;
        }

        public byte[] borrow()
        {
            if(usingSet.Count > coreBuffers.Length)
            {
                return null;
            }
            for(int i = 0; i < coreBuffers.Length; i ++)
            {
                if(coreBuffers[i] == null)
                {
                    coreBuffers[i] = new byte[size];
                    return coreBuffers[i];
                }
                if (!usingSet.Contains(coreBuffers[i]))
                {
                    return coreBuffers[i];
                }
            }
            return null;
        }

        public void giveBack(byte[] obj)
        {
            usingSet.Remove(obj);
        }       
    }
}
