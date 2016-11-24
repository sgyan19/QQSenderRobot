using BlackRain;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QQRobot
{
    public class TwitterApi
    {
        private static TwitterApi instance;

        public static TwitterApi getInstance()
        {
            if (instance == null)
            {
                instance  = new TwitterApi
                {
                    OAuthConsumerKey = "2yeyhTAMl3Euzc2aXSlOA",
                    OAuthConsumerSecret = "8yXVBamNBfd82hSbCbBNWPQvto0cjgqVjYuBbLXm0"
                };
            }
            return instance;
        }

        protected TwitterApi() { }
        private PageRequest mRequest = new PageRequest();

        public string OAuthConsumerSecret { get; set; }        
        public string OAuthConsumerKey { get; set; }

        public string GetAccessToken(string proxy)
        {
            var customerInfo = Convert.ToBase64String(new UTF8Encoding().GetBytes(OAuthConsumerKey + ":" + OAuthConsumerSecret));
            string html =  mRequest.PostData(1, new Dictionary<string, string> (){
                {"Authorization", "Basic " + customerInfo },

            }, "https://api.twitter.com/oauth2/token", "grant_type=client_credentials",proxy);
            dynamic json = JSON.Parse(html);
            return json["access_token"];
        }

        public string GetTwitters(string screenName, string sinceId = null,string accessToken = null, string proxy = null)
        {
            if (accessToken == null)
            {
                accessToken = GetAccessToken(proxy);
            }
            string address = string.Format("https://api.twitter.com/1.1/statuses/user_timeline.json?count={0}&screen_name={1}&exclude_replies=false&contributor_details=false", 10, screenName);
            if (!string.IsNullOrEmpty(sinceId))
            {
                address += string.Format("&since_id={0}", sinceId);
            }
            return mRequest.GetData(1, new Dictionary<string, string>(){
                {"Authorization", "Bearer " + accessToken },
            }, address, proxy);
        }
    }
}