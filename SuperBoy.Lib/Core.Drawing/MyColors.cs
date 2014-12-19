using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Core.Drawing
{
    #region Public Structs


    public struct MyColor
    {
        public Rgb Rgb;
        public Hsb Hsb;
        public Cmyk Cmyk;

        public MyColor(Color color)
        {
            this.Rgb = color;
            this.Hsb = color;
            this.Cmyk = color;
        }

        public static implicit operator MyColor(Color color)
        {
            return new MyColor(color);
        }

        public static implicit operator Color(MyColor color)
        {
            return color.Rgb;
        }

        public static bool operator ==(MyColor left, MyColor right)
        {
            return (left.Rgb == right.Rgb) && (left.Hsb == right.Hsb) && (left.Cmyk == right.Cmyk);
        }

        public static bool operator !=(MyColor left, MyColor right)
        {
            return !(left == right);
        }

        public void RgbUpdate()
        {
            this.Hsb = this.Rgb;
            this.Cmyk = this.Rgb;
        }

        public void HsbUpdate()
        {
            this.Rgb = this.Hsb;
            this.Cmyk = this.Hsb;
        }

        public void CmykUpdate()
        {
            this.Rgb = this.Cmyk;
            this.Hsb = this.Cmyk;
        }

        public override string ToString()
        {
            return String.Format("{0}\r\n{1}\r\n{2}", Rgb, Hsb, Cmyk);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }

    public struct Rgb
    {
        private int _red;
        private int _green;
        private int _blue;

        public int Red
        {
            get { return _red; }
            set { _red = ColorHelper.CheckColor(value); }
        }

        public int Green
        {
            get { return _green; }
            set { _green = ColorHelper.CheckColor(value); }
        }

        public int Blue
        {
            get { return _blue; }
            set { _blue = ColorHelper.CheckColor(value); }
        }

        public Rgb(int red, int green, int blue)
            : this()
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
        }

        public Rgb(Color color)
        {
            this = new Rgb(color.R, color.G, color.B);
        }

        public static implicit operator Rgb(Color color)
        {
            return new Rgb(color.R, color.G, color.B);
        }

        public static implicit operator Color(Rgb color)
        {
            return color.ToColor();
        }

        public static implicit operator Hsb(Rgb color)
        {
            return color.ToHSB();
        }

        public static implicit operator Cmyk(Rgb color)
        {
            return color.ToCMYK();
        }

        public static bool operator ==(Rgb left, Rgb right)
        {
            return (left.Red == right.Red) && (left.Green == right.Green) && (left.Blue == right.Blue);
        }

        public static bool operator !=(Rgb left, Rgb right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return String.Format("Red: {0}, Green: {1}, Blue: {2}", Red, Green, Blue);
        }

        public static Color ToColor(int red, int green, int blue)
        {
            return Color.FromArgb(red, green, blue);
        }

        public Color ToColor()
        {
            return ToColor(Red, Green, Blue);
        }

        public static Hsb ToHSB(Color color)
        {
            var hsb = new Hsb();

            int max, min;

            if (color.R > color.G) { max = color.R; min = color.G; }
            else { max = color.G; min = color.R; }
            if (color.B > max) max = color.B;
            else if (color.B < min) min = color.B;

            var diff = max - min;

            hsb.Brightness = (double)max / 255;

            if (max == 0) hsb.Saturation = 0;
            else hsb.Saturation = (double)diff / max;

            double q;
            if (diff == 0) q = 0;
            else q = (double)60 / diff;

            if (max == color.R)
            {
                if (color.G < color.B) hsb.Hue = (360 + q * (color.G - color.B)) / 360;
                else hsb.Hue = q * (color.G - color.B) / 360;
            }
            else if (max == color.G) hsb.Hue = (120 + q * (color.B - color.R)) / 360;
            else if (max == color.B) hsb.Hue = (240 + q * (color.R - color.G)) / 360;
            else hsb.Hue = 0.0;

            return hsb;
        }

        public Hsb ToHSB()
        {
            return ToHSB(this);
        }

        public static Cmyk ToCMYK(Color color)
        {
            var cmyk = new Cmyk();
            var low = 1.0;

            cmyk.Cyan = (double)(255 - color.R) / 255;
            if (low > cmyk.Cyan)
                low = cmyk.Cyan;

            cmyk.Magenta = (double)(255 - color.G) / 255;
            if (low > cmyk.Magenta)
                low = cmyk.Magenta;

            cmyk.Yellow = (double)(255 - color.B) / 255;
            if (low > cmyk.Yellow)
                low = cmyk.Yellow;

            if (low > 0.0)
            {
                cmyk.Key = low;
            }

            return cmyk;
        }

        public Cmyk ToCMYK()
        {
            return ToCMYK(this);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }

    public struct Hsb
    {
        private double _hue;
        private double _saturation;
        private double _brightness;

        public double Hue
        {
            get { return _hue; }
            set { _hue = ColorHelper.CheckColor(value); }
        }

        public double Hue360
        {
            get { return _hue * 360; }
            set { _hue = ColorHelper.CheckColor(value / 360); }
        }

        public double Saturation
        {
            get { return _saturation; }
            set { _saturation = ColorHelper.CheckColor(value); }
        }

        public double Saturation100
        {
            get { return _saturation * 100; }
            set { _saturation = ColorHelper.CheckColor(value / 100); }
        }

        public double Brightness
        {
            get { return _brightness; }
            set { _brightness = ColorHelper.CheckColor(value); }
        }

        public double Brightness100
        {
            get { return _brightness * 100; }
            set { _brightness = ColorHelper.CheckColor(value / 100); }
        }

        public Hsb(double hue, double saturation, double brightness)
            : this()
        {
            this.Hue = hue;
            this.Saturation = saturation;
            this.Brightness = brightness;
        }

        public Hsb(int hue, int saturation, int brightness)
            : this()
        {
            this.Hue360 = hue;
            this.Saturation100 = saturation;
            this.Brightness100 = brightness;
        }

        public Hsb(Color color)
        {
            this = Rgb.ToHSB(color);
        }

        public static implicit operator Hsb(Color color)
        {
            return Rgb.ToHSB(color);
        }

        public static implicit operator Color(Hsb color)
        {
            return color.ToColor();
        }

        public static implicit operator Rgb(Hsb color)
        {
            return color.ToColor();
        }

        public static implicit operator Cmyk(Hsb color)
        {
            return color.ToColor();
        }

        public static bool operator ==(Hsb left, Hsb right)
        {
            return (left.Hue == right.Hue) && (left.Saturation == right.Saturation) && (left.Brightness == right.Brightness);
        }

        public static bool operator !=(Hsb left, Hsb right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return String.Format("Hue: {0}, Saturation: {1}, Brightness: {2}", ColorHelper.Round(Hue360),
              ColorHelper.Round(Saturation100), ColorHelper.Round(Brightness100));
        }

        public static Color ToColor(Hsb hsb)
        {
            int mid;

            var max = ColorHelper.Round(hsb.Brightness * 255);
            var min = ColorHelper.Round((1.0 - hsb.Saturation) * (hsb.Brightness / 1.0) * 255);
            var q = (double)(max - min) / 255;

            if (hsb.Hue >= 0 && hsb.Hue <= (double)1 / 6)
            {
                mid = ColorHelper.Round(((hsb.Hue - 0) * q) * 1530 + min);
                return Color.FromArgb(max, mid, min);
            }
            if (hsb.Hue <= (double)1 / 3)
            {
                mid = ColorHelper.Round(-((hsb.Hue - (double)1 / 6) * q) * 1530 + max);
                return Color.FromArgb(mid, max, min);
            }
            if (hsb.Hue <= 0.5)
            {
                mid = ColorHelper.Round(((hsb.Hue - (double)1 / 3) * q) * 1530 + min);
                return Color.FromArgb(min, max, mid);
            }
            if (hsb.Hue <= (double)2 / 3)
            {
                mid = ColorHelper.Round(-((hsb.Hue - 0.5) * q) * 1530 + max);
                return Color.FromArgb(min, mid, max);
            }
            if (hsb.Hue <= (double)5 / 6)
            {
                mid = ColorHelper.Round(((hsb.Hue - (double)2 / 3) * q) * 1530 + min);
                return Color.FromArgb(mid, min, max);
            }
            if (hsb.Hue <= 1.0)
            {
                mid = ColorHelper.Round(-((hsb.Hue - (double)5 / 6) * q) * 1530 + max);
                return Color.FromArgb(max, min, mid);
            }
            return Color.FromArgb(0, 0, 0);
        }

        public static Color ToColor(double hue, double saturation, double brightness)
        {
            return ToColor(new Hsb(hue, saturation, brightness));
        }

        public Color ToColor()
        {
            return ToColor(this);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }

    public struct Cmyk
    {
        private double _cyan;
        private double _magenta;
        private double _yellow;
        private double _key;

        public double Cyan
        {
            get { return _cyan; }
            set { _cyan = ColorHelper.CheckColor(value); }
        }

        public double Cyan100
        {
            get { return _cyan * 100; }
            set { _cyan = ColorHelper.CheckColor(value / 100); }
        }

        public double Magenta
        {
            get { return _magenta; }
            set { _magenta = ColorHelper.CheckColor(value); }
        }

        public double Magenta100
        {
            get { return _magenta * 100; }
            set { _magenta = ColorHelper.CheckColor(value / 100); }
        }

        public double Yellow
        {
            get { return _yellow; }
            set { _yellow = ColorHelper.CheckColor(value); }
        }

        public double Yellow100
        {
            get { return _yellow * 100; }
            set { _yellow = ColorHelper.CheckColor(value / 100); }
        }

        public double Key
        {
            get { return _key; }
            set { _key = ColorHelper.CheckColor(value); }
        }

        public double Key100
        {
            get { return _key * 100; }
            set { _key = ColorHelper.CheckColor(value / 100); }
        }

        public Cmyk(double cyan, double magenta, double yellow, double key)
            : this()
        {
            this.Cyan = cyan;
            this.Magenta = magenta;
            this.Yellow = yellow;
            this.Key = key;
        }

        public Cmyk(int cyan, int magenta, int yellow, int key)
            : this()
        {
            this.Cyan100 = cyan;
            this.Magenta100 = magenta;
            this.Yellow100 = yellow;
            this.Key100 = key;
        }

        public Cmyk(Color color)
        {
            this = Rgb.ToCMYK(color);
        }

        public static implicit operator Cmyk(Color color)
        {
            return Rgb.ToCMYK(color);
        }

        public static implicit operator Color(Cmyk color)
        {
            return color.ToColor();
        }

        public static implicit operator Rgb(Cmyk color)
        {
            return color.ToColor();
        }

        public static implicit operator Hsb(Cmyk color)
        {
            return color.ToColor();
        }

        public static bool operator ==(Cmyk left, Cmyk right)
        {
            return (left.Cyan == right.Cyan) && (left.Magenta == right.Magenta) && (left.Yellow == right.Yellow) &&
                (left.Key == right.Key);
        }

        public static bool operator !=(Cmyk left, Cmyk right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return String.Format("Cyan: {0}, Magenta: {1}, Yellow: {2}, Key: {3}", ColorHelper.Round(Cyan100),
              ColorHelper.Round(Magenta100), ColorHelper.Round(Yellow100), ColorHelper.Round(Key100));
        }

        public static Color ToColor(Cmyk cmyk)
        {
            var red = ColorHelper.Round(255 - (255 * cmyk.Cyan));
            var green = ColorHelper.Round(255 - (255 * cmyk.Magenta));
            var blue = ColorHelper.Round(255 - (255 * cmyk.Yellow));

            return Color.FromArgb(red, green, blue);
        }

        public static Color ToColor(double cyan, double magenta, double yellow, double key)
        {
            return ToColor(new Cmyk(cyan, magenta, yellow, key));
        }

        public Color ToColor()
        {
            return ToColor(this);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }

    #endregion

    internal class ColorHelper
    {
        #region CheckColorHelper
        public static double CheckColor(double number)
        {
            return GetBetween(number, 0, 1);
        }

        public static int CheckColor(int number)
        {
            return GetBetween(number, 0, 255);
        }

        public static int GetBetween(int number, int min, int max)
        {
            return Math.Max(Math.Min(number, max), min);
        }

        public static double GetBetween(double value, double min, double max)
        {
            return Math.Max(Math.Min(value, max), min);
        }

        public static int Round(double val)
        {
            var retVal = (int)val;

            var temp = (int)(val * 100);

            if ((temp % 100) >= 50)
                retVal += 1;

            return retVal;
        }
        #endregion
    }

    /// <summary>
    /// RGB颜色操作辅助类
    /// </summary>
    public static class MyColors
    {
        public static Color ParseColor(string color)
        {
            if (color.StartsWith("#"))
            {
                return MyColors.HexToColor(color);
            }
            else if (color.Contains(","))
            {
                var colorArray = color.Split(',');
                var colorList = new List<int>();
                foreach (var strColor in colorArray)
                {
                    colorList.Add(Convert.ToInt32(strColor));
                }
                var colors = colorList.ToArray();

                if (colors.Length == 3)
                {
                    return Color.FromArgb(colors[0], colors[1], colors[2]);
                }
                if (colors.Length == 4)
                {
                    return Color.FromArgb(colors[0], colors[1], colors[2], colors[3]);
                }
            }

            return Color.FromName(color);
        }

        public static string ColorToHex(Color color)
        {
            return string.Format("{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
        }

        public static int ColorToDecimal(Color color)
        {
            return HexToDecimal(ColorToHex(color));
        }

        public static Color HexToColor(string hex)
        {
            if (hex.StartsWith("#"))
            {
                hex = hex.Substring(1);
            }

            var a = string.Empty;

            if (hex.Length == 8)
            {
                a = hex.Substring(0, 2);
                hex = hex.Substring(2);
            }

            var r = hex.Substring(0, 2);
            var g = hex.Substring(2, 2);
            var b = hex.Substring(4, 2);

            if (string.IsNullOrEmpty(a))
            {
                return Color.FromArgb(HexToDecimal(r), HexToDecimal(g), HexToDecimal(b));
            }
            else
            {
                return Color.FromArgb(HexToDecimal(a), HexToDecimal(r), HexToDecimal(g), HexToDecimal(b));
            }
        }

        public static int HexToDecimal(string hex)
        {
            //return int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return Convert.ToInt32(hex, 16);
        }

        public static string DecimalToHex(int dec)
        {
            return dec.ToString("X6");
        }

        public static Color DecimalToColor(int dec)
        {
            return Color.FromArgb(dec & 0xFF, (dec & 0xff00) / 256, dec / 65536);
        }

        public static Color GetPixelColor(Point point)
        {
            var bmp = new Bitmap(1, 1);
            Graphics.FromImage(bmp).CopyFromScreen(point, new Point(0, 0), new Size(1, 1));
            return bmp.GetPixel(0, 0);
        }

        public static Color SetHue(Color c, double hue)
        {
            var hsb = Rgb.ToHSB(c);
            hsb.Hue = hue;
            return hsb.ToColor();
        }

        public static Color ModifyHue(Color c, double hue)
        {
            var hsb = Rgb.ToHSB(c);
            hsb.Hue *= hue;
            return hsb.ToColor();
        }

        public static Color SetSaturation(Color c, double saturation)
        {
            var hsb = Rgb.ToHSB(c);
            hsb.Saturation = saturation;
            return hsb.ToColor();
        }

        public static Color ModifySaturation(Color c, double saturation)
        {
            var hsb = Rgb.ToHSB(c);
            hsb.Saturation *= saturation;
            return hsb.ToColor();
        }

        public static Color SetBrightness(Color c, double brightness)
        {
            var hsb = Rgb.ToHSB(c);
            hsb.Brightness = brightness;
            return hsb.ToColor();
        }

        public static Color ModifyBrightness(Color c, double brightness)
        {
            var hsb = Rgb.ToHSB(c);
            hsb.Brightness *= brightness;
            return hsb.ToColor();
        }

        public static Color RandomColor()
        {
            var rand = new Random();
            return Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
        }
    }
}
