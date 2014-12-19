﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Web;
using System.Xml;

namespace Common
{
    public class HtmlUtils
    {
        #region BaseMethod
        /// <summary>
        /// 多个匹配内容
        /// </summary>
        /// <param name="sInput">输入内容</param>
        /// <param name="sRegex">表达式字符串</param>
        /// <param name="iGroupIndex">第几个分组, 从1开始, 0代表不分组</param>
        public static List<string> GetList(string sInput, string sRegex, int iGroupIndex)
        {
            var list = new List<string>();
            var re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            var mcs = re.Matches(sInput);
            foreach (Match mc in mcs)
            {
                if (iGroupIndex > 0)
                {
                    list.Add(mc.Groups[iGroupIndex].Value);
                }
                else
                {
                    list.Add(mc.Value);
                }
            }
            return list;
        }

        /// <summary>
        /// 多个匹配内容
        /// </summary>
        /// <param name="sInput">输入内容</param>
        /// <param name="sRegex">表达式字符串</param>
        /// <param name="sGroupName">分组名, ""代表不分组</param>
        public static List<string> GetList(string sInput, string sRegex, string sGroupName)
        {
            var list = new List<string>();
            var re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            var mcs = re.Matches(sInput);
            foreach (Match mc in mcs)
            {
                if (sGroupName != "")
                {
                    list.Add(mc.Groups[sGroupName].Value);
                }
                else
                {
                    list.Add(mc.Value);
                }
            }
            return list;
        }

        /// <summary>
        /// 单个匹配内容
        /// </summary>
        /// <param name="sInput">输入内容</param>
        /// <param name="sRegex">表达式字符串</param>
        /// <param name="iGroupIndex">分组序号, 从1开始, 0不分组</param>
        public static string GetText(string sInput, string sRegex, int iGroupIndex)
        {
            var re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            var mc = re.Match(sInput);
            var result = "";
            if (mc.Success)
            {
                if (iGroupIndex > 0)
                {
                    result = mc.Groups[iGroupIndex].Value;
                }
                else
                {
                    result = mc.Value;
                }
            }
            return result;
        }

        /// <summary>
        /// 单个匹配内容
        /// </summary>
        /// <param name="sInput">输入内容</param>
        /// <param name="sRegex">表达式字符串</param>
        /// <param name="sGroupName">分组名, ""代表不分组</param>
        public static string GetText(string sInput, string sRegex, string sGroupName)
        {
            var re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            var mc = re.Match(sInput);
            var result = "";
            if (mc.Success)
            {
                if (sGroupName != "")
                {
                    result = mc.Groups[sGroupName].Value;
                }
                else
                {
                    result = mc.Value;
                }
            }
            return result;
        }

        /// <summary>
        /// 替换指定内容
        /// </summary>
        /// <param name="sInput">输入内容</param>
        /// <param name="sRegex">表达式字符串</param>
        /// <param name="sReplace">替换值</param>
        /// <param name="iGroupIndex">分组序号, 0代表不分组</param>
        public static string Replace(string sInput, string sRegex, string sReplace, int iGroupIndex)
        {
            var re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            var mcs = re.Matches(sInput);
            foreach (Match mc in mcs)
            {
                if (iGroupIndex > 0)
                {
                    sInput = sInput.Replace(mc.Groups[iGroupIndex].Value, sReplace);
                }
                else
                {
                    sInput = sInput.Replace(mc.Value, sReplace);
                }
            }
            return sInput;
        }

        /// <summary>
        /// 替换指定内容
        /// </summary>
        /// <param name="sInput">输入内容</param>
        /// <param name="sRegex">表达式字符串</param>
        /// <param name="sReplace">替换值</param>
        /// <param name="sGroupName">分组名, "" 代表不分组</param>
        public static string Replace(string sInput, string sRegex, string sReplace, string sGroupName)
        {
            var re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            var mcs = re.Matches(sInput);
            foreach (Match mc in mcs)
            {
                if (sGroupName != "")
                {
                    sInput = sInput.Replace(mc.Groups[sGroupName].Value, sReplace);
                }
                else
                {
                    sInput = sInput.Replace(mc.Value, sReplace);
                }
            }
            return sInput;
        }

        /// <summary>
        /// 分割指定内容
        /// </summary>
        /// <param name="sInput">输入内容</param>
        /// <param name="sRegex">表达式字符串</param>
        /// <param name="iStrLen">最小保留字符串长度</param>
        public static List<string> Split(string sInput, string sRegex, int iStrLen)
        {
            var re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            var sArray = re.Split(sInput);
            var list = new List<string>();
            list.Clear();
            foreach (var s in sArray)
            {
                if (s.Trim().Length < iStrLen)
                    continue;

                list.Add(s.Trim());
            }
            return list;
        }

        #endregion BaseMethod

        #region 获得特定内容

        /// <summary>
        /// 多个链接
        /// </summary>
        /// <param name="sInput">输入内容</param>
        public static List<string> GetLinks(string sInput)
        {
            return GetList(sInput, @"<a[^>]+href=\s*(?:'(?<href>[^']+)'|""(?<href>[^""]+)""|(?<href>[^>\s]+))\s*[^>]*>", "href");
        }

        /// <summary>
        /// 单个链接
        /// </summary>
        /// <param name="sInput">输入内容</param>
        public static string GetLinkHelp(string sInput)
        {
            return GetText(sInput, @"<a[^>]+href=\s*(?:'(?<href>[^']+)'|""(?<href>[^""]+)""|(?<href>[^>\s]+))\s*[^>]*>", "href");
        }

        /// <summary>
        /// 图片标签
        /// </summary>
        /// <param name="sInput">输入内容</param>
        public static List<string> GetImgTag(string sInput)
        {
            return GetList(sInput, "<img[^>]+src=\\s*(?:'(?<src>[^']+)'|\"(?<src>[^\"]+)\"|(?<src>[^>\\s]+))\\s*[^>]*>", "");
        }

        /// <summary>
        /// 图片地址
        /// </summary>
        /// <param name="sInput">输入内容</param>
        public static string GetImgSrc(string sInput)
        {
            return GetText(sInput, "<img[^>]+src=\\s*(?:'(?<src>[^']+)'|\"(?<src>[^\"]+)\"|(?<src>[^>\\s]+))\\s*[^>]*>", "src");
        }

        /// <summary>
        /// 根据URL获得域名
        /// </summary>
        /// <param name="sUrl">输入内容</param>
        public static string GetDomain(string sInput)
        {
            return GetText(sInput, @"http(s)?://([\w-]+\.)+(\w){2,}", 0);
        }

        #endregion 获得特定内容

        #region 根据表达式，获得文章内容
        /// <summary>
        /// 文章标题
        /// </summary>
        /// <param name="sInput">输入内容</param>
        /// <param name="sRegex">表达式字符串</param>
        public static string GetTitle(string sInput, string sRegex)
        {
            var sTitle = GetText(sInput, sRegex, "Title");
            sTitle = ClearTag(sTitle);
            if (sTitle.Length > 99)
            {
                sTitle = sTitle.Substring(0, 99);
            }
            return sTitle;
        }

        /// <summary>
        /// 网页标题
        /// </summary>
        public static string GetTitle(string sInput)
        {
            return GetText(sInput, @"<Title[^>]*>(?<Title>[\s\S]{10,})</Title>", "Title");
        }

