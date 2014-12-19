using System; 
using System.Text; 
using System.Security.Cryptography;
namespace Common
{ 
	/// <summary> 
	/// RSA���ܽ��ܼ�RSAǩ������֤
	/// </summary> 
	public class RsaCryption 
	{ 		
		public RsaCryption() 
		{ 			
		} 
		

		#region RSA ���ܽ��� 

		#region RSA ����Կ���� 
	
		/// <summary>
		/// RSA ����Կ���� ����˽Կ �͹�Կ 
		/// </summary>
		/// <param name="xmlKeys"></param>
		/// <param name="xmlPublicKey"></param>
		public void RsaKey(out string xmlKeys,out string xmlPublicKey) 
		{ 			
				var rsa=new RSACryptoServiceProvider(); 
				xmlKeys=rsa.ToXmlString(true); 
				xmlPublicKey = rsa.ToXmlString(false); 			
		} 
		#endregion 

		#region RSA�ļ��ܺ��� 
		//############################################################################## 
		//RSA ��ʽ���� 
		//˵��KEY������XML����ʽ,���ص����ַ��� 
		//����һ����Ҫ˵�������ü��ܷ�ʽ�� ���� ���Ƶģ��� 
		//############################################################################## 

		//RSA�ļ��ܺ���  string
		public string RSAEncrypt(string xmlPublicKey,string mStrEncryptString ) 
		{ 
			
			byte[] plainTextBArray; 
			byte[] cypherTextBArray; 
			string result; 
			var rsa=new RSACryptoServiceProvider(); 
			rsa.FromXmlString(xmlPublicKey); 
			plainTextBArray = (new UnicodeEncoding()).GetBytes(mStrEncryptString); 
			cypherTextBArray = rsa.Encrypt(plainTextBArray, false); 
			result=Convert.ToBase64String(cypherTextBArray); 
			return result; 
			
		} 
		//RSA�ļ��ܺ��� byte[]
		public string RSAEncrypt(string xmlPublicKey,byte[] encryptString ) 
		{ 
			
			byte[] cypherTextBArray; 
			string result; 
			var rsa=new RSACryptoServiceProvider(); 
			rsa.FromXmlString(xmlPublicKey); 
			cypherTextBArray = rsa.Encrypt(encryptString, false); 
			result=Convert.ToBase64String(cypherTextBArray); 
			return result; 
			
		} 
		#endregion 

		#region RSA�Ľ��ܺ��� 
		//RSA�Ľ��ܺ���  string
		public string RSADecrypt(string xmlPrivateKey, string mStrDecryptString ) 
		{			
			byte[] plainTextBArray; 
			byte[] dypherTextBArray; 
			string result; 
			var rsa=new RSACryptoServiceProvider(); 
			rsa.FromXmlString(xmlPrivateKey); 
			plainTextBArray =Convert.FromBase64String(mStrDecryptString); 
			dypherTextBArray=rsa.Decrypt(plainTextBArray, false); 
			result=(new UnicodeEncoding()).GetString(dypherTextBArray); 
			return result; 
			
		} 

		//RSA�Ľ��ܺ���  byte
		public string RSADecrypt(string xmlPrivateKey, byte[] decryptString ) 
		{			
			byte[] dypherTextBArray; 
			string result; 
			var rsa=new RSACryptoServiceProvider(); 
			rsa.FromXmlString(xmlPrivateKey); 
			dypherTextBArray=rsa.Decrypt(decryptString, false); 
			result=(new UnicodeEncoding()).GetString(dypherTextBArray); 
			return result; 
			
		} 
		#endregion 

		#endregion 

		#region RSA����ǩ�� 

		#region ��ȡHash������ 
		//��ȡHash������ 
		public bool GetHash(string mStrSource, ref byte[] hashData) 
		{ 			
			//���ַ�����ȡ��Hash���� 
			byte[] buffer; 
			var md5 = System.Security.Cryptography.HashAlgorithm.Create("MD5"); 
			buffer = System.Text.Encoding.GetEncoding("GB2312").GetBytes(mStrSource); 
			hashData = md5.ComputeHash(buffer); 

			return true; 			
		} 

