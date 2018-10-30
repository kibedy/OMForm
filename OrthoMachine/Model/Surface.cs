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
        }

        public Bitmap Run()
        {
            sc = new ScanPoints(filename, rastersize, offset, obj);
            sc.startprocess(filename);
            imagebitmap = sc.SurfaceMap;
            return imagebitmap;
        }

        public Bitmap LoadSurface(string SavePath, Form1 obj)
        {
            //Bitmap bitmap = new Bitmap("surface.png");
            StreamReader sr = new StreamReader(SavePath + "\\" + "surface.xyz");
            string s = sr.ReadLine();
            string[] ss = s.Split(' ');
            //Surface sf = new Surface("surface", float.Parse(ss[3]), float.Parse(ss[2]), obj);
            this.sc = new ScanPoints("surface", float.Parse(ss[2]), float.Parse(ss[3]), obj);
            this.sc.X0 = double.Parse(ss[0]);
            this.sc.Z0 = double.Parse(ss[1]);
            this.rastersize = float.Parse(ss[2]);
            this.offset = float.Parse(ss[3]);

            //image = new Bitmap("surface.png");
            sc.image = new Image<Gray, ushort>(SavePath+"\\surface.png");
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("pixelérték: " + sc.image.Data[500 + i, 500 + i, 0]);
            }
            return sc.image.ToBitmap();
            ;
        }

         public void fillHoles(Form1 form1)
         {
            //Emgu.CV.Image<Bgr, Byte> surface = new Emgu.CV.Image<Bgr, Byte>(image);
            ;
            Emgu.CV.Image<Gray, ushort> filledsurface = sc.image;

            /*
             for (int i = 1; i < sc.image.Width - 1; i++)
             {
                 for (int j = 1; j < sc.image.Height - 1; j++)
                 {
                     if (sc.image.Data[i, j, 0] == 0)
                     {
                         if (sc.image.Data[i + 1, j, 0] != 0 && sc.image.Data[i - 1, j, 0] != 0 && sc.image.Data[i, j - 1, 0] != 0 && sc.image.Data[i, j + 1, 0] != 0)
                         {
                            double r1 = 1 / 2 * sc.image.Data[i - 1, j - 1, 0] + (1 / 2) * sc.image.Data[i + 1, j - 1, 0];
                            double r2 = 1 / 2 * sc.image.Data[i - 1, j + 1, 0] + (1 / 2) * sc.image.Data[i + 1, j + 1, 0];
                            double p = 1 / 2 * r1 + 1 / 2 * r2;
                            filledsurface.Data[i, j, 0] = (ushort)p;
                            ;
                         }
                     }
                 }
             }*/
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
                        if (filledsurface.Data[i + k, j-3, 0] != 0)
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
                        if (filledsurface.Data[i -3, j + k, 0] != 0 && frame == true)
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

                    if (frame==false)
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
                    if (count==0 || count==25)
                    {
                        continue;
                    }
                    //Console.WriteLine(sumframe+"  " + suminner +" " + count);
                    if (Math.Abs(suminner/count - sumframe/24)> 1 )
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
                         if (sc.image.Data[i + 1, j+1, 0] != 0 && sc.image.Data[i - 1, j-1, 0] != 0 && sc.image.Data[i+1, j - 1, 0] != 0 && sc.image.Data[i-1, j + 1, 0] != 0)
                         {
                            ushort a11 = sc.image.Data[i - 1, j - 1, 0];
                            ushort a12 = sc.image.Data[i + 1, j - 1, 0];
                            ushort a21 = sc.image.Data[i - 1, j + 1, 0];
                            ushort a22 = sc.image.Data[i + 1, j + 1, 0];

                            float r1 =  a11 / 2 + a12 / 2;
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
                        if (sc.image.Data[i + 1, j, 0] != 0 && sc.image.Data[i - 1, j, 0] != 0 && sc.image.Data[i, j - 1, 0] != 0 && sc.image.Data[i , j + 1, 0] != 0)
                        {
                            ushort a11 = sc.image.Data[i - 1, j , 0];
                            ushort a12 = sc.image.Data[i , j + 1, 0];
                            ushort a21 = sc.image.Data[i , j - 1, 0];
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
            form1.pictureBox1.Image = filledsurface.ToBitmap();
            sc.image = filledsurface;
        }


    } //class
    } //namespace
