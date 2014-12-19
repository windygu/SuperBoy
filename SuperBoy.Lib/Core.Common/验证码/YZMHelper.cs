using System;
using System.Web;
using System.Drawing;
using System.Security.Cryptography;

namespace Core.Common
{
    /// <summary>
    /// 验证码类
    /// </summary>
    public class Rand
    {
        #region 生成随机数字
        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="length">生成长度</param>
        public static string Number(int length)
        {
            return Number(length, false);
        }

        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="length">生成长度</param>
        /// <param name="sleep">是否要在生成前将当前线程阻止以避免重复</param>
        public static string Number(int length, bool sleep)
        {
            if (sleep) System.Threading.Thread.Sleep(3);
            var result = "";
            var random = new Random();
            for (var i = 0; i < length; i++)
            {
                result += random.Next(10).ToString();
            }
            return result;
        }
        #endregion

        #region 生成随机字母与数字
        /// <summary>
        /// 生成随机字母与数字
        /// </summary>
        /// <param name="IntStr">生成长度</param>
        public static string Str(int length)
        {
            return Str(length, false);
        }

        /// <summary>
        /// 生成随机字母与数字
        /// </summary>
        /// <param name="length">生成长度</param>
        /// <param name="sleep">是否要在生成前将当前线程阻止以避免重复</param>
        public static string Str(int length, bool sleep)
        {
            if (sleep) System.Threading.Thread.Sleep(3);
            var pattern = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            var result = "";
            var n = pattern.Length;
            var random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (var i = 0; i < length; i++)
            {
                var rnd = random.Next(0, n);
                result += pattern[rnd];
            }
            return result;
        }
        #endregion

        #region 生成随机纯字母随机数
        /// <summary>
        /// 生成随机纯字母随机数
        /// </summary>
        /// <param name="IntStr">生成长度</param>
        public static string Str_char(int length)
        {
            return Str_char(length, false);
        }

        /// <summary>
        /// 生成随机纯字母随机数
        /// </summary>
        /// <param name="length">生成长度</param>
        /// <param name="sleep">是否要在生成前将当前线程阻止以避免重复</param>
        public static string Str_char(int length, bool sleep)
        {
            if (sleep) System.Threading.Thread.Sleep(3);
            var pattern = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            var result = "";
            var n = pattern.Length;
            var random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (var i = 0; i < length; i++)
            {
                var rnd = random.Next(0, n);
                result += pattern[rnd];
            }
            return result;
        }
        #endregion
    }

    /// <summary>
    /// 验证图片类
    /// </summary>
    public class YzmHelper
    {
        #region 私有字段
        private string _text;
        private Bitmap _image;
        private int _letterCount = 4;   //验证码位数
        private int _letterWidth = 16;  //单个字体的宽度范围
        private int _letterHeight = 20; //单个字体的高度范围
        private static byte[] _randb = new byte[4];
        private static RNGCryptoServiceProvider _rand = new RNGCryptoServiceProvider();
        private Font[] _fonts = 
    {
       new Font(new FontFamily("Times New Roman"),10 +Next(1),System.Drawing.FontStyle.Regular),
       new Font(new FontFamily("Georgia"), 10 + Next(1),System.Drawing.FontStyle.Regular),
       new Font(new FontFamily("Arial"), 10 + Next(1),System.Drawing.FontStyle.Regular),
       new Font(new FontFamily("Comic Sans MS"), 10 + Next(1),System.Drawing.FontStyle.Regular)
    };
        #endregion

        #region 公有属性
        /// <summary>
        /// 验证码
        /// </summary>
        public string Text
        {
            get { return this._text; }
        }

        /// <summary>
        /// 验证码图片
        /// </summary>
        public Bitmap Image
        {
            get { return this._image; }
        }
        #endregion

        #region 构造函数
        public YzmHelper()
        {
            HttpContext.Current.Response.Expires = 0;
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1);
            HttpContext.Current.Response.AddHeader("pragma", "no-cache");
            HttpContext.Current.Response.CacheControl = "no-cache";
            this._text = Rand.Number(4);
            CreateImage();
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获得下一个随机数
        /// </summary>
        /// <param name="max">最大值</param>
        private static int Next(int max)
        {
            _rand.GetBytes(_randb);
            var value = BitConverter.ToInt32(_randb, 0);
            value = value % (max + 1);
            if (value < 0) value = -value;
            return value;
        }

