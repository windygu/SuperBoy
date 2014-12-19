using System; 
using System.Text; 
using System.Security.Cryptography;
namespace Common
{ 
	/// <summary> 
	/// RSA加密解密及RSA签名和验证
	/// </summary> 
	public class RsaCryption 
	{ 		
		public RsaCryption() 
		{ 			
		} 
		

		#region RSA 加密解密 

		#region RSA 的密钥产生 
	
		/// <summary>
		/// RSA 的密钥产生 产生私钥 和公钥 
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

		#region RSA的加密函数 
		//############################################################################## 
		//RSA 方式加密 
		//说明KEY必须是XML的行式,返回的是字符串 
		//在有一点需要说明！！该加密方式有 长度 限制的！！ 
		//############################################################################## 

		//RSA的加密函数  string
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
		//RSA的加密函数 byte[]
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

		#region RSA的解密函数 
		//RSA的解密函数  string
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

		//RSA的解密函数  byte
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

		#region RSA数字签名 

		#region 获取Hash描述表 
		//获取Hash描述表 
		public bool GetHash(string mStrSource, ref byte[] hashData) 
		{ 			
			//从字符串中取得Hash描述 
			byte[] buffer; 
			var md5 = System.Security.Cryptography.HashAlgorithm.Create("MD5"); 
			buffer = System.Text.Encoding.GetEncoding("GB2312").GetBytes(mStrSource); 
			hashData = md5.ComputeHash(buffer); 

			return true; 			
		} 

		//获取Hash描述表 
		public bool GetHash(string mStrSource, ref string strHashData) 
		{ 
			
			//从字符串中取得Hash描述 
			byte[] buffer; 
			byte[] hashData; 
			var md5 = System.Security.Cryptography.HashAlgorithm.Create("MD5"); 
			buffer = System.Text.Encoding.GetEncoding("GB2312").GetBytes(mStrSource); 
			hashData = md5.ComputeHash(buffer); 

			strHashData = Convert.ToBase64String(hashData); 
			return true; 
			
		} 

		//获取Hash描述表 
		public bool GetHash(System.IO.FileStream objFile, ref byte[] hashData) 
		{ 
			
			//从文件中取得Hash描述 
			var md5 = System.Security.Cryptography.HashAlgorithm.Create("MD5"); 
			hashData = md5.ComputeHash(objFile); 
			objFile.Close(); 

			return true; 
			
		} 

		//获取Hash描述表 
		public bool GetHash(System.IO.FileStream objFile, ref string strHashData) 
		{ 
			
			//从文件中取得Hash描述 
			byte[] hashData; 
			var md5 = System.Security.Cryptography.HashAlgorithm.Create("MD5"); 
			hashData = md5.ComputeHash(objFile); 
			objFile.Close(); 

			strHashData = Convert.ToBase64String(hashData); 

			return true; 
			
		} 
		#endregion 

		#region RSA签名 
		//RSA签名 
		public bool SignatureFormatter(string pStrKeyPrivate, byte[] hashbyteSignature, ref byte[] encryptedSignatureData) 
		{ 
			
				var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(); 

				rsa.FromXmlString(pStrKeyPrivate); 
				var rsaFormatter = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(rsa); 
				//设置签名的算法为MD5 
				rsaFormatter.SetHashAlgorithm("MD5"); 
				//执行签名 
				encryptedSignatureData = rsaFormatter.CreateSignature(hashbyteSignature); 

				return true; 
			
		} 

		//RSA签名 
		public bool SignatureFormatter(string pStrKeyPrivate, byte[] hashbyteSignature, ref string mStrEncryptedSignatureData) 
		{ 
			
				byte[] encryptedSignatureData; 

				var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(); 

				rsa.FromXmlString(pStrKeyPrivate); 
				var rsaFormatter = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(rsa); 
				//设置签名的算法为MD5 
				rsaFormatter.SetHashAlgorithm("MD5"); 
				//执行签名 
				encryptedSignatureData = rsaFormatter.CreateSignature(hashbyteSignature); 

				mStrEncryptedSignatureData = Convert.ToBase64String(encryptedSignatureData); 

				return true; 
			
		} 

		//RSA签名 
		public bool SignatureFormatter(string pStrKeyPrivate, string mStrHashbyteSignature, ref byte[] encryptedSignatureData) 
		{ 
			
				byte[] hashbyteSignature; 

				hashbyteSignature = Convert.FromBase64String(mStrHashbyteSignature); 
				var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(); 

				rsa.FromXmlString(pStrKeyPrivate); 
				var rsaFormatter = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(rsa); 
				//设置签名的算法为MD5 
				rsaFormatter.SetHashAlgorithm("MD5"); 
				//执行签名 
				encryptedSignatureData = rsaFormatter.CreateSignature(hashbyteSignature); 

				return true; 
			
		} 

		//RSA签名 
		public bool SignatureFormatter(string pStrKeyPrivate, string mStrHashbyteSignature, ref string mStrEncryptedSignatureData) 
		{ 
			
				byte[] hashbyteSignature; 
				byte[] encryptedSignatureData; 

				hashbyteSignature = Convert.FromBase64String(mStrHashbyteSignature); 
				var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(); 

				rsa.FromXmlString(pStrKeyPrivate); 
				var rsaFormatter = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(rsa); 
				//设置签名的算法为MD5 
				rsaFormatter.SetHashAlgorithm("MD5"); 
				//执行签名 
				encryptedSignatureData = rsaFormatter.CreateSignature(hashbyteSignature); 

				mStrEncryptedSignatureData = Convert.ToBase64String(encryptedSignatureData); 

				return true; 
			
		} 
		#endregion 

		#region RSA 签名验证 

		public bool SignatureDeformatter(string pStrKeyPublic, byte[] hashbyteDeformatter, byte[] deformatterData) 
		{ 
			
				var rsa = new System.Security.Cryptography.RSACryptoServiceProvider(); 

				rsa.FromXmlString(pStrKeyPublic); 
				var rsaDeformatter = new System.Security.Cryptography.RSAPKCS1SignatureDeformatter(rsa); 
				//指定解密的时候HASH算法为MD5 
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
				//指定解密的时候HASH算法为MD5 
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
				//指定解密的时候HASH算法为MD5 
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
				//指定解密的时候HASH算法为MD5 
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
