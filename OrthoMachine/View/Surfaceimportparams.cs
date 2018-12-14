using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OM_Form.View
{
    public partial class Form2 : Form
    {
        public Form1 form1;

        public Form2(Form1 form1)
        {
            this.DialogResult = DialogResult.None;
            InitializeComponent();
            this.form1 = form1;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            //this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                form1.rastersize = float.Parse(this.rasterbox.Text.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                form1.offset = float.Parse(this.offsetbox.Text.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                this.DialogResult = DialogResult.OK;
                form1.Show();
            }
            catch
            {
                MessageBox.Show("Invalid input data!");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            form1.Show();
        }
    }//class
}//namespace