        /// <summary>
        /// 获得下一个随机数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        private static int Next(int min, int max)
        {
            var value = Next(max - min) + min;
            return value;
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 绘制验证码
        /// </summary>
        public void CreateImage()
        {
            var intImageWidth = this._text.Length * _letterWidth;
            var image = new Bitmap(intImageWidth, _letterHeight);
            var g = Graphics.FromImage(image);
            g.Clear(Color.White);
            for (var i = 0; i < 2; i++)
            {
                var x1 = Next(image.Width - 1);
                var x2 = Next(image.Width - 1);
                var y1 = Next(image.Height - 1);
                var y2 = Next(image.Height - 1);
                g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
            }
            int _x = -12, _y = 0;
            for (var intIndex = 0; intIndex < this._text.Length; intIndex++)
            {
                _x += Next(12, 16);
                _y = Next(-2, 2);
                var strChar = this._text.Substring(intIndex, 1);
                strChar = Next(1) == 1 ? strChar.ToLower() : strChar.ToUpper();
                Brush newBrush = new SolidBrush(GetRandomColor());
                var thePos = new Point(_x, _y);
                g.DrawString(strChar, _fonts[Next(_fonts.Length - 1)], newBrush, thePos);
            }
            for (var i = 0; i < 10; i++)
            {
                var x = Next(image.Width - 1);
                var y = Next(image.Height - 1);
                image.SetPixel(x, y, Color.FromArgb(Next(0, 255), Next(0, 255), Next(0, 255)));
            }
            image = TwistImage(image, true, Next(1, 3), Next(4, 6));
            g.DrawRectangle(new Pen(Color.LightGray, 1), 0, 0, intImageWidth - 1, (_letterHeight - 1));
            this._image = image;
        }

        /// <summary>
        /// 字体随机颜色
        /// </summary>
        public Color GetRandomColor()
        {
            var randomNumFirst = new Random((int)DateTime.Now.Ticks);
            System.Threading.Thread.Sleep(randomNumFirst.Next(50));
            var randomNumSencond = new Random((int)DateTime.Now.Ticks);
            var intRed = randomNumFirst.Next(180);
            var intGreen = randomNumSencond.Next(180);
            var intBlue = (intRed + intGreen > 300) ? 0 : 400 - intRed - intGreen;
            intBlue = (intBlue > 255) ? 255 : intBlue;
            return Color.FromArgb(intRed, intGreen, intBlue);
        }

        /// <summary>
        /// 正弦曲线Wave扭曲图片
        /// </summary>
        /// <param name="srcBmp">图片路径</param>
        /// <param name="bXDir">如果扭曲则选择为True</param>
        /// <param name="nMultValue">波形的幅度倍数，越大扭曲的程度越高,一般为3</param>
        /// <param name="dPhase">波形的起始相位,取值区间[0-2*PI)</param>
        public System.Drawing.Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        {
            var pi = 6.283185307179586476925286766559;
            var destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);
            var graph = Graphics.FromImage(destBmp);
            graph.FillRectangle(new SolidBrush(Color.White), 0, 0, destBmp.Width, destBmp.Height);
            graph.Dispose();
            var dBaseAxisLen = bXDir ? (double)destBmp.Height : (double)destBmp.Width;
            for (var i = 0; i < destBmp.Width; i++)
            {
                for (var j = 0; j < destBmp.Height; j++)
                {
                    double dx = 0;
                    dx = bXDir ? (pi * (double)j) / dBaseAxisLen : (pi * (double)i) / dBaseAxisLen;
                    dx += dPhase;
                    var dy = Math.Sin(dx);
                    int nOldX = 0, nOldY = 0;
                    nOldX = bXDir ? i + (int)(dy * dMultValue) : i;
                    nOldY = bXDir ? j : j + (int)(dy * dMultValue);

                    var color = srcBmp.GetPixel(i, j);
                    if (nOldX >= 0 && nOldX < destBmp.Width
                     && nOldY >= 0 && nOldY < destBmp.Height)
                    {
                        destBmp.SetPixel(nOldX, nOldY, color);
                    }
                }
            }
            srcBmp.Dispose();
            return destBmp;
        }
        #endregion
    }
}