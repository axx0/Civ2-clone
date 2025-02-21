using System.IO;
using System.Net.Mime;
using System.Text;
using Raylib_CSharp.Images;
using Raylib_CSharp.Transformations;
using Civ2engine;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Model;
using Model.Images;
using System.Numerics;
using Raylib_CSharp.Colors;

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

        public static ImageProps ExtractBitmapData(IImageSource imageSource, string[]? searchPaths)
        {
            return ExtractBitmapData(imageSource, null, searchPaths: searchPaths);
        }

        public static Image ExtractBitmap(IImageSource imageSource, IUserInterface active)
        {
            return ExtractBitmapData(imageSource, active).Image;
        }

        public static Image ExtractBitmap(IImageSource imageSource)
        {
            return ExtractBitmapData(imageSource, active: null).Image;
        }

        public static int GetImageWidth(IImageSource? imageSource, IUserInterface active, float scale = 1f) 
        { 
            return imageSource == null ? 0 : (int)(ExtractBitmap(imageSource, active).Width * scale);
        }

        public static int GetImageHeight(IImageSource? imageSource, IUserInterface active, float scale = 1f)
        {
            return imageSource == null ? 0 : (int)(ExtractBitmap(imageSource, active).Height * scale);
        }

        public static ImageProps ExtractBitmapData(IImageSource imageSource, IUserInterface? active, int owner = -1, string[]? searchPaths = null)
        {
            var imageProps = new ImageProps();
            int flag1X = 0, flag1Y = 0, flag2X = 0, flag2Y = 0;

            var key = imageSource.GetKey(owner);
            if (_imageCache.TryGetValue(key, out var bitmap))
            {
                imageProps.Image = bitmap;
                return imageProps;
            }
            
            switch (imageSource)
            {
                case BinaryStorage binarySource:
                    {
                        var sourceKey = $"Binary-{binarySource.Filename}-{binarySource.DataStart}-Source";
                        if (!_imageCache.ContainsKey(sourceKey))
                        {
                            var path = Utils.GetFilePath(binarySource.Filename);
                            var source_img_bpp = Images.LoadImageFromFile(path, binarySource.DataStart, binarySource.Length);
                            _imageCache[sourceKey] = source_img_bpp.Image;
                            _sourceBpp[sourceKey] = source_img_bpp.ColourDepth;
                        }

                        var sourceImage = _imageCache[sourceKey];
                        var rect = binarySource.Location;
                        if (rect.Width == 0)
                        {
                            rect = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);
                        }
                        var image = Image.FromImage(sourceImage, rect);
                        _imageCache[binarySource.Key] = image;
                        break;
                    }
                case BitmapStorage bitmapStorage:
                    {
                        var sourceKey = $"{bitmapStorage.Filename}-Source";
                        if (!_imageCache.ContainsKey(sourceKey))
                        {
                            string[] _paths = active != null ?
                                active.MainApp.ActiveRuleSet.Paths : searchPaths ?? Settings.SearchPaths;
                            var path = Utils.GetFilePath(bitmapStorage.Filename, _paths, bitmapStorage.Extension);
                            path ??= Utils.GetFilePath(bitmapStorage.Filename, Settings.SearchPaths, bitmapStorage.Extension);
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
                        var image = Image.FromImage(sourceImage, rect);

                        // Upper-left pixel transparency (not for 8bpp gif/bmp)
                        if (_sourceBpp[sourceKey] > 8 && bitmapStorage.TransparencyPixel)
                        {
                            var transparent = image.GetColor(0, 0);
                            image.ReplaceColor(transparent, new Color(transparent.R, transparent.G, transparent.B, 0));
                        }

                        // Get location of shields/city flags
                        if (bitmapStorage.SearchFlagLoc)
                        {
                            var bluePixel = new Color(0, 0, 255, 255);
                            var orangePixel = new Color(255, 155, 0, 255);
                            for (var col = 0; col < rect.Width; col++)
                            {
                                var pixelColour = sourceImage.GetColor((int)rect.X + col, (int)rect.Y - 1);
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
                                var pixelColour = sourceImage.GetColor((int)rect.X - 1, (int)rect.Y + row);
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
                        var image = memoryStorage.Image.Copy();
                        image.ReplaceColor(memoryStorage.ReplacementColour.Value,
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
        
        public static void ClearCache()
        {
            foreach (var image in _imageCache.Where(t => !t.Key.StartsWith("Binary")))
            {
                _imageCache.Remove(image.Key);
            }
        }
    }
}
