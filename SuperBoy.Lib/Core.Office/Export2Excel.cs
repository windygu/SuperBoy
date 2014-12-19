using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;
using Range = Excel.Range;
using Shape = Microsoft.Office.Interop.Excel.Shape;
using Worksheet = Excel.Worksheet;
using XlBorderWeight = Microsoft.Office.Interop.Excel.XlBorderWeight;
using XlColorIndex = Microsoft.Office.Interop.Excel.XlColorIndex;
using XlLineStyle = Excel.XlLineStyle;
using XlWBATemplate = Excel.XlWBATemplate;

namespace Core.Office
{
    /// <summary>
    ///     利用VBA对象，导出DataView到一个Excel文档中的Excel辅助类
    /// </summary>
    public class Export2Excel
    {
        #region Constructor

        //Constructs a new export2Excel object. The user must
        //call the createExcelDocument method once a valid export2Excel
        //object has been instantiated
        public Export2Excel(Application exl)
        {
            _exl = exl;
        }

        #endregion

        #region EXCEL : UseTemplate

        //Exports a DataView to Excel. The following steps are carried out
        //in order to export the DataView to Excel
        //Create Excel Objects And Open Template File
        //Select All Used Cells
        //Create Headers/Footers
        //Set Status Finished
        //Save workbook & Tidy up all objects
        //@param path : The path to save/open the EXCEL file to/from
        public void UseTemplate(string path, string templatePath, string[,] myTemplateValues)
        {
            try
            {
                _myTemplateValues = myTemplateValues;
                //create new EXCEL application
                ////  EXL = new Microsoft.Office.Interop.Excel.ApplicationClass();
                //Yes file exists, so open the file
                //workbook = EXL.Workbooks.Open(templatePath,
                //    0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "",
                //    true, false, 0, true, false, false);
                ////get the workbook sheets collection
                //sheets = workbook.Sheets;
                ////get the new sheet
                //worksheet = (Worksheet)sheets.get_Item(1);
                ////Change its name to that requested
                //worksheet.Name = "ATemplate";
                ////Fills the Excel Template File Selected With A 2D Test Array
                //FillTemplate_WithTestValues();
                ////Select all used cells within current worksheet
                //SelectAllUsedCells();

                #region Finish and Release

                try
                {
                    Nar(_sheets);
                    Nar(_worksheet);
                    _workbook.Close(true, path, Type.Missing);
                    Nar(_workbook);

                    _exl.UserControl = false;

                    _exl.Quit();
                    Nar(_exl);

                    //kill the EXCEL process as a safety measure
                    KillExcel();
                    // Show that processing is finished
                    var pe = new ProgressEventArgs(100);
                    OnProgressChange(pe);

                    //MessageBox.Show("Finished adding test values to Template", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (COMException)
                {
                    Console.WriteLine("User closed Excel manually, so we don't have to do that");
                }

                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message);
            }
        }

        #endregion

        #region STEP 1 : Create Column & Row Workbook Cell Rendering Styles

        //Creates 2 Custom styles for the workbook These styles are
        //  styleColumnHeadings
        //  styleRows
        //These 2 styles are used when filling the individual Excel cells with the
        //DataView values. If the current cell relates to a DataView column heading
        //then the style styleColumnHeadings will be used to render the current cell.
        //If the current cell relates to a DataView row then the style styleRows will
        //be used to render the current cell.
        private void SetUpStyles()
        {
            // Style styleColumnHeadings
            try
            {
                _styleColumnHeadings = _workbook.Styles["styleColumnHeadings"];
            }
                // Style doesn't exist yet.
            catch
            {
                _styleColumnHeadings = _workbook.Styles.Add("styleColumnHeadings", Type.Missing);
                _styleColumnHeadings.Font.Name = "Arial";
                _styleColumnHeadings.Font.Size = 14;
                _styleColumnHeadings.Font.Color = (255 << 16) | (255 << 8) | 255;
                _styleColumnHeadings.Interior.Color = (0 << 16) | (0 << 8) | 0;
                _styleColumnHeadings.Interior.Pattern = XlPattern.xlPatternSolid;
            }

            // Style styleRows
            try
            {
                _styleRows = _workbook.Styles["styleRows"];
            }
                // Style doesn't exist yet.
            catch
            {
                _styleRows = _workbook.Styles.Add("styleRows", Type.Missing);
                _styleRows.Font.Name = "Arial";
                _styleRows.Font.Size = 10;
                _styleRows.Font.Color = (0 << 16) | (0 << 8) | 0;
                _styleRows.Interior.Color = (192 << 16) | (192 << 8) | 192;
                _styleRows.Interior.Pattern = XlPattern.xlPatternSolid;
            }
        }

