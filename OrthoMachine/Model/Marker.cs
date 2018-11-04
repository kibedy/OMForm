using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoMachine.Model
{
    public class Marker
    {
        public float X;
        public float Y;
        string imagename;
        public Marker(float x, float y, string imagename)
        {
            this.X = x;
            this.Y = y;
            this.imagename = imagename;
        }


        public override string ToString()
        {
            return X.ToString("0.00") + ", " + Y.ToString("0.00");
        }
    }

   

}
