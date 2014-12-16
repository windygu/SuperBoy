using SuperBoy.Model.Public;

namespace SuperBoy.Model.Interface
{
    public interface IAnalyticalModel
    {
        string Select(DatabseSend database);

        string Inster(DatabseSend database);

        string Update(DatabseSend database);
        string Delete(DatabseSend database);

        string Online(DatabseSend database);
        string Manage(DatabseSend database);
       
    }
}