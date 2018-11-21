using Emgu.CV;
using Emgu.CV.Structure;
using OM_Form;
using OrthoMachine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrthoMachine.View
{
    public partial class ImageProcess : Form
    {
        string filename;
        private int ImageWidth, ImageHeight, ImageWidthS, ImageHeightS;
        private float ImageScale = 1.0f;
        private float ImageScaleS = 1.0f;
        Point _mousePt = new Point();
        bool _tracking = false;
        List<Marker> markers;
        Form1 form1;
        bool EnablePickpoints;
        //bool EnablePictureBox1Functions;
        Image<Gray, ushort> surface;
        Orientation orientation;
        Point pp;
        //Image<Bgra, byte> markerimage;
        Image<Gray, byte> markerimage;
        Image<Bgra, byte> photo;
        Image<Bgra, byte> photo_orig;
        Image<Bgra, byte> rgbsurf;
        ArrayList colors;
        enum ShowState { rgb, intensity, depth, photo };
        ShowState SSS;





        public ImageProcess(Form1 form1, string filename, Orientation orientation)
        {
            InitializeComponent();
            this.filename = filename;
            this.orientation = orientation;
            this.form1 = form1;
            photo = new Image<Bgra, byte>(form1.SavePath + "\\photos\\" + filename);
            photo_orig = photo.Clone();
            this.pictureBox1.Image = photo.ToBitmap();
            ImageWidth = pictureBox1.Image.Width;
            ImageHeight = pictureBox1.Image.Height;
            //this.pictureBox1.BackgroundImage = photo.ToBitmap();
            //ImageWidth = pictureBox1.BackgroundImage.Width;
            //ImageHeight = pictureBox1.BackgroundImage.Height;
            colors = CreateColorList();
            //this.FormClosing + = ImageProcess_Closing;
            this.FormClosing += ImageProcess_FormClosing;
            SSS = ShowState.depth;


            if (form1.filetype == 7)
            {
                rGBToolStripMenuItem.Visible = true;
                intensityToolStripMenuItem.Visible = true;
                depthToolStripMenuItem.Visible = true;
            }
            else if (form1.filetype == 4)
            {
                intensityToolStripMenuItem.Visible = true;
                depthToolStripMenuItem.Visible = true;
                rGBToolStripMenuItem.Visible = false;
            }
            else
            {
                intensityToolStripMenuItem.Visible = false;
                depthToolStripMenuItem.Visible = false;
                rGBToolStripMenuItem.Visible = false;
            }

            //markerimage = new Image<Gray, byte>(new Size(ImageWidth, ImageHeight));
            ;
            //markerimage.ro
            //photo.Mul()

            //this.pictureBox1.Cursor = Cursors.Hand;

            //EnablePictureBox1Functions = true;


            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox1.Size = new Size(ImageWidth, ImageHeight);
            EnablePickpoints = false;
            /*PictureBox pictureBoxOverlay = new PictureBox();
            pictureBoxOverlay.BackColor = Color.Transparent;
            pictureBoxOverlay.Location = new Point(0, 0);
            pictureBoxOverlay.Parent = pictureBox1;
            */
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.Columns.Add("Point Id", 80, HorizontalAlignment.Left);
            this.listView1.Columns.Add("Image X", 80, HorizontalAlignment.Left);
            this.listView1.Columns.Add("Image Y", 80, HorizontalAlignment.Left);
            this.listView2.View = System.Windows.Forms.View.Details;
            this.listView2.Columns.Add("Point Id", 80, HorizontalAlignment.Left);
            this.listView2.Columns.Add("Global X", 80, HorizontalAlignment.Left);
            this.listView2.Columns.Add("Global Z", 80, HorizontalAlignment.Left);
            this.listView2.Columns.Add("Global Y", 80, HorizontalAlignment.Left);

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
            DrawMarkers(listView1, pictureBox1, photo.ToBitmap());
            listView2.Refresh();
            //SSS = ShowState.depth;
            //DrawMarkers(listView2, pictureBox2,form1.sf.sc.image.ToBitmap());
            SetOrientationForm();
        }

        private void ImageProcess_FormClosing(object sender, FormClosingEventArgs e)
        {
            string[] fn = filename.Split('.');
            StreamWriter sw = new StreamWriter(form1.SavePath + "\\photos\\" + fn[0] + ".ori");
            //throw new NotImplementedException();
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


        #region PictureBox_1_Events
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            const float scale_per_delta = 0.1f / 120;
            ImageScale += e.Delta * scale_per_delta;


            if (ImageScale < 0) ImageScale = 0;
            this.pictureBox1.Size = new Size((int)(ImageWidth * ImageScale), (int)(ImageHeight * ImageScale));
            if ((pictureBox1.Image.Width > this.ClientSize.Width || pictureBox1.Image.Height > this.ClientSize.Height))
            //if ((pictureBox1.BackgroundImage.Width > this.ClientSize.Width || pictureBox1.BackgroundImage.Height > this.ClientSize.Height))
            {
                panel1.AutoScrollPosition = new Point(-panel1.AutoScrollPosition.X + (e.X), -panel1.AutoScrollPosition.Y + (e.Y));
            }

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
            const float scale_per_delta = 0.1f / 120;
            ImageScaleS += e.Delta * scale_per_delta;


            if (ImageScaleS < 0) ImageScaleS = 0;
            this.pictureBox2.Size = new Size((int)(ImageWidthS * ImageScaleS), (int)(ImageHeightS * ImageScaleS));
            Console.WriteLine(this.pictureBox2.Size.Width + " " + this.pictureBox2.Size.Height + " kell_width:" + ImageWidthS * ImageScaleS + " kellHeight:" + ImageHeightS * ImageScaleS);
            if ((pictureBox2.Image.Width > this.ClientSize.Width || pictureBox2.Image.Height > this.ClientSize.Height))
            {
                panel2.AutoScrollPosition = new Point(-panel2.AutoScrollPosition.X + (e.X), -panel2.AutoScrollPosition.Y + (e.Y));
            }

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

                //rgbsurf= new Image<Bgra, byte>(form1.SavePath +"\\surface_rgb.png)");
                //pictureBox2.Image = form1.sf.sc.RGBsurfImage.ToBitmap();
                SSS = ShowState.rgb;
                DrawMarkers(listView2, pictureBox2, form1.sf.sc.RGBsurfImage.ToBitmap());

                //this.pictureBox2.Image=
            }
            else if (e.ClickedItem.Name == "intensityToolStripMenuItem")
            {
                SSS = ShowState.intensity;
                //pictureBox2.Image = form1.sf.sc.intSurfImage.ToBitmap();
                DrawMarkers(listView2, pictureBox2, form1.sf.sc.intSurfImage.ToBitmap());
            }
            else
            {
                SSS = ShowState.depth;
                //pictureBox2.Image = form1.sf.sc.image.ToBitmap();
                DrawMarkers(listView2, pictureBox2, form1.sf.sc.image.ToBitmap());
            }
        }

        private void panel1_MouseWheel(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SetOrientationForm()
        {
            EnablePickpoints = true;
            buttonCalculate.Visible = true;
            //panel1.Height -= 140;
            panel1.Height = this.Height - 200;
            panel1.Size = new Size(this.Width / 2 - 2, panel1.Height);
            //panel2.Size = new Size(this.Width / 2, panel1.Height);
            this.listView1.Visible = true;
            this.listView2.Visible = true;
            panel2.Location = new Point(panel1.Location.X + panel1.Width + 2, panel1.Location.Y);
            panel2.Width = panel1.Width - 4;
            panel2.Height = panel1.Height;


            //surface = new Image<Gray, ushort>(form1.SavePath + "\\" + "surface.png");
            surface = form1.sf.sc.image;
            SSS = ShowState.depth;
            this.pictureBox2.Visible = true;
            this.panel2.Visible = true;
            //this.pictureBox2.Image = surface.ToBitmap();
            this.pictureBox2.Cursor = Cursors.Hand;
            ImageWidthS = surface.Bitmap.Width;
            //ImageHeightS = pictureBox2.Image.Height;
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
            //listView1.Refresh();

            //DrawMarkers(listView2, pictureBox2, surface.Bitmap);
            //DrawMarkers(listView1, pictureBox1, photo.ToBitmap());




        }

        private void orientateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetOrientationForm();
            /*
            EnablePickpoints = true;
            buttonCalculate.Visible = true;
            //panel1.Height -= 140;
            panel1.Height = this.Height - 200;
            panel1.Size = new Size(this.Width / 2 - 2, panel1.Height);
            //panel2.Size = new Size(this.Width / 2, panel1.Height);
            this.listView1.Visible = true;
            this.listView2.Visible = true;
            panel2.Location = new Point(panel1.Location.X + panel1.Width + 2, panel1.Location.Y);
            panel2.Width = panel1.Width - 4;
            panel2.Height = panel1.Height;


            //surface = new Image<Gray, ushort>(form1.SavePath + "\\" + "surface.png");
            surface = form1.sf.sc.image;
            SSS = ShowState.depth;
            this.pictureBox2.Visible = true;
            this.panel2.Visible = true;
            this.pictureBox2.Image = surface.ToBitmap();
            this.pictureBox2.Cursor = Cursors.Hand;
            ImageWidthS = pictureBox2.Image.Width;
            ImageHeightS = pictureBox2.Image.Height;
            this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox2.Size = new Size(ImageWidthS, ImageHeightS);
            this.SizeChanged += new System.EventHandler(this.ImageProcess_SizeChanged);
            this.buttonPhotoUp.Visible = true;
            this.buttonPhotoDown.Visible = true;
            this.buttonPhotoDel.Visible = true;
            this.buttonSurfaceDel.Visible = true;
            this.buttonSurfaceUp.Visible = true;
            this.buttonSurfaceDown.Visible = true;
            */

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
            /*PictureBox photomarker = new PictureBox();           
            photomarker.Location = new System.Drawing.Point(0, 0);
            photomarker.Name = "pictureBox1";
            photomarker.Size = new System.Drawing.Size(123, 71);
            photomarker.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            photomarker.TabIndex = 0;
            photomarker.BackColor = Color.Transparent;
            this.panel1.Controls.Add(photomarker);
            Graphics g = panel1.CreateGraphics();
            Pen p = new Pen(Color.Red);
            SolidBrush sb = new SolidBrush(Color.Red);
            g.DrawEllipse(p, pp.X, pp.X, 30, 30);
            //g.FillEllipse(sb,pp.X, pp.Y, 6, 6);
            //pictureBox1.SendToBack();
            //pictureBox1.Refresh();
            */

        }

        private void addMarkerSurface(object sender, MouseEventArgs e)
        {
            if (EnablePickpoints)
            {
                Point pp = e.Location;
                //markers.Add(new Marker(pp.X / ImageScale, pp.Y / ImageScale, filename));
                int xx = (int)(pp.X / ImageScaleS);
                int yy = (int)(pp.Y / ImageScaleS);
                var item = new ListViewItem(new[] { (listView2.Items.Count.ToString()).ToString(), xx.ToString("0"), yy.ToString("0"), (surface.Data[yy, xx, 0]).ToString() });
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
                DrawMarkers(listView2, pictureBox2, new Bitmap(pictureBox2.Image));
            }
        }

        private void addMarkerPhoto(object sender, MouseEventArgs e)
        {
            if (EnablePickpoints)
            {
                pp = e.Location;
                var item = new ListViewItem(new[] { listView1.Items.Count.ToString(), (pp.X / ImageScale).ToString("0.00"), (pp.Y / ImageScale).ToString("0.00") });

                if (listView1.Items.Count < 20)
                {
                    this.listView1.Items.Add(item);
                }
                else
                {
                    MessageBox.Show("Too many markers!");
                }

                //DrawMarkers(listView1, pictureBox1, new Bitmap(pictureBox1.Image));
                DrawMarkers(listView1, pictureBox1, new Bitmap(photo.ToBitmap()));

                //DrawMarkers(listView1, pictureBox1, new Bitmap(pictureBox1.BackgroundImage));
                panel1.Invalidate();
            }
        }



        private void DrawMarkers(ListView list, PictureBox pbox, Image source)
        {
            //markerimage = new Image<Gray, byte>(new Size(source.Width, source.Height));
            //markerimage = new Image<Bgra, byte>(new Size(source.Width, source.Height));
            Image<Bgra, byte> toDraw = new Image<Bgra, byte>((Bitmap)source);
            Image<Bgra, byte> temp = (toDraw.Clone());
            if (list.Items.Count != 0)
            {
                ListViewItem[] items = new ListViewItem[list.Items.Count];
                list.Items.CopyTo(items, 0);
                int i = 0;
                foreach (ListViewItem item in list.Items)
                {
                    markerimage = new Image<Gray, byte>(new Size(source.Width, source.Height));

                    float X = float.Parse(item.SubItems[1].Text);
                    float Y = float.Parse(item.SubItems[2].Text);
                    PointF center = new PointF(X, Y);
                    float r = 10;
                    CircleF circle1 = new CircleF(center, r);
                    CircleF circle2 = new CircleF(center, 1);
                    markerimage.Draw(circle1, new Gray(250), 1);
                    //markerimage.Draw(circle1, (Bgra)colors[0], i);
                    //markerimage.Draw(circle2, (Bgra)colors[0], i);
                    //markerimage.Draw(circle2, (Bgra)colors[0], i);
                    //i++;
                    markerimage.Draw(circle2, new Gray(250), 1);

                    markerimage._SmoothGaussian(3, 3, 1, 1);
                    temp.SetValue((Bgra)colors[i], markerimage);
                    i++;
                    GC.Collect();

                }
                pbox.Image = temp.Bitmap;
            }
            else
            {
                pbox.Image = source;
            }
            GC.Collect();
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
                        DrawMarkers(listView2, pictureBox2, form1.sf.sc.image.ToBitmap());
                    }
                    else if (SSS == ShowState.rgb)
                    {
                        DrawMarkers(listView2, pictureBox2, form1.sf.sc.RGBsurfImage.ToBitmap());
                    }
                    else
                    {
                        DrawMarkers(listView2, pictureBox2, form1.sf.sc.intSurfImage.ToBitmap());
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
                    if (SSS == ShowState.depth)
                    {
                        DrawMarkers(listView2, pictureBox2, form1.sf.sc.image.ToBitmap());
                    }
                    else if (SSS == ShowState.rgb)
                    {
                        DrawMarkers(listView2, pictureBox2, form1.sf.sc.RGBsurfImage.ToBitmap());
                    }
                    else
                    {
                        DrawMarkers(listView2, pictureBox2, form1.sf.sc.intSurfImage.ToBitmap());
                    }
                }
            }
            catch { }
        }

        private void DelMarker(ListView listview, PictureBox picturebox, ShowState SSS)
        {
            if (listview.Items.Count >= 1)
            {
                ListViewItem item = listview.SelectedItems[0];
                int index = listview.SelectedItems[0].Index;
                listview.Items.RemoveAt(item.Index);
                ListViewItem lowerItem;
                if (index != (listview.Items.Count) && listview.Items.Count != 0)
                {
                    for (int i = index; i < listview.Items.Count; i++)
                    {
                        lowerItem = listview.Items[i];
                        lowerItem.Text = (i).ToString();
                    }
                }

            }
            listview.Refresh();
            if (SSS == ShowState.depth)
            {
                DrawMarkers(listview, picturebox, form1.sf.sc.image.ToBitmap());
            }
            else if (SSS == ShowState.rgb)
            {
                DrawMarkers(listview, picturebox, form1.sf.sc.RGBsurfImage.ToBitmap());
            }
            else if (SSS == ShowState.intensity)
            {
                DrawMarkers(listview, picturebox, form1.sf.sc.intSurfImage.ToBitmap());
            }
            else if (SSS == ShowState.photo)
            {
                DrawMarkers(listview, picturebox, photo.ToBitmap());
            }

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

        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            initOrientation(listView1, listView2);
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
                DrawMarkers(listView1, pictureBox1, photo.ToBitmap());
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
                DrawMarkers(listView1, pictureBox1, photo.ToBitmap());
            }
        }



        /* private void pictureBox1_Click(object sender, EventArgs e)
         {
             ;
         }*/

        //orientation
        //initialization

        private void initOrientation(ListView listView1, ListView listView2)
        {
            double Xo, Yo, Zo; //init pic postition
            double o = 0;
            double p = 0;
            double k = 0;    //omega phi kappa
            double sumX = 0;
            double sumY = 0;
            double sumZ = 0;
            double c = 28;
            foreach (ListViewItem item in listView2.Items)
            {
                sumX += double.Parse(item.SubItems[1].Text);
                sumY += double.Parse(item.SubItems[3].Text);
                sumZ += double.Parse(item.SubItems[2].Text);
            }
            Xo = sumX / listView2.Items.Count;
            Yo = sumY / listView2.Items.Count - 20;  //start position in negative 
            Zo = sumZ / listView2.Items.Count;

            int iter = 0;
            double var2 = 0.01;
            double var3 = 0.01;
            double a11, a12, a13, a21, a22, a23, a31, a32, a33;
            int pointpaircount = Math.Min(listView1.Items.Count, listView2.Items.Count);
            double[] DX = new double[pointpaircount];
            double[] DY = new double[pointpaircount];
            double[] DZ = new double[pointpaircount];
            double[] x = new double[pointpaircount];
            double[] y = new double[pointpaircount];
            double[] xo = new double[pointpaircount * 2];

            //int i = 0;
            //foreach (ListViewItem item in listView1.Items)
            for (int j = 0; j < pointpaircount; j++)
            {
                ListViewItem item = listView1.Items[j];
                x[j] = double.Parse(item.SubItems[1].Text) - Xo;
                y[j] = double.Parse(item.SubItems[2].Text) - Yo;
            }
            int j = 0;

            while (iter < 20)
            {
                a11 = cos(p) * cos(k);
                a12 = -cos(p) * sin(k);
                a13 = sin(p);
                a21 = cos(o) * sin(k) + sin(o) * sin(p) * cos(k);
                a22 = cos(o) * cos(k) - sin(o) * sin(p) * sin(k);
                a23 = -sin(o) * cos(p);
                a31 = sin(o) * sin(k) - cos(o) * sin(p) * cos(k);
                a32 = sin(o) * cos(k) + cos(o) * sin(p) * sin(k);
                a33 = cos(o) * cos(p);
                int i = 0;
                foreach (ListViewItem item in listView2.Items)
                {
                    DX[i] = double.Parse(item.SubItems[1].Text) - Xo;
                    DY[i] = double.Parse(item.SubItems[3].Text) - Yo;
                    DZ[i] = double.Parse(item.SubItems[2].Text) - Zo;
                    xo[2 * i] = c * (a11 * DX[i] + a21 * DY[i] + a31 * DZ[i]) / (a13 * DX[i] + a23 * DY[i] + a33 * DZ[i]);
                    xo[2 * i + 1] = c * (a12 * DX[i] + a22 * DY[i] + a32 * DZ[i]) / (a13 * DX[i] + a23 * DY[i] + a33 * DZ[i]);
                    //lm[i] = DZ[i] / (a31 * x[i] + a32 * y[i] + a33 * c);
                    i++;
                }
                //TO DO Folytatni

            }



        }

        double cos(double deg) { return Math.Cos(deg); }
        double sin(double deg) { return Math.Sin(deg); }


    }//class
}//namespace