        #endregion

        #region STEP 2 : Fill Worksheet With DataView

        //Fills an Excel worksheet with the values contained in the DataView
        //parameter
        private void FillWorksheet_WithDataView()
        {
            _position = 0;
            //Add DataView Columns To Worksheet
            var row = 1;
            var col = 1;

            var rowIndex = 1; //记录写入Excel中Row的序号

            var allRowsCount = _dvList.Sum(dv => dv.Count + 1);

            foreach (var dv in _dvList)
            {
                // Loop thought the columns
                for (var i = 0; i < dv.Table.Columns.Count; i++)
                {
                    FillExcelCell(_worksheet, row, col++, dv.Table.Columns[i].ToString(), _styleColumnHeadings.Name);
                }

                //Add DataView Rows To Worksheet
                row++;
                col = 1;
                rowIndex++;

                for (var i = 0; i < dv.Table.Rows.Count; i++)
                {
                    for (var j = 0; j < dv.Table.Columns.Count; j++)
                    {
                        FillExcelCell(_worksheet, row, col++, dv[i][j].ToString(), _styleRows.Name);
                    }

                    _position = (100*rowIndex)/(allRowsCount);
                    var pe = new ProgressEventArgs(_position);
                    OnProgressChange(pe);

                    col = 1;
                    row++;
                    rowIndex++;
                }

                row = row + 2; //第二个开始空两行
            }
        }

        #endregion

        #region STEP 3 : Fill Individual Cell and Render Using Predefined Style

        //Formats the current cell based on the Style setting parameter name
        //provided here
        //@param worksheet : The worksheet
        //@param row : Current row
        //@param col : Current Column
        //@param Value : The value for the cell
        //@param StyleName : The style name to use
        protected virtual void FillExcelCell(Worksheet worksheet, int row, int col, Object value, string styleName)
        {
            var rng = (Range) worksheet.Cells[row, col];
            rng.Select();
            rng.Value2 = value.ToString();
            rng.Style = styleName;
            rng.Columns.EntireColumn.AutoFit();
            rng.Borders.Weight = XlBorderWeight.xlThin;
            rng.Borders.LineStyle = XlLineStyle.xlContinuous;
            rng.Borders.ColorIndex = XlColorIndex.xlColorIndexAutomatic;
        }

        #endregion

        #region STEP 4 : Add Auto Shapes To Excel Worksheet

        //Add some WordArt objecs to the Excel worksheet
        private void AddAutoShapesToExcel()
        {
            //Method fields
            const float txtSize = 80;
            const float left = 100.0F;
            const float top = 100.0F;
            //Have 2 objects
            var numShapes = new int[2];
            var myShapes = new Shape[numShapes.Length];

            try
            {
                //loop through the object count
                for (var i = 0; i < numShapes.Length; i++)
                {
                    //Add the object to Excel
                    myShapes[i] =
                        (Shape)
                            _worksheet.Shapes.AddTextEffect(MsoPresetTextEffect.msoTextEffect1, "DRAFT", "Arial Black",
                                txtSize, MsoTriState.msoFalse, MsoTriState.msoFalse, (left*(i*3)), top);

                    //Manipulate the object settings
                    myShapes[i].Rotation = 45F;
                    myShapes[i].Fill.Visible = MsoTriState.msoFalse;
                    myShapes[i].Fill.Transparency = 0F;
                    myShapes[i].Line.Weight = 1.75F;
                    myShapes[i].Line.DashStyle = MsoLineDashStyle.msoLineSolid;
                    myShapes[i].Line.Transparency = 0F;
                    myShapes[i].Line.Visible = MsoTriState.msoTrue;
                    myShapes[i].Line.ForeColor.RGB = (0 << 16) | (0 << 8) | 0;
                    myShapes[i].Line.BackColor.RGB = (255 << 16) | (255 << 8) | 255;
                }
            }
            catch (Exception ex)
            {
                // ignored
            }
        }

        #endregion

        #region STEP 5 : Select All Used Cells

        //Selects all used cells for the Excel worksheet
        private void SelectAllUsedCells()
        {
            var myAllRange = _worksheet.Cells;
            myAllRange.Select();
            myAllRange.CurrentRegion.Select();
        }

