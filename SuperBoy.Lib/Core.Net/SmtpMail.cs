using System;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text;
using System.Web.Configuration;
using System.Collections.Generic;

namespace Core.Net
{
    /// <summary>
    /// Smtp配置
    /// </summary>
    public class SmtpConfig
    {
        /// <summary>
        /// SMTP在web.config的默认配置
        /// </summary>
        public SmtpConfig()
        {
            SetWebConfigBindding();
            ContentEncoding = Encoding.Default;
        }

        private void SetWebConfigBindding()
        {
            try
            {
                var sectionGroup = (MailSettingsSectionGroup)WebConfigurationManager.OpenWebConfiguration("~/").GetSectionGroup("system.net/mailSettings");
                if (sectionGroup == null)
                {
                    SmtpServer = "localhost";
                    Port = 25;
                    SslConnect = false;
                }
                else
                {

                    if (sectionGroup.Smtp.Network.Host != "")
                    {
                        SmtpServer = sectionGroup.Smtp.Network.Host;
                    }
                    Port = sectionGroup.Smtp.Network.Port;
                    UserName = sectionGroup.Smtp.Network.UserName;
                    Password = sectionGroup.Smtp.Network.Password;

                    if (sectionGroup.Smtp.Network.DefaultCredentials == true)
                    {
                        Credentials = null;
                    }
                    else
                    {
                        Credentials = new NetworkCredential(UserName, Password);
                    }
                }
            }
            catch (Exception)
            {

            }

        }

        private string _smtpServerField;
        /// <summary>
        /// 发送邮件服务器
        /// </summary>
        public string SmtpServer 
        {
            get { return _smtpServerField; }
            set { _smtpServerField = value; }
        }
        
        private int _portField = 25;
        /// <summary>
        /// 服务器连接端口，默认为25。
        /// </summary>
        public int Port 
        {
        	get { return _portField; }
        	set { _portField = value; }
        }

        private string _userNameField;
        /// <summary>
        /// 连接用户名
        /// </summary>
        public string UserName 
        { 
        	get { return _userNameField; }
            set { _userNameField = value; }
        }

        private string _passwordField;
        /// <summary>
        /// 连接密码
        /// </summary>
        public string Password 
        { 
        	get { return _passwordField; }
            set { _passwordField = value; } 
        }
        
        private bool _sslConnectField = false;
        /// <summary>
        /// 是否是安全套接字连接，默认为否。
        /// </summary>
        public bool SslConnect 
        { 
        	get { return _sslConnectField; }
            set { _sslConnectField = value; } 
        }
        
        private Encoding _contentEncodingField;
        /// <summary>
        /// 邮件内容编码
        /// </summary>
        public Encoding ContentEncoding 
        { 
        	get { return _contentEncodingField; }
            set { _contentEncodingField = value; } 
        }
        
        private NetworkCredential _credentialsField;
        /// <summary>
        /// 访问凭据
        /// </summary>
        public NetworkCredential Credentials 
        { 
        	get { return _credentialsField; }
            set { _credentialsField = value; } 
        }
    }
    
    /// <summary>
    /// SMTP邮件发送
    /// </summary>
    public class SmtpMail
    {
        /// <summary>
        /// 发送HTML格式邮件(UTF8)
        /// </summary>
        public static string MailTo(SmtpConfig config,
            MailAddress addrFrom, MailAddress addrTo,
            MailAddressCollection cc, MailAddressCollection bCc,
            string subject, string bodyContent, bool isHtml, List<Attachment> attC)
        {
            var msg = new MailMessage(addrFrom, addrTo);

            #region 抄送
            if (cc != null && cc.Count > 0)
            {
                foreach (var cAddr in cc)
                {
                    msg.CC.Add(cAddr);
                }
            }
            #endregion

            #region 密送
            if (bCc != null && bCc.Count > 0)
            {
                foreach (var cAddr in bCc)
                {
                    msg.Bcc.Add(cAddr);
                }
            }
            #endregion

            #region 附件列表
            if (attC != null && attC.Count > 0)
            {
                foreach (var item in attC)
                {
                    msg.Attachments.Add(item);
                }
            }
            #endregion

            msg.IsBodyHtml = isHtml;
            msg.Priority = MailPriority.High;

            msg.Subject = subject;
            msg.SubjectEncoding = config.ContentEncoding;
            msg.BodyEncoding = config.ContentEncoding;
            msg.Body = bodyContent;

            var client = new SmtpClient(config.SmtpServer, config.Port);
            if (config.Credentials != null)
            {
                client.Credentials = config.Credentials;
            }
            client.EnableSsl = config.SslConnect;

            try
            {
                client.Send(msg);
                return "0";
            }
            catch (Exception exp)
            {
                return exp.Message;
            }

        }


        /// <summary>
        /// 发送HTML格式邮件
        /// </summary>
        /// <param name="config">SMTP配置</param>
        /// <param name="addrFrom">发件人邮箱</param>
        /// <param name="addrTo">收件人邮箱</param>
        /// <param name="subject">主题</param>
        /// <param name="bodyContent">内容</param>
        /// <returns></returns>
        public static string MailTo(SmtpConfig config,
            MailAddress addrFrom, MailAddress addrTo,
            string subject, string bodyContent)
        {
            return MailTo(config, addrFrom, addrTo, null, null, subject, bodyContent, true, null);
        }

    }
}
