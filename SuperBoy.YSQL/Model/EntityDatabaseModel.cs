using System.Collections;
using System.Collections.Generic;
using SuperBoy.YSQL.Model;

namespace SuperBoy.YSQL.Control
{
    public class EntityDatabaseModel
    {
        public Dictionary<EnumArrayYSQL.TableHead, List<object>> TableHead { get; set; }
        //字段属性
        public List<FieldInfoModelYsql> FieldInfoModel { get; set; }
        //内容
        //键为字段名字，值为字段的值
        public Dictionary<string, string[]> Value { get; set; }

        public EntityDatabaseModel()
        {

        }
        public EntityDatabaseModel(Dictionary<string, EnumArrayYSQL.FieldType> fieldTypes, string tableName, string databaseName, string modifier, string sysNamespace, IEnumerable<string> userName)
        {
            this.TableHead = new Dictionary<EnumArrayYSQL.TableHead, List<object>>
            {
                {EnumArrayYSQL.TableHead.DatabaseName, new List<object>() {databaseName}},
                {EnumArrayYSQL.TableHead.Namespace, new List<object>() {sysNamespace}},
                {EnumArrayYSQL.TableHead.TableName, new List<object>() {tableName}},
                {EnumArrayYSQL.TableHead.Modifier, new List<object>() {modifier}},
                {EnumArrayYSQL.TableHead.Subjection, new List<object>() {"sys","admin","jyf"}}
            };
            this.FieldInfoModel = new List<FieldInfoModelYsql>();
            //拼接基础字段
            foreach (var fieldType in fieldTypes)
            {
                var fieldInfo = new FieldInfoModelYsql
                {
                    Name = fieldType.Key,
                    FieldType = fieldType.Value
                };
                this.FieldInfoModel.Add(fieldInfo);
            }

            //将用户添加进去
            if (userName == null) return;
            this.TableHead[EnumArrayYSQL.TableHead.Subjection].AddRange(userName);
        }
    }
}