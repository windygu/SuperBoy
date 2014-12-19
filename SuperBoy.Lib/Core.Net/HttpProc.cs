using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.IO.Compression;
using System.Net.Cache;

namespace Core.Net
{
    /// <summary>    
    /// �ϴ����ݲ���    
    /// </summary>    
    public class UploadEventArgs : EventArgs
    {
        int _bytesSent;
        int _totalBytes;
        /// <summary>    
        /// �ѷ��͵��ֽ���    
        /// </summary>    
        public int BytesSent
        {
            get { return _bytesSent; }
            set { _bytesSent = value; }
        }
        /// <summary>    
        /// ���ֽ���    
        /// </summary>    
        public int TotalBytes
        {
            get { return _totalBytes; }
            set { _totalBytes = value; }
        }
    }
    /// <summary>    
    /// �������ݲ���    
    /// </summary>    
    public class DownloadEventArgs : EventArgs
    {
        int _bytesReceived;
        int _totalBytes;
        byte[] _receivedData;
        /// <summary>    
        /// �ѽ��յ��ֽ���    
        /// </summary>    
        public int BytesReceived
        {
            get { return _bytesReceived; }
            set { _bytesReceived = value; }
        }
        /// <summary>    
        /// ���ֽ���    
        /// </summary>    
        public int TotalBytes
        {
            get { return _totalBytes; }
            set { _totalBytes = value; }
        }
        /// <summary>    
        /// ��ǰ���������յ�����    
        /// </summary>    
        public byte[] ReceivedData
        {
            get { return _receivedData; }
            set { _receivedData = value; }
        }
    }

