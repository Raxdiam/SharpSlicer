using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;

namespace SharpSlicer
{
    internal class Program
    {
        public static readonly Color RemovedMarker = Color.FromArgb(128, 0, 0, 128);

        private static void Main(string[] args)
        {
            var argc = args.Length;
            if (argc != 2 && argc != 3) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Usage: <input dir or zip> <output dir> [<leftover dir>]");
                Console.ResetColor();
                return;
            }

            var inputPath = args[0];

            var outputPath = args[1];
            Directory.GetParent(outputPath).Create();

            string lefoverPath;
            if (argc == 3) {
                lefoverPath = args[2];
                Directory.GetParent(lefoverPath).Create();
            }
            else {
                lefoverPath = null;
            }

            string temp = null;
            if (!Util.IsDirectory(inputPath)) {
                if (Path.GetExtension(inputPath) == ".zip") {
                    temp = Path.Combine(Path.GetTempPath(), "slicer", Path.GetFileNameWithoutExtension(inputPath));
                    if (!Directory.Exists(temp)) Directory.CreateDirectory(temp);
                    else Directory.Delete(temp, true);
                    ZipFile.ExtractToDirectory(inputPath, temp);
                    inputPath = temp;
                }
                else {
                    throw new IOException("Expected either directory or zip file");
                }
            }

            Process(inputPath, outputPath, lefoverPath);

            if (temp != null) Directory.Delete(temp, true);
        }

        private static Box B256(int x, int y, int w, int h) => new Box(x, y, w, h, 256, 256);

        private static Box B128(int x, int y, int w, int h) => new Box(x, y, w, h, 128, 128);
        
        private static readonly Func<Bitmap, Bitmap> Square = img => {
            var width = img.Width;
            var height = img.Height;
            var dim = Math.Max(width, height);

            var dx = (dim - width) / 2;
            var dy = (dim - height) / 2;

            var newImg = new Bitmap(dim, dim, /*PixelFormat.Format32bppArgb*/img.PixelFormat);
            using (var g = Graphics.FromImage(newImg)) g.DrawImage(img, dx, dy, width, height);
            return newImg;
        };

        private static OutputFile GridSprite(string path, int x, int y, int w, int h, int xOff, int yOff, int xScale, int yScale) =>
            new OutputFile(path, B256(xScale * x + xOff, yScale * y + yOff, w * xScale, h * yScale));

        private static OutputFile Painting(string path, int x, int y, int w, int h) => GridSprite("assets/minecraft/textures/painting/" + path + ".png", x, y, w, h, 0, 0, 16, 16);

        private static OutputFile Effect(string path, int x, int y) => GridSprite("assets/minecraft/textures/mob_effect/" + path + ".png", x, y, 1, 1, 0, 198, 18, 18);

        private static OutputFile Particle(string path, int x, int y, int w, int h) => GridSprite("assets/minecraft/textures/particle/" + path + ".png", x, y, w, h, 0, 0, 8, 8);

        private static OutputFile Explosion(string path, int x, int y) => new OutputFile("assets/minecraft/textures/particle/" + path + ".png", B128(32 * x, 32 * y, 32, 32));

        private static OutputFile Particle(string path, int x, int y) => Particle(path, x, y, 1, 1);

        private static OutputFile Particle(string path, int x, int y, int xOff, int yOff, int w, int h) =>
            GridSprite("assets/minecraft/textures/particle/" + path + ".png", x, y, w, h, xOff, yOff, 8, 8);

        private static OutputFile Sweep(int i, int x, int y) =>
            new OutputFile("assets/minecraft/textures/particle/sweep_" + i + ".png", new Box(32 * x, 16 * y, 32, 16, 128, 32)).Apply(Square);

        private static InputFile Input(string path, params OutputFile[] outputs) => new InputFile(path).Outputs(outputs);

