using System;
using System.IO;
using System.Threading;
using System.Web;

namespace Core.IO
{
    /// <summary>
    /// 文件下载类
    /// </summary>
    public class FileDown
    {
        public FileDown()
        { }

        /// <summary>
        /// 参数为虚拟路径
        /// </summary>
        public static string FileNameExtension(string fileName)
        {
            return Path.GetExtension(MapPathFile(fileName));
        }

        /// <summary>
        /// 获取物理地址
        /// </summary>
        public static string MapPathFile(string fileName)
        {
            return HttpContext.Current.Server.MapPath(fileName);
        }

        /// <summary>
        /// 普通下载
        /// </summary>
        /// <param name="fileName">文件虚拟路径</param>
        public static void DownLoadold(string fileName)
        {
            var destFileName = MapPathFile(fileName);
            if (File.Exists(destFileName))
            {
                var fi = new FileInfo(destFileName);
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ClearHeaders();
                HttpContext.Current.Response.Buffer = false;
                HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(Path.GetFileName(destFileName), System.Text.Encoding.UTF8));
                HttpContext.Current.Response.AppendHeader("Content-Length", fi.Length.ToString());
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.WriteFile(destFileName);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
            }
        }

        /// <summary>
        /// 分块下载
        /// </summary>
        /// <param name="fileName">文件虚拟路径</param>
        public static void DownLoad(string fileName)
        {
            var filePath = MapPathFile(fileName);
            long chunkSize = 204800;             //指定块大小 
            var buffer = new byte[chunkSize]; //建立一个200K的缓冲区 
            long dataToRead = 0;                 //已读的字节数   
            FileStream stream = null;
            try
            {
                //打开文件   
                stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                dataToRead = stream.Length;

                //添加Http头   
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachement;filename=" + HttpUtility.UrlEncode(Path.GetFileName(filePath)));
                HttpContext.Current.Response.AddHeader("Content-Length", dataToRead.ToString());

                while (dataToRead > 0)
                {
                    if (HttpContext.Current.Response.IsClientConnected)
                    {
                        var length = stream.Read(buffer, 0, Convert.ToInt32(chunkSize));
                        HttpContext.Current.Response.OutputStream.Write(buffer, 0, length);
                        HttpContext.Current.Response.Flush();
                        HttpContext.Current.Response.Clear();
                        dataToRead -= length;
                    }
                    else
                    {
                        dataToRead = -1; //防止client失去连接 
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write("Error:" + ex.Message);
            }
            finally
            {
                if (stream != null) stream.Close();
                HttpContext.Current.Response.Close();
            }
        }
     
        #region ResponseFile 输出硬盘文件，提供下载 支持大文件、续传、速度限制、资源占用小

        /// <summary>
        ///  输出硬盘文件，提供下载 支持大文件、续传、速度限制、资源占用小
        /// </summary>
        /// <param name="request">Page.Request对象</param>
        /// <param name="response">Page.Response对象</param>
        /// <param name="fileName">下载文件名</param>
        /// <param name="fullPath">带文件名下载路径</param>
        /// <param name="speed">每秒允许下载的字节数</param>
        /// <returns>返回是否成功</returns>
        //---------------------------------------------------------------------
        //调用：
        // string FullPath=Server.MapPath("count.txt");
        // ResponseFile(this.Request,this.Response,"count.txt",FullPath,100);
        //---------------------------------------------------------------------
        public static bool ResponseFile(HttpRequest request, HttpResponse response, string fileName, string fullPath, long speed)
        {
            try
            {
                var myFile = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var br = new BinaryReader(myFile);
                try
                {
                    response.AddHeader("Accept-Ranges", "bytes");
                    response.Buffer = false;

                    var fileLength = myFile.Length;
                    long startBytes = 0;
                    const int pack = 10240; //10K bytes
                    var sleep = (int)Math.Floor(d: (double)(1000 * pack / speed)) + 1;

                    if (request.Headers["Range"] != null)
                    {
                        response.StatusCode = 206;
                        var range = request.Headers["Range"].Split(new char[] { '=', '-' });
                        startBytes = Convert.ToInt64(range[1]);
                    }
                    response.AddHeader("Content-Length", (fileLength - startBytes).ToString());
                    if (startBytes != 0)
                    {
                        response.AddHeader("Content-Range", string.Format(" bytes {0}-{1}/{2}", startBytes, fileLength - 1, fileLength));
                    }

                    response.AddHeader("Connection", "Keep-Alive");
                    response.ContentType = "application/octet-stream";
                    response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));

                    br.BaseStream.Seek(startBytes, SeekOrigin.Begin);
                    var maxCount = (int)Math.Floor(d: (double)((fileLength - startBytes) / pack)) + 1;

                    for (var i = 0; i < maxCount; i++)
                    {
                        if (response.IsClientConnected)
                        {
                            response.BinaryWrite(br.ReadBytes(pack));
                            Thread.Sleep(sleep);
                        }
                        else
                        {
                            i = maxCount;
                        }
                    }
                }
                catch
                {
                    return false;
                }
                finally
                {
                    br.Close();
                    myFile.Close();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
