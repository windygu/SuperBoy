using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using System.Collections;

namespace Core.Common
{
    /// <summary>
    /// 共用工具类
    /// </summary>
    public static class Tools
    {

        

        #region 简单验证

        /// <summary>
        /// 邮编有效性
        /// </summary>
        /// <param name="zip"></param>
        /// <returns></returns>
        public static bool IsValidZip(string zip)
        {
            var rx = new Regex(@"^\d{6}$", RegexOptions.None);
            var m = rx.Match(zip);
            return m.Success;
        }

  
        /// <summary>
        /// Url有效性
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        static public bool IsValidUrl(string url)
        {
            return Regex.IsMatch(url, @"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&%\$#\=~])*[^\.\,\)\(\s]$");
        }



        /// <summary>
        /// domain 有效性
        /// </summary>
        /// <param name="host">域名</param>
        /// <returns></returns>
        public static bool IsValidDomain(string host)
        {
            var r = new Regex(@"^\d+$");
            if (host.IndexOf(".") == -1)
            {
                return false;
            }
            return r.IsMatch(host.Replace(".", string.Empty)) ? false : true;
        }

       

        /// <summary>
        /// 验证字符串是否是GUID
        /// </summary>
        /// <param name="guid">字符串</param>
        /// <returns></returns>
        public static bool IsGuid(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return false;

            return Regex.IsMatch(guid, "[A-F0-9]{8}(-[A-F0-9]{4}){3}-[A-F0-9]{12}|[A-F0-9]{32}", RegexOptions.IgnoreCase);
        }

        #endregion
 
        #region 其他功能验证

        #endregion
     
        #region 对于用户权限从数据库中读出的解密过程
        public static string switch_riddle(string sCh)//解密
        {
            string sOut, sTemp, temp;
            var iLen = sCh.Length;
            if (iLen == 0 || sCh == "")
            {
                sOut = "0";
            }
            temp = "";
            sTemp = "";
            sOut = "";
            for (var i = 0; i <= iLen - 1; i++)
            {
                temp = sCh.Substring(i, 1);
                switch (temp)
                {
                    case "a": sTemp = "1010";
                        break;
                    case "b": sTemp = "1011";
                        break;
                    case "c": sTemp = "1100";
                        break;
                    case "d": sTemp = "1101";
                        break;
                    case "e": sTemp = "1110";
                        break;
                    case "f": sTemp = "1111";
                        break;
                    case "0": sTemp = "0000";
                        break;
                    case "1": sTemp = "0001";
                        break;
                    case "2": sTemp = "0010";
                        break;
                    case "3": sTemp = "0011";
                        break;
                    case "4": sTemp = "0100";
                        break;
                    case "5": sTemp = "0101";
                        break;
                    case "6": sTemp = "0110";
                        break;
                    case "7": sTemp = "0111";
                        break;
                    case "8": sTemp = "1000";
                        break;
                    case "9": sTemp = "1001";
                        break;
                    default: sTemp = "0000";
                        break;
                }
                sOut = sOut + sTemp;
                sTemp = "";
            }
            return sOut;
        }
        #endregion
    
        #region 用户权限的加密过程
        public static string switch_encrypt(string sCh)
        {
            string sOut, sTemp, temp;
            var iLen = 64;
            if (iLen == 0 || sCh == "")
            {
                sOut = "0000";
            }
            temp = "";
            sTemp = "";
            sOut = "";
            for (var i = 0; i <= iLen - 1; i = i + 4)
            {
                temp = sCh.Substring(i, 4);
                switch (temp)
                {
                    case "1010": sTemp = "a";
                        break;
                    case "1011": sTemp = "b";
                        break;
                    case "1100": sTemp = "c";
                        break;
                    case "1101": sTemp = "d";
                        break;
                    case "1110": sTemp = "e";
                        break;
                    case "1111": sTemp = "f";
                        break;
                    case "0000": sTemp = "0";
                        break;
                    case "0001": sTemp = "1";
                        break;
                    case "0010": sTemp = "2";
                        break;
                    case "0011": sTemp = "3";
                        break;
                    case "0100": sTemp = "4";
                        break;
                    case "0101": sTemp = "5";
                        break;
                    case "0110": sTemp = "6";
                        break;
                    case "0111": sTemp = "7";
                        break;
                    case "1000": sTemp = "8";
                        break;
                    case "1001": sTemp = "9";
                        break;
                    default: sTemp = "0";
                        break;
                }
                sOut = sOut + sTemp;
                sTemp = "";
            }
            return sOut;
        }//加密
        #endregion