        #endregion

        #region STEP 6 : Fill Template With Test Values

        //Fills the Excel Template File Selected With A 2D Test Array parameter
        private void FillTemplate_WithTestValues()
        {
            //Initilaise the correct Start Row/Column to match the Template
            var startRow = 3;
            var startCol = 2;

            _position = 0;

            // Display the array elements within the Output window, make sure its correct before
            for (var i = 0; i <= _myTemplateValues.GetUpperBound(0); i++)
            {
                //loop through array and put into EXCEL template
                for (var j = 0; j <= _myTemplateValues.GetUpperBound(1); j++)
                {
                    //update position in progress bar
                    _position = (100/_myTemplateValues.Length)*i;
                    var pe = new ProgressEventArgs(_position);
                    OnProgressChange(pe);

                    //put into EXCEL template
                    var rng = (Range) _worksheet.Cells[startRow, startCol++];
                    rng.Select();
                    rng.Value2 = _myTemplateValues[i, j];
                    rng.Rows.EntireRow.AutoFit();
                }
                //New row, so column needs to be reset
                startCol = 2;
                startRow++;
            }
        }

        #endregion

        #region Events

        /// Raises the OnProgressChange event for the parent form.
        public virtual void OnProgressChange(ProgressEventArgs e)
        {
            if (OnProgressHandler != null)
            {
                // Invokes the delegates. 
                OnProgressHandler(this, e);
            }
        }

        #endregion

        #region InstanceFields

        //Instance Fields
        public delegate void ProgressHandler(object sender, ProgressEventArgs e);

        public event ProgressHandler OnProgressHandler;

        private List<DataView> _dvList;

        private Style _styleRows;
        private Style _styleColumnHeadings;
        private readonly Application _exl;
        private Workbook _workbook;
        private Sheets _sheets;
        private Worksheet _worksheet;
        private string[,] _myTemplateValues;
        private int _position;

        #endregion

        #region EXCEL : ExportToExcel

        /// <summary>
        ///     Exports a DataView to Excel
        /// </summary>
        /// <param name="dv">DataView to use</param>
        /// <param name="path">The path to save/open the EXCEL file to/from</param>
        /// <param name="sheetName">The target sheet within the EXCEL file</param>
        public void ExportToExcel(DataView dv, string path, string sheetName)
        {
            var dvList = new List<DataView> {dv};
            ExportToExcel(dvList, path, sheetName);
        }


