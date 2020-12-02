using System;
using System.Drawing;
using System.Linq;
using civ2.Enums;

namespace civ2.Bitmaps
{
    public partial class Draw
    {
        public Bitmap City(City city, bool citySizeWindow)
        {
            // Define a bitmap for drawing
            Bitmap cityPic = new Bitmap(64, 48);

            // Determine city style
            //For everything not modern or industrial => 4 city size styles (0=sizes 1...3, 1=sizes 4...5, 2=sizes 6...7, 3=sizes >= 8)
            //If city is capital => 3 size styles (1=sizes 1...3, 2=sizes 4...5, 3=sizes >= 6)
            CityStyleType cityStyle = city.CityStyle;
            int sizeStyle = 0;
            if (cityStyle < 4)
            {
                if (Array.Exists(city.Improvements, element => element.Type == ImprovementType.Palace)) //palace exists
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
            //If city is industrial => 4 city size styles (0=sizes 1...4, 1=sizes 5...7, 2=sizes 8...10, 3=sizes >= 11)
            //If city is capital => 3 size styles (1=sizes 1...4, 2=sizes 5...7, 3=sizes >= 8)
            else if (cityStyle == 4)
            {
                if (Array.Exists(city.Improvements, element => element.Type == ImprovementType.Palace)) //palace exists
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
            //If city is modern => 4 city size styles (0=sizes 1...4, 1=sizes 5...10, 2=sizes 11...18, 3=sizes >= 19)
            //If city is capital => 3 size styles (1=sizes 1...4, 2=sizes 5...10, 3=sizes >= 11)
            else
            {
                if (Array.Exists(city.Improvements, element => element.Type == ImprovementType.Palace)) //palace exists
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

            //If no units are in the city, draw no flag
            bool flagPresent = false;
            if (Game.GetUnits.Any(unit => unit.X == city.X && unit.Y == city.Y)) flagPresent = true;

            using (Graphics graphics = Graphics.FromImage(map))
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                if (!Array.Exists(city.Improvements, element => element.Type == ImprovementType.CityWalls))  //no city walls
                {
                    graphics.DrawImage(Draw.City[cityStyle, sizeStyle], 0, 0);
                    if (citySizeWindow) //Draw city size window
                    {
                        graphics.DrawRectangle(new Pen(Color.Black), Draw.citySizeWindowLoc[cityStyle, sizeStyle, 0] - 1, Draw.citySizeWindowLoc[cityStyle, sizeStyle, 1] - 1, 9, 13);  //rectangle
                        graphics.FillRectangle(new SolidBrush(CivColors.Light[city.OwnerId]), Draw.citySizeWindowLoc[cityStyle, sizeStyle, 0], Draw.citySizeWindowLoc[cityStyle, sizeStyle, 1], 8, 12); //filling of rectangle
                        graphics.DrawString(city.Size.ToString(), new Font("Times New Roman", 10.0f, FontStyle.Bold), new SolidBrush(Color.Black), Draw.citySizeWindowLoc[cityStyle, sizeStyle, 0] + 4, Draw.citySizeWindowLoc[cityStyle, sizeStyle, 1] + 6, sf);    //Size text
                    }
                    if (flagPresent)    //Draw city flag
                    {
                        graphics.DrawImage(Draw.CityFlag[city.OwnerId], Draw.cityFlagLoc[cityStyle, sizeStyle, 0] - 3, Draw.cityFlagLoc[cityStyle, sizeStyle, 1] - 17);
                    }
                }
                else
                {
                    graphics.DrawImage(Draw.CityWall[cityStyle, sizeStyle], 0, 0);
                    if (citySizeWindow)
                    {
                        graphics.DrawRectangle(new Pen(Color.Black), Draw.cityWallSizeWindowLoc[cityStyle, sizeStyle, 0] - 1, Draw.cityWallSizeWindowLoc[cityStyle, sizeStyle, 1] - 1, 9, 13); //Draw city (+Wall) size window
                        graphics.FillRectangle(new SolidBrush(CivColors.Light[city.OwnerId]), Draw.cityWallSizeWindowLoc[cityStyle, sizeStyle, 0], Draw.cityWallSizeWindowLoc[cityStyle, sizeStyle, 1], 8, 12); //filling of rectangle
                        graphics.DrawString(city.Size.ToString(), new Font("Times New Roman", 10.0f, FontStyle.Bold), new SolidBrush(Color.Black), Draw.cityWallSizeWindowLoc[cityStyle, sizeStyle, 0] + 4, Draw.cityWallSizeWindowLoc[cityStyle, sizeStyle, 1] + 6, sf);    //Size text    
                    }
                    if (flagPresent)    //Draw city flag
                    {
                        graphics.DrawImage(Draw.CityFlag[city.OwnerId], Draw.cityWallFlagLoc[cityStyle, sizeStyle, 0] - 3, Draw.cityWallFlagLoc[cityStyle, sizeStyle, 1] - 17);    //Draw city flag
                    }
                }

                sf.Dispose();
            }

            return map;
        }

    }
}
