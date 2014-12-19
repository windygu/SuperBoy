using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
//�����ַ���,ע��strEncrKey�ĳ���Ϊ8λ(���Ҫ���ӻ��߼���key����,����IV�ĳ��Ⱦ�����) 
//public string DesEncrypt(string strText, string strEncrKey) 

//�����ַ���,ע��strEncrKey�ĳ���Ϊ8λ(���Ҫ���ӻ��߼���key����,����IV�ĳ��Ⱦ�����) 
//public string DesDecrypt(string strText,string sDecrKey) 

//���������ļ�,ע��strEncrKey�ĳ���Ϊ8λ(���Ҫ���ӻ��߼���key����,����IV�ĳ��Ⱦ�����) 
//public void DesEncrypt(string m_InFilePath,string m_OutFilePath,string strEncrKey) 

//���������ļ�,ע��strEncrKey�ĳ���Ϊ8λ(���Ҫ���ӻ��߼���key����,����IV�ĳ��Ⱦ�����) 
//public void DesDecrypt(string m_InFilePath,string m_OutFilePath,string sDecrKey) 

//MD5���� 
//public string MD5Encrypt(string strText) 
using Common;

namespace Commons
{
    /// <summary>
    /// DES�ԳƼӽ��ܡ�AES RijndaelManaged�ӽ��ܡ�Base64���ܽ��ܡ�MD5���ܵȲ���������
    /// </summary>
    public sealed class EncodeHelper
    {
        #region DES�ԳƼ��ܽ���
        public const string DefaultEncryptKey = "12345678";

