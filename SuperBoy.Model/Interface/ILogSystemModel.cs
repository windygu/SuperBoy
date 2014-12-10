using SuperBoy.Model.Public;
namespace SuperBoy.Model.Interface
{

    public interface ILogSystemModel
    {
        bool CommonLog(string systemInformation);
        bool CommonLog(EnumArryModel.LogType format, string SystemInformation);
    }
}