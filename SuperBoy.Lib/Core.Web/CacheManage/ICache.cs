using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace Core.Web.CacheManage
{
    /// <summary>
    /// ������ӿ�
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// ��ȡһ��ֵ,ָʾ��ǰ�Ļ����Ƿ����
        /// </summary>
        bool EnableCache
        {
            get;
        }

        /// <summary>
        /// ��ȡ���������
        /// </summary>
        CacheType Type
        {
            get;
        }

        /// <summary>
        /// ��黺�����Ƿ����ָ���ļ�
        /// </summary>
        /// <param name="key">Ҫ���ļ�</param>
        /// <returns>����һ��ֵ,ָʾ���ļ��Ƿ����</returns>
        bool Contains(string key);


        /// <summary>
        /// ���ϵͳ���Ƿ����ָ���Ļ���
        /// </summary>
        /// <typeparam name="T">����</typeparam>
        /// <param name="key">����key</param>
        /// <returns>����������͵�ֵ�Ƿ����</returns>
        bool Contains<T>(string key);


        /// <summary>
        /// �ӻ����л�ȡָ������ֵ
        /// </summary>
        /// <param name="key">Ҫ��ȡ�ļ�</param>
        /// <returns>����ָ������ֵ</returns>
        T Get<T>(string key);

        /// <summary>
        /// ��ȡ�����м�ֵ������
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// ��ӻ���
        /// </summary>
        /// <param name="key">�ؼ���</param>
        /// <param name="value">����ֵ</param>
        /// <returns>������ӵļ�ֵ</returns>
        void Add<T>(string key, T value);

        /// <summary>
        /// ��ӻ���
        /// </summary>
        /// <param name="key">�ؼ���</param>
        /// <param name="value">����ֵ</param>
        /// <param name="absoluteExpiration">����ʱ��</param>
        /// <returns>������ӵļ�ֵ</returns>
        void Add<T>(string key, T value, DateTime absoluteExpiration);

        /// <summary>
        /// ��ӻ���
        /// </summary>
        /// <param name="key">�ؼ���</param>
        /// <param name="value">����ֵ</param>
        /// <param name="slidingExpiration">����ʱ��</param>
        /// <returns>������ӵļ�ֵ</returns>
        void Add<T>(string key, T value, TimeSpan slidingExpiration);

        /// <summary>
        /// ��ӻ���
        /// </summary>
        /// <param name="key">�ؼ���</param>
        /// <param name="value">����ֵ</param>
        /// <param name="minutes">����ʱ��(����)</param>
        /// <returns>������ӵļ�ֵ</returns>
        void Add<T>(string key, T value, int minutes);

        /// <summary>
        /// ��ӻ���
        /// </summary>
        /// <param name="key">�ؼ���</param>
        /// <param name="value">����ֵ</param>
        /// <param name="priority">���ȼ�</param>
        /// <param name="slidingExpiration">����ʱ��</param>
        void Add<T>(string key, T value, CachePriority priority, TimeSpan slidingExpiration);

        /// <summary>
        /// ��ӻ���
        /// </summary>
        /// <param name="key">�ؼ���</param>
        /// <param name="value">����ֵ</param>
        /// <param name="priority">���ȼ�</param>
        /// <param name="absoluteExpiration">����ʱ��</param>
        void Add<T>(string key, T value, CachePriority priority, DateTime absoluteExpiration);


        /// <summary>
        /// ���Է���ָ���Ļ���
        /// </summary>
        /// <typeparam name="T">�������ݵ�����</typeparam>
        /// <param name="key">�����key</param>
        /// <param name="value">���������</param>
        /// <returns>�Ƿ�����������</returns>
        bool TryGetValue<T>(string key, out T value);

       


        /// <summary>
        /// �Ƴ�����ĳ�ؼ��ֵĻ��沢������Ӧ��ֵ
        /// </summary>
        /// <param name="key">�ؼ���</param>
        void Remove(string key);

        /// <summary>
        /// �Ƴ����д�ĳ�ؼ��ֵĻ���
        /// </summary>
        /// <param name="key">�ؼ���</param>
        /// <returns>���ر��Ƴ����������</returns>
        int RemoveContains(string key);

        /// <summary>
        /// �Ƴ�������ĳ�ؼ��ֿ�ͷ�Ļ���
        /// </summary>
        /// <param name="key">�ؼ���</param>
        /// <returns>���ر��Ƴ����������</returns>
        int RemoveStartWith(string key);

        /// <summary>
        /// �Ƴ�������ĳ�ؼ��ֽ�β�Ļ���
        /// </summary>
        /// <param name="key">�ؼ���</param>
        /// <returns>���ر��Ƴ����������</returns>
        int RemoveEndWith(string key);

        /// <summary>
        /// �Ƴ��������еĻ���
        /// </summary>
        /// <returns>���ر��Ƴ����������</returns>
        int Clear();

        /// <summary>
        /// ���������еļ��б�
        /// </summary>
        ReadOnlyCollection<string> Keys
        {
            get;
        }
    }

    /// <summary>
    /// ��������ȼ�
    /// </summary>
    public enum CachePriority
    {
        /// <summary>
        /// ��
        /// </summary>
        None = 0,

        /// <summary>
        /// ��
        /// </summary>
        Low = 1,

        /// <summary>
        /// ��ͨ
        /// </summary>
        Normal = 2,

        /// <summary>
        /// ��
        /// </summary>
        High = 3,

        /// <summary>
        /// ���ܱ�ɾ��
        /// </summary>
        NotRemovable = 4,
    }
}

