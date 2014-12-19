using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Core.IO
{ 
    /// <summary>
    /// INI文件操作辅助类
    /// </summary>
    public class IniFileHelper
    {
         public string Path;

        /// <summary>
        /// 传入INI文件路径构造对象
        /// </summary>
        /// <param name="iniPath">INI文件路径</param>
         public IniFileHelper(string iniPath)
		{
			Path = iniPath;
		}

		[DllImport("kernel32")]
		private static extern long WritePrivateProfileString(string section,string key,string val,string filePath);

		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string section,string key,string def, StringBuilder retVal,int size,string filePath);

	
		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string section, string key, string defVal, Byte[] retVal, int size, string filePath);


		/// <summary>
		/// 写INI文件
		/// </summary>
		/// <param name="section">分组节点</param>
		/// <param name="key">关键字</param>
		/// <param name="value">值</param>
		public void IniWriteValue(string section,string key,string value)
		{
			WritePrivateProfileString(section,key,value,this.Path);
		}

		/// <summary>
		/// 读取INI文件
		/// </summary>
		/// <param name="section">分组节点</param>
		/// <param name="key">关键字</param>
		/// <returns></returns>
		public string IniReadValue(string section,string key)
		{
			var temp = new StringBuilder(255);
			var i = GetPrivateProfileString(section,key,"",temp, 255, this.Path);
			return temp.ToString();
		}

		public byte[] IniReadValues(string section, string key)
		{
			var temp = new byte[255];
			var i = GetPrivateProfileString(section, key, "", temp, 255, this.Path);
			return temp;

		}

		/// <summary>
		/// 删除ini文件下所有段落
		/// </summary>
		public void ClearAllSection()
		{
			IniWriteValue(null,null,null);
		}

		/// <summary>
		/// 删除ini文件下指定段落下的所有键
		/// </summary>
		/// <param name="section"></param>
		public void ClearSection(string section)
		{
			IniWriteValue(section,null,null);
		}
    }
}
