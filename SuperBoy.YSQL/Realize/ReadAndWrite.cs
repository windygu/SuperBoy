using System;
using System.IO;
using SuperBoy.YSQL.Interface;
using SuperBoy.YSQL.Model;

namespace SuperBoy.YSQL.Realize
{
    public class ReadAndWrite : IReadAndWriteYSQL
    {
        //   private const string Path = @"D:\SuperBoy\SuperBoy\SuperBoy.YSQL\Tdb\SystemInfo.conf";
        //读取指定信息
        public string Read(string path)
        {
            if (!File.Exists(path)) throw new Exception("No config");
            var read = new StreamReader(path);
            return read.ReadToEnd();
        }
        //追加
        public void Write(string txt, string path)
        {
            if (!File.Exists(path))
            {
                var wenjianjia = path.Substring(0, path.LastIndexOf("\\", StringComparison.Ordinal));
                if (!Directory.Exists(wenjianjia))
                {
                    Directory.CreateDirectory(wenjianjia);
                }
                var f = File.Create(path);
                f.Close();
                f.Dispose();
            }
            var f2 = new StreamWriter(path, false, System.Text.Encoding.UTF8);
            f2.WriteLine(txt);
            f2.Close();
            f2.Dispose();
        }

        //读取指定信息
        public string ReadSys(Model.EnumArray.ReadType readType)
        {
            string address;
            switch (readType)
            {
                case EnumArray.ReadType.SystemInfo:
                    address = Lib.DirectoryUtil.GetCurrentDirectory() + "\\conf\\SystemInfo.conf";
                    break;

                case EnumArray.ReadType.SysDatabase:
                    address = Lib.DirectoryUtil.GetCurrentDirectory() + "\\dbInfo\\SysDatabase.ydbc";
                    break;
                case EnumArray.ReadType.SystemBak:
                    address = Lib.DirectoryUtil.GetCurrentDirectory() + "\\dbBak\\SysDatabase.ybak";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("readType");
            }
            return Read(address);
        }
        //写入指定信息
        public void WriteSys(string txt, Model.EnumArray.WriteType writeType)
        {
            string address = null;
            switch (writeType)
            {
                case EnumArray.WriteType.SystemInfo:
                    address = Lib.DirectoryUtil.GetCurrentDirectory() + "\\conf\\SystemInfo.conf";
                    break;
                case EnumArray.WriteType.SysDatabase:
                    address = Lib.DirectoryUtil.GetCurrentDirectory() + "\\dbInfo\\SysDatabase.ydbc";
                    break;
                case EnumArray.WriteType.SystemBak:
                    address = Lib.DirectoryUtil.GetCurrentDirectory() + "\\dbBak\\SysDatabase.ybak";
                    break;
            }
            Write(txt, address);
        }
    }
}