using System.Drawing;

namespace civ2.Bitmaps
{
    public partial class Draw
    {
        public static Bitmap CitymapFoodLarge, CitymapFoodLargeBigger, CitymapHungerLarge, CitymapHungerLargeBigger, CitymapFoodSmall,
            CitymapFoodSmallBigger,
            CitymapShieldLarge, CitymapShieldLargeBigger, CitymapShieldSmall, CitymapShieldSmallBigger, CitymapTradeLarge, CitymapTradeLargeBigger, CitymapTradeSmall,
            CitymapTradeSmallBigger, CitymapShortageLargeBigger, CitymapShortageLarge, CitymapCorruptionLarge, CitymapCorruptionLargeBigger, CitymapSupportLarge,
            CitymapSupportLargeBigger, CitymapLuxLarge, CitymapLuxLargeBigger, CitymapTaxLarge, CitymapTaxLargeBigger, CitymapSciLarge, CitymapSciLargeBigger, NextCity;

        public static Bitmap DrawFoodStorage(City city)
        {
            Bitmap icons = new Bitmap(291, 244);    //define a bitmap for drawing icons
            using (Graphics graphics = Graphics.FromImage(icons))
            {
                int wheatW = 21;   //width. Original=14 (50% scaling).
                int wheatH = 21;   //height. Original=14 (50% scaling).

                //First determine spacing between wheat icons
                //NOTE (not 100% accurate, spacing also tends to switch between two numbers)
                int wheat_spacing;
                switch (city.Size)
                {
                    case int n when (n <= 9): wheat_spacing = 26; break;  //original=17 (50% scaled)
                    case int n when (n == 10): wheat_spacing = 24; break;  //original=16 (50% scaled)
                    case int n when (n == 11): wheat_spacing = 20; break;  //original=13 (50% scaled)
                    case int n when (n == 12): wheat_spacing = 18; break;  //original=12 (50% scaled)
                    case int n when (n == 13): wheat_spacing = 17; break;  //original=11 (50% scaled)
                    case int n when (n == 14): wheat_spacing = 15; break;  //original=10 (50% scaled)
                    case int n when (n == 15 || n == 16): wheat_spacing = 14; break;  //original=9 (50% scaled)
                    case int n when (n == 17): wheat_spacing = 12; break;  //original=8 (50% scaled)
                    case int n when (n >= 18 && n <= 20): wheat_spacing = 11; break;  //original=7 (50% scaled)
                    case int n when (n == 21 || n == 22): wheat_spacing = 9; break;  //original=6 (50% scaled)
                    case int n when (n >= 23 && n <= 26): wheat_spacing = 8; break;  //original=5 (50% scaled)
                    case int n when (n >= 27 && n <= 33): wheat_spacing = 6; break;  //original=4 (50% scaled)
                    case int n when (n >= 34 && n <= 40): wheat_spacing = 5; break;  //original=3 (50% scaled)
                    case int n when (n >= 41 && n <= 80): wheat_spacing = 3; break;  //original=2 (50% scaled)
                    case int n when (n >= 81): wheat_spacing = 2; break;  //original=1 (50% scaled)
                    default: wheat_spacing = 26; break;
                }

                //Draw rectangle around wheat icons     
                //1st horizontal line
                int line_width = (city.Size + 1) * wheat_spacing + wheatW - wheat_spacing + 2 + 5;
                int starting_x = (int)((291 - line_width) / 2);   //291 = width of drawing panel
                int starting_y = 23;    //original=15, this is 50 % scaled
                graphics.DrawLine(new Pen(Color.FromArgb(75, 155, 35)), starting_x, starting_y, starting_x + line_width, starting_y);
                //3rd horizontal line
                starting_y = 240;    //original=160, this is 50 % scaled
                graphics.DrawLine(new Pen(Color.FromArgb(0, 51, 0)), starting_x, starting_y, starting_x + line_width, starting_y);
                //1st vertical line
                starting_y = 23;
                int line_height = 216;  //original=144 (50% scaled)
                graphics.DrawLine(new Pen(Color.FromArgb(75, 155, 35)), starting_x, starting_y, starting_x, starting_y + line_height);
                //2nd vertical line
                graphics.DrawLine(new Pen(Color.FromArgb(0, 51, 0)), starting_x + line_width, starting_y, starting_x + line_width, starting_y + line_height);

                //Draw wheat icons
                int count = 0;
                starting_x += 3;    //wheat icons 2px to the right in original (50% scaled)
                for (int row = 0; row < 10; row++)
                {
                    for (int col = 0; col <= city.Size; col++)
                    {
                        graphics.DrawImage(Images.CitymapFoodLargeBigger, starting_x + wheat_spacing * col, 27 + wheatH * row);
                        count++;

                        if (count >= city.FoodInStorage) break;
                    }
                    if (count >= city.FoodInStorage) break;
                }

                //3rd horizontal line (shorter)
                line_width -= 12;   //orignal=8 px shorter (50% scaled)
                starting_x -= 3;    //correct from above
                starting_x += 6;
                starting_y = 131;   //orignal=87 (50% scaled)
                graphics.DrawLine(new Pen(Color.FromArgb(75, 155, 35)), starting_x, starting_y, starting_x + line_width, starting_y);
            }

            return icons;
        }

