﻿using Emgu.CV;
using Emgu.CV.Structure;
using OM_Form;
//using OrthoMachine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace OrthoMachine.View
{
    public partial class ImageProcess: Form
    {
        string filename;
        private int ImageWidth, ImageHeight, ImageWidthS, ImageHeightS;
        private float ImageScale = 1.0f;
        private float ImageScaleS = 1.0f;
        Point _mousePt = new Point();
        bool _tracking = false;
        Form1 form1;
        bool EnablePickpoints;
        Image<Gray, ushort> surface;
        Orientation orientation;
        Point pp;
        Image<Gray, byte> markerimage;
        Image<Bgra, byte> photo;
        Image<Bgra, byte> photo_orig;
        Image<Gray, byte> visible;
        ArrayList colors;
        enum ShowState { rgb, intensity, depth, photo, ortho };
        ShowState SSS;
        double[][] A;
        double[][] Qxx;
        double[][] At;
        double[][] dX;
        double[][] l;
        int pointpaircount;
        Image<Bgr, byte> ortho;
        double focus;
        double pix_mm;
        double Xo, Yo, Zo; //init pic postition
        double o;
        double p;
        double k;    //omega phi kappa
        double[][] a;
        public BackgroundWorker backgroundWorker1;
        int maxthreads;
        int procbarvalue;





        public ImageProcess(Form1 form1, string filename, Orientation orientation)
        {
            InitializeComponent();
            this.filename = filename;
            this.orientation = orientation;
            this.form1 = form1;
            GCHandle gchphoto = GCHandle.Alloc(photo, GCHandleType.Pinned); 
            photo = new Image<Bgra, byte>(form1.SavePath + "\\photos\\" + filename);
            photo_orig = photo.Clone();
            this.pictureBox1.Image = photo.ToBitmap();
            ImageWidth = pictureBox1.Image.Width;
            ImageHeight = pictureBox1.Image.Height;

            colors = CreateColorList();

            this.FormClosing += ImageProcess_FormClosing;
            SSS = ShowState.depth;


            if (form1.filetype == 7)
            {
                rGBToolStripMenuItem.Visible = true;
                intensityToolStripMenuItem.Visible = true;
                depthToolStripMenuItem.Visible = true;
                orthoToolStripMenuItem.Enabled = false;
            }
            else if (form1.filetype == 4)
            {
                intensityToolStripMenuItem.Visible = true;
                depthToolStripMenuItem.Visible = true;
                rGBToolStripMenuItem.Visible = false;
                orthoToolStripMenuItem.Enabled = false;
            }
            else
            {
                intensityToolStripMenuItem.Visible = false;
                depthToolStripMenuItem.Visible = false;
                rGBToolStripMenuItem.Visible = false;
                orthoToolStripMenuItem.Enabled = false;
            }



            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox1.Size = new Size(ImageWidth, ImageHeight);
            EnablePickpoints = false;

            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.Columns.Add("Point Id", 80, HorizontalAlignment.Left);
            this.listView1.Columns.Add("Image X", 80, HorizontalAlignment.Left);
            this.listView1.Columns.Add("Image Y", 80, HorizontalAlignment.Left);
            this.listView2.View = System.Windows.Forms.View.Details;
            this.listView2.Columns.Add("Point Id", 80, HorizontalAlignment.Left);
            this.listView2.Columns.Add("Global X", 80, HorizontalAlignment.Left);
            this.listView2.Columns.Add("Global Y", 80, HorizontalAlignment.Left);
            this.listView2.Columns.Add("Global Z", 80, HorizontalAlignment.Left);

            LoadMarkerCoordinates();

        }

        private void LoadMarkerCoordinates()
        {
            try
            {
                string[] fn = filename.Split('.');
                StreamReader sr = new StreamReader(form1.SavePath + "\\photos\\" + fn[0] + ".ori");
                bool photobool = true;
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] lineE = line.Split(';');
                    if (lineE[0].Equals("zzz"))
                    {
                        photobool = false;
                        continue;
                    }
                    else
                    {
                        if (photobool == true)
                        {
                            ListViewItem item = new ListViewItem(new[] { listView1.Items.Count.ToString(), lineE[1], lineE[2] });
                            listView1.Items.Add(item);
                        }
                        if (photobool == false)
                        {
                            ListViewItem item = new ListViewItem(new[] { listView2.Items.Count.ToString(), lineE[1], lineE[2], lineE[3] });
                            listView2.Items.Add(item);
                        }
                    }

                }
                sr.Close();


            }
            catch { }
            listView1.Refresh();
            DrawMarkersPhoto();
            listView2.Refresh();      
            surface = form1.sf.sc.image;
            DrawMarkerSurface();
            SetOrientationForm();
        }

        private void SaveMarkers()
        {
            string[] fn = filename.Split('.');
            StreamWriter sw = new StreamWriter(form1.SavePath + "\\photos\\" + fn[0] + ".ori");           
            if (listView1.Items.Count > 0)
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    sw.WriteLine(item.SubItems[0].Text + ";" + item.SubItems[1].Text + ";" + item.SubItems[2].Text);
                }
            }
            sw.WriteLine("zzz");

            if (listView2.Items.Count > 0)
            {
                foreach (ListViewItem item in listView2.Items)
                {
                    sw.WriteLine(item.SubItems[0].Text + ";" + item.SubItems[1].Text + ";" + item.SubItems[2].Text + ";" + item.SubItems[3].Text);
                }
            }
            sw.Close();
        }

        private void ImageProcess_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                for (int i = 0 ; i < objThread.Length ; i++)
                {
                    objThread[i].Abort();
                }
                this.backgroundWorker1.CancelAsync();
            }
            catch { }
            SaveMarkers();
        }


        #region PictureBox_1_Events
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            const float scale_per_delta = 0.1f / 120;
            ImageScale += e.Delta * scale_per_delta;


            if (ImageScale < 0.1) ImageScale = 0.1f;
            
            this.pictureBox1.Size = new Size((int)(ImageWidth * ImageScale), (int)(ImageHeight * ImageScale));            
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

        # region PictureBox_2_Events

        private void pictureBox2_MouseWheel(object sender, MouseEventArgs e)
        {
            pictureBox2.Refresh();
            const float scale_per_delta = 0.1f / 120;
            ImageScaleS += e.Delta * scale_per_delta;


            if (ImageScaleS < 0.1) ImageScaleS = 0.1f;
            this.pictureBox2.Size = new Size((int)(ImageWidthS * ImageScaleS), (int)(ImageHeightS * ImageScaleS));
          
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _mousePt = e.Location;
                _tracking = true;
            }
        }


        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (_tracking &&
             (pictureBox2.Image.Width > this.ClientSize.Width ||
             pictureBox2.Image.Height > this.ClientSize.Height))
            {
                panel2.AutoScrollPosition = new Point(-panel2.AutoScrollPosition.X + (_mousePt.X - e.X),
                 -panel2.AutoScrollPosition.Y + (_mousePt.Y - e.Y));
            }
        }


        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            _tracking = false;
        }
        #endregion


        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem item = e.ClickedItem;
            if (e.ClickedItem.Name == "rGBToolStripMenuItem")
            {
                SSS = ShowState.rgb;                
            }
            else if (e.ClickedItem.Name == "intensityToolStripMenuItem")
            {
                SSS = ShowState.intensity;
            }
            else if (e.ClickedItem.Name == "orthoToolStripMenuItem")
            {
                SSS = ShowState.ortho;                
            }
            else
            {
                SSS = ShowState.depth;                
            }
            DrawMarkerSurface();
        }

        private void panel1_MouseWheel(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SetOrientationForm()
        {
            EnablePickpoints = true;
            buttonCalculate.Visible = true;
         
            panel1.Height = this.Height - 200;
            panel1.Size = new Size(this.Width / 2 - 2, panel1.Height);
         
            this.listView1.Visible = true;
            this.listView2.Visible = true;
            panel2.Location = new Point(panel1.Location.X + panel1.Width + 2, panel1.Location.Y);
            panel2.Width = panel1.Width - 4;
            panel2.Height = panel1.Height;


           
            surface = form1.sf.sc.image;
            SSS = ShowState.depth;
            this.pictureBox2.Visible = true;
            this.panel2.Visible = true;
           
            this.pictureBox2.Cursor = Cursors.Hand;
            ImageWidthS = surface.Bitmap.Width;
           
            ImageHeightS = surface.Bitmap.Height;
            this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox2.Size = new Size(ImageWidthS, ImageHeightS);
            this.SizeChanged += new System.EventHandler(this.ImageProcess_SizeChanged);
            this.buttonPhotoUp.Visible = true;
            this.buttonPhotoDown.Visible = true;
            this.buttonPhotoDel.Visible = true;
            this.buttonSurfaceDel.Visible = true;
            this.buttonSurfaceUp.Visible = true;
            this.buttonSurfaceDown.Visible = true;
            this.progressBar1.Visible = true;
            this.label1.Visible = true;
            this.textBox1.Visible = true;
           

        }

        private void orientateToolStripMenuItem_Click(object sender, EventArgs e)
        {

            SaveMarkers();

        }

        private void ImageProcess_SizeChanged(object sender, EventArgs e)
        {
            this.panel1.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            this.panel2.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            panel1.Size = new Size(this.Width / 2 - 10, this.Height - 200);
            panel2.Size = new Size(this.Width / 2 - 10, panel1.Height);
            panel2.Location = new Point(panel1.Location.X + panel1.Width + 2, panel1.Location.Y);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
           

        }

        private void addMarkerSurface(object sender, MouseEventArgs e)
        {
            if (EnablePickpoints)
            {
                Point pp = e.Location;                
                int xx = (int)(pp.X / ImageScaleS);
                int yy = (int)(pp.Y / ImageScaleS);

                float xxm = (float)(xx * form1.rastersize + form1.sf.sc.X0);
                float yym = (float)((surface.Height - yy) * form1.rastersize + form1.sf.sc.Y0);
                float hh = (float)(surface.Data[(int)yy, (int)xx, 0]) / 1000;
                var item = new ListViewItem(new[] { (listView2.Items.Count.ToString()).ToString(), xxm.ToString("0.000"), yym.ToString("0.000"), hh.ToString("0.000") });

                if (listView1.Items.Count < 20)
                {
                    this.listView2.Items.Add(item);
                }
                else
                {
                    MessageBox.Show("Too many markers!");
                }
                if (true)
                {

                }
                DrawMarkerSurface();
                panel2.Invalidate();
            }
            GC.Collect();
        }

        private void addMarkerPhoto(object sender, MouseEventArgs e)
        {
            if (EnablePickpoints)
            {
                pp = e.Location;              
                var item = new ListViewItem(new[] { listView1.Items.Count.ToString(), ((pp.X / ImageScale) - photo.Width / 2).ToString("0.00"), ((-(pp.Y / ImageScale) + photo.Height / 2)).ToString("0.00") });

                if (listView1.Items.Count < 20)
                {
                    this.listView1.Items.Add(item);
                }
                else
                {
                    MessageBox.Show("Too many markers!");
                }
               
                DrawMarkersPhoto();               
                panel1.Invalidate();
                GC.Collect();
            }


        }



        private void DrawMarkersPhoto()
        {
           

            Image<Bgra, byte> toDraw = new Image<Bgra, byte>(photo.Bitmap);
           
            Image<Bgra, byte> temp = new Image<Bgra, byte>(toDraw.Bitmap);
            if (listView1.Items.Count != 0)
            {
                ListViewItem[] items = new ListViewItem[listView1.Items.Count];
                listView1.Items.CopyTo(items, 0);
                int i = 0;
                foreach (ListViewItem item in listView1.Items)
                {
           
                    markerimage = new Image<Gray, byte>(new Size(photo.Width, photo.Height));

                    float X = float.Parse(item.SubItems[1].Text) + photo.Width / 2;
                    float Y = -float.Parse(item.SubItems[2].Text) + photo.Height / 2;
                    PointF center = new PointF(X, Y);
                    float r = 10;
                    CircleF circle1 = new CircleF(center, r);
                    CircleF circle2 = new CircleF(center, 1);
                    markerimage.Draw(circle1, new Gray(250), 1);
                
                    markerimage.Draw(circle2, new Gray(250), 1);

                    markerimage._SmoothGaussian(3, 3, 1, 1);
                    temp.SetValue((Bgra)colors[i], markerimage);
                    i++;
                   


                }
                pictureBox1.Image = temp.Bitmap;
            }
            else
            {
                pictureBox1.Image = photo.Bitmap;
            }
        }

        private void DrawMarkerSurface()
        {
            Image<Bgra, byte> toDraw;
            if (SSS == ShowState.depth)
            {
                toDraw = new Image<Bgra, byte>(form1.sf.sc.image.Bitmap);
            }
            else if (SSS == ShowState.rgb)
            {
                toDraw = new Image<Bgra, byte>(form1.sf.sc.RGBsurfImage.Bitmap);
            }
            else if (SSS == ShowState.intensity)
            {
                toDraw = new Image<Bgra, byte>(form1.sf.sc.intSurfImage.Bitmap);

            }
            else
            {
                toDraw = new Image<Bgra, byte>(ortho.Bitmap);
            }            
            Image<Bgra, byte> temp = new Image<Bgra, byte>(toDraw.Bitmap);
            if (listView2.Items.Count != 0)
            {
                ListViewItem[] items = new ListViewItem[listView2.Items.Count];
                listView2.Items.CopyTo(items, 0);
                int i = 0;
                foreach (ListViewItem item in listView2.Items)
                {

                    markerimage = new Image<Gray, byte>(new Size(surface.Width, surface.Height));
                    float X = (float)(((float.Parse(item.SubItems[1].Text)) - form1.sf.sc.X0) / form1.rastersize);
                    float Y = (float)(surface.Height - (float.Parse(item.SubItems[2].Text) - form1.sf.sc.Y0) / form1.rastersize);


                    PointF center = new PointF(X, Y);
                    float r = 10;
                    CircleF circle1 = new CircleF(center, r);
                    CircleF circle2 = new CircleF(center, 1);
                    markerimage.Draw(circle1, new Gray(250), 1);
                    markerimage.Draw(circle2, new Gray(250), 1);

                    markerimage._SmoothGaussian(3, 3, 1, 1);
                    temp.SetValue((Bgra)colors[i], markerimage);
                    i++;                    


                }
                pictureBox2.Image = temp.Bitmap;
            }
            else
            {
                pictureBox2.Image = toDraw.Bitmap;
            }
        }



        private void buttonPhotoDel_Click(object sender, EventArgs e)
        {
            DelMarker(listView1, pictureBox1, ShowState.photo);
        }

        private void buttonSurfaceUp_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView2.Items.Count >= 2 && listView2.SelectedItems[0].Index >= 1)
                {
                    ListViewItem item = listView2.SelectedItems[0];
                    int index = listView2.SelectedItems[0].Index;
                    ListViewItem itemUpper = listView2.Items[index - 1];
                    itemUpper.Text = index.ToString();
                    item.Text = (index - 1).ToString();
                    listView2.Items.RemoveAt(item.Index - 1);
                    listView2.Items.RemoveAt(item.Index);
                    listView2.Items.Insert(index - 1, item);
                    listView2.Items.Insert(index, itemUpper);
                    listView2.Refresh();
                    if (SSS == ShowState.depth)
                    {                        
                        DrawMarkerSurface();
                    }
                    else if (SSS == ShowState.rgb)
                    {                        
                        DrawMarkerSurface();
                    }
                    else if (SSS == ShowState.ortho)
                    {                        
                        DrawMarkerSurface();
                    }
                    else
                    {                        
                        DrawMarkerSurface();
                    }
                }
            }
            catch { }
        }

        private void buttonSurfaceDown_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView2.Items.Count >= 2 && listView2.SelectedItems[0].Index < listView2.Items.Count - 1)
                {
                    ListViewItem item = listView2.SelectedItems[0];
                    int index = listView2.SelectedItems[0].Index;
                    ListViewItem lowerItem = listView2.Items[index + 1];
                    lowerItem.Text = (index).ToString();
                    item.Text = (index + 1).ToString();
                    listView2.Items.RemoveAt(item.Index + 1);
                    listView2.Items.RemoveAt(item.Index);
                    listView2.Items.Insert(index, lowerItem);
                    listView2.Items.Insert(index + 1, item);
                    listView2.Refresh();                   
                    DrawMarkerSurface();
                }
            }
            catch { }
        }

        private void DelMarker(ListView listview, PictureBox picturebox, ShowState SSS)
        {
            try
            {
                int index;
                if (listview.Items.Count >= 1)
                {
                    ListViewItem item = listview.SelectedItems[0];
                    index = listview.SelectedItems[0].Index;
                    listview.Items.RemoveAt(item.Index);
                    ListViewItem lowerItem;
                    if (index != (listview.Items.Count) && listview.Items.Count != 0)
                    {
                        for (int i = index ; i < listview.Items.Count ; i++)
                        {
                            lowerItem = listview.Items[i];
                            lowerItem.Text = (i).ToString();
                        }
                    }
                    if (index > 1)
                    {
                        listview.Items[(index - 1)].Selected = true;
                        listview.Refresh();
                    }

                }
            }
            catch { MessageBox.Show("No item selected!"); }
            listview.Refresh();
          
            if (SSS == ShowState.photo)
            {
                DrawMarkersPhoto();
            }
            else DrawMarkerSurface();

        }
        private void buttonSurfaceDel_Click(object sender, EventArgs e)
        {
            DelMarker(listView2, pictureBox2, SSS);

        }

        private ArrayList CreateColorList()
        {
            ArrayList colors = new ArrayList();         

            colors.Add(new Bgra(0, 0, 255, 255));
            colors.Add(new Bgra(0, 255, 0, 255));
            colors.Add(new Bgra(255, 0, 0, 255));
            colors.Add(new Bgra(255, 0, 255, 255));
            colors.Add(new Bgra(255, 255, 0, 255));
            colors.Add(new Bgra(0, 255, 255, 255));
            colors.Add(new Bgra(128, 128, 128, 255));
            colors.Add(new Bgra(128, 0, 0, 255));
            colors.Add(new Bgra(0, 128, 0, 255));
            colors.Add(new Bgra(0, 0, 128, 255));
            colors.Add(new Bgra(128, 128, 0, 255));
            colors.Add(new Bgra(0, 128, 128, 255));
            colors.Add(new Bgra(128, 0, 128, 255));
            colors.Add(new Bgra(50, 50, 50, 255));
            colors.Add(new Bgra(50, 0, 0, 255));
            colors.Add(new Bgra(0, 50, 0, 255));
            colors.Add(new Bgra(0, 0, 50, 255));
            colors.Add(new Bgra(50, 50, 0, 255));
            colors.Add(new Bgra(0, 50, 50, 255));
            colors.Add(new Bgra(50, 0, 50, 255));

            return colors;

        }


        float foc;
        bool ok = false;
        private void buttonCalculate_Click(object sender, EventArgs e)
        {
           
            
            try
            {
                foc = float.Parse(this.textBox1.Text.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
               
                if (foc > 0)
                {
                    foc = -1 * foc;
                    ok = true;
                }
            }
            catch (Exception)
            {
                ok = false;
                MessageBox.Show("Invalid input!");
            }                
            if (ok==true)
            {
                CalculateOrientation(listView1, listView2, foc);
            }
        }

        private void buttonPhotoUp_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count >= 2 && listView1.SelectedItems[0].Index >= 1)
            {
                ListViewItem item = listView1.SelectedItems[0];
                int index = listView1.SelectedItems[0].Index;
                ListViewItem itemUpper = listView1.Items[index - 1];
                itemUpper.Text = index.ToString();
                item.Text = (index - 1).ToString();
                listView1.Items.RemoveAt(item.Index - 1);
                listView1.Items.RemoveAt(item.Index);
                listView1.Items.Insert(index - 1, item);
                listView1.Items.Insert(index, itemUpper);
                listView1.Refresh();
                //DrawMarkers(listView1, pictureBox1, photo.ToBitmap());
                DrawMarkersPhoto();
            }
        }


        private void buttonPhotoDown_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count >= 2 && listView1.SelectedItems[0].Index < listView1.Items.Count - 1)
            {
                ListViewItem item = listView1.SelectedItems[0];
                int index = listView1.SelectedItems[0].Index;
                ListViewItem lowerItem = listView1.Items[index + 1];
                lowerItem.Text = (index).ToString();
                item.Text = (index + 1).ToString();
                listView1.Items.RemoveAt(item.Index + 1);
                listView1.Items.RemoveAt(item.Index);
                listView1.Items.Insert(index, lowerItem);
                listView1.Items.Insert(index + 1, item);
                listView1.Refresh();                
                DrawMarkersPhoto();
            }
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            DrawMarkersPhoto();
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            DrawMarkerSurface();
        }


        private void CalculateOrientation(ListView listView1, ListView listView2, float foc)
        {
            SaveMarkers();
            double sumX = 0;
            double sumY = 0;
            double sumZ = 0;
            focus = foc;            
            pix_mm = 0.0067515;


            foreach (ListViewItem item in listView2.Items)
            {
                sumX += double.Parse(item.SubItems[1].Text);
                sumY += double.Parse(item.SubItems[2].Text);
                sumZ += double.Parse(item.SubItems[3].Text);
            }
            Xo = sumX / listView2.Items.Count; //in meter
            Yo = sumY / listView2.Items.Count;  //start position in negative 
            Zo = (sumZ / listView2.Items.Count + 20);

            int iter = 0;
            double var2 = 0.01;
            double var3 = 0.00000001;

            pointpaircount = Math.Min(listView1.Items.Count, listView2.Items.Count);
            double[] DX = new double[pointpaircount];
            double[] DY = new double[pointpaircount];
            double[] DZ = new double[pointpaircount];
            double[] x = new double[pointpaircount];
            double[] y = new double[pointpaircount];
            double[] xo = new double[pointpaircount * 2];
            double[] lm = new double[pointpaircount];

            dX = MatrixCreate(1, 6);  // new double[1,6];            
            Qxx = MatrixCreate(6, 6); // new double[6, 6];

            
            for (int j = 0 ; j < pointpaircount ; j++)
            {
                ListViewItem item = listView1.Items[j];
                x[j] = (double.Parse(item.SubItems[1].Text)) * pix_mm;
                y[j] = (double.Parse(item.SubItems[2].Text)) * pix_mm;
            }
            
            const int iterlimit = 1000;
            a = MatrixCreate(3, 3);

            while (iter < iterlimit)
            {
                a[0][0] = cos(p) * cos(k);
                a[0][1] = -cos(p) * sin(k);
                a[0][2] = sin(p);

                a[1][0] = cos(o) * sin(k) + sin(o) * sin(p) * cos(k);
                a[1][1] = cos(o) * cos(k) - sin(o) * sin(p) * sin(k);
                a[1][2] = -sin(o) * cos(p);

                a[2][0] = sin(o) * sin(k) - cos(o) * sin(p) * cos(k);
                a[2][1] = sin(o) * cos(k) + cos(o) * sin(p) * sin(k);
                a[2][2] = cos(o) * cos(p);



                for (int i = 0 ; i < pointpaircount ; i++)
                {
                    ListViewItem item = listView2.Items[i];

                    DX[i] = double.Parse(item.SubItems[1].Text) - Xo;
                    DY[i] = double.Parse(item.SubItems[2].Text) - Yo;
                    DZ[i] = double.Parse(item.SubItems[3].Text) - Zo;

                    xo[2 * i] = focus * (a[0][0] * DX[i] + a[1][0] * DY[i] + a[2][0] * DZ[i]) / (a[0][2] * DX[i] + a[1][2] * DY[i] + a[2][2] * DZ[i]);
                    xo[2 * i + 1] = focus * (a[0][1] * DX[i] + a[1][1] * DY[i] + a[2][1] * DZ[i]) / (a[0][2] * DX[i] + a[1][2] * DY[i] + a[2][2] * DZ[i]);
                    lm[i] = DZ[i] / (a[2][0] * x[i] + a[2][1] * y[i] + a[2][2] * focus);                   

                }

                //Beobachtungsvektor                               
                l = MatrixCreate(pointpaircount * 2, 1);
                for (int i = 0 ; i < pointpaircount ; i++)
                {
                    l[2 * i][0] = x[i] - xo[2 * i];
                    l[2 * i + 1][0] = y[i] - xo[2 * i + 1];
                }

                //Designmatrix
                double[] tmp = new double[pointpaircount];

                A = MatrixCreate(pointpaircount * 2, 6);
                At = MatrixCreate(6, pointpaircount * 2);
                double[][] Ata = MatrixCreate(pointpaircount * 2, 6);

                for (int i = 0 ; i < pointpaircount ; i++)
                {
                    tmp[i] = focus * lm[i];
                    A[2 * i][0] = (a[0][2] * x[i] - a[0][0] * focus) / tmp[i];
                    A[2 * i][1] = (a[1][2] * x[i] - a[1][0] * focus) / tmp[i];
                    A[2 * i][2] = (a[2][2] * x[i] - a[2][0] * focus) / tmp[i];
                    A[2 * i][3] = y[i] * sin(p) + (x[i] / focus * (x[i] * sin(k) + y[i] * cos(k)) + focus * sin(k)) * cos(p);
                    A[2 * i][4] = -focus * cos(k) - (x[i] / focus) * (x[i] * cos(k) - y[i] * sin(k));
                    A[2 * i][5] = y[i];
                    A[2 * i + 1][0] = (a[0][2] * y[i] - a[0][1] * focus) / tmp[i];
                    A[2 * i + 1][1] = (a[1][2] * y[i] - a[1][1] * focus) / tmp[i];
                    A[2 * i + 1][2] = (a[2][2] * y[i] - a[2][1] * focus) / tmp[i];
                    A[2 * i + 1][3] = -x[i] * sin(p) + ((y[i] / focus) * (x[i] * sin(k) + y[i] * cos(k)) + focus * cos(k)) * cos(p);
                    A[2 * i + 1][4] = focus * sin(k) - (y[i] / focus) * (x[i] * cos(k) - y[i] * sin(k));
                    A[2 * i + 1][5] = -x[i];



                }

                //Normálegyenlet megoldása
                double[] vec = new double[6];

                At = Transpose(A);
                Ata = MatrixProduct(At, A);
                Qxx = MatrixCreate(6, 6);
                Qxx = MatrixIdentity(6);
                Qxx = MatrixInverse(Ata);

                //double[][] Atl;
                double[][] Atl_;
                Atl_ = MatrixProduct(At, l);             

                dX = MatrixProduct(Qxx, Atl_);
                Xo = Xo + dX[0][0];
                Yo = Yo + dX[1][0];
                Zo = Zo + dX[2][0];
                o = o + dX[3][0];
                p = p + dX[4][0];
                k = k + dX[5][0];

                Console.WriteLine(Xo + " " + Yo + " " + Zo + " Omega:" + (o % Math.PI) + " Phi:" + (p % Math.PI) + " Kappa:" + (k % Math.PI));


                //Exit conditions
                double[] dvec = new double[6];
                var2 = 0;
                for (int i = 0 ; i < 6 ; i++)
                {
                    dvec[i] = dX[i][0] - vec[i];
                    var2 = var2 + Math.Abs(dvec[i]);
                }

                if (var2 < var3)
                {
                    Console.WriteLine("Iteration stopped");
                    Console.WriteLine(Xo + " " + Yo + " " + Zo + " Omega:" + (o % Math.PI) + " Phi:" + (p % Math.PI) + " Kappa:" + (k % Math.PI));
                    Console.WriteLine("after " + iter + " iterations");
                    break; //Break Iteration
                }

                iter++;
            }//while

            if (iter > iterlimit)
            {
                Console.WriteLine("no solution!!");
            }
            else
            {
                string ff = ComputeErrors();
                ff = ("Xo:" + Xo.ToString("0.000") + "\nYo:" + Yo.ToString("0.000") + " \nZo:" + Zo.ToString("0.000") +
                    "\nOmega:" + (o % Math.PI * 180 / Math.PI).ToString("0.000") + " deg\nPhi:" + (p % Math.PI * 180 / Math.PI).ToString("0.000") +
                    " deg\nKappa:" + (k % Math.PI * 180 / Math.PI).ToString("0.000") + " deg\n\n") + ff;
                
                DialogResult dr = MessageBox.Show(ff, "Exterior Orientation", MessageBoxButtons.YesNo);
                switch (dr)
                {
                    case DialogResult.Yes:
                      
                        InitializeBackgroundWorker();
                        backgroundWorker1.WorkerReportsProgress = true;
                        backgroundWorker1.WorkerSupportsCancellation = true;

                        backgroundWorker1.RunWorkerAsync();
                       
                        break;
                    case DialogResult.No:

                        break;
                }
            }

        }
        Thread[] objThread;
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
    
            maxthreads = Environment.ProcessorCount-2;
            maxthreads = Math.Max(2, maxthreads);
            progressBar1.Invoke(new MethodInvoker(delegate { progressBar1.Value = 0; }));
            procbarvalue = 0;

            objThread = new Thread[maxthreads];
            for (int i = 0 ; i < maxthreads ; i++)
            {
                int sl = surface.Width / maxthreads * i;
                int sh = surface.Width / maxthreads * (i + 1);
                objThread[i] = new Thread(() =>
                {
                    try
                    {
                        Thread.CurrentThread.IsBackground = true;
                        Visibilitycheck(sl, sh, 0, surface.Height);
                    }
                    finally
                    {                        
                        progressBar1.Invoke(new MethodInvoker(delegate { progressBar1.Value += 100 / maxthreads; }));
                    }
                    
                });                
                objThread[i].Start();

            }

            for (int i = 0 ; i < objThread.Length ; i++)
            {
                // Wait until thread is finished.
                objThread[i].Join();
            }
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
                MakeOrthoPhoto();
                orthoToolStripMenuItem.Enabled = true;
                form1.overlapToolStripMenuItem.Enabled = true;
                progressBar1.Value = 100;
            }

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //form1.progressBar1.Value = e.ProgressPercentage;
        }
        private void InitializeBackgroundWorker()
        {
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
        }

        private void MakeOrthoPhoto()
        {
            if (!Directory.Exists(form1.SavePath + "\\ortho\\"))
            {
                DirectoryInfo di = Directory.CreateDirectory(form1.SavePath + "\\ortho\\");
            }

            ortho = new Image<Bgr, byte>(surface.Width, surface.Height);
            //visible.Save(form1.SavePath + "\\ortho\\visible.png");

            double[][] aa = MatrixCreate(3, 3);
            double co = cos(o);
            double so = sin(o);
            double cp = cos(p);
            double sp = sin(p);
            double cka = cos(k);
            double ska = sin(k);
            aa[0][0] = cp * cka + sp * so * ska;
            aa[0][1] = co * ska;
            aa[0][2] = -sp * cka + cp * so * ska;
            aa[1][0] = -cp * ska + sp * so * cka;
            aa[1][1] = co * cka;
            aa[1][2] = sp * ska + cp * so * cka;
            aa[2][0] = sp * co;
            aa[2][1] = -so;
            aa[2][2] = cp * co;
            double[][] a_inv = MatrixCreate(3, 3);
            a_inv = MatrixInverse(a);

            int jj = 0;
            for (int i = 0 ; i < surface.Height ; i++)
            {
                for (int j = 0 ; j < surface.Width ; j++)
                {
                    if (surface.Data[i, j, 0] != 0 && visible.Data[i, j, 0] == 255)
                    {
                        double X = (form1.sf.sc.X0 + j * form1.rastersize);
                        double Y = (form1.sf.sc.Y0 + (surface.Height - i) * form1.rastersize);
                        double Z = ((double)(surface.Data[i, j, 0])) / 1000;
                        double DX = X - Xo;
                        double DY = Y - Yo;
                        double DZ = Z - Zo;
                        int xb = (int)((focus * (a[0][0] * DX + a[1][0] * DY + a[2][0] * DZ) / (a[0][2] * DX + a[1][2] * DY + a[2][2] * DZ)) / pix_mm + 0.5);
                        int yb = (int)((focus * (a[0][1] * DX + a[1][1] * DY + a[2][1] * DZ) / (a[0][2] * DX + a[1][2] * DY + a[2][2] * DZ)) / pix_mm + 0.5);
                        int[] xy = Image2PixCoord(xb, yb);

                        if (xy[1] > 0 && xy[1] < photo.Height && xy[0] > 0 && xy[0] < photo.Width && visible.Data[i, j, 0] == 255)
                        {
                            byte b = photo.Data[xy[1], xy[0], 0];
                            byte g = photo.Data[xy[1], xy[0], 1];
                            byte r = photo.Data[xy[1], xy[0], 2];
                            ortho.Data[i, j, 0] = b;
                            ortho.Data[i, j, 1] = g;
                            ortho.Data[i, j, 2] = r;
                            jj++;
                        }
                        else
                        {
                            ortho.Data[i, j, 0] = 0;
                            ortho.Data[i, j, 1] = 0;
                            ortho.Data[i, j, 2] = 0;
                        }
                    }
                }
            }

            SSS = ShowState.ortho;
            pictureBox2.Image = ortho.Bitmap;
            string[] ss = filename.Split('.');
           
            
            string o_path = form1.SavePath + "\\ortho\\" + ss[0] + "_ortho.png";
            ortho.Save(o_path);
            StreamWriter sw = new StreamWriter(form1.SavePath + "\\ortholist.txt", true);          
            sw.WriteLine(o_path);            
            sw.Close();
        }

        private int[] Image2PixCoord(int x, int y)
        {
            int[] result = new int[2];
            int xx = x + photo.Width / 2;
            int yy = photo.Height - (photo.Height / 2 + y);
            result[0] = xx;
            result[1] = yy;
            return result;
        }

        private string ComputeErrors()
        {
            double[][] AQxx = MatrixCreate(6, 6);
            AQxx = MatrixProduct(A, Qxx);

            double[][] Qll_k = MatrixProduct(AQxx, At);

            double[][] v = MatrixProduct(A, dX);
            for (int i = 0 ; i != 6 ; i++)
            {
                v[i][0] = v[i][0] - l[i][0];
            }
            double[][] vt = Transpose(v);

            double[][] vtv = MatrixProduct(vt, v);
            double mo = Math.Sqrt(vtv[0][0] / (pointpaircount * 2 - 6));
            double[] sx = new double[pointpaircount];
            double[] sy = new double[pointpaircount];
            string result = "";      
            double[] s = new double[6];
            for (int i = 0 ; i < 3 ; i++)
            {
                s[i] = mo * Math.Sqrt(Qxx[i][i]);
                s[i + 3] = mo * Math.Sqrt(Qxx[i + 3][i + 3]) / Math.PI * 180;

            }
            result += ("\nStandard deviation:");
            result += ("\nX: " + (s[0]).ToString("0.000000") + " mm");
            result += ("\nY: " + (s[1]).ToString("0.000000") + " mm");
            result += ("\nZ: " + (s[2]).ToString("0.000000") + " mm");
            result += ("\nomega:" + s[3].ToString("0.0000000") + " deg");
            result += ("\nphi:" + s[4].ToString("0.0000000") + " deg");
            result += ("\nkappa:" + s[5].ToString("0.0000000") + " deg");

            return result;
        }



        private void Visibilitycheck(int fromX, int toX, int fromY, int toY)
        {            
            visible = new Image<Gray, byte>(surface.Width, surface.Height, new Gray(255));            
            //double f = 0.01; //3D step
            double sv = form1.rastersize / 2;
           
            double xh, yh, s1, s2, S, se, sh, x, y, z;
            int ix, iy, sw;            
            for (int i = fromY ; i < toY ; i++)
            {
                for (int j = fromX ; j < toX ; j++)
                {
                    if (surface.Data[i, j, 0] == 0)
                    {
                        visible.Data[i, j, 0] = 0;
                        continue;
                    }
                    double X = (form1.sf.sc.X0 + j * form1.rastersize);
                    double Y = (form1.sf.sc.Y0 + (surface.Height - i) * form1.rastersize);
                    double Z = ((double)(surface.Data[i, j, 0])) / 1000;

                    xh = Xo - X;
                    yh = Yo - Y;
                    S = Math.Sqrt(xh * xh + yh * yh);
                    s1 = (Xo - X) * S / xh;
                    s2 = (Yo - Y) * S / yh;
                    if (s1 > s2)
                    {
                        se = s2;
                    }
                    else se = s1;
                    double dx = (xh * sv) / S;
                    double dy = (yh * sv) / S;
                    double dz = (Zo - Z) * sv / S;
                    x = X;
                    y = Y;
                    z = Z;
                    sw = 0;

                    sh = 0;
                    while (sh <= se)
                    {
                        sh += sv;
                        x += dx;
                        y += dy;
                        z += dz;

                        
                        ix = (int)((x - form1.sf.sc.X0) / form1.rastersize);
                        iy = (int)(surface.Height - (y - form1.sf.sc.Y0) / form1.rastersize);                        
                        if (iy >= 0 && iy < surface.Height && ix > 0 && ix < surface.Width && surface.Data[iy, ix, 0] / 1000 >= z)
                        {
                            sw = 1;
                        }
                    }
                    if (sw == 1)
                    {
                        visible.Data[i, j, 0] = 0;  //invisible
                    }

                }
            }           
            procbarvalue += (int)(100 / maxthreads);           
        }




        double cos(double deg) { return Math.Cos(deg); }
        double sin(double deg) { return Math.Sin(deg); }



      
        #region Matrix procedures

        public double[][] Transpose(double[][] matrix)
        {
            int w = matrix.Length;
            int h = matrix[0].Length;

            double[][] result = MatrixCreate(h, w);

            for (int i = 0 ; i < w ; i++)
            {
                for (int j = 0 ; j < h ; j++)
                {
                    result[j][i] = matrix[i][j];
                }
            }

            return result;
        }
        static double[][] MatrixCreate(int rows, int cols)
        {
            double[][] result = new double[rows][];
            for (int i = 0 ; i < rows ; ++i)
                result[i] = new double[cols];
            return result;
        }

        static double[][] MatrixIdentity(int n)
        {
            
            double[][] result = MatrixCreate(n, n);
            for (int i = 0 ; i < n ; ++i)
                result[i][i] = 1.0;

            return result;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        static double[][] MatrixProduct(double[][] matrixA, double[][] matrixB)
        {
            int aRows = matrixA.Length; int aCols = matrixA[0].Length;
            int bRows = matrixB.Length; int bCols = matrixB[0].Length;
            if (aCols != bRows)
                throw new Exception("Non-conformable matrices in MatrixProduct");

            double[][] result = MatrixCreate(aRows, bCols);

            for (int i = 0 ; i < aRows ; ++i) // each row of A
                for (int j = 0 ; j < bCols ; ++j) // each col of B
                    for (int k = 0 ; k < aCols ; ++k) // could use k less-than bRows
                        result[i][j] += matrixA[i][k] * matrixB[k][j];

            return result;
        }

        static double[][] MatrixInverse(double[][] matrix)
        {
            int n = matrix.Length;
            double[][] result = MatrixDuplicate(matrix);

            int[] perm;
            int toggle;
            double[][] lum = MatrixDecompose(matrix, out perm,
              out toggle);
            if (lum == null)
                throw new Exception("Unable to compute inverse");

            double[] b = new double[n];
            for (int i = 0 ; i < n ; ++i)
            {
                for (int j = 0 ; j < n ; ++j)
                {
                    if (i == perm[j])
                        b[j] = 1.0;
                    else
                        b[j] = 0.0;
                }

                double[] x = HelperSolve(lum, b);

                for (int j = 0 ; j < n ; ++j)
                    result[j][i] = x[j];
            }
            return result;
        }

        static double[][] MatrixDuplicate(double[][] matrix)
        {
            // allocates/creates a duplicate of a matrix.
            double[][] result = MatrixCreate(matrix.Length, matrix[0].Length);
            for (int i = 0 ; i < matrix.Length ; ++i) // copy the values
                for (int j = 0 ; j < matrix[i].Length ; ++j)
                    result[i][j] = matrix[i][j];
            return result;
        }

        static double[] HelperSolve(double[][] luMatrix, double[] b)
        {
            // before calling this helper, permute b using the perm array
            // from MatrixDecompose that generated luMatrix
            int n = luMatrix.Length;
            double[] x = new double[n];
            b.CopyTo(x, 0);

            for (int i = 1 ; i < n ; ++i)
            {
                double sum = x[i];
                for (int j = 0 ; j < i ; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum;
            }

            x[n - 1] /= luMatrix[n - 1][n - 1];
            for (int i = n - 2 ; i >= 0 ; --i)
            {
                double sum = x[i];
                for (int j = i + 1 ; j < n ; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum / luMatrix[i][i];
            }

            return x;
        }

        static double[][] MatrixDecompose(double[][] matrix, out int[] perm, out int toggle)
        {
            // Doolittle LUP decomposition with partial pivoting.
            // rerturns: result is L (with 1s on diagonal) and U;
            // perm holds row permutations; toggle is +1 or -1 (even or odd)
            int rows = matrix.Length;
            int cols = matrix[0].Length; // assume square
            if (rows != cols)
                throw new Exception("Attempt to decompose a non-square m");

            int n = rows; // convenience

            double[][] result = MatrixDuplicate(matrix);

            perm = new int[n]; // set up row permutation result
            for (int i = 0 ; i < n ; ++i) { perm[i] = i; }

            toggle = 1; // toggle tracks row swaps.
                        // +1 -greater-than even, -1 -greater-than odd. used by MatrixDeterminant

            for (int j = 0 ; j < n - 1 ; ++j) // each column
            {
                double colMax = Math.Abs(result[j][j]); // find largest val in col
                int pRow = j;
              

                // reader Matt V needed this:
                for (int i = j + 1 ; i < n ; ++i)
                {
                    if (Math.Abs(result[i][j]) > colMax)
                    {
                        colMax = Math.Abs(result[i][j]);
                        pRow = i;
                    }
                }
                // Not sure if this approach is needed always, or not.

                if (pRow != j) // if largest value not on pivot, swap rows
                {
                    double[] rowPtr = result[pRow];
                    result[pRow] = result[j];
                    result[j] = rowPtr;

                    int tmp = perm[pRow]; // and swap perm info
                    perm[pRow] = perm[j];
                    perm[j] = tmp;

                    toggle = -toggle; // adjust the row-swap toggle
                }

            

                if (result[j][j] == 0.0)
                {
                    // find a good row to swap
                    int goodRow = -1;
                    for (int row = j + 1 ; row < n ; ++row)
                    {
                        if (result[row][j] != 0.0)
                            goodRow = row;
                    }

                    if (goodRow == -1)
                        throw new Exception("Cannot use Doolittle's method");

                    // swap rows so 0.0 no longer on diagonal
                    double[] rowPtr = result[goodRow];
                    result[goodRow] = result[j];
                    result[j] = rowPtr;

                    int tmp = perm[goodRow]; // and swap perm info
                    perm[goodRow] = perm[j];
                    perm[j] = tmp;

                    toggle = -toggle; // adjust the row-swap toggle
                }
            

                for (int i = j + 1 ; i < n ; ++i)
                {
                    result[i][j] /= result[j][j];
                    for (int k = j + 1 ; k < n ; ++k)
                    {
                        result[i][k] -= result[i][j] * result[j][k];
                    }
                }


            } // main j column loop

            return result;
        }

    
        #endregion






    }//class
}//namespace

