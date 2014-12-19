using System;
using System.Text;
using System.IO;

namespace Core.Net
{
    public class FtpOperater
    {
        #region ����
        private FtpClient _ftp;
        /// <summary>
        /// ȫ��FTP���ʱ���
        /// </summary>
        public FtpClient Ftp
        {
            get { return _ftp; }
            set { _ftp = value; }
        }

        private string _server;
        /// <summary>
        /// Ftp������
        /// </summary>
        public string Server
        {
            get { return _server; }
            set { _server = value; }
        }

        private string _user;
        /// <summary>
        /// Ftp�û�
        /// </summary>
        public string User
        {
            get { return _user; }
            set { _user = value; }
        }

        private string _pass;
        /// <summary>
        /// Ftp����
        /// </summary>
        public string Pass
        {
            get { return _pass; }
            set { _pass = value; }
        }

        private string _folderZj;
        /// <summary>
        /// Ftp����
        /// </summary>
        public string FolderZj
        {
            get { return _folderZj; }
            set { _folderZj = value; }
        }

        private string _folderWx;
        /// <summary>
        /// Ftp����
        /// </summary>
        public string FolderWx
        {
            get { return _folderWx; }
            set { _folderWx = value; }
        }
        #endregion

        /// <summary>
        /// �õ��ļ��б�
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
        /// �����ļ�
        /// </summary>
        /// <param name="ftpFolder">ftpĿ¼</param>
        /// <param name="ftpFileName">ftp�ļ���</param>
        /// <param name="localFolder">����Ŀ¼</param>
        /// <param name="localFileName">�����ļ���</param>
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
        /// �޸��ļ�
        /// </summary>
        /// <param name="ftpFolder">����Ŀ¼</param>
        /// <param name="ftpFileName">�����ļ���temp</param>
        /// <param name="localFolder">����Ŀ¼</param>
        /// <param name="localFileName">�����ļ���</param>
        public bool AddMscFile(string ftpFolder, string ftpFileName, string localFolder, string localFileName, string bscInfo)
        {
            var sLine = "";
            var sResult = "";
            var path = "���Ӧ�ó������ڵ�������·��";
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
        /// ɾ���ļ�
        /// </summary>
        /// <param name="ftpFolder">ftpĿ¼</param>
        /// <param name="ftpFileName">ftp�ļ���</param>
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
        /// �ϴ��ļ�
        /// </summary>
        /// <param name="ftpFolder">ftpĿ¼</param>
        /// <param name="ftpFileName">ftp�ļ���</param>
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
        /// �����ļ�
        /// </summary>
        /// <param name="ftpFolder">ftpĿ¼</param>
        /// <param name="ftpFileName">ftp�ļ���</param>
        /// <param name="localFolder">����Ŀ¼</param>
        /// <param name="localFileName">�����ļ���</param>
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
        /// �õ�FTP���ļ���Ϣ
        /// </summary>
        /// <param name="ftpFolder">FTPĿ¼</param>
        /// <param name="ftpFileName">ftp�ļ���</param>
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
        /// ����FTP�������Ƿ�ɵ�½
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
        /// �õ�FTP���ļ���Ϣ
        /// </summary>
        /// <param name="ftpFolder">FTPĿ¼</param>
        /// <param name="ftpFileName">ftp�ļ���</param>
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
        /// �õ��ļ��б�
        /// </summary>
        /// <param name="ftpFolder">FTPĿ¼</param>
        /// <returns>FTPͨ�����</returns>
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
        ///�õ�FTP�������
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