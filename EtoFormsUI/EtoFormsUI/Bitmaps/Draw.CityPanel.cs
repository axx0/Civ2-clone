using System;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Production;
using Eto.Drawing;
using EtoFormsUIExtensionMethods;

namespace EtoFormsUI
{
    public static partial class Draw
    {
        public static void CityFoodStorage(Graphics g, City city, int cityZoom, Point dest)
        {
            g.AntiAlias = false;

            var panelSize = new Size(195 * (2 + cityZoom) / 2, 163 * (2 + cityZoom) / 2);
            var panelPos = new Point(437 * (2 + cityZoom) / 2, 0);

            // Get normal zoom from city zoom (-1/0/1)
            int zoom = cityZoom * 4;

            // Icons
            int wheatW = 14 * (2 + cityZoom) / 2;
            int wheatH = 14 * (2 + cityZoom) / 2;
            var wheat_spacing = city.Size switch
            {
                int n when (n <= 9) => 17,
                int n when (n == 10) => 16,
                int n when (n == 11) => 13,
                int n when (n == 12) => 12,
                int n when (n == 13) => 11,
                int n when (n == 14) => 10,
                int n when (n == 15 || n == 16) => 9,
                int n when (n == 17) => 8,
                int n when (n >= 18 && n <= 20) => 7,
                int n when (n == 21 || n == 22) => 6,
                int n when (n >= 23 && n <= 26) => 5,
                int n when (n >= 27 && n <= 33) => 4,
                int n when (n >= 34 && n <= 40) => 3,
                int n when (n >= 41 && n <= 80) => 2,
                int n when (n >= 81) => 1,
                _ => 17,
            };
            wheat_spacing = wheat_spacing.ZoomScale(zoom);

            // Draw rectangle around wheat icons     
            using var _pen1 = new Pen(Color.FromArgb(75, 155, 35));
            using var _pen2 = new Pen(Color.FromArgb(0, 51, 0));
            // 1st horizontal line
            int line_width = city.Size * wheat_spacing + wheatW + 7 * (2 + cityZoom) / 2;
            int starting_x = dest.X + panelPos.X + panelSize.Width / 2 - line_width / 2;
            int starting_y = dest.Y + 15 * (2 + cityZoom) / 2;
            g.DrawLine(_pen1, starting_x, starting_y, starting_x + line_width, starting_y);
            // 2nd horizontal line
            starting_y = dest.Y + 160 * (2 + cityZoom) / 2;
            g.DrawLine(_pen2, starting_x, starting_y, starting_x + line_width, starting_y);
            // 1st vertical line
            starting_y = dest.Y + 15 * (2 + cityZoom) / 2;
            int line_height = 144 * (2 + cityZoom) / 2;
            g.DrawLine(_pen1, starting_x, starting_y, starting_x, starting_y + line_height);
            // 2nd vertical line
            g.DrawLine(_pen2, starting_x + line_width, starting_y, starting_x + line_width, starting_y + line_height);

            // Draw wheat icons
            int count = 0;
            starting_x += 3 * (2 + cityZoom) / 2;
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col <= city.Size; col++)
                {
                    g.DrawImage(CityImages.FoodBig.Resize(zoom), starting_x + wheat_spacing * col, dest.Y +  15 * (2 + cityZoom) / 2 + 3 + wheatH * row);
                    count++;

                    if (count >= city.FoodInStorage) break;
                }
                if (count >= city.FoodInStorage) break;
            }

