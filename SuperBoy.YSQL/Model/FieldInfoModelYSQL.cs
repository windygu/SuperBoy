using System.Collections.Generic;

namespace SuperBoy.YSQL.Model
{
    public class FieldInfoModelYsql
    {
        /*
         
         {
   "$": {
       "@id": {
           "type": "int",
           "IsNull": "Null",
           "Pk": "true",
           "identity": {
               "startIndex": "1",
               "EndIndex": "100",
               "increment": "1"
           },
           "scope": "#sysDatabase[$table[@id]]"
       }
   }
}
         */
        public string Name { get; set; }
        public EnumArrayYSQL.FieldType FieldType { get; set; }//属性
        public bool IsNull { get; set; }//是否为空
        public bool Principal { get; set; }//是否主键 优级查询列
        public Dictionary<EnumArrayYSQL.Identity, string> Identity { get; set; }//自增属性
        public string Scope { get; set; }//关联

    }
}