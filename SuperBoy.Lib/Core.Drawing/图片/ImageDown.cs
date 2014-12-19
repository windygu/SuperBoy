using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Core.Drawing
{
    /// <summary>
    /// 图片下载
    /// </summary>
    public class ImageDown
    {
        public ImageDown()
        { }

        #region 私有方法
        /// <summary>
        /// 获取图片标志
        /// </summary>
        private string[] GetImgTag(string htmlStr)
        {
            var regObj = new Regex("<img.+?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var strAry = new string[regObj.Matches(htmlStr).Count];
            var i = 0;
            foreach (Match matchItem in regObj.Matches(htmlStr))
            {
                strAry[i] = GetImgUrl(matchItem.Value);
                i++;
            }
            return strAry;
        }

        /// <summary>
        /// 获取图片URL地址
        /// </summary>
        private string GetImgUrl(string imgTagStr)
        {
            var str = "";
            var regObj = new Regex("http://.+.(?:jpg|gif|bmp|png)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            foreach (Match matchItem in regObj.Matches(imgTagStr))
            {
                str = matchItem.Value;
            }
            return str;
        }
        #endregion

        /// <summary>
        /// 下载图片到本地
        /// </summary>
        /// <param name="strHtml">HTML</param>
        /// <param name="path">路径</param>
        /// <param name="nowyymm">年月</param>
        /// <param name="nowdd">日</param>
        public string SaveUrlPics(string strHtml, string path)
        {
            var nowym = DateTime.Now.ToString("yyyy-MM");  //当前年月
            var nowdd = DateTime.Now.ToString("dd");       //当天号数
            path = path + nowym + "/" + nowdd;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var imgurlAry = GetImgTag(strHtml);
            try
            {
                for (var i = 0; i < imgurlAry.Length; i++)
                {
                    var preStr = System.DateTime.Now.ToString() + "_";
                    preStr = preStr.Replace("-", "");
                    preStr = preStr.Replace(":", "");
                    preStr = preStr.Replace(" ", "");
                    var wc = new WebClient();
                    wc.DownloadFile(imgurlAry[i], path + "/" + preStr + imgurlAry[i].Substring(imgurlAry[i].LastIndexOf("/") + 1));
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return strHtml;
        }
    }
}