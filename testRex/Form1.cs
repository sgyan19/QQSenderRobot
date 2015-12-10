using BlackRain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testRex
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string cookie = "SUBP=0033WrSXqPxfM725Ws9jqgMF55529P9D9W5yxPxmUkbz25mT1XVYeY.p5JpX5KMt; UOR=www.china.com.cn,widget.weibo.com,www.baidu.com; SINAGLOBAL=1862124125381.5225.1446697561491; ULV=1449711600603:20:8:3:5005859597794.194.1449711600598:1449625648434; SUHB=0Ry7gNzgoUSzCv; wvr=6; TC-Ugrow-G0=968b70b7bcdc28ac97c8130dd353b55e; SUS=SID-2538148691-1449711666-XD-uzxbs-cde06aded97856e6a980930b9d30c248; SUE=es%3Dcaa1b821dfaaa1a44a234f0fbbc9f3d8%26ev%3Dv1%26es2%3Dba8c36e41d61ba00f8a04bcd6b6dfe2b%26rs0%3DtdeXScLNtw5MLQVxr7aEvW11sHG3moqBvDH9VmIO4rmPL6%252FrmU%252BfzzXudZP2MaAYs%252BK6OrCAVRiD66med%252BPUNDROpOyMQQ6J8vaD2BkQIDqvAdsliVsW5yCJC1clmKa0XBdxILuh%252FuhO2Smhf2y%252B1fIJzbVfGIojlxC2OHWZlJA%253D%26rv%3D0; SUP=cv%3D1%26bt%3D1449711666%26et%3D1449798066%26d%3Dc909%26i%3Dc248%26us%3D1%26vf%3D0%26vt%3D0%26ac%3D0%26st%3D0%26uid%3D2538148691%26name%3Dmaths326009812%2540sina.com%26nick%3Dshing%26fmp%3D%26lcp%3D; SUB=_2A257bKhjDeTxGeRL6FoQ9CbKwj2IHXVYG56rrDV8PUNbvtAPLXP7kW8dyeCstY7NaK9DV7dvGfWynpMFlw..; ALF=1481247666; SSOLoginState=1449711667; TC-V5-G0=1e4d14527a0d458a29b1435fb7d41cc3; _s_tentry=login.sina.com.cn; Apache=5005859597794.194.1449711600598; TC-Page-G0=9151a132144e87253eb430a7bc179e6b";
            PageRequest request = new PageRequest();
            string html =  request.GetData("http://weibo.com/u/1774653601", cookie, "http://weibo.com");
            Regex gex = new Regex("<div class=\\\\\\\"WB_text W_f14\\\\\\\"[\\s\\S]*?>\\\\n[\\s]*?(?<content>[\\s\\S]*?)<\\\\/div>");
            Regex biaoqian = new Regex("<[\\s\\S]*?>");
            Regex kongbai = new Regex("[ \\\\/]");
            MatchCollection matches = gex.Matches(html);
            if(matches.Count > 0)
            {
                foreach(Match match in matches)
                {
                    string content = match.Groups["content"].Value;
                    string result = biaoqian.Replace(content, "");
                    string r = kongbai.Replace(result, "");
                    int x = 1212 + 2323;
                }
            }

        }
    }
}