        /// <summary>
        /// ʹ��Ĭ�ϼ���
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static string DesEncrypt(string strText)
        {
            try
            {
                return DesEncrypt(strText, DefaultEncryptKey);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// ʹ��Ĭ�Ͻ���
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static string DesDecrypt(string strText)
        {
            try
            {
                return DesDecrypt(strText, DefaultEncryptKey);
            }
            catch
            {
                return "";
            }
        }

        /// <summary> 
        /// Encrypt the string 
        /// Attention:key must be 8 bits 
        /// </summary> 
        /// <param name="strText">string</param> 
        /// <param name="strEncrKey">key</param> 
        /// <returns></returns> 
        public static string DesEncrypt(string strText, string strEncrKey)
        {
            byte[] byKey = null;
            byte[] iv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

            byKey = Encoding.UTF8.GetBytes(strEncrKey.Substring(0, 8));
            var des = new DESCryptoServiceProvider();
            var inputByteArray = Encoding.UTF8.GetBytes(strText);
            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateEncryptor(byKey, iv), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary> 
        /// Decrypt string 
        /// Attention:key must be 8 bits 
        /// </summary> 
        /// <param name="strText">Decrypt string</param> 
        /// <param name="sDecrKey">key</param> 
        /// <returns>output string</returns> 
        public static string DesDecrypt(string strText, string sDecrKey)
        {
            byte[] byKey = null;
            byte[] iv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            var inputByteArray = new Byte[strText.Length];

            byKey = Encoding.UTF8.GetBytes(sDecrKey.Substring(0, 8));
            var des = new DESCryptoServiceProvider();
            inputByteArray = Convert.FromBase64String(strText);
            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateDecryptor(byKey, iv), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            Encoding encoding = new UTF8Encoding();
            return encoding.GetString(ms.ToArray());
        }

        /// <summary> 
        /// Encrypt files 
        /// Attention:key must be 8 bits 
        /// </summary> 
        /// <param name="m_InFilePath">Encrypt file path</param> 
        /// <param name="m_OutFilePath">output file</param> 
        /// <param name="strEncrKey">key</param> 
        public static void DesEncrypt(string mInFilePath, string mOutFilePath, string strEncrKey)
        {
            byte[] byKey = null;
            byte[] iv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

            byKey = Encoding.UTF8.GetBytes(strEncrKey.Substring(0, 8));
            var fin = new FileStream(mInFilePath, FileMode.Open, FileAccess.Read);
            var fout = new FileStream(mOutFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);
            //Create variables to help with read and write. 
            var bin = new byte[100]; //This is intermediate storage for the encryption. 
            long rdlen = 0; //This is the total number of bytes written. 
            var totlen = fin.Length; //This is the total length of the input file. 
            int len; //This is the number of bytes to be written at a time. 

            DES des = new DESCryptoServiceProvider();
            var encStream = new CryptoStream(fout, des.CreateEncryptor(byKey, iv), CryptoStreamMode.Write);

            //Read from the input file, then encrypt and write to the output file. 
            while (rdlen < totlen)
            {
                len = fin.Read(bin, 0, 100);
                encStream.Write(bin, 0, len);
                rdlen = rdlen + len;
            }
            encStream.Close();
            fout.Close();
            fin.Close();
        }

        /// <summary> 
        /// Decrypt files 
        /// Attention:key must be 8 bits 
        /// </summary> 
        /// <param name="m_InFilePath">Decrypt filepath</param> 
        /// <param name="m_OutFilePath">output filepath</param> 
        /// <param name="sDecrKey">key</param> 
        public static void DesDecrypt(string mInFilePath, string mOutFilePath, string sDecrKey)
        {
            byte[] byKey = null;
            byte[] iv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

            byKey = Encoding.UTF8.GetBytes(sDecrKey.Substring(0, 8));
            var fin = new FileStream(mInFilePath, FileMode.Open, FileAccess.Read);
            var fout = new FileStream(mOutFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);
            //Create variables to help with read and write. 
            var bin = new byte[100]; //This is intermediate storage for the encryption. 
            long rdlen = 0; //This is the total number of bytes written. 
            var totlen = fin.Length; //This is the total length of the input file. 
            int len; //This is the number of bytes to be written at a time. 

            DES des = new DESCryptoServiceProvider();
            var encStream = new CryptoStream(fout, des.CreateDecryptor(byKey, iv), CryptoStreamMode.Write);

            //Read from the input file, then encrypt and write to the output file. 
            while (rdlen < totlen)
            {
                len = fin.Read(bin, 0, 100);
                encStream.Write(bin, 0, len);
                rdlen = rdlen + len;
            }
            encStream.Close();
            fout.Close();
            fin.Close();
        } 
        #endregion

        #region �ԳƼ����㷨AES RijndaelManaged���ܽ���
        private static readonly string DefaultAesKey = "@#kim123";
        private static byte[] _keys = { 0x41, 0x72, 0x65, 0x79, 0x6F, 0x75, 0x6D, 0x79,
                                             0x53,0x6E, 0x6F, 0x77, 0x6D, 0x61, 0x6E, 0x3F };

        /// <summary>
        /// �ԳƼ����㷨AES RijndaelManaged����(RijndaelManaged��AES���㷨�ǿ�ʽ�����㷨)
        /// </summary>
        /// <param name="encryptString">�������ַ���</param>
        /// <returns>���ܽ���ַ���</returns>
        public static string AES_Encrypt(string encryptString)
        {
            return AES_Encrypt(encryptString, DefaultAesKey);
        }

        /// <summary>
        /// �ԳƼ����㷨AES RijndaelManaged����(RijndaelManaged��AES���㷨�ǿ�ʽ�����㷨)
        /// </summary>
        /// <param name="encryptString">�������ַ���</param>
        /// <param name="encryptKey">������Կ�������ַ�</param>
        /// <returns>���ܽ���ַ���</returns>
        public static string AES_Encrypt(string encryptString, string encryptKey)
        {
            encryptKey = GetSubString(encryptKey, 32, "");
            encryptKey = encryptKey.PadRight(32, ' ');

            var rijndaelProvider = new RijndaelManaged();
            rijndaelProvider.Key = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 32));
            rijndaelProvider.IV = _keys;
            var rijndaelEncrypt = rijndaelProvider.CreateEncryptor();

            var inputData = Encoding.UTF8.GetBytes(encryptString);
            var encryptedData = rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);

            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// �ԳƼ����㷨AES RijndaelManaged�����ַ���
        /// </summary>
        /// <param name="decryptString">�����ܵ��ַ���</param>
        /// <returns>���ܳɹ����ؽ��ܺ���ַ���,ʧ�ܷ�Դ��</returns>
        public static string AES_Decrypt(string decryptString)
        {
            return AES_Decrypt(decryptString, DefaultAesKey);
        }

        /// <summary>
        /// �ԳƼ����㷨AES RijndaelManaged�����ַ���
        /// </summary>
        /// <param name="decryptString">�����ܵ��ַ���</param>
        /// <param name="decryptKey">������Կ,�ͼ�����Կ��ͬ</param>
        /// <returns>���ܳɹ����ؽ��ܺ���ַ���,ʧ�ܷ��ؿ�</returns>
        public static string AES_Decrypt(string decryptString, string decryptKey)
        {
            try
            {
                decryptKey = GetSubString(decryptKey, 32, "");
                decryptKey = decryptKey.PadRight(32, ' ');

                var rijndaelProvider = new RijndaelManaged();
                rijndaelProvider.Key = Encoding.UTF8.GetBytes(decryptKey);
                rijndaelProvider.IV = _keys;
                var rijndaelDecrypt = rijndaelProvider.CreateDecryptor();

                var inputData = Convert.FromBase64String(decryptString);
                var decryptedData = rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);

                return Encoding.UTF8.GetString(decryptedData);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// ���ֽڳ���(���ֽ�,һ������Ϊ2���ֽ�)ȡ��ĳ�ַ�����һ����
        /// </summary>
        /// <param name="sourceString">Դ�ַ���</param>
        /// <param name="length">��ȡ�ַ����ֽڳ���</param>
        /// <param name="tailString">�����ַ���(���ַ���������ʱ��β������ӵ��ַ�����һ��Ϊ"...")</param>
        /// <returns>ĳ�ַ�����һ����</returns>
        private static string GetSubString(string sourceString, int length, string tailString)
        {
            return GetSubString(sourceString, 0, length, tailString);
        }

        /// <summary>
        /// ���ֽڳ���(���ֽ�,һ������Ϊ2���ֽ�)ȡ��ĳ�ַ�����һ����
        /// </summary>
        /// <param name="sourceString">Դ�ַ���</param>
        /// <param name="startIndex">����λ�ã���0��ʼ</param>
        /// <param name="length">��ȡ�ַ����ֽڳ���</param>
        /// <param name="tailString">�����ַ���(���ַ���������ʱ��β������ӵ��ַ�����һ��Ϊ"...")</param>
        /// <returns>ĳ�ַ�����һ����</returns>
        private static string GetSubString(string sourceString, int startIndex, int length, string tailString)
        {
            var myResult = sourceString;

            //�������Ļ���ʱ(ע:���ĵķ�Χ:\u4e00 - \u9fa5, ������\u0800 - \u4e00, ����Ϊ\xAC00-\xD7A3)
            if (System.Text.RegularExpressions.Regex.IsMatch(sourceString, "[\u0800-\u4e00]+") ||
                System.Text.RegularExpressions.Regex.IsMatch(sourceString, "[\xAC00-\xD7A3]+"))
            {
                //����ȡ����ʼλ�ó����ֶδ�����ʱ
                if (startIndex >= sourceString.Length)
                {
                    return string.Empty;
                }
                else
                {
                    return sourceString.Substring(startIndex,
                                                   ((length + startIndex) > sourceString.Length) ? (sourceString.Length - startIndex) : length);
                }
            }

            //�����ַ�����"�й�����abcd123"
            if (length <= 0)
            {
                return string.Empty;
            }
            var bytesSource = Encoding.Default.GetBytes(sourceString);

            //���ַ������ȴ�����ʼλ��
            if (bytesSource.Length > startIndex)
            {
                var endIndex = bytesSource.Length;

                //��Ҫ��ȡ�ĳ������ַ�������Ч���ȷ�Χ��
                if (bytesSource.Length > (startIndex + length))
                {
                    endIndex = length + startIndex;
                }
                else
                {   //��������Ч��Χ��ʱ,ֻȡ���ַ����Ľ�β
                    length = bytesSource.Length - startIndex;
                    tailString = "";
                }

                var anResultFlag = new int[length];
                var nFlag = 0;
                //�ֽڴ���127Ϊ˫�ֽ��ַ�
                for (var i = startIndex; i < endIndex; i++)
                {
                    if (bytesSource[i] > 127)
                    {
                        nFlag++;
                        if (nFlag == 3)
                        {
                            nFlag = 1;
                        }
                    }
                    else
                    {
                        nFlag = 0;
                    }
                    anResultFlag[i] = nFlag;
                }
                //���һ���ֽ�Ϊ˫�ֽ��ַ���һ��
                if ((bytesSource[endIndex - 1] > 127) && (anResultFlag[length - 1] == 1))
                {
                    length = length + 1;
                }

                var bsResult = new byte[length];
                Array.Copy(bytesSource, startIndex, bsResult, 0, length);
                myResult = Encoding.Default.GetString(bsResult);
                myResult = myResult + tailString;

                return myResult;
            }

            return string.Empty;

        }

        /// <summary>
        /// �����ļ���
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static CryptoStream AES_EncryptStrream(FileStream fs, string decryptKey)
        {
            decryptKey = GetSubString(decryptKey, 32, "");
            decryptKey = decryptKey.PadRight(32, ' ');

            var rijndaelProvider = new RijndaelManaged();
            rijndaelProvider.Key = Encoding.UTF8.GetBytes(decryptKey);
            rijndaelProvider.IV = _keys;

            var encrypto = rijndaelProvider.CreateEncryptor();
            var cytptostreamEncr = new CryptoStream(fs, encrypto, CryptoStreamMode.Write);
            return cytptostreamEncr;
        }

        /// <summary>
        /// �����ļ���
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static CryptoStream AES_DecryptStream(FileStream fs, string decryptKey)
        {
            decryptKey = GetSubString(decryptKey, 32, "");
            decryptKey = decryptKey.PadRight(32, ' ');

            var rijndaelProvider = new RijndaelManaged();
            rijndaelProvider.Key = Encoding.UTF8.GetBytes(decryptKey);
            rijndaelProvider.IV = _keys;
            var decrypto = rijndaelProvider.CreateDecryptor();
            var cytptostreamDecr = new CryptoStream(fs, decrypto, CryptoStreamMode.Read);
            return cytptostreamDecr;
        }

        /// <summary>
        /// ��ָ���ļ�����
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        public static bool AES_EncryptFile(string inputFile, string outputFile)
        {
            try
            {
                var decryptKey = "www.iqidi.com";

                var fr = new FileStream(inputFile, FileMode.Open);
                var fren = new FileStream(outputFile, FileMode.Create);
                var enfr = AES_EncryptStrream(fren, decryptKey);
                var bytearrayinput = new byte[fr.Length];
                fr.Read(bytearrayinput, 0, bytearrayinput.Length);
                enfr.Write(bytearrayinput, 0, bytearrayinput.Length);
                enfr.Close();
                fr.Close();
                fren.Close();
            }
            catch
            {
                //�ļ��쳣
                return false;
            }
            return true;
        }

        /// <summary>
        /// ��ָ�����ļ���ѹ��
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        public static bool AES_DecryptFile(string inputFile, string outputFile)
        {
            try
            {
                var decryptKey = "www.iqidi.com";
                var fr = new FileStream(inputFile, FileMode.Open);
                var frde = new FileStream(outputFile, FileMode.Create);
                var defr = AES_DecryptStream(fr, decryptKey);
                var bytearrayoutput = new byte[1024];
                var mCount = 0;

                do
                {
                    mCount = defr.Read(bytearrayoutput, 0, bytearrayoutput.Length);
                    frde.Write(bytearrayoutput, 0, mCount);
                    if (mCount < bytearrayoutput.Length)
                        break;
                } while (true);

                defr.Close();
                fr.Close();
                frde.Close();
            }
            catch
            {
                //�ļ��쳣
                return false;
            }
            return true;
        }
        
        #endregion

        #region Base64���ܽ���
        /// <summary>
        /// Base64��һ�Nʹ��64����λ��Ӌ��������ʹ��2�����η������H����ӡ��ASCII ��Ԫ��
        /// �@ʹ�����Á���������]���Ă�ݔ���a����Base64�е�׃��ʹ����ԪA-Z��a-z��0-9 ��
        /// �@�ӹ���62����Ԫ���Á������_ʼ��64�����֣�����ɂ��Á����锵�ֵķ�̖�ڲ�ͬ��
        /// ϵ�y�ж���ͬ��
        /// Base64����
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Base64Encrypt(string str)
        {
            var encbuff = System.Text.Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(encbuff);
        }

        /// <summary>
        /// Base64����
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Base64Decrypt(string str)
        {
            var decbuff = Convert.FromBase64String(str);
            return System.Text.Encoding.UTF8.GetString(decbuff);
        } 
        #endregion

        #region MD5����
        /// <summary> 
        /// MD5 Encrypt 
        /// </summary> 
        /// <param name="strText">text</param> 
        /// <returns>md5 Encrypt string</returns> 
        public static string Md5Encrypt(string strText)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            var result = md5.ComputeHash(Encoding.Default.GetBytes(strText));
            return Encoding.Default.GetString(result);
        }

        public static string Md5EncryptHash(String input)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            //the GetBytes method returns byte array equavalent of a string
            var res = md5.ComputeHash(Encoding.Default.GetBytes(input), 0, input.Length);
            var temp = new char[res.Length];
            //copy to a char array which can be passed to a String constructor
            Array.Copy(res, temp, res.Length);
            //return the result as a string
            return new String(temp);
        }

