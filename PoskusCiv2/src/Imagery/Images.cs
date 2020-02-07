using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Reflection;
using System.IO;
using RTciv2.Enums;
using RTciv2.Units;

namespace RTciv2.Imagery
{
    public static class Images
    {
        public static Bitmap CityWallpaper, CityStatusWallpaper, DefenseMinWallpaper, AttitudeAdvWallpaper, TradeAdvWallpaper, ScienceAdvWallpaper, WondersOfWorldWallpaper, 
                             DemographicsWallpaper, MainScreenSymbol, MainScreenSinai, Irrigation, Farmland, Mining, Pollution, Fortified, Fortress, Airbase, AirbasePlane, 
                             Shield, ViewingPieces, WallpaperMapForm, WallpaperStatusForm, UnitShieldShadow, GridLines, GridLinesVisible, Dither, DitherBlank, Blank, DitherBase, 
                             SellIcon, SellIconLarge, CitymapFoodLarge, CitymapFoodLargeBigger, CitymapHungerLarge, CitymapHungerLargeBigger, CitymapFoodSmall, CitymapFoodSmallBigger, 
                             CitymapShieldLarge, CitymapShieldLargeBigger, CitymapShieldSmall, CitymapShieldSmallBigger, CitymapTradeLarge, CitymapTradeLargeBigger, CitymapTradeSmall, 
                             CitymapTradeSmallBigger, CitymapShortageLargeBigger, CitymapShortageLarge, CitymapCorruptionLarge, CitymapCorruptionLargeBigger, CitymapSupportLarge, 
                             CitymapSupportLargeBigger, CitymapLuxLarge, CitymapLuxLargeBigger, CitymapTaxLarge, CitymapTaxLargeBigger, CitymapSciLarge, CitymapSciLargeBigger, NextCity, 
                             NextCityLarge, PrevCity, PrevCityLarge, ZoomIN, ZoomOUT;
        public static Bitmap[] Desert, Plains, Grassland, ForestBase, HillsBase, MtnsBase, Tundra, Glacier, Swamp, Jungle, Ocean, River, Forest, Mountains, Hills,  RiverMouth, Road, 
                               Railroad, Units, UnitShield, NoBorderUnitShield, CityFlag, Improvements, ImprovementsLarge, ImprovementsSmall;
        public static Bitmap[,] Coast, City, CityWall, DitherDesert, DitherPlains, DitherGrassland, DitherForest, DitherHills, DitherMountains, DitherTundra, DitherGlacier, DitherSwamp, 
                                DitherJungle, PeopleL, PeopleLshadow, ResearchIcons;
        public static int[,] unitShieldLocation = new int[63, 2];
        public static int[,,] cityFlagLoc, cityWallFlagLoc, citySizeWindowLoc, cityWallSizeWindowLoc;
        //public static int[,,] cityWallFlagLoc = new int[6, 4, 2];