        public static Bitmap DrawCitizens(City city, double scale_factor) //Draw faces in cityform
        {
            Bitmap faces = new Bitmap(630, 50);
            //using (Graphics graphics = Graphics.FromImage(faces))
            //{
            //    int spacing;
            //    switch (city.Size)
            //    {
            //        case int n when (n <= 15): spacing = 42; break;    //50 % larger (orignal = 28)
            //        case int n when (n == 16): spacing = 39; break;    //50 % larger (orignal = 26)
            //        case int n when (n == 17): spacing = 36; break;    //50 % larger (orignal = 24)
            //        case int n when (n == 18): spacing = 35; break;    //50 % larger (orignal = 23)
            //        case int n when (n == 19): spacing = 32; break;    //50 % larger (orignal = 21)
            //        case int n when (n == 20): spacing = 30; break;    //50 % larger (orignal = 20)
            //        case int n when (n == 21): spacing = 29; break;   //50 % larger (orignal = 19)
            //        case int n when (n == 22): spacing = 27; break;   //50 % larger (orignal = 18)
            //        case int n when (n == 23 || n == 24): spacing = 26; break;   //50 % larger (orignal = 17)
            //        case int n when (n == 25): spacing = 24; break;   //50 % larger (orignal = 16)
            //        case int n when (n == 26 || n == 27): spacing = 23; break;   //50 % larger (orignal = 15)
            //        case int n when (n == 28 || n == 29): spacing = 21; break;   //50 % larger (orignal = 14)
            //        case int n when (n == 30 || n == 31): spacing = 20; break;   //50 % larger (orignal = 13)
            //        case int n when (n == 32 || n == 33): spacing = 18; break;   //50 % larger (orignal = 12)
            //        case int n when (n >= 34 && n <= 36): spacing = 17; break;   //50 % larger (orignal = 11)
            //        case int n when (n >= 37 && n <= 41): spacing = 15; break;   //50 % larger (orignal = 10)
            //        case int n when (n == 42 || n == 43): spacing = 14; break;   //50 % larger (orignal = 9)
            //        case int n when (n >= 44 && n <= 50): spacing = 12; break;   //50 % larger (orignal = 8)
            //        case int n when (n >= 51 && n <= 57): spacing = 11; break;   //50 % larger (orignal = 7)
            //        case int n when (n >= 58 && n <= 66): spacing = 9; break;   //50 % larger (orignal = 6)
            //        case int n when (n >= 67 && n <= 79): spacing = 8; break;   //50 % larger (orignal = 5)
            //        case int n when (n >= 80 && n <= 99): spacing = 6; break;   //50 % larger (orignal = 4)
            //        case int n when (n >= 100): spacing = 5; break;   //50 % larger (orignal = 3)
            //        default: spacing = 30; break;
            //    }
            //    //Draw icons
            //    PeopleType[] peoples = city.People;
            //    int drawIndex = 0;
            //    for (int i = 0; i < city.Size; i++)
            //    {
            //        drawIndex = (int)peoples[i];
            //        if (i % 2 == 1 && (drawIndex == 0 || drawIndex == 2 || drawIndex == 4 || drawIndex == 6)) drawIndex++;  //to change men/woman appearance
            //        graphics.DrawImage(ModifyImage.ResizeImage(PeopleLshadow[drawIndex, 0], (int)(27 * scale_factor), (int)(30 * scale_factor)), i * spacing + 1, 1);   //shadow
            //        graphics.DrawImage(ModifyImage.ResizeImage(PeopleL[drawIndex, 0], (int)(27 * scale_factor), (int)(30 * scale_factor)), i * spacing, 0);
            //    }
            //}

            return faces;
        }

