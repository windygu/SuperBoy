using System;
using System.Security.Cryptography;
using System.Text;


namespace Core.Common
{
    /// <summary>
    /// �ǶԳƼ�����֤������
    /// </summary>
    public class RsaSecurityHelper
    {
        /// <summary>
        /// ��ע����Ϣ���ݲ��÷ǶԳƼ��ܵķ�ʽ����
        /// </summary>
        /// <param name="originalString">δ���ܵ��ı����������</param>
        /// <param name="encrytedString">���ܺ���ı�����ע�����к�</param>
        /// <returns>�����֤�ɹ�����True������ΪFalse</returns>
        public static bool Validate(string originalString, string encrytedString)
        {
            var bPassed = false;
            using (var rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    rsa.FromXmlString(UiConstants.PublicKey); //��Կ
                    var formatter = new RSAPKCS1SignatureDeformatter(rsa);
                    formatter.SetHashAlgorithm("SHA1");

                    var key = Convert.FromBase64String(encrytedString); //��֤
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