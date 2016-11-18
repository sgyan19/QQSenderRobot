using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackRain;
using SimpleJSON;
using System.Collections.Specialized;
using System.IO;

namespace QQRobot
{
    class WeiboApi
    {
        private static WeiboApi instance;

        public static WeiboApi getInstance()
        {
            if(instance == null)
            {
                instance = new WeiboApi();
            }
            return instance;
        }

        protected WeiboApi(){}

        private const string AUTHOR_URL = "https://api.weibo.com/oauth2";
        private const string AUTHOR_API = "/authorize";
        private const string TOKEN_API = "/access_token";

        private const string API_URL = "https://upload.api.weibo.com/2";
        private const string USER_INFO_API = "/users/show.json";
        private const string USER_WEIBO_LIST_API = "/statuses/timeline_batch.json";

        private const string USER_SEND_WEIBO_API = "/statuses/upload.json";

        // form 闪记
        private const string WEIBO_APP_KEY = "1242380827";
        private const string WEIBO_REDIRECT_URL = "http://www.sina.com";
        private const string WEIBO_SCOPE = "email,direct_messages_read,direct_messages_write,"
                            + "friendships_groups_read,friendships_groups_write,statuses_to_me_read,"
                            + "follow_app_official_microblog,"
                            + "invitation_write";

        private PageRequest mRequest = new PageRequest();
        private string mToken = "2.00H7ollCZtgRxDc42d182d5ajIR83D";

        private string getAuthorUri()
        {
            return string.Format(AUTHOR_URL + AUTHOR_API + "?client_id={0}&response_type={1}&redirect_uri={2}", WEIBO_APP_KEY, WEIBO_SCOPE, WEIBO_REDIRECT_URL);
        }

        private string getTokenUrl(string client_secret, string grant_type, string code)
        {
            return string.Format(AUTHOR_URL + AUTHOR_API + "?client_id={0}&redirect_uri={1}&client_secret={2}&grant_type={3}&code={4}", WEIBO_APP_KEY, WEIBO_REDIRECT_URL, client_secret, grant_type, code);
        }

        public void author()
        {
            string html = mRequest.GetData(getAuthorUri(),"");
        }

        public BaseUser getUserInfo(params string[] args)
        {
            #region request url
            StringBuilder builder = new StringBuilder();
            builder.Append(API_URL).Append(USER_INFO_API).Append("?access_token=").Append(mToken);
            if(args.Length > 0 && args[0] != null)
            {
                builder.Append("&uid=").Append(args[0]);
            }
            if(args.Length > 1 && args[1] != null)
            {
                builder.Append("&screen_name=").Append(args[1]);
            }
            #endregion

            #region response
            BaseUser user = null;
            try
            {
                string json = mRequest.GetData(builder.ToString());
                var jnode = JSON.Parse(json);
                user = new BaseUser();
                user.UserId = jnode["id"];
                user.UserName = jnode["screen_name"];
                user.UserHeaderUri = jnode["profile_image_url"];
            }
            catch (Exception)
            {}
            #endregion

            return user;
        }

        public List<Weibo> getWeiboList(params string[] args)
        {
            #region request arg
            StringBuilder builder = new StringBuilder();
            builder.Append(API_URL).Append(USER_WEIBO_LIST_API).Append("?access_token=").Append(mToken);
            int argsIndex = 0;
            if (args.Length > argsIndex && args[argsIndex] != null)
            {
                builder.Append("&uids=").Append(args[argsIndex]);
            }
            argsIndex++;
            if (args.Length > argsIndex && args[argsIndex] != null)
            {
                builder.Append("&screen_names=").Append(args[argsIndex]);
            }
            argsIndex++;
            if (args.Length > argsIndex && args[argsIndex] != null)
            {
                builder.Append("&count=").Append(args[argsIndex]);
            }
            argsIndex++;
            if (args.Length > argsIndex && args[argsIndex] != null)
            {
                builder.Append("&page=").Append(args[argsIndex]);
            }
            argsIndex++;
            if (args.Length > argsIndex && args[argsIndex] != null)
            {
                builder.Append("&base_app=").Append(args[argsIndex]);
            }
            argsIndex++;
            if (args.Length > argsIndex && args[argsIndex] != null)
            {
                builder.Append("&feature=").Append(args[argsIndex]);
            }
            argsIndex++;
            #endregion

            List<Weibo> rel = null;
            try
            {
                string json = mRequest.GetData(builder.ToString());
                JSONNode node = JSON.Parse(json);
                rel = new List<Weibo>();
                 
                foreach (JSONNode item in node["statuses"].AsArray)
                {
                    Weibo data = new Weibo();
                    data.Mid = item["mid"];
                    data.Text = item["text"];
                    data.TimeStamp = item["created_at"];
                    rel.Add(data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            return rel;
        }

        public void sendWeibo(List<Weibo> data)
        {
            string url = API_URL + USER_SEND_WEIBO_API;
            StringBuilder builder = new StringBuilder();
            if (data != null && data.Count > 0)
            {
                NameValueCollection args = new NameValueCollection();
                List<UploadFile> files = new List<UploadFile>();
                foreach (Weibo item in data)
                {
                    args.Clear();
                    files.Clear();
                    args.Set("access_token", mToken);
                    args.Set("status", item.Text);
                    if (item.ImgUrls != null)
                    {
                        foreach (string uri in item.ImgUrls)
                        {
                            string path = FileHelper.getInstance().download(null, uri);
                            UploadFile file = new UploadFile();
                            file.Name = "pic";
                            file.Filename = "pic";
                            file.Data = File.ReadAllBytes(path);
                            files.Add(file);
                        }
                    }
                    mRequest.PostData(url, files, args);
                }
            }
        }
    }
}
