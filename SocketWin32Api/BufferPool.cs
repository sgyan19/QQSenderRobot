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
                            LogHelper.getInstance().InfoFormat("BufferPool borrow new usingSet buffer:{0} Count:{1} index:{2}", result.GetHashCode(), usingSet.Count, i);
                            break;
                        }
                        else if (!usingSet.Contains(coreBuffers[i]))
                        {
                            result = coreBuffers[i];
                            usingSet.Add(coreBuffers[i]);
                            LogHelper.getInstance().InfoFormat("BufferPool borrow old usingSet buffer:{0} Count:{1} index:{2}", result.GetHashCode(), usingSet.Count, i);
                            break;
                        }
                    }
                }
            }
            
            return result;
        }

        public void giveBack(byte[] obj)
        {
            lock (lockObj)
            {
                usingSet.Remove(obj);
                LogHelper.getInstance().InfoFormat("BufferPool giveBack buffer: {0} usingSet Count:{1}", obj.GetHashCode(),usingSet.Count);
            }
        }       
    }
}
