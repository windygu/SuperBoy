using System;
using System.Collections;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using DataGrid = System.Windows.Forms.DataGrid;


namespace Core.Office
{
    /// <summary>
    /// Excel操作辅助类（无需VBA引用）
    /// </summary>
    public class ExcelHelper
    {

        /// <summary>
        /// Excel 版本
        /// </summary>
        public enum ExcelType
        {
            Excel2003, Excel2007
        }

        /// <summary>
        /// IMEX 三种模式。
        /// IMEX是用来告诉驱动程序使用Excel文件的模式，其值有0、1、2三种，分别代表导出、导入、混合模式。
        /// </summary>
        public enum ImexType
        {
            ExportMode = 0, ImportMode = 1, LinkedMode = 2
        }

        #region 获取Excel连接字符串

        /// <summary>
        /// 返回Excel 连接字符串   [IMEX=1]
        /// </summary>
        /// <param name="excelPath">Excel文件 绝对路径</param>
        /// <param name="header">是否把第一行作为列名</param>
        /// <param name="eType">Excel 版本 </param>
        /// <returns></returns>
        public static string GetExcelConnectstring(string excelPath, bool header, ExcelType eType)
        {
            return GetExcelConnectstring(excelPath, header, eType, ImexType.ImportMode);
        }

        /// <summary>
        /// 返回Excel 连接字符串
        /// </summary>
        /// <param name="excelPath">Excel文件 绝对路径</param>
        /// <param name="header">是否把第一行作为列名</param>
        /// <param name="eType">Excel 版本 </param>
        /// <param name="imex">IMEX模式</param>
        /// <returns></returns>
        public static string GetExcelConnectstring(string excelPath, bool header, ExcelType eType, ImexType imex)
        {
            if (!File.Exists(excelPath))
                throw new FileNotFoundException("Excel路径不存在!");

            var connectstring = string.Empty;

            var hdr = "NO";
            if (header)
                hdr = "YES";

            if (eType == ExcelType.Excel2003)
                connectstring = "Provider=Microsoft.Jet.OleDb.4.0; data source=" + excelPath + ";Extended Properties='Excel 8.0; HDR=" + hdr + "; IMEX=" + imex.GetHashCode() + "'";
            else
                connectstring = "Provider=Microsoft.ACE.OLEDB.12.0; data source=" + excelPath + ";Extended Properties='Excel 12.0 Xml; HDR=" + hdr + "; IMEX=" + imex.GetHashCode() + "'";

            return connectstring;
        }

        #endregion

        #region 获取Excel工作表名

        /// <summary>
        /// 返回Excel工作表名
        /// </summary>
        /// <param name="excelPath">Excel文件 绝对路径</param>
        /// <param name="eType">Excel 版本 </param>
        /// <returns></returns>
        public static List<string> GetExcelTablesName(string excelPath, ExcelType eType)
        {
            var connectstring = GetExcelConnectstring(excelPath, true, eType);
            return GetExcelTablesName(connectstring);
        }

        /// <summary>
        /// 返回Excel工作表名
        /// </summary>
        /// <param name="connectstring">excel连接字符串</param>
        /// <returns></returns>
        public static List<string> GetExcelTablesName(string connectstring)
        {
            using (var conn = new OleDbConnection(connectstring))
            {
                return GetExcelTablesName(conn);
            }
        }

        /// <summary>
        /// 返回Excel工作表名
        /// </summary>
        /// <param name="connection">excel连接</param>
        /// <returns></returns>
        public static List<string> GetExcelTablesName(OleDbConnection connection)
        {
            var list = new List<string>();

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var dt = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if (dt != null && dt.Rows.Count > 0)
            {
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    list.Add(ConvertTo<string>(dt.Rows[i][2]));
                }
            }

            return list;
        }
        
        /// <summary>
        /// 返回Excel第一个工作表表名
        /// </summary>
        /// <param name="excelPath">Excel文件 绝对路径</param>
        /// <param name="eType">Excel 版本 </param>
        /// <returns></returns>
        public static string GetExcelFirstTableName(string excelPath, ExcelType eType)
        {
            var connectstring = GetExcelConnectstring(excelPath, true, eType);
            return GetExcelFirstTableName(connectstring);
        }

        /// <summary>
        /// 返回Excel第一个工作表表名
        /// </summary>
        /// <param name="connectstring">excel连接字符串</param>
        /// <returns></returns>
        public static string GetExcelFirstTableName(string connectstring)
        {
            using (var conn = new OleDbConnection(connectstring))
            {
                return GetExcelFirstTableName(conn);
            }
        }

