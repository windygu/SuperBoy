using System;
using System.Text;

namespace Core.Common
{
    /// <summary>
    /// 使用Random类生成伪随机数
    /// </summary>
    public class RandomHelper
    {

        #region 在指定的字符串随机出现
        /// <summary>
        /// 在指定的字符串随机出现
        /// </summary>
        /// <param name="pwdchars">字符串</param>
        /// <param name="pwdlen">长度</param>
        /// <returns></returns>
        public static string MakeRandomString(string pwdchars, int pwdlen)
        {
            var builder = new StringBuilder();
            var random = new Random();
            for (var i = 0; i < pwdlen; i++)
            {
                var num = random.Next(pwdchars.Length);
                builder.Append(pwdchars[num]);
            }
            return builder.ToString();
        }

        #endregion

        #region 返回不重复随机数数组
        /// <summary>
        /// 返回不重复随机数数组
        /// </summary>
        /// <param name="Num">随机数个数</param>
        /// <param name="minNum">随机数下限</param>
        /// <param name="maxNum">随机数上限</param>
        /// <returns></returns>
        public static int[] GetRandomArray(int number, int minNum, int maxNum)
        {
            int j;
            var b = new int[number];
            var r = new Random();
            for (j = 0; j < number; j++)
            {
                var i = r.Next(minNum, maxNum + 1);
                var num = 0;
                for (var k = 0; k < j; k++)
                {
                    if (b[k] == i)
                    {
                        num = num + 1;
                    }
                }
                if (num == 0)
                {
                    b[j] = i;
                }
                else
                {
                    j = j - 1;
                }
            }
            return b;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        private static int _rep = 0;
        /// <summary>
        /// 随机生成不重复数字字符串 
        /// </summary>
        /// <param name="codeCount">随机数长度</param>
        /// <returns></returns>
        public static string GenerateCheckCodeNum(int codeCount)
        {
            var str = string.Empty;
            var num2 = DateTime.Now.Ticks + _rep;
            _rep++;
            var random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> _rep)));
            for (var i = 0; i < codeCount; i++)
            {
                var num = random.Next();
                str = str + ((char)(0x30 + ((ushort)(num % 10)))).ToString();
            }
            return str;
        }


        /// <summary>
        /// 随机生成字符串（数字和字母混和）
        /// </summary>
        /// <param name="codeCount">随机数长度</param>
        /// <returns></returns>
        public static string GenerateCheckCode(int codeCount)
        {
            var str = string.Empty;
            var num2 = DateTime.Now.Ticks + _rep;
            _rep++;
            var random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> _rep)));
            for (var i = 0; i < codeCount; i++)
            {
                char ch;
                var num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            return str;
        }

    }
}
