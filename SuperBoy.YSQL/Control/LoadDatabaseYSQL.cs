using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SuperBoy.YSQL.Model;
using SuperBoy.YSQL.Realize;

namespace SuperBoy.YSQL.Control
{
    internal static class LoadDatabaseYsql
    {

        internal static void LoadDatabase(Dictionary<string, string[]> address)
        {
            foreach (var addres in address["SysDatabase"])
            {
                //创建和加载实体数据库文件
                //判断是否存在
                if (File.Exists(addres))
                {
                    //存在则加载
                    try
                    {
                        var json = ServiceYSQL.ReadAndWrite.Read(addres);
                        //当前数据库信息
                        ServiceYSQL.EntityDatabase = JsonConvert.DeserializeObject<EntityTable>(json);
                        //取当前数据库名
                        //ServiceYSQL.CurrentDatabase = ServiceYSQL.EntityDatabase.TableHead[]
                    }
                    catch (Exception)
                    {
                        //数据库不符合规定
                        throw new Exception();
                    }
                }
                else
                {
                    //创建数据库
                    CreateYSQL.CreateBasetable(addres);
                }
            }
        }
    }
}