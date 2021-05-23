using System.Collections.Generic;
using Civ2engine;
using Eto.Drawing;
using EtoFormsUIExtensionMethods;

namespace EtoFormsUI.ImageLoader
{
    public static class UnitLoader
    {
        public static void LoadUnits(Ruleset path)
        {
            using var unitsImage = Common.LoadBitmapFrom("UNITS", path);

            // Initialize objects
            var units = new List<UnitImage>();

            // Define transparent colors
            var transparentGray =
                unitsImage.GetPixel(unitsImage.Width - 1, unitsImage.Height - 1); //define transparent back color (gray)
            var transparentPink = unitsImage.GetPixel(2, 2); //define transparent back color (pink)

            var borderColour = unitsImage.GetPixel(0, 0);
            var flagColour = borderColour;
            for (var i = 0; i < 100; i++)
            {
                flagColour = unitsImage.GetPixel(0, i);
                if (flagColour != borderColour) break;
            }

            var borderColours = new List<Color> {borderColour, flagColour};

            var height = 0;
            for (var i = 1; i < unitsImage.Height; i++)
            {
                if (borderColours.IndexOf(unitsImage.GetPixel(1, i)) == -1) continue;
                height = i;
                break;
            }

            var width = 0;


            for (var i = 1; i < unitsImage.Width; i++)
            {
                if (borderColours.IndexOf(unitsImage.GetPixel(i, 1)) == -1) continue;
                width = i;
                break;
            }

            unitsImage.ReplaceColors(transparentGray, Colors.Transparent);
            unitsImage.ReplaceColors(transparentPink, Colors.Transparent);

            for (var row = 0; row < unitsImage.Height - height; row += height)
            {
                for (var col = 0; col < unitsImage.Width - width; col += width)
                {
                    int flagX = 0;
                    int flagY = 0;

                    for (var i = col; i < col + width; i++)
                    {
                        var colour = unitsImage.GetPixel(i, row);
                        if (colour == borderColour) continue;
                        flagX = i - col;
                        break;
                    }

                    for (var i = row; i < row + height; i++)
                    {
                        var colour = unitsImage.GetPixel(col, i);
                        if (colour == borderColour) continue;
                        flagY = i - row;
                        break;
                    }

                    if (flagX == 0 || flagY == 0)
                    {
                        continue;
                    }

                    units.Add(new UnitImage
                    {
                        Bitmap = unitsImage.Clone(new Rectangle(col + 1, row + 1, width - 1, height - 1)),
                        FlagLoc = new Point(flagX, flagY)
                    });
                }
            }

            MapImages.Units = units.ToArray();

            // Extract shield without black border (used for stacked units)
            //     var _backUnitShield = unitsImage.Clone(new Rectangle(586, 1, 12, 20));
            //     _backUnitShield.ReplaceColors(transparentGray, Colors.Transparent);
            //
            //     // Extract unit shield
            //     var _unitShield = unitsImage.Clone(new Rectangle(597, 30, 12, 20));
            //     _unitShield.ReplaceColors(transparentGray, Colors.Transparent);
            //
            //     // Make shields of different colors for 8 different civs
            //     ShieldFront[0] = CreateNonIndexedImage(_unitShield); // convert GIF to non-indexed picture
            //     ShieldFront[1] = CreateNonIndexedImage(_unitShield);
            //     ShieldFront[2] = CreateNonIndexedImage(_unitShield);
            //     ShieldFront[3] = CreateNonIndexedImage(_unitShield);
            //     ShieldFront[4] = CreateNonIndexedImage(_unitShield);
            //     ShieldFront[5] = CreateNonIndexedImage(_unitShield);
            //     ShieldFront[6] = CreateNonIndexedImage(_unitShield);
            //     ShieldFront[7] = CreateNonIndexedImage(_unitShield);
            //     ShieldBack[0] = CreateNonIndexedImage(_backUnitShield);
            //     ShieldBack[1] = CreateNonIndexedImage(_backUnitShield);
            //     ShieldBack[2] = CreateNonIndexedImage(_backUnitShield);
            //     ShieldBack[3] = CreateNonIndexedImage(_backUnitShield);
            //     ShieldBack[4] = CreateNonIndexedImage(_backUnitShield);
            //     ShieldBack[5] = CreateNonIndexedImage(_backUnitShield);
            //     ShieldBack[6] = CreateNonIndexedImage(_backUnitShield);
            //     ShieldBack[7] = CreateNonIndexedImage(_backUnitShield);
            //     ShieldShadow = CreateNonIndexedImage(_backUnitShield);
            //     // Replace colors for unit shield and dark unit shield
            //     for (int x = 0; x < 12; x++)
            //     {
            //         for (int y = 0; y < 20; y++)
            //         {
            //             if (_unitShield.GetPixel(x, y) == transparentPink)   // If color is pink, replace it
            //             {
            //                 ShieldFront[0].SetPixel(x, y, MapImages.PlayerColours[0]);  // red
            //                 ShieldFront[1].SetPixel(x, y, MapImages.PlayerColours[1]);  // white
            //                 ShieldFront[2].SetPixel(x, y, MapImages.PlayerColours[2]);  // green
            //                 ShieldFront[3].SetPixel(x, y, MapImages.PlayerColours[3]);  // blue
            //                 ShieldFront[4].SetPixel(x, y, MapImages.PlayerColours[4]);  // yellow
            //                 ShieldFront[5].SetPixel(x, y, MapImages.PlayerColours[5]);  // cyan
            //                 ShieldFront[6].SetPixel(x, y, MapImages.PlayerColours[6]);  // orange
            //                 ShieldFront[7].SetPixel(x, y, MapImages.PlayerColours[7]);  // purple
            //             }
            //
            //             if (_backUnitShield.GetPixel(x, y) == Color.FromArgb(255, 0, 0))    // If color is red, replace it
            //             {
            //                 ShieldBack[0].SetPixel(x, y, MapImages.DarkColours[0]);  // red
            //                 ShieldBack[1].SetPixel(x, y, MapImages.DarkColours[1]);  // white
            //                 ShieldBack[2].SetPixel(x, y, MapImages.DarkColours[2]);  // green
            //                 ShieldBack[3].SetPixel(x, y, MapImages.DarkColours[3]);  // blue
            //                 ShieldBack[4].SetPixel(x, y, MapImages.DarkColours[4]);  // yellow
            //                 ShieldBack[5].SetPixel(x, y, MapImages.DarkColours[5]);  // cyan
            //                 ShieldBack[6].SetPixel(x, y, MapImages.DarkColours[6]);  // orange
            //                 ShieldBack[7].SetPixel(x, y, MapImages.DarkColours[7]);  // purple
            //                 ShieldShadow.SetPixel(x, y, Color.FromArgb(51, 51, 51));    // Color of the shield shadow
            //             }
            //         }
            //     }
            // }
        }
    }

    public class UnitImage
    {
        public Bitmap Bitmap { get; set; }
        public Point FlagLoc { get; set; }
    }
}