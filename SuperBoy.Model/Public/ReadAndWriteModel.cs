using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SuperBoy.Model.Interface;

namespace SuperBoy.Model.Public
{
    /// <summary>
    /// 读取文件与写入文件类
    /// </summary>
    public class ReadAndWriteModel : IReadAndWriteModel
    {
        private static Encoding _fondDefault = Encoding.UTF8;
        // public static string PathDefault = SuperBoy.Model.Controller.ModelController.Dicts[Public.EnumArry.Master.MasterPath].ToString() + "\\Temp.log";
        private static string _pathDefault = "\\Master\\Temp.log";


        #region read
        public List<string> read(string path, Encoding fond)
        {
            var list = new List<string>();
            if (File.Exists(path))
            {
                var sread = new StreamReader(path, Encoding.UTF8);
                var input = "";
                while ((input = sread.ReadLine()) != null)
                {
                    list.Add(input);
                }
                sread.Close();
                return list;
            }
            else
            {
                return list;
                //throw new Exception("文件地址不存在！");
            }
        }
        public string read(string path, Encoding fond, int index)
        {
            return read(path, fond)[index];
        }
        #endregion

        #region write
        /// <summary>
        /// 追加
        /// </summary>
        /// <param name="text"></param>
        /// <param name="Path"></param>
        public void write(string text, string path)
        {
            var fs = new FileStream(path, FileMode.Append);
            var sw = new StreamWriter(fs);
            sw.WriteLine(text);
            sw.Close(); 
            fs.Close();
        }

        /// <summary>
        /// 插入指定行数，如果行数不满足插入行数，则成参数前添加回车，如果插入行数为空则直接插入，如插入行数有数据，则所有数据后移将插入的数据填充。
        /// </summary>
        /// <param name="text">插入的文本</param>
        /// <param name="Index">插入的下标1为起始值</param>
        /// <param name="path">插入的文本，如果没有则自动新建</param>
        /// <returns></returns>
        public void write(string text, int Index, string path)
        {
            //这个是是否进入过插入
            var ist = 0;
            //默认函数值改为当前路径
            _pathDefault = path;
            //取出所有的数据并存入数组
            var textRead = read(path, Encoding.UTF8);

            //申明
            var write = new StreamWriter(path);
            //如果总行数的行小于要插入的行数
            if (textRead.Count < Index)
            {
                //声明一个空格
                var kg = "";
                //循环这个空格的行，例如数据有两行就空两个
                for (var index = textRead.Count; index < Index; index++)
                {
                    kg += "\r\n";
                }
                //拼接数据
                text = kg + text;
                //跳到指定标签
                goto first;
            }
            string writes;
            //能到这里说明该位置要被写入，也就是总行数大于要被写入的行数
            ist = 1;
            //
            if (textRead[Index - 1].Equals("") || textRead[Index - 1].Equals("\r"))
            {
                textRead[Index - 1] = text;
                writes = String.Join("\r\n", textRead);
            }
            else
            {
                //数组转List
                var list = new List<string>(textRead);
                list.Insert(Index - 1, text);
                writes = string.Join("\r\n", list.ToArray());
            }
            write.Write(writes);
        //跳到标签位置
        first:
            //申明IO写入
            if (ist.Equals(0))
            {
                write.Write(text);
            }
            //关闭
            write.Close();
        }

        #endregion

    }

}
