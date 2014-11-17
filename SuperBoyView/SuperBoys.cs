using System;
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
            int count= (int)ProgramConfiguration.MasterDiction[EnumArry.Master.DataDefaultCount];
            //select database, request data
            //send request
            this.Views.DataSource = ControlSuperBoy.select(count);

        }
    }
}