        /// <summary>
        /// 网页内容
        /// </summary>
        /// <param name="sInput">输入内容</param>
        public static string GetHtml(string sInput)
        {
            return Replace(sInput, @"(?<Head>[^<]+)<", "", "Head");
        }

        /// <summary>
        /// 网页Body内容
        /// </summary>
        public static string GetBodyHelp(string sInput)
        {
            return GetText(sInput, @"<Body[^>]*>(?<Body>[\s\S]{10,})</body>", "Body");
        }

        /// <summary>
        /// 网页Body内容
        /// </summary>
        /// <param name="sInput">输入内容</param>
        /// <param name="sRegex">表达式字符串</param>
        public static string GetBody(string sInput, string sRegex)
        {
            return GetText(sInput, sRegex, "Body");
        }

        /// <summary>
        /// 文章来源
        /// </summary>
        /// <param name="sInput">输入内容</param>
        /// <param name="sRegex">表达式字符串</param>
        public static string GetSource(string sInput, string sRegex)
        {
            var sSource = GetText(sInput, sRegex, "Source");
            sSource = ClearTag(sSource);
            if (sSource.Length > 99)
                sSource = sSource.Substring(0, 99);
            return sSource;
        }

        /// <summary>
        /// 作者名
        /// </summary>
        /// <param name="sInput">输入内容</param>
        /// <param name="sRegex">表达式字符串</param>
        public static string GetAuthor(string sInput, string sRegex)
        {
            var sAuthor = GetText(sInput, sRegex, "Author");
            sAuthor = ClearTag(sAuthor);
            if (sAuthor.Length > 99)
                sAuthor = sAuthor.Substring(0, 99);
            return sAuthor;
        }

        /// <summary>
        /// 分页链接地址
        /// </summary>
        /// <param name="sInput">输入内容</param>
        /// <param name="sRegex">表达式字符串</param>
        public static List<string> GetPageLinks(string sInput, string sRegex)
        {
            return GetList(sInput, sRegex, "href");
        }

        /// <summary>
        /// 根据相对路径得到绝对路径
        /// </summary>
        /// <param name="sUrl">输入内容</param>
        /// <param name="sInput">原始网站地址</param>
        /// <param name="sRelativeUrl">相对链接地址</param>
        public static string GetUrl(string sInput, string sRelativeUrl)
        {
            var sReturnUrl = "";
            var sUrl = _GetStandardUrlDepth(sInput);//返回了http://www.163.com/news/这种形式

            if (sRelativeUrl.ToLower().StartsWith("http") || sRelativeUrl.ToLower().StartsWith("https"))
            {
                sReturnUrl = sRelativeUrl.Trim();
            }
            else if (sRelativeUrl.StartsWith("/"))
            {
                sReturnUrl = GetDomain(sInput) + sRelativeUrl;
            }
            else if (sRelativeUrl.StartsWith("../"))
            {
                sUrl = sUrl.Substring(0, sUrl.Length - 1);
                while (sRelativeUrl.IndexOf("../") >= 0)
                {
                    var temp = sUrl.Substring(0, sUrl.LastIndexOf("/")); ;// CString.GetPreStrByLast(sUrl, "/");
                    if (temp.Length > 6)
                    {//temp != "http:/"，否则的话，说明已经回溯到尽头了，"../"与网址的层次对应不上。存在这种情况，网页上面的链接是错误的，但浏览器还能正常显示
                        sUrl = temp;
                    }
                    sRelativeUrl = sRelativeUrl.Substring(3);
                }
                sReturnUrl = sUrl + "/" + sRelativeUrl.Trim();
            }
            else if (sRelativeUrl.StartsWith("./"))
            {
                sReturnUrl = sUrl + sRelativeUrl.Trim().Substring(2);
            }
            else if (sRelativeUrl.Trim() != "")
            {//2007images/modecss.css
                sReturnUrl = sUrl + sRelativeUrl.Trim();
            }
            else
            {
                sRelativeUrl = sUrl;
            }
            return sReturnUrl;
        }

        /// <summary>
        /// 获得标准的URL路径深度
        /// </summary>
        /// <param name="url"></param>
        /// <returns>返回标准的形式：http://www.163.com/或http://www.163.com/news/。</returns>
        private static string _GetStandardUrlDepth(string url)
        {
            var sheep = url.Trim().ToLower();
            var header = "http://";
            if (sheep.IndexOf("https://") != -1)
            {
                header = "https://";
                sheep = sheep.Replace("https://", "");
            }
            else
            {
                sheep = sheep.Replace("http://", "");
            }

            var p = sheep.LastIndexOf("/");
            if (p == -1)
            {//www.163.com
                sheep += "/";
            }
            else if (p == sheep.Length - 1)
            {//传来的是：http://www.163.com/news/
            }
            else if (sheep.Substring(p).IndexOf(".") != -1)
            {//传来的是：http://www.163.com/news/hello.htm 这种形式
                sheep = sheep.Substring(0, p + 1);
            }
            else
            {
                sheep += "/";
            }

            return header + sheep;
        }

