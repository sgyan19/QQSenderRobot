using BlackRain;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QQRobot
{
    public abstract class BaseTaker
    {
        protected PageRequest request = new PageRequest();
        public int Interval { set; get; }
        public string Proxy { set; get; }
        public string Cookie { set; get; }
        public string Uid { set; get; }
        public int TopCount { set; get; }
        protected BaseData[] lastTake;
        protected WebClient wb = new WebClient(); // IE控件，用于下载微博图片
        public BaseUser User { set; get; }

        public static BaseTaker factory(string name)
        {
            BaseTaker taker = null;
            if (string.Equals(name, "weibo"))
            {
                taker = new WeiboTaker();
            }
            else if (string.Equals(name, "twitter"))
            {
                taker = new TwitterTaker();
            }
            return taker;
        }

        public BaseData[] Take()
        {
            string html = takePage();
            if (User == null)
            {
                paserUser(html);
                downloadUserHeader(User);
            }

            BaseData[] newObjs = paser(html);
            foreach (BaseData item in newObjs)
            {
                if(newObjs != null)
                {
                    item.Taker = this;
                }
            }
            return checkNew(newObjs, lastTake);
        }

        /// <summary>
        /// 设置置顶数
        /// </summary>
        /// <param name="topCount"></param>
        public void setTopCount(string topCount)
        {
            TopCount = int.Parse(topCount);
        }
        /// <summary>
        /// 设置cookie
        /// </summary>
        /// <param name="cookie"></param>
        public void setCookie(string cookie)
        {
            Cookie = cookie;
        }
        /// <summary>
        /// 保险值，超过多少条微博，判断为对比异常。
        /// </summary>
        /// <param name="interval"></param>
        public void setInterval(string interval)
        {
            Interval = int.Parse(interval);
            if (Interval < 5)
                Interval = 5;
        }

        /// <summary>
        /// 设置微博用户id
        /// </summary>
        /// <param name="uid"></param>
        public void setUid(string uid)
        {
            Uid = uid;
        }

        public void setProxy(string proxy)
        {
            try
            {
                wb.Proxy = new WebProxy(proxy);
                Proxy = proxy;
            }
            catch (Exception)
            {
            }
        }

        public abstract string takePage();

        public abstract BaseUser paserUser(string html);

        public abstract BaseData[] paser(string html);

        public BaseData[] checkNew(BaseData[] newTakeData)
        {
            BaseData[] result = checkNew(newTakeData, lastTake);
            if (lastTake == null)
                lastTake = newTakeData;
            if (newTakeData != null && newTakeData.Length > 0) lastTake = newTakeData;
            return result;
        }

        public BaseData[] getLastTake()
        {
            return lastTake;
        }

        public abstract BaseData[] checkNew(BaseData[] newTakeData,BaseData[] oldTakeData);

        public virtual string getTakerName()
        {
            return "未知";
        }

        public virtual Image makeLongImage(BaseData data)
        {
            Image longImage = null;
            if (data.ImgUrls != null && data.ImgUrls.Length > 0)
            {
                Image[] imgs = new Image[data.ImgUrls.Length];
                for (int i = 0; i < data.ImgUrls.Length; i++)
                {
                    imgs[i] = ImageHelper.download(wb, data.ImgUrls[i], 3);
                }
                longImage = ImageHelper.longImageMake(imgs);
                data.LongImgPath = ImageHelper.save(longImage);
            }
            return longImage;
        }

        public virtual void downloadUserHeader(BaseUser user)
        {
            if (user != null)
            {
                if (user.UserHeader == null)
                {
                    string path = ImageHelper.download(wb, user.UserHeaderUri);
                    if (path != null)
                    {
                        try
                        {
                            user.UserHeader = Image.FromFile(path);
                            user.UserHeader = ImageHelper.changeSize(user.UserHeader, 20, 20);
                        }
                        catch (Exception)
                        {
                            File.Delete(path);
                        }

                    }
                }
            }
        }
    }
}
