using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using PoskusCiv2.Units;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Imagery
{
    class Draw
    {
        //Draw entire game map
        public static Bitmap DrawMap()
        {
            Bitmap map = new Bitmap(Game.Data.MapXdim * 64 + 32, Game.Data.MapYdim * 32 + 16);    //define a bitmap for drawing map
            
            Squares square = new Squares();
                       
            using (Graphics graphics = Graphics.FromImage(map))
            {
                for (int col = 0; col < Game.Data.MapXdim; col++)
                {
                    for (int row = 0; row < Game.Data.MapYdim; row++)
                    {
                        graphics.DrawImage(square.Terrain(col, row), 64 * col + 32 * (row % 2) + 1, 16 * row + 1);
                    }
                }
            }
            
            return map;
        }

        //Draw unit
        public Bitmap DrawUnit(IUnit unit, bool stacked)
        {
            Bitmap square = new Bitmap(64, 48);    //define a bitmap for drawing

            using (Graphics graphics = Graphics.FromImage(square))
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                string shieldText;
                switch (unit.Action)
                {
                    case UnitAction.Fortify: shieldText = "f"; break;
                    case UnitAction.Fortified: shieldText = "F"; break;
                    case UnitAction.Sentry: shieldText = "S"; break;
                    case UnitAction.BuildFortress: shieldText = "bf"; break;
                    case UnitAction.BuildRoadRR: shieldText = "R"; break;
                    case UnitAction.BuildIrrigation: shieldText = "I"; break;
                    case UnitAction.BuildMine: shieldText = "M"; break;
                    case UnitAction.TransformTerr: shieldText = "O"; break;
                    case UnitAction.CleanPollution: shieldText = "P"; break;
                    case UnitAction.BuildAirbase: shieldText = "A"; break;
                    case UnitAction.GoTo: shieldText = "G"; break;
                    case UnitAction.NoOrders: shieldText = "-"; break;
                    default: shieldText = "/"; break;
                }

                //draw unit shields
                //First determine if the shield is on the left or right side
                int firstShieldXLoc = Images.unitShieldLocation[(int)unit.Type, 0];
                int secondShieldXLoc = firstShieldXLoc;
                int secondShieldBorderXLoc;
                int borderShieldOffset;
                if (firstShieldXLoc < 32)
                {
                    borderShieldOffset = -1;
                    secondShieldXLoc -= 4;
                    secondShieldBorderXLoc = secondShieldXLoc - 1;
                }
                else
                {
                    borderShieldOffset = 1;
                    secondShieldXLoc += 4;
                    secondShieldBorderXLoc = secondShieldXLoc + 1;
                }

                if (stacked)    //draw dark shield if unit is stacked on top of others
                {
                    graphics.DrawImage(Images.BorderUnitShield, secondShieldBorderXLoc, Images.unitShieldLocation[(int)unit.Type, 1]); //black shield border
                    graphics.DrawImage(Images.NoBorderUnitShield[(int)unit.Civ], secondShieldXLoc, Images.unitShieldLocation[(int)unit.Type, 1]);   //dark shield
                }
                graphics.DrawImage(Images.BorderUnitShield, Images.unitShieldLocation[(int)unit.Type, 0] + borderShieldOffset, Images.unitShieldLocation[(int)unit.Type, 1]); //black shield border
                graphics.DrawImage(Images.UnitShield[(int)unit.Civ], Images.unitShieldLocation[(int)unit.Type, 0], Images.unitShieldLocation[(int)unit.Type, 1]); //main shield
                graphics.DrawString(shieldText, new Font("Arial", 8.0f), new SolidBrush(Color.Black), Images.unitShieldLocation[(int)unit.Type, 0] + 6, Images.unitShieldLocation[(int)unit.Type, 1] + 12, sf);    //Action on shield

                if (unit.Action != UnitAction.Sentry)
                {
                    graphics.DrawImage(Images.Units[(int)unit.Type], 0, 0);    //draw unit
                }
                else
                {
                    graphics.DrawImage(Images.Units[(int)unit.Type], new Rectangle(0, 0, 64, 48), 0, 0, 64, 48, GraphicsUnit.Pixel, ModifyImage.ConvertToGray());    //draw sentry unit
                }


                if (unit.Action == UnitAction.Fortified)
                {
                    graphics.DrawImage(Images.Fortified, 0, 0); //draw fortification
                }

                sf.Dispose();
            }

            return square;
        }

        //Draw city
        public Bitmap DrawCity(City city, bool citySizeWindow)
        {
            Bitmap map = new Bitmap(64, 48);    //define a bitmap for drawing map

            //Determine city bitmap
            //For everything not modern or industrial => 4 city size styles (0=sizes 1...3, 1=sizes 4...5, 2=sizes 6...7, 3=sizes >= 8)
            //If city is capital => 3 size styles (1=sizes 1...3, 2=sizes 4...5, 3=sizes >= 6)
            int cityStyle = Game.Civs[city.Owner].CityStyle;
            int sizeStyle = 0;
            if (cityStyle < 4)
            {
                if (Array.Exists(city.Improvements, element => element.Type == ImprovementType.Palace)) //palace exists
                {
                    if (city.Size <= 3) { sizeStyle = 1; }
                    else if (city.Size > 3 && city.Size <= 5) { sizeStyle = 2; }
                    else { sizeStyle = 3; }

                }
                else
                {
                    if (city.Size <= 3) { sizeStyle = 0; }
                    else if (city.Size > 3 && city.Size <= 5) { sizeStyle = 1; }
                    else if (city.Size > 5 && city.Size <= 7) { sizeStyle = 2; }
                    else { sizeStyle = 3; }
                }
            }
            //If city is industrial => 4 city size styles (0=sizes 1...4, 1=sizes 5...7, 2=sizes 8...10, 3=sizes >= 11)
            //If city is capital => 3 size styles (1=sizes 1...4, 2=sizes 5...7, 3=sizes >= 8)
            else if (cityStyle == 4)
            {
                if (Array.Exists(city.Improvements, element => element.Type == ImprovementType.Palace)) //palace exists
                {
                    if (city.Size <= 4) { sizeStyle = 1; }
                    else if (city.Size > 4 && city.Size <= 7) { sizeStyle = 2; }
                    else { sizeStyle = 3; }

                }
                else
                {
                    if (city.Size <= 4) { sizeStyle = 0; }
                    else if (city.Size > 4 && city.Size <= 7) { sizeStyle = 1; }
                    else if (city.Size > 7 && city.Size <= 10) { sizeStyle = 2; }
                    else { sizeStyle = 3; }
                }
            }
            //If city is modern => 4 city size styles (0=sizes 1...4, 1=sizes 5...10, 2=sizes 11...18, 3=sizes >= 19)
            //If city is capital => 3 size styles (1=sizes 1...4, 2=sizes 5...10, 3=sizes >= 11)
            else
            {
                if (Array.Exists(city.Improvements, element => element.Type == ImprovementType.Palace)) //palace exists
                {
                    if (city.Size <= 4) { sizeStyle = 1; }
                    else if (city.Size > 4 && city.Size <= 10) { sizeStyle = 2; }
                    else { sizeStyle = 3; }

                }
                else
                {
                    if (city.Size <= 4) { sizeStyle = 0; }
                    else if (city.Size > 4 && city.Size <= 10) { sizeStyle = 1; }
                    else if (city.Size > 10 && city.Size <= 18) { sizeStyle = 2; }
                    else { sizeStyle = 3; }
                }
            }

            //If no units are in the city, draw no flag
            bool flagPresent = false;
            if (Game.Units.Any(unit => unit.X == city.X && unit.Y == city.Y)) { flagPresent = true; }

            using (Graphics graphics = Graphics.FromImage(map))
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                if (!Array.Exists(city.Improvements, element => element.Type == ImprovementType.CityWalls))  //no city walls
                {
                    graphics.DrawImage(Images.City[cityStyle, sizeStyle], 0, 0);
                    if (citySizeWindow) //Draw city size window
                    {
                        graphics.DrawRectangle(new Pen(Color.Black), Images.citySizeWindowLoc[cityStyle, sizeStyle, 0] - 1, Images.citySizeWindowLoc[cityStyle, sizeStyle, 1] - 1, 9, 13);  //rectangle
                        graphics.FillRectangle(new SolidBrush(CivColors.Light[city.Owner]), Images.citySizeWindowLoc[cityStyle, sizeStyle, 0], Images.citySizeWindowLoc[cityStyle, sizeStyle, 1], 8, 12); //filling of rectangle
                        graphics.DrawString(city.Size.ToString(), new Font("Times New Roman", 10.0f, FontStyle.Bold), new SolidBrush(Color.Black), Images.citySizeWindowLoc[cityStyle, sizeStyle, 0] + 4, Images.citySizeWindowLoc[cityStyle, sizeStyle, 1] + 6, sf);    //Size text
                    }
                    if (flagPresent)    //Draw city flag
                    {
                        graphics.DrawImage(Images.CityFlag[city.Owner], Images.cityFlagLoc[cityStyle, sizeStyle, 0] - 3, Images.cityFlagLoc[cityStyle, sizeStyle, 1] - 17);
                    }
                }
                else
                {
                    graphics.DrawImage(Images.CityWall[cityStyle, sizeStyle], 0, 0);
                    if (citySizeWindow)
                    {
                        graphics.DrawRectangle(new Pen(Color.Black), Images.cityWallSizeWindowLoc[cityStyle, sizeStyle, 0] - 1, Images.cityWallSizeWindowLoc[cityStyle, sizeStyle, 1] - 1, 9, 13); //Draw city (+Wall) size window
                        graphics.FillRectangle(new SolidBrush(CivColors.Light[city.Owner]), Images.cityWallSizeWindowLoc[cityStyle, sizeStyle, 0], Images.cityWallSizeWindowLoc[cityStyle, sizeStyle, 1], 8, 12); //filling of rectangle
                        graphics.DrawString(city.Size.ToString(), new Font("Times New Roman", 10.0f, FontStyle.Bold), new SolidBrush(Color.Black), Images.cityWallSizeWindowLoc[cityStyle, sizeStyle, 0] + 4, Images.cityWallSizeWindowLoc[cityStyle, sizeStyle, 1] + 6, sf);    //Size text    
                    }
                    if (flagPresent)    //Draw city flag
                    {
                        graphics.DrawImage(Images.CityFlag[city.Owner], Images.cityWallFlagLoc[cityStyle, sizeStyle, 0] - 3, Images.cityWallFlagLoc[cityStyle, sizeStyle, 1] - 17);    //Draw city flag
                    }
                }

                sf.Dispose();
            }

            return map;
        }

        //Draw terrain in city form
        public Bitmap DrawCityFormMap(City city)
        {
            Bitmap map = new Bitmap(4 * 64, 4 * 32);    //define a bitmap for drawing map

            Squares square = new Squares();

            Bitmap image;
            using (Graphics graphics = Graphics.FromImage(map))
            {
                for (int x_ = -3; x_ <= 3; x_++)
                {
                    for (int y_ = -3; y_ <= 3; y_++)
                    {
                        if ((x_ == -1 & y_ == -3) || (x_ == 1 & y_ == -3) || (x_ == -2 & y_ == -2) || (x_ == 0 & y_ == -2) || (x_ == 2 & y_ == -2) || (x_ == -3 & y_ == -1) || (x_ == -1 & y_ == -1) || (x_ == 1 & y_ == -1) || (x_ == 3 & y_ == -1) || (x_ == -2 & y_ == 0) || (x_ == 0 & y_ == 0) || (x_ == 2 & y_ == 0) || (x_ == -3 & y_ == 1) || (x_ == -1 & y_ == 1) || (x_ == 1 & y_ == 1) || (x_ == 3 & y_ == 1) || (x_ == -2 & y_ == 2) || (x_ == 0 & y_ == 2) || (x_ == 2 & y_ == 2) || (x_ == -1 & y_ == 3) || (x_ == 1 & y_ == 3))
                        {
                            int newX = city.X2 + x_;
                            int newY = city.Y2 + y_;
                            if (newX >= 0 && newX < 2 * Game.Data.MapXdim && newY >= 0 && newY < Game.Data.MapYdim)
                            {
                                image = square.Terrain((newX - (newY % 2)) / 2, newY);
                            }
                            else
                            {
                                image = Images.Blank;
                            }
                            graphics.DrawImage(image, 32 * (x_ + 3), 16 * (y_ + 3));
                        }
                    }
                }
                graphics.DrawImage(DrawCity(city, false), 64 * 1 + 32 * (3 % 2) + 1, 16 * 2 + 1);
            }

            return map;
        }

        //Draw food/shields/trade icons in city form sqare which is offset (offsetX, offsetY) from the square with the city
        public Bitmap DrawCityFormMapIcons(City city, int offsetX, int offsetY)
        {            
            offsetX = (offsetX - (offsetY % 2)) / 2;    //First turn offsetX/Y from Civ2 to real coordinates

            Bitmap icons = new Bitmap(64, 32);    //define a bitmap for drawing icons
            using (Graphics graphics = Graphics.FromImage(icons))
            {
                //First count all icons on this square to determine the spacing between icons (10 = no spacing, 15 = no spacing @ 50% scaled)
                int spacing;
                int countF = Game.Terrain[city.X + offsetX, city.Y + offsetY].Food;
                int countS = Game.Terrain[city.X + offsetX, city.Y + offsetY].Shields;
                int countT = Game.Terrain[city.X + offsetX, city.Y + offsetY].Trade;
                switch (countF + countS + countT)
                {
                    case 1:
                    case 2: { spacing = 17; break; }    //50 % larger (orignal = 11, 1 pixel gap)
                    case 3: { spacing = 15; break; }    //50 % larger (orignal = 10, no gap)
                    case 4: { spacing = 11; break; }    //50 % larger (orignal = 7)
                    case 5: { spacing = 8; break; }    //50 % larger (orignal = 5)
                    case 6: { spacing = 6; break; }    //50 % larger (orignal = 4)
                    case 7:
                    case 8: { spacing = 5; break; }    //50 % larger (orignal = 3)
                    case 9: { spacing = 3; break; }    //50 % larger (orignal = 2)
                    case 10: { spacing = 2; break; }    //50 % larger (orignal = 1)
                    default: { spacing = 2; break; }    //50 % larger (orignal = 1)
                }
                //First draw food, then shields, then trade icons
                for (int i = 0; i < countF; i++)
                {
                    graphics.DrawImage(Images.CitymapFoodSmallBigger, i * spacing, 0);
                }
                for (int i = 0; i < countS; i++)
                {
                    graphics.DrawImage(Images.CitymapShieldSmallBigger, (countF + i) * spacing, 0);
                }
                for (int i = 0; i < countT; i++)
                {
                    graphics.DrawImage(Images.CitymapTradeSmallBigger, (countF + countS + i) * spacing, 0);
                }
            }
            return icons;
        }

        //Draw icons in city resources (surplus < 0 is hunger)
        public Bitmap DrawCityIcons(City city, int foodIcons, int surplusIcons, int tradeIcons, int corruptionIcons, int taxIcons, int luxIcons, int sciIcons, int supportIcons, int productionIcons)
        {
            int x_size = 330;
            int y_size = 200;
            Bitmap icons = new Bitmap(x_size, y_size);    //define a bitmap for drawing icons
            using (Graphics graphics = Graphics.FromImage(icons))
            {
                //Number of food+surplus/hunger icons determines spacing between icons
                int spacing;
                switch (foodIcons + Math.Abs(surplusIcons))
                {
                    case int n when (n >= 1 && n <= 15): { spacing = 23; break; }    //50 % larger (orignal = 15, 1 pixel gap)
                    case int n when (n == 16 || n == 17): { spacing = 20; break; }   //50 % larger (orignal = 13, 1 pixel overlap)
                    case int n when (n == 18 || n == 19): { spacing = 17; break; }   //50 % larger (orignal = 11, 3 pixel overlap)
                    case int n when (n == 20 || n == 21): { spacing = 15; break; }   //50 % larger (orignal = 10, 4 pixel overlap)
                    case int n when (n == 22 || n == 23): { spacing = 14; break; }   //50 % larger (orignal = 9, 5 pixel overlap)
                    case int n when (n == 24 || n == 25): { spacing = 12; break; }   //50 % larger (orignal = 8, 6 pixel overlap)
                    case int n when (n >= 26 && n <= 29): { spacing = 11; break; }   //50 % larger (orignal = 7, 7 pixel overlap)
                    case int n when (n >= 30 && n <= 33): { spacing = 9; break; }    //50 % larger (orignal = 6, 8 pixel overlap)
                    case int n when (n >= 34 && n <= 37): { spacing = 8; break; }    //50 % larger (orignal = 5, 9 pixel overlap)
                    case int n when (n >= 38 && n <= 49): { spacing = 6; break; }    //50 % larger (orignal = 4, 10 pixel overlap)
                    case int n when (n >= 50 && n <= 65): { spacing = 5; break; }    //50 % larger (orignal = 3, 11 pixel overlap)
                    case int n when (n >= 66): { spacing = 3; break; }               //50 % larger (orignal = 2, 12 pixel overlap)
                    default: { spacing = 2; break; }
                }
                //First draw background rectangle
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(71, 147, 31)), 0, 0, spacing * foodIcons + 21 - spacing + 6, 23); //background square for food
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(55, 123, 23)), x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing + 3), 0, spacing * Math.Abs(surplusIcons) + 21 - spacing + 6, 23); //background square for surplus/hunger
                //Draw food & surplus icons
                for (int i = 0; i < foodIcons; i++)
                {
                    graphics.DrawImage(Images.CitymapFoodLargeBigger, i * spacing + 3, 1);
                }
                for (int i = 0; i < Math.Abs(surplusIcons); i++)
                {
                    if (surplusIcons < 0)
                    {
                        graphics.DrawImage(Images.CitymapHungerLargeBigger, x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing) + i * spacing, 1); //hunger
                    }
                    else
                    {
                        graphics.DrawImage(Images.CitymapFoodLargeBigger, x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing) + i * spacing, 1); //hunger
                    }

                }

                //Next draw trade + corruption icons
                switch (tradeIcons + Math.Abs(corruptionIcons))
                {
                    case int n when (n >= 1 && n <= 15): { spacing = 23; break; }    //50 % larger (orignal = 15, 1 pixel gap)
                    case int n when (n == 16 || n == 17): { spacing = 20; break; }   //50 % larger (orignal = 13, 1 pixel overlap)
                    case int n when (n == 18 || n == 19): { spacing = 17; break; }   //50 % larger (orignal = 11, 3 pixel overlap)
                    case int n when (n == 20 || n == 21): { spacing = 15; break; }   //50 % larger (orignal = 10, 4 pixel overlap)
                    case int n when (n == 22 || n == 23): { spacing = 14; break; }   //50 % larger (orignal = 9, 5 pixel overlap)
                    case int n when (n == 24 || n == 25): { spacing = 12; break; }   //50 % larger (orignal = 8, 6 pixel overlap)
                    case int n when (n >= 26 && n <= 29): { spacing = 11; break; }   //50 % larger (orignal = 7, 7 pixel overlap)
                    case int n when (n >= 30 && n <= 33): { spacing = 9; break; }    //50 % larger (orignal = 6, 8 pixel overlap)
                    case int n when (n >= 34 && n <= 37): { spacing = 8; break; }    //50 % larger (orignal = 5, 9 pixel overlap)
                    case int n when (n >= 38 && n <= 49): { spacing = 6; break; }    //50 % larger (orignal = 4, 10 pixel overlap)
                    case int n when (n >= 50 && n <= 65): { spacing = 5; break; }    //50 % larger (orignal = 3, 11 pixel overlap)
                    case int n when (n >= 66): { spacing = 3; break; }               //50 % larger (orignal = 2, 12 pixel overlap)
                    default: { spacing = 2; break; }
                }
                //First draw background rectangle
                //graphics.FillRectangle(new SolidBrush(Color.FromArgb(71, 147, 31)), 0, 0, spacing * foodIcons + 21 - spacing + 6, 23); //background square for food
                //graphics.FillRectangle(new SolidBrush(Color.FromArgb(55, 123, 23)), x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing + 3), 0, spacing * Math.Abs(surplusIcons) + 21 - spacing + 6, 23); //background square for surplus/hunger
                //Draw trade & corruption icons
                for (int i = 0; i < tradeIcons; i++)
                {
                    graphics.DrawImage(Images.CitymapTradeLargeBigger, i * spacing + 3, 63);
                }
                for (int i = 0; i < Math.Abs(corruptionIcons); i++)
                {

                    graphics.DrawImage(Images.CitymapCorruptionLargeBigger, x_size - (spacing * Math.Abs(corruptionIcons) + 21 - spacing) + i * spacing, 63); //hunger
                }

                //Next draw tax+lux+sci icons
                switch (taxIcons + luxIcons + sciIcons)
                {
                    case int n when (n >= 1 && n <= 15): { spacing = 23; break; }    //50 % larger (orignal = 15, 1 pixel gap)
                    case int n when (n == 16 || n == 17): { spacing = 20; break; }   //50 % larger (orignal = 13, 1 pixel overlap)
                    case int n when (n == 18 || n == 19): { spacing = 17; break; }   //50 % larger (orignal = 11, 3 pixel overlap)
                    case int n when (n == 20 || n == 21): { spacing = 15; break; }   //50 % larger (orignal = 10, 4 pixel overlap)
                    case int n when (n == 22 || n == 23): { spacing = 14; break; }   //50 % larger (orignal = 9, 5 pixel overlap)
                    case int n when (n == 24 || n == 25): { spacing = 12; break; }   //50 % larger (orignal = 8, 6 pixel overlap)
                    case int n when (n >= 26 && n <= 29): { spacing = 11; break; }   //50 % larger (orignal = 7, 7 pixel overlap)
                    case int n when (n >= 30 && n <= 33): { spacing = 9; break; }    //50 % larger (orignal = 6, 8 pixel overlap)
                    case int n when (n >= 34 && n <= 37): { spacing = 8; break; }    //50 % larger (orignal = 5, 9 pixel overlap)
                    case int n when (n >= 38 && n <= 49): { spacing = 6; break; }    //50 % larger (orignal = 4, 10 pixel overlap)
                    case int n when (n >= 50 && n <= 65): { spacing = 5; break; }    //50 % larger (orignal = 3, 11 pixel overlap)
                    case int n when (n >= 66): { spacing = 3; break; }               //50 % larger (orignal = 2, 12 pixel overlap)
                    default: { spacing = 2; break; }
                }
                //First draw background rectangle
                //graphics.FillRectangle(new SolidBrush(Color.FromArgb(71, 147, 31)), 0, 0, spacing * foodIcons + 21 - spacing + 6, 23); //background square for food
                //graphics.FillRectangle(new SolidBrush(Color.FromArgb(55, 123, 23)), x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing + 3), 0, spacing * Math.Abs(surplusIcons) + 21 - spacing + 6, 23); //background square for surplus/hunger
                //Draw trade & corruption icons
                for (int i = 0; i < taxIcons; i++)
                {
                    graphics.DrawImage(Images.CitymapTaxLargeBigger, i * spacing + 3, 99);  //tax
                }
                for (int i = 0; i < luxIcons; i++)
                {
                    //TO-DO !!!
                    //graphics.DrawImage(Images.CitymapLuxLargeBigger, i * spacing + 3, 99);  //lux
                }
                for (int i = 0; i < sciIcons; i++)
                {
                    graphics.DrawImage(Images.CitymapSciLargeBigger, x_size - (spacing * sciIcons + 21 - spacing) + i * spacing, 99); //sci
                }

                //Next draw support+production icons
                switch (supportIcons + productionIcons)
                {
                    case int n when (n >= 1 && n <= 15): { spacing = 23; break; }    //50 % larger (orignal = 15, 1 pixel gap)
                    case int n when (n == 16 || n == 17): { spacing = 20; break; }   //50 % larger (orignal = 13, 1 pixel overlap)
                    case int n when (n == 18 || n == 19): { spacing = 17; break; }   //50 % larger (orignal = 11, 3 pixel overlap)
                    case int n when (n == 20 || n == 21): { spacing = 15; break; }   //50 % larger (orignal = 10, 4 pixel overlap)
                    case int n when (n == 22 || n == 23): { spacing = 14; break; }   //50 % larger (orignal = 9, 5 pixel overlap)
                    case int n when (n == 24 || n == 25): { spacing = 12; break; }   //50 % larger (orignal = 8, 6 pixel overlap)
                    case int n when (n >= 26 && n <= 29): { spacing = 11; break; }   //50 % larger (orignal = 7, 7 pixel overlap)
                    case int n when (n >= 30 && n <= 33): { spacing = 9; break; }    //50 % larger (orignal = 6, 8 pixel overlap)
                    case int n when (n >= 34 && n <= 37): { spacing = 8; break; }    //50 % larger (orignal = 5, 9 pixel overlap)
                    case int n when (n >= 38 && n <= 49): { spacing = 6; break; }    //50 % larger (orignal = 4, 10 pixel overlap)
                    case int n when (n >= 50 && n <= 65): { spacing = 5; break; }    //50 % larger (orignal = 3, 11 pixel overlap)
                    case int n when (n >= 66): { spacing = 3; break; }               //50 % larger (orignal = 2, 12 pixel overlap)
                    default: { spacing = 2; break; }
                }
                //First draw background rectangle
                //graphics.FillRectangle(new SolidBrush(Color.FromArgb(71, 147, 31)), 0, 0, spacing * foodIcons + 21 - spacing + 6, 23); //background square for food
                //graphics.FillRectangle(new SolidBrush(Color.FromArgb(55, 123, 23)), x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing + 3), 0, spacing * Math.Abs(surplusIcons) + 21 - spacing + 6, 23); //background square for surplus/hunger
                //Draw trade & corruption icons
                for (int i = 0; i < supportIcons; i++)
                {
                    graphics.DrawImage(Images.CitymapSupportLargeBigger, i * spacing + 3, 161);  //support
                }
                for (int i = 0; i < productionIcons; i++)
                {
                    graphics.DrawImage(Images.CitymapSupportLargeBigger, x_size - (spacing * productionIcons + 21 - spacing) + i * spacing, 161); //production
                }

            }
            return icons;
        }
    }
}
