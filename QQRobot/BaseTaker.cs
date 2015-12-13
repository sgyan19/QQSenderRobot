using BlackRain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQRobot
{
    abstract class BaseTaker
    {
        protected PageRequest request = new PageRequest();
        public int Interval { set; get; }

        protected Weibo[] lastTake;

        public Weibo[] Take()
        {
            string html = takePage();
            Weibo[] newObjs = paser(html);
            return checkNew(newObjs, lastTake);
        }

        public abstract string takePage();

        public abstract Weibo[] paser(string html);

        public Weibo[] checkNew(Weibo[] newTakeData)
        {
            Weibo[] result = checkNew(newTakeData, lastTake);
            lastTake = newTakeData;
            return result;
        }

        public Weibo[] getLastTake()
        {
            return lastTake;
        }

        public abstract Weibo[] checkNew(Weibo[] newTakeData,Weibo[] oldTakeData);
    }
}
