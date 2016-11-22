using NetDimension.OpenAuth.Sina;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQRobot
{
    class WeiboSender : Sender
    {
        private SinaWeiboClient mSinaClient = null;
        private string mUserName;
        private string mUserId;
        private string mHeaderUrl;
        private string mToken;
        public override void send(string msg, Image[] imgs)
        {
            throw new NotImplementedException();
        }

        public static WeiboSender CreateSender(string token)
        {
            WeiboSender sender = new WeiboSender();
            try
            {
                sender.mToken = token;
                sender.mSinaClient = new SinaWeiboClient(token);
                /*
                string userJson = WeiboApi2.GetUserInfo(sender.mSinaClient, 3);
                SimpleJSON.JSONNode jsonNode = SimpleJSON.JSON.Parse(userJson);
                sender.mUserId = jsonNode["id"];
                sender.mUserName = jsonNode["screen_name"];
                sender.mHeaderUrl = jsonNode["profile_image_url"];
                */
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            return sender;
        }

        public override void sendWithUser(string userName, Image userHeader, string source, string msg, Image[] imgs, string longImgPath)
        {
            WeiboApi2.sendWeibo(mSinaClient, msg, longImgPath);
        }

        public override string getName()
        {
            return mToken;
        }
    }
}
