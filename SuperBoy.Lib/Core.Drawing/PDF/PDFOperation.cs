using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Core.Drawing
{
    /// <summary>
    /// PDF文档操作类
    /// </summary>
    //------------------------------------调用--------------------------------------------
    //PDFOperation pdf = new PDFOperation();
    //pdf.Open(new FileStream(path, FileMode.Create));
    //pdf.SetBaseFont(@"C:\Windows\Fonts\SIMHEI.TTF");
    //pdf.AddParagraph("测试文档（生成时间：" + DateTime.Now + "）", 15, 1, 20, 0, 0);
    //pdf.Close();
    //-------------------------------------------------------------------------------------
    public class PdfOperation
    {
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public PdfOperation()
        {
            _rect = PageSize.A4;
            _document = new Document(_rect);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">页面大小(如"A4")</param>
        public PdfOperation(string type)
        {
            SetPageSize(type);
            _document = new Document(_rect);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">页面大小(如"A4")</param>
        /// <param name="marginLeft">内容距左边框距离</param>
        /// <param name="marginRight">内容距右边框距离</param>
        /// <param name="marginTop">内容距上边框距离</param>
        /// <param name="marginBottom">内容距下边框距离</param>
        public PdfOperation(string type, float marginLeft, float marginRight, float marginTop, float marginBottom)
        {
            SetPageSize(type);
            _document = new Document(_rect, marginLeft, marginRight, marginTop, marginBottom);
        }
        #endregion

        #region 私有字段
        private Font _font;
        private Rectangle _rect;   //文档大小
        private Document _document;//文档对象
        private BaseFont _basefont;//字体
        #endregion

        #region 设置字体
        /// <summary>
        /// 设置字体
        /// </summary>
        public void SetBaseFont(string path)
        {
            _basefont = BaseFont.CreateFont(path, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        }

        /// <summary>
        /// 设置字体
        /// </summary>
        /// <param name="size">字体大小</param>
        public void SetFont(float size)
        {
            _font = new Font(_basefont, size);
        }
        #endregion

        #region 设置页面大小
        /// <summary>
        /// 设置页面大小
        /// </summary>
        /// <param name="type">页面大小(如"A4")</param>
        public void SetPageSize(string type)
        {
            switch (type.Trim())
            {
                case "A4":
                    _rect = PageSize.A4;
                    break;
                case "A8":
                    _rect = PageSize.A8;
                    break;
            }
        }
        #endregion

        #region 实例化文档
        /// <summary>
        /// 实例化文档
        /// </summary>
        /// <param name="os">文档相关信息（如路径，打开方式等）</param>
        public void GetInstance(Stream os)
        {
            PdfWriter.GetInstance(_document, os);
        }
        #endregion

        #region 打开文档对象
        /// <summary>
        /// 打开文档对象
        /// </summary>
        /// <param name="os">文档相关信息（如路径，打开方式等）</param>
        public void Open(Stream os)
        {
            GetInstance(os);
            _document.Open();
        }
        #endregion

        #region 关闭打开的文档
        /// <summary>
        /// 关闭打开的文档
        /// </summary>
        public void Close()
        {
            _document.Close();
        }
        #endregion

        #region 添加段落
        /// <summary>
        /// 添加段落
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="fontsize">字体大小</param>
        public void AddParagraph(string content, float fontsize)
        {
            SetFont(fontsize);
            var pra = new Paragraph(content, _font);
            _document.Add(pra);
        }

        /// <summary>
        /// 添加段落
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="fontsize">字体大小</param>
        /// <param name="alignment">对齐方式（1为居中，0为居左，2为居右）</param>
        /// <param name="spacingAfter">段后空行数（0为默认值）</param>
        /// <param name="spacingBefore">段前空行数（0为默认值）</param>
        /// <param name="multipliedLeading">行间距（0为默认值）</param>
        public void AddParagraph(string content, float fontsize, int alignment, float spacingAfter, float spacingBefore, float multipliedLeading)
        {
            SetFont(fontsize);
            var pra = new Paragraph(content, _font);
            pra.Alignment = alignment;
            if (spacingAfter != 0)
            {
                pra.SpacingAfter = spacingAfter;
            }
            if (spacingBefore != 0)
            {
                pra.SpacingBefore = spacingBefore;
            }
            if (multipliedLeading != 0)
            {
                pra.MultipliedLeading = multipliedLeading;
            }
            _document.Add(pra);
        }
        #endregion

        #region 添加图片
        /// <summary>
        /// 添加图片
        /// </summary>
        /// <param name="path">图片路径</param>
        /// <param name="alignment">对齐方式（1为居中，0为居左，2为居右）</param>
        /// <param name="newWidth">图片宽（0为默认值，如果宽度大于页宽将按比率缩放）</param>
        /// <param name="newHeight">图片高</param>
        public void AddImage(string path, int alignment, float newWidth, float newHeight)
        {
            var img = Image.GetInstance(path);
            img.Alignment = alignment;
            if (newWidth != 0)
            {
                img.ScaleAbsolute(newWidth, newHeight);
            }
            else
            {
                if (img.Width > PageSize.A4.Width)
                {
                    img.ScaleAbsolute(_rect.Width, img.Width * img.Height / _rect.Height);
                }
            }
            _document.Add(img);
        }
        #endregion

        #region 添加链接、点
        /// <summary>
        /// 添加链接
        /// </summary>
        /// <param name="content">链接文字</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="reference">链接地址</param>
        public void AddAnchorReference(string content, float fontSize, string reference)
        {
            SetFont(fontSize);
            var auc = new Anchor(content, _font);
            auc.Reference = reference;
            _document.Add(auc);
        }

        /// <summary>
        /// 添加链接点
        /// </summary>
        /// <param name="content">链接文字</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="name">链接点名</param>
        public void AddAnchorName(string content, float fontSize, string name)
        {
            SetFont(fontSize);
            var auc = new Anchor(content, _font);
            auc.Name = name;
            _document.Add(auc);
        }
        #endregion
    }
}