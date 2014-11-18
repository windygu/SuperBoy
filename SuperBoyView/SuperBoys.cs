﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using System.Data;

namespace SuperBoyView
{
    public partial class SuperBoys : Form
    {
        public SuperBoys()
        {
            InitializeComponent();
        }
        // Dictionary<EnumArry.Master, object> dict = ProgramConfiguration.MasterDiction;
        private void SuperBoys_Load(object sender, EventArgs e)
        {
            //request master default count
            int count = (int)ProgramConfiguration.MasterDiction[EnumArry.Master.DataDefaultCount];
            //select database, request data
            //send request
            DataSet ds = ControlSuperBoy.select(count);
            this.Views.DataSource = ds.Tables[0];
            // this.Views.DataBind();
        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Query q = new Query(this);
            q.Show();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string[] str = new string[View.Rows.Count];
        }

        private void Views_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //Gets the current display line
            int rowindex = e.RowIndex;
            //for (int i = 0; i < Views.Rows[rowindex].Cells.Count; i++)
            //gets the current display column
            int CurrentIndex = this.Views.CurrentCell.ColumnIndex;
            //this is a current column Title
            string currentsColTitle = this.Views.Columns[CurrentIndex].HeaderText;
            //the first column Test
            string FirstCol = Views.Rows[Views.CurrentCell.RowIndex].Cells[0].Value.ToString();
            //the current change lines of text
            string item = Views.Rows[rowindex].Cells[Views.CurrentCell.ColumnIndex].Value.ToString();
            string Where = "update [dbo].[CW100_Comment] set " + currentsColTitle + "='" + item + "' where id = '" + FirstCol + "'";
            MessageBox.Show(ControlSuperBoy.Update(Where).ToString());

        }
    }
}
