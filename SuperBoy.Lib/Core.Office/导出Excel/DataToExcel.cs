using System;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Collections;
using System.Diagnostics;
namespace Core.Office
{
    /// <summary>
    /// ����EXCEL�������ݱ������
    /// </summary>
    public class DataToExcel
    {
        public DataToExcel(DateTime afterTime, DateTime beforeTime)
        {
            _afterTime = afterTime;
            _beforeTime = beforeTime;
        }

        const string ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=Excel 8.0;";

        /// <summary>
        /// ����EXECL ���
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="excelPath"></param>
        /// <returns></returns>
        public string DataTableToExcel(System.Data.DataTable dt, string excelPath)
        {


            if (dt == null)
            {
                return "DataTable����Ϊ��";
            }

            //����sheet1����
            dt.TableName = "Sheet1";

            var rows = dt.Rows.Count;
            var cols = dt.Columns.Count;

            if (rows == 0)
            {
                return "û������";
            }

            var sb = new StringBuilder();
            var connString = string.Format(ConnectionString, excelPath);

            //���ɴ�����Ľű�
            sb.Append("CREATE TABLE ");
            sb.Append(dt.TableName + " ( ");

            for (var i = 0; i < cols; i++)
            {
                sb.Append(i < cols - 1
                    ? string.Format("{0} nvarchar,", dt.Columns[i].ColumnName)
                    : string.Format("{0} nvarchar)", dt.Columns[i].ColumnName));
            }

            using (var objConn = new OleDbConnection(connString))
            {
                var objCmd = new OleDbCommand {Connection = objConn, CommandText = sb.ToString()};


                try
                {
                    objConn.Open();
                    objCmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    return "��Excel�д�����ʧ�ܣ�������Ϣ��" + e.Message;
                }

                #region ���ɲ������ݽű�
                sb.Remove(0, sb.Length);
                sb.Append("INSERT INTO ");
                sb.Append(dt.TableName + " ( ");

                for (var i = 0; i < cols; i++)
                {
                    if (i < cols - 1)
                        sb.Append(dt.Columns[i].ColumnName + ",");
                    else
                        sb.Append(dt.Columns[i].ColumnName + ") values (");
                }

                for (var i = 0; i < cols; i++)
                {
                    if (i < cols - 1)
                        sb.Append("@" + dt.Columns[i].ColumnName + ",");
                    else
                        sb.Append("@" + dt.Columns[i].ColumnName + ")");
                }
                #endregion


                //�������붯����Command
                objCmd.CommandText = sb.ToString();
                var param = objCmd.Parameters;

                for (var i = 0; i < cols; i++)
                {
                    param.Add(new OleDbParameter("@" + dt.Columns[i].ColumnName, OleDbType.VarChar));
                }

                //����DataTable�����ݲ����½���Excel�ļ���
                foreach (DataRow row in dt.Rows)
                {
                    for (var i = 0; i < param.Count; i++)
                    {
                        param[i].Value = row[i];
                    }

                    objCmd.ExecuteNonQuery();
                }

                return "�����ѳɹ�����Excel";
            }//end using
        }


        /// <summary> 
        /// ��ȡExcel�ĵ� 
        /// </summary> 
        /// <param name="path">�ļ�����</param> 
        /// <returns>����һ�����ݼ�</returns> 
        public DataSet ExcelToDs(string path)
        {
            var strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";";
            var conn = new System.Data.OleDb.OleDbConnection(strConn);
            conn.Open();
            var strExcel = "";
            System.Data.OleDb.OleDbDataAdapter myCommand = null;
            DataSet ds = null;
            strExcel = "select * from [Sheet1$]";
            myCommand = new System.Data.OleDb.OleDbDataAdapter(strExcel, strConn);
            ds = new DataSet();
            myCommand.Fill(ds, "table1");
            conn.Close();
            return ds;
        }

        #region ����EXCEL��һ����(��ҪExcel.dll֧��)

        private int _titleColorindex = 15;
        /// <summary>
        /// ���ⱳ��ɫ
        /// </summary>
        public int TitleColorIndex
        {
            set { _titleColorindex = value; }
            get { return _titleColorindex; }
        }

        private readonly DateTime _beforeTime;			//Excel����֮ǰʱ��
        private readonly DateTime _afterTime;				//Excel����֮��ʱ��

