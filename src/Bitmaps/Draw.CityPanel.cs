using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using civ2.Enums;
using civ2.Units;

namespace civ2.Bitmaps
{
    public static partial class Draw
    {
        public static Bitmap FoodStorage(City city)
        {
            var panel = new Bitmap(195, 163);
            using (Graphics g = Graphics.FromImage(panel))
            {
                // Icons
                int wheatW = 14;   //width. Norm=14, big=21
                int wheatH = 14;   //height. Norm=14, big=21

                // First determine spacing between wheat icons
                // NOTE: not 100% accurate, spacing also tends to switch between two numbers
                int wheat_spacing;
                switch (city.Size)
                {
                    case int n when (n <= 9): wheat_spacing = 17; break;  // norm=17, big=26
                    case int n when (n == 10): wheat_spacing = 16; break;  // norm=16, big=24
                    case int n when (n == 11): wheat_spacing = 13; break;  // norm=13, big=20
                    case int n when (n == 12): wheat_spacing = 12; break;  // norm=12, big=18
                    case int n when (n == 13): wheat_spacing = 11; break;  // norm=11, big=17
                    case int n when (n == 14): wheat_spacing = 10; break;  // norm=10, big=15
                    case int n when (n == 15 || n == 16): wheat_spacing = 9; break;  // norm=9, big=14
                    case int n when (n == 17): wheat_spacing = 8; break;  // norm=8, big=12
                    case int n when (n >= 18 && n <= 20): wheat_spacing = 7; break;  // norm=7, big=11
                    case int n when (n == 21 || n == 22): wheat_spacing = 6; break;  // norm=6, big=9
                    case int n when (n >= 23 && n <= 26): wheat_spacing = 5; break;  // norm=5, big=8
                    case int n when (n >= 27 && n <= 33): wheat_spacing = 4; break;  // norm=4, big=6
                    case int n when (n >= 34 && n <= 40): wheat_spacing = 3; break;  // norm=3, big=5
                    case int n when (n >= 41 && n <= 80): wheat_spacing = 2; break;  // norm=2, big=3
                    case int n when (n >= 81): wheat_spacing = 1; break;  // norm=1, big=2
                    default: wheat_spacing = 17; break;
                }

                // Draw rectangle around wheat icons     
                // 1st horizontal line
                int line_width = city.Size * wheat_spacing + wheatW + 7;
                int starting_x = (panel.Width - line_width) / 2;
                int starting_y = 0;
                g.DrawLine(new Pen(Color.FromArgb(75, 155, 35)), starting_x, starting_y, starting_x + line_width, starting_y);
                // 3rd horizontal line
                starting_y = 145;    // norm=145, big=?
                g.DrawLine(new Pen(Color.FromArgb(0, 51, 0)), starting_x, starting_y, starting_x + line_width, starting_y);
                // 1st vertical line
                starting_y = 0;
                int line_height = 144;  // norm=144, big=216
                g.DrawLine(new Pen(Color.FromArgb(75, 155, 35)), starting_x, starting_y, starting_x, starting_y + line_height);
                // 2nd vertical line
                g.DrawLine(new Pen(Color.FromArgb(0, 51, 0)), starting_x + line_width, starting_y, starting_x + line_width, starting_y + line_height);

                // Draw wheat icons
                int count = 0;
                starting_x += 3;    // norm=2px, big=3px
                for (int row = 0; row < 10; row++)
                {
                    for (int col = 0; col <= city.Size; col++)
                    {
                        g.DrawImage(Images.CityFoodBig, starting_x + wheat_spacing * col, 3 + wheatH * row);
                        count++;

                        if (count >= city.FoodInStorage) break;
                    }
                    if (count >= city.FoodInStorage) break;
                }

                // 3rd horizontal line (shorter)
                line_width -= 10;   // norm=8, big=12
                starting_x += 2;
                starting_y = 72;   // norm=72, big=?
                g.DrawLine(new Pen(Color.FromArgb(75, 155, 35)), starting_x, starting_y, starting_x + line_width, starting_y);
            }

            return panel;
        }

