using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using RTciv2.Units;
using RTciv2.Enums;

namespace RTciv2.Imagery
{
    public class DrawFunctions
    {
        //Draw entire game map
        //public static Bitmap DrawMap()
        //{
        //    Bitmap map = new Bitmap(Game.Data.MapXdim * 64 + 32, Game.Data.MapYdim * 32 + 16);    //define a bitmap for drawing map
            
        //    Squares square = new Squares();
                       
        //    using (Graphics graphics = Graphics.FromImage(map))
        //    {
        //        for (int col = 0; col < Game.Data.MapXdim; col++)
        //        {
        //            for (int row = 0; row < Game.Data.MapYdim; row++)
        //            {
        //                graphics.DrawImage(square.Terrain(col, row), 64 * col + 32 * (row % 2) + 1, 16 * row + 1);
        //            }
        //        }
        //    }
            
        //    return map;
        //}

        //Draw unit type (not in game units and their stats, just unit types for e.g. defense minister statistics)
        public Bitmap DrawUnitType(int Id, int civId)  //Id = order from RULES.TXT, civId = id of civ (0=barbarian)
        {
            Bitmap square = new Bitmap(64, 48);     //define a bitmap for drawing

            using (Graphics graphics = Graphics.FromImage(square))
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                //draw unit shields
                //First determine if the shield is on the left or right side
                int firstShieldXLoc = Images.unitShieldLocation[Id, 0];
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
                graphics.DrawImage(Images.UnitShieldShadow, Images.unitShieldLocation[Id, 0] + borderShieldOffset, Images.unitShieldLocation[Id, 1]); //shield shadow
                graphics.DrawImage(Images.UnitShield[civId], Images.unitShieldLocation[Id, 0], Images.unitShieldLocation[Id, 1]); //main shield
                graphics.DrawString("-", new Font("Arial", 8.0f), new SolidBrush(Color.Black), Images.unitShieldLocation[Id, 0] + 6, Images.unitShieldLocation[Id, 1] + 12, sf);    //Action on shield
                graphics.DrawImage(Images.Units[Id], 0, 0);    //draw unit
                sf.Dispose();
            }

            return square;
        }

        //Draw unit
        public Bitmap DrawUnit(IUnit unit, bool stacked, double scale_factor)
        {            
            Bitmap square = new Bitmap(64, 48);     //define a bitmap for drawing       

            using (Graphics graphics = Graphics.FromImage(square))
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                string shieldText;
                switch (unit.Order)
                {
                    case OrderType.Fortify: shieldText = "f"; break;
                    case OrderType.Fortified: shieldText = "F"; break;
                    case OrderType.Sleep: shieldText = "S"; break;
                    case OrderType.BuildFortress: shieldText = "bf"; break;
                    case OrderType.BuildRoad: shieldText = "R"; break;
                    case OrderType.BuildIrrigation: shieldText = "I"; break;
                    case OrderType.BuildMine: shieldText = "M"; break;
                    case OrderType.Transform: shieldText = "O"; break;
                    case OrderType.CleanPollution: shieldText = "P"; break;
                    case OrderType.BuildAirbase: shieldText = "A"; break;
                    case OrderType.GoTo: shieldText = "G"; break;
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

                //Determine hitpoints bar size
                int hitpointsBarX = (int)Math.Floor((float)unit.HitPoints * 12 / (float)unit.MaxHitPoints);
                Color hitpointsColor;
                if (hitpointsBarX <= 3) hitpointsColor = Color.FromArgb(243, 0, 0);
                else if (hitpointsBarX >= 4 && hitpointsBarX <= 8) hitpointsColor = Color.FromArgb(255, 223, 79);
                else hitpointsColor = Color.FromArgb(87, 171, 39);

                if (stacked)    //draw dark shield if unit is stacked on top of others
                {
                    graphics.DrawImage(Images.UnitShieldShadow, secondShieldBorderXLoc, Images.unitShieldLocation[(int)unit.Type, 1]); //shield shadow
                    graphics.DrawImage(Images.NoBorderUnitShield[(int)unit.CivId], secondShieldXLoc, Images.unitShieldLocation[(int)unit.Type, 1]);   //dark shield
                }

                //shield shadow
                graphics.DrawImage(Images.UnitShieldShadow, Images.unitShieldLocation[(int)unit.Type, 0] + borderShieldOffset, Images.unitShieldLocation[(int)unit.Type, 1]);

                //main shield
                graphics.DrawImage(Images.UnitShield[(int)unit.CivId], Images.unitShieldLocation[(int)unit.Type, 0], Images.unitShieldLocation[(int)unit.Type, 1]);

                //Draw black background for hitpoints bar
                graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(Images.unitShieldLocation[(int)unit.Type, 0], Images.unitShieldLocation[(int)unit.Type, 1] + 2, 12, 3));

                //Draw hitpoints bar
                graphics.FillRectangle(new SolidBrush(hitpointsColor), new Rectangle(Images.unitShieldLocation[(int)unit.Type, 0], Images.unitShieldLocation[(int)unit.Type, 1] + 2, hitpointsBarX, 3));

                //Action on shield
                graphics.DrawString(shieldText, new Font("Arial", 8.0f), new SolidBrush(Color.Black), Images.unitShieldLocation[(int)unit.Type, 0] + 6, Images.unitShieldLocation[(int)unit.Type, 1] + 12, sf);    

                if (unit.Order != OrderType.Sleep)
                {
                    graphics.DrawImage(Images.Units[(int)unit.Type], 0, 0);    //draw unit
                }
                else
                {
                    graphics.DrawImage(Images.Units[(int)unit.Type], new Rectangle(0, 0, 64, 48), 0, 0, 64, 48, GraphicsUnit.Pixel, ModifyImage.ConvertToGray());    //draw sentry unit
                }

                //draw fortification
                if (unit.Order == OrderType.Fortified) graphics.DrawImage(Images.Fortified, 0, 0);

                sf.Dispose();
            }

