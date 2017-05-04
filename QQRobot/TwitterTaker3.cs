using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QQRobot
{
    class TwitterTaker3 : BaseTaker
    {
        private const string UserTemplet = "<img class=\\\"ProfileAvatar-image \\\" src=\\\"(?<url>[\\S]*?)\\\" alt=\\\"(?<name>[\\S]*?)\\\">";
        private const string ItemTemplet = "<div class=\\\"tweet js-stream-tweet js-actionable-tweet(?<item>[\\s\\S]*?)<div class=\\\"stream-item-footer\\\">";
        private const string ItemTextTemplet = "<p class=\\\"TweetTextSize TweetTextSize--[\\S]{1,6} js-tweet-text tweet-text\\\" lang=\\\"[a-z]{1,3}\\\" data-aria-label-part=\\\"[\\d]\\\">(?<content>[\\s\\S]*?)</p>";
        private const string ItemImgTemplet = "<img data-aria-label-part src=\\\"(?<url>[\\S]*?)\\\" alt=";
        private const string ItemLinkTemplet = "<a[\\s\\S]*?href=\\\"(?<url>[\\S]*?)\\\"[\\s\\S]*?>[\\s]*?(?<name>[\\S]*?)[\\s]*?</a>";
        private const string ItemLink2Templet = "<a[\\s\\S]*?title=\\\"(?<url>[\\S]*?)\\\"[\\s\\S]*?>[\\s]*?(?<name>[\\S]*?)[\\s]*?</a>";
        private const string HtmlLabel1Templet = "<[\\s\\S]*?>";
        private const string HtmlLabel2Templet = "</[\\s\\S]*?>";
        private const string HtmlLabel3Templet = "<[\\S]>";
        private const string HtmlLabel4Templet = "</[b-z]>";

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
        private Regex mItemLink2Reg = new Regex(ItemLink2Templet);
        private string mLinkUrlGroups = "url";
        private string mLinkNameGroups = "name";
        private Regex mHtmlLabel1Reg = new Regex(HtmlLabel1Templet);
        private Regex mHtmlLabel2Reg = new Regex(HtmlLabel2Templet);
        private Regex mHtmlLabel3Reg = new Regex(HtmlLabel3Templet);
        private Regex mHtmlLabel4Reg = new Regex(HtmlLabel4Templet);

        private const string AtEndTemplet = "@([a-zA-Z\\d_ ]*?)$";
        private const string AtStartTemplet = "^(@[a-zA-Z\\d_]*?) ";
        private const string HttpUriTemplet = "https{0,1}://\\S{1,}";
        private const string HttpTwitterTimplet = "https://twitter.com\\S{1,}";
        private const int HistorySize = 15;
        private List<BaseData> mTakeHistory = new List<BaseData>();

        private Regex mAtEndNameReg = new Regex(AtEndTemplet);
        private Regex mStartAtNameReg = new Regex(AtStartTemplet);
        private Regex mHttpUriReg = new Regex(HttpUriTemplet);
        private Regex mHttpTwitterReg = new Regex(HttpTwitterTimplet);
        public TwitterTaker3()
        {
            SafeCount = int.MaxValue;
        }

        public override BaseData[] checkNew(BaseData[] newTakeData, BaseData[] oldTakeData)
        {
            BaseData[] result = null;
            if(oldTakeData != null && oldTakeData.Length > 0 && newTakeData.Length > 0)
            {
                result = new BaseData[newTakeData.Length];
                for (int i = 0; i < newTakeData.Length; i++)
                {
                    result[newTakeData.Length - 1 - i] = newTakeData[i];
                }
            }
            return result;
        }

        public override BaseData[] paser(string html)
        {
            Twitter[] twitters = null;
            JSONNode json = JSON.Parse(html);
            var enumerableTwitts = (json as JSONArray).Childs;

            if (enumerableTwitts == null)
            {
                return null;
            }
            IEnumerable<Twitter> tws = enumerableTwitts.Select((t) => paserTwitterFormJson(t));
            twitters = tws == null ? new Twitter[0] : tws.ToArray<Twitter>();
            if(twitters.Length > 0)
            {
                User = twitters[0].User;
                User.Source = getTakerName();
                downloadUserHeader(User);
            }
            return twitters;
        }

        public override string takePage()
        {
            return TwitterApi.getInstance().GetTwitters(Uid, lastTake == null || lastTake.Length <= 0 ? null : (lastTake[0] as Twitter).Id, null, Proxy);
        }

        /// <summary>
        /// 整页html提取用户信息，头像，昵称
        /// </summary>
        /// <param name="html"></param>
        public override BaseUser paserUser(string html)
        {
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

        public override BaseData[] createData(int count)
        {
            return new Twitter[count];
        }

        public override BaseData[] getShowData(BaseData[] newData)
        {
            BaseData[]  showData = newData.Length == 0 && lastTake != null ? lastTake : newData;
            return showData;
        }

        public override void updateLastTake(BaseData[] newTake)
        {
            if (newTake != null && newTake.Length > 0)
            {
                mTakeHistory.InsertRange(0, newTake);
                while (mTakeHistory.Count > HistorySize)
                {
                    mTakeHistory.RemoveAt(mTakeHistory.Count - 1);
                }
            }
            lastTake = mTakeHistory.ToArray<BaseData>();
        }

        private Twitter paserTwitterFormJson(JSONNode json)
        {
            return new Twitter
            {
                Text = json["text"],
                Id = json["id_str"],
                TimeStamp = json["created_at"],
                ImgUrls = json["extended_entities"]["media"] == null ? new string[0] : (json["extended_entities"]["media"] as JSONArray).Childs.Select((m) => ((string)m["media_url"])).ToArray<string>(),
                User = paserUserFormJson(json),
                ReplyId = json["in_reply_to_status_id"],
                Truncated = json["truncated"],
                ReplyUser = new TwitterUser
                {
                    ScreenName = json["in_reply_to_screen_name"],
                    UserId = json["in_reply_to_user_id_str"],
                },
            };
        }

        private TwitterUser paserUserFormJson(JSONNode json)
        {
            return new TwitterUser
            {
                UserName = json["user"]["name"],
                ScreenName = json["user"]["screen_name"],
                UserId = json["user"]["id_str"],
                UserHeaderUri = json["user"]["profile_image_url"],
                Uri = json["user"]["url"],
                Location = json["user"]["location"],
                Description = json["user"]["description"],
                FriendsCount = json["user"]["friends_count"],
                ListedCount = json["user"]["listed_count"],
                FavouritesCount = json["user"]["favourites_count"],
                FollowersCount = json["user"]["followers_count"],
            };
        }

        public override BaseData onUse(BaseData data)
        {
            Twitter d = data as Twitter;
            if (d.Reply == null)
            {
                d = location302(d);
                d = requestTruncated(d);
                if (!string.IsNullOrEmpty(d.ReplyId) && !string.Equals(d.ReplyId, "null"))
                {
                    string text = mAtEndNameReg.Replace(d.Text, "") + string.Format(" //@{0} ", d.ReplyUser.ScreenName);

                    List<string> urls = new List<string>();
                    urls.AddRange(d.ImgUrls);
                    recursionReply(d, ref text, urls);
                    d.Text = text.Replace( "@"+ ((TwitterUser)User).ScreenName, "@" + ((TwitterUser)User).UserName);
                    d.ImgUrls = urls.ToArray();
                }
            }
            
            return d;
        }

        private void recursionReply(Twitter data, ref string fullText, List<string> fullUrl)
        {
            if (string.IsNullOrEmpty(data.ReplyId))
            {
                return ;
            }
            int times = 3;
            string jsonStr = "";
            Twitter reply = null;
        tryRequest:
            try
            {
                times--;
                jsonStr = TwitterApi.getInstance().GetTwitter(data.ReplyId, null, Proxy);
            }
            catch (Exception)
            {
                if (times >= 0)
                    goto tryRequest;
            }
            try
            {
                reply = paserTwitterFormJson(JSON.Parse(jsonStr));
            }
            catch (Exception) { }
            reply = location302(reply);
            reply = requestTruncated(reply);
            try
            {
                fullText += mAtEndNameReg.Replace(mStartAtNameReg.Replace(mStartAtNameReg.Replace(reply.Text, ""), ""), "") + ((string.IsNullOrEmpty(reply.ReplyId) || string.Equals(reply.ReplyId, "null")) ? "" : string.Format(" //@{0} ", reply.ReplyUser.ScreenName));
                fullUrl.AddRange(reply.ImgUrls);
                recursionReply(reply, ref fullText, fullUrl);
                data.Reply = reply;
            }catch  (Exception) { }
        }

        private Twitter requestTruncated(Twitter d)
        {
            if (bool.Parse(d.Truncated))
            {
                Match match = mHttpTwitterReg.Match(d.Text);
                if (match.Success)
                {
                tryRequest:
                    string html = null;
                    try {
                        html = request.GetData(3, null, match.Value, Proxy, Cookie, "https://twitter.com/");
                    }catch(TimeoutException e)
                    {
                        goto tryRequest;
                    }
                    match = mItemTextReg.Match(html);
                    if (match.Success)
                    {
                        string content = match.Groups[mTextContentGroups].Value;
                        content = mHtmlLabel1Reg.Replace(content, " ");
                        d.Text = content;
                    }
                    MatchCollection imgMatches = mItemImgsReg.Matches(html);
                    string[] imgUrls = new string[imgMatches.Count];
                    int i = 0;
                    foreach (Match m in imgMatches)
                    {
                        imgUrls[i++] = m.Groups[mImgUrlGroups].Value.Replace("\\", "");
                    }
                    d.ImgUrls = imgUrls;
                }
            }
            return d;
        }

        private Twitter location302(Twitter d)
        {
            MatchCollection matches = mHttpUriReg.Matches(d.Text);
            foreach (Match match in matches)
            {
                string location = null;
            reTry:
                try
                {
                    location = request.Location(match.Value, Proxy, null);
                }
                catch (TimeoutException)
                {
                    goto reTry;
                }
                if (!string.IsNullOrEmpty(location))
                {
                    d.Text = d.Text.Replace(match.Value, location);
                }
            }
            return d;
        }
    }
}
