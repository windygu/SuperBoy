using System;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace Core.Net
{
    public class FtpClient
    {
        public static object Obj = new object();

        #region ���캯��
        /// <summary>
        /// ȱʡ���캯��
        /// </summary>
        public FtpClient()
        {
            _strRemoteHost = "";
            _strRemotePath = "";
            _strRemoteUser = "";
            _strRemotePass = "";
            _strRemotePort = 21;
            _bConnected = false;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        public FtpClient(string remoteHost, string remotePath, string remoteUser, string remotePass, int remotePort)
        {
            _strRemoteHost = remoteHost;
            _strRemotePath = remotePath;
            _strRemoteUser = remoteUser;
            _strRemotePass = remotePass;
            _strRemotePort = remotePort;
            Connect();
        }
        #endregion

        #region �ֶ�
        private int _strRemotePort;
        private Boolean _bConnected;
        private string _strRemoteHost;
        private string _strRemotePass;
        private string _strRemoteUser;
        private string _strRemotePath;

        /// <summary>
        /// ���������ص�Ӧ����Ϣ(����Ӧ����)
        /// </summary>
        private string _strMsg;
        /// <summary>
        /// ���������ص�Ӧ����Ϣ(����Ӧ����)
        /// </summary>
        private string _strReply;
        /// <summary>
        /// ���������ص�Ӧ����
        /// </summary>
        private int _iReplyCode;
        /// <summary>
        /// ���п������ӵ�socket
        /// </summary>
        private Socket _socketControl;
        /// <summary>
        /// ����ģʽ
        /// </summary>
        private TransferType _trType;
        /// <summary>
        /// ���պͷ������ݵĻ�����
        /// </summary>
        private static int _blockSize = 512;
        /// <summary>
        /// ���뷽ʽ
        /// </summary>
        Encoding _ascii = Encoding.ASCII;
        /// <summary>
        /// �ֽ�����
        /// </summary>
        Byte[] _buffer = new Byte[_blockSize];
        #endregion

        #region ����
        /// <summary>
        /// FTP������IP��ַ
        /// </summary>
        public string RemoteHost
        {
            get
            {
                return _strRemoteHost;
            }
            set
            {
                _strRemoteHost = value;
            }
        }

        /// <summary>
        /// FTP�������˿�
        /// </summary>
        public int RemotePort
        {
            get
            {
                return _strRemotePort;
            }
            set
            {
                _strRemotePort = value;
            }
        }

        /// <summary>
        /// ��ǰ������Ŀ¼
        /// </summary>
        public string RemotePath
        {
            get
            {
                return _strRemotePath;
            }
            set
            {
                _strRemotePath = value;
            }
        }

        /// <summary>
        /// ��¼�û��˺�
        /// </summary>
        public string RemoteUser
        {
            set
            {
                _strRemoteUser = value;
            }
        }

        /// <summary>
        /// �û���¼����
        /// </summary>
        public string RemotePass
        {
            set
            {
                _strRemotePass = value;
            }
        }

        /// <summary>
        /// �Ƿ��¼
        /// </summary>
        public bool Connected
        {
            get
            {
                return _bConnected;
            }
        }
        #endregion

        #region ����
        /// <summary>
        /// �������� 
        /// </summary>
        public void Connect()
        {
            lock (Obj)
            {
                _socketControl = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var ep = new IPEndPoint(IPAddress.Parse(RemoteHost), _strRemotePort);
                try
                {
                    _socketControl.Connect(ep);
                }
                catch (Exception)
                {
                    throw new IOException("��������ftp������");
                }
            }
            ReadReply();
            if (_iReplyCode != 220)
            {
                DisConnect();
                throw new IOException(_strReply.Substring(4));
            }
            SendCommand("USER " + _strRemoteUser);
            if (!(_iReplyCode == 331 || _iReplyCode == 230))
            {
                CloseSocketConnect();
                throw new IOException(_strReply.Substring(4));
            }
            if (_iReplyCode != 230)
            {
                SendCommand("PASS " + _strRemotePass);
                if (!(_iReplyCode == 230 || _iReplyCode == 202))
                {
                    CloseSocketConnect();
                    throw new IOException(_strReply.Substring(4));
                }
            }
            _bConnected = true;
            ChDir(_strRemotePath);
        }

        /// <summary>
        /// �ر�����
        /// </summary>
        public void DisConnect()
        {
            if (_socketControl != null)
            {
                SendCommand("QUIT");
            }
            CloseSocketConnect();
        }
        #endregion

        #region ����ģʽ
        /// <summary>
        /// ����ģʽ:���������͡�ASCII����
        /// </summary>
        public enum TransferType { Binary, Ascii };

        /// <summary>
        /// ���ô���ģʽ
        /// </summary>
        /// <param name="ttType">����ģʽ</param>
        public void SetTransferType(TransferType ttType)
        {
            if (ttType == TransferType.Binary)
            {
                SendCommand("TYPE I");//binary���ʹ���
            }
            else
            {
                SendCommand("TYPE A");//ASCII���ʹ���
            }
            if (_iReplyCode != 200)
            {
                throw new IOException(_strReply.Substring(4));
            }
            else
            {
                _trType = ttType;
            }
        }

        /// <summary>
        /// ��ô���ģʽ
        /// </summary>
        /// <returns>����ģʽ</returns>
        public TransferType GetTransferType()
        {
            return _trType;
        }
        #endregion

        #region �ļ�����
        /// <summary>
        /// ����ļ��б�
        /// </summary>
        /// <param name="strMask">�ļ�����ƥ���ַ���</param>
        public string[] Dir(string strMask)
        {
            if (!_bConnected)
            {
                Connect();
            }
            var socketData = CreateDataSocket();
            SendCommand("NLST " + strMask);
            if (!(_iReplyCode == 150 || _iReplyCode == 125 || _iReplyCode == 226))
            {
                throw new IOException(_strReply.Substring(4));
            }
            _strMsg = "";
            Thread.Sleep(2000);
            while (true)
            {
                var iBytes = socketData.Receive(_buffer, _buffer.Length, 0);
                _strMsg += _ascii.GetString(_buffer, 0, iBytes);
                if (iBytes < _buffer.Length)
                {
                    break;
                }
            }
            char[] seperator = { '\n' };
            var strsFileList = _strMsg.Split(seperator);
            socketData.Close(); //����socket�ر�ʱҲ���з�����
            if (_iReplyCode != 226)
            {
                ReadReply();
                if (_iReplyCode != 226)
                {

                    throw new IOException(_strReply.Substring(4));
                }
            }
            return strsFileList;
        }

        public void NewPutByGuid(string strFileName, string strGuid)
        {
            if (!_bConnected)
            {
                Connect();
            }
            var str = strFileName.Substring(0, strFileName.LastIndexOf("\\"));
            var strTypeName = strFileName.Substring(strFileName.LastIndexOf("."));
            strGuid = str + "\\" + strGuid;
            var socketData = CreateDataSocket();
            SendCommand("STOR " + Path.GetFileName(strGuid));
            if (!(_iReplyCode == 125 || _iReplyCode == 150))
            {
                throw new IOException(_strReply.Substring(4));
            }
            var input = new FileStream(strGuid, FileMode.Open);
            input.Flush();
            var iBytes = 0;
            while ((iBytes = input.Read(_buffer, 0, _buffer.Length)) > 0)
            {
                socketData.Send(_buffer, iBytes, 0);
            }
            input.Close();
            if (socketData.Connected)
            {
                socketData.Close();
            }
            if (!(_iReplyCode == 226 || _iReplyCode == 250))
            {
                ReadReply();
                if (!(_iReplyCode == 226 || _iReplyCode == 250))
                {
                    throw new IOException(_strReply.Substring(4));
                }
            }
        }

        /// <summary>
        /// ��ȡ�ļ���С
        /// </summary>
        /// <param name="strFileName">�ļ���</param>
        /// <returns>�ļ���С</returns>
        public long GetFileSize(string strFileName)
        {
            if (!_bConnected)
            {
                Connect();
            }
            SendCommand("SIZE " + Path.GetFileName(strFileName));
            long lSize = 0;
            if (_iReplyCode == 213)
            {
                lSize = Int64.Parse(_strReply.Substring(4));
            }
            else
            {
                throw new IOException(_strReply.Substring(4));
            }
            return lSize;
        }


        /// <summary>
        /// ��ȡ�ļ���Ϣ
        /// </summary>
        /// <param name="strFileName">�ļ���</param>
        /// <returns>�ļ���С</returns>
        public string GetFileInfo(string strFileName)
        {
            if (!_bConnected)
            {
                Connect();
            }
            var socketData = CreateDataSocket();
            SendCommand("LIST " + strFileName);
            var strResult = "";
            if (!(_iReplyCode == 150 || _iReplyCode == 125
                || _iReplyCode == 226 || _iReplyCode == 250))
            {
                throw new IOException(_strReply.Substring(4));
            }
            var b = new byte[512];
            var ms = new MemoryStream();

            while (true)
            {
                var iBytes = socketData.Receive(b, b.Length, 0);
                ms.Write(b, 0, iBytes);
                if (iBytes <= 0)
                {

                    break;
                }
            }
            var bt = ms.GetBuffer();
            strResult = System.Text.Encoding.ASCII.GetString(bt);
            ms.Close();
            return strResult;
        }

        /// <summary>
        /// ɾ��
        /// </summary>
        /// <param name="strFileName">��ɾ���ļ���</param>
        public void Delete(string strFileName)
        {
            if (!_bConnected)
            {
                Connect();
            }
            SendCommand("DELE " + strFileName);
            if (_iReplyCode != 250)
            {
                throw new IOException(_strReply.Substring(4));
            }
        }

        /// <summary>
        /// ������(������ļ����������ļ�����,�����������ļ�)
        /// </summary>
        /// <param name="strOldFileName">���ļ���</param>
        /// <param name="strNewFileName">���ļ���</param>
        public void Rename(string strOldFileName, string strNewFileName)
        {
            if (!_bConnected)
            {
                Connect();
            }
            SendCommand("RNFR " + strOldFileName);
            if (_iReplyCode != 350)
            {
                throw new IOException(_strReply.Substring(4));
            }
            //  ������ļ�����ԭ���ļ�����,������ԭ���ļ�
            SendCommand("RNTO " + strNewFileName);
            if (_iReplyCode != 250)
            {
                throw new IOException(_strReply.Substring(4));
            }
        }
        #endregion

        #region �ϴ�������
        /// <summary>
        /// ����һ���ļ�
        /// </summary>
        /// <param name="strFileNameMask">�ļ�����ƥ���ַ���</param>
        /// <param name="strFolder">����Ŀ¼(������\����)</param>
        public void Get(string strFileNameMask, string strFolder)
        {
            if (!_bConnected)
            {
                Connect();
            }
            var strFiles = Dir(strFileNameMask);
            foreach (var strFile in strFiles)
            {
                if (!strFile.Equals(""))//һ����˵strFiles�����һ��Ԫ�ؿ����ǿ��ַ���
                {
                    Get(strFile, strFolder, strFile);
                }
            }
        }

        /// <summary>
        /// ����һ���ļ�
        /// </summary>
        /// <param name="strRemoteFileName">Ҫ���ص��ļ���</param>
        /// <param name="strFolder">����Ŀ¼(������\����)</param>
        /// <param name="strLocalFileName">�����ڱ���ʱ���ļ���</param>
        public void Get(string strRemoteFileName, string strFolder, string strLocalFileName)
        {
            var socketData = CreateDataSocket();
            try
            {
                if (!_bConnected)
                {
                    Connect();
                }
                SetTransferType(TransferType.Binary);
                if (strLocalFileName.Equals(""))
                {
                    strLocalFileName = strRemoteFileName;
                }
                SendCommand("RETR " + strRemoteFileName);
                if (!(_iReplyCode == 150 || _iReplyCode == 125 || _iReplyCode == 226 || _iReplyCode == 250))
                {
                    throw new IOException(_strReply.Substring(4));
                }
                var output = new FileStream(strFolder + "\\" + strLocalFileName, FileMode.Create);
                while (true)
                {
                    var iBytes = socketData.Receive(_buffer, _buffer.Length, 0);
                    output.Write(_buffer, 0, iBytes);
                    if (iBytes <= 0)
                    {
                        break;
                    }
                }
                output.Close();
                if (socketData.Connected)
                {
                    socketData.Close();
                }
                if (!(_iReplyCode == 226 || _iReplyCode == 250))
                {
                    ReadReply();
                    if (!(_iReplyCode == 226 || _iReplyCode == 250))
                    {
                        throw new IOException(_strReply.Substring(4));
                    }
                }
            }
            catch
            {
                socketData.Close();
                socketData = null;
                _socketControl.Close();
                _bConnected = false;
                _socketControl = null;
            }
        }

        /// <summary>
        /// ����һ���ļ�
        /// </summary>
        /// <param name="strRemoteFileName">Ҫ���ص��ļ���</param>
        /// <param name="strFolder">����Ŀ¼(������\����)</param>
        /// <param name="strLocalFileName">�����ڱ���ʱ���ļ���</param>
        public void GetNoBinary(string strRemoteFileName, string strFolder, string strLocalFileName)
        {
            if (!_bConnected)
            {
                Connect();
            }

            if (strLocalFileName.Equals(""))
            {
                strLocalFileName = strRemoteFileName;
            }
            var socketData = CreateDataSocket();
            SendCommand("RETR " + strRemoteFileName);
            if (!(_iReplyCode == 150 || _iReplyCode == 125 || _iReplyCode == 226 || _iReplyCode == 250))
            {
                throw new IOException(_strReply.Substring(4));
            }
            var output = new FileStream(strFolder + "\\" + strLocalFileName, FileMode.Create);
            while (true)
            {
                var iBytes = socketData.Receive(_buffer, _buffer.Length, 0);
                output.Write(_buffer, 0, iBytes);
                if (iBytes <= 0)
                {
                    break;
                }
            }
            output.Close();
            if (socketData.Connected)
            {
                socketData.Close();
            }
            if (!(_iReplyCode == 226 || _iReplyCode == 250))
            {
                ReadReply();
                if (!(_iReplyCode == 226 || _iReplyCode == 250))
                {
                    throw new IOException(_strReply.Substring(4));
                }
            }
        }

        /// <summary>
        /// �ϴ�һ���ļ�
        /// </summary>
        /// <param name="strFolder">����Ŀ¼(������\����)</param>
        /// <param name="strFileNameMask">�ļ���ƥ���ַ�(���԰���*��?)</param>
        public void Put(string strFolder, string strFileNameMask)
        {
            var strFiles = Directory.GetFiles(strFolder, strFileNameMask);
            foreach (var strFile in strFiles)
            {
                Put(strFile);
            }
        }

        /// <summary>
        /// �ϴ�һ���ļ�
        /// </summary>
        /// <param name="strFileName">�����ļ���</param>
        public void Put(string strFileName)
        {
            if (!_bConnected)
            {
                Connect();
            }
            var socketData = CreateDataSocket();
            if (Path.GetExtension(strFileName) == "")
                SendCommand("STOR " + Path.GetFileNameWithoutExtension(strFileName));
            else
                SendCommand("STOR " + Path.GetFileName(strFileName));

            if (!(_iReplyCode == 125 || _iReplyCode == 150))
            {
                throw new IOException(_strReply.Substring(4));
            }

            var input = new FileStream(strFileName, FileMode.Open);
            var iBytes = 0;
            while ((iBytes = input.Read(_buffer, 0, _buffer.Length)) > 0)
            {
                socketData.Send(_buffer, iBytes, 0);
            }
            input.Close();
            if (socketData.Connected)
            {
                socketData.Close();
            }
            if (!(_iReplyCode == 226 || _iReplyCode == 250))
            {
                ReadReply();
                if (!(_iReplyCode == 226 || _iReplyCode == 250))
                {
                    throw new IOException(_strReply.Substring(4));
                }
            }
        }


        /// <summary>
        /// �ϴ�һ���ļ�
        /// </summary>
        /// <param name="strFileName">�����ļ���</param>
        public void PutByGuid(string strFileName, string strGuid)
        {
            if (!_bConnected)
            {
                Connect();
            }
            var str = strFileName.Substring(0, strFileName.LastIndexOf("\\"));
            var strTypeName = strFileName.Substring(strFileName.LastIndexOf("."));
            strGuid = str + "\\" + strGuid;
            System.IO.File.Copy(strFileName, strGuid);
            System.IO.File.SetAttributes(strGuid, System.IO.FileAttributes.Normal);
            var socketData = CreateDataSocket();
            SendCommand("STOR " + Path.GetFileName(strGuid));
            if (!(_iReplyCode == 125 || _iReplyCode == 150))
            {
                throw new IOException(_strReply.Substring(4));
            }
            var input = new FileStream(strGuid, FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
            var iBytes = 0;
            while ((iBytes = input.Read(_buffer, 0, _buffer.Length)) > 0)
            {
                socketData.Send(_buffer, iBytes, 0);
            }
            input.Close();
            File.Delete(strGuid);
            if (socketData.Connected)
            {
                socketData.Close();
            }
            if (!(_iReplyCode == 226 || _iReplyCode == 250))
            {
                ReadReply();
                if (!(_iReplyCode == 226 || _iReplyCode == 250))
                {
                    throw new IOException(_strReply.Substring(4));
                }
            }
        }
        #endregion

        #region Ŀ¼����
        /// <summary>
        /// ����Ŀ¼
        /// </summary>
        /// <param name="strDirName">Ŀ¼��</param>
        public void MkDir(string strDirName)
        {
            if (!_bConnected)
            {
                Connect();
            }
            SendCommand("MKD " + strDirName);
            if (_iReplyCode != 257)
            {
                throw new IOException(_strReply.Substring(4));
            }
        }

        /// <summary>
        /// ɾ��Ŀ¼
        /// </summary>
        /// <param name="strDirName">Ŀ¼��</param>
        public void RmDir(string strDirName)
        {
            if (!_bConnected)
            {
                Connect();
            }
            SendCommand("RMD " + strDirName);
            if (_iReplyCode != 250)
            {
                throw new IOException(_strReply.Substring(4));
            }
        }

        /// <summary>
        /// �ı�Ŀ¼
        /// </summary>
        /// <param name="strDirName">�µĹ���Ŀ¼��</param>
        public void ChDir(string strDirName)
        {
            if (strDirName.Equals(".") || strDirName.Equals(""))
            {
                return;
            }
            if (!_bConnected)
            {
                Connect();
            }
            SendCommand("CWD " + strDirName);
            if (_iReplyCode != 250)
            {
                throw new IOException(_strReply.Substring(4));
            }
            this._strRemotePath = strDirName;
        }
        #endregion

        #region �ڲ�����
        /// <summary>
        /// ��һ��Ӧ���ַ�����¼��strReply��strMsg,Ӧ�����¼��iReplyCode
        /// </summary>
        private void ReadReply()
        {
            _strMsg = "";
            _strReply = ReadLine();
            _iReplyCode = Int32.Parse(_strReply.Substring(0, 3));
        }

        /// <summary>
        /// ���������������ӵ�socket
        /// </summary>
        /// <returns>��������socket</returns>
        private Socket CreateDataSocket()
        {
            SendCommand("PASV");
            if (_iReplyCode != 227)
            {
                throw new IOException(_strReply.Substring(4));
            }
            var index1 = _strReply.IndexOf('(');
            var index2 = _strReply.IndexOf(')');
            var ipData = _strReply.Substring(index1 + 1, index2 - index1 - 1);
            var parts = new int[6];
            var len = ipData.Length;
            var partCount = 0;
            var buf = "";
            for (var i = 0; i < len && partCount <= 6; i++)
            {
                var ch = Char.Parse(ipData.Substring(i, 1));
                if (Char.IsDigit(ch))
                    buf += ch;
                else if (ch != ',')
                {
                    throw new IOException("Malformed PASV strReply: " + _strReply);
                }
                if (ch == ',' || i + 1 == len)
                {
                    try
                    {
                        parts[partCount++] = Int32.Parse(buf);
                        buf = "";
                    }
                    catch (Exception)
                    {
                        throw new IOException("Malformed PASV strReply: " + _strReply);
                    }
                }
            }
            var ipAddress = parts[0] + "." + parts[1] + "." + parts[2] + "." + parts[3];
            var port = (parts[4] << 8) + parts[5];
            var s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ep = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            try
            {
                s.Connect(ep);
            }
            catch (Exception)
            {
                throw new IOException("�޷�����ftp������");
            }
            return s;
        }

        /// <summary>
        /// �ر�socket����(���ڵ�¼��ǰ)
        /// </summary>
        private void CloseSocketConnect()
        {
            lock (Obj)
            {
                if (_socketControl != null)
                {
                    _socketControl.Close();
                    _socketControl = null;
                }
                _bConnected = false;
            }
        }

        /// <summary>
        /// ��ȡSocket���ص������ַ���
        /// </summary>
        /// <returns>����Ӧ������ַ�����</returns>
        private string ReadLine()
        {
            lock (Obj)
            {
                while (true)
                {
                    var iBytes = _socketControl.Receive(_buffer, _buffer.Length, 0);
                    _strMsg += _ascii.GetString(_buffer, 0, iBytes);
                    if (iBytes < _buffer.Length)
                    {
                        break;
                    }
                }
            }
            char[] seperator = { '\n' };
            var mess = _strMsg.Split(seperator);
            if (_strMsg.Length > 2)
            {
                _strMsg = mess[mess.Length - 2];
            }
            else
            {
                _strMsg = mess[0];
            }
            if (!_strMsg.Substring(3, 1).Equals(" ")) //�����ַ�����ȷ������Ӧ����(��220��ͷ,�����һ�ո�,�ٽ��ʺ��ַ���)
            {
                return ReadLine();
            }
            return _strMsg;
        }

        /// <summary>
        /// ���������ȡӦ��������һ��Ӧ���ַ���
        /// </summary>
        /// <param name="strCommand">����</param>
        public void SendCommand(String strCommand)
        {
            lock (Obj)
            {
                var cmdBytes = Encoding.ASCII.GetBytes((strCommand + "\r\n").ToCharArray());
                _socketControl.Send(cmdBytes, cmdBytes.Length, 0);
                Thread.Sleep(500);
                ReadReply();
            }
        }
        #endregion
    }
}