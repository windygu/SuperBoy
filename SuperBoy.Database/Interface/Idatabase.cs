using System.Collections.Generic;

namespace SuperBoy.Database.Interface
{
    public interface IDatabaseControl
    {
        Dictionary<string, string> AutoCallDatabaseInfo();

        void AutoCompressSerialize<T>(T value);
    }
}
