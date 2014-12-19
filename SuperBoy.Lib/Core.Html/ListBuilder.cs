/**********************************************
 * �����ã�   ���Ӹ�����
 * �����ˣ�   abaal
 * ����ʱ�䣺 2008-09-03 
 * Copyright (C) 2007-2008 abaal
 * All rights reserved
 * http://blog.csdn.net/abaal888
 ***********************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;

namespace Svnhost.Common
{
    /*  ʹ�÷�ʽ         
    ListBuilder lb1=new ListBuilder("ul");
    lb1.SetTemplate("<a href=\"{#url}\" target=\"_blank\"><strong>{#sortName}</strong></a>\r\n{#lb2}");

    ListBuilder lb2=new ListBuilder("ul");
    lb2.SetTemplate("<a href=\""+url+"?tid={#id}"+"\" target=\"_blank\">{#title}</a> <span class='date'>[{#date}]</span>");
    lb2.BindData(dt2);
    ����
    lb1.AddTemplateData("sortName",dr["sortName"].ToString());
    lb1.AddTemplateData("url",url+"#sid."+dr["id"].ToString()+"/page.1/");
    lb1.AddTemplateData("lb2",lb2.ToString());
    lb1.EndTemplateData();
    lSitemap.Text=lb1.ToString();
 */
    public class ListBuilder
    {
        protected string Type = "dl";
        protected StringBuilder Sb = new StringBuilder();
        protected string Template = string.Empty;
        protected System.Collections.Specialized.NameValueCollection Nvc = new System.Collections.Specialized.NameValueCollection();

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="_type">�б����ͣ���ѡ"ul","dl"��Ĭ��"ul"</param>
        /// <param name="className"></param>
        public ListBuilder(string _type, string className)
        {
            if (_type != string.Empty) Type = _type;
            if (className != string.Empty) className = " class='" + className + "'";
            Sb.Append("<" + Type + className + ">");
        }
        /// <summary>
        /// ��ul�����б�
        /// </summary>
        public ListBuilder()
        {
            Sb.Append("<dl>");
        }
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="_type">�б����ͣ���ѡ"ul","dl"��Ĭ��"ul"</param>
        public ListBuilder(string _type)
        {
            Type = _type;
            Sb.Append("<" + Type + ">");
        }

        /// <summary>
        /// ����ģ��
        /// </summary>
        /// <param name="_template">ģ�棬��{#name}��Ϊ������name��������Դdatatable����ִ������</param>
        public void SetTemplate(string _template)
        {
            Template = _template;
        }

        /// <summary>
        /// ���ģ������
        /// </summary>
        /// <param name="name">����</param>
        /// <param name="value">ֵ</param>
        public void AddTemplateData(string name, string value)
        {
            Nvc.Add(name, value);
        }

        /// <summary>
        /// ģ�����������ɺ�����ô˷���
        /// </summary>
        /// <param name="className">�б��li����dd����css����</param>
        public void EndTemplateData(string className)
        {
            var itemName = "dd";
            if (Type != "dl")
            {
                itemName = "li";
            }
            if (className != string.Empty) className = " class='" + className + "'";
            Sb.Append("<" + itemName + className + ">");
            Sb.Append(GenerateString());
            Sb.Append("</" + itemName + ">");
            Nvc.Clear();
        }

        /// <summary>
        /// ģ�����������ɺ�����ô˷���
        /// </summary>
        public void EndTemplateData()
        {
            EndTemplateData("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        protected string GenerateString()
        {
            var regex = new Regex(@"{#(\w+)}", RegexOptions.IgnoreCase);
            return regex.Replace(Template, new MatchEvaluator(this.MatchEvaluator));
        }

        /// <summary>
        /// ����ƥ���е�ί�ж���
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        protected string MatchEvaluator(Match m)
        {
            var name = m.Groups[1].Captures[0].Value;
            return Nvc[name];
        }

        /// <summary>
        /// ���ñ���
        /// </summary>
        /// <param name="text">�ı�</param>
        /// <param name="url">����</param>
        /// <param name="target">Ŀ�괰�ڣ���ѡ"_blank","_self"�ȣ�Ĭ��"_self"</param>
        public void SetTitle(string text, string url, string target)
        {
            this.SetTitle(text, url, target, "");
        }
        /// <summary>
        /// ���ñ���
        /// </summary>
        /// <param name="text">�ı�</param>
        /// <param name="url">����</param>
        public void SetTitle(string text, string url)
        {
            this.SetTitle(text, url, "", "");
        }
        /// <summary>
        /// ���ô��ı�����
        /// </summary>
        /// <param name="text">�ı�</param>
        public void SetTitle(string text)
        {
            this.SetTitle(text, "", "", "");
        }
        /// <summary>
        /// ���ñ���
        /// </summary>
        /// <param name="text">�ı�</param>
        /// <param name="url">����</param>
        /// <param name="target">Ŀ�괰�ڣ���ѡ"_blank","_self"�ȣ�Ĭ��"_self"</param>
        /// <param name="className">CSS����</param>
        public void SetTitle(string text, string url, string target, string className)
        {


            var itemName = "dt";
            if (Type != "dl")
            {
                itemName = "li";
                className += " title ";
            }
            if (className != string.Empty) className = " class='" + className + "'";
            Sb.Append("<" + itemName + className + ">");
            if (url == string.Empty)
            {
                Sb.Append(text);
            }
            else
            {
                if (target != string.Empty) target = " target='" + target + "'";
                Sb.Append("<a href='" + url + "'" + target + ">");
                Sb.Append(text);
                Sb.Append("</a>");
            }
            Sb.Append("</" + itemName + ">");
        }
        /// <summary>
        /// ���һ��
        /// </summary>
        /// <param name="text">�ı�</param>
        /// <param name="url">����</param>
        /// <param name="target">Ŀ�괰�ڣ���ѡ"_blank","_self"�ȣ�Ĭ��"_self"</param>
        /// <param name="className">CSS����</param>
        public void AddItem(string text, string url, string target, string className)
        {
            var itemName = "dd";
            if (Type != "dl") itemName = "li";
            if (className != string.Empty) className = " class='" + className + "'";
            Sb.Append("<" + itemName + className + ">");
            if (url == string.Empty)
            {
                Sb.Append(text);
            }
            else
            {
                if (target != string.Empty) target = " target='" + target + "'";
                Sb.Append("<a href='" + url + "'" + target + ">");
                Sb.Append(text);
                Sb.Append("</a>");
            }
            Sb.Append("</" + itemName + ">");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="url"></param>
        /// <param name="target"></param>
        public void AddItem(string text, string url, string target)
        {
            this.AddItem(text, url, target, "");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="url"></param>
        public void AddItem(string text, string url)
        {
            this.AddItem(text, url, "", "");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void AddItem(string text)
        {
            this.AddItem(text, "", "", "");
        }

        /// <summary>
        /// �����б��HTML
        /// </summary>
        /// <returns>�б��HTML</returns>
        public override string ToString()
        {
            Sb.Append("</" + Type + ">");
            return Sb.ToString();
        }

        /// <summary>
        /// ����ģ���һ��dataTable��ʹ��datatable�е�ֵ�����ģ��
        /// </summary>
        /// <param name="dataTable"></param>
        public void BindData(System.Data.DataTable dataTable)
        {
            foreach (System.Data.DataRow dr in dataTable.Rows)
            {
                foreach (DataColumn column in dataTable.Columns)
                {
                    AddTemplateData(column.ColumnName, dr[column.ColumnName].ToString());
                }
                EndTemplateData();
            }
        }
    }
}
