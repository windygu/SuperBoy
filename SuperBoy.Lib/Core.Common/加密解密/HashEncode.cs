using System;
using System.Text;
using System.Security.Cryptography;
namespace Common
{
	/// <summary>
	/// �õ������ȫ�루��ϣ���ܣ���
	/// </summary>
	public class HashEncode
	{
		public HashEncode()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}
		/// <summary>
		/// �õ������ϣ�����ַ���
		/// </summary>
		/// <returns></returns>
		public static string GetSecurity()
		{			
			var security = HashEncoding(GetRandomValue());		
			return security;
		}
		/// <summary>
		/// �õ�һ�������ֵ
		/// </summary>
		/// <returns></returns>
		public static string GetRandomValue()
		{			
			var seed = new Random();
			var randomVaule = seed.Next(1, int.MaxValue).ToString();
			return randomVaule;
		}
		/// <summary>
		/// ��ϣ����һ���ַ���
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		public static string HashEncoding(string security)
		{						
			byte[] value;
			var code = new UnicodeEncoding();
			var message = code.GetBytes(security);
			var arithmetic = new SHA512Managed();
			value = arithmetic.ComputeHash(message);
			security = "";
			foreach(var o in value)
			{
				security += (int) o + "O";
			}
			return security;
		}
	}
}
