using Emgu.CV;
using Emgu.CV.Structure;
using OM_Form;
using OrthoMachine.Model;
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
        private int ImageWidth, ImageHeight;
        private float ImageScale = 1.0f;
        Point _mousePt = new Point();
        bool _tracking = false;
        List<Marker> markers;
        Form1 form1;
        bool EnablePickpoints;
        //bool EnablePictureBox1Functions;
        Image<Gray, ushort> surface;


        public ImageProcess(Form1 form1, string filename, List<Marker> markers)
        {
            InitializeComponent();
            this.filename = filename;
            this.markers = markers;
            this.form1 = form1;
            Image<Bgr, byte> photo = new Image<Bgr, byte>(form1.SavePath + "\\photos\\"+filename);
            this.pictureBox1.Image = photo.ToBitmap();
            this.pictureBox1.Cursor = Cursors.Hand;
            
            //EnablePictureBox1Functions = true;

            ImageWidth = pictureBox1.Image.Width;
            ImageHeight = pictureBox1.Image.Height;
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox1.Size = new Size(ImageWidth, ImageHeight);
            EnablePickpoints = false;
            /*PictureBox pictureBoxOverlay = new PictureBox();
            pictureBoxOverlay.BackColor = Color.Transparent;
            pictureBoxOverlay.Location = new Point(0, 0);
            pictureBoxOverlay.Parent = pictureBox1;
            */
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.Columns.Add("Point Id", 28, HorizontalAlignment.Left);
            this.listView1.Columns.Add("Image X", 55, HorizontalAlignment.Left);
            this.listView1.Columns.Add("Image Y", 55, HorizontalAlignment.Left);
            this.listView2.View = System.Windows.Forms.View.Details;
            this.listView2.Columns.Add("Point Id", 28, HorizontalAlignment.Left);
            this.listView2.Columns.Add("Global X", 55, HorizontalAlignment.Left);
            this.listView2.Columns.Add("Global Y", 55, HorizontalAlignment.Left);
            this.listView2.Columns.Add("Global Z", 55, HorizontalAlignment.Left);

        }



        #region PictureBox_1_Events
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            const float scale_per_delta = 0.1f / 120;
            ImageScale += e.Delta * scale_per_delta;
            
            
                if (ImageScale < 0) ImageScale = 0;
                this.pictureBox1.Size = new Size((int)(ImageWidth * ImageScale), (int)(ImageHeight * ImageScale));
                if ((pictureBox1.Image.Width > this.ClientSize.Width || pictureBox1.Image.Height > this.ClientSize.Height))
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
            if (_tracking  && (pictureBox1.Image.Width > this.ClientSize.Width || pictureBox1.Image.Height > this.ClientSize.Height))
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


        private void panel1_MouseWheel(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void orientateToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            EnablePickpoints = true;
            panel1.Height -= 80;
            panel1.Size = new Size(this.Width / 2-2, panel1.Height);
            //panel2.Size = new Size(this.Width / 2, panel1.Height);
            this.listView1.Visible = true;
            this.listView2.Visible = true;
            panel2.Location= new Point( panel1.Location.X+panel1.Width+2, panel1.Location.Y) ;
            panel2.Width = panel1.Width-4;
           
            surface = new Image<Gray, ushort>(form1.SavePath + "\\" + "surface.png");
            this.pictureBox2.Image = surface.ToBitmap();
            this.pictureBox2.Cursor = Cursors.Hand;
            this.pictureBox2.Visible = true;
            this.panel2.Visible = true;
            this.SizeChanged += new System.EventHandler(this.ImageProcess_SizeChanged);
            this.buttonPhotoUp.Visible = true;
            this.buttonPhotoDown.Visible = true;
            this.buttonPhotoDel.Visible = true;
            this.buttonSurfaceDel.Visible = true;
            this.buttonSurfaceUp.Visible = true;
            this.buttonSurfaceDown.Visible = true;

        }

        private void ImageProcess_SizeChanged(object sender, EventArgs e)
        {
            this.panel1.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            this.panel2.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            panel1.Size = new Size(this.Width / 2 - 10, this.Height-160);
            panel2.Size = new Size(this.Width / 2 - 10, panel1.Height);
            panel2.Location = new Point(panel1.Location.X + panel1.Width + 2, panel1.Location.Y);
        }

        private void addMarkerPhoto(object sender, MouseEventArgs e)
        {   if (EnablePickpoints)
            {
                Point pp = e.Location;
                //markers.Add(new Marker(pp.X / ImageScale, pp.Y / ImageScale, filename));
                var item = new ListViewItem(new[] { listView1.Items.Count.ToString(), (pp.X / ImageScale).ToString("0.00"), (pp.Y / ImageScale).ToString("0.00") });
                this.listView1.Items.Add(item);
            }
        }

        private void buttonPhotoUp_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count>2)
            {
                /*int index = listView1.SelectedIndices[0];
                index--;
                this.listView1.Items[index].Selected = true;
                listView1.Refresh();*/
                ListViewItem item = listView1.SelectedItems[0];
                int index = listView1.SelectedItems[0].Index;
                ListViewItem itemUpper = listView1.Items[index-1];
                listView1.Items.RemoveAt(item.Index - 1);
                listView1.Items.RemoveAt(item.Index);
                listView1.Items.Insert(index - 1, item);
                listView1.Items.Insert(index,itemUpper);                
                //listView1.Items[item.Index-1] = item;
                listView1.Refresh();
                //TODO change ID number;
            }
        }

        private void addMarkerSurface(object sender, MouseEventArgs e)
        {
            if (EnablePickpoints)
            {
                Point pp = e.Location;
                //markers.Add(new Marker(pp.X / ImageScale, pp.Y / ImageScale, filename));
                int xx = (int)(pp.X / ImageScale);
                int yy = (int)(pp.Y / ImageScale);
                var item = new ListViewItem(new[] { (listView2.Items.Count.ToString()).ToString(), xx.ToString("0"),(surface.Data[yy,xx,0]).ToString()  ,yy.ToString("0") });
                this.listView2.Items.Add(item);
            }
            
            
        }




        /* private void pictureBox1_Click(object sender, EventArgs e)
         {
             ;
         }*/
    }
}
