using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OM_Form.View;
using Emgu.CV;
using Emgu.CV.Structure;
using OrthoMachine.View;
//using OrthoMachine.Model;

namespace OM_Form.ViewModel
{
    public class Photo
    {
        private ImageList imgs;
        public ImageList projimages;
        public List<string> projimagefilenames; //image list with filenames
        List<string> projthumbfilenames;
        Form1 form1;        
        Orientation orientation;

        internal void SilentLoadPhotos(Form1 form1, List<string> imagelist)
        {
            projimagefilenames = new List<string>();
            projthumbfilenames = new List<string>();
            this.form1 = form1;

            foreach (string item in imagelist)
            {
                string[] fname = item.Split('\\');
                string thumb = form1.SavePath + "\\photos\\thumbs\\" + fname[fname.Length - 1];
                projthumbfilenames.Add(thumb);
                projimagefilenames.Add(item);
            }


            try
            {
                imgs = new ImageList();

                form1.listView1.View = System.Windows.Forms.View.LargeIcon;
                imgs.ImageSize = new Size(150, 150);
                form1.listView1.LargeImageList = imgs;
                form1.listView1.LargeImageList.ColorDepth = ColorDepth.Depth24Bit;
                int i = 0;
                foreach (String file in projthumbfilenames)
                {
                    string filename = file.Split('\\')[file.Split('\\').Length - 1];
                    imgs.Images.Add(filename, Image.FromFile(file));
                    ListViewItem item = new ListViewItem(filename);
                    item.Tag = filename;
                    item.ImageIndex = i;
                    //j++;
                    form1.listView1.Items.Add(item);
                    i++;
                    form1.progressBar1.Value = (int)(i / projthumbfilenames.Count);
                    
                }
                if (projimagefilenames.Count>0)
                {
                    form1.removeSelectedToolStripMenuItem.Enabled = true;
                }
            }

            catch { MessageBox.Show("Missing file(s)"); }
        }


        internal void LoadPhotos(Form1 form1)
        {
            this.form1 = form1;            
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (projimagefilenames == null)
            {
                projimagefilenames = new List<string>();
                projthumbfilenames = new List<string>();

            }


            openFileDialog1.Filter = "Images (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|" + "All files (*.*)|*.*";
            openFileDialog1.Multiselect = true;
            openFileDialog1.Title = "My Image Browser";

            DialogResult dr = openFileDialog1.ShowDialog();


            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                
                form1.listView1.View = System.Windows.Forms.View.Details;
                form1.listView1.Columns.Add("Photos", -2, HorizontalAlignment.Right);
                form1.listView1.Columns.Add("Filename", -2, HorizontalAlignment.Left);
                form1.listView1.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.HeaderSize);
                form1.progressBar1.Value = 0;
                string imagesavepath = (form1.SavePath + "\\photos");

                try
                {
                    if (!Directory.Exists(imagesavepath))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(imagesavepath);
                        DirectoryInfo dith = Directory.CreateDirectory(imagesavepath + "\\thumbs");
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Can't make Directory");
                }

                
                foreach (String file in openFileDialog1.FileNames)
                {                    
                    try
                    {
                        string[] fname = file.Split('\\');
                        string savename = imagesavepath + "\\"+fname[fname.Length - 1];
                        projimagefilenames.Add(savename);

                        Image<Bgr, byte> loadedimage = new Image<Bgr, byte>(file);

                        double scale = 200f / loadedimage.Width;
                        Image<Bgr, byte> thumb = loadedimage.Resize(scale, Emgu.CV.CvEnum.Inter.Linear);
                        //thumb.Resize()
                        string ss = imagesavepath + "\\thumbs\\" + fname[fname.Length - 1];
                        projthumbfilenames.Add(ss);
                        thumb.Save(ss);                   

                        FileInfo ff = new FileInfo(file);
                        ff.CopyTo(savename, true);


                    }
                    catch (IOException)
                    {                        
                        MessageBox.Show("Can't load images.\n\n");

                    }
                    catch (Exception ex)
                    {                        
                        MessageBox.Show("Cannot display the image: " + file.Substring(file.LastIndexOf('\\'))
                            + ". You may not have permission to read the file, or " +
                            "it may be corrupt.\n\nReported error: " + ex.Message);
                    }
                }

                imgs = new ImageList();

                form1.listView1.View = System.Windows.Forms.View.LargeIcon;
                imgs.ImageSize = new Size(150, 150);
                form1.listView1.LargeImageList = imgs;
                form1.listView1.LargeImageList.ColorDepth = ColorDepth.Depth24Bit;
                int i = 0;
                form1.listView1.Items.Clear();
                foreach (String file in projthumbfilenames)
                {
                    string filename = file.Split('\\')[file.Split('\\').Length - 1];
                    imgs.Images.Add(filename, Image.FromFile(file));
                    ListViewItem item = new ListViewItem(filename);
                    item.Tag = filename;
                    item.ImageIndex = i;              
                    form1.listView1.Items.Add(item);
                    i++;
                    form1.progressBar1.Value = (int)(i / projthumbfilenames.Count);

                }

                StreamWriter sw = new StreamWriter(form1.SavePath + "\\imagelist.txt");
                foreach (string item in projimagefilenames)
                {
                    sw.WriteLine(item);
                }                
                sw.Close();
                
                form1.progressBar1.Value = 100;
                if (projimagefilenames.Count > 0)
                {
                    form1.removeSelectedToolStripMenuItem.Enabled = true;
                }
            }
        }

        internal void ShowPhoto()
        {           
            ImageProcess improcform = new ImageProcess(form1, form1.listView1.SelectedItems[0].SubItems[0].Text, orientation);
            improcform.ShowDialog();         
        }

        public Photo()
        {

        }


    }//class
}//namespace
