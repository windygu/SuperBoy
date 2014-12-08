using SuperBoy.Model.Public;
namespace SuperBoy.Model.Interface
{

    public interface ILogSystem
    {
        void CommonLog(string txt);
        void CommonLog(EnumArry.LogType logType, string txt);
    }
}