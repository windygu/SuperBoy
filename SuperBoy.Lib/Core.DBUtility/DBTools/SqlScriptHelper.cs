﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Data;
using System.Data.SqlClient;

namespace Core.DBUtility
{
    /// <summary>
    /// OSql命令操作函数（可用于安装程序的时候数据库脚本执行）
    /// </summary>
    public class SqlScriptHelper
    {
        #region OSql操作函数

        /// <summary>
        /// 本地执行SQL脚本
        /// </summary>
        /// <param name="path">脚本文件路径全名</param>
        public static void DoSQL(string path)
        {
            var arguments = String.Format(" -E -S (local) -i \"{0}\"", path);
            RunDos("osql.exe", arguments, false);
        }

        /// <summary>
        /// 执行SQL脚本
        /// </summary>
        /// <param name="path">脚本文件路径全名</param>
        /// <param name="userId">数据库登录ID</param>
        /// <param name="password">数据库登录密码</param>
        /// <param name="server">数据库服务器地址</param>
        public static void DoSQL(string path, string userId, string password, string server)
        {
            // -U -P -S -i关键字区分大小写
            var arguments = String.Format(" -U {0} -P {1} -S {2} -i \"{3}\"", userId, password, server, path);

            RunDos("osql.exe", arguments, false);

            //			if(File.Exists(path))
            //			{
            //				File.Delete(path);
            //			}
        }

        /// <summary>
        /// 后台执行DOS文件
        /// </summary>
        /// <param name="fileName">文件名(包含路径)</param>
        /// <param name="argument">运行参数</param>
        /// <param name="hidden">是否隐藏窗口</param>
        public static void RunDos(string fileName, string argument, bool hidden)
        {
            var process = new Process();
            process.EnableRaisingEvents = false;
            process.StartInfo.FileName = string.Format("\"{0}\"", fileName);
            process.StartInfo.Arguments = argument;
            if (hidden)
            {
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }
            else
            {
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            }
            process.Start();
            process.WaitForExit();
        }

        ///   <summary>   
        ///   运行指定DOS命令行   
        ///   </summary>   
        ///   <param name="cmd">命令</param>   
        ///   <param name="tempFolder">命令行参数</param>   
        ///   <param name="comfirm">写入命令行的确认信息</param>   
        ///   <param name="showWindow">是否显示窗口</param> 
        ///   <returns></returns>   
        private static string ExecuteCmd(string cmd, string arg, string comfirm, bool showWindow)
        {
            var p = new Process();
            p.StartInfo.FileName = cmd;
            p.StartInfo.Arguments = arg;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;

            // 是否显示窗口
            p.StartInfo.CreateNoWindow = !showWindow;
            // 窗口状态
            if (showWindow)
            {
                p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            }
            else
            {
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }

            p.Start();
            if (comfirm != null)
                p.StandardInput.WriteLine(comfirm);
            var msg = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();

            return msg;
        }

        /// <summary>
        /// 在运行脚本之前把脚本中的数据库名称替换成安装界面输入的数据库名称
        /// </summary>
        /// <param name="filePath">脚本文件名</param>
        /// <param name="oldDbName">原有的数据库名称</param>
        /// <param name="newDbName">新的数据库名称</param>
        public static void ReplaceDbName(string filePath, string oldDbName, string newDbName)
        {
            if (newDbName.CompareTo(oldDbName) != 0)
            {
                var fileText = string.Empty;
                using (var streamReader = new StreamReader(filePath, Encoding.Default))
                {
                    fileText = streamReader.ReadToEnd();
                    fileText = fileText.Replace(oldDbName, newDbName);
                }

                using (var streamWriter = new StreamWriter(filePath, false, Encoding.Default))
                {
                    streamWriter.Write(fileText);
                }
            }
        }

        /// <summary>
        /// 为测试使用的函数
        /// </summary>
        /// <param name="fileText">输出的内容</param>
        private static void WriteLog(string fileText)
        {
            var filePath = "C:\\Log.txt";
            using (var streamWriter = new StreamWriter(filePath, true, Encoding.Default))
            {
                streamWriter.Write(fileText);
            }
        }

