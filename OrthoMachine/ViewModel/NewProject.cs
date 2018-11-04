using OM_Form.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OM_Form.ViewModel
{
    public class NewProject
    {
        private string filename;
        public float rastersize;
        public float offset;
        OpenFileDialog ofd;
        Form1 form1;
        //string SavePath;

        public string Filename { get => filename; set => filename = value; }

        public NewProject(Form1 form1)
        {
            ofd = new OpenFileDialog();
            ofd.DefaultExt = ".asc";
            ofd.Filter = "Point cloud (*.txt)|*.txt|ASCII file (*.asc)|*.asc";
            this.form1 = form1;
            //SavePath = form1.SavePath;
        }

        public void GetProjectDir()
        {
            var folderBrowserDialog1 = new FolderBrowserDialog();


            string initpath = "g:\\_Magán\\_Óbudai Egyetem\\Szakdolgozat 1\\OM_projekt\\";
            folderBrowserDialog1.SelectedPath = initpath;
            DialogResult result = folderBrowserDialog1.ShowDialog();


            if (result == DialogResult.OK)
            {
                ;
                if (form1.SavePath != null)
                {
                    if (MessageBox.Show("Close this and create new Project?", "Create new project?", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        form1.SavePath = folderBrowserDialog1.SelectedPath;
                        form1.openToolStripMenuItem.Enabled = true;
                        //return SavePath;
                    }
                }
                else
                {
                    form1.SavePath = folderBrowserDialog1.SelectedPath;
                    form1.openToolStripMenuItem.Enabled = true;
                    //return SavePath;
                }
            }
            //return SavePath;
        }


        public bool GetSurfParams()
        {
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Filename = ofd.FileName;
                using (var form2 = new Form2(form1))
                {
                    form2.ShowDialog();
                    if (form2.DialogResult == DialogResult.OK)
                    {
                        //sf = new Surface(Filename, Offset, Rastersize, this);
                        rastersize = float.Parse(form2.rasterbox.Text.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                        offset = float.Parse(form2.offsetbox.Text.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            else
            {
                return false;
            }
        }

    }
}
