using System;
using System.IO;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Drawing;

namespace Core.Drawing
{
    /// <summary>
    /// 文件类型
    /// </summary>
    public enum FileExtension
    {
        Jpg = 255216,
        Gif = 7173,
        Bmp = 6677,
        Png = 13780,
        Rar = 8297,
        jpg = 255216,
        Exe = 7790,
        Xml = 6063,
        Html = 6033,
        Aspx = 239187,
        Cs = 117115,
        Js = 119105,
        Txt = 210187,
        Sql = 255254
    }

    /// <summary>
    /// 图片检测类
    /// </summary>
    public static class FileValidation
    {
        #region 上传图片检测类
        /// <summary>
        /// 是否允许
        /// </summary>
        public static bool IsAllowedExtension(HttpPostedFile oFile, FileExtension[] fileEx)
        {
            var fileLen = oFile.ContentLength;
            var imgArray = new byte[fileLen];
            oFile.InputStream.Read(imgArray, 0, fileLen);
            var ms = new MemoryStream(imgArray);
            var br = new System.IO.BinaryReader(ms);
            var fileclass = "";
            byte buffer;
            try
            {
                buffer = br.ReadByte();
                fileclass = buffer.ToString();
                buffer = br.ReadByte();
                fileclass += buffer.ToString();
            }
            catch { }
            br.Close();
            ms.Close();
            foreach (var fe in fileEx)
            {
                if (Int32.Parse(fileclass) == (int)fe) return true;
            }
            return false;
        }

        /// <summary>
        /// 上传前的图片是否可靠
        /// </summary>
        public static bool IsSecureUploadPhoto(HttpPostedFile oFile)
        {
            var isPhoto = false;
            var fileExtension = System.IO.Path.GetExtension(oFile.FileName).ToLower();
            string[] allowedExtensions = { ".gif", ".png", ".jpeg", ".jpg", ".bmp" };
            for (var i = 0; i < allowedExtensions.Length; i++)
            {
                if (fileExtension == allowedExtensions[i])
                {
                    isPhoto = true;
                    break;
                }
            }
            if (!isPhoto)
            {
                return true;
            }
            FileExtension[] fe = { FileExtension.Bmp, FileExtension.Gif, FileExtension.Jpg, FileExtension.Png };

            if (IsAllowedExtension(oFile, fe))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 上传后的图片是否安全
        /// </summary>
        /// <param name="photoFile">物理地址</param>
        public static bool IsSecureUpfilePhoto(string photoFile)
        {
            var isPhoto = false;
            var img = "Yes";
            var fileExtension = System.IO.Path.GetExtension(photoFile).ToLower();
            string[] allowedExtensions = { ".gif", ".png", ".jpeg", ".jpg", ".bmp" };
            for (var i = 0; i < allowedExtensions.Length; i++)
            {
                if (fileExtension == allowedExtensions[i])
                {
                    isPhoto = true;
                    break;
                }
            }

            if (!isPhoto)
            {
                return true;
            }
            var sr = new StreamReader(photoFile, System.Text.Encoding.Default);
            var strContent = sr.ReadToEnd();
            sr.Close();
            var str = "request|<script|.getfolder|.createfolder|.deletefolder|.createdirectory|.deletedirectory|.saveas|wscript.shell|script.encode|server.|.createobject|execute|activexobject|language=";
            foreach (var s in str.Split('|'))
            {
                if (strContent.ToLower().IndexOf(s) != -1)
                {
                    File.Delete(photoFile);
                    img = "No";
                    break;
                }
            }
            return (img == "Yes");
        }
        #endregion
    }

