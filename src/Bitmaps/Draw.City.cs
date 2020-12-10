using System.Drawing;
using civ2.Enums;

namespace civ2.Bitmaps
{
    public partial class Draw
    {
        // Draw an image of city
        public static Bitmap City(City city, bool citySizeWindow, int zoom)
        {
            // Define a bitmap for drawing
            Bitmap cityPic = new Bitmap(64, 48);

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

            // If no units are in the city, draw no flag
            bool flagPresent = city.AnyUnitsPresent();

            using (Graphics graphics = Graphics.FromImage(cityPic))
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                if (!city.ImprovementExists(ImprovementType.CityWalls))  // no city walls
                {
                    graphics.DrawImage(Images.City[(int)style, sizeStyle], 0, 0);
                    
                    // Draw city size window
                    if (citySizeWindow)
                    {
                        // Rectangle
                        graphics.DrawRectangle(new Pen(Color.Black), Images.citySizeWindowLoc[(int)style, sizeStyle, 0] - 1, Images.citySizeWindowLoc[(int)style, sizeStyle, 1] - 1, 9, 13);

                        // Filling of rectangle
                        graphics.FillRectangle(new SolidBrush(CivColors.Light[city.OwnerId]), Images.citySizeWindowLoc[(int)style, sizeStyle, 0], Images.citySizeWindowLoc[(int)style, sizeStyle, 1], 8, 12);

                        // Size text
                        graphics.DrawString(city.Size.ToString(), new Font("Times New Roman", 10.0f, FontStyle.Bold), new SolidBrush(Color.Black), Images.citySizeWindowLoc[(int)style, sizeStyle, 0] + 4, Images.citySizeWindowLoc[(int)style, sizeStyle, 1] + 6, sf);
                    }

                    // Draw city flag
                    if (flagPresent)
                    {
                        graphics.DrawImage(Images.CityFlag[city.OwnerId], Images.cityFlagLoc[(int)style, sizeStyle, 0] - 3, Images.cityFlagLoc[(int)style, sizeStyle, 1] - 17);
                    }
                }
                else
                {
                    graphics.DrawImage(Images.CityWall[(int)style, sizeStyle], 0, 0);
                    if (citySizeWindow)
                    {
                        // Draw city (+Wall) size window
                        graphics.DrawRectangle(new Pen(Color.Black), Images.cityWallSizeWindowLoc[(int)style, sizeStyle, 0] - 1, Images.cityWallSizeWindowLoc[(int)style, sizeStyle, 1] - 1, 9, 13);

                        // Filling of rectangle
                        graphics.FillRectangle(new SolidBrush(CivColors.Light[city.OwnerId]), Images.cityWallSizeWindowLoc[(int)style, sizeStyle, 0], Images.cityWallSizeWindowLoc[(int)style, sizeStyle, 1], 8, 12);
                        
                        // Size text
                        graphics.DrawString(city.Size.ToString(), new Font("Times New Roman", 10.0f, FontStyle.Bold), new SolidBrush(Color.Black), Images.cityWallSizeWindowLoc[(int)style, sizeStyle, 0] + 4, Images.cityWallSizeWindowLoc[(int)style, sizeStyle, 1] + 6, sf);
                    }

                    // Draw city flag
                    if (flagPresent)
                    {
                        graphics.DrawImage(Images.CityFlag[city.OwnerId], Images.cityWallFlagLoc[(int)style, sizeStyle, 0] - 3, Images.cityWallFlagLoc[(int)style, sizeStyle, 1] - 17);
                    }
                }

                sf.Dispose();
            }

            cityPic = ModifyImage.ResizeImage(cityPic, 8 * (8 + zoom), 6 * (8 + zoom));

            return cityPic;
        }

        public static Bitmap CityName(City city, int zoom)
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

            // Draw
            Graphics gr = Graphics.FromImage(new Bitmap(1, 1));
            SizeF stringSize = gr.MeasureString(city.Name, new Font("Times New Roman", fontSize));
            int stringWidth = (int)stringSize.Width;
            int stringHeight = (int)stringSize.Height;
            Bitmap _textGraphic = new Bitmap(stringWidth + 2, stringHeight + 2);
            Graphics g = Graphics.FromImage(_textGraphic);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            g.DrawString(city.Name, new Font("Times New Roman", fontSize), Brushes.Black, new PointF(shadowOffset, 0));
            g.DrawString(city.Name, new Font("Times New Roman", fontSize), Brushes.Black, new PointF(0, shadowOffset));
            g.DrawString(city.Name, new Font("Times New Roman", fontSize), new SolidBrush(CivColors.CityTextColor[city.OwnerId]), new PointF(0, 0));

            return _textGraphic;
        }
    }
}
