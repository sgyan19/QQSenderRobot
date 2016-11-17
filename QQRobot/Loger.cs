using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQRobot
{
    /// <summary>
    /// 日志类，用于输出抓取和发送日志。
    /// </summary>
    class Loger
    {
        private string logPath = ".\\log.txt";
        private FileStream aFile;
        private StreamWriter sw;
        public void SetPath(string path)
        {
            logPath = path;
            try
            {
                if (sw != null)
                    sw.Close();
                if (aFile != null)
                    aFile.Close();
                aFile = new FileStream(path, FileMode.OpenOrCreate);
                sw = new StreamWriter(aFile);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                sw = null;
                aFile = null;
            }
        }

        public Loger()
        {
            string newLogPath = logPath.Insert(logPath.LastIndexOf('.'), time());
            logPath = newLogPath;
        }

        public Loger(String path)
        {
            logPath = path.Insert(path.LastIndexOf('.'), time());
        }

        private void openFile()
        {
            try
            {
                if (aFile != null)
                    aFile.Close();
                aFile = new FileStream(logPath, FileMode.OpenOrCreate);
                if (sw != null)
                    sw.Close();
                sw = new StreamWriter(aFile);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                sw = null;
                aFile = null;
            }
        }

        public void log(string weibos)
        {
            lock (this)
            {
                if (sw == null)
                {
                    openFile();
                }
                if (sw != null)
                {
                    sw.Write(weibos);
                    sw.Flush();
                }
            }
        }
        /// <summary>
        /// 输出多条微博，用于记录一次抓取结果
        /// </summary>
        /// <param name="weibos">多条抓取结果</param>
        /// <param name="count">条数</param>
        public void log(BaseData[] weibos, int count)
        {
            lock (this)
            {
                if(sw == null)
                {
                    openFile();
                }
                if (sw != null)
                {
                    sw.WriteLine("");
                    sw.WriteLine("{0}  [第{1}次]", DateTime.Now.ToString(), count);
                    sw.WriteLine("");
                    foreach (BaseData weibo in weibos)
                    {
                        sw.WriteLine("    ===========================================================");
                        sw.WriteLine("    [Text]  {0}", weibo.Text);
                        sw.WriteLine("    [Imgs]");
                        if (weibo.ImgUrls != null && weibo.ImgUrls.Length > 0)
                        {
                            for (int i = 0; i < weibo.ImgUrls.Length; i++)
                            {
                                sw.WriteLine("           {0}", weibo.ImgUrls[i]);
                            }
                        }
                    }
                    sw.Flush();
                }
            }
        }
        /// <summary>
        /// 输出单条微博，用于记录单条发送。
        /// </summary>
        /// <param name="weibo"></param>
        public void log(BaseData weibo)
        {
            lock (this)
            {
                if (sw == null)
                {
                    openFile();
                }
                if (sw != null)
                {
                    sw.WriteLine(DateTime.Now.ToString());
                    sw.WriteLine("       {0} {1}", "[Text]", weibo.Text);
                    if(weibo.ImgUrls != null && weibo.ImgUrls.Length > 0)
                    {
                        for (int i = 0; i < weibo.ImgUrls.Length; i ++ )
                        {
                            sw.WriteLine("       {0} {1}", "[Imgs]", weibo.ImgUrls[i]);
                        }
                    }
                    sw.Flush();
                }
            }
        }
        public string time()
        {
            string dNow = DateTime.Now.ToString().Trim().Replace(":","").Replace(" ","").Replace("/","");
            string fileName = "[" + dNow + "]";
            return fileName;
        }
        ~Loger()
        {
            try
            {
                if (sw != null)
                {
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            try
            {
                if (aFile != null)
                    aFile.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

        }
    }
}
