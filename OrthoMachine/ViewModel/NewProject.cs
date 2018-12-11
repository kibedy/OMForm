using Emgu.CV;
using Emgu.CV.Structure;
using OM_Form.View;
using ortomachine.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
            form1.DisableAllMenus();
            
        }

        public void GetProjectDir()
        {
            var folderBrowserDialog1 = new FolderBrowserDialog();


            //string initpath = "g:\\_Magán\\_Óbudai Egyetem\\Szakdolgozat 1\\OM_projekt_XY\\";
            string initpath = "d:\\__magán\\orto_mentések\\";            
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

        public void LoadProjectParams(Form1 form1, string SavePath)
        {
            try
            {
                StreamReader sr = new StreamReader(SavePath + "\\project.prj");
                string s = sr.ReadLine();
                form1.sf = new Surface("", form1.offset, form1.rastersize, form1);
                form1.filetype = int.Parse(s);
                
                form1.pictureBox1.Image = form1.sf.LoadSurface(SavePath, form1);
                GCHandle gch = GCHandle.Alloc(form1.pictureBox1.Image,GCHandleType.Normal);

                if (form1.filetype == 7)
                {
                    //form1.pictureBox1.Image = form1.sf.LoadSurface(SavePath, form1);
                    form1.sf.sc.RGBsurfImage = new Image<Bgr, byte>(SavePath + "\\surface_rgb.png");
                    form1.sf.sc.intSurfImage = new Image<Gray, byte>(SavePath + "\\surface_int.png");
                    //GCHandle gchrgb = GCHandle.Alloc(form1.sf.sc.RGBsurfImage);
                    //GCHandle gchint = GCHandle.Alloc(form1.sf.sc.intSurfImage);
                    form1.intensityToolStripMenuItem.Enabled = true;
                    form1.depthToolStripMenuItem.Enabled = true;                    
                    form1.rGBToolStripMenuItem.Enabled = true;
                }
                else if (form1.filetype == 4)
                {
                    form1.sf.sc.intSurfImage = new Image<Gray, byte>(SavePath + "\\surface_int.png");
                    //GCHandle gchint = GCHandle.Alloc(form1.sf.sc.intSurfImage);
                    form1.intensityToolStripMenuItem.Enabled = true;
                    form1.depthToolStripMenuItem.Enabled = true;
                    form1.rGBToolStripMenuItem.Enabled = false;
                }
                else
                {
                    //form1.pictureBox1.Image = form1.sf.LoadSurface(SavePath, form1);
                    
                    form1.intensityToolStripMenuItem.Enabled = false;
                    form1.depthToolStripMenuItem.Enabled = false;
                    form1.rGBToolStripMenuItem.Enabled = false;
                }

                sr.Close();
                if (form1.photos == null)
                {
                    form1.photos = new Photo();
                }
                try
                {
                    sr = new StreamReader(SavePath + "\\imagelist.txt");
                    List<string> photolist = new List<string>();
                    while (!sr.EndOfStream)
                    {
                        photolist.Add(sr.ReadLine());
                    }
                    //form1.photos.projimagefilenames = photolist;
                    form1.photos.SilentLoadPhotos(form1, photolist);
                    form1.saveProjectToolStripMenuItem.Enabled = true;
                    form1.removeSelectedToolStripMenuItem.Enabled = true;                    
                    sr.Close();

                }
                catch { }
                form1.EnalbleAllMenus();
                
            }
            catch { MessageBox.Show("Can't open project"); }
            }

    }
}
