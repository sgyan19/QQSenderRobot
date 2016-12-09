using log4net;
using SocketWin32Api;
using SocketWin32Api.Define;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace QQRobotService
{
    public partial class QQRobotService : ServiceBase
    {
        private ILog mLoger;
        private SocketServer mSocketServer;

        public QQRobotService()
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
            mSocketServer.start((int)Port.Service);
            mLoger.Info("OnStart end");
        }

        protected override void OnStop()
        {
            mLoger.Info("OnStop");
            mSocketServer.stop();
        }
    }
}
