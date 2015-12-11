using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQRobot
{
    abstract class Sender
    {
        public abstract void send(string msg, Image[] imgs);
        public delegate void Send(string msg, Image[] imgs);
    }
}
