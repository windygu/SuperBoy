using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Data.OleDb;


namespace Core.IO
{
    /// <summary>
    /// CSV文件转换类
    /// </summary>
    public static class CsvHelper
    {
        /// <summary>
        /// 导出报表为Csv
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="strFilePath">物理路径</param>
        /// <param name="tableheader">表头</param>
        /// <param name="columname">字段标题,逗号分隔</param>
        public static bool Dt2Csv(DataTable dt, string strFilePath, string tableheader, string columname)
        {
            try
            {
                var strBufferLine = "";
                var strmWriterObj = new StreamWriter(strFilePath, false, System.Text.Encoding.UTF8);
                strmWriterObj.WriteLine(tableheader);
                strmWriterObj.WriteLine(columname);
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    strBufferLine = "";
                    for (var j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j > 0)
                            strBufferLine += ",";
                        strBufferLine += dt.Rows[i][j].ToString();
                    }
                    strmWriterObj.WriteLine(strBufferLine);
                }
                strmWriterObj.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 将Csv读入DataTable
        /// </summary>
        /// <param name="filePath">csv文件路径</param>
        /// <param name="n">表示第n行是字段title,第n+1行是记录开始</param>
        public static DataTable Csv2Dt(string filePath, int n, DataTable dt)
        {
            var reader = new StreamReader(filePath, System.Text.Encoding.UTF8, false);
            int i = 0, m = 0;
            reader.Peek();
            while (reader.Peek() > 0)
            {
                m = m + 1;
                var str = reader.ReadLine();
                if (m >= n + 1)
                {
                    var split = str.Split(',');

                    var dr = dt.NewRow();
                    for (i = 0; i < split.Length; i++)
                    {
                        dr[i] = split[i];
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        /// <summary>
        /// CSV转换成DataTable（OleDb数据库访问方式）
        /// </summary>
        /// <param name="csvPath">csv文件路径</param>
        /// <returns></returns>
        public static DataTable CsvToDataTableByOledb(string csvPath)
        {
            var csvdt = new DataTable("csv");
            if (!File.Exists(csvPath))
            {
                throw new FileNotFoundException("csv文件路径不存在!");
            }

            var fileInfo = new FileInfo(csvPath);
            using (var conn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileInfo.DirectoryName + ";Extended Properties='Text;'"))
            {
                var adapter = new OleDbDataAdapter("SELECT * FROM [" + fileInfo.Name + "]", conn);
                adapter.Fill(csvdt);
            }
            return csvdt;
        }

        /// <summary>
        /// CSV转换成DataTable（文件流方式）
        /// </summary>
        /// <param name="csvPath">csv文件路径</param>
        /// <returns></returns>
        public static DataTable CsvToDataTableByStreamReader(string csvPath)
        {
            var csvdt = new DataTable("csv");

            var intColCount = 0;
            var blnFlag = true;
            DataColumn column;
            DataRow row;
            string strline = null;
            string[] aryline;

            using (var reader = new StreamReader(csvPath, FileEncoding.GetEncoding(csvPath)))
            {
                while (!string.IsNullOrEmpty((strline = reader.ReadLine())))
                {
                    aryline = strline.Split(new char[] { ',' });

                    if (blnFlag)
                    {
                        blnFlag = false;
                        intColCount = aryline.Length;
                        for (var i = 0; i < aryline.Length; i++)
                        {
                            column = new DataColumn(aryline[i]);
                            csvdt.Columns.Add(column);
                        }
                        continue;
                    }

                    row = csvdt.NewRow();
                    for (var i = 0; i < intColCount; i++)
                    {
                        row[i] = aryline[i];
                    }
                    csvdt.Rows.Add(row);
                }
            }

            return csvdt;
        }

        /// <summary>
        /// DataTable 生成 CSV
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="csvPath">csv文件路径</param>
        public static void DataTableToCsv(DataTable dt, string csvPath)
        {
            if (null == dt)
                return;

            var csvText = new StringBuilder();
            var csvrowText = new StringBuilder();
            foreach (DataColumn dc in dt.Columns)
            {
                csvrowText.Append(",");
                csvrowText.Append(dc.ColumnName);
            }
            csvText.AppendLine(csvrowText.ToString().Substring(1));

            foreach (DataRow dr in dt.Rows)
            {
                csvrowText = new StringBuilder();
                foreach (DataColumn dc in dt.Columns)
                {
                    csvrowText.Append(",");
                    csvrowText.Append(dr[dc.ColumnName].ToString().Replace(',', ' '));
                }
                csvText.AppendLine(csvrowText.ToString().Substring(1));
            }

            File.WriteAllText(csvPath, csvText.ToString(), Encoding.Default);
        }
    
    }
}