        public static string Md5EncryptHashHex(String input)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            //the GetBytes method returns byte array equavalent of a string
            var res = md5.ComputeHash(Encoding.Default.GetBytes(input), 0, input.Length);

            var returnThis = string.Empty;

            for (var i = 0; i < res.Length; i++)
            {
                returnThis += Uri.HexEscape((char)res[i]);
            }
            returnThis = returnThis.Replace("%", "");
            returnThis = returnThis.ToLower();

            return returnThis;
        }

        /// <summary>
        /// MD5 ���μ����㷨.�������: (QQʹ��)
        /// 1. ��֤��תΪ��д
        /// 2. ������ʹ����������������μ��ܺ�,����֤����е���
        /// 3. Ȼ�󽫵��Ӻ�������ٴ�MD5һ��,�õ�������֤���ֵ
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string EncyptMD5_3_16(string s)
        {
            var md5 = MD5CryptoServiceProvider.Create();
            var bytes = System.Text.Encoding.ASCII.GetBytes(s);
            var bytes1 = md5.ComputeHash(bytes);
            var bytes2 = md5.ComputeHash(bytes1);
            var bytes3 = md5.ComputeHash(bytes2);

            var sb = new StringBuilder();
            foreach (var item in bytes3)
            {
                sb.Append(item.ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString().ToUpper();
        }
        #endregion

        /// <summary>
        /// SHA256����
        /// </summary>
        /// <param name="str">ԭʼ�ַ���</param>
        /// <returns>SHA256���(���س���Ϊ44�ֽڵ��ַ���)</returns>
        public static string SHA256(string str)
        {
            var sha256Data = Encoding.UTF8.GetBytes(str);
            var sha256 = new SHA256Managed();
            var result = sha256.ComputeHash(sha256Data);
            return Convert.ToBase64String(result);  //���س���Ϊ44�ֽڵ��ַ���
        }


        /// <summary>
        /// �����ַ���
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string EncryptString(string input)
        {
            return Md5Util.AddMd5Profix(Base64Util.Encrypt(Md5Util.AddMd5Profix(input)));
            //return Base64.Encrypt(MD5.AddMD5Profix(Base64.Encrypt(input)));
        }

        /// <summary>
        /// ���ܼӹ��ܵ��ַ���
        /// </summary>
        /// <param name="input"></param>
        /// <param name="throwException">����ʧ���Ƿ����쳣</param>
        /// <returns></returns>
        public static string DecryptString(string input, bool throwException)
        {
            var res = "";
            try
            {
                res = input;// Base64.Decrypt(input);
                if (Md5Util.ValidateValue(res))
                {
                    return Md5Util.RemoveMd5Profix(Base64Util.Decrypt(Md5Util.RemoveMd5Profix(res)));
                }
                else
                {
                    throw new Exception("�ַ����޷�ת���ɹ���");
                }
            }
            catch
            {
                if (throwException)
                {
                    throw;
                }
                else
                {
                    return "";
                }
            }
        }
    }
}