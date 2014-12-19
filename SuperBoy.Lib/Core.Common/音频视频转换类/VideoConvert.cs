using System.Configuration;
using System.Web;

namespace Core.Common
{
    //if (this.fload.HasFile)
    //{
    //    string upFileName = HttpContext.Current.Server.MapPath("~/savefile") + "\\" + this.fload.PostedFile.FileName;
    //    string saveName   = DateTime.Now.ToString("yyyyMMddHHmmssffff");
    //    string playFile   = Server.MapPath(VideoConvert.savefile + saveName);
    //    string imgFile    = Server.MapPath(VideoConvert.savefile + saveName);

    //    VideoConvert pm = new VideoConvert();
    //    string m_strExtension = VideoConvert.GetExtension(this.fload.PostedFile.FileName).ToLower();
    //    if (m_strExtension == "flv")
    //    {
    //        System.IO.File.Copy(upFileName, playFile + ".flv");
    //        pm.CatchImg(upFileName, imgFile);
    //    }
    //    string Extension = pm.CheckExtension(m_strExtension);
    //    if (Extension == "ffmpeg")
    //    {
    //        pm.ChangeFilePhy(upFileName, playFile, imgFile);
    //    }
    //    else if (Extension == "mencoder")
    //    {
    //        pm.MChangeFilePhy(upFileName, playFile, imgFile);
    //    }
    //}
    public class VideoConvert : System.Web.UI.Page
    {
        public VideoConvert()
        { }

        string[] _strArrMencoder = new string[] { "wmv", "rmvb", "rm" };
        string[] _strArrFfmpeg = new string[] { "asf", "avi", "mpg", "3gp", "mov" };

        #region 配置
        public static string Ffmpegtool = ConfigurationManager.AppSettings["ffmpeg"];
        public static string Mencodertool = ConfigurationManager.AppSettings["mencoder"];
        public static string Savefile = ConfigurationManager.AppSettings["savefile"] + "/";
        public static string SizeOfImg = ConfigurationManager.AppSettings["CatchFlvImgSize"];
        public static string WidthOfFile = ConfigurationManager.AppSettings["widthSize"];
        public static string HeightOfFile = ConfigurationManager.AppSettings["heightSize"];
        #endregion

        #region 获取文件的名字
        /// <summary>
        /// 获取文件的名字
        /// </summary>
        public static string GetFileName(string fileName)
        {
            var i = fileName.LastIndexOf("\\") + 1;
            var name = fileName.Substring(i);
            return name;
        }
        #endregion

        #region 获取文件扩展名
        /// <summary>
        /// 获取文件扩展名
        /// </summary>
        public static string GetExtension(string fileName)
        {
            var i = fileName.LastIndexOf(".") + 1;
            var name = fileName.Substring(i);
            return name;
        }
        #endregion

        #region 获取文件类型
        /// <summary>
        /// 获取文件类型
        /// </summary>
        public string CheckExtension(string extension)
        {
            var mStrReturn = "";
            foreach (var var in this._strArrFfmpeg)
            {
                if (var == extension)
                {
                    mStrReturn = "ffmpeg"; break;
                }
            }
            if (mStrReturn == "")
            {
                foreach (var var in _strArrMencoder)
                {
                    if (var == extension)
                    {
                        mStrReturn = "mencoder"; break;
                    }
                }
            }
            return mStrReturn;
        }
        #endregion

