using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 配置文件读写接口，调用系统kernel32.dll的WritePrivateProfileString，GetPrivateProfileString方法。
/// </summary>

namespace QQRobot
{
    class Win32Api
    {
        private static Win32Api instance;

        public static Win32Api getInstance()
        {
            if (instance == null)
            {
                instance = new Win32Api();
            }
            return instance;
        }

        public const uint ES_SYSTEM_REQUIRED = 0x00000001;
        public const uint ES_DISPLAY_REQUIRED = 0x00000002;
        public const uint ES_CONTINUOUS = 0x80000000;
        // 声明INI文件的写操作函数 WritePrivateProfileString()
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        // 声明INI文件的读操作函数 GetPrivateProfileString()
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);

        // 声明INI文件的读操作函数 GetPrivateProfileString()
        [System.Runtime.InteropServices.DllImport("kernel32")]
        public static extern void SetThreadExecutionState(uint esFlags);

        /// <summary>
        /// 模拟windows登录域
        /// </summary>
        [System.Runtime.InteropServices.DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        private string sPath = null;
        public Win32Api setPath(string path)
        {
            sPath = path;
            return this;
        }

        public void WriteValue(string section, string key, string value)
        {
            // section=配置节，key=键名，value=键值，path=路径
            WritePrivateProfileString(section, key, value, sPath);
        }

        public string ReadValue(string section, string key)
        {
            // 每次从ini中读取多少字节
            System.Text.StringBuilder temp = new System.Text.StringBuilder(255);
            // section=配置节，key=键名，temp=上面，path=路径
            GetPrivateProfileString(section, key, "", temp, 255, sPath);
            return temp.ToString();
        }

        public string ReadValue(string section, string key, string def)
        {
            // 每次从ini中读取多少字节
            System.Text.StringBuilder temp = new System.Text.StringBuilder(255);
            // section=配置节，key=键名，temp=上面，path=路径
            GetPrivateProfileString(section, key, def, temp, 255, sPath);
            return temp.ToString();
        }

        public static void SystemUnsleepLock()
        {
            SetThreadExecutionState(ES_CONTINUOUS | ES_DISPLAY_REQUIRED | ES_SYSTEM_REQUIRED);
        }
        public static void SystemUnsleepLockRelase()
        {
            SetThreadExecutionState(ES_CONTINUOUS);
        }

        IntPtr admin_token = default(IntPtr);

        public void Login(string name, string pwd)
        {
            LogonUser(name, "", pwd, 9, 0, ref admin_token);
        }
    }
}
