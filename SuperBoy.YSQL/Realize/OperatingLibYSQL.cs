using System;
using SuperBoy.Model.Public;
using SuperBoy.YSQL.Interface;

namespace SuperBoy.YSQL.Realize
{
    public class OperatingLibYsql : IOperatingLibYsql
    {
        public delegate bool Cud(string txt);

        public delegate object Select(EnumArryModel.ReturnType returnType, string txt);

        /// <summary>
        /// 该方法是增删改方法
        /// </summary>
        /// <param name="delege"></param>
        /// <param name="txt" />
        /// <returns></returns>
        public bool CnnectionCud(OperatingLibYsql.Cud delege, string txt)
        {
            return delege(txt);
        }

        public object CnnectionSelect(EnumArryModel.ReturnType returnValue, string txt)
        {
            
                throw new NotImplementedException();
            
        }

        /// <summary>
        /// 此方法为查询方法
        /// </summary>
        /// <param name="delege"></param>
        /// <param name="returnType"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public virtual object CnnectionSelect(OperatingLibYsql.Select delege, EnumArryModel.ReturnType returnType, string txt)
        {
            return delege(returnType, txt);
        }

        protected virtual string SelectDatabaseReturn(EnumArryModel.ReturnType returnType, string txt)
        {

            return null;
        }
    }
}
