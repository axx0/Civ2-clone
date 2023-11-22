using System.IO;
using System.Net.Mime;
using System.Text;
using Civ2.ImageLoader;
using Raylib_cs;
using Civ2engine;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Model.Images;

namespace RaylibUI
{
    public static class Images
    {
   
        //public static void LoadPeopleIcons(Ruleset ruleset)
        //{
        //    using var iconsImage = Common.LoadBitmapFrom("PEOPLE", ruleset.Paths);

        //    var peopleL = new Bitmap[11, 4];
        //    var peopleLshadow = new Bitmap[11, 4];

        //    var transparentPink = Color.FromArgb(255, 0, 255);

        //    for (int row = 0; row < 4; row++)
        //    {
        //        for (int col = 0; col < 11; col++)
        //        {
        //            peopleL[col, row] = iconsImage.Clone(new Rectangle((27 * col) + 2 + col, (30 * row) + 6 + row, 27, 30));
        //            peopleL[col, row].SetTransparent(new Color[] { transparentPink });

        //            peopleLshadow[col, row] = peopleL[col, row].Clone();
        //            peopleLshadow[col, row].ToSingleColor(Colors.Black);
        //        }
        //    }

        //    CityImages.PeopleLarge = peopleL;
        //    CityImages.PeopleShadowLarge = peopleLshadow;
        //}

        //public static void LoadCityWallpaper(Ruleset ruleset)
        //{
        //    var wallpaper = Common.LoadBitmapFrom("CITY", ruleset.Paths);
        //    CityImages.Wallpaper = wallpaper.CropImage(new Rectangle(0, 0, 636, 421));
        //}

        //public static void ImportWallpapersFromIconsFile()
        //{
        //    using var icons = Common.LoadBitmapFrom("ICONS", Settings.SearchPaths);
        //    MapImages.PanelOuterWallpaper = icons.Clone(new Rectangle(199, 322, 64, 32));
        //    MapImages.PanelInnerWallpaper = icons.Clone(new Rectangle(298, 190, 32, 32));
        //}

        /// <summary>
        /// Convert indexed to non-indexed images (required for making transparent pixels, etc.)
        /// </summary>
        /// <param name="src">Source indexed image</param>
        /// <returns>Non-indexed image</returns>
        //public static Bitmap CreateNonIndexedImage(Image src)
        //{
        //    var newBmp = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppRgba);

        //    using var g = new Graphics(newBmp);
        //    g.DrawImage(src, 0, 0);

        //    return newBmp;
        //}

        private static Dictionary<string, Image> ImageCache = new();

        private const string tempPath = "temp";
        
        public static Image ExtractBitmap(IImageSource imageSource)
        {
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            if (ImageCache.TryGetValue(imageSource.Key, out var bitmap)) return bitmap;
            
            switch (imageSource)
            {
                case BinaryStorage binarySource:
                    ImageCache[imageSource.Key] = ExtractBitmap(binarySource.FileName, binarySource.DataStart, binarySource.Length, imageSource.Key);
                    break;
                case BitmapStorage bitmapStorage:
                {
                    
                    var sourceKey = $"{bitmapStorage.Filename}-Source";
                    if (!ImageCache.ContainsKey(sourceKey))
                    {
                        var path = Utils.GetFilePath(bitmapStorage.Filename, Settings.SearchPaths, bitmapStorage.Extension);
                        ImageCache[sourceKey] = Raylib.LoadImageFromMemory(Path.GetExtension(path).ToLowerInvariant(), File.ReadAllBytes(path));
                    }

                    var sourceImage = ImageCache[sourceKey];
                    var rect = bitmapStorage.Location;
                    if (rect.width == 0)
                    {
                        rect = new Rectangle(0, 0, sourceImage.width, sourceImage.height);
                    }
                    var image = Raylib.ImageFromImage(ImageCache[sourceKey], rect);
                    if (bitmapStorage.Transparencies != null)
                    {
                        foreach (var transparency in bitmapStorage.Transparencies)
                        {
                            Raylib.ImageColorReplace(ref image, transparency, new Color(0,0,0,0));
                        }
                    }
                    ImageCache[bitmapStorage.Key] = image;
                    break;
                }
                default:
                    throw new NotImplementedException("Other image sources not currently implemented");
            }

            return ImageCache[imageSource.Key];
        }
        
        public static Image ExtractBitmap(string filename, int start, int length, string key)
        {
            if (!Files.ContainsKey(filename))
            {
                Files[filename] = File.ReadAllBytes(Utils.GetFilePath(filename));
            }

            return ExtractBitmap(Files[filename], start, length, key);
        }

        public static Dictionary<string, byte[]> Files { get; } = new();

        static byte[] extn = Encoding.ASCII.GetBytes("Gif\0");

        private static Image ExtractBitmap(byte[] byteArray, int start, int length, string key)
        {
            // Make empty byte array to hold GIF bytes
            byte[] newBytesRange = new byte[length];

            // Copy GIF bytes in DLL byte array into empty array
            Array.Copy(byteArray, start, newBytesRange, 0, length);
            var fileName = Path.Combine(tempPath, key + ".gif");
            using (var file = File.Create(fileName))
            {
                file.Write(newBytesRange);
                file.Flush();
            }

            return Raylib.LoadImage(fileName);

        }
    }
}
