﻿using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QQRobot
{
    class TwitterTaker2 : BaseTaker
    {
        private string[] NoneStringArray = new string[0];
        private const string AtEndTemplet = "@([a-zA-Z\\d_ ]*?)$";
        private const string AtStartTemplet = "^(@[a-zA-Z\\d_]*?) ";
        private const string HttpUriTemplet = "https{0,1}://\\S{1,}";
        private const int HistorySize = 15;
        private List<BaseData> mTakeHistory = new List<BaseData>();

        private Regex mAtEndNameReg = new Regex(AtEndTemplet);
        private Regex mStartAtNameReg = new Regex(AtStartTemplet);
        private Regex mHttpUriReg = new Regex(HttpUriTemplet);
        public TwitterTaker2()
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
            Twitter twitter = new Twitter
            {
                Text = json["text"],
                FullText = json["full_text"],
                Id = json["id_str"],
                TimeStamp = json["created_at"],
                ImgUrls = json["extended_entities"]["media"] == null ? NoneStringArray : (json["extended_entities"]["media"] as JSONArray).Childs.Select((m) => ((string)m["media_url"])).ToArray<string>(),
                User = paserUserFormJson(json),
                ReplyId = json["in_reply_to_status_id"],
                Truncated = json["truncated"],
                ReplyUser = new TwitterUser
                {
                    ScreenName = json["in_reply_to_screen_name"],
                    UserId = json["in_reply_to_user_id_str"],
                },
                IsQuoteStatus = json["is_quote_status"],
                QuotedStatusId = json["quoted_status_id_str"],
                ExpandedUrls = json["entities"]["urls"] == null ? NoneStringArray : (json["entities"]["urls"] as JSONArray).Childs.Select((m) => ((string)m["expanded_url"])).ToArray(),
            };

            if (!string.IsNullOrEmpty(twitter.FullText))
            {
                twitter.Text = twitter.FullText;
            }

            try
            {
                if (bool.Parse(twitter.IsQuoteStatus))
                {
                    twitter.QuotedStatus = paserTwitterFormJson(json["quoted_status"]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return twitter;
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
                int times = 3;
                if (bool.Parse(d.Truncated))
                {
                    Twitter newData = null;
                tryRequest:
                    try
                    {
                        times--;
                        string tr = TwitterApi.getInstance().GetTwitter(d.Id, null, Proxy);
                        JSONNode jsonNode = JSON.Parse(tr);
                        newData = paserTwitterFormJson(jsonNode);
                    }
                    catch (Exception)
                    {
                        if (times >= 0)
                            goto tryRequest;
                    }
                    if (newData != null)
                    {
                        d = newData;
                    }
                }
                if (!string.IsNullOrEmpty(d.ReplyId) && !string.Equals(d.ReplyId, "null"))
                {
                    string text = mAtEndNameReg.Replace(d.Text, "") + string.Format(" //@{0} ", d.ReplyUser.ScreenName);

                    List<string> urls = new List<string>();
                    urls.AddRange(d.ImgUrls);
                    recursionReply(d, ref text, urls);
                    d.Text = text.Replace(((TwitterUser)User).ScreenName, ((TwitterUser)User).UserName);
                    d.ImgUrls = urls.ToArray();
                }
                MatchCollection matches = mHttpUriReg.Matches(d.Text);
                foreach (Match match in matches)
                {
                    string location = null;
                reTry:
                    try
                    {
                        location = request.Location(match.Value, Proxy, new Dictionary<string, string>(0));
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
            try
            {
                fullText += mAtEndNameReg.Replace(mStartAtNameReg.Replace(mStartAtNameReg.Replace(reply.Text, ""), ""), "") + ((string.IsNullOrEmpty(reply.ReplyId) || string.Equals(reply.ReplyId, "null")) ? "" : string.Format(" //@{0} ", reply.ReplyUser.ScreenName));
                fullUrl.AddRange(reply.ImgUrls);
                recursionReply(reply, ref fullText, fullUrl);
                data.Reply = reply;
            }catch  (Exception) { }
        }
    }
}
