using System.Collections.Generic;
using System.Linq;
using Civ2engine;
using Eto.Drawing;
using EtoFormsUIExtensionMethods;

namespace EtoFormsUI
{
    public static class MapImageLoader
    {
        /// <summary>
        //  Read file in local directory. If it doesn't exist there, read it in root civ2 directory.
        /// </summary>
        /// <param name="name">the filename to load</param>
        /// <param name="path">the local directory to load from</param>
        /// <returns></returns>
        private static Bitmap LoadBitmapFrom(string name, Ruleset ruleset)
        {
            var filePath = Utils.GetFilePath(name, ruleset.Paths, "bmp", "gif");

            return filePath == null ? new Bitmap(640, 480, PixelFormat.Format32bppRgba) : new Bitmap(filePath);
        }

        private static readonly Color _flagColour = Color.FromArgb(0, 0, 255);

        public static CityImage[] Cities { get; set; }

        public static void LoadCities(Ruleset path)
        {
            // Read file in local directory. If it doesn't exist there, read it in root civ2 directory.
            var citiesImage = LoadBitmapFrom("CITIES", path);

            // Initialize objects
            var cities = new List<CityImage>();
            var flags = new List<Bitmap>();
            
            // Load special colours
            var colours = new List<Color>();
            var first = new List<int>();
            var last = new List<int>();
            var frequency = new List<int>();
            var max = 0;

            for (var i = 0; i < citiesImage.Height; i++)
            {
                var colour = citiesImage.GetPixel(0, i);
                var index = colours.IndexOf(colour);
                if (index == -1)
                {
                    colours.Add(colour);
                    first.Add(i);
                    last.Add(i);
                    frequency.Add(1);
                }
                else
                {
                    last[index] = i;
                    frequency[index]++;
                    if (index != max && frequency[index] > frequency[max])
                    {
                        max = index;
                    }
                }
            }

            var borderColour = colours[max];
            var firstRow = first[max];

            var firstTransparent = citiesImage.GetPixel(0, 0);
            var secondTransparent = citiesImage.GetPixel(1, firstRow + 1);
            
            var borderColours = colours.Where((_, i) => first[i] >= firstRow).ToList();
            
            var height = 0;
            for (var i = firstRow + 1; i < citiesImage.Height; i++)
            {
                if (borderColours.IndexOf(citiesImage.GetPixel(1, i)) == -1) continue;
                height = i - firstRow;
                break;
            }

            var width = 0;

            
            for (var i = 1; i < citiesImage.Width; i++)
            {
                if (borderColours.IndexOf(citiesImage.GetPixel(i, firstRow + 1)) == -1) continue;
                width = i;
                break;
            }
            
            citiesImage.ReplaceColors(firstTransparent, Colors.Transparent);
            citiesImage.ReplaceColors(secondTransparent, Colors.Transparent);

            for (var i = firstRow; i < citiesImage.Height - firstRow; i++)
            {
                if (citiesImage.GetPixel(1, i) != borderColour ||
                    citiesImage.GetPixel(1, i + height) != borderColour) continue;
                //We have a candidate row
                for (var j = 0; j < citiesImage.Width - width; j++)
                {
                    if (citiesImage.GetPixel(j, i + 1) != borderColour ||
                        citiesImage.GetPixel(j + width, i + 1) != borderColour) continue;
                    //This looks like a city image
                    var cityImage = MakeCityImage(citiesImage, i, j, width, height);
                    if (cityImage != null)
                    {
                        cities.Add(cityImage);
                    }
                }
            }

            Cities = cities.ToArray();

            var lastRow = last[max];
            


            //
            // // Get flag bitmaps
            // for (int col = 0; col < 9; col++)
            // {
            //     CityFlag[col] = citiesImage.Clone(new Rectangle(1 + (15 * col), 425, 14, 22));
            //     CityFlag[col].ReplaceColors(transparentGray, Colors.Transparent);
            // }
            //

            // Fortified = citiesImage.Clone(new Rectangle(143, 423, 64, 48));
            // Fortified.ReplaceColors(transparentGray, Colors.Transparent);
            // Fortified.ReplaceColors(transparentPink, Colors.Transparent);
            //
            // Fortress = citiesImage.Clone(new Rectangle(208, 423, 64, 48));
            // Fortress.ReplaceColors(transparentGray, Colors.Transparent);
            // Fortress.ReplaceColors(transparentPink, Colors.Transparent);
            //
            // Airbase = citiesImage.Clone(new Rectangle(273, 423, 64, 48));
            // Airbase.ReplaceColors(transparentGray, Colors.Transparent);
            // Airbase.ReplaceColors(transparentPink, Colors.Transparent);
            //
            // AirbasePlane = citiesImage.Clone(new Rectangle(338, 423, 64, 48));
            // AirbasePlane.ReplaceColors(transparentGray, Colors.Transparent);
            // AirbasePlane.ReplaceColors(transparentPink, Colors.Transparent);
            //
            // citiesImage.Dispose();
        }

        private static CityImage MakeCityImage(Bitmap citiesImage, int y, int x, int width, int height)
        {
            int flagX = 0;
            int flagY = 0;
            int sizeX = 0;
            int sizeY = 0;
            var borderColour = citiesImage.GetPixel(x, y);
            for (var i = x; i < x + width; i++)
            {
                var colour = citiesImage.GetPixel(i, y);
                if (colour == borderColour) continue;
                
                if (colour != _flagColour)
                {
                    sizeX = i - x;
                }
                else
                {
                    flagX = i - x;
                }
            }
            for (var i = y; i < y + height; i++)
            {
                var colour = citiesImage.GetPixel(x, i);
                if (colour == borderColour) continue;
                
                if (colour != _flagColour)
                {
                    sizeY = i - y;
                }
                else
                {
                    flagY = i - y;
                }
            }

            if (flagX == 0 || flagY == 0 || sizeX == 0 || sizeY == 0)
            {
                return null;
            }

            return new CityImage
            {
                Bitmap = citiesImage.Clone(new Rectangle(x + 1, y + 1, width - 1, height - 1)),
                FlagLoc = new Point(flagX, flagY),
                SizeLoc = new Point(sizeX, sizeY)
            };
        }
    }

    public sealed class CityImage
    {
        public Bitmap Bitmap { get; set; }
        public Point FlagLoc { get; set; }
        public Point SizeLoc { get; set; }
    }
}