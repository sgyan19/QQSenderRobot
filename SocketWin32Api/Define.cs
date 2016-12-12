using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketWin32Api
{
    namespace Define
    {
        public enum Port
        {
            Service = 19194,
            Form = 19193,
        }
        public enum RequestCode
        {
            RunCmd = 1,
            FindWindow = 2,
            SendWindowInfo = 3,
            
            RemoteFindWindow = 1002,
            RemoteSendWindowText = 1003,
            DisConnect = 9999,
        }

        public class RequestKey
        {
            public const string Code = "code";
            public const string Args = "args";
        }

        public class ResponseKey
        {
            public const string Code = "code";
            public const string Data = "data";
            public const string Message = "message";
            public const string Error = "error";
        }
    }
}