        #region   访问权限
        public static bool CheckTrue(string sAdmin, int a)
        {
            var sTemp = "";
            sTemp = sAdmin.Substring(a - 1, 1);   //s_admin为全局变量
            if (sTemp == "" || sTemp == "1")
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion

        #region 用户名密码格式

        /// <summary>
        /// 返回字符串真实长度, 1个汉字长度为2
        /// </summary>
        /// <returns>字符长度</returns>
        public static int GetStringLength(string stringValue)
        {
            return Encoding.Default.GetBytes(stringValue).Length;
        }

        /// <summary>
        /// 检测用户名格式是否有效
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool IsValidUserName(string userName)
        {
            var userNameLength = GetStringLength(userName);
            if (userNameLength >= 4 && userNameLength <= 20 && Regex.IsMatch(userName, @"^([\u4e00-\u9fa5A-Za-z_0-9]{0,})$"))
            {   // 判断用户名的长度（4-20个字符）及内容（只能是汉字、字母、下划线、数字）是否合法
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 密码有效性
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool IsValidPassword(string password)
        {
            return Regex.IsMatch(password, @"^[A-Za-z_0-9]{6,16}$");
        }
        #endregion


        #region 是否由特定字符组成
        public static bool isContainSameChar(string strInput)
        {
            var charInput = string.Empty;
            if (!string.IsNullOrEmpty(strInput))
            {
                charInput = strInput.Substring(0, 1);
            }
            return isContainSameChar(strInput, charInput, strInput.Length);
        }

        public static bool isContainSameChar(string strInput, string charInput, int lenInput)
        {
            if (string.IsNullOrEmpty(charInput))
            {
                return false;
            }
            else
            {
                var regNumber = new Regex(string.Format("^([{0}])+$", charInput));
                //Regex RegNumber = new Regex(string.Format("^([{0}]{{1}})+$", charInput,lenInput));
                var m = regNumber.Match(strInput);
                return m.Success;
            }
        }
        #endregion

       

        /// <summary>
        /// 错误消息输出
        /// </summary>
        private static void page_Error(object sender, EventArgs e)
        {
            var page1 = (Page)sender;
            var exception1 = page1.Server.GetLastError();
            var builder1 = new StringBuilder();
            builder1.Append("<div style=\"font-size:10pt;font-family:verdana;line-height:150%;\">");
            builder1.AppendFormat("<strong>\u9519\u8bef\u6d88\u606f\uff1a</strong>{0} \n", exception1.Message);
            builder1.AppendFormat("<strong>\u5bfc\u81f4\u9519\u8bef\u7684\u5e94\u7528\u7a0b\u5e8f\u6216\u5bf9\u8c61\u7684\u540d\u79f0</strong>\uff1a{0} \n", exception1.Source);
            builder1.AppendFormat("<div style=\"background-color:yellow;color:red;padding:12px;\"><strong>\u5806\u6808\u5185\u5bb9</strong>\uff1a{0} </div>\n", exception1.StackTrace);
            builder1.AppendFormat("<strong>\u5f15\u53d1\u5f02\u5e38\u7684\u65b9\u6cd5</strong>\uff1a{0} \n", exception1.TargetSite);
            builder1.AppendFormat("<strong>\u9519\u8bef\u9875\u9762</strong>\uff1a{0} \n", page1.Request.RawUrl);
            builder1.Append("</div>");
            page1.Server.ClearError();
            page1.Response.Write(builder1.ToString().Replace("\n", "<br/>"));
        }

        /// <summary>
        /// MyMethod()
        /// </summary>
        public static void RegisterOnlineDebug(Page page)
        {
            var uri1 = page.Request.UrlReferrer;
            if (((page.Request["DEBUG"] != null) && (page.Request["DEBUG"] == "true")) ||
                ((uri1 != null) && (uri1.ToString().IndexOf("DEBUG=") != -1)))
            {
                page.Error += new EventHandler(page_Error);
            }
        }



        #region 字符文本类


        /// <summary>
        /// HTML代码生成
        /// </summary>
        /// <param name="dTab">数据表格源</param>
        /// <param name="objModel">模版数据(Item,AlterItem,LiteralCount,Lieteral,Header,Footer)</param>
        /// <returns>根据相应Repeater模版生成的字符集</returns>
        /// <remarks>ST:ToPractice More, Fixed 2006-4-4</remarks>
        public static string GeneralHtmlBind(DataTable dTab, params string[] objModel)
        {
            var sb = new StringBuilder(5000);
            DataRow dRow = null;
            var k = 1;

            if (dTab != null)
            {
                for (var i = 0; i < dTab.Rows.Count; i++)
                {
                    dRow = dTab.Rows[i];
                    if (k == 1 && objModel.Length > 4)
                    {
                        sb.Append(String.Format(objModel[4] + "\n", dRow.ItemArray));
                    }

                    if (objModel.Length > 1 && objModel[1] != string.Empty && k % 2 == 0)
                    {
                        sb.Append(String.Format(objModel[1].Replace("$", k.ToString()), dRow.ItemArray));
                    }
                    else
                    {
                        sb.Append(String.Format(objModel[0].Replace("$", k.ToString()), dRow.ItemArray));
                    }

                    if (objModel.Length > 3 && objModel[3] != string.Empty && IsNumerical(objModel[2]))
                    {
                        if (k % int.Parse(objModel[2]) == 0 && k < dTab.Rows.Count)
                        {
                            sb.Append(objModel[3]);
                        }
                    }

                    if (k == dTab.Rows.Count && objModel.Length > 5)
                    {
                        sb.Append(String.Format(objModel[5] + "\n", dRow.ItemArray));
                    }
                    k++;
                }
                dTab.Dispose();
            }
            return sb.ToString();
        }



        /// <summary>
        /// 判断输入对象是否为数字类型
        /// </summary>
        /// <param name="strInput">输入对象</param>
        /// <returns>是否为不含小数点的数字类型</returns>
        private static bool IsNumerical(object strInput)
        {
            if (strInput == null)
            {
                return false;

            }
            var bValue = true;
            var strCheck = strInput.ToString();
            if (strCheck.Length == 0) return false;
            for (var i = 0; i < strCheck.Length; i++)
            {
                if (!char.IsDigit(strCheck, i))
                {
                    bValue = false;
                    break;
                }
            }
            return bValue;
        }
    
        /// <summary>
        /// 获取定长像素的网页片段，多余部分用省略号代替。(ellipsis)
        /// </summary>
        /// <param name="htmlText">片段内容</param>
        /// <param name="width">片段长度</param>
        /// <returns>相关属性样式的DIV片段</returns>
        /// <remarks>2008-1-29 by Ridge Wong</remarks>
        public static string GetFixedDivEllipsis(string htmlText, int width)
        {
            return string.Format("<div style=\"width:{0}px;overflow:hidden;float:left;text-overflow:ellipsis;white-space:nowrap;\">{1}</div>",
                width, htmlText);
        }

        /// <summary>
        /// 返回指定数据的重复次数
        /// </summary>
        /// <param name="objInt">int型数据</param>
        /// <param name="strRepeat">重复的字符片段</param>
        /// <returns>Example:ReplicateObject(5,"*")="*****"</returns>
        public static string ReplicateObject(object objInt, string strRepeat)
        {
            var strRet = string.Empty;
            if (objInt != null)
            {
                var count = Convert.ToInt32(objInt);
                strRet = (count > 0) ? (new string('*', count)) : "";
            }
            else
            {
                strRet = "";
            }
            return strRet.Replace("*", strRepeat);
        }

        /// <summary>
        /// 去掉小数位后无意义的0，如21.50结尾为21.5。
        /// </summary>
        /// <param name="pointNum">金额绑定对象</param>
        /// <returns>相关小数点金额</returns>
        public static string StripZeroEnd(object pointNum)
        {
            if (pointNum == null) { return "0"; }
            return Regex.Replace(pointNum.ToString(), @"(\.)?0{1,}$", "");
        }

        #endregion 字符文本类

        
 
    


        #region 文本框格式化(value=?使用)
        ///   <summary>   
        ///   过滤输出字符串   
        ///   </summary>   
        ///   <param   name="inputString">要过滤的字符串</param>   
        ///   <returns>过滤后的字符串</returns>   
        public static string Output(object inputString)
        {
            if (inputString == null)
                return string.Empty;

            var str1 = HttpContext.Current.Server.HtmlEncode(inputString.ToString());
            str1 = str1.Replace("&amp;", "&");
            str1 = str1.Replace("&lt;", "<");
            str1 = str1.Replace("&gt;", ">");
            str1 = str1.Replace("&quot;", ((char)34).ToString());
            return str1.ToString();

            //前台显示DataBinder.Eval(Container.DataItem,   "Content").ToString().Replace("<","&lt;").Replace(">","&gt;").Replace("\r\n","<br>").Replace("   ","&nbsp;")   
        }
        #endregion


        #region 文本框格式化(前台显示=?使用)
        ///   <summary>   
        ///   过滤输出字符串   
        ///   </summary>   
        ///   <param   name="inputString">要过滤的字符串</param>   
        ///   <returns>过滤后的字符串</returns>   
        public static string Outhtml(object htmlString)
        {
            if (htmlString == null)
                return string.Empty;

            var str2 = HttpContext.Current.Server.HtmlEncode(htmlString.ToString());
            str2 = str2.Replace("&", "&amp;");
            str2 = str2.Replace("<", "&lt;");
            str2 = str2.Replace(">", "&gt;");
            str2 = str2.Replace(((char)34).ToString(), "&quot;");
            str2 = str2.Replace("\r\n", "<br>");
            return str2.ToString();

            //前台显示DataBinder.Eval(Container.DataItem,   "Content").ToString().Replace("<","&lt;").Replace(">","&gt;").Replace("\r\n","<br>").Replace("   ","&nbsp;")   
        }
        #endregion


        #region 使用正则表达式删除用户输入中的html内容
        /// <summary> 
        /// 使用正则表达式删除用户输入中的html内容 
        /// </summary> 
        /// <param name="text">输入内容</param> 
        /// <returns>清理后的文本</returns> 
        public static string clearHtml(string text)
        {
            string pattern;

            if (text.Length == 0)
                return text;

            pattern = @"(<[a-zA-Z].*?>)|(<[\/][a-zA-Z].*?>)";
            text = Regex.Replace(text, pattern, String.Empty, RegexOptions.IgnoreCase);
            text = text.Replace("<", "<");
            text = text.Replace(">", ">");

            return text;
        }
        public static string ClearHtml(string html)
        {
            if (html == string.Empty || string.IsNullOrEmpty(html))
                return "";
            var regexFrame = new Regex(@"<\/*[^<>]*>", RegexOptions.IgnoreCase);
            return regexFrame.Replace(html, string.Empty).Replace("&nbsp;", string.Empty);
        }
       
        #endregion


        #region 使用正则表达式删除用户输入中的JS脚本内容
        /// <summary> 
        /// 使用正则表达式删除用户输入中的JS脚本内容 
        /// </summary> 
        /// <param name="text">输入内容</param> 
        /// <returns>清理后的文本</returns> 
        public static string ClearScript(string text)
        {
            string pattern;

            if (text.Length == 0)
                return text;

            pattern = @"(?i)<script([^>])*>(\w|\W)*</script([^>])*>";
            text = Regex.Replace(text, pattern, String.Empty, RegexOptions.IgnoreCase);

            pattern = @"<script([^>])*>";
            text = Regex.Replace(text, pattern, String.Empty, RegexOptions.IgnoreCase);

            pattern = @"</script>";
            text = Regex.Replace(text, pattern, String.Empty, RegexOptions.IgnoreCase);

            return text;
        }
        #endregion


        #region 过滤SQL,所有涉及到输入的用户直接输入的地方都要使用
        /// <summary> 
        /// 过滤SQL,所有涉及到输入的用户直接输入的地方都要使用。 
        /// </summary> 
        /// <param name="text">输入内容</param> 
        /// <returns>过滤后的文本</returns> 
        public static string FilterSql(string text)
        {
            text = text.Replace("'", "''");
            text = text.Replace("{", "{");
            text = text.Replace("}", "}");

            return text;
        }
        #endregion


        #region 过滤SQL,将SQL字符串里面的(')转换成('')，再在字符串的两边加上(')
        /// <summary> 
        /// 将SQL字符串里面的(')转换成('')，再在字符串的两边加上(')。 
        /// </summary> 
        /// <param name="text">输入内容</param> 
        /// <returns>过滤后的文本</returns> 
        public static String GetQuotedString(String text)
        {
            return ("'" + FilterSql(text) + "'");
        }
        #endregion
       

        #region 提取中文首字母
        #region 获取中文字首字拼写
        /// <summary>
        /// 获取中文字首字拼写
        /// </summary>
        /// <param name="chinese"></param>
        /// <returns></returns>
        static public string GetChineseSpell(string strText)
        {
            var len = strText.Length;
            var myStr = "";
            for (var i = 0; i < len; i++)
            {
                myStr += GetSpell(strText.Substring(i, 1));
            }
            return myStr;
        }
        #endregion

        #region 获取单个字首字母 (GB2312)
        /// <summary>
        /// 获取单个字首字母 (GB2312)
        /// </summary>
        /// <param name="cnChar"></param>
        /// <returns></returns>
        static public string GetSpell(string cnChar)
        {
            var arrCn = Encoding.Default.GetBytes(cnChar);
            if (arrCn.Length > 1)
            {
                int area = (short)arrCn[0];
                int pos = (short)arrCn[1];
                var code = (area << 8) + pos;
                int[] areacode = { 45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481 };
                for (var i = 0; i < 26; i++)
                {
                    var max = 55290;
                    if (i != 25) max = areacode[i + 1];
                    if (areacode[i] <= code && code < max)
                    {
                        return Encoding.Default.GetString(new byte[] { (byte)(97 + i) });
                    }
                }
                return "*";
            }
            else return cnChar;
        }
        #endregion
        #endregion


        #region AjaxPro使用的分页方法
        /// <summary> 
        /// 使用AjaxPro时候使用的方法
        /// </summary> 
        /// <param name="pageIndex">第几页</param> 
        /// <param name="pageIndex">总共多少页</param> 
        /// <param name="pageIndex">当前页条数</param> 
        /// <param name="pageIndex">总条数</param> 
        /// <returns>分页导航</returns> 
        public static string AjaxPages(int pageIndex, int pageCount, int roscount, int counts)
        {
            var text = new StringBuilder();

            //新的分页
            text.Append("<br><TABLE class='tableborder' cellSpacing='1' cellPadding='3' width='98%'  border='0' align='center'>");
            text.Append("<td align='left' width='250px'>第" + pageIndex + "页/总" + pageCount + "页　本页" + roscount + "条/总" + counts + "条 </td>");
            text.Append("<td align='left' width='40px'><a href='javascript:JumpPage(1)'>首页</a></td>");
            if (pageIndex < pageCount)
            {
                text.Append("<td align='left' width='40px'><a href='javascript:JumpPage(" + (pageIndex + 1) + ")'>下一页</a></td>");
            }
            else
            {
                text.Append("<td align='left' width='40px'>下一页</a></td>");
            }
            if (pageIndex > 1)
            {
                text.Append("<td align='left' width='40px'><a href='javascript:JumpPage(" + (pageIndex - 1) + ")'>上一页</a></td>");
            }
            else
            {
                text.Append("<td align='left' width='40px'>上一页</a></td>");
            }
            text.Append("<td align='left' width='40px'><a href='javascript:JumpPage(" + pageCount + ")'>尾页</a><td>");

            var basePage = (pageIndex / 10) * 10;
            if (basePage > 0)
            {
                text.Append("<td align='left' width='20px'><a href='javascript:JumpPage(" + (basePage - 9) + ")'>&lt;&lt;</a></td>");
            }
            for (var j = 1; j < 11; j++)
            {
                var pageNumber = basePage + j;
                if (pageNumber > pageCount)
                {
                    break;
                }
                if (pageNumber == Convert.ToInt32(pageIndex))
                {
                    text.Append("<td align='left' width='20px'><font color='#FF0000'>" + pageNumber + "</font></td>");
                }
                else
                {
                    text.Append("<td align='left' width='20px'><a href='javascript:JumpPage(" + pageNumber + ")'>" + pageNumber + "</a></td>");
                }

            }
            if (pageCount - 1 > basePage)
            {
                text.Append("<td align='left' width='20px'><a href='javascript:JumpPage(" + (basePage + 11) + ")'>&gt;&gt;</a></td>");
            }
            text.Append("</table>");

            return text.ToString();
        }
        #endregion

        #region 替换XML文档不接受的字符
        /// <summary> 
        /// 替换XML文档不接受的字符 
        /// </summary> 
        /// <param name="input">传入值</param> 
        /// <returns>替换后的字符</returns> 
        public static string FormatForXml(object input)
        {
            var str = input.ToString();

            //替换XML文档不接受的字符 
            str = str.Replace(" ", " ");
            str = str.Replace("&", "&");
            str = str.Replace("\"", "''");
            str = str.Replace("'", "&apos;");

            return str;
        }
        #endregion
    }
}
