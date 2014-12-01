using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperBoy.Model.Controller
{
    public class ModelController
    {
        public static Dictionary<SuperBoy.Model.Public.EnumArry.Master, object> Dicts = null;
        public static void Load(Dictionary<SuperBoy.Model.Public.EnumArry.Master, object> Dict)
        {
            //load pages
            Dicts = Dict;

        }
    }
}
