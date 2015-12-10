using BlackRain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace QQRobot
{
    class XinyusizhiguangTaker : BaseTaker
    {
        public const string PageUrl = "http://weibo.com/u/{0}";
        public const string WeiboItemTemplet = "<div class=\\\\\\\"WB_detail\\\\\\\">(?<item>[\\s\\S]*?)<a class=\\\\\\\"S_txt2\\\\\\\"";
        public const string WeiboTextTemplet = "<div class=\\\\\\\"WB_text W_f14\\\\\\\"[\\s\\S]*?>\\\\n[\\s]*?(?<content>[\\s\\S]*?)<\\\\/div>";
        public const string WeiboLabelTemplet = "<[\\s\\S]*?>";
        public const string WeiboFilterTemplet = "[ \\\\/]";
        public const string WeiboImgTemplet = "<img[\\s\\S]*?src=\\\\\\\"(?<url>[\\s\\S]*?)\\\\\\\"[\\s\\S]*?>";
        
        private Regex mWeiboItemReg = new Regex(WeiboItemTemplet);
        private string mWeiboItemGoups = "item";
        private Regex mWeiboTextReg = new Regex(WeiboTextTemplet);
        private string mWeiboTextGoups = "content";
        private Regex mWeiboImgReg = new Regex(WeiboImgTemplet);
        private string mWeiboImgGoups = "url";
        private Regex mWeiboLabelReg = new Regex(WeiboLabelTemplet);
        private Regex mWeiboFilterReg = new Regex(WeiboFilterTemplet);
        private string cookie;
        private string uid;
        public void setCookie(string cookie)
        {
            this.cookie = cookie;
        }

        public void setInterval(string interval)
        {
            Interval = int.Parse(interval);
            if (Interval < 5)
                Interval = 5;
        }

        public void setUid(string uid)
        {
            this.uid = uid;
        }

        public XinyusizhiguangTaker()
        {
            Interval = 30;
        }

        public override string takePage()
        {
            return request.GetData(String.Format(PageUrl, uid), cookie, "http://weibo.com");
        }

        public override Weibo[] paser(string html)
        {
            Weibo[] weibos = null;
            MatchCollection mathes = mWeiboItemReg.Matches(html);
            string[] weiboHtmls = new string[mathes.Count];
            if (mathes.Count > 0)
            {
                int matchIndex = 0;
                foreach (Match m in mathes)
                {
                    weiboHtmls[matchIndex] = m.Groups[mWeiboItemGoups].Value;
                    matchIndex++;
                }
            }
            weibos = new Weibo[weiboHtmls.Length];
            for (int i = 0; i < weiboHtmls.Length; i++)
            {
                weibos[i] = paserItem(weiboHtmls[i]);
            }
            weibos = deleteTop(weibos);
            return weibos;
        }

        public override Weibo[] checkNew(Weibo[] newTakeData, Weibo[] oldTakeData)
        {
            Weibo[] result = null;
            LinkedList<Weibo> news = new LinkedList<Weibo>();
            if (oldTakeData != null && oldTakeData.Length > 0 && newTakeData.Length > 0)
            {
                for (int i = 0; i < newTakeData.Length; i++)
                {
                    if (newTakeData[i].Text == null || newTakeData[i].Text.Equals(oldTakeData[0].Text))
                    {
                        break;
                    }
                    news.AddFirst(newTakeData[i]);
                }
            }
            if (news.Count == 0)
            {
                result = new Weibo[0];
            }
            else
            {
                result = new Weibo[news.Count];
                int i = 0;
                foreach(Weibo weibo in news)
                {
                    result[i++] = weibo;
                }
            }
            return result;
        }

        private Weibo paserItem(string weiboHtml)
        {
            Weibo weibo = new Weibo();
            Match textMatch = mWeiboTextReg.Match(weiboHtml);
            if (textMatch.Success)
            {
                weibo.Text = mWeiboFilterReg.Replace(mWeiboLabelReg.Replace(textMatch.Groups[mWeiboTextGoups].Value, ""), "") ;
            }

            MatchCollection imgMathes = mWeiboImgReg.Matches(weiboHtml);

            string[] imgUrls = new string[imgMathes.Count];
            weibo.ImgUrls = imgUrls;
            int i = 0;
            foreach (Match imgMatch in imgMathes)
            {
                imgUrls[i++] = imgMatch.Groups[mWeiboImgGoups].Value.Replace("\\", "").Replace("thumbnail", "bmiddle");
            }
            return weibo;
        }

        private Weibo[] deleteTop(Weibo[] data)
        {
            if(data.Length < 1)
            {
                return data;
            }
            Weibo[] newWeibos = new Weibo[data.Length - 1];
            for (int i = 1; i < data.Length; i++)
            {
                newWeibos[i - 1] = data[i];
            }
            return newWeibos;
        }
    }
}
