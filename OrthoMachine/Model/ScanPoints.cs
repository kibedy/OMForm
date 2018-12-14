using Emgu.CV;
using Emgu.CV.Structure;
using OM_Form;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OM_Form.View;
using System.ComponentModel;

//using Emgu.CV;
//using Emgu.Util;
//using Emgu.CV.Structure;

namespace ortomachine.Model
{
    public class ScanPoints
    {
        LinkedList<Points> PointList = new LinkedList<Points>();
        //string path = "";        
        UInt16[,] surface;
        byte[,] intSurface;
        byte[,,] RGBsurface;
        ushort xwidth;
        ushort yheight;
        public int filetype;
        float offset;    //offset: black border
        float rastersize;
        public double Xmax, Ymax, X0, Y0;
        public Bitmap SurfaceMap;
        string filename;
        Form1 form1;
        //Form1 obj= null;
        public int lc;
        int actline;
        public int procbarvalue;
        public Image<Gray, ushort> image;
        public Image<Gray, byte> intSurfImage;
        public Image<Bgr, byte> RGBsurfImage; 
        //BackgroundWorker worker;
        //public int progress;
        //public delegate void ProgressUpdate(int value);
        //public event ProgressUpdate OnProgressupdate;


        //public ScanPoints(string filename, float rastersize, float offset, Form1 obj, BackgroundWorker worker, DoWorkEventArgs e)
        public ScanPoints(string filename, float rastersize, float offset, Form1 obj)
        {
            this.rastersize = rastersize;
            this.offset = offset;
            this.filename = filename;
            form1 = obj;
            procbarvalue = 0;
            //startprocess(filename);
            procbarvalue = 0;
            //this.worker = worker;
            lc = 0;
            

        }

        public void startprocess(string filename)
        {
            try
            {
                preprocess(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid file");
            }

            BoundingBox();
            Surface(this.rastersize, this.offset);
            SurfaceMap = saveSurface();

        }
        

        private void preprocess(string fn)
        {
            StreamReader srl = new StreamReader(fn);
            while (!srl.EndOfStream)
            {
                srl.ReadLine();
                lc++;
            }
            srl.Close();
           
            StreamReader sr = new StreamReader(fn);
            char[] delimiterChars = { ' ', ',', '\t' };

            string line = sr.ReadLine();
            string[] numbers = line.Split(delimiterChars);
            filetype = numbers.Length;
            form1.filetype = filetype;
            actline = 1;
            ;


            if (filetype == 3)
            {
                double X = double.Parse(numbers[0], System.Globalization.CultureInfo.InvariantCulture);
                double Y = double.Parse(numbers[1], System.Globalization.CultureInfo.InvariantCulture);
                double Z = double.Parse(numbers[2], System.Globalization.CultureInfo.InvariantCulture);

                Points point = new Points(X, Y, Z);
                PointList.AddFirst(point);
                procbarvalue = 0;

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    numbers = line.Split(delimiterChars);

                    //procbarvalue = CountLines(lc);
                    CountLines(lc);
                    ;
                    //form1.backgroundWorker1.ReportProgress(pro);


                    {
                        point = new Points(
                            double.Parse(numbers[0], System.Globalization.CultureInfo.InvariantCulture),
                            double.Parse(numbers[1], System.Globalization.CultureInfo.InvariantCulture),
                            double.Parse(numbers[2], System.Globalization.CultureInfo.InvariantCulture));
                        PointList.AddLast(point);
                    }
                }
            }

            if (filetype == 4)      //intensity only
            {
                Points point = new Points(
                    double.Parse(numbers[0], System.Globalization.CultureInfo.InvariantCulture),
                    double.Parse(numbers[1], System.Globalization.CultureInfo.InvariantCulture),
                    double.Parse(numbers[2], System.Globalization.CultureInfo.InvariantCulture),
                    int.Parse(numbers[3]));
                PointList.AddFirst(point);

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    numbers = line.Split(delimiterChars);
                    CountLines(lc);
                    {
                        point = new Points(
                            double.Parse(numbers[0], System.Globalization.CultureInfo.InvariantCulture),
                            double.Parse(numbers[1], System.Globalization.CultureInfo.InvariantCulture),
                            double.Parse(numbers[2], System.Globalization.CultureInfo.InvariantCulture),
                            int.Parse(numbers[3]));
                        PointList.AddLast(point);
                    }
                }
            }

            if (filetype == 7)
            {
                Points point = new Points(
                        double.Parse(numbers[0], System.Globalization.CultureInfo.InvariantCulture),
                        double.Parse(numbers[1], System.Globalization.CultureInfo.InvariantCulture),
                        double.Parse(numbers[2], System.Globalization.CultureInfo.InvariantCulture),
                        int.Parse(numbers[3]),
                        int.Parse(numbers[4]),
                        int.Parse(numbers[5]),
                        int.Parse(numbers[6]));


                PointList.AddFirst(point);

                while (!sr.EndOfStream)
                {

                    line = sr.ReadLine();
                    numbers = line.Split(delimiterChars);
                    CountLines(lc);
                    {
                        point = new Points(
                            double.Parse(numbers[0], System.Globalization.CultureInfo.InvariantCulture),
                            double.Parse(numbers[1], System.Globalization.CultureInfo.InvariantCulture),
                            double.Parse(numbers[2], System.Globalization.CultureInfo.InvariantCulture),
                            int.Parse(numbers[3]),
                            int.Parse(numbers[4]),
                            int.Parse(numbers[5]),
                            int.Parse(numbers[6]));
                        PointList.AddLast(point);
                    }
                }
            }
        }

