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
            sc.image = new Image<Gray, ushort>("surface.png");
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("pixelérték: " + sc.image.Data[500 + i, 500 + i, 0]);
            }
            return sc.image.ToBitmap();
            ;
        }

        /* public void fillHoles()
         {
             //Emgu.CV.Image<Bgr, Byte> surface = new Emgu.CV.Image<Bgr, Byte>(image);


             for (int i = 1; i < image.Width - 1; i++)
             {
                 for (int j = 1; j < image.Height - 1; j++)
                 {
                     if (surface.Data[i, j, 0] == 0)
                     {
                         if (surface.Data[i + 1, j, 0] != 0 && surface.Data[i - 1, j, 0] != 0 && surface.Data[i, j - 1, 0] != 0 && surface.Data[i, j + 1, 0] != 0)
                         {
                             double r1 = 1 / 2 * surface.Data[i - 1, j - 1, 0] + (1 / 2) * surface.Data[i + 1, j - 1, 0];
                             double r2 = 1 / 2 * surface.Data[i - 1, j + 1, 0] + (1 / 2) * surface.Data[i + 1, j + 1, 0];
                             double p = 1 / 2 * r1 + 1 / 2 * r2;
                             ;
                         }
                     }
                 }
             }
         }*/




    } //class
} //namespace
