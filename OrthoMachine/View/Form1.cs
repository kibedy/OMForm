using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ortomachine.Model;
using Microsoft.Win32;
using System.Threading;
using OM_Form.View;
using OM_Form.ViewModel;
using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;
using OrthoMachine.ViewModel;
using OrthoMachine.View;

namespace OM_Form
{
    public partial class Form1 : Form
    {
        public string filename;
        //Thread thread;
        public float offset, rastersize;
        public string SavePath;
        public Surface sf;
        public BackgroundWorker backgroundWorker1;
        NewProject project;
        public Photo photos;
        public List<Bitmap> photolist;
        public int filetype;
        Point _mousePt = new Point();
        bool _tracking = false;
        private float ImageScale = 1.0f;



        public Form1()
        {
            InitializeComponent();
            openToolStripMenuItem.Enabled = true;
            createToolStripMenuItem.Enabled = false;
            SavePath = null;
            InitializeBackgroundWorker();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            project = new NewProject(this);
        }

        public void DisableAllMenus()
        {
            this.overlapToolStripMenuItem.Enabled = false;
            this.fillHolesToolStripMenuItem.Enabled = false;
            this.resizeToolStripMenuItem.Enabled = false;
            this.bilinearFillHolesToolStripMenuItem.Enabled = false;
            this.addPicturesToolStripMenuItem.Enabled = false;
            this.saveProjectToolStripMenuItem.Enabled = false;
            this.addPicturesToolStripMenuItem.Enabled = false;            
            //this.removeSelectedPictureToolStripMenuItem.Enabled = false;
        }

        #region Backgroundworker for PC loading
        private void InitializeBackgroundWorker()
        {
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            sf = new Surface(project.Filename, project.offset, project.rastersize, this);
            e.Result = sf.Run();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // First, handle the case where an exception was thrown.
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {

            }
            else
            {

            }
            pictureBox1.Image = (Bitmap)e.Result;
            this.progressBar1.Value = 100;
            addPicturesToolStripMenuItem.Enabled = true;
            fillHolesToolStripMenuItem.Enabled = true;
            bilinearFillHolesToolStripMenuItem.Enabled = true;
            resizeToolStripMenuItem.Enabled = true;
            saveProjectToolStripMenuItem.Enabled = true;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar1.Value = e.ProgressPercentage;
        }
        #endregion


        #region PictureBox_1_Events
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            const float scale_per_delta = 0.1f / 120;
            ImageScale += e.Delta * scale_per_delta;