        /// <summary>
        /// 加入安装文件的路径，方便Web端访问
        /// </summary>
        public static void UpdatePathEnvironment(string physicalRoot)
        {
            //得到原来Path的变量值
            var registerKey = "SYSTEM\\ControlSet001\\Control\\Session Manager\\Environment";
            var key = "Path";
            var regKey = Registry.LocalMachine.OpenSubKey(registerKey);
            var result = regKey.GetValue(key).ToString();

            //添加新的值
            if (result.IndexOf(physicalRoot) < 0)
            {
                result += string.Format(";{0}", physicalRoot);
            }

            regKey = Registry.LocalMachine.OpenSubKey(registerKey, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.SetValue);
            regKey.SetValue(key, result);
        }

        #endregion

        #region 附加、分离、备份、恢复数据库操作

        /// <summary>
        /// 附加SqlServer数据库
        /// </summary>
        public bool AttachDb(string connectionString, string dataBaseName, string dataBaseMdf, string dataBaseLdf)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var comm = new SqlCommand();
                comm.Connection = conn;
                comm.CommandText = "sp_attach_db";
                comm.Parameters.Add(new SqlParameter("dbname", SqlDbType.NVarChar));
                comm.Parameters["dbname"].Value = dataBaseName;
                comm.Parameters.Add(new SqlParameter("filename1", SqlDbType.NVarChar));
                comm.Parameters["filename1"].Value = dataBaseMdf;
                comm.Parameters.Add(new SqlParameter("filename2", SqlDbType.NVarChar));
                comm.Parameters["filename2"].Value = dataBaseLdf;
                comm.CommandType = CommandType.StoredProcedure;
                comm.ExecuteNonQuery();
            }

            return true;
        }

        /// <summary>
        /// 分离SqlServer数据库
        /// </summary>
        public bool DetachDb(string connectionString, string dataBaseName)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var comm = new SqlCommand();
                comm.Connection = conn;
                comm.CommandText = "sp_detach_db";
                comm.Parameters.Add(new SqlParameter("dbname", SqlDbType.NVarChar));
                comm.Parameters["dbname"].Value = dataBaseName;
                comm.CommandType = CommandType.StoredProcedure;
                comm.ExecuteNonQuery();
            }
            return true;
        }

        /// <summary>
        /// 还原数据库
        /// </summary>
        public bool RestoreDataBase(string connectionString, string dataBaseName, string dataBaseOfBackupPath, string dataBaseOfBackupName)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var comm = new SqlCommand();
                comm.Connection = conn;
                comm.CommandText = "use master;restore database @DataBaseName From disk = @BackupFile with replace;";
                comm.Parameters.Add(new SqlParameter("DataBaseName", SqlDbType.NVarChar));
                comm.Parameters["DataBaseName"].Value = dataBaseName;
                comm.Parameters.Add(new SqlParameter("BackupFile", SqlDbType.NVarChar));
                comm.Parameters["BackupFile"].Value = Path.Combine(dataBaseOfBackupPath, dataBaseOfBackupName);
                comm.CommandType = CommandType.Text;
                comm.ExecuteNonQuery();
            }
            return true;
        }

        /// <summary>
        /// 备份SqlServer数据库
        /// </summary>
        public bool BackupDataBase(string connectionString, string dataBaseName, string dataBaseOfBackupPath, string dataBaseOfBackupName)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var comm = new SqlCommand();
                comm.Connection = conn;
                comm.CommandText = "use master;backup database @dbname to disk = @backupname;";
                comm.Parameters.Add(new SqlParameter("dbname", SqlDbType.NVarChar));
                comm.Parameters["dbname"].Value = dataBaseName;
                comm.Parameters.Add(new SqlParameter("backupname", SqlDbType.NVarChar));
                comm.Parameters["backupname"].Value = Path.Combine(dataBaseOfBackupPath, dataBaseOfBackupName);
                comm.CommandType = CommandType.Text;
                comm.ExecuteNonQuery();
            }
            return true;
        } 

        #endregion

    }
}
