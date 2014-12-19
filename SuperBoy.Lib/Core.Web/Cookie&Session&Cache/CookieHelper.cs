using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;

namespace Core.Web
{
    /// <summary>
    ///  Cookie操作辅助类
    /// </summary>
    public class CookieHelper
    {
        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetGetCookie(string url, string cookieName, StringBuilder cookieData, ref int size);

        public static CookieContainer GetUriCookieContainer(Uri uri)
        {
            CookieContainer cookies = null;
            //定义Cookie数据的大小。   
            var datasize = 256;
            var cookieData = new StringBuilder(datasize);
            if (!InternetGetCookie(uri.ToString(), null, cookieData, ref datasize))
            {
                if (datasize < 0) return null;
                // 确信有足够大的空间来容纳Cookie数据。   
                cookieData = new StringBuilder(datasize);
                if (!InternetGetCookie(uri.ToString(), null, cookieData, ref datasize)) return null;
            }
            if (cookieData.Length > 0)
            {
                cookies = new CookieContainer();
                cookies.SetCookies(uri, cookieData.ToString().Replace(';', ','));
            }
            return cookies;
        }

        public static string PrintCookies(CookieContainer cookies, Uri uri)
        {
            var cc = cookies.GetCookies(uri);
            var sb = new StringBuilder();
            foreach (Cookie cook in cc)
            {
                sb.AppendLine(string.Format("{0}:{1}", cook.Name, cook.Value));
            }
            return sb.ToString();
        }

        #region 清除CooKie 

        #region 清除指定Cookie
        /// <summary>
        /// 清除指定Cookie
        /// </summary>
        /// <param name="cookiename">cookiename</param>
        public static void ClearCookie(string cookiename)
        {
            var cookie = HttpContext.Current.Request.Cookies[cookiename];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddYears(-3);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        #endregion
     
        #region   删除所有cookie值
        /// <summary>
        /// 删除所有cookie值
        /// </summary>
        public static void Clear()
        {
            var n = HttpContext.Current.Response.Cookies.Count;
            for (var i = 0; i < n; i++)
            {
                var myCookie = HttpContext.Current.Response.Cookies[i];
                myCookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(myCookie);
            }
        }
        #endregion
    
        #region 清除系统指定的Cookie
        /// <summary>
        /// 清除系统指定的Cookie
        /// </summary>
        /// <param name="ck">指定的Cookie</param>
        /// <param name="url">Cookie的域</param>
        public static void ClearCookies(string ck, string url)
        {
            var cks = GetCks(ck);
            var domains = GetDomains(url);
            for (var i = 0; i < cks.Length; i++)
            {
                var nv = cks[i].Split('=');
                for (var j = 0; j < domains.Count; j++)
                {
                    InternetSetCookie(domains[j], nv[0], "abc;expires = Sat, 31-Dec-2007 14:00:00 GMT");
                }
            }

        }
        #endregion

        #region   将浏览器中的Cookie清除
        
        /// <summary>
        /// 将浏览器中的Cookie清除
        /// </summary>
        /// <param name="browser">浏览器</param>
        public static void ClearCookies(WebBrowser browser)
        {
            if (browser != null && browser.Document != null)
            {
                ClearCookies(browser.Document.Cookie, browser.Url.ToString());
            }
        }
        #endregion

        #region 清除系统的Cookie文件

        
        /// <summary>
        /// 清除系统的Cookie文件
        /// </summary>
        public static void ClearCookiesFiles()
        {
            var di = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Cookies));
            foreach (var file in di.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch
                {
                }
            }
        }
        #endregion
        #endregion
   
        #region 获取Cookie
    
        #region 获取指定Cookie值

        /// <summary>
        /// 获取指定Cookie值
        /// </summary>
        /// <param name="cookiename">cookiename</param>
        /// <returns></returns>
        public static string GetCookieValue(string cookiename)
        {
            var cookie = HttpContext.Current.Request.Cookies[cookiename];
            var str = String.Empty;
            if (cookie != null)
            {
                str = cookie.Value;
            }
            return str;
        }

        /// <summary>
        /// 根据Key值得到Cookie值,Key不区分大小写
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="cookie">字符串Cookie</param>
        /// <returns></returns>
        public static string GetCookieValue(string key, string cookie)
        {
            foreach (var item in GetCookieList(cookie))
            {
                if (item.Key == key)
                    return item.Value;
            }
            return "";
        }

        /// <summary>
        /// 根据字符生成Cookie列表
        /// </summary>
        /// <param name="cookie">Cookie字符串</param>
        /// <returns></returns>
        public static List<CookieItem> GetCookieList(string cookie)
        {
            var cookielist = new List<CookieItem>();
            foreach (var item in cookie.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (Regex.IsMatch(item, @"([\s\S]*?)=([\s\S]*?)$"))
                {
                    var m = Regex.Match(item, @"([\s\S]*?)=([\s\S]*?)$");
                    cookielist.Add(new CookieItem() { Key = m.Groups[1].Value, Value = m.Groups[2].Value });
                }
            }
            return cookielist;
        }
        #endregion
      