        // Draw icons in city resources (surplus < 0 is hunger)
        public static Bitmap CityResources(City city)//, int foodIcons, int surplusIcons, int tradeIcons, int corruptionIcons, int taxIcons, int luxIcons, int sciIcons, int supportIcons, int productionIcons)
        {
            int x_size = 226;
            int y_size = 151;
            var icons = new Bitmap(x_size, y_size);
            using (Graphics g = Graphics.FromImage(icons))
            {
                using var sf1 = new StringFormat();
                using var sf2 = new StringFormat();
                sf1.Alignment = StringAlignment.Far;
                sf2.Alignment = StringAlignment.Center;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

                // FOOD
                // Text
                g.DrawString($"Food: {city.Food}", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.Black), new Point(1, 1));
                g.DrawString($"Food: {city.Food}", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(87, 171, 39)), new Point(0, 0));
                g.DrawString($"Surplus: {city.SurplusHunger}", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.Black), new Point(226, 1), sf1);
                g.DrawString($"Surplus: {city.SurplusHunger}", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(63, 139, 31)), new Point(225, 0), sf1);
                // Number of food+surplus/hunger icons determines spacing between icons
                int spacing;
                switch (city.Food + Math.Abs(city.SurplusHunger))
                {
                    case int n when (n >= 1 && n <= 15): spacing = 15; break;    // norm=15, big=23, gap=1px
                    case int n when (n == 16 || n == 17): spacing = 13; break;   // norm=13, big=20, overlap=1px
                    case int n when (n == 18 || n == 19): spacing = 11; break;   // norm=11, big=17, overlap=3px
                    case int n when (n == 20 || n == 21): spacing = 10; break;   // norm=10, big=15, overlap=4px
                    case int n when (n == 22 || n == 23): spacing = 9; break;    // norm=9, big=14, overlap=5px
                    case int n when (n == 24 || n == 25): spacing = 8; break;    // norm=8, big=12, overlap=6px
                    case int n when (n >= 26 && n <= 29): spacing = 7; break;    // norm=7, big=11, overlap=7px
                    case int n when (n >= 30 && n <= 33): spacing = 6; break;    // norm=6, big=9, overlap=8px
                    case int n when (n >= 34 && n <= 37): spacing = 5; break;    // norm=5, big=8, overlap=9px
                    case int n when (n >= 38 && n <= 49): spacing = 4; break;    // norm=4, big=6, overlap=10px
                    case int n when (n >= 50 && n <= 65): spacing = 3; break;    // norm=3, big=5, overlap=11px
                    case int n when (n >= 66): spacing = 2; break;               // norm=2, big=3, overlap=12px
                    default: spacing = 2; break;
                }
                // TODO: Draw background rectangle
                //g.FillRectangle(new SolidBrush(Color.FromArgb(71, 147, 31)), 0, 0, spacing * city.Food + 21 - spacing + 6, 23); // Background square for food
                //g.FillRectangle(new SolidBrush(Color.FromArgb(55, 123, 23)), x_size - (spacing * Math.Abs(city.SurplusHunger) + 21 - spacing + 3), 
                //    0, spacing * Math.Abs(city.SurplusHunger) + 21 - spacing + 6, 23); // Background square for surplus/hunger
                // Icons
                for (int i = 0; i < city.Food; i++) g.DrawImage(Images.CityFoodBig, i * spacing + 3, 15);
                for (int i = 0; i < Math.Abs(city.SurplusHunger); i++)
                {
                    if (city.SurplusHunger < 0) g.DrawImage(Images.CityHungerBig, x_size - (spacing * Math.Abs(city.SurplusHunger) + 14 - spacing) + i * spacing, 15); // Hunger
                    else g.DrawImage(Images.CityFoodBig, x_size - (spacing * Math.Abs(city.SurplusHunger) + 14 - spacing) + i * spacing, 15); // Surplus
                }

                // TRADE
                // Text
                g.DrawString($"Trade: {city.Trade}", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.Black), new Point(1, 42));
                g.DrawString($"Trade: {city.Trade}", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(239, 159, 7)), new Point(0, 41));
                g.DrawString($"Corruption: {city.Corruption}", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.Black), new Point(226, 42), sf1);
                g.DrawString($"Corruption: {city.Corruption}", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(227, 83, 15)), new Point(225, 41), sf1);
                // Spacing between icons
                switch (city.Trade + city.Corruption)
                {
                    case int n when (n >= 1 && n <= 15): spacing = 15; break;    // norm=15, big=23, gap=1px
                    case int n when (n == 16 || n == 17): spacing = 13; break;   // norm=13, big=20, overlap=1px
                    case int n when (n == 18 || n == 19): spacing = 11; break;   // norm=11, big=17, overlap=3px
                    case int n when (n == 20 || n == 21): spacing = 10; break;   // norm=10, big=15, overlap=4px
                    case int n when (n == 22 || n == 23): spacing = 9; break;    // norm=9, big=14, overlap=5px
                    case int n when (n == 24 || n == 25): spacing = 8; break;    // norm=8, big=12, overlap=6px
                    case int n when (n >= 26 && n <= 29): spacing = 7; break;    // norm=7, big=11, overlap=7px
                    case int n when (n >= 30 && n <= 33): spacing = 6; break;    // norm=6, big=9, overlap=8px
                    case int n when (n >= 34 && n <= 37): spacing = 5; break;    // norm=5, big=8, overlap=9px
                    case int n when (n >= 38 && n <= 49): spacing = 4; break;    // norm=4, big=6, overlap=10px
                    case int n when (n >= 50 && n <= 65): spacing = 3; break;    // norm=3, big=5, overlap=11px
                    case int n when (n >= 66): spacing = 2; break;               // norm=2, big=3, overlap=12px
                    default: spacing = 2; break;
                }
                // TODO: Draw background rectangle
                //g.FillRectangle(new SolidBrush(Color.FromArgb(71, 147, 31)), 0, 0, spacing * foodIcons + 21 - spacing + 6, 23); //background square for food
                //g.FillRectangle(new SolidBrush(Color.FromArgb(55, 123, 23)), x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing + 3), 0, spacing * Math.Abs(surplusIcons) + 21 - spacing + 6, 23); //background square for surplus/hunger
                // Icons
                for (int i = 0; i < city.Trade; i++) g.DrawImage(Images.CityTradeBig, i * spacing + 3, 56);
                for (int i = 0; i < city.Corruption; i++) g.DrawImage(Images.CityCorruptBig, x_size - (spacing * city.Corruption + 14 - spacing) + i * spacing, 56);

                // TAX+LUX+SCI
                // Text
                g.DrawString($"50% Tax: {city.Tax}", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.Black), new Point(1, 96));
                g.DrawString($"50% Tax: {city.Tax}", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(239, 159, 7)), new Point(0, 95));
                g.DrawString($"0% Lux: {city.Lux}", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.Black), new Point(112, 96), sf2);
                g.DrawString($"0% Lux: {city.Lux}", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.White), new Point(111, 95), sf2);
                g.DrawString($"50% Sci: {city.Science}", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.Black), new Point(226, 96), sf1);
                g.DrawString($"50% Sci: {city.Science}", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(63, 187, 199)), new Point(225, 95), sf1);
                // Spacing between icons
                switch (city.Tax + city.Lux + city.Science)
                {
                    case int n when (n >= 1 && n <= 15): spacing = 15; break;    // norm=15, big=23, gap=1px
                    case int n when (n == 16 || n == 17): spacing = 13; break;   // norm=13, big=20, overlap=1px
                    case int n when (n == 18 || n == 19): spacing = 11; break;   // norm=11, big=17, overlap=3px
                    case int n when (n == 20 || n == 21): spacing = 10; break;   // norm=10, big=15, overlap=4px
                    case int n when (n == 22 || n == 23): spacing = 9; break;    // norm=9, big=14, overlap=5px
                    case int n when (n == 24 || n == 25): spacing = 8; break;    // norm=8, big=12, overlap=6px
                    case int n when (n >= 26 && n <= 29): spacing = 7; break;    // norm=7, big=11, overlap=7px
                    case int n when (n >= 30 && n <= 33): spacing = 6; break;    // norm=6, big=9, overlap=8px
                    case int n when (n >= 34 && n <= 37): spacing = 5; break;    // norm=5, big=8, overlap=9px
                    case int n when (n >= 38 && n <= 49): spacing = 4; break;    // norm=4, big=6, overlap=10px
                    case int n when (n >= 50 && n <= 65): spacing = 3; break;    // norm=3, big=5, overlap=11px
                    case int n when (n >= 66): spacing = 2; break;               // norm=2, big=3, overlap=12px
                    default: spacing = 2; break;
                }
                // TODO: Draw background rectangle
                //g.FillRectangle(new SolidBrush(Color.FromArgb(71, 147, 31)), 0, 0, spacing * foodIcons + 21 - spacing + 6, 23); //background square for food
                //g.FillRectangle(new SolidBrush(Color.FromArgb(55, 123, 23)), x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing + 3), 0, spacing * Math.Abs(surplusIcons) + 21 - spacing + 6, 23); //background square for surplus/hunger
                // Icons
                for (int i = 0; i < city.Tax; i++) g.DrawImage(Images.CityTaxBig, i * spacing + 3, 80);
                for (int i = 0; i < city.Science; i++) g.DrawImage(Images.CitySciBig, x_size - (spacing * city.Science + 14 - spacing) + i * spacing, 80);
                for (int i = 0; i < city.Lux; i++) g.DrawImage(Images.CityLuxBig, x_size / 2 - (spacing * city.Lux + 14 - spacing) / 2, 80);

                // SUPPORT+PRODUCTION
                // Text
                g.DrawString($"Support: {city.Support}", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.Black), new Point(1, 136));
                g.DrawString($"Support: {city.Support}", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(63, 79, 167)), new Point(0, 135));
                g.DrawString($"Production: {city.Production}", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.Black), new Point(226, 136), sf1);
                g.DrawString($"Production: {city.Production}", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(7, 11, 103)), new Point(225, 135), sf1);
                // Spacing between icons
                switch (city.Support + city.Production)
                {
                    case int n when (n >= 1 && n <= 15): spacing = 15; break;    // norm=15, big=23, gap=1px
                    case int n when (n == 16 || n == 17): spacing = 13; break;   // norm=13, big=20, overlap=1px
                    case int n when (n == 18 || n == 19): spacing = 11; break;   // norm=11, big=17, overlap=3px
                    case int n when (n == 20 || n == 21): spacing = 10; break;   // norm=10, big=15, overlap=4px
                    case int n when (n == 22 || n == 23): spacing = 9; break;    // norm=9, big=14, overlap=5px
                    case int n when (n == 24 || n == 25): spacing = 8; break;    // norm=8, big=12, overlap=6px
                    case int n when (n >= 26 && n <= 29): spacing = 7; break;    // norm=7, big=11, overlap=7px
                    case int n when (n >= 30 && n <= 33): spacing = 6; break;    // norm=6, big=9, overlap=8px
                    case int n when (n >= 34 && n <= 37): spacing = 5; break;    // norm=5, big=8, overlap=9px
                    case int n when (n >= 38 && n <= 49): spacing = 4; break;    // norm=4, big=6, overlap=10px
                    case int n when (n >= 50 && n <= 65): spacing = 3; break;    // norm=3, big=5, overlap=11px
                    case int n when (n >= 66): spacing = 2; break;               // norm=2, big=3, overlap=12px
                    default: spacing = 2; break;
                }
                // TODO: Draw background rectangle
                //g.FillRectangle(new SolidBrush(Color.FromArgb(71, 147, 31)), 0, 0, spacing * foodIcons + 21 - spacing + 6, 23); //background square for food
                //g.FillRectangle(new SolidBrush(Color.FromArgb(55, 123, 23)), x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing + 3), 0, spacing * Math.Abs(surplusIcons) + 21 - spacing + 6, 23); //background square for surplus/hunger
                // Icons
                for (int i = 0; i < city.Support; i++) g.DrawImage(Images.CitySupportBig, i * spacing + 3, 121);
                for (int i = 0; i < city.Production; i++) g.DrawImage(Images.CitySupportBig, x_size - (spacing * city.Production + 14 - spacing) + i * spacing, 121);
            }
            return icons;
        }

        // Draw faces in City Panel
        public static Bitmap Citizens(City city, int zoom)
        {
            Bitmap faces = new Bitmap(630, 50);
            using (Graphics graphics = Graphics.FromImage(faces))
            {
                int spacing;
                switch (city.Size)
                {
                    case int n when (n <= 15): spacing = 28; break;    // 28-42-xx (orig-50%larger-50%smaller)
                    case int n when (n == 16): spacing = 26; break;    // 26-39-xx (orig-50%larger-50%smaller)
                    case int n when (n == 17): spacing = 24; break;    // 24-36-xx (orig-50%larger-50%smaller)
                    case int n when (n == 18): spacing = 23; break;    // 23-35-xx (orig-50%larger-50%smaller)
                    case int n when (n == 19): spacing = 21; break;    // 21-32-xx (orig-50%larger-50%smaller)
                    case int n when (n == 20): spacing = 20; break;    // 20-30-xx (orig-50%larger-50%smaller)
                    case int n when (n == 21): spacing = 19; break;   // 19-29-xx (orig-50%larger-50%smaller)
                    case int n when (n == 22): spacing = 18; break;   // 18-27-xx (orig-50%larger-50%smaller)
                    case int n when (n == 23 || n == 24): spacing = 17; break; // 17-26-xx (orig - 50 % larger - 50 % smaller)
                    case int n when (n == 25): spacing = 16; break;   // 16-24-xx (orig - 50 % larger - 50 % smaller)
                    case int n when (n == 26 || n == 27): spacing = 15; break;   // 15-23-xx (orig - 50 % larger - 50 % smaller)
                    case int n when (n == 28 || n == 29): spacing = 14; break;   // 14-21-xx (orig - 50 % larger - 50 % smaller)
                    case int n when (n == 30 || n == 31): spacing = 13; break;   // 13-20-xx (orig - 50 % larger - 50 % smaller)
                    case int n when (n == 32 || n == 33): spacing = 12; break;   // 12-18-xx (orig - 50 % larger - 50 % smaller)
                    case int n when (n >= 34 && n <= 36): spacing = 11; break;   // 11-17-xx (orig - 50 % larger - 50 % smaller)
                    case int n when (n >= 37 && n <= 41): spacing = 10; break;   // 10-15-xx (orig - 50 % larger - 50 % smaller)
                    case int n when (n == 42 || n == 43): spacing = 14; break;   // 9-14-xx (orig - 50 % larger - 50 % smaller)
                    case int n when (n >= 44 && n <= 50): spacing = 8; break;   // 8-12-xx (orig - 50 % larger - 50 % smaller)
                    case int n when (n >= 51 && n <= 57): spacing = 7; break;   // 7-11-xx (orig - 50 % larger - 50 % smaller)
                    case int n when (n >= 58 && n <= 66): spacing = 6; break;   // 6-9-xx (orig - 50 % larger - 50 % smaller)
                    case int n when (n >= 67 && n <= 79): spacing = 5; break;   // 5-8-xx (orig - 50 % larger - 50 % smaller)
                    case int n when (n >= 80 && n <= 99): spacing = 4; break;   // 4-6-xx (orig - 50 % larger - 50 % smaller)
                    case int n when (n >= 100): spacing = 3; break;   // 3-5-xx (orig - 50 % larger - 50 % smaller)
                    default: spacing = 20; break;
                }
                // Draw icons
                PeopleType[] peoples = city.People;
                int drawIndex;
                for (int i = 0; i < city.Size; i++)
                {
                    drawIndex = (int)peoples[i];
                    if (i % 2 == 1 && (drawIndex == 0 || drawIndex == 2 || drawIndex == 4 || drawIndex == 6))
                    {
                        drawIndex++;  // Change men/woman appearance
                    }
                    //graphics.DrawImage(Images.PeopleL[drawIndex, 0], i * spacing + 1, 1);   // Shadow
                    graphics.DrawImage(ModifyImage.Resize(Images.PeopleLshadow[drawIndex, 0], zoom), i * spacing + 1, 1);   // Shadow
                    graphics.DrawImage(ModifyImage.Resize(Images.PeopleL[drawIndex, 0], zoom), i * spacing, 0);
                }
            }
            return faces;
        }

        public static Bitmap CityProduction(City city)
        {
            var panel = new Bitmap(195, 191);    // Same size as production panel in city panel
            using (var g = Graphics.FromImage(panel))
            {
                // Draw rectangle around icons
                int cost;
                if (city.ItemInProduction < 62) cost = Game.Rules.UnitCost[city.ItemInProduction];   // Item is unit
                else cost = Game.Rules.ImprovementCost[city.ItemInProduction - 62 + 1];    // Item is improvement (first 62 are units, +1 because first improvement is "Nothing")
                int vertSpacing = Math.Min(10, cost);    // Max 10 lines
                g.DrawLine(new Pen(Color.FromArgb(83, 103, 191)), 5, 42, 5 + 182, 42);   // 1st horizontal
                g.DrawLine(new Pen(Color.FromArgb(83, 103, 191)), 5, 42, 5, 42 + 4 + vertSpacing * 14);   // 1st vertical
                g.DrawLine(new Pen(Color.FromArgb(0, 0, 95)), 5, 42 + 4 + vertSpacing * 14, 5 + 182, 42 + 4 + vertSpacing * 14);   // 2nd horizontal
                g.DrawLine(new Pen(Color.FromArgb(0, 0, 95)), 5 + 182, 42, 5 + 182, 42 + 4 + vertSpacing * 14);   // 2nd vertical

                // Draw icons
                int count = 0;
                int dx, dy;
                for (int row = 0; row < Math.Min(cost, 10); row++)   // There are never more than 10 rows
                {
                    for (int col = 0; col < Math.Max(cost, 10); col++)  // There are never less than 10 columns
                    {
                        dx = Convert.ToInt32(2 + col * (182 - 14 - 4) / ((float)Math.Max(cost, 10) - 1)); //horizontal separation between icons
                        dy = 14;    // Vertical separation of icons (space between icons in y-directions is always 0)
                        g.DrawImage(Images.CitySupportBig, 5 + dx, 42 + 2 + dy * row);

                        count++;
                        if (count >= city.ShieldsProgress) break;
                    }
                    if (count >= city.ShieldsProgress) break;
                }
            }

            return panel;
        }

        // Draw resource map
        public static Bitmap CityResourcesMap(City city, int zoom)
        {
            var map = new Bitmap(4 * 8 * (8 + zoom), 4 * 4 * (8 + zoom));

            using (Graphics g = Graphics.FromImage(map))
            {
                // First draw squares around city
                int newX, newY;
                City cityHere;
                List<IUnit> unitsHere;
                for (int y_ = -3; y_ <= 3; y_++)
                {
                    for (int x_ = -3; x_ <= 3; x_++)
                    {
                        if ((x_ == -1 && y_ == -3) || (x_ == 1 && y_ == -3) || (x_ == -2 && y_ == -2) || (x_ == 0 && y_ == -2) || (x_ == 2 && y_ == -2) || (x_ == -3 && y_ == -1)
                            || (x_ == -1 && y_ == -1) || (x_ == 1 && y_ == -1) || (x_ == 3 && y_ == -1) || (x_ == -2 && y_ == 0) || (x_ == 0 && y_ == 0) || (x_ == 2 && y_ == 0)
                            || (x_ == -3 && y_ == 1) || (x_ == -1 && y_ == 1) || (x_ == 1 && y_ == 1) || (x_ == 3 && y_ == 1) || (x_ == -2 && y_ == 2) || (x_ == 0 && y_ == 2)
                            || (x_ == 2 && y_ == 2) || (x_ == -1 && y_ == 3) || (x_ == 1 && y_ == 3))
                        {
                            newX = city.X + x_;
                            newY = city.Y + y_;

                            // First draw blank tiles
                            g.DrawImage(ModifyImage.Resize(Images.Blank, zoom), 4 * (8 + zoom) * (x_ + 3), 2 * (8 + zoom) * (y_ + 3));
                            // Then draw tiles if they are visible
                            if (Map.IsTileVisibleC2(newX, newY, city.Owner.Id))
                                g.DrawImage(ModifyImage.Resize(Map.TileC2(newX, newY).Graphic, zoom), 4 * (8 + zoom) * (x_ + 3), 2 * (8 + zoom) * (y_ + 3));
                            // TODO: implement dithering on edges or depending on where invisible tiles are
                            // Draw cities
                            cityHere = Game.CityHere(newX, newY);
                            if (cityHere != null)
                                Draw.City(g, cityHere, false, zoom, new Point(4 * (8 + zoom) * (x_ + 3), 2 * (8 + zoom) * (y_ + 3) - 2 * (8 + zoom)));
                                //g.DrawImage(cityHere.Graphic(false, zoom), 4 * (8 + zoom) * (x_ + 3), 2 * (8 + zoom) * (y_ + 3) - 2 * (8 + zoom));
                            // Draw units
                            unitsHere = Game.UnitsHere(newX, newY).FindAll(unit => (unit.Owner != Game.ActiveCiv) && (unit.Type != Enums.UnitType.Settlers));
                            //if (unitsHere.Count > 0 && cityHere == null)
                            //    g.DrawImage(unitsHere.Last().Graphic(true, zoom), 4 * (8 + zoom) * (x_ + 3), 2 * (8 + zoom) * (y_ + 3) - 2 * (8 + zoom));

                            // TODO: make sure you're not drawing beyond map edges
                            //if (newX >= 0 && newX < 2 * Data.MapXdim && newY >= 0 && newY < Data.MapYdim) image = TerrainBitmap((newX - (newY % 2)) / 2, newY);
                        }
                    }
                }

                // Then draw food/shield/trade icons around the city (21 squares around city)
                int[,] offsets = new int[21, 2] { { 0, 0 }, { -1, -3 }, { -3, -1 }, { -3, 1 }, { -1, 3 }, { 1, 3 }, { 3, 1 }, { 3, -1 }, { 1, -3 }, { -2, -2 }, { -2, 2 }, { 2, 2 },
                                                      { 2, -2 }, { 0, -2 }, { -1, -1 }, { -2, 0 }, { -1, 1 }, { 0, 2 }, { 1, 1 }, { 2, 0 }, { 1, -1 } };    // offset of squares from city (0,0)
                int[] cityFood = city.FoodDistribution;
                int[] cityShld = city.ShieldDistribution;
                int[] cityTrad = city.TradeDistribution;
                for (int i = 0; i < 21; i++)
                    if (city.DistributionWorkers[i])
                    {
                        // First count all icons on this square to determine the spacing between icons (10 = no spacing, 15 = no spacing @ 50% scaled)
                        int spacing;
                        switch (cityFood[i] + cityShld[i] + cityTrad[i])
                        {
                            case 1:
                            case 2: spacing = 11; break;    // normal=11, big = 17, 1 pixel gap
                            case 3: spacing = 10; break;    // normal=10, big = 15, no gap
                            case 4: spacing = 7; break;    // normal=7, big = 11
                            case 5: spacing = 5; break;    // normal=5, big = 8
                            case 6: spacing = 4; break;    // normal=4, big = 6
                            case 7:
                            case 8: spacing = 3; break;    // normal=3, big = 5
                            case 9: spacing = 2; break;    // normal=2, big = 3
                            case 10: spacing = 1; break;    // normal=1, big = 2
                            default: spacing = 1; break;    //  normal=1, big = 2
                        }

                        // First draw food, then shields, then trade icons
                        int x_offset = 4 * (8 + zoom) - ((cityFood[i] + cityShld[i] + cityTrad[i] - 1) * spacing + 15) / 2;
                        int y_offset = 9;
                        for (int j = 0; j < cityFood[i]; j++) g.DrawImage(Images.CityFoodSmall, x_offset + (3 + offsets[i, 0]) * 4 * (8 + zoom) + j * spacing, y_offset + (3 + offsets[i, 1]) * 2 * (8 + zoom));
                        for (int j = 0; j < cityShld[i]; j++) g.DrawImage(Images.CitySupportSmall, x_offset + (3 + offsets[i, 0]) * 4 * (8 + zoom) + (cityFood[i] + j) * spacing, y_offset + (3 + offsets[i, 1]) * 2 * (8 + zoom));
                        for (int j = 0; j < cityTrad[i]; j++) g.DrawImage(Images.CityTradeSmall, x_offset + (3 + offsets[i, 0]) * 4 * (8 + zoom) + (cityFood[i] + cityShld[i] + j) * spacing, y_offset + (3 + offsets[i, 1]) * 2 * (8 + zoom));
                    }
            }
            return map;
        }
    }
}
