using System;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;

namespace Core.IO
{
    /// <summary>
    /// 文件上传类
    /// </summary>
    public class FileUp
    {
        public FileUp()
        { }

        /// <summary>
        /// 转换为字节数组
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>字节数组</returns>
        public byte[] GetBinaryFile(string filename)
        {
            if (File.Exists(filename))
            {
                FileStream fsm = null;
                try
                {
                    fsm = File.OpenRead(filename);
                    return this.ConvertStreamToByteBuffer(fsm);
                }
                catch
                {
                    return new byte[0];
                }
                finally
                {
                    fsm.Close();
                }
            }
            else
            {
                return new byte[0];
            }
        }

        /// <summary>
        /// 流转化为字节数组
        /// </summary>
        /// <param name="theStream">流</param>
        /// <returns>字节数组</returns>
        public byte[] ConvertStreamToByteBuffer(System.IO.Stream theStream)
        {
            int bi;
            var tempStream = new System.IO.MemoryStream();
            try
            {
                while ((bi = theStream.ReadByte()) != -1)
                {
                    tempStream.WriteByte(((byte)bi));
                }
                return tempStream.ToArray();
            }
            catch
            {
                return new byte[0];
            }
            finally
            {
                tempStream.Close();
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="posPhotoUpload">控件</param>
        /// <param name="saveFileName">保存的文件名</param>
        /// <param name="imagePath">保存的文件路径</param>
        public string FileSc(FileUpload posPhotoUpload, string saveFileName, string imagePath)
        {
            var state = "";
            if (posPhotoUpload.HasFile)
            {
                if (posPhotoUpload.PostedFile.ContentLength / 1024 < 10240)
                {
                    var mimeType = posPhotoUpload.PostedFile.ContentType;
                    if (String.Equals(mimeType, "image/gif") || String.Equals(mimeType, "image/pjpeg"))
                    {
                        var extFileString = System.IO.Path.GetExtension(posPhotoUpload.PostedFile.FileName);
                        posPhotoUpload.PostedFile.SaveAs(HttpContext.Current.Server.MapPath(imagePath));
                    }
                    else
                    {
                        state = "上传文件类型不正确";
                    }
                }
                else
                {
                    state = "上传文件不能大于10M";
                }
            }
            else
            {
                state = "没有上传文件";
            }
            return state;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="binData">字节数组</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileType">文件类型</param>
        //-------------------调用----------------------
        //byte[] by = GetBinaryFile("E:\\Hello.txt");
        //this.SaveFile(by,"Hello",".txt");
        //---------------------------------------------
        public void SaveFile(byte[] binData, string fileName, string fileType)
        {
            FileStream fileStream = null;
            var m = new MemoryStream(binData);
            try
            {
                var savePath = HttpContext.Current.Server.MapPath("~/File/");
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
                var file = savePath + fileName + fileType;
                fileStream = new FileStream(file, FileMode.Create);
                m.WriteTo(fileStream);
            }
            finally
            {
                m.Close();
                fileStream.Close();
            }
        }

       

    
    }
}