using System;
using System.Data;

namespace Core.DBUtility
{
    /// <summary>
    /// 转换IDataReader字段对象的格式辅助类
    /// 可以转换有默认值、可空类型的字段数据
    /// </summary>
    public sealed class SmartDataReader
    {
        private DateTime _defaultDate;
        private IDataReader _reader;

        /// <summary>
        /// 构造函数，传入IDataReader对象
        /// </summary>
        /// <param name="reader"></param>
        public SmartDataReader(IDataReader reader)
        {
            this._defaultDate = Convert.ToDateTime("01/01/1970 00:00:00");
            this._reader = reader;
        }

        /// <summary>
        /// 继续读取下一个操作
        /// </summary>
        public bool Read()
        {
            return this._reader.Read();
        }
        
        /// <summary>
        /// 转换为Int类型数据
        /// </summary>
        public int GetInt32(string column)
        {
            return GetInt32(column, 0);
        }

        /// <summary>
        /// 转换为Int类型数据
        /// </summary>
        public int GetInt32(string column, int defaultIfNull)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? (int)defaultIfNull : int.Parse(_reader[column].ToString());
            return data;
        }

        /// <summary>
        /// 转换为Int类型数据
        /// </summary>
        public int? GetInt32Nullable(string column)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? (int?)null : int.Parse(_reader[column].ToString());
            return data;
        }

        /// <summary>
        /// 转换为Int16类型数据
        /// </summary>
        public short GetInt16(string column)
        {
            return GetInt16(column, 0);
        }

        /// <summary>
        /// 转换为Int16类型数据
        /// </summary>
        public short GetInt16(string column, short defaultIfNull)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? defaultIfNull : short.Parse(_reader[column].ToString());
            return data;
        }

        /// <summary>
        /// 转换为Int16类型数据
        /// </summary>
        public short? GetInt16Nullable(string column)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? (short?)null : short.Parse(_reader[column].ToString());
            return data;
        }

        /// <summary>
        /// 转换为Float类型数据
        /// </summary>
        public float GetFloat(string column)
        {
            return GetFloat(column, 0);
        }

        /// <summary>
        /// 转换为Float类型数据
        /// </summary>
        public float GetFloat(string column, float defaultIfNull)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? defaultIfNull : float.Parse(_reader[column].ToString());
            return data;
        }

        /// <summary>
        /// 转换为Float类型数据
        /// </summary>
        public float? GetFloatNullable(string column)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? (float?)null : float.Parse(_reader[column].ToString());
            return data;
        }

        /// <summary>
        /// 转换为Double类型数据
        /// </summary>
        public double GetDouble(string column)
        {
            return GetDouble(column, 0);
        }

        /// <summary>
        /// 转换为Double类型数据
        /// </summary>
        public double GetDouble(string column, double defaultIfNull)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? defaultIfNull : double.Parse(_reader[column].ToString());
            return data;
        }

        /// <summary>
        /// 转换为Double类型数据(可空类型）
        /// </summary>
        public double? GetDoubleNullable(string column)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? (double?)null : double.Parse(_reader[column].ToString());
            return data;
        }

        /// <summary>
        /// 转换为Decimal类型数据
        /// </summary>
        public decimal GetDecimal(string column)
        {
            return GetDecimal(column, 0);
        }

        /// <summary>
        /// 转换为Decimal类型数据
        /// </summary>
        public decimal GetDecimal(string column, decimal defaultIfNull)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? defaultIfNull : decimal.Parse(_reader[column].ToString());
            return data;
        }

        /// <summary>
        /// 转换为Decimal类型数据(可空类型）
        /// </summary>
        public decimal? GetDecimalNullable(string column)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? (decimal?)null : decimal.Parse(_reader[column].ToString());
            return data;
        }

        /// <summary>
        /// 转换为Single类型数据
        /// </summary>
        public Single GetSingle(string column)
        {
            return GetSingle(column, 0);
        }

        /// <summary>
        /// 转换为Single类型数据
        /// </summary>
        public Single GetSingle(string column, Single defaultIfNull)
		{
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? defaultIfNull : Single.Parse(_reader[column].ToString());
			return data;
		}

        /// <summary>
        /// 转换为Single类型数据(可空类型）
        /// </summary>
        public Single? GetSingleNullable(string column)
		{
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? (Single?)null : Single.Parse(_reader[column].ToString());
			return data;
		}

        /// <summary>
        /// 转换为布尔类型数据
        /// </summary>
        public bool GetBoolean(string column)
        {
            return GetBoolean(column, false);
        }

        /// <summary>
        /// 转换为布尔类型数据
        /// </summary>
        public bool GetBoolean(string column, bool defaultIfNull)
        {
            var str = _reader[column].ToString();
            try
            {
                var i = Convert.ToInt32(str);
                return i > 0;
            }
            catch { }

            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? defaultIfNull : bool.Parse(str);
            return data;
        }

        /// <summary>
        /// 转换为布尔类型数据(可空类型）
        /// </summary>
        public bool? GetBooleanNullable(string column)
        {
            var str = _reader[column].ToString();
            try
            {
                var i = Convert.ToInt32(str);
                return i > 0;
            }
            catch { }

            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? (bool?)null : bool.Parse(str);
            return data;
        }

        /// <summary>
        /// 转换为字符串类型数据
        /// </summary>
        public String GetString(string column)
        {
            return GetString(column, "");
        }

        /// <summary>
        /// 转换为字符串类型数据
        /// </summary>
        public string GetString(string column, string defaultIfNull)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? defaultIfNull : _reader[column].ToString();
            return data;
        }

        /// <summary>
        /// 转换为Byte字节数据类型数据
        /// </summary>
        public byte[] GetBytes(string column)
        {
            return GetBytes(column, null);
        }

        /// <summary>
        /// 转换为Byte字节数据类型数据
        /// </summary>
        public byte[] GetBytes(string column, string defaultIfNull)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? defaultIfNull : _reader[column].ToString();
            return System.Text.Encoding.UTF8.GetBytes(data);
        }

        /// <summary>
        /// 转换为Guid类型数据
        /// </summary>
        public Guid GetGuid(string column)
        {
            return GetGuid(column, null);
        }

        /// <summary>
        /// 转换为Guid类型数据
        /// </summary>
        public Guid GetGuid(string column, string defaultIfNull)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? defaultIfNull : _reader[column].ToString();
            var guid = Guid.Empty;
            if (data != null)
            {
                guid = new Guid(data);             
            }
            return guid;
        }

        /// <summary>
        /// 转换为Guid类型数据(可空类型）
        /// </summary> 
        public Guid? GetGuidNullable(string column)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? null : _reader[column].ToString();
            Guid? guid = null;
            if (data != null)
            {
                guid = new Guid(data);
            }
            return guid;
        }

        /// <summary>
        /// 转换为DateTime类型数据
        /// </summary>
        public DateTime GetDateTime(string column)
        {
            return GetDateTime(column, _defaultDate);
        }

        /// <summary>
        /// 转换为DateTime类型数据
        /// </summary>
        public DateTime GetDateTime(string column, DateTime defaultIfNull)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? defaultIfNull : Convert.ToDateTime(_reader[column].ToString());
            return data;
        }

        /// <summary>
        /// 转换为可空DateTime类型数据
        /// </summary>
        public DateTime? GetDateTimeNullable(string column)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? (DateTime?)null : Convert.ToDateTime(_reader[column].ToString());
            return data;
        }
    }

}
