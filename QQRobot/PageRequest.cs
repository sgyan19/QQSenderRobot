using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Cache;
using System.IO;
using System.IO.Compression;
using System.Collections;
using System.Collections.Specialized;

namespace BlackRain
{
    /// <summary>
    /// http请求类
    /// </summary>
    public class PageRequest
    {
        /// <summary>
        /// Post方式请求数据
        /// </summary>
        /// <param name="_url">请求地址</param>
        /// <param name="_data">请求数据</param>
        /// <param name="_cookie">请求时的Cookie</param>
        /// <returns>返回请求得到的值</returns>
        public string PostData(string _url, string _data, CookieCollection _cookie)
        {
            #region 创建HttpWebRequest请求对象
            //创建HttpWebRequest请求对象
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(_url));
            if (httpWebRequest == null)
            {
                throw new ApplicationException(string.Format("Invalid url string: {0}", _url));
            }
            #endregion

            #region 对httpWebRequest对象属性填充
            //对httpWebRequest对象属性填充
            httpWebRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
            if (_cookie != null)
            {
                CookieContainer cookieCon = new CookieContainer();
                cookieCon.Add(new Uri(_url), _cookie);
                httpWebRequest.CookieContainer = cookieCon;
            }
            httpWebRequest.Timeout = 20000;
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
            //httpWebRequest.Headers.Add("Accept-Encoding", "gzip,deflate;bzip2,sdch");
            httpWebRequest.Headers.Add("Pragma", "no-cache");
            httpWebRequest.Headers.Add("Accept-Charset", "GB2312,utf-8;q=0.7,*;q=0.7");
            //httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.1.5) Gecko/20091102 Firefox/3.5.5 (.NET CLR 3.5.30729)";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            httpWebRequest.Method = "POST";
            #endregion

            #region post内容写入请求流中
            //Post请求的内容
            byte[] postData = Encoding.UTF8.GetBytes(_data);
            httpWebRequest.ContentLength = postData.Length;
            Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(postData, 0, postData.Length);
            requestStream.Close();
            #endregion

            #region 发送post请求到服务器并读取服务器返回信息
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream responseStream = httpWebResponse.GetResponseStream();
            #endregion

            #region 读取服务器返回信息
            string _returnValue = "";
            using (StreamReader responseReader = new StreamReader(responseStream))
            {
                _returnValue = responseReader.ReadToEnd();
            }
            responseStream.Close();
            httpWebResponse.Close();
            return _returnValue;
            #endregion
        }
        public string PostData(string _url, string _data, ref CookieContainer _cookie, Hashtable headerTable, string referer="")
        {
            #region 创建HttpWebRequest请求对象
            //创建HttpWebRequest请求对象
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(_url));
            if (httpWebRequest == null)
            {
                throw new ApplicationException(string.Format("Invalid url string: {0}", _url));
            }
            #endregion

            #region 对httpWebRequest对象属性填充
            //对httpWebRequest对象属性填充
            httpWebRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
            httpWebRequest.CookieContainer = _cookie;
            httpWebRequest.Timeout = 20000;
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
            //httpWebRequest.Headers.Add("Accept-Encoding", "gzip,deflate;bzip2,sdch");
            httpWebRequest.Headers.Add("Pragma", "no-cache");
            httpWebRequest.Headers.Add("Accept-Charset", "GB2312,utf-8;q=0.7,*;q=0.7");
            //httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.1.5) Gecko/20091102 Firefox/3.5.5 (.NET CLR 3.5.30729)";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            httpWebRequest.Method = "POST";
            httpWebRequest.Referer = referer;
            if (headerTable != null)
            {
                ICollection tableKeys = headerTable.Keys;
                foreach (string key in tableKeys)
                {
                    httpWebRequest.Headers.Add(key.ToString(), headerTable[key].ToString());
                }
            }
            #endregion

            #region post内容写入请求流中
            //Post请求的内容
            byte[] postData = Encoding.UTF8.GetBytes(_data);
            httpWebRequest.ContentLength = postData.Length;
            Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(postData, 0, postData.Length);
            requestStream.Close();
            #endregion

            #region 发送post请求到服务器并读取服务器返回信息
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream responseStream = httpWebResponse.GetResponseStream();
            #endregion

