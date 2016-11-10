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
    /// <summary>
    /// 微博抓取器核心，负责具体的依据weibo id，http请求微博页面，拉去数据。通过大量的正则匹配，分析，归纳为微博正文和图片两个部分。回调给handle进一步处理
    /// </summary>
    class WeiboTaker : BaseTaker
    {
        public const string Host = "http://weibo.com";
        public const string PageUrl = "http://weibo.com/{0}?is_all=1";
        public const string WeiboItemTemplet = "<div class=\\\\\\\"WB_feed_detail clearfix\\\\\\\"[\\s\\S]*?\\\\\\\">(?<item>[\\s\\S]*?)<div class=\\\\\\\"WB_feed_handle\\\\\\\"";
        public const string WeiboTextTemplet = "<div class=\\\\\\\"WB_text W_f14\\\\\\\"[\\s\\S]*?>\\\\n[\\s]*?(?<content>[\\s\\S]*?)<\\\\/div>";
        public const string WeiboTextFullTemplet = "<div class=\\\\\\\"WB_text W_f14\\\\\\\" node-type=\\\\\\\"feed_list_content_full\\\\\\\"[\\s\\S]*?>\\\\n[\\s]*?(?<content>[\\s\\S]*?)<\\\\/div>";
        public const string WeiboLabelTemplet = "<[\\s\\S]*?>";
        public const string WeiboFilterTemplet = "[ \\\\]";
        public const string WeiboImgTemplet = "<img[\\s\\S]*?src=\\\\\\\"(?<url>[\\s\\S]*?)\\\\\\\"[\\s\\S]*?>";
        public const string WeiboLinkTemplet = "<a[\\s\\S]*?href=\\\\\\\"(?<url>[\\S]*?)\\\\\\\"[\\s\\S]*?>(?<name>[\\s\\S]*?)<\\\\/a>";

        public const string WeiboUserTemplet = "<img usercard=\\\\\\\"[\\S]*?\\\\\\\" src=\\\\\\\"(?<url>[\\S]*?)\\\\\\\" width=\\\\\\\"[\\S]*?\\\\\\\" height=\\\\\\\"[\\S]*?\\\\\\\" alt=\\\\\\\"(?<name>[\\S]*?)\\\\\\\" class=\\\\\\\"W_face_radius\\\\\\\"";

        public const string WeiboContentImg = "ww[0-9].sinaimg.cn";

        public const string WeiboImageCropTemplet = "(crop.[\\d]{1,3}.[\\d]{1,3}.[\\d]{1,3}.[\\d]{1,3}.)[\\d]{1,3}";

        private Regex mWeiboItemReg = new Regex(WeiboItemTemplet);
        private string mWeiboItemGoups = "item";
        private Regex mWeiboTextReg = new Regex(WeiboTextTemplet);
        private Regex mWeiboTextFullReg = new Regex(WeiboTextTemplet);
        private string mWeiboTextGoups = "content";
        private Regex mWeiboImgReg = new Regex(WeiboImgTemplet);
        private string mWeiboImgGoups = "url";
        private Regex mWeiboLabelReg = new Regex(WeiboLabelTemplet);
        private Regex mWeiboFilterReg = new Regex(WeiboFilterTemplet);
        private Regex mWeiboLinkReg = new Regex(WeiboLinkTemplet);
        private Regex mWeiboUserReg = new Regex(WeiboUserTemplet);
        private Regex mWeiboImageCropReg = new Regex(WeiboImageCropTemplet);
        private string mWeiboUserImgGroups = "url";
        private string mWeiboUserNameGroups = "name";
        private string cookie;
        private string uid;
        private int topCount;
        /// <summary>
        /// 设置置顶数
        /// </summary>
        /// <param name="topCount"></param>
        public void setTopCount(string topCount)
        {
            this.topCount = int.Parse(topCount);
        }
        /// <summary>
        /// 设置cookie
        /// </summary>
        /// <param name="cookie"></param>
        public void setCookie(string cookie)
        {
            this.cookie = cookie;
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
            this.uid = uid;
        }

        public WeiboTaker()
        {
            Interval = 30;
        }
        /// <summary>
        /// 抓取html页面
        /// </summary>
        /// <returns></returns>
        public override string takePage()
        {
            return request.GetData(String.Format(PageUrl, uid), cookie, "http://weibo.com");
        }

        /// <summary>
        /// 处理html结果，生成微博对象
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public override Weibo[] paser(string html)
        {
            Weibo[] weibos = null;
            if(User == null)
            {
                paserUser(html);
            }
            MatchCollection mathes = mWeiboItemReg.Matches(html);
            string[] weiboHtmls = new string[mathes.Count];
            if (mathes.Count > 0)
            {
                int matchIndex = 0;
                foreach (Match m in mathes)
                {
                    weiboHtmls[matchIndex] = m.Groups[mWeiboItemGoups].Value.Replace("&amp;","&");
                    matchIndex++;
                }
            }
            weibos = new Weibo[weiboHtmls.Length];
            for (int i = 0; i < weiboHtmls.Length; i++)
            {
                weibos[i] = paserItem(weiboHtmls[i]);
            }
            weibos = deleteTop(topCount,weibos);
            return weibos;
        }

        private void paserUser(String html)
        {
            Match mathe = mWeiboUserReg.Match(html);
            if(mathe.Success)
            {
                User = new WeiboUser();
                User.UserId = uid;
                User.UserHeaderUri = mathe.Groups[mWeiboUserImgGroups].Value.Replace("\\", "");
                User.UserHeaderUri = mWeiboImageCropReg.Replace(User.UserHeaderUri, "${1}" + "20");
                User.UserName = mathe.Groups[mWeiboUserNameGroups].Value.Replace("\\", "");
            }
        }

        /// <summary>
        /// 对比旧数据，提取新微博
        /// </summary>
        /// <param name="newTakeData"></param>
        /// <param name="oldTakeData"></param>
        /// <returns></returns>
        public override Weibo[] checkNew(Weibo[] newTakeData, Weibo[] oldTakeData)
        {
            Weibo[] result = null;
            LinkedList<Weibo> news = new LinkedList<Weibo>();
            if (oldTakeData != null && oldTakeData.Length > 0 && newTakeData.Length > 0)
            {
                if (oldTakeData[0].Text == null) oldTakeData[0].Text = "";
                int oldLen = oldTakeData[0].Text.Length;
                for (int i = 0; i < newTakeData.Length; i++)
                {
                    if(newTakeData[i].Text == null)
                    {
                        continue;
                    }
                    int len = newTakeData[i].Text.Length > oldLen ? oldLen : newTakeData[i].Text.Length;
                    string newText = newTakeData[i].Text.Substring(0, len);
                    string oldText = oldTakeData[0].Text.Substring(0, len);
                    if (string.Equals(newText,oldText))
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

        private string paserItemText(string itemHtml)
        {
            string result = "";
            Match textMatch = mWeiboTextReg.Match(itemHtml);
            bool fullText = true;
            string url = "";
            if (textMatch.Success)
            {
                MatchCollection linkMatches = mWeiboLinkReg.Matches(textMatch.Groups[mWeiboTextGoups].Value);
                string content = textMatch.Groups[mWeiboTextGoups].Value;
                foreach (Match linkMatch in linkMatches)
                {
                    string name = mWeiboLabelReg.Replace(linkMatch.Groups["name"].Value, "");
                    if (name[0] != '@')
                    {
                        url = linkMatch.Groups["url"].Value.Replace("\\", "");
                        content = content.Replace(linkMatch.Value, name + ":[" + url + "]");
                    }
                    else
                    {
                        content = content.Replace(name, "[" + name + "]");
                    }
                    if (name.Contains("展开全文"))
                    {
                        fullText = false;
                        break;
                    }
                }
 
                content = content.Replace("O网页链接", "网页链接");
                result = mWeiboFilterReg.Replace(mWeiboLabelReg.Replace(content, ""), "");
            }
            string newResult = null;
            if (!fullText)
            {
                string newItem = null;
                try
                {
                    newItem = request.GetData(Host + url, cookie, "http://weibo.com");
                }
                catch (Exception)
                {
                }
                if (!string.IsNullOrEmpty(newItem))
                {
                    newResult = paserItemText(newItem);
                }
            }
            return string.IsNullOrEmpty(newResult) ? result : newResult;
        }

        /// <summary>
        /// 处理单条微博html，解析出正文和图片，正文替换特殊符号
        /// </summary>
        /// <param name="weiboHtml"></param>
        /// <returns></returns>
        private Weibo paserItem(string weiboHtml)
        {
            Weibo weibo = new Weibo();
            weibo.Text = paserItemText(weiboHtml);
            MatchCollection imgMathes = mWeiboImgReg.Matches(weiboHtml);

            string[] imgUrls = new string[imgMathes.Count];
            weibo.ImgUrls = imgUrls;
            int i = 0;
            foreach (Match imgMatch in imgMathes)
            {
                imgUrls[i++] = imgMatch.Groups[mWeiboImgGoups].Value.Replace("\\", "").Replace("thumbnail", "bmiddle").Replace("square", "bmiddle").Replace("orj480","mw690").Replace("thumb180", "mw690");
            }
            if(imgUrls.Length > 0 )
            {
                ArrayList newImgUrlList = new ArrayList();
                foreach(string item in imgUrls)
                {
                    Match match = Regex.Match(item, WeiboContentImg);
                    if (match.Success)
                    {
                        newImgUrlList.Add(item);
                    }
                }
                newImgUrlList.ToArray();
                string[] newImgUrls = new string[newImgUrlList.Count];
                for (int j = 0; j < newImgUrlList.Count; j++)
                {
                    newImgUrls[j] = (string)newImgUrlList[j];
                }
                weibo.ImgUrls = newImgUrls;
            }

            return weibo;
        }
        /// <summary>
        /// 去掉置顶的微博
        /// </summary>
        /// <param name="topCount">置顶</param>
        /// <param name="data">微博结果</param>
        /// <returns></returns>
        private Weibo[] deleteTop(int topCount,Weibo[] data)
        {
            if(data.Length < topCount || topCount == 0)
            {
                return data;
            }
            Weibo[] newWeibos = new Weibo[data.Length - topCount];
            for (int i = topCount; i < data.Length; i++)
            {
                newWeibos[i - topCount] = data[i];
            }
            return newWeibos;
        }
    }
}
