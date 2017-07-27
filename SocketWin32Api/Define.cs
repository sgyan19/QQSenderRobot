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
        public class HeaderCode
        {
            public const byte ASK = 0x19;
            public const byte ANS = 0X91;
            public const byte RAW = 0x2B;
            public const byte CKRAW = 0x3B;
            public const byte CK_SUC_RAW = 0x3C;
            public const byte CK_FAIL_RAW = 0x3D;
            public const byte JSON = 0x7B;

            public static byte[] BYTES_ASK = { ASK ,ASK + 1, ASK + 2, ASK + 3 };
            public static byte[] BYTES_ANS = { ANS, ANS + 1, ANS + 2, ANS + 3 };
            public static byte[] BYTES_RAW = { RAW, RAW + 1, RAW + 2, RAW + 3 };
            public static byte[] BYTES_CKRAW = { CKRAW, CKRAW + 1, CKRAW + 2, CKRAW + 3 };
            public static byte[] BYTES_CK_SUC_RAW = { CK_SUC_RAW };
            public static byte[] BYTES_CK_FAIL_RAW = { CK_FAIL_RAW };
            public static byte[] BYTES_JSON = { JSON, JSON + 1, JSON + 2, JSON + 3 };
        }
        public enum ResponseCode
        {
            Success = 0,
            ErrorOverBuffer = 400,
            ErrorSocketRecive = 401,
            ErrorRawNotExist = 402,
            ErrorRawSend = 403,
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

            MobileTerminalJson=11,
            MobileTerminalRaw=12,

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
            //public const string Format = "format";
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

        public enum RequestId
        {
            REQUEST_KEY_NOBODY = -1,
            REQUEST_KEY_ANYBODY = 0,
        }
    }
}
