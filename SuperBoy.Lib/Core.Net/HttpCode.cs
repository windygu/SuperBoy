/*
 * 玄机网C# 基类库 Http请求类
 * 基础功能1:基于HttpWebRequest封装的 同步/异步 (Get/Post)
 * 基础功能2:基于Wininet系统API封装的 同步(Get/Post)
 * 基于以上功能实现的一键请求类,让你摆脱Cookie的困扰,让你解除对多线程的恐惧,. 
 * Coding by 君临
 * 09-27/2014
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Collections;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.IO.Compression;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Reflection;

namespace HttpCodeLib
{
    partial class HttpRequest
    {
        public const int DefaultTimeOutSpan = 30 * 1000;
        public HttpResults Result = new HttpResults();
        //HttpWebRequest对象用来发起请求
        public HttpWebRequest Request = null;
        //获取影响流的数据对象
        public HttpWebResponse Response = null;
        //响应流对象
        public Stream StreamResponse;
        public Action<HttpResults> CallBack;
        public HttpItems ObjHttpCodeItem;
        public MemoryStream MemoryStream = new MemoryStream();
        public int MSemaphore = 0;
        //默认的编码
        public Encoding Encoding;
    }
    /// <summary>
    /// Http连接操作帮助类 
    /// </summary>
    public class HttpHelpers
    {
        #region 预定义方法或者变更
        //默认的编码
        private Encoding _encoding = Encoding.Default;
        //HttpWebRequest对象用来发起请求
        private HttpWebRequest _request = null;
        //获取影响流的数据对象
        private HttpWebResponse _response = null;
        /// <summary>
        /// 根据相传入的数据，得到相应页面数据
        /// </summary>
        /// <param name="strPostdata">传入的数据Post方式,get方式传NUll或者空字符串都可以</param>
        /// <returns>string类型的响应数据</returns>
        private HttpResults GetHttpRequestData(HttpItems objHttpItems)
        {
            //返回参数
            var result = new HttpResults();
            try
            {
                #region 得到请求的response
                result.CookieCollection = new CookieCollection();
                _response = (HttpWebResponse)_request.GetResponse();

                result.Header = _response.Headers;
                if (_response.Cookies != null)
                {
                    result.CookieCollection = _response.Cookies;
                }
                if (_response.Headers["set-cookie"] != null)
                {
                    result.Cookie = _response.Headers["set-cookie"];
                }
                //处理返回值Container
                result.Container = objHttpItems.Container;
                var stream = new MemoryStream();
                //GZIIP处理
                if (_response.ContentEncoding != null && _response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                {
                    stream = GetMemoryStream(new GZipStream(_response.GetResponseStream(), CompressionMode.Decompress));
                }
                else
                {
                    stream = GetMemoryStream(_response.GetResponseStream());

                }
                //获取Byte
                var rawResponse = stream.ToArray();
                //是否返回Byte类型数据
                if (objHttpItems.ResultType == ResultType.Byte)
                {
                    result.ResultByte = rawResponse;
                    return result;
                }
                //无视编码
                if (_encoding == null)
                {
                    var temp = Encoding.Default.GetString(rawResponse, 0, rawResponse.Length);
                    //<meta(.*?)charset([\s]?)=[^>](.*?)>
                    var meta = Regex.Match(temp, "<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    var charter = (meta.Groups.Count > 2) ? meta.Groups[2].Value : string.Empty;
                    charter = charter.Replace("\"", string.Empty).Replace("'", string.Empty).Replace(";", string.Empty);
                    if (charter.Length > 0)
                    {
                        charter = charter.ToLower().Replace("iso-8859-1", "gbk");
                        _encoding = Encoding.GetEncoding(charter);
                    }
                    else
                    {

                        if (_response.CharacterSet != null)
                        {
                            if (_response.CharacterSet.ToLower().Trim() == "iso-8859-1")
                            {
                                _encoding = Encoding.GetEncoding("gbk");
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(_response.CharacterSet.Trim()))
                                {
                                    _encoding = Encoding.UTF8;
                                }
                                else
                                {
                                    _encoding = Encoding.GetEncoding(_response.CharacterSet);
                                }
                            }
                        }
                    }
                }
                //得到返回的HTML
                try
                {
                    if (rawResponse.Length > 0)
                    {
                        result.Html = _encoding.GetString(rawResponse);
                    }
                    else
                    {
                        result.Html = "";
                    }
                    stream.Close();
                    _response.Close();
                }
                catch
                {
                    return null;
                }
                //最后释放流


                #endregion


            }
            catch (WebException ex)
            {
                //这里是在发生异常时返回的错误信息
                result.Html = "String Error";
                _response = (HttpWebResponse)ex.Response;
                return result;
            }
            if (objHttpItems.IsToLower)
            {
                result.Html = result.Html.ToLower();
            }
            return result;
        }
        private void AsyncResponseData(IAsyncResult result)
        {
            var hrt = result.AsyncState as HttpRequest;
            if (System.Threading.Interlocked.Increment(ref hrt.MSemaphore) != 1)
                return;
            try
            {
                hrt.Response = (HttpWebResponse)hrt.Request.EndGetResponse(result);
                if (hrt.Response.ContentEncoding != null && hrt.Response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                {
                    hrt.StreamResponse = new GZipStream(hrt.Response.GetResponseStream(), CompressionMode.Decompress);
                }
                else
                {
                    hrt.StreamResponse = hrt.Response.GetResponseStream();
                }
                hrt.MemoryStream = GetMemoryStream(hrt.StreamResponse);
                AsyncCallBackData(hrt);
            }
            catch (Exception ex)
            {
                hrt.Result.Html = ex.Message;
                AsyncCallBackData(hrt);
            }
        }
        /// <summary>
        /// 无视编码
        /// </summary>
        /// <param name="hrt">请求参数</param>
        /// <param name="rawResponse">响应值</param>
        /// <returns></returns>
        HttpRequest GetEncoding(HttpRequest hrt, ref byte[] rawResponse)
        {
            if (hrt.Encoding == null)
            {
                var temp = Encoding.Default.GetString(rawResponse, 0, rawResponse.Length);
                var meta = Regex.Match(temp, "<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                var charter = (meta.Groups.Count > 2) ? meta.Groups[2].Value : string.Empty;
                charter = charter.Replace("\"", string.Empty).Replace("'", string.Empty).Replace(";", string.Empty);
                if (charter.Length > 0)
                {
                    charter = charter.ToLower().Replace("iso-8859-1", "gbk");
                    hrt.Encoding = Encoding.GetEncoding(charter);
                }
                else
                {

                    if (hrt.Response.CharacterSet != null)
                    {
                        if (hrt.Response.CharacterSet.ToLower().Trim() == "iso-8859-1")
                        {
                            hrt.Encoding = Encoding.GetEncoding("gbk");
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(hrt.Response.CharacterSet.Trim()))
                            {
                                hrt.Encoding = Encoding.UTF8;
                            }
                            else
                            {
                                hrt.Encoding = Encoding.GetEncoding(hrt.Response.CharacterSet);
                            }
                        }
                    }
                }
            }
            return hrt;
        }
        /// <summary>
        /// 处理/解析数据方法
        /// </summary>
        /// <param name="hrt"></param>
        void AsyncCallBackData(HttpRequest hrt)
        {
            try
            {
                var rawResponse = hrt.MemoryStream.ToArray();
                hrt.Result.CookieCollection = hrt.Response.Cookies;
                //无视编码
                hrt = GetEncoding(hrt, ref rawResponse);
                //是否返回Byte类型数据  
                if (hrt.ObjHttpCodeItem.ResultType == ResultType.Byte)
                {
                    hrt.Result.ResultByte = rawResponse;
                }
                //得到返回的HTML
                try
                {
                    hrt.Result.Html = Encoding.UTF8.GetString(rawResponse);
                    hrt.CallBack.Invoke(hrt.Result);
                }
                catch
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                hrt.Result.Html = ex.Message;
                hrt.CallBack.Invoke(hrt.Result);
            }
        }
        /// <summary>
        /// 根据相传入的数据，得到相应页面数据
        /// </summary>
        /// <param name="strPostdata">传入的数据Post方式,get方式传NUll或者空字符串都可以</param>
        /// <returns>string类型的响应数据</returns>
        private void AsyncGetHttpRequestData(HttpItems objItems, Action<HttpResults> callBack)
        {
            var hrt = new HttpRequest();
            SetRequest(objItems);
            hrt.ObjHttpCodeItem = objItems;
            hrt.Request = _request;
            hrt.CallBack = callBack;
            try
            {

                var mAr = hrt.Request.BeginGetResponse(AsyncResponseData, hrt);
                System.Threading.ThreadPool.RegisterWaitForSingleObject(mAr.AsyncWaitHandle,
                    TimeoutCallback, hrt, HttpRequest.DefaultTimeOutSpan, true);
            }
            catch (Exception ex)
            {
                hrt.Result.Html = "TimeOut";
            }
        }
        /// <summary>
        /// 超时回调
        /// </summary>
        /// <param name="state"></param>
        /// <param name="timedOut"></param>
        void TimeoutCallback(object state, bool timedOut)
        {
            var pa = state as HttpRequest;
            if (timedOut)
                if (System.Threading.Interlocked.Increment(ref pa.MSemaphore) == 1)
                    pa.Result.Html = "TimeOut";
        }
        /// <summary>
        /// 4.0以下.net版本取数据使用
        /// </summary>
        /// <param name="streamResponse">流</param>
        private MemoryStream GetMemoryStream(Stream streamResponse)
        {
            var stream = new MemoryStream();
            var length = 256;
            var buffer = new Byte[length];
            var bytesRead = streamResponse.Read(buffer, 0, length);
            // write the required bytes  
            while (bytesRead > 0)
            {
                stream.Write(buffer, 0, bytesRead);
                bytesRead = streamResponse.Read(buffer, 0, length);
            }
            return stream;
        }

        /// <summary>
        /// 为请求准备参数
        /// </summary>
        ///<param name="objHttpItems">参数列表</param>
        /// <param name="_Encoding">读取数据时的编码方式</param>
        private void SetRequest(HttpItems objHttpItems)
        {

            // 验证证书
            SetCer(objHttpItems);
            //设置Header参数
            if (objHttpItems.Header != null)
            {
                try
                {
                    _request.Headers = objHttpItems.Header;
                }
                catch
                {
                    return;
                }
            }
            if (objHttpItems.IsAjax)
            {
                _request.Headers.Add("x-requested-with: XMLHttpRequest");
            }
            // 设置代理
            SetProxy(objHttpItems);
            //请求方式Get或者Post
            _request.Method = objHttpItems.Method;
            _request.Timeout = objHttpItems.Timeout;
            _request.ReadWriteTimeout = objHttpItems.ReadWriteTimeout;
            //Accept
            _request.Accept = objHttpItems.Accept;
            //ContentType返回类型
            _request.ContentType = objHttpItems.ContentType;
            //UserAgent客户端的访问类型，包括浏览器版本和操作系统信息
            _request.UserAgent = objHttpItems.UserAgent;
            // 编码
            SetEncoding(objHttpItems);
            //设置Cookie
            SetCookie(objHttpItems);
            //来源地址
            _request.Referer = objHttpItems.Referer;
            //是否执行跳转功能
            _request.AllowAutoRedirect = objHttpItems.Allowautoredirect;
            //设置Post数据
            SetPostData(objHttpItems);
            //设置最大连接
            if (objHttpItems.Connectionlimit > 0)
            {
                _request.ServicePoint.ConnectionLimit = objHttpItems.Connectionlimit;
            }
        }
        /// <summary>
        /// 设置证书
        /// </summary>
        /// <param name="objHttpItems"></param>
        private void SetCer(HttpItems objHttpItems)
        {
            if (!string.IsNullOrEmpty(objHttpItems.CerPath))
            {
                //这一句一定要写在创建连接的前面。使用回调的方法进行证书验证。
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                //初始化对像，并设置请求的URL地址
                _request = (HttpWebRequest)WebRequest.Create(GetUrl(objHttpItems.Url));
                //创建证书文件
                var objx509 = new X509Certificate(objHttpItems.CerPath);
                //添加到请求里
                _request.ClientCertificates.Add(objx509);
            }
            else
            {
                //初始化对像，并设置请求的URL地址
                try
                {
                    _request = (HttpWebRequest)WebRequest.Create(GetUrl(objHttpItems.Url));
                }
                catch
                {
                    return;
                }
            }
        }
        /// <summary>
        /// 设置编码
        /// </summary>
        /// <param name="objHttpItems">Http参数</param>
        private void SetEncoding(HttpItems objHttpItems)
        {
            if (string.IsNullOrEmpty(objHttpItems.Encoding) || objHttpItems.Encoding.ToLower().Trim() == "null")
            {
                //读取数据时的编码方式
                _encoding = null;
            }
            else
            {
                //读取数据时的编码方式
                _encoding = System.Text.Encoding.GetEncoding(objHttpItems.Encoding);
            }
        }
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="objHttpItems">Http参数</param>
        private void SetCookie(HttpItems objHttpItems)
        {
            //获取当前的cookie

            if (!string.IsNullOrEmpty(objHttpItems.Cookie))
            {
                //Cookie
                _request.Headers[HttpRequestHeader.Cookie] = objHttpItems.Cookie;
            }
            //设置Cookie

            if (objHttpItems.CookieCollection != null)
            {
                _request.CookieContainer = new CookieContainer();
                _request.CookieContainer.Add(objHttpItems.CookieCollection);
            }
            if (objHttpItems.Container != null)
            {
                _request.CookieContainer = objHttpItems.Container;
            }
        }

        /// <summary>
        /// 设置Post数据
        /// </summary>
        /// <param name="objHttpItems">Http参数</param>
        private void SetPostData(HttpItems objHttpItems)
        {
            //验证在得到结果时是否有传入数据
            if (_request.Method.Trim().ToLower().Contains("post"))
            {
                //写入Byte类型
                if (objHttpItems.PostDataType == PostDataType.Byte)
                {
                    //验证在得到结果时是否有传入数据
                    if (objHttpItems.PostdataByte != null && objHttpItems.PostdataByte.Length > 0)
                    {
                        _request.ContentLength = objHttpItems.PostdataByte.Length;
                        _request.GetRequestStream().Write(objHttpItems.PostdataByte, 0, objHttpItems.PostdataByte.Length);
                    }
                }//写入文件
                else if (objHttpItems.PostDataType == PostDataType.FilePath)
                {
                    var r = new StreamReader(objHttpItems.Postdata, _encoding);
                    var buffer = Encoding.Default.GetBytes(r.ReadToEnd());
                    r.Close();
                    _request.ContentLength = buffer.Length;
                    _request.GetRequestStream().Write(buffer, 0, buffer.Length);
                }
                else
                {
                    //验证在得到结果时是否有传入数据
                    if (!string.IsNullOrEmpty(objHttpItems.Postdata))
                    {
                        var buffer = Encoding.Default.GetBytes(objHttpItems.Postdata);
                        _request.ContentLength = buffer.Length;
                        _request.GetRequestStream().Write(buffer, 0, buffer.Length);
                    }
                }
            }
        }
        /// <summary>
        /// 设置代理
        /// </summary>
        /// <param name="objHttpItems">参数对象</param>
        private void SetProxy(HttpItems objHttpItems)
        {
            if (string.IsNullOrEmpty(objHttpItems.ProxyUserName) && string.IsNullOrEmpty(objHttpItems.ProxyPwd) && string.IsNullOrEmpty(objHttpItems.ProxyIp))
            {
                //不需要设置
            }
            else
            {
                //设置代理服务器
                var myProxy = new WebProxy(objHttpItems.ProxyIp, false);
                //建议连接
                myProxy.Credentials = new NetworkCredential(objHttpItems.ProxyUserName, objHttpItems.ProxyPwd);
                //给当前请求对象
                _request.Proxy = myProxy;
                //设置安全凭证
                _request.Credentials = CredentialCache.DefaultNetworkCredentials;
            }
        }
        /// <summary>
        /// 回调验证证书问题
        /// </summary>
        /// <param name="sender">流对象</param>
        /// <param name="certificate">证书</param>
        /// <param name="chain">X509Chain</param>
        /// <param name="errors">SslPolicyErrors</param>
        /// <returns>bool</returns>
        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            // 总是接受    
            return true;
        }
        #endregion
        #region 普通类型
        /// <summary>    
        /// 传入一个正确或不正确的URl，返回正确的URL
        /// </summary>    
        /// <param name="url">url</param>   
        /// <returns>
        /// </returns>
        public string GetUrl(string url)
        {
            if (!(url.Contains("http://") || url.Contains("https://")))
            {
                url = "http://" + url;
            }
            return url;
        }
        ///<summary>
        ///采用https协议访问网络,根据传入的URl地址，得到响应的数据字符串。
        ///</summary>
        ///<param name="objHttpItems">参数列表</param>
        ///<returns>String类型的数据</returns>
        public HttpResults GetHtml(HttpItems objHttpItems)
        {
            //准备参数
            SetRequest(objHttpItems);
            //调用专门读取数据的类
            return GetHttpRequestData(objHttpItems);
        }
        ///<summary>
        ///采用异步方式访问网络,根据传入的URl地址，得到响应的数据字符串。
        ///</summary>
        ///<param name="objHttpItems">参数列表</param>
        ///<returns>String类型的数据</returns>
        public void AsyncGetHtml(HttpItems objHttpItems, Action<HttpResults> callBack)
        {
            //调用专门读取数据的类
            AsyncGetHttpRequestData(objHttpItems, callBack);
        }
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="objHttpItems">参数列表</param>
        /// <returns>Img</returns>
        public Image GetImg(HttpResults hr)
        {

            return ByteArrayToImage(hr.ResultByte);

        }
        /// <summary>
        /// 字节数组生成图片
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns>图片</returns>
        private Image ByteArrayToImage(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                var outputImg = Image.FromStream(ms);
                return outputImg;
            }
        }
        #endregion
    }
    /// <summary>
    /// Http请求参考类 
    /// </summary>
    public class HttpItems
    {
        string _url;
        /// <summary>
        /// 请求URL必须填写
        /// </summary>
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }
        string _method = "GET";
        /// <summary>
        /// 请求方式默认为GET方式
        /// </summary>
        public string Method
        {
            get { return _method; }
            set { _method = value; }
        }
        int _timeout = 3000;
        /// <summary>
        /// 默认请求超时时间
        /// </summary>
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }
        int _readWriteTimeout = 30000;
        /// <summary>
        /// 默认写入Post数据超时间
        /// </summary>
        public int ReadWriteTimeout
        {
            get { return _readWriteTimeout; }
            set { _readWriteTimeout = value; }
        }
        string _accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*";
        /// <summary>
        /// 请求标头值 默认为image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/x-shockwave-flash, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/msword, */*
        /// </summary>
        public string Accept
        {
            get { return _accept; }
            set { _accept = value; }
        }
        string _contentType = "application/x-www-form-urlencoded";
        /// <summary>
        /// 请求返回类型默认 application/x-www-form-urlencoded
        /// </summary>
        public string ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }
        string _userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:17.0) Gecko/20100101 Firefox/17.0";
        /// <summary>
        /// 客户端访问信息默认Mozilla/5.0 (Windows NT 6.1; WOW64; rv:17.0) Gecko/20100101 Firefox/17.0
        /// </summary>
        public string UserAgent
        {
            get { return _userAgent; }
            set { _userAgent = value; }
        }
        string _encoding = string.Empty;
        /// <summary>
        /// 返回数据编码默认为NUll,可以自动识别
        /// </summary>
        public string Encoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }
        private PostDataType _postDataType = PostDataType.String;
        /// <summary>
        /// Post的数据类型
        /// </summary>
        public PostDataType PostDataType
        {
            get { return _postDataType; }
            set { _postDataType = value; }
        }
        string _postdata;
        /// <summary>
        /// Post请求时要发送的字符串Post数据
        /// </summary>
        public string Postdata
        {
            get { return _postdata; }
            set { _postdata = value; }
        }
        private byte[] _postdataByte = null;
        /// <summary>
        /// Post请求时要发送的Byte类型的Post数据
        /// </summary>
        public byte[] PostdataByte
        {
            get { return _postdataByte; }
            set { _postdataByte = value; }
        }
        CookieCollection _cookiecollection = null;
        /// <summary>
        /// Cookie对象集合
        /// </summary>
        public CookieCollection CookieCollection
        {
            get { return _cookiecollection; }
            set { _cookiecollection = value; }
        }
        private CookieContainer _container = null;
        /// <summary>
        /// 自动处理cookie
        /// </summary>
        public CookieContainer Container
        {
            get { return _container; }
            set { _container = value; }
        }



        string _cookie = string.Empty;
        /// <summary>
        /// 请求时的Cookie
        /// </summary>
        public string Cookie
        {
            get { return _cookie; }
            set { _cookie = value; }
        }
        string _referer = string.Empty;
        /// <summary>
        /// 来源地址，上次访问地址
        /// </summary>
        public string Referer
        {
            get { return _referer; }
            set { _referer = value; }
        }
        string _cerPath = string.Empty;
        /// <summary>
        /// 证书绝对路径
        /// </summary>
        public string CerPath
        {
            get { return _cerPath; }
            set { _cerPath = value; }
        }
        private Boolean _isToLower = false;
        /// <summary>
        /// 是否设置为全文小写
        /// </summary>
        public Boolean IsToLower
        {
            get { return _isToLower; }
            set { _isToLower = value; }
        }
        private Boolean _isAjax = false;
        /// <summary>
        /// 是否增加异步请求头
        /// </summary>
        public Boolean IsAjax
        {
            get { return _isAjax; }
            set { _isAjax = value; }
        }

        private Boolean _allowautoredirect = true;
        /// <summary>
        /// 支持跳转页面，查询结果将是跳转后的页面
        /// </summary>
        public Boolean Allowautoredirect
        {
            get { return _allowautoredirect; }
            set { _allowautoredirect = value; }
        }
        private int _connectionlimit = 1024;
        /// <summary>
        /// 最大连接数
        /// </summary>
        public int Connectionlimit
        {
            get { return _connectionlimit; }
            set { _connectionlimit = value; }
        }
        private string _proxyusername = string.Empty;
        /// <summary>
        /// 代理Proxy 服务器用户名
        /// </summary>
        public string ProxyUserName
        {
            get { return _proxyusername; }
            set { _proxyusername = value; }
        }
        private string _proxypwd = string.Empty;
        /// <summary>
        /// 代理 服务器密码
        /// </summary>
        public string ProxyPwd
        {
            get { return _proxypwd; }
            set { _proxypwd = value; }
        }
        private string _proxyip = string.Empty;
        /// <summary>
        /// 代理 服务IP
        /// </summary>
        public string ProxyIp
        {
            get { return _proxyip; }
            set { _proxyip = value; }
        }
        private ResultType _resulttype = ResultType.String;
        /// <summary>
        /// 设置返回类型String和Byte
        /// </summary>
        public ResultType ResultType
        {
            get { return _resulttype; }
            set { _resulttype = value; }
        }
        private WebHeaderCollection _header = new WebHeaderCollection();
        //header对象
        public WebHeaderCollection Header
        {
            get { return _header; }
            set { _header = value; }
        }
    }
    /// <summary>
    /// Http返回参数类
    /// </summary>
    public class HttpResults
    {
        CookieContainer _container;
        /// <summary>
        /// 自动处理Cookie集合对象
        /// </summary>
        public CookieContainer Container
        {
            get { return _container; }
            set { _container = value; }
        }
        string _cookie = string.Empty;
        /// <summary>
        /// Http请求返回的Cookie
        /// </summary>
        public string Cookie
        {
            get { return _cookie; }
            set { _cookie = value; }
        }
        CookieCollection _cookiecollection = null;
        /// <summary>
        /// Cookie对象集合
        /// </summary>
        public CookieCollection CookieCollection
        {
            get { return _cookiecollection; }
            set { _cookiecollection = value; }
        }
        private string _html = string.Empty;
        /// <summary>
        /// 返回的String类型数据 只有ResultType.String时才返回数据，其它情况为空
        /// </summary>
        public string Html
        {
            get { return _html; }
            set { _html = value; }
        }
        private byte[] _resultbyte = null;
        /// <summary>
        /// 返回的Byte数组 只有ResultType.Byte时才返回数据，其它情况为空
        /// </summary>
        public byte[] ResultByte
        {
            get { return _resultbyte; }
            set { _resultbyte = value; }
        }
        private WebHeaderCollection _header = new WebHeaderCollection();
        //header对象
        public WebHeaderCollection Header
        {
            get { return _header; }
            set { _header = value; }
        }
    }

    /// <summary>
    /// 返回类型
    /// </summary>
    public enum ResultType
    {
        String,//表示只返回字符串
        Byte//表示返回字符串和字节流
    }

    /// <summary>
    /// Post的数据格式默认为string
    /// </summary>
    public enum PostDataType
    {
        String,//字符串
        Byte,//字符串和字节流
        FilePath//表示传入的是文件
    }

    /// <summary>
    /// WinInet的方式请求数据
    /// </summary>
    public class Wininet
    {
        #region WininetAPI
        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        private static extern int InternetOpen(string strAppName, int ulAccessType, string strProxy, string strProxyBypass, int ulFlags);
        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        private static extern int InternetConnect(int ulSession, string strServer, int ulPort, string strUser, string strPassword, int ulService, int ulFlags, int ulContext);
        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        private static extern bool InternetCloseHandle(int ulSession);
        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        private static extern bool HttpAddRequestHeaders(int hRequest, string szHeasers, uint headersLen, uint modifiers);
        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        private static extern int HttpOpenRequest(int hConnect, string szVerb, string szUri, string szHttpVersion, string szReferer, string accetpType, long dwflags, int dwcontext);
        [DllImport("wininet.dll")]
        private static extern bool HttpSendRequestA(int hRequest, string szHeaders, int headersLen, string options, int optionsLen);
        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        private static extern bool InternetReadFile(int hRequest, byte[] pByte, int size, out int revSize);
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool InternetGetCookieEx(string pchUrl, string pchCookieName, StringBuilder pchCookieData, ref System.UInt32 pcchCookieData, int dwFlags, IntPtr lpReserved);
        #endregion

        #region 重载方法
        /// <summary>
        /// WinInet 方式GET
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns></returns>
        public string GetData(string url)
        {
            using (var ms = GetHtml(url, ""))
            {
                if (ms != null)
                {
                    //无视编码
                    var meta = Regex.Match(Encoding.Default.GetString(ms.ToArray()), "<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
                    var c = (meta.Groups.Count > 1) ? meta.Groups[2].Value.ToUpper().Trim() : string.Empty;
                    if (c.Length > 2)
                    {
                        if (c.IndexOf("UTF-8") != -1)
                        {
                            return Encoding.GetEncoding("UTF-8").GetString(ms.ToArray());
                        }
                    }
                    return Encoding.GetEncoding("GBK").GetString(ms.ToArray());
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// POST
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="postData">提交数据</param>
        /// <returns></returns>
        public string PostData(string url, string postData)
        {
            using (var ms = GetHtml(url, postData))
            {
                //无视编码
                var meta = Regex.Match(Encoding.Default.GetString(ms.ToArray()), "<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
                var c = (meta.Groups.Count > 1) ? meta.Groups[2].Value.ToUpper().Trim() : string.Empty;
                if (c.Length > 2)
                {
                    if (c.IndexOf("UTF-8") != -1)
                    {
                        return Encoding.GetEncoding("UTF-8").GetString(ms.ToArray());
                    }
                }
                return Encoding.GetEncoding("GBK").GetString(ms.ToArray());
            }
        }
        /// <summary>
        /// GET（UTF-8）模式
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns></returns>
        public string GetUtf8(string url)
        {
            using (var ms = GetHtml(url, ""))
            {
                return Encoding.GetEncoding("UTF-8").GetString(ms.ToArray());
            }
        }
        /// <summary>
        /// POST（UTF-8）
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="postData">提交数据</param>
        /// <returns></returns>
        public string PostUtf8(string url, string postData)
        {
            using (var ms = GetHtml(url, postData))
            {
                return Encoding.GetEncoding("UTF-8").GetString(ms.ToArray());
            }
        }
        /// <summary>
        /// 获取网页图片(Image)
        /// </summary>
        /// <param name="url">图片地址</param>
        /// <returns></returns>
        public Image GetImage(string url)
        {
            using (var ms = GetHtml(url, ""))
            {
                var img = Image.FromStream(ms);
                return img;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 请求数据
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postdata">提交的数据</param>
        /// <param name="header">请求头</param>
        /// <returns></returns>
        private MemoryStream GetHtml(string url, string postdata, StringBuilder header = null)
        {
            try
            {
                //声明部分变量
                var uri = new Uri(url);
                var method = "GET";
                if (postdata != "")
                {
                    method = "POST";
                }
                var userAgent = "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1; 125LA; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022)";
                var hSession = InternetOpen(userAgent, 1, "", "", 0);//会话句柄
                if (hSession == 0)
                {
                    InternetCloseHandle(hSession);
                    return null;//Internet句柄获取失败则返回
                }
                var hConnect = InternetConnect(hSession, uri.Host, uri.Port, "", "", 3, 0, 0);//连接句柄
                if (hConnect == 0)
                {
                    InternetCloseHandle(hConnect);
                    InternetCloseHandle(hSession);
                    return null;//Internet连接句柄获取失败则返回
                }
                //请求标记
                long gettype = -2147483632;
                if (url.Substring(0, 5) == "https")
                {
                    gettype = -2139095024;
                }
                else
                {
                    gettype = -2147467248;
                }
                //取HTTP请求句柄
                var hRequest = HttpOpenRequest(hConnect, method, uri.PathAndQuery, "HTTP/1.1", "", "", gettype, 0);//请求句柄
                if (hRequest == 0)
                {
                    InternetCloseHandle(hRequest);
                    InternetCloseHandle(hConnect);
                    InternetCloseHandle(hSession);
                    return null;//HTTP请求句柄获取失败则返回
                }
                //添加HTTP头
                var sb = new StringBuilder();
                if (header == null)
                {
                    sb.Append("Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\r\n");
                    sb.Append("Content-Type:application/x-www-form-urlencoded\r\n");
                    sb.Append("Accept-Language:zh-cn\r\n");
                    sb.Append("Referer:" + url);
                }
                else
                {
                    sb = header;
                }
                //获取返回数据
                if (string.Equals(method, "GET", StringComparison.OrdinalIgnoreCase))
                {
                    HttpSendRequestA(hRequest, sb.ToString(), sb.Length, "", 0);
                }
                else
                {
                    HttpSendRequestA(hRequest, sb.ToString(), sb.Length, postdata, postdata.Length);
                }
                //处理返回数据
                var revSize = 0;//计次
                var bytes = new byte[1024];
                var ms = new MemoryStream();
                while (true)
                {
                    var readResult = InternetReadFile(hRequest, bytes, 1024, out revSize);
                    if (readResult && revSize > 0)
                    {
                        ms.Write(bytes, 0, revSize);
                    }
                    else
                    {
                        break;
                    }
                }
                InternetCloseHandle(hRequest);
                InternetCloseHandle(hConnect);
                InternetCloseHandle(hSession);
                return ms;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region 获取webbrowser的cookies
        /// <summary>
        /// 取出cookies
        /// </summary>
        /// <param name="url">完整的链接格式</param>
        /// <returns></returns>
        public string GetCookies(string url)
        {
            uint datasize = 256;
            var cookieData = new StringBuilder((int)datasize);
            if (!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x2000, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;

                cookieData = new StringBuilder((int)datasize);
                if (!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x00002000, IntPtr.Zero))
                    return null;
            }
            return cookieData.ToString() + ";";
        }
        #endregion

        #region String与CookieContainer互转
        /// <summary>
        /// 遍历CookieContainer
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public List<Cookie> GetAllCookies(CookieContainer cc)
        {
            var lstCookies = new List<Cookie>();
            var table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            foreach (var pathList in table.Values)
            {
                var lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) lstCookies.Add(c);
            }
            return lstCookies;

        }
        /// <summary>
        /// 将String转CookieContainer
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public CookieContainer StringToCookie(string url, string cookie)
        {
            var arrCookie = cookie.Split(';');
            var cookieContainer = new CookieContainer();    //加载Cookie
            foreach (var sCookie in arrCookie)
            {
                if (sCookie.IndexOf("expires") > 0)
                    continue;
                cookieContainer.SetCookies(new Uri(url), sCookie);
            }
            return cookieContainer;
        }
        /// <summary>
        /// 将CookieContainer转换为string类型
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public string CookieToString(CookieContainer cc)
        {
            var lstCookies = new System.Collections.Generic.List<Cookie>();
            var table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });
            var sb = new StringBuilder();
            foreach (var pathList in table.Values)
            {
                var lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies)
                    {
                        sb.Append(c.Name).Append("=").Append(c.Value).Append(";");
                    }
            }
            return sb.ToString();
        }
        #endregion
    }

    /// <summary>
    /// 玄机网一键HTTP类库
    /// </summary>
    public class Xjhttp
    {
        HttpItems _item = new HttpItems();
        HttpHelpers _http = new HttpHelpers();
        Wininet _wnet = new Wininet();
        HttpResults _hr;
        /// <summary>
        /// 普通请求.直接返回标准结果
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <returns>返回结果</returns>
        public HttpResults GetHtml(string url)
        {
            _item.Url = url;
            return _http.GetHtml(_item);
        }
        /// <summary>
        /// 普通请求.直接返回标准结果
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="cc">当前Cookie</param>
        /// <returns></returns>
        public HttpResults GetHtml(string url, CookieContainer cc)
        {
            _item.Url = url;
            _item.Container = cc;
            return _http.GetHtml(_item);
        }
        /// <summary>
        ///  普通请求.直接返回标准结果
        /// </summary>
        /// <param name="picurl">图片请求地址</param>
        /// <param name="referer">上一次请求地址</param>
        /// <param name="cc">当前Cookie</param>
        /// <returns></returns>
        public HttpResults GetImage(string picurl, string referer, CookieContainer cc)
        {
            _item.Url = picurl;
            _item.Referer = referer;
            _item.Container = cc;
            _item.ResultType = ResultType.Byte;
            return _http.GetHtml(_item);
        }
        /// <summary>
        /// 普通请求.直接返回Image格式图像
        /// </summary>
        /// <param name="picurl">图片请求地址</param>
        /// <param name="referer">上一次请求地址</param>
        /// <param name="cc">当前Cookie</param>
        /// <returns></returns>
        public Image GetImageByImage(string picurl, string referer, CookieContainer cc)
        {
            _item.Url = picurl;
            _item.Referer = referer;
            _item.Container = cc;
            _item.ResultType = ResultType.Byte;
            return _http.GetImg(_http.GetHtml(_item));
        }
        /// <summary>
        /// 普通请求.直接返回标准结果
        /// </summary>
        /// <param name="posturl">post地址</param>
        /// <param name="referer">上一次请求地址</param>
        /// <param name="postdata">请求数据</param>
        /// <param name="isAjax">是否需要异步标识</param>
        /// <param name="cc">当前Cookie</param>
        /// <returns></returns>
        public HttpResults PostHtml(string posturl, string referer, string postdata, bool isAjax, CookieContainer cc)
        {
            _item.Url = posturl;
            _item.Referer = referer;
            _item.Method = "Post";
            _item.IsAjax = isAjax;
            _item.ResultType = ResultType.String;
            _item.Postdata = postdata;
            _item.Container = cc;
            return _http.GetHtml(_item);
        }
        /// <summary>
        /// 获取当前请求所有Cookie
        /// </summary>
        /// <param name="items"></param>
        /// <returns>Cookie集合</returns>
        public List<Cookie> GetAllCookieByHttpItems(HttpItems items)
        {
            return _wnet.GetAllCookies(items.Container);
        }
        /// <summary>
        /// 获取CookieContainer 中的所有对象
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public List<Cookie> GetAllCookie(CookieContainer cc)
        {
            return _wnet.GetAllCookies(cc);
        }
        /// <summary>
        /// 将 CookieContainer 对象转换为字符串类型
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public string CookieTostring(CookieContainer cc)
        {
            return _wnet.CookieToString(cc);
        }
        /// <summary>
        /// 将文字Cookie转换为CookieContainer 对象
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public CookieContainer StringToCookie(string url, string cookie)
        {
            return _wnet.StringToCookie(url, cookie);
        }
        /// <summary>
        /// 从Wininet中获取Cookie对象
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string StringToCookieByWininet(string url)
        {
            return _wnet.GetCookies(url);
        }

        /// <summary>
        /// 异步POST请求 通过回调返回结果
        /// </summary>
        /// <param name="objHttpItems">请求项</param>
        /// <param name="callBack">回调地址</param>
        public void AsyncPostHtml(HttpItems objHttpItems, Action<HttpResults> callBack)
        {
            _http.AsyncGetHtml(objHttpItems, callBack);
        }
        /// <summary>
        /// 异步GET请求 通过回调返回结果
        /// </summary>
        /// <param name="objHttpItems">请求项</param>
        /// <param name="callBack">回调地址</param>
        public void AsyncGetHtml(HttpItems objHttpItems, Action<HttpResults> callBack)
        {
            _http.AsyncGetHtml(objHttpItems, callBack);
        }
        /// <summary>
        /// WinInet方式GET请求  直接返回网页内容
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <returns></returns>
        public string GetHtmlByWininet(string url)
        {
            return _wnet.GetData(url);
        }
        /// <summary>
        /// WinInet方式GET请求(UTF8)  直接返回网页内容
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <returns></returns>
        public string GetHtmlByWininetUtf8(string url)
        {
            return _wnet.GetUtf8(url);
        }
        /// <summary>
        /// WinInet方式POST请求  直接返回网页内容
        /// </summary>
        /// <param name="url">提交地址</param>
        /// <param name="postdata">提交内容</param>
        /// <returns></returns>
        public string PostHtmlByWininet(string url, string postdata)
        {
            return _wnet.PostData(url, postdata);
        }
        /// <summary>
        /// WinInet方式POST请求  直接返回网页内容
        /// </summary>
        /// <param name="url">提交地址</param>
        /// <param name="postdata">提交内容</param>
        /// <returns></returns>
        public string PostHtmlByWininetUtf8(string url, string postdata)
        {
            return _wnet.GetUtf8(url);
        }
        /// <summary>
        /// WinInet方式请求 图片  直接返回Image
        /// </summary>
        /// <param name="url">提交地址</param>
        /// <returns></returns>
        public Image GetImageByWininet(string url)
        {
            return _wnet.GetImage(url);
        }

        /// <summary>
        /// 获取JS时间戳 13位
        /// </summary>
        /// <returns></returns>
        public string GetTime()
        {
            var obj = Type.GetTypeFromProgID("ScriptControl");
            if (obj == null) return null;
            var scriptControl = Activator.CreateInstance(obj);
            obj.InvokeMember("Language", BindingFlags.SetProperty, null, scriptControl, new object[] { "JScript" });
            var js = "function time(){return new Date().getTime()}";
            obj.InvokeMember("AddCode", BindingFlags.InvokeMethod, null, scriptControl, new object[] { js });
            return obj.InvokeMember("Eval", BindingFlags.InvokeMethod, null, scriptControl, new object[] { "time()" }).ToString();
        }
        /// <summary>  
        /// 获取时间戳 C# 10位 
        /// </summary>  
        /// <returns></returns>  
        public string GetTimeStamp()
        {
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
    }

}
