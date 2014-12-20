using SuperBoy.YSQL.Model;

namespace SuperBoy.YSQL.Interface
{
    // ReSharper disable once InconsistentNaming
    public interface IReadAndWriteYSQL
    {
        string ReadSys(EnumArray.ReadType readType);
        void Write(string txt,string path);
        string Read(string path);
        void WriteSys(string txt, EnumArray.WriteType writeType);
    }
}