    /// <summary>
    /// 图片上传类
    /// </summary>
    //----------------调用-------------------
    //imageUpload iu = new imageUpload();
    //iu.AddText = "";
    //iu.CopyIamgePath = "";
    //iu.DrawString_x = ;
    //iu.DrawString_y = ;
    //iu.DrawStyle = ;
    //iu.Font = "";
    //iu.FontSize = ;
    //iu.FormFile = File1;
    //iu.IsCreateImg =;
    //iu.IsDraw = ;
    //iu.OutFileName = "";
    //iu.OutThumbFileName = "";
    //iu.SavePath = @"~/image/";
    //iu.SaveType = ;
    //iu.sHeight  = ;
    //iu.sWidth   = ;
    //iu.Upload();
    //--------------------------------------
    public class ImageUpload
    {
        #region 私有成员
        private int _error = 0;//返回上传状态。 
        private int _maxSize = 1024 * 1024;//最大单个上传文件 (默认)
        private string _fileType = "jpg;gif;bmp;png";//所支持的上传类型用"/"隔开 
        private string _savePath = System.Web.HttpContext.Current.Server.MapPath(".") + "\\";//保存文件的实际路径 
        private int _saveType = 0;//上传文件的类型，0代表自动生成文件名 
        private HtmlInputFile _formFile;//上传控件。 
        private string _inFileName = "";//非自动生成文件名设置。 
        private string _outFileName = "";//输出文件名。 
        private bool _isCreateImg = true;//是否生成缩略图。 
        private bool _iss = false;//是否有缩略图生成.
        private int _height = 0;//获取上传图片的高度 
        private int _width = 0;//获取上传图片的宽度 
        private int _sHeight = 120;//设置生成缩略图的高度 
        private int _sWidth = 120;//设置生成缩略图的宽度
        private bool _isDraw = false;//设置是否加水印
        private int _drawStyle = 0;//设置加水印的方式０：文字水印模式，１：图片水印模式,2:不加
        private int _drawStringX = 10;//绘制文本的Ｘ坐标（左上角）
        private int _drawStringY = 10;//绘制文本的Ｙ坐标（左上角）
        private string _addText = "GlobalNatureCrafts";//设置水印内容
        private string _font = "宋体";//设置水印字体
        private int _fontSize = 12;//设置水印字大小
        private int _fileSize = 0;//获取已经上传文件的大小
        private string _copyIamgePath = System.Web.HttpContext.Current.Server.MapPath(".") + "/images/5dm_new.jpg";//图片水印模式下的覆盖图片的实际地址
        #endregion

        #region 公有属性
        /// <summary>
        /// Error返回值
        /// 1、没有上传的文件
        /// 2、类型不允许
        /// 3、大小超限
        /// 4、未知错误
        /// 0、上传成功。 
        /// </summary>
        public int Error
        {
            get { return _error; }
        }

        /// <summary>
        /// 最大单个上传文件
        /// </summary>
        public int MaxSize
        {
            set { _maxSize = value; }
        }

        /// <summary>
        /// 所支持的上传类型用";"隔开 
        /// </summary>
        public string FileType
        {
            set { _fileType = value; }
        }

        /// <summary>
        /// 保存文件的实际路径 
        /// </summary>
        public string SavePath
        {
            set { _savePath = System.Web.HttpContext.Current.Server.MapPath(value); }
            get { return _savePath; }
        }

        /// <summary>
        /// 上传文件的类型，0代表自动生成文件名
        /// </summary>
        public int SaveType
        {
            set { _saveType = value; }
        }

        /// <summary>
        /// 上传控件
        /// </summary>
        public HtmlInputFile FormFile
        {
            set { _formFile = value; }
        }

        /// <summary>
        /// 非自动生成文件名设置。
        /// </summary>
        public string InFileName
        {
            set { _inFileName = value; }
        }

        /// <summary>
        /// 输出文件名
        /// </summary>
        public string OutFileName
        {
            get { return _outFileName; }
            set { _outFileName = value; }
        }

        /// <summary>
        /// 输出的缩略图文件名
        /// </summary>
        public string OutThumbFileName
        {
            get;
            set;
        }