        private static readonly List<InputFile> Inputs = new List<InputFile> {
            Input("assets/minecraft/textures/painting/paintings_kristoffer_zetterstrand.png",
                  Painting("back", 15, 0, 1, 1),

                  Painting("kebab", 0, 0, 1, 1),
                  Painting("aztec", 1, 0, 1, 1),
                  Painting("alban", 2, 0, 1, 1),
                  Painting("aztec2", 3, 0, 1, 1),
                  Painting("bomb", 4, 0, 1, 1),
                  Painting("plant", 5, 0, 1, 1),
                  Painting("wasteland", 6, 0, 1, 1),
                  Painting("pool", 0, 2, 2, 1),
                  Painting("courbet", 2, 2, 2, 1),
                  Painting("sea", 4, 2, 2, 1),
                  Painting("sunset", 6, 2, 2, 1),
                  Painting("creebet", 8, 2, 2, 1),
                  Painting("wanderer", 0, 4, 1, 2),
                  Painting("graham", 1, 4, 1, 2),
                  Painting("match", 0, 8, 2, 2),
                  Painting("bust", 2, 8, 2, 2),
                  Painting("stage", 4, 8, 2, 2),
                  Painting("void", 6, 8, 2, 2),
                  Painting("skull_and_roses", 8, 8, 2, 2),
                  Painting("wither", 10, 8, 2, 2),
                  Painting("fighters", 0, 6, 4, 2),
                  Painting("pointer", 0, 12, 4, 4),
                  Painting("pigscene", 4, 12, 4, 4),
                  Painting("burning_skull", 8, 12, 4, 4),
                  Painting("skeleton", 12, 4, 4, 3),
                  Painting("donkey_kong", 12, 7, 4, 3)
            ),
            Input("assets/minecraft/textures/gui/container/inventory.png",
                  Effect("speed", 0, 0),
                  Effect("slowness", 1, 0),
                  Effect("haste", 2, 0),
                  Effect("mining_fatigue", 3, 0),
                  Effect("strength", 4, 0),
                  Effect("jump_boost", 2, 1),
                  Effect("nausea", 3, 1),
                  Effect("regeneration", 7, 0),
                  Effect("resistance", 6, 1),
                  Effect("fire_resistance", 7, 1),
                  Effect("water_breathing", 0, 2),
                  Effect("invisibility", 0, 1),
                  Effect("blindness", 5, 1),
                  Effect("night_vision", 4, 1),
                  Effect("hunger", 1, 1),
                  Effect("weakness", 5, 0),
                  Effect("poison", 6, 0),
                  Effect("wither", 1, 2),
                  Effect("health_boost", 7, 2),
                  Effect("absorption", 2, 2),
                  Effect("glowing", 4, 2),
                  Effect("levitation", 3, 2),
                  Effect("luck", 5, 2),
                  Effect("unluck", 6, 2),
                  Effect("slow_falling", 8, 0),
                  Effect("conduit_power", 9, 0),
                  Effect("dolphins_grace", 10, 0)
            ),
            Input("assets/minecraft/textures/particle/particles.png",
                  Particle("generic_0", 0, 0),
                  Particle("generic_1", 1, 0),
                  Particle("generic_2", 2, 0),
                  Particle("generic_3", 3, 0),
                  Particle("generic_4", 4, 0),
                  Particle("generic_5", 5, 0),
                  Particle("generic_6", 6, 0),
                  Particle("generic_7", 7, 0),

                  Particle("splash_0", 3, 1),
                  Particle("splash_1", 4, 1),
                  Particle("splash_2", 5, 1),
                  Particle("splash_3", 6, 1),

                  Particle("sga_a", 1, 14),
                  Particle("sga_b", 2, 14),
                  Particle("sga_c", 3, 14),
                  Particle("sga_d", 4, 14),
                  Particle("sga_e", 5, 14),
                  Particle("sga_f", 6, 14),
                  Particle("sga_g", 7, 14),
                  Particle("sga_h", 8, 14),
                  Particle("sga_i", 9, 14),
                  Particle("sga_j", 10, 14),
                  Particle("sga_k", 11, 14),
                  Particle("sga_l", 12, 14),
                  Particle("sga_m", 13, 14),
                  Particle("sga_n", 14, 14),
                  Particle("sga_o", 15, 14),
                  Particle("sga_p", 0, 15),
                  Particle("sga_q", 1, 15),
                  Particle("sga_r", 2, 15),
                  Particle("sga_s", 3, 15),
                  Particle("sga_t", 4, 15),
                  Particle("sga_u", 5, 15),
                  Particle("sga_v", 6, 15),
                  Particle("sga_w", 7, 15),
                  Particle("sga_x", 8, 15),
                  Particle("sga_y", 9, 15),
                  Particle("sga_z", 10, 15),

                  Particle("effect_0", 0, 8),
                  Particle("effect_1", 1, 8),
                  Particle("effect_2", 2, 8),
                  Particle("effect_3", 3, 8),
                  Particle("effect_4", 4, 8),
                  Particle("effect_5", 5, 8),
                  Particle("effect_6", 6, 8),
                  Particle("effect_7", 7, 8),

                  Particle("glitter_0", 0, 11),
                  Particle("glitter_1", 1, 11),
                  Particle("glitter_2", 2, 11),
                  Particle("glitter_3", 3, 11),
                  Particle("glitter_4", 4, 11),
                  Particle("glitter_5", 5, 11),
                  Particle("glitter_6", 6, 11),
                  Particle("glitter_7", 7, 11),

                  Particle("spark_0", 0, 10),
                  Particle("spark_1", 1, 10),
                  Particle("spark_2", 2, 10),
                  Particle("spark_3", 3, 10),
                  Particle("spark_4", 4, 10),
                  Particle("spark_5", 5, 10),
                  Particle("spark_6", 6, 10),
                  Particle("spark_7", 7, 10),

                  Particle("spell_0", 0, 9),
                  Particle("spell_1", 1, 9),
                  Particle("spell_2", 2, 9),
                  Particle("spell_3", 3, 9),
                  Particle("spell_4", 4, 9),
                  Particle("spell_5", 5, 9),
                  Particle("spell_6", 6, 9),
                  Particle("spell_7", 7, 9),

                  Particle("bubble_pop_0", 0 * 2, 16, 0, 3, 2, 2),
                  Particle("bubble_pop_1", 1 * 2, 16, 0, 3, 2, 2),
                  Particle("bubble_pop_2", 2 * 2, 16, 0, 3, 2, 2),
                  Particle("bubble_pop_3", 3 * 2, 16, 0, 3, 2, 2),
                  Particle("bubble_pop_4", 4 * 2, 16, 0, 3, 2, 2),

                  Particle("flash", 4, 2, 4, 4),
                  Particle("nautilus", 0, 13),
                  Particle("note", 0, 4),
                  Particle("angry", 1, 5),
                  Particle("bubble", 0, 2),
                  Particle("damage", 3, 4),
                  Particle("flame", 0, 3),
                  Particle("lava", 1, 3),
                  Particle("heart", 0, 5),
                  Particle("glint", 2, 5),
                  Particle("enchanted_hit", 2, 4),
                  Particle("critical_hit", 1, 4),
                  Particle("drip_hang", 0, 7),
                  Particle("drip_fall", 1, 7),
                  Particle("drip_land", 2, 7),

                  new OutputFile("assets/minecraft/textures/entity/fishing_hook.png", B256(8 * 1, 8 * 2, 8, 8))
            ),
            Input("assets/minecraft/textures/entity/explosion.png",
                  Explosion("explosion_0", 0, 0),
                  Explosion("explosion_1", 1, 0),
                  Explosion("explosion_2", 2, 0),
                  Explosion("explosion_3", 3, 0),

                  Explosion("explosion_4", 0, 1),
                  Explosion("explosion_5", 1, 1),
                  Explosion("explosion_6", 2, 1),
                  Explosion("explosion_7", 3, 1),

                  Explosion("explosion_8", 0, 2),
                  Explosion("explosion_9", 1, 2),
                  Explosion("explosion_10", 2, 2),
                  Explosion("explosion_11", 3, 2),

                  Explosion("explosion_12", 0, 3),
                  Explosion("explosion_13", 1, 3),
                  Explosion("explosion_14", 2, 3),
                  Explosion("explosion_15", 3, 3)
            ),
            Input("assets/minecraft/textures/entity/sweep.png",
                  Sweep(0, 0, 0),
                  Sweep(1, 1, 0),
                  Sweep(2, 2, 0),
                  Sweep(3, 3, 0),
                  Sweep(4, 0, 1),
                  Sweep(5, 1, 1),
                  Sweep(6, 2, 1),
                  Sweep(7, 3, 1)
            )
        };

        private static void Process(string inputPath, string outputPath, string leftoverPath = null) => Inputs.ForEach(f => f.Process(inputPath, outputPath, leftoverPath));
    }
}