            // 3rd horizontal line (granary effect)
            if (city.ImprovementExists(ImprovementType.Granary))
            {
                line_width -= 10 * (2 + cityZoom) / 2;
                starting_x += 2 * (2 + cityZoom) / 2;
                starting_y = dest.Y + 87 * (2 + cityZoom) / 2;
                g.DrawLine(_pen1, starting_x, starting_y, starting_x + line_width, starting_y);
            }
        }

        // Draw icons in city resources (surplus < 0 is hunger)
        public static void CityResources(Graphics g, City city, int cityZoom, Point dest)
        {
            var fontSize = cityZoom == -1 ? 4 : (cityZoom == 0 ? 9 : 13);
            using var _font = new Font("Arial", fontSize, FontStyle.Bold);

            // Get normal zoom from city zoom (-1/0/1)
            int zoom = cityZoom * 4;

            // FOOD
            // Text
            var _txtFrame = new Rectangle(dest.X + 203.ZoomScale(4 * cityZoom), dest.Y + 61.ZoomScale(4 * cityZoom), 228.ZoomScale(4 * cityZoom), 12.ZoomScale(4 * cityZoom));
            Draw.Text(g, $"Food: {city.Food}", _font, Color.FromArgb(87, 171, 39), _txtFrame, FormattedTextAlignment.Left, Colors.Black, 1, 1);
            Draw.Text(g, $"Surplus: {city.SurplusHunger}", _font, Color.FromArgb(63, 139, 31), _txtFrame, FormattedTextAlignment.Right, Colors.Black, 1, 1);
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
            spacing = (int)(spacing * (1 + ((float)zoom / 8.0)));   // Make spacing city zoom dependent
            // TODO: Draw background rectangle
            //g.FillRectangle(new SolidBrush(Color.FromArgb(71, 147, 31)), 0, 0, spacing * city.Food + 21 - spacing + 6, 23); // Background square for food
            //g.FillRectangle(new SolidBrush(Color.FromArgb(55, 123, 23)), x_size - (spacing * Math.Abs(city.SurplusHunger) + 21 - spacing + 3), 
            //    0, spacing * Math.Abs(city.SurplusHunger) + 21 - spacing + 6, 23); // Background square for surplus/hunger
            // Icons
            for (int i = 0; i < city.Food; i++) g.DrawImage(CityImages.FoodBig.Resize(zoom), dest.X + 206 * (2 + cityZoom) / 2 + i * spacing, dest.Y + 76 * (2 + cityZoom) / 2);
            for (int i = 0; i < Math.Abs(city.SurplusHunger); i++)
            {
                if (city.SurplusHunger < 0) g.DrawImage(CityImages.HungerBig.Resize(zoom), dest.X + (431 - (spacing * Math.Abs(city.SurplusHunger) + 14 - spacing) + i * spacing) * (2 + cityZoom) / 2, dest.Y + 76 * (2 + cityZoom) / 2); // Hunger
                else g.DrawImage(CityImages.FoodBig.Resize(zoom), dest.X + (431 - (spacing * Math.Abs(city.SurplusHunger) + 14 - spacing) + i * spacing) * (2 + cityZoom) / 2, dest.Y + 76 * (2 + cityZoom) / 2); // Surplus
            }

            // TRADE
            // Text
            _txtFrame = new Rectangle(dest.X + 203 * (2 + cityZoom) / 2, dest.Y + 102 * (2 + cityZoom) / 2, 228 * (2 + cityZoom) / 2, 12 * (2 + cityZoom) / 2);
            Draw.Text(g, $"Trade: {city.Trade}", _font, Color.FromArgb(239, 159, 7), _txtFrame, FormattedTextAlignment.Left, Colors.Black, 1, 1);
            Draw.Text(g, $"Corruption: {city.Corruption}", _font, Color.FromArgb(227, 83, 15), _txtFrame, FormattedTextAlignment.Right, Colors.Black, 1, 1);
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
            spacing = (int)(spacing * (1 + ((float)zoom / 8.0)));   // Make spacing city zoom dependent
            // TODO: Draw background rectangle
            //g.FillRectangle(new SolidBrush(Color.FromArgb(71, 147, 31)), 0, 0, spacing * foodIcons + 21 - spacing + 6, 23); //background square for food
            //g.FillRectangle(new SolidBrush(Color.FromArgb(55, 123, 23)), x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing + 3), 0, spacing * Math.Abs(surplusIcons) + 21 - spacing + 6, 23); //background square for surplus/hunger
            // Icons
            for (int i = 0; i < city.Trade; i++) g.DrawImage(CityImages.TradeBig.Resize(zoom), dest.X + 206 * (2 + cityZoom) / 2 + i * spacing, dest.Y + 117 * (2 + cityZoom) / 2);
            for (int i = 0; i < city.Corruption; i++) g.DrawImage(CityImages.CorruptBig.Resize(zoom), dest.X + (431 - (spacing * city.Corruption + 14 - spacing) + i * spacing) * (2 + cityZoom) / 2, dest.Y + 117 * (2 + cityZoom) / 2);

            // TAX+LUX+SCI
            // Text
            _txtFrame = new Rectangle(dest.X + 204 * (2 + cityZoom) / 2, dest.Y + 156 * (2 + cityZoom) / 2, 228 * (2 + cityZoom) / 2, 12 * (2 + cityZoom) / 2);
            Draw.Text(g, $"50% Tax: {city.Tax}", _font, Color.FromArgb(239, 159, 7), _txtFrame, FormattedTextAlignment.Left, Colors.Black, 1, 1);
            Draw.Text(g, $"0% Lux: {city.Lux}", _font, Colors.White, _txtFrame, FormattedTextAlignment.Center, Colors.Black, 1, 1);
            Draw.Text(g, $"50% Sci: {city.Science}", _font, Color.FromArgb(63, 187, 199), _txtFrame, FormattedTextAlignment.Right, Colors.Black, 1, 1);
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
            spacing = (int)(spacing * (1 + ((float)zoom / 8.0)));   // Make spacing city zoom dependent
            // TODO: Draw background rectangle
            //g.FillRectangle(new SolidBrush(Color.FromArgb(71, 147, 31)), 0, 0, spacing * foodIcons + 21 - spacing + 6, 23); //background square for food
            //g.FillRectangle(new SolidBrush(Color.FromArgb(55, 123, 23)), x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing + 3), 0, spacing * Math.Abs(surplusIcons) + 21 - spacing + 6, 23); //background square for surplus/hunger
            // Icons
            for (int i = 0; i < city.Tax; i++) g.DrawImage(CityImages.TaxBig.Resize(zoom), dest.X + 206 * (2 + cityZoom) / 2 + i * spacing, dest.Y + 141 * (2 + cityZoom) / 2);
            for (int i = 0; i < city.Lux; i++) g.DrawImage(CityImages.LuxBig.Resize(zoom), dest.X + 290 * (2 + cityZoom) / 2 + i * spacing, dest.Y + 141 * (2 + cityZoom) / 2);
            for (int i = 0; i < city.Science; i++) g.DrawImage(CityImages.SciBig.Resize(zoom), dest.X + (431 - (spacing * city.Science + 14 - spacing) + i * spacing) * (2 + cityZoom) / 2, dest.Y + 141 * (2 + cityZoom) / 2);

            // SUPPORT+PRODUCTION
            // Text
            _txtFrame = new Rectangle(dest.X + 204 * (2 + cityZoom) / 2, dest.Y + 196 * (2 + cityZoom) / 2, 228 * (2 + cityZoom) / 2, 12 * (2 + cityZoom) / 2);
            Draw.Text(g, $"Support: {city.Support}", _font, Color.FromArgb(63, 79, 167), _txtFrame, FormattedTextAlignment.Left, Colors.Black, 1, 1);
            Draw.Text(g, $"Production: {city.Production}", _font, Color.FromArgb(7, 11, 103), _txtFrame, FormattedTextAlignment.Right, Colors.Black, 1, 1);
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
            spacing = (int)(spacing * (1 + ((float)zoom / 8.0)));   // Make spacing city zoom dependent
            // TODO: Draw background rectangle
            //g.FillRectangle(new SolidBrush(Color.FromArgb(71, 147, 31)), 0, 0, spacing * foodIcons + 21 - spacing + 6, 23); //background square for food
            //g.FillRectangle(new SolidBrush(Color.FromArgb(55, 123, 23)), x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing + 3), 0, spacing * Math.Abs(surplusIcons) + 21 - spacing + 6, 23); //background square for surplus/hunger
            // Icons
            for (int i = 0; i < city.Support; i++) g.DrawImage(CityImages.SupportBig.Resize(zoom), dest.X + 206 * (2 + cityZoom) / 2 + i * spacing, dest.Y + 181 * (2 + cityZoom) / 2);
            for (int i = 0; i < city.Production; i++) g.DrawImage(CityImages.SupportBig.Resize(zoom), dest.X + (431 - (spacing * city.Production + 14 - spacing) + i * spacing) * (2 + cityZoom) / 2, dest.Y + 181 * (2 + cityZoom) / 2);
        }

        // Draw faces in City Panel
        public static void CityCitizens(Graphics g, City city, int cityZoom, Point dest)
        {
            // Get normal zoom from city zoom (-1/0/1)
            int zoom = cityZoom * 4;
            var spacing = city.Size switch
            {
                int n when (n <= 15) => 28,
                int n when (n == 16) => 26,
                int n when (n == 17) => 24,
                int n when (n == 18) => 23,
                int n when (n == 19) => 21,
                int n when (n == 20) => 20,
                int n when (n == 21) => 19,
                int n when (n == 22) => 18,
                int n when (n == 23 || n == 24) => 17,
                int n when (n == 25) => 16,
                int n when (n == 26 || n == 27) => 15,
                int n when (n == 28 || n == 29) => 14,
                int n when (n == 30 || n == 31) => 13,
                int n when (n == 32 || n == 33) => 12,
                int n when (n >= 34 && n <= 36) => 11,
                int n when (n >= 37 && n <= 41) => 10,
                int n when (n == 42 || n == 43) => 14,
                int n when (n >= 44 && n <= 50) => 8,
                int n when (n >= 51 && n <= 57) => 7,
                int n when (n >= 58 && n <= 66) => 6,
                int n when (n >= 67 && n <= 79) => 5,
                int n when (n >= 80 && n <= 99) => 4,
                int n when (n >= 100) => 3,
                _ => 20,
            };
            spacing = spacing.ZoomScale(zoom);

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
                using var plpShPic = CityImages.PeopleShadowLarge[drawIndex, (int)city.Owner.Epoch].Resize(zoom);
                g.DrawImage(plpShPic, dest.X + 5 * (2 + cityZoom) / 2 + i * spacing + 1, dest.Y + 9 * (2 + cityZoom) / 2 + 1);   // Shadow
                using var plpPic = CityImages.PeopleLarge[drawIndex, (int)city.Owner.Epoch].Resize(zoom);
                g.DrawImage(plpPic, dest.X + 5 * (2 + cityZoom) / 2 + i * spacing, dest.Y + 9 * (2 + cityZoom) / 2 + 0);
            }
        }

        public static void CityProduction(Graphics g, City city, int cityZoom, Point dest)
        {
            var panelSize = new Size(195 * (2 + cityZoom) / 2, 191 * (2 + cityZoom) / 2);
            var panelPos = new Point(437 * (2 + cityZoom) / 2, 165 * (2 + cityZoom) / 2);

            // Show item currently in production (ProductionItem=0...61 are units, 62...127 are improvements)
            // zoom: Units=-1(norm), Improvements=0(norm)
            var fontSize = cityZoom == -1 ? 4 : (cityZoom == 0 ? 9 : 13);
            using var font = new Font("Arial", fontSize, FontStyle.Bold);
            
            var cost = city.ItemInProduction.Cost;
            // Units
            switch (city.ItemInProduction.Type)
            {
                case ItemType.Unit:
                    int unitZoom = cityZoom == -1 ? -5 : (cityZoom == 0 ? -1 : 1);
                    UnitSprite(g, city.ItemInProduction.ImageIndex, false, false, unitZoom, new Point(dest.X + panelPos.X + 72 * (2 + cityZoom) / 2, dest.Y + panelPos.Y + 3 * (2 + cityZoom) / 2));
                    break;
                case ItemType.Building:
                    var index = city.ItemInProduction.ImageIndex;
                    Text(g, Game.Rules.Improvements[index].Name, font, Color.FromArgb(63, 79, 167), new Point(dest.X + panelPos.X + 97 * (2 + cityZoom) / 2, dest.Y + panelPos.Y + 8 * (2 + cityZoom) / 2), true, true, Colors.Black, 1, 1);
                    CityImprovement(g, (ImprovementType)index, 4 * cityZoom, new Point(dest.X + panelPos.X + 79 * (2 + cityZoom) / 2, dest.Y + panelPos.Y + 18 * (2 + cityZoom) / 2));
                    break;
                default:
                    throw new NotSupportedException("Unknown production type");
            }

            // Draw rectangle around icons
            int vertSpacing = Math.Min(10, cost);    // Max 10 lines
            using var _pen1 = new Pen(Color.FromArgb(83, 103, 191));
            using var _pen2 = new Pen(Color.FromArgb(0, 0, 95));
            g.DrawLine(_pen1, dest.X + 442 * (2 + cityZoom) / 2, dest.Y + 207 * (2 + cityZoom) / 2, dest.X + 624 * (2 + cityZoom) / 2, dest.Y + 207 * (2 + cityZoom) / 2);   // 1st horizontal
            g.DrawLine(_pen1, dest.X + 442 * (2 + cityZoom) / 2, dest.Y + 207 * (2 + cityZoom) / 2, dest.X + 442 * (2 + cityZoom) / 2, dest.Y + (211 + vertSpacing * 14) * (2 + cityZoom) / 2);   // 1st vertical
            g.DrawLine(_pen2, dest.X + 442 * (2 + cityZoom) / 2, dest.Y + (211 + vertSpacing * 14) * (2 + cityZoom) / 2, dest.X + 624 * (2 + cityZoom) / 2, dest.Y + (211 + vertSpacing * 14) * (2 + cityZoom) / 2);   // 2nd horizontal
            g.DrawLine(_pen2, dest.X + 624 * (2 + cityZoom) / 2, dest.Y + 207 * (2 + cityZoom) / 2, dest.X + 624 * (2 + cityZoom) / 2, dest.Y + (211 + vertSpacing * 14) * (2 + cityZoom) / 2);   // 2nd vertical

            // Draw icons
            int count = 0;
            int dx, dy;
            for (int row = 0; row < Math.Min(cost, 10); row++)   // There are never more than 10 rows
            {
                for (int col = 0; col < Math.Max(cost, 10); col++)  // There are never less than 10 columns
                {
                    dx = Convert.ToInt32(2 + col * (182 - 14 - 4) / ((float)Math.Max(cost, 10) - 1)) ; // Horizontal separation between icons
                    dy = 14;    // Vertical separation of icons (space between icons in y-directions is always 0)
                    g.DrawImage(CityImages.SupportBig.Resize(4 * cityZoom), dest.X + panelPos.X + (6 + dx) * (2 + cityZoom) / 2, dest.Y + panelPos.Y + (45 + dy * row) * (2 + cityZoom) / 2);

                    count++;
                    if (count >= city.ShieldsProgress) break;
                }
                if (count >= city.ShieldsProgress) break;
            }
        }

        // Draw resource map
        public static void CityResourcesMap(Graphics g, City city, int cityZoom, Point dest)
        {
            // Get normal zoom from city zoom (-1/0/1)
            int zoom;
            if (cityZoom == 0) zoom = -2;
            else if (cityZoom == 1) zoom = 1;
            else zoom = -5;

            int offsetX = 5 * (2 + cityZoom) / 2;
            int offsetY = 84 * (2 + cityZoom) / 2;

            // First draw squares around city
            int newX, newY;
            City cityHere;
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
                        using var blankPic = MapImages.Terrains[Map.MapIndex].Blank.Resize(zoom);
                        g.DrawImage(blankPic, dest.X + offsetX + 4 * (8 + zoom) * (x_ + 3), dest.Y + offsetY + 2 * (8 + zoom) * (y_ + 3));
                        // Then draw tiles if they are visible
                        if (!Map.IsValidTileC2(newX, newY)) continue;
                        var tile = Map.TileC2(newX, newY);
                        if (tile.Visibility[city.Owner.Id])
                        {
                            using var mapPic = Images.MapTileGraphicC2(newX, newY).Resize(zoom);
                            g.DrawImage(mapPic, dest.X + offsetX + 4 * (8 + zoom) * (x_ + 3),
                                dest.Y + offsetY + 2 * (8 + zoom) * (y_ + 3));

                            // TODO: implement dithering on edges or depending on where invisible tiles are
                            // Draw cities

                            if (tile.CityHere != null)
                            {
                                City(g, tile.CityHere, false, zoom,
                                    new Point(dest.X + offsetX + 4 * (8 + zoom) * (x_ + 3),
                                        dest.Y + offsetY + 2 * (8 + zoom) * (y_ + 3) - 2 * (8 + zoom)));
                            } else if (tile.UnitsHere.Count > 0)
                            {
                                var unit = tile.GetTopUnit();

                                if (unit != null && unit.AttackBase > 0)
                                {
                                    Unit(g, unit, tile.UnitsHere.Count > 1, zoom,
                                        new Point(dest.X + offsetX + 4 * (8 + zoom) * (x_ + 3),
                                            dest.Y + offsetY + 2 * (8 + zoom) * (y_ + 3) - 2 * (8 + zoom)));
                                }
                            }
                            //// TODO: make sure you're not drawing beyond map edges
                            ////if (newX >= 0 && newX < 2 * Data.MapXdim && newY >= 0 && newY < Data.MapYdim) image = TerrainBitmap((newX - (newY % 2)) / 2, newY);
                        }
                    }
                }
            }

            // Then draw food/shield/trade icons around the city (21 squares around city)