		//��ȡHash������ 
		public bool GetHash(string mStrSource, ref string strHashData) 
		{ 
			
			//���ַ�����ȡ��Hash���� 
			byte[] buffer; 
			byte[] hashData; 
			var md5 = System.Security.Cryptography.HashAlgorithm.Create("MD5"); 
			buffer = System.Text.Encoding.GetEncoding("GB2312").GetBytes(mStrSource); 
			hashData = md5.ComputeHash(buffer); 

			strHashData = Convert.ToBase64String(hashData); 
			return true; 
			
		} 

		//��ȡHash������ 
		public bool GetHash(System.IO.FileStream objFile, ref byte[] hashData) 
		{ 
			
			//���ļ���ȡ��Hash���� 
			var md5 = System.Security.Cryptography.HashAlgorithm.Create("MD5"); 
			hashData = md5.ComputeHash(objFile); 
			objFile.Close(); 

			return true; 
			
		} 

		//��ȡHash������ 
		public bool GetHash(System.IO.FileStream objFile, ref string strHashData) 
		{ 
			
			//���ļ���ȡ��Hash���� 
			byte[] hashData; 
			var md5 = System.Security.Cryptography.HashAlgorithm.Create("MD5"); 
			hashData = md5.ComputeHash(objFile); 
			objFile.Close(); 

			strHashData = Convert.ToBase64String(hashData); 

			return true; 
			
		} 
		#endregion 

		#region RSAǩ�� 
		//RSAǩ�� 
		public bool SignatureFormatter(string pStrKeyPrivate, byte[] hashbyteSignature, ref byte[] encryptedSignatureData) 
		{ 
			
				var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(); 

				rsa.FromXmlString(pStrKeyPrivate); 
				var rsaFormatter = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(rsa); 
				//����ǩ�����㷨ΪMD5 
				rsaFormatter.SetHashAlgorithm("MD5"); 
				//ִ��ǩ�� 
				encryptedSignatureData = rsaFormatter.CreateSignature(hashbyteSignature); 

				return true; 
			
		} 

		//RSAǩ�� 
		public bool SignatureFormatter(string pStrKeyPrivate, byte[] hashbyteSignature, ref string mStrEncryptedSignatureData) 
		{ 
			
				byte[] encryptedSignatureData; 

				var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(); 

				rsa.FromXmlString(pStrKeyPrivate); 
				var rsaFormatter = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(rsa); 
				//����ǩ�����㷨ΪMD5 
				rsaFormatter.SetHashAlgorithm("MD5"); 
				//ִ��ǩ�� 
				encryptedSignatureData = rsaFormatter.CreateSignature(hashbyteSignature); 

				mStrEncryptedSignatureData = Convert.ToBase64String(encryptedSignatureData); 

				return true; 
			
		} 

		//RSAǩ�� 
		public bool SignatureFormatter(string pStrKeyPrivate, string mStrHashbyteSignature, ref byte[] encryptedSignatureData) 
		{ 
			
				byte[] hashbyteSignature; 

				hashbyteSignature = Convert.FromBase64String(mStrHashbyteSignature); 
				var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(); 

				rsa.FromXmlString(pStrKeyPrivate); 
				var rsaFormatter = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(rsa); 
				//����ǩ�����㷨ΪMD5 
				rsaFormatter.SetHashAlgorithm("MD5"); 
				//ִ��ǩ�� 
				encryptedSignatureData = rsaFormatter.CreateSignature(hashbyteSignature); 

				return true; 
			
		} 

