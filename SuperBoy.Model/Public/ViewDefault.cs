using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperBoy.Model.Public
{
    /// <summary>
    /// reques head
    /// </summary>
    public class ViewDefault
    {
        //Request number
        //auto create ID
        public string No { get; set; }
        //need to operation
        //send type 
        public string Type { get; set; }

        public string Table { get; set; }

        public string Database { get; set; }
        //Key
        //option,update and inserter Key Name
        public string[] Key { get; set; }
        //value
        public string[] Value { get; set; }
        //get type
        public string ReceiveType { get; set; }
        //mi yao
        public string PDK { get; set; }

        public ViewDefault(string type, string[] key, string[] value, string receiveType)
        {

            //auto
            this.No = "";
            this.Table = "";
            this.Database = "";
            this.Type = type;
            this.Key = key;
            this.Value = value;
            this.ReceiveType = receiveType;
        }
    }

}
