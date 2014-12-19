using System.Web;
using System.Text;

namespace Core.Drawing
{
    public static class Psd2SwfHelper
    {
        /// <summary>
        /// 转换所有的页，图片质量80%
        /// </summary>
        /// <param name="pdfPath">PDF文件地址</param>
        /// <param name="swfPath">生成后的SWF文件地址</param>
        public static bool PDF2SWF(string pdfPath, string swfPath)
        {
            return PDF2SWF(pdfPath, swfPath, 1, GetPageCount(HttpContext.Current.Server.MapPath(pdfPath)), 80);
        }

        /// <summary>
        /// 转换前N页，图片质量80%
        /// </summary>
        /// <param name="pdfPath">PDF文件地址</param>
        /// <param name="swfPath">生成后的SWF文件地址</param>
        /// <param name="page">页数</param>
        public static bool PDF2SWF(string pdfPath, string swfPath, int page)
        {
            return PDF2SWF(pdfPath, swfPath, 1, page, 80);
        }

        /// <summary>
        /// PDF格式转为SWF
        /// </summary>
        /// <param name="pdfPath">PDF文件地址</param>
        /// <param name="swfPath">生成后的SWF文件地址</param>
        /// <param name="beginpage">转换开始页</param>
        /// <param name="endpage">转换结束页</param>
        private static bool PDF2SWF(string pdfPath, string swfPath, int beginpage, int endpage, int photoQuality)
        {
            var exe = HttpContext.Current.Server.MapPath("~/Bin/tools/pdf2swf-0.9.1.exe");
            pdfPath = HttpContext.Current.Server.MapPath(pdfPath);
            swfPath = HttpContext.Current.Server.MapPath(swfPath);
            if (!System.IO.File.Exists(exe) || !System.IO.File.Exists(pdfPath) || System.IO.File.Exists(swfPath))
            {
                return false;
            }
            var sb = new StringBuilder();
            sb.Append(" \"" + pdfPath + "\"");
            sb.Append(" -o \"" + swfPath + "\"");
            sb.Append(" -s flashversion=9");
            if (endpage > GetPageCount(pdfPath)) endpage = GetPageCount(pdfPath);
            sb.Append(" -p " + "\"" + beginpage + "" + "-" + endpage + "\"");
            sb.Append(" -j " + photoQuality);
            var command = sb.ToString();
            var p = new System.Diagnostics.Process();
            p.StartInfo.FileName = exe;
            p.StartInfo.Arguments = command;
            p.StartInfo.WorkingDirectory = HttpContext.Current.Server.MapPath("~/Bin/");
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = false;
            p.Start();
            p.BeginErrorReadLine();
            p.WaitForExit();
            p.Close();
            p.Dispose();
            return true;
        }

        /// <summary>
        /// 返回页数
        /// </summary>
        /// <param name="pdfPath">PDF文件地址</param>
        private static int GetPageCount(string pdfPath)
        {
            var buffer = System.IO.File.ReadAllBytes(pdfPath);
            var length = buffer.Length;
            if (buffer == null)
                return -1;
            if (buffer.Length <= 0)
                return -1;
            var pdfText = Encoding.Default.GetString(buffer);
            var rx1 = new System.Text.RegularExpressions.Regex(@"/Type\s*/Page[^s]");
            var matches = rx1.Matches(pdfText);
            return matches.Count;
        }
    }
}
