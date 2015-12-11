using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQRobot
{
    abstract class BaseSender
    {
        public void threadSend(string msg, Image[] imgs)
        {

        }

        public abstract void send();
        private delegate void Send(string msg, Image[] imgs);


        private delegate void Heartbeat();
        public abstract void heartbeat();
    }
}
