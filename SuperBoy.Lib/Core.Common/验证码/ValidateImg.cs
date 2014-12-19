﻿using System;
using System.Drawing;

namespace Core.Common
{
    /// <summary>
    /// 验证码 继承 System.Web.UI.Page ，Session["xk_validate_code"]
    /// </summary>
    public class ValidateImg : System.Web.UI.Page
    {
        private void Page_Load(object sender, EventArgs e)
        {
            var chars = "023456789".ToCharArray();
            var random = new Random();

            var validateCode = string.Empty;
            for (var i = 0; i < 4; i++)
            {
                var rc = chars[random.Next(0, chars.Length)];
                if (validateCode.IndexOf(rc) > -1)
                {
                    i--;
                    continue;
                }
                validateCode += rc;
            }
            Session["xk_validate_code"] = validateCode;
            CreateImage(validateCode);
        }
        /// <summary>
        /// 创建图片
        /// </summary>
        /// <param name="checkCode"></param>
        private void CreateImage(string checkCode)
        {
            var iwidth = (int)(checkCode.Length * 11);
            var image = new System.Drawing.Bitmap(iwidth, 19);
            var g = Graphics.FromImage(image);
            g.Clear(Color.White);
            //定义颜色
            Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Chocolate, Color.Brown, Color.DarkCyan, Color.Purple };
            var rand = new Random();

            //输出不同字体和颜色的验证码字符
            for (var i = 0; i < checkCode.Length; i++)
            {
                var cindex = rand.Next(7);
                var f = new System.Drawing.Font("Microsoft Sans Serif", 11);
                Brush b = new System.Drawing.SolidBrush(c[cindex]);
                g.DrawString(checkCode.Substring(i, 1), f, b, (i * 10) + 1, 0, StringFormat.GenericDefault);
            }
            //画一个边框
            g.DrawRectangle(new Pen(Color.Black, 0), 0, 0, image.Width - 1, image.Height - 1);

            //输出到浏览器
            var ms = new System.IO.MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            Response.ClearContent();
            Response.ContentType = "image/Jpeg";
            Response.BinaryWrite(ms.ToArray());
            g.Dispose();
            image.Dispose();
        }
    }
}
