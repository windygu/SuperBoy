using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperBoy.Dynamic.Interface;

namespace SuperBoy.Dynamic.Realize
{
    public class SerializationDynamic : ISerializationDynamic
    {
        /// <summary>
        /// 第一步序列化参数
        /// </summary>
        /// <param name="analyticalString"></param>
        /// <returns></returns>
        public string AnalyticalMain(string analyticalString)
        {
            //IAnalyticalDynamic Iad = new AnalyticalDynamic();
            var liftInt = analyticalString.IndexOf("{", StringComparison.Ordinal) + 1;
            var rightInt = analyticalString.IndexOf("}", StringComparison.Ordinal);
            var lastliftInt = analyticalString.LastIndexOf("{", StringComparison.Ordinal) + 1;
            var lastrightInt = analyticalString.LastIndexOf("}", StringComparison.Ordinal);
            var head = analyticalString.Substring(liftInt, rightInt - liftInt);
            var body = analyticalString.Substring(lastliftInt, lastrightInt - lastliftInt);

            IAnalyticalDynamic iand = new AnalyticalDynamic();
            var headDic = DictAnalytical(head);
            HeadAnalytical(headDic);
            var bodyDic = DictAnalytical(body);
            BodyAnalytical(bodyDic);
            return head;
        }
        public Dictionary<string, string> DictAnalytical(string jsonArray)
        {
            // string[] item = JsonArray.Replace("\"", "").Split(',');
            var item = jsonArray.Split(',');
            return item.Select(t => t.Split(':')).ToDictionary(keyValue => keyValue[0].Trim().Trim('\"').ToLower(), keyValue => keyValue[1].Trim().Trim('\"'));
        }

        public string HeadAnalytical(Dictionary<string, string> headDic)
        {
            //验证权限

            return "";
        }

        public string BodyAnalytical(Dictionary<string, string> boduDic)
        {
            IAnalyticalDynamic anly = new AnalyticalDynamic();
            string returnValue;
            //岔开分支
            switch (boduDic["type"])
            {
                case "SELECT":
                    returnValue = anly.Select(boduDic);
                    break;
                case "INSTER":
                    returnValue = anly.Inster(boduDic);
                    break;
                case "UPDATE":
                    returnValue = anly.Update(boduDic);
                    break;
                case "DELETE":
                    returnValue = anly.Delete(boduDic);
                    break;
                case "ONLINE":
                    returnValue = anly.OnLine(boduDic);
                    break;
                case "MANAGE":
                    returnValue = anly.Manage(boduDic);
                    break;
                default:
                    //配置问题
                    returnValue = "";
                    break;
            }

            return returnValue;
        }

        /// <summary>
        /// 序列化Json
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public string AnalyticalJson(Dictionary<string, string> dictionary)
        {
            var list = dictionary.Keys.ToList();
            var sb = new StringBuilder();
            sb.Append('{');
            string item;
            foreach (var t in list)
            {
                //  string item = string.Join("\",\"", dictionary[list[index]].Split(','));
                item = dictionary[t].Replace(",", "\",\"");
                sb.Append("\"" + t + "\":[\"" + item + "\"],");
            }
            item = sb.ToString().Trim(',');
           // sb.Append('}');
            return item + "}";

        }
    }
}
