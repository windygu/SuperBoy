using SuperBoy.Model.Public;
using SuperBoy.YSQL.Realize;

namespace SuperBoy.YSQL.Interface
{

    public interface IOperatingLibYsql
    {
        bool CnnectionCud(OperatingLibYsql.Cud delege, string txt);

        object CnnectionSelect(EnumArryModel.ReturnType returnValue, string txt);


    }
}
