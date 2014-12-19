using System;
using System.Web;



public class RequestHelp
{
    /// <summary>
    /// �жϵ�ǰҳ���Ƿ���յ���Post����
    /// </summary>
    /// <returns>�Ƿ���յ���Post����</returns>
    public static bool IsPost()
    {
        return HttpContext.Current.Request.HttpMethod.Equals("POST");
    }
    /// <summary>
    /// �жϵ�ǰҳ���Ƿ���յ���Get����
    /// </summary>
    /// <returns>�Ƿ���յ���Get����</returns>
    public static bool IsGet()
    {
        return HttpContext.Current.Request.HttpMethod.Equals("GET");
    }

    /// <summary>
    /// ����ָ���ķ�����������Ϣ
    /// </summary>
    /// <param name="strName">������������</param>
    /// <returns>������������Ϣ</returns>
    public static string GetServerString(string strName)
    {
        //
        if (HttpContext.Current.Request.ServerVariables[strName] == null)
        {
            return "";
        }
        return HttpContext.Current.Request.ServerVariables[strName].ToString();
    }

    /// <summary>
    /// ������һ��ҳ��ĵ�ַ
    /// </summary>
    /// <returns>��һ��ҳ��ĵ�ַ</returns>
    public static string GetUrlReferrer()
    {
        string retVal = null;

        try
        {
            retVal = HttpContext.Current.Request.UrlReferrer.ToString();
        }
        catch { }

        if (retVal == null)
            return "";

        return retVal;

    }

    /// <summary>
    /// �õ���ǰ��������ͷ
    /// </summary>
    /// <returns></returns>
    public static string GetCurrentFullHost()
    {
        var request = System.Web.HttpContext.Current.Request;
        if (!request.Url.IsDefaultPort)
        {
            return string.Format("{0}:{1}", request.Url.Host, request.Url.Port.ToString());
        }
        return request.Url.Host;
    }

    /// <summary>
    /// �õ�����ͷ
    /// </summary>
    /// <returns></returns>
    public static string GetHost()
    {
        return HttpContext.Current.Request.Url.Host;
    }


    /// <summary>
    /// ��ȡ��ǰ�����ԭʼ URL(URL ������Ϣ֮��Ĳ���,������ѯ�ַ���(�������))
    /// </summary>
    /// <returns>ԭʼ URL</returns>
    public static string GetRawUrl()
    {
        return HttpContext.Current.Request.RawUrl;
    }

    /// <summary>
    /// �жϵ�ǰ�����Ƿ�������������
    /// </summary>
    /// <returns>��ǰ�����Ƿ�������������</returns>
    public static bool IsBrowserGet()
    {
        string[] browserName = { "ie", "opera", "netscape", "mozilla", "konqueror", "firefox" };
        var curBrowser = HttpContext.Current.Request.Browser.Type.ToLower();
        for (var i = 0; i < browserName.Length; i++)
        {
            if (curBrowser.IndexOf(browserName[i]) >= 0)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// ���ص�ǰҳ���Ƿ��ǿ�վ�ύ
    /// </summary>
    /// <returns>��ǰҳ���Ƿ��ǿ�վ�ύ</returns>
    public static bool IsCrossSitePost()
    {

        // ��������ύ��Ϊtrue
        if (!RequestHelp.IsPost())
        {
            return true;
        }
        return IsCrossSitePost(RequestHelp.GetUrlReferrer(), RequestHelp.GetHost());
    }

    /// <summary>
    /// �ж��Ƿ��ǿ�վ�ύ
    /// </summary>
    /// <param name="urlReferrer">�ϸ�ҳ���ַ</param>
    /// <param name="host">��̳url</param>
    /// <returns></returns>
    public static bool IsCrossSitePost(string urlReferrer, string host)
    {
        if (urlReferrer.Length < 7)
        {
            return true;
        }
        var u = new Uri(urlReferrer);
        return u.Host != host;
    }

    /// <summary>
    /// �ж��Ƿ�����������������
    /// </summary>
    /// <returns>�Ƿ�����������������</returns>
    public static bool IsSearchEnginesGet()
    {
        if (HttpContext.Current.Request.UrlReferrer == null)
        {
            return false;
        }
        string[] searchEngine = { "google", "yahoo", "msn", "baidu", "sogou", "sohu", "sina", "163", "lycos", "tom", "yisou", "iask", "soso", "gougou", "zhongsou" };
        var tmpReferrer = HttpContext.Current.Request.UrlReferrer.ToString().ToLower();
        for (var i = 0; i < searchEngine.Length; i++)
        {
            if (tmpReferrer.IndexOf(searchEngine[i]) >= 0)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// ��õ�ǰ����Url��ַ
    /// </summary>
    /// <returns>��ǰ����Url��ַ</returns>
    public static string GetUrl()
    {
        return HttpContext.Current.Request.Url.ToString();
    }


    /// <summary>
    /// ���ָ��Url������ֵ
    /// </summary>
    /// <param name="strName">Url����</param>
    /// <returns>Url������ֵ</returns>
    public static string GetQueryString(string strName)
    {

        if (HttpContext.Current.Request.QueryString[strName] == null)
        {
            return String.Empty;
        }
        return HttpContext.Current.Request.QueryString[strName];

    }
    /// <summary>
    /// ��õ�ǰҳ�������
    /// </summary>
    /// <returns>��ǰҳ�������</returns>
    public static string GetPageName()
    {
        var urlArr = HttpContext.Current.Request.Url.AbsolutePath.Split('/');
        return urlArr[urlArr.Length - 1].ToLower();
    }

    /// <summary>
    /// ���ر���Url�������ܸ���
    /// </summary>
    /// <returns></returns>
    public static int GetParamCount()
    {
        return HttpContext.Current.Request.Form.Count + HttpContext.Current.Request.QueryString.Count;
    }


    /// <summary>
    /// ���ָ����������ֵ
    /// </summary>
    /// <param name="strName">������</param>
    /// <returns>��������ֵ</returns>
    public static string GetFormString(string strName)
    {
        if (HttpContext.Current.Request.Form[strName] == null)
        {
            return "";
        }
        return HttpContext.Current.Request.Form[strName];
    }

    /// <summary>
    /// ���Url���������ֵ, ���ж�Url�����Ƿ�Ϊ���ַ���, ��ΪTrue�򷵻ر�������ֵ
    /// </summary>
    /// <param name="strName">����</param>
    /// <returns>Url���������ֵ</returns>
    public static string GetString(string strName)
    {
        if ("".Equals(GetQueryString(strName)))
        {
            return GetFormString(strName);
        }
        else
        {
            return GetQueryString(strName);
        }
    }



}

