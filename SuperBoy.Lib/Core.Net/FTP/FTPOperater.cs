using System;
using System.Text;
using System.IO;

namespace Core.Net
{
    public class FtpOperater
    {
        #region 属性
        private FtpClient _ftp;
        /// <summary>
        /// 全局FTP访问变量
        /// </summary>
        public FtpClient Ftp
        {
            get { return _ftp; }
            set { _ftp = value; }
        }

        private string _server;
        /// <summary>
        /// Ftp服务器
        /// </summary>
        public string Server
        {
            get { return _server; }
            set { _server = value; }
        }

        private string _user;
        /// <summary>
        /// Ftp用户
        /// </summary>
        public string User
        {
            get { return _user; }
            set { _user = value; }
        }

        private string _pass;
        /// <summary>
        /// Ftp密码
        /// </summary>
        public string Pass
        {
            get { return _pass; }
            set { _pass = value; }
        }

        private string _folderZj;
        /// <summary>
        /// Ftp密码
        /// </summary>
        public string FolderZj
        {
            get { return _folderZj; }
            set { _folderZj = value; }
        }

        private string _folderWx;
        /// <summary>
        /// Ftp密码
        /// </summary>
        public string FolderWx
        {
            get { return _folderWx; }
            set { _folderWx = value; }
        }
        #endregion

        /// <summary>
        /// 得到文件列表
        /// </summary>
        /// <returns></returns>
        public string[] GetList(string strPath)
        {
            if (_ftp == null) _ftp = this.GetFtpClient();
            _ftp.Connect();
            _ftp.ChDir(strPath);
            return _ftp.Dir("*");
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="ftpFolder">ftp目录</param>
        /// <param name="ftpFileName">ftp文件名</param>
        /// <param name="localFolder">本地目录</param>
        /// <param name="localFileName">本地文件名</param>
        public bool GetFile(string ftpFolder, string ftpFileName, string localFolder, string localFileName)
        {
            try
            {
                if (_ftp == null) _ftp = this.GetFtpClient();
                if (!_ftp.Connected)
                {
                    _ftp.Connect();
                    _ftp.ChDir(ftpFolder);
                }
                _ftp.Get(ftpFileName, localFolder, localFileName);

                return true;
            }
            catch
            {
                try
                {
                    _ftp.DisConnect();
                    _ftp = null;
                }
                catch { _ftp = null; }
                return false;
            }
        }

        /// <summary>
        /// 修改文件
        /// </summary>
        /// <param name="ftpFolder">本地目录</param>
        /// <param name="ftpFileName">本地文件名temp</param>
        /// <param name="localFolder">本地目录</param>
        /// <param name="localFileName">本地文件名</param>
        public bool AddMscFile(string ftpFolder, string ftpFileName, string localFolder, string localFileName, string bscInfo)
        {
            var sLine = "";
            var sResult = "";
            var path = "获得应用程序所在的完整的路径";
            path = path.Substring(0, path.LastIndexOf("\\"));
            try
            {
                var fsFile = new FileStream(ftpFolder + "\\" + ftpFileName, FileMode.Open);
                var fsFileWrite = new FileStream(localFolder + "\\" + localFileName, FileMode.Create);
                var sr = new StreamReader(fsFile);
                var sw = new StreamWriter(fsFileWrite);
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                while (sr.Peek() > -1)
                {
                    sLine = sr.ReadToEnd();
                }
                var arStr = sLine.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                for (var i = 0; i < arStr.Length - 1; i++)
                {
                    sResult += bscInfo + "," + arStr[i].Trim() + "\n";
                }
                sr.Close();
                var connect = new UTF8Encoding(true).GetBytes(sResult);
                fsFileWrite.Write(connect, 0, connect.Length);
                fsFileWrite.Flush();
                sw.Close();
                fsFile.Close();
                fsFileWrite.Close();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="ftpFolder">ftp目录</param>
        /// <param name="ftpFileName">ftp文件名</param>
        public bool DelFile(string ftpFolder, string ftpFileName)
        {
            try
            {
                if (_ftp == null) _ftp = this.GetFtpClient();
                if (!_ftp.Connected)
                {
                    _ftp.Connect();
                    _ftp.ChDir(ftpFolder);
                }
                _ftp.Delete(ftpFileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="ftpFolder">ftp目录</param>
        /// <param name="ftpFileName">ftp文件名</param>
        public bool PutFile(string ftpFolder, string ftpFileName)
        {
            try
            {
                if (_ftp == null) _ftp = this.GetFtpClient();
                if (!_ftp.Connected)
                {
                    _ftp.Connect();
                    _ftp.ChDir(ftpFolder);
                }
                _ftp.Put(ftpFileName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="ftpFolder">ftp目录</param>
        /// <param name="ftpFileName">ftp文件名</param>
        /// <param name="localFolder">本地目录</param>
        /// <param name="localFileName">本地文件名</param>
        public bool GetFileNoBinary(string ftpFolder, string ftpFileName, string localFolder, string localFileName)
        {
            try
            {
                if (_ftp == null) _ftp = this.GetFtpClient();
                if (!_ftp.Connected)
                {
                    _ftp.Connect();
                    _ftp.ChDir(ftpFolder);
                }
                _ftp.GetNoBinary(ftpFileName, localFolder, localFileName);
                return true;
            }
            catch
            {
                try
                {
                    _ftp.DisConnect();
                    _ftp = null;
                }
                catch
                {
                    _ftp = null;
                }
                return false;
            }
        }

        /// <summary>
        /// 得到FTP上文件信息
        /// </summary>
        /// <param name="ftpFolder">FTP目录</param>
        /// <param name="ftpFileName">ftp文件名</param>
        public string GetFileInfo(string ftpFolder, string ftpFileName)
        {
            var strResult = "";
            try
            {
                if (_ftp == null) _ftp = this.GetFtpClient();
                if (_ftp.Connected) _ftp.DisConnect();
                _ftp.Connect();
                _ftp.ChDir(ftpFolder);
                strResult = _ftp.GetFileInfo(ftpFileName);
                return strResult;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 测试FTP服务器是否可登陆
        /// </summary>
        public bool CanConnect()
        {
            if (_ftp == null) _ftp = this.GetFtpClient();
            try
            {
                _ftp.Connect();
                _ftp.DisConnect();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 得到FTP上文件信息
        /// </summary>
        /// <param name="ftpFolder">FTP目录</param>
        /// <param name="ftpFileName">ftp文件名</param>
        public string GetFileInfoConnected(string ftpFolder, string ftpFileName)
        {
            var strResult = "";
            try
            {
                if (_ftp == null) _ftp = this.GetFtpClient();
                if (!_ftp.Connected)
                {
                    _ftp.Connect();
                    _ftp.ChDir(ftpFolder);
                }
                strResult = _ftp.GetFileInfo(ftpFileName);
                return strResult;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 得到文件列表
        /// </summary>
        /// <param name="ftpFolder">FTP目录</param>
        /// <returns>FTP通配符号</returns>
        public string[] GetFileList(string ftpFolder, string strMask)
        {
            string[] strResult;
            try
            {
                if (_ftp == null) _ftp = this.GetFtpClient();
                if (!_ftp.Connected)
                {
                    _ftp.Connect();
                    _ftp.ChDir(ftpFolder);
                }
                strResult = _ftp.Dir(strMask);
                return strResult;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///得到FTP传输对象
        /// </summary>
        public FtpClient GetFtpClient()
        {
            var ft = new FtpClient();
            ft.RemoteHost = this.Server;
            ft.RemoteUser = this.User;
            ft.RemotePass = this.Pass;
            return ft;
        }
    }
}