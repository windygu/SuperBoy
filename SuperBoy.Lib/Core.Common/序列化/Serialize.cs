﻿using System;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;

namespace Core.Common
{

    #region 序列化
    public class  Serialize  
    {   
        /// <summary>
        /// 序列化为对象
        /// </summary>
        /// <param name="objname"></param>
        /// <param name="obj"></param>
        public static void BinarySerialize(string objname,object obj)
        {
            try
            {
                var filename = objname + ".Binary";
                if(System.IO.File.Exists(filename))
                    System.IO.File.Delete(filename);
                using (var fileStream = new FileStream(filename, FileMode.Create))
                {
                    // 用二进制格式序列化
                    var binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(fileStream, obj);
                    fileStream.Close();
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 从二进制文件中反序列化
        /// </summary>
        /// <param name="objname"></param>
        /// <returns></returns>
        public static object BinaryDeserialize(string objname)
        {
            System.Runtime.Serialization.IFormatter formatter = new BinaryFormatter();
            //二进制格式反序列化
            object obj;
            var filename = objname + ".Binary";
            if(!System.IO.File.Exists(filename))
                throw new Exception("在反序列化之前,请先序列化");
            using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                obj = formatter.Deserialize(stream);
                stream.Close();
            }
            //using (FileStream fs = new FileStream(filename, FileMode.Open))
            //{
            //    BinaryFormatter formatter = new BinaryFormatter();
            //    object obj = formatter.Deserialize(fs);
            //}
            return obj;

        }

        /// <summary>
        /// 序列化为soap 即xml
        /// </summary>
        /// <param name="objname"></param>
        /// <returns></returns>
        public static void SoapSerialize(string objname,object obj)
        {
            try
            {  
                var filename=objname+".Soap";
                if(System.IO.File.Exists(filename))
                    System.IO.File.Delete(filename);
                using (var fileStream = new FileStream(filename, FileMode.Create))
                {
                    // 序列化为Soap
                    var formatter = new SoapFormatter();
                    formatter.Serialize(fileStream, obj);
                    fileStream.Close();
                }

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// 反序列对象
        /// </summary>
        /// <param name="objname"></param>
        public static object SoapDeserialize(string objname)
        {
            object obj;
            System.Runtime.Serialization.IFormatter formatter = new SoapFormatter();
            var filename=objname+".Soap";
            if (!System.IO.File.Exists(filename))
                throw new Exception("对反序列化之前,请先序列化");
            //Soap格式反序列化
            using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                obj = formatter.Deserialize(stream);
                stream.Close();
            }
            return obj;
        }

        public static void XmlSerialize(string objname,object obj)
        {
             
            try
            {
                var filename=objname+".xml";
                if(System.IO.File.Exists(filename))
                    System.IO.File.Delete(filename);
                using (var fileStream = new FileStream(filename, FileMode.Create))
                {
                    // 序列化为xml
                    var formatter = new XmlSerializer(typeof(Car));
                    formatter.Serialize(fileStream, obj);
                    fileStream.Close();
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


        /// <summary>
        /// 从xml序列中反序列化
        /// </summary>
        /// <param name="objname"></param>
        /// <returns></returns>
        public static object XmlDeserailize(string objname)
        {
           // System.Runtime.Serialization.IFormatter formatter = new XmlSerializer(typeof(Car));
            var filename=objname+".xml";
            object obj;
            if (!System.IO.File.Exists(filename))
                throw new Exception("对反序列化之前,请先序列化");
            //Xml格式反序列化
            using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var formatter = new XmlSerializer(typeof(Car));
                obj = (Car)formatter.Deserialize(stream);
                stream.Close();
            }
            return obj; 
        }
    }
    #endregion

    #region 要序列化的类
    [Serializable]
    public class Car
    {
        private string _price;
        private string _owner;
        private string _mFilename;

        [XmlElement(ElementName = "Price")]
        public string Price
        {
            get { return this._price; }
            set { this._price = value; }
        }

        [XmlElement(ElementName = "Owner")]
        public string Owner
        {
            get { return this._owner; }
            set { this._owner = value; }
        }

        public string Filename
        {
            get
            {
                return _mFilename;
            }
            set
            {
                _mFilename = value;
            }
        }

        public Car(string o, string p)
        {
            this.Price = p;
            this.Owner = o;
        }

        public Car()
        {

        }
    }
    #endregion

    #region 调用示例
    public class Demo
    {
        public void DemoFunction()
        {
            //序列化
            var car = new Car("chenlin", "120万");
            Serialize.BinarySerialize("Binary序列化", car);
            Serialize.SoapSerialize("Soap序列化", car);
            Serialize.XmlSerialize("XML序列化", car);
            //反序列化
            var car2 = (Car)Serialize.BinaryDeserialize("Binary序列化");
            car2 = (Car)Serialize.SoapDeserialize("Soap序列化");
            car2 = (Car)Serialize.XmlDeserailize("XML序列化");
        }
    }
    #endregion
}