        /// <summary>
        /// 返回Excel第一个工作表表名
        /// </summary>
        /// <param name="connection">excel连接</param>
        /// <returns></returns>
        public static string GetExcelFirstTableName(OleDbConnection connection)
        {
            var tableName = string.Empty;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var dt = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if (dt != null && dt.Rows.Count > 0)
            {
                tableName = ConvertTo<string>(dt.Rows[0][2]);
            }

            return tableName;
        }

        /// <summary>
        /// 获取Excel文件中指定工作表的列
        /// </summary>
        /// <param name="excelPath">Excel文件 绝对路径</param>
        /// <param name="table">名称 excel table  例如：Sheet1$</param>
        /// <returns></returns>
        public static List<string> GetColumnsList(string excelPath, ExcelType eType, string table)
        {
            var list = new List<string>();
            DataTable tableColumns = null;
            var connectstring = GetExcelConnectstring(excelPath, true, eType);
            using (var conn = new OleDbConnection(connectstring))
            {
                conn.Open();
                tableColumns = GetReaderSchema(table, conn);
            }
            foreach (DataRow dr in tableColumns.Rows)
            {
                var columnName = dr["ColumnName"].ToString();
                var datatype = ((OleDbType)dr["ProviderType"]).ToString();//对应数据库类型
                var netType = dr["DataType"].ToString();//对应的.NET类型，如System.String
                list.Add(columnName);
            }         

            return list;
        }

        private static DataTable GetReaderSchema(string tableName, OleDbConnection connection)
        {
            DataTable schemaTable = null;
            IDbCommand cmd = new OleDbCommand();
            cmd.CommandText = string.Format("select * from [{0}]", tableName);
            cmd.Connection = connection;

            using (var reader = cmd.ExecuteReader(CommandBehavior.KeyInfo | CommandBehavior.SchemaOnly))
            {
                schemaTable = reader.GetSchemaTable();
            }
            return schemaTable;
        }

        #endregion

        #region EXCEL导入DataSet

        /// <summary>
        /// EXCEL导入DataSet
        /// </summary>
        /// <param name="excelPath">Excel文件 绝对路径</param>
        /// <param name="table">名称 excel table  例如：Sheet1$ </param>
        /// <param name="header">是否把第一行作为列名</param>
        /// <param name="eType">Excel 版本 </param>
        /// <returns>返回Excel相应工作表中的数据 DataSet   [table不存在时返回空的DataSet]</returns>
        public static DataSet ExcelToDataSet(string excelPath, string table, bool header, ExcelType eType)
        {
            var connectstring = GetExcelConnectstring(excelPath, header, eType);
            return ExcelToDataSet(connectstring, table);
        }

        /// <summary>
        /// 判断工作表名是否存在
        /// </summary>
        /// <param name="connection">excel连接</param>
        /// <param name="table">名称 excel table  例如：Sheet1$</param>
        /// <returns></returns>
        private static bool IsExistExcelTableName(OleDbConnection connection, string table)
        {
            var list = GetExcelTablesName(connection);
            foreach (var tName in list)
            {
                if (tName.ToLower() == table.ToLower())
                    return true;
            }

            return false;
        }

        /// <summary>
        /// EXCEL导入DataSet
        /// </summary>
        /// <param name="connectstring">excel连接字符串</param>
        /// <param name="table">名称 excel table  例如：Sheet1$ </param>
        /// <returns>返回Excel相应工作表中的数据 DataSet   [table不存在时返回空的DataSet]</returns>
        public static DataSet ExcelToDataSet(string connectstring, string table)
        {
            using (var conn = new OleDbConnection(connectstring))
            {
                var ds = new DataSet();

                //判断该工作表在Excel中是否存在
                if (IsExistExcelTableName(conn, table))
                {
                    var adapter = new OleDbDataAdapter("SELECT * FROM [" + table + "]", conn);
                    adapter.Fill(ds, table);
                }

                return ds;
            }
        }

        /// <summary>
        /// EXCEL所有工作表导入DataSet
        /// </summary>
        /// <param name="excelPath">Excel文件 绝对路径</param>
        /// <param name="header">是否把第一行作为列名</param>
        /// <param name="eType">Excel 版本 </param>
        /// <returns>返回Excel第一个工作表中的数据 DataSet </returns>
        public static DataSet ExcelToDataSet(string excelPath, bool header, ExcelType eType)
        {
            var connectstring = GetExcelConnectstring(excelPath, header, eType);
            return ExcelToDataSet(connectstring);
        }