            #region 读取服务器返回信息
            string _returnValue = "";
            using (StreamReader responseReader = new StreamReader(responseStream))
            {
                _returnValue = responseReader.ReadToEnd();
            }
            responseStream.Close();
            httpWebResponse.Close();
            return _returnValue;
            #endregion
        }
        public string PostData(string _url, string _data, string _cookie)
        {
            string _returnValue = "";
            try
            {
                Uri uri = new Uri(_url);//(betUrl, true);
                #region 创建httpWebRequest对象
                WebRequest webRequest = WebRequest.Create(uri);
                HttpWebRequest httpRequest = webRequest as HttpWebRequest;
                if (httpRequest == null)
                {
                    throw new ApplicationException(string.Format("Invalid url string: {0}", _url));
                }
                #endregion

                #region 填充httpWebRequest的基本信息
                HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
                httpRequest.CachePolicy = policy;
                httpRequest.CookieContainer = new CookieContainer();
                httpRequest.CookieContainer.SetCookies(uri, _cookie);
                httpRequest.KeepAlive = true;
                //httpRequest.Timeout = 20000;
                httpRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
                httpRequest.Headers.Add("Accept-Encoding", "gzip,deflate");
                httpRequest.Headers.Add("Pragma", "no-cache");

                httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";// : "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
                httpRequest.ContentType = "application/x-www-form-urlencoded";
                httpRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8, */*";//各类图片匹配
                #endregion

                #region post内容
                byte[] postData = Encoding.UTF8.GetBytes(_data);
                httpRequest.Method = "POST";
                httpRequest.ContentLength = postData.Length;
                Stream _Stream = httpRequest.GetRequestStream();
                _Stream.Flush();
                _Stream.Write(postData, 0, postData.Length);
                _Stream.Close();
                #endregion

                #region 发送post请求到服务器并读取服务器返回信息
                HttpWebResponse hwResponse = (HttpWebResponse)httpRequest.GetResponse();

                Stream responseStream = hwResponse.GetResponseStream();

                #endregion

                #region 读取服务器返回信息

                if (hwResponse.Headers["Content-Encoding"] != null && hwResponse.Headers["Content-Encoding"].ToLower().IndexOf("gzip") != -1)
                {
                    _returnValue = GZipDecompress(responseStream);
                }
                else
                {
                    using (StreamReader responseReader = new StreamReader(responseStream))
                    {
                        _returnValue = responseReader.ReadToEnd();
                    }
                }
                responseStream.Close();
                hwResponse.Close();
                //webRequest.Abort();
                #endregion

                httpRequest.Abort();
            }
            catch (Exception e)
            {
                throw e;
            }
            return _returnValue;
        }
        public string PostData(string _url, string _data, string _cookie, ref string statusCode,ref string encode)
        {
            string _returnValue = "";
            try
            {
                Uri uri = new Uri(_url);//(betUrl, true);
                #region 创建httpWebRequest对象
                WebRequest webRequest = WebRequest.Create(uri);
                HttpWebRequest httpRequest = webRequest as HttpWebRequest;
                if (httpRequest == null)
                {
                    throw new ApplicationException(string.Format("Invalid url string: {0}", _url));
                }
                #endregion

                #region 填充httpWebRequest的基本信息
                HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
                httpRequest.CachePolicy = policy;
                httpRequest.CookieContainer = new CookieContainer();
                httpRequest.CookieContainer.SetCookies(uri, _cookie);
                httpRequest.KeepAlive = true;
                //httpRequest.Timeout = 20000;
                httpRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
                httpRequest.Headers.Add("Accept-Encoding", "gzip,deflate");
                httpRequest.Headers.Add("Pragma", "no-cache");

                httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";// : "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
                httpRequest.ContentType = "application/x-www-form-urlencoded";
                httpRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8, */*";//各类图片匹配
                #endregion

                #region post内容
                byte[] postData = Encoding.UTF8.GetBytes(_data);
                httpRequest.Method = "POST";
                httpRequest.ContentLength = postData.Length;
                Stream _Stream = httpRequest.GetRequestStream();
                _Stream.Flush();
                _Stream.Write(postData, 0, postData.Length);
                _Stream.Close();
                #endregion

                #region 发送post请求到服务器并读取服务器返回信息
                HttpWebResponse hwResponse = (HttpWebResponse)httpRequest.GetResponse();

                Stream responseStream = hwResponse.GetResponseStream();

                #endregion

                #region 读取服务器返回信息

                statusCode = "HTTP " + (int)hwResponse.StatusCode;
                encode = hwResponse.Headers["Content-Type"];
                if(!string.IsNullOrEmpty(encode))
                {
                    int pos = encode.IndexOf("charset=");
                    if (pos >= 0)
                    {
                        pos += "charset=".Length;
                        encode = encode.Substring(pos, encode.Length - pos);
                    }
                    else
                        encode = "";
                }

                string  encodingStr =  null;
                if (string.IsNullOrEmpty(encode))
                    encodingStr = "utf-8";
                else
                    encodingStr = encode;
                Encoding encoding = null;
                try
                {
                    encoding = Encoding.GetEncoding(encodingStr);
                }catch(Exception ex)
                {
                    encoding = Encoding.UTF8;
                }

                if (hwResponse.Headers["Content-Encoding"] != null && hwResponse.Headers["Content-Encoding"].ToLower().IndexOf("gzip") != -1)
                {
                    _returnValue = GZipDecompress(responseStream);
                }
                else
                {
                    using (StreamReader responseReader = new StreamReader(responseStream, encoding))
                    {
                        Encoding es = responseReader.CurrentEncoding ;
                        _returnValue = responseReader.ReadToEnd();
                    }
                }
                responseStream.Close();
                hwResponse.Close();
                //webRequest.Abort();
                #endregion

                httpRequest.Abort();
            }
            catch (Exception e)
            {
                throw e;
            }
            return _returnValue;
        }
        public string PostData(string _url, string _data, string _cookie, ref string statusCode, ref string encode, string proxy)
        {
            string _returnValue = "";
            try
            {
                Uri uri = new Uri(_url);//(betUrl, true);
                #region 创建httpWebRequest对象
                WebRequest webRequest = WebRequest.Create(uri);
                HttpWebRequest httpRequest = webRequest as HttpWebRequest;
                if (httpRequest == null)
                {
                    throw new ApplicationException(string.Format("Invalid url string: {0}", _url));
                }
                #endregion

                #region 填充httpWebRequest的基本信息
                HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
                httpRequest.CachePolicy = policy;
                httpRequest.CookieContainer = new CookieContainer();
                httpRequest.CookieContainer.SetCookies(uri, _cookie);
                httpRequest.KeepAlive = true;
                // proxy
                WebProxy myProxy = new WebProxy();
                myProxy.Address = new Uri(proxy);
                httpRequest.Proxy = myProxy;
                //httpRequest.Timeout = 20000;
                httpRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
                httpRequest.Headers.Add("Accept-Encoding", "gzip,deflate");
                httpRequest.Headers.Add("Pragma", "no-cache");

                httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";// : "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
                httpRequest.ContentType = "application/x-www-form-urlencoded";
                httpRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8, */*";//各类图片匹配
                #endregion

                #region post内容
                byte[] postData = Encoding.UTF8.GetBytes(_data);
                httpRequest.Method = "POST";
                httpRequest.ContentLength = postData.Length;
                Stream _Stream = httpRequest.GetRequestStream();
                _Stream.Flush();
                _Stream.Write(postData, 0, postData.Length);
                _Stream.Close();
                #endregion

                #region 发送post请求到服务器并读取服务器返回信息
                HttpWebResponse hwResponse = (HttpWebResponse)httpRequest.GetResponse();

                Stream responseStream = hwResponse.GetResponseStream();

                #endregion

                #region 读取服务器返回信息

                statusCode = "HTTP " + (int)hwResponse.StatusCode;
                encode = hwResponse.Headers["Content-Type"];
                if (!string.IsNullOrEmpty(encode))
                {
                    int pos = encode.IndexOf("charset=");
                    if (pos >= 0)
                    {
                        pos += "charset=".Length;
                        encode = encode.Substring(pos, encode.Length - pos);
                    }
                    else
                        encode = "";
                }

                string encodingStr = null;
                if (string.IsNullOrEmpty(encode))
                    encodingStr = "utf-8";
                else
                    encodingStr = encode;
                Encoding encoding = null;
                try
                {
                    encoding = Encoding.GetEncoding(encodingStr);
                }
                catch (Exception ex)
                {
                    encoding = Encoding.UTF8;
                }

                if (hwResponse.Headers["Content-Encoding"] != null && hwResponse.Headers["Content-Encoding"].ToLower().IndexOf("gzip") != -1)
                {
                    _returnValue = GZipDecompress(responseStream);
                }
                else
                {
                    using (StreamReader responseReader = new StreamReader(responseStream, encoding))
                    {
                        Encoding es = responseReader.CurrentEncoding;
                        _returnValue = responseReader.ReadToEnd();
                    }
                }
                responseStream.Close();
                hwResponse.Close();
                //webRequest.Abort();
                #endregion

                httpRequest.Abort();
            }
            catch (Exception e)
            {
                throw e;
            }
            return _returnValue;
        }
        public string PostData(string _url, string _data, string _cookie,int timeout)
        {
            string _returnValue = "";
            try
            {
                Uri uri = new Uri(_url);//(betUrl, true);
                #region 创建httpWebRequest对象
                WebRequest webRequest = WebRequest.Create(uri);
                HttpWebRequest httpRequest = webRequest as HttpWebRequest;
                if (httpRequest == null)
                {
                    throw new ApplicationException(string.Format("Invalid url string: {0}", _url));
                }
                #endregion

                #region 填充httpWebRequest的基本信息
                HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
                httpRequest.CachePolicy = policy;
                httpRequest.CookieContainer = new CookieContainer();
                httpRequest.CookieContainer.SetCookies(uri, _cookie);
                httpRequest.KeepAlive = true;
                httpRequest.Timeout = timeout;
                httpRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
                httpRequest.Headers.Add("Accept-Encoding", "gzip,deflate");
                httpRequest.Headers.Add("Pragma", "no-cache");

                httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";// : "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
                httpRequest.ContentType = "application/x-www-form-urlencoded";
                httpRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8, */*";//各类图片匹配
                #endregion

                #region post内容
                byte[] postData = Encoding.UTF8.GetBytes(_data);
                httpRequest.Method = "POST";
                httpRequest.ContentLength = postData.Length;
                Stream _Stream = httpRequest.GetRequestStream();
                _Stream.Flush();
                _Stream.Write(postData, 0, postData.Length);
                _Stream.Close();
                #endregion

                #region 发送post请求到服务器并读取服务器返回信息
                HttpWebResponse hwResponse = (HttpWebResponse)httpRequest.GetResponse();

                Stream responseStream = hwResponse.GetResponseStream();

                #endregion

                #region 读取服务器返回信息

                if (hwResponse.Headers["Content-Encoding"] != null && hwResponse.Headers["Content-Encoding"].ToLower().IndexOf("gzip") != -1)
                {
                    _returnValue = GZipDecompress(responseStream);
                }
                else
                {
                    using (StreamReader responseReader = new StreamReader(responseStream))
                    {
                        _returnValue = responseReader.ReadToEnd();
                    }
                }
                responseStream.Close();
                hwResponse.Close();
                //webRequest.Abort();
                #endregion

                httpRequest.Abort();
            }
            catch (Exception e)
            {
                throw e;
            }
            return _returnValue;
        }
        public string PostData(string _url, string _data, ref string _cookie, bool isAddCooke)
        {
            string _returnValue = "";
            try
            {
                Uri uri = new Uri(_url);//(betUrl, true);
                #region 创建httpWebRequest对象
                WebRequest webRequest = WebRequest.Create(uri);
                HttpWebRequest httpRequest = webRequest as HttpWebRequest;
                if (httpRequest == null)
                {
                    throw new ApplicationException(string.Format("Invalid url string: {0}", _url));
                }
                #endregion

                #region 填充httpWebRequest的基本信息
                HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
                httpRequest.CachePolicy = policy;
                httpRequest.CookieContainer = new CookieContainer();
                httpRequest.CookieContainer.SetCookies(uri, _cookie);
                httpRequest.KeepAlive = true;
                httpRequest.AllowAutoRedirect = false;
                //httpRequest.Timeout = 20000;
                httpRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
                //httpRequest.Headers.Add("Accept-Encoding", "gzip,deflate;bzip2,sdch");
                httpRequest.Headers.Add("Pragma", "no-cache");

                httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";// : "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
                httpRequest.ContentType = "application/x-www-form-urlencoded";
                httpRequest.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, */*";//各类图片匹配
                #endregion

                #region post内容
                byte[] postData = Encoding.UTF8.GetBytes(_data);
                httpRequest.Method = "POST";
                httpRequest.ContentLength = postData.Length;
                Stream _Stream = httpRequest.GetRequestStream();
                _Stream.Flush();
                _Stream.Write(postData, 0, postData.Length);
                _Stream.Close();
                #endregion

                #region 发送post请求到服务器并读取服务器返回信息
                HttpWebResponse hwResponse = (HttpWebResponse)httpRequest.GetResponse();

                Stream responseStream = hwResponse.GetResponseStream();

                #endregion

                #region 读取服务器返回信息

                using (StreamReader responseReader = new StreamReader(responseStream))
                {
                    _returnValue = responseReader.ReadToEnd();
                    responseReader.Close();
                }
                if (hwResponse.Headers.Get("Set-Cookie") != null)
                {
                    if (isAddCooke)
                    {
                        _cookie += ("," + hwResponse.Headers.Get("Set-Cookie"));
                        _cookie = _cookie.TrimEnd(',');
                    }
                    else
                    {
                        _cookie = hwResponse.Headers.Get("Set-Cookie");
                    }
                }
                responseStream.Close();
                //hwResponse.Close();
                webRequest.Abort();
                #endregion
            }
            catch (Exception e)
            {
                throw e;
            }
            return _returnValue;
        }
        public string PostData(string _url, string _data, ref string _cookie, string Referer)
        {
            string _returnValue = "";
            _cookie = _cookie.TrimStart(',');
            try
            {
                Uri uri = new Uri(_url);//(betUrl, true);
                #region 创建httpWebRequest对象
                WebRequest webRequest = WebRequest.Create(uri);
                HttpWebRequest httpRequest = webRequest as HttpWebRequest;
                if (httpRequest == null)
                {
                    throw new ApplicationException(string.Format("Invalid url string: {0}", _url));
                }
                #endregion

                #region 填充httpWebRequest的基本信息
                HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
                httpRequest.CachePolicy = policy;
                httpRequest.CookieContainer = new CookieContainer();
                httpRequest.CookieContainer.SetCookies(uri, _cookie);
                httpRequest.KeepAlive = true;
                //httpRequest.Timeout = 20000;
                httpRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
                //httpRequest.Headers.Add("Accept-Encoding", "gzip,deflate;bzip2,sdch");
                httpRequest.Headers.Add("Pragma", "no-cache");
                httpRequest.Referer = Referer;
                httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";// : "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
                httpRequest.ContentType = "application/x-www-form-urlencoded";
                httpRequest.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, */*";//各类图片匹配
                #endregion

                #region post内容
                byte[] postData = Encoding.UTF8.GetBytes(_data);
                httpRequest.Method = "POST";
                httpRequest.ContentLength = postData.Length;
                Stream _Stream = httpRequest.GetRequestStream();
                _Stream.Flush();
                _Stream.Write(postData, 0, postData.Length);
                _Stream.Close();
                #endregion

                #region 发送post请求到服务器并读取服务器返回信息
                HttpWebResponse hwResponse = (HttpWebResponse)httpRequest.GetResponse();

                Stream responseStream = hwResponse.GetResponseStream();

                #endregion

                #region 读取服务器返回信息

                using (StreamReader responseReader = new StreamReader(responseStream))
                {
                    _returnValue = responseReader.ReadToEnd();
                    responseReader.Close();
                }
                _cookie += ("," + hwResponse.Headers.Get("Set-Cookie"));
                _cookie = _cookie.TrimEnd(',');
                responseStream.Close();
                //hwResponse.Close();
                webRequest.Abort();
                #endregion
            }
            catch (Exception e)
            {
                throw e;
            }
            return _returnValue;
        }
        public string PostData(string _url, string _data, ref string _cookie, string Referer,int timeout)
        {
            string _returnValue = "";
            try
            {
                Uri uri = new Uri(_url);//(betUrl, true);
                #region 创建httpWebRequest对象
                WebRequest webRequest = WebRequest.Create(uri);
                HttpWebRequest httpRequest = webRequest as HttpWebRequest;
                if (httpRequest == null)
                {
                    throw new ApplicationException(string.Format("Invalid url string: {0}", _url));
                }
                #endregion

                #region 填充httpWebRequest的基本信息
                HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
                httpRequest.CachePolicy = policy;
                httpRequest.CookieContainer = new CookieContainer();
                httpRequest.CookieContainer.SetCookies(uri, _cookie);
                httpRequest.KeepAlive = true;
                httpRequest.Timeout = timeout;
                httpRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
                //httpRequest.Headers.Add("Accept-Encoding", "gzip,deflate;bzip2,sdch");
                httpRequest.Headers.Add("Pragma", "no-cache");
                httpRequest.Referer = Referer;
                httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";// : "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
                httpRequest.ContentType = "application/x-www-form-urlencoded";
                httpRequest.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, */*";//各类图片匹配
                #endregion

                #region post内容
                byte[] postData = Encoding.UTF8.GetBytes(_data);
                httpRequest.Method = "POST";
                httpRequest.ContentLength = postData.Length;
                Stream _Stream = httpRequest.GetRequestStream();
                _Stream.Flush();
                _Stream.Write(postData, 0, postData.Length);
                _Stream.Close();
                #endregion

                #region 发送post请求到服务器并读取服务器返回信息
                HttpWebResponse hwResponse = (HttpWebResponse)httpRequest.GetResponse();

                Stream responseStream = hwResponse.GetResponseStream();

                #endregion

                #region 读取服务器返回信息

                using (StreamReader responseReader = new StreamReader(responseStream))
                {
                    _returnValue = responseReader.ReadToEnd();
                    responseReader.Close();
                }
                _cookie += ("," + hwResponse.Headers.Get("Set-Cookie"));
                _cookie = _cookie.TrimEnd(',');
                responseStream.Close();
                //hwResponse.Close();
                webRequest.Abort();
                #endregion
            }
            catch (Exception e)
            {
                throw e;
            }
            return _returnValue;
        }
        /// <summary>
        /// Get方式请求数据
        /// </summary>
        /// <param name="_url">请求地址</param>
        /// <param name="_cookie">请求的Cookie</param>
        /// <returns>返回请求得到的值</returns>
        public string GetData(string _url, CookieCollection _cookie)
        {
            #region 创建HttpWebRequest请求对象
            //创建HttpWebRequest请求对象
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(_url));
            if (httpWebRequest == null)
            {
                throw new ApplicationException(string.Format("Invalid url string: {0}", _url));
            }
            #endregion

            #region 对httpWebRequest对象属性填充
            //对httpWebRequest对象属性填充
            httpWebRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
            CookieContainer cookieCon = new CookieContainer();
            cookieCon.Add(new Uri(_url), _cookie);
            httpWebRequest.CookieContainer = cookieCon;
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Timeout = 20000;
            httpWebRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
            //httpWebRequest.Headers.Add("Accept-Encoding", "gzip,deflate;bzip2,sdch");
            httpWebRequest.Headers.Add("Pragma", "no-cache");
            //httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
            httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 1.1.4322)";
            httpWebRequest.ContentType = "text/html; charset=gb2312";
            httpWebRequest.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, */*";//各类图片匹配
            httpWebRequest.Method = "GET";
            #endregion

            #region 发送Get请求到服务器并读取服务器返回信息
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream responseStream = httpWebResponse.GetResponseStream();
            #endregion

            #region 读取服务器返回信息
            string _returnData = "";
            using (StreamReader responseReader = new StreamReader(responseStream))
            {
                _returnData = responseReader.ReadToEnd();
            }
            responseStream.Close();
            httpWebResponse.Close();
            return _returnData;
            #endregion
        }
        public string GetData(string _url, ref CookieContainer _cookie, Hashtable headerTable, string referer="")
        {
            #region 创建HttpWebRequest请求对象
            //创建HttpWebRequest请求对象
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(_url));
            if (httpWebRequest == null)
            {
                throw new ApplicationException(string.Format("Invalid url string: {0}", _url));
            }
            #endregion

            #region 对httpWebRequest对象属性填充
            //对httpWebRequest对象属性填充
            httpWebRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
            httpWebRequest.CookieContainer = _cookie;
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Timeout = 20000;
            httpWebRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
            //httpWebRequest.Headers.Add("Accept-Encoding", "gzip,deflate;bzip2,sdch");
            httpWebRequest.Headers.Add("Pragma", "no-cache");
            //httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
            httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 1.1.4322)";
            httpWebRequest.ContentType = "text/html; charset=gb2312";
            httpWebRequest.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, */*";//各类图片匹配
            httpWebRequest.Method = "GET";
            httpWebRequest.Referer = referer;
            if (headerTable != null)
            {
                ICollection tableKeys = headerTable.Keys;
                foreach (string key in tableKeys)
                {
                    httpWebRequest.Headers.Add(key.ToString(), headerTable[key].ToString());
                }
            }

            #endregion

            #region 发送Get请求到服务器并读取服务器返回信息
            HttpWebResponse httpWebResponse;
            Stream responseStream;
            try
            {
                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                responseStream = httpWebResponse.GetResponseStream();
            }
            catch (Exception)
            {
                return "";
            }
            #endregion

            #region 读取服务器返回信息
            string _returnData = "";
            using (StreamReader responseReader = new StreamReader(responseStream))
            {
                _returnData = responseReader.ReadToEnd();
            }
            responseStream.Close();
            httpWebResponse.Close();
            return _returnData;
            #endregion
        }
        public string GetData(string _url)
        {
            string _returnData = "";
            try
            {
                WebRequest webRequest = null;
                HttpWebRequest httpRequest = null;
                Uri uri = new Uri(_url);
                webRequest = WebRequest.Create(uri);
                httpRequest = webRequest as HttpWebRequest;
                httpRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
                httpRequest.Headers.Add("Accept-Encoding", "gzip,deflate");//压缩
                //httpRequest.Headers.Add("Cache-Control", "max-age=0");
                httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";// : "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
                httpRequest.ContentType = "text/html; charset=gb2312";
                httpRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8, */*";//各类图片匹配
                httpRequest.Method = "GET";

                httpRequest.AllowAutoRedirect = false;
                //httpRequest.Timeout = 20000;
                HttpWebResponse web = (HttpWebResponse)httpRequest.GetResponse();
                Stream webstream = web.GetResponseStream();
                if (web.Headers["Content-Encoding"] != null && web.Headers["Content-Encoding"].ToLower().IndexOf("gzip") != -1)
                    _returnData = GZipDecompress(webstream);
                else
                {
                    using (StreamReader sr = new StreamReader(webstream, System.Text.Encoding.Default))
                    {
                        _returnData = sr.ReadToEnd();
                    }
                }
                webstream.Close();
                web.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            return _returnData;
        }
        public string GetData(string _url, string _cookie)
        {
            string _returnData = "";
            try
            {
                WebRequest webRequest = null;
                HttpWebRequest httpRequest = null;
                Uri uri = new Uri(_url);
                webRequest = WebRequest.Create(uri);
                httpRequest = webRequest as HttpWebRequest;
                httpRequest.CookieContainer = new CookieContainer();
                httpRequest.CookieContainer.SetCookies(uri, _cookie);
                httpRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
                httpRequest.Headers.Add("Accept-Encoding", "gzip,deflate");//压缩
                //httpRequest.Headers.Add("Cache-Control", "max-age=0");
                httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";// : "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
                httpRequest.ContentType = "text/html; charset=gb2312";
                httpRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8, */*";//各类图片匹配
                httpRequest.Method = "GET";

                httpRequest.AllowAutoRedirect = false;
                //httpRequest.Timeout = 20000;
                HttpWebResponse web = (HttpWebResponse)httpRequest.GetResponse();
                Stream webstream = web.GetResponseStream();
                if (web.Headers["Content-Encoding"] != null && web.Headers["Content-Encoding"].ToLower().IndexOf("gzip") != -1)
                    _returnData = GZipDecompress(webstream);
                else
                {
                    using (StreamReader sr = new StreamReader(webstream, System.Text.Encoding.Default))
                    {
                        _returnData = sr.ReadToEnd();
                    }
                }
                webstream.Close();
                web.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            return _returnData;
        }
        public string GetData(string _url, string _cookie, ref string statusCode, ref string encode)
        {
            string _returnData = "";
            try
            {
                WebRequest webRequest = null;
                HttpWebRequest httpRequest = null;
                Uri uri = new Uri(_url);
                webRequest = WebRequest.Create(uri);
                httpRequest = webRequest as HttpWebRequest;
                httpRequest.CookieContainer = new CookieContainer();
                httpRequest.CookieContainer.SetCookies(uri, _cookie);
                httpRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
                httpRequest.Headers.Add("Accept-Encoding", "gzip,deflate");//压缩
                //httpRequest.Headers.Add("Cache-Control", "max-age=0");
                httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";// : "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
                httpRequest.ContentType = "text/html; charset=gb2312";
                httpRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8, */*";//各类图片匹配
                httpRequest.Method = "GET";

                httpRequest.AllowAutoRedirect = false;
                //httpRequest.Timeout = 20000;
                HttpWebResponse web = (HttpWebResponse)httpRequest.GetResponse();
                Stream webstream = web.GetResponseStream();
                statusCode = "HTTP " + (int)web.StatusCode;
                encode = web.Headers["charset"];
                encode = web.Headers["Content-Type"];
                if (!string.IsNullOrEmpty(encode))
                {
                    int pos = encode.IndexOf("charset=") + "charset=".Length;
                    encode = encode.Substring(pos, encode.Length - pos);
                }
                if (web.Headers["Content-Encoding"] != null && web.Headers["Content-Encoding"].ToLower().IndexOf("gzip") != -1)
                    _returnData = GZipDecompress(webstream);
                else
                {
                    using (StreamReader sr = new StreamReader(webstream, System.Text.Encoding.Default))
                    {
                        _returnData = sr.ReadToEnd();
                    }
                }
                webstream.Close();
                web.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            return _returnData;
        }
        public string GetData(string _url, string _cookie, ref string statusCode, ref string encode, string proxy)
        {
            string _returnData = "";
            try
            {
                WebRequest webRequest = null;
                HttpWebRequest httpRequest = null;
                Uri uri = new Uri(_url);
                webRequest = WebRequest.Create(uri);
                httpRequest = webRequest as HttpWebRequest;
                httpRequest.CookieContainer = new CookieContainer();
                httpRequest.CookieContainer.SetCookies(uri, _cookie);
                httpRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
                httpRequest.Headers.Add("Accept-Encoding", "gzip,deflate");//压缩
                //httpRequest.Headers.Add("Cache-Control", "max-age=0");
                httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";// : "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
                httpRequest.ContentType = "text/html; charset=gb2312";
                httpRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8, */*";//各类图片匹配
                httpRequest.Method = "GET";
                httpRequest.AllowAutoRedirect = false;
                // proxy
                WebProxy myProxy = new WebProxy();
                myProxy.Address = new Uri(proxy);
                httpRequest.Proxy = myProxy;
                //httpRequest.Timeout = 20000;
                HttpWebResponse web = (HttpWebResponse)httpRequest.GetResponse();
                Stream webstream = web.GetResponseStream();
                statusCode = "HTTP " + (int)web.StatusCode;
                encode = web.Headers["charset"];
                encode = web.Headers["Content-Type"];
                if (!string.IsNullOrEmpty(encode))
                {
                    int pos = encode.IndexOf("charset=") + "charset=".Length;
                    encode = encode.Substring(pos, encode.Length - pos);
                }
                if (web.Headers["Content-Encoding"] != null && web.Headers["Content-Encoding"].ToLower().IndexOf("gzip") != -1)
                    _returnData = GZipDecompress(webstream);
                else
                {
                    using (StreamReader sr = new StreamReader(webstream, System.Text.Encoding.Default))
                    {
                        _returnData = sr.ReadToEnd();
                    }
                }
                webstream.Close();
                web.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            return _returnData;
        }
        public string GetData(string _url, string _cookie, string referer)
        {
            string _returnData = "";
            _cookie = _cookie.Replace(';', ',');
            _cookie = _cookie.TrimStart(',');
            try
            {
                WebRequest webRequest = null;
                HttpWebRequest httpRequest = null;
                Uri uri = new Uri(_url);
                webRequest = WebRequest.Create(uri);
                httpRequest = webRequest as HttpWebRequest;
                httpRequest.CookieContainer = new CookieContainer();
                httpRequest.CookieContainer.SetCookies(uri, _cookie);
                httpRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
                httpRequest.Headers.Add("Accept-Encoding", "gzip,deflate");//压缩
                httpRequest.Headers.Add("Cache-Control", "max-age=0");
                httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";// : "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
                httpRequest.ContentType = "text/html; charset=gb2312";
                httpRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8, */*";//各类图片匹配
                httpRequest.Method = "GET";
                httpRequest.Referer = referer;
                httpRequest.AllowAutoRedirect = false;
                httpRequest.Timeout = 5*1000;
                //httpRequest.Timeout = 20000;
                HttpWebResponse web = (HttpWebResponse)httpRequest.GetResponse();
                Stream webstream = web.GetResponseStream();
                if (web.Headers["Content-Encoding"] != null && web.Headers["Content-Encoding"].ToLower().IndexOf("gzip") != -1)
                    _returnData = GZipDecompress(webstream);
                else if(web.Headers["Content-Type"] != null )
                {
                    int index = web.Headers["Content-Type"].IndexOf("utf-8");
                    if(index >= 0)
                    {
                        using (StreamReader sr = new StreamReader(webstream, System.Text.Encoding.UTF8))
                        {
                            _returnData = sr.ReadToEnd();
                        }
                    }
                    else
                    {
                        index = web.Headers["Content-Type"].IndexOf("gbk");
                        if(index >=0)
                        {
                            using (StreamReader sr = new StreamReader(webstream, System.Text.Encoding.ASCII))
                            {
                                _returnData = sr.ReadToEnd();
                            }
                        }
                    }
                }
                else
                {
                    using (StreamReader sr = new StreamReader(webstream, System.Text.Encoding.Default))
                    {
                        _returnData = sr.ReadToEnd();
                    }
                }
                webstream.Close();
                web.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            return _returnData;
        }
        public string GetData(string _url, string _cookie, string referer, String proxy)
        {
            string _returnData = "";
            _cookie = _cookie.Replace(';', ',');
            _cookie = _cookie.TrimStart(',');
            try
            {
                WebRequest webRequest = null;
                HttpWebRequest httpRequest = null;
                Uri uri = new Uri(_url);
                webRequest = WebRequest.Create(uri);
                httpRequest = webRequest as HttpWebRequest;
                httpRequest.CookieContainer = new CookieContainer();
                httpRequest.CookieContainer.SetCookies(uri, _cookie);
                httpRequest.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3");
                httpRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br");//压缩
                httpRequest.Headers.Add("Cache-Control", "gzip, deflate, br");//压缩
                httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:49.0) Gecko/20100101 Firefox/49.0";// : "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
                httpRequest.ContentType = "text/html; charset=gb2312";
                httpRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8, */*";//各类图片匹配
                httpRequest.Method = "GET";
                httpRequest.KeepAlive = true;
                //httpRequest.Referer = referer;
                httpRequest.AllowAutoRedirect = false;
                httpRequest.Timeout = 10 * 1000;

                if(proxy != null && proxy.Length > 0)
                {
                    WebProxy myProxy = new WebProxy();
                    myProxy.Address = new Uri(proxy);
                    httpRequest.Proxy = myProxy;
                }

                //httpRequest.Timeout = 20000;
                HttpWebResponse web = (HttpWebResponse)httpRequest.GetResponse();
                Stream webstream = web.GetResponseStream();
                if (web.Headers["Content-Encoding"] != null && web.Headers["Content-Encoding"].ToLower().IndexOf("gzip") != -1)
                    _returnData = GZipDecompress(webstream);
                else if (web.Headers["Content-Type"] != null)
                {
                    int index = web.Headers["Content-Type"].IndexOf("utf-8");
                    if (index >= 0)
                    {
                        using (StreamReader sr = new StreamReader(webstream, System.Text.Encoding.UTF8))
                        {
                            _returnData = sr.ReadToEnd();
                        }
                    }
                    else
                    {
                        index = web.Headers["Content-Type"].IndexOf("gbk");
                        if (index >= 0)
                        {
                            using (StreamReader sr = new StreamReader(webstream, System.Text.Encoding.ASCII))
                            {
                                _returnData = sr.ReadToEnd();
                            }
                        }
                    }
                }
                else
                {
                    using (StreamReader sr = new StreamReader(webstream, System.Text.Encoding.Default))
                    {
                        _returnData = sr.ReadToEnd();
                    }
                }
                webstream.Close();
                web.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            return _returnData;
        }
        public string GetData(string _url, string _cookie, string referer,int timeout)
        {
            string _returnData = "";
            try
            {
                WebRequest webRequest = null;
                HttpWebRequest httpRequest = null;
                Uri uri = new Uri(_url);
                webRequest = WebRequest.Create(uri);
                httpRequest = webRequest as HttpWebRequest;
                httpRequest.CookieContainer = new CookieContainer();
                httpRequest.CookieContainer.SetCookies(uri, _cookie);
                httpRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
                httpRequest.Headers.Add("Accept-Encoding", "gzip,deflate");//压缩
                httpRequest.Headers.Add("Cache-Control", "max-age=0");
                httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";// : "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
                httpRequest.ContentType = "text/html; charset=gb2312";
                httpRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8, */*";//各类图片匹配
                httpRequest.Method = "GET";
                httpRequest.Referer = referer;
                httpRequest.AllowAutoRedirect = false;
                httpRequest.Timeout = timeout;
                HttpWebResponse web = (HttpWebResponse)httpRequest.GetResponse();
                Stream webstream = web.GetResponseStream();
                if (web.Headers["Content-Encoding"] != null && web.Headers["Content-Encoding"].ToLower().IndexOf("gzip") != -1)
                    _returnData = GZipDecompress(webstream);
                else
                {
                    using (StreamReader sr = new StreamReader(webstream, System.Text.Encoding.Default))
                    {
                        _returnData = sr.ReadToEnd();
                    }
                }
                webstream.Close();
                web.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            return _returnData;
        }
        public string GetData(string _url, ref string _cookie, bool isAddCooke)
        {
            string _returnData = "";
            try
            {
                WebRequest webRequest = null;
                HttpWebRequest httpRequest = null;
                Uri uri = new Uri(_url);
                webRequest = WebRequest.Create(uri);
                httpRequest = webRequest as HttpWebRequest;
                httpRequest.CookieContainer = new CookieContainer();
                if (!String.IsNullOrEmpty(_cookie))
                    httpRequest.CookieContainer.SetCookies(uri, _cookie);
                httpRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
                //httpRequest.Headers.Add("referer:http://ftp6g.ibcbet.com/UnderOver.aspx?Market=t&DispVer=new");
                //httpRequest.Headers.Add("Accept-Encoding", "gzip,deflate;bzip2,sdch");//压缩

                httpRequest.Headers.Add("Cache-Control", "max-age=0");
                httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";// : "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
                httpRequest.ContentType = "text/html; charset=gb2312";
                httpRequest.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, */*";//各类图片匹配
                httpRequest.Method = "GET";
                httpRequest.AllowAutoRedirect = false;
                //httpRequest.Timeout = 20000;
                HttpWebResponse web = (HttpWebResponse)httpRequest.GetResponse();
                string tmpcookie = string.Empty;
                if (web.Headers["Set-Cookie"] != null)
                {
                    if (web.StatusCode == HttpStatusCode.Redirect)
                        tmpcookie = web.Headers["Set-Cookie"].Replace("HttpOnly%2c", "");
                    else
                        tmpcookie = web.Headers["Set-Cookie"].Replace("HttpOnly%2c", "");
                }
                Stream webstream = web.GetResponseStream();
                if (isAddCooke)
                    _cookie += "," + tmpcookie;
                else
                    _cookie = tmpcookie;
                _cookie = _cookie.TrimEnd(',');
                using (StreamReader sr = new StreamReader(webstream, System.Text.Encoding.Default))
                {
                    _returnData = sr.ReadToEnd();
                }
                webstream.Close();
                web.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            return _returnData;
        }
        public string GetData(string _url, ref string _cookie, bool isAddCooke, bool allowAutoRedirect)
        {
            string _returnData = "";
            try
            {
                WebRequest webRequest = null;
                HttpWebRequest httpRequest = null;
                Uri uri = new Uri(_url);
                webRequest = WebRequest.Create(uri);
                httpRequest = webRequest as HttpWebRequest;
                httpRequest.CookieContainer = new CookieContainer();
                httpRequest.CookieContainer.SetCookies(uri, _cookie);
                httpRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
                //httpRequest.Headers.Add("referer:http://ftp6g.ibcbet.com/UnderOver.aspx?Market=t&DispVer=new");
                //httpRequest.Headers.Add("Accept-Encoding", "gzip,deflate;bzip2,sdch");//压缩

                httpRequest.Headers.Add("Cache-Control", "max-age=0");
                httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";// : "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
                httpRequest.ContentType = "text/html; charset=gb2312";
                httpRequest.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, */*";//各类图片匹配
                httpRequest.Method = "GET";
                httpRequest.AllowAutoRedirect = allowAutoRedirect;
                //httpRequest.Timeout = 20000;
                HttpWebResponse web = (HttpWebResponse)httpRequest.GetResponse();
                string tmpcookie = string.Empty;
                if (web.Headers["Set-Cookie"] != null)
                {
                    if (web.StatusCode == HttpStatusCode.Redirect)
                        tmpcookie = web.Headers["Set-Cookie"].Replace("HttpOnly%2c", "");
                    else
                        tmpcookie = web.Headers["Set-Cookie"].Replace("HttpOnly%2c", "");
                }
                Stream webstream = web.GetResponseStream();
                if (isAddCooke)
                    _cookie += "," + tmpcookie;
                else
                    _cookie = tmpcookie;
                _cookie = _cookie.TrimEnd(',');
                using (StreamReader sr = new StreamReader(webstream, System.Text.Encoding.Default))
                {
                    _returnData = sr.ReadToEnd();
                }
                webstream.Close();
                web.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            return _returnData;
        }
        public string GetData(string _url, ref string _cookie, bool isAddCooke, string referer)
        {
            string _returnData = "";

            _cookie = _cookie.Replace(';', ',');
            _cookie = _cookie.TrimStart(',');
            try
            {
                WebRequest webRequest = null;
                HttpWebRequest httpRequest = null;
                Uri uri = new Uri(_url);
                webRequest = WebRequest.Create(uri);
                httpRequest = webRequest as HttpWebRequest;
                httpRequest.CookieContainer = new CookieContainer();
                httpRequest.CookieContainer.SetCookies(uri, _cookie);
                httpRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
                //httpRequest.Headers.Add("referer:http://ftp6g.ibcbet.com/UnderOver.aspx?Market=t&DispVer=new");
                //httpRequest.Headers.Add("Accept-Encoding", "gzip,deflate;bzip2,sdch");//压缩

                httpRequest.Headers.Add("Cache-Control", "max-age=0");
                httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";// : "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
                httpRequest.ContentType = "text/html; charset=gb2312";
                httpRequest.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, */*";//各类图片匹配
                httpRequest.Method = "GET";
                httpRequest.Referer = referer;
                httpRequest.AllowAutoRedirect = false;
                //httpRequest.Timeout = 20000;
                HttpWebResponse web = (HttpWebResponse)httpRequest.GetResponse();
                string tmpcookie = string.Empty;
                if (web.Headers["Set-Cookie"] != null)
                {
                    if (web.StatusCode == HttpStatusCode.Redirect)
                        tmpcookie = web.Headers["Set-Cookie"].Replace("HttpOnly%2c", "");
                    else
                        tmpcookie = web.Headers["Set-Cookie"].Replace("HttpOnly%2c", "");
                }
                Stream webstream = web.GetResponseStream();
                if (isAddCooke)
                    _cookie += "," + tmpcookie;
                else
                    _cookie = tmpcookie;
                _cookie = _cookie.TrimEnd(',');
                using (StreamReader sr = new StreamReader(webstream, System.Text.Encoding.Default))
                {
                    _returnData = sr.ReadToEnd();
                }
                webstream.Close();
                web.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            return _returnData;
        }
        /// <summary>
        /// 获取页面的SessionID值
        /// </summary>
        /// <param name="url">页面地址</param>
        /// <returns>SessionID</returns>
        public string GetHtmlSession(string url)
        {
            WebRequest webRequest = WebRequest.Create(new Uri(url));
            webRequest.Credentials = CredentialCache.DefaultCredentials;
            HttpWebRequest httpWebRequest = webRequest as HttpWebRequest;
            httpWebRequest.KeepAlive = true;
            httpWebRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
            //httpWebRequest.Headers.Add("Accept-Encoding", "gzip,deflate;bzip2,sdch");
            httpWebRequest.Headers.Add("Pragma", "no-cache");
            httpWebRequest.Headers.Add("Accept-Charset", "GB2312,utf-8;q=0.7,*;q=0.7");
            //httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.1.5) Gecko/20091102 Firefox/3.5.5 (.NET CLR 3.5.30729)";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            httpWebRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
            httpWebRequest.AllowAutoRedirect = false;
            httpWebRequest.Timeout = 20000;
            WebResponse webResponse = httpWebRequest.GetResponse();
            string cookie = webResponse.Headers.Get("Set-Cookie");
            return cookie;
        }
        public Stream GetHtmlByBytes(string _url, string _data, string _cookie)
        {
            Uri uri = new Uri(_url);//(betUrl, true);
            #region 创建httpWebRequest对象
            WebRequest webRequest = WebRequest.Create(uri);
            HttpWebRequest httpRequest = webRequest as HttpWebRequest;
            if (httpRequest == null)
            {
                throw new ApplicationException(string.Format("Invalid url string: {0}", _url));
            }
            #endregion

            #region 填充httpWebRequest的基本信息
            HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
            httpRequest.CachePolicy = policy;
            httpRequest.CookieContainer = new CookieContainer();
            //httpRequest.Timeout = 20000;
            httpRequest.CookieContainer.SetCookies(uri, _cookie);
            httpRequest.KeepAlive = true;
            httpRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
            //httpRequest.Headers.Add("Accept-Encoding", "gzip,deflate;bzip2,sdch");
            httpRequest.Headers.Add("Pragma", "no-cache");

            httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";// : "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            httpRequest.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, */*";//各类图片匹配
            #endregion

            #region post内容
            byte[] postData = Encoding.UTF8.GetBytes(_data);
            httpRequest.Method = "POST";
            httpRequest.ContentLength = postData.Length;
            Stream _Stream = httpRequest.GetRequestStream();
            _Stream.Flush();
            _Stream.Write(postData, 0, postData.Length);
            _Stream.Close();
            #endregion

            #region 发送post请求到服务器并读取服务器返回信息
            HttpWebResponse hwResponse = (HttpWebResponse)httpRequest.GetResponse();

            Stream responseStream = hwResponse.GetResponseStream();

            #endregion
            return responseStream;

        }
        public Stream GetHtmlByBytes(string _url, string _cookie)
        {
            Uri uri = new Uri(_url);//(betUrl, true);
            #region 创建httpWebRequest对象
            WebRequest webRequest = WebRequest.Create(uri);
            HttpWebRequest httpRequest = webRequest as HttpWebRequest;
            if (httpRequest == null)
            {
                throw new ApplicationException(string.Format("Invalid url string: {0}", _url));
            }
            #endregion

            #region 填充httpWebRequest的基本信息
            HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
            httpRequest.CachePolicy = policy;
            httpRequest.CookieContainer = new CookieContainer();
            httpRequest.CookieContainer.SetCookies(uri, _cookie);
            httpRequest.KeepAlive = true;
            //httpRequest.Timeout =10000;
            httpRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
            //httpRequest.Headers.Add("Accept-Encoding", "gzip,deflate;bzip2,sdch");
            httpRequest.Headers.Add("Pragma", "no-cache");

            httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";// : "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            httpRequest.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, */*";//各类图片匹配
            httpRequest.Method = "GET";
            #endregion

            #region 发送post请求到服务器并读取服务器返回信息
            HttpWebResponse hwResponse = (HttpWebResponse)httpRequest.GetResponse();
            Stream responseStream = hwResponse.GetResponseStream();

            #endregion
            return responseStream;

        }

        public string GetDataAuto(string _domain, string _path, ref string _cookie)
        {
            string _url = _domain + _path;
            string _returnData = "";
            WebRequest webRequest = null;
            HttpWebRequest httpRequest = null;
            Uri uri = new Uri(_url);
            webRequest = WebRequest.Create(uri);
            httpRequest = webRequest as HttpWebRequest;
            httpRequest.CookieContainer = new CookieContainer();
            httpRequest.CookieContainer.SetCookies(uri, _cookie);
            httpRequest.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
            //httpRequest.Headers.Add("referer:http://ftp6g.ibcbet.com/UnderOver.aspx?Market=t&DispVer=new");
            //httpRequest.Headers.Add("Accept-Encoding", "gzip,deflate;bzip2,sdch");//压缩

            httpRequest.Headers.Add("Cache-Control", "max-age=0");
            httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";// : "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.2; Trident/4.0; .NET";
            httpRequest.ContentType = "text/html; charset=gb2312";
            httpRequest.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, */*";//各类图片匹配
            httpRequest.Method = "GET";
            httpRequest.AllowAutoRedirect = false;
            //httpRequest.Timeout = 20000;
            HttpWebResponse web = (HttpWebResponse)httpRequest.GetResponse();
            string tmpcookie = string.Empty;
            if (web.Headers["Set-Cookie"] != null)
            {
                if (web.StatusCode == HttpStatusCode.Redirect)
                    tmpcookie = web.Headers["Set-Cookie"].Replace("HttpOnly%2c", "");
                else
                    tmpcookie = web.Headers["Set-Cookie"].Replace("HttpOnly%2c", "");
            }
            string location = web.Headers["Location"] != null ? web.Headers["Location"] : "";
            Stream webstream = web.GetResponseStream();
            _cookie += "," + tmpcookie;
            _cookie = _cookie.TrimEnd(',');
            using (StreamReader sr = new StreamReader(webstream, System.Text.Encoding.Default))
            {
                _returnData = sr.ReadToEnd();
            }
            webstream.Close();
            web.Close();
            if (!string.IsNullOrEmpty(location))
                return GetDataAuto(_domain, location, ref _cookie);
            return _returnData;
        }
        /// <summary>
        /// 获取压缩后的数据
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public string GZipDecompress(Stream stream)
        {
            byte[] buffer = new byte[40960];
            int length = 0;
            using (GZipStream gz = new GZipStream(stream, CompressionMode.Decompress))
            {
                using (MemoryStream msTemp = new MemoryStream())
                {
                    while ((length = gz.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        msTemp.Write(buffer, 0, length);
                    }

                    return System.Text.Encoding.UTF8.GetString(msTemp.ToArray());
                }
            }
            return "";
        }


        /// <summary>
        /// 以Post 形式提交数据到 uri
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="files"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public byte[] PostData(string uri, IEnumerable<UploadFile> files, NameValueCollection values)
        {
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            request.Credentials = CredentialCache.DefaultCredentials;


            MemoryStream stream = new MemoryStream();

            byte[] line = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            /*//提交文本字段
            if (values != null)
            {
                string format = "{0}={1}&";
                foreach (string key in values.Keys)
                {
                    string s = string.Format(format, key, values[key]);
                    byte[] data = Encoding.UTF8.GetBytes(s);
                    stream.Write(data, 0, data.Length);
                }
                stream.Write(line, 0, line.Length);
            }*/

            //提交文本字段
            if (values != null)
            {
                string format = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";
                foreach (string key in values.Keys)
                {
                    string s = string.Format(format, key, values[key]);
                    byte[] data = Encoding.UTF8.GetBytes(s);
                    stream.Write(data, 0, data.Length);
                }
                stream.Write(line, 0, line.Length);
            }

            //提交文件
            if (files != null)
            {
                string fformat = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n Content-Type: application/octet-stream\r\n\r\n";
                foreach (UploadFile file in files)
                {
                    string s = string.Format(fformat, file.Name, file.Filename);
                    byte[] data = Encoding.UTF8.GetBytes(s);
                    stream.Write(data, 0, data.Length);



                    stream.Write(file.Data, 0, file.Data.Length);
                    stream.Write(line, 0, line.Length);
                }
            }


            request.ContentLength = stream.Length;


            Stream requestStream = request.GetRequestStream();

            stream.Position = 0L;
            stream.CopyTo(requestStream);
            stream.Close();

            requestStream.Close();



            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var mstream = new MemoryStream())
            {
                responseStream.CopyTo(mstream);
                return mstream.ToArray();
            }
        }
    }
    /// <summary>
    /// 上传文件
    /// </summary>
    public class UploadFile
    {
        public UploadFile()
        {
            ContentType = "application/octet-stream";
        }
        public string Name { get; set; }
        public string Filename { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
    }
}
