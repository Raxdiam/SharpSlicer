namespace SharpSlicer
{
    internal class Box
    {
        public Box(int x, int y, int w, int h, int totalW, int totalH)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
            TotalW = totalW;
            TotalH = totalH;
        }

        public int X { get; }
        public int Y { get; }
        public int W { get; }
        public int H { get; }

        public int TotalW { get; set; }
        public int TotalH { get; set; }

        public int ScaleX(int imgWidth) => X * imgWidth / TotalW;

        public int ScaleY(int imgHeight) => Y * imgHeight / TotalH;

        public int ScaleW(int imgWidth) => W * imgWidth / TotalW;

        public int ScaleH(int imgHeight) => H * imgHeight / TotalH;
    }
}
