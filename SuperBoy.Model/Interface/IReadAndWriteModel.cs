﻿using System.Collections.Generic;
using System.Text;

namespace SuperBoy.Model.Interface
{
    public interface IReadAndWriteModel
    {
        
        List<string> read(string path);

        void write(string text, string path);
        void write(string text, int Index, string path);
    }
}
