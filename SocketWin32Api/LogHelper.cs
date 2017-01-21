using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketWin32Api
{
    public class LogHelper
    {
        private static LogHelper instance = new LogHelper();

        public static LogHelper getInstance()
        {
            return instance;
        }
        private ILog coreLog;

        public ILog CoreLog
        {
            set
            {
                coreLog = value;
                instance = this;
            }
            get
            {
                return coreLog;
            }
        }

        public void Info(object message)
        {
            if(CoreLog != null){
                CoreLog.Info(message);
            }
        }

        public void InfoFormat(string message, params object[] args)
        {
            if (CoreLog != null)
            {
                CoreLog.InfoFormat(message, args);
            }
        }

        public void Warn(object message)
        {
            if (CoreLog != null)
            {
                CoreLog.Warn(message);
            }
        }

        public void WarnFormat(string message, params object[] args)
        {
            if (CoreLog != null)
            {
                CoreLog.WarnFormat(message, args);
            }
        }

        public void Error(object message)
        {
            if (CoreLog != null)
            {
                CoreLog.Error(message);
            }
        }

        public void ErrorFormat(string message, params object[] args)
        {
            if (CoreLog != null)
            {
                CoreLog.ErrorFormat(message, args);
            }
        }
    }
}
