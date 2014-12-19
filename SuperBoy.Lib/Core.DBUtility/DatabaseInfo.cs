using System;
using System.Text;

namespace Core.DBUtility
{
	/// <summary>
	/// DatabaseInfo 的摘要说明。
	/// </summary>
	public class DatabaseInfo
	{
		public DatabaseInfo()
		{
		}

		/// <summary>
		/// 可以接受三种格式的数据库连接字符串
		/// 1. 服务名称=(local);数据库名称=EDNSM;用户名称=sa;用户密码=123456
		/// 2. Data Source=(local);Initial Catalog=EDNSM;User ID=sa;Password=123456
		/// 3. server=(local);uid=sa;pwd=;
		/// </summary>
		/// <param name="connectionString"></param>
		public DatabaseInfo(string connectionString)
		{
			#region 服务器名

			this._server = this.GetSubItemValue(connectionString, "服务名称");
			if (this._server == null)
			{
				this._server = this.GetSubItemValue(connectionString, "Data Source");
			}
			if (this._server == null)
			{
				this._server = this.GetSubItemValue(connectionString, "server");
			}

			#endregion

			#region 数据库名

			this._database = this.GetSubItemValue(connectionString, "数据库名称");
			if (this._database == null)
			{
				this._database = this.GetSubItemValue(connectionString, "Initial Catalog");
			}
			if (this._database == null)
			{
				this._database = this.GetSubItemValue(connectionString, "database");
			}

			#endregion

			#region 用户名称

			this._userId = this.GetSubItemValue(connectionString, "用户名称");
			if (this._userId == null)
			{
				this._userId = this.GetSubItemValue(connectionString, "User ID");
			}
			if (this._userId == null)
			{
				this._userId = this.GetSubItemValue(connectionString, "uid");
			}

			#endregion

			#region 用户密码

			this._password = this.GetSubItemValue(connectionString, "用户密码");
			if (this._password == null)
			{
				this._password = this.GetSubItemValue(connectionString, "Password");
			}
			if (this._password == null)
			{
				this._password = this.GetSubItemValue(connectionString, "pwd");
			}

			#endregion
		}

		#region 变量及属性

		public string Server
		{
			get { return _server; }
			set { this._server = value; }
		}

		public string Database
		{
			get { return _database; }
			set { this._database = value; }
		}

		public string UserId
		{
			get { return _userId; }
			set { this._userId = value; }
		}

		public string Password
		{
			get { return _password; }
			set { this._password = value; }
		}

		private string _server;
		private string _database;
		private string _userId;
		private string _password;

		#endregion

		/// <summary>
		/// 加密后的连接字符串
		/// </summary>
		public string EncryptConnectionString
		{
			get { return EncodeBase64(this.ConnectionString); }
		}


		/// <summary>
		/// 没有加密的字符串
		/// </summary>
		public string ConnectionString
		{
			get
			{
                var connString = "";
                if (!string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(Password))
                {
                    connString = string.Format("Persist Security Info=False;Data Source={0};Initial Catalog={1};User ID={2};Password={3};Packet Size=4096;Pooling=true;Max Pool Size=100;Min Pool Size=1",
                                                  this._server, this._database, this._userId, this._password);
                }
                else
                {
                    connString = string.Format("Persist Security Info=False;Data Source={0};Initial Catalog={1};Integrated Security=SSPI;Packet Size=4096;Pooling=true;Max Pool Size=100;Min Pool Size=1",
                                                  this._server, this._database);
                }
				return connString;
			}
		}

		/// <summary>
		/// 提供OLEDB数据源的链接字符串
		/// </summary>
		public string OleDbConnectionString
		{
			get
			{
			    const string connectionPrefix = "Driver={SQL Server};";
			    return connectionPrefix + this.ConnectionString;
			}
		}

		#region 辅助函数

		/// <summary>
		/// 获取给定字符串中的子节点的值, 如果不存在返回Null
		/// </summary>
		/// <param name="itemValueString">多个值的字符串</param>
		/// <param name="subKeyName"></param>
		/// <returns></returns>
		private string GetSubItemValue(string itemValueString, string subKeyName)
		{
			var item = itemValueString.Split(new char[] {';'});
			for (var i = 0; i < item.Length; i++)
			{
				var itemValue = item[i].ToLower();
				if (itemValue.IndexOf(subKeyName.ToLower()) >= 0) //如果含有指定的关键字
				{
					var startIndex = item[i].IndexOf("="); //等号开始的位置
					return item[i].Substring(startIndex + 1); //获取等号后面的值即为Value
				}
			}
			return null;
		}


		private string EncodeBase64(string source)
		{
			var buffer1 = Encoding.UTF8.GetBytes(source);
			return Convert.ToBase64String(buffer1, 0, buffer1.Length);
		}

		#endregion
	}
}