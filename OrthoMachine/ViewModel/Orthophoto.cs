using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.DataSets;
using Accord.Collections;
using Accord.MachineLearning;
using Emgu.CV;
using Emgu.CV.Structure;
using OM_Form;
using OrthoMachine.View;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord;
using Accord.Math;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.Xml;
using System.Windows.Forms;

namespace OrthoMachine.ViewModel
{
    public class Orthophoto
    {
        public Form1 form1;
        Image<Bgr, byte>[] ortholist;
        Image<Lab, byte>[] orthoLab;
        List<string> orthofilenames;
        List<string> orthothumbfilenames;
        ImageList imgs;
        Ortho orthoform;

        public Orthophoto(Form1 form1, Ortho orthoform)
        {
            this.form1 = form1;
            this.orthoform = orthoform;
        }



        internal void SilentLoadOrthothumbs()
        {
            DirectoryInfo d = new DirectoryInfo(form1.SavePath + "\\ortho");
            FileInfo[] Files = d.GetFiles("*_ortho.png");
            orthofilenames = new List<string>();
            orthothumbfilenames = new List<string>();
            foreach (FileInfo item in Files)
            {
                string x = form1.SavePath + "\\ortho\\" + item.Name;
                string thumbname = form1.SavePath + "\\ortho\\thumbs\\" + item.Name;
                orthothumbfilenames.Add(thumbname);
                orthofilenames.Add(x);
            }


            imgs = new ImageList();


            string imagesavepath = form1.SavePath + "\\ortho";
            try
            {
                if (!Directory.Exists(imagesavepath + "\\thumb"))
                {
                    DirectoryInfo di = Directory.CreateDirectory(imagesavepath);
                    DirectoryInfo dith = Directory.CreateDirectory(imagesavepath + "\\thumb");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Can't make Directory");
            }

            double scale;
            Image<Bgr, byte> loadedimage;
            Image<Bgr, byte> thumb;
            orthoform.listView1.View = System.Windows.Forms.View.LargeIcon;
            imgs.ImageSize = new Size(150, 150);
            orthoform.listView1.LargeImageList = imgs;
            orthoform.listView1.LargeImageList.ColorDepth = ColorDepth.Depth24Bit;
            int i = 0;
            foreach (FileInfo filename in Files)
            {
                try
                {
                    thumb = new Image<Bgr, byte>(form1.SavePath + "\\ortho\\thumb\\" + filename.Name);


                }
                catch
                {
                    loadedimage = new Image<Bgr, byte>(form1.SavePath + "\\ortho\\" + filename.Name);
                    scale = 200f / loadedimage.Width;
                    thumb = loadedimage.Resize(scale, Emgu.CV.CvEnum.Inter.Linear);
                    thumb.Save(form1.SavePath + "\\ortho\\thumb\\" + filename.Name);
                }
                finally
                {
                    imgs.Images.Add(filename.Name, System.Drawing.Image.FromFile(form1.SavePath + "\\ortho\\thumb\\" + filename.Name));
                    ListViewItem item = new ListViewItem(filename.Name);
                    item.Tag = filename.Name;
                    item.ImageIndex = i;
                    orthoform.listView1.Items.Add(item);
                    i++;
                }
            }
        }


        internal void Overlap()
        {

            List<string> Files = new List<string>();
            foreach (ListViewItem item in orthoform.listView1.SelectedItems)
            {
                Files.Add((string)item.Tag);    
            }

            if (Files.Count == 2)
            {


                ortholist = new Image<Bgr, byte>[Files.Count];
                orthoLab = new Image<Lab, byte>[Files.Count];
                int k = 0;
                foreach (string file in Files)
                {
                    string x = form1.SavePath + "\\ortho\\" + file;
                    ortholist[k] = new Image<Bgr, byte>(x);
                    orthoLab[k] = new Image<Lab, byte>(ortholist[k].Width, ortholist[k].Height);
                    CvInvoke.CvtColor(ortholist[k], orthoLab[k], Emgu.CV.CvEnum.ColorConversion.Bgr2Lab);
                    k++;
                    ;
                }


                try
                {
                    double[,] avgvalues = new double[2, 3];
                    int count = 0;
                    for (int i = 0; i < ortholist[0].Height; i++)
                    {
                        for (int j = 0; j < ortholist[0].Width; j++)
                        {
                            if (ortholist[0].Data[i, j, 0] != 0 && ortholist[0].Data[i, j, 1] != 0 && ortholist[0].Data[i, j, 2] != 0 &&
                                ortholist[1].Data[i, j, 0] != 0 && ortholist[1].Data[i, j, 1] != 0 && ortholist[1].Data[i, j, 2] != 0)
                            {
                                avgvalues[0, 0] += orthoLab[0].Data[i, j, 0];
                                avgvalues[1, 0] += orthoLab[1].Data[i, j, 0];
                                avgvalues[0, 1] += orthoLab[0].Data[i, j, 1];
                                avgvalues[1, 1] += orthoLab[1].Data[i, j, 1];
                                avgvalues[0, 2] += orthoLab[0].Data[i, j, 2];
                                avgvalues[1, 2] += orthoLab[1].Data[i, j, 2];
                                count++;
                            }

                        }
                    }

                    float l0 = (int)(avgvalues[0, 0] / count);
                    float a0 = (int)(avgvalues[0, 1] / count);
                    float b0 = (int)(avgvalues[0, 2] / count);
                    float l1 = (int)(avgvalues[1, 0] / count);
                    float a1 = (int)(avgvalues[1, 1] / count);
                    float b1 = (int)(avgvalues[1, 2] / count);
                    float l_avg = (l0 + l1) / 2;
                    float a_avg = (a0 + a1) / 2;
                    float b_avg = (b0 + b1) / 2;
                    float l0_mul = l_avg / l0;
                    float l1_mul = l_avg / l1;
                    float a0_mul = a_avg / a0;
                    float a1_mul = a_avg / a1;
                    float b0_mul = b_avg / b0;
                    float b1_mul = b_avg / b1;

                    for (int i = 0; i < ortholist[0].Height; i++)
                    {
                        for (int j = 0; j < ortholist[0].Width; j++)
                        {
                            if (ortholist[0].Data[i, j, 0] != 0 && ortholist[0].Data[i, j, 1] != 0 && ortholist[0].Data[i, j, 2] != 0 &&
                               ortholist[1].Data[i, j, 0] != 0 && ortholist[1].Data[i, j, 1] != 0 && ortholist[1].Data[i, j, 2] != 0)
                            {
                                orthoLab[0].Data[i, j, 0] = (byte)(orthoLab[0].Data[i, j, 0] * l0_mul);
                                orthoLab[1].Data[i, j, 0] = (byte)(orthoLab[1].Data[i, j, 0] * l1_mul);
                                //orthoLab[0].Data[i, j, 1] = (byte)(orthoLab[0].Data[i, j, 1] * a0_mul);
                                //orthoLab[1].Data[i, j, 1] = (byte)(orthoLab[1].Data[i, j, 1] * a1_mul);
                                //orthoLab[0].Data[i, j, 2] = (byte)(orthoLab[0].Data[i, j, 2] * b0_mul);
                                //orthoLab[1].Data[i, j, 2] = (byte)(orthoLab[1].Data[i, j, 2] * b1_mul);

                            }
                        }
                    }

                    Bitmap img1_lab = orthoLab[0].Bitmap;
                    Bitmap img2_lab = orthoLab[1].Bitmap;
                    Merge merge_lab = new Merge(img1_lab);
                    Bitmap resultImage_lab = merge_lab.Apply(img2_lab);
                    resultImage_lab.Save(form1.SavePath + "\\ortho\\" + Files[0] + "_" + Files[1], ImageFormat.Png);
                    orthoform.pictureBox1.Image = resultImage_lab;
                    SilentLoadOrthothumbs();
                }
                catch
                {
                    MessageBox.Show("No overlapping or missing images");
                }
            }

        }


        internal int[] CountOverlappingPix(int imgnumber, int referenceOrtho)
        {
            int[] count = new int[imgnumber - 1];
            for (int h = 0 ; h < imgnumber ; h++)
            {
                int cc = 0;
                for (int i = 0 ; i < orthoLab[0].Height ; i++)
                {
                    for (int j = 0 ; j < orthoLab[0].Width ; j++)
                    {
                        if (referenceOrtho != j && ortholist[referenceOrtho].Data[i, j, 0] != 0 && ortholist[referenceOrtho].Data[i, j, 2] != 0 && ortholist[h].Data[i, j, 0] != 0 && ortholist[h].Data[i, j, 2] != 0)
                        {
                            cc++;
                        }
                    }
                }
                count[h] = cc;
            }
            return count;
        }

        internal int[] Orderoverlap(int[] count)
        {
            int[] order = new int[count.Length];
            //int ii = count[0];
            for (int i = 0 ; i < count.Length ; i++)
            {
                for (int j = 1 ; j < count.Length ; j++)
                {
                    if (count[i] >= count[j])
                    {
                        order[i] = i;
                    }
                    else order[i] = j;
                }
            }



            return order;
        }

    }//class
}//namespace
