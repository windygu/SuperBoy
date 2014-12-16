using System;

namespace SuperBoy.Model.Public
{
    //这个模型里面包括初始化该程序集最基本的信息
    public class LoadModel
    {
        //编号等于客户端终端类型+客户端编码+客户端随机数字组成的序列号
        public string No { get; set; }
        //初始化时间
        public DateTime DateTime { get; set; }
        //文件可访问路径
        public string Address { get; set; }

        public LoadModel()
        {
            this.DateTime = DateTime.Now;
            this.No = "WindowsDeskApplicationABCDEFG123456";
            this.Address = "d\\superBoy.log";

        }
    }
}
