using System.IO;
using System.Net.Mime;
using System.Text;
using Raylib_cs;
using Civ2engine;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Model;
using Model.Images;
using System.Numerics;

namespace RaylibUtils
{
    public static partial class Images
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

        private static Dictionary<string, Image> _imageCache = new();
        private static Dictionary<string, int> _sourceBpp = new();

        private const string TempPath = "temp";

        public static ImageProps ExtractBitmapData(IImageSource imageSource)
        {
            return ExtractBitmapData(imageSource, null);
        }

        public static Image ExtractBitmap(IImageSource imageSource)
        {
            return ExtractBitmapData(imageSource, null).Image;
        }

        public static ImageProps ExtractBitmapData(IImageSource imageSource, IUserInterface? active, int owner = -1)
        {
            var imageProps = new ImageProps();
            int flag1X = 0, flag1Y = 0, flag2X = 0, flag2Y = 0;

            if (!Directory.Exists(TempPath))
            {
                Directory.CreateDirectory(TempPath);
            }

            var key = imageSource.GetKey(owner);
            if (_imageCache.TryGetValue(key, out var bitmap))
            {
                imageProps.Image = bitmap;
                return imageProps;
            }
            
            switch (imageSource)
            {

                case BinaryStorage binarySource:
                    _imageCache[key] = ExtractBitmap(binarySource.FileName, binarySource.DataStart, binarySource.Length, key);
                    break;
                case BitmapStorage bitmapStorage:
                    {
                        var sourceKey = $"{bitmapStorage.Filename}-Source";
                        if (!_imageCache.ContainsKey(sourceKey))
                        {
                            var path = Utils.GetFilePath(bitmapStorage.Filename, Settings.SearchPaths, bitmapStorage.Extension);
                            var source_img_bpp = Images.LoadImageFromFile(path);
                            _imageCache[sourceKey] = source_img_bpp.Image;
                            _sourceBpp[sourceKey] = source_img_bpp.ColourDepth;
                        }

                        var sourceImage = _imageCache[sourceKey];
                        var rect = bitmapStorage.Location;
                        if (rect.Width == 0)
                        {
                            rect = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);
                        }
                        var image = Raylib.ImageFromImage(sourceImage, rect);

                        // Upper-left pixel transparency (not for 8bpp gif/bmp)
                        if (_sourceBpp[sourceKey] > 8 && bitmapStorage.TransparencyPixel)
                        {
                            var transparent = Raylib.GetImageColor(image, 0, 0);
                            Raylib.ImageColorReplace(ref image, transparent,
                                new Color(transparent.R, transparent.G, transparent.B, (byte)0));
                        }

                        // Get location of shields/city flags
                        if (bitmapStorage.SearchFlagLoc)
                        {
                            var bluePixel = new Color(0, 0, 255, 255);
                            var orangePixel = new Color(255, 155, 0, 255);
                            for (var col = 0; col < rect.Width; col++)
                            {
                                var pixelColour = Raylib.GetImageColor(sourceImage, (int)rect.X + col, (int)rect.Y - 1);
                                if (bluePixel.R == pixelColour.R && bluePixel.G == pixelColour.G && bluePixel.B == pixelColour.B)
                                {
                                    flag1X = col;
                                }
                                else if (orangePixel.R == pixelColour.R && orangePixel.G == pixelColour.G && orangePixel.B == pixelColour.B)
                                {
                                    flag2X = col;
                                }
                            }
                            for (var row = 0; row < rect.Height; row++)
                            {
                                var pixelColour = Raylib.GetImageColor(sourceImage, (int)rect.X - 1, (int)rect.Y + row);
                                if (bluePixel.R == pixelColour.R && bluePixel.G == pixelColour.G && bluePixel.B == pixelColour.B)
                                {
                                    flag1Y = row;
                                }
                                else if (orangePixel.R == pixelColour.R && orangePixel.G == pixelColour.G && orangePixel.B == pixelColour.B)
                                {
                                    flag2Y = row;
                                }
                            }
                        }

                        _imageCache[bitmapStorage.Key] = image;
                    break;
                }
                case MemoryStorage memoryStorage:
                {
                    if (owner != -1 && memoryStorage.ReplacementColour != null && active != null)
                    {
                        var image = Raylib.ImageCopy(memoryStorage.Image);
                        Raylib.ImageColorReplace(ref image, memoryStorage.ReplacementColour.Value,
                            memoryStorage.Dark
                                ? active.PlayerColours[owner].DarkColour
                                : active.PlayerColours[owner].LightColour);
                        _imageCache[key] = image;
                    }
                    else
                    {
                        _imageCache[key] = memoryStorage.Image;
                    }
                    break;
                }
                default:
                    throw new NotImplementedException("Other image sources not currently implemented");
            }

            imageProps.Image = _imageCache[key];
            imageProps.Flag1 = new Vector2(flag1X, flag1Y);
            imageProps.Flag2 = new Vector2(flag2X, flag2Y);

            return imageProps;
        }
        
        public static Image ExtractBitmap(string filename, int start, int length, string key)
        {
            if (!Files.ContainsKey(filename))
            {
                var path = Utils.GetFilePath(filename);
                if (string.IsNullOrEmpty(path))
                {
                    Console.Error.WriteLine("Failed to load file " + filename + " please check value");
                    return Raylib.GenImageColor(1, 1, new Color(0, 0, 0, 0));
                }
                Files[filename] = File.ReadAllBytes(path);
            }

            return ExtractBitmap(Files[filename], start, length, key);
        }

        public static void ClearCache()
        {
            _imageCache.Clear();
            Files.Clear();
        }

        public static Dictionary<string, byte[]> Files { get; } = new();

        static byte[] _extn = Encoding.ASCII.GetBytes("Gif\0");

        private static Image ExtractBitmap(byte[] byteArray, int start, int length, string key)
        {
            // Make empty byte array to hold GIF bytes
            byte[] newBytesRange = new byte[length];

            // Copy GIF bytes in DLL byte array into empty array
            Array.Copy(byteArray, start, newBytesRange, 0, length);
            var fileName = Path.Combine(TempPath, key + ".gif");
            using (var file = File.Create(fileName))
            {
                file.Write(newBytesRange);
                file.Flush();
            }

            return Raylib.LoadImage(fileName);

        }
    }
}
