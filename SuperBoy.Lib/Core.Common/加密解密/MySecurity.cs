using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using Core.Common;

namespace Common
{
    /// <summary>
    /// MySecurity(安全类) 的摘要说明。
    /// </summary>
    public class MySecurity
    {
        /// <summary>
        /// 初始化安全类
        /// </summary>
        public MySecurity()
        {
            ///默认密码
            _key = "0123456789";
        }
        private string _key; //默认密钥

        private byte[] _sKey;
        private byte[] _sIv;

        #region 加密字符串
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="inputStr">输入字符串</param>
        /// <param name="keyStr">密码，可以为“”</param>
        /// <returns>输出加密后字符串</returns>
        public static string SEncryptString(string inputStr, string keyStr)
        {
            var ws = new MySecurity();
            return ws.EncryptString(inputStr, keyStr);
        }
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="inputStr">输入字符串</param>
        /// <param name="keyStr">密码，可以为“”</param>
        /// <returns>输出加密后字符串</returns>
        public string EncryptString(string inputStr, string keyStr)
        {
            var des = new DESCryptoServiceProvider();
            if (keyStr == "")
                keyStr = _key;
            var inputByteArray = Encoding.Default.GetBytes(inputStr);
            var keyByteArray = Encoding.Default.GetBytes(keyStr);
            SHA1 ha = new SHA1Managed();
            var hb = ha.ComputeHash(keyByteArray);
            _sKey = new byte[8];
            _sIv = new byte[8];
            for (var i = 0; i < 8; i++)
                _sKey[i] = hb[i];
            for (var i = 8; i < 16; i++)
                _sIv[i - 8] = hb[i];
            des.Key = _sKey;
            des.IV = _sIv;
            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            var ret = new StringBuilder();
            foreach (var b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            cs.Close();
            ms.Close();
            return ret.ToString();
        }
        #endregion

        #region 加密字符串 密钥为系统默认 0123456789
        /// <summary>
        /// 加密字符串 密钥为系统默认
        /// </summary>
        /// <param name="inputStr">输入字符串</param>
        /// <returns>输出加密后字符串</returns>
        static public string SEncryptString(string inputStr)
        {
            var ws = new MySecurity();
            return ws.EncryptString(inputStr, "");
        }
        #endregion

        #region 加密文件
        /// <summary>
        /// 加密文件
        /// </summary>
        /// <param name="filePath">输入文件路径</param>
        /// <param name="savePath">加密后输出文件路径</param>
        /// <param name="keyStr">密码，可以为“”</param>
        /// <returns></returns>  
        public bool EncryptFile(string filePath, string savePath, string keyStr)
        {
            var des = new DESCryptoServiceProvider();
            if (keyStr == "")
                keyStr = _key;
            var fs = File.OpenRead(filePath);
            var inputByteArray = new byte[fs.Length];
            fs.Read(inputByteArray, 0, (int)fs.Length);
            fs.Close();
            var keyByteArray = Encoding.Default.GetBytes(keyStr);
            SHA1 ha = new SHA1Managed();
            var hb = ha.ComputeHash(keyByteArray);
            _sKey = new byte[8];
            _sIv = new byte[8];
            for (var i = 0; i < 8; i++)
                _sKey[i] = hb[i];
            for (var i = 8; i < 16; i++)
                _sIv[i - 8] = hb[i];
            des.Key = _sKey;
            des.IV = _sIv;
            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            fs = File.OpenWrite(savePath);
            foreach (var b in ms.ToArray())
            {
                fs.WriteByte(b);
            }
            fs.Close();
            cs.Close();
            ms.Close();
            return true;
        }
        #endregion

        #region 解密字符串
        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="inputStr">要解密的字符串</param>
        /// <param name="keyStr">密钥</param>
        /// <returns>解密后的结果</returns>
        static public string SDecryptString(string inputStr, string keyStr)
        {
            var ws = new MySecurity();
            return ws.DecryptString(inputStr, keyStr);
        }
        /// <summary>
        ///  解密字符串 密钥为系统默认
        /// </summary>
        /// <param name="inputStr">要解密的字符串</param>
        /// <returns>解密后的结果</returns>
        static public string SDecryptString(string inputStr)
        {
            var ws = new MySecurity();
            return ws.DecryptString(inputStr, "");
        }
        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="inputStr">要解密的字符串</param>
        /// <param name="keyStr">密钥</param>
        /// <returns>解密后的结果</returns>
        public string DecryptString(string inputStr, string keyStr)
        {
            var des = new DESCryptoServiceProvider();
            if (keyStr == "")
                keyStr = _key;
            var inputByteArray = new byte[inputStr.Length / 2];
            for (var x = 0; x < inputStr.Length / 2; x++)
            {
                var i = (Convert.ToInt32(inputStr.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }
            var keyByteArray = Encoding.Default.GetBytes(keyStr);
            SHA1 ha = new SHA1Managed();
            var hb = ha.ComputeHash(keyByteArray);
            _sKey = new byte[8];
            _sIv = new byte[8];
            for (var i = 0; i < 8; i++)
                _sKey[i] = hb[i];
            for (var i = 8; i < 16; i++)
                _sIv[i - 8] = hb[i];
            des.Key = _sKey;
            des.IV = _sIv;
            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            var ret = new StringBuilder();
            return System.Text.Encoding.Default.GetString(ms.ToArray());
        }
        #endregion

        #region 解密文件
        /// <summary>
        /// 解密文件
        /// </summary>
        /// <param name="filePath">输入文件路径</param>
        /// <param name="savePath">解密后输出文件路径</param>
        /// <param name="keyStr">密码，可以为“”</param>
        /// <returns></returns>    
        public bool DecryptFile(string filePath, string savePath, string keyStr)
        {
            var des = new DESCryptoServiceProvider();
            if (keyStr == "")
                keyStr = _key;
            var fs = File.OpenRead(filePath);
            var inputByteArray = new byte[fs.Length];
            fs.Read(inputByteArray, 0, (int)fs.Length);
            fs.Close();
            var keyByteArray = Encoding.Default.GetBytes(keyStr);
            SHA1 ha = new SHA1Managed();
            var hb = ha.ComputeHash(keyByteArray);
            _sKey = new byte[8];
            _sIv = new byte[8];
            for (var i = 0; i < 8; i++)
                _sKey[i] = hb[i];
            for (var i = 8; i < 16; i++)
                _sIv[i - 8] = hb[i];
            des.Key = _sKey;
            des.IV = _sIv;
            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            fs = File.OpenWrite(savePath);
            foreach (var b in ms.ToArray())
            {
                fs.WriteByte(b);
            }
            fs.Close();
            cs.Close();
            ms.Close();
            return true;
        }
        #endregion


        

        #region Base64加密
        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="text">要加密的字符串</param>
        /// <returns></returns>
        public static string EncodeBase64(string text)
        {
            //如果字符串为空，则返回
            if (StringHelper.IsNullOrEmpty<string>(text))
            {
                return "";
            }

            try
            {
                var base64Code = new char[]{'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T',
											'U','V','W','X','Y','Z','a','b','c','d','e','f','g','h','i','j','k','l','m','n',
											'o','p','q','r','s','t','u','v','w','x','y','z','0','1','2','3','4','5','6','7',
											'8','9','+','/','='};
                var empty = (byte)0;
                var byteMessage = new ArrayList(Encoding.Default.GetBytes(text));
                StringBuilder outmessage;
                var messageLen = byteMessage.Count;
                var page = messageLen / 3;
                var use = 0;
                if ((use = messageLen % 3) > 0)
                {
                    for (var i = 0; i < 3 - use; i++)
                        byteMessage.Add(empty);
                    page++;
                }
                outmessage = new System.Text.StringBuilder(page * 4);
                for (var i = 0; i < page; i++)
                {
                    var instr = new byte[3];
                    instr[0] = (byte)byteMessage[i * 3];
                    instr[1] = (byte)byteMessage[i * 3 + 1];
                    instr[2] = (byte)byteMessage[i * 3 + 2];
                    var outstr = new int[4];
                    outstr[0] = instr[0] >> 2;
                    outstr[1] = ((instr[0] & 0x03) << 4) ^ (instr[1] >> 4);
                    if (!instr[1].Equals(empty))
                        outstr[2] = ((instr[1] & 0x0f) << 2) ^ (instr[2] >> 6);
                    else
                        outstr[2] = 64;
                    if (!instr[2].Equals(empty))
                        outstr[3] = (instr[2] & 0x3f);
                    else
                        outstr[3] = 64;
                    outmessage.Append(base64Code[outstr[0]]);
                    outmessage.Append(base64Code[outstr[1]]);
                    outmessage.Append(base64Code[outstr[2]]);
                    outmessage.Append(base64Code[outstr[3]]);
                }
                return outmessage.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Base64解密
        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="text">要解密的字符串</param>
        public static string DecodeBase64(string text)
        {
            //如果字符串为空，则返回
            if (StringHelper.IsNullOrEmpty<string>(text))
            {
                return "";
            }

            //将空格替换为加号
            text = text.Replace(" ", "+");

            try
            {
                if ((text.Length % 4) != 0)
                {
                    return "包含不正确的BASE64编码";
                }
                if (!Regex.IsMatch(text, "^[A-Z0-9/+=]*$", RegexOptions.IgnoreCase))
                {
                    return "包含不正确的BASE64编码";
                }
                var base64Code = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
                var page = text.Length / 4;
                var outMessage = new ArrayList(page * 3);
                var message = text.ToCharArray();
                for (var i = 0; i < page; i++)
                {
                    var instr = new byte[4];
                    instr[0] = (byte)base64Code.IndexOf(message[i * 4]);
                    instr[1] = (byte)base64Code.IndexOf(message[i * 4 + 1]);
                    instr[2] = (byte)base64Code.IndexOf(message[i * 4 + 2]);
                    instr[3] = (byte)base64Code.IndexOf(message[i * 4 + 3]);
                    var outstr = new byte[3];
                    outstr[0] = (byte)((instr[0] << 2) ^ ((instr[1] & 0x30) >> 4));
                    if (instr[2] != 64)
                    {
                        outstr[1] = (byte)((instr[1] << 4) ^ ((instr[2] & 0x3c) >> 2));
                    }
                    else
                    {
                        outstr[2] = 0;
                    }
                    if (instr[3] != 64)
                    {
                        outstr[2] = (byte)((instr[2] << 6) ^ instr[3]);
                    }
                    else
                    {
                        outstr[2] = 0;
                    }
                    outMessage.Add(outstr[0]);
                    if (outstr[1] != 0)
                        outMessage.Add(outstr[1]);
                    if (outstr[2] != 0)
                        outMessage.Add(outstr[2]);
                }
                var outbyte = (byte[])outMessage.ToArray(Type.GetType("System.Byte"));
                return Encoding.Default.GetString(outbyte);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
