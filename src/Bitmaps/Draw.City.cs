using System.Drawing;
using civ2.Enums;
using ExtensionMethods;

namespace civ2.Bitmaps
{
    public static partial class Draw
    {
        // Draw an image of city
        public static void City(Graphics g, City city, bool isCitySizeWindow, int zoom, Point dest)
        {
            // Determine city style
            // For everything not modern or industrial => 4 city size styles (0=sizes 1...3, 1=sizes 4...5, 2=sizes 6...7, 3=sizes >= 8)
            // If city is capital => 3 size styles (1=sizes 1...3, 2=sizes 4...5, 3=sizes >= 6)
            CityStyleType style = city.Owner.CityStyle;
            int sizeStyle;
            if (style != CityStyleType.Industrial && style != CityStyleType.Modern)
            {
                if (city.ImprovementExists(ImprovementType.Palace)) // palace exists
                {
                    if (city.Size <= 3) sizeStyle = 1;
                    else if (city.Size > 3 && city.Size <= 5) sizeStyle = 2;
                    else sizeStyle = 3;
                }
                else
                {
                    if (city.Size <= 3) sizeStyle = 0;
                    else if (city.Size > 3 && city.Size <= 5) sizeStyle = 1;
                    else if (city.Size > 5 && city.Size <= 7) sizeStyle = 2;
                    else sizeStyle = 3;
                }
            }
            // If city is industrial => 4 city size styles (0=sizes 1...4, 1=sizes 5...7, 2=sizes 8...10, 3=sizes >= 11)
            // If city is capital => 3 size styles (1=sizes 1...4, 2=sizes 5...7, 3=sizes >= 8)
            else if (style == CityStyleType.Industrial)
            {
                if (city.ImprovementExists(ImprovementType.Palace)) // palace exists
                {
                    if (city.Size <= 4) sizeStyle = 1;
                    else if (city.Size > 4 && city.Size <= 7) sizeStyle = 2;
                    else sizeStyle = 3;
                }
                else
                {
                    if (city.Size <= 4) sizeStyle = 0;
                    else if (city.Size > 4 && city.Size <= 7) sizeStyle = 1;
                    else if (city.Size > 7 && city.Size <= 10) sizeStyle = 2;
                    else sizeStyle = 3;
                }
            }
            // If city is modern => 4 city size styles (0=sizes 1...4, 1=sizes 5...10, 2=sizes 11...18, 3=sizes >= 19)
            // If city is capital => 3 size styles (1=sizes 1...4, 2=sizes 5...10, 3=sizes >= 11)
            else
            {
                if (city.ImprovementExists(ImprovementType.Palace)) // palace exists
                {
                    if (city.Size <= 4) sizeStyle = 1;
                    else if (city.Size > 4 && city.Size <= 10) sizeStyle = 2;
                    else sizeStyle = 3;
                }
                else
                {
                    if (city.Size <= 4) sizeStyle = 0;
                    else if (city.Size > 4 && city.Size <= 10) sizeStyle = 1;
                    else if (city.Size > 10 && city.Size <= 18) sizeStyle = 2;
                    else sizeStyle = 3;
                }
            }

            using var sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;

            // Depending on the presence of a wall, get images of city and locations of size window & flag
            var cityPic = city.ImprovementExists(ImprovementType.CityWalls) ? Images.CityWall[(int)style, sizeStyle] : Images.City[(int)style, sizeStyle];
            var sizeWinLoc = city.ImprovementExists(ImprovementType.CityWalls) ? Images.CityWallSizeWindowLoc[(int)style, sizeStyle] : Images.CitySizeWindowLoc[(int)style, sizeStyle];
            var flagLoc = city.ImprovementExists(ImprovementType.CityWalls) ? Images.CityWallFlagLoc[(int)style, sizeStyle] : Images.CityFlagLoc[(int)style, sizeStyle];

            // Draw city
            g.DrawImage(ModifyImage.Resize(cityPic, zoom), dest.X, dest.Y);

            // Draw city size window
            if (isCitySizeWindow)
            {
                // Rectangle
                using var _pen = new Pen(Color.Black);
                g.DrawRectangle(_pen,
                    dest.X + Ext.ZoomScale(sizeWinLoc.X, zoom) - 1,
                    dest.Y + Ext.ZoomScale(sizeWinLoc.Y, zoom) - 1,
                    Ext.ZoomScale(9, zoom),
                    Ext.ZoomScale(13, zoom));

                // Fill rectangle
                using var _brush1 = new SolidBrush(CivColors.Light[city.OwnerId]);
                g.FillRectangle(_brush1,
                    dest.X + Ext.ZoomScale(sizeWinLoc.X, zoom),
                    dest.Y + Ext.ZoomScale(sizeWinLoc.Y, zoom),
                    Ext.ZoomScale(8, zoom),
                    Ext.ZoomScale(12, zoom));

                // Size text
                using var _brush2 = new SolidBrush(Color.Black);
                using var _font = new Font("Times New Roman", Ext.ZoomScale(10, zoom), FontStyle.Bold);
                g.DrawString(city.Size.ToString(),
                    _font, _brush2,
                    dest.X + Ext.ZoomScale(sizeWinLoc.X + 4, zoom),
                    dest.Y + Ext.ZoomScale(sizeWinLoc.Y + 6, zoom),
                    sf);
            }

            // Draw city flag if units are present in the city
            if (city.AnyUnitsPresent())
            {
                g.DrawImage(ModifyImage.Resize(Images.CityFlag[city.OwnerId], zoom),
                    dest.X + Ext.ZoomScale(flagLoc.X - 3, zoom),
                    dest.Y + Ext.ZoomScale(flagLoc.Y - 17, zoom));
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

            Point dest = new Point(4 * (8 + zoom) * offsetSqXY[0] + 4 * (8 + zoom), 2 * (8 + zoom) * (offsetSqXY[1] + 2) + 2);

            using var sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            using var font = new Font("Times New Roman", fontSize);
            using var brush = new SolidBrush(CivColors.CityTextColor[city.OwnerId]);
            g.DrawString(city.Name, font, Brushes.Black, new Point(dest.X + shadowOffset, dest.Y), sf);
            g.DrawString(city.Name, font, Brushes.Black, new Point(dest.X, dest.Y + shadowOffset), sf);
            g.DrawString(city.Name, font, brush, dest, sf);
        }
    }
}
