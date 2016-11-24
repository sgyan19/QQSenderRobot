using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQRobot
{
    public class TwitterUser : BaseUser
    {
        public string Uri { set; get; }
        public string ScreenName { set; get; }
        public string Location { set; get; }
        public string Description { set; get; }
        public string FriendsCount { set; get; }
        public string ListedCount { set; get; }
        public string FavouritesCount { set; get; }
        public string FollowersCount { set; get; }
    }
}