        #region ����һ��Excelʾ��
        /// <summary>
        /// ����һ��Excelʾ��
        /// </summary>
        public void CreateExcel()
        {
            //Excel.Application excel = new Excel.Application();
            //excel.Application.Workbooks.Add(true);
            //excel.Cells[1, 1] = "��1�е�1��";
            //excel.Cells[1, 2] = "��1�е�2��";
            //excel.Cells[2, 1] = "��2�е�1��";
            //excel.Cells[2, 2] = "��2�е�2��";
            //excel.Cells[3, 1] = "��3�е�1��";
            //excel.Cells[3, 2] = "��3�е�2��";

            ////����
            //excel.ActiveWorkbook.SaveAs("./tt.xls", XlFileFormat.xlExcel9795, null, null, false, false, Excel.XlSaveAsAccessMode.xlNoChange, null, null, null, null, null);
            ////����ʾ
            //excel.Visible = true;
            ////			excel.Quit();
            ////			excel=null;            
            ////			GC.Collect();//��������
        }
        #endregion

        #region ��DataTable�����ݵ�����ʾΪ����
        /// <summary>
        /// ��DataTable�����ݵ�����ʾΪ����
        /// </summary>
        /// <param name="dt">Ҫ����������</param>
        /// <param name="strTitle">��������ı���</param>
        /// <param name="FilePath">�����ļ���·��</param>
        /// <returns></returns>
        //public string OutputExcel(System.Data.DataTable dt, string strTitle, string FilePath)
        //{
        //    beforeTime = DateTime.Now;

        //    Excel.Application excel;
        //    Excel._Workbook xBk;
        //    Excel._Worksheet xSt;

        //    int rowIndex = 4;
        //    int colIndex = 1;

        //    excel = new Excel.ApplicationClass();
        //    xBk = excel.Workbooks.Add(true);
        //    xSt = (Excel._Worksheet)xBk.ActiveSheet;

        //    //ȡ���б���			
        //    foreach (DataColumn col in dt.Columns)
        //    {
        //        colIndex++;
        //        excel.Cells[4, colIndex] = col.ColumnName;

        //        //���ñ����ʽΪ���ж���
        //        xSt.get_Range(excel.Cells[4, colIndex], excel.Cells[4, colIndex]).Font.Bold = true;
        //        xSt.get_Range(excel.Cells[4, colIndex], excel.Cells[4, colIndex]).HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;
        //        xSt.get_Range(excel.Cells[4, colIndex], excel.Cells[4, colIndex]).Select();
        //        xSt.get_Range(excel.Cells[4, colIndex], excel.Cells[4, colIndex]).Interior.ColorIndex = titleColorindex;//19;//����Ϊǳ��ɫ��������56��
        //    }


        //    //ȡ�ñ���е�����			
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        rowIndex++;
        //        colIndex = 1;
        //        foreach (DataColumn col in dt.Columns)
        //        {
        //            colIndex++;
        //            if (col.DataType == System.Type.GetType("System.DateTime"))
        //            {
        //                excel.Cells[rowIndex, colIndex] = (Convert.ToDateTime(row[col.ColumnName].ToString())).ToString("yyyy-MM-dd");
        //                xSt.get_Range(excel.Cells[rowIndex, colIndex], excel.Cells[rowIndex, colIndex]).HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//���������͵��ֶθ�ʽΪ���ж���
        //            }
        //            else
        //                if (col.DataType == System.Type.GetType("System.String"))
        //                {
        //                    excel.Cells[rowIndex, colIndex] = "'" + row[col.ColumnName].ToString();
        //                    xSt.get_Range(excel.Cells[rowIndex, colIndex], excel.Cells[rowIndex, colIndex]).HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter;//�����ַ��͵��ֶθ�ʽΪ���ж���
        //                }
        //                else
        //                {
        //                    excel.Cells[rowIndex, colIndex] = row[col.ColumnName].ToString();
        //                }
        //        }
        //    }

        //    //����һ���ϼ���			
        //    int rowSum = rowIndex + 1;
        //    int colSum = 2;
        //    excel.Cells[rowSum, 2] = "�ϼ�";
        //    xSt.get_Range(excel.Cells[rowSum, 2], excel.Cells[rowSum, 2]).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
        //    //����ѡ�еĲ��ֵ���ɫ			
        //    xSt.get_Range(excel.Cells[rowSum, colSum], excel.Cells[rowSum, colIndex]).Select();
        //    //xSt.get_Range(excel.Cells[rowSum,colSum],excel.Cells[rowSum,colIndex]).Interior.ColorIndex =Assistant.GetConfigInt("ColorIndex");// 1;//����Ϊǳ��ɫ��������56��

        //    //ȡ����������ı���			
        //    excel.Cells[2, 2] = strTitle;

        //    //������������ı����ʽ			
        //    xSt.get_Range(excel.Cells[2, 2], excel.Cells[2, 2]).Font.Bold = true;
        //    xSt.get_Range(excel.Cells[2, 2], excel.Cells[2, 2]).Font.Size = 22;

        //    //���ñ�����Ϊ����Ӧ���			
        //    xSt.get_Range(excel.Cells[4, 2], excel.Cells[rowSum, colIndex]).Select();
        //    xSt.get_Range(excel.Cells[4, 2], excel.Cells[rowSum, colIndex]).Columns.AutoFit();

