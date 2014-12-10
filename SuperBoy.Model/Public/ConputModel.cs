using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperBoy.Model.Public
{
    public class computModel
    {
        public string randomNo { get; set; }
        public string computNo { set; get; }
        public string User { get; set; }
        public string SecretKey { get; set; }
        public string Software { get; set; }
        /// <summary>
        /// 该数据中部分从配置文件读取
        /// </summary>
        public computModel()
        {
            randomNo = GenerateRandomNumber(17);
            //第一次加载的时候生成
            computNo = "A43369970";
            //当前登陆的用户
            User = "admin";
            //登陆秘钥
            SecretKey = "123456";
            //授权Key
            Software = "abcdefghijklmnopqrstuvwxyz";
        }
        private static char[] constant =   
      {   
        '0','1','2','3','4','5','6','7','8','9',  
        'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',   
        'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'   
      };
        public static string GenerateRandomNumber(int Length)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(62);
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(62)]);
            }
            return newRandom.ToString();
        }
    }
}
