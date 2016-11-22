using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetDimension.OpenAuth.Sina;
using System.IO;

namespace QQRobot
{
    class WeiboApi2
    {
        public static void sendWeibo(SinaWeiboClient sw, string text, string picPath = null)
        {
            if (sw == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(picPath))
            {
                // 参考：http://open.weibo.com/wiki/2/statuses/update
                sw.HttpPost("statuses/update.json", new
                {
                    status = text
                });
            }
            else
            {
                FileInfo imageFile = new FileInfo(picPath);
                sw.HttpPost("statuses/upload.json", new
                {
                    status = text,
                    pic = imageFile //imgFile: 对于文件上传，这里可以直接传FileInfo对象
                });
            }
        }

        public static string GetUserInfo(SinaWeiboClient sw,int tryTimes)
        {
            // 调用获取获取用户信息api
            // 参考：http://open.weibo.com/wiki/2/users/show
            string result = null; ;
            while(result == null && tryTimes > 0)
            {
                var response = sw.HttpGet("users/show.json", new
                {
                    //可以传入一个Dictionary<string,object>类型的对象，也可以直接传入一个匿名类。参数与官方api文档中的参数相对应
                    uid = sw.UID
                });
                result = response.ToString();
                tryTimes--;
            }
            return result;
        }
    }
}
