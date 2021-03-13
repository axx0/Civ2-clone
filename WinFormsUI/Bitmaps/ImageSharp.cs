using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using Civ2engine;

namespace WinFormsUI
{
    public class ImageSharp
    {
        public static Image Desert;
        public static Image Terrain;

        public static void LoadSpriteMaps()
        {
            Terrain = Image.Load(Settings.Civ2Path + Path.DirectorySeparatorChar + "TERRAIN1.GIF");
        }

        public static Image GetImage()
        {
            //return Terrain.Crop(new Rectangle(0, 0, 100, 100));
            return Terrain.Clone(x => x.Crop(new Rectangle(0, 0, 50, 50)));
        }

        public static void Draw()
        {

        }
    }
}
