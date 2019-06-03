using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SharpSlicer
{
    internal static class Util
    {
        public static void WriteImage(string path, Bitmap image)
        {
            /*Directory.CreateDirectory(Path.GetDirectoryName(path));
            if (File.Exists(path)) File.Delete(path);
            try {
                var fs = File.OpenWrite(path);
                image.Save(fs, ImageFormat.Png);
                Console.WriteLine(path);
            }
            catch (IOException e) {
                Console.WriteLine(e);
                throw;
            }*/

            Directory.GetParent(path).Create();
            if (File.Exists(path)) File.Delete(path);
            image.Save(path, ImageFormat.Png);
            Console.WriteLine(path);

            /*try {
                var fs = File.OpenWrite(path);
                image.Save(fs, ImageFormat.Png);
                Console.WriteLine(path);
            }
            catch (IOException e) {
                Console.WriteLine(e);
                throw;
            }*/
        }

        public static bool IsDirectory(string path)
        {
            var attr = File.GetAttributes(path);
            return attr.HasFlag(FileAttributes.Directory);
        }
    }
}
