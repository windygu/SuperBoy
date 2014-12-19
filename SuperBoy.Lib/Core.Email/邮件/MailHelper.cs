using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace EmailHelp
{
    /// <summary>
    /// 邮件操作类
    /// </summary>
    public class MailHelper
    {
        /// <summary>
        /// 获取Email登陆地址
        /// </summary>
        /// <param name="email">email地址</param>
        /// <returns></returns>
        public static string GetEMailLoginUrl(string email)
        {
            if ((email == string.Empty) || (email.IndexOf("@") <= 0))
            {
                return string.Empty;
            }
            var index = email.IndexOf("@");
            email = "http://mail." + email.Substring(index + 1);
            return email;
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailSubjct">邮件主题</param>
        /// <param name="mailBody">邮件正文</param>
        /// <param name="mailFrom">发送者</param>
        /// <param name="mailAddress">邮件地址列表</param>
        /// <param name="hostIp">主机IP</param>
        /// <returns></returns>
        public static string sendMail(string mailSubjct, string mailBody, string mailFrom, List<string> mailAddress, string hostIp)
        {
            var str = "";
            try
            {
                var message = new MailMessage
                {
                    IsBodyHtml = false,
                    Subject = mailSubjct,
                    Body = mailBody,
                    From = new MailAddress(mailFrom)
                };
                for (var i = 0; i < mailAddress.Count; i++)
                {
                    message.To.Add(mailAddress[i]);
                }
                new SmtpClient { UseDefaultCredentials = false, DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis, Host = hostIp, Port = (char)0x19 }.Send(message);
            }
            catch (Exception exception)
            {
                str = exception.Message;
            }
            return str;
        }
        /// <summary>
        /// 发送邮件（要求登陆）
        /// </summary>
        /// <param name="mailSubjct">邮件主题</param>
        /// <param name="mailBody">邮件正文</param>
        /// <param name="mailFrom">发送者</param>
        /// <param name="mailAddress">接收地址列表</param>
        /// <param name="hostIp">主机IP</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public static bool sendMail(string mailSubjct, string mailBody, string mailFrom, List<string> mailAddress, string hostIp, string username, string password)
        {
            bool flag;
            var str = sendMail(mailSubjct, mailBody, mailFrom, mailAddress, hostIp, 0x19, username, password, false, string.Empty, out flag);
            return flag;
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailSubjct">邮件主题</param>
        /// <param name="mailBody">邮件正文</param>
        /// <param name="mailFrom">发送者</param>
        /// <param name="mailAddress">接收地址列表</param>
        /// <param name="hostIp">主机IP</param>
        /// <param name="filename">附件名</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="ssl">加密类型</param>
        /// <returns></returns>
        public static string sendMail(string mailSubjct, string mailBody, string mailFrom, List<string> mailAddress, string hostIp, string filename, string username, string password, bool ssl)
        {
            var str = "";
            try
            {
                var message = new MailMessage
                {
                    IsBodyHtml = false,
                    Subject = mailSubjct,
                    Body = mailBody,

                    From = new MailAddress(mailFrom)
                };
                for (var i = 0; i < mailAddress.Count; i++)
                {
                    message.To.Add(mailAddress[i]);
                }
                if (System.IO.File.Exists(filename))
                {
                    message.Attachments.Add(new Attachment(filename));
                }
                var client = new SmtpClient
                {
                    EnableSsl = ssl,
                    UseDefaultCredentials = false
                };
                var credential = new NetworkCredential(username, password);
                client.Credentials = credential;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Host = hostIp;
                client.Port = 0x19;
                client.Send(message);
            }
            catch (Exception exception)
            {
                str = exception.Message;
            }
            return str;
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailSubjct"></param>
        /// <param name="mailBody"></param>
        /// <param name="mailFrom"></param>
        /// <param name="mailAddress"></param>
        /// <param name="hostIp"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="ssl"></param>
        /// <param name="replyTo"></param>
        /// <param name="sendOk"></param>
        /// <returns></returns>
        public static string sendMail(string mailSubjct, string mailBody, string mailFrom, List<string> mailAddress, string hostIp, int port, string username, string password, bool ssl, string replyTo, out bool sendOk)
        {
            sendOk = true;
            var str = "";
            try
            {
                var message = new MailMessage
                {
                    IsBodyHtml = false,
                    Subject = mailSubjct,
                    Body = mailBody,
                    From = new MailAddress(mailFrom)
                };
                if (replyTo != string.Empty)
                {
                    var address = new MailAddress(replyTo);
#pragma warning disable 618
                    message.ReplyTo = address;
#pragma warning restore 618
                }
                var regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
                for (var i = 0; i < mailAddress.Count; i++)
                {
                    if (regex.IsMatch(mailAddress[i]))
                    {
                        message.To.Add(mailAddress[i]);
                    }
                }
                if (message.To.Count == 0)
                {
                    return string.Empty;
                }
                var client = new SmtpClient
                {
                    EnableSsl = ssl,
                    UseDefaultCredentials = false
                };
                var credential = new NetworkCredential(username, password);
                client.Credentials = credential;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Host = hostIp;
                client.Port = port;
                client.Send(message);
            }
            catch (Exception exception)
            {
                str = exception.Message;
                sendOk = false;
            }
            return str;
        }
    }
}
