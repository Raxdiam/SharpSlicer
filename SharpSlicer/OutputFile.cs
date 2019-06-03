using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace SharpSlicer
{
    internal class OutputFile
    {
        private readonly string _path;
        private readonly Box _box;
        private readonly List<Func<Bitmap, Bitmap>> _transformers = new List<Func<Bitmap, Bitmap>>();

        public OutputFile(string path, Box box)
        {
            _path = path;
            _box = box;
        }

        public void Process(string root, Bitmap image)
        {
            var width = image.Width;
            var height = image.Height;

            var outputPath = Path.Combine(root, _path);
            var x = _box.ScaleX(width);
            var y = _box.ScaleY(height);
            var w = _box.ScaleW(width);
            var h = _box.ScaleH(height);

            var subImage = image.Clone(new Rectangle(x, y, w, h), image.PixelFormat);

            foreach (var op in _transformers) {
                subImage = op.Invoke(subImage);
            }

            Util.WriteImage(outputPath, subImage);

            using (var g = Graphics.FromImage(image))
            using (var brush = new SolidBrush(Program.RemovedMarker))
                g.FillRectangle(brush, x, y, w, h);
        }

        public OutputFile Apply(Func<Bitmap, Bitmap> transform)
        {
            _transformers.Add(transform);
            return this;
        }
    }
}
