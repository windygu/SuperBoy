using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;

namespace Core.Common
{
    /// <summary>
    /// 用于验证码图片识别的类
    /// GrayByPixels(); //灰度处理
    /// GetPicValidByValue(128, 4); //得到有效空间
    /// Bitmap[] pics = GetSplitPics(4, 1);     //分割
    /// string code = GetSingleBmpCode(pics[i], 128);   //得到代码串
    /// </summary>
    public class UnCodebase
    {
        public Bitmap Bmpobj;

        public UnCodebase(Bitmap pic)
        {
            Bmpobj = new Bitmap(pic);    //转换为Format32bppRgb
        }

        public UnCodebase(string filename)
        {
            if (File.Exists(filename))
            {
                this.Bmpobj = (Bitmap)Image.FromFile(filename);
            }
        }

        /// <summary>
        /// 根据RGB，计算灰度值
        /// </summary>
        /// <param name="posClr">Color值</param>
        /// <returns>灰度值，整型</returns>
        private int GetGrayNumColor(System.Drawing.Color posClr)
        {
            return (posClr.R * 19595 + posClr.G * 38469 + posClr.B * 7472) >> 16;
        }

        /// <summary>
        /// 灰度转换,逐点方式
        /// </summary>
        public void GrayByPixels()
        {
            for (var i = 0; i < Bmpobj.Height; i++)
            {
                for (var j = 0; j < Bmpobj.Width; j++)
                {
                    var tmpValue = GetGrayNumColor(Bmpobj.GetPixel(j, i));
                    Bmpobj.SetPixel(j, i, Color.FromArgb(tmpValue, tmpValue, tmpValue));
                }
            }
        }

        /// <summary>
        /// 去图形边框
        /// </summary>
        /// <param name="borderWidth"></param>
        public void ClearPicBorder(int borderWidth)
        {
            for (var i = 0; i < Bmpobj.Height; i++)
            {
                for (var j = 0; j < Bmpobj.Width; j++)
                {
                    if (i < borderWidth || j < borderWidth || j > Bmpobj.Width - 1 - borderWidth || i > Bmpobj.Height - 1 - borderWidth)
                        Bmpobj.SetPixel(j, i, Color.FromArgb(255, 255, 255));
                }
            }
        }

        /// <summary>
        /// 灰度转换,逐行方式
        /// </summary>
        public void GrayByLine()
        {
            var rec = new Rectangle(0, 0, Bmpobj.Width, Bmpobj.Height);
            var bmpData = Bmpobj.LockBits(rec, ImageLockMode.ReadWrite, Bmpobj.PixelFormat);// PixelFormat.Format32bppPArgb);
            //    bmpData.PixelFormat = PixelFormat.Format24bppRgb;
            var scan0 = bmpData.Scan0;
            var len = Bmpobj.Width * Bmpobj.Height;
            var pixels = new int[len];
            Marshal.Copy(scan0, pixels, 0, len);

            //对图片进行处理
            var grayValue = 0;
            for (var i = 0; i < len; i++)
            {
                grayValue = GetGrayNumColor(Color.FromArgb(pixels[i]));
                pixels[i] = (byte)(Color.FromArgb(grayValue, grayValue, grayValue)).ToArgb();      //Color转byte
            }

            Bmpobj.UnlockBits(bmpData);
        }

        /// <summary>
        /// 得到有效图形并调整为可平均分割的大小
        /// </summary>
        /// <param name="dgGrayValue">灰度背景分界值</param>
        /// <param name="charsCount">有效字符数</param>
        /// <returns></returns>
        public void GetPicValidByValue(int dgGrayValue, int charsCount)
        {
            var posx1 = Bmpobj.Width; var posy1 = Bmpobj.Height;
            var posx2 = 0; var posy2 = 0;
            for (var i = 0; i < Bmpobj.Height; i++)      //找有效区
            {
                for (var j = 0; j < Bmpobj.Width; j++)
                {
                    int pixelValue = Bmpobj.GetPixel(j, i).R;
                    if (pixelValue < dgGrayValue)     //根据灰度值
                    {
                        if (posx1 > j) posx1 = j;
                        if (posy1 > i) posy1 = i;

                        if (posx2 < j) posx2 = j;
                        if (posy2 < i) posy2 = i;
                    };
                };
            };
            // 确保能整除
            var span = charsCount - (posx2 - posx1 + 1) % charsCount;   //可整除的差额数
            if (span < charsCount)
            {
                var leftSpan = span / 2;    //分配到左边的空列 ，如span为单数,则右边比左边大1
                if (posx1 > leftSpan)
                    posx1 = posx1 - leftSpan;
                if (posx2 + span - leftSpan < Bmpobj.Width)
                    posx2 = posx2 + span - leftSpan;
            }
            //复制新图
            var cloneRect = new Rectangle(posx1, posy1, posx2 - posx1 + 1, posy2 - posy1 + 1);
            Bmpobj = Bmpobj.Clone(cloneRect, Bmpobj.PixelFormat);
        }

