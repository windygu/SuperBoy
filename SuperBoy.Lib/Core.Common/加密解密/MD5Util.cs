using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Commons
{
    /// <summary>
    /// MD5各种长度加密字符、验证MD5等操作辅助类
    /// </summary>
    public class Md5Util
    {
        public Md5Util()
        {
        }

        /// <summary>
        /// 获得32位的MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5_32(string input)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            var data = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(input));
            var sb = new StringBuilder();
            for (var i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获得16位的MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5_16(string input)
        {
            return GetMD5_32(input).Substring(8, 16);
        }

        /// <summary>
        /// 获得8位的MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5_8(string input)
        {
            return GetMD5_32(input).Substring(8, 8);
        }

        /// <summary>
        /// 获得4位的MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMD5_4(string input)
        {
            return GetMD5_32(input).Substring(8, 4);
        }

        /// <summary>
        /// 添加MD5的前缀，便于检查有无篡改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string AddMd5Profix(string input)
        {
            return GetMD5_4(input) + input;
        }

        /// <summary>
        /// 移除MD5的前缀
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveMd5Profix(string input)
        {
            return input.Substring(4);
        }

        /// <summary>
        /// 验证MD5前缀处理的字符串有无被篡改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool ValidateValue(string input)
        {
            var res = false;
            if (input.Length >= 4)
            {
                var tmp = input.Substring(4);
                if (input.Substring(0, 4) == GetMD5_4(tmp))
                {
                    res = true;
                }
            }
            return res;
        }

        #region MD5签名验证
        /// <summary>
        /// 对给定文件路径的文件加上标签
        /// </summary>
        /// <param name="path">要加密的文件的路径</param>
        /// <returns>标签的值</returns>
        public static bool AddMd5(string path)
        {
            var isNeed = true;

            if (CheckMd5(path))                                  //已进行MD5处理
                isNeed = false;

            try
            {
                var fsread = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                var md5File = new byte[fsread.Length];
                fsread.Read(md5File, 0, (int)fsread.Length);                               // 将文件流读取到Buffer中
                fsread.Close();

                if (isNeed)
                {
                    var result = Md5Buffer(md5File, 0, md5File.Length);             // 对Buffer中的字节内容算MD5
                    var md5 = System.Text.Encoding.ASCII.GetBytes(result);       // 将字符串转换成字节数组以便写人到文件中
                    var fsWrite = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                    fsWrite.Write(md5File, 0, md5File.Length);                               // 将文件，MD5值 重新写入到文件中。
                    fsWrite.Write(md5, 0, md5.Length);
                    fsWrite.Close();
                }
                else
                {
                    var fsWrite = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                    fsWrite.Write(md5File, 0, md5File.Length);
                    fsWrite.Close();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 对给定路径的文件进行验证
        /// </summary>
        /// <param name="path"></param>
        /// <returns>是否加了标签或是否标签值与内容值一致</returns>
        public static bool CheckMd5(string path)
        {
            try
            {
                var getFile = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                var md5File = new byte[getFile.Length];                                      // 读入文件
                getFile.Read(md5File, 0, (int)getFile.Length);
                getFile.Close();

                var result = Md5Buffer(md5File, 0, md5File.Length - 32);             // 对文件除最后32位以外的字节计算MD5，这个32是因为标签位为32位。
                var md5 = System.Text.Encoding.ASCII.GetString(md5File, md5File.Length - 32, 32);   //读取文件最后32位，其中保存的就是MD5值
                return result == md5;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 计算文件的MD5值
        /// </summary>
        /// <param name="md5File">MD5签名文件字符数组</param>
        /// <param name="index">计算起始位置</param>
        /// <param name="count">计算终止位置</param>
        /// <returns>计算结果</returns>
        private static string Md5Buffer(byte[] md5File, int index, int count)
        {
            var getMd5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var hashByte = getMd5.ComputeHash(md5File, index, count);
            var result = System.BitConverter.ToString(hashByte);

            result = result.Replace("-", "");
            return result;
        } 
        #endregion

        private void Test()
        {
            var o = "i love u";
            o = AddMd5Profix(o);
            //o += " ";
            Console.WriteLine(o);
            Console.WriteLine(ValidateValue(o));

            o = RemoveMd5Profix(o);
            Console.WriteLine(o);

        }
    }
}
