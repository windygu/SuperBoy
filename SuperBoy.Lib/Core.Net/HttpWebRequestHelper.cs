using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace Core.Net
{
    /// <summary>
    /// 网页抓取帮助
    /// </summary>
    public class HttpWebRequestHelper
    {
        #region 构造函数

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cookie"></param>
        public HttpWebRequestHelper(CookieContainer cookie)
        {
            _cookieContainer = cookie;
        }

        /// <summary>
        /// 
        /// </summary>
        public HttpWebRequestHelper()
        {
            _cookieContainer = new CookieContainer();
        }

        #endregion

        /// <summary>
        /// cookie集合 
        /// </summary>
        private CookieContainer _cookieContainer;

        /// <summary>
        /// 获取页面html   encodingname:gb2312
        /// </summary>
        /// <param name="uri">访问url</param>
        /// <returns></returns>
        public string Get(string uri)
        {
            return Get(uri, uri, "gb2312");
        }

        /// <summary>
        /// 获取页面html   encodingname:gb2312
        /// </summary>
        /// <param name="uri">访问url</param>
        /// <param name="refererUri">来源url</param>
        /// <returns></returns>
        public string Get(string uri, string refererUri)
        {
            return Get(uri, refererUri, "gb2312");
        }

        /// <summary>
        /// 获取页面html
        /// </summary>
        /// <param name="uri">访问url</param>
        /// <param name="refererUri">来源url</param>
        /// <param name="encodingName">编码名称  例如：gb2312</param>
        /// <returns></returns>
        public string Get(string uri, string refererUri, string encodingName)
        {
            return Get(uri, refererUri, encodingName, (WebProxy)null);
        }

        /// <summary>
        /// 获取页面html
        /// </summary>
        /// <param name="uri">访问url</param>
        /// <param name="refererUri">来源url</param>
        /// <param name="encodingName">编码名称  例如：gb2312</param>
        /// <param name="webproxy">代理</param>
        /// <returns></returns>
        public string Get(string uri, string refererUri, string encodingName, WebProxy webproxy)
        {
            var html = string.Empty;

            var request = (HttpWebRequest)WebRequest.Create(uri);

            request.ContentType = "text/html;charset=" + encodingName;
            request.Method = "Get";
            request.CookieContainer = _cookieContainer;

            if (null != webproxy)
            {
                request.Proxy = webproxy;
                if (null != webproxy.Credentials)
                    request.UseDefaultCredentials = true;
            }

            if (!string.IsNullOrEmpty(refererUri))
                request.Referer = refererUri;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var streamResponse = response.GetResponseStream())
                {
                    using (var streamResponseReader = new StreamReader(streamResponse, Encoding.GetEncoding(encodingName)))
                    {
                        html = streamResponseReader.ReadToEnd();
                    }
                }
            }

            return html;

        }

        /// <summary>
        /// 获取文件或图片 （验证码）
        /// </summary>
        /// <param name="uri">访问url</param>
        /// <returns></returns>
        public Byte[] GetBytes(string uri)
        {
            return GetBytes(uri, uri);
        }

        /// <summary>
        /// 获取文件或图片 （验证码）
        /// </summary>
        /// <param name="uri">访问url</param>
        /// <param name="refererUri">来源url</param>
        /// <returns></returns>
        public Byte[] GetBytes(string uri, string refererUri)
        {
            return GetBytes(uri, refererUri, (WebProxy)null);
        }

        /// <summary>
        /// 获取文件或图片 （验证码）
        /// </summary>
        /// <param name="uri">访问url</param>
        /// <param name="refererUri">来源url</param>
        /// <returns></returns>
        public Byte[] GetBytes(string uri, string refererUri, WebProxy webproxy)
        {
            var buffer = new byte[1024];

            using (var responseStream = GetStream(uri, refererUri, webproxy))
            {
                using (var memoryStream = new MemoryStream())
                {
                    var count = 0;
                    do
                    {
                        count = responseStream.Read(buffer, 0, buffer.Length);
                        memoryStream.Write(buffer, 0, count);

                    } while (count != 0);

                    return memoryStream.ToArray();
                }
            }
        }


        /// <summary>
        /// 获取文件或图片 （验证码）
        /// </summary>
        /// <param name="uri">访问url</param>
        /// <returns></returns>
        public Stream GetStream(string uri)
        {
            return GetStream(uri, uri);
        }

        /// <summary>
        /// 获取文件或图片 （验证码）
        /// </summary>
        /// <param name="uri">访问url</param>
        /// <param name="refererUri">来源url</param>
        /// <returns></returns>
        public Stream GetStream(string uri, string refererUri)
        {
            return GetStream(uri, refererUri, (WebProxy)null);
        }

        /// <summary>
        /// 获取文件或图片 （验证码）
        /// </summary>
        /// <param name="uri">访问url</param>
        /// <param name="refererUri">来源url</param>
        /// <param name="webproxy">代理</param>
        /// <returns></returns>
        public Stream GetStream(string uri, string refererUri, WebProxy webproxy)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);

            request.Method = "GET";
            request.CookieContainer = _cookieContainer;
            if (null != webproxy)
            {
                request.Proxy = webproxy;
                if (null != webproxy.Credentials)
                    request.UseDefaultCredentials = true;
            }

            if (!string.IsNullOrEmpty(refererUri))
                request.Referer = refererUri;

            using (var reponse = (HttpWebResponse)request.GetResponse())
            {
                return reponse.GetResponseStream();
            }
        }

        /// <summary>
        /// POST提交   模拟xp IE7.0     默认GB2312
        /// </summary>
        /// <param name="uri">访问url</param>
        /// <param name="postData">提交的数据</param>
        /// <returns></returns>
        public string Post(string uri, string postData)
        {
            return Post(uri, uri, postData, "gb2312");
        }

        /// <summary>
        /// POST提交   模拟xp IE7.0     默认GB2312
        /// </summary>
        /// <param name="uri">访问url</param>
        /// <param name="refererUri">来源url</param>
        /// <param name="postData">提交的数据</param>
        /// <returns></returns>
        public string Post(string uri, string refererUri, string postData)
        {
            return Post(uri, refererUri, postData, "gb2312");
        }

        /// <summary>
        /// POST提交    模拟xp IE7.0
        /// </summary>
        /// <param name="uri">访问url</param>
        /// <param name="refererUri">来源url</param>
        /// <param name="postData">提交的数据</param>
        /// <param name="encodingName">编码名称  例如：gb2312</param>
        /// <returns></returns>
        public string Post(string uri, string refererUri, string postData, string encodingName)
        {
            return Post(uri, refererUri, postData, encodingName, (WebProxy)null);
        }

        /// <summary>
        /// POST提交    模拟xp IE7.0
        /// </summary>
        /// <param name="uri">访问url</param>
        /// <param name="refererUri">来源url</param>
        /// <param name="postData">提交的数据</param>
        /// <param name="encodingName">编码名称  例如：gb2312</param>
        /// <param name="webproxy">代理</param>
        /// <returns></returns>
        public string Post(string uri, string refererUri, string postData, string encodingName, WebProxy webproxy)
        {
            var html = string.Empty;

            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Accept = "*/*";
            request.Headers.Add("Accept-Language", "zh-cn");
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("UA-CPU", "x86");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.UserAgent = @"Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET CLR 1.1.4322; .NET CLR 3.0.04506.30)";
            request.Headers.Add("Cache-Control", "no-cache");
            request.CookieContainer = _cookieContainer;
            request.Method = "POST";
            if (!string.IsNullOrEmpty(refererUri))
                request.Referer = refererUri;
            if (null != webproxy)
            {
                request.Proxy = webproxy;
                if (null != webproxy.Credentials)
                    request.UseDefaultCredentials = true;
            }
            var encode = Encoding.GetEncoding(encodingName);
            var bytesSend = encode.GetBytes(postData);
            request.ContentLength = bytesSend.Length;

            var cookieTextSend = new StringBuilder();
            if (_cookieContainer != null)
            {
                var cc = _cookieContainer.GetCookies(request.RequestUri);

                foreach (System.Net.Cookie cokie in cc)
                {
                    cookieTextSend.Append(cokie + ";");
                }
            }
            if (cookieTextSend.Length > 0)
            {
                long time;
                time = DateTime.UtcNow.Ticks - (new DateTime(1970, 1, 1)).Ticks;
                time = (long)(time / 10000);
                request.Headers.Add("Cookie", string.Concat("cookLastGetMsgTime=", time.ToString(), "; ", cookieTextSend.ToString()));
            }

            using (var streamSend = request.GetRequestStream())
            {
                streamSend.Write(bytesSend, 0, bytesSend.Length);
            }

            using (var reponse = (HttpWebResponse)request.GetResponse())
            {
                using (var streamRespone = reponse.GetResponseStream())
                {
                    using (var streamReaderResponse = new StreamReader(streamRespone, encode))
                    {
                        html = streamReaderResponse.ReadToEnd();
                    }
                }
            }

            return html;
        }
    }
}
