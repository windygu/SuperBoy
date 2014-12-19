using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Html
{
     public class HtmlTools
    {
         #region 获得发表日期、编码
         public static DateTime GetCreateDate(string sContent, string sRegex)
         {
             var dt = System.DateTime.Now;

             var re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
             var mc = re.Match(sContent);
             if (mc.Success)
             {
                 try
                 {
                     var iYear = int.Parse(mc.Groups["Year"].Value);
                     var iMonth = int.Parse(mc.Groups["Month"].Value);
                     var iDay = int.Parse(mc.Groups["Day"].Value);
                     var iHour = dt.Hour;
                     var iMinute = dt.Minute;

                     var sHour = mc.Groups["Hour"].Value;
                     var sMintue = mc.Groups["Mintue"].Value;

                     if (sHour != "")
                         iHour = int.Parse(sHour);
                     if (sMintue != "")
                         iMinute = int.Parse(sMintue);

                     dt = new DateTime(iYear, iMonth, iDay, iHour, iMinute, 0);
                 }
                 catch { }
             }
             return dt;
         }
         #endregion 获得发表日期

    }
}
