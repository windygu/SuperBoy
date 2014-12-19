using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Common
{
    /// <summary>
    /// 随机汉字辅助类
    /// </summary>
    public class RandomChinese
    {
        /// <summary>
        /// 可以随机生成一个长度为2的十六进制字节数组，
        /// 使用GetString ()方法对其进行解码就可以得到汉字字符了。
        /// 不过对于生成中文汉字验证码来说，因为第15区也就是AF区以前都没有汉字，
        /// 只有少量符号，汉字都从第16区B0开始，并且从区位D7开始以后的汉字都是和很难见到的繁杂汉字，
        /// 所以这些都要排出掉。所以随机生成的汉字十六进制区位码第1位范围在B、C、D之间，
        /// 如果第1位是D的话，第2位区位码就不能是7以后的十六进制数。
        /// 在来看看区位码表发现每区的第一个位置和最后一个位置都是空的，没有汉字，
        /// 因此随机生成的区位码第3位如果是A的话，第4位就不能是0；第3位如果是F的话，
        /// 第4位就不能是F。知道了原理，随机生成中文汉字的程序也就出来了，
        /// < src="http://code.xrss.cn/AdJs/csdntitle.Js">
        /// 以下就是生成长度为N的随机汉字C#台代码：
        /// </summary> 
        public static string GetRandomChinese(int strlength)
        {
            // 获取GB2312编码页（表） 
            var gb = Encoding.GetEncoding("gb2312");

            var bytes = CreateRegionCode(strlength);

            var sb = new StringBuilder();

            for (var i = 0; i < strlength; i++)
            {
                var temp = gb.GetString((byte[])Convert.ChangeType(bytes[i], typeof(byte[])));
                sb.Append(temp);
            }

            return sb.ToString();
        }

        /** 
        此函数在汉字编码范围内随机创建含两个元素的十六进制字节数组，每个字节数组代表一个汉字，并将 
        四个字节数组存储在object数组中。 
        参数：strlength，代表需要产生的汉字个数 
        **/
        private static object[] CreateRegionCode(int strlength)
        {
            //定义一个字符串数组储存汉字编码的组成元素 
            var rBase = new String[16] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };

            var rnd = new Random();

            //定义一个object数组用来 
            var bytes = new object[strlength];

            /**
             每循环一次产生一个含两个元素的十六进制字节数组，并将其放入bytes数组中 
             每个汉字有四个区位码组成 
             区位码第1位和区位码第2位作为字节数组第一个元素 
             区位码第3位和区位码第4位作为字节数组第二个元素 
            **/
            for (var i = 0; i < strlength; i++)
            {
                //区位码第1位 
                var r1 = rnd.Next(11, 14);
                var strR1 = rBase[r1].Trim();

                //区位码第2位 
                rnd = new Random(r1 * unchecked((int)DateTime.Now.Ticks) + i); // 更换随机数发生器的 种子避免产生重复值 
                int r2;
                if (r1 == 13)
                {
                    r2 = rnd.Next(0, 7);
                }
                else
                {
                    r2 = rnd.Next(0, 16);
                }
                var strR2 = rBase[r2].Trim();

                //区位码第3位 
                rnd = new Random(r2 * unchecked((int)DateTime.Now.Ticks) + i);
                var r3 = rnd.Next(10, 16);
                var strR3 = rBase[r3].Trim();

                //区位码第4位 
                rnd = new Random(r3 * unchecked((int)DateTime.Now.Ticks) + i);
                int r4;
                if (r3 == 10)
                {
                    r4 = rnd.Next(1, 16);
                }
                else if (r3 == 15)
                {
                    r4 = rnd.Next(0, 15);
                }
                else
                {
                    r4 = rnd.Next(0, 16);
                }
                var strR4 = rBase[r4].Trim();

                // 定义两个字节变量存储产生的随机汉字区位码 
                var byte1 = Convert.ToByte(strR1 + strR2, 16);
                var byte2 = Convert.ToByte(strR3 + strR4, 16);
                // 将两个字节变量存储在字节数组中 
                var strR = new byte[] { byte1, byte2 };

                // 将产生的一个汉字的字节数组放入object数组中 
                bytes.SetValue(strR, i);
            }

            return bytes;
        }

        /// <summary>
        /// <param name="length">字符串长度</param>
        /// <param name="seed">随机函数种子值</param>
        /// <returns>指定长度的随机字符串</returns>
        /// </summary> 
        public static string GetRandomChars(int length, params int[] seed)
        {
            var strSep = ",";
            var chrSep = strSep.ToCharArray();
            //这里定义字符集
            var strChar = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z"
             + ",A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,W,X,Y,Z";

            var aryChar = strChar.Split(chrSep, strChar.Length);

            var strRandom = string.Empty;
            Random rnd;
            if (seed != null && seed.Length > 0)
            {
                rnd = new Random(seed[0]);
            }
            else
            {
                rnd = new Random();
            }

            //生成随机字符串
            for (var i = 0; i < length; i++)
            {
                strRandom += aryChar[rnd.Next(aryChar.Length)];
            }

            return strRandom;
        }

        /// <summary>
        /// 此函数为生成指定数目的汉字
        /// </summary>
        /// <param name="charLen">汉字数目</param>
        /// <returns>所有汉字</returns>
        public static string GetRandomChinese2(int strlength)
        {
            int area, code;//汉字由区位和码位组成(都为0-94,其中区位16-55为一级汉字区,56-87为二级汉字区,1-9为特殊字符区)
            var charArrary = new string[strlength];
            var rand = new Random();
            for (var i = 0; i < strlength; i++)
            {
                area = rand.Next(16, 88);
                if (area == 55)//第55区只有89个字符
                {
                    code = rand.Next(1, 90);
                }
                else
                {
                    code = rand.Next(1, 94);
                }
                charArrary[i] = Encoding.GetEncoding("GB2312").GetString(new byte[] { Convert.ToByte(area + 160), Convert.ToByte(code + 160) });
            }

            var sb = new StringBuilder();
            for (var i = 0; i < charArrary.Length; i++)
            {
                sb.Append(charArrary[i]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="length">生成长度</param>
        /// <returns></returns>
        public static string GetRandomNumber(int length)
        {
            return GetRandomNumber(length, false);
        }

        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="length">生成长度</param>
        /// <param name="sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns></returns>
        public static string GetRandomNumber(int length, bool sleep)
        {
            if (sleep)
                System.Threading.Thread.Sleep(3);
            var result = "";
            var random = new Random();
            for (var i = 0; i < length; i++)
            {
                result += random.Next(10).ToString();
            }
            return result;
        }

        /// <summary>
        /// 生成随机纯字母随机数
        /// </summary>
        /// <param name="IntStr">生成长度</param>
        /// <returns></returns>
        public static string GetRandomPureChar(int length)
        {
            return GetRandomPureChar(length, false);
        }

        /// <summary>
        /// 生成随机纯字母随机数
        /// </summary>
        /// <param name="length">生成长度</param>
        /// <param name="sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns></returns>
        public static string GetRandomPureChar(int length, bool sleep)
        {
            if (sleep)
                System.Threading.Thread.Sleep(3);
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

        #region 生成随机数字字符串
        /// <summary>
        /// 生成随机数字字符串
        /// </summary>
        /// <param name="int_NumberLength">数字长度</param>
        /// <returns></returns>
        public static string GetRandomNumberString(int intNumberLength, bool onlyNumber, string _randTag)
        {
            var randTag = string.IsNullOrEmpty(_randTag) ? Guid.NewGuid().ToString() : _randTag;
            var random = new Random();
            return GetRandomNumberString(intNumberLength, onlyNumber, random, randTag);
        }
        /// <summary>
        /// 生成随机数字字符串
        /// </summary>
        /// <param name="int_NumberLength">数字长度</param>
        /// <returns></returns>
        public static string GetRandomNumberString(int intNumberLength, bool onlyNumber, Random random, string randTag)
        {
            var strings = "123456789";
            if (!onlyNumber) strings += "abcdefghjkmnpqrstuvwxyz";
            var chars = strings.ToCharArray();
            var returnCode = string.Empty;
            for (var i = 0; i < intNumberLength; i++)
                returnCode += chars[random.Next(0, chars.Length)].ToString();
            return returnCode;
        }
        /// <summary>
        /// 生成产品订单号，全站统一格式(年月日时分秒+4位随机数)
        /// </summary>
        /// <returns></returns>
        public static string GetProductOrderNum()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss") + GetRandomNumberString(4, true, Guid.NewGuid().ToString());
        }
        /// <summary>
        /// 产生随机数字字符串
        /// </summary>
        /// <returns></returns>
        public static string RandomStr(int num)
        {
            int number;
            char code;
            var returnCode = String.Empty;

            var random = new Random();

            for (var i = 0; i < num; i++)
            {
                number = random.Next();
                code = (char)('0' + (char)(number % 10));
                returnCode += code.ToString();
            }
            return returnCode;
        }
        #endregion
    }
}
