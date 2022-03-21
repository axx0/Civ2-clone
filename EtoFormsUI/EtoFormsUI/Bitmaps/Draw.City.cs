using Eto.Drawing;
using EtoFormsUIExtensionMethods;
using Civ2engine;
using Civ2engine.Enums;
using System.Diagnostics;

namespace EtoFormsUI
{
    public static partial class Draw
    {
        // Draw an image of city
        public static void City(Graphics g, City city, bool isCitySizeWindow, int zoom, Point dest)
        {
            g.AntiAlias = false;

            // Determine city style
            // ANCIENT OR RENAISSANCE EPOCH => 4 city size styles (0=sizes 1...3, 1=sizes 4...5, 2=sizes 6...7, 3=sizes >= 8)
            // If city is capital => 3 size styles (1=sizes 1...3, 2=sizes 4...5, 3=sizes >= 6)
            var style = city.Owner.CityStyle;
            int sizeStyle;
            if (city.Owner.Epoch == EpochType.Ancient || city.Owner.Epoch == EpochType.Renaissance)
            {
                sizeStyle = city.Size switch
                {
                    <= 3 => 0,
                    > 3 and <= 5 => 1,
                    > 5 and <= 7 => 2,
                    _ => 3
                };

            }
            // INDUSTRIAL EPOCH => 4 city size styles (0=sizes 1...4, 1=sizes 5...7, 2=sizes 8...10, 3=sizes >= 11)
            // If city is capital => 3 size styles (1=sizes 1...4, 2=sizes 5...7, 3=sizes >= 8)
            else if (city.Owner.Epoch == EpochType.Industrial)
            {
                sizeStyle = city.Size switch
                {
                    <= 4 => 0,
                    > 4 and <= 7 => 1,
                    > 7 and <= 10 => 2,
                    _ => 3
                };
            }
            // MODERN EPOCH => 4 city size styles (0=sizes 1...4, 1=sizes 5...10, 2=sizes 11...18, 3=sizes >= 19)
            // If city is capital => 3 size styles (1=sizes 1...4, 2=sizes 5...10, 3=sizes >= 11)
            else
            {
                sizeStyle = city.Size switch
                {
                    <= 4 => 0,
                    > 4 and <= 10 => 1,
                    > 10 and <= 18 => 2,
                    _ => 3
                };
            }

            if (sizeStyle < 3 && city.ImprovementExists(ImprovementType.Palace))
            {
                sizeStyle++;
            }

            int cityIndex;
            if (city.Owner.Epoch == EpochType.Industrial)
            {
                cityIndex = 4 * 8 + sizeStyle;
            }
            else if (city.Owner.Epoch == EpochType.Modern)
            {
                cityIndex = 5 * 8 + sizeStyle;
            }
            else
            {
                cityIndex = (int)style * 8 + sizeStyle;
            }
            if(city.ImprovementExists(ImprovementType.CityWalls))
            {
                cityIndex += 4;
            }

            var cityImage = MapImages.Cities[cityIndex];

            // Depending on the presence of a wall, get images of city and locations of size window & flag
            var cityPic = cityImage.Bitmap;
            var sizeWinLoc = cityImage.SizeLoc;
            var flagLoc = cityImage.FlagLoc;

            // Draw city
            g.DrawImage(cityPic.Resize(zoom), new Point(dest.X, dest.Y));

            // Draw city size window
            if (isCitySizeWindow)
            {
                // Size text
                var formattedText = new FormattedText()
                {
                    Font = new Font("Times New Roman", 10.ZoomScale(zoom), FontStyle.Bold),
                    ForegroundBrush = new SolidBrush(Colors.Black),
                    Text = city.Size.ToString(),
                    Alignment = FormattedTextAlignment.Center
                };
                var textSize = formattedText.Measure();
                var textW = (int)textSize.Width;
                var textH = (int)textSize.Height;

                // Rectangle
                g.DrawRectangle(new Pen(Colors.Black),
                    dest.X + sizeWinLoc.X.ZoomScale(zoom) - 1,
                    dest.Y + sizeWinLoc.Y.ZoomScale(zoom) - 1,
                    textW - 1,
                    textH - 1);

                // Fill rectangle
                g.FillRectangle(new SolidBrush(MapImages.PlayerColours[city.OwnerId].LightColour),
                    dest.X + sizeWinLoc.X.ZoomScale(zoom),
                    dest.Y + sizeWinLoc.Y.ZoomScale(zoom),
                    textW - 2,
                    textH - 2);

                g.DrawText(formattedText, new Point(dest.X + sizeWinLoc.X.ZoomScale(zoom) - 1, dest.Y + sizeWinLoc.Y.ZoomScale(zoom) - 1));
            }

            // Draw city flag if units are present in the city
            if (city.AnyUnitsPresent())
            {
                using var _flagPic = MapImages.PlayerColours[city.OwnerId].Normal.Resize(zoom);
                g.DrawImage(_flagPic,
                    dest.X + (flagLoc.X - 3).ZoomScale(zoom),
                    dest.Y + (flagLoc.Y - 17).ZoomScale(zoom));
            }
        }

