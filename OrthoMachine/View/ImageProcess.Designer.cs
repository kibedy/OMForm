﻿using System;
using System.Windows.Forms;

namespace OrthoMachine.View
{
    partial class ImageProcess
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.orientateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listView2 = new System.Windows.Forms.ListView();
            this.buttonPhotoDown = new System.Windows.Forms.Button();
            this.buttonPhotoDel = new System.Windows.Forms.Button();
            this.buttonPhotoUp = new System.Windows.Forms.Button();
            this.buttonSurfaceDel = new System.Windows.Forms.Button();
            this.buttonSurfaceUp = new System.Windows.Forms.Button();
            this.buttonSurfaceDown = new System.Windows.Forms.Button();
            this.buttonCalculate = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.depthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.intensityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rGBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.orthoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(4, 27);
            this.panel1.MinimumSize = new System.Drawing.Size(100, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(930, 470);
            this.panel1.TabIndex = 3;
            this.panel1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseWheel);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(123, 71);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.addMarkerPhoto);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseEnter += new System.EventHandler(this.pictureBox1_MouseEnter);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoScroll = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.pictureBox2);
            this.panel2.Location = new System.Drawing.Point(888, 27);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(46, 401);
            this.panel2.TabIndex = 6;
            this.panel2.Visible = false;
            this.panel2.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseWheel);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox2.Location = new System.Drawing.Point(0, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(41, 50);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Visible = false;
            this.pictureBox2.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.addMarkerSurface);
            this.pictureBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseDown);
            this.pictureBox2.MouseEnter += new System.EventHandler(this.pictureBox2_MouseEnter);
            this.pictureBox2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseMove);
            this.pictureBox2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseUp);
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(4, 395);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(360, 102);
            this.listView1.TabIndex = 4;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.Visible = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.orientateToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(940, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // orientateToolStripMenuItem
            // 
            this.orientateToolStripMenuItem.Name = "orientateToolStripMenuItem";
            this.orientateToolStripMenuItem.Size = new System.Drawing.Size(88, 20);
            this.orientateToolStripMenuItem.Text = "Save markers";
            this.orientateToolStripMenuItem.Click += new System.EventHandler(this.orientateToolStripMenuItem_Click);
            // 
            // listView2
            // 
            this.listView2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.listView2.GridLines = true;
            this.listView2.Location = new System.Drawing.Point(574, 395);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(360, 102);
            this.listView2.TabIndex = 7;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.Visible = false;
            // 
            // buttonPhotoDown
            // 
            this.buttonPhotoDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonPhotoDown.BackgroundImage = global::OrthoMachine.Properties.Resources.down_arrow;
            this.buttonPhotoDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonPhotoDown.Location = new System.Drawing.Point(370, 418);
            this.buttonPhotoDown.Name = "buttonPhotoDown";
            this.buttonPhotoDown.Size = new System.Drawing.Size(24, 23);
            this.buttonPhotoDown.TabIndex = 2;
            this.buttonPhotoDown.UseVisualStyleBackColor = true;
            this.buttonPhotoDown.Visible = false;
            this.buttonPhotoDown.Click += new System.EventHandler(this.buttonPhotoDown_Click);
            // 
            // buttonPhotoDel
            // 
            this.buttonPhotoDel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonPhotoDel.BackgroundImage = global::OrthoMachine.Properties.Resources.red_X;
            this.buttonPhotoDel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonPhotoDel.Location = new System.Drawing.Point(370, 441);
            this.buttonPhotoDel.Name = "buttonPhotoDel";
            this.buttonPhotoDel.Size = new System.Drawing.Size(24, 23);
            this.buttonPhotoDel.TabIndex = 8;
            this.buttonPhotoDel.UseVisualStyleBackColor = true;
            this.buttonPhotoDel.Visible = false;
            this.buttonPhotoDel.Click += new System.EventHandler(this.buttonPhotoDel_Click);
            // 
            // buttonPhotoUp
            // 
            this.buttonPhotoUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonPhotoUp.BackgroundImage = global::OrthoMachine.Properties.Resources.up_arrow;
            this.buttonPhotoUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonPhotoUp.Location = new System.Drawing.Point(370, 395);
            this.buttonPhotoUp.Name = "buttonPhotoUp";
            this.buttonPhotoUp.Size = new System.Drawing.Size(24, 23);
            this.buttonPhotoUp.TabIndex = 1;
            this.buttonPhotoUp.UseVisualStyleBackColor = true;
            this.buttonPhotoUp.Visible = false;
            this.buttonPhotoUp.Click += new System.EventHandler(this.buttonPhotoUp_Click);
            // 
            // buttonSurfaceDel
            // 
            this.buttonSurfaceDel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSurfaceDel.BackgroundImage = global::OrthoMachine.Properties.Resources.red_X;
            this.buttonSurfaceDel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonSurfaceDel.Location = new System.Drawing.Point(544, 441);
            this.buttonSurfaceDel.Name = "buttonSurfaceDel";
            this.buttonSurfaceDel.Size = new System.Drawing.Size(24, 23);
            this.buttonSurfaceDel.TabIndex = 11;
            this.buttonSurfaceDel.UseVisualStyleBackColor = true;
            this.buttonSurfaceDel.Visible = false;
            this.buttonSurfaceDel.Click += new System.EventHandler(this.buttonSurfaceDel_Click);
            // 
            // buttonSurfaceUp
            // 
            this.buttonSurfaceUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSurfaceUp.BackgroundImage = global::OrthoMachine.Properties.Resources.up_arrow;
            this.buttonSurfaceUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonSurfaceUp.Location = new System.Drawing.Point(544, 395);
            this.buttonSurfaceUp.Name = "buttonSurfaceUp";
            this.buttonSurfaceUp.Size = new System.Drawing.Size(24, 23);
            this.buttonSurfaceUp.TabIndex = 9;
            this.buttonSurfaceUp.UseVisualStyleBackColor = true;
            this.buttonSurfaceUp.Visible = false;
            this.buttonSurfaceUp.Click += new System.EventHandler(this.buttonSurfaceUp_Click);
            // 
            // buttonSurfaceDown
            // 
            this.buttonSurfaceDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSurfaceDown.BackgroundImage = global::OrthoMachine.Properties.Resources.down_arrow;
            this.buttonSurfaceDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonSurfaceDown.Location = new System.Drawing.Point(544, 418);
            this.buttonSurfaceDown.Name = "buttonSurfaceDown";
            this.buttonSurfaceDown.Size = new System.Drawing.Size(24, 23);
            this.buttonSurfaceDown.TabIndex = 10;
            this.buttonSurfaceDown.UseVisualStyleBackColor = true;
            this.buttonSurfaceDown.Visible = false;
            this.buttonSurfaceDown.Click += new System.EventHandler(this.buttonSurfaceDown_Click);
            // 
            // buttonCalculate
            // 
            this.buttonCalculate.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCalculate.AutoSize = true;
            this.buttonCalculate.Location = new System.Drawing.Point(412, 395);
            this.buttonCalculate.MaximumSize = new System.Drawing.Size(150, 45);
            this.buttonCalculate.MinimumSize = new System.Drawing.Size(113, 45);
            this.buttonCalculate.Name = "buttonCalculate";
            this.buttonCalculate.Size = new System.Drawing.Size(113, 45);
            this.buttonCalculate.TabIndex = 12;
            this.buttonCalculate.Text = "Calculate orientation";
            this.buttonCalculate.UseVisualStyleBackColor = true;
            this.buttonCalculate.Visible = false;
            this.buttonCalculate.Click += new System.EventHandler(this.buttonCalculate_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.depthToolStripMenuItem,
            this.intensityToolStripMenuItem,
            this.rGBToolStripMenuItem,
            this.orthoToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(120, 92);
            this.contextMenuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip1_ItemClicked);
            // 
            // depthToolStripMenuItem
            // 
            this.depthToolStripMenuItem.Name = "depthToolStripMenuItem";
            this.depthToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.depthToolStripMenuItem.Text = "Depth";
            // 
            // intensityToolStripMenuItem
            // 
            this.intensityToolStripMenuItem.Name = "intensityToolStripMenuItem";
            this.intensityToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.intensityToolStripMenuItem.Text = "Intensity";
            // 
            // rGBToolStripMenuItem
            // 
            this.rGBToolStripMenuItem.Name = "rGBToolStripMenuItem";
            this.rGBToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.rGBToolStripMenuItem.Text = "RGB";
            // 
            // orthoToolStripMenuItem
            // 
            this.orthoToolStripMenuItem.Name = "orthoToolStripMenuItem";
            this.orthoToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.orthoToolStripMenuItem.Text = "ortho";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(412, 481);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(113, 15);
            this.progressBar1.TabIndex = 13;
            this.progressBar1.Visible = false;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.textBox1.Location = new System.Drawing.Point(441, 455);
            this.textBox1.MaximumSize = new System.Drawing.Size(58, 20);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(58, 20);
            this.textBox1.TabIndex = 16;
            this.textBox1.Text = "21";
            this.textBox1.Visible = false;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label1.Location = new System.Drawing.Point(416, 439);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 21);
            this.label1.TabIndex = 17;
            this.label1.Text = "Focal length im mm";
            this.label1.Visible = false;
            // 
            // ImageProcess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(940, 503);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.buttonCalculate);
            this.Controls.Add(this.buttonPhotoDel);
            this.Controls.Add(this.buttonPhotoDown);
            this.Controls.Add(this.buttonSurfaceDel);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.buttonSurfaceUp);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.buttonPhotoUp);
            this.Controls.Add(this.buttonSurfaceDown);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.panel1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(956, 542);
            this.Name = "ImageProcess";
            this.Text = "Image Processing";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        /*private void rGBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }*/

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private ListView listView1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem orientateToolStripMenuItem;
        private Panel panel2;
        private PictureBox pictureBox2;
        private ListView listView2;
        private Button buttonPhotoUp;
        private Button buttonPhotoDown;
        private Button buttonPhotoDel;
        private Button buttonSurfaceDel;
        private Button buttonSurfaceUp;
        private Button buttonSurfaceDown;
        private Button buttonCalculate;        
        public System.Windows.Forms.ToolStripMenuItem depthToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem rGBToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem intensityToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem orthoToolStripMenuItem;
        private ContextMenuStrip contextMenuStrip1;
        private ProgressBar progressBar1;
        private TextBox textBox1;
        private Label label1;
    }
}