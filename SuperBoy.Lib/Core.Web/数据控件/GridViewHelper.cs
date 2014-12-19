using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using System.Reflection;

namespace Core.Web
{
    public class GridViewHelper
    {
        #region 私有方法
        /// <summary>
        /// 截取内容长度
        /// </summary>
        /// <param name="oStr">原字符串</param>
        /// <param name="len">截取长度</param>
        /// <returns>截取后字符串</returns>
        private static string GetStrPartly(string oStr, int len)
        {
            if (len == 0)
            {
                return oStr;
            }
            else
            {
                if (oStr.Length > len)
                {
                    return oStr.Substring(0, len) + "..";
                }
                else
                {
                    return oStr;
                }
            }
        }

        /// <summary>
        /// 获取单元格内容
        /// </summary>
        /// <param name="cell">TableCell</param>
        /// <returns>内容</returns>
        private static string GetCellText(TableCell cell)
        {
            var text = cell.Text;
            if (!string.IsNullOrEmpty(text))
            {
                return text;
            }
            foreach (Control control in cell.Controls)
            {
                if (control != null && control is IButtonControl)
                {
                    var btn = control as IButtonControl;
                    text = btn.Text.Replace("\r\n", "").Trim();
                    break;
                }
                if (control != null && control is ITextControl)
                {
                    var lc = control as LiteralControl;
                    if (lc != null)
                    {
                        continue;
                    }
                    var l = control as ITextControl;
                    text = l.Text.Replace("\r\n", "").Trim();
                    break;
                }
            }
            return text;
        }

        /// <summary>
        /// 设置单元格内容
        /// </summary>
        /// <param name="cell">TableCell</param>
        /// <param name="maxLen">最大长度</param>
        private static void SetCellText(TableCell cell, int maxLen)
        {
            var text = cell.Text;
            if (!string.IsNullOrEmpty(text))
            {
                cell.Text = GetStrPartly(text, maxLen);
            }
            foreach (Control control in cell.Controls)
            {
                if (control != null && control is IButtonControl)
                {
                    var btn = control as IButtonControl;
                    text = btn.Text.Replace("\r\n", "").Trim();
                    btn.Text = GetStrPartly(text, maxLen);
                    break;
                }
                if (control != null && control is ITextControl)
                {
                    var lc = control as LiteralControl;
                    if (lc != null)
                    {
                        continue;
                    }
                    var l = control as ITextControl;
                    text = l.Text.Replace("\r\n", "").Trim();
                    if (l is DataBoundLiteralControl)
                    {
                        cell.Text = GetStrPartly(text, maxLen);
                        break;
                    }
                    else
                    {
                        l.Text = GetStrPartly(text, maxLen);
                        break;
                    }
                }
            }
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 从GridView的数据生成DataTable
        /// </summary>
        /// <param name="gv">GridView对象</param>
        public static DataTable GridView2DataTable(GridView gv)
        {
            var table = new DataTable();
            var rowIndex = 0;
            var cols = new List<string>();
            if (!gv.ShowHeader && gv.Columns.Count == 0)
            {
                return table;
            }
            var headerRow = gv.HeaderRow;
            var columnCount = headerRow.Cells.Count;
            for (var i = 0; i < columnCount; i++)
            {
                var text = GetCellText(headerRow.Cells[i]);
                cols.Add(text);
            }
            foreach (GridViewRow r in gv.Rows)
            {
                if (r.RowType == DataControlRowType.DataRow)
                {
                    var row = table.NewRow();
                    var j = 0;
                    for (var i = 0; i < columnCount; i++)
                    {
                        var text = GetCellText(r.Cells[i]);
                        if (!String.IsNullOrEmpty(text))
                        {
                            if (rowIndex == 0)
                            {
                                var columnName = cols[i];
                                if (String.IsNullOrEmpty(columnName))
                                {
                                    continue;
                                }
                                if (table.Columns.Contains(columnName))
                                {
                                    continue;
                                }
                                var dc = table.Columns.Add();
                                dc.ColumnName = columnName;
                                dc.DataType = typeof(string);
                            }
                            row[j] = text;
                            j++;
                        }
                    }
                    rowIndex++;
                    table.Rows.Add(row);
                }
            }
            return table;
        }

        /// <summary>
        /// 将集合类转换成DataTable
        /// </summary>
        /// <param name="list">集合</param>
        public static DataTable ToDataTable(IList list)
        {
            var result = new DataTable();
            if (list.Count > 0)
            {
                var propertys = list[0].GetType().GetProperties();
                foreach (var pi in propertys)
                {
                    result.Columns.Add(pi.Name, pi.PropertyType);
                }

                for (var i = 0; i < list.Count; i++)
                {
                    var tempList = new ArrayList();
                    foreach (var pi in propertys)
                    {
                        var obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    var array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

        /// <summary>
        /// 将泛型集合类转换成DataTable
        /// </summary>
        /// <typeparam name="T">集合项类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="propertyName">需要返回的列的列名</param>
        /// <returns>数据集(表)</returns>
        public static DataTable ToDataTable<T>(IList<T> list, params string[] propertyName)
        {
            var propertyNameList = new List<string>();
            if (propertyName != null) propertyNameList.AddRange(propertyName);

            var result = new DataTable();
            if (list.Count > 0)
            {
                var propertys = list[0].GetType().GetProperties();
                foreach (var pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name)) result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }

                for (var i = 0; i < list.Count; i++)
                {
                    var tempList = new ArrayList();
                    foreach (var pi in propertys)
                    {
                        if (propertyNameList.Count == 0)
                        {
                            var obj = pi.GetValue(list[i], null);
                            tempList.Add(obj);
                        }
                        else
                        {
                            if (propertyNameList.Contains(pi.Name))
                            {
                                var obj = pi.GetValue(list[i], null);
                                tempList.Add(obj);
                            }
                        }
                    }
                    var array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }
        #endregion
    }
}