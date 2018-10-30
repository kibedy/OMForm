namespace ortomachine.Model
{
    internal class Points
    {
        public double X, Y, Z;
        public int intensity, R, G, B;
        public Points(double x, double y, double z, int intensity, int R, int G, int B)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.intensity = intensity;
            this.R = R;
            this.G = G;
            this.B = B;
        }
        public Points(double x, double y, double z, int R, int G, int B)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.R = R;
            this.G = G;
            this.B = B;

        }

        public Points(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public Points(double x, double y, double z, int intensity)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.intensity = intensity;

        }
    }
}