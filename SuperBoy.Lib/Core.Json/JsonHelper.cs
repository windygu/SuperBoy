using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
namespace Core.Json
{
    public class JsonHelper
    {
        public static string GetJsonFromObject(object obj)
        {
            string str;
            var builder = new StringBuilder();
            builder.Append("{");
            var type = obj.GetType();
            var property = type.GetProperty("Values");
            var info2 = type.GetProperty("Keys");
            if ((property != null) || (info2 != null))
            {
                var is2 = (ICollection)property.GetValue(obj, null);
                var is3 = (ICollection)info2.GetValue(obj, null);
                str = string.Empty;
                var list = new List<string>();
                foreach (var obj2 in is3)
                {
                    list.Add(obj2.ToString());
                }
                var num = 0;
                str = string.Empty;
                foreach (var obj2 in is2)
                {
                    var type2 = obj2.GetType();
                    if (((type2.ToString() == "System.String") || (type2.ToString() == "System.Int32")) || (type2.ToString() == "System.Boolean"))
                    {
                        if (obj2 is string)
                        {
                            str = str + ",\"" + list[num] + "\":\"" + obj2.ToString().Replace(@"\", @"\\").Replace("\"", "\\\"").Replace("\n", @"\n").Replace("\r", @"\r") + "\"";
                        }
                        else
                        {
                            str = str + ",\"" + list[num] + "\":" + obj2.ToString();
                        }
                    }
                    else
                    {
                        str = str + ",\"" + list[num] + "\":{" + Tojson(obj2) + "}";
                    }
                    num++;
                }
                if (str.Length > 0)
                {
                    str = str.Substring(1);
                    builder.Append(str);
                }
            }
            else
            {
                str = string.Empty;
                if ((obj is int) || (obj is bool))
                {
                    str = str + ",\"" + type.Name + "\":" + obj.ToString();
                }
                else if (obj is string)
                {
                    str = str + ",\"" + type.Name + "\":\"" + obj.ToString().Replace(@"\", @"\\").Replace("\"", "\\\"").Replace("\n", @"\n").Replace("\r", @"\r") + "\"";
                }
                else
                {
                    foreach (var info3 in type.GetProperties())
                    {
                        var obj3 = info3.GetValue(obj, null);
                        var type3 = obj3.GetType();
                        if ((obj3 is int) || (obj3 is bool))
                        {
                            str = str + ",\"" + info3.Name + "\":" + obj3.ToString();
                        }
                        else if (obj3 is string)
                        {
                            str = str + ",\"" + info3.Name + "\":\"" + obj3.ToString().Replace(@"\", @"\\").Replace("\"", "\\\"").Replace("\n", @"\n").Replace("\r", @"\r") + "\"";
                        }
                        else
                        {
                            str = str + ",\"" + info3.Name + "\":{" + Tojson(obj3) + "}";
                        }
                    }
                }
                if (str.Length > 0)
                {
                    builder.Append(str.Substring(1));
                }
            }
            builder.Append("}");
            return builder.ToString();
        }

        private static string Tojson(object obj)
        {
            string str;
            var type = obj.GetType();
            var str2 = string.Empty;
            var property = type.GetProperty("Values");
            var info2 = type.GetProperty("Keys");
            if ((property != null) && (info2 != null))
            {
                var is2 = (ICollection)info2.GetValue(obj, null);
                var is3 = (ICollection)property.GetValue(obj, null);
                str = string.Empty;
                var list = new List<string>();
                foreach (var obj2 in is2)
                {
                    list.Add(obj2.ToString());
                }
                var num = 0;
                str = string.Empty;
                foreach (var obj2 in is3)
                {
                    var type2 = obj2.GetType();
                    if (((type2.ToString() == "System.String") || (type2.ToString() == "System.Int32")) || (type2.ToString() == "System.Boolean"))
                    {
                        if (obj2 is string)
                        {
                            str = str + ",\"" + list[num] + "\":\"" + obj2.ToString().Replace(@"\", @"\\").Replace("\"", "\\\"").Replace("\n", @"\n").Replace("\r", @"\r") + "\"";
                        }
                        else
                        {
                            str = str + ",\"" + list[num] + "\":" + obj2.ToString();
                        }
                    }
                    else
                    {
                        str = str + ",\"" + list[num] + "\":{" + Tojson(obj2) + "}";
                    }
                    num++;
                }
                if (str.Length > 0)
                {
                    str = str.Substring(1);
                    str2 = str2 + str;
                }
                return str2;
            }
            str = string.Empty;
            if ((obj is int) || (obj is bool))
            {
                str = str + ",\"" + type.Name + "\":" + obj.ToString();
            }
            else if (obj is string)
            {
                str = str + ",\"" + type.Name + "\":\"" + obj.ToString().Replace(@"\", @"\\").Replace("\"", "\\\"").Replace("\n", @"\n").Replace("\r", @"\r") + "\"";
            }
            else
            {
                foreach (var info3 in type.GetProperties())
                {
                    var obj3 = info3.GetValue(obj, null);
                    var type3 = obj3.GetType();
                    if ((obj3 is int) || (obj3 is bool))
                    {
                        str = str + ",\"" + info3.Name + "\":" + obj3.ToString();
                    }
                    else if (obj3 is string)
                    {
                        str = str + ",\"" + info3.Name + "\":\"" + obj3.ToString().Replace(@"\", @"\\").Replace("\"", "\\\"").Replace("\n", @"\n").Replace("\r", @"\r") + "\"";
                    }
                    else
                    {
                        str = str + ",\"" + info3.Name + "\":{" + Tojson(obj3) + "}";
                    }
                }
            }
            if (str.Length > 0)
            {
                str2 = str2 + str.Substring(1);
            }
            return str2;
        }
    }
}