        #region  获取cookie数组
      
        /// <summary>
        /// 获取cookie数组
        /// </summary>
        /// <param name="ck"></param>
        /// <returns></returns>
        public static string[] GetCks(string ck)
        {
            if (ck != null)
            {
                return ck.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                return new string[0];
            }
        }
          #endregion

        #region   获得浏览器里的Cookie字符串
      
      
        /// <summary>
        /// 获得浏览器里的Cookie字符串
        /// </summary>
        /// <param name="browser">浏览器</param>
        /// <returns>剔除重复值后的Cookie字符串</returns>
        public static string GetCK(WebBrowser browser)
        {
            var res = "";
            if (browser.Document != null && browser.Document.Cookie != null)
            {
                var ck = browser.Document.Cookie;
                var cks = GetCks(ck);
                res = GetCK(cks);
            }
            return res;

        }
        #endregion

        #region  从Cookie数组中转换成不重复的Cookie字符串，相同的Cookie取前面的

        /// <summary>
        /// 从Cookie数组中转换成不重复的Cookie字符串，相同的Cookie取前面的
        /// </summary>
        /// <param name="cks">Cookie数组</param>
        /// <returns>剔除重复的Cookie字符串</returns>
        public static string GetCK(string[] cks)
        {
            var res = "";
            var list = new List<string>();
            for (var i = 0; i < cks.Length; i++)
            {
                if (cks[i].Trim() != "")
                {
                    if (!IncludeCk(list, cks[i]))
                    {
                        list.Add(cks[i].Trim());
                    }
                }
            }
            for (var i = 0; i < list.Count; i++)
            {
                res += list[i] + ";";
            }
            return res;
        }
        #endregion

        #region  从CookieCollection中获取Cookie字符串


        /// <summary>
        /// 从CookieCollection中获取Cookie字符串
        /// </summary>
        /// <param name="cc">CookieCollection，一般用在WebRequest中</param>
        /// <returns>转换后的Cookie字符串</returns>
        public static string GetCK(CookieCollection cc)
        {
            var ck = "";
            for (var i = 0; i < cc.Count; i++)
            {
                ck += cc[i].Name + "=" + cc[i].Value + ";";
            }
            return ck;
        }
        #endregion

        #region   将Cookie字符串填充到CookieCollection中


        /// <summary>
        /// 将Cookie字符串填充到CookieCollection中
        /// </summary>
        /// <param name="ck">Cookie字符串</param>
        /// <param name="url">Cookie的域</param>
        /// <returns>添加后的CookieCollection</returns>
        public static CookieCollection GetCK(string ck, string url)
        {
            var cc = new CookieCollection();
            var domain = "";
            try
            {
                var u = new Uri(url);
                domain = u.Host;
            }
            catch { }
            var cks = GetCks(ck);
            for (var i = 0; i < cks.Length; i++)
            {
                if (cks[i].IndexOf("=") > -1)
                {
                    try
                    {
                        var n = cks[i].Substring(0, cks[i].IndexOf("="));
                        var v = cks[i].Substring(cks[i].IndexOf("=") + 1);
                        var c = new System.Net.Cookie();
                        c.Name = n.Trim();
                        c.Value = v.Trim();
                        c.Domain = domain;
                        cc.Add(c);
                    }
                    catch { }
                }
            }
            return cc;
        }
        #endregion
        #endregion
    
        #region 添加Cookie



        #region   添加一个Cookie（24小时过期）

        /// <summary>
        /// 添加一个Cookie（24小时过期）
        /// </summary>
        /// <param name="cookiename"></param>
        /// <param name="cookievalue"></param>
        public static void SetCookie(string cookiename, string cookievalue)
        {
            SetCookie(cookiename, cookievalue, DateTime.Now.AddDays(1.0));
        }
        #endregion

