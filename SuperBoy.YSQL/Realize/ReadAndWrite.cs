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
        public string ReadSys(Model.EnumArrayYSQL.ReadType readType)
        {
            string address;
            switch (readType)
            {
                case EnumArrayYSQL.ReadType.SystemInfo:
                    address = Lib.DirectoryUtil.GetCurrentDirectory() + "\\conf\\SystemInfo.conf";
                    break;

                case EnumArrayYSQL.ReadType.SysDatabase:
                    address = Lib.DirectoryUtil.GetCurrentDirectory() + "\\dbInfo\\SysDatabase.ydbc";
                    break;
                case EnumArrayYSQL.ReadType.SystemBak:
                    address = Lib.DirectoryUtil.GetCurrentDirectory() + "\\dbBak\\SysDatabase.ybak";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("readType");
            }
            return Read(address);
        }
        //写入指定信息
        public void WriteSys(string txt, Model.EnumArrayYSQL.WriteType writeType)
        {
            string address = null;
            switch (writeType)
            {
                case EnumArrayYSQL.WriteType.SystemInfo:
                    address = Lib.DirectoryUtil.GetCurrentDirectory() + "\\conf\\SystemInfo.conf";
                    break;
                case EnumArrayYSQL.WriteType.SysDatabase:
                    address = Lib.DirectoryUtil.GetCurrentDirectory() + "\\dbInfo\\SysDatabase.ydbc";
                    break;
                case EnumArrayYSQL.WriteType.SystemBak:
                    address = Lib.DirectoryUtil.GetCurrentDirectory() + "\\dbBak\\SysDatabase.ybak";
                    break;
            }
            Write(txt, address);
        }
    }
}