using Emgu.CV;
using Emgu.CV.Structure;
using OM_Form;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrthoMachine.View
{
    public partial class ImageProcess : Form
    {
        string filename;



        public ImageProcess(Form1 form1, string filename)
        {
            InitializeComponent();
            this.filename = filename;
            Image<Bgr, byte> photo = new Image<Bgr, byte>(form1.SavePath + "\\"+filename);
            this.pictureBox1.Image = photo.ToBitmap();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
