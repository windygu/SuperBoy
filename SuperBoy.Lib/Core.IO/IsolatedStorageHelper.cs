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
    /// 独立存储操作辅助类
    /// </summary>
    public sealed class IsolatedStorageHelper
    {
        #region 程序运行时间的保存操作
        /// <summary>
        /// 加密并保存指定时间到"独立存贮空间" （以分号(;)追加保存）
        /// </summary>
        public static void SaveDataTime()
        {
            SaveDataTime(DateTime.Now);
        }

        /// <summary>
        /// 加密并保存当前时间到"独立存贮空间" （以分号(;)追加保存）
        /// </summary>
        public static void SaveDataTime(DateTime fromDate)
        {
            var fromDataTime = fromDate.ToString("MM-dd-yyyy HH:mm:ss");
            var oldTime = GetDataTime().Trim();
            if (!string.IsNullOrEmpty(oldTime))
            {
                fromDataTime = oldTime + ";" + fromDataTime; //追加最后时间到左边
            }
            fromDataTime = DesEncrypt.Encrypt(fromDataTime, UiConstants.IsolatedStorageEncryptKey); //加密

            #region 将fromDataTime保存在"独立存贮空间"

            var username = fromDataTime;
            //按用户、域、程序集获取独立存储区 
            var isoStore =
                IsolatedStorageFile.GetStore(
                    IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);
            var myusername = isoStore.GetDirectoryNames(UiConstants.IsolatedStorageDirectoryName);
            IsolatedStorageFileStream isoStream1 = null;
            if (myusername.Length == 0) //没有目录 
            {
                //创建目录 
                isoStore.CreateDirectory(UiConstants.IsolatedStorageDirectoryName);
                //创建文件 
                using (isoStream1 = new IsolatedStorageFileStream(UiConstants.IsolatedStorage, FileMode.Create, isoStore))
                {
                    //写入文件 
                    using (var writer = new StreamWriter(isoStream1))
                    {
                        writer.WriteLine(fromDataTime);
                    }
                }
            }
            else
            {
                myusername = isoStore.GetFileNames(UiConstants.IsolatedStorage);
                if (myusername.Length == 0) //没有文件 
                {
                    //创建文件 
                    using (isoStream1 = new IsolatedStorageFileStream(UiConstants.IsolatedStorage, FileMode.Create, isoStore))
                    {
                        //写入文件 
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
                        //写入文件 
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
        /// 从"独立存贮空间"取程序第一次运行的时间并解密
        /// </summary> 
        /// <returns></returns> 
        public static string GetDataTime()
        {
            string fromDataTime;

            //按用户、域、程序集获取独立存储区 
            var isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User
                | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, null, null);

            var myusername = isoStore.GetDirectoryNames(UiConstants.IsolatedStorageDirectoryName);
            if (myusername.Length == 0) //没有文件夹
            {
                return string.Empty; //域中没有他的目录
            }

            myusername = isoStore.GetFileNames(UiConstants.IsolatedStorage);
            if (myusername.Length == 0) //没有文件 
            {
                return string.Empty; //域中没有他的用户名 
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
                if (!string.IsNullOrEmpty(fromDataTime)) //解密
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

        #region 基本操作函数

        /// <summary>
        /// 保存对象到独立存储区
        /// </summary>
        /// <param name="objectToSave">待保存的对象</param>
        /// <param name="key">保存的键值</param>
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
        /// 根据键值加载独立存储区的内容
        /// </summary>
        /// <param name="key">独立存储的键值(路径）</param>
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
        /// 加载存在用户标识范围、应用程序范围内的存储值
        /// </summary>
        /// <param name="d">待填充的字典对象</param>
        /// <param name="filename">文件名</param>
        public static void LoadFromUserStoreForApplication(IDictionary d, string filename)
        {
            Load(d, IsolatedStorageScope.Application | IsolatedStorageScope.User, filename);
        }

        /// <summary>
        /// 保存在用户标识范围、应用程序范围内的值
        /// </summary>
        /// <param name="d">待保存的字典对象</param>
        /// <param name="filename">文件名</param>
        public static void SaveToUserStoreForApplication(IDictionary d, string filename)
        {
            Save(d, IsolatedStorageScope.Application | IsolatedStorageScope.User, filename);
        }

        /// <summary>
        /// 加载用户范围、应用范围、程序集范围内的存储值
        /// </summary>
        /// <param name="d">待填充的字典对象.</param>
        /// <param name="filename">文件名</param>
        public static void LoadFromUserStoreForDomain(IDictionary d, string filename)
        {
            Load(d, IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain |
                IsolatedStorageScope.User, filename);
        }

        /// <summary>
        /// 保存用户范围、应用范围、程序集范围内的存储值
        /// </summary>
        /// <param name="d">待保存的字典对象</param>
        /// <param name="filename">文件名</param>
        public static void SaveToUserStoreForDomain(IDictionary d, string filename)
        {
            Save(d, IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain |
                IsolatedStorageScope.User, filename);
        }

        /// <summary>
        /// 加载在独立存储内的指定文件内容
        /// </summary>
        /// <param name="d">待填充的字典内容</param>
        /// <param name="scope">独立存储范围对象</param>
        /// <param name="filename">文件名</param>
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
        /// 在独立存储范围内保存字典内容到指定文件
        /// </summary>
        /// <param name="d">待保存的字典内容</param>
        /// <param name="scope">独立存储范围对象</param>
        /// <param name="filename">文件名</param>
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
        /// 删除指定区域的存储区内容
        /// </summary>
        /// <param name="fileName">待删除的文件</param>
        /// <param name="scope">独立存储范围对象</param>
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
                throw new Exception("无法在存储区内删除文件.", ex);
            }
        }

        /// <summary>
        /// 在存储区内创建目录
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
                throw new Exception("无法在存储区内创建目录.", ex);
            }
        }

        /// <summary>
        /// 在存储区内删除目录
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
                throw new Exception("无法在存储区内删除目录.", ex);
            }
        }

        #endregion
    }
}