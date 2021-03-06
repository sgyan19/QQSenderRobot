﻿using SimpleJSON;
using SocketWin32Api.Define;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketWin32Api
{
    public class SocketHelper
    {

        public static Hashtable LockObject = new Hashtable();
        private static int mLogKey = 0;
        private static byte[]  NONE_NYTE_ARRAY = new byte[0];
        private static Hashtable FileMd5table = new Hashtable();


        public static void request(Socket socket, byte[] buffer, string code, ref string bakeCode, ref string backData, params string[] args)
        {
            JSONClass request = new JSONClass();
            request.Add(RequestKey.Code, code);
            JSONArray array = new JSONArray();
            foreach (var item in args)
            {
                array.Add(item);
            }
            request.Add(RequestKey.Args, array);
            socket.Send(Encoding.UTF8.GetBytes(request.ToString()));
            int receiveNumber = socket.Receive(buffer);
            JSONClass response = JSON.Parse(Encoding.UTF8.GetString(buffer, 0, receiveNumber)) as JSONClass;
            bakeCode = response[ResponseKey.Code];
            backData = response[ResponseKey.Data];
        }

        public static string makeResponseJson(string code, string requestId, params string[] data)
        {
            JSONClass response = new JSONClass();
            response.Add(ResponseKey.Code, code);
            JSONArray dataArray = new JSONArray();
            foreach(string item in data)
            {
                dataArray.Add(null, item);
            }
            response.Add(ResponseKey.Data, dataArray);
            //response.Add(ResponseKey.Format, dataFormat);
            response.Add(ResponseKey.RequestId, requestId);
            return response.ToString();
        }

        public static void responseJson(Socket socket, string responseStr)
        {
            socket.Send(HeaderCode.BYTES_JSON);
            sendTextFrame(socket, responseStr);
        }

        public static string responseJson(Socket socket, string code, string requestId, params string[] data)
        {
            JSONClass response = new JSONClass();
            response.Add(ResponseKey.Code, code);
            JSONArray dataArray = new JSONArray();
            foreach (string item in data)
            {
                if(item != null)
                {
                    dataArray.Add(null, item);
                }
            }
            response.Add(ResponseKey.Data, dataArray);
            //response.Add(ResponseKey.Data, data);
            //response.Add(ResponseKey.Format, dataFormat);
            response.Add(ResponseKey.RequestId, requestId);
            string responseJson = response.ToString();
            socket.Send(HeaderCode.BYTES_JSON);
            sendTextFrame(socket, responseJson);
            return responseJson;
        }

        public static int responseRaw(Socket socket, byte[] buf, string dir, string name)
        {
            sendTextFrame(socket, name);
            return sendRawFrame(socket, buf, dir, name);
        }

        public static void sendTextFrame(Socket socket, string text)
        {
            byte[] forSend = Encoding.UTF8.GetBytes(text);
            byte[] size = BitConverter.GetBytes(forSend.Length);
            socket.Send(size);
            socket.Send(forSend);
        }

        public static int sendRawFrame(Socket socket, byte[] buf, string dir, string name)
        {
            string path = dir + "\\" + name;
            int len;
            int size;
            object obj = LockObject[name];
            if(obj != null)
            {
                lock (obj)
                {
                    FileInfo info = new FileInfo(path);
                    size = (int)info.Length;
                    socket.Send(BitConverter.GetBytes(size));
                    using (FileStream stream = new FileStream(path,
                        FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        while ((len = stream.Read(buf, 0, buf.Length)) > 0)
                        {
                            socket.Send(buf, 0, len, SocketFlags.None);
                        }
                        stream.Close();
                    }
                }
            }
            else
            {

                FileInfo info = new FileInfo(path);
                size = (int)info.Length;
                socket.Send(BitConverter.GetBytes(size));
                using (FileStream stream = new FileStream(path,
                        FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    while ((len = stream.Read(buf, 0, buf.Length)) > 0)
                    {
                        socket.Send(buf, 0, len, SocketFlags.None);
                    }
                    stream.Close();
                }
            }
            return size;
        }

        public static string receiveTextFrame(Socket socket, byte[] buf, LogHelper loger = null)
        {
            int key = mLogKey++;
            if (loger != null)
            {
                //loger.InfoFormat("receiveTextFrame key:{0}", key);
            }
            socket.ReceiveTimeout = 10000;
            int len = socket.Receive(buf, 4, SocketFlags.None);
            Int32 size = BitConverter.ToInt32(buf, 0);
            if (size > buf.Length)
            {
                throw new SocketException();
            }
            int offset = 0;
            while (size > 0 && (len = socket.Receive(buf, offset, size, SocketFlags.None)) > 0)
            {
                offset += len;
                size = size - len;
            }
            socket.ReceiveTimeout = -1;
            if (loger != null)
            {
                //loger.InfoFormat("receiveTextFrame key:{0}", key);
            }
            return Encoding.UTF8.GetString(buf, 0, offset);
        }

        public static string getRawMd5(string dirPath, string name, LogHelper loger = null)
        {
            string path = dirPath + "\\" + name;
            object obj = LockObject[name];
            string md5Data = null;
            if (File.Exists(path))
            {
                if (obj == null)
                {
                    obj = new object();
                    LockObject.Add(name, obj);
                }
                lock (obj)
                {
                    try
                    {
                        using (FileStream stream = new FileStream(path, FileMode.Open))
                        {
                            System.Security.Cryptography.MD5 mdr = new System.Security.Cryptography.MD5CryptoServiceProvider();
                            md5Data = toHexString(mdr.ComputeHash(stream));
                            stream.Close();
                        }
                    }catch(Exception)
                    {
                    }
                }
            }
            if(md5Data == null)
            {
                md5Data = "null";
            }
            return md5Data;
        }

        public static bool rawMd5Check(Socket socket, string applyMd5, string dirPath, string name, LogHelper loger = null)
        {
            string path = dirPath + "\\" + name;
            if (FileMd5table[name] != null)
            {
                if (applyMd5.Equals(FileMd5table[name] as string))
                {
                    return true;

                }
            }
            else
            {
                string md5 = getRawMd5(dirPath, name);
                if (applyMd5.Equals(md5))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool rawMd5ExistCheck(Socket socket, string applyMd5, string dirPath, string name, LogHelper loger = null)
        {
            string path = dirPath + "\\" + name;
            if (rawMd5Check(socket, applyMd5, dirPath, name, loger))
            {
                return true;
            }
            foreach (DictionaryEntry item in FileMd5table)
            {
                if (applyMd5.Equals(item.Value as string))
                {
                    string oldPath = dirPath + "\\" + item.Key;
                    if (File.Exists(oldPath))
                    {
                        File.Copy(oldPath, path, true);
                        return true;
                    }
                }
            }
            if (!FileMd5table.ContainsKey(name))
            {
                FileMd5table.Add(name, applyMd5);
            }
            return false;
        }

        public static bool Equals(byte[] a, byte[] b)
        {
            if (a == null || b == null) return false;
            if (a.Length != b.Length ) return false;
            int len = a.Length <= b.Length ? a.Length : b.Length;
            for (int i = 0; i < len; i++)
                if (a[i] != b[i])
                    return false;
            return true;
        }

        public static int receiveRawFrame(Socket socket, byte[] buf, LogHelper loger = null)
        {
            int key = mLogKey++;
            if (loger != null)
            {
                //loger.InfoFormat("receiveTextFrame key:{0}", key);
            }
            socket.ReceiveTimeout = 10000;
            int len = socket.Receive(buf, 4, SocketFlags.None);
            Int32 size = BitConverter.ToInt32(buf, 0);
            if (size > buf.Length)
            {
                throw new SocketException();
            }
            int offset = 0;
            while (size > 0 && (len = socket.Receive(buf, offset, size, SocketFlags.None)) > 0)
            {
                offset += len;
                size = size - len;
            }
            socket.ReceiveTimeout = -1;
            return offset;
        }

        public static int receiveRawFrame(Socket socket, byte[] buf, string dirPath, string name = null, LogHelper loger = null)
        {
            int key = mLogKey++;
            if (loger != null)
            {
                //loger.InfoFormat("receiveRawFrame key:{0}", key);
            }
            socket.ReceiveTimeout = 10000;
            int len = socket.Receive(buf, 4, SocketFlags.None);
            Int32 size = BitConverter.ToInt32(buf, 0);
            int result = size;
            if (string.IsNullOrEmpty(name))
            {
                name = "server." + DateTime.UtcNow.ToString();
            }
            string path = dirPath + "\\" + name;
            object obj = LockObject[name];
            if (obj == null)
            {
                obj = new object();
                LockObject.Add(name, obj);
            }
            lock (obj)
            {
                try
                {
                    using (FileStream stream = new FileStream(path,
                        FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        while (size > 0)
                        {
                            len = socket.Receive(buf, 0, size > buf.Length ? buf.Length : size, SocketFlags.None);
                            if (len == 0) break;
                            size = size - len;
                            stream.Write(buf, 0, len);
                        }
                        stream.Close();
                    }
                }
                finally
                {
                    LockObject.Remove(name);
                }
            }
            socket.ReceiveTimeout = -1;
            if (loger != null)
            {
                //loger.InfoFormat("receiveRawFrame key:{0}", key);
            }
            return result;
        }

        private static char[] HEX_DIGITS = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'D', 'E', 'F' };

        public static string toHexString(byte[] b)
        {
            StringBuilder sb = new StringBuilder(b.Length * 2);
            for (int i = 0; i < b.Length; i++)
            {
                sb.Append(HEX_DIGITS[(b[i] & 0xf0) >> 4]);
                sb.Append(HEX_DIGITS[b[i] & 0x0f]);
            }
            return sb.ToString();
        }
    }
}
