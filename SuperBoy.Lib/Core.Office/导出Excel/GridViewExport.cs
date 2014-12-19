﻿using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;

namespace Core.Office
{
    /// <summary>
    /// Summary description for GridViewExport
    /// </summary>
    public class GridViewExport
    {
        public GridViewExport()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static void Export(string fileName, GridView gv)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader(
                "content-disposition", string.Format("attachment; filename={0}", fileName));
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            //HttpContext.Current.Response.Charset = "utf-8";


            using (var sw = new StringWriter())
            {
                using (var htw = new HtmlTextWriter(sw))
                {
                    //  Create a form to contain the grid
                    var table = new Table();
                    table.GridLines = GridLines.Both;  //单元格之间添加实线

                    //  add the header row to the table
                    if (gv.HeaderRow != null)
                    {
                        PrepareControlForExport(gv.HeaderRow);
                        table.Rows.Add(gv.HeaderRow);
                    }

                    //  add each of the data rows to the table
                    foreach (GridViewRow row in gv.Rows)
                    {
                        PrepareControlForExport(row);
                        table.Rows.Add(row);
                    }

                    //  add the footer row to the table
                    if (gv.FooterRow != null)
                    {
                        PrepareControlForExport(gv.FooterRow);
                        table.Rows.Add(gv.FooterRow);
                    }

                    //  render the table into the htmlwriter
                    table.RenderControl(htw);

                    //  render the htmlwriter into the response
                    HttpContext.Current.Response.Write(sw.ToString());
                    HttpContext.Current.Response.End();
                }
            }
        }
        public static void Export(GridView gv, string nd)
        {

            var filename = HttpUtility.UrlEncode(nd) + ".xls";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", filename));
            HttpContext.Current.Response.Charset = "UTF-8";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            HttpContext.Current.Response.ContentType = "application/ms-excel";

            using (var sw = new StringWriter())
            {
                using (var htw = new HtmlTextWriter(sw))
                {
                    ///Excel.Application exc = new Excel.ApplicationClass();

                    var table = new Table();
                    table.GridLines = gv.GridLines;
                    //.CellPadding = 1;
                    // table.CellSpacing = 1;
                    var style = @"<style> .text { mso-number-format:\@; } </style> ";
                    // 增加最少的excel头文件格式串, 否则导出的excel无表格线, 不美观
                    var xlsHeader = @"<html xmlns:x=urn:schemas-microsoft-com:office:excel>";
                    xlsHeader += "<head>";
                    xlsHeader += "<!--[if gte mso 9]>";
                    xlsHeader += "<xml>";
                    xlsHeader += "    <x:ExcelWorkbook>";
                    xlsHeader += "        <x:ExcelWorksheets>";
                    xlsHeader += "            <x:ExcelWorksheet>";
                    xlsHeader += "                <x:Name>ExportData</x:Name>";
                    xlsHeader += "                <x:WorksheetOptions>";
                    xlsHeader += "                    <x:Selected/>";
                    xlsHeader += "                </x:WorksheetOptions>";
                    xlsHeader += "            </x:ExcelWorksheet>";
                    xlsHeader += "        </x:ExcelWorksheets>";
                    xlsHeader += "    </x:ExcelWorkbook>";
                    xlsHeader += "</xml>";
                    xlsHeader += "<![endif]-->";
                    xlsHeader += "</head>";

                    if (gv.HeaderRow != null)
                    {
                        PrepareControlForExport(gv.HeaderRow);
                        table.Rows.Add(gv.HeaderRow);
                    }


                    foreach (GridViewRow row in gv.Rows)
                    {
                        PrepareControlForExport(row);
                        table.Rows.Add(row);
                    }


                    if (gv.FooterRow != null)
                    {
                        PrepareControlForExport(gv.FooterRow);
                        table.Rows.Add(gv.FooterRow);
                    }


                    table.RenderControl(htw);
                    HttpContext.Current.Response.Write(xlsHeader);
                    HttpContext.Current.Response.Write(style);

                    HttpContext.Current.Response.Write(sw.ToString());
                    HttpContext.Current.Response.End();
                }
            }
        }
        /// <summary>
        /// Replace any of the contained controls with literals
        /// </summary>
        /// <param name="control"></param>
        private static void PrepareControlForExport(Control control)
        {
            for (var i = 0; i < control.Controls.Count; i++)
            {
                var current = control.Controls[i];
                if (current is LinkButton)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as LinkButton).Text));
                }
                else if (current is ImageButton)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as ImageButton).AlternateText));
                }
                else if (current is HyperLink)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as HyperLink).Text));
                }
                else if (current is DropDownList)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as DropDownList).SelectedItem.Text));
                }
                else if (current is CheckBox)
                {
                    control.Controls.Remove(current);
                    control.Controls.AddAt(i, new LiteralControl((current as CheckBox).Checked ? "True" : "False"));
                }

                if (current.HasControls())
                {
                    PrepareControlForExport(current);
                }
            }
        }


        /// <summary>
        /// 导出Grid的数据(全部)到Excel
        /// 字段全部为BoundField类型时可用
        /// 要是字段为TemplateField模板型时就取不到数据
        /// </summary>
        /// <param name="grid">grid的ID</param>
        /// <param name="dt">数据源</param>
        /// <param name="excelFileName">要导出Excel的文件名</param>
        public static void OutputExcel(GridView grid, DataTable dt, string excelFileName)
        {
            var page = (Page)HttpContext.Current.Handler;
            page.Response.Clear();
            var fileName = System.Web.HttpUtility.UrlEncode(System.Text.Encoding.UTF8.GetBytes(excelFileName));
            page.Response.AddHeader("Content-Disposition", "attachment:filename=" + fileName + ".xls");
            page.Response.ContentType = "application/vnd.ms-excel";
            page.Response.Charset = "utf-8";

            var s = new StringBuilder();
            s.Append("<HTML><HEAD><TITLE>" + fileName + "</TITLE><META http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"></head><body>");

            var count = grid.Columns.Count;

            s.Append("<table border=1>");
            s.AppendLine("<tr>");
            for (var i = 0; i < count; i++)
            {

                if (grid.Columns[i].GetType() == typeof(BoundField))
                    s.Append("<td>" + grid.Columns[i].HeaderText + "</td>");

                //s.Append("<td>" + grid.Columns[i].HeaderText + "</td>");

            }
            s.Append("</tr>");

            foreach (DataRow dr in dt.Rows)
            {
                s.AppendLine("<tr>");
                for (var n = 0; n < count; n++)
                {
                    if (grid.Columns[n].Visible && grid.Columns[n].GetType() == typeof(BoundField))
                        s.Append("<td>" + dr[((BoundField)grid.Columns[n]).DataField].ToString() + "</td>");

                }
                s.AppendLine("</tr>");
            }

            s.Append("</table>");
            s.Append("</body></html>");

            page.Response.BinaryWrite(System.Text.Encoding.GetEncoding("utf-8").GetBytes(s.ToString()));
            page.Response.End();
        }





    }
}
