using System;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Collections;
//方法有关键字 命名空间重新起名
namespace EmailHelp_X
{
    public enum MailFormat { Text, Html };
    public enum MailPriority { Low = 1, Normal = 3, High = 5 };

    /// <summary>
    /// 添加附件
    /// </summary>
    public class MailAttachments
    {
        #region 构造函数
        public MailAttachments()
        {
            _attachments = new ArrayList();
        }
        #endregion

        #region 私有字段
        private IList _attachments;
        private const int MaxAttachmentNum = 10;
        #endregion

        #region 索引器
        public string this[int index]
        {
            get { return (string)_attachments[index]; }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 添加邮件附件
        /// </summary>
        /// <param name="FilePath">附件的绝对路径</param>
        public void Add(params string[] filePath)
        {
            if (filePath == null)
            {
                throw (new ArgumentNullException("非法的附件"));
            }
            else
            {
                for (var i = 0; i < filePath.Length; i++)
                {
                    Add(filePath[i]);
                }
            }
        }

        /// <summary>
        /// 添加一个附件,当指定的附件不存在时，忽略该附件，不产生异常。
        /// </summary>
        /// <param name="filePath">附件的绝对路径</param>
        public void Add(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                if (_attachments.Count < MaxAttachmentNum)
                {
                    _attachments.Add(filePath);
                }
            }
        }

        /// <summary>
        /// 清除所有附件
        /// </summary>
        public void Clear()
        {
            _attachments.Clear();
        }

        /// <summary>
        /// 获取附件个数
        /// </summary>
        public int Count
        {
            get { return _attachments.Count; }
        }
        #endregion
    }

    /// <summary>
    /// 邮件信息
    /// </summary>
    public class MailMessage
    {
        #region 构造函数
        public MailMessage()
        {
            _recipients = new ArrayList();        //收件人列表
            _attachments = new MailAttachments(); //附件
            _bodyFormat = MailFormat.Html;        //缺省的邮件格式为HTML
            _priority = MailPriority.Normal;
            _charset = "GB2312";
        }
        #endregion

        #region 私有字段
        private int _maxRecipientNum = 30;
        private string _from;      //发件人地址
        private string _fromName;  //发件人姓名
        private IList _recipients; //收件人
        private MailAttachments _attachments;//附件
        private string _body;      //内容
        private string _subject;   //主题
        private MailFormat _bodyFormat;     //邮件格式
        private string _charset = "GB2312"; //字符编码格式
        private MailPriority _priority;     //邮件优先级
        #endregion

        #region 公有属性
        /// <summary>
        /// 设定语言代码，默认设定为GB2312，如不需要可设置为""
        /// </summary>
        public string Charset
        {
            get { return _charset; }
            set { _charset = value; }
        }

        /// <summary>
        /// 最大收件人
        /// </summary>
        public int MaxRecipientNum
        {
            get { return _maxRecipientNum; }
            set { _maxRecipientNum = value; }
        }

        /// <summary>
        /// 发件人地址
        /// </summary>
        public string From
        {
            get { return _from; }
            set { _from = value; }
        }

        /// <summary>
        /// 发件人姓名
        /// </summary>
        public string FromName
        {
            get { return _fromName; }
            set { _fromName = value; }
        }

        /// <summary>
        /// 内容
        /// </summary>
        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }

        /// <summary>
        /// 主题
        /// </summary>
        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        /// <summary>
        /// 附件
        /// </summary>
        public MailAttachments Attachments
        {
            get { return _attachments; }
            set { _attachments = value; }
        }

        /// <summary>
        /// 优先权
        /// </summary>
        public MailPriority Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        /// <summary>
        /// 收件人
        /// </summary>
        public IList Recipients
        {
            get { return _recipients; }
        }