        public static void CityName(Graphics g, City city, int zoom, int[] offsetSqXY)
        {
            // Define text characteristics for zoom levels
            int shadowOffset, fontSize;
            switch (zoom)
            {
                case -7: shadowOffset = 0; fontSize = 1; break;
                case -6: shadowOffset = 0; fontSize = 3; break;
                case -5: shadowOffset = 0; fontSize = 5; break;
                case -4: shadowOffset = 1; fontSize = 7; break;
                case -3: shadowOffset = 1; fontSize = 10; break;
                case -2: shadowOffset = 1; fontSize = 11; break;
                case -1: shadowOffset = 1; fontSize = 13; break;
                case 0: shadowOffset = 2; fontSize = 14; break;
                case 1: shadowOffset = 2; fontSize = 16; break;
                case 2: shadowOffset = 2; fontSize = 17; break;
                case 3: shadowOffset = 2; fontSize = 19; break;
                case 4: shadowOffset = 2; fontSize = 21; break;
                case 5: shadowOffset = 2; fontSize = 24; break;
                case 6: shadowOffset = 2; fontSize = 25; break;
                case 7: shadowOffset = 2; fontSize = 26; break;
                case 8: shadowOffset = 2; fontSize = 28; break;
                default: shadowOffset = 2; fontSize = 14; break;
            }

            Point dest = new Point(Map.Xpx * offsetSqXY[0] + Map.Xpx, Map.Ypx * (offsetSqXY[1] + 2) + 2);

            //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

            var formattedFrontText = new FormattedText()
            {
                Font = new Font("Times New Roman", fontSize),
                ForegroundBrush = new SolidBrush(MapImages.PlayerColours[city.OwnerId].TextColour),
                Text = city.Name
            };
            var formattedShadow1Text = new FormattedText()
            {
                Font = new Font("Times New Roman", fontSize),
                ForegroundBrush = new SolidBrush(Colors.Black),
                Text = city.Name
            };
            var formattedShadow2Text = new FormattedText()
            {
                Font = new Font("Times New Roman", fontSize),
                ForegroundBrush = new SolidBrush(Colors.Black),
                Text = city.Name
            };

            var textSize = formattedFrontText.Measure();
            g.DrawText(formattedShadow1Text, new Point(dest.X + shadowOffset - (int)textSize.Width / 2, dest.Y - (int)textSize.Height / 2));
            g.DrawText(formattedShadow2Text, new Point(dest.X - (int)textSize.Width / 2, dest.Y + shadowOffset - (int)textSize.Height / 2));
            g.DrawText(formattedFrontText, new Point(dest.X - (int)textSize.Width / 2, dest.Y - (int)textSize.Height / 2));
        }

        public static void CityImprovement(Graphics g, ImprovementType type, int zoom, Point dest)
        {
            using var improvPic = CityImages.Improvements[(int)type].Resize(zoom);
            g.DrawImage(improvPic, dest);
        }

    }
}
