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

        protected TwitterApi()
        {
            AccessToken = "AAAAAAAAAAAAAAAAAAAAAL3fIAAAAAAAEfVOgPGO28dyJXjtiMHinyAUQOs%3Dg7PR2ALfEZb20tK8a1K5Eqec92TkHE8lNX3v2vgk";
        }
        private PageRequest mRequest = new PageRequest();

        public string OAuthConsumerSecret { get; set; }        
        public string OAuthConsumerKey { get; set; }
        public string AccessToken { get; set; }


        public string GetAccessToken(string proxy)
        {
            if (!string.IsNullOrEmpty(AccessToken))
            {
                return AccessToken;
            }
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
            string address = string.Format("https://api.twitter.com/1.1/statuses/user_timeline.json?count={0}&screen_name={1}&exclude_replies=false&contributor_details=true", 10, screenName);
            if (!string.IsNullOrEmpty(sinceId))
            {
                address += string.Format("&since_id={0}", sinceId);
            }
            return mRequest.GetData(1, new Dictionary<string, string>(){
                {"Authorization", "Bearer " + accessToken },
            }, address, proxy);
        }

        public string GetTwitters2(string screenName, string sinceId = null, string accessToken = null, string proxy = null, string cookie = null)
        {
            if (accessToken == null)
            {
                accessToken = GetAccessToken(proxy);
            }
            string address = string.Format("https://twitter.com/i/profiles/show/{0}/timeline/tweets?composed_count=0&include_available_features=1&include_entities=1&include_new_items_bar=true&interval=30000&latent_count=0", screenName);
            if (!string.IsNullOrEmpty(sinceId))
            {
                address += string.Format("&min_position={0}", sinceId);
            }
            return mRequest.GetData(2, new Dictionary<string, string>(){
                {"Authorization", "Bearer " + accessToken },
            }, address, proxy, cookie);
        }

        public string GetTwitter(string tweetId , string accessToken = null, string proxy = null)
        {
            if (accessToken == null)
            {
                accessToken = GetAccessToken(proxy);
            }
            string address = string.Format("https://api.twitter.com/1.1/statuses/show.json?id={0}&include_my_retweet=true&include_entities=true", tweetId);
            return mRequest.GetData(1, new Dictionary<string, string>(){
                {"Authorization", "Bearer " + accessToken },
            }, address, proxy);
        }
    }
}