    public class WebClient
    {
        Encoding _encoding = Encoding.Default;
        string _respHtml = "";
        WebProxy _proxy;
        static CookieContainer _cc;
        WebHeaderCollection _requestHeaders;
        WebHeaderCollection _responseHeaders;
        int _bufferSize = 15240;
        public event EventHandler<UploadEventArgs> UploadProgressChanged;
        public event EventHandler<DownloadEventArgs> DownloadProgressChanged;
        static WebClient()
        {
            LoadCookiesFromDisk();
        }
        /// <summary>    
        /// ����WebClient��ʵ��    
        /// </summary>    
        public WebClient()
        {
            _requestHeaders = new WebHeaderCollection();
            _responseHeaders = new WebHeaderCollection();
        }
        /// <summary>    
        /// ���÷��ͺͽ��յ����ݻ����С    
        /// </summary>    
        public int BufferSize
        {
            get { return _bufferSize; }
            set { _bufferSize = value; }
        }
        /// <summary>    
        /// ��ȡ��Ӧͷ����    
        /// </summary>    
        public WebHeaderCollection ResponseHeaders
        {
            get { return _responseHeaders; }
        }
        /// <summary>    
        /// ��ȡ����ͷ����    
        /// </summary>    
        public WebHeaderCollection RequestHeaders
        {
            get { return _requestHeaders; }
        }
        /// <summary>    
        /// ��ȡ�����ô���    
        /// </summary>    
        public WebProxy Proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }
        /// <summary>    
        /// ��ȡ��������������Ӧ���ı����뷽ʽ    
        /// </summary>    
        public Encoding Encoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }
        /// <summary>    
        /// ��ȡ��������Ӧ��html����    
        /// </summary>    
        public string RespHtml
        {
            get { return _respHtml; }
            set { _respHtml = value; }
        }
        /// <summary>    
        /// ��ȡ�����������������Cookie����    
        /// </summary>    
        public CookieContainer CookieContainer
        {
            get { return _cc; }
            set { _cc = value; }
        }
        /// <summary>    
        ///  ��ȡ��ҳԴ����    
        /// </summary>    
        /// <param name="url">��ַ</param>    
        /// <returns></returns>    
        public string GetHtml(string url)
        {
            var request = CreateRequest(url, "GET");
            _respHtml = _encoding.GetString(GetData(request));
            return _respHtml;
        }
        /// <summary>    
        /// �����ļ�    
        /// </summary>    
        /// <param name="url">�ļ�URL��ַ</param>    
        /// <param name="filename">�ļ���������·��</param>    
        public void DownloadFile(string url, string filename)
        {
            FileStream fs = null;
            try
            {
                var request = CreateRequest(url, "GET");
                var data = GetData(request);
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
                fs.Write(data, 0, data.Length);
            }
            finally
            {
                if (fs != null) fs.Close();
            }
        }
        /// <summary>    
        /// ��ָ��URL��������    
        /// </summary>    
        /// <param name="url">��ַ</param>    
        /// <returns></returns>    
        public byte[] GetData(string url)
        {
            var request = CreateRequest(url, "GET");
            return GetData(request);
        }
        /// <summary>    
        /// ��ָ��URL�����ı�����    
        /// </summary>    
        /// <param name="url">��ַ</param>    
        /// <param name="postData">urlencode������ı�����</param>    
        /// <returns></returns>    
        public string Post(string url, string postData)
        {
            var data = _encoding.GetBytes(postData);
            return Post(url, data);
        }
        /// <summary>    
        /// ��ָ��URL�����ֽ�����    
        /// </summary>    
        /// <param name="url">��ַ</param>    
        /// <param name="postData">���͵��ֽ�����</param>    
        /// <returns></returns>    
        public string Post(string url, byte[] postData)
        {
            var request = CreateRequest(url, "POST");
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postData.Length;
            request.KeepAlive = true;
            PostData(request, postData);
            _respHtml = _encoding.GetString(GetData(request));
            return _respHtml;
        }
        /// <summary>    
        /// ��ָ����ַ����mulitpart���������    
        /// </summary>    
        /// <param name="url">��ַ</param>    
        /// <param name="mulitpartForm">mulitpart form data</param>    
        /// <returns></returns>    
        public string Post(string url, MultipartForm mulitpartForm)
        {
            var request = CreateRequest(url, "POST");
            request.ContentType = mulitpartForm.ContentType;
            request.ContentLength = mulitpartForm.FormData.Length;
            request.KeepAlive = true;
            PostData(request, mulitpartForm.FormData);
            _respHtml = _encoding.GetString(GetData(request));
            return _respHtml;
        }
        
        /// <summary>    
        /// ��ȡ���󷵻ص�����    
        /// </summary>    
        /// <param name="request">�������</param>    
        /// <returns></returns>    
        private byte[] GetData(HttpWebRequest request)
        {
            var response = (HttpWebResponse)request.GetResponse();
            var stream = response.GetResponseStream();
            _responseHeaders = response.Headers;
            //SaveCookiesToDisk();

            var args = new DownloadEventArgs();
            if (_responseHeaders[HttpResponseHeader.ContentLength] != null)
                args.TotalBytes = Convert.ToInt32(_responseHeaders[HttpResponseHeader.ContentLength]);

            var ms = new MemoryStream();
            var count = 0;
            var buf = new byte[_bufferSize];
            while ((count = stream.Read(buf, 0, buf.Length)) > 0)
            {
                ms.Write(buf, 0, count);
                if (this.DownloadProgressChanged != null)
                {
                    args.BytesReceived += count;
                    args.ReceivedData = new byte[count];
                    Array.Copy(buf, args.ReceivedData, count);
                    this.DownloadProgressChanged(this, args);
                }
            }
            stream.Close();
            //��ѹ    
            if (ResponseHeaders[HttpResponseHeader.ContentEncoding] != null)
            {
                var msTemp = new MemoryStream();
                count = 0;
                buf = new byte[100];
                switch (ResponseHeaders[HttpResponseHeader.ContentEncoding].ToLower())
                {
                    case "gzip":
                        var gzip = new GZipStream(ms, CompressionMode.Decompress);
                        while ((count = gzip.Read(buf, 0, buf.Length)) > 0)
                        {
                            msTemp.Write(buf, 0, count);
                        }
                        return msTemp.ToArray();
                    case "deflate":
                        var deflate = new DeflateStream(ms, CompressionMode.Decompress);
                        while ((count = deflate.Read(buf, 0, buf.Length)) > 0)
                        {
                            msTemp.Write(buf, 0, count);
                        }
                        return msTemp.ToArray();
                    default:
                        break;
                }
            }
            return ms.ToArray();
        }
        /// <summary>    
        /// ������������    
        /// </summary>    
        /// <param name="request">�������</param>    
        /// <param name="postData">�����͵��ֽ�����</param>    
        private void PostData(HttpWebRequest request, byte[] postData)
        {
            var offset = 0;
            var sendBufferSize = _bufferSize;
            var remainBytes = 0;
            var stream = request.GetRequestStream();
            var args = new UploadEventArgs();
            args.TotalBytes = postData.Length;
            while ((remainBytes = postData.Length - offset) > 0)
            {
                if (sendBufferSize > remainBytes) sendBufferSize = remainBytes;
                stream.Write(postData, offset, sendBufferSize);
                offset += sendBufferSize;
                if (this.UploadProgressChanged != null)
                {
                    args.BytesSent = offset;
                    this.UploadProgressChanged(this, args);
                }
            }
            stream.Close();
        }
        /// <summary>    
        /// ����HTTP����    
        /// </summary>    
        /// <param name="url">URL��ַ</param>    
        /// <returns></returns>    
        private HttpWebRequest CreateRequest(string url, string method)
        {
            var uri = new Uri(url);

            if (uri.Scheme == "https")
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(this.CheckValidationResult);

            // Set a default policy level for the "http:" and "https" schemes.    
            var policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
            HttpWebRequest.DefaultCachePolicy = policy;

            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.AllowAutoRedirect = false;
            request.AllowWriteStreamBuffering = false;
            request.Method = method;
            if (_proxy != null) 
                request.Proxy = _proxy;
            request.CookieContainer = _cc;
            foreach (string key in _requestHeaders.Keys)
            {
                request.Headers.Add(key, _requestHeaders[key]);
            }
            _requestHeaders.Clear();
            return request;
        }
        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
        /// <summary>    
        /// ��Cookie���浽����    
        /// </summary>    
        private static void SaveCookiesToDisk()
        {
            var cookieFile = System.Environment.GetFolderPath(Environment.SpecialFolder.Cookies) + "\\webclient.cookie";
            FileStream fs = null;
            try
            {
                fs = new FileStream(cookieFile, FileMode.Create);
                var formater = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formater.Serialize(fs, _cc);
            }
            finally
            {
                if (fs != null) fs.Close();
            }
        }
        /// <summary>    
        /// �Ӵ��̼���Cookie    
        /// </summary>    
        private static void LoadCookiesFromDisk()
        {
            _cc = new CookieContainer();
            var cookieFile = System.Environment.GetFolderPath(Environment.SpecialFolder.Cookies) + "\\webclient.cookie";
            if (!File.Exists(cookieFile))
                return;
            FileStream fs = null;
            try
            {
                fs = new FileStream(cookieFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                var formater = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                _cc = (CookieContainer)formater.Deserialize(fs);
            }
            finally
            {
                if (fs != null) fs.Close();
            }
        }
    }

    /// <summary>    
    /// ���ļ����ı����ݽ���Multipart��ʽ�ı���    
    /// </summary>    
    public class MultipartForm
    {
        private Encoding _encoding;
        private MemoryStream _ms;
        private string _boundary;
        private byte[] _formData;
        /// <summary>    
        /// ��ȡ�������ֽ�����    
        /// </summary>    
        public byte[] FormData
        {
            get
            {
                if (_formData == null)
                {
                    var buffer = _encoding.GetBytes("--" + this._boundary + "--\r\n");
                    _ms.Write(buffer, 0, buffer.Length);
                    _formData = _ms.ToArray();
                }
                return _formData;
            }
        }
        /// <summary>    
        /// ��ȡ�˱������ݵ�����    
        /// </summary>    
        public string ContentType
        {
            get { return string.Format("multipart/form-data; boundary={0}", this._boundary); }
        }
        /// <summary>    
        /// ��ȡ�����ö��ַ������õı�������    
        /// </summary>    
        public Encoding StringEncoding
        {
            set { _encoding = value; }
            get { return _encoding; }
        }
        /// <summary>    
        /// ʵ����    
        /// </summary>    
        public MultipartForm()
        {
            _boundary = string.Format("--{0}--", Guid.NewGuid());
            _ms = new MemoryStream();
            _encoding = Encoding.Default;
        }
        /// <summary>    
        /// ���һ���ļ�    
        /// </summary>    
        /// <param name="name">�ļ�������</param>    
        /// <param name="filename">�ļ�������·��</param>    
        public void AddFlie(string name, string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException("������Ӳ����ڵ��ļ���", filename);
            FileStream fs = null;
            byte[] fileData = { };
            try
            {
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                fileData = new byte[fs.Length];
                fs.Read(fileData, 0, fileData.Length);
                this.AddFlie(name, Path.GetFileName(filename), fileData, fileData.Length);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (fs != null) fs.Close();
            }
        }
        /// <summary>    
        /// ���һ���ļ�    
        /// </summary>    
        /// <param name="name">�ļ�������</param>    
        /// <param name="filename">�ļ���</param>    
        /// <param name="fileData">�ļ�����������</param>    
        /// <param name="dataLength">���������ݴ�С</param>    
        public void AddFlie(string name, string filename, byte[] fileData, int dataLength)
        {
            if (dataLength <= 0 || dataLength > fileData.Length)
            {
                dataLength = fileData.Length;
            }
            var sb = new StringBuilder();
            sb.AppendFormat("--{0}\r\n", this._boundary);
            sb.AppendFormat("Content-Disposition: form-data; name=\"{0}\";filename=\"{1}\"\r\n", name, filename);
            sb.AppendFormat("Content-Type: {0}\r\n", this.GetContentType(filename));
            sb.Append("\r\n");
            var buf = _encoding.GetBytes(sb.ToString());
            _ms.Write(buf, 0, buf.Length);
            _ms.Write(fileData, 0, dataLength);
            var crlf = _encoding.GetBytes("\r\n");
            _ms.Write(crlf, 0, crlf.Length);
        }
        /// <summary>    
        /// ����ַ���    
        /// </summary>    
        /// <param name="name">�ı�������</param>    
        /// <param name="value">�ı�ֵ</param>    
        public void AddString(string name, string value)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("--{0}\r\n", this._boundary);
            sb.AppendFormat("Content-Disposition: form-data; name=\"{0}\"\r\n", name);
            sb.Append("\r\n");
            sb.AppendFormat("{0}\r\n", value);
            var buf = _encoding.GetBytes(sb.ToString());
            _ms.Write(buf, 0, buf.Length);
        }
        /// <summary>    
        /// ��ע����ȡ�ļ�����    
        /// </summary>    
        /// <param name="filename">������չ�����ļ���</param>    
        /// <returns>�磺application/stream</returns>    
        private string GetContentType(string filename)
        {
            Microsoft.Win32.RegistryKey fileExtKey = null; ;
            var contentType = "application/stream";
            try
            {
                fileExtKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(Path.GetExtension(filename));
                contentType = fileExtKey.GetValue("Content Type", contentType).ToString();
            }
            finally
            {
                if (fileExtKey != null) fileExtKey.Close();
            }
            return contentType;
        }
    }
}
