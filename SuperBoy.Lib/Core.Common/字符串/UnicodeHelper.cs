using System;
using System.Linq;
using System.Text;

namespace Core.Common
{
    public class UnicodeHelper
    {
        /// <summary>
        /// 将原始字串转换为unicode,格式为\u.\u.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StringToUnicode(string str)
        {
            //中文转为UNICODE字符
            const string outStr = "";
            return string.IsNullOrEmpty(str) ? outStr : str.Aggregate(outStr, (current, t) => current + ("\\u" + ((int) t).ToString("x")));
        }

        /// <summary>
        /// 将Unicode字串\u.\u.格式字串转换为原始字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UnicodeToString(string str)
        {
            if (str == null) throw new ArgumentNullException("str");
            var outStr = "";

            str = "";// RegexHelper.Replace(str, "[\r\n]", "", 0);

            if (string.IsNullOrEmpty(str)) return outStr;
            var strlist = str.Replace("\\u", "㊣").Split('㊣');
            try
            {
                outStr += strlist[0];
                for (var i = 1; i < strlist.Length; i++)
                {
                    var strTemp = strlist[i];
                    if (string.IsNullOrEmpty(strTemp) || strTemp.Length < 4) continue;
                    strTemp = strlist[i].Substring(0, 4);
                    //将unicode字符转为10进制整数，然后转为char中文字符
                    outStr += (char)int.Parse(strTemp , System.Globalization.NumberStyles.HexNumber);
                    outStr += strlist[i].Substring(4);
                }
            }
            catch (FormatException ex)
            {
                outStr += "Erorr"+ex.Message;
            }
            return outStr;
        }

        /// <summary>
        /// GB2312转换成unicode编码 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Gb2Unicode(string str)
        {
            var gb = Encoding.GetEncoding("GB2312");
            var gbBytes = gb.GetBytes(str);
            return gbBytes.Select(t => "%" + t.ToString("X")).Aggregate("", (current, hh) => current + hh);
        }

        /// <summary>
        /// 得到单个字符的值
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        private static ushort GetByte(char ch)
        {
            ushort rtnNum;
            switch (ch)
            {
                case 'a':
                case 'A': rtnNum = 10; break;
                case 'b':
                case 'B': rtnNum = 11; break;
                case 'c':
                case 'C': rtnNum = 12; break;
                case 'd':
                case 'D': rtnNum = 13; break;
                case 'e':
                case 'E': rtnNum = 14; break;
                case 'f':
                case 'F': rtnNum = 15; break;
                default: rtnNum = ushort.Parse(ch.ToString()); break;

            }
            return rtnNum;
        }

        /// <summary>
        /// 转换一个字符，输入如"Π"中的"03a0"
        /// </summary>
        /// <param name="unicodeSingle"></param>
        /// <returns></returns>
        public static string ConvertSingle(string unicodeSingle)
        {
            if (unicodeSingle.Length!=4) 
                return null ;
              var unicode = Encoding.Unicode;
              var unicodeBytes = new byte[] { 0, 0 };
              for (var i = 0; i < 4; i++)
              {
                  switch (i)
                  {
                      case 0: unicodeBytes[1] += (byte)(GetByte(unicodeSingle[i]) * 16); break;
                      case 1: unicodeBytes[1] += (byte)GetByte(unicodeSingle[i]); break;
                      case 2: unicodeBytes[0] += (byte)(GetByte(unicodeSingle[i]) * 16); break;
                      case 3: unicodeBytes[0] += (byte)GetByte(unicodeSingle[i]); break;
                  }
              }

              var asciiChars = new char[unicode.GetCharCount(unicodeBytes, 0, unicodeBytes.Length)];
              unicode.GetChars(unicodeBytes, 0, unicodeBytes.Length, asciiChars, 0);
              var asciiString = new string(asciiChars);

              return asciiString;

        }

        /// <summary>
        /// unicode编码转换成GB2312汉字 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UtoGb(string str)
        {
            var ss = str.Replace("\\", "").Split('u');
            var bs = new Byte[ss.Length - 1];
            for (var i = 1; i < ss.Length; i++)
            {
                bs[i - 1] = Convert.ToByte(Convert2Hex(ss[i]));   //ss[0]为空串   
            }
            var chrs = Encoding.GetEncoding("GB2312").GetChars(bs);
            return chrs.Aggregate("", (current, t) => current + t.ToString());
        }

        private static string Convert2Hex(string pstr)
        {
            if (pstr.Length == 2)
            {
                pstr = pstr.ToUpper();
                const string hexstr = "0123456789ABCDEF";
                var cint = hexstr.IndexOf(pstr.Substring(0, 1), StringComparison.Ordinal) * 16 + hexstr.IndexOf(pstr.Substring(1, 1), StringComparison.Ordinal);
                return cint.ToString();
            }
            else
            {
                return "";
            }
        }

    }
}
