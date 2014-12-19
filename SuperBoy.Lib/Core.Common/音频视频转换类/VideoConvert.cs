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

        #region ����
        public static string Ffmpegtool = ConfigurationManager.AppSettings["ffmpeg"];
        public static string Mencodertool = ConfigurationManager.AppSettings["mencoder"];
        public static string Savefile = ConfigurationManager.AppSettings["savefile"] + "/";
        public static string SizeOfImg = ConfigurationManager.AppSettings["CatchFlvImgSize"];
        public static string WidthOfFile = ConfigurationManager.AppSettings["widthSize"];
        public static string HeightOfFile = ConfigurationManager.AppSettings["heightSize"];
        #endregion

        #region ��ȡ�ļ�������
        /// <summary>
        /// ��ȡ�ļ�������
        /// </summary>
        public static string GetFileName(string fileName)
        {
            var i = fileName.LastIndexOf("\\") + 1;
            var name = fileName.Substring(i);
            return name;
        }
        #endregion

        #region ��ȡ�ļ���չ��
        /// <summary>
        /// ��ȡ�ļ���չ��
        /// </summary>
        public static string GetExtension(string fileName)
        {
            var i = fileName.LastIndexOf(".") + 1;
            var name = fileName.Substring(i);
            return name;
        }
        #endregion

        #region ��ȡ�ļ�����
        /// <summary>
        /// ��ȡ�ļ�����
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

        #region ��Ƶ��ʽתΪFlv
        /// <summary>
        /// ��Ƶ��ʽתΪFlv
        /// </summary>
        /// <param name="vFileName">ԭ��Ƶ�ļ���ַ</param>
        /// <param name="exportName">���ɺ��Flv�ļ���ַ</param>
        public bool ConvertFlv(string vFileName, string exportName)
        {
            if ((!System.IO.File.Exists(Ffmpegtool)) || (!System.IO.File.Exists(HttpContext.Current.Server.MapPath(vFileName))))
            {
                return false;
            }
            vFileName = HttpContext.Current.Server.MapPath(vFileName);
            exportName = HttpContext.Current.Server.MapPath(exportName);
            var command = " -i \"" + vFileName + "\" -y -ab 32 -ar 22050 -b 800000 -s  480*360 \"" + exportName + "\""; //Flv��ʽ     
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

        #region ����Flv��Ƶ������ͼ
        /// <summary>
        /// ����Flv��Ƶ������ͼ
        /// </summary>
        /// <param name="vFileName">��Ƶ�ļ���ַ</param>
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

                //ע��:ͼƬ��ȡ�ɹ���,�������ڴ滺��д��������Ҫʱ��ϳ�,�����3,4����������;
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

        #region ����FFMpeg����Ƶ����(����·��)
        /// <summary>
        /// ת���ļ���������ָ���ļ�����
        /// </summary>
        /// <param name="fileName">�ϴ���Ƶ�ļ���·����ԭ�ļ���</param>
        /// <param name="playFile">ת������ļ���·�������粥���ļ���</param>
        /// <param name="imgFile">����Ƶ�ļ���ץȡ��ͼƬ·��</param>
        /// <returns>�ɹ�:����ͼƬ�����ַ;ʧ��:���ؿ��ַ���</returns>
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
                System.Diagnostics.Process.Start(filestartInfo);//ת��
                CatchImg(fileName, imgFile); //��ͼ
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

        #region ����FFMpeg����Ƶ����(���·��)
        /// <summary>
        /// ת���ļ���������ָ���ļ�����
        /// </summary>
        /// <param name="fileName">�ϴ���Ƶ�ļ���·����ԭ�ļ���</param>
        /// <param name="playFile">ת������ļ���·�������粥���ļ���</param>
        /// <param name="imgFile">����Ƶ�ļ���ץȡ��ͼƬ·��</param>
        /// <returns>�ɹ�:����ͼƬ�����ַ;ʧ��:���ؿ��ַ���</returns>
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

            ///ע��:ͼƬ��ȡ�ɹ���,�������ڴ滺��д��������Ҫʱ��ϳ�,�����3,4����������;   
            ///�����Ҫ��ʱ���ټ��,�ҷ�������ʱ8��,���������8��ͼƬ�Բ�����,��Ϊ��ͼʧ��;    
            if (System.IO.File.Exists(flvImg))
            {
                return flvImg;
            }
            return "";
        }
        #endregion

        #region ����mencoder����Ƶ������ת��(����·��)
        /// <summary>
        /// ����mencoder����Ƶ������ת��
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