/**********************************************
 * �����ã�   �û�ʵ����
 * �����ˣ�   abaal
 * ����ʱ�䣺 2008-09-03 
 * Copyright (C) 2007-2008 abaal
 * All rights reserved
 * http://blog.csdn.net/abaal888
 ***********************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Svnhost.Common
{
    /// <summary>
    /// �û�ʵ���࣬�Զ��崰�������֤ʱ����ʹ�á�
    /// </summary>
    public sealed class UserUtil
    {
        /// <summary>
        /// �û���¼����
        /// </summary>
        /// <param name="username">�û���</param>
        /// <param name="roles">�û���ɫ</param>
        /// <param name="isPersistent">�Ƿ�־�cookie</param>
        public static void Login(string username, string roles, bool isPersistent)
        {
            var dt = isPersistent ? DateTime.Now.AddMinutes(99999) : DateTime.Now.AddMinutes(60);
            var ticket =
                new FormsAuthenticationTicket(
                1, // Ʊ�ݰ汾��
                username, // Ʊ�ݳ�����
                DateTime.Now, //����Ʊ�ݵ�ʱ��
                dt, // ʧЧʱ��
                isPersistent, // ��Ҫ�û��� cookie 
                roles, // �û����ݣ�������ʵ�����û��Ľ�ɫ
              FormsAuthentication.FormsCookiePath);//cookie��Ч·��

            //ʹ�û�����machine key����cookie��Ϊ�˰�ȫ����
            var hash = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hash); //����֮���cookie

            //��cookie��ʧЧʱ������Ϊ��Ʊ��tikets��ʧЧʱ��һ�� 
            var uCookie = new HttpCookie("username", username);
            if (ticket.IsPersistent)
            {
                uCookie.Expires = ticket.Expiration;
                cookie.Expires = ticket.Expiration;
            }

            //���cookie��ҳ��������Ӧ��
            HttpContext.Current.Response.Cookies.Add(cookie);
            HttpContext.Current.Response.Cookies.Add(uCookie);
        }

        /// <summary>
        /// �û��˳�����
        /// </summary>
        public static void Logout()
        {
            var cookie = HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName];

            if (cookie == null)
            {
                cookie = new HttpCookie(FormsAuthentication.FormsCookieName);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            cookie.Expires = DateTime.Now.AddYears(-10);

            var uCookie = new HttpCookie("username", string.Empty);
            uCookie.Expires = DateTime.Now.AddYears(-10);
            HttpContext.Current.Response.Cookies.Add(uCookie);
        }
    }
}