        #region   添加一个Cookie
        /// <summary>
        /// 添加一个Cookie
        /// </summary>
        /// <param name="cookiename">cookie名</param>
        /// <param name="cookievalue">cookie值</param>
        /// <param name="expires">过期时间 DateTime</param>
        public static void SetCookie(string cookiename, string cookievalue, DateTime expires)
        {
            var cookie = new HttpCookie(cookiename)
            {
                Value = cookievalue,
                Expires = expires
            };
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
        #endregion
        #endregion

        #region 检查Cookie集合中是否包含指定的Cookie值

        
        /// <summary>
        /// 检查Cookie集合中是否包含指定的Cookie值
        /// </summary>
        /// <param name="cks">Cookie集合</param>
        /// <param name="ck">待判断的Cookie</param>
        /// <returns>Cookie集合中是否包含指定的Cookie</returns>
        protected static bool IncludeCk(List<string> cks, string ck)
        {
            try
            {
                var subCk = ck.Remove(ck.IndexOf('=') + 1).Trim().ToLower();
                for (var i = 0; i < cks.Count; i++)
                {
                    if (cks[i].ToLower().Trim().IndexOf(subCk) != -1)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch { return false; }

        }
        #endregion

        #region 设置系统Cookie

        /// <summary>
        /// 设置系统Cookie
        /// </summary>
        /// <param name="lpszUrlName">Cookie域</param>
        /// <param name="lbszCookieName">Cookie名</param>
        /// <param name="lpszCookieData">Cookie数据</param>
        /// <returns>设置成功与否</returns>
        [System.Runtime.InteropServices.DllImport("wininet.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);
        #endregion

        #region   获取所有可能的Cookie域

        /// <summary>
        /// 获取所有可能的Cookie域
        /// </summary>
        /// <param name="url">域的全称</param>
        /// <returns>所有可能的域</returns>
        public static List<string> GetDomains(string url)
        {
            var res = new List<string>();
            try
            {
                url = url.Remove(url.IndexOf("?"));
            }
            catch { }
            try
            {
                var uri = new Uri(url);
                var baseDomain = uri.Scheme + "://" + uri.Host;
                for (var i = 0; i < uri.Segments.Length; i++)
                {
                    baseDomain = baseDomain + uri.Segments[i];
                    res.Add(baseDomain);
                }
            }
            catch { }
            return res;
        }
        #endregion

        #region   获取浏览器的所有可能的Cookie域


        /// <summary>
        /// 获取浏览器的所有可能的Cookie域
        /// </summary>
        /// <param name="browser">浏览器</param>
        /// <returns>所有可能的域</returns>
        public static List<string> GetDomains(WebBrowser browser)
        {
            if (browser != null && browser.Url != null)
            {
                return GetDomains(browser.Url.ToString());
            }
            return new List<string>();
        }
        #endregion

        #region   将定制的Cookie字符串发给浏览器


        /// <summary>
        /// 将定制的Cookie字符串发给浏览器
        /// </summary>
        /// <param name="browser">浏览器</param>
        /// <param name="ck">Cookie字符串</param>
        public static void SetCkToBrowser(WebBrowser browser, string ck)
        {
            if (browser.Document != null)
            {
                var cks = GetCks(ck);
                for (var i = 0; i < cks.Length; i++)
                {
                    if (cks[i] != "")
                    {
                        browser.Document.Cookie = cks[i];
                    }
                }
            }
        }
        #endregion

        #region  将Cookie字符串描述的Cookie追加到CookieCoollection


        /// <summary>
        /// 将Cookie字符串描述的Cookie追加到CookieCoollection
        /// </summary>
        /// <param name="cc">CookieCoollection</param>
        /// <param name="ck">Cookie字符串</param>
        /// <param name="url">Cookie的域</param>
        public static void SetCkAppendToCc(CookieCollection cc, string ck, string url)
        {
            var tmp = GetCK(ck, url);
            for (var i = 0; i < tmp.Count; i++)
            {
                cc.Add(tmp[i]);
            }
        }
        #endregion

        #region 将Cookie字符串设置到系统中，便于浏览器使用


        /// <summary>
        /// 将Cookie字符串设置到系统中，便于浏览器使用
        /// </summary>
        /// <param name="ck">Cookie字符串</param>
        /// <param name="url">Cookie的域</param>
        public static void SetCKToSystem(string ck, string url)
        {
            var cks = GetCks(ck);
            for (var i = 0; i < cks.Length; i++)
            {
                var nv = cks[i].Split('=');
                InternetSetCookie(url, nv[0], nv.Length > 1 ? nv[1] : "");
            }
        }
        #endregion

        #region 将CookieCollection中的Cookie设置到系统中，便于浏览器使用


        /// <summary>
        /// 将CookieCollection中的Cookie设置到系统中，便于浏览器使用
        /// </summary>
        /// <param name="cc">CookieCollection</param>
        /// <param name="url">Cookie的域</param>
        public static void SetCKToSystem(CookieCollection cc, string url)
        {
            var domains = GetDomains(url);
            for (var i = 0; i < cc.Count; i++)
            {
                for (var j = 0; j < domains.Count; j++)
                {
                    InternetSetCookie(domains[j], cc[i].Name, cc[i].Value);
                }
            }
        }
        #endregion

        /// <summary>
        /// 格式化Cookie为标准格式
        /// </summary>
        /// <param name="key">Key值</param>
        /// <param name="value">Value值</param>
        /// <returns></returns>
        public static string CookieFormat(string key, string value)
        {
            return string.Format("{0}={1};", key, value);
        }
    }
  
    
    
    public class Test
    {
        static void Main(string[] args)
        {
            var url = @"http://www.kaixin001.com/";
            var uri = new Uri(url);
            var cookies = CookieHelper.GetUriCookieContainer(uri);
            CookieHelper.PrintCookies(cookies, uri);

            Console.ReadKey();
        }
    }

    /// <summary>
    /// Cookie对象
    /// </summary>
    public class CookieItem
    {
        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }

}
