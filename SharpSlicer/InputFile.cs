using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace SharpSlicer
{
    internal class InputFile
    {
        private readonly string _path;
        private readonly List<OutputFile> _outputs = new List<OutputFile>();

        public InputFile(string path)
        {
            _path = path;
        }

        public InputFile Outputs(params OutputFile[] files)
        {
            _outputs.AddRange(files.ToList());
            return this;
        }

        public void Process(string inputRoot, string outputRoot, string leftoverRoot = null)
        {
            var inputPath = Path.Combine(inputRoot, _path);
            if (File.Exists(inputPath))
            {
                try {
                    Bitmap image;
                    using (var imgTemp = new Bitmap(inputPath)) image = new Bitmap(imgTemp);
                    foreach (var outputFile in _outputs) {
                        outputFile.Process(outputRoot, image);
                    }

                    if (leftoverRoot != null) {
                        var leftoverPath = Path.Combine(leftoverRoot, _path);
                        Util.WriteImage(leftoverPath, image);
                    }

                    image.Dispose();
                }
                catch (IOException e) {
                    Console.WriteLine(e);
                    throw;
                }
            }
            else {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Input file {inputPath} not found, skipping!");
                Console.ResetColor();
            }
        }
    }
}
