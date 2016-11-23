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

        public async Task<IEnumerable<Twitter>> GetTwitts(string userName,int count, string accessToken = null)
        {
            if (accessToken == null)
            {
                accessToken = await GetAccessToken();   
            }

            var requestUserTimeline = new HttpRequestMessage(HttpMethod.Get, string.Format("https://api.twitter.com/1.1/statuses/user_timeline.json?count={0}&screen_name={1}&exclude_replies=false&contributor_details=false", 10, userName));
            requestUserTimeline.Headers.Add("Authorization", "Bearer " + accessToken);
            var httpClient = new HttpClient();
            HttpResponseMessage responseUserTimeLine = await httpClient.SendAsync(requestUserTimeline);
            dynamic json = JSON.Parse(await responseUserTimeLine.Content.ReadAsStringAsync());
            var enumerableTwitts = (json as IEnumerable<dynamic>);

            if (enumerableTwitts == null)
            {
                return null;
            }
            return enumerableTwitts.Select((t) => new Twitter{
                Text = json["text"].ToString(),
                Id = json["id_str"].ToString()
            });                       
        }

        public async Task<IEnumerable<string>> GetTwitter(string tweetId, string accessToken = null)
        {
            if (accessToken == null)
            {
                accessToken = await GetAccessToken();
            }

            var requestUserTimeline = new HttpRequestMessage(HttpMethod.Get, string.Format("https://api.twitter.com/1.1/statuses/retweets/:{0}.json",  tweetId));
            requestUserTimeline.Headers.Add("Authorization", "Bearer " + accessToken);
            var httpClient = new HttpClient();
            HttpResponseMessage responseUserTimeLine = await httpClient.SendAsync(requestUserTimeline);
            dynamic json = JSON.Parse(await responseUserTimeLine.Content.ReadAsStringAsync());
            var enumerableTwitts = (json as IEnumerable<dynamic>);

            if (enumerableTwitts == null)
            {
                return null;
            }
            return enumerableTwitts.Select(t => (string)(t["text"].ToString()));
        }

        public string GetAccessToken(string proxy)
        {
            mRequest.PostData("https://api.twitter.com/oauth2/token", "grant_type=client_credentials","");
            return null;
        }

        public async Task<string> GetAccessToken()
        {
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitter.com/oauth2/token ");
            var customerInfo = Convert.ToBase64String(new UTF8Encoding().GetBytes(OAuthConsumerKey + ":" + OAuthConsumerSecret));
            request.Headers.Add("Authorization", "Basic " + customerInfo);
            request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

            HttpResponseMessage response = await httpClient.SendAsync(request);

            string json = await response.Content.ReadAsStringAsync();
            dynamic item = JSON.Parse(await response.Content.ReadAsStringAsync());
            return  item["access_token"];            
        }
    }
}