using System;
using System.Data;

namespace Core.DBUtility
{
    /// <summary>
    /// ת��IDataReader�ֶζ���ĸ�ʽ������
    /// ����ת����Ĭ��ֵ���ɿ����͵��ֶ�����
    /// </summary>
    public sealed class SmartDataReader
    {
        private DateTime _defaultDate;
        private IDataReader _reader;

        /// <summary>
        /// ���캯��������IDataReader����
        /// </summary>
        /// <param name="reader"></param>
        public SmartDataReader(IDataReader reader)
        {
            this._defaultDate = Convert.ToDateTime("01/01/1970 00:00:00");
            this._reader = reader;
        }

        /// <summary>
        /// ������ȡ��һ������
        /// </summary>
        public bool Read()
        {
            return this._reader.Read();
        }
        
        /// <summary>
        /// ת��ΪInt��������
        /// </summary>
        public int GetInt32(string column)
        {
            return GetInt32(column, 0);
        }

        /// <summary>
        /// ת��ΪInt��������
        /// </summary>
        public int GetInt32(string column, int defaultIfNull)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? (int)defaultIfNull : int.Parse(_reader[column].ToString());
            return data;
        }

        /// <summary>
        /// ת��ΪInt��������
        /// </summary>
        public int? GetInt32Nullable(string column)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? (int?)null : int.Parse(_reader[column].ToString());
            return data;
        }

        /// <summary>
        /// ת��ΪInt16��������
        /// </summary>
        public short GetInt16(string column)
        {
            return GetInt16(column, 0);
        }

        /// <summary>
        /// ת��ΪInt16��������
        /// </summary>
        public short GetInt16(string column, short defaultIfNull)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? defaultIfNull : short.Parse(_reader[column].ToString());
            return data;
        }

        /// <summary>
        /// ת��ΪInt16��������
        /// </summary>
        public short? GetInt16Nullable(string column)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? (short?)null : short.Parse(_reader[column].ToString());
            return data;
        }

        /// <summary>
        /// ת��ΪFloat��������
        /// </summary>
        public float GetFloat(string column)
        {
            return GetFloat(column, 0);
        }

        /// <summary>
        /// ת��ΪFloat��������
        /// </summary>
        public float GetFloat(string column, float defaultIfNull)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? defaultIfNull : float.Parse(_reader[column].ToString());
            return data;
        }

        /// <summary>
        /// ת��ΪFloat��������
        /// </summary>
        public float? GetFloatNullable(string column)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? (float?)null : float.Parse(_reader[column].ToString());
            return data;
        }

        /// <summary>
        /// ת��ΪDouble��������
        /// </summary>
        public double GetDouble(string column)
        {
            return GetDouble(column, 0);
        }

        /// <summary>
        /// ת��ΪDouble��������
        /// </summary>
        public double GetDouble(string column, double defaultIfNull)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? defaultIfNull : double.Parse(_reader[column].ToString());
            return data;
        }

        /// <summary>
        /// ת��ΪDouble��������(�ɿ����ͣ�
        /// </summary>
        public double? GetDoubleNullable(string column)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? (double?)null : double.Parse(_reader[column].ToString());
            return data;
        }

        /// <summary>
        /// ת��ΪDecimal��������
        /// </summary>
        public decimal GetDecimal(string column)
        {
            return GetDecimal(column, 0);
        }

        /// <summary>
        /// ת��ΪDecimal��������
        /// </summary>
        public decimal GetDecimal(string column, decimal defaultIfNull)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? defaultIfNull : decimal.Parse(_reader[column].ToString());
            return data;
        }

        /// <summary>
        /// ת��ΪDecimal��������(�ɿ����ͣ�
        /// </summary>
        public decimal? GetDecimalNullable(string column)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? (decimal?)null : decimal.Parse(_reader[column].ToString());
            return data;
        }

        /// <summary>
        /// ת��ΪSingle��������
        /// </summary>
        public Single GetSingle(string column)
        {
            return GetSingle(column, 0);
        }

        /// <summary>
        /// ת��ΪSingle��������
        /// </summary>
        public Single GetSingle(string column, Single defaultIfNull)
		{
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? defaultIfNull : Single.Parse(_reader[column].ToString());
			return data;
		}

        /// <summary>
        /// ת��ΪSingle��������(�ɿ����ͣ�
        /// </summary>
        public Single? GetSingleNullable(string column)
		{
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? (Single?)null : Single.Parse(_reader[column].ToString());
			return data;
		}

        /// <summary>
        /// ת��Ϊ������������
        /// </summary>
        public bool GetBoolean(string column)
        {
            return GetBoolean(column, false);
        }

        /// <summary>
        /// ת��Ϊ������������
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
        /// ת��Ϊ������������(�ɿ����ͣ�
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
        /// ת��Ϊ�ַ�����������
        /// </summary>
        public String GetString(string column)
        {
            return GetString(column, "");
        }

        /// <summary>
        /// ת��Ϊ�ַ�����������
        /// </summary>
        public string GetString(string column, string defaultIfNull)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? defaultIfNull : _reader[column].ToString();
            return data;
        }

        /// <summary>
        /// ת��ΪByte�ֽ�������������
        /// </summary>
        public byte[] GetBytes(string column)
        {
            return GetBytes(column, null);
        }

        /// <summary>
        /// ת��ΪByte�ֽ�������������
        /// </summary>
        public byte[] GetBytes(string column, string defaultIfNull)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? defaultIfNull : _reader[column].ToString();
            return System.Text.Encoding.UTF8.GetBytes(data);
        }

        /// <summary>
        /// ת��ΪGuid��������
        /// </summary>
        public Guid GetGuid(string column)
        {
            return GetGuid(column, null);
        }

        /// <summary>
        /// ת��ΪGuid��������
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
        /// ת��ΪGuid��������(�ɿ����ͣ�
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
        /// ת��ΪDateTime��������
        /// </summary>
        public DateTime GetDateTime(string column)
        {
            return GetDateTime(column, _defaultDate);
        }

        /// <summary>
        /// ת��ΪDateTime��������
        /// </summary>
        public DateTime GetDateTime(string column, DateTime defaultIfNull)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? defaultIfNull : Convert.ToDateTime(_reader[column].ToString());
            return data;
        }

        /// <summary>
        /// ת��Ϊ�ɿ�DateTime��������
        /// </summary>
        public DateTime? GetDateTimeNullable(string column)
        {
            var data = (_reader.IsDBNull(_reader.GetOrdinal(column))) ? (DateTime?)null : Convert.ToDateTime(_reader[column].ToString());
            return data;
        }
    }

}
