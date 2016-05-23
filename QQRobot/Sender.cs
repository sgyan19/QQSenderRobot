using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQRobot
{
    /// <summary>
    /// 发送基类，后期可能提供发送微博等功能，预留基类便于扩展
    /// </summary>
    abstract class Sender
    {
        public abstract void send(string msg, Image[] imgs);
        public delegate void Send(string msg, Image[] imgs);
    }
}
