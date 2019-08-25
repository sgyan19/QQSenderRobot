﻿using BlackRain;
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
using SimpleJSON;
using SocketWin32Api;
using SocketWin32Api.Define;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace QQRobot
{
    public partial class Form1 : Form
    {
        public void test()
        {

        }

        private WebClient wb = new WebClient();

        private string cookie;
        private string uid;
        private string interval;
        private string[] windows;
        private string[] weiboTokens;
        private string topCount;
        private string proxy;
        private string takerKind;
        private Handle handle;
        private Server server;
        private bool ifLog;
        private bool ifHeader = true;
        private bool ifFooter = true;
        private bool autoStartServer = true;
        private SocketServer socketServer;
        private ILog mLog;

        public Form1()
        {
            InitializeComponent();
            init();
            readConfig();
            //JSONNode json = JSON.Parse(TwitterApi.getInstance().GetTwitter("801958626390896641", null, proxy));
            //Console.WriteLine(json.ToString());
            //new PageRequest().Location(  "https://t.co/UizmmdoTmk", proxy, null);
            //bool locked = Win32Api.getInstance().isLockedWindow();
            
        }

        public void init()
        {
            Directory.SetCurrentDirectory(Application.StartupPath);
            handle = new Handle();
            handle.loger = new Loger();
            handle.takeLoger = new Loger("takeLog.txt");
            server = Server.getInstance();
            server.Callback = handle;
            socketServer = new SocketServer();
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
            socketServer.start((int)Port.Form);
            socketServer.setLoger(LogManager.GetLogger("SocketServer"));
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(server!= null)
            {
                server.Stop();
                socketServer.stop();
                Thread.Sleep(2000);
                try
                {
                    server.AbortStop();
                }
                catch (Exception)
                {

                }
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
            server.Start(taker);
            Win32Api.SystemUnsleepLock();
        }
        private ArrayList contols = new ArrayList();

        public void readConfig()
        {
            Win32Api ini = Win32Api.getInstance().setPath(".\\config.ini");
            string cookiePath = ini.ReadValue("robot", "cookie");
            string windowText = ini.ReadValue("robot", "windows");
            string tokenText = ini.ReadValue("robot", "weibo_sender_tokens");
            interval = ini.ReadValue("robot", "interval");
            uid = ini.ReadValue("robot", "uid");
            topCount = ini.ReadValue("robot","top");
            try
            { ifLog = bool.Parse(ini.ReadValue("robot", "iflog"));}
            catch (Exception) { }
            try
            { ifHeader = bool.Parse(ini.ReadValue("robot", "show_user_info", "true")); }
            catch (Exception) { }
            try
            { ifFooter = bool.Parse(ini.ReadValue("robot", "show_source","true")); }
            catch (Exception) { }
            windows = string.IsNullOrEmpty(windowText)? new string[0] : windowText.Split(',');
            weiboTokens = string.IsNullOrEmpty(tokenText) ? new string[0] : tokenText.Split(',');
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
            autoStartServer = bool.Parse(ini.ReadValue("robot", "auto_start", "false"));
            textBox1.Text = cookie;
            listBox1.Items.Clear();
            textBox3.Text = uid;
            textBox4.Text = interval;
            textBox5.Text = topCount;
            textBox7.Text = takerKind;
            textBox8.Text = proxy;
            checkBox1.Checked = ifHeader;
            checkBox2.Checked = ifFooter;
            for (int i = 0; i< windows.Length; i++)
            {
                listBox1.Items.Add(windows[i]);
            }
            for (int i = 0; i < weiboTokens.Length; i++)
            {
                listBox1.Items.Add(weiboTokens[i]);
            }
            handle.senders.Clear();
            handle.ifLog = ifLog;
            handle.showHeader = ifHeader;
            handle.showFooter = ifFooter;
            handle.ip = BlackRain.PageRequest.GetIPAddress();
            listBox2.Items.Clear();
            bool qqWindowReady = windows.Length <= 0 ? true : false;
            foreach (string win in windows)
            {
                QQSender sender = QQSender.CreateSender(win,this);
                if (sender != null)
                {
                    handle.senders.AddLast(sender);
                    listBox2.Items.Add(sender.getName());
                    qqWindowReady = true;
                }
            }
            foreach (string token in weiboTokens)
            {
                WeiboSender sender = WeiboSender.CreateSender(token);
                if (sender != null)
                {
                    handle.senders.AddLast(sender);
                    listBox2.Items.Add(sender.getName());
                }
            }


            if (autoStartServer && handle.senders.Count > 0)
            {
                if (qqWindowReady)
                {
                    SynchronizationContext.Current.Post(new SendOrPostCallback(t => { button2_Click(null, null); }), null);
                    //this.Invoke((Action) delegate{ button2_Click(null, null); });
                    
                }else
                {
                    Thread.Sleep(15000);
                    readConfig();
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
            taker.setProxy(proxy);
            taker.setUid(uid);
            taker.setInterval(interval);
            taker.setTopCount(topCount);
            server.StartOnce(taker);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
            string num = textBox6.Text;
            int n = int.Parse(num);
            if (n > 9) n = 9;
            if (n < 0) return;
            server.handLastTime(n);
            
            /*
            IntPtr ptr = SenderApi2.GetFindHwnd();
            ClipboardWrapper.Clear();
            ClipboardWrapper.SetText("TEST\nLine2");
            SenderApi2.QQPasteln(ptr);
            ClipboardWrapper.Clear();
            ClipboardWrapper.SetText("Line3");
            SenderApi2.QQPasteAndSumbit(ptr);
            */
        }
    }
}
