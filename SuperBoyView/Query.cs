using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SuperBoyView
{
    public partial class Query : Form
    {

        public Query(SuperBoys sup)
        {
            InitializeComponent();
            sups = sup;
        }

        private SuperBoys sups;
        private void button1_Click(object sender, EventArgs e)
        {
            //SuperBoys super = new SuperBoys ();
            // super.Views.DataSource = null;
            //  super.Text = "123";
            DataSet ds = ControlSuperBoy.select("where " + this.comboBox1.Text + " like '%" + this.Where.Text + "%'", Convert.ToInt32(this.Count.Text));
            sups.Views.DataSource = ds.Tables[0];
        }
    }
}
