using SuperBoy.Model.Public;
namespace SuperBoy.Model.Interface
{

    public interface ILogSystem
    {
        bool CommonLog(string systemInformation);
        bool CommonLog(EnumArry.LogType format, string SystemInformation);
    }
}