        #region Load bitmaps from files
        public static void LoadBitmapsFromFiles()
        {
            LoadTerrain();
            LoadCities();
            LoadUnits();
            LoadPeople();
            LoadIcons();
            LoadCityWallpaper();
            LoadDLLimages();
        }
        #region Load Terrain Tiles
        public static void LoadTerrain()
        {
            Bitmap terrain1 = new Bitmap(Properties.Resources.TERRAIN1);
            Bitmap terrain2 = new Bitmap(Properties.Resources.TERRAIN2);
            Desert = new Bitmap[4];
            Plains = new Bitmap[4];
            Grassland = new Bitmap[4];
            ForestBase = new Bitmap[4];
            HillsBase = new Bitmap[4];
            MtnsBase = new Bitmap[4];
            Tundra = new Bitmap[4];
            Glacier = new Bitmap[4];
            Swamp = new Bitmap[4];
            Jungle = new Bitmap[4];
            Ocean = new Bitmap[4];
            Coast = new Bitmap[8, 4];
            River = new Bitmap[16];
            Forest = new Bitmap[16];
            Mountains = new Bitmap[16];
            Hills = new Bitmap[16];
            RiverMouth = new Bitmap[4];
            Road = new Bitmap[9];
            Railroad = new Bitmap[9];

            //define transparent colors
            Color transparentGray = Color.FromArgb(135, 135, 135);    //define transparent back color (gray)
            Color transparentPink = Color.FromArgb(255, 0, 255);    //define transparent back color (pink)
            Color transparentCyan = Color.FromArgb(0, 255, 255);    //define transparent back color (cyan)

            //Tiles
            for (int i = 0; i < 4; i++) { 
                Desert[i] = terrain1.Clone(new Rectangle(i + 1 + i * 64, 1, 64, 32), terrain1.PixelFormat);
                Desert[i].MakeTransparent(transparentGray);
                Desert[i].MakeTransparent(transparentPink);
                Plains[i] = terrain1.Clone(new Rectangle(i + 1 + i * 64, 2 * 1 + 1 * 32, 64, 32), terrain1.PixelFormat);
                Plains[i].MakeTransparent(transparentGray);
                Plains[i].MakeTransparent(transparentPink);
                Grassland[i] = terrain1.Clone(new Rectangle(i + 1 + i * 64, 3 * 1 + 2 * 32, 64, 32), terrain1.PixelFormat);
                Grassland[i].MakeTransparent(transparentGray);
                Grassland[i].MakeTransparent(transparentPink);
                ForestBase[i] = terrain1.Clone(new Rectangle(i + 1 + i * 64, 4 * 1 + 3 * 32, 64, 32), terrain1.PixelFormat);
                ForestBase[i].MakeTransparent(transparentGray);
                ForestBase[i].MakeTransparent(transparentPink);
                HillsBase[i] = terrain1.Clone(new Rectangle(i + 1 + i * 64, 5 * 1 + 4 * 32, 64, 32), terrain1.PixelFormat);
                HillsBase[i].MakeTransparent(transparentGray);
                HillsBase[i].MakeTransparent(transparentPink);
                MtnsBase[i] = terrain1.Clone(new Rectangle(i + 1 + i * 64, 6 * 1 + 5 * 32, 64, 32), terrain1.PixelFormat);
                MtnsBase[i].MakeTransparent(transparentGray);
                MtnsBase[i].MakeTransparent(transparentPink);
                Tundra[i] = terrain1.Clone(new Rectangle(i + 1 + i * 64, 7 * 1 + 6 * 32, 64, 32), terrain1.PixelFormat);
                Tundra[i].MakeTransparent(transparentGray);
                Tundra[i].MakeTransparent(transparentPink);
                Glacier[i] = terrain1.Clone(new Rectangle(i + 1 + i * 64, 8 * 1 + 7 * 32, 64, 32), terrain1.PixelFormat);
                Glacier[i].MakeTransparent(transparentGray);
                Glacier[i].MakeTransparent(transparentPink);
                Swamp[i] = terrain1.Clone(new Rectangle(i + 1 + i * 64, 9 * 1 + 8 * 32, 64, 32), terrain1.PixelFormat);
                Swamp[i].MakeTransparent(transparentGray);
                Swamp[i].MakeTransparent(transparentPink);
                Jungle[i] = terrain1.Clone(new Rectangle(i + 1 + i * 64, 10 * 1 + 9 * 32, 64, 32), terrain1.PixelFormat);
                Jungle[i].MakeTransparent(transparentGray);
                Jungle[i].MakeTransparent(transparentPink);
                Ocean[i] = terrain1.Clone(new Rectangle(i + 1 + i * 64, 11 * 1 + 10 * 32, 64, 32), terrain1.PixelFormat);
                Ocean[i].MakeTransparent(transparentGray);
                Ocean[i].MakeTransparent(transparentPink); }

            //Dither
            DitherBlank = terrain1.Clone(new Rectangle(1, 447, 64, 32), terrain1.PixelFormat);

            //Blank tile
            Blank = terrain1.Clone(new Rectangle(131, 447, 64, 32), terrain1.PixelFormat);
            Blank.MakeTransparent(transparentGray);

            //Dither base (only useful for grasland?)
            DitherBase = terrain1.Clone(new Rectangle(196, 447, 64, 32), terrain1.PixelFormat);

            //Replace black dither pixels with base pixels
            DitherDesert = new Bitmap[2, 2];   //4 dither tiles for one 64x32 map tile
            DitherPlains = new Bitmap[2, 2];
            DitherGrassland = new Bitmap[2, 2];
            DitherForest = new Bitmap[2, 2];
            DitherHills = new Bitmap[2, 2];
            DitherMountains = new Bitmap[2, 2];
            DitherTundra = new Bitmap[2, 2];
            DitherGlacier = new Bitmap[2, 2];
            DitherSwamp = new Bitmap[2, 2];
            DitherJungle = new Bitmap[2, 2];
            Color replacementColor;
            for (int tileX = 0; tileX < 2; tileX++) {    //for 4 directions (NE, SE, SW, NW)
                for (int tileY = 0; tileY < 2; tileY++) {
                    DitherDesert[tileX, tileY] = new Bitmap(32, 16);
                    DitherPlains[tileX, tileY] = new Bitmap(32, 16);
                    DitherGrassland[tileX, tileY] = new Bitmap(32, 16);
                    DitherForest[tileX, tileY] = new Bitmap(32, 16);
                    DitherHills[tileX, tileY] = new Bitmap(32, 16);
                    DitherMountains[tileX, tileY] = new Bitmap(32, 16);
                    DitherTundra[tileX, tileY] = new Bitmap(32, 16);
                    DitherGlacier[tileX, tileY] = new Bitmap(32, 16);
                    DitherSwamp[tileX, tileY] = new Bitmap(32, 16);
                    DitherJungle[tileX, tileY] = new Bitmap(32, 16);
                    for (int col = 0; col < 32; col++) {
                        for (int row = 0; row < 16; row++) {
                            replacementColor = DitherBlank.GetPixel(tileX * 32 + col, tileY * 16 + row);
                            if (replacementColor == Color.FromArgb(0, 0, 0)) {
                                DitherDesert[tileX, tileY].SetPixel(col, row, Desert[0].GetPixel(tileX * 32 + col, tileY * 16 + row));
                                DitherPlains[tileX, tileY].SetPixel(col, row, Plains[0].GetPixel(tileX * 32 + col, tileY * 16 + row));
                                DitherGrassland[tileX, tileY].SetPixel(col, row, Grassland[0].GetPixel(tileX * 32 + col, tileY * 16 + row));
                                DitherForest[tileX, tileY].SetPixel(col, row, ForestBase[0].GetPixel(tileX * 32 + col, tileY * 16 + row));
                                DitherHills[tileX, tileY].SetPixel(col, row, HillsBase[0].GetPixel(tileX * 32 + col, tileY * 16 + row));
                                DitherMountains[tileX, tileY].SetPixel(col, row, MtnsBase[0].GetPixel(tileX * 32 + col, tileY * 16 + row));
                                DitherTundra[tileX, tileY].SetPixel(col, row, Tundra[0].GetPixel(tileX * 32 + col, tileY * 16 + row));
                                DitherGlacier[tileX, tileY].SetPixel(col, row, Glacier[0].GetPixel(tileX * 32 + col, tileY * 16 + row));
                                DitherSwamp[tileX, tileY].SetPixel(col, row, Swamp[0].GetPixel(tileX * 32 + col, tileY * 16 + row));
                                DitherJungle[tileX, tileY].SetPixel(col, row, Jungle[0].GetPixel(tileX * 32 + col, tileY * 16 + row)); }
                            else {
                                DitherDesert[tileX, tileY].SetPixel(col, row, Color.Transparent);
                                DitherPlains[tileX, tileY].SetPixel(col, row, Color.Transparent);
                                DitherGrassland[tileX, tileY].SetPixel(col, row, Color.Transparent);
                                DitherForest[tileX, tileY].SetPixel(col, row, Color.Transparent);
                                DitherHills[tileX, tileY].SetPixel(col, row, Color.Transparent);
                                DitherMountains[tileX, tileY].SetPixel(col, row, Color.Transparent);
                                DitherTundra[tileX, tileY].SetPixel(col, row, Color.Transparent);
                                DitherGlacier[tileX, tileY].SetPixel(col, row, Color.Transparent);
                                DitherSwamp[tileX, tileY].SetPixel(col, row, Color.Transparent);
                                DitherJungle[tileX, tileY].SetPixel(col, row, Color.Transparent); } } } } }


            //Rivers, Forest, Mountains, Hills
            for (int i = 0; i < 16; i++) {
                River[i] = terrain2.Clone(new Rectangle(i % 8 + 1 + (i % 8) * 64, 3 + i / 8 + (2 + i / 8) * 32, 64, 32), terrain2.PixelFormat);
                River[i].MakeTransparent(transparentGray);
                River[i].MakeTransparent(transparentPink);
                Forest[i] = terrain2.Clone(new Rectangle(i % 8 + 1 + (i % 8) * 64, 5 + i / 8 + (4 + i / 8) * 32, 64, 32), terrain2.PixelFormat);
                Forest[i].MakeTransparent(transparentGray);
                Forest[i].MakeTransparent(transparentPink);
                Mountains[i] = terrain2.Clone(new Rectangle(i % 8 + 1 + (i % 8) * 64, 7 + i / 8 + (6 + i / 8) * 32, 64, 32), terrain2.PixelFormat);
                Mountains[i].MakeTransparent(transparentGray);
                Mountains[i].MakeTransparent(transparentPink);
                Hills[i] = terrain2.Clone(new Rectangle(i % 8 + 1 + (i % 8) * 64, 9 + i / 8 + (8 + i / 8) * 32, 64, 32), terrain2.PixelFormat);
                Hills[i].MakeTransparent(transparentGray);
                Hills[i].MakeTransparent(transparentPink); }

            //River mouths
            for (int i = 0; i < 4; i++) {
                RiverMouth[i] = terrain2.Clone(new Rectangle(i + 1 + i * 64, 11 * 1 + 10 * 32, 64, 32), terrain2.PixelFormat);
                RiverMouth[i].MakeTransparent(transparentGray);
                RiverMouth[i].MakeTransparent(transparentPink);
                RiverMouth[i].MakeTransparent(transparentCyan); }

            //Coast
            for (int i = 0; i < 8; i++) {
                Coast[i, 0] = terrain2.Clone(new Rectangle(2 * i + 1 + 2 * i * 32, 429, 32, 16), terrain2.PixelFormat);  // N
                Coast[i, 0].MakeTransparent(transparentGray);
                Coast[i, 1] = terrain2.Clone(new Rectangle(2 * i + 1 + 2 * i * 32, 429 + 1 * 1 + 1 * 16, 32, 16), terrain2.PixelFormat);  // S
                Coast[i, 1].MakeTransparent(transparentGray);
                Coast[i, 2] = terrain2.Clone(new Rectangle(2 * i + 1 + 2 * i * 32, 429 + 2 * 1 + 2 * 16, 32, 16), terrain2.PixelFormat);  // W
                Coast[i, 2].MakeTransparent(transparentGray);
                Coast[i, 3] = terrain2.Clone(new Rectangle(2 * (i + 1) + (2 * i + 1) * 32, 429 + 2 * 1 + 2 * 16, 32, 16), terrain2.PixelFormat);  // E
                Coast[i, 3].MakeTransparent(transparentGray); }

            //Road & railorad
            for (int i = 0; i < 9; i++) {
                Road[i] = terrain1.Clone(new Rectangle(i + 1 + i * 64, 364, 64, 32), terrain1.PixelFormat);
                Road[i].MakeTransparent(transparentGray);
                Road[i].MakeTransparent(transparentPink);
                Railroad[i] = terrain1.Clone(new Rectangle(i + 1 + i * 64, 397, 64, 32), terrain1.PixelFormat);
                Railroad[i].MakeTransparent(transparentGray);
                Railroad[i].MakeTransparent(transparentPink); }

            Irrigation = terrain1.Clone(new Rectangle(456, 100, 64, 32), terrain1.PixelFormat);
            Irrigation.MakeTransparent(transparentGray);
            Irrigation.MakeTransparent(transparentPink);

            Farmland = terrain1.Clone(new Rectangle(456, 133, 64, 32), terrain1.PixelFormat);
            Farmland.MakeTransparent(transparentGray);
            Farmland.MakeTransparent(transparentPink);

            Mining = terrain1.Clone(new Rectangle(456, 166, 64, 32), terrain1.PixelFormat);
            Mining.MakeTransparent(transparentGray);
            Mining.MakeTransparent(transparentPink);

            Pollution = terrain1.Clone(new Rectangle(456, 199, 64, 32), terrain1.PixelFormat);
            Pollution.MakeTransparent(transparentGray);
            Pollution.MakeTransparent(transparentPink);

            Shield = terrain1.Clone(new Rectangle(456, 232, 64, 32), terrain1.PixelFormat);
            Shield.MakeTransparent(transparentGray);
            Shield.MakeTransparent(transparentPink);

            terrain1.Dispose();
            terrain2.Dispose();
        }
        #endregion
        #region Load Cities
        public static void LoadCities()
        {
            Bitmap cities = new Bitmap(Properties.Resources.CITIES);
            City = new Bitmap[6, 4];
            CityFlag = new Bitmap[9];
            CityWall = new Bitmap[6, 4];
            cityFlagLoc = new int[6, 4, 2];
            cityWallFlagLoc = new int[6, 4, 2];
            citySizeWindowLoc = new int[6, 4, 2];
            cityWallSizeWindowLoc = new int[6, 4, 2];

            //define transparent colors
            Color transparentGray = Color.FromArgb(135, 135, 135);    //define transparent back color (gray)
            Color transparentPink = Color.FromArgb(255, 0, 255);    //define transparent back color (pink)
            //Color transparentCyan = Color.FromArgb(0, 255, 255);    //define transparent back color (cyan)

            //Get city bitmaps
            for (int row = 0; row < 6; row++) {
                for (int col = 0; col < 4; col++) {
                    City[row, col] = cities.Clone(new Rectangle(1 + 65 * col, 39 + 49 * row, 64, 48), cities.PixelFormat);
                    City[row, col].MakeTransparent(transparentGray);
                    City[row, col].MakeTransparent(transparentPink);
                    CityWall[row, col] = cities.Clone(new Rectangle(334 + 65 * col, 39 + 49 * row, 64, 48), cities.PixelFormat);
                    CityWall[row, col].MakeTransparent(transparentGray);
                    CityWall[row, col].MakeTransparent(transparentPink);
                    //determine where the city size window is located (x-y)
                    for (int ix = 0; ix < 64; ix++) { //in x-direction
                        if (cities.GetPixel(65 * col + ix, 38 + 49 * row) == Color.FromArgb(0, 0, 255)) cityFlagLoc[row, col, 0] = ix;  //if pixel on border is blue
                        if (cities.GetPixel(333 + 65 * col + ix, 38 + 49 * row) == Color.FromArgb(0, 0, 255)) cityWallFlagLoc[row, col, 0] = ix; } //for cities with wall
                    for (int iy = 0; iy < 48; iy++) { //in y-direction
                        if (cities.GetPixel(65 * col, 38 + 49 * row + iy) == Color.FromArgb(0, 0, 255)) cityFlagLoc[row, col, 1] = iy;
                        if (cities.GetPixel(333 + 65 * col, 38 + 49 * row + iy) == Color.FromArgb(0, 0, 255)) cityWallFlagLoc[row, col, 1] = iy; } } }

            //Get flag bitmaps
            for (int col = 0; col < 9; col++) {
                CityFlag[col] = cities.Clone(new Rectangle(1 + 15 * col, 425, 14, 22), cities.PixelFormat);
                CityFlag[col].MakeTransparent(transparentGray); }

            //Locations of city size windows
            citySizeWindowLoc[0, 0, 0] = 13;    //stone age
            citySizeWindowLoc[0, 0, 1] = 23;
            citySizeWindowLoc[0, 1, 0] = 52;
            citySizeWindowLoc[0, 1, 1] = 18;
            citySizeWindowLoc[0, 2, 0] = 0;
            citySizeWindowLoc[0, 2, 1] = 23;
            citySizeWindowLoc[0, 3, 0] = 24;
            citySizeWindowLoc[0, 3, 1] = 29;
            citySizeWindowLoc[1, 0, 0] = 10;    //ancient
            citySizeWindowLoc[1, 0, 1] = 23;
            citySizeWindowLoc[1, 1, 0] = 50;
            citySizeWindowLoc[1, 1, 1] = 25;
            citySizeWindowLoc[1, 2, 0] = 1;
            citySizeWindowLoc[1, 2, 1] = 17;
            citySizeWindowLoc[1, 3, 0] = 12;
            citySizeWindowLoc[1, 3, 1] = 27;
            citySizeWindowLoc[2, 0, 0] = 3;    //far east
            citySizeWindowLoc[2, 0, 1] = 20;
            citySizeWindowLoc[2, 1, 0] = 48;
            citySizeWindowLoc[2, 1, 1] = 7;
            citySizeWindowLoc[2, 2, 0] = 50;
            citySizeWindowLoc[2, 2, 1] = 5;
            citySizeWindowLoc[2, 3, 0] = 28;
            citySizeWindowLoc[2, 3, 1] = 27;
            citySizeWindowLoc[3, 0, 0] = 5;    //medieval
            citySizeWindowLoc[3, 0, 1] = 22;
            citySizeWindowLoc[3, 1, 0] = 2;
            citySizeWindowLoc[3, 1, 1] = 18;
            citySizeWindowLoc[3, 2, 0] = 0;
            citySizeWindowLoc[3, 2, 1] = 18;
            citySizeWindowLoc[3, 3, 0] = 27;
            citySizeWindowLoc[3, 3, 1] = 27;
            citySizeWindowLoc[4, 0, 0] = 4;    //industrial
            citySizeWindowLoc[4, 0, 1] = 20;
            citySizeWindowLoc[4, 1, 0] = 1;
            citySizeWindowLoc[4, 1, 1] = 20;
            citySizeWindowLoc[4, 2, 0] = 2;
            citySizeWindowLoc[4, 2, 1] = 22;
            citySizeWindowLoc[4, 3, 0] = 28;
            citySizeWindowLoc[4, 3, 1] = 30;
            citySizeWindowLoc[5, 0, 0] = 8;    //modern
            citySizeWindowLoc[5, 0, 1] = 18;
            citySizeWindowLoc[5, 1, 0] = 2;
            citySizeWindowLoc[5, 1, 1] = 19;
            citySizeWindowLoc[5, 2, 0] = 8;
            citySizeWindowLoc[5, 2, 1] = 20;
            citySizeWindowLoc[5, 3, 0] = 27;
            citySizeWindowLoc[5, 3, 1] = 30;
            cityWallSizeWindowLoc[0, 0, 0] = 12;    //stone + wall
            cityWallSizeWindowLoc[0, 0, 1] = 23;
            cityWallSizeWindowLoc[0, 1, 0] = 52;
            cityWallSizeWindowLoc[0, 1, 1] = 22;
            cityWallSizeWindowLoc[0, 2, 0] = 0;
            cityWallSizeWindowLoc[0, 2, 1] = 19;
            cityWallSizeWindowLoc[0, 3, 0] = 24;
            cityWallSizeWindowLoc[0, 3, 1] = 29;
            cityWallSizeWindowLoc[1, 0, 0] = 10;    //ancient + wall
            cityWallSizeWindowLoc[1, 0, 1] = 13;
            cityWallSizeWindowLoc[1, 1, 0] = 50;
            cityWallSizeWindowLoc[1, 1, 1] = 21;
            cityWallSizeWindowLoc[1, 2, 0] = 1;
            cityWallSizeWindowLoc[1, 2, 1] = 17;
            cityWallSizeWindowLoc[1, 3, 0] = 11;
            cityWallSizeWindowLoc[1, 3, 1] = 22;
            cityWallSizeWindowLoc[2, 0, 0] = 4;    //far east + wall
            cityWallSizeWindowLoc[2, 0, 1] = 18;
            cityWallSizeWindowLoc[2, 1, 0] = 48;
            cityWallSizeWindowLoc[2, 1, 1] = 6;
            cityWallSizeWindowLoc[2, 2, 0] = 51;
            cityWallSizeWindowLoc[2, 2, 1] = 4;
            cityWallSizeWindowLoc[2, 3, 0] = 28;
            cityWallSizeWindowLoc[2, 3, 1] = 27;
            cityWallSizeWindowLoc[3, 0, 0] = 3;    //medieval + wall
            cityWallSizeWindowLoc[3, 0, 1] = 18;
            cityWallSizeWindowLoc[3, 1, 0] = 2;
            cityWallSizeWindowLoc[3, 1, 1] = 20;
            cityWallSizeWindowLoc[3, 2, 0] = 1;
            cityWallSizeWindowLoc[3, 2, 1] = 15;
            cityWallSizeWindowLoc[3, 3, 0] = 27;
            cityWallSizeWindowLoc[3, 3, 1] = 29;
            cityWallSizeWindowLoc[4, 0, 0] = 4;    //industrial + wall
            cityWallSizeWindowLoc[4, 0, 1] = 18;
            cityWallSizeWindowLoc[4, 1, 0] = 1;
            cityWallSizeWindowLoc[4, 1, 1] = 20;
            cityWallSizeWindowLoc[4, 2, 0] = 1;
            cityWallSizeWindowLoc[4, 2, 1] = 18;
            cityWallSizeWindowLoc[4, 3, 0] = 26;
            cityWallSizeWindowLoc[4, 3, 1] = 28;
            cityWallSizeWindowLoc[5, 0, 0] = 3;    //modern + wall
            cityWallSizeWindowLoc[5, 0, 1] = 21;
            cityWallSizeWindowLoc[5, 1, 0] = 0;
            cityWallSizeWindowLoc[5, 1, 1] = 20;
            cityWallSizeWindowLoc[5, 2, 0] = 8;
            cityWallSizeWindowLoc[5, 2, 1] = 20;
            cityWallSizeWindowLoc[5, 3, 0] = 27;
            cityWallSizeWindowLoc[5, 3, 1] = 30;

            Fortified = cities.Clone(new Rectangle(143, 423, 64, 48), cities.PixelFormat);
            Fortified.MakeTransparent(transparentGray);
            Fortified.MakeTransparent(transparentPink);

            Fortress = cities.Clone(new Rectangle(208, 423, 64, 48), cities.PixelFormat);
            Fortress.MakeTransparent(transparentGray);
            Fortress.MakeTransparent(transparentPink);

            Airbase = cities.Clone(new Rectangle(273, 423, 64, 48), cities.PixelFormat);
            Airbase.MakeTransparent(transparentGray);
            Airbase.MakeTransparent(transparentPink);

            AirbasePlane = cities.Clone(new Rectangle(338, 423, 64, 48), cities.PixelFormat);
            AirbasePlane.MakeTransparent(transparentGray);
            AirbasePlane.MakeTransparent(transparentPink);

            cities.Dispose();
        }
        #endregion
        #region Load Units
        public static void LoadUnits()
        {
            Bitmap units = new Bitmap(Properties.Resources.UNITS);

            Units = new Bitmap[63];
            UnitShield = new Bitmap[8];
            NoBorderUnitShield = new Bitmap[8];

            //define transparent colors
            Color transparentGray = Color.FromArgb(135, 83, 135);    //define transparent back color (gray)
            Color transparentPink = Color.FromArgb(255, 0, 255);    //define transparent back color (pink)

            int count = 0;
            for (int row = 0; row < 7; row++) {
                for (int col = 0; col < 9; col++) {
                    Units[count] = units.Clone(new Rectangle(64 * col + 1 + col, 48 * row + 1 + row, 64, 48), units.PixelFormat);
                    Units[count].MakeTransparent(transparentGray);
                    Units[count].MakeTransparent(transparentPink);
                    //determine where the unit shield is located (x-y)
                    for (int ix = 0; ix < 64; ix++) if (units.GetPixel(65 * col + ix, 49 * row) == Color.FromArgb(0, 0, 255)) unitShieldLocation[count, 0] = ix;  //if pixel on border is blue, in x-direction
                    for (int iy = 0; iy < 48; iy++) if (units.GetPixel(65 * col, 49 * row + iy) == Color.FromArgb(0, 0, 255)) unitShieldLocation[count, 1] = iy;  //in y-direction
                    count++; } }

            //Extract shield without black border (used for stacked units)
            Bitmap _backUnitShield = units.Clone(new Rectangle(586, 1, 12, 20), units.PixelFormat);
            _backUnitShield.MakeTransparent(transparentGray);

            //Extract unit shield
            Bitmap _unitShield = units.Clone(new Rectangle(597, 30, 12, 20), units.PixelFormat);
            _unitShield.MakeTransparent(transparentGray);

            //Make shields of different colors for 8 different civs
            UnitShield[0] = CreateNonIndexedImage(_unitShield); //convert GIF to non-indexed picture
            UnitShield[1] = CreateNonIndexedImage(_unitShield);
            UnitShield[2] = CreateNonIndexedImage(_unitShield);
            UnitShield[3] = CreateNonIndexedImage(_unitShield);
            UnitShield[4] = CreateNonIndexedImage(_unitShield);
            UnitShield[5] = CreateNonIndexedImage(_unitShield);
            UnitShield[6] = CreateNonIndexedImage(_unitShield);
            UnitShield[7] = CreateNonIndexedImage(_unitShield);
            NoBorderUnitShield[0] = CreateNonIndexedImage(_backUnitShield);
            NoBorderUnitShield[1] = CreateNonIndexedImage(_backUnitShield);
            NoBorderUnitShield[2] = CreateNonIndexedImage(_backUnitShield);
            NoBorderUnitShield[3] = CreateNonIndexedImage(_backUnitShield);
            NoBorderUnitShield[4] = CreateNonIndexedImage(_backUnitShield);
            NoBorderUnitShield[5] = CreateNonIndexedImage(_backUnitShield);
            NoBorderUnitShield[6] = CreateNonIndexedImage(_backUnitShield);
            NoBorderUnitShield[7] = CreateNonIndexedImage(_backUnitShield);
            UnitShieldShadow = CreateNonIndexedImage(_backUnitShield);
            //Replace colors for unit shield and dark unit shield
            for (int x = 0; x < 12; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    if (_unitShield.GetPixel(x, y) == transparentPink)   //if color is pink, replace it
                    {
                        UnitShield[0].SetPixel(x, y, CivColors.Light[0]);  //red
                        UnitShield[1].SetPixel(x, y, CivColors.Light[1]);  //white
                        UnitShield[2].SetPixel(x, y, CivColors.Light[2]);  //green
                        UnitShield[3].SetPixel(x, y, CivColors.Light[3]);  //blue
                        UnitShield[4].SetPixel(x, y, CivColors.Light[4]);  //yellow
                        UnitShield[5].SetPixel(x, y, CivColors.Light[5]);  //cyan
                        UnitShield[6].SetPixel(x, y, CivColors.Light[6]);  //orange
                        UnitShield[7].SetPixel(x, y, CivColors.Light[7]);  //purple
                    }
            
                    if (_backUnitShield.GetPixel(x, y) == Color.FromArgb(255, 0, 0))    //if color is red, replace it
                    {
                        NoBorderUnitShield[0].SetPixel(x, y, CivColors.Dark[0]);  //red
                        NoBorderUnitShield[1].SetPixel(x, y, CivColors.Dark[1]);  //white
                        NoBorderUnitShield[2].SetPixel(x, y, CivColors.Dark[2]);  //green
                        NoBorderUnitShield[3].SetPixel(x, y, CivColors.Dark[3]);  //blue
                        NoBorderUnitShield[4].SetPixel(x, y, CivColors.Dark[4]);  //yellow
                        NoBorderUnitShield[5].SetPixel(x, y, CivColors.Dark[5]);  //cyan
                        NoBorderUnitShield[6].SetPixel(x, y, CivColors.Dark[6]);  //orange
                        NoBorderUnitShield[7].SetPixel(x, y, CivColors.Dark[7]);  //purple
                        UnitShieldShadow.SetPixel(x, y, Color.FromArgb(51, 51, 51));    //color of the shield shadow
                    }
                }
            }   