        //Exports a DataView to Excel. The following steps are carried out
        //in order to export the DataView to Excel
        //Create Excel Objects
        //Create Column & Row Workbook Cell Rendering Styles
        //Fill Worksheet With DataView
        //Add Auto Shapes To Excel Worksheet
        //Select All Used Cells
        //Create Headers/Footers
        //Set Status Finished
        //Save workbook & Tidy up all objects
        //@param dv : DataView to use
        //@param path : The path to save/open the EXCEL file to/from
        //@param sheetName : The target sheet within the EXCEL file
        public void ExportToExcel(List<DataView> dvList, string path, string sheetName)
        {
            try
            {
                //Assign Instance Fields
                _dvList = dvList;

                #region NEW EXCEL DOCUMENT : Create Excel Objects

                //create new EXCEL application
                // EXL = new Microsoft.Office.Interop.Excel.ApplicationClass();
                //index to hold location of the requested sheetName in the workbook sheets
                //collection
                int indexOfsheetName;

                #region FILE EXISTS

                //Does the file exist for the given path
                if (File.Exists(path))
                {
                    //Yes file exists, so open the file
                    _workbook = _exl.Workbooks.Open(path,
                        0, false, 5, "", "", false, XlPlatform.xlWindows, "",
                        true, false, 0, true, false, false);

                    //get the workbook sheets collection
                    _sheets = _workbook.Sheets;

                    //set the location of the requested sheetName to -1, need to find where
                    //it is. It may not actually exist
                    indexOfsheetName = -1;

                    //loop through the sheets collection
                    for (var i = 1; i <= _sheets.Count; i++)
                    {
                        //get the current worksheet at index (i)
                        _worksheet = (Worksheet) _sheets.Item[i];

                        //is the current worksheet the sheetName that was requested
                        if (!_worksheet.Name.Equals(sheetName)) continue;
                        //yes it is, so store its index
                        indexOfsheetName = i;

                        //Select all cells, and clear the contents
                        var myAllRange = _worksheet.Cells;
                        myAllRange.Select();
                        myAllRange.CurrentRegion.Select();
                        myAllRange.ClearContents();
                    }

                    //At this point it is known that the sheetName that was requested
                    //does not exist within the found file, so create a new sheet within the
                    //sheets collection
                    if (indexOfsheetName == -1)
                    {
                        //Create a new sheet for the requested sheet
                        var sh = (Worksheet) _workbook.Sheets.Add(
                            Type.Missing, (Worksheet) _sheets.Item[_sheets.Count],
                            Type.Missing, Type.Missing);
                        //Change its name to that requested
                        sh.Name = sheetName;
                    }
                }
                    #endregion

                    #region FILE DOESNT EXIST

                //No the file DOES NOT exist, so create a new file
                else
                {
                    //Add a new workbook to the file
                    _workbook = _exl.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                    //get the workbook sheets collection
                    _sheets = _workbook.Sheets;
                    //get the new sheet
                    _worksheet = (Worksheet) _sheets.Item[1];
                    //Change its name to that requested
                    _worksheet.Name = sheetName;
                }

                #endregion

                #region get correct worksheet index for requested sheetName

                //get the workbook sheets collection
                _sheets = _workbook.Sheets;

                //set the location of the requested sheetName to -1, need to find where
                //it is. It will definately exist now as it has just been added
                indexOfsheetName = -1;

                //loop through the sheets collection
                for (var i = 1; i <= _sheets.Count; i++)
                {
                    //get the current worksheet at index (i)
                    _worksheet = (Worksheet) _sheets.Item[i];


                    //is the current worksheet the sheetName that was requested
                    if (_worksheet.Name.Equals(sheetName))
                    {
                        //yes it is, so store its index
                        indexOfsheetName = i;
                    }
                }

                //set the worksheet that the DataView should write to, to the known index of the
                //requested sheet
                _worksheet = (Worksheet) _sheets.Item[indexOfsheetName];

                #endregion

                #endregion

                // Set styles 1st
                SetUpStyles();
                //Fill EXCEL worksheet with DataView values
                FillWorksheet_WithDataView();

                ////Add the autoshapes to EXCEL
                //AddAutoShapesToExcel();

                //Select all used cells within current worksheet
                SelectAllUsedCells();

                #region Finish and Release

                try
                {
                    Nar(_sheets);
                    Nar(_worksheet);
                    _workbook.Close(true, path, Type.Missing);
                    Nar(_workbook);

                    _exl.UserControl = false;

                    _exl.Quit();
                    Nar(_exl);

                    //kill the EXCEL process as a safety measure
                    KillExcel();
                    // Show that processing is finished
                    var pe = new ProgressEventArgs(100);
                    OnProgressChange(pe);

                    //MessageBox.Show("Finished adding dataview to Excel", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (COMException cex)
                {
                    MessageBox.Show("User closed Excel manually, so we don't have to do that");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error " + ex.Message);
                }

                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error " + ex.Message);
            }
        }

        #endregion

        #region Kill EXCEL

        //As a safety check go through all processes and make
        //doubly sure excel is shutdown. Working with COM
        //have sometimes noticed that the EXL.Quit() call
        //does always do the job
        private static void KillExcel()
        {
            try
            {
                var ps = Process.GetProcesses();
                foreach (var p in ps.Where(p => p.ProcessName.ToLower().Equals("excel")))
                {
                    p.Kill();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message);
            }
        }

        /// <summary>
        ///     释放对象内存，推出进程
        /// </summary>
        /// <param name="obj"></param>
        private static void Nar(object obj)
        {
            try
            {
                Marshal.ReleaseComObject(obj);
            }
            catch
            {
                // ignored
            }
            finally
            {
                obj = null;
            }
        }

        #endregion
    }

    /// <summary>
    ///     自定义的事件处理参数类
    /// </summary>
    public class ProgressEventArgs : EventArgs
    {
        #region Instance Fields

        //Instance fields
        private readonly int _prgValue;

        #endregion

        #region Public Constructor

        /// Constructs a new ProgressEventArgs object using the parameters provided
        /// @param prgValue : new progress value
        public ProgressEventArgs(int prgValue)
        {
            _prgValue = prgValue;
        }

        #endregion

        #region Public Methods/Properties

        /// Returns the progress value
        public int ProgressValue
        {
            get { return _prgValue; }
        }

        #endregion
    }
}