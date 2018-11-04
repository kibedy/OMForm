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


        public ImageProcess(Form1 form1, string filename, List<Marker> markers)
        {
            InitializeComponent();
            this.filename = filename;
            this.markers = markers;
            Image<Bgr, byte> photo = new Image<Bgr, byte>(form1.SavePath + "\\"+filename);
            this.pictureBox1.Image = photo.ToBitmap();
            this.pictureBox1.Cursor = Cursors.Hand;

            ImageWidth = pictureBox1.Image.Width;
            ImageHeight = pictureBox1.Image.Height;
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBox1.Size = new Size(ImageWidth, ImageHeight);

            /*PictureBox pictureBoxOverlay = new PictureBox();
            pictureBoxOverlay.BackColor = Color.Transparent;
            pictureBoxOverlay.Location = new Point(0, 0);
            pictureBoxOverlay.Parent = pictureBox1;
            */
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.Columns.Add("Id", 28, HorizontalAlignment.Left);
            this.listView1.Columns.Add("X", 55, HorizontalAlignment.Left);
            this.listView1.Columns.Add("Y", 55, HorizontalAlignment.Left);

        }





        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            const float scale_per_delta = 0.1f / 120;
            ImageScale += e.Delta * scale_per_delta;
            if (ImageScale < 0) ImageScale = 0;
            this.pictureBox1.Size = new Size(
                (int)(ImageWidth * ImageScale),
                (int)(ImageHeight * ImageScale));
            if ((pictureBox1.Image.Width > this.ClientSize.Width || pictureBox1.Image.Height > this.ClientSize.Height))
            {
                panel1.AutoScrollPosition = new Point(-panel1.AutoScrollPosition.X + (e.X),-panel1.AutoScrollPosition.Y + (e.Y));
                ;
            }

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _mousePt = e.Location;
                _tracking = true;
                ;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_tracking &&
             (pictureBox1.Image.Width > this.ClientSize.Width ||
             pictureBox1.Image.Height > this.ClientSize.Height))
            {
                panel1.AutoScrollPosition = new Point(-panel1.AutoScrollPosition.X + (_mousePt.X - e.X),
                 -panel1.AutoScrollPosition.Y + (_mousePt.Y - e.Y));
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            _tracking = false;
        }

        private void panel1_MouseWheel(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void addMarker(object sender, MouseEventArgs e)
        {
            //string tablaformazas = "{0,-10}{1,-20}";
            Point pp = e.Location;
            markers.Add(new Marker(pp.X / ImageScale, pp.Y / ImageScale, filename));
            //string listaelem = (pp.X / ImageScale).ToString() + ", " + (pp.Y / ImageScale).ToString();

           
            ;

            var item = new ListViewItem(new[] {(markers.Count-1).ToString() ,(pp.X / ImageScale).ToString(), (pp.Y / ImageScale).ToString() });

            this.listView1.Items.Add(item);


            ;

        }

      
        


        /* private void pictureBox1_Click(object sender, EventArgs e)
         {
             ;
         }*/
    }
}
