using System;
using System.Security.Cryptography;  
using System.Text;
using System.IO;

namespace Common
{
    public class Encrypt
    {
        public Encrypt()
        {
        }

        //密钥
        private static byte[] _arrDesKey = new byte[] {42, 16, 93, 156, 78, 4, 218, 32};
        private static byte[] _arrDesiv = new byte[] {55, 103, 246, 79, 36, 99, 167, 3};

        /// <summary>
        /// 加密。
        /// </summary>
        /// <param name="m_Need_Encode_String"></param>
        /// <returns></returns>
        public static string Encode(string mNeedEncodeString)
        {
            if (mNeedEncodeString == null)
            {
                throw new Exception("Error: \n源字符串为空！！");
            }
            var objDes = new DESCryptoServiceProvider();
            var objMemoryStream = new MemoryStream();
            var objCryptoStream = new CryptoStream(objMemoryStream, objDes.CreateEncryptor(_arrDesKey, _arrDesiv),
                CryptoStreamMode.Write);
            var objStreamWriter = new StreamWriter(objCryptoStream);
            objStreamWriter.Write(mNeedEncodeString);
            objStreamWriter.Flush();
            objCryptoStream.FlushFinalBlock();
            objMemoryStream.Flush();
            return Convert.ToBase64String(objMemoryStream.GetBuffer(), 0, (int) objMemoryStream.Length);
        }

        /// <summary>
        /// 解密。
        /// </summary>
        /// <param name="m_Need_Encode_String"></param>
        /// <returns></returns>
        public static string Decode(string mNeedEncodeString)
        {
            if (mNeedEncodeString == null)
            {
                throw new Exception("Error: \n源字符串为空！！");
            }
            var objDes = new DESCryptoServiceProvider();
            var arrInput = Convert.FromBase64String(mNeedEncodeString);
            var objMemoryStream = new MemoryStream(arrInput);
            var objCryptoStream = new CryptoStream(objMemoryStream, objDes.CreateDecryptor(_arrDesKey, _arrDesiv),
                CryptoStreamMode.Read);
            var objStreamReader = new StreamReader(objCryptoStream);
            return objStreamReader.ReadToEnd();
        }

        /// <summary>
        /// md5
        /// </summary>
        /// <param name="encypStr"></param>
        /// <returns></returns>
        public static string Md5(string encypStr)
        {
            string retStr;
            var m5 = new MD5CryptoServiceProvider();
            byte[] inputBye;
            byte[] outputBye;
            inputBye = System.Text.Encoding.ASCII.GetBytes(encypStr);
            outputBye = m5.ComputeHash(inputBye);
            retStr = Convert.ToBase64String(outputBye);
            return (retStr);
        }

        

        

        #region TripleDES加密

        /// <summary>
        /// TripleDES加密
        /// </summary>
        public static string TripleDesEncrypting(string strSource)
        {
            try
            {
                var bytIn = Encoding.Default.GetBytes(strSource);
                byte[] key =
                {
                    42, 16, 93, 156, 78, 4, 218, 32, 15, 167, 44, 80, 26, 20, 155, 112, 2, 94, 11, 204, 119,
                    35, 184, 197
                }; //定义密钥
                byte[] iv = {55, 103, 246, 79, 36, 99, 167, 3}; //定义偏移量
                var tripleDes = new TripleDESCryptoServiceProvider();
                tripleDes.IV = iv;
                tripleDes.Key = key;
                var encrypto = tripleDes.CreateEncryptor();
                var ms = new System.IO.MemoryStream();
                var cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
                cs.Write(bytIn, 0, bytIn.Length);
                cs.FlushFinalBlock();
                var bytOut = ms.ToArray();
                return System.Convert.ToBase64String(bytOut);
            }
            catch (Exception ex)
            {
                throw new Exception("加密时候出现错误!错误提示:\n" + ex.Message);
            }
        }

        #endregion

        #region TripleDES解密

        /// <summary>
        /// TripleDES解密
        /// </summary>
        public static string TripleDesDecrypting(string source)
        {
            try
            {
                var bytIn = System.Convert.FromBase64String(source);
                byte[] key =
                {
                    42, 16, 93, 156, 78, 4, 218, 32, 15, 167, 44, 80, 26, 20, 155, 112, 2, 94, 11, 204, 119,
                    35, 184, 197
                }; //定义密钥
                byte[] iv = {55, 103, 246, 79, 36, 99, 167, 3}; //定义偏移量
                var tripleDes = new TripleDESCryptoServiceProvider();
                tripleDes.IV = iv;
                tripleDes.Key = key;
                var encrypto = tripleDes.CreateDecryptor();
                var ms = new System.IO.MemoryStream(bytIn, 0, bytIn.Length);
                var cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
                var strd = new StreamReader(cs, Encoding.Default);
                return strd.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new Exception("解密时候出现错误!错误提示:\n" + ex.Message);
            }
        }

        #endregion
    }
}