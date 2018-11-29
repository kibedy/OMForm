using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using OM_Form;
using ortomachine.Model;
using OM_Form.View;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ortomachine.Model
{
    public class Surface
    {
        public ScanPoints sc;
        public Bitmap imagebitmap;
        string filename;
        public Thread thread;
        public float offset, rastersize;
        Form1 obj;
        public int procbarvalue;


        public Surface(string filename, float offset, float rastersize, Form1 obj)
        {
            this.filename = filename;
            this.offset = offset;
            this.rastersize = rastersize;
            this.obj = obj;
            sc = new ScanPoints(filename, rastersize, offset, obj);
        }

        public Bitmap Run()
        {
            sc = new ScanPoints(filename, rastersize, offset, obj);
            sc.startprocess(filename);
            imagebitmap = sc.SurfaceMap;
            return imagebitmap;
        }

        public Bitmap LoadSurface(string SavePath, Form1 form1)
        {
            //Bitmap bitmap = new Bitmap("surface.png");
            StreamReader sr = new StreamReader(SavePath + "\\" + "surface.xyz");
            string s = sr.ReadLine();
            string[] ss = s.Split(' ');
            //Surface sf = new Surface("surface", float.Parse(ss[3]), float.Parse(ss[2]), obj);
            this.sc = new ScanPoints("surface", float.Parse(ss[2]), float.Parse(ss[3]), obj);
            this.sc.X0 = double.Parse(ss[0]);
            this.sc.Y0 = double.Parse(ss[1]);
            form1.rastersize = float.Parse(ss[2]);
            form1.offset = float.Parse(ss[3]);
            sr.Close();

            //image = new Bitmap("surface.png");
            sc.image = new Image<Gray, ushort>(SavePath + "\\surface.png");
            //GCHandle gch = GCHandle.Alloc(sc.image);
            /*for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("pixelérték: " + sc.image.Data[500 + i, 500 + i, 0]);
            }*/
            return sc.image.ToBitmap();
            
        }

        public void fillHoles(Form1 form1)
        {
            //Emgu.CV.Image<Bgr, Byte> surface = new Emgu.CV.Image<Bgr, Byte>(image);
            ;
            Emgu.CV.Image<Gray, ushort> filledsurface = sc.image;
           
            for (int i = 3; i < filledsurface.Height - 4; i++)
            {
                form1.progressBar1.Value = i / filledsurface.Height;
                for (int j = 3; j < filledsurface.Width - 4; j++)
                {
                    bool frame = true;
                    uint sumframe = 0;
                    uint suminner = 0;
                    int count = 0;
                    #region frame check
                    for (int k = -3; k <= 3; k++)
                    {
                        if (filledsurface.Data[i + k, j - 3, 0] != 0)
                        {
                            sumframe += filledsurface.Data[i + k, j - 3, 0];
                        }
                        else
                        {
                            frame = false;
                        }
                        if (filledsurface.Data[i + k, j + 3, 0] != 0 && frame == true)
                        {
                            sumframe += filledsurface.Data[i + k, j + 3, 0];
                        }
                        else
                        {
                            frame = false;
                        }
                    }
                    for (int k = -2; k <= 2; k++)
                    {
                        if (filledsurface.Data[i - 3, j + k, 0] != 0 && frame == true)
                        {
                            sumframe += filledsurface.Data[i - 3, j + k, 0];
                        }
                        else
                        {
                            frame = false;
                        }
                        if (filledsurface.Data[i + 3, j + k, 0] != 0 && frame == true)
                        {
                            sumframe += filledsurface.Data[i + 3, j + k, 0];
                        }
                        else
                        {
                            frame = false;
                        }
                    }
                    #endregion

                    if (frame == false)
                    {
                        continue;
                    }
                    for (int k = -2; k <= 2; k++)
                    {
                        for (int l = -2; l <= 2; l++)
                        {
                            if (filledsurface.Data[i + k, j + l, 0] != 0)
                            {
                                suminner += filledsurface.Data[i + k, j + l, 0];
                                count++;
                            }
                        }
                    }
                    if (count == 0 || count == 25)
                    {
                        continue;
                    }
                    //Console.WriteLine(sumframe+"  " + suminner +" " + count);
                    if (Math.Abs(suminner / count - sumframe / 24) > 1)
                    {
                        for (int k = -2; k <= 2; k++)
                        {
                            for (int l = -2; l <= 2; l++)
                            {
                                if (filledsurface.Data[i + k, j + l, 0] == 0)
                                {
                                    filledsurface.Data[i + k, j + l, 0] = (ushort)(suminner / count);
                                    //filledsurface.Data[i + k, j + l, 0] = 65000;
                                    ;
                                }
                            }
                        }
                    }

                }
            }

            form1.progressBar1.Value = 100;
            filledsurface.Save(form1.SavePath + "\\" + "surface_filled.png");
            filledsurface.Save(form1.SavePath + "\\" + "surface.png");
            form1.pictureBox1.Image = filledsurface.ToBitmap();
            sc.image = filledsurface;

        }

        public void bilinearfillHoles(Form1 form1)
        {
            //Emgu.CV.Image<Bgr, Byte> surface = new Emgu.CV.Image<Bgr, Byte>(image);
            ;
            Emgu.CV.Image<Gray, ushort> filledsurface = sc.image;



            for (int i = 1; i < sc.image.Height - 1; i++)
            {
                form1.progressBar1.Value = i / filledsurface.Height;
                for (int j = 1; j < sc.image.Width - 1; j++)
                {
                    if (sc.image.Data[i, j, 0] == 0)
                    {
                        if (sc.image.Data[i + 1, j + 1, 0] != 0 && sc.image.Data[i - 1, j - 1, 0] != 0 && sc.image.Data[i + 1, j - 1, 0] != 0 && sc.image.Data[i - 1, j + 1, 0] != 0)
                        {
                            ushort a11 = sc.image.Data[i - 1, j - 1, 0];
                            ushort a12 = sc.image.Data[i + 1, j - 1, 0];
                            ushort a21 = sc.image.Data[i - 1, j + 1, 0];
                            ushort a22 = sc.image.Data[i + 1, j + 1, 0];

                            float r1 = a11 / 2 + a12 / 2;
                            float r2 = a21 / 2 + a22 / 2;
                            float p = r1 / 2 + r2 / 2;
                            filledsurface.Data[i, j, 0] = (ushort)p;
                        }
                    }
                }
                for (int j = 1; j < sc.image.Width - 1; j++)
                {
                    form1.progressBar1.Value = i / filledsurface.Height;
                    if (sc.image.Data[i, j, 0] == 0)
                    {
                        if (sc.image.Data[i + 1, j, 0] != 0 && sc.image.Data[i - 1, j, 0] != 0 && sc.image.Data[i, j - 1, 0] != 0 && sc.image.Data[i, j + 1, 0] != 0)
                        {
                            ushort a11 = sc.image.Data[i - 1, j, 0];
                            ushort a12 = sc.image.Data[i, j + 1, 0];
                            ushort a21 = sc.image.Data[i, j - 1, 0];
                            ushort a22 = sc.image.Data[i + 1, j, 0];

                            float r1 = a11 / 2 + a12 / 2;
                            float r2 = a21 / 2 + a22 / 2;
                            float p = r1 / 2 + r2 / 2;
                            filledsurface.Data[i, j, 0] = (ushort)p;
                        }
                    }
                }
            }
            form1.progressBar1.Value = 100;
            filledsurface.Save(form1.SavePath + "\\" + "surface_bilin_filled.png");
            filledsurface.Save(form1.SavePath + "\\" + "surface.png");
            form1.pictureBox1.Image = filledsurface.ToBitmap();
            sc.image = filledsurface;
        }

        public void SurfaceResize(Form1 form1)
        {
            using (var surfaceresize = new Surfaceresize(form1))
            {
                DialogResult result = surfaceresize.ShowDialog();
                if (surfaceresize.DialogResult == DialogResult.OK)
                {

                    float newRastersize = float.Parse(surfaceresize.rasterbox.Text.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                    //Bilinresize(newRastersize, form1);
                    new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        Bilinresize(newRastersize, form1);
                    }).Start();


                }
            }

        }

        private void Bilinresize(float newrastersize, Form1 form1)
        {
            //throw new NotImplementedException();
            Emgu.CV.Image<Gray, ushort> resizedSurf = new Image<Gray, ushort>((int)(sc.image.Width / newrastersize * form1.rastersize), (int)(sc.image.Height / newrastersize * form1.rastersize));


            for (int i = 1; i < resizedSurf.Height - 1; i++)
            {              
                for (int j = 1; j < resizedSurf.Width - 1; j++)
                {
                    if (i * newrastersize / rastersize < (sc.image.Height - 2) && j * newrastersize / rastersize < (sc.image.Width - 2))
                    {                        
                        int surf_X = (int)Math.Ceiling(j / rastersize * newrastersize); //lower neighbor oldpixel
                        int surf_Y = (int)Math.Ceiling(i / rastersize * newrastersize);

                        float dx = (j / rastersize * newrastersize) - surf_X;
                        float dy = (i / rastersize * newrastersize) - surf_Y;

                        ushort a11 = form1.sf.sc.image.Data[surf_Y, surf_X, 0];
                        ushort a12 = form1.sf.sc.image.Data[surf_Y, surf_X + 1, 0];
                        ushort a21 = form1.sf.sc.image.Data[surf_Y + 1, surf_X, 0];
                        ushort a22 = form1.sf.sc.image.Data[surf_Y + 1, surf_X + 1, 0];
                        if (a11!=0 && a12 != 0 && a21 != 0 && a22 != 0)
                        {
                            resizedSurf.Data[i, j, 0] = (ushort)(a11 + dx * (a11 - a12) + dy * (a21 - a11) + dx * dy * (a11 - a12 - a21 + a22));
                        }                        
                    }
                }
            }

            resizedSurf.Save(form1.SavePath + "\\" + "surface.png");           
            sc.image.Save(form1.SavePath + "\\" + "surface_origsize.png");


            if (form1.filetype == 4 || form1.filetype == 7)
            {
                
                try //intensity surface model resizing
                {
                    //Image<Gray, byte> resizedInt = new Image<Gray, byte>((int)(sc.image.Width / newrastersize * rastersize), (int)(sc.image.Height / newrastersize * rastersize));
                    Image<Gray, byte> intsurf = new Image<Gray, byte>(form1.SavePath + "\\surface_int.png");
                    Image<Gray, byte> resizedInt = intsurf.Resize((int)(sc.image.Width / newrastersize * rastersize), (int)(sc.image.Height / newrastersize * rastersize), Emgu.CV.CvEnum.Inter.Linear);

                    /*
                    for (int i = 1 ; i < resizedInt.Height - 1 ; i++)
                    {
                        for (int j = 1 ; j < resizedInt.Width - 1 ; j++)
                        {
                            if (i * newrastersize / rastersize < (sc.image.Height - 2) && j * newrastersize / rastersize < (sc.image.Width - 2))
                            {
                                int surf_X = (int)Math.Ceiling(j / rastersize * newrastersize); //lower neighbor oldpixel
                                int surf_Y = (int)Math.Ceiling(i / rastersize * newrastersize);

                                float dx = (j / rastersize * newrastersize) - surf_X;
                                float dy = (i / rastersize * newrastersize) - surf_Y;

                                ushort a11 = intsurf.Data[surf_Y, surf_X, 0];
                                ushort a12 = intsurf.Data[surf_Y, surf_X + 1, 0];
                                ushort a21 = intsurf.Data[surf_Y + 1, surf_X, 0];
                                ushort a22 = intsurf.Data[surf_Y + 1, surf_X + 1, 0];
                                if (a11 != 0 && a12 != 0 && a21 != 0 && a22 != 0)
                                {
                                    resizedInt.Data[i, j, 0] = (byte)(a11 + dx * (a11 - a12) + dy * (a21 - a11) + dx * dy * (a11 - a12 - a21 + a22));
                                }
                            }
                        }
                    }*/
                    resizedInt.Save(form1.SavePath + "\\" + "surface_int.png");
                    intsurf.Save(form1.SavePath + "\\" + "surface_int_origsize.png");
                    intsurf = resizedInt;
                    sc.intSurfImage = intsurf;

                }
                catch
                {
                }
            }

            if (form1.filetype == 7)
            {
                try  //rgb point cloud surf image resizing
                {
                    //Image<Bgr, byte> resizedRGB = new Image<Bgr, byte>((int)(sc.image.Width / newrastersize * rastersize), (int)(sc.image.Height / newrastersize * rastersize));

                    Image<Bgr, byte> rgbsurf = new Image<Bgr, byte>(form1.SavePath + "\\surface_rgb.png");
                    /*
                    for (int i = 1 ; i < resizedRGB.Height - 1 ; i++)
                    {
                        for (int j = 1 ; j < resizedRGB.Width - 1 ; j++)
                        {
                            if (i * newrastersize / rastersize < (sc.image.Height - 2) && j * newrastersize / rastersize < (sc.image.Width - 2))
                            {
                                int surf_X = (int)Math.Ceiling(j / rastersize * newrastersize); //lower neighbor oldpixel
                                int surf_Y = (int)Math.Ceiling(i / rastersize * newrastersize);

                                float dx = (j / rastersize * newrastersize) - surf_X;
                                float dy = (i / rastersize * newrastersize) - surf_Y;

                                byte a11 = rgbsurf.Data[surf_Y, surf_X, 0];
                                byte a12 = rgbsurf.Data[surf_Y, surf_X + 1, 0];
                                byte a21 = rgbsurf.Data[surf_Y + 1, surf_X, 0];
                                byte a22 = rgbsurf.Data[surf_Y + 1, surf_X + 1, 0];
                                if (a11 != 0 && a12 != 0 && a21 != 0 && a22 != 0)
                                {
                                    resizedRGB.Data[i, j, 0] = (byte)(a11 + dx * (a11 - a12) + dy * (a21 - a11) + dx * dy * (a11 - a12 - a21 + a22));
                                }



                                a11 = rgbsurf.Data[surf_Y, surf_X, 1];
                                a12 = rgbsurf.Data[surf_Y, surf_X + 1, 1];
                                a21 = rgbsurf.Data[surf_Y + 1, surf_X, 1];
                                a22 = rgbsurf.Data[surf_Y + 1, surf_X + 1, 1];

                                if (a11 != 0 && a12 != 0 && a21 != 0 && a22 != 0)
                                {
                                    resizedRGB.Data[i, j, 1] = (byte)(a11 + dx * (a11 - a12) + dy * (a21 - a11) + dx * dy * (a11 - a12 - a21 + a22));
                                }


                                a11 = rgbsurf.Data[surf_Y, surf_X, 2];
                                a12 = rgbsurf.Data[surf_Y, surf_X + 1, 2];
                                a21 = rgbsurf.Data[surf_Y + 1, surf_X, 2];
                                a22 = rgbsurf.Data[surf_Y + 1, surf_X + 1, 2];

                                if (a11 != 0 && a12 != 0 && a21 != 0 && a22 != 0)
                                {
                                    resizedRGB.Data[i, j, 2] = (byte)(a11 + dx * (a11 - a12) + dy * (a21 - a11) + dx * dy * (a11 - a12 - a21 + a22));
                                }


                            }
                        }
                    }*/
                    Image<Bgr, byte> resizedRGB = rgbsurf.Resize((int)(sc.image.Width / newrastersize * rastersize), (int)(sc.image.Height / newrastersize * rastersize), Emgu.CV.CvEnum.Inter.Linear);

                    rgbsurf = resizedRGB;
                    resizedRGB.Save(form1.SavePath + "\\" + "surface_rgb.png");
                    rgbsurf.Save(form1.SavePath + "\\" + "surface_rgb_origsize.png");
                    sc.RGBsurfImage = rgbsurf;
                }
                catch
                {
                    Console.WriteLine("No color in Cloud");
                }
            }

            form1.rastersize = newrastersize;
            if (form1.pictureBox1.InvokeRequired)
            {
                form1.pictureBox1.BeginInvoke((MethodInvoker)delegate ()
                {
                    form1.pictureBox1.Image = resizedSurf.ToBitmap(); 
                    Application.DoEvents();
                });
            }
            StreamWriter sw = new StreamWriter(form1.SavePath + "\\" + "surface.xyz");
            sw.WriteLine("{0} {1} {2} {3}", (sc.X0 - offset).ToString(), (sc.Y0 - offset).ToString(), form1.rastersize.ToString(), offset.ToString());
            sw.Close();
            sc.image = resizedSurf;
        }


    } //class
} //namespace
