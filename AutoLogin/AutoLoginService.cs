using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Net.Sockets;
using System.Net;
using SimpleJSON;
using System.Threading;
using QQRobot;
using SocketWin32Api;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace AutoLogin
{
    public partial class AutoLoginService : ServiceBase
    {
        private ILog mLoger;
        private SocketServer mSocketServer;

        public AutoLoginService()
        {
            InitializeComponent();
            mLoger = LogManager.GetLogger("AutoLogin.logging");
            mLoger.Info("init");
            mSocketServer = new SocketServer();
        }

        protected override void OnStart(string[] args)
        {
            mLoger.Info("OnStart");
            mLoger.Info("Bind port 19190");
            mSocketServer.start(Define.Port.AutoLogin);
            mLoger.Info("OnStart end");
        }

        protected override void OnStop()
        {
            mLoger.Info("OnStop");
            mSocketServer.stop();
        }
    }
}