        //    //������������ı���Ϊ���о���			
        //    xSt.get_Range(excel.Cells[2, 2], excel.Cells[2, colIndex]).Select();
        //    xSt.get_Range(excel.Cells[2, 2], excel.Cells[2, colIndex]).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenterAcrossSelection;

        //    //���Ʊ߿�			
        //    xSt.get_Range(excel.Cells[4, 2], excel.Cells[rowSum, colIndex]).Borders.LineStyle = 1;
        //    xSt.get_Range(excel.Cells[4, 2], excel.Cells[rowSum, 2]).Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlThick;//��������߼Ӵ�
        //    xSt.get_Range(excel.Cells[4, 2], excel.Cells[4, colIndex]).Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlThick;//�����ϱ��߼Ӵ�
        //    xSt.get_Range(excel.Cells[4, colIndex], excel.Cells[rowSum, colIndex]).Borders[Excel.XlBordersIndex.xlEdgeRight].Weight = Excel.XlBorderWeight.xlThick;//�����ұ��߼Ӵ�
        //    xSt.get_Range(excel.Cells[rowSum, 2], excel.Cells[rowSum, colIndex]).Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlThick;//�����±��߼Ӵ�



        //    afterTime = DateTime.Now;

        //    //��ʾЧ��			
        //    //excel.Visible=true;			
        //    //excel.Sheets[0] = "sss";

        //    ClearFile(FilePath);
        //    string filename = DateTime.Now.ToString("yyyyMMddHHmmssff") + ".xls";
        //    excel.ActiveWorkbook.SaveAs(FilePath + filename, Excel.XlFileFormat.xlExcel9795, null, null, false, false, Excel.XlSaveAsAccessMode.xlNoChange, null, null, null, null, null);

        //    //wkbNew.SaveAs strBookName;
        //    //excel.Save(strExcelFileName);

        //    #region  ����Excel����

        //    //��Ҫ��Excel��DCOM�����������:dcomcnfg


        //    //excel.Quit();
        //    //excel=null;            

        //    xBk.Close(null, null, null);
        //    excel.Workbooks.Close();
        //    excel.Quit();


        //    //ע�⣺�����õ�������Excel����Ҫִ����������������������Excel����
        //    //			if(rng != null)
        //    //			{
        //    //				System.Runtime.InteropServices.Marshal.ReleaseComObject(rng);
        //    //				rng = null;
        //    //			}
        //    //			if(tb != null)
        //    //			{
        //    //				System.Runtime.InteropServices.Marshal.ReleaseComObject(tb);
        //    //				tb = null;
        //    //			}
        //    if (xSt != null)
        //    {
        //        System.Runtime.InteropServices.Marshal.ReleaseComObject(xSt);
        //        xSt = null;
        //    }
        //    if (xBk != null)
        //    {
        //        System.Runtime.InteropServices.Marshal.ReleaseComObject(xBk);
        //        xBk = null;
        //    }
        //    if (excel != null)
        //    {
        //        System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);
        //        excel = null;
        //    }
        //    GC.Collect();//��������
        //    #endregion

        //    return filename;

        //}
        #endregion

        #region Kill Excel����

        /// <summary>
        /// ����Excel����
        /// </summary>
        public void KillExcelProcess()
        {
            Process[] myProcesses;
            DateTime startTime;
            myProcesses = Process.GetProcessesByName("Excel");

            //�ò���Excel����ID����ʱֻ���жϽ�������ʱ��
            foreach (var myProcess in myProcesses)
            {
                startTime = myProcess.StartTime;
                if (startTime > _beforeTime && startTime < _afterTime)
                {
                    myProcess.Kill();
                }
            }
        }
        #endregion

        #endregion

        #region ��DataTable�����ݵ�����ʾΪ����(��ʹ��Excel����ʹ��COM.Excel)

        #region ʹ��ʾ��
        /*ʹ��ʾ����
         * DataSet ds=(DataSet)Session["AdBrowseHitDayList"];
            string ExcelFolder=Assistant.GetConfigString("ExcelFolder");
            string FilePath=Server.MapPath(".")+"\\"+ExcelFolder+"\\";
			
            //�����е����Ķ�Ӧ��
            Hashtable nameList = new Hashtable();
            nameList.Add("ADID", "������");
            nameList.Add("ADName", "�������");
            nameList.Add("year", "��");
            nameList.Add("month", "��");
            nameList.Add("browsum", "��ʾ��");
            nameList.Add("hitsum", "�����");
            nameList.Add("BrowsinglIP", "����IP��ʾ");
            nameList.Add("HitsinglIP", "����IP���");
            //����excel����
            DataToExcel dte=new DataToExcel();
            string filename="";
            try
            {			
                if(ds.Tables[0].Rows.Count>0)
                {
                    filename=dte.DataExcel(ds.Tables[0],"����",FilePath,nameList);
                }
            }
            catch
            {
                //dte.KillExcelProcess();
            }
			
            if(filename!="")
            {
                Response.Redirect(ExcelFolder+"\\"+filename,true);
            }
         * 
         * */

