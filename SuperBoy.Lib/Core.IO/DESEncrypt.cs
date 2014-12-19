using System;
using System.Security.Cryptography;  
using System.Text;
namespace Core.IO
{
	/// <summary>
	/// DES加密/解密类。
	/// </summary>
	public class DesEncrypt
	{
		public DesEncrypt()
		{			
		}

		#region ========加密======== 

	    /// <summary> 
		/// 加密数据 
		/// </summary> 
		/// <param name="text"></param> 
		/// <param name="sKey"></param> 
		/// <returns></returns> 
		public static string Encrypt(string text,string sKey = "MATICSOFT") 
		{ 
			var des = new DESCryptoServiceProvider();
	        var inputByteArray = Encoding.Default.GetBytes(text);
	        var hashPasswordForStoringInConfigFile = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5");
	        if (hashPasswordForStoringInConfigFile != null)
	            des.Key = Encoding.ASCII.GetBytes(hashPasswordForStoringInConfigFile.Substring(0, 8));
	        var passwordForStoringInConfigFile = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5");
	        if (passwordForStoringInConfigFile != null)
	            des.IV = Encoding.ASCII.GetBytes(passwordForStoringInConfigFile.Substring(0, 8));
	        var ms=new System.IO.MemoryStream(); 
			var cs=new CryptoStream(ms,des.CreateEncryptor(),CryptoStreamMode.Write); 
			cs.Write(inputByteArray,0,inputByteArray.Length); 
			cs.FlushFinalBlock(); 
			var ret=new StringBuilder(); 
			foreach( var b in ms.ToArray()) 
			{ 
				ret.AppendFormat("{0:X2}",b); 
			} 
			return ret.ToString(); 
		} 

		#endregion
		
		#region ========解密======== 

	    /// <summary> 
		/// 解密数据 
		/// </summary> 
		/// <param name="text"></param> 
		/// <param name="sKey"></param> 
		/// <returns></returns> 
		public static string Decrypt(string text,string sKey = "MATICSOFT") 
		{ 
			var des = new DESCryptoServiceProvider();
	        var len = text.Length/2; 
			var inputByteArray = new byte[len]; 
			int x,i; 
			for(x=0;x<len;x++) 
			{ 
				i = Convert.ToInt32(text.Substring(x * 2, 2), 16); 
				inputByteArray[x]=(byte)i; 
			}
	        var hashPasswordForStoringInConfigFile = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5");
	        if (hashPasswordForStoringInConfigFile != null)
	            des.Key = Encoding.ASCII.GetBytes(hashPasswordForStoringInConfigFile.Substring(0, 8));
	        var passwordForStoringInConfigFile = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5");
	        if (passwordForStoringInConfigFile != null)
	            des.IV = Encoding.ASCII.GetBytes(passwordForStoringInConfigFile.Substring(0, 8));
	        var ms=new System.IO.MemoryStream(); 
			var cs=new CryptoStream(ms,des.CreateDecryptor(),CryptoStreamMode.Write); 
			cs.Write(inputByteArray,0,inputByteArray.Length); 
			cs.FlushFinalBlock(); 
			return Encoding.Default.GetString(ms.ToArray()); 
		} 
 
		#endregion 


	}

}
