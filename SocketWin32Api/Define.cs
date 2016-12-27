using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketWin32Api
{
    namespace Define
    {
        public class Config
        {
            public const int SocketBufferSize = 1024 * 100;
        }
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

            ConversationLongLink = 11,
            ConversationNote = 12,
            ConversationDisconnect = 13,

            RemoteFindWindow = 1002,
            RemoteSendWindowText = 1003,
            DisConnect = 9999,
        }

        public class RequestKey
        {
            public const string Code = "code";
            public const string Args = "args";
            public const string RequestId = "requestId";
            public const string DeviceId = "deviceId";
        }

        public class Request
        {
            public int Code { set; get; }
            public string[] Args { set; get; }
            public string RequestId { set; get; }
            public string DeviceId { set; get; }
        }

        public class ResponseKey
        {
            public const string Code = "code";
            public const string Data = "data";
            public const string RequestId = "requestId";
            public const string Message = "message";
            public const string Error = "error";
        }

        public class CvsNoteKey
        {
            public const string Id = "id";
            public const string Content = "content";
            public const string UserName = "userName";
            public const string UserId = "userId";
            public const string TimeFormat = "timeFormat";
            public const string timeStamp = "timeStamp";
            public const string Extend = "extend";
            public const string IsSend = "isSend";
        }
    }
}