            if (ImageScale < 0.1) ImageScale = 0.1f;
            // Console.WriteLine(ImageScale);
            this.pictureBox1.Size = new Size((int)(sf.sc.image.Width * ImageScale), (int)(sf.sc.image.Height * ImageScale));           
        }


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _mousePt = e.Location;
                _tracking = true;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_tracking && (pictureBox1.Image.Width > this.ClientSize.Width || pictureBox1.Image.Height > this.ClientSize.Height))
            {
                panel1.AutoScrollPosition = new Point(-panel1.AutoScrollPosition.X + (_mousePt.X - e.X), -panel1.AutoScrollPosition.Y + (_mousePt.Y - e.Y));
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {

            _tracking = false;


        }
        #endregion

        private void openToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            //project = new NewProject(this);
            project.GetProjectDir();
            //sf = new Surface(project.Filename, project.offset, project.rastersize, this);
            //pictureBox1.Image = sf.LoadSurface(SavePath,this);            

            
            project.LoadProjectParams(this, SavePath);
            /*
            createToolStripMenuItem.Enabled = true;
            loadSurfaceToolStripMenuItem.Enabled = true;
            this.fillHolesToolStripMenuItem.Enabled = true;
            this.resizeToolStripMenuItem.Enabled = true;
            this.bilinearFillHolesToolStripMenuItem.Enabled = true;
            this.addPicturesToolStripMenuItem.Enabled = true;
            this.saveProjectToolStripMenuItem.Enabled = true;
            this.orthoToolStripMenuItem.Enabled = true;
            this.overlapToolStripMenuItem.Enabled = true;
            this.removeSpikesToolStripMenuItem.Enabled = true;
            */

        }
        public void EnalbleAllMenus()
        {
            createToolStripMenuItem.Enabled = true;
            loadSurfaceToolStripMenuItem.Enabled = true;
            this.fillHolesToolStripMenuItem.Enabled = true;
            this.resizeToolStripMenuItem.Enabled = true;
            this.bilinearFillHolesToolStripMenuItem.Enabled = true;
            this.addPicturesToolStripMenuItem.Enabled = true;
            this.saveProjectToolStripMenuItem.Enabled = true;
            this.orthoToolStripMenuItem.Enabled = true;
            this.overlapToolStripMenuItem.Enabled = true;
            this.removeSpikesToolStripMenuItem.Enabled = true;
        }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //project = new NewProject(this);
            project.GetProjectDir();
            createToolStripMenuItem.Enabled = true;
            loadSurfaceToolStripMenuItem.Enabled = false;
            this.fillHolesToolStripMenuItem.Enabled = false;
            this.resizeToolStripMenuItem.Enabled = false;
            this.bilinearFillHolesToolStripMenuItem.Enabled = false;
            this.overlapToolStripMenuItem.Enabled = false;            
            listView1.Clear();
            sf = new Surface("surface", 0, 0, this);
            

        }


        
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void addPicturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (photos == null)
            {
                photos = new Photo();
            }

            photos.LoadPhotos(this);
            SaveProject();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (project.GetSurfParams() == true)
            {
                // Start the asynchronous operation.
                backgroundWorker1.RunWorkerAsync();                
            }
            
        }

        private void fillHolesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //uint[,] surface = new uint[sf.image.Width,sf.image.Height];
            sf.fillHoles(this);

        }

        private void loadSurfaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sf = new Surface("surface", 0, 0, this);
            pictureBox1.Image = sf.LoadSurface(SavePath, this);
            fillHolesToolStripMenuItem.Enabled = true;
            bilinearFillHolesToolStripMenuItem.Enabled = true;
            resizeToolStripMenuItem.Enabled = true;
            saveProjectToolStripMenuItem.Enabled = true;
        }

        private void bilinearFillHolesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sf.bilinearfillHoles(this);
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {            
            //photos.ShowPhoto();

        }

        private void resizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sf.SurfaceResize(this);
        }


        private void SaveProject()
        {
            StreamWriter sw = new StreamWriter(SavePath + "\\project.prj");
            sw.WriteLine(filetype);

            sw.Close();

            sw = new StreamWriter(SavePath + "\\imagelist.txt");
            try
            {
                foreach (string item in photos.projimagefilenames)
                {
                    sw.WriteLine(item);
                }
            }
            catch
            {

            }
            sw.Close();
        }
        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveProject();
            /*
            StreamWriter sw = new StreamWriter(SavePath+"\\project.prj");
            sw.WriteLine(filetype);

            sw.Close();

            sw = new StreamWriter(SavePath + "\\imagelist.txt");
            try
            {               
                foreach (string item in photos.projimagefilenames)
                {
                    sw.WriteLine(item);
                }               
            }            
            catch
            {

            }
            sw.Close();
            */
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            photos.ShowPhoto();
        }

        private void removeSelectedPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count >= 1)
            {
                ListViewItem item = listView1.SelectedItems[0];
                int index = listView1.SelectedItems[0].Index;
                listView1.Items.RemoveAt(index);
                photos.projimagefilenames.RemoveAt(index);
                //Save
            }
        }

        private void FileStripMenuItem1_Click(object sender, EventArgs e)
        {
           
        }
        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

            ToolStripItem item = e.ClickedItem;
            if (e.ClickedItem.Name == "rGBToolStripMenuItem")
            {                
                pictureBox1.Image = sf.sc.RGBsurfImage.ToBitmap();
            }
            else if (e.ClickedItem.Name == "intensityToolStripMenuItem")
            {               
                pictureBox1.Image = sf.sc.intSurfImage.ToBitmap();                
            }
            else
            {                
                pictureBox1.Image = sf.sc.image.ToBitmap();                
            }
        }

        private void overlapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ortho ortho = new Ortho(this);
            ortho.ShowDialog();
            //ortho.Overlap();
        }

        private void removeSpikesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //sf.sc.image=sf.sc.image.SmoothMedian(3);
            sf.MedianFilter(this);
        }

        private void smoothSurfaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sf.BlurFilter(this);
        }

        private void contextMenuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (listView1.Items.Count >= 1)
            {
                ListViewItem item = listView1.SelectedItems[0];
                int index = listView1.SelectedItems[0].Index;
                listView1.Items.RemoveAt(index);
                photos.projimagefilenames.RemoveAt(index);
                SaveProject();
            }
        }


    }
}
