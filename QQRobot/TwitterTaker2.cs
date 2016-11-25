using SimpleJSON;
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
        private const int HistorySize = 15;
        private List<BaseData> mTakeHistory = new List<BaseData>();

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
            IEnumerable<Twitter> tws = enumerableTwitts.Select((t) => new Twitter
            {
                Text = t["text"],
                Id = t["id_str"],
                TimeStamp = t["created_at"],
                ImgUrls = t["entities"]["media"] == null ? new string[0] : (t["entities"]["media"] as JSONArray).Childs.Select((m) => ((string)m["media_url"])).ToArray<string>(),
                User = new TwitterUser
                {
                    UserName = t["user"]["name"],
                    ScreenName = t["user"]["screen_name"],
                    UserId = t["user"]["id_str"],
                    UserHeaderUri = t["user"]["profile_image_url"],
                    Uri = t["user"]["url"],
                    Location = t["user"]["location"],
                    Description = t["user"]["description"],
                    FriendsCount = t["user"]["friends_count"],
                    ListedCount = t["user"]["listed_count"],
                    FavouritesCount = t["user"]["favourites_count"],
                    FollowersCount = t["user"]["followers_count"],
                },
            });
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
    }
}