        /// <summary>
        /// 关键字
        /// </summary>
        /// <param name="sInput">输入内容</param>
        public static string GetKeyWord(string sInput)
        {
            var list = Split(sInput, "(,|，|\\+|＋|。|;|；|：|:|“)|”|、|_|\\(|（|\\)|）", 2);
            var listReturn = new List<string>();
            Regex re;
            foreach (var str in list)
            {
                re = new Regex(@"[a-zA-z]+", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                var mcs = re.Matches(str);
                var sTemp = str;
                foreach (Match mc in mcs)
                {
                    if (mc.Value.ToString().Length > 2)
                        listReturn.Add(mc.Value.ToString());
                    sTemp = sTemp.Replace(mc.Value.ToString(), ",");
                }
                re = new Regex(@",{1}", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                mcs = re.Matches(sTemp);
                foreach (var s in re.Split(sTemp))
                {
                    if (s.Trim().Length <= 2)
                        continue;
                    listReturn.Add(s);
                }
            }
            var sReturn = "";
            for (var i = 0; i < listReturn.Count - 1; i++)
            {
                for (var j = i + 1; j < listReturn.Count; j++)
                {
                    if (listReturn[i] == listReturn[j])
                    {
                        listReturn[j] = "";
                    }
                }
            }
            foreach (var str in listReturn)
            {
                if (str.Length > 2)
                    sReturn += str + ",";
            }
            if (sReturn.Length > 0)
                sReturn = sReturn.Substring(0, sReturn.Length - 1);
            else
                sReturn = sInput;
            if (sReturn.Length > 99)
                sReturn = sReturn.Substring(0, 99);
            return sReturn;
        }

     

        public static string GetContent(string sOriContent, string sOtherRemoveReg, string sPageUrl, DataTable dtAntiLink)
        {
            var sFormartted = sOriContent;

            //去掉有危险的标记
            sFormartted = Regex.Replace(sFormartted, @"<script[\s\S]*?</script>", "", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
            sFormartted = Regex.Replace(sFormartted, @"<iframe[^>]*>[\s\S]*?</iframe>", "", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
            var r = new Regex(@"<input[\s\S]+?>|<form[\s\S]+?>|</form[\s\S]*?>|<select[\s\S]+?>?</select>|<textarea[\s\S]*?>?</textarea>|<file[\s\S]*?>|<noscript>|</noscript>", RegexOptions.IgnoreCase);
            sFormartted = r.Replace(sFormartted, "");
            var sOtherReg = sOtherRemoveReg.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var sRemoveReg in sOtherReg)
            {
                sFormartted = Replace(sFormartted, sRemoveReg, "", 0);
            }

            //图片路径
            //sFormartted = _ReplaceUrl("<img[^>]+src\\s*=\\s*(?:'(?<src>[^']+)'|\"(?<src>[^\"]+)\"|(?<src>[^>\\s]+))\\s*[^>]*>", "src", sFormartted,sPageUrl);
            sFormartted = _ReplaceUrl("<img[\\s\\S]+?src\\s*=\\s*(?:'(?<src>[^']+)'|\"(?<src>[^\"]+)\"|(?<src>[^>\\s]+))\\s*[^>]*>", "src", sFormartted, sPageUrl);
            //反防盗链
            var domain = GetDomain(sPageUrl);
            var drs = dtAntiLink.Select("Domain='" + domain + "'");
            if (drs.Length > 0)
            {
                foreach (var dr in drs)
                {
                    switch (Convert.ToInt32(dr["Type"]))
                    {
                        case 1://置换
                            sFormartted = sFormartted.Replace(dr["imgUrl"].ToString(), "http://stat.580k.com/t.asp?url=");
                            break;
                        default://附加
                            sFormartted = sFormartted.Replace(dr["imgUrl"].ToString(), "http://stat.580k.com/t.asp?url=" + dr["imgUrl"].ToString());
                            break;
                    }
                }
            }

            //A链接
            sFormartted = _ReplaceUrl(@"<a[^>]+href\s*=\s*(?:'(?<href>[^']+)'|""(?<href>[^""]+)""|(?<href>[^>\s]+))\s*[^>]*>", "href", sFormartted, sPageUrl);

            //CSS
            sFormartted = _ReplaceUrl(@"<link[^>]+href\s*=\s*(?:'(?<href>[^']+)'|""(?<href>[^""]+)""|(?<href>[^>\s]+))\s*[^>]*>", "href", sFormartted, sPageUrl);

            //BACKGROUND
            sFormartted = _ReplaceUrl(@"background\s*=\s*(?:'(?<img>[^']+)'|""(?<img>[^""]+)""|(?<img>[^>\s]+))", "img", sFormartted, sPageUrl);
            //style方式的背景：background-image:url(...)
            sFormartted = _ReplaceUrl(@"background-image\s*:\s*url\s*\x28(?<img>[^\x29]+)\x29", "img", sFormartted, sPageUrl);

            //FLASH
            sFormartted = _ReplaceUrl(@"<param\s[^>]+""movie""[^>]+value\s*=\s*""(?<flash>[^"">]+\x2eswf)""[^>]*>", "flash", sFormartted, sPageUrl);

            //XSL
            if (IsXml(sFormartted))
            {
                sFormartted = _ReplaceUrl(@"<\x3fxml-stylesheet\s+[^\x3f>]+href=\s*(?:'(?<href>[^']+)'|""(?<href>[^""]+)"")\s*[^\x3f>]*\x3f>", "href", sFormartted, sPageUrl);
            }

            //script
            //sFormartted = _ReplaceUrl(@"<script[^>]+src\s*=\s*(?:'(?<src>[^']+)'|""(?<src>[^""]+)""|(?<src>[^>\s]+))\s*[^>]*>", "src", sFormartted,sPageUrl);

            return sFormartted;
        }

        /// <summary>
        /// 置换连接
        /// </summary>
        private static string _ReplaceUrl(string strRe, string subMatch, string sFormartted, string sPageUrl)
        {
            var re = new Regex(strRe, RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
            var mcs = re.Matches(sFormartted);
            var sOriStr = "";
            var sSubMatch = "";
            var sReplaceStr = "";
            foreach (Match mc in mcs)
            {
                sOriStr = mc.Value;
                sSubMatch = mc.Groups[subMatch].Value;
                sReplaceStr = sOriStr.Replace(sSubMatch, GetUrl(sPageUrl, sSubMatch));
                sFormartted = sFormartted.Replace(sOriStr, sReplaceStr);
            }

            return sFormartted;
        }

        public static bool IsXml(string sFormartted)
        {
            var re = new Regex(@"<\x3fxml\s+", RegexOptions.IgnoreCase);
            var mcs = re.Matches(sFormartted);
            return (mcs.Count > 0);
        }

        #endregion 根据表达式，获得文章内容


        #region HTML相关操作
        public static string ClearTag(string sHtml)
        {
            if (sHtml == "")
                return "";
            var sTemp = sHtml;
            var re = new Regex(@"(<[^>\s]*\b(\w)+\b[^>]*>)|(<>)|(&nbsp;)|(&gt;)|(&lt;)|(&amp;)|\r|\n|\t", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            return re.Replace(sHtml, "");
        }
        public static string ClearTag(string sHtml, string sRegex)
        {
            var sTemp = sHtml;
            var re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            return re.Replace(sHtml, "");
        }
        public static string ConvertToJs(string sHtml)
        {
            var sText = new StringBuilder();
            Regex re;
            re = new Regex(@"\r\n", RegexOptions.IgnoreCase);
            var strArray = re.Split(sHtml);
            foreach (var strLine in strArray)
            {
                sText.Append("document.writeln(\"" + strLine.Replace("\"", "\\\"") + "\");\r\n");
            }
            return sText.ToString();
        }
       
 
        /// <summary>
        /// 删除字符串中的特定标记 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="tag"></param>
        /// <param name="isContent">是否清除内容 </param>
        /// <returns></returns>
        public static string DelTag(string str, string tag, bool isContent)
        {
            if (tag == null || tag == " ")
            {
                return str;
            }

            if (isContent) //要求清除内容 
            {
                return Regex.Replace(str, string.Format("<({0})[^>]*>([\\s\\S]*?)<\\/\\1>", tag), "", RegexOptions.IgnoreCase);
            }

            return Regex.Replace(str, string.Format(@"(<{0}[^>]*(>)?)|(</{0}[^>] *>)|", tag), "", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 删除字符串中的一组标记 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="tagA"></param>
        /// <param name="isContent">是否清除内容 </param>
        /// <returns></returns>
        public static string DelTagArray(string str, string tagA, bool isContent)
        {

            var tagAa = tagA.Split(',');

            foreach (var sr1 in tagAa) //遍历所有标记，删除 
            {
                str = DelTag(str, sr1, isContent);
            }
            return str;

        }

        #endregion HTML相关操作
       
        #region 根据内容获得链接
        public static string GetLink(string sContent)
        {
            var strReturn = "";
            var re = new Regex(@"<a\s+[^>]*href\s*=\s*(?:'(?<href>[^']+)'|""(?<href>[^""]+)""|(?<href>[^>\s]+))\s*[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            var js = new Regex(@"(href|onclick)=[^>]+javascript[^>]+(('(?<href>[\w\d/-]+\.[^']*)')|(&quot;(?<href>[\w\d/-]+\.[^;]*)&quot;))[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            var mc = js.Match(sContent);//获取javascript中的链接，有待改进
            if (mc.Success)
            {
                strReturn = mc.Groups["href"].Value;
            }
            else
            {
                var me = re.Match(sContent);
                if (me.Success)
                {
                    strReturn = System.Web.HttpUtility.HtmlDecode(me.Groups["href"].Value);
                    //strReturn = RemoveByReg(strReturn, @";.*|javascript:.*");
                    strReturn = RemoveByReg(strReturn, @";[^?&]*|javascript:.*");
                }
            }

            return strReturn;
        }
        public static string GetTextByLink(string sContent)
        {
            var re = new Regex(@"<a(?:\s+[^>]*)?>([\s\S]*)?</a>", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            var email = new Regex(@"(href|onclick)=[^>]+mailto[^>]+@[^>]+>", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            var me = email.Match(sContent);
            if (me.Success)
                return "";

            var mc = re.Match(sContent);
            if (mc.Success)
                return mc.Groups[1].Value;
            else
                return "";
        }

        /// <summary>
        /// 获取所有有效链接，过滤广告
        /// </summary>
        /// <param name="sContent"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetLinks(string sContent, string sUrl)
        {
            var lisDes = new Dictionary<string, string>();
            return GetLinks(sContent, sUrl, ref lisDes);
        }

        public static Dictionary<string, string> GetLinks(string sContent, string sUrl, ref Dictionary<string, string> lisDes)
        {
            var lisA = new Dictionary<string, string>();

            _GetLinks(sContent, sUrl, ref lisA);

            var domain = GetDomain(sUrl).ToLower();

            //抓取脚本输出的链接
            var re = new Regex(@"<script[^>]+src\s*=\s*(?:'(?<src>[^']+)'|""(?<src>[^""]+)""|(?<src>[^>\s]+))\s*[^>]*>", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
            var mcs = re.Matches(sContent);
            //foreach (Match mc in mcs)
            for (var i = mcs.Count - 1; i >= 0; i--)
            {
                var mc = mcs[i];
                var subUrl = GetUrl(sUrl, mc.Groups["src"].Value);
                if (domain.CompareTo(GetDomain(subUrl).ToLower()) != 0)
                {
                    //同一域的才提炼
                    continue;
                }
                var subContent = GetHtmlByUrl(subUrl);
                if (subContent.Length == 0)
                {
                    continue;
                }
                _GetLinks(subContent, subUrl, ref lisA);
            }

            if (lisA.Count == 0)
            {
                return GetLinksFromRss(sContent, sUrl, ref lisDes);
            }

            return lisA;
        }

        private static void _GetLinks(string sContent, string sUrl, ref Dictionary<string, string> lisA)
        {
            const string sFilter =
@"首页|下载|中文|English|反馈|讨论区|投诉|建议|联系|关于|about|诚邀|工作|简介|新闻|掠影|风采
|登录|注销|注册|使用|体验|立即|收藏夹|收藏|添加|加入
|更多|more|专题|精选|热卖|热销|推荐|精彩
|加盟|联盟|友情|链接|相关
|订阅|阅读器|RSS
|免责|条款|声明|我的|我们|组织|概况|有限|免费|公司|法律|导航|广告|地图|隐私
|〖|〗|【|】|（|）|［|］|『|』|\.";

            var re = new Regex(@"<a\s+[^>]*href\s*=\s*[^>]+>[\s\S]*?</a>", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            var re2 = new Regex(@"""|'", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            var mcs = re.Matches(sContent);
            //foreach (Match mc in mcs)
            for (var i = mcs.Count - 1; i >= 0; i--)
            {
                var mc = mcs[i];
                var strHref = GetLink(mc.Value).Trim();

                strHref = strHref.Replace("\\\"", "");//针对JS输出链接
                strHref = strHref.Replace("\\\'", "");

                var strTemp = RemoveByReg(strHref, @"^http.*/$");//屏蔽以“http”开头“/”结尾的链接地址
                if (strTemp.Length < 2)
                {
                    continue;
                }

                //过滤广告或无意义的链接
                var strText = ClearTag(GetTextByLink(mc.Value)).Trim();
                strTemp = RemoveByReg(strText, sFilter);
                if (Encoding.Default.GetBytes(strTemp).Length < 9)
                {
                    continue;
                }
                if (re2.IsMatch(strText))
                {
                    continue;
                }

                //换上绝对地址
                strHref = GetUrlByRelative(sUrl, strHref);
                if (strHref.Length <= 18)//例如，http://www.163.com = 18
                {
                    continue;
                }

                //计算#字符出现的位置，移除它后面的内容
                //如果是域名地址，就跳过
                var charIndex = strHref.IndexOf('#');
                if (charIndex > -1)
                {
                    strHref = strHref.Substring(0, charIndex);
                }
                strHref = strHref.Trim(new char[] { '/', '\\' });
                var tmpDomainUrl = GetDomain(strHref);
                if (strHref.Equals(tmpDomainUrl, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!lisA.ContainsKey(strHref) && !lisA.ContainsValue(strText))
                {
                    lisA.Add(strHref, strText);
                }
            }
        }

        public static bool IsExistsScriptLink(string sHtml)
        {
            var re = new Regex(@"<script[^>]+src\s*=\s*(?:'(?<src>[^']+)'|""(?<src>[^""]+)""|(?<src>[^>\s]+))\s*[^>]*>", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
            return re.IsMatch(sHtml);
        }

        /// <summary>
        /// 在现有链接中用关键字过滤
        /// </summary>
        /// <param name="listA"></param>
        /// <param name="listKey"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetLinksByKey(Dictionary<string, string> listA, List<string> listKey)
        {
            if (listKey == null)
            {
                return listA;
            }

            var listNeed = new Dictionary<string, string>();

            //准备好关键字正则表达式
            var sKey = "";
            foreach (var s in listKey)
            {
                sKey += "([\\s\\S]*" + _ForReguSpeciChar(s) + "[\\s\\S]*)|";
            }
            sKey = (sKey != "") ? sKey.Substring(0, sKey.Length - 1) : "[\\s\\S]+";
            var reKey = new Regex(sKey, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);

            foreach (var kvp in listA)
            {
                if (reKey.Match(kvp.Value).Success)
                {
                    if (!listNeed.ContainsKey(kvp.Key))
                    {
                        listNeed.Add(kvp.Key, kvp.Value);
                    }
                }
            }

            return listNeed;
        }

        private static string _ForReguSpeciChar(string txtRegular)
        {
            var arSpecial = new string[] { ".", "$", "^", "{", "[", "(", "|", ")", "*", "+", "?", "#" };
            var txtTranRegular = txtRegular;

            foreach (var s in arSpecial)
            {
                txtTranRegular = txtTranRegular.Replace(s, "\\" + s);
            }

            return txtTranRegular;
        }


        /// <summary>
        /// 从RSS FEED中读取
        /// </summary>
        /// <param name="sContent"></param>
        /// <param name="listKey"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetLinksFromRss(string sContent, string sUrl)
        {
            var lisDes = new Dictionary<string, string>();
            return GetLinksFromRss(sContent, sUrl, ref lisDes);
        }

        public static Dictionary<string, string> GetLinksFromRss(string sContent, string sUrl, ref Dictionary<string, string> lisDes)
        {
            var listResult = new Dictionary<string, string>();

            var xml = new XmlDocument();

            //RSS2.0
            try
            {
                xml.LoadXml(sContent.Trim());
                var nodes = xml.SelectNodes("/rss/channel/item");
                if (nodes.Count > 0)
                {
                    //for (int i = 0; i < nodes.Count; i++)
                    for (var i = nodes.Count - 1; i >= 0; i--)
                    {
                        try
                        {
                            var sLink = GetUrlByRelative(sUrl, nodes[i].SelectSingleNode("link").InnerText);
                            listResult.Add(sLink, nodes[i].SelectSingleNode("title").InnerText);
                            lisDes.Add(sLink, nodes[i].SelectSingleNode("description").InnerText);
                        }
                        catch { }
                    }
                    return listResult;
                }

            }
            catch { }

            //RSS1.0（RDF）
            try
            {
                var nsMgr = new XmlNamespaceManager(xml.NameTable);
                nsMgr.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
                nsMgr.AddNamespace("rss", "http://purl.org/rss/1.0/");
                var nodes = xml.SelectNodes("/rdf:RDF//rss:item", nsMgr);
                if (nodes.Count > 0)
                {
                    for (var i = nodes.Count - 1; i >= 0; i--)
                    {
                        try
                        {
                            var sLink = GetUrlByRelative(sUrl, nodes[i].SelectSingleNode("rss:link", nsMgr).InnerText);
                            listResult.Add(sLink, nodes[i].SelectSingleNode("rss:title", nsMgr).InnerText);
                            lisDes.Add(sLink, nodes[i].SelectSingleNode("rss:description", nsMgr).InnerText);
                        }
                        catch { }
                        //listResult.Add("<a href=\"" + nodes[i].SelectSingleNode("rss:link",nsMgr).InnerText + "\">" + nodes[i].SelectSingleNode("rss:title",nsMgr).InnerText + "</a>");
                    }
                    return listResult;
                }
            }
            catch { }

            //RSS ATOM
            try
            {
                var nsMgr = new XmlNamespaceManager(xml.NameTable);
                nsMgr.AddNamespace("atom", "http://purl.org/atom/ns#");
                var nodes = xml.SelectNodes("/atom:feed/atom:entry", nsMgr);
                if (nodes.Count > 0)
                {
                    for (var i = nodes.Count - 1; i >= 0; i--)
                    {
                        try
                        {
                            var sLink = GetUrlByRelative(sUrl, nodes[i].SelectSingleNode("atom:link", nsMgr).Attributes["href"].InnerText);
                            listResult.Add(sLink, nodes[i].SelectSingleNode("atom:title", nsMgr).InnerText);
                            lisDes.Add(sLink, nodes[i].SelectSingleNode("atom:content", nsMgr).InnerText);
                        }
                        catch { }
                        //listResult.Add("<a href=\"" + nodes[i].SelectSingleNode("atom:link",nsMgr).Attributes["href"].InnerText + "\">" + nodes[i].SelectSingleNode("atom:title",nsMgr).InnerText + "</a>");
                    }
                    return listResult;
                }
            }
            catch { }

            return listResult;
        }
        public static string GetTitleFromRss(string sContent)
        {
            var title = "";
            var xml = new XmlDocument();

            //RSS2.0
            try
            {
                xml.LoadXml(sContent.Trim());
                title = xml.SelectSingleNode("/rss/channel/title").InnerText;
            }
            catch { }

            return title;
        }

        #region 已过时的方法
        [Obsolete("已过时的方法。")]
        public static List<string> GetLinksByKey(string sContent, /*string sUrl,*/ List<string> listKey)
        {
            var listResult = new List<string>();
            var list = new List<string>();
            var sKey = "";
            string strKey;

            //提取链接
            var re = new Regex(@"<a\s+[^>]*href\s*=\s*[^>]+>[\s\S]*?</a>", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            var mcs = re.Matches(sContent);
            foreach (Match mc in mcs)
            {
                strKey = RemoveByReg(GetLink(mc.Value), @"^http.*/$");//屏蔽以“http”开头“/”结尾的链接地址
                if (strKey.Length > 0)
                {
                    list.Add(mc.Value);
                }
            }

            //准备好关键字
            foreach (var s in listKey)
            {
                sKey += "([\\s\\S]*" + s + "[\\s\\S]*)|";
            }
            if (sKey != "")
                sKey = sKey.Substring(0, sKey.Length - 1);
            if (sKey == "")
                sKey = "[\\s\\S]+";
            var reKey = new Regex(sKey, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);

            Match tmpmc;
            //链接的文字一定要5个字以上才算有效？
            re = new Regex(@"<a\s+[^>]+>([\s\S]{5,})?</a>", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            foreach (var s in list)
            {
                tmpmc = re.Match(s);
                if (tmpmc.Success)
                {
                    strKey = ClearTag(tmpmc.Groups[1].Value.Trim());
                    strKey = RemoveByReg(strKey, @"更多|登录|添加|推荐|收藏夹|加盟|关于|订阅|阅读器|我的|有限|免费|公司|more|RSS|about|\.");
                    if (Encoding.Default.GetBytes(strKey).Length > 8)//最起码是5个是为了屏蔽垃圾信息。
                    {
                        if (reKey.Match(strKey).Success)
                        {
                            listResult.Add(s);
                        }
                    }
                }
            }

            #region 对RSS的支持
            if (listResult.Count == 0)
            {
                return GetLinksByKeyFromRss(sContent, listKey);
            }
            #endregion

            return listResult;
        }

        /// <summary>
        /// 从RSS FEED中读取
        /// </summary>
        /// <param name="sContent"></param>
        /// <param name="listKey"></param>
        /// <returns></returns>
        [Obsolete("已过时的方法。")]
        public static List<string> GetLinksByKeyFromRss(string sContent, List<string> listKey)
        {
            var listResult = new List<string>();

            var sKey = "";
            foreach (var s in listKey)
            {
                sKey += "([\\s\\S]*" + s + "[\\s\\S]*)|";
            }
            if (sKey != "")
                sKey = sKey.Substring(0, sKey.Length - 1);
            if (sKey == "")
                sKey = "[\\s\\S]+";
            var reKey = new Regex(sKey, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);

            var xml = new XmlDocument();

            //RSS2.0
            try
            {
                xml.LoadXml(sContent.Trim());
                var nodes = xml.SelectNodes("/rss/channel/item");
                if (nodes.Count > 0)
                {
                    for (var i = 0; i < nodes.Count; i++)
                    {
                        listResult.Add("<a href=\"" + nodes[i].SelectSingleNode("link").InnerText + "\">" + nodes[i].SelectSingleNode("title").InnerText + "</a>");
                        //listResult.Add(nodes[i].SelectSingleNode("link").InnerText, nodes[i].SelectSingleNode("title").InnerText);
                    }
                    return listResult;
                }

            }
            catch { }

            //RSS1.0（RDF）
            try
            {
                var nsMgr = new XmlNamespaceManager(xml.NameTable);
                nsMgr.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
                nsMgr.AddNamespace("rss", "http://purl.org/rss/1.0/");
                var nodes = xml.SelectNodes("/rdf:RDF//rss:item", nsMgr);
                if (nodes.Count > 0)
                {
                    for (var i = 0; i < nodes.Count; i++)
                    {
                        //listResult.Add(nodes[i].SelectSingleNode("rss:link", nsMgr).InnerText, nodes[i].SelectSingleNode("rss:title", nsMgr).InnerText);
                        listResult.Add("<a href=\"" + nodes[i].SelectSingleNode("rss:link", nsMgr).InnerText + "\">" + nodes[i].SelectSingleNode("rss:title", nsMgr).InnerText + "</a>");
                    }
                    return listResult;
                }
            }
            catch { }

            //RSS ATOM
            try
            {
                var nsMgr = new XmlNamespaceManager(xml.NameTable);
                nsMgr.AddNamespace("atom", "http://purl.org/atom/ns#");
                var nodes = xml.SelectNodes("/atom:feed/atom:entry", nsMgr);
                if (nodes.Count > 0)
                {
                    for (var i = 0; i < nodes.Count; i++)
                    {
                        //listResult.Add(nodes[i].SelectSingleNode("atom:link", nsMgr).Attributes["href"].InnerText, nodes[i].SelectSingleNode("atom:title", nsMgr).InnerText);
                        listResult.Add("<a href=\"" + nodes[i].SelectSingleNode("atom:link", nsMgr).Attributes["href"].InnerText + "\">" + nodes[i].SelectSingleNode("atom:title", nsMgr).InnerText + "</a>");
                    }
                    return listResult;
                }
            }
            catch { }

            return listResult;
        }
        #endregion

        public static string RemoveByReg(string sContent, string sRegex)
        {
            var re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            var mcs = re.Matches(sContent);
            foreach (Match mc in mcs)
            {
                sContent = sContent.Replace(mc.Value, "");
            }
            return sContent;
        }

        public static string ReplaceByReg(string sContent, string sReplace, string sRegex)
        {
            var re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            sContent = re.Replace(sContent, sReplace);
            return sContent;
        }
       
        /// <summary>
        ///  网页Body内容
        /// </summary>
        /// <param name="sContent"></param>
        /// <returns></returns>
        public static string GetBody(string sContent)
        {
            var re = new Regex(@"[\s\S]*?<\bbody\b[^>]*>", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            sContent = re.Replace(sContent, "");

            re = new Regex(@"</\bbody\b[^>]*>\s*</html>", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.RightToLeft);
            sContent = re.Replace(sContent, "");
            return sContent;
        }
        #endregion 根据超链接地址获取页面内容

        #region 根据内容作字符串分析
        public static string GetTextByReg(string sContent, string sRegex)
        {
            var re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            var mc = re.Match(sContent);
            var str = "";
            if (mc.Success)
                str = mc.Groups[0].Value;
            while (str.EndsWith("_"))
            {
                str = RemoveEndWith(str, "_");
            }
            return str;
        }

        // charset=[\s]*(?<Coding>[^'"]+)[\s]*['"]?[\s]*[/]?>
        public static string GetTextByReg(string sContent, string sRegex, string sGroupName)
        {
            var re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            var mc = re.Match(sContent);
            var str = "";
            if (mc.Success)
                str = mc.Groups[sGroupName].Value;
            return str;
        }

        /// <summary>
        /// 获得链接的绝对路径
        /// </summary>
        /// <param name="sUrl"></param>
        /// <param name="sRUrl"></param>
        /// <returns></returns>
        public static string GetUrlByRelative(string sUrl, string sRUrl)
        {
            try
            {
                //http://q.yesky.com/grp/dsc/view.do;jsessionid=A6324FD46B4893303124F70C0B2AAC1E?grpId=201595&rvId=8215876
                var baseUri = new Uri(sUrl);
                if (!sUrl.EndsWith("/"))
                {
                    var i = baseUri.Segments.Length - 1;
                    if (i > 0)
                    {
                        var file = baseUri.Segments[i];
                        if (file.IndexOf('.') < 1)
                        {

                            baseUri = new Uri(sUrl + "/");
                        }
                    }
                }

                var myUri = new Uri(baseUri, sRUrl);

                return myUri.AbsoluteUri;
            }
            catch
            {
                return sUrl;
            }
        }

        public static List<string> GetListByReg(string sContent, string sRegex)
        {
            var list = new List<string>();
            var re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            var mcs = re.Matches(sContent);
            foreach (Match mc in mcs)
            {
                list.Add(mc.Groups["href"].Value);
            }
            return list;
        }

        public static string GetDomainUrl(string sUrl)
        {
            try
            {
                var baseUri = new Uri(sUrl);

                return baseUri.Scheme + "://" + baseUri.Authority;
            }
            catch
            {
                return sUrl;
            }

            //Regex re = new Regex(@"http(s)?://([\w-]+\.)+(\w){2,}", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            //Match mc1 = re.Match(sUrl);
            //if (mc1.Success)
            //{
            //    return mc1.Value;
            //}
            //else
            //    return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sOri"></param>
        /// <returns></returns>
        public static List<string> GetKeys(string sOri)
        {
            if (sOri.Trim().Length == 0)
            {
                return null;
            }

            //Regex re = new Regex("(,{1})|(，{1})", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            //string[] sArray = re.Split(sOri);
            var sArray = sOri.Split(new char[] { ',', '，', '\\', '/', '、' });
            var listStr = new List<string>();
            foreach (var sContent in sArray)
            {
                if (sContent.Length == 0)
                    continue;
                listStr.Add(sContent);
            }
            return listStr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sOri"></param>
        /// <returns></returns>
        public static string Split(string sOri)
        {
            var re = new Regex("(,{1})|(，{1})|(\\+{1})|(＋{1})|(。{1})|(;{1})|(；{1})|(：{1})|(:{1})|(“{1})|(”{1})|(、{1})|(_{1})",
                                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            var sArray = re.Split(sOri);
            var listStr = new List<string>();
            listStr.Clear();
            foreach (var sContent in sArray)
            {
                if (sContent.Length <= 2)
                    continue;
                re = new Regex(@"[a-zA-z]+", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                var mcs = re.Matches(sContent);
                var sTemp = sContent;
                foreach (Match mc in mcs)
                {
                    if (mc.Value.ToString().Length > 2)
                        listStr.Add(mc.Value.ToString());
                    sTemp = sTemp.Replace(mc.Value.ToString(), ",");
                }
                re = new Regex(@",{1}", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                mcs = re.Matches(sTemp);
                foreach (var s in re.Split(sTemp))
                {
                    if (s.Trim().Length <= 2)
                        continue;
                    listStr.Add(s);
                }
            }
            var sReturn = "";
            for (var i = 0; i < listStr.Count - 1; i++)
            {
                for (var j = i + 1; j < listStr.Count; j++)
                {
                    if (listStr[i] == listStr[j])
                    {
                        listStr[j] = "";
                    }
                }
            }

            foreach (var str in listStr)
            {
                if (str.Length > 2)
                    sReturn += str + ",";
            }
            if (sReturn.Length > 0)
                return sReturn.Substring(0, sReturn.Length - 1);
            else
                return sReturn;
        }
        #endregion
        



        #region 杂项

        public static string GetTxtFromHtml(string sHtml)
        {
            var del = @"<head[^>]*>[\s\S]*?</head>";
            var content = RemoveByReg(sHtml, del);

            del = @"(<script[^>]*>[\s\S]*?</script>)|(<IFRAME[^>]*>[\s\S]*?</IFRAME>)|(<style[^>]*>[\s\S]*?</style>|<title[^>]*>[\s\S]*?</title>|<meta[^>]*>|<option[^>]*>[\s\S]*?</option>)";
            content = RemoveByReg(content, del);

            del = @"(&nbsp;)|([\n\t]+)";
            content = RemoveByReg(content, del);

            var re = @"(<table(\s+[^>]*)*>)|(<td(\s+[^>]*)*>)|(<tr(\s+[^>]*)*>)|(<p(\s+[^>]*)*>)|(<div(\s+[^>]*)*>)|(<ul(\s+[^>]*)*>)|(<li(\s+[^>]*)*>)|</table>|</td>|</tr>|</p>|<br>|</div>|</li>|</ul>|<p />|<br />";
            content = ReplaceByReg(content, "", re);
            content = ReplaceByReg(content, "", @"[\f\n\r\v]+");

            content = RemoveByReg(content, @"<a(\s+[^>]*)*>[\s\S]*?</a>");
            content = RemoveByReg(content, "<[^>]+>");//去除各种HTML标记，获得纯内容

            content = content.Replace("\n", "");
            content = content.Replace("\r", "");
            content = content.Trim();
            return content;
        }

        /// <summary>
        /// 和GetTxtFromHtml功能一样，不过保留换行符号
        /// </summary>
        /// <param name="sHtml"></param>
        /// <returns></returns>
        public static string GetTxtFromHtml2(string sHtml)
        {
            var del = @"<head[^>]*>[\s\S]*?</head>";
            var content = RemoveByReg(sHtml, del);

            del = @"(<script[^>]*>[\s\S]*?</script>)|(<IFRAME[^>]*>[\s\S]*?</IFRAME>)|(<style[^>]*>[\s\S]*?</style>|<title[^>]*>[\s\S]*?</title>|<meta[^>]*>|<option[^>]*>[\s\S]*?</option>)";
            content = RemoveByReg(content, del);

            del = @"(&nbsp;)|([\t]+)";//del = @"(&nbsp;)|([\n\t]+)";
            content = RemoveByReg(content, del);

            var re = @"(<table(\s+[^>]*)*>)|(<td(\s+[^>]*)*>)|(<tr(\s+[^>]*)*>)|(<p(\s+[^>]*)*>)|(<div(\s+[^>]*)*>)|(<ul(\s+[^>]*)*>)|(<li(\s+[^>]*)*>)|</table>|</td>|</tr>|</p>|<br>|</div>|</li>|</ul>|<p />|<br />";
            content = ReplaceByReg(content, "", re);
            //content = CText.ReplaceByReg(content, "", @"[\f\n\r\v]+");

            content = RemoveByReg(content, @"<a(\s+[^>]*)*>[\s\S]*?</a>");
            content = RemoveByReg(content, "<[^>]+>");//去除各种HTML标记，获得纯内容
            content = content.Trim();

            return content;
        }
        #endregion

        public static string RemoveEndWith(string sOrg, string sEnd)
        {
            if (sOrg.EndsWith(sEnd))
                sOrg = sOrg.Remove(sOrg.IndexOf(sEnd), sEnd.Length);
            return sOrg;
        }

        #region 根据超链接地址获取页面内容
        public static string GetHtmlByUrl(string sUrl)
        {
            return GetHtmlByUrl(sUrl, "auto");
        }

        public static string GetHtmlByUrl(string sUrl, string sCoding)
        {
            return GetHtmlByUrl(ref sUrl, sCoding);
        }
        public static string GetHtmlByUrl(ref string sUrl, string sCoding)
        {
            var content = "";

            try
            {
                var response = _MyGetResponse(sUrl);
                if (response == null)
                {
                    return content;
                }

                sUrl = response.ResponseUri.AbsoluteUri;

                var stream = response.GetResponseStream();
                var buffer = GetContent(stream);
                stream.Close();
                stream.Dispose();

                var charset = "";
                if (sCoding == null || sCoding == "" || sCoding.ToLower() == "auto")
                {//如果不指定编码，那么系统代为指定

                    //首先，从返回头信息中寻找
                    var ht = response.GetResponseHeader("Content-Type");
                    response.Close();
                    var regCharSet = "[\\s\\S]*charset=(?<charset>[\\S]*)";
                    var r = new Regex(regCharSet, RegexOptions.IgnoreCase);
                    var m = r.Match(ht);
                    charset = (m.Captures.Count != 0) ? m.Result("${charset}") : "";
                    if (charset == "-8") charset = "utf-8";

                    if (charset == "")
                    {//找不到，则在文件信息本身中查找

                        //先按gb2312来获取文件信息
                        content = System.Text.Encoding.GetEncoding("gb2312").GetString(buffer);

                        regCharSet = "(<meta[^>]*charset=(?<charset>[^>'\"]*)[\\s\\S]*?>)|(xml[^>]+encoding=(\"|')*(?<charset>[^>'\"]*)[\\s\\S]*?>)";
                        r = new Regex(regCharSet, RegexOptions.IgnoreCase);
                        m = r.Match(content);
                        if (m.Captures.Count == 0)
                        {//没办法，都找不到编码，只能返回按"gb2312"获取的信息
                            //content = CText.RemoveByReg(content, @"<!--[\s\S]*?-->");
                            return content;
                        }
                        charset = m.Result("${charset}");
                    }
                }
                else
                {
                    response.Close();
                    charset = sCoding.ToLower();
                }

                try
                {
                    content = System.Text.Encoding.GetEncoding(charset).GetString(buffer);
                }
                catch (ArgumentException)
                {//指定的编码不可识别
                    content = System.Text.Encoding.GetEncoding("gb2312").GetString(buffer);
                }

                //content = CText.RemoveByReg(content, @"<!--[\s\S]*?-->");
            }
            catch
            {
                content = "";
            }

            return content;
        }
        private static HttpWebResponse _MyGetResponse(string sUrl)
        {
            var iTimeOut = 10000;
            //try
            //{
            //    //iTimeOut = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SocketTimeOut"]);
            //}
            //catch { iTimeOut = 10000; }

            var bCookie = false;
            var bRepeat = false;
            var target = new Uri(sUrl);

        ReCatch:
            try
            {
                var resquest = (HttpWebRequest)WebRequest.Create(target);
                resquest.MaximumResponseHeadersLength = -1;
                resquest.ReadWriteTimeout = 120000;//120秒就超时
                resquest.Timeout = iTimeOut;
                resquest.MaximumAutomaticRedirections = 50;
                resquest.MaximumResponseHeadersLength = 5;
                resquest.AllowAutoRedirect = true;
                if (bCookie)
                {
                    resquest.CookieContainer = new CookieContainer();
                }
                resquest.UserAgent = "Mozilla/6.0 (compatible; MSIE 6.0; Windows NT 5.1)";
                //resquest.UserAgent = @"Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727; InfoPath.1) Web-Sniffer/1.0.24";
                //resquest.KeepAlive = true;
                return (HttpWebResponse)resquest.GetResponse();
            }
            catch (WebException we)
            {
                if (!bRepeat)
                {
                    bRepeat = true;
                    bCookie = true;
                    goto ReCatch;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        private static byte[] GetContent(Stream stream)
        {
            var arBuffer = new ArrayList();
            const int buffsize = 4096;

            try
            {
                var buffer = new byte[buffsize];
                var count = stream.Read(buffer, 0, buffsize);
                while (count > 0)
                {
                    for (var i = 0; i < count; i++)
                    {
                        arBuffer.Add(buffer[i]);
                    }
                    count = stream.Read(buffer, 0, buffsize);
                }
            }
            catch { }

            return (byte[])arBuffer.ToArray(System.Type.GetType("System.Byte"));
        }

        public static string GetHttpHead(string sUrl)
        {
            var sHead = "";
            var uri = new Uri(sUrl);
            try
            {
                var req = WebRequest.Create(uri);
                var resp = req.GetResponse();
                var headers = resp.Headers;
                var sKeys = headers.AllKeys;
                foreach (var sKey in sKeys)
                {
                    sHead += sKey + ":" + headers[sKey] + "\r\n";
                }
            }
            catch
            {
            }
            return sHead;
        }

        /// <summary>
        /// 处理框架页面问题。如果该页面是框架结构的话，返回该框架
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string[] DealWithFrame(string url, string content)
        {
            var regFrame = @"<frame\s+[^>]*src\s*=\s*(?:""(?<src>[^""]+)""|'(?<src>[^']+)'|(?<src>[^\s>""']+))[^>]*>";
            return DealWithFrame(regFrame, url, content);
        }

        /// <summary>
        /// 处理浮动桢问题。如果该页面存在浮动桢，返回浮动桢
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string[] DealWithIFrame(string url, string content)
        {
            var regiFrame = @"<iframe\s+[^>]*src\s*=\s*(?:""(?<src>[^""]+)""|'(?<src>[^']+)'|(?<src>[^\s>""']+))[^>]*>";
            return DealWithFrame(regiFrame, url, content);
        }

        private static string[] DealWithFrame(string strReg, string url, string content)
        {
            var alFrame = new ArrayList();
            var r = new Regex(strReg, RegexOptions.IgnoreCase);
            var m = r.Match(content);
            while (m.Success)
            {
                alFrame.Add(GetUrl(url, m.Groups["src"].Value));
                m = m.NextMatch();
            }

            return (string[])alFrame.ToArray(System.Type.GetType("System.String"));
        }

        #endregion 根据超链接地址获取页面内容

        #region 获得多个页面
        public static List<KeyValuePair<int, string>> GetHtmlByUrlList(List<KeyValuePair<int, string>> listUrl, string sCoding)
        {
            var iTimeOut = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SocketTimeOut"]);
            var sbHtml = new StringBuilder();
            var listResult = new List<KeyValuePair<int, string>>();
            var nBytes = 0;
            Socket sock = null;
            IPHostEntry ipHostInfo = null;
            try
            {
                // 初始化				
                var site = new Uri(listUrl[0].Value.ToString());
                try
                {
                    ipHostInfo = System.Net.Dns.GetHostEntry(site.Host);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                var ipAddress = ipHostInfo.AddressList[0];
                var remoteEp = new IPEndPoint(ipAddress, site.Port);
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.SendTimeout = iTimeOut;
                sock.ReceiveTimeout = iTimeOut;
                try
                {
                    sock.Connect(remoteEp);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                foreach (var kvUrl in listUrl)
                {
                    site = new Uri(kvUrl.Value);
                    var sendMsg = "GET " + HttpUtility.UrlDecode(site.PathAndQuery) + " HTTP/1.1\r\n" +
                        "Accept: image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/vnd.ms-excel, application/msword, application/vnd.ms-powerpoint, */*\r\n" +
                        "Accept-Language:en-us\r\n" +
                        "Accept-Encoding:gb2312, deflate\r\n" +
                        "User-Agent: Mozilla/4.0\r\n" +
                        "Host: " + site.Host + "\r\n\r\n" + '\0';
                    // 发送
                    var msg = Encoding.GetEncoding(sCoding).GetBytes(sendMsg);
                    if ((nBytes = sock.Send(msg)) == 0)
                    {
                        sock.Shutdown(SocketShutdown.Both);
                        sock.Close();
                        return listResult;
                    }
                    // 接受
                    var bytes = new byte[2048];
                    var bt = Convert.ToByte('\x7f');
                    do
                    {
                        var count = 0;
                        try
                        {
                            nBytes = sock.Receive(bytes, bytes.Length - 1, 0);
                        }
                        catch (Exception ex)
                        {
                            var str = ex.Message;
                            nBytes = -1;
                        }
                        if (nBytes <= 0) break;
                        if (bytes[nBytes - 1] > bt)
                        {
                            for (var i = nBytes - 1; i >= 0; i--)
                            {
                                if (bytes[i] > bt)
                                    count++;
                                else
                                    break;
                            }
                            if (count % 2 == 1)
                            {
                                count = sock.Receive(bytes, nBytes, 1, 0);
                                if (count < 0)
                                    break;
                                nBytes = nBytes + count;
                            }
                        }
                        else
                            bytes[nBytes] = (byte)'\0';
                        var s = Encoding.GetEncoding(sCoding).GetString(bytes, 0, nBytes);
                        sbHtml.Append(s);
                    } while (nBytes > 0);

                    listResult.Add(new KeyValuePair<int, string>(kvUrl.Key, sbHtml.ToString()));
                    sbHtml = null;
                    sbHtml = new StringBuilder();
                }
            }
            catch (Exception ex)
            {
                var s = ex.Message;
                try
                {
                    sock.Shutdown(SocketShutdown.Both);
                    sock.Close();
                }
                catch { }
            }
            finally
            {
                try
                {
                    sock.Shutdown(SocketShutdown.Both);
                    sock.Close();
                }
                catch { }
            }
            return listResult;
        }
        #endregion 根据超链接地址获取页面内容

        public enum PageType : int { Html = 0, Rss };
        public static PageType GetPageType(string sUrl, ref string sHtml)
        {
            var pt = PageType.Html;

            //看有没有RSS FEED
            var regRss = @"<link\s+[^>]*((type=""application/rss\+xml"")|(type=application/rss\+xml))[^>]*>";
            var r = new Regex(regRss, RegexOptions.IgnoreCase);
            var m = r.Match(sHtml);
            if (m.Captures.Count != 0)
            {//有，则转向从RSS FEED中抓取
                var regHref = @"href=\s*(?:'(?<href>[^']+)'|""(?<href>[^""]+)""|(?<href>[^>\s]+))";
                r = new Regex(regHref, RegexOptions.IgnoreCase);
                m = r.Match(m.Captures[0].Value);
                if (m.Captures.Count > 0)
                {
                    //有可能是相对路径，加上绝对路径
                    var rssFile = GetUrl(sUrl, m.Groups["href"].Value);
                    sHtml = GetHtmlByUrl(rssFile);
                    pt = PageType.Rss;
                }
            }
            else
            {//看这个地址本身是不是一个Rss feed
                r = new Regex(@"<rss\s+[^>]*>", RegexOptions.IgnoreCase);
                m = r.Match(sHtml);
                if (m.Captures.Count > 0)
                {
                    pt = PageType.Rss;
                }
            }

            return pt;
        }
    }
}