        /// <summary>
        /// EXCEL所有工作表导入DataSet
        /// </summary>
        /// <param name="connectstring">excel连接字符串</param>
        /// <returns>返回Excel第一个工作表中的数据 DataSet </returns>
        public static DataSet ExcelToDataSet(string connectstring)
        {
            using (var conn = new OleDbConnection(connectstring))
            {
                var ds = new DataSet();
                var tableNames = GetExcelTablesName(conn);

                foreach (var tableName in tableNames)
                {
                    var adapter = new OleDbDataAdapter("SELECT * FROM [" + tableName + "]", conn);
                    adapter.Fill(ds, tableName);
                }
                return ds;
            }
        }

        #endregion

        #region 数据导出至Excel文件

        #region 把一个数据集中的数据导出到Excel文件中(XML格式操作)
        
        /// <summary>
        /// 把一个数据集中的数据导出到Excel文件中(XML格式操作)
        /// </summary>
        /// <param name="source">DataSet数据</param>
        /// <param name="fileName">保存的Excel文件名</param>
        public static void DataSetToExcel(DataSet source, string fileName)
        {
            #region Excel格式内容
            var excelDoc = new StreamWriter(fileName);
            const string startExcelXml = "<xml version>\r\n<Workbook " +
                  "xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n" +
                  " xmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\n " +
                  "xmlns:x=\"urn:schemas-    microsoft-com:office:" +
                  "excel\"\r\n xmlns:ss=\"urn:schemas-microsoft-com:" +
                  "office:spreadsheet\">\r\n <Styles>\r\n " +
                  "<Style ss:ID=\"Default\" ss:Name=\"Normal\">\r\n " +
                  "<Alignment ss:Vertical=\"Bottom\"/>\r\n <Borders/>" +
                  "\r\n <Font/>\r\n <Interior/>\r\n <NumberFormat/>" +
                  "\r\n <Protection/>\r\n </Style>\r\n " +
                  "<Style ss:ID=\"BoldColumn\">\r\n <Font " +
                  "x:Family=\"Swiss\" ss:Bold=\"1\"/>\r\n </Style>\r\n " +
                  "<Style     ss:ID=\"StringLiteral\">\r\n <NumberFormat" +
                  " ss:Format=\"@\"/>\r\n </Style>\r\n <Style " +
                  "ss:ID=\"Decimal\">\r\n <NumberFormat " +
                  "ss:Format=\"#,##0.###\"/>\r\n </Style>\r\n " +
                  "<Style ss:ID=\"Integer\">\r\n <NumberFormat " +
                  "ss:Format=\"0\"/>\r\n </Style>\r\n <Style " +
                  "ss:ID=\"DateLiteral\">\r\n <NumberFormat " +
                  "ss:Format=\"yyyy-mm-dd;@\"/>\r\n </Style>\r\n " +
                  "</Styles>\r\n ";
            const string endExcelXml = "</Workbook>";
            #endregion

            var sheetCount = 1;
            excelDoc.Write(startExcelXml);
            for (var i = 0; i < source.Tables.Count; i++)
            {
                var rowCount = 0;
                var dt = source.Tables[i];

                excelDoc.Write("<Worksheet ss:Name=\"Sheet" + sheetCount + "\">");
                excelDoc.Write("<Table>");
                excelDoc.Write("<Row>");
                for (var x = 0; x < dt.Columns.Count; x++)
                {
                    excelDoc.Write("<Cell ss:StyleID=\"BoldColumn\"><Data ss:Type=\"String\">");
                    excelDoc.Write(source.Tables[0].Columns[x].ColumnName);
                    excelDoc.Write("</Data></Cell>");
                }
                excelDoc.Write("</Row>");
                foreach (DataRow x in dt.Rows)
                {
                    rowCount++;
                    //if the number of rows is > 64000 create a new page to continue output

                    if (rowCount == 64000)
                    {
                        rowCount = 0;
                        sheetCount++;
                        excelDoc.Write("</Table>");
                        excelDoc.Write(" </Worksheet>");
                        excelDoc.Write("<Worksheet ss:Name=\"Sheet" + sheetCount + "\">");
                        excelDoc.Write("<Table>");
                    }
                    excelDoc.Write("<Row>"); //ID=" + rowCount + "

                    for (var y = 0; y < source.Tables[0].Columns.Count; y++)
                    {
                        var rowType = x[y].GetType();
                        #region 根据不同数据类型生成内容
                        switch (rowType.ToString())
                        {
                            case "System.String":
                                var xmLstring = x[y].ToString();
                                xmLstring = xmLstring.Trim();
                                xmLstring = xmLstring.Replace("&", "&");
                                xmLstring = xmLstring.Replace(">", ">");
                                xmLstring = xmLstring.Replace("<", "<");
                                excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                               "<Data ss:Type=\"String\">");
                                excelDoc.Write(xmLstring);
                                excelDoc.Write("</Data></Cell>");
                                break;
                            case "System.DateTime":
                                //Excel has a specific Date Format of YYYY-MM-DD followed by
                                //the letter 'T' then hh:mm:sss.lll Example 2005-01-31T24:01:21.000
                                //The Following Code puts the date stored in XMLDate
                                //to the format above

                                var xmlDate = (DateTime)x[y];

                                var xmlDatetoString = xmlDate.Year +
                                                         "-" +
                                                         (xmlDate.Month < 10
                                                              ? "0" +
                                                                xmlDate.Month
                                                              : xmlDate.Month.ToString()) +
                                                         "-" +
                                                         (xmlDate.Day < 10
                                                              ? "0" +
                                                                xmlDate.Day
                                                              : xmlDate.Day.ToString()) +
                                                         "T" +
                                                         (xmlDate.Hour < 10
                                                              ? "0" +
                                                                xmlDate.Hour
                                                              : xmlDate.Hour.ToString()) +
                                                         ":" +
                                                         (xmlDate.Minute < 10
                                                              ? "0" +
                                                                xmlDate.Minute
                                                              : xmlDate.Minute.ToString()) +
                                                         ":" +
                                                         (xmlDate.Second < 10
                                                              ? "0" +
                                                                xmlDate.Second
                                                              : xmlDate.Second.ToString()) +
                                                         ".000";
                                excelDoc.Write("<Cell ss:StyleID=\"DateLiteral\">" +
                                             "<Data ss:Type=\"DateTime\">");
                                excelDoc.Write(xmlDatetoString);
                                excelDoc.Write("</Data></Cell>");
                                break;
                            case "System.Boolean":
                                excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                            "<Data ss:Type=\"String\">");
                                excelDoc.Write(x[y].ToString());
                                excelDoc.Write("</Data></Cell>");
                                break;
                            case "System.Int16":
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Byte":
                                excelDoc.Write("<Cell ss:StyleID=\"Integer\">" +
                                        "<Data ss:Type=\"Number\">");
                                excelDoc.Write(x[y].ToString());
                                excelDoc.Write("</Data></Cell>");
                                break;
                            case "System.Decimal":
                            case "System.Double":
                                excelDoc.Write("<Cell ss:StyleID=\"Decimal\">" +
                                      "<Data ss:Type=\"Number\">");
                                excelDoc.Write(x[y].ToString());
                                excelDoc.Write("</Data></Cell>");
                                break;
                            case "System.DBNull":
                                excelDoc.Write("<Cell ss:StyleID=\"StringLiteral\">" +
                                      "<Data ss:Type=\"String\">");
                                excelDoc.Write("");
                                excelDoc.Write("</Data></Cell>");
                                break;
                            default:
                                throw (new Exception(rowType.ToString() + " not handled."));
                        }
                        #endregion
                    }
                    excelDoc.Write("</Row>");
                }
                excelDoc.Write("</Table>");
                excelDoc.Write(" </Worksheet>");