        #region 视频格式转为Flv
        /// <summary>
        /// 视频格式转为Flv
        /// </summary>
        /// <param name="vFileName">原视频文件地址</param>
        /// <param name="exportName">生成后的Flv文件地址</param>
        public bool ConvertFlv(string vFileName, string exportName)
        {
            if ((!System.IO.File.Exists(Ffmpegtool)) || (!System.IO.File.Exists(HttpContext.Current.Server.MapPath(vFileName))))
            {
                return false;
            }
            vFileName = HttpContext.Current.Server.MapPath(vFileName);
            exportName = HttpContext.Current.Server.MapPath(exportName);
            var command = " -i \"" + vFileName + "\" -y -ab 32 -ar 22050 -b 800000 -s  480*360 \"" + exportName + "\""; //Flv格式     
            var p = new System.Diagnostics.Process();
            p.StartInfo.FileName = Ffmpegtool;
            p.StartInfo.Arguments = command;
            p.StartInfo.WorkingDirectory = HttpContext.Current.Server.MapPath("~/tools/");
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = false;
            p.Start();
            p.BeginErrorReadLine();
            p.WaitForExit();
            p.Close();
            p.Dispose();
            return true;
        }
        #endregion

        #region 生成Flv视频的缩略图
        /// <summary>
        /// 生成Flv视频的缩略图
        /// </summary>
        /// <param name="vFileName">视频文件地址</param>
        public string CatchImg(string vFileName)
        {
            if ((!System.IO.File.Exists(Ffmpegtool)) || (!System.IO.File.Exists(HttpContext.Current.Server.MapPath(vFileName)))) return "";
            try
            {
                var flvImgP = vFileName.Substring(0, vFileName.Length - 4) + ".jpg";
                var command = " -i " + HttpContext.Current.Server.MapPath(vFileName) + " -y -f image2 -t 0.1 -s " + SizeOfImg + " " + HttpContext.Current.Server.MapPath(flvImgP);
                var p = new System.Diagnostics.Process();
                p.StartInfo.FileName = Ffmpegtool;
                p.StartInfo.Arguments = command;
                p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                try
                {
                    p.Start();
                }
                catch
                {
                    return "";
                }
                finally
                {
                    p.Close();
                    p.Dispose();
                }
                System.Threading.Thread.Sleep(4000);

                //注意:图片截取成功后,数据由内存缓存写到磁盘需要时间较长,大概在3,4秒甚至更长;
                if (System.IO.File.Exists(HttpContext.Current.Server.MapPath(flvImgP)))
                {
                    return flvImgP;
                }
                return "";
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region 运行FFMpeg的视频解码(绝对路径)
        /// <summary>
        /// 转换文件并保存在指定文件夹下
        /// </summary>
        /// <param name="fileName">上传视频文件的路径（原文件）</param>
        /// <param name="playFile">转换后的文件的路径（网络播放文件）</param>
        /// <param name="imgFile">从视频文件中抓取的图片路径</param>
        /// <returns>成功:返回图片虚拟地址;失败:返回空字符串</returns>
        public string ChangeFilePhy(string fileName, string playFile, string imgFile)
        {
            var ffmpeg = Server.MapPath(VideoConvert.Ffmpegtool);
            if ((!System.IO.File.Exists(ffmpeg)) || (!System.IO.File.Exists(fileName)))
            {
                return "";
            }
            var flvFile = System.IO.Path.ChangeExtension(playFile, ".flv");
            var flvImgSize = VideoConvert.SizeOfImg;
            var filestartInfo = new System.Diagnostics.ProcessStartInfo(ffmpeg);
            filestartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            filestartInfo.Arguments = " -i " + fileName + " -ab 56 -ar 22050 -b 500 -r 15 -s " + WidthOfFile + "x" + HeightOfFile + " " + flvFile;
            try
            {
                System.Diagnostics.Process.Start(filestartInfo);//转换
                CatchImg(fileName, imgFile); //截图
            }
            catch
            {
                return "";
            }
            return "";
        }

        public string CatchImg(string fileName, string imgFile)
        {
            var ffmpeg = Server.MapPath(VideoConvert.Ffmpegtool);
            var flvImg = imgFile + ".jpg";
            var flvImgSize = VideoConvert.SizeOfImg;
            var imgstartInfo = new System.Diagnostics.ProcessStartInfo(ffmpeg);
            imgstartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            imgstartInfo.Arguments = "   -i   " + fileName + "  -y  -f  image2   -ss 2 -vframes 1  -s   " + flvImgSize + "   " + flvImg;
            try
            {
                System.Diagnostics.Process.Start(imgstartInfo);
            }
            catch
            {
                return "";
            }
            if (System.IO.File.Exists(flvImg))
            {
                return flvImg;
            }
            return "";
        }
        #endregion

        #region 运行FFMpeg的视频解码(相对路径)
        /// <summary>
        /// 转换文件并保存在指定文件夹下
        /// </summary>
        /// <param name="fileName">上传视频文件的路径（原文件）</param>
        /// <param name="playFile">转换后的文件的路径（网络播放文件）</param>
        /// <param name="imgFile">从视频文件中抓取的图片路径</param>
        /// <returns>成功:返回图片虚拟地址;失败:返回空字符串</returns>
        public string ChangeFileVir(string fileName, string playFile, string imgFile)
        {
            var ffmpeg = Server.MapPath(VideoConvert.Ffmpegtool);
            if ((!System.IO.File.Exists(ffmpeg)) || (!System.IO.File.Exists(fileName)))
            {
                return "";
            }
            var flvImg = System.IO.Path.ChangeExtension(Server.MapPath(imgFile), ".jpg");
            var flvFile = System.IO.Path.ChangeExtension(Server.MapPath(playFile), ".flv");
            var flvImgSize = VideoConvert.SizeOfImg;

            var imgstartInfo = new System.Diagnostics.ProcessStartInfo(ffmpeg);
            imgstartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            imgstartInfo.Arguments = "   -i   " + fileName + "   -y   -f   image2   -t   0.001   -s   " + flvImgSize + "   " + flvImg;

            var filestartInfo = new System.Diagnostics.ProcessStartInfo(ffmpeg);
            filestartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            filestartInfo.Arguments = " -i " + fileName + " -ab 56 -ar 22050 -b 500 -r 15 -s " + WidthOfFile + "x" + HeightOfFile + " " + flvFile;
            try
            {
                System.Diagnostics.Process.Start(filestartInfo);
                System.Diagnostics.Process.Start(imgstartInfo);
            }
            catch
            {
                return "";
            }

            ///注意:图片截取成功后,数据由内存缓存写到磁盘需要时间较长,大概在3,4秒甚至更长;   
            ///这儿需要延时后再检测,我服务器延时8秒,即如果超过8秒图片仍不存在,认为截图失败;    
            if (System.IO.File.Exists(flvImg))
            {
                return flvImg;
            }
            return "";
        }
        #endregion

        #region 运行mencoder的视频解码器转换(绝对路径)
        /// <summary>
        /// 运行mencoder的视频解码器转换
        /// </summary>
        public string MChangeFilePhy(string vFileName, string playFile, string imgFile)
        {
            var tool = Server.MapPath(VideoConvert.Mencodertool);
            if ((!System.IO.File.Exists(tool)) || (!System.IO.File.Exists(vFileName)))
            {
                return "";
            }
            var flvFile = System.IO.Path.ChangeExtension(playFile, ".flv");
            var flvImgSize = VideoConvert.SizeOfImg;
            var filestartInfo = new System.Diagnostics.ProcessStartInfo(tool);
            filestartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            filestartInfo.Arguments = " " + vFileName + " -o " + flvFile + " -of lavf -lavfopts i_certify_that_my_video_stream_does_not_use_b_frames -oac mp3lame -lameopts abr:br=56 -ovc lavc -lavcopts vcodec=flv:vbitrate=200:mbd=2:mv0:trell:v4mv:cbp:last_pred=1:dia=-1:cmp=0:vb_strategy=1 -vf scale=" + WidthOfFile + ":" + HeightOfFile + " -ofps 12 -srate 22050";
            try
            {
                System.Diagnostics.Process.Start(filestartInfo);
                CatchImg(flvFile, imgFile);
            }
            catch
            {
                return "";
            }
            return "";
        }
        #endregion
    }
}