            units.Dispose();
        }
        #endregion
        #region Load People
        public static void LoadPeople()
        {
            Bitmap icons = new Bitmap(Properties.Resources.PEOPLE);

            PeopleL = new Bitmap[11, 4];
            PeopleLshadow = new Bitmap[11, 4];

            //define transparent colors
            Color transparentPink = Color.FromArgb(255, 0, 255);    //define transparent back color (pink)

            //Make shadows of faces
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 11; col++)
                {
                    PeopleL[col, row] = icons.Clone(new Rectangle(27 * col + 2 + col, 30 * row + 6 + row, 27, 30), icons.PixelFormat);

                    PeopleLshadow[col, row] = CreateNonIndexedImage(PeopleL[col, row]); //convert GIF to non-indexed picture

                    //If color is non-pink, replace it with black to get shadow (otherwise make transparent)
                    for (int x = 0; x < 27; x++)
                        for (int y = 0; y < 30; y++)
                            if (PeopleL[col, row].GetPixel(x, y) != transparentPink) PeopleLshadow[col, row].SetPixel(x, y, Color.Black);
                            else PeopleLshadow[col, row].SetPixel(x, y, Color.Transparent);
                    PeopleL[col, row].MakeTransparent(transparentPink);
                }
            }

            icons.Dispose();
        }
        #endregion
        #region Load Icons
        public static void LoadIcons()
        {
            Bitmap icons = new Bitmap(Properties.Resources.ICONS);

            Improvements = new Bitmap[67];
            ImprovementsLarge = new Bitmap[67];
            ImprovementsSmall = new Bitmap[67];
            ResearchIcons = new Bitmap[5, 4];

            //define transparent colors
            Color transparentLightPink = Color.FromArgb(255, 159, 163);//define transparent back color (light pink)
            Color transparentPink = Color.FromArgb(255, 0, 255);    //define transparent back color (pink)
            //Color transparentGray = Color.FromArgb(135, 83, 135);    //define transparent back color (gray)

            //Improvement icons
            int count = 1;  //start at 1. 0 is for no improvement.
            for (int row = 0; row < 5; row++)
                for (int col = 0; col < 8; col++)
                {
                    Improvements[count] = icons.Clone(new Rectangle(343 + 36 * col + col, 1 + 20 * row + row, 36, 20), icons.PixelFormat);
                    ImprovementsLarge[count] = ModifyImage.ResizeImage(Improvements[count], 54, 30);    //50% larger
                    ImprovementsSmall[count] = ModifyImage.ResizeImage(Improvements[count], 29, 16);    //25% smaller
                    count++;
                    if (count == 39) break;
                }

            //WondersIcons
            for (int row = 0; row < 4; row++)
                for (int col = 0; col < 7; col++)
                {
                    Improvements[count] = icons.Clone(new Rectangle(343 + 36 * col + col, 106 + 20 * row + row, 36, 20), icons.PixelFormat);
                    ImprovementsLarge[count] = ModifyImage.ResizeImage(Improvements[count], 54, 30);    //50% larger
                    ImprovementsSmall[count] = ModifyImage.ResizeImage(Improvements[count], 29, 16);    //25% smaller
                    count++;
                }
            
            //Research icons
            for (int row = 0; row < 4; row++)
                for (int col = 0; col < 5; col++)
                    ResearchIcons[col, row] = icons.Clone(new Rectangle(343 + 36 * col + col, 211 + 20 * row + row, 36, 20), icons.PixelFormat);

            SellIcon = icons.Clone(new Rectangle(16, 320, 14, 14), icons.PixelFormat);
            SellIcon.MakeTransparent(transparentLightPink);

            SellIconLarge = ModifyImage.ResizeImage(SellIcon, 16, 16);

            ViewingPieces = icons.Clone(new Rectangle(199, 256, 64, 32), icons.PixelFormat);
            ViewingPieces.MakeTransparent(transparentLightPink);
            ViewingPieces.MakeTransparent(transparentPink);

            GridLines = icons.Clone(new Rectangle(183, 430, 64, 32), icons.PixelFormat);
            GridLines.MakeTransparent(transparentLightPink);
            GridLines.MakeTransparent(transparentPink);

            GridLinesVisible = icons.Clone(new Rectangle(248, 430, 64, 32), icons.PixelFormat);
            GridLinesVisible.MakeTransparent(transparentLightPink);
            GridLinesVisible.MakeTransparent(transparentPink);

            WallpaperMapForm = icons.Clone(new Rectangle(199, 322, 64, 32), icons.PixelFormat);
            WallpaperStatusForm = icons.Clone(new Rectangle(299, 190, 31, 31), icons.PixelFormat);

            //Big icons in city resources
            CitymapHungerLarge = icons.Clone(new Rectangle(1, 290, 14, 14), icons.PixelFormat);
            CitymapHungerLarge.MakeTransparent(transparentLightPink);
            CitymapHungerLargeBigger = ModifyImage.ResizeImage(CitymapHungerLarge, 21, 21);    //50% larger

            CitymapShortageLarge = icons.Clone(new Rectangle(16, 290, 14, 14), icons.PixelFormat);
            CitymapShortageLarge.MakeTransparent(transparentLightPink);
            CitymapShortageLargeBigger = ModifyImage.ResizeImage(CitymapShortageLarge, 21, 21);    //50% larger

            CitymapCorruptionLarge = icons.Clone(new Rectangle(31, 290, 14, 14), icons.PixelFormat);
            CitymapCorruptionLarge.MakeTransparent(transparentLightPink);
            CitymapCorruptionLargeBigger = ModifyImage.ResizeImage(CitymapCorruptionLarge, 21, 21);    //50% larger

            CitymapFoodLarge = icons.Clone(new Rectangle(1, 305, 14, 14), icons.PixelFormat);
            CitymapFoodLarge.MakeTransparent(transparentLightPink);
            CitymapFoodLargeBigger = ModifyImage.ResizeImage(CitymapFoodLarge, 21, 21);    //50% larger

            CitymapSupportLarge = icons.Clone(new Rectangle(16, 305, 14, 14), icons.PixelFormat);
            CitymapSupportLarge.MakeTransparent(transparentLightPink);
            CitymapSupportLargeBigger = ModifyImage.ResizeImage(CitymapSupportLarge, 21, 21);    //50% larger

            CitymapTradeLarge = icons.Clone(new Rectangle(31, 305, 14, 14), icons.PixelFormat);
            CitymapTradeLarge.MakeTransparent(transparentLightPink);
            CitymapTradeLargeBigger = ModifyImage.ResizeImage(CitymapTradeLarge, 21, 21);    //50% larger

            CitymapLuxLarge = icons.Clone(new Rectangle(1, 320, 14, 14), icons.PixelFormat);
            CitymapLuxLarge.MakeTransparent(transparentLightPink);
            CitymapLuxLargeBigger = ModifyImage.ResizeImage(CitymapLuxLarge, 21, 21);    //50% larger

            CitymapTaxLarge = icons.Clone(new Rectangle(16, 320, 14, 14), icons.PixelFormat);
            CitymapTaxLarge.MakeTransparent(transparentLightPink);
            CitymapTaxLargeBigger = ModifyImage.ResizeImage(CitymapTaxLarge, 21, 21);    //50% larger

            CitymapSciLarge = icons.Clone(new Rectangle(31, 320, 14, 14), icons.PixelFormat);
            CitymapSciLarge.MakeTransparent(transparentLightPink);
            CitymapSciLargeBigger = ModifyImage.ResizeImage(CitymapSciLarge, 21, 21);    //50% larger
            
            //Small icons in city resources
            CitymapFoodSmall = icons.Clone(new Rectangle(49, 334, 10, 10), icons.PixelFormat);
            CitymapFoodSmall.MakeTransparent(transparentLightPink);
            CitymapFoodSmallBigger = ModifyImage.ResizeImage(CitymapFoodSmall, 15, 15);    //50% larger

            CitymapShieldSmall = icons.Clone(new Rectangle(60, 334, 10, 10), icons.PixelFormat);
            CitymapShieldSmall.MakeTransparent(transparentLightPink);
            CitymapShieldSmallBigger = ModifyImage.ResizeImage(CitymapShieldSmall, 15, 15);    //50% larger

            CitymapTradeSmall = icons.Clone(new Rectangle(71, 334, 10, 10), icons.PixelFormat);
            CitymapTradeSmall.MakeTransparent(transparentLightPink);
            CitymapTradeSmallBigger = ModifyImage.ResizeImage(CitymapTradeSmall, 15, 15);    //50% larger

            //Icon for next/previous city (black arrow)
            NextCity = icons.Clone(new Rectangle(227, 389, 18, 24), icons.PixelFormat);
            PrevCity = icons.Clone(new Rectangle(246, 389, 18, 24), icons.PixelFormat);
            NextCity.MakeTransparent(transparentLightPink);
            PrevCity.MakeTransparent(transparentLightPink);
            NextCityLarge = ModifyImage.ResizeImage(NextCity, 27, 36);    //50% larger
            PrevCityLarge = ModifyImage.ResizeImage(PrevCity, 27, 36);    //50% larger

            //Zoom icons
            ZoomIN = icons.Clone(new Rectangle(18, 389, 16, 16), icons.PixelFormat);
            ZoomOUT = icons.Clone(new Rectangle(35, 389, 16, 16), icons.PixelFormat);

            icons.Dispose();
        }
        #endregion
        #region Load City Wallpaper
        public static void LoadCityWallpaper()
        {
            Bitmap cityWallpaper = new Bitmap(Properties.Resources.CITY);
            //CityWallpaper = (Bitmap)cityWallpaper;
            CityWallpaper = ModifyImage.CropImage(cityWallpaper, new Rectangle(0, 0, 640, 420));
        }
        #endregion        
        #region Loads DLL images
        public static void LoadDLLimages()
        {
            Bitmap cityStatusWallpaper = new Bitmap(Properties.Resources.DLL01);
            CityStatusWallpaper = cityStatusWallpaper;
            CityStatusWallpaper = ModifyImage.CropImage(CityStatusWallpaper, new Rectangle(0, 0, 600, 400));

            Bitmap defenseMinWallpaper = new Bitmap(Properties.Resources.DLL02);
            DefenseMinWallpaper = defenseMinWallpaper;
            DefenseMinWallpaper = ModifyImage.CropImage(DefenseMinWallpaper, new Rectangle(0, 0, 600, 400));

            Bitmap attitudeAdvWallpaper = new Bitmap(Properties.Resources.DLL04);
            AttitudeAdvWallpaper = attitudeAdvWallpaper;
            AttitudeAdvWallpaper = ModifyImage.CropImage(AttitudeAdvWallpaper, new Rectangle(0, 0, 600, 400));

            Bitmap tradeAdvWallpaper = new Bitmap(Properties.Resources.DLL05);
            TradeAdvWallpaper = tradeAdvWallpaper;
            TradeAdvWallpaper = ModifyImage.CropImage(TradeAdvWallpaper, new Rectangle(0, 0, 600, 400));

            Bitmap scienceAdvWallpaper = new Bitmap(Properties.Resources.DLL06);
            ScienceAdvWallpaper = scienceAdvWallpaper;
            ScienceAdvWallpaper = ModifyImage.CropImage(ScienceAdvWallpaper, new Rectangle(0, 0, 600, 400));

            Bitmap wowWallpaper = new Bitmap(Properties.Resources.DLL07);
            WondersOfWorldWallpaper = wowWallpaper;
            WondersOfWorldWallpaper = ModifyImage.CropImage(WondersOfWorldWallpaper, new Rectangle(0, 0, 600, 400));

            Bitmap demographicsWallpaper = new Bitmap(Properties.Resources.DLL09);
            DemographicsWallpaper = demographicsWallpaper;
            DemographicsWallpaper = ModifyImage.CropImage(DemographicsWallpaper, new Rectangle(0, 0, 600, 400));

            Bitmap mainScreenSymbol = new Bitmap(Properties.Resources.DLL23);
            MainScreenSymbol = mainScreenSymbol;

            Bitmap mainScreenSinai = new Bitmap(Properties.Resources.DLL_2_01);
            MainScreenSinai = mainScreenSinai;
        }
        #endregion
        #endregion    
        private static Bitmap CreateNonIndexedImage(Image src)  //Converting GIFs to non-indexed images (required for SetPixel method)
        {
            Bitmap newBmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics gfx = Graphics.FromImage(newBmp))
            {
                gfx.DrawImage(src, 0, 0);
            }

            return newBmp;
        }

        public static void CreateTerrainBitmaps()   //creates bitmaps for whole map (used before game starts)
        {
            for (int col = 0; col < Data.MapXdim; col++)
                for (int row = 0; row < Data.MapYdim; row++)
                    Game.Map[col, row].Graphic = TerrainBitmap(col, row);

        }

        #region Create bitmaps of tiles
        public static Bitmap TerrainBitmap(int col, int row)
        {
            Bitmap square = new Bitmap(64, 32); //define a bitmap for drawing in MapForm

            using (Graphics graphics = Graphics.FromImage(square))  //Draw tiles
            {
                Bitmap maptype;
                switch (Game.Map[col, row].Type)
                {
                    case TerrainType.Desert: maptype = Images.Desert[0]; break;
                    case TerrainType.Forest: maptype = Images.ForestBase[0]; break;
                    case TerrainType.Glacier: maptype = Images.Glacier[0]; break;
                    case TerrainType.Grassland: maptype = Images.Grassland[0]; break;
                    case TerrainType.Hills: maptype = Images.HillsBase[0]; break;
                    case TerrainType.Jungle: maptype = Images.Jungle[0]; break;
                    case TerrainType.Mountains: maptype = Images.MtnsBase[0]; break;
                    case TerrainType.Ocean: maptype = Images.Ocean[0]; break;
                    case TerrainType.Plains: maptype = Images.Plains[0]; break;
                    case TerrainType.Swamp: maptype = Images.Swamp[0]; break;
                    case TerrainType.Tundra: maptype = Images.Tundra[0]; break;
                    default: throw new ArgumentOutOfRangeException();
                }
                graphics.DrawImage(maptype, 0, 0);

                //Dither
                int col_ = 2 * col + row % 2;   //to civ2-style
                //First check if you are on map edge. If not, look at type of terrain in all 4 directions.
                TerrainType[,] tiletype = new TerrainType[2, 2];
                if ((col_ != 0) && (row != 0)) tiletype[0, 0] = Game.Map[((col_ - 1) - (row - 1) % 2) / 2, row - 1].Type;
                if ((col_ != 2 * Data.MapXdim - 1) && (row != 0)) tiletype[1, 0] = Game.Map[((col_ + 1) - (row - 1) % 2) / 2, row - 1].Type;
                if ((col_ != 0) && (row != Data.MapYdim - 1)) tiletype[0, 1] = Game.Map[((col_ - 1) - (row + 1) % 2) / 2, row + 1].Type;
                if ((col_ != 2 * Data.MapXdim - 1) && (row != Data.MapYdim - 1)) tiletype[1, 1] = Game.Map[((col_ + 1) - (row + 1) % 2) / 2, row + 1].Type;
                //implement dither on 4 locations in square
                for (int tileX = 0; tileX < 2; tileX++)    //for 4 directions
                    for (int tileY = 0; tileY < 2; tileY++)
                        switch (tiletype[tileX, tileY])
                        {
                            case TerrainType.Desert: graphics.DrawImage(Images.DitherDesert[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Plains: graphics.DrawImage(Images.DitherPlains[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Grassland: graphics.DrawImage(Images.DitherGrassland[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Forest: graphics.DrawImage(Images.DitherForest[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Hills: graphics.DrawImage(Images.DitherHills[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Mountains: graphics.DrawImage(Images.DitherMountains[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Tundra: graphics.DrawImage(Images.DitherTundra[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Glacier: graphics.DrawImage(Images.DitherGlacier[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Swamp: graphics.DrawImage(Images.DitherSwamp[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Jungle: graphics.DrawImage(Images.DitherJungle[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Ocean: graphics.DrawImage(Images.DitherGrassland[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            default: break;
                        }


                //Draw coast & river mouth
                if (Game.Map[col, row].Type == TerrainType.Ocean)
                {
                    int[] land = IsLandPresent(col, row);   //Determine if land is present in 8 directions

                    //draw coast & river mouth tiles
                    //NW+N+NE tiles
                    if (land[7] == 0 && land[0] == 0 && land[1] == 0) graphics.DrawImage(Images.Coast[0, 0], 16, 0);
                    if (land[7] == 1 && land[0] == 0 && land[1] == 0) graphics.DrawImage(Images.Coast[1, 0], 16, 0);
                    if (land[7] == 0 && land[0] == 1 && land[1] == 0) graphics.DrawImage(Images.Coast[2, 0], 16, 0);
                    if (land[7] == 1 && land[0] == 1 && land[1] == 0) graphics.DrawImage(Images.Coast[3, 0], 16, 0);
                    if (land[7] == 0 && land[0] == 0 && land[1] == 1) graphics.DrawImage(Images.Coast[4, 0], 16, 0);
                    if (land[7] == 1 && land[0] == 0 && land[1] == 1) graphics.DrawImage(Images.Coast[5, 0], 16, 0);
                    if (land[7] == 0 && land[0] == 1 && land[1] == 1) graphics.DrawImage(Images.Coast[6, 0], 16, 0);
                    if (land[7] == 1 && land[0] == 1 && land[1] == 1) graphics.DrawImage(Images.Coast[7, 0], 16, 0);
                    //SW+S+SE tiles
                    if (land[3] == 0 && land[4] == 0 && land[5] == 0) graphics.DrawImage(Images.Coast[0, 1], 16, 16);
                    if (land[3] == 1 && land[4] == 0 && land[5] == 0) graphics.DrawImage(Images.Coast[1, 1], 16, 16);
                    if (land[3] == 0 && land[4] == 1 && land[5] == 0) graphics.DrawImage(Images.Coast[2, 1], 16, 16);
                    if (land[3] == 1 && land[4] == 1 && land[5] == 0) graphics.DrawImage(Images.Coast[3, 1], 16, 16);
                    if (land[3] == 0 && land[4] == 0 && land[5] == 1) graphics.DrawImage(Images.Coast[4, 1], 16, 16);
                    if (land[3] == 1 && land[4] == 0 && land[5] == 1) graphics.DrawImage(Images.Coast[5, 1], 16, 16);
                    if (land[3] == 0 && land[4] == 1 && land[5] == 1) graphics.DrawImage(Images.Coast[6, 1], 16, 16);
                    if (land[3] == 1 && land[4] == 1 && land[5] == 1) graphics.DrawImage(Images.Coast[7, 1], 16, 16);
                    //SW+W+NW tiles
                    if (land[5] == 0 && land[6] == 0 && land[7] == 0) graphics.DrawImage(Images.Coast[0, 2], 0, 8);
                    if (land[5] == 1 && land[6] == 0 && land[7] == 0) graphics.DrawImage(Images.Coast[1, 2], 0, 8);
                    if (land[5] == 0 && land[6] == 1 && land[7] == 0) graphics.DrawImage(Images.Coast[2, 2], 0, 8);
                    if (land[5] == 1 && land[6] == 1 && land[7] == 0) graphics.DrawImage(Images.Coast[3, 2], 0, 8);
                    if (land[5] == 0 && land[6] == 0 && land[7] == 1) graphics.DrawImage(Images.Coast[4, 2], 0, 8);
                    if (land[5] == 1 && land[6] == 0 && land[7] == 1) graphics.DrawImage(Images.Coast[5, 2], 0, 8);
                    if (land[5] == 0 && land[6] == 1 && land[7] == 1) graphics.DrawImage(Images.Coast[6, 2], 0, 8);
                    if (land[5] == 1 && land[6] == 1 && land[7] == 1) graphics.DrawImage(Images.Coast[7, 2], 0, 8);
                    //NE+E+SE tiles
                    if (land[1] == 0 && land[2] == 0 && land[3] == 0) graphics.DrawImage(Images.Coast[0, 3], 32, 8);
                    if (land[1] == 1 && land[2] == 0 && land[3] == 0) graphics.DrawImage(Images.Coast[1, 3], 32, 8);
                    if (land[1] == 0 && land[2] == 1 && land[3] == 0) graphics.DrawImage(Images.Coast[2, 3], 32, 8);
                    if (land[1] == 1 && land[2] == 1 && land[3] == 0) graphics.DrawImage(Images.Coast[3, 3], 32, 8);
                    if (land[1] == 0 && land[2] == 0 && land[3] == 1) graphics.DrawImage(Images.Coast[4, 3], 32, 8);
                    if (land[1] == 1 && land[2] == 0 && land[3] == 1) graphics.DrawImage(Images.Coast[5, 3], 32, 8);
                    if (land[1] == 0 && land[2] == 1 && land[3] == 1) graphics.DrawImage(Images.Coast[6, 3], 32, 8);
                    if (land[1] == 1 && land[2] == 1 && land[3] == 1) graphics.DrawImage(Images.Coast[7, 3], 32, 8);

                    //River mouth
                    //if next to ocean is river, draw river mouth on this tile                            
                    col_ = 2 * col + row % 2; //rewrite indexes in Civ2-style
                    int Xdim = 2 * Data.MapXdim;   //X=50 in markted as X=100 in Civ2
                    int Ydim = Data.MapYdim;   //no need for such correction for Y
                    if (col_ + 1 < Xdim && row - 1 >= 0)    //NE there is no edge of map
                    {
                        if (land[1] == 1 && Game.Map[((col_ + 1) - (row - 1) % 2) / 2, row - 1].River) graphics.DrawImage(Images.RiverMouth[0], 0, 0);
                    }
                    if (col_ + 1 < Xdim && row + 1 < Ydim)    //SE there is no edge of map
                    {
                        if (land[3] == 1 && Game.Map[((col_ + 1) - (row + 1) % 2) / 2, row + 1].River) graphics.DrawImage(Images.RiverMouth[1], 0, 0);
                    }
                    if (col_ - 1 >= 0 && row + 1 < Ydim)    //SW there is no edge of map
                    {
                        if (land[5] == 1 && Game.Map[((col_ - 1) - (row + 1) % 2) / 2, row + 1].River) graphics.DrawImage(Images.RiverMouth[2], 0, 0);
                    }
                    if (col_ - 1 >= 0 && row - 1 >= 0)    //NW there is no edge of map
                    {
                        if (land[7] == 1 && Game.Map[((col_ - 1) - (row - 1) % 2) / 2, row - 1].River) graphics.DrawImage(Images.RiverMouth[3], 0, 0);
                    }
                }

                //Draw forests
                if (Game.Map[col, row].Type == TerrainType.Forest)
                {
                    int[] forestAround = IsForestAround(col, row);

                    //draw forest tiles
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 0, 0, 0 })) graphics.DrawImage(Images.Forest[0], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 0, 0, 0 })) graphics.DrawImage(Images.Forest[1], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 1, 0, 0 })) graphics.DrawImage(Images.Forest[2], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 1, 0, 0 })) graphics.DrawImage(Images.Forest[3], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 0, 1, 0 })) graphics.DrawImage(Images.Forest[4], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 0, 1, 0 })) graphics.DrawImage(Images.Forest[5], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 1, 1, 0 })) graphics.DrawImage(Images.Forest[6], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 1, 1, 0 })) graphics.DrawImage(Images.Forest[7], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 0, 0, 1 })) graphics.DrawImage(Images.Forest[8], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 0, 0, 1 })) graphics.DrawImage(Images.Forest[9], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 1, 0, 1 })) graphics.DrawImage(Images.Forest[10], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 1, 0, 1 })) graphics.DrawImage(Images.Forest[11], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 0, 1, 1 })) graphics.DrawImage(Images.Forest[12], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 0, 1, 1 })) graphics.DrawImage(Images.Forest[13], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 1, 1, 1 })) graphics.DrawImage(Images.Forest[14], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 1, 1, 1 })) graphics.DrawImage(Images.Forest[15], 0, 0);
                }

                //Draw mountains
                //CORRECT THIS: IF SHIELD IS AT MOUNTAIN IT SHOULD BE Mountains[2] and Mountains[3] !!!
                if (Game.Map[col, row].Type == TerrainType.Mountains)
                {
                    int[] mountAround = IsMountAround(col, row);

                    //draw forest tiles
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 0, 0, 0 })) graphics.DrawImage(Images.Mountains[0], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 0, 0, 0 })) graphics.DrawImage(Images.Mountains[1], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 1, 0, 0 })) graphics.DrawImage(Images.Mountains[2], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 1, 0, 0 })) graphics.DrawImage(Images.Mountains[3], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 0, 1, 0 })) graphics.DrawImage(Images.Mountains[4], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 0, 1, 0 })) graphics.DrawImage(Images.Mountains[5], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 1, 1, 0 })) graphics.DrawImage(Images.Mountains[6], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 1, 1, 0 })) graphics.DrawImage(Images.Mountains[7], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 0, 0, 1 })) graphics.DrawImage(Images.Mountains[8], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 0, 0, 1 })) graphics.DrawImage(Images.Mountains[9], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 1, 0, 1 })) graphics.DrawImage(Images.Mountains[10], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 1, 0, 1 })) graphics.DrawImage(Images.Mountains[11], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 0, 1, 1 })) graphics.DrawImage(Images.Mountains[12], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 0, 1, 1 })) graphics.DrawImage(Images.Mountains[13], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 1, 1, 1 })) graphics.DrawImage(Images.Mountains[14], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 1, 1, 1 })) graphics.DrawImage(Images.Mountains[15], 0, 0);
                }

                //Draw hills
                if (Game.Map[col, row].Type == TerrainType.Hills)
                {
                    int[] hillAround = IsHillAround(col, row);

                    //draw forest tiles
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 0, 0, 0 })) graphics.DrawImage(Images.Hills[0], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 0, 0, 0 })) graphics.DrawImage(Images.Hills[1], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 1, 0, 0 })) graphics.DrawImage(Images.Hills[2], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 1, 0, 0 })) graphics.DrawImage(Images.Hills[3], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 0, 1, 0 })) graphics.DrawImage(Images.Hills[4], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 0, 1, 0 })) graphics.DrawImage(Images.Hills[5], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 1, 1, 0 })) graphics.DrawImage(Images.Hills[6], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 1, 1, 0 })) graphics.DrawImage(Images.Hills[7], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 0, 0, 1 })) graphics.DrawImage(Images.Hills[8], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 0, 0, 1 })) graphics.DrawImage(Images.Hills[9], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 1, 0, 1 })) graphics.DrawImage(Images.Hills[10], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 1, 0, 1 })) graphics.DrawImage(Images.Hills[11], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 0, 1, 1 })) graphics.DrawImage(Images.Hills[12], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 0, 1, 1 })) graphics.DrawImage(Images.Hills[13], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 1, 1, 1 })) graphics.DrawImage(Images.Hills[14], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 1, 1, 1 })) graphics.DrawImage(Images.Hills[15], 0, 0);
                }

                //Draw rivers
                if (Game.Map[col, row].River)
                {
                    int[] riverAround = IsRiverAround(col, row);

                    //draw river tiles
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 0, 0, 0 })) graphics.DrawImage(Images.River[0], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 0, 0, 0 })) graphics.DrawImage(Images.River[1], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 1, 0, 0 })) graphics.DrawImage(Images.River[2], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 1, 0, 0 })) graphics.DrawImage(Images.River[3], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 0, 1, 0 })) graphics.DrawImage(Images.River[4], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 0, 1, 0 })) graphics.DrawImage(Images.River[5], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 1, 1, 0 })) graphics.DrawImage(Images.River[6], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 1, 1, 0 })) graphics.DrawImage(Images.River[7], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 0, 0, 1 })) graphics.DrawImage(Images.River[8], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 0, 0, 1 })) graphics.DrawImage(Images.River[9], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 1, 0, 1 })) graphics.DrawImage(Images.River[10], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 1, 0, 1 })) graphics.DrawImage(Images.River[11], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 0, 1, 1 })) graphics.DrawImage(Images.River[12], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 0, 1, 1 })) graphics.DrawImage(Images.River[13], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 1, 1, 1 })) graphics.DrawImage(Images.River[14], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 1, 1, 1 })) graphics.DrawImage(Images.River[15], 0, 0);
                }

                //Draw special resources if they exist
                if (Game.Map[col, row].SpecType != null)
                {
                    switch (Game.Map[col, row].SpecType)
                    {
                        case SpecialType.Oasis: maptype = Images.Desert[2]; break;
                        case SpecialType.DesertOil: maptype = Images.Desert[3]; break;
                        case SpecialType.Buffalo: maptype = Images.Plains[2]; break;
                        case SpecialType.Wheat: maptype = Images.Plains[3]; break;
                        case SpecialType.GrasslandShield: maptype = Images.Shield; break;
                        case SpecialType.Pheasant: maptype = Images.ForestBase[2]; break;
                        case SpecialType.Silk: maptype = Images.ForestBase[3]; break;
                        case SpecialType.Coal: maptype = Images.HillsBase[2]; break;
                        case SpecialType.Wine: maptype = Images.HillsBase[3]; break;
                        case SpecialType.Gold: maptype = Images.MtnsBase[2]; break;
                        case SpecialType.Iron: maptype = Images.MtnsBase[3]; break;
                        case SpecialType.Game: maptype = Images.Tundra[2]; break;
                        case SpecialType.Furs: maptype = Images.Tundra[3]; break;
                        case SpecialType.Ivory: maptype = Images.Glacier[2]; break;
                        case SpecialType.GlacierOil: maptype = Images.Glacier[3]; break;
                        case SpecialType.Peat: maptype = Images.Swamp[2]; break;
                        case SpecialType.Spice: maptype = Images.Swamp[3]; break;
                        case SpecialType.Gems: maptype = Images.Jungle[2]; break;
                        case SpecialType.Fruit: maptype = Images.Jungle[3]; break;
                        case SpecialType.Fish: maptype = Images.Ocean[2]; break;
                        case SpecialType.Whales: maptype = Images.Ocean[3]; break;
                        default: throw new ArgumentOutOfRangeException();
                    }
                    graphics.DrawImage(maptype, 0, 0);
                }

                //Roads (cites also act as road tiles)
                if (Game.Map[col, row].Road || Game.Map[col, row].CityPresent)
                {
                    int[] roadAround = IsRoadAround(col, row);

                    //draw roads
                    if (roadAround[0] == 1) graphics.DrawImage(Images.Road[8], 0, 0);  //to N
                    if (roadAround[1] == 1) graphics.DrawImage(Images.Road[1], 0, 0);  //to NE
                    if (roadAround[2] == 1) graphics.DrawImage(Images.Road[2], 0, 0);  //to E
                    if (roadAround[3] == 1) graphics.DrawImage(Images.Road[3], 0, 0);  //to SE
                    if (roadAround[4] == 1) graphics.DrawImage(Images.Road[4], 0, 0);  //to S
                    if (roadAround[5] == 1) graphics.DrawImage(Images.Road[5], 0, 0);  //to SW
                    if (roadAround[6] == 1) graphics.DrawImage(Images.Road[6], 0, 0);  //to W
                    if (roadAround[7] == 1) graphics.DrawImage(Images.Road[7], 0, 0);  //to NW
                    if (Enumerable.SequenceEqual(roadAround, new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 })) graphics.DrawImage(Images.Road[0], 0, 0);    //no road around
                }

                // !!!! NOT AS SIMPLE AS THIS. CORRECT THIS !!!!!
                //Railroads (cites also act as railroad tiles)
                //if (Game.Map[i, j].Railroad || Game.Map[i, j].CityPresent)
                //{
                //    int[] railroadAround = IsRailroadAround(i, j);

                //    //draw roads
                //    if (railroadAround[0] == 1) { graphics.DrawImage(Images.Railroad[8], 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }  //to N
                //    if (railroadAround[1] == 1) { graphics.DrawImage(Images.Railroad[1], 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }  //to NE
                //    if (railroadAround[2] == 1) { graphics.DrawImage(Images.Railroad[2], 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }  //to E
                //    if (railroadAround[3] == 1) { graphics.DrawImage(Images.Railroad[3], 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }  //to SE
                //    if (railroadAround[4] == 1) { graphics.DrawImage(Images.Railroad[4], 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }  //to S
                //    if (railroadAround[5] == 1) { graphics.DrawImage(Images.Railroad[5], 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }  //to SW
                //    if (railroadAround[6] == 1) { graphics.DrawImage(Images.Railroad[6], 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }  //to W
                //    if (railroadAround[7] == 1) { graphics.DrawImage(Images.Railroad[7], 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }  //to NW
                //    if (Enumerable.SequenceEqual(railroadAround, new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 })) { graphics.DrawImage(Images.Railroad[0], 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }    //no railroad around
                //}

                //Irrigation
                if (Game.Map[col, row].Irrigation) graphics.DrawImage(Images.Irrigation, 0, 0);

                //Farmland
                if (Game.Map[col, row].Farmland) graphics.DrawImage(Images.Farmland, 0, 0);

                //Mining
                if (Game.Map[col, row].Mining) graphics.DrawImage(Images.Mining, 0, 0);

                //Pollution
                if (Game.Map[col, row].Pollution) graphics.DrawImage(Images.Pollution, 0, 0);

                //Fortress
                if (Game.Map[col, row].Fortress) graphics.DrawImage(Images.Fortress, 0, 0);

                //Airbase
                if (Game.Map[col, row].Airbase) graphics.DrawImage(Images.Airbase, 0, 0);

            }
            return square;
        }

        private static int[] IsLandPresent(int i, int j)
        {
            int[] land = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 }; //in start we presume all surrounding tiles are water (land=1, water=0). Starting 0 is North, follows in clockwise direction.

            //rewrite indexes in Civ2-style
            int i_ = 2 * i + j % 2;
            int Xdim = 2 * Data.MapXdim;   //X=50 in markted as X=100 in Civ2
            int Ydim = Data.MapYdim;   //no need for such correction for Y

            //observe in all directions if land is present next to ocean
            //N:
            if (j - 2 < 0) land[0] = 1;   //if N tile is out of map (black tile), we presume it is land
            else if (Game.Map[(i_ - (j - 2) % 2) / 2, j - 2].Type != TerrainType.Ocean) land[0] = 1;
            //NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) land[1] = 1;  //NE is black tile
            else if (Game.Map[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type != TerrainType.Ocean) land[1] = 1;    //if it is not ocean, it is land
            //E:
            if (i_ + 2 >= Xdim) land[2] = 1;  //E is black tile
            else if (Game.Map[((i_ + 2) - j % 2) / 2, j].Type != TerrainType.Ocean) land[2] = 1;
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) land[3] = 1;  //SE is black tile
            else if (Game.Map[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type != TerrainType.Ocean) land[3] = 1;
            //S:
            if (j + 2 >= Ydim) land[4] = 1;   //S is black tile
            else if (Game.Map[(i_ - (j + 2) % 2) / 2, j + 2].Type != TerrainType.Ocean) land[4] = 1;
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) land[5] = 1;  //SW is black tile
            else if (Game.Map[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type != TerrainType.Ocean) land[5] = 1;
            //W:
            if (i_ - 2 < 0) land[6] = 1;  //W is black tile
            else if (Game.Map[((i_ - 2) - j % 2) / 2, j].Type != TerrainType.Ocean) land[6] = 1;
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) land[7] = 1;  //NW is black tile
            else if (Game.Map[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type != TerrainType.Ocean) land[7] = 1;

            return land;
        }

        private static int[] IsForestAround(int i, int j)
        {
            int[] forestAround = new int[4] { 0, 0, 0, 0 }; //in start we presume all surrounding tiles are not forest (forest=1, no forest=0). Starting 0 is NE, follows in clockwise direction.

            //rewrite indexes in Civ2-style
            int i_ = 2 * i + j % 2;
            int Xdim = 2 * Data.MapXdim;   //X=50 in markted as X=100 in Civ2
            int Ydim = Data.MapYdim;   //no need for such correction for Y

            //observe in all directions if forest is present
            //NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) forestAround[0] = 0;  //NE is black tile (we presume no forest is there)
            else if (Game.Map[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Forest) forestAround[0] = 1;
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) forestAround[1] = 0;  //SE is black tile
            else if (Game.Map[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Forest) forestAround[1] = 1;
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) forestAround[2] = 0;  //SW is black tile
            else if (Game.Map[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Forest) forestAround[2] = 1;
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) forestAround[3] = 0;  //NW is black tile
            else if (Game.Map[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Forest) forestAround[3] = 1;

            return forestAround;
        }

        private static int[] IsMountAround(int i, int j)
        {
            int[] mountAround = new int[4] { 0, 0, 0, 0 }; //in start we presume all surrounding tiles are not mountains (mount=1, no mount=0). Starting 0 is NE, follows in clockwise direction.

            //rewrite indexes in Civ2-style
            int i_ = 2 * i + j % 2;
            int Xdim = 2 * Data.MapXdim;   //X=50 in markted as X=100 in Civ2
            int Ydim = Data.MapYdim;   //no need for such correction for Y

            //observe in all directions if mountain is present
            //NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) mountAround[0] = 0;  //NE is black tile (we presume no mountain is there)
            else if (Game.Map[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Mountains) mountAround[0] = 1;
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) mountAround[1] = 0;  //SE is black tile
            else if (Game.Map[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Mountains) mountAround[1] = 1;
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) mountAround[2] = 0;  //SW is black tile
            else if (Game.Map[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Mountains) mountAround[2] = 1;
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) mountAround[3] = 0;  //NW is black tile
            else if (Game.Map[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Mountains) mountAround[3] = 1;

            return mountAround;
        }

        private static int[] IsHillAround(int i, int j)
        {
            int[] hillAround = new int[4] { 0, 0, 0, 0 }; //in start we presume all surrounding tiles are not hills (hill=1, no hill=0). Starting 0 is NE, follows in clockwise direction.

            //rewrite indexes in Civ2-style
            int i_ = 2 * i + j % 2;
            int Xdim = 2 * Data.MapXdim;   //X=50 in markted as X=100 in Civ2
            int Ydim = Data.MapYdim;   //no need for such correction for Y

            //observe in all directions if hill is present
            //NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) hillAround[0] = 0;  //NE is black tile (we presume no hill is there)
            else if (Game.Map[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Hills) hillAround[0] = 1;
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) hillAround[1] = 0;  //SE is black tile
            else if (Game.Map[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Hills) hillAround[1] = 1;
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) hillAround[2] = 0;  //SW is black tile
            else if (Game.Map[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Hills) hillAround[2] = 1;
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) hillAround[3] = 0;  //NW is black tile
            else if (Game.Map[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Hills) hillAround[3] = 1;

            return hillAround;
        }

        private static int[] IsRiverAround(int i, int j)
        {
            int[] riverAround = new int[4] { 0, 0, 0, 0 }; //in start we presume all surrounding tiles are not rivers (river=1, no river=0). Starting 0 is NE, follows in clockwise direction.

            //rewrite indexes in Civ2-style
            int i_ = 2 * i + j % 2;
            int Xdim = 2 * Data.MapXdim;   //X=50 in markted as X=100 in Civ2
            int Ydim = Data.MapYdim;   //no need for such correction for Y

            //observe in all directions if river is present
            //NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) riverAround[0] = 0;  //NE is black tile (we presume no river is there)
            else if (Game.Map[((i_ + 1) - (j - 1) % 2) / 2, j - 1].River || Game.Map[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Ocean) riverAround[0] = 1;
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) riverAround[1] = 0;  //SE is black tile
            else if (Game.Map[((i_ + 1) - (j + 1) % 2) / 2, j + 1].River || Game.Map[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Ocean) riverAround[1] = 1;
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) riverAround[2] = 0;  //SW is black tile
            else if (Game.Map[((i_ - 1) - (j + 1) % 2) / 2, j + 1].River || Game.Map[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Ocean) riverAround[2] = 1;
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) riverAround[3] = 0;  //NW is black tile
            else if (Game.Map[((i_ - 1) - (j - 1) % 2) / 2, j - 1].River || Game.Map[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Ocean) riverAround[3] = 1;

            return riverAround;
        }

        private static int[] IsRoadAround(int i, int j)
        {
            int[] roadAround = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 }; //in start we presume all surrounding tiles do not have roads. Starting 0 is NE, follows in clockwise direction.

            //rewrite indexes in Civ2-style
            int i_ = 2 * i + j % 2;
            int Xdim = 2 * Data.MapXdim;   //X=50 in markted as X=100 in Civ2
            int Ydim = Data.MapYdim;   //no need for such correction for Y

            //observe in all directions if road or city is present next to tile
            //N:
            if (j - 2 < 0) roadAround[0] = 0;   //if N tile is out of map (black tile), we presume there is no road
            else if (Game.Map[(i_ - (j - 2) % 2) / 2, j - 2].Road || Game.Map[(i_ - (j - 2) % 2) / 2, j - 2].CityPresent) roadAround[0] = 1;
            //NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) roadAround[1] = 0;  //NE is black tile
            else if (Game.Map[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Road || Game.Map[((i_ + 1) - (j - 1) % 2) / 2, j - 1].CityPresent) roadAround[1] = 1;
            //E:
            if (i_ + 2 >= Xdim) roadAround[2] = 0;  //E is black tile
            else if (Game.Map[((i_ + 2) - j % 2) / 2, j].Road || Game.Map[((i_ + 2) - j % 2) / 2, j].CityPresent) roadAround[2] = 1;
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) roadAround[3] = 0;  //SE is black tile
            else if (Game.Map[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Road || Game.Map[((i_ + 1) - (j + 1) % 2) / 2, j + 1].CityPresent) roadAround[3] = 1;
            //S:
            if (j + 2 >= Ydim) roadAround[4] = 0;   //S is black tile
            else if (Game.Map[(i_ - (j + 2) % 2) / 2, j + 2].Road || Game.Map[(i_ - (j + 2) % 2) / 2, j + 2].CityPresent) roadAround[4] = 1;
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) roadAround[5] = 0;  //SW is black tile
            else if (Game.Map[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Road || Game.Map[((i_ - 1) - (j + 1) % 2) / 2, j + 1].CityPresent) roadAround[5] = 1;
            //W:
            if (i_ - 2 < 0) roadAround[6] = 0;  //W is black tile
            else if (Game.Map[((i_ - 2) - j % 2) / 2, j].Road || Game.Map[((i_ - 2) - j % 2) / 2, j].CityPresent) roadAround[6] = 1;
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) roadAround[7] = 0;  //NW is black tile
            else if (Game.Map[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Road || Game.Map[((i_ - 1) - (j - 1) % 2) / 2, j - 1].CityPresent) roadAround[7] = 1;

            return roadAround;
        }

        private static int[] IsRailroadAround(int i, int j)
        {
            int[] railroadAround = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 }; //in start we presume all surrounding tiles do not have railroads. Starting 0 is NE, follows in clockwise direction.

            //rewrite indexes in Civ2-style
            int i_ = 2 * i + j % 2;
            int Xdim = 2 * Data.MapXdim;   //X=50 in markted as X=100 in Civ2
            int Ydim = Data.MapYdim;   //no need for such correction for Y

            //observe in all directions if road or city is present next to tile
            //N:
            if (j - 2 < 0) railroadAround[0] = 0;   //if N tile is out of map (black tile), we presume there is no road
            else if (Game.Map[(i_ - (j - 2) % 2) / 2, j - 2].Railroad || Game.Map[(i_ - (j - 2) % 2) / 2, j - 2].CityPresent) railroadAround[0] = 1;
            //NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) railroadAround[1] = 0;  //NE is black tile
            else if (Game.Map[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Railroad || Game.Map[((i_ + 1) - (j - 1) % 2) / 2, j - 1].CityPresent) railroadAround[1] = 1;
            //E:
            if (i_ + 2 >= Xdim) railroadAround[2] = 0;  //E is black tile
            else if (Game.Map[((i_ + 2) - j % 2) / 2, j].Railroad || Game.Map[((i_ + 2) - j % 2) / 2, j].CityPresent) railroadAround[2] = 1;
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) railroadAround[3] = 0;  //SE is black tile
            else if (Game.Map[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Railroad || Game.Map[((i_ + 1) - (j + 1) % 2) / 2, j + 1].CityPresent) railroadAround[3] = 1;
            //S:
            if (j + 2 >= Ydim) railroadAround[4] = 0;   //S is black tile
            else if (Game.Map[(i_ - (j + 2) % 2) / 2, j + 2].Railroad || Game.Map[(i_ - (j + 2) % 2) / 2, j + 2].CityPresent) railroadAround[4] = 1;
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) railroadAround[5] = 0;  //SW is black tile
            else if (Game.Map[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Railroad || Game.Map[((i_ - 1) - (j + 1) % 2) / 2, j + 1].CityPresent) railroadAround[5] = 1;
            //W:
            if (i_ - 2 < 0) railroadAround[6] = 0;  //W is black tile
            else if (Game.Map[((i_ - 2) - j % 2) / 2, j].Railroad || Game.Map[((i_ - 2) - j % 2) / 2, j].CityPresent) railroadAround[6] = 1;
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) railroadAround[7] = 0;  //NW is black tile
            else if (Game.Map[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Railroad || Game.Map[((i_ - 1) - (j - 1) % 2) / 2, j - 1].CityPresent) railroadAround[7] = 1;

            return railroadAround;
        }
        #endregion

        public static Bitmap CreateUnitBitmap(IUnit unit, bool drawInStack, int zoom)
        {
            Bitmap square = new Bitmap(64, 48);     //define a bitmap for drawing       

            using (Graphics graphics = Graphics.FromImage(square))
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

                string shieldText;
                switch (unit.Order)
                {
                    case OrderType.Fortify:         shieldText = "F"; break;
                    case OrderType.Fortified:       shieldText = "F"; break;
                    case OrderType.Sleep:           shieldText = "S"; break;
                    case OrderType.BuildFortress:   shieldText = "F"; break;
                    case OrderType.BuildRoad:       shieldText = "R"; break;
                    case OrderType.BuildIrrigation: shieldText = "I"; break;
                    case OrderType.BuildMine:       shieldText = "m"; break;
                    case OrderType.Transform:       shieldText = "O"; break;
                    case OrderType.CleanPollution:  shieldText = "p"; break;
                    case OrderType.BuildAirbase:    shieldText = "E"; break;
                    case OrderType.GoTo:            shieldText = "G"; break;
                    case OrderType.NoOrders:        shieldText = "-"; break;
                    default:                        shieldText = "-"; break;
                }

                //Draw unit shields. First determine if the shield is on the left or right side
                int firstShieldXLoc = unitShieldLocation[(int)unit.Type, 0];
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
                int hitpointsBarX = (int)Math.Floor((float)unit.HitPoints * 12 / unit.MaxHitPoints);
                Color hitpointsColor;
                if (hitpointsBarX <= 3) 
                    hitpointsColor = Color.FromArgb(243, 0, 0);
                else if (hitpointsBarX >= 4 && hitpointsBarX <= 8) 
                    hitpointsColor = Color.FromArgb(255, 223, 79);
                else 
                    hitpointsColor = Color.FromArgb(87, 171, 39);

                //Draw shadow for unit in stack
                if (drawInStack)    //draw dark shield if unit is stacked on top of others
                {
                    graphics.DrawImage(UnitShieldShadow, secondShieldBorderXLoc, unitShieldLocation[(int)unit.Type, 1]); //shield shadow
                    graphics.DrawImage(NoBorderUnitShield[unit.Civ], secondShieldXLoc, unitShieldLocation[(int)unit.Type, 1]);   //dark shield
                }

                //shield shadow
                graphics.DrawImage(UnitShieldShadow, unitShieldLocation[(int)unit.Type, 0] + borderShieldOffset, unitShieldLocation[(int)unit.Type, 1] - borderShieldOffset);

                //main shield
                graphics.DrawImage(UnitShield[unit.Civ], unitShieldLocation[(int)unit.Type, 0], unitShieldLocation[(int)unit.Type, 1]);

                //Draw black background for hitpoints bar
                graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(unitShieldLocation[(int)unit.Type, 0], unitShieldLocation[(int)unit.Type, 1] + 2, 12, 3));

                //Draw hitpoints bar
                graphics.FillRectangle(new SolidBrush(hitpointsColor), new Rectangle(unitShieldLocation[(int)unit.Type, 0], unitShieldLocation[(int)unit.Type, 1] + 2, hitpointsBarX, 3));

                //Action on shield
                graphics.DrawString(shieldText, new Font("Arial", 6.5f), new SolidBrush(Color.Black), unitShieldLocation[(int)unit.Type, 0] + 7, unitShieldLocation[(int)unit.Type, 1] + 12, sf);

                if (unit.Order != OrderType.Sleep) graphics.DrawImage(Units[(int)unit.Type], 0, 0);    //draw unit
                else graphics.DrawImage(Units[(int)unit.Type], new Rectangle(0, 0, 64, 48), 0, 0, 64, 48, GraphicsUnit.Pixel, ModifyImage.ConvertToGray());    //draw sentry unit

                //draw fortification
                if (unit.Order == OrderType.Fortified) graphics.DrawImage(Fortified, 0, 0);

                sf.Dispose();
            }

            //Resize image if required
            square = ModifyImage.ResizeImage(square, 8 * zoom, 6 * zoom);

            return square;
        }

        public static Bitmap CreateCityBitmap(City city, bool citySizeWindow, int zoom)
        {
            Bitmap graphic = new Bitmap(64, 48);    //define a bitmap for drawing map

            //Determine city bitmap
            //For everything not modern or industrial => 4 city size styles (0=sizes 1...3, 1=sizes 4...5, 2=sizes 6...7, 3=sizes >= 8)
            //If city is capital => 3 size styles (1=sizes 1...3, 2=sizes 4...5, 3=sizes >= 6)
            int cityStyle = Game.Civs[city.Owner].CityStyle;
            int sizeStyle = 0;
            if (cityStyle < 4) {
                if (Array.Exists(city.Improvements, element => element.Type == ImprovementType.Palace)) { //palace exists
                    if (city.Size <= 3) sizeStyle = 1;
                    else if (city.Size > 3 && city.Size <= 5) sizeStyle = 2;
                    else sizeStyle = 3; }
                else {
                    if (city.Size <= 3) sizeStyle = 0;
                    else if (city.Size > 3 && city.Size <= 5) sizeStyle = 1;
                    else if (city.Size > 5 && city.Size <= 7) sizeStyle = 2;
                    else sizeStyle = 3; } }
            //If city is industrial => 4 city size styles (0=sizes 1...4, 1=sizes 5...7, 2=sizes 8...10, 3=sizes >= 11)
            //If city is capital => 3 size styles (1=sizes 1...4, 2=sizes 5...7, 3=sizes >= 8)
            else if (cityStyle == 4) {
                if (Array.Exists(city.Improvements, element => element.Type == ImprovementType.Palace)) { //palace exists
                    if (city.Size <= 4) sizeStyle = 1;
                    else if (city.Size > 4 && city.Size <= 7) sizeStyle = 2;
                    else sizeStyle = 3; }
                else {
                    if (city.Size <= 4) sizeStyle = 0;
                    else if (city.Size > 4 && city.Size <= 7) sizeStyle = 1;
                    else if (city.Size > 7 && city.Size <= 10) sizeStyle = 2;
                    else sizeStyle = 3; } }
            //If city is modern => 4 city size styles (0=sizes 1...4, 1=sizes 5...10, 2=sizes 11...18, 3=sizes >= 19)
            //If city is capital => 3 size styles (1=sizes 1...4, 2=sizes 5...10, 3=sizes >= 11)
            else {
                if (Array.Exists(city.Improvements, element => element.Type == ImprovementType.Palace)) { //palace exists
                    if (city.Size <= 4) sizeStyle = 1;
                    else if (city.Size > 4 && city.Size <= 10) sizeStyle = 2;
                    else sizeStyle = 3; }
                else {
                    if (city.Size <= 4) sizeStyle = 0;
                    else if (city.Size > 4 && city.Size <= 10) sizeStyle = 1;
                    else if (city.Size > 10 && city.Size <= 18) sizeStyle = 2;
                    else sizeStyle = 3; } }

            //If no units are in the city, draw no flag
            bool flagPresent = false;
            if (Game.Units.Any(unit => unit.X == city.X && unit.Y == city.Y)) flagPresent = true;

            using (Graphics graphics = Graphics.FromImage(graphic))
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

                if (!Array.Exists(city.Improvements, element => element.Type == ImprovementType.CityWalls))  //no city walls
                {
                    graphics.DrawImage(City[cityStyle, sizeStyle], 0, 0);
                    if (citySizeWindow) //Draw city size window
                    {
                        graphics.DrawRectangle(new Pen(Color.Black), citySizeWindowLoc[cityStyle, sizeStyle, 0] - 1, citySizeWindowLoc[cityStyle, sizeStyle, 1] - 1, 9, 13);  //rectangle
                        graphics.FillRectangle(new SolidBrush(CivColors.Light[city.Owner]), citySizeWindowLoc[cityStyle, sizeStyle, 0], citySizeWindowLoc[cityStyle, sizeStyle, 1], 8, 12); //filling of rectangle
                        graphics.DrawString(city.Size.ToString(), new Font("Times New Roman", 10.0f, FontStyle.Bold), new SolidBrush(Color.Black), citySizeWindowLoc[cityStyle, sizeStyle, 0] + 4, citySizeWindowLoc[cityStyle, sizeStyle, 1] + 6, sf);    //Size text
                    }
                    if (flagPresent) graphics.DrawImage(CityFlag[city.Owner], cityFlagLoc[cityStyle, sizeStyle, 0] - 3, cityFlagLoc[cityStyle, sizeStyle, 1] - 17); //draw city flag
                    
                }
                else
                {
                    graphics.DrawImage(CityWall[cityStyle, sizeStyle], 0, 0);
                    if (citySizeWindow)
                    {
                        graphics.DrawRectangle(new Pen(Color.Black), cityWallSizeWindowLoc[cityStyle, sizeStyle, 0] - 1, cityWallSizeWindowLoc[cityStyle, sizeStyle, 1] - 1, 9, 13); //Draw city (+Wall) size window
                        graphics.FillRectangle(new SolidBrush(CivColors.Light[city.Owner]), cityWallSizeWindowLoc[cityStyle, sizeStyle, 0], cityWallSizeWindowLoc[cityStyle, sizeStyle, 1], 8, 12); //filling of rectangle
                        graphics.DrawString(city.Size.ToString(), new Font("Times New Roman", 10.0f, FontStyle.Bold), new SolidBrush(Color.Black), cityWallSizeWindowLoc[cityStyle, sizeStyle, 0] + 4, cityWallSizeWindowLoc[cityStyle, sizeStyle, 1] + 6, sf);    //Size text    
                    }
                    if (flagPresent) graphics.DrawImage(CityFlag[city.Owner], cityWallFlagLoc[cityStyle, sizeStyle, 0] - 3, cityWallFlagLoc[cityStyle, sizeStyle, 1] - 17);    //Draw city flag                    
                }
                sf.Dispose();
            }

            graphic = ModifyImage.ResizeImage(graphic, 8 * zoom, 6 * zoom);

            return graphic;
        }

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
                    case int n when (n <= 9):               wheat_spacing = 26; break;  //original=17 (50% scaled)
                    case int n when (n == 10):              wheat_spacing = 24; break;  //original=16 (50% scaled)
                    case int n when (n == 11):              wheat_spacing = 20; break;  //original=13 (50% scaled)
                    case int n when (n == 12):              wheat_spacing = 18; break;  //original=12 (50% scaled)
                    case int n when (n == 13):              wheat_spacing = 17; break;  //original=11 (50% scaled)
                    case int n when (n == 14):              wheat_spacing = 15; break;  //original=10 (50% scaled)
                    case int n when (n == 15 || n == 16):   wheat_spacing = 14; break;  //original=9 (50% scaled)
                    case int n when (n == 17):              wheat_spacing = 12; break;  //original=8 (50% scaled)
                    case int n when (n >= 18 && n <= 20):   wheat_spacing = 11; break;  //original=7 (50% scaled)
                    case int n when (n == 21 || n == 22):   wheat_spacing = 9; break;  //original=6 (50% scaled)
                    case int n when (n >= 23 && n <= 26):   wheat_spacing = 8; break;  //original=5 (50% scaled)
                    case int n when (n >= 27 && n <= 33):   wheat_spacing = 6; break;  //original=4 (50% scaled)
                    case int n when (n >= 34 && n <= 40):   wheat_spacing = 5; break;  //original=3 (50% scaled)
                    case int n when (n >= 41 && n <= 80):   wheat_spacing = 3; break;  //original=2 (50% scaled)
                    case int n when (n >= 81):              wheat_spacing = 2; break;  //original=1 (50% scaled)
                    default:                                wheat_spacing = 26; break;
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

        public static Bitmap DrawFaces(City city, double scale_factor) //Draw faces in cityform
        {
            Bitmap faces = new Bitmap(630, 50);
            using (Graphics graphics = Graphics.FromImage(faces))
            {
                int spacing;
                switch (city.Size)
                {
                    case int n when (n <= 15): spacing = 42; break;    //50 % larger (orignal = 28)
                    case int n when (n == 16): spacing = 39; break;    //50 % larger (orignal = 26)
                    case int n when (n == 17): spacing = 36; break;    //50 % larger (orignal = 24)
                    case int n when (n == 18): spacing = 35; break;    //50 % larger (orignal = 23)
                    case int n when (n == 19): spacing = 32; break;    //50 % larger (orignal = 21)
                    case int n when (n == 20): spacing = 30; break;    //50 % larger (orignal = 20)
                    case int n when (n == 21): spacing = 29; break;   //50 % larger (orignal = 19)
                    case int n when (n == 22): spacing = 27; break;   //50 % larger (orignal = 18)
                    case int n when (n == 23 || n == 24): spacing = 26; break;   //50 % larger (orignal = 17)
                    case int n when (n == 25): spacing = 24; break;   //50 % larger (orignal = 16)
                    case int n when (n == 26 || n == 27): spacing = 23; break;   //50 % larger (orignal = 15)
                    case int n when (n == 28 || n == 29): spacing = 21; break;   //50 % larger (orignal = 14)
                    case int n when (n == 30 || n == 31): spacing = 20; break;   //50 % larger (orignal = 13)
                    case int n when (n == 32 || n == 33): spacing = 18; break;   //50 % larger (orignal = 12)
                    case int n when (n >= 34 && n <= 36): spacing = 17; break;   //50 % larger (orignal = 11)
                    case int n when (n >= 37 && n <= 41): spacing = 15; break;   //50 % larger (orignal = 10)
                    case int n when (n == 42 || n == 43): spacing = 14; break;   //50 % larger (orignal = 9)
                    case int n when (n >= 44 && n <= 50): spacing = 12; break;   //50 % larger (orignal = 8)
                    case int n when (n >= 51 && n <= 57): spacing = 11; break;   //50 % larger (orignal = 7)
                    case int n when (n >= 58 && n <= 66): spacing = 9; break;   //50 % larger (orignal = 6)
                    case int n when (n >= 67 && n <= 79): spacing = 8; break;   //50 % larger (orignal = 5)
                    case int n when (n >= 80 && n <= 99): spacing = 6; break;   //50 % larger (orignal = 4)
                    case int n when (n >= 100): spacing = 5; break;   //50 % larger (orignal = 3)
                    default: spacing = 30; break;
                }
                //Draw icons
                for (int i = 0; i < city.Size; i++)
                {
                    graphics.DrawImage(ModifyImage.ResizeImage(Images.PeopleLshadow[2 + i % 2, 0], (int)(27 * scale_factor), (int)(30 * scale_factor)), i * spacing + 1, 1);  //shadow
                    graphics.DrawImage(ModifyImage.ResizeImage(Images.PeopleL[2 + i % 2, 0], (int)(27 * scale_factor), (int)(30 * scale_factor)), i * spacing, 0);  //man-woman exchange turns
                }
            }

            return faces;
        }

        public static Bitmap DrawCityProduction(City city)
        {
            Bitmap icons = new Bitmap(293, 287);    //same size as production panel in city form
            using (Graphics graphics = Graphics.FromImage(icons))
            {
                //Draw rectangle around icons
                int IIP = city.ItemInProduction;
                int cost;
                if (IIP < 62) cost = ReadFiles.UnitCost[IIP];   //Item is unit
                else cost = ReadFiles.ImprovementCost[IIP - 62 + 1];    //Item is improvement (first 62 are units, +1 because first improfement is "Nothing")
                int vertSpacing = Math.Min(10, cost);    //max 10 lines
                graphics.DrawLine(new Pen(Color.FromArgb(83, 103, 191)), 9, 65, 9 + 271, 65);   //1st horizontal
                graphics.DrawLine(new Pen(Color.FromArgb(83, 103, 191)), 9, 65, 9, 65 + 27 + (vertSpacing - 1) * 21);   //1st vertical
                graphics.DrawLine(new Pen(Color.FromArgb(0, 0, 95)), 9, 65 + 27 + (vertSpacing - 1) * 21, 9 + 271, 65 + 27 + (vertSpacing - 1) * 21);   //2nd horizontal
                graphics.DrawLine(new Pen(Color.FromArgb(0, 0, 95)), 9 + 271, 65, 9 + 271, 65 + 27 + (vertSpacing - 1) * 21);   //2nd vertical

                //Draw icons
                int count = 0;
                for (int row = 0; row < Math.Min(cost, 10); row++)   //there are never more than 10 rows
                {
                    for (int col = 0; col < Math.Max(cost, 10); col++)  //there are never less than 10 columns
                    {
                        int dx = Convert.ToInt32(2 + col * (272 - 21 - 4) / ((float)Math.Max(cost, 10) - 1)); //horizontal separation between icons
                        int dy = 21;    //vertical separation of icons (space between icons in y-directions is always 0)
                        graphics.DrawImage(Images.CitymapSupportLargeBigger, 10 + dx, 65 + 3 + dy * row);

                        count++;
                        if (count >= city.ShieldsProgress) break;
                    }
                    if (count >= city.ShieldsProgress) break;
                }
            }

            return icons;
        }

        public static Bitmap DrawCityFormMap(City city)    //Draw terrain in city form
        {
            Bitmap map = new Bitmap(4 * 64, 4 * 32);

            Bitmap image;
            using (Graphics graphics = Graphics.FromImage(map))
            {
                //First draw squares around city
                for (int x_ = -3; x_ <= 3; x_++)
                    for (int y_ = -3; y_ <= 3; y_++)
                        if ((x_ == -1 & y_ == -3) || (x_ == 1 & y_ == -3) || (x_ == -2 & y_ == -2) || (x_ == 0 & y_ == -2) || (x_ == 2 & y_ == -2) || (x_ == -3 & y_ == -1) || (x_ == -1 & y_ == -1) || (x_ == 1 & y_ == -1) || (x_ == 3 & y_ == -1) || (x_ == -2 & y_ == 0) || (x_ == 0 & y_ == 0) || (x_ == 2 & y_ == 0) || (x_ == -3 & y_ == 1) || (x_ == -1 & y_ == 1) || (x_ == 1 & y_ == 1) || (x_ == 3 & y_ == 1) || (x_ == -2 & y_ == 2) || (x_ == 0 & y_ == 2) || (x_ == 2 & y_ == 2) || (x_ == -1 & y_ == 3) || (x_ == 1 & y_ == 3))
                        {
                            int newX = city.X + x_;
                            int newY = city.Y + y_;
                            if (newX >= 0 && newX < 2 * Data.MapXdim && newY >= 0 && newY < Data.MapYdim) image = TerrainBitmap((newX - (newY % 2)) / 2, newY);
                            else image = Images.Blank;
                            graphics.DrawImage(image, 32 * (x_ + 3), 16 * (y_ + 3));
                        }

                //Then draw city
                graphics.DrawImage(CreateCityBitmap(city, false, 8), 64 * 1 + 32 * (3 % 2) + 1, 16 * 2 + 1);
            }

            return map;
        }

        public static Bitmap DrawCityFormMapIcons(City city, int offsetX, int offsetY)
        {
            offsetX = (offsetX - (offsetY % 2)) / 2;    //First turn offsetX/Y from Civ2 to real coordinates

            Bitmap icons = new Bitmap(64, 32);    //define a bitmap for drawing icons
            using (Graphics graphics = Graphics.FromImage(icons))
            {
                //First count all icons on this square to determine the spacing between icons (10 = no spacing, 15 = no spacing @ 50% scaled)
                int spacing;
                int countF = Game.Map[city.X + offsetX, city.Y + offsetY].Food;
                int countS = Game.Map[city.X + offsetX, city.Y + offsetY].Shields;
                int countT = Game.Map[city.X + offsetX, city.Y + offsetY].Trade;
                switch (countF + countS + countT)
                {
                    case 1:
                    case 2: spacing = 17; break;    //50 % larger (orignal = 11, 1 pixel gap)
                    case 3: spacing = 15; break;    //50 % larger (orignal = 10, no gap)
                    case 4: spacing = 11; break;    //50 % larger (orignal = 7)
                    case 5: spacing = 8; break;    //50 % larger (orignal = 5)
                    case 6: spacing = 6; break;    //50 % larger (orignal = 4)
                    case 7:
                    case 8: spacing = 5; break;    //50 % larger (orignal = 3)
                    case 9: spacing = 3; break;    //50 % larger (orignal = 2)
                    case 10: spacing = 2; break;    //50 % larger (orignal = 1)
                    default: spacing = 2; break;    //50 % larger (orignal = 1)
                }
                //First draw food, then shields, then trade icons
                for (int i = 0; i < countF; i++) graphics.DrawImage(CitymapFoodSmallBigger, i * spacing, 0);
                for (int i = 0; i < countS; i++) graphics.DrawImage(CitymapShieldSmallBigger, (countF + i) * spacing, 0);
                for (int i = 0; i < countT; i++) graphics.DrawImage(CitymapTradeSmallBigger, (countF + countS + i) * spacing, 0);
            }
            return icons;
        }
                
        public static Bitmap DrawCityIcons(City city, int foodIcons, int surplusIcons, int tradeIcons, int corruptionIcons, int taxIcons, int luxIcons, int sciIcons, 
            int supportIcons, int productionIcons)  //Draw icons in city resources (surplus < 0 is hunger)
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
                    case int n when (n >= 1 && n <= 15): spacing = 23; break;    //50 % larger (orignal = 15, 1 pixel gap)
                    case int n when (n == 16 || n == 17): spacing = 20; break;   //50 % larger (orignal = 13, 1 pixel overlap)
                    case int n when (n == 18 || n == 19): spacing = 17; break;   //50 % larger (orignal = 11, 3 pixel overlap)
                    case int n when (n == 20 || n == 21): spacing = 15; break;   //50 % larger (orignal = 10, 4 pixel overlap)
                    case int n when (n == 22 || n == 23): spacing = 14; break;   //50 % larger (orignal = 9, 5 pixel overlap)
                    case int n when (n == 24 || n == 25): spacing = 12; break;   //50 % larger (orignal = 8, 6 pixel overlap)
                    case int n when (n >= 26 && n <= 29): spacing = 11; break;   //50 % larger (orignal = 7, 7 pixel overlap)
                    case int n when (n >= 30 && n <= 33): spacing = 9; break;    //50 % larger (orignal = 6, 8 pixel overlap)
                    case int n when (n >= 34 && n <= 37): spacing = 8; break;    //50 % larger (orignal = 5, 9 pixel overlap)
                    case int n when (n >= 38 && n <= 49): spacing = 6; break;    //50 % larger (orignal = 4, 10 pixel overlap)
                    case int n when (n >= 50 && n <= 65): spacing = 5; break;    //50 % larger (orignal = 3, 11 pixel overlap)
                    case int n when (n >= 66): spacing = 3; break;               //50 % larger (orignal = 2, 12 pixel overlap)
                    default: spacing = 2; break;
                }
                //First draw background rectangle
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(71, 147, 31)), 0, 0, spacing * foodIcons + 21 - spacing + 6, 23); //background square for food
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(55, 123, 23)), x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing + 3), 0, spacing * Math.Abs(surplusIcons) + 21 - spacing + 6, 23); //background square for surplus/hunger
                //Draw food & surplus icons
                for (int i = 0; i < foodIcons; i++) graphics.DrawImage(Images.CitymapFoodLargeBigger, i * spacing + 3, 1);
                for (int i = 0; i < Math.Abs(surplusIcons); i++)
                {
                    if (surplusIcons < 0) graphics.DrawImage(Images.CitymapHungerLargeBigger, x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing) + i * spacing, 1); //hunger
                    else graphics.DrawImage(Images.CitymapFoodLargeBigger, x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing) + i * spacing, 1); //hunger
                }

                //Next draw trade + corruption icons
                switch (tradeIcons + Math.Abs(corruptionIcons))
                {
                    case int n when (n >= 1 && n <= 15): spacing = 23; break;    //50 % larger (orignal = 15, 1 pixel gap)
                    case int n when (n == 16 || n == 17): spacing = 20; break;   //50 % larger (orignal = 13, 1 pixel overlap)
                    case int n when (n == 18 || n == 19): spacing = 17; break;   //50 % larger (orignal = 11, 3 pixel overlap)
                    case int n when (n == 20 || n == 21): spacing = 15; break;   //50 % larger (orignal = 10, 4 pixel overlap)
                    case int n when (n == 22 || n == 23): spacing = 14; break;   //50 % larger (orignal = 9, 5 pixel overlap)
                    case int n when (n == 24 || n == 25): spacing = 12; break;   //50 % larger (orignal = 8, 6 pixel overlap)
                    case int n when (n >= 26 && n <= 29): spacing = 11; break;   //50 % larger (orignal = 7, 7 pixel overlap)
                    case int n when (n >= 30 && n <= 33): spacing = 9; break;    //50 % larger (orignal = 6, 8 pixel overlap)
                    case int n when (n >= 34 && n <= 37): spacing = 8; break;    //50 % larger (orignal = 5, 9 pixel overlap)
                    case int n when (n >= 38 && n <= 49): spacing = 6; break;    //50 % larger (orignal = 4, 10 pixel overlap)
                    case int n when (n >= 50 && n <= 65): spacing = 5; break;    //50 % larger (orignal = 3, 11 pixel overlap)
                    case int n when (n >= 66): spacing = 3; break;               //50 % larger (orignal = 2, 12 pixel overlap)
                    default: spacing = 2; break;
                }
                //First draw background rectangle
                //graphics.FillRectangle(new SolidBrush(Color.FromArgb(71, 147, 31)), 0, 0, spacing * foodIcons + 21 - spacing + 6, 23); //background square for food
                //graphics.FillRectangle(new SolidBrush(Color.FromArgb(55, 123, 23)), x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing + 3), 0, spacing * Math.Abs(surplusIcons) + 21 - spacing + 6, 23); //background square for surplus/hunger
                //Draw trade & corruption icons
                for (int i = 0; i < tradeIcons; i++) graphics.DrawImage(Images.CitymapTradeLargeBigger, i * spacing + 3, 63);

                for (int i = 0; i < Math.Abs(corruptionIcons); i++) graphics.DrawImage(Images.CitymapCorruptionLargeBigger, x_size - (spacing * Math.Abs(corruptionIcons) + 21 - spacing) + i * spacing, 63); //hunger

                //Next draw tax+lux+sci icons
                switch (taxIcons + luxIcons + sciIcons)
                {
                    case int n when (n >= 1 && n <= 15): spacing = 23; break;    //50 % larger (orignal = 15, 1 pixel gap)
                    case int n when (n == 16 || n == 17): spacing = 20; break;   //50 % larger (orignal = 13, 1 pixel overlap)
                    case int n when (n == 18 || n == 19): spacing = 17; break;   //50 % larger (orignal = 11, 3 pixel overlap)
                    case int n when (n == 20 || n == 21): spacing = 15; break;   //50 % larger (orignal = 10, 4 pixel overlap)
                    case int n when (n == 22 || n == 23): spacing = 14; break;   //50 % larger (orignal = 9, 5 pixel overlap)
                    case int n when (n == 24 || n == 25): spacing = 12; break;   //50 % larger (orignal = 8, 6 pixel overlap)
                    case int n when (n >= 26 && n <= 29): spacing = 11; break;   //50 % larger (orignal = 7, 7 pixel overlap)
                    case int n when (n >= 30 && n <= 33): spacing = 9; break;    //50 % larger (orignal = 6, 8 pixel overlap)
                    case int n when (n >= 34 && n <= 37): spacing = 8; break;    //50 % larger (orignal = 5, 9 pixel overlap)
                    case int n when (n >= 38 && n <= 49): spacing = 6; break;    //50 % larger (orignal = 4, 10 pixel overlap)
                    case int n when (n >= 50 && n <= 65): spacing = 5; break;    //50 % larger (orignal = 3, 11 pixel overlap)
                    case int n when (n >= 66): spacing = 3; break;               //50 % larger (orignal = 2, 12 pixel overlap)
                    default: spacing = 2; break;
                }
                //First draw background rectangle
                //graphics.FillRectangle(new SolidBrush(Color.FromArgb(71, 147, 31)), 0, 0, spacing * foodIcons + 21 - spacing + 6, 23); //background square for food
                //graphics.FillRectangle(new SolidBrush(Color.FromArgb(55, 123, 23)), x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing + 3), 0, spacing * Math.Abs(surplusIcons) + 21 - spacing + 6, 23); //background square for surplus/hunger
                //Draw trade & corruption icons
                for (int i = 0; i < taxIcons; i++) graphics.DrawImage(Images.CitymapTaxLargeBigger, i * spacing + 3, 99);  //tax

                for (int i = 0; i < luxIcons; i++)
                {
                    //TO-DO !!!
                    //graphics.DrawImage(Images.CitymapLuxLargeBigger, i * spacing + 3, 99);  //lux
                }
                for (int i = 0; i < sciIcons; i++) graphics.DrawImage(Images.CitymapSciLargeBigger, x_size - (spacing * sciIcons + 21 - spacing) + i * spacing, 99); //sci

                //Next draw support+production icons
                switch (supportIcons + productionIcons)
                {
                    case int n when (n >= 1 && n <= 15): spacing = 23; break;    //50 % larger (orignal = 15, 1 pixel gap)
                    case int n when (n == 16 || n == 17): spacing = 20; break;   //50 % larger (orignal = 13, 1 pixel overlap)
                    case int n when (n == 18 || n == 19): spacing = 17; break;   //50 % larger (orignal = 11, 3 pixel overlap)
                    case int n when (n == 20 || n == 21): spacing = 15; break;   //50 % larger (orignal = 10, 4 pixel overlap)
                    case int n when (n == 22 || n == 23): spacing = 14; break;   //50 % larger (orignal = 9, 5 pixel overlap)
                    case int n when (n == 24 || n == 25): spacing = 12; break;   //50 % larger (orignal = 8, 6 pixel overlap)
                    case int n when (n >= 26 && n <= 29): spacing = 11; break;   //50 % larger (orignal = 7, 7 pixel overlap)
                    case int n when (n >= 30 && n <= 33): spacing = 9; break;    //50 % larger (orignal = 6, 8 pixel overlap)
                    case int n when (n >= 34 && n <= 37): spacing = 8; break;    //50 % larger (orignal = 5, 9 pixel overlap)
                    case int n when (n >= 38 && n <= 49): spacing = 6; break;    //50 % larger (orignal = 4, 10 pixel overlap)
                    case int n when (n >= 50 && n <= 65): spacing = 5; break;    //50 % larger (orignal = 3, 11 pixel overlap)
                    case int n when (n >= 66): spacing = 3; break;               //50 % larger (orignal = 2, 12 pixel overlap)
                    default: spacing = 2; break;
                }
                //First draw background rectangle
                //graphics.FillRectangle(new SolidBrush(Color.FromArgb(71, 147, 31)), 0, 0, spacing * foodIcons + 21 - spacing + 6, 23); //background square for food
                //graphics.FillRectangle(new SolidBrush(Color.FromArgb(55, 123, 23)), x_size - (spacing * Math.Abs(surplusIcons) + 21 - spacing + 3), 0, spacing * Math.Abs(surplusIcons) + 21 - spacing + 6, 23); //background square for surplus/hunger
                //Draw trade & corruption icons
                for (int i = 0; i < supportIcons; i++) graphics.DrawImage(Images.CitymapSupportLargeBigger, i * spacing + 3, 161);  //support

                for (int i = 0; i < productionIcons; i++) graphics.DrawImage(Images.CitymapSupportLargeBigger, x_size - (spacing * productionIcons + 21 - spacing) + i * spacing, 161); //production

            }
            return icons;
        }
    }
}
