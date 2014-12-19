using System;
using System.Text;

namespace Core.DBUtility
{
	/// <summary>
	/// DatabaseInfo ��ժҪ˵����
	/// </summary>
	public class DatabaseInfo
	{
		public DatabaseInfo()
		{
		}

		/// <summary>
		/// ���Խ������ָ�ʽ�����ݿ������ַ���
		/// 1. ��������=(local);���ݿ�����=EDNSM;�û�����=sa;�û�����=123456
		/// 2. Data Source=(local);Initial Catalog=EDNSM;User ID=sa;Password=123456
		/// 3. server=(local);uid=sa;pwd=;
		/// </summary>
		/// <param name="connectionString"></param>
		public DatabaseInfo(string connectionString)
		{
			#region ��������

			this._server = this.GetSubItemValue(connectionString, "��������");
			if (this._server == null)
			{
				this._server = this.GetSubItemValue(connectionString, "Data Source");
			}
			if (this._server == null)
			{
				this._server = this.GetSubItemValue(connectionString, "server");
			}

			#endregion

			#region ���ݿ���

			this._database = this.GetSubItemValue(connectionString, "���ݿ�����");
			if (this._database == null)
			{
				this._database = this.GetSubItemValue(connectionString, "Initial Catalog");
			}
			if (this._database == null)
			{
				this._database = this.GetSubItemValue(connectionString, "database");
			}

			#endregion

			#region �û�����

			this._userId = this.GetSubItemValue(connectionString, "�û�����");
			if (this._userId == null)
			{
				this._userId = this.GetSubItemValue(connectionString, "User ID");
			}
			if (this._userId == null)
			{
				this._userId = this.GetSubItemValue(connectionString, "uid");
			}

			#endregion

			#region �û�����

			this._password = this.GetSubItemValue(connectionString, "�û�����");
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

		#region ����������

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
		/// ���ܺ�������ַ���
		/// </summary>
		public string EncryptConnectionString
		{
			get { return EncodeBase64(this.ConnectionString); }
		}


		/// <summary>
		/// û�м��ܵ��ַ���
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
		/// �ṩOLEDB����Դ�������ַ���
		/// </summary>
		public string OleDbConnectionString
		{
			get
			{
			    const string connectionPrefix = "Driver={SQL Server};";
			    return connectionPrefix + this.ConnectionString;
			}
		}

		#region ��������

		/// <summary>
		/// ��ȡ�����ַ����е��ӽڵ��ֵ, ��������ڷ���Null
		/// </summary>
		/// <param name="itemValueString">���ֵ���ַ���</param>
		/// <param name="subKeyName"></param>
		/// <returns></returns>
		private string GetSubItemValue(string itemValueString, string subKeyName)
		{
			var item = itemValueString.Split(new char[] {';'});
			for (var i = 0; i < item.Length; i++)
			{
				var itemValue = item[i].ToLower();
				if (itemValue.IndexOf(subKeyName.ToLower()) >= 0) //�������ָ���Ĺؼ���
				{
					var startIndex = item[i].IndexOf("="); //�Ⱥſ�ʼ��λ��
					return item[i].Substring(startIndex + 1); //��ȡ�Ⱥź����ֵ��ΪValue
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