        /// <summary>
        /// 是否有缩略图生成.
        /// </summary>
        public bool Iss
        {
            get { return _iss; }
        }

        /// <summary>
        /// 获取上传图片的宽度
        /// </summary>
        public int Width
        {
            get { return _width; }
        }

        /// <summary>
        /// 获取上传图片的高度
        /// </summary>
        public int Height
        {
            get { return _height; }
        }

        /// <summary>
        /// 设置缩略图的宽度
        /// </summary>
        public int SWidth
        {
            get { return _sWidth; }
            set { _sWidth = value; }
        }

        /// <summary>
        /// 设置缩略图的高度
        /// </summary>
        public int SHeight
        {
            get { return _sHeight; }
            set { _sHeight = value; }
        }

        /// <summary>
        /// 是否生成缩略图
        /// </summary>
        public bool IsCreateImg
        {
            get { return _isCreateImg; }
            set { _isCreateImg = value; }
        }

        /// <summary>
        /// 是否加水印
        /// </summary>
        public bool IsDraw
        {
            get { return _isDraw; }
            set { _isDraw = value; }
        }

        /// <summary>
        /// 设置加水印的方式
        /// 0:文字水印模式
        /// 1:图片水印模式
        /// 2:不加
        /// </summary>
        public int DrawStyle
        {
            get { return _drawStyle; }
            set { _drawStyle = value; }
        }

        /// <summary>
        /// 绘制文本的Ｘ坐标（左上角）
        /// </summary>
        public int DrawStringX
        {
            get { return _drawStringX; }
            set { _drawStringX = value; }
        }

        /// <summary>
        /// 绘制文本的Ｙ坐标（左上角）
        /// </summary>
        public int DrawStringY
        {
            get { return _drawStringY; }
            set { _drawStringY = value; }
        }

        /// <summary>
        /// 设置文字水印内容
        /// </summary>
        public string AddText
        {
            get { return _addText; }
            set { _addText = value; }
        }

        /// <summary>
        /// 设置文字水印字体
        /// </summary>
        public string Font
        {
            get { return _font; }
            set { _font = value; }
        }

        /// <summary>
        /// 设置文字水印字的大小
        /// </summary>
        public int FontSize
        {
            get { return _fontSize; }
            set { _fontSize = value; }
        }

        /// <summary>
        /// 文件大小
        /// </summary>
        public int FileSize
        {
            get { return _fileSize; }
            set { _fileSize = value; }
        }

        /// <summary>
        /// 图片水印模式下的覆盖图片的实际地址
        /// </summary>
        public string CopyIamgePath
        {
            set { _copyIamgePath = System.Web.HttpContext.Current.Server.MapPath(value); }
        }

        #endregion

        #region 私有方法
        /// <summary>
        /// 获取文件的后缀名 
        /// </summary>
        private string GetExt(string path)
        {
            return Path.GetExtension(path);
        }

        /// <summary>
        /// 获取输出文件的文件名
        /// </summary>
        private string FileName(string ext)
        {
            if (_saveType == 0 || _inFileName.Trim() == "")
                return DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext;
            else
                return _inFileName;
        }

        /// <summary>
        /// 检查上传的文件的类型，是否允许上传。
        /// </summary>
        private bool IsUpload(string ext)
        {
            ext = ext.Replace(".", "");
            var b = false;
            var arrFileType = _fileType.Split(';');
            foreach (var str in arrFileType)
            {
                if (str.ToLower() == ext.ToLower())
                {
                    b = true;
                    break;
                }
            }
            return b;
        }
        #endregion