        public void Surface(float rastersize, double offset)
        {
            xwidth = (ushort)((((Xmax - X0) + 2 * offset) / rastersize) + 1);
            yheight = (ushort)((((Ymax - Y0) + 2 * offset) / rastersize) + 1);

            surface = new UInt16[xwidth, yheight];
            if (form1.filetype == 7)
            {
                intSurface = new byte[xwidth, yheight];
                RGBsurface = new byte[xwidth, yheight,3];
                form1.intensityToolStripMenuItem.Enabled = true;
                form1.depthToolStripMenuItem.Enabled = true;
                form1.rGBToolStripMenuItem.Enabled = true;
                RGBsurface = new byte[xwidth, yheight, 3];
                foreach (Points item in PointList)
                {
                    uint i = (uint)(((item.X - X0) + offset) / rastersize);
                    uint j = (uint)(((item.Y - Y0) + offset) / rastersize);
                    if (surface[i, j] == 0 || surface[i, j] < item.Z * 1000)
                    {
                        surface[i, j] = (ushort)(item.Z * 1000);  //computing in mm                
                        RGBsurface[i, j, 0] = (byte)item.R;
                        RGBsurface[i, j, 1] = (byte)item.G;
                        RGBsurface[i, j, 2] = (byte)item.B;
                        intSurface[i, j] = (byte)item.intensity;

                    }
                }

            }
            else if (form1.filetype == 4)
            {
                form1.intensityToolStripMenuItem.Enabled = true;
                form1.depthToolStripMenuItem.Enabled = true;                
                intSurface = new byte[xwidth, yheight];
                foreach (Points item in PointList)
                {
                    uint i = (uint)(((item.X - X0) + offset) / rastersize);
                    uint j = (uint)(((item.Y - Y0) + offset) / rastersize);
                    if (surface[i, j] == 0 || surface[i, j] < item.Z * 1000)
                    {
                        surface[i, j] = (ushort)(item.Z * 1000);    //computing in mm     
                        intSurface[i, j] = (byte)item.intensity;
                    }

                }
            }

            //double minY = 999999;

            else
            {
                foreach (Points item in PointList)
                {
                    uint i = (uint)(((item.X - X0) + offset) / rastersize);
                    uint j = (uint)(((item.Y - Y0) + offset) / rastersize);
                    if (surface[i, j] == 0 || surface[i, j] < item.Z * 1000)
                    {
                        surface[i, j] = (ushort)(item.Z * 1000);    //computing in mm                
                    }

                }
            }
        }

        private void CountLines(int lineCount)
        {
            actline++;
            if (actline + 2 > lineCount / 20)
            {
                procbarvalue += 5;
                actline = 0;
                form1.backgroundWorker1.ReportProgress(procbarvalue);
            }
        }
      

        private void BoundingBox()
        {
            Xmax = 0; Ymax = 0; X0 = 999999999; Y0 = 999999999; //global coordinates
            foreach (Points item in PointList)
            {
                if (item.X < X0)
                {
                    X0 = item.X;
                }
                if (item.X > Xmax)
                {
                    Xmax = item.X;
                }
                if (item.Y < Y0)
                {
                    Y0 = item.Y;
                }
                if (item.Y > Ymax)
                {
                    Ymax = item.Y;
                }
            }

            ;
        }

        public Bitmap saveSurface()
        {
            GCHandle gch = GCHandle.Alloc(image, GCHandleType.Pinned);
            image = new Image<Gray, ushort>(xwidth, yheight);
            if (filetype ==4)
            {                
                intSurfImage = new Image<Gray, byte>(xwidth, yheight);
                for (int x = 0; x < xwidth; x++)
                {
                    for (int y = 0; y < yheight; y++)
                    {
                        intSurfImage.Data[yheight - y - 1, x, 0] = intSurface[x, y];                       
                    }
                }
                intSurfImage.Save(form1.SavePath + "\\" + "surface_int.png");
            }
            if (filetype == 7)
            {
                intSurfImage = new Image<Gray, byte>(xwidth, yheight);
                RGBsurfImage = new Image<Bgr, byte>(xwidth, yheight);
                for (int x = 0; x < xwidth; x++)
                {
                    for (int y = 0; y < yheight; y++)
                    {
                        intSurfImage.Data[yheight - y - 1, x, 0] = intSurface[x, y];
                        RGBsurfImage.Data[yheight - y - 1, x, 2] = RGBsurface[x, y, 0];
                        RGBsurfImage.Data[yheight - y - 1, x, 1] = RGBsurface[x, y, 1];
                        RGBsurfImage.Data[yheight - y - 1, x, 0] = RGBsurface[x, y, 2];
                    }
                }
                intSurfImage.Save(form1.SavePath + "\\" + "surface_int.png");
                RGBsurfImage.Save(form1.SavePath + "\\" + "surface_rgb.png");
            }
            
            
            for (int x = 0; x < xwidth; x++)
            {
                for (int y = 0; y < yheight; y++)
                {
                    image.Data[yheight - y - 1, x, 0] = surface[x, y];
                }
            }
            image.Save(form1.SavePath + "\\" + "surface.png");
            image.Save(form1.SavePath + "\\" + "surface_raw.png");
            StreamWriter sw = new StreamWriter(form1.SavePath + "\\" + "surface.xyz");
            sw.WriteLine("{0} {1} {2} {3}", (X0-offset).ToString(), (Y0-offset).ToString(), rastersize.ToString(), offset.ToString());
            sw.Close();
            return image.ToBitmap();
        }



    }
}
