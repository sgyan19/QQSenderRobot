using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QQRobot
{
    class FileHelper
    {
        private static FileHelper instance;

        public static FileHelper getInstance()
        {
            if (instance == null)
            {
                instance = new FileHelper();
            }
            return instance;
        }

        protected FileHelper() { }

        WebClient wb = new WebClient(); // IE控件，用于下载微博图片

        public string download(string proxy,string url)
        {
            try
            {
                if (!string.IsNullOrEmpty(proxy))
                {
                    wb.Proxy = new WebProxy(proxy);
                }
                else
                {
                    wb.Proxy = null;
                }
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            int index = url.LastIndexOf('/');
            string name = url.Substring(index + 1, url.Length - index - 1);
            if (name.Length > 100)
            {
                name = name.Substring(0, 100);
            }
            if (!Directory.Exists("tmp"))
            {
                Directory.CreateDirectory("tmp");
            }
            string path = "tmp\\" + name;
            if (!File.Exists(path))
            {
                /*
                try
                {
                    File.Delete(path);
                }
                catch (Exception e)
                {
                    int i = 1;
                    while (File.Exists(path))
                    {
                        path += "(" + i + ")";
                        i++;
                    }
                }
                */
                try
                {
                    wb.DownloadFile(url, path);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            return path;
        }

        public Image download(string url, int count)
        {
            int index = url.LastIndexOf('/');
            string name = url.Substring(index + 1, url.Length - index - 1);
            if (name.Length > 100)
            {
                name = name.Substring(0, 100);
            }
            if (!Directory.Exists("tmp"))
            {
                Directory.CreateDirectory("tmp");
            }
            string path = "tmp\\" + name;
        download:
            if (!File.Exists(path))
            {
                try
                {
                    wb.DownloadFile(url, path);
                }
                catch (Exception e)
                {
                    if (count > 0)
                    {
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        count--;
                        goto download;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            Image image = null;
            try
            {
                image = Image.FromFile(path);
            }
            catch (Exception)
            {
                if (count > 0)
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    count--;
                    goto download;
                }
            }
            return image;
        }
    }
}