        public static Bitmap DrawCityProduction(City city)
        {
            Bitmap icons = new Bitmap(293, 287);    //same size as production panel in city form
            //using (Graphics graphics = Graphics.FromImage(icons))
            //{
            //    //Draw rectangle around icons
            //    int IIP = city.ItemInProduction;
            //    int cost;
            //    if (IIP < 62) cost = Rules.UnitCost[IIP];   //Item is unit
            //    else cost = Rules.ImprovementCost[IIP - 62 + 1];    //Item is improvement (first 62 are units, +1 because first improfement is "Nothing")
            //    int vertSpacing = Math.Min(10, cost);    //max 10 lines
            //    graphics.DrawLine(new Pen(Color.FromArgb(83, 103, 191)), 9, 65, 9 + 271, 65);   //1st horizontal
            //    graphics.DrawLine(new Pen(Color.FromArgb(83, 103, 191)), 9, 65, 9, 65 + 27 + (vertSpacing - 1) * 21);   //1st vertical
            //    graphics.DrawLine(new Pen(Color.FromArgb(0, 0, 95)), 9, 65 + 27 + (vertSpacing - 1) * 21, 9 + 271, 65 + 27 + (vertSpacing - 1) * 21);   //2nd horizontal
            //    graphics.DrawLine(new Pen(Color.FromArgb(0, 0, 95)), 9 + 271, 65, 9 + 271, 65 + 27 + (vertSpacing - 1) * 21);   //2nd vertical

            //    //Draw icons
            //    int count = 0;
            //    for (int row = 0; row < Math.Min(cost, 10); row++)   //there are never more than 10 rows
            //    {
            //        for (int col = 0; col < Math.Max(cost, 10); col++)  //there are never less than 10 columns
            //        {
            //            int dx = Convert.ToInt32(2 + col * (272 - 21 - 4) / ((float)Math.Max(cost, 10) - 1)); //horizontal separation between icons
            //            int dy = 21;    //vertical separation of icons (space between icons in y-directions is always 0)
            //            graphics.DrawImage(Images.CitymapSupportLargeBigger, 10 + dx, 65 + 3 + dy * row);

            //            count++;
            //            if (count >= city.ShieldsProgress) break;
            //        }
            //        if (count >= city.ShieldsProgress) break;
            //    }
            //}

            return icons;
        }