		//RSAǩ�� 
		public bool SignatureFormatter(string pStrKeyPrivate, string mStrHashbyteSignature, ref string mStrEncryptedSignatureData) 
		{ 
			
				byte[] hashbyteSignature; 
				byte[] encryptedSignatureData; 

				hashbyteSignature = Convert.FromBase64String(mStrHashbyteSignature); 
				var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(); 

				rsa.FromXmlString(pStrKeyPrivate); 
				var rsaFormatter = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(rsa); 
				//����ǩ�����㷨ΪMD5 
				rsaFormatter.SetHashAlgorithm("MD5"); 
				//ִ��ǩ�� 
				encryptedSignatureData = rsaFormatter.CreateSignature(hashbyteSignature); 

				mStrEncryptedSignatureData = Convert.ToBase64String(encryptedSignatureData); 

				return true; 
			
		} 
		#endregion 

		#region RSA ǩ����֤ 

		public bool SignatureDeformatter(string pStrKeyPublic, byte[] hashbyteDeformatter, byte[] deformatterData) 
		{ 
			
				var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(); 

				rsa.FromXmlString(pStrKeyPublic); 
				var rsaDeformatter = new System.Security.Cryptography.RSAPKCS1SignatureDeformatter(rsa); 
				//ָ�����ܵ�ʱ��HASH�㷨ΪMD5 
				rsaDeformatter.SetHashAlgorithm("MD5"); 

				if(rsaDeformatter.VerifySignature(hashbyteDeformatter,deformatterData)) 
				{ 
					return true; 
				} 
				else 
				{ 
					return false; 
				} 
			
		} 

		public bool SignatureDeformatter(string pStrKeyPublic, string pStrHashbyteDeformatter, byte[] deformatterData) 
		{ 
			
				byte[] hashbyteDeformatter; 

				hashbyteDeformatter = Convert.FromBase64String(pStrHashbyteDeformatter); 

				var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(); 

				rsa.FromXmlString(pStrKeyPublic); 
				var rsaDeformatter = new System.Security.Cryptography.RSAPKCS1SignatureDeformatter(rsa); 
				//ָ�����ܵ�ʱ��HASH�㷨ΪMD5 
				rsaDeformatter.SetHashAlgorithm("MD5"); 

				if(rsaDeformatter.VerifySignature(hashbyteDeformatter,deformatterData)) 
				{ 
					return true; 
				} 
				else 
				{ 
					return false; 
				} 
			
		} 

		public bool SignatureDeformatter(string pStrKeyPublic, byte[] hashbyteDeformatter, string pStrDeformatterData) 
		{ 
			
				byte[] deformatterData; 

				var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(); 

				rsa.FromXmlString(pStrKeyPublic); 
				var rsaDeformatter = new System.Security.Cryptography.RSAPKCS1SignatureDeformatter(rsa); 
				//ָ�����ܵ�ʱ��HASH�㷨ΪMD5 
				rsaDeformatter.SetHashAlgorithm("MD5"); 

				deformatterData =Convert.FromBase64String(pStrDeformatterData); 

				if(rsaDeformatter.VerifySignature(hashbyteDeformatter,deformatterData)) 
				{ 
					return true; 
				} 
				else 
				{ 
					return false; 
				} 
			
		} 

		public bool SignatureDeformatter(string pStrKeyPublic, string pStrHashbyteDeformatter, string pStrDeformatterData) 
		{ 
			
				byte[] deformatterData; 
				byte[] hashbyteDeformatter; 

				hashbyteDeformatter = Convert.FromBase64String(pStrHashbyteDeformatter); 
				var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(); 

				rsa.FromXmlString(pStrKeyPublic); 
				var rsaDeformatter = new System.Security.Cryptography.RSAPKCS1SignatureDeformatter(rsa); 
				//ָ�����ܵ�ʱ��HASH�㷨ΪMD5 
				rsaDeformatter.SetHashAlgorithm("MD5"); 

				deformatterData =Convert.FromBase64String(pStrDeformatterData); 

				if(rsaDeformatter.VerifySignature(hashbyteDeformatter,deformatterData)) 
				{ 
					return true; 
				} 
				else 
				{ 
					return false; 
				} 
			
		} 


		#endregion 


		#endregion 

	} 
} 