                sheetCount++;
            }

            excelDoc.Write(endExcelXml);
            excelDoc.Close();
        }
        #endregion

        #region 将DataTable导出为Excel(OleDb 方式操作）

       
        /// <summary>
        /// 将DataTable导出为Excel(OleDb 方式操作）
        /// </summary>
        /// <param name="dataTable">表</param>
        /// <param name="fileName">导出默认文件名</param>
        public static void DataSetToExcel(DataTable dataTable, string fileName)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "xls files (*.xls)|*.xls";
            saveFileDialog.FileName = fileName;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = saveFileDialog.FileName;
                if (File.Exists(fileName))
                {
                    try
                    {
                        File.Delete(fileName);
                    }
                    catch
                    {
                        MessageBox.Show("该文件正在使用中,关闭文件或重新命名导出文件再试!");
                        return;
                    }
                }
                var oleDbConn = new OleDbConnection();
                var oleDbCmd = new OleDbCommand();
                var sSql = "";
                try
                {
                    oleDbConn.ConnectionString = "Provider=Microsoft.Jet.OleDb.4.0;Data Source=" + fileName + @";Extended ProPerties=""Excel 8.0;HDR=Yes;""";
                    oleDbConn.Open();
                    oleDbCmd.CommandType = CommandType.Text;
                    oleDbCmd.Connection = oleDbConn;
                    sSql = "CREATE TABLE sheet1 (";
                    for (var i = 0; i < dataTable.Columns.Count; i++)
                    {
                        // 字段名称出现关键字会导致错误。
                        if (i < dataTable.Columns.Count - 1)
                            sSql += "[" + dataTable.Columns[i].Caption + "] TEXT(100) ,";
                        else
                            sSql += "[" + dataTable.Columns[i].Caption + "] TEXT(200) )";
                    }
                    oleDbCmd.CommandText = sSql;
                    oleDbCmd.ExecuteNonQuery();
                    for (var j = 0; j < dataTable.Rows.Count; j++)
                    {
                        sSql = "INSERT INTO sheet1 VALUES('";
                        for (var i = 0; i < dataTable.Columns.Count; i++)
                        {
                            if (i < dataTable.Columns.Count - 1)
                                sSql += dataTable.Rows[j][i].ToString() + " ','";
                            else
                                sSql += dataTable.Rows[j][i].ToString() + " ')";
                        }
                        oleDbCmd.CommandText = sSql;
                        oleDbCmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("导出EXCEL成功");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("导出EXCEL失败:" + ex.Message);
                }
                finally
                {
                    oleDbCmd.Dispose();
                    oleDbConn.Close();
                    oleDbConn.Dispose();
                }
            }
        }
        #endregion

        #region   导出Excel文件，自动返回可下载的文件流

        /// <summary> 
        /// 导出Excel文件，自动返回可下载的文件流 
        /// </summary> 
        public static void DataTable1Excel(DataTable dtData)
        {
            GridView gvExport = null;
            var curContext = HttpContext.Current;
            StringWriter strWriter = null;
            HtmlTextWriter htmlWriter = null;
            if (dtData != null)
            {
                curContext.Response.ContentType = "application/vnd.ms-excel";
                curContext.Response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
                curContext.Response.Charset = "utf-8";
                strWriter = new StringWriter();
                htmlWriter = new HtmlTextWriter(strWriter);
                gvExport = new GridView();
                gvExport.DataSource = dtData.DefaultView;
                gvExport.AllowPaging = false;
                gvExport.DataBind();
                gvExport.RenderControl(htmlWriter);
                curContext.Response.Write("<meta http-equiv=\"Content-Type\" content=\"text/html;charset=gb2312\"/>" + strWriter.ToString());
                curContext.Response.End();
            }
        }
        #endregion

        #region  导出Excel文件，转换为可读模式

        
        /// <summary>
        /// 导出Excel文件，转换为可读模式
        /// </summary>
        public static void DataTable2Excel(DataTable dtData)
        {
            System.Web.UI.WebControls.DataGrid dgExport = null;
            var curContext = HttpContext.Current;
            StringWriter strWriter = null;
            HtmlTextWriter htmlWriter = null;

            if (dtData != null)
            {
                curContext.Response.ContentType = "application/vnd.ms-excel";
                curContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
                curContext.Response.Charset = "";
                strWriter = new StringWriter();
                htmlWriter = new HtmlTextWriter(strWriter);
                dgExport = new System.Web.UI.WebControls.DataGrid();
                dgExport.DataSource = dtData.DefaultView;
                dgExport.AllowPaging = false;
                dgExport.DataBind();
                dgExport.RenderControl(htmlWriter);
                curContext.Response.Write(strWriter.ToString());
                curContext.Response.End();
            }
        }
        #endregion

        #region    导出Excel文件，并自定义文件名
      
        /// <summary>
        /// 导出Excel文件，并自定义文件名
        /// </summary>
        public static void DataTable3Excel(DataTable dtData, String fileName)
        {
            GridView dgExport = null;
            var curContext = HttpContext.Current;
            StringWriter strWriter = null;
            HtmlTextWriter htmlWriter = null;

            if (dtData != null)
            {
                HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8);
                curContext.Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + ".xls");
                curContext.Response.ContentType = "application nd.ms-excel";
                curContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
                curContext.Response.Charset = "GB2312";
                strWriter = new StringWriter();
                htmlWriter = new HtmlTextWriter(strWriter);
                dgExport = new GridView();
                dgExport.DataSource = dtData.DefaultView;
                dgExport.AllowPaging = false;
                dgExport.DataBind();
                dgExport.RenderControl(htmlWriter);
                curContext.Response.Write(strWriter.ToString());
                curContext.Response.End();
            }
        }
        #endregion

        #region 将数据导出至Excel文件
       
        /// <summary>
        /// 将数据导出至Excel文件
        /// </summary>
        /// <param name="table">DataTable对象</param>
        /// <param name="excelFilePath">Excel文件路径</param>
        public static bool OutputToExcel(DataTable table, string excelFilePath)
        {
            if (File.Exists(excelFilePath))
            {
                throw new Exception("该文件已经存在！");
            }

            if ((table.TableName.Trim().Length == 0) || (table.TableName.ToLower() == "table"))
            {
                table.TableName = "Sheet1";
            }

            //数据表的列数
            var colCount = table.Columns.Count;

            //用于记数，实例化参数时的序号
            var i = 0;

            //创建参数
            var para = new OleDbParameter[colCount];

            //创建表结构的SQL语句
            var tableStructStr = @"Create Table " + table.TableName + "(";

            //连接字符串
            var connString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + excelFilePath + ";Extended Properties=Excel 8.0;";
            var objConn = new OleDbConnection(connString);

            //创建表结构
            var objCmd = new OleDbCommand();

            //数据类型集合
            var dataTypeList = new ArrayList();
            dataTypeList.Add("System.Decimal");
            dataTypeList.Add("System.Double");
            dataTypeList.Add("System.Int16");
            dataTypeList.Add("System.Int32");
            dataTypeList.Add("System.Int64");
            dataTypeList.Add("System.Single");

            //遍历数据表的所有列，用于创建表结构
            foreach (DataColumn col in table.Columns)
            {
                //如果列属于数字列，则设置该列的数据类型为double
                if (dataTypeList.IndexOf(col.DataType.ToString()) >= 0)
                {
                    para[i] = new OleDbParameter("@" + col.ColumnName, OleDbType.Double);
                    objCmd.Parameters.Add(para[i]);

                    //如果是最后一列
                    if (i + 1 == colCount)
                    {
                        tableStructStr += col.ColumnName + " double)";
                    }
                    else
                    {
                        tableStructStr += col.ColumnName + " double,";
                    }
                }
                else
                {
                    para[i] = new OleDbParameter("@" + col.ColumnName, OleDbType.VarChar);
                    objCmd.Parameters.Add(para[i]);

                    //如果是最后一列
                    if (i + 1 == colCount)
                    {
                        tableStructStr += col.ColumnName + " varchar)";
                    }
                    else
                    {
                        tableStructStr += col.ColumnName + " varchar,";
                    }
                }
                i++;
            }

            //创建Excel文件及文件结构
            try
            {
                objCmd.Connection = objConn;
                objCmd.CommandText = tableStructStr;

                if (objConn.State == ConnectionState.Closed)
                {
                    objConn.Open();
                }
                objCmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                throw exp;
            }

            //插入记录的SQL语句
            var insertSql1 = "Insert into " + table.TableName + " (";
            var insertSql2 = " Values (";
            var insertSql = "";

            //遍历所有列，用于插入记录，在此创建插入记录的SQL语句
            for (var colId = 0; colId < colCount; colId++)
            {
                if (colId + 1 == colCount)  //最后一列
                {
                    insertSql1 += table.Columns[colId].ColumnName + ")";
                    insertSql2 += "@" + table.Columns[colId].ColumnName + ")";
                }
                else
                {
                    insertSql1 += table.Columns[colId].ColumnName + ",";
                    insertSql2 += "@" + table.Columns[colId].ColumnName + ",";
                }
            }

            insertSql = insertSql1 + insertSql2;

            //遍历数据表的所有数据行
            for (var rowId = 0; rowId < table.Rows.Count; rowId++)
            {
                for (var colId = 0; colId < colCount; colId++)
                {
                    if (para[colId].DbType == DbType.Double && table.Rows[rowId][colId].ToString().Trim() == "")
                    {
                        para[colId].Value = 0;
                    }
                    else
                    {
                        para[colId].Value = table.Rows[rowId][colId].ToString().Trim();
                    }
                }
                try
                {
                    objCmd.CommandText = insertSql;
                    objCmd.ExecuteNonQuery();
                }
                catch (Exception exp)
                {
                    var str = exp.Message;
                }
            }
            try
            {
                if (objConn.State == ConnectionState.Open)
                {
                    objConn.Close();
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
            return true;
        }
        #endregion

        #region 将数据导出至Excel文件

       
        /// <summary>
        /// 将数据导出至Excel文件
        /// </summary>
        /// <param name="table">DataTable对象</param>
        /// <param name="columns">要导出的数据列集合</param>
        /// <param name="excelFilePath">Excel文件路径</param>
        public static bool OutputToExcel(DataTable table, ArrayList columns, string excelFilePath)
        {
            if (File.Exists(excelFilePath))
            {
                throw new Exception("该文件已经存在！");
            }

            //如果数据列数大于表的列数，取数据表的所有列
            if (columns.Count > table.Columns.Count)
            {
                for (var s = table.Columns.Count + 1; s <= columns.Count; s++)
                {
                    columns.RemoveAt(s);   //移除数据表列数后的所有列
                }
            }

            //遍历所有的数据列，如果有数据列的数据类型不是 DataColumn，则将它移除
            var column = new DataColumn();
            for (var j = 0; j < columns.Count; j++)
            {
                try
                {
                    column = (DataColumn)columns[j];
                }
                catch (Exception)
                {
                    columns.RemoveAt(j);
                }
            }
            if ((table.TableName.Trim().Length == 0) || (table.TableName.ToLower() == "table"))
            {
                table.TableName = "Sheet1";
            }

            //数据表的列数
            var colCount = columns.Count;

            //创建参数
            var para = new OleDbParameter[colCount];

            //创建表结构的SQL语句
            var tableStructStr = @"Create Table " + table.TableName + "(";

            //连接字符串
            var connString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + excelFilePath + ";Extended Properties=Excel 8.0;";
            var objConn = new OleDbConnection(connString);

            //创建表结构
            var objCmd = new OleDbCommand();

            //数据类型集合
            var dataTypeList = new ArrayList();
            dataTypeList.Add("System.Decimal");
            dataTypeList.Add("System.Double");
            dataTypeList.Add("System.Int16");
            dataTypeList.Add("System.Int32");
            dataTypeList.Add("System.Int64");
            dataTypeList.Add("System.Single");

            var col = new DataColumn();

            //遍历数据表的所有列，用于创建表结构
            for (var k = 0; k < colCount; k++)
            {
                col = (DataColumn)columns[k];

                //列的数据类型是数字型
                if (dataTypeList.IndexOf(col.DataType.ToString().Trim()) >= 0)
                {
                    para[k] = new OleDbParameter("@" + col.Caption.Trim(), OleDbType.Double);
                    objCmd.Parameters.Add(para[k]);

                    //如果是最后一列
                    if (k + 1 == colCount)
                    {
                        tableStructStr += col.Caption.Trim() + " Double)";
                    }
                    else
                    {
                        tableStructStr += col.Caption.Trim() + " Double,";
                    }
                }
                else
                {
                    para[k] = new OleDbParameter("@" + col.Caption.Trim(), OleDbType.VarChar);
                    objCmd.Parameters.Add(para[k]);

                    //如果是最后一列
                    if (k + 1 == colCount)
                    {
                        tableStructStr += col.Caption.Trim() + " VarChar)";
                    }
                    else
                    {
                        tableStructStr += col.Caption.Trim() + " VarChar,";
                    }
                }
            }

            //创建Excel文件及文件结构
            try
            {
                objCmd.Connection = objConn;
                objCmd.CommandText = tableStructStr;

                if (objConn.State == ConnectionState.Closed)
                {
                    objConn.Open();
                }
                objCmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                throw exp;
            }

            //插入记录的SQL语句
            var insertSql1 = "Insert into " + table.TableName + " (";
            var insertSql2 = " Values (";
            var insertSql = "";

            //遍历所有列，用于插入记录，在此创建插入记录的SQL语句
            for (var colId = 0; colId < colCount; colId++)
            {
                if (colId + 1 == colCount)  //最后一列
                {
                    insertSql1 += columns[colId].ToString().Trim() + ")";
                    insertSql2 += "@" + columns[colId].ToString().Trim() + ")";
                }
                else
                {
                    insertSql1 += columns[colId].ToString().Trim() + ",";
                    insertSql2 += "@" + columns[colId].ToString().Trim() + ",";
                }
            }

            insertSql = insertSql1 + insertSql2;

            //遍历数据表的所有数据行
            var dataCol = new DataColumn();
            for (var rowId = 0; rowId < table.Rows.Count; rowId++)
            {
                for (var colId = 0; colId < colCount; colId++)
                {
                    //因为列不连续，所以在取得单元格时不能用行列编号，列需得用列的名称
                    dataCol = (DataColumn)columns[colId];
                    if (para[colId].DbType == DbType.Double && table.Rows[rowId][dataCol.Caption].ToString().Trim() == "")
                    {
                        para[colId].Value = 0;
                    }
                    else
                    {
                        para[colId].Value = table.Rows[rowId][dataCol.Caption].ToString().Trim();
                    }
                }
                try
                {
                    objCmd.CommandText = insertSql;
                    objCmd.ExecuteNonQuery();
                }
                catch (Exception exp)
                {
                    var str = exp.Message;
                }
            }
            try
            {
                if (objConn.State == ConnectionState.Open)
                {
                    objConn.Close();
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
            return true;
        }
        #endregion
        #endregion

        #region  获取Excel文件数据表列表
       
        /// <summary>
        /// 获取Excel文件数据表列表
        /// </summary>
        public static ArrayList GetExcelTables(string excelFileName)
        {
            var dt = new DataTable();
            var tablesList = new ArrayList();
            if (File.Exists(excelFileName))
            {
                using (var conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Extended Properties=Excel 8.0;Data Source=" + excelFileName))
                {
                    try
                    {
                        conn.Open();
                        dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                    }
                    catch (Exception exp)
                    {
                        throw exp;
                    }

                    //获取数据表个数
                    var tablecount = dt.Rows.Count;
                    for (var i = 0; i < tablecount; i++)
                    {
                        var tablename = dt.Rows[i][2].ToString().Trim().TrimEnd('$');
                        if (tablesList.IndexOf(tablename) < 0)
                        {
                            tablesList.Add(tablename);
                        }
                    }
                }
            }
            return tablesList;
        }
        #endregion

        #region      将Excel文件导出至DataTable(第一行作为表头)
       
        /// <summary>
        /// 将Excel文件导出至DataTable(第一行作为表头)
        /// </summary>
        /// <param name="excelFilePath">Excel文件路径</param>
        /// <param name="tableName">数据表名，如果数据表名错误，默认为第一个数据表名</param>
        public static DataTable InputFromExcel(string excelFilePath, string tableName)
        {
            if (!File.Exists(excelFilePath))
            {
                throw new Exception("Excel文件不存在！");
            }

            //如果数据表名不存在，则数据表名为Excel文件的第一个数据表
            var tableList = new ArrayList();
            tableList = GetExcelTables(excelFilePath);

            if (tableName.IndexOf(tableName) < 0)
            {
                tableName = tableList[0].ToString().Trim();
            }

            var table = new DataTable();
            var dbcon = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + excelFilePath + ";Extended Properties=Excel 8.0");
            var cmd = new OleDbCommand("select * from [" + tableName + "$]", dbcon);
            var adapter = new OleDbDataAdapter(cmd);

            try
            {
                if (dbcon.State == ConnectionState.Closed)
                {
                    dbcon.Open();
                }
                adapter.Fill(table);
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (dbcon.State == ConnectionState.Open)
                {
                    dbcon.Close();
                }
            }
            return table;
        }
        #endregion
   
        #region 获取Excel文件指定数据表的数据列表

       
        /// <summary>
        /// 获取Excel文件指定数据表的数据列表
        /// </summary>
        /// <param name="excelFileName">Excel文件名</param>
        /// <param name="tableName">数据表名</param>
        public static ArrayList GetExcelTableColumns(string excelFileName, string tableName)
        {
            var dt = new DataTable();
            var colsList = new ArrayList();
            if (File.Exists(excelFileName))
            {
                using (var conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Extended Properties=Excel 8.0;Data Source=" + excelFileName))
                {
                    conn.Open();
                    dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, tableName, null });

                    //获取列个数
                    var colcount = dt.Rows.Count;
                    for (var i = 0; i < colcount; i++)
                    {
                        var colname = dt.Rows[i]["Column_Name"].ToString().Trim();
                        colsList.Add(colname);
                    }
                }
            }
            return colsList;
        }
        #endregion
      
        #region  清理过时的Excel文件

        private void ClearFile(string filePath)
        {
            var files = System.IO.Directory.GetFiles(filePath);
            if (files.Length > 10)
            {
                for (var i = 0; i < 10; i++)
                {
                    try
                    {
                        System.IO.File.Delete(files[i]);
                    }
                    catch
                    {
                    }

                }
            }
        }
        #endregion
        
        #region 将数据转换为指定类型
        /// <summary>
        /// 将数据转换为指定类型
        /// </summary>
        /// <param name="data">转换的数据</param>
        /// <param name="targetType">转换的目标类型</param>
        public static object ConvertTo(object data, Type targetType)
        {
            if (data == null || Convert.IsDBNull(data))
            {
                return null;
            }

            var type2 = data.GetType();
            if (targetType == type2)
            {
                return data;
            }
            if (((targetType == typeof(Guid)) || (targetType == typeof(Guid?))) && (type2 == typeof(string)))
            {
                if (string.IsNullOrEmpty(data.ToString()))
                {
                    return null;
                }
                return new Guid(data.ToString());
            }

            if (targetType.IsEnum)
            {
                try
                {
                    return Enum.Parse(targetType, data.ToString(), true);
                }
                catch
                {
                    return Enum.ToObject(targetType, data);
                }
            }

            if (targetType.IsGenericType)
            {
                targetType = targetType.GetGenericArguments()[0];
            }

            return Convert.ChangeType(data, targetType);
        }

        /// <summary>
        /// 将数据转换为指定类型
        /// </summary>
        /// <typeparam name="T">转换的目标类型</typeparam>
        /// <param name="data">转换的数据</param>
        public static T ConvertTo<T>(object data)
        {
            if (data == null || Convert.IsDBNull(data))
                return default(T);

            var obj = ConvertTo(data, typeof(T));
            if (obj == null)
            {
                return default(T);
            }
            return (T)obj;
        }
        #endregion
    }
}
