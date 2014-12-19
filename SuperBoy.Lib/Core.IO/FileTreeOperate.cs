using System;
using System.Text;
using System.Web;
using System.IO;

namespace Core.IO
{
    public class FileTreeOperate
    {

        #region 获取指定文件夹下所有子目录及文件(树形)
        
        /// <summary>
        /// 获取指定文件夹下所有子目录及文件
        /// </summary>
        /// <param name="path">详细路径</param>
        public static string GetFoldAll(string path)
        {

            var str = "";
            var thisOne = new DirectoryInfo(path);
            str = ListTreeShow(thisOne, 0, str);
            return str;

        }

        /// <summary>
        /// 获取指定文件夹下所有子目录及文件函数
        /// </summary>
        /// <param name="theDir">指定目录</param>
        /// <param name="nLevel">默认起始值,调用时,一般为0</param>
        /// <param name="rn">用于迭加的传入值,一般为空</param>
        /// <returns></returns>
        public static string ListTreeShow(DirectoryInfo theDir, int nLevel, string rn)//递归目录 文件
        {
            var subDirectories = theDir.GetDirectories();//获得目录
            foreach (var dirinfo in subDirectories)
            {

                if (nLevel == 0)
                {
                    rn += "├";
                }
                else
                {
                    var s = "";
                    for (var i = 1; i <= nLevel; i++)
                    {
                        s += "│&nbsp;";
                    }
                    rn += s + "├";
                }
                rn += "<b>" + dirinfo.Name.ToString() + "</b><br />";
                var fileInfo = dirinfo.GetFiles();   //目录下的文件
                foreach (var fInfo in fileInfo)
                {
                    if (nLevel == 0)
                    {
                        rn += "│&nbsp;├";
                    }
                    else
                    {
                        var f = "";
                        for (var i = 1; i <= nLevel; i++)
                        {
                            f += "│&nbsp;";
                        }
                        rn += f + "│&nbsp;├";
                    }
                    rn += fInfo.Name.ToString() + " <br />";
                }
                rn = ListTreeShow(dirinfo, nLevel + 1, rn);


            }
            return rn;
        }


        
        /// <summary>
        /// 获取指定文件夹下所有子目录及文件(下拉框形)
        /// </summary>
        /// <param name="path">详细路径</param>
        ///<param name="dropName">下拉列表名称</param>
        ///<param name="tplPath">默认选择模板名称</param>
        public static string GetFoldAll(string path, string dropName, string tplPath)
        {
            var strDrop = "<select name=\"" + dropName + "\" id=\"" + dropName + "\"><option value=\"\">--请选择详细模板--</option>";
            var str = "";
            var thisOne = new DirectoryInfo(path);
            str = ListTreeShow(thisOne, 0, str, tplPath);
            return strDrop + str + "</select>";

        }

        /// <summary>
        /// 获取指定文件夹下所有子目录及文件函数
        /// </summary>
        /// <param name="theDir">指定目录</param>
        /// <param name="nLevel">默认起始值,调用时,一般为0</param>
        /// <param name="rn">用于迭加的传入值,一般为空</param>
        /// <param name="tplPath">默认选择模板名称</param>
        /// <returns></returns>
        public static string ListTreeShow(DirectoryInfo theDir, int nLevel, string rn, string tplPath)//递归目录 文件
        {
            var subDirectories = theDir.GetDirectories();//获得目录

            foreach (var dirinfo in subDirectories)
            {

                rn += "<option value=\"" + dirinfo.Name.ToString() + "\"";
                if (tplPath.ToLower() == dirinfo.Name.ToString().ToLower())
                {
                    rn += " selected ";
                }
                rn += ">";

                if (nLevel == 0)
                {
                    rn += "┣";
                }
                else
                {
                    var s = "";
                    for (var i = 1; i <= nLevel; i++)
                    {
                        s += "│&nbsp;";
                    }
                    rn += s + "┣";
                }
                rn += "" + dirinfo.Name.ToString() + "</option>";


                var fileInfo = dirinfo.GetFiles();   //目录下的文件
                foreach (var fInfo in fileInfo)
                {
                    rn += "<option value=\"" + dirinfo.Name.ToString() + "/" + fInfo.Name.ToString() + "\"";
                    if (tplPath.ToLower() == fInfo.Name.ToString().ToLower())
                    {
                        rn += " selected ";
                    }
                    rn += ">";

                    if (nLevel == 0)
                    {
                        rn += "│&nbsp;├";
                    }
                    else
                    {
                        var f = "";
                        for (var i = 1; i <= nLevel; i++)
                        {
                            f += "│&nbsp;";
                        }
                        rn += f + "│&nbsp;├";
                    }
                    rn += fInfo.Name.ToString() + "</option>";
                }
                rn = ListTreeShow(dirinfo, nLevel + 1, rn, tplPath);


            }
            return rn;
        }
        #endregion
 

    }
}