        #region 上传图片
        public void Upload()
        {
            var hpFile = _formFile.PostedFile;
            if (hpFile == null || hpFile.FileName.Trim() == "")
            {
                _error = 1;
                return;
            }
            var ext = GetExt(hpFile.FileName);
            if (!IsUpload(ext))
            {
                _error = 2;
                return;
            }
            var iLen = hpFile.ContentLength;
            if (iLen > _maxSize)
            {
                _error = 3;
                return;
            }
            try
            {
                if (!Directory.Exists(_savePath)) Directory.CreateDirectory(_savePath);
                var bData = new byte[iLen];
                hpFile.InputStream.Read(bData, 0, iLen);
                string fName;
                fName = FileName(ext);
                var tempFile = "";
                if (_isDraw)
                {
                    tempFile = fName.Split('.').GetValue(0).ToString() + "_temp." + fName.Split('.').GetValue(1).ToString();
                }
                else
                {
                    tempFile = fName;
                }
                var newFile = new FileStream(_savePath + tempFile, FileMode.Create);
                newFile.Write(bData, 0, bData.Length);
                newFile.Flush();
                var fileSizeTemp = hpFile.ContentLength;

                var imageFilePath = _savePath + fName;
                if (_isDraw)
                {
                    if (_drawStyle == 0)
                    {
                        var img1 = System.Drawing.Image.FromStream(newFile);
                        var g = Graphics.FromImage(img1);
                        g.DrawImage(img1, 100, 100, img1.Width, img1.Height);
                        var f = new Font(_font, _fontSize);
                        Brush b = new SolidBrush(Color.Red);
                        var addtext = _addText;
                        g.DrawString(addtext, f, b, _drawStringX, _drawStringY);
                        g.Dispose();
                        img1.Save(imageFilePath);
                        img1.Dispose();
                    }
                    else
                    {
                        var image = System.Drawing.Image.FromStream(newFile);
                        var copyImage = System.Drawing.Image.FromFile(_copyIamgePath);
                        var g = Graphics.FromImage(image);
                        g.DrawImage(copyImage, new Rectangle(image.Width - copyImage.Width - 5, image.Height - copyImage.Height - 5, copyImage.Width, copyImage.Height), 0, 0, copyImage.Width, copyImage.Height, GraphicsUnit.Pixel);
                        g.Dispose();
                        image.Save(imageFilePath);
                        image.Dispose();
                    }
                }

                //获取图片的高度和宽度
                var img = System.Drawing.Image.FromStream(newFile);
                _width = img.Width;
                _height = img.Height;

                //生成缩略图部分 
                if (_isCreateImg)
                {
                    #region 缩略图大小只设置了最大范围，并不是实际大小
                    var realbili = (float)_width / (float)_height;
                    var wishbili = (float)_sWidth / (float)_sHeight;

                    //实际图比缩略图最大尺寸更宽矮，以宽为准
                    if (realbili > wishbili)
                    {
                        _sHeight = (int)((float)_sWidth / realbili);
                    }
                    //实际图比缩略图最大尺寸更高长，以高为准
                    else
                    {
                        _sWidth = (int)((float)_sHeight * realbili);
                    }
                    #endregion

                    this.OutThumbFileName = fName.Split('.').GetValue(0).ToString() + "_s." + fName.Split('.').GetValue(1).ToString();
                    var imgFilePath = _savePath + this.OutThumbFileName;

                    var newImg = img.GetThumbnailImage(_sWidth, _sHeight, null, System.IntPtr.Zero);
                    newImg.Save(imgFilePath);
                    newImg.Dispose();
                    _iss = true;
                }

                if (_isDraw)
                {
                    if (File.Exists(_savePath + fName.Split('.').GetValue(0).ToString() + "_temp." + fName.Split('.').GetValue(1).ToString()))
                    {
                        newFile.Dispose();
                        File.Delete(_savePath + fName.Split('.').GetValue(0).ToString() + "_temp." + fName.Split('.').GetValue(1).ToString());
                    }
                }
                newFile.Close();
                newFile.Dispose();
                _outFileName = fName;
                _fileSize = fileSizeTemp;
                _error = 0;
                return;
            }
            catch (Exception ex)
            {
                _error = 4;
                return;
            }
        }
        #endregion
    }
}