            //Resize image if required
            square = ModifyImage.ResizeImage(square, (int)(64 * scale_factor), (int)(48 * scale_factor));

            return square;
        }
                
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
            if (Game.Units.Any(unit => unit.X == city.X && unit.Y == city.Y)) flagPresent = true;

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
        
        //public Bitmap DrawCityFormMap(City city)    //Draw terrain in city form
        //{
        //    Bitmap map = new Bitmap(4 * 64, 4 * 32);    //define a bitmap for drawing map

        //    Squares square = new Squares();

        //    Bitmap image;
        //    using (Graphics graphics = Graphics.FromImage(map))
        //    {
        //        for (int x_ = -3; x_ <= 3; x_++)
        //        {
        //            for (int y_ = -3; y_ <= 3; y_++)
        //            {
        //                if ((x_ == -1 & y_ == -3) || (x_ == 1 & y_ == -3) || (x_ == -2 & y_ == -2) || (x_ == 0 & y_ == -2) || (x_ == 2 & y_ == -2) || (x_ == -3 & y_ == -1) || (x_ == -1 & y_ == -1) || (x_ == 1 & y_ == -1) || (x_ == 3 & y_ == -1) || (x_ == -2 & y_ == 0) || (x_ == 0 & y_ == 0) || (x_ == 2 & y_ == 0) || (x_ == -3 & y_ == 1) || (x_ == -1 & y_ == 1) || (x_ == 1 & y_ == 1) || (x_ == 3 & y_ == 1) || (x_ == -2 & y_ == 2) || (x_ == 0 & y_ == 2) || (x_ == 2 & y_ == 2) || (x_ == -1 & y_ == 3) || (x_ == 1 & y_ == 3))
        //                {
        //                    int newX = city.X2 + x_;
        //                    int newY = city.Y2 + y_;
        //                    if (newX >= 0 && newX < 2 * Game.Data.MapXdim && newY >= 0 && newY < Game.Data.MapYdim) image = square.Terrain((newX - (newY % 2)) / 2, newY);
        //                    else image = Images.Blank;
        //                    graphics.DrawImage(image, 32 * (x_ + 3), 16 * (y_ + 3));
        //                }
        //            }
        //        }
        //        graphics.DrawImage(DrawCity(city, false), 64 * 1 + 32 * (3 % 2) + 1, 16 * 2 + 1);
        //    }

        //    return map;
        //}

        //Draw food/shields/trade icons in city form sqare which is offset (offsetX, offsetY) from the square with the city
        //public Bitmap DrawCityFormMapIcons(City city, int offsetX, int offsetY)
        //{            
        //    offsetX = (offsetX - (offsetY % 2)) / 2;    //First turn offsetX/Y from Civ2 to real coordinates

        //    Bitmap icons = new Bitmap(64, 32);    //define a bitmap for drawing icons
        //    using (Graphics graphics = Graphics.FromImage(icons))
        //    {
        //        //First count all icons on this square to determine the spacing between icons (10 = no spacing, 15 = no spacing @ 50% scaled)
        //        int spacing;
        //        int countF = Game.TerrainTile[city.X + offsetX, city.Y + offsetY].Food;
        //        int countS = Game.TerrainTile[city.X + offsetX, city.Y + offsetY].Shields;
        //        int countT = Game.TerrainTile[city.X + offsetX, city.Y + offsetY].Trade;
        //        switch (countF + countS + countT)
        //        {
        //            case 1:
        //            case 2: spacing = 17; break;    //50 % larger (orignal = 11, 1 pixel gap)
        //            case 3: spacing = 15; break;    //50 % larger (orignal = 10, no gap)
        //            case 4: spacing = 11; break;    //50 % larger (orignal = 7)
        //            case 5: spacing = 8; break;    //50 % larger (orignal = 5)
        //            case 6: spacing = 6; break;    //50 % larger (orignal = 4)
        //            case 7:
        //            case 8: spacing = 5; break;    //50 % larger (orignal = 3)
        //            case 9: spacing = 3; break;    //50 % larger (orignal = 2)
        //            case 10: spacing = 2; break;    //50 % larger (orignal = 1)
        //            default: spacing = 2; break;    //50 % larger (orignal = 1)
        //        }
        //        //First draw food, then shields, then trade icons
        //        for (int i = 0; i < countF; i++) graphics.DrawImage(Images.CitymapFoodSmallBigger, i * spacing, 0);
        //        for (int i = 0; i < countS; i++) graphics.DrawImage(Images.CitymapShieldSmallBigger, (countF + i) * spacing, 0);
        //        for (int i = 0; i < countT; i++) graphics.DrawImage(Images.CitymapTradeSmallBigger, (countF + countS + i) * spacing, 0);
        //    }
        //    return icons;
        //}

        //public Bitmap DrawFaces(City city, double scale_factor) //Draw faces in cityform
        //{
        //    Bitmap faces = new Bitmap(630, 50);
        //    using (Graphics graphics = Graphics.FromImage(faces))
        //    {
        //        int spacing;
        //        switch (city.Size)
        //        {
        //            case int n when (n <= 15): spacing = 42; break;    //50 % larger (orignal = 28)
        //            case int n when (n == 16): spacing = 39; break;    //50 % larger (orignal = 26)
        //            case int n when (n == 17): spacing = 36; break;    //50 % larger (orignal = 24)
        //            case int n when (n == 18): spacing = 35; break;    //50 % larger (orignal = 23)
        //            case int n when (n == 19): spacing = 32; break;    //50 % larger (orignal = 21)
        //            case int n when (n == 20): spacing = 30; break;    //50 % larger (orignal = 20)
        //            case int n when (n == 21): spacing = 29; break;   //50 % larger (orignal = 19)
        //            case int n when (n == 22): spacing = 27; break;   //50 % larger (orignal = 18)
        //            case int n when (n == 23 || n == 24): spacing = 26; break;   //50 % larger (orignal = 17)
        //            case int n when (n == 25): spacing = 24; break;   //50 % larger (orignal = 16)
        //            case int n when (n == 26 || n == 27): spacing = 23; break;   //50 % larger (orignal = 15)
        //            case int n when (n == 28 || n == 29): spacing = 21; break;   //50 % larger (orignal = 14)
        //            case int n when (n == 30 || n == 31): spacing = 20; break;   //50 % larger (orignal = 13)
        //            case int n when (n == 32 || n == 33): spacing = 18; break;   //50 % larger (orignal = 12)
        //            case int n when (n >= 34 && n <= 36): spacing = 17; break;   //50 % larger (orignal = 11)
        //            case int n when (n >= 37 && n <= 41): spacing = 15; break;   //50 % larger (orignal = 10)
        //            case int n when (n == 42 || n == 43): spacing = 14; break;   //50 % larger (orignal = 9)
        //            case int n when (n >= 44 && n <= 50): spacing = 12; break;   //50 % larger (orignal = 8)
        //            case int n when (n >= 51 && n <= 57): spacing = 11; break;   //50 % larger (orignal = 7)
        //            case int n when (n >= 58 && n <= 66): spacing = 9; break;   //50 % larger (orignal = 6)
        //            case int n when (n >= 67 && n <= 79): spacing = 8; break;   //50 % larger (orignal = 5)
        //            case int n when (n >= 80 && n <= 99): spacing = 6; break;   //50 % larger (orignal = 4)
        //            case int n when (n >= 100): spacing = 5; break;   //50 % larger (orignal = 3)
        //            default: spacing = 30; break;
        //        }
        //        //Draw icons
        //        for (int i = 0; i < city.Size; i++)
        //        {
        //            graphics.DrawImage(ModifyImage.ResizeImage(Images.PeopleLshadow[2 + i % 2, 0], (int)(27 * scale_factor), (int)(30 * scale_factor)), i * spacing + 1, 1);  //shadow
        //            graphics.DrawImage(ModifyImage.ResizeImage(Images.PeopleL[2 + i % 2, 0], (int)(27 * scale_factor), (int)(30 * scale_factor)), i * spacing, 0);  //man-woman exchange turns
        //        }
        //    }            

        //    return faces;
        //}

        ////Draw icons in city resources (surplus < 0 is hunger)
        //public Bitmap DrawCityIcons(City city, int foodIcons, int surplusIcons, int tradeIcons, int corruptionIcons, int taxIcons, int luxIcons, int sciIcons, int supportIcons, int productionIcons)
        //{
        //    int x_size = 330;
        //    int y_size = 200;
        //    Bitmap icons = new Bitmap(x_size, y_size);    //define a bitmap for drawing icons
        //    using (Graphics graphics = Graphics.FromImage(icons))
        //    {
        //        //Number of food+surplus/hunger icons determines spacing between icons
        //        int spacing;
        //        switch (foodIcons + Math.Abs(surplusIcons))
        //        {
        //            case int n when (n >= 1 && n <= 15): spacing = 23; break;    //50 % larger (orignal = 15, 1 pixel gap)
        //            case int n when (n == 16 || n == 17): spacing = 20; break;   //50 % larger (orignal = 13, 1 pixel overlap)
        //            case int n when (n == 18 || n == 19): spacing = 17; break;   //50 % larger (orignal = 11, 3 pixel overlap)
        //            case int n when (n == 20 || n == 21): spacing = 15; break;   //50 % larger (orignal = 10, 4 pixel overlap)
        //            case int n when (n == 22 || n == 23): spacing = 14; break;   //50 % larger (orignal = 9, 5 pixel overlap)
        //            case int n when (n == 24 || n == 25): spacing = 12; break;   //50 % larger (orignal = 8, 6 pixel overlap)
        //            case int n when (n >= 26 && n <= 29): spacing = 11; break;   //50 % larger (orignal = 7, 7 pixel overlap)
        //            case int n when (n >= 30 && n <= 33): spacing = 9; break;    //50 % larger (orignal = 6, 8 pixel overlap)
        //            case int n when (n >= 34 && n <= 37): spacing = 8; break;    //50 % larger (orignal = 5, 9 pixel overlap)
        //            case int n when (n >= 38 && n <= 49): spacing = 6; break;    //50 % larger (orignal = 4, 10 pixel overlap)
        //            case int n when (n >= 50 && n <= 65): spacing = 5; break;    //50 % larger (orignal = 3, 11 pixel overlap)
        //            case int n when (n >= 66): spacing = 3; break;               //50 % larger (orignal = 2, 12 pixel overlap)
        //            default: spacing = 2; break;
        //        }
        //        //First draw background rectangle
        //        graphics.FillRectangle(new SolidBrush(Color.FromArgb(71, 147, 31)), 0, 0, spacing * foodIcons + 21 - spacing + 6, 23); //background square for food
        //        graphics.FillRectangle(new SolidBrush(Color.FromArgb(55, 123, 23)), x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing + 3), 0, spacing * Math.Abs(surplusIcons) + 21 - spacing + 6, 23); //background square for surplus/hunger
        //        //Draw food & surplus icons
        //        for (int i = 0; i < foodIcons; i++) graphics.DrawImage(Images.CitymapFoodLargeBigger, i * spacing + 3, 1);
        //        for (int i = 0; i < Math.Abs(surplusIcons); i++)
        //        {
        //            if (surplusIcons < 0) graphics.DrawImage(Images.CitymapHungerLargeBigger, x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing) + i * spacing, 1); //hunger
        //            else graphics.DrawImage(Images.CitymapFoodLargeBigger, x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing) + i * spacing, 1); //hunger
        //        }

        //        //Next draw trade + corruption icons
        //        switch (tradeIcons + Math.Abs(corruptionIcons))
        //        {
        //            case int n when (n >= 1 && n <= 15): spacing = 23; break;    //50 % larger (orignal = 15, 1 pixel gap)
        //            case int n when (n == 16 || n == 17): spacing = 20; break;   //50 % larger (orignal = 13, 1 pixel overlap)
        //            case int n when (n == 18 || n == 19): spacing = 17; break;   //50 % larger (orignal = 11, 3 pixel overlap)
        //            case int n when (n == 20 || n == 21): spacing = 15; break;   //50 % larger (orignal = 10, 4 pixel overlap)
        //            case int n when (n == 22 || n == 23): spacing = 14; break;   //50 % larger (orignal = 9, 5 pixel overlap)
        //            case int n when (n == 24 || n == 25): spacing = 12; break;   //50 % larger (orignal = 8, 6 pixel overlap)
        //            case int n when (n >= 26 && n <= 29): spacing = 11; break;   //50 % larger (orignal = 7, 7 pixel overlap)
        //            case int n when (n >= 30 && n <= 33): spacing = 9; break;    //50 % larger (orignal = 6, 8 pixel overlap)
        //            case int n when (n >= 34 && n <= 37): spacing = 8; break;    //50 % larger (orignal = 5, 9 pixel overlap)
        //            case int n when (n >= 38 && n <= 49): spacing = 6; break;    //50 % larger (orignal = 4, 10 pixel overlap)
        //            case int n when (n >= 50 && n <= 65): spacing = 5; break;    //50 % larger (orignal = 3, 11 pixel overlap)
        //            case int n when (n >= 66): spacing = 3; break;               //50 % larger (orignal = 2, 12 pixel overlap)
        //            default: spacing = 2; break;
        //        }
        //        //First draw background rectangle
        //        //graphics.FillRectangle(new SolidBrush(Color.FromArgb(71, 147, 31)), 0, 0, spacing * foodIcons + 21 - spacing + 6, 23); //background square for food
        //        //graphics.FillRectangle(new SolidBrush(Color.FromArgb(55, 123, 23)), x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing + 3), 0, spacing * Math.Abs(surplusIcons) + 21 - spacing + 6, 23); //background square for surplus/hunger
        //        //Draw trade & corruption icons
        //        for (int i = 0; i < tradeIcons; i++) graphics.DrawImage(Images.CitymapTradeLargeBigger, i * spacing + 3, 63);

        //        for (int i = 0; i < Math.Abs(corruptionIcons); i++) graphics.DrawImage(Images.CitymapCorruptionLargeBigger, x_size - (spacing * Math.Abs(corruptionIcons) + 21 - spacing) + i * spacing, 63); //hunger

        //        //Next draw tax+lux+sci icons
        //        switch (taxIcons + luxIcons + sciIcons)
        //        {
        //            case int n when (n >= 1 && n <= 15): spacing = 23; break;    //50 % larger (orignal = 15, 1 pixel gap)
        //            case int n when (n == 16 || n == 17): spacing = 20; break;   //50 % larger (orignal = 13, 1 pixel overlap)
        //            case int n when (n == 18 || n == 19): spacing = 17; break;   //50 % larger (orignal = 11, 3 pixel overlap)
        //            case int n when (n == 20 || n == 21): spacing = 15; break;   //50 % larger (orignal = 10, 4 pixel overlap)
        //            case int n when (n == 22 || n == 23): spacing = 14; break;   //50 % larger (orignal = 9, 5 pixel overlap)
        //            case int n when (n == 24 || n == 25): spacing = 12; break;   //50 % larger (orignal = 8, 6 pixel overlap)
        //            case int n when (n >= 26 && n <= 29): spacing = 11; break;   //50 % larger (orignal = 7, 7 pixel overlap)
        //            case int n when (n >= 30 && n <= 33): spacing = 9; break;    //50 % larger (orignal = 6, 8 pixel overlap)
        //            case int n when (n >= 34 && n <= 37): spacing = 8; break;    //50 % larger (orignal = 5, 9 pixel overlap)
        //            case int n when (n >= 38 && n <= 49): spacing = 6; break;    //50 % larger (orignal = 4, 10 pixel overlap)
        //            case int n when (n >= 50 && n <= 65): spacing = 5; break;    //50 % larger (orignal = 3, 11 pixel overlap)
        //            case int n when (n >= 66): spacing = 3; break;               //50 % larger (orignal = 2, 12 pixel overlap)
        //            default: spacing = 2; break;
        //        }
        //        //First draw background rectangle
        //        //graphics.FillRectangle(new SolidBrush(Color.FromArgb(71, 147, 31)), 0, 0, spacing * foodIcons + 21 - spacing + 6, 23); //background square for food
        //        //graphics.FillRectangle(new SolidBrush(Color.FromArgb(55, 123, 23)), x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing + 3), 0, spacing * Math.Abs(surplusIcons) + 21 - spacing + 6, 23); //background square for surplus/hunger
        //        //Draw trade & corruption icons
        //        for (int i = 0; i < taxIcons; i++) graphics.DrawImage(Images.CitymapTaxLargeBigger, i * spacing + 3, 99);  //tax

        //        for (int i = 0; i < luxIcons; i++)
        //        {
        //            //TO-DO !!!
        //            //graphics.DrawImage(Images.CitymapLuxLargeBigger, i * spacing + 3, 99);  //lux
        //        }
        //        for (int i = 0; i < sciIcons; i++) graphics.DrawImage(Images.CitymapSciLargeBigger, x_size - (spacing * sciIcons + 21 - spacing) + i * spacing, 99); //sci

        //        //Next draw support+production icons
        //        switch (supportIcons + productionIcons)
        //        {
        //            case int n when (n >= 1 && n <= 15): spacing = 23; break;    //50 % larger (orignal = 15, 1 pixel gap)
        //            case int n when (n == 16 || n == 17): spacing = 20; break;   //50 % larger (orignal = 13, 1 pixel overlap)
        //            case int n when (n == 18 || n == 19): spacing = 17; break;   //50 % larger (orignal = 11, 3 pixel overlap)
        //            case int n when (n == 20 || n == 21): spacing = 15; break;   //50 % larger (orignal = 10, 4 pixel overlap)
        //            case int n when (n == 22 || n == 23): spacing = 14; break;   //50 % larger (orignal = 9, 5 pixel overlap)
        //            case int n when (n == 24 || n == 25): spacing = 12; break;   //50 % larger (orignal = 8, 6 pixel overlap)
        //            case int n when (n >= 26 && n <= 29): spacing = 11; break;   //50 % larger (orignal = 7, 7 pixel overlap)
        //            case int n when (n >= 30 && n <= 33): spacing = 9; break;    //50 % larger (orignal = 6, 8 pixel overlap)
        //            case int n when (n >= 34 && n <= 37): spacing = 8; break;    //50 % larger (orignal = 5, 9 pixel overlap)
        //            case int n when (n >= 38 && n <= 49): spacing = 6; break;    //50 % larger (orignal = 4, 10 pixel overlap)
        //            case int n when (n >= 50 && n <= 65): spacing = 5; break;    //50 % larger (orignal = 3, 11 pixel overlap)
        //            case int n when (n >= 66): spacing = 3; break;               //50 % larger (orignal = 2, 12 pixel overlap)
        //            default: spacing = 2; break;
        //        }
        //        //First draw background rectangle
        //        //graphics.FillRectangle(new SolidBrush(Color.FromArgb(71, 147, 31)), 0, 0, spacing * foodIcons + 21 - spacing + 6, 23); //background square for food
        //        //graphics.FillRectangle(new SolidBrush(Color.FromArgb(55, 123, 23)), x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing + 3), 0, spacing * Math.Abs(surplusIcons) + 21 - spacing + 6, 23); //background square for surplus/hunger
        //        //Draw trade & corruption icons
        //        for (int i = 0; i < supportIcons; i++) graphics.DrawImage(Images.CitymapSupportLargeBigger, i * spacing + 3, 161);  //support

        //        for (int i = 0; i < productionIcons; i++) graphics.DrawImage(Images.CitymapSupportLargeBigger, x_size - (spacing * productionIcons + 21 - spacing) + i * spacing, 161); //production

        //    }
        //    return icons;
        //}

        //public Bitmap DrawFoodStorage(City city)
        //{
        //    Bitmap icons = new Bitmap(291, 244);    //define a bitmap for drawing icons
        //    using (Graphics graphics = Graphics.FromImage(icons))
        //    {
        //        int wheatW = 21;   //width. Original=14 (50% scaling).
        //        int wheatH = 21;   //height. Original=14 (50% scaling).

        //        //First determine spacing between wheat icons
        //        //NOTE (not 100% accurate, spacing also tends to switch between two numbers)
        //        int wheat_spacing;
        //        switch (city.Size)
        //        {
        //            case int n when (n <= 9): wheat_spacing = 26; break;  //original=17 (50% scaled)
        //            case int n when (n == 10): wheat_spacing = 24; break;  //original=16 (50% scaled)
        //            case int n when (n == 11): wheat_spacing = 20; break;  //original=13 (50% scaled)
        //            case int n when (n == 12): wheat_spacing = 18; break;  //original=12 (50% scaled)
        //            case int n when (n == 13): wheat_spacing = 17; break;  //original=11 (50% scaled)
        //            case int n when (n == 14): wheat_spacing = 15; break;  //original=10 (50% scaled)
        //            case int n when (n == 15 || n == 16): wheat_spacing = 14; break;  //original=9 (50% scaled)
        //            case int n when (n == 17): wheat_spacing = 12; break;  //original=8 (50% scaled)
        //            case int n when (n >= 18 && n <= 20): wheat_spacing = 11; break;  //original=7 (50% scaled)
        //            case int n when (n == 21 || n == 22): wheat_spacing = 9; break;  //original=6 (50% scaled)
        //            case int n when (n >= 23 && n <= 26): wheat_spacing = 8; break;  //original=5 (50% scaled)
        //            case int n when (n >= 27 && n <= 33): wheat_spacing = 6; break;  //original=4 (50% scaled)
        //            case int n when (n >= 34 && n <= 40): wheat_spacing = 5; break;  //original=3 (50% scaled)
        //            case int n when (n >= 41 && n <= 80): wheat_spacing = 3; break;  //original=2 (50% scaled)
        //            case int n when (n >= 81): wheat_spacing = 2; break;  //original=1 (50% scaled)
        //            default: wheat_spacing = 26; break;
        //        }

        //        //Draw rectangle around wheat icons     
        //        //1st horizontal line
        //        int line_width = (city.Size + 1) * wheat_spacing + wheatW - wheat_spacing + 2 + 5;
        //        int starting_x = (int)((291 - line_width) / 2);   //291 = width of drawing panel
        //        int starting_y = 23;    //original=15, this is 50 % scaled
        //        graphics.DrawLine(new Pen(Color.FromArgb(75, 155, 35)), starting_x, starting_y, starting_x + line_width, starting_y);
        //        //3rd horizontal line
        //        starting_y = 240;    //original=160, this is 50 % scaled
        //        graphics.DrawLine(new Pen(Color.FromArgb(0, 51, 0)), starting_x, starting_y, starting_x + line_width, starting_y);
        //        //1st vertical line
        //        starting_y = 23;
        //        int line_height = 216;  //original=144 (50% scaled)
        //        graphics.DrawLine(new Pen(Color.FromArgb(75, 155, 35)), starting_x, starting_y, starting_x, starting_y + line_height);
        //        //2nd vertical line
        //        graphics.DrawLine(new Pen(Color.FromArgb(0, 51, 0)), starting_x + line_width, starting_y, starting_x + line_width, starting_y + line_height);

        //        //Draw wheat icons
        //        int count = 0;
        //        starting_x += 3;    //wheat icons 2px to the right in original (50% scaled)
        //        for (int row = 0; row < 10; row++)
        //        {
        //            for (int col = 0; col <= city.Size; col++)
        //            {
        //                graphics.DrawImage(Images.CitymapFoodLargeBigger, starting_x + wheat_spacing * col, 27 + wheatH * row);
        //                count++;

        //                if (count >= city.FoodInStorage) break;
        //            }
        //            if (count >= city.FoodInStorage) break;
        //        }

        //        //3rd horizontal line (shorter)
        //        line_width -= 12;   //orignal=8 px shorter (50% scaled)
        //        starting_x -= 3;    //correct from above
        //        starting_x += 6;
        //        starting_y = 131;   //orignal=87 (50% scaled)
        //        graphics.DrawLine(new Pen(Color.FromArgb(75, 155, 35)), starting_x, starting_y, starting_x + line_width, starting_y);

        //        //Draw string
        //        StringFormat sf = new StringFormat();
        //        sf.Alignment = StringAlignment.Center;
        //        graphics.DrawString("Food Storage", new Font("Arial", 12), new SolidBrush(Color.Black), new Point(147, 5), sf);
        //        graphics.DrawString("Food Storage", new Font("Arial", 12), new SolidBrush(Color.FromArgb(75, 155, 35)), new Point(146, 4), sf);
        //        sf.Dispose();
        //    }

        //    return icons;
        //}

        //public Bitmap DrawCityProduction(City city)
        //{
        //    Bitmap icons = new Bitmap(293, 287);    //same size as production panel in city form
        //    using (Graphics graphics = Graphics.FromImage(icons))
        //    {
        //        //Draw rectangle around icons
        //        int IIP = city.ItemInProduction;
        //        int cost;
        //        if (IIP < 62) cost = ReadFiles.UnitCost[IIP];   //Item is unit
        //        else cost = ReadFiles.ImprovementCost[IIP - 62 + 1];    //Item is improvement (first 62 are units, +1 because first improfement is "Nothing")
        //        int vertSpacing = Math.Min(10, cost);    //max 10 lines
        //        graphics.DrawLine(new Pen(Color.FromArgb(83, 103, 191)), 9, 65, 9 + 271, 65);   //1st horizontal
        //        graphics.DrawLine(new Pen(Color.FromArgb(83, 103, 191)), 9, 65, 9, 65 + 27 + (vertSpacing - 1) * 21);   //1st vertical
        //        graphics.DrawLine(new Pen(Color.FromArgb(0, 0, 95)), 9, 65 + 27 + (vertSpacing - 1) * 21, 9 + 271, 65 + 27 + (vertSpacing - 1) * 21);   //2nd horizontal
        //        graphics.DrawLine(new Pen(Color.FromArgb(0, 0, 95)), 9 + 271, 65, 9 + 271, 65 + 27 + (vertSpacing - 1) * 21);   //2nd vertical

        //        //Draw icons
        //        int count = 0;
        //        for (int row = 0; row < Math.Min(cost, 10); row++)   //there are never more than 10 rows
        //        {
        //            for (int col = 0; col < Math.Max(cost, 10); col++)  //there are never less than 10 columns
        //            {
        //                int dx = Convert.ToInt32(2 + col * (272 - 21 - 4) / ((float)Math.Max(cost, 10) - 1)); //horizontal separation between icons
        //                int dy = 21;    //vertical separation of icons (space between icons in y-directions is always 0)
        //                graphics.DrawImage(Images.CitymapSupportLargeBigger, 10 + dx, 65 + 3 + dy * row);

        //                count++;
        //                if (count >= city.ShieldsProgress) break;
        //            }
        //            if (count >= city.ShieldsProgress) break;
        //        }
        //    }

        //    return icons;
        //}
    }
}