//            int[,] offsets = new int[21, 2] { { 0, 0 }, { -1, -3 }, { -3, -1 }, { -3, 1 }, { -1, 3 }, { 1, 3 }, { 3, 1 }, { 3, -1 }, { 1, -3 }, { -2, -2 }, { -2, 2 }, { 2, 2 }, { 2, -2 }, { 0, -2 }, { -1, -1 }, { -2, 0 }, { -1, 1 }, { 0, 2 }, { 1, 1 }, { 2, 0 }, { 1, -1 } };    // Offset of squares from city (0,0)
            
            var organization = city.OrganizationLevel;
            var hasSupermarket = city.ImprovementExists(ImprovementType.Supermarket);
            var hasSuperhighways = city.ImprovementExists(ImprovementType.Superhighways);
            
            foreach (var tile in city.WorkedTiles)
            {
                var food = tile.GetFood(organization == 0, hasSupermarket);
                var shields = tile.GetShields(organization == 0);
                var trade = tile.GetTrade(organization, hasSuperhighways);
                var spacing = (food + shields+trade) switch
                {
                    1 => 11,
                    2 => 11,
                    3 => 10,
                    4 => 7,
                    5 => 5,
                    6 => 4,
                    7 => 3,
                    8 => 3,
                    9 => 2,
                    10 => 1,
                    _ => 1
                };
                
                spacing = (int)((float)spacing * (2.0 + (float)cityZoom) / 2.0);    // Make spacing zoom dependent
                
                int x_offset = 4 * (8 + zoom) - ((food + shields + trade - 1) * spacing + 15) / 2;
                const int yConstant = 9;
                var combinedXOffsets = dest.X + offsetX + x_offset + (3 + tile.X - city.Location.X) * 4 * (8 + zoom);
                var combinedYOffsets = dest.Y + offsetY + yConstant + (3 + tile.Y - city.Location.Y) * 2 * (8 + zoom);
                    
                    
                // First draw food, then shields, then trade icons
                for (int j = 0; j < food; j++) g.DrawImage(CityImages.FoodSmall.Resize(4 * cityZoom),  combinedXOffsets+ j * spacing, combinedYOffsets);
                for (int j = 0; j < shields; j++) g.DrawImage(CityImages.SupportSmall.Resize(4 * cityZoom), combinedXOffsets + (food + j) * spacing, combinedYOffsets);
                for (int j = 0; j < trade; j++) g.DrawImage(CityImages.TradeSmall.Resize(4 * cityZoom), combinedXOffsets + (food+ shields + j) * spacing, combinedYOffsets);
            }
            
            // int[] cityFood = city.FoodDistribution;
            // int[] cityShld = city.ShieldDistribution;
            // int[] cityTrad = city.TradeDistribution;
            // for (int i = 0; i < 21; i++)
            //     if (city.DistributionWorkers[i])
            //     {
            //         // First count all icons on this square to determine the spacing between icons (10 = no spacing, 15 = no spacing @ 50% scaled)
            //         int spacing = (cityFood[i] + cityShld[i] + cityTrad[i]) switch
            //         {
            //             1 => 11,
            //             2 => 11,
            //             3 => 10,
            //             4 => 7,
            //             5 => 5,
            //             6 => 4,
            //             7 => 3,
            //             8 => 3,
            //             9 => 2,
            //             10 => 1,
            //             _ => 1
            //         };
            //         spacing = (int)((float)spacing * (2.0 + (float)cityZoom) / 2.0);    // Make spacing zoom dependent
            //
            //         // First draw food, then shields, then trade icons
            //         int x_offset = 4 * (8 + zoom) - ((cityFood[i] + cityShld[i] + cityTrad[i] - 1) * spacing + 15) / 2;
            //         int y_offset = 9;
            //         for (int j = 0; j < cityFood[i]; j++) g.DrawImage(Images.CityFoodSmall.Resize(4 * cityZoom), dest.X + offsetX + x_offset + (3 + offsets[i, 0]) * 4 * (8 + zoom) + j * spacing, dest.Y + offsetY + y_offset + (3 + offsets[i, 1]) * 2 * (8 + zoom));
            //         for (int j = 0; j < cityShld[i]; j++) g.DrawImage(Images.CitySupportSmall.Resize(4 * cityZoom), dest.X + offsetX + x_offset + (3 + offsets[i, 0]) * 4 * (8 + zoom) + (cityFood[i] + j) * spacing, dest.Y + offsetY + y_offset + (3 + offsets[i, 1]) * 2 * (8 + zoom));
            //         for (int j = 0; j < cityTrad[i]; j++) g.DrawImage(Images.CityTradeSmall.Resize(4 * cityZoom), dest.X + offsetX + x_offset + (3 + offsets[i, 0]) * 4 * (8 + zoom) + (cityFood[i] + cityShld[i] + j) * spacing, dest.Y + offsetY + y_offset + (3 + offsets[i, 1]) * 2 * (8 + zoom));
            //     }

        }
    }
}
