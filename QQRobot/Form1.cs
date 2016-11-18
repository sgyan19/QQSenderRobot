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
using System.Collections;
using System.Net;
using System.IO;
using System.Threading;

namespace QQRobot
{
    public partial class Form1 : Form
    {
        public void test()
        {

        }

        private string cookie;
        private string uid;
        private string interval;
        private string[] windows;
        private string topCount;
        private string proxy;
        private string takerKind;
        private Handle handle;
        private Server server;
        private bool ifLog;

        public Form1()
        {
            InitializeComponent();
            init();
            readConfig();


        }

        public void init()
        {
            handle = new Handle();
            handle.loger = new Loger();
            handle.takeLoger = new Loger("takeLog.txt");
            server = Server.getInstance();
            server.Callback = handle;
            handle.senders = new LinkedList<Sender>();
            handle.shower = new UiShower(this);
            handle.shower.takeInfo = textBox2;
            handle.shower.statusLabel = label1;
            handle.shower.countDownLabel = label2;
            handle.shower.readConfigBtn = button1;
            handle.shower.startBtn = button2;
            handle.shower.stopBtn = button3;
            handle.shower.doBtn = button4;
            handle.shower.sendCountLabel = label9;
            FormClosed += Form1_FormClosed;

            List<Weibo> weibos = new List<Weibo>();
            Weibo weibo = new Weibo();
            weibos.Add(weibo);
            weibo.Text = "1212";
            weibo.ImgUrls = new string[1];
            weibo.ImgUrls[0] = "http://ww1.sinaimg.cn/mw690/5ed0f760jw1f6uc4cu91zj20go0cidhi.jpg";
            WeiboApi.getInstance().sendWeibo(weibos);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(server!= null)
            {
                server.Stop();
                Thread.Sleep(2000);
                server.AbortStop();
            }
        }

        public void start()
        {
            BaseTaker taker = BaseTaker.factory(takerKind);
            taker.setCookie(cookie);
            taker.setUid(uid);
            taker.setInterval(interval);
            taker.setTopCount(topCount);
            taker.setProxy(proxy);
            handle.setProxy(proxy);
            server.Start(taker);
            Win32Api.SystemUnsleepLock();
        }
        private ArrayList contols = new ArrayList();

        public void readConfig()
        {
            Win32Api ini = new Win32Api(".\\config.ini");
            string cookiePath = ini.ReadValue("robot", "cookie");
            string windowText = ini.ReadValue("robot", "windows");
            interval = ini.ReadValue("robot", "interval");
            uid = ini.ReadValue("robot", "uid");
            topCount = ini.ReadValue("robot","top");
            ifLog = bool.Parse(ini.ReadValue("robot", "iflog"));
            windows = windowText.Split(',');
            if (File.Exists(cookiePath))
            {
                try
                {
                    cookie = File.ReadAllText(cookiePath);
                }catch(Exception e)
                {
                    cookie = "";
                }
            }
            proxy = ini.ReadValue("robot", "proxy");
            takerKind = ini.ReadValue("robot", "taker", "weibo");
            textBox1.Text = cookie;
            listBox1.Items.Clear();
            textBox3.Text = uid;
            textBox4.Text = interval;
            textBox5.Text = topCount;
            textBox7.Text = takerKind;
            textBox8.Text = proxy;
            for (int i = 0; i< windows.Length; i++)
            {
                listBox1.Items.Add(windows[i]);
            }
            handle.senders.Clear();
            handle.ifLog = ifLog;
            listBox2.Items.Clear();
            foreach (string win in windows)
            {
                QQSender sender = QQSender.CreateSender(win,this);
                if (sender != null)
                {
                    handle.senders.AddLast(sender);
                    listBox2.Items.Add(sender.getWndName());
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            readConfig();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = true;
            start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label2.Text = "正在停止";
            server.Stop();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            BaseTaker taker = BaseTaker.factory(takerKind);

            taker.setCookie(cookie);
            taker.setUid(uid);
            taker.setInterval(interval);
            taker.setTopCount(topCount);
            server.StartOnce(taker);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string num = textBox6.Text;
            int n = int.Parse(num);
            if (n > 5) n = 5;
            if (n < 0) return;
            server.handLastTime(n);
        }
    }
}
