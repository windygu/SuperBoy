using SuperBoy.Dynamic.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperBoy.Dynamic
{
    public class SerializationDynamic : ISerializationDynamic
    {
        /// <summary>
        /// 第一步序列化参数
        /// </summary>
        /// <param name="AnalyticalString"></param>
        /// <returns></returns>
        public string AnalyticalMain(string AnalyticalString)
        {
            //IAnalyticalDynamic Iad = new AnalyticalDynamic();
            int liftInt = AnalyticalString.IndexOf("{") + 1;
            int rightInt = AnalyticalString.IndexOf("}");
            int LastliftInt = AnalyticalString.LastIndexOf("{") + 1;
            int LastrightInt = AnalyticalString.LastIndexOf("}");
            string head = AnalyticalString.Substring(liftInt, rightInt - liftInt);
            string body = AnalyticalString.Substring(LastliftInt, LastrightInt - LastliftInt);

            IAnalyticalDynamic Iand = new AnalyticalDynamic();
            Dictionary<string, string> HeadDic = DictAnalytical(head);
            HeadAnalytical(HeadDic);
            Dictionary<string, string> bodyDic = DictAnalytical(body);
            BodyAnalytical(bodyDic);


            return head;
        }
        public Dictionary<string, string> DictAnalytical(string JsonArray)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            // string[] item = JsonArray.Replace("\"", "").Split(',');
            string[] item = JsonArray.Split(',');
            for (int index = 0; index < item.Length; index++)
            {
                string[] KeyValue = item[index].Split(':');
                dic.Add(KeyValue[0].Trim().Trim('\"').ToLower(), KeyValue[1].Trim().Trim('\"'));
            }
            return dic;
        }

        public string HeadAnalytical(Dictionary<string, string> headDic)
        {
            //验证权限

            return "";
        }

        public string BodyAnalytical(Dictionary<string, string> boduDic)
        {
            IAnalyticalDynamic anly = new AnalyticalDynamic();
            string returnValue = "";
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
                    returnValue = anly.MANAGE(boduDic);
                    break;
                default:
                    //配置问题
                    returnValue = "";
                    break;
            }

            return returnValue;
        }
    }
}
