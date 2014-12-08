using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperBoy.Model.Interface
{
    public interface IReadAndWrite
    {
        List<string> read(string Path, Encoding Fond);
        string read(string Path, Encoding Fond, int Index);

        void write(string text, string Path);
        void write(string text, int Index, string Path);
    }
}