        #endregion

         ///<summary>
         ///��DataTable�����ݵ�����ʾΪ����(��ʹ��Excel����)
         ///</summary>
         ///<param name="dt">����DataTable</param>
         ///<param name="strTitle">����</param>
         ///<param name="filePath">�����ļ���·��</param>
         ///<param name="nameList"></param>
         ///<returns></returns>
        public string DataExcel(System.Data.DataTable dt, string strTitle, string filePath, Hashtable nameList)
        {
            var excel = new COM.Excel.cExcelFile();
            ClearFile(filePath);
            var filename = DateTime.Now.ToString("yyyyMMddHHmmssff") + ".xls";
            excel.CreateFile(filePath + filename);
            excel.PrintGridLines = false;

            var mt1 = COM.Excel.cExcelFile.MarginTypes.xlsTopMargin;
            var mt2 = COM.Excel.cExcelFile.MarginTypes.xlsLeftMargin;
            var mt3 = COM.Excel.cExcelFile.MarginTypes.xlsRightMargin;
            var mt4 = COM.Excel.cExcelFile.MarginTypes.xlsBottomMargin;

            var height = 1.5;
            excel.SetMargin(ref mt1, ref height);
            excel.SetMargin(ref mt2, ref height);
            excel.SetMargin(ref mt3, ref height);
            excel.SetMargin(ref mt4, ref height);

            var ff = COM.Excel.cExcelFile.FontFormatting.xlsNoFormat;
            var font = "����";
            short fontsize = 9;
            excel.SetFont(ref font, ref fontsize, ref ff);

            byte b1 = 1,
                b2 = 12;
            short s3 = 12;
            excel.SetColumnWidth(ref b1, ref b2, ref s3);

            var header = "ҳü";
            var footer = "ҳ��";
            excel.SetHeader(ref header);
            excel.SetFooter(ref footer);


            var vt = COM.Excel.cExcelFile.ValueTypes.xlsText;
            var cf = COM.Excel.cExcelFile.CellFont.xlsFont0;
            var ca = COM.Excel.cExcelFile.CellAlignment.xlsCentreAlign;
            var chl = COM.Excel.cExcelFile.CellHiddenLocked.xlsNormal;

            // �������
            var cellformat = 1;
            //			int rowindex = 1,colindex = 3;					
            //			object title = (object)strTitle;
            //			excel.WriteValue(ref vt, ref cf, ref ca, ref chl,ref rowindex,ref colindex,ref title,ref cellformat);

            var rowIndex = 1;//��ʼ��
            var colIndex = 0;



            //ȡ���б���				
            foreach (DataColumn colhead in dt.Columns)
            {
                colIndex++;
                var name = colhead.ColumnName.Trim();
                var namestr = (object)name;
                var Enum = nameList.GetEnumerator();
                while (Enum.MoveNext())
                {
                    if (Enum.Key.ToString().Trim() == name)
                    {
                        namestr = Enum.Value;
                    }
                }
                excel.WriteValue(ref vt, ref cf, ref ca, ref chl, ref rowIndex, ref colIndex, ref namestr, ref cellformat);
            }

            //ȡ�ñ���е�����			
            foreach (DataRow row in dt.Rows)
            {
                rowIndex++;
                colIndex = 0;
                foreach (DataColumn col in dt.Columns)
                {
                    colIndex++;
                    if (col.DataType == System.Type.GetType("System.DateTime"))
                    {
                        var str = (object)(Convert.ToDateTime(row[col.ColumnName].ToString())).ToString("yyyy-MM-dd"); ;
                        excel.WriteValue(ref vt, ref cf, ref ca, ref chl, ref rowIndex, ref colIndex, ref str, ref cellformat);
                    }
                    else
                    {
                        var str = (object)row[col.ColumnName].ToString();
                        excel.WriteValue(ref vt, ref cf, ref ca, ref chl, ref rowIndex, ref colIndex, ref str, ref cellformat);
                    }
                }
            }
            var ret = excel.CloseFile();

            //			if(ret!=0)
            //			{
            //				//MessageBox.Show(this,"Error!");
            //			}
            //			else
            //			{
            //				//MessageBox.Show(this,"����ļ�c:\\test.xls!");
            //			}
            return filename;

        }
        
        #endregion

        #region  �����ʱ��Excel�ļ�

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

    }
}
