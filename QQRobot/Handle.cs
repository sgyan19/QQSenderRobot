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
    class Handle : WeiboTakeEvent
    {
        public LinkedList<Sender> senders;

        public Loger loger;
        public Loger takeLoger;
        public int Count;
        public UiShower shower;
        public int sendCount;
        public bool ifLog;

        public void NewWeibos(Weibo[] newWeibos, Weibo[] all)
        {
            foreach(Weibo weibo in newWeibos)
            {
                Image[] imgs = new Image[weibo.ImgUrls.Length];
                for (int i = 0; i < weibo.ImgUrls.Length; i++)
                {
                    string path = download(weibo.ImgUrls[i]);
                    imgs[i] = Image.FromFile(path);
                }
                if(senders != null && senders.Count > 0)
                {
                    sendCount += senders.Count;
                    foreach (Sender sender in senders)
                    {
                        sender.send(weibo.Text, imgs);
                    }
                    shower.showCount("已发送：" + sendCount);
                }
                if(ifLog && loger != null)
                {
                    loger.log(weibo);
                }
            }

            if(newWeibos != null && newWeibos.Length > 0)
            {
                if ( ifLog && takeLoger != null )
                {
                    takeLoger.log(format(all));
                }
            }else
            {
                if ( ifLog && takeLoger != null && Count != 1 )
                {
                    takeLoger.log(formatNoChange());
                }
            }
        }

        public void TakeWeiboes(Weibo[] takeWeibos)
        {
            Count++;
            if (ifLog && Count ==1 && takeLoger != null)
            {
                takeLoger.log(format(takeWeibos));
            }
            if (shower != null)
            {
                shower.showResult(String.Format("第{0}次，{1}条",Count, takeWeibos.Length), format(takeWeibos));
            }
        }

        WebClient wb = new WebClient();

        private string download(string url)
        {
            int index = url.LastIndexOf('/');
            string name = url.Substring(index, url.Length - index) ;

            if (!Directory.Exists("tmp"))
            {
                Directory.CreateDirectory("tmp");
            }
            string path = "tmp\\" + name;
            if (!File.Exists(path))
            {
                wb.DownloadFile(url, path);
            }
            
            return path;
        }


        private string format(Exception e)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(String.Format("{0}  [第{1}次]", DateTime.Now.ToString(), Count));
            
            builder.AppendLine("    ===========================================================");
            builder.AppendLine("    " + e.Message);
            builder.AppendLine("    " + e.StackTrace);
            builder.AppendLine("");
            return builder.ToString();
        }

        private string formatNoChange()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(String.Format("{0}  [第{1}次] 无变化", DateTime.Now.ToString(), Count));
            return builder.ToString();
        }

        private string format(Weibo[] all)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(String.Format("{0}  [第{1}次]", DateTime.Now.ToString(), Count));

            foreach (Weibo weibo in all)
            {
                builder.AppendLine("    ===========================================================");
                builder.AppendLine(String.Format("    [Text]  {0}", weibo.Text));
                builder.AppendLine("    [Imgs]");
                if (weibo.ImgUrls != null && weibo.ImgUrls.Length > 0)
                {
                    for (int i = 0; i < weibo.ImgUrls.Length; i++)
                    {
                        builder.AppendLine("           "+ weibo.ImgUrls[i]);
                    }
                }
            }
            builder.AppendLine("");
            return builder.ToString();
        }

        public void OnCountDown(int secounds)
        {
            string show;
            if(secounds == 0)
            {
                show = "抓取中";
            }
            else
            {
                show = "倒计时：" + secounds;
            }
            shower.showCountDown(show);
        }

        public void OnStart()
        {
            shower.showStart();
        }

        public void OnStop()
        {
            shower.showStop();
        }

        public void OnException(Exception e)
        {
            Count++;
            string text = format(e);
            shower.showResult(String.Format("第{0}次，{1}条", Count, 0), text);
            takeLoger.log(text);
        }
    }
}
