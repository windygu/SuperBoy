using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperBoyView
{
    /// <summary>
    /// this a class current use for transitional information
    /// </summary>
    public class SuperBoy
    {
        public string clientId { get; set; }
        public string controlDatabase { get; set; }
        public string controlTable { get; set; }
        public string controlOperation { get; set; }
        public string controlCondition { get; set; }
        public string other { get; set; }
        public string Key { get; set; }
        //  public int MyProperty { get; set; }
        // public int MyProperty { get; set; }
        public SuperBoy()
        {
            clientId = "";
            controlDatabase = "";
            controlTable = "";
            controlTable = "";
            controlOperation = "";
            controlCondition = "";
            other = "";
            Key = "";
        }
    }
}
