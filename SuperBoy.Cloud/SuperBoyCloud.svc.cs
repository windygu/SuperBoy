﻿using SuperBoy.Dynamic;
using SuperBoy.Dynamic.Interface;
using SuperBoy.Dynamic.Realize;

namespace SuperBoy.Cloud
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class SuperBoyICloud : ISuperBoyICloud
    {

        public string SuperBoyCloud(string cloudJson)
        {
            //调用页面序列化器皿
            ISerializationDynamic ser = new SerializationDynamic();
            ser.AnalyticalMain(cloudJson);
            return "";
        }
    }
}