        /// <summary>
        /// 得到有效图形,图形为类变量
        /// </summary>
        /// <param name="dgGrayValue">灰度背景分界值</param>
        /// <param name="CharsCount">有效字符数</param>
        /// <returns></returns>
        public void GetPicValidByValue(int dgGrayValue)
        {
            var posx1 = Bmpobj.Width; var posy1 = Bmpobj.Height;
            var posx2 = 0; var posy2 = 0;
            for (var i = 0; i < Bmpobj.Height; i++)      //找有效区
            {
                for (var j = 0; j < Bmpobj.Width; j++)
                {
                    int pixelValue = Bmpobj.GetPixel(j, i).R;
                    if (pixelValue < dgGrayValue)     //根据灰度值
                    {
                        if (posx1 > j) posx1 = j;
                        if (posy1 > i) posy1 = i;

                        if (posx2 < j) posx2 = j;
                        if (posy2 < i) posy2 = i;
                    };
                };
            };
            //复制新图
            var cloneRect = new Rectangle(posx1, posy1, posx2 - posx1 + 1, posy2 - posy1 + 1);
            Bmpobj = Bmpobj.Clone(cloneRect, Bmpobj.PixelFormat);
        }

        /// <summary>
        /// 得到有效图形,图形由外面传入
        /// </summary>
        /// <param name="dgGrayValue">灰度背景分界值</param>
        /// <param name="CharsCount">有效字符数</param>
        /// <returns></returns>
        public Bitmap GetPicValidByValue(Bitmap singlepic, int dgGrayValue)
        {
            var posx1 = singlepic.Width; var posy1 = singlepic.Height;
            var posx2 = 0; var posy2 = 0;
            for (var i = 0; i < singlepic.Height; i++)      //找有效区
            {
                for (var j = 0; j < singlepic.Width; j++)
                {
                    int pixelValue = singlepic.GetPixel(j, i).R;
                    if (pixelValue < dgGrayValue)     //根据灰度值
                    {
                        if (posx1 > j) posx1 = j;
                        if (posy1 > i) posy1 = i;

                        if (posx2 < j) posx2 = j;
                        if (posy2 < i) posy2 = i;
                    };
                };
            };
            //复制新图
            var cloneRect = new Rectangle(posx1, posy1, posx2 - posx1 + 1, posy2 - posy1 + 1);
            return singlepic.Clone(cloneRect, singlepic.PixelFormat);
        }

        /// <summary>
        /// 平均分割图片
        /// </summary>
        /// <param name="rowNum">水平上分割数</param>
        /// <param name="colNum">垂直上分割数</param>
        /// <returns>分割好的图片数组</returns>
        public Bitmap[] GetSplitPics(int rowNum, int colNum)
        {
            if (rowNum == 0 || colNum == 0)
                return null;
            var singW = Bmpobj.Width / rowNum;
            var singH = Bmpobj.Height / colNum;
            var picArray = new Bitmap[rowNum * colNum];

            Rectangle cloneRect;
            for (var i = 0; i < colNum; i++)      //找有效区
            {
                for (var j = 0; j < rowNum; j++)
                {
                    cloneRect = new Rectangle(j * singW, i * singH, singW, singH);
                    picArray[i * rowNum + j] = Bmpobj.Clone(cloneRect, Bmpobj.PixelFormat);//复制小块图
                }
            }
            return picArray;
        }

        /// <summary>
        /// 返回灰度图片的点阵描述字串，1表示灰点，0表示背景
        /// </summary>
        /// <param name="singlepic">灰度图</param>
        /// <param name="dgGrayValue">背前景灰色界限</param>
        /// <returns></returns>
        public string GetSingleBmpCode(Bitmap singlepic, int dgGrayValue)
        {
            Color piexl;
            var code = "";
            for (var posy = 0; posy < singlepic.Height; posy++)
                for (var posx = 0; posx < singlepic.Width; posx++)
                {
                    piexl = singlepic.GetPixel(posx, posy);
                    if (piexl.R < dgGrayValue)    // Color.Black )
                        code = code + "1";
                    else
                        code = code + "0";
                }
            return code;
        }


        #region MyRegion
        //public string getPicnum(List<UnCodeInfo> unCodeList)
        //{
        //    GrayByPixels(); //灰度处理
        //    GetPicValidByValue(128, 4); //得到有效空间
        //    Bitmap[] pics = GetSplitPics(4, 1);     //分割

        //    if (pics.Length != 4)
        //    {
        //        return ""; //分割错误
        //    }
        //    else  // 重新调整大小
        //    {
        //        pics[0] = GetPicValidByValue(pics[0], 128);
        //        pics[1] = GetPicValidByValue(pics[1], 128);
        //        pics[2] = GetPicValidByValue(pics[2], 128);
        //        pics[3] = GetPicValidByValue(pics[3], 128);
        //    }

        //    //      if (!textBoxInput.Text.Equals(""))
        //    string result = "";
        //    char singleChar = ' ';

        //    for (int i = 0; i < 4; i++)
        //    {
        //        string code = GetSingleBmpCode(pics[i], 128);   //得到代码串

        //        foreach (UnCodeInfo objUnCode in unCodeList)
        //        {
        //            if (objUnCode.ImgCode == code)
        //            {
        //                result += objUnCode.Code;
        //            }
        //        }
        //    }

        //    return result;
        //} 
        #endregion

    }
}
