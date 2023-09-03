using System.Collections.Generic;
using System.IO;
using Civ2engine;
using Raylib_cs;
using System.Numerics;

namespace RaylibUI.ImageLoader
{
    public static class UnitLoader
    {
        private static readonly Color _shadowColour = new Color(51, 51, 51, 255);

        public static unsafe void LoadUnits(Ruleset ruleset)
        {
            var unitsImage = Images.LoadImage("UNITS", ruleset.Paths, "gif");

            // Initialize objects
            var units = new List<UnitImage>();

            var imageColours = Raylib.LoadImageColors(unitsImage);
            //// Define transparent colors
            var transparentGray = imageColours[(unitsImage.height-1)*unitsImage.width -2];
            //unitsImage.GetPixel(unitsImage.Width - 1, unitsImage.Height - 1); //define transparent back color (gray)
            var transparentPink =
                imageColours[
                    unitsImage.width * 2 + 2]; //  unitsImage.GetPixel(2, 2); //define transparent back color (pink)

            var borderColour = imageColours[0]; //  unitsImage.GetPixel(0, 0);
            var flagColour = borderColour;
            for (var i = 0; i < 100; i++)
            {
                flagColour = imageColours[unitsImage.width * i]; // unitsImage.GetPixel(0, i);
                if (!flagColour.Equals(borderColour)) break;
            }

            var borderColours = new List<Color> { borderColour, flagColour };

            var height = 0;
            for (var i = 1; i < unitsImage.height; i++)
            {
                var colour = imageColours[1 + i * unitsImage.width];
                if (colour.Equals(borderColour) || colour.Equals(flagColour))
                {
                    height = i;
                    break;
                }
            }

            var width = 0;


            for (var i = 1; i < unitsImage.width; i++)
            {
                var colour = imageColours[i + unitsImage.width];
                if (colour.Equals(borderColour) || colour.Equals(flagColour))
                {
                    width = i;
                    break;
                }
            }

            MakeSheilds(unitsImage, imageColours, width, borderColour, transparentGray);


            Raylib.UnloadImageColors(imageColours);

            // Make transparent colors
            Raylib.ImageColorReplace(ref unitsImage, transparentGray, new Color(0, 0, 0, 0));
            Raylib.ImageColorReplace(ref unitsImage, transparentPink, new Color(0, 0, 0, 0));

            imageColours = Raylib.LoadImageColors(unitsImage);

            for (var row = 0; row < 7; row++)
            {
                var rowIndex = row * unitsImage.width;
                for (var col = 0; col < 9; col++)
                {
                    var flagX = 0;
                    var flagY = 0;

                    for (var i = col; i < col + width; i++)
                    {
                        var colour = imageColours[rowIndex + i]; //  unitsImage.GetPixel(i, row);
                        if (colour.Equals(borderColour)) continue;
                        flagX = i - col - 1;
                        break;
                    }

                    for (var i = row; i < row + height; i++)
                    {
                        var colour = imageColours[col + i * unitsImage.width]; // unitsImage.GetPixel(col, i);
                        if (colour.Equals(borderColour)) continue;
                        flagY = i - row - 1;
                        break;
                    }

                    // if (flagX == 0 || flagY == 0)
                    // {
                    //     continue;
                    // }

                    var img = Raylib.ImageFromImage(unitsImage, new Rectangle(1 + col * 65, 1 + row * 49, 64, 48));
                    units.Add(new UnitImage
                    {
                        Image = img,
                        FlagLoc = new Vector2(flagX, flagY)
                    });
                }
            }

            MapImages.UnitRectangle = new Rectangle(0, 0, width, height);
            MapImages.Units = units.ToArray();
            Raylib.UnloadImageColors(imageColours);
        }

        private static unsafe void MakeSheilds(Image unitsImage, Color* colours, int width, Color borderColour,
            Color transparentGray)
        {
            int lastBorder;
            for (lastBorder = unitsImage.width - 1; lastBorder > width; lastBorder--)
            {
                if (colours[lastBorder].Equals(borderColour)) break;
            }

            var shieldWidth = 0;
            for (var i = lastBorder - 1; i >= 0; i--)
            {
                if (!colours[unitsImage.width + i].Equals(borderColour)) continue;
                shieldWidth = lastBorder - i;
                break;
            }

            var shieldHeight = 0;
            for (var i = 1; i < unitsImage.height; i++)
            {
                if (!colours[lastBorder -1 + i* unitsImage.width].Equals(borderColour)) continue;
                shieldHeight = i;
                break;
            }


            var unitShield = Raylib.ImageFromImage(unitsImage,new Rectangle(lastBorder - shieldWidth * (shieldWidth < shieldHeight ? 2 : 1) + 1,
                1, shieldWidth - 1, shieldHeight - 1));
            
            Raylib.ImageColorReplace(ref unitShield, transparentGray, new Color(0, 0, 0, 0));


            var firstColour =
                colours[unitsImage.width * 4 + lastBorder - shieldWidth * (shieldWidth < shieldHeight ? 2 : 1) + 1 + 3];// unitShield.GetPixel(3, 3);
            var shieldRec = new Rectangle(0, 0, unitShield.width, unitShield.height);
            Image MakeShield(Color colour)
            {
                var shield = Raylib.ImageFromImage(unitShield, shieldRec);// unitShield.Clone();
                Raylib.ImageColorReplace(ref shield, firstColour, colour);
                return shield;
            }

            MapImages.Shields = MapImages.PlayerColours.Select(c=>c.LightColour).Select((Func<Color,Image>) MakeShield).ToArray();
            MapImages.ShieldBack = MapImages.PlayerColours.Select(c=>c.DarkColour).Select((Func<Color,Image>) MakeShield).ToArray();
            MapImages.ShieldShadow = MakeShield(_shadowColour);
        }
    }

    public class UnitImage
    {
        public Image Image { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 FlagLoc { get; set; }
    }
}