using BlackRain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Collections;

namespace QQRobot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            PageRequest request = new PageRequest();
            string cookie = "SUBP=0033WrSXqPxfM725Ws9jqgMF55529P9D9W5yxPxmUkbz25mT1XVYeY.p5JpX5K-t;UOR=www.china.com.cn,widget.weibo.com,www.baidu.com;SINAGLOBAL=1862124125381.5225.1446697561491;ULV=1449544689013:18:6:1:5025435082489.2295.1449544689009:1449208045965; SUHB=0OM126Vn-PaHA3; wvr=6; _s_tentry=www.wxrb.com;YF-Ugrow-G0=57484c7c1ded49566c905773d5d00f82;YF-V5-G0=9717632f62066ddd544bf04f733ad50a; SUS=SID-2538148691-1449542195-GZ-tz0nh-8c8433892e304787d217a0c9e4d6c248; SUE=es%3D52dde42ccfdba0d348e97d82aa04206b%26ev%3Dv1%26es2%3Db37f9183fd1d72deac498dc275cfb861%26rs0%3DogYc86E4SH20WBAKOn1rycviYDgRpqTwHfKOzoXJU87%252FPKIwy5ye%252B3CJpaGpdHy1bhk%252Fspi%252BP7G4DGpozTUt1rXAfsuPHyhc3LYHyNyTKYzFgJvwLniyrHcRbyVnyU7eW3%252FLzcGROROk05Q%252BcAMlD333NLTemRS%252BtK2wU3xZ1DY%253D%26rv%3D0; SUP=cv%3D1%26bt%3D1449542195%26et%3D1449628595%26d%3Dc909%26i%3Dc248%26us%3D1%26vf%3D0%26vt%3D0%26ac%3D0%26st%3D0%26uid%3D2538148691%26name%3Dmaths326009812%2540sina.com%26nick%3Dshing%26fmp%3D%26lcp%3D; SUB=_2A257YjJjDeTxGeRL6FoQ9CbKwj2IHXVYFiSrrDV8PUJbvNBeLUf7kW8jYWAXq1x3ozUMEvM6uWx1CCZjAg..; ALF=1481075408; SSOLoginState=1449542195; Apache=5025435082489.2295.1449544689009; YF-Page-G0=f017d20b1081f0a1606831bba19e407b";

            string html = request.GetData("http://weibo.com/u/1774653601 ", cookie, "http://weibo.com");

            //int n = html.IndexOf("<html>");

            //html = html.Substring(n, html.Length - n).Replace("\\r", "").Replace("\\n","").Replace("\\t", "").Replace("\r\n","").Replace("\"","").Replace("\\","");
            
            Regex weiboItem = new Regex("<div class=\\\\\\\"WB_detail\\\\\\\">(?<item>[\\s\\S]*?手机微博触屏版)<\\\\/a>");
            MatchCollection ms= weiboItem.Matches(html);
            string[] weibos = new string[ms.Count];
            if (ms.Count > 0 )
            {
                int i = 0;
                foreach (Match m in ms)
                {
                    weibos[i] = m.Value;
                    i++;
                }
            }

            Regex contentRex = new Regex("<div class=\\\\\\\"WB_text W_f14\\\\\\\" node-type=\\\\\\\"feed_list_content\\\\\\\" nick-name=\\\\\\\"新语丝之光\\\\\\\">\\\\n[\\s]*?(?<content>[\\S]*?)[\\s]*?<\\\\/div>");

            Match cm = contentRex.Match(weibos[7]);
            if (cm.Success)
            {
                string content = cm.Groups["content"].Value;
                int xasd = 10 * 10;
            }

            Regex imgRex = new Regex("<img[\\s\\S]*?class=\\\\\\\"bigcursor\\\\\\\"[\\s\\S]*?src=\\\\\\\"(?<url>[\\s\\S]*?)\\\\\\\"[\\s\\S]*?>");
            Match imgm = imgRex.Match(weibos[7]);
            if (imgm.Success)
            {
                string url = imgm.Groups["url"].Value;
                int xasd = 10 * 10;
            }
            int x = 10 * 10;

            /*
            Sender sender = Sender.CreateSender("等[0-9]个会话");
            if(sender != null)
            {
                sender.send("这是标题",Image.FromFile("pic.png"));
            }
            Sender sender2 = Sender.CreateSender("apple");

            if (sender2 != null)
            {
                sender2.send("这是标题", Image.FromFile("pic.png"));
            }
            */
        }
    }
}
