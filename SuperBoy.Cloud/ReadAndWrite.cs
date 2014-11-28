using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace SuperBoy.Cloud
{
    /// <summary>
    /// 读取文件与写入文件类
    /// </summary>
    class ReadAndWrite : EnumArry
    {

        #region 基本操作（basic）
        /// <summary>
        /// read config all（ALL）
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public static string[] SelectAll(string Path)
        {
            StreamReader read = new StreamReader(Path);
            string[] item = read.ReadToEnd().Replace("\r\n", "|").Split('|');
            return Filter(item);

        }
        /// <summary>
        /// 根据数据过滤注释和空格
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string[] Filter(string[] item)
        {
            //string[] item = obj as string[];
            try
            {
                for (int length = 0; length < item.Length; length++)
                {

                    if (item[length] != null && !item[length].IndexOf("#").Equals(-1))
                    {
                        int indexs = item[length].IndexOf("#");
                        item[length] = item[length].Substring(0, indexs).Replace("	", "").Replace(" ", "");
                    }
                    else
                    {
                        item[length] = item[length];
                    }
                }
                return item;
            }
            catch (Exception)
            {
                throw new Exception();
            }
        }
        /// <summary>
        /// 根据字符串过滤注释
        /// </summary>
        /// <param name="Item"></param>
        /// <returns></returns>
        public static string Filter(string Item)
        {
            if (!Item.IndexOf("#").Equals(-1))
            {
                return Item.Substring(0, Item.IndexOf("#"));
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 按需要读取配置文件指定某一行(SelectIndex)以#作为注释
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <param name="Font">字符类型</param>
        /// <param name="Index">指定某个下标</param>
        /// <returns>指定数据</returns>
        private static string SelectIndex(string Path, int Index)
        {
            using (StreamReader read = new StreamReader(Path))
            {
                string item;
                int ItemIndex = 0;
                while ((item = read.ReadLine()) != null)
                {
                    ItemIndex++;

                    if (ItemIndex.Equals(Index))
                    {
                        return item;
                    }

                }
            }
            return null;
        }

        /// <summary>
        /// 查询标准字符返回string
        /// </summary>
        /// <param name="Path">地址</param>
        /// <param name="StartStr">起始标志</param>
        /// <param name="EndStr">结束标志</param>
        /// <returns></returns>
        public static StringBuilder SelectHeadStringBuilder(string Path, string StartStr, string EndStr)
        {
            if (File.Exists(Path))
            {
                StreamReader read = new StreamReader(Path);
                StringBuilder sb = new StringBuilder();
                string input;
                while ((input = read.ReadLine()) != null)
                {
                    if (input.IndexOf(StartStr) != -1)
                    {
                        if (input.Length > StartStr.Length)
                        {
                            sb.Append(input.Substring(input.IndexOf(StartStr) + StartStr.Length) + "\r\n");
                        }

                        while ((input = read.ReadLine()) != null)
                        {
                            if (input.IndexOf(EndStr) == -1)
                            {
                                if (!input.Length.Equals(0))
                                {
                                    sb.AppendLine(input);
                                }
                            }
                            else
                            {
                                if ((input.Substring(0, input.IndexOf(EndStr)).Length > EndStr.Length))
                                {
                                    sb.AppendLine(input.Substring(0, input.IndexOf(EndStr) - EndStr.Length));
                                }
                                break;
                            }

                        }
                        read.Close();
                        return sb;
                    }
                }
                return null;
            }
            else
            {
                throw new Exception("不合法的路径");
            }
        }

        /// <summary>
        /// 查询头文件返回数组
        /// </summary>
        /// <param name="Path">地址</param>
        /// <param name="StartStr">起始标志</param>
        /// <param name="EndStr">结束标志</param>
        /// <returns></returns>
        public static string[] SelectHeadAtrry(string Path, string StartStr, string EndStr)
        {
            if (File.Exists(Path))
            {
                StreamReader read = new StreamReader(Path);
                ModelHead model = new ModelHead(false);
                string[] Sbstr = new string[model.GetType().GetProperties().Count()];
                string input;
                int item = -1;

                while ((input = read.ReadLine()) != null)
                {
                    if (input.IndexOf(StartStr) != -1)
                    {
                        item++;
                        if (input.Length > StartStr.Length)
                        {
                            Sbstr[item] = (input.Substring(input.IndexOf(StartStr) + StartStr.Length) + "\r\n");
                        }
                        else
                        {
                            item--;
                        }

                        while ((input = read.ReadLine()) != null)
                        {

                            if (input.IndexOf(EndStr) == -1)
                            {
                                if (!input.Length.Equals(0))
                                {
                                    item++;
                                    Sbstr[item] = input;
                                }
                            }
                            else
                            {
                                if ((input.Substring(0, input.IndexOf(EndStr)).Length > EndStr.Length))
                                {
                                    item++;
                                    Sbstr[item] = (input.Substring(0, input.IndexOf(EndStr) - EndStr.Length));
                                }
                                break;
                            }

                        }
                        read.Close();
                        return Filter(Sbstr);
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 将新数据追加到此文本的结尾
        /// </summary>
        /// <param name="Path">插入地址</param>
        /// <param name="Text">插入文本</param>
        /// <returns></returns>
        public static Boolean InserINtoWriteLine(string Path, string Text)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(Path))
                {
                    writer.Write(Text);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 插入一行到新行
        /// </summary>
        /// <param name="Path">地址</param>
        /// <param name="Text">文本</param>
        /// <returns></returns>
        public static Boolean InserINtoWriteLine(string Path, string Text, bool ISappend)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(Path, ISappend))
                {
                    writer.WriteLine(Text);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 插入数组到新行
        /// </summary>
        /// <param name="Path">插入地址</param>
        /// <param name="Text">插入文本</param>
        /// <param name="ISappend">是否追加,若为假则覆盖，若为真则追加</param>
        /// <returns></returns>
        public static Boolean InserINtoWriteLine(string Path, string[] Text, bool ISappend)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(Path, ISappend))
                {
                    //循环插入
                    for (int i = 0; i < Text.Length; i++)
                    {
                        writer.WriteLine(Text[i]);
                    }

                    /*循环数组拼接再插入
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < Text.Length; i++)
                    {
                        sb.Append(Text[i] + "\r\n");
                    }
                    writer.Write(sb.ToString());
                    */
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 替换某一行（Replace）
        /// </summary>
        /// <param name="path">地址</param>
        /// <param name="index">下标</param>
        /// <param name="newLine">新行</param>
        /// <returns></returns>
        public static bool Replace(string path, int index, string newLine)
        {

            try
            {
                if (File.Exists(path))
                {

                    using (StreamReader read = new StreamReader(path))
                    {
                        string item;
                        int Index = 0;
                        StringBuilder strb = new StringBuilder();
                        while ((item = read.ReadLine()) != null)
                        {
                            Index++;
                            strb.AppendLine(item);
                            //一行一行读取
                            if (Index.Equals(index))
                            {
                                //item = newLine;
                                strb.AppendLine(newLine);
                                strb.AppendLine(read.ReadToEnd());
                                read.Close();
                                using (StreamWriter write = new StreamWriter(path))
                                {
                                    write.Write(strb.ToString());
                                }
                                return true;
                            }
                        }
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
            return false;
        }


        #region 数值的备份与还原


        public static bool TextBak(string table, string text, int Index, string Key, string Value, bool IsKey)
        {
            //调用命名空间内的表获得命名空间，获取他的位置和命名空间

            //在命名空间里面增加一个还原备份文件地址

            //写入头文件

            return true;
        }
        /// <summary>
        /// 获取命名空间
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public static string nameSpace(string Path)
        {
            try
            {
                if (File.Exists(Path))
                {
                    //获取头文件，查询命名空间
                    ModelHead model = GetHeadToModel(Path);
                    return model.Namespace;
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }
        /// <summary>
        /// 根据Key获得值
        /// </summary>
        /// <param name="path"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string nameSpace(string path, string Key)
        {
            //不启用全局配置
            //string StartHeadStrType = AssemblyConfiguration.MasterDiction[Master.HeadText].ToString().Split('|')[0].Split(',')[0];
            //string EndHeadStrType = AssemblyConfiguration.MasterDiction[Master.HeadText].ToString().Split('|')[0].Split(',')[1];

            string StartHeadStrType = "#*";
            string EndHeadStrType = "*#";
            //根据读取类型来读取头文件
            ModelHead Model = new ModelHead(false);
            string[] item = SelectHeadAtrry(path, StartHeadStrType, EndHeadStrType);
            Key = Key.ToLower();
            for (int index = 0; index < item.Length; index++)
            {
                item[index] = item[index].ToLower();
                if (!item[index].IndexOf(Key).Equals(-1) && !item[index].IndexOf(":").Equals(-1))
                {
                    return item[index].Split(':')[1].Trim();
                }
            }
            return null;
        }
        /// <summary>
        /// 获取头文件并返回对象
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ModelHead GetHeadToModel(string path)
        {
            //不启用全局配置
            //string StartHeadStrType = AssemblyConfiguration.MasterDiction[Master.HeadText].ToString().Split('|')[0].Split(',')[0];
            //string EndHeadStrType = AssemblyConfiguration.MasterDiction[Master.HeadText].ToString().Split('|')[0].Split(',')[1];

            string StartHeadStrType = "#*";
            string EndHeadStrType = "*#";
            //根据读取类型来读取头文件
            ModelHead Model = new ModelHead(false);
            string[] item = SelectHeadAtrry(path, StartHeadStrType, EndHeadStrType);
            for (int Index = 0; Index < item.Length; Index++)
            {
                // string[] KeyValue = item[items].Split(':');
                if (!item[Index].IndexOf(':').Equals(-1))
                {
                    string[] KeyValue = { item[Index].Substring(0, item[Index].IndexOf(':')), item[Index].Substring(item[Index].IndexOf(':') + 1) };
                    string Key = KeyValue[0].Trim().ToLower();

                    switch (Key)
                    {
                        case "backup":
                            Model.BackUp = KeyValue[1];
                            break;

                        case "createman":
                            Model.CreateMan = KeyValue[1];
                            break;

                        case "creationdate":
                            Model.CreationDate = KeyValue[1];
                            break;

                        case "lastupdatedate":
                            Model.LastUpdateDate = KeyValue[1];
                            break;

                        case "tablename":
                            Model.DataBase = KeyValue[1];
                            break;

                        case "namespace":
                            Model.Namespace = KeyValue[1];
                            break;

                        case "updatecode":
                            Model.UpdateCode = KeyValue[1];
                            break;
                        case "versiontype":
                            string value = KeyValue[1].Trim().ToLower();

                            switch (value)
                            {
                                case "plain":
                                    Model.VersionType = HeadType.Plain;
                                    break;
                                case "log":
                                    Model.VersionType = HeadType.Log;
                                    break;
                                case "system":
                                    Model.VersionType = HeadType.System;
                                    break;
                                case "master":
                                    Model.VersionType = HeadType.Master;
                                    break;
                                case "bak":
                                    Model.VersionType = HeadType.bak;
                                    break;
                                case "odb":
                                    Model.VersionType = HeadType.odb;
                                    break;
                                default:
                                    break;
                            }
                            break;

                        case "versionnumber":
                            Model.VersionNumber = KeyValue[1];
                            break;

                        case "datetype":
                            string valueDateType = KeyValue[1].Trim().ToLower();
                            switch (valueDateType)
                            {
                                case "array":
                                    Model.DateType = ConfigFormat.Array;
                                    break;
                                case "json":
                                    Model.DateType = ConfigFormat.Json;
                                    break;

                                case "xml":
                                    Model.DateType = ConfigFormat.XML;
                                    break;

                                default:

                                    break;
                            }
                            break;
                        default:

                            break;
                    }
                }
                else
                {
                    if (!item[Index].IndexOf("[").Equals(-1) && !item[Index].IndexOf("]").Equals(-1))
                    {
                        Model.DataBase = item[Index].Trim().Trim('[').Trim(']');
                    }
                    else
                    {
                        throw new Exception("读取异常");
                    }
                }
            }
            return Model;
        }


        /// <summary>
        /// 查询头文件返回字典集合
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetHeadMessDictionary(string path)
        {
            //不启用全局配置
            //string StartHeadStrType = AssemblyConfiguration.MasterDiction[Master.HeadText].ToString().Split('|')[0].Split(',')[0];
            //string EndHeadStrType = AssemblyConfiguration.MasterDiction[Master.HeadText].ToString().Split('|')[0].Split(',')[1];

            string StartHeadStrType = "#*";
            string EndHeadStrType = "*#";

            //根据读取类型来读取头文件
            ModelHead Model = new ModelHead(false);
            string[] item = SelectHeadAtrry(path, StartHeadStrType, EndHeadStrType);
            Dictionary<string, string> dict = new Dictionary<string, string>();

            for (int Index = 0; Index < item.Length; Index++)
            {
                if (!item[Index].IndexOf(':').Equals(-1))
                {
                    string[] KeyValue = item[Index].Split(':');
                    dict.Add(KeyValue[0].Trim(), KeyValue[1].Trim());
                }
            }
            return dict;

        }

        #endregion

        /// <summary>
        /// 根据Key修改值
        /// </summary>
        /// <param name="path">地址</param>
        /// <param name="Index">下标</param>
        /// <returns></returns>
        public static object UpdateValue(string path, string Key)
        {
            //获取此文件是什么类型的文件，版本号等
            ModelHead model = GetHeadToModel(path);
            //获得需要修改的位置
            //朝向处理
            switch (model.DateType)
            {
                case ConfigFormat.Json:

                    break;
                case ConfigFormat.XML:

                    break;
                case ConfigFormat.Array:

                    break;
                default:
                    break;
            }

            return "";

        }

        public ModelHead GetHeadMess()
        {
            /*
             * 获取方法，先去运行程序集内查询有没有此文件集合，如果有则使用
             * 如果没有则调取，调取之后加载到头文件中
             */
            ModelHead model = new ModelHead(false);

            return model;
        }

        /// <summary>
        /// 根据键修改值
        /// </summary>
        /// <param name="path"></param>
        /// <param name="Index"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static object UpdateValue(string path, int Index, string Key)
        {
            //获取头文件

            return "";
        }

        /// <summary>
        /// 通过键值对数组转换json
        /// </summary>
        /// <param name="Key">键</param>
        /// <param name="Value">值</param>
        /// <returns>拼接后的值</returns>
        public static string TextToJson(string[] Key, string[] Value)
        {
            StringBuilder sbStr = new StringBuilder();
            if (Key.Length.Equals(Value.Length))
            {
                sbStr.Append("{");
                for (int item = 0; item < Key.Length; item++)
                {
                    sbStr.Append(Key[item] + ":" + Value[item]);
                    if (item != Key.Length - 1) sbStr.Append(",");
                }

                sbStr.Append("}");
                return sbStr.ToString();
            }
            else
            {
                return null;
            }
        }
        public static string TextToJson(Dictionary<string[], string[]> dic)
        {
            //return TextToJson(dic.Keys, dic.Values);
            return "";
        }
        /// <summary>
        /// json转键值对
        /// </summary>
        /// <param name="JsonText"></param>
        /// <returns></returns>
        public static string TextToJson(string JsonText)
        {

            return "";
        }

        #endregion

        #region


        #endregion
    }
}
