using System;  
using System.Collections.Generic;  
using System.Text;  
using System.IO;  
using System.Net;
using System.Text.RegularExpressions;
using System.Configuration;

namespace Core.Net
{
    public class FtpHelper
    {
        #region 字段
        string _ftpUri;
        string _ftpUserId;
        string _ftpServerIp;
        string _ftpPassword;
        string _ftpRemotePath;
        #endregion

        /// <summary>  
        /// 连接FTP服务器
        /// </summary>  
        /// <param name="ftpServerIp">FTP连接地址</param>  
        /// <param name="FtpRemotePath">指定FTP连接成功后的当前目录, 如果不指定即默认为根目录</param>  
        /// <param name="ftpUserId">用户名</param>  
        /// <param name="FtpPassword">密码</param>  
        public FtpHelper(string ftpServerIp, string FtpRemotePath, string ftpUserId, string FtpPassword)
        {
            _ftpServerIp = ftpServerIp;
            _ftpRemotePath = FtpRemotePath;
            _ftpUserId = ftpUserId;
            _ftpPassword = FtpPassword;
            _ftpUri = "ftp://" + _ftpServerIp + "/" + _ftpRemotePath + "/";
        }

        /// <summary>  
        /// 上传  
        /// </summary>   
        public void Upload(string filename)
        {
            var fileInf = new FileInfo(filename);
            FtpWebRequest reqFtp;
            reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(_ftpUri + fileInf.Name));
            reqFtp.Credentials = new NetworkCredential(_ftpUserId, _ftpPassword);
            reqFtp.Method = WebRequestMethods.Ftp.UploadFile;
            reqFtp.KeepAlive = false;
            reqFtp.UseBinary = true;
            reqFtp.ContentLength = fileInf.Length;
            var buffLength = 2048;
            var buff = new byte[buffLength];
            int contentLen;
            var fs = fileInf.OpenRead();
            try
            {
                var strm = reqFtp.GetRequestStream();
                contentLen = fs.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                strm.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>  
        /// 下载  
        /// </summary>   
        public void Download(string filePath, string fileName)
        {
            try
            {
                var outputStream = new FileStream(filePath + "\\" + fileName, FileMode.Create);
                FtpWebRequest reqFtp;
                reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(_ftpUri + fileName));
                reqFtp.Credentials = new NetworkCredential(_ftpUserId, _ftpPassword);
                reqFtp.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFtp.UseBinary = true;
                var response = (FtpWebResponse)reqFtp.GetResponse();
                var ftpStream = response.GetResponseStream();
                var cl = response.ContentLength;
                var bufferSize = 2048;
                int readCount;
                var buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                ftpStream.Close();
                outputStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>  
        /// 删除文件  
        /// </summary>  
        public void Delete(string fileName)
        {
            try
            {
                FtpWebRequest reqFtp;
                reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(_ftpUri + fileName));
                reqFtp.Credentials = new NetworkCredential(_ftpUserId, _ftpPassword);
                reqFtp.Method = WebRequestMethods.Ftp.DeleteFile;
                reqFtp.KeepAlive = false;
                var result = String.Empty;
                var response = (FtpWebResponse)reqFtp.GetResponse();
                var size = response.ContentLength;
                var datastream = response.GetResponseStream();
                var sr = new StreamReader(datastream);
                result = sr.ReadToEnd();
                sr.Close();
                datastream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>  
        /// 获取当前目录下明细(包含文件和文件夹)  
        /// </summary>  
        public string[] GetFilesDetailList()
        {
            try
            {
                var result = new StringBuilder();
                FtpWebRequest ftp;
                ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri(_ftpUri));
                ftp.Credentials = new NetworkCredential(_ftpUserId, _ftpPassword);
                ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                var response = ftp.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());
                var line = reader.ReadLine();
                line = reader.ReadLine();
                line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf("\n"), 1);
                reader.Close();
                response.Close();
                return result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>  
        /// 获取FTP文件列表(包括文件夹)
        /// </summary>   
        private string[] GetAllList(string url)
        {
            var list = new List<string>();
            var req = (FtpWebRequest)WebRequest.Create(new Uri(url));
            req.Credentials = new NetworkCredential(_ftpPassword, _ftpPassword);
            req.Method = WebRequestMethods.Ftp.ListDirectory;
            req.UseBinary = true;
            req.UsePassive = true;
            try
            {
                using (var res = (FtpWebResponse)req.GetResponse())
                {
                    using (var sr = new StreamReader(res.GetResponseStream()))
                    {
                        string s;
                        while ((s = sr.ReadLine()) != null)
                        {
                            list.Add(s);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return list.ToArray();
        }

        /// <summary>  
        /// 获取当前目录下文件列表(不包括文件夹)  
        /// </summary>  
        public string[] GetFileList(string url)
        {
            var result = new StringBuilder();
            FtpWebRequest reqFtp;
            try
            {
                reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
                reqFtp.UseBinary = true;
                reqFtp.Credentials = new NetworkCredential(_ftpPassword, _ftpPassword);
                reqFtp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                var response = reqFtp.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());
                var line = reader.ReadLine();
                while (line != null)
                {

                    if (line.IndexOf("<DIR>") == -1)
                    {
                        result.Append(Regex.Match(line, @"[\S]+ [\S]+", RegexOptions.IgnoreCase).Value.Split(' ')[1]);
                        result.Append("\n");
                    }
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                reader.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return result.ToString().Split('\n');
        }

        /// <summary>  
        /// 判断当前目录下指定的文件是否存在  
        /// </summary>  
        /// <param name="remoteFileName">远程文件名</param>  
        public bool FileExist(string remoteFileName)
        {
            var fileList = GetFileList("*.*");
            foreach (var str in fileList)
            {
                if (str.Trim() == remoteFileName.Trim())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>  
        /// 创建文件夹  
        /// </summary>   
        public void MakeDir(string dirName)
        {
            FtpWebRequest reqFtp;
            try
            {
                reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(_ftpUri + dirName));
                reqFtp.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFtp.UseBinary = true;
                reqFtp.Credentials = new NetworkCredential(_ftpUserId, _ftpPassword);
                var response = (FtpWebResponse)reqFtp.GetResponse();
                var ftpStream = response.GetResponseStream();
                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            { }
        }

        /// <summary>  
        /// 获取指定文件大小  
        /// </summary>  
        public long GetFileSize(string filename)
        {
            FtpWebRequest reqFtp;
            long fileSize = 0;
            try
            {
                reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(_ftpUri + filename));
                reqFtp.Method = WebRequestMethods.Ftp.GetFileSize;
                reqFtp.UseBinary = true;
                reqFtp.Credentials = new NetworkCredential(_ftpUserId, _ftpPassword);
                var response = (FtpWebResponse)reqFtp.GetResponse();
                var ftpStream = response.GetResponseStream();
                fileSize = response.ContentLength;
                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            { }
            return fileSize;
        }

        /// <summary>  
        /// 更改文件名  
        /// </summary> 
        public void ReName(string currentFilename, string newFilename)
        {
            FtpWebRequest reqFtp;
            try
            {
                reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(_ftpUri + currentFilename));
                reqFtp.Method = WebRequestMethods.Ftp.Rename;
                reqFtp.RenameTo = newFilename;
                reqFtp.UseBinary = true;
                reqFtp.Credentials = new NetworkCredential(_ftpUserId, _ftpPassword);
                var response = (FtpWebResponse)reqFtp.GetResponse();
                var ftpStream = response.GetResponseStream();
                ftpStream.Close();
                response.Close();
            }
            catch (Exception ex)
            { }
        }

        /// <summary>  
        /// 移动文件  
        /// </summary>  
        public void MovieFile(string currentFilename, string newDirectory)
        {
            ReName(currentFilename, newDirectory);
        }

        /// <summary>  
        /// 切换当前目录  
        /// </summary>  
        /// <param name="isRoot">true:绝对路径 false:相对路径</param>   
        public void GotoDirectory(string directoryName, bool isRoot)
        {
            if (isRoot)
            {
                _ftpRemotePath = directoryName;
            }
            else
            {
                _ftpRemotePath += directoryName + "/";
            }
            _ftpUri = "ftp://" + _ftpServerIp + "/" + _ftpRemotePath + "/";
        }
        /// <summary>
        /// 获取上一层目录
        /// </summary>
        /// <returns></returns>
        public static string GetParentDirectory()
        {
            var context = System.Web.HttpContext.Current;
            var path = context.Session["Path"].ToString();
            if (path == "./")
                return ("../");
            else if (path == "/")
                return (ConfigurationManager.AppSettings["rootPath"].ToString());
            else
            {
                if (path.LastIndexOf("/") == path.Length - 1)
                {
                    path = path.Remove(path.LastIndexOf("/"), (path.Length - path.LastIndexOf("/")));
                }
                try
                {//path = path.Remove(path.LastIndexOf("/"), (path.Length - path.LastIndexOf("/")));
                    return (path + "/");
                }
                catch
                {
                    return (ConfigurationManager.AppSettings["rootPath"]);	// default to root;
                }
            }

        }


        /// <summary>
        /// 错误报告
        /// </summary>
        /// <param name="problem">问题</param>
        /// <param name="tech">技术</param>
        /// <param name="suggestion">建议</param>
        public static void ReportError(string problem, string tech, string suggestion)
        {
            var context = System.Web.HttpContext.Current;
            var output = "<font color=red><BIG>问题:</BIG> " + problem + "</font><hr>";
            output += "建议: " + suggestion + "<hr>";
            output += "<small>Technical details: " + tech + "</small><hr>";
            context.Response.Write(output);
        }
    }
}