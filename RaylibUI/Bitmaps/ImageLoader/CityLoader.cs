using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Civ2engine;
using Raylib_cs;
using System.Numerics;

namespace RaylibUI.ImageLoader
{
    public static class CityLoader
    {
        
        public unsafe static void LoadCities(Ruleset ruleset)
        {
            // First load city templates
            int _frames = 0;
            var citiesImage = Raylib.LoadImageAnim(ruleset.Paths[0] + Path.DirectorySeparatorChar + "CITIES.gif", out _frames);
            
            // Make transparent colors
            Raylib.ImageColorReplace(ref citiesImage, new Color(135, 135, 135, 255), new Color(135, 135, 135, 0));
            Raylib.ImageColorReplace(ref citiesImage, new Color(255, 0, 255, 255), new Color(255, 0, 255, 0));

            // Initialize objects
            var cities = new List<CityImage>();
            
            int offsetX;
            int offsetY = -10;
            for (int row = 0; row < 6; row++)
            {
                offsetX = 1;
                offsetY += 49;
                for (int col = 0; col < 8; col++)
                {
                    offsetX += 65;
                    if (col == 3) offsetX += 73;

                    var img = Raylib.ImageFromImage(citiesImage, new Rectangle(offsetX, offsetY, 64, 48));
                    cities.Add(new CityImage()
                    {
                        Bitmap = img,
                        Texture = Raylib.LoadTextureFromImage(img),
                        FlagLoc = new Vector2(0, 0),
                        SizeLoc = new Vector2(0, 0)
                    });
                    Raylib.UnloadImage(img);
                }
            }
            MapImages.Cities = cities.ToArray();
        }

        //private static readonly Color _flagColour = Color.FromArgb(0, 0, 255);
        //private static CityImage MakeCityImage(Bitmap citiesImage, int y, int x, int width, int height)
        //{
        //    int flagX = 0;
        //    int flagY = 0;
        //    int sizeX = 0;
        //    int sizeY = 0;
        //    var borderColour = citiesImage.GetPixel(x, y);
        //    for (var i = x; i < x + width; i++)
        //    {
        //        var colour = citiesImage.GetPixel(i, y);
        //        if (colour == borderColour) continue;

        //        if (colour != _flagColour)
        //        {
        //            sizeX = i - x;
        //            if (flagX == 0) flagX = sizeX;
        //        }
        //        else
        //        {
        //            flagX = i - x;
        //            if (sizeX == 0) sizeX = flagX;
        //        }
        //    }
        //    for (var i = y; i < y + height; i++)
        //    {
        //        var colour = citiesImage.GetPixel(x, i);
        //        if (colour == borderColour) continue;

        //        if (colour != _flagColour)
        //        {
        //            sizeY = i - y;
        //            if (flagY == 0) flagY = sizeY;
        //        }
        //        else
        //        {
        //            flagY = i - y;
        //            if (sizeY == 0) sizeY = flagY;
        //        }
        //    }


        //    if (flagX == 0 || flagY == 0 || sizeX == 0 || sizeY == 0)
        //    {
        //        return null;
        //    }

        //    return new CityImage
        //    {
        //        Bitmap = citiesImage.Clone(new Rectangle(x + 1, y + 1, width - 1, height - 1)),
        //        FlagLoc = new Point(flagX, flagY),
        //        SizeLoc = new Point(sizeX, sizeY)
        //    };
        //}
    }
}