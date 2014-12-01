using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperBoy.Model.Interface
{
    public interface IReadAndWrite
    {
        #region 读取方法
        List<string> read();
        string read(int Index);
        List<string> read(string Path);
        string read(string Path, int Index);
        List<string> read(string Path, Encoding Fond);
        string read(string Path, Encoding Fond, int Index);
        #endregion

        #region 写入方法
        bool write(string text);
        bool write(string text, int Index);
        bool write(string text, int Index, string Path);
        #endregion
    }
}
