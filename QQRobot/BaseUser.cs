using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQRobot
{
    public class BaseUser
    {
        public string UserName { set; get; }
        public string UserId { set; get; }
        public string UserHeaderUri { set; get; }
        public string Source { set; get; }
        public Image UserHeader { set; get; }
    }
}