        public static Bitmap DrawCityResourcesMap(City city)    //Draw terrain in city form
        {
            Bitmap map = new Bitmap(4 * 64, 4 * 32);

            //Bitmap image;
            //using (Graphics graphics = Graphics.FromImage(map))
            //{
            //    //First draw squares around city
            //    for (int x_ = -3; x_ <= 3; x_++)
            //        for (int y_ = -3; y_ <= 3; y_++)
            //            if ((x_ == -1 & y_ == -3) || (x_ == 1 & y_ == -3) || (x_ == -2 & y_ == -2) || (x_ == 0 & y_ == -2) || (x_ == 2 & y_ == -2) || (x_ == -3 & y_ == -1) || (x_ == -1 & y_ == -1) || (x_ == 1 & y_ == -1) || (x_ == 3 & y_ == -1) || (x_ == -2 & y_ == 0) || (x_ == 0 & y_ == 0) || (x_ == 2 & y_ == 0) || (x_ == -3 & y_ == 1) || (x_ == -1 & y_ == 1) || (x_ == 1 & y_ == 1) || (x_ == 3 & y_ == 1) || (x_ == -2 & y_ == 2) || (x_ == 0 & y_ == 2) || (x_ == 2 & y_ == 2) || (x_ == -1 & y_ == 3) || (x_ == 1 & y_ == 3))
            //            {
            //                int newX = city.X + x_;
            //                int newY = city.Y + y_;
            //                //TODO: correct this
            //                //if (newX >= 0 && newX < 2 * Data.MapXdim && newY >= 0 && newY < Data.MapYdim) image = TerrainBitmap((newX - (newY % 2)) / 2, newY);
            //                //else image = Blank;
            //                image = Blank;
            //                graphics.DrawImage(image, 32 * (x_ + 3), 16 * (y_ + 3));
            //            }

            //    //Then draw city
            //    graphics.DrawImage(CreateCityBitmap(city, false, 8), 64 * 1 + 32 * (3 % 2) + 1, 16 * 2 + 1);

            //    //Then draw food/shield/trade icons around the city (21 squares around city)
            //    int[,] offsets = new int[21, 2] { { 0, 0 }, { -1, -3 }, { -3, -1 }, { -3, 1 }, { -1, 3 }, { 1, 3 }, { 3, 1 }, { 3, -1 }, { 1, -3 }, { -2, -2 }, { -2, 2 }, { 2, 2 },
            //                                      { 2, -2 }, { 0, -2 }, { -1, -1 }, { -2, 0 }, { -1, 1 }, { 0, 2 }, { 1, 1 }, { 2, 0 }, { 1, -1 } };    //offset of squares from city square (0,0)
            //    int[] cityFood = city.FoodDistribution;
            //    int[] cityShld = city.ShieldDistribution;
            //    int[] cityTrad = city.TradeDistribution;
            //    for (int i = 0; i < 21; i++)
            //        if (city.DistributionWorkers[i] == 1)
            //        {
            //            //First count all icons on this square to determine the spacing between icons (10 = no spacing, 15 = no spacing @ 50% scaled)
            //            int spacing;
            //            switch (cityFood[i] + cityShld[i] + cityTrad[i])
            //            {
            //                case 1:
            //                case 2: spacing = 17; break;    //50 % larger (orignal = 11, 1 pixel gap)
            //                case 3: spacing = 15; break;    //50 % larger (orignal = 10, no gap)
            //                case 4: spacing = 11; break;    //50 % larger (orignal = 7)
            //                case 5: spacing = 8; break;    //50 % larger (orignal = 5)
            //                case 6: spacing = 6; break;    //50 % larger (orignal = 4)
            //                case 7:
            //                case 8: spacing = 5; break;    //50 % larger (orignal = 3)
            //                case 9: spacing = 3; break;    //50 % larger (orignal = 2)
            //                case 10: spacing = 2; break;    //50 % larger (orignal = 1)
            //                default: spacing = 2; break;    //50 % larger (orignal = 1)
            //            }

            //            //First draw food, then shields, then trade icons
            //            int x_offset = 32 - ((cityFood[i] + cityShld[i] + cityTrad[i] - 1) * spacing + 15) / 2;
            //            int y_offset = 9;
            //            for (int j = 0; j < cityFood[i]; j++) graphics.DrawImage(CitymapFoodSmallBigger, x_offset + (3 + offsets[i, 0]) * 32 + j * spacing, y_offset + (3 + offsets[i, 1]) * 16);
            //            for (int j = 0; j < cityShld[i]; j++) graphics.DrawImage(CitymapShieldSmallBigger, x_offset + (3 + offsets[i, 0]) * 32 + (cityFood[i] + j) * spacing, y_offset + (3 + offsets[i, 1]) * 16);
            //            for (int j = 0; j < cityTrad[i]; j++) graphics.DrawImage(CitymapTradeSmallBigger, x_offset + (3 + offsets[i, 0]) * 32 + (cityFood[i] + cityShld[i] + j) * spacing, y_offset + (3 + offsets[i, 1]) * 16);
            //        }
            //}
            return map;
        }
    }
}