        /// <summary>
        /// 邮件格式
        /// </summary>
        public MailFormat BodyFormat
        {
            set { _bodyFormat = value; }
            get { return _bodyFormat; }
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 增加一个收件人地址
        /// </summary>
        /// <param name="recipient">收件人的Email地址</param>
        public void AddRecipients(string recipient)
        {
            if (_recipients.Count < MaxRecipientNum)
            {
                _recipients.Add(recipient);
            }
        }

        /// <summary>
        /// 增加多个收件人地址
        /// </summary>
        /// <param name="recipient">收件人的Email地址集合</param>
        public void AddRecipients(params string[] recipient)
        {
            if (recipient == null)
            {
                throw (new ArgumentException("收件人不能为空."));
            }
            else
            {
                for (var i = 0; i < recipient.Length; i++)
                {
                    AddRecipients(recipient[i]);
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 邮件操作
    /// </summary>
    public class SmtpServerHelper
    {
        #region 构造函数、析构函数
        public SmtpServerHelper()
        {
            SmtpCodeAdd();
        }

        ~SmtpServerHelper()
        {
            _networkStream.Close();
            _tcpClient.Close();
        }
        #endregion

        #region 私有字段
        /// <summary>
        /// 回车换行
        /// </summary>
        private string _crlf = "\r\n";

        /// <summary>
        /// 错误消息反馈
        /// </summary>
        private string _errmsg;

        /// <summary>
        /// TcpClient对象，用于连接服务器
        /// </summary> 
        private TcpClient _tcpClient;

        /// <summary>
        /// NetworkStream对象
        /// </summary> 
        private NetworkStream _networkStream;

        /// <summary>
        /// 服务器交互记录
        /// </summary>
        private string _logs = "";

        /// <summary>
        /// SMTP错误代码哈希表
        /// </summary>
        private Hashtable _errCodeHt = new Hashtable();

        /// <summary>
        /// SMTP正确代码哈希表
        /// </summary>
        private Hashtable _rightCodeHt = new Hashtable();
        #endregion

        #region 公有属性
        /// <summary>
        /// 错误消息反馈
        /// </summary>
        public string ErrMsg
        {
            set { _errmsg = value; }
            get { return _errmsg; }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 将字符串编码为Base64字符串
        /// </summary>
        /// <param name="str">要编码的字符串</param>
        private string Base64Encode(string str)
        {
            byte[] barray;
            barray = Encoding.Default.GetBytes(str);
            return Convert.ToBase64String(barray);
        }

        /// <summary>
        /// 将Base64字符串解码为普通字符串
        /// </summary>
        /// <param name="str">要解码的字符串</param>
        private string Base64Decode(string str)
        {
            byte[] barray;
            barray = Convert.FromBase64String(str);
            return Encoding.Default.GetString(barray);
        }

        /// <summary>
        /// 得到上传附件的文件流
        /// </summary>
        /// <param name="filePath">附件的绝对路径</param>
        private string GetStream(string filePath)
        {
            var fileStr = new System.IO.FileStream(filePath, System.IO.FileMode.Open);
            var by = new byte[System.Convert.ToInt32(fileStr.Length)];
            fileStr.Read(by, 0, by.Length);
            fileStr.Close();
            return (System.Convert.ToBase64String(by));
        }

        /// <summary>
        /// SMTP回应代码哈希表
        /// </summary>
        private void SmtpCodeAdd()
        {
            _errCodeHt.Add("421", "服务未就绪，关闭传输信道");
            _errCodeHt.Add("432", "需要一个密码转换");
            _errCodeHt.Add("450", "要求的邮件操作未完成，邮箱不可用（例如，邮箱忙）");
            _errCodeHt.Add("451", "放弃要求的操作；处理过程中出错");
            _errCodeHt.Add("452", "系统存储不足，要求的操作未执行");
            _errCodeHt.Add("454", "临时认证失败");
            _errCodeHt.Add("500", "邮箱地址错误");
            _errCodeHt.Add("501", "参数格式错误");
            _errCodeHt.Add("502", "命令不可实现");
            _errCodeHt.Add("503", "服务器需要SMTP验证");
            _errCodeHt.Add("504", "命令参数不可实现");
            _errCodeHt.Add("530", "需要认证");
            _errCodeHt.Add("534", "认证机制过于简单");
            _errCodeHt.Add("538", "当前请求的认证机制需要加密");
            _errCodeHt.Add("550", "要求的邮件操作未完成，邮箱不可用（例如，邮箱未找到，或不可访问）");
            _errCodeHt.Add("551", "用户非本地，请尝试<forward-path>");
            _errCodeHt.Add("552", "过量的存储分配，要求的操作未执行");
            _errCodeHt.Add("553", "邮箱名不可用，要求的操作未执行（例如邮箱格式错误）");
            _errCodeHt.Add("554", "传输失败");

            _rightCodeHt.Add("220", "服务就绪");
            _rightCodeHt.Add("221", "服务关闭传输信道");
            _rightCodeHt.Add("235", "验证成功");
            _rightCodeHt.Add("250", "要求的邮件操作完成");
            _rightCodeHt.Add("251", "非本地用户，将转发向<forward-path>");
            _rightCodeHt.Add("334", "服务器响应验证Base64字符串");
            _rightCodeHt.Add("354", "开始邮件输入，以<CRLF>.<CRLF>结束");
        }

        /// <summary>
        /// 发送SMTP命令
        /// </summary> 
        private bool SendCommand(string str)
        {
            byte[] writeBuffer;
            if (str == null || str.Trim() == String.Empty)
            {
                return true;
            }
            _logs += str;
            writeBuffer = Encoding.Default.GetBytes(str);
            try
            {
                _networkStream.Write(writeBuffer, 0, writeBuffer.Length);
            }
            catch
            {
                _errmsg = "网络连接错误";
                return false;
            }
            return true;
        }

        /// <summary>
        /// 接收SMTP服务器回应
        /// </summary>
        private string RecvResponse()
        {
            int streamSize;
            var returnvalue = String.Empty;
            var readBuffer = new byte[1024];
            try
            {
                streamSize = _networkStream.Read(readBuffer, 0, readBuffer.Length);
            }
            catch
            {
                _errmsg = "网络连接错误";
                return "false";
            }

            if (streamSize == 0)
            {
                return returnvalue;
            }
            else
            {
                returnvalue = Encoding.Default.GetString(readBuffer).Substring(0, streamSize);
                _logs += returnvalue + this._crlf;
                return returnvalue;
            }
        }

        /// <summary>
        /// 与服务器交互，发送一条命令并接收回应。
        /// </summary>
        /// <param name="str">一个要发送的命令</param>
        /// <param name="errstr">如果错误，要反馈的信息</param>
        private bool Dialog(string str, string errstr)
        {
            if (str == null || str.Trim() == string.Empty)
            {
                return true;
            }
            if (SendCommand(str))
            {
                var rr = RecvResponse();
                if (rr == "false")
                {
                    return false;
                }

                //检查返回的代码，根据[RFC 821]返回代码为3位数字代码如220
                var rrCode = rr.Substring(0, 3);
                if (_rightCodeHt[rrCode] != null)
                {
                    return true;
                }
                else
                {
                    if (_errCodeHt[rrCode] != null)
                    {
                        _errmsg += (rrCode + _errCodeHt[rrCode].ToString());
                        _errmsg += _crlf;
                    }
                    else
                    {
                        _errmsg += rr;
                    }
                    _errmsg += errstr;
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 与服务器交互，发送一组命令并接收回应。
        /// </summary>
        private bool Dialog(string[] str, string errstr)
        {
            for (var i = 0; i < str.Length; i++)
            {
                if (!Dialog(str[i], ""))
                {
                    _errmsg += _crlf;
                    _errmsg += errstr;
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        private bool Connect(string smtpServer, int port)
        {
            try
            {
                _tcpClient = new TcpClient(smtpServer, port);
            }
            catch (Exception e)
            {
                _errmsg = e.ToString();
                return false;
            }
            _networkStream = _tcpClient.GetStream();

            if (_rightCodeHt[RecvResponse().Substring(0, 3)] == null)
            {
                _errmsg = "网络连接失败";
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取优先级
        /// </summary>
        /// <param name="mailPriority">优先级</param>
        private string GetPriorityString(MailPriority mailPriority)
        {
            var priority = "Normal";
            if (mailPriority == MailPriority.Low)
            {
                priority = "Low";
            }
            else if (mailPriority == MailPriority.High)
            {
                priority = "High";
            }
            return priority;
        }

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="smtpServer">发信SMTP服务器</param>
        /// <param name="port">端口，默认为25</param>
        /// <param name="username">发信人邮箱地址</param>
        /// <param name="password">发信人邮箱密码</param>
        /// <param name="mailMessage">邮件内容</param>
        private bool SendEmail(string smtpServer, int port, bool eSmtp, string username, string password, MailMessage mailMessage)
        {
            if (Connect(smtpServer, port) == false) return false;

            var priority = GetPriorityString(mailMessage.Priority);

            var html = (mailMessage.BodyFormat == MailFormat.Html);

            string[] sendBuffer;
            string sendBufferstr;

            //进行SMTP验证
            if (eSmtp)
            {
                sendBuffer = new String[4];
                sendBuffer[0] = "EHLO " + smtpServer + _crlf;
                sendBuffer[1] = "AUTH LOGIN" + _crlf;
                sendBuffer[2] = Base64Encode(username) + _crlf;
                sendBuffer[3] = Base64Encode(password) + _crlf;
                if (!Dialog(sendBuffer, "SMTP服务器验证失败，请核对用户名和密码。")) return false;
            }
            else
            {
                sendBufferstr = "HELO " + smtpServer + _crlf;
                if (!Dialog(sendBufferstr, "")) return false;
            }

            //发件人地址
            sendBufferstr = "MAIL FROM:<" + username + ">" + _crlf;
            if (!Dialog(sendBufferstr, "发件人地址错误，或不能为空")) return false;

            //收件人地址
            sendBuffer = new string[mailMessage.Recipients.Count];
            for (var i = 0; i < mailMessage.Recipients.Count; i++)
            {
                sendBuffer[i] = "RCPT TO:<" + (string)mailMessage.Recipients[i] + ">" + _crlf;
            }
            if (!Dialog(sendBuffer, "收件人地址有误")) return false;

            sendBufferstr = "DATA" + _crlf;
            if (!Dialog(sendBufferstr, "")) return false;

            //发件人姓名
            sendBufferstr = "From:" + mailMessage.FromName + "<" + mailMessage.From + ">" + _crlf;

            if (mailMessage.Recipients.Count == 0)
            {
                return false;
            }
            else
            {
                sendBufferstr += "To:=?" + mailMessage.Charset.ToUpper() + "?B?" + Base64Encode((string)mailMessage.Recipients[0]) + "?=" + "<" + (string)mailMessage.Recipients[0] + ">" + _crlf;
            }
            sendBufferstr += ((mailMessage.Subject == String.Empty || mailMessage.Subject == null) ? "Subject:" : ((mailMessage.Charset == "") ? ("Subject:" + mailMessage.Subject) : ("Subject:" + "=?" + mailMessage.Charset.ToUpper() + "?B?" + Base64Encode(mailMessage.Subject) + "?="))) + _crlf;
            sendBufferstr += "X-Priority:" + priority + _crlf;
            sendBufferstr += "X-MSMail-Priority:" + priority + _crlf;
            sendBufferstr += "Importance:" + priority + _crlf;
            sendBufferstr += "X-Mailer: Lion.Web.Mail.SmtpMail Pubclass [cn]" + _crlf;
            sendBufferstr += "MIME-Version: 1.0" + _crlf;
            if (mailMessage.Attachments.Count != 0)
            {
                sendBufferstr += "Content-Type: multipart/mixed;" + _crlf;
                sendBufferstr += " boundary=\"=====" + (html ? "001_Dragon520636771063_" : "001_Dragon303406132050_") + "=====\"" + _crlf + _crlf;
            }
            if (html)
            {
                if (mailMessage.Attachments.Count == 0)
                {
                    sendBufferstr += "Content-Type: multipart/alternative;" + _crlf; //内容格式和分隔符
                    sendBufferstr += " boundary=\"=====003_Dragon520636771063_=====\"" + _crlf + _crlf;
                    sendBufferstr += "This is a multi-part message in MIME format." + _crlf + _crlf;
                }
                else
                {
                    sendBufferstr += "This is a multi-part message in MIME format." + _crlf + _crlf;
                    sendBufferstr += "--=====001_Dragon520636771063_=====" + _crlf;
                    sendBufferstr += "Content-Type: multipart/alternative;" + _crlf; //内容格式和分隔符
                    sendBufferstr += " boundary=\"=====003_Dragon520636771063_=====\"" + _crlf + _crlf;
                }
                sendBufferstr += "--=====003_Dragon520636771063_=====" + _crlf;
                sendBufferstr += "Content-Type: text/plain;" + _crlf;
                sendBufferstr += ((mailMessage.Charset == "") ? (" charset=\"iso-8859-1\"") : (" charset=\"" + mailMessage.Charset.ToLower() + "\"")) + _crlf;
                sendBufferstr += "Content-Transfer-Encoding: base64" + _crlf + _crlf;
                sendBufferstr += Base64Encode("邮件内容为HTML格式，请选择HTML方式查看") + _crlf + _crlf;

                sendBufferstr += "--=====003_Dragon520636771063_=====" + _crlf;

                sendBufferstr += "Content-Type: text/html;" + _crlf;
                sendBufferstr += ((mailMessage.Charset == "") ? (" charset=\"iso-8859-1\"") : (" charset=\"" + mailMessage.Charset.ToLower() + "\"")) + _crlf;
                sendBufferstr += "Content-Transfer-Encoding: base64" + _crlf + _crlf;
                sendBufferstr += Base64Encode(mailMessage.Body) + _crlf + _crlf;
                sendBufferstr += "--=====003_Dragon520636771063_=====--" + _crlf;
            }
            else
            {
                if (mailMessage.Attachments.Count != 0)
                {
                    sendBufferstr += "--=====001_Dragon303406132050_=====" + _crlf;
                }
                sendBufferstr += "Content-Type: text/plain;" + _crlf;
                sendBufferstr += ((mailMessage.Charset == "") ? (" charset=\"iso-8859-1\"") : (" charset=\"" + mailMessage.Charset.ToLower() + "\"")) + _crlf;
                sendBufferstr += "Content-Transfer-Encoding: base64" + _crlf + _crlf;
                sendBufferstr += Base64Encode(mailMessage.Body) + _crlf;
            }
            if (mailMessage.Attachments.Count != 0)
            {
                for (var i = 0; i < mailMessage.Attachments.Count; i++)
                {
                    var filepath = (string)mailMessage.Attachments[i];
                    sendBufferstr += "--=====" + (html ? "001_Dragon520636771063_" : "001_Dragon303406132050_") + "=====" + _crlf;
                    sendBufferstr += "Content-Type: text/plain;" + _crlf;
                    sendBufferstr += " name=\"=?" + mailMessage.Charset.ToUpper() + "?B?" + Base64Encode(filepath.Substring(filepath.LastIndexOf("\\") + 1)) + "?=\"" + _crlf;
                    sendBufferstr += "Content-Transfer-Encoding: base64" + _crlf;
                    sendBufferstr += "Content-Disposition: attachment;" + _crlf;
                    sendBufferstr += " filename=\"=?" + mailMessage.Charset.ToUpper() + "?B?" + Base64Encode(filepath.Substring(filepath.LastIndexOf("\\") + 1)) + "?=\"" + _crlf + _crlf;
                    sendBufferstr += GetStream(filepath) + _crlf + _crlf;
                }
                sendBufferstr += "--=====" + (html ? "001_Dragon520636771063_" : "001_Dragon303406132050_") + "=====--" + _crlf + _crlf;
            }
            sendBufferstr += _crlf + "." + _crlf;
            if (!Dialog(sendBufferstr, "错误信件信息")) return false;

            sendBufferstr = "QUIT" + _crlf;
            if (!Dialog(sendBufferstr, "断开连接时错误")) return false;

            _networkStream.Close();
            _tcpClient.Close();
            return true;
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 发送电子邮件,SMTP服务器不需要身份验证
        /// </summary>
        /// <param name="smtpServer">发信SMTP服务器</param>
        /// <param name="port">端口，默认为25</param>
        /// <param name="mailMessage">邮件内容</param>
        public bool SendEmail(string smtpServer, int port, MailMessage mailMessage)
        {
            return SendEmail(smtpServer, port, false, "", "", mailMessage);
        }

        /// <summary>
        /// 发送电子邮件,SMTP服务器需要身份验证
        /// </summary>
        /// <param name="smtpServer">发信SMTP服务器</param>
        /// <param name="port">端口，默认为25</param>
        /// <param name="username">发信人邮箱地址</param>
        /// <param name="password">发信人邮箱密码</param>
        /// <param name="mailMessage">邮件内容</param>
        public bool SendEmail(string smtpServer, int port, string username, string password, MailMessage mailMessage)
        {
            return SendEmail(smtpServer, port, true, username, password, mailMessage);
        }
        #endregion
    }

    /// <summary>
    /// 发送邮件
    /// </summary>
    //--------------------调用-----------------------
    //MailAttachments ma=new MailAttachments();
    //ma.Add(@"附件地址");
    //MailMessage mail = new MailMessage();
    //mail.Attachments=ma;
    //mail.Body="你好";
    //mail.AddRecipients("zjy99684268@163.com");
    //mail.From="zjy99684268@163.com";
    //mail.FromName="zjy";
    //mail.Subject="Hello";
    //SmtpClient sp = new SmtpClient();
    //sp.SmtpServer = "smtp.163.com";
    //sp.Send(mail, "zjy99684268@163.com", "123456");
    //------------------------------------------------
    public class SmtpClient
    {
        #region 构造函数
        public SmtpClient()
        { }

        public SmtpClient(string smtpServer)
        {
            _smtpServer = smtpServer;
        }
        #endregion

        #region 私有字段
        private string _errmsg;
        private string _smtpServer;
        #endregion

        #region 公有属性
        /// <summary>
        /// 错误消息反馈
        /// </summary>
        public string ErrMsg
        {
            get { return _errmsg; }
        }

        /// <summary>
        /// 邮件服务器
        /// </summary>
        public string SmtpServer
        {
            set { _smtpServer = value; }
            get { return _smtpServer; }
        }
        #endregion

        public bool Send(MailMessage mailMessage, string username, string password)
        {
            var helper = new SmtpServerHelper();
            if (helper.SendEmail(_smtpServer, 25, username, password, mailMessage))
                return true;
            else
            {
                _errmsg = helper.ErrMsg;
                return false;
            }
        }
    }

    /// <summary>
    /// 操作服务器上邮件
    /// </summary>
    public class SmtpMail
    {
        public SmtpMail()
        { }

        #region 字段
        private StreamReader _sr;
        private StreamWriter _sw;
        private TcpClient _tcpClient;
        private NetworkStream _networkStream;
        #endregion

        #region 私有方法
        /// <summary>
        /// 向服务器发送信息
        /// </summary>
        private bool SendDataToServer(string str)
        {
            try
            {
                _sw.WriteLine(str);
                _sw.Flush();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 从网络流中读取服务器回送的信息
        /// </summary>
        private string ReadDataFromServer()
        {
            string str = null;
            try
            {
                str = _sr.ReadLine();
                if (str[0] == '-')
                {
                    str = null;
                }
            }
            catch (Exception err)
            {
                str = err.Message;
            }
            return str;
        }
        #endregion

        #region 获取邮件信息
        /// <summary>
        /// 获取邮件信息
        /// </summary>
        /// <param name="uid">邮箱账号</param>
        /// <param name="pwd">邮箱密码</param>
        /// <returns>邮件信息</returns>
        public ArrayList ReceiveMail(string uid, string pwd)
        {
            var emailMes = new ArrayList();
            string str;
            var index = uid.IndexOf('@');
            var pop3Server = "pop3." + uid.Substring(index + 1);
            _tcpClient = new TcpClient(pop3Server, 110);
            _networkStream = _tcpClient.GetStream();
            _sr = new StreamReader(_networkStream);
            _sw = new StreamWriter(_networkStream);

            if (ReadDataFromServer() == null) return emailMes;
            if (SendDataToServer("USER " + uid) == false) return emailMes;
            if (ReadDataFromServer() == null) return emailMes;
            if (SendDataToServer("PASS " + pwd) == false) return emailMes;
            if (ReadDataFromServer() == null) return emailMes;
            if (SendDataToServer("LIST") == false) return emailMes;
            if ((str = ReadDataFromServer()) == null) return emailMes;

            var splitString = str.Split(' ');
            var count = int.Parse(splitString[1]);
            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    if ((str = ReadDataFromServer()) == null) return emailMes;
                    splitString = str.Split(' ');
                    emailMes.Add(string.Format("第{0}封邮件，{1}字节", splitString[0], splitString[1]));
                }
                return emailMes;
            }
            else
            {
                return emailMes;
            }
        }
        #endregion

        #region 读取邮件内容
        /// <summary>
        /// 读取邮件内容
        /// </summary>
        /// <param name="mailMessage">第几封</param>
        /// <returns>内容</returns>
        public string ReadEmail(string str)
        {
            var state = "";
            if (SendDataToServer("RETR " + str) == false)
                state = "Error";
            else
            {
                state = _sr.ReadToEnd();
            }
            return state;
        }
        #endregion

        #region 删除邮件
        /// <summary>
        /// 删除邮件
        /// </summary>
        /// <param name="str">第几封</param>
        /// <returns>操作信息</returns>
        public string DeleteEmail(string str)
        {
            var state = "";
            if (SendDataToServer("DELE " + str) == true)
            {
                state = "成功删除";
            }
            else
            {
                state = "Error";
            }
            return state;
        }
        #endregion

        #region 关闭服务器连接
        /// <summary>
        /// 关闭服务器连接
        /// </summary>
        public void CloseConnection()
        {
            SendDataToServer("QUIT");
            _sr.Close();
            _sw.Close();
            _networkStream.Close();
            _tcpClient.Close();
        }
        #endregion
    }

}

