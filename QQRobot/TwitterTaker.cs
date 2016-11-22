using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QQRobot
{
    class TwitterTaker : BaseTaker
    {
        public const string Host = "https://twitter.com/";
        public const string PageUrl = "https://twitter.com/{0}";

        private const string UserTemplet = "<img class=\\\"ProfileAvatar-image \\\" src=\\\"(?<url>[\\S]*?)\\\" alt=\\\"(?<name>[\\S]*?)\\\">";
        private const string ItemTemplet = "<div class=\\\"tweet js-stream-tweet js-actionable-tweet(?<item>[\\s\\S]*?)<div class=\\\"stream-item-footer\\\">";
        private const string ItemTextTemplet = "<p class=\\\"TweetTextSize TweetTextSize--[\\d]{1,3}px js-tweet-text tweet-text\\\" lang=\\\"[a-z]{1,3}\\\" data-aria-label-part=\\\"[\\d]\\\">(?<content>[\\s\\S]*?)</p>";
        private const string ItemImgTemplet = "<img data-aria-label-part src=\\\"(?<url>[\\S]*?)\\\" alt=";
        private const string ItemLinkTemplet = "<a[\\s\\S]*?href=\\\"(?<url>[\\S]*?)\\\"[\\s\\S]*?>[\\s]*?(?<name>[\\S]*?)[\\s]*?</a>";
        private const string HtmlLabel1Templet = "<[\\s\\S]*?/>";
        private const string HtmlLabel2Templet = "<[\\s\\S]*?>[\\s\\S]*?</[\\s\\S]*?>";

        private Regex mUserReg = new Regex(UserTemplet);
        private string mUserImgGroups = "url";
        private string mUserNameGroups = "name";
        private Regex mItemReg = new Regex(ItemTemplet);
        private string mItemGroups = "item";
        private Regex mItemTextReg = new Regex(ItemTextTemplet);
        private string mTextContentGroups = "content";
        private Regex mItemImgsReg = new Regex(ItemImgTemplet);
        private string mImgUrlGroups = "url";
        private Regex mItemLinkReg = new Regex(ItemLinkTemplet);
        private string mLinkUrlGroups = "url";
        private string mLinkNameGroups = "name";
        private Regex mHtmlLabel1Reg = new Regex(HtmlLabel1Templet);
        private Regex mHtmlLabel2Reg = new Regex(HtmlLabel2Templet);

        public override BaseData[] checkNew(BaseData[] newTakeData, BaseData[] oldTakeData)
        {
            BaseData[] result = null;
            LinkedList<BaseData> news = new LinkedList<BaseData>();
            if (oldTakeData != null && oldTakeData.Length > 0 && newTakeData.Length > 0)
            {
                if (oldTakeData[0].ImgUrls == null) oldTakeData[0].ImgUrls = new String[0];
                if (oldTakeData[0].Text == null) oldTakeData[0].Text = "";
                int oldLen = oldTakeData[0].Text.Length;
                for (int i = 0; i < newTakeData.Length; i++)
                {
                    if (newTakeData[i].Text == null)
                    {
                        continue;
                    }
                    int len = newTakeData[i].Text.Length > oldLen ? oldLen : newTakeData[i].Text.Length;
                    string newText = newTakeData[i].Text.Substring(0, len);
                    string oldText = oldTakeData[0].Text.Substring(0, len);
                    if (string.Equals(newText, oldText))
                    {
                        break;
                    }
                    else
                    {
                        if (newTakeData[i].ImgUrls == null) newTakeData[i].ImgUrls = new String[0];
                        if (oldTakeData[i].ImgUrls.Length == newTakeData[i].ImgUrls.Length)
                        {
                            bool same = true;
                            for (int j = 0; j < oldTakeData[i].ImgUrls.Length; j++)
                            {
                                if (!String.Equals(oldTakeData[0].ImgUrls[j], newTakeData[i].ImgUrls[j]))
                                {
                                    same = false;
                                    break;
                                }
                            }
                            if (same && len > 10)
                            {
                                len = 10;
                                newText = newTakeData[i].Text.Substring(0, len);
                                oldText = oldTakeData[0].Text.Substring(0, len);
                                if (string.Equals(newText, oldText))
                                {
                                    break;
                                }
                            }
                        }

                    }
                    news.AddFirst(newTakeData[i]);
                }
            }
            if (news.Count == 0)
            {
                result = new BaseData[0];
            }
            else
            {
                result = new BaseData[news.Count];
                int i = 0;
                foreach (BaseData weibo in news)
                {
                    result[i++] = weibo;
                }
            }
            return result;
        }

        public override BaseData[] paser(string html)
        {
            MatchCollection mathes = mItemReg.Matches(html);
            string[] itemHtmls = new string[mathes.Count];
            if (mathes.Count > 0)
            {
                int matchIndex = 0;
                foreach (Match m in mathes)
                {
                    itemHtmls[matchIndex] = m.Groups[mItemGroups].Value.Replace("&amp;", "&").Replace("&nbsp;"," ");
                    matchIndex++;
                }
            }
            BaseData[] twitters = null;
            twitters = new BaseData[itemHtmls.Length];
            for (int i = 0; i < itemHtmls.Length; i++)
            {
                twitters[i] = paserItem(itemHtmls[i]);
            }
            twitters = deleteTop(TopCount, twitters);
            return twitters;
        }

        public override string takePage()
        {
            return request.GetData(String.Format(PageUrl, Uid), Cookie, Host, Proxy);
        }

        /// <summary>
        /// 整页html提取用户信息，头像，昵称
        /// </summary>
        /// <param name="html"></param>
        public override BaseUser paserUser(String html)
        {
            Match mathe = mUserReg.Match(html);
            if (mathe.Success)
            {
                User = new BaseUser();
                User.UserId = Uid;
                User.UserHeaderUri = mathe.Groups[mUserImgGroups].Value.Replace("\\", "").Replace("400x400","normal");
                User.UserName = mathe.Groups[mUserNameGroups].Value.Replace("\\", "");
                User.Source = getTakerName();
            }
            return User;
        }

        /// <summary>
        /// 处理单条微博html，解析出正文和图片，正文替换特殊符号
        /// </summary>
        /// <param name="twitterHtml"></param>
        /// <returns></returns>
        private BaseData paserItem(string twitterHtml)
        {
            BaseData twitter = new BaseData();
            Match textMatch = mItemTextReg.Match(twitterHtml);
            if (textMatch.Success)
            {
                string content = textMatch.Groups[mTextContentGroups].Value;
                string url = "";
                MatchCollection linkMatches = mItemLinkReg.Matches(content);
                foreach (Match linkMatch in linkMatches)
                {
                    string name = linkMatch.Groups[mLinkNameGroups].Value;
                    if (name[0] != '@')
                    {
                        url = linkMatch.Groups[mLinkUrlGroups].Value.Replace("\\", "");
                        content = content.Replace(linkMatch.Value, name + ": [ " + url + " ]");
                    }
                    else
                    {
                        content = content.Replace(name, "[" + name + "]");
                    }
                }
                twitter.Text = mHtmlLabel2Reg.Replace(mHtmlLabel1Reg.Replace(content,""), "");
            }

            MatchCollection imgMatches = mItemImgsReg.Matches(twitterHtml);
            string[] imgUrls = new string[imgMatches.Count];
            int i = 0;
            foreach (Match match in imgMatches)
            {
                imgUrls[i++] = match.Groups[mImgUrlGroups].Value.Replace("\\", "");
            }
            twitter.ImgUrls = imgUrls;
            return twitter;
        }

        /// <summary>
        /// 去掉置顶的微博
        /// </summary>
        /// <param name="topCount">置顶</param>
        /// <param name="data">微博结果</param>
        /// <returns></returns>
        private BaseData[] deleteTop(int topCount, BaseData[] data)
        {
            return data;
        }

        override public string getTakerName()
        {
            return "twitter";
        }
    }
}
