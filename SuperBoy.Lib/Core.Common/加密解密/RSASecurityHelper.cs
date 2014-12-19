using System;
using System.Security.Cryptography;
using System.Text;


namespace Core.Common
{
    /// <summary>
    /// 非对称加密验证辅助类
    /// </summary>
    public class RsaSecurityHelper
    {
        /// <summary>
        /// 对注册信息数据采用非对称加密的方式加密
        /// </summary>
        /// <param name="originalString">未加密的文本，如机器码</param>
        /// <param name="encrytedString">加密后的文本，如注册序列号</param>
        /// <returns>如果验证成功返回True，否则为False</returns>
        public static bool Validate(string originalString, string encrytedString)
        {
            var bPassed = false;
            using (var rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    rsa.FromXmlString(UiConstants.PublicKey); //公钥
                    var formatter = new RSAPKCS1SignatureDeformatter(rsa);
                    formatter.SetHashAlgorithm("SHA1");

                    var key = Convert.FromBase64String(encrytedString); //验证
                    var sha = new SHA1Managed();
                    var name = sha.ComputeHash(ASCIIEncoding.ASCII.GetBytes(originalString));
                    if (formatter.VerifySignature(name, key))
                    {
                        bPassed = true;
                    }
                }
                catch
                {
                }
            }
            return bPassed;
        }
    }
}