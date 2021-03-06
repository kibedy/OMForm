﻿using OM_Form;
using OrthoMachine.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrthoMachine.View
{
    public partial class Ortho: Form
    {
        Orthophoto orthophoto;
        private float ImageScale = 1.0f;
        private int ImageWidth, ImageHeight;
        Point _mousePt = new Point();
        bool _tracking = false;
        Form1 form1;
        List<string> orthofilenames;
        List<string> orthothumbfilenames;

        public Ortho(Form1 form1)
        {
            InitializeComponent();
            orthophoto = new Orthophoto(form1, this);
            orthophoto.SilentLoadOrthothumbs();
            ImageWidth = 1;
            ImageHeight = 1;
            this.form1 = form1;


        }

        private void Ortho_Load(object sender, EventArgs e)
        {
         
        }

       


        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 2)
            {
                listView1.SelectedItems[0].Selected = false;                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            orthophoto.Overlap();
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            const float scale_per_delta = 0.1f / 120;
            ImageScale += e.Delta * scale_per_delta;
            if (ImageScale < 0.1) ImageScale = 0.1f;            
            this.pictureBox1.Size = new Size((int)(form1.sf.sc.image.Width * ImageScale), (int)(form1.sf.sc.image.Height * ImageScale));           
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


    }
}
