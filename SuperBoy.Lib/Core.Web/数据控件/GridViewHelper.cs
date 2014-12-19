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
        #region ˽�з���
        /// <summary>
        /// ��ȡ���ݳ���
        /// </summary>
        /// <param name="oStr">ԭ�ַ���</param>
        /// <param name="len">��ȡ����</param>
        /// <returns>��ȡ���ַ���</returns>
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
        /// ��ȡ��Ԫ������
        /// </summary>
        /// <param name="cell">TableCell</param>
        /// <returns>����</returns>
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
        /// ���õ�Ԫ������
        /// </summary>
        /// <param name="cell">TableCell</param>
        /// <param name="maxLen">��󳤶�</param>
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

        #region ���з���
        /// <summary>
        /// ��GridView����������DataTable
        /// </summary>
        /// <param name="gv">GridView����</param>
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
        /// ��������ת����DataTable
        /// </summary>
        /// <param name="list">����</param>
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
        /// �����ͼ�����ת����DataTable
        /// </summary>
        /// <typeparam name="T">����������</typeparam>
        /// <param name="list">����</param>
        /// <param name="propertyName">��Ҫ���ص��е�����</param>
        /// <returns>���ݼ�(��)</returns>
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