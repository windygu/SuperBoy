using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperBoy.Database.Interface
{
    public interface Idatabase
    {
        Dictionary<string, object> AutoCallDatabaseInfo();
    }
}
