using System;
using System.IO;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Core.IO
{
    /// <summary>
    /// �����洢����������
    /// </summary>
    public sealed class IsolatedStorageHelper
    {
        #region ��������ʱ��ı������
        /// <summary>
        /// ���ܲ�����ָ��ʱ�䵽"���������ռ�" ���Էֺ�(;)׷�ӱ��棩
        /// </summary>
        public static void SaveDataTime()
        {
            SaveDataTime(DateTime.Now);
        }

        /// <summary>
        /// ���ܲ����浱ǰʱ�䵽"���������ռ�" ���Էֺ�(;)׷�ӱ��棩
        /// </summary>
        public static void SaveDataTime(DateTime fromDate)
        {
            var fromDataTime = fromDate.ToString("MM-dd-yyyy HH:mm:ss");
            var oldTime = GetDataTime().Trim();
            if (!string.IsNullOrEmpty(oldTime))
            {
                fromDataTime = oldTime + ";" + fromDataTime; //׷�����ʱ�䵽���
            }
            fromDataTime = DesEncrypt.Encrypt(fromDataTime, UiConstants.IsolatedStorageEncryptKey); //����

            #region ��fromDataTime������"���������ռ�"

            var username = fromDataTime;
            //���û����򡢳��򼯻�ȡ�����洢�� 
            var isoStore =
                IsolatedStorageFile.GetStore(
                    IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);
            var myusername = isoStore.GetDirectoryNames(UiConstants.IsolatedStorageDirectoryName);
            IsolatedStorageFileStream isoStream1 = null;
            if (myusername.Length == 0) //û��Ŀ¼ 
            {
                //����Ŀ¼ 
                isoStore.CreateDirectory(UiConstants.IsolatedStorageDirectoryName);
                //�����ļ� 
                using (isoStream1 = new IsolatedStorageFileStream(UiConstants.IsolatedStorage, FileMode.Create, isoStore))
                {
                    //д���ļ� 
                    using (var writer = new StreamWriter(isoStream1))
                    {
                        writer.WriteLine(fromDataTime);
                    }
                }
            }
            else
            {
                myusername = isoStore.GetFileNames(UiConstants.IsolatedStorage);
                if (myusername.Length == 0) //û���ļ� 
                {
                    //�����ļ� 
                    using (isoStream1 = new IsolatedStorageFileStream(UiConstants.IsolatedStorage, FileMode.Create, isoStore))
                    {
                        //д���ļ� 
                        using (var writer = new StreamWriter(isoStream1))
                        {
                            writer.WriteLine(fromDataTime);
                        }
                    }
                }
                else
                {
                    using (isoStream1 = new IsolatedStorageFileStream(UiConstants.IsolatedStorage, FileMode.Open, isoStore))
                    {
                        //д���ļ� 
                        using (var writer = new StreamWriter(isoStream1))
                        {
                            writer.WriteLine(fromDataTime);
                        }
                    }
                }
            }

            #endregion
        }

        /// <summary> 
        /// ��"���������ռ�"ȡ�����һ�����е�ʱ�䲢����
        /// </summary> 
        /// <returns></returns> 
        public static string GetDataTime()
        {
            string fromDataTime;

            //���û����򡢳��򼯻�ȡ�����洢�� 
            var isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User
                | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);

            var myusername = isoStore.GetDirectoryNames(UiConstants.IsolatedStorageDirectoryName);
            if (myusername.Length == 0) //û���ļ���
            {
                return string.Empty; //����û������Ŀ¼
            }

            myusername = isoStore.GetFileNames(UiConstants.IsolatedStorage);
            if (myusername.Length == 0) //û���ļ� 
            {
                return string.Empty; //����û�������û��� 
            }
            else
            {
                using (var isoStream1 = new IsolatedStorageFileStream(UiConstants.IsolatedStorage, FileMode.OpenOrCreate, isoStore))
                {
                    using (var reader = new StreamReader(isoStream1))
                    {
                        fromDataTime = reader.ReadLine();
                    }
                }
                if (!string.IsNullOrEmpty(fromDataTime)) //����
                {
                    try
                    {
                        fromDataTime = DesEncrypt.Decrypt(fromDataTime, UiConstants.IsolatedStorageEncryptKey);
                    }
                    catch
                    {
                    }
                }
                return fromDataTime;
            }
        } 
        #endregion

        #region ������������

        /// <summary>
        /// ������󵽶����洢��
        /// </summary>
        /// <param name="objectToSave">������Ķ���</param>
        /// <param name="key">����ļ�ֵ</param>
        public static void Save(object objectToSave, string key)
        {
            using (var store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User |
                IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, null, null))
            {
                using (var stream = new IsolatedStorageFileStream(key, FileMode.Create, FileAccess.Write, store))
                {
                    var serializer = new BinaryFormatter();
                    serializer.Serialize(stream, objectToSave);
                }
            }
        }

        /// <summary>
        /// ���ݼ�ֵ���ض����洢��������
        /// </summary>
        /// <param name="key">�����洢�ļ�ֵ(·����</param>
        /// <returns></returns>
        public static object Load(string key)
        {
            try
            {
                using (var store = IsolatedStorageFile.GetStore(IsolatedStorageScope.User |
                    IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, null, null))
                {
                    using (var stream = new IsolatedStorageFileStream(key, FileMode.Open, FileAccess.Read, store))
                    {
                        stream.Position = 0;
                        var deserializer = new BinaryFormatter();
                        return deserializer.Deserialize(stream);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (SerializationException)
            {
                return null;
            }
        }

        /// <summary>
        /// ���ش����û���ʶ��Χ��Ӧ�ó���Χ�ڵĴ洢ֵ
        /// </summary>
        /// <param name="d">�������ֵ����</param>
        /// <param name="filename">�ļ���</param>
        public static void LoadFromUserStoreForApplication(IDictionary d, string filename)
        {
            Load(d, IsolatedStorageScope.Application | IsolatedStorageScope.User, filename);
        }

        /// <summary>
        /// �������û���ʶ��Χ��Ӧ�ó���Χ�ڵ�ֵ
        /// </summary>
        /// <param name="d">��������ֵ����</param>
        /// <param name="filename">�ļ���</param>
        public static void SaveToUserStoreForApplication(IDictionary d, string filename)
        {
            Save(d, IsolatedStorageScope.Application | IsolatedStorageScope.User, filename);
        }

        /// <summary>
        /// �����û���Χ��Ӧ�÷�Χ�����򼯷�Χ�ڵĴ洢ֵ
        /// </summary>
        /// <param name="d">�������ֵ����.</param>
        /// <param name="filename">�ļ���</param>
        public static void LoadFromUserStoreForDomain(IDictionary d, string filename)
        {
            Load(d, IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain |
                IsolatedStorageScope.User, filename);
        }

        /// <summary>
        /// �����û���Χ��Ӧ�÷�Χ�����򼯷�Χ�ڵĴ洢ֵ
        /// </summary>
        /// <param name="d">��������ֵ����</param>
        /// <param name="filename">�ļ���</param>
        public static void SaveToUserStoreForDomain(IDictionary d, string filename)
        {
            Save(d, IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain |
                IsolatedStorageScope.User, filename);
        }

        /// <summary>
        /// �����ڶ����洢�ڵ�ָ���ļ�����
        /// </summary>
        /// <param name="d">�������ֵ�����</param>
        /// <param name="scope">�����洢��Χ����</param>
        /// <param name="filename">�ļ���</param>
        public static void Load(IDictionary d, IsolatedStorageScope scope, string filename)
        {
            d.Clear();
            using (var storage = IsolatedStorageFile.GetStore(scope, null, null))
            {
                var files = storage.GetFileNames(filename);
                if ((files.Length > 0) && (files[0] == filename))
                {
                    using (Stream stream =
                        new IsolatedStorageFileStream(filename, FileMode.OpenOrCreate, storage))
                    {
                        IFormatter formatter = new BinaryFormatter();
                        var data = (IDictionary)formatter.Deserialize(stream);
                        var enumerator = data.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            d.Add(enumerator.Key, enumerator.Value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// �ڶ����洢��Χ�ڱ����ֵ����ݵ�ָ���ļ�
        /// </summary>
        /// <param name="d">��������ֵ�����</param>
        /// <param name="scope">�����洢��Χ����</param>
        /// <param name="filename">�ļ���</param>
        public static void Save(IDictionary d, IsolatedStorageScope scope, string filename)
        {
            var storage = IsolatedStorageFile.GetStore(scope, null, null);
            using (var stream =
                new IsolatedStorageFileStream(filename, FileMode.Create, storage))
            {
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, d);
                }
            }
        }

        /// <summary>
        /// ɾ��ָ������Ĵ洢������
        /// </summary>
        /// <param name="fileName">��ɾ�����ļ�</param>
        /// <param name="scope">�����洢��Χ����</param>
        public static void Delete(string fileName, IsolatedStorageScope scope)
        {
            try
            {
                using (var isoStore = IsolatedStorageFile.GetStore(scope, null, null))
                {
                    if (!string.IsNullOrEmpty(fileName) && isoStore.GetFileNames(fileName).Length > 0)
                    {
                        isoStore.DeleteFile(fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("�޷��ڴ洢����ɾ���ļ�.", ex);
            }
        }

        /// <summary>
        /// �ڴ洢���ڴ���Ŀ¼
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="dirName"></param>
        public static void CreateDirectory(IsolatedStorageFile storage, string dirName)
        {
            try
            {
                if (!string.IsNullOrEmpty(dirName) && storage.GetDirectoryNames(dirName).Length > 0)
                {
                    storage.CreateDirectory(dirName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("�޷��ڴ洢���ڴ���Ŀ¼.", ex);
            }
        }

        /// <summary>
        /// �ڴ洢����ɾ��Ŀ¼
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="dirName"></param>
        public static void DeleteDirectory(IsolatedStorageFile storage, string dirName)
        {
            try
            {
                if (!string.IsNullOrEmpty(dirName) && storage.GetDirectoryNames(dirName).Length > 0)
                {
                    storage.DeleteDirectory(dirName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("�޷��ڴ洢����ɾ��Ŀ¼.", ex);
            }
        }

        #endregion
    }
}