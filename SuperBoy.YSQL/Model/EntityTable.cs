using System.Collections.Generic;

namespace SuperBoy.YSQL.Model
{
    /// <summary>
    /// 系统数据实体类
    /// </summary>
    public class EntityTable
    {
        //头文件
        public Dictionary<EnumArray.TableHead, object> TableMaster { get; set; }
        //字段属性
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once CollectionNeverQueried.Global
        public Dictionary<string, Field> FieldInfoModel { get; set; }
        //内容
        //键为字段名字，值为字段的值
        public Dictionary<string, string[]> Value { get; set; }

        public EntityTable()
        {
            //初始化空的数据库类型
        }
        public EntityTable(Dictionary<string, string> fieldTypes, string tableName, string databaseName, string modifier, string sysNamespace, IEnumerable<string> userName)
        {
            #region 头文件
            this.TableMaster =
               new Dictionary<EnumArray.TableHead, object>
                {
                    {EnumArray.TableHead.DatabaseName, databaseName},
                    {EnumArray.TableHead.Namespace,sysNamespace},
                    {EnumArray.TableHead.TableName, tableName},
                    {EnumArray.TableHead.Modifier, modifier}
                };
            var list = new List<string>() { "sys", "admin", "jyf" };
            if (userName != null)
            {
                list.AddRange(userName);
            }
            TableMaster.Add(EnumArray.TableHead.Subjection, list);


            #endregion

            #region 循环添加到属性里面
            this.FieldInfoModel = new Dictionary<string, Field>();
            foreach (var fieldType in fieldTypes)
            {
                var fieldInfo = new Field
                {
                    FieldType = fieldType.Value,
                    IsNull = true,
                    Principal = false
                };
                this.FieldInfoModel.Add(fieldType.Key, fieldInfo);
            }
            #endregion
        }
    }
}