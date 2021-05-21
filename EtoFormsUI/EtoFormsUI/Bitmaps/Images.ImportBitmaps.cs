using Eto.Drawing;
using System.IO;
using System.Diagnostics;
using Civ2engine;
using EtoFormsUIExtensionMethods;

namespace EtoFormsUI
{
    public static partial class Images
    {
        public static Bitmap CityHungerBig, CityShortageBig, CityCorruptBig, CityFoodBig, CitySupportBig, CityTradeBig, CityLuxBig, CityTaxBig, CitySciBig, CityFoodSmall, CitySupportSmall, CityTradeSmall, NextCity, CityWallpaper, PanelOuterWallpaper, PanelInnerWallpaper, Irrigation, Farmland, Mining, Pollution, Fortified, Fortress, Airbase, AirbasePlane, GrasslandShield, ViewPiece, GridLines, GridLinesVisible, Dither, Blank, DitherBase, SellIcon, NextCityLarge, PrevCity, PrevCityLarge, CityExit, CityZoomIN, CityZoomOUT, ShieldShadow;
        public static Bitmap[] Desert, Plains, Grassland, ForestBase, HillsBase, MtnsBase, Tundra, Glacier, Swamp, Jungle, Ocean, River, Forest, Mountains, Hills, RiverMouth, Road, Railroad, Units, ShieldFront, ShieldBack, CityFlag, Improvements, BattleAnim;
        public static Bitmap[,] Coast, City, CityWall, DitherBlank, DitherDots, DitherDesert, DitherPlains, DitherGrassland, DitherForest, DitherHills, DitherMountains, DitherTundra, DitherGlacier, DitherSwamp, DitherJungle, PeopleL, PeopleLshadow, ResearchIcons;
        public static Point[] UnitShieldLoc = new Point[63];
        public static Point[,] CityFlagLoc, CitySizeWindowLoc, CityWallFlagLoc, CityWallSizeWindowLoc;
        
        public static Bitmap[,] MapTileGraphic;

        public static void LoadGraphicsAssetsFromFiles(string path)
        {
            MapImageLoader.LoadCities(new Ruleset { FolderPath = path});
            TerrainBitmapsImportFromFile(path);
            CitiesBitmapsImportFromFile(path);
            UnitsBitmapsImportFromFile(path);
            PeopleIconsBitmapsImportFromFile(path);
            IconsBitmapsImportFromFile(path);
            CityWallpaperBitmapImportFromFile();
        }

        public static void LoadGraphicsAssetsAtIntroScreen()
        {
            ImportDLLimages();
            ImportWallpapersFromIconsFile();
            //ImportCiv2Icon();
        }

        public static Bitmap MapTileGraphicC2(int xC2, int yC2) => MapTileGraphic[(((xC2 + 2 * Map.Instance.Xdim) % (2 * Map.Instance.Xdim)) - yC2 % 2) / 2, yC2];  // Return tile graphics for civ2-coords input

        // Extract icon from civ2.exe file
        //public static void ImportCiv2Icon()
        //{
        //    try
        //    {
        //        Civ2Icon = Icon.ExtractAssociatedIcon(Settings.Civ2Path + "civ2.exe");
        //    }
        //    catch
        //    {
        //        Debug.WriteLine("Civ2.exe not found!");
        //    }
        //}

        public static void TerrainBitmapsImportFromFile(string path)
        {
            // Read file in local directory. If it doesn't exist there, read it in root civ2 directory.
            var terrain1 = LoadBitmapFrom("TERRAIN1.GIF", path);
            var terrain2 = LoadBitmapFrom( "TERRAIN2.GIF", path);

            // Initialize objects
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

            // Define transparent colors
            Color transparentGray = Color.FromArgb(135, 135, 135);    // Define transparent back color (gray)
            Color transparentPink = Color.FromArgb(255, 0, 255);    // Define transparent back color (pink)
            Color transparentCyan = Color.FromArgb(0, 255, 255);    // Define transparent back color (cyan)

            // Tiles
            for (int i = 0; i < 4; i++)
            {
                Desert[i] = terrain1.Clone(new Rectangle(i + 1 + (i * 64), 1, 64, 32));
                Desert[i].ReplaceColors(transparentGray, Colors.Transparent);
                Desert[i].ReplaceColors(transparentPink, Colors.Transparent);
                Plains[i] = terrain1.Clone(new Rectangle(i + 1 + (i * 64), (2 * 1) + (1 * 32), 64, 32));
                Plains[i].ReplaceColors(transparentGray, Colors.Transparent);
                Plains[i].ReplaceColors(transparentPink, Colors.Transparent);
                Grassland[i] = terrain1.Clone(new Rectangle(i + 1 + (i * 64), (3 * 1) + (2 * 32), 64, 32));
                Grassland[i].ReplaceColors(transparentGray, Colors.Transparent);
                Grassland[i].ReplaceColors(transparentPink, Colors.Transparent);
                ForestBase[i] = terrain1.Clone(new Rectangle(i + 1 + (i * 64), (4 * 1) + (3 * 32), 64, 32));
                ForestBase[i].ReplaceColors(transparentGray, Colors.Transparent);
                ForestBase[i].ReplaceColors(transparentPink, Colors.Transparent);
                HillsBase[i] = terrain1.Clone(new Rectangle(i + 1 + (i * 64), (5 * 1) + (4 * 32), 64, 32));
                HillsBase[i].ReplaceColors(transparentGray, Colors.Transparent);
                HillsBase[i].ReplaceColors(transparentPink, Colors.Transparent);
                MtnsBase[i] = terrain1.Clone(new Rectangle(i + 1 + (i * 64), (6 * 1) + (5 * 32), 64, 32));
                MtnsBase[i].ReplaceColors(transparentGray, Colors.Transparent);
                MtnsBase[i].ReplaceColors(transparentPink, Colors.Transparent);
                Tundra[i] = terrain1.Clone(new Rectangle(i + 1 + (i * 64), (7 * 1) + (6 * 32), 64, 32));
                Tundra[i].ReplaceColors(transparentGray, Colors.Transparent);
                Tundra[i].ReplaceColors(transparentPink, Colors.Transparent);
                Glacier[i] = terrain1.Clone(new Rectangle(i + 1 + (i * 64), (8 * 1) + (7 * 32), 64, 32));
                Glacier[i].ReplaceColors(transparentGray, Colors.Transparent);
                Glacier[i].ReplaceColors(transparentPink, Colors.Transparent);
                Swamp[i] = terrain1.Clone(new Rectangle(i + 1 + (i * 64), (9 * 1) + (8 * 32), 64, 32));
                Swamp[i].ReplaceColors(transparentGray, Colors.Transparent);
                Swamp[i].ReplaceColors(transparentPink, Colors.Transparent);
                Jungle[i] = terrain1.Clone(new Rectangle(i + 1 + (i * 64), (10 * 1) + (9 * 32), 64, 32));
                Jungle[i].ReplaceColors(transparentGray, Colors.Transparent);
                Jungle[i].ReplaceColors(transparentPink, Colors.Transparent);
                Ocean[i] = terrain1.Clone(new Rectangle(i + 1 + (i * 64), (11 * 1) + (10 * 32), 64, 32));
                Ocean[i].ReplaceColors(transparentGray, Colors.Transparent);
                Ocean[i].ReplaceColors(transparentPink, Colors.Transparent);
            }

            // 4 small dither tiles
            DitherBlank = new Bitmap[2, 2];
            DitherDots = new Bitmap[2, 2];
            for (int tileX = 0; tileX < 2; tileX++)
            {
                for (int tileY = 0; tileY < 2; tileY++)
                {
                    DitherBlank[tileX, tileY] = terrain1.Clone(new Rectangle((tileX * 32) + 1, (tileY * 16) + 447, 32, 16));
                    DitherDots[tileX, tileY] = DitherBlank[tileX, tileY];
                    DitherDots[tileX, tileY].ReplaceColors(transparentGray, Colors.Transparent);
                    DitherDots[tileX, tileY].ReplaceColors(transparentPink, Colors.Transparent);
                }
            }

            // Blank tile
            Blank = terrain1.Clone(new Rectangle(131, 447, 64, 32));
            Blank.ReplaceColors(transparentGray, Colors.Transparent);

            // Dither base (only useful for grasland?)
            DitherBase = terrain1.Clone(new Rectangle(196, 447, 64, 32));

            // Replace black dither pixels with base pixels
            DitherDesert = new Bitmap[2, 2];   // 4 dither tiles for one 64x32 map tile
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
            for (int tileX = 0; tileX < 2; tileX++)
            {    // For 4 directions (NE, SE, SW, NW)
                for (int tileY = 0; tileY < 2; tileY++)
                {
                    DitherDesert[tileX, tileY] = new Bitmap(32, 16, PixelFormat.Format32bppRgba);
                    DitherPlains[tileX, tileY] = new Bitmap(32, 16, PixelFormat.Format32bppRgba);
                    DitherGrassland[tileX, tileY] = new Bitmap(32, 16, PixelFormat.Format32bppRgba);
                    DitherForest[tileX, tileY] = new Bitmap(32, 16, PixelFormat.Format32bppRgba);
                    DitherHills[tileX, tileY] = new Bitmap(32, 16, PixelFormat.Format32bppRgba);
                    DitherMountains[tileX, tileY] = new Bitmap(32, 16, PixelFormat.Format32bppRgba);
                    DitherTundra[tileX, tileY] = new Bitmap(32, 16, PixelFormat.Format32bppRgba);
                    DitherGlacier[tileX, tileY] = new Bitmap(32, 16, PixelFormat.Format32bppRgba);
                    DitherSwamp[tileX, tileY] = new Bitmap(32, 16, PixelFormat.Format32bppRgba);
                    DitherJungle[tileX, tileY] = new Bitmap(32, 16, PixelFormat.Format32bppRgba);
                    for (int col = 0; col < 32; col++)
                    {
                        for (int row = 0; row < 16; row++)
                        {
                            // replacementColor = DitherBlank.GetPixel(tileX * 32 + col, tileY * 16 + row);
                            replacementColor = DitherBlank[tileX, tileY].GetPixel(col, row);
                            if (replacementColor == Color.FromArgb(0, 0, 0))
                            {
                                DitherDesert[tileX, tileY].SetPixel(col, row, Desert[0].GetPixel((tileX * 32) + col, (tileY * 16) + row));
                                DitherPlains[tileX, tileY].SetPixel(col, row, Plains[0].GetPixel((tileX * 32) + col, (tileY * 16) + row));
                                DitherGrassland[tileX, tileY].SetPixel(col, row, Grassland[0].GetPixel((tileX * 32) + col, (tileY * 16) + row));
                                DitherForest[tileX, tileY].SetPixel(col, row, ForestBase[0].GetPixel((tileX * 32) + col, (tileY * 16) + row));
                                DitherHills[tileX, tileY].SetPixel(col, row, HillsBase[0].GetPixel((tileX * 32) + col, (tileY * 16) + row));
                                DitherMountains[tileX, tileY].SetPixel(col, row, MtnsBase[0].GetPixel((tileX * 32) + col, (tileY * 16) + row));
                                DitherTundra[tileX, tileY].SetPixel(col, row, Tundra[0].GetPixel((tileX * 32) + col, (tileY * 16) + row));
                                DitherGlacier[tileX, tileY].SetPixel(col, row, Glacier[0].GetPixel((tileX * 32) + col, (tileY * 16) + row));
                                DitherSwamp[tileX, tileY].SetPixel(col, row, Swamp[0].GetPixel((tileX * 32) + col, (tileY * 16) + row));
                                DitherJungle[tileX, tileY].SetPixel(col, row, Jungle[0].GetPixel((tileX * 32) + col, (tileY * 16) + row));
                            }
                            else
                            {
                                DitherDesert[tileX, tileY].SetPixel(col, row, Color.FromArgb(0, 0, 0, 0));
                                DitherPlains[tileX, tileY].SetPixel(col, row, Color.FromArgb(0, 0, 0, 0));
                                DitherGrassland[tileX, tileY].SetPixel(col, row, Color.FromArgb(0, 0, 0, 0));
                                DitherForest[tileX, tileY].SetPixel(col, row, Color.FromArgb(0, 0, 0, 0));
                                DitherHills[tileX, tileY].SetPixel(col, row, Color.FromArgb(0, 0, 0, 0));
                                DitherMountains[tileX, tileY].SetPixel(col, row, Color.FromArgb(0, 0, 0, 0));
                                DitherTundra[tileX, tileY].SetPixel(col, row, Color.FromArgb(0, 0, 0, 0));
                                DitherGlacier[tileX, tileY].SetPixel(col, row, Color.FromArgb(0, 0, 0, 0));
                                DitherSwamp[tileX, tileY].SetPixel(col, row, Color.FromArgb(0, 0, 0, 0));
                                DitherJungle[tileX, tileY].SetPixel(col, row, Color.FromArgb(0, 0, 0, 0));
                            }
                        }
                    }
                }
            }

            // Rivers, Forest, Mountains, Hills
            for (int i = 0; i < 16; i++)
            {
                River[i] = terrain2.Clone(new Rectangle((i % 8) + 1 + ((i % 8) * 64), 3 + (i / 8) + ((2 + (i / 8)) * 32), 64, 32));
                River[i].ReplaceColors(transparentGray, Colors.Transparent);
                River[i].ReplaceColors(transparentPink, Colors.Transparent);
                Forest[i] = terrain2.Clone(new Rectangle((i % 8) + 1 + ((i % 8) * 64), 5 + (i / 8) + ((4 + (i / 8)) * 32), 64, 32));
                Forest[i].ReplaceColors(transparentGray, Colors.Transparent);
                Forest[i].ReplaceColors(transparentPink, Colors.Transparent);
                Mountains[i] = terrain2.Clone(new Rectangle((i % 8) + 1 + ((i % 8) * 64), 7 + (i / 8) + ((6 + (i / 8)) * 32), 64, 32));
                Mountains[i].ReplaceColors(transparentGray, Colors.Transparent);
                Mountains[i].ReplaceColors(transparentPink, Colors.Transparent);
                Hills[i] = terrain2.Clone(new Rectangle((i % 8) + 1 + ((i % 8) * 64), 9 + (i / 8) + ((8 + (i / 8)) * 32), 64, 32));
                Hills[i].ReplaceColors(transparentGray, Colors.Transparent);
                Hills[i].ReplaceColors(transparentPink, Colors.Transparent);
            }

            // River mouths
            for (int i = 0; i < 4; i++)
            {
                RiverMouth[i] = terrain2.Clone(new Rectangle(i + 1 + (i * 64), (11 * 1) + (10 * 32), 64, 32));
                RiverMouth[i].ReplaceColors(transparentGray, Colors.Transparent);
                RiverMouth[i].ReplaceColors(transparentPink, Colors.Transparent);
                RiverMouth[i].ReplaceColors(transparentCyan, Colors.Transparent);
            }

            // Coast
            for (int i = 0; i < 8; i++)
            {
                Coast[i, 0] = terrain2.Clone(new Rectangle((2 * i) + 1 + (2 * i * 32), 429, 32, 16));  // N
                Coast[i, 0].ReplaceColors(transparentGray, Colors.Transparent);
                Coast[i, 1] = terrain2.Clone(new Rectangle((2 * i) + 1 + (2 * i * 32), 429 + (1 * 1) + (1 * 16), 32, 16));  // S
                Coast[i, 1].ReplaceColors(transparentGray, Colors.Transparent);
                Coast[i, 2] = terrain2.Clone(new Rectangle((2 * i) + 1 + (2 * i * 32), 429 + (2 * 1) + (2 * 16), 32, 16));  // W
                Coast[i, 2].ReplaceColors(transparentGray, Colors.Transparent);
                Coast[i, 3] = terrain2.Clone(new Rectangle((2 * (i + 1)) + (((2 * i) + 1) * 32), 429 + (2 * 1) + (2 * 16), 32, 16));  // E
                Coast[i, 3].ReplaceColors(transparentGray, Colors.Transparent);
            }

            // Road & railorad
            for (int i = 0; i < 9; i++)
            {
                Road[i] = terrain1.Clone(new Rectangle(i + 1 + (i * 64), 364, 64, 32));
                Road[i].ReplaceColors(transparentGray, Colors.Transparent);
                Road[i].ReplaceColors(transparentPink, Colors.Transparent);
                Railroad[i] = terrain1.Clone(new Rectangle(i + 1 + (i * 64), 397, 64, 32));
                Railroad[i].ReplaceColors(transparentGray, Colors.Transparent);
                Railroad[i].ReplaceColors(transparentPink, Colors.Transparent);
            }

            Irrigation = terrain1.Clone(new Rectangle(456, 100, 64, 32));
            Irrigation.ReplaceColors(transparentGray, Colors.Transparent);
            Irrigation.ReplaceColors(transparentPink, Colors.Transparent);

            Farmland = terrain1.Clone(new Rectangle(456, 133, 64, 32));
            Farmland.ReplaceColors(transparentGray, Colors.Transparent);
            Farmland.ReplaceColors(transparentPink, Colors.Transparent);

            Mining = terrain1.Clone(new Rectangle(456, 166, 64, 32));
            Mining.ReplaceColors(transparentGray, Colors.Transparent);
            Mining.ReplaceColors(transparentPink, Colors.Transparent);

            Pollution = terrain1.Clone(new Rectangle(456, 199, 64, 32));
            Pollution.ReplaceColors(transparentGray, Colors.Transparent);
            Pollution.ReplaceColors(transparentPink, Colors.Transparent);

            GrasslandShield = terrain1.Clone(new Rectangle(456, 232, 64, 32));
            GrasslandShield.ReplaceColors(transparentGray, Colors.Transparent);
            GrasslandShield.ReplaceColors(transparentPink, Colors.Transparent);

            terrain1.Dispose();
            terrain2.Dispose();
        }

        public static void CitiesBitmapsImportFromFile(string path)
        {
            // Read file in local directory. If it doesn't exist there, read it in root civ2 directory.
            var cities = LoadBitmapFrom("CITIES.GIF", path);

            // Initialize objects
            // City = new Bitmap[6, 4];
            CityFlag = new Bitmap[9];
            // CityWall = new Bitmap[6, 4];
            // CityFlagLoc = new Point[6, 4];
            // CityWallFlagLoc = new Point[6, 4];
            // CitySizeWindowLoc = new Point[6, 4];
            // CityWallSizeWindowLoc = new Point[6, 4];

            // Define transparent colors
            Color transparentGray = Color.FromArgb(135, 135, 135);    //define transparent back color (gray)
            Color transparentPink = Color.FromArgb(255, 0, 255);    //define transparent back color (pink)
            //Color transparentCyan = Color.FromArgb(0, 255, 255);    //define transparent back color (cyan)

            // Get city bitmaps
            // for (int row = 0; row < 6; row++)
            // {
            //     for (int col = 0; col < 4; col++)
            //     {
                    // City[row, col] = cities.Clone(new Rectangle(1 + (65 * col), 39 + (49 * row), 64, 48));
                    // City[row, col].ReplaceColors(transparentGray, Colors.Transparent);
                    // City[row, col].ReplaceColors(transparentPink, Colors.Transparent);
                    // CityWall[row, col] = cities.Clone(new Rectangle(334 + (65 * col), 39 + (49 * row), 64, 48));
                    // CityWall[row, col].ReplaceColors(transparentGray, Colors.Transparent);
                    // CityWall[row, col].ReplaceColors(transparentPink, Colors.Transparent);
                    // Determine where the city size window is located (x-y)
                    for (int ix = 0; ix < 64; ix++) // In x-direction
                    {
                        // if (cities.GetPixel((65 * col) + ix, 38 + (49 * row)) == Color.FromArgb(0, 0, 255)) CityFlagLoc[row, col].X = ix;  // If pixel on border is blue
                        // if (cities.GetPixel(333 + (65 * col) + ix, 38 + (49 * row)) == Color.FromArgb(0, 0, 255)) CityWallFlagLoc[row, col].X = ix;
                    }
                    // For cities with wall
                    // for (int iy = 0; iy < 48; iy++) // In y-direction
                    // {
                        // if (cities.GetPixel(65 * col, 38 + (49 * row) + iy) == Color.FromArgb(0, 0, 255)) CityFlagLoc[row, col].Y = iy;
                        // if (cities.GetPixel(333 + (65 * col), 38 + (49 * row) + iy) == Color.FromArgb(0, 0, 255)) CityWallFlagLoc[row, col].Y = iy;
                    // }
            //     }
            // }

            // Get flag bitmaps
            for (int col = 0; col < 9; col++)
            {
                CityFlag[col] = cities.Clone(new Rectangle(1 + (15 * col), 425, 14, 22));
                CityFlag[col].ReplaceColors(transparentGray, Colors.Transparent);
            }

            // Locations of city size windows (for standard zoom)
            // CitySizeWindowLoc[0, 0] = new Point(13, 23);    // Stone age
            // CitySizeWindowLoc[0, 1] = new Point(52, 18);
            // CitySizeWindowLoc[0, 2] = new Point(0, 23);
            // CitySizeWindowLoc[0, 3] = new Point(24, 29);
            // CitySizeWindowLoc[1, 0] = new Point(10, 23);    // Ancient
            // CitySizeWindowLoc[1, 1] = new Point(50, 25);
            // CitySizeWindowLoc[1, 2] = new Point(1, 17);
            // CitySizeWindowLoc[1, 3] = new Point(12, 27);
            // CitySizeWindowLoc[2, 0] = new Point(3, 20);    // Far east
            // CitySizeWindowLoc[2, 1] = new Point(48, 7);
            // CitySizeWindowLoc[2, 2] = new Point(50, 5);
            // CitySizeWindowLoc[2, 3] = new Point(28, 27);
            // CitySizeWindowLoc[3, 0] = new Point(5, 22);    // Medieval
            // CitySizeWindowLoc[3, 1] = new Point(2, 18);
            // CitySizeWindowLoc[3, 2] = new Point(0, 18);
            // CitySizeWindowLoc[3, 3] = new Point(27, 27);
            // CitySizeWindowLoc[4, 0] = new Point(4, 20);    // Industrial
            // CitySizeWindowLoc[4, 1] = new Point(1, 20);
            // CitySizeWindowLoc[4, 2] = new Point(2, 22);
            // CitySizeWindowLoc[4, 3] = new Point(28, 30);
            // CitySizeWindowLoc[5, 0] = new Point(8, 18);    // Modern
            // CitySizeWindowLoc[5, 1] = new Point(2, 19);
            // CitySizeWindowLoc[5, 2] = new Point(8, 20);
            // CitySizeWindowLoc[5, 3] = new Point(27, 30);
            // CityWallSizeWindowLoc[0, 0] = new Point(12, 23);    // Stone + wall
            // CityWallSizeWindowLoc[0, 1] = new Point(52, 22);
            // CityWallSizeWindowLoc[0, 2] = new Point(0, 19);
            // CityWallSizeWindowLoc[0, 3] = new Point(24, 29);
            // CityWallSizeWindowLoc[1, 0] = new Point(10, 13);    // Ancient + wall
            // CityWallSizeWindowLoc[1, 1] = new Point(50, 21);
            // CityWallSizeWindowLoc[1, 2] = new Point(1, 17);
            // CityWallSizeWindowLoc[1, 3] = new Point(11, 22);
            // CityWallSizeWindowLoc[2, 0] = new Point(4, 18);    // Far east + wall
            // CityWallSizeWindowLoc[2, 1] = new Point(48, 6);
            // CityWallSizeWindowLoc[2, 2] = new Point(51, 4);
            // CityWallSizeWindowLoc[2, 3] = new Point(28, 27);
            // CityWallSizeWindowLoc[3, 0] = new Point(3, 18);    // Medieval + wall
            // CityWallSizeWindowLoc[3, 1] = new Point(2, 20);
            // CityWallSizeWindowLoc[3, 2] = new Point(1, 15);
            // CityWallSizeWindowLoc[3, 3] = new Point(27, 29);
            // CityWallSizeWindowLoc[4, 0] = new Point(4, 18);    // Industrial + wall
            // CityWallSizeWindowLoc[4, 1] = new Point(1, 20);
            // CityWallSizeWindowLoc[4, 2] = new Point(1, 18);
            // CityWallSizeWindowLoc[4, 3] = new Point(26, 28);
            // CityWallSizeWindowLoc[5, 0] = new Point(3, 21);    // Modern + wall
            // CityWallSizeWindowLoc[5, 1] = new Point(0, 20);
            // CityWallSizeWindowLoc[5, 2] = new Point(8, 20);
            // CityWallSizeWindowLoc[5, 3] = new Point(27, 30);

            Fortified = cities.Clone(new Rectangle(143, 423, 64, 48));
            Fortified.ReplaceColors(transparentGray, Colors.Transparent);
            Fortified.ReplaceColors(transparentPink, Colors.Transparent);

            Fortress = cities.Clone(new Rectangle(208, 423, 64, 48));
            Fortress.ReplaceColors(transparentGray, Colors.Transparent);
            Fortress.ReplaceColors(transparentPink, Colors.Transparent);

            Airbase = cities.Clone(new Rectangle(273, 423, 64, 48));
            Airbase.ReplaceColors(transparentGray, Colors.Transparent);
            Airbase.ReplaceColors(transparentPink, Colors.Transparent);

            AirbasePlane = cities.Clone(new Rectangle(338, 423, 64, 48));
            AirbasePlane.ReplaceColors(transparentGray, Colors.Transparent);
            AirbasePlane.ReplaceColors(transparentPink, Colors.Transparent);

            cities.Dispose();
        }

        public static void UnitsBitmapsImportFromFile(string path)
        {
            var units = LoadBitmapFrom("UNITS.GIF", path);

            // Initialize objects
            Units = new Bitmap[63];
            ShieldFront = new Bitmap[8];
            ShieldBack = new Bitmap[8];

            // Define transparent colors
            Color transparentGray = units.GetPixel(1, 2); //define transparent back color (gray)
            Color transparentPink = units.GetPixel(1, 1); //define transparent back color (pink)


            int count = 0;
            for (int row = 0; row < 7; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    Units[count] = units.Clone(new Rectangle((64 * col) + 1 + col, (48 * row) + 1 + row, 64, 48));
                    Units[count].ReplaceColors(transparentGray, Colors.Transparent);
                    Units[count].ReplaceColors(transparentPink, Colors.Transparent);
                    // Determine where the unit shield is located (x-y)
                    for (int ix = 0; ix < 64; ix++)
                        if (units.GetPixel((65 * col) + ix, 49 * row) == Color.FromArgb(0, 0, 255)) UnitShieldLoc[count].X = ix - 1;  // If pixel on border is blue, in x-direction
                    for (int iy = 0; iy < 48; iy++)
                        if (units.GetPixel(65 * col, (49 * row) + iy) == Color.FromArgb(0, 0, 255)) UnitShieldLoc[count].Y = iy - 1;  // In y-direction
                    count++;
                }
            }

            // Extract shield without black border (used for stacked units)
            var _backUnitShield = units.Clone(new Rectangle(586, 1, 12, 20));
            _backUnitShield.ReplaceColors(transparentGray, Colors.Transparent);

            // Extract unit shield
            var _unitShield = units.Clone(new Rectangle(597, 30, 12, 20));
            _unitShield.ReplaceColors(transparentGray, Colors.Transparent);

            // Make shields of different colors for 8 different civs
            ShieldFront[0] = CreateNonIndexedImage(_unitShield); // convert GIF to non-indexed picture
            ShieldFront[1] = CreateNonIndexedImage(_unitShield);
            ShieldFront[2] = CreateNonIndexedImage(_unitShield);
            ShieldFront[3] = CreateNonIndexedImage(_unitShield);
            ShieldFront[4] = CreateNonIndexedImage(_unitShield);
            ShieldFront[5] = CreateNonIndexedImage(_unitShield);
            ShieldFront[6] = CreateNonIndexedImage(_unitShield);
            ShieldFront[7] = CreateNonIndexedImage(_unitShield);
            ShieldBack[0] = CreateNonIndexedImage(_backUnitShield);
            ShieldBack[1] = CreateNonIndexedImage(_backUnitShield);
            ShieldBack[2] = CreateNonIndexedImage(_backUnitShield);
            ShieldBack[3] = CreateNonIndexedImage(_backUnitShield);
            ShieldBack[4] = CreateNonIndexedImage(_backUnitShield);
            ShieldBack[5] = CreateNonIndexedImage(_backUnitShield);
            ShieldBack[6] = CreateNonIndexedImage(_backUnitShield);
            ShieldBack[7] = CreateNonIndexedImage(_backUnitShield);
            ShieldShadow = CreateNonIndexedImage(_backUnitShield);
            // Replace colors for unit shield and dark unit shield
            for (int x = 0; x < 12; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    if (_unitShield.GetPixel(x, y) == transparentPink)   // If color is pink, replace it
                    {
                        ShieldFront[0].SetPixel(x, y, CivColors.Light[0]);  // red
                        ShieldFront[1].SetPixel(x, y, CivColors.Light[1]);  // white
                        ShieldFront[2].SetPixel(x, y, CivColors.Light[2]);  // green
                        ShieldFront[3].SetPixel(x, y, CivColors.Light[3]);  // blue
                        ShieldFront[4].SetPixel(x, y, CivColors.Light[4]);  // yellow
                        ShieldFront[5].SetPixel(x, y, CivColors.Light[5]);  // cyan
                        ShieldFront[6].SetPixel(x, y, CivColors.Light[6]);  // orange
                        ShieldFront[7].SetPixel(x, y, CivColors.Light[7]);  // purple
                    }

                    if (_backUnitShield.GetPixel(x, y) == Color.FromArgb(255, 0, 0))    // If color is red, replace it
                    {
                        ShieldBack[0].SetPixel(x, y, CivColors.Dark[0]);  // red
                        ShieldBack[1].SetPixel(x, y, CivColors.Dark[1]);  // white
                        ShieldBack[2].SetPixel(x, y, CivColors.Dark[2]);  // green
                        ShieldBack[3].SetPixel(x, y, CivColors.Dark[3]);  // blue
                        ShieldBack[4].SetPixel(x, y, CivColors.Dark[4]);  // yellow
                        ShieldBack[5].SetPixel(x, y, CivColors.Dark[5]);  // cyan
                        ShieldBack[6].SetPixel(x, y, CivColors.Dark[6]);  // orange
                        ShieldBack[7].SetPixel(x, y, CivColors.Dark[7]);  // purple
                        ShieldShadow.SetPixel(x, y, Color.FromArgb(51, 51, 51));    // Color of the shield shadow
                    }
                }
            }

            units.Dispose();
        }

        public static void PeopleIconsBitmapsImportFromFile(string path)
        {
            var pplIcons = LoadBitmapFrom("PEOPLE.GIF", path);

            // Initialize objects
            PeopleL = new Bitmap[11, 4];
            PeopleLshadow = new Bitmap[11, 4];

            // Define transparent colors
            Color transparentPink = Color.FromArgb(255, 0, 255);    //define transparent back color (pink)

            // Make shadows of faces
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 11; col++)
                {
                    PeopleL[col, row] = pplIcons.Clone(new Rectangle((27 * col) + 2 + col, (30 * row) + 6 + row, 27, 30));

                    PeopleLshadow[col, row] = CreateNonIndexedImage(PeopleL[col, row]); //convert GIF to non-indexed picture

                    // If color is non-pink, replace it with black to get shadow (otherwise make transparent)
                    for (int x = 0; x < 27; x++)
                    {
                        for (int y = 0; y < 30; y++)
                        {
                            if (PeopleL[col, row].GetPixel(x, y) != transparentPink)
                                PeopleLshadow[col, row].SetPixel(x, y, Colors.Black);
                            else
                            {
                                Color transparent = PeopleLshadow[col, row].GetPixel(x, y);
                                transparent.A = 0;
                                PeopleLshadow[col, row].SetPixel(x, y, transparent);
                            }
                                
                        }
                    }
                    PeopleL[col, row].ReplaceColors(transparentPink, Colors.Transparent);
                }
            }

            pplIcons.Dispose();
        }

        // Import wallpapers for intro screen
        public static void ImportWallpapersFromIconsFile()
        {
            using (var icons = LoadBitmapFrom("ICONS.GIF"))
            {
                PanelOuterWallpaper = icons.Clone(new Rectangle(199, 322, 64, 32));
                PanelInnerWallpaper = icons.Clone(new Rectangle(298, 190, 32, 32));
            }
        }

        /// <summary>
        //  Read file in local directory. If it doesn't exist there, read it in root civ2 directory.
        /// </summary>
        /// <param name="name">the filename to load</param>
        /// <param name="path">the local directory to load from</param>
        /// <returns></returns>
        private static Bitmap LoadBitmapFrom(string name, string path = null)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                string FilePath_local = path + Path.DirectorySeparatorChar + name;
                if (File.Exists(FilePath_local))
                {
                    return new Bitmap(FilePath_local);
                }
            }

            string FilePath_root = Settings.Civ2Path + name;
            if (File.Exists(FilePath_root))
            {
                return new Bitmap(FilePath_root);
            }
            Debug.WriteLine(name + " not found!");
            return new Bitmap(640, 480, PixelFormat.Format32bppRgba);
        }

        public static void IconsBitmapsImportFromFile(string path)
        {
            var icons = LoadBitmapFrom(path, "ICONS.GIF");

            // Initialize objects
            Improvements = new Bitmap[67];
            ResearchIcons = new Bitmap[5, 4];

            // Define transparent colors
            Color transparentLightPink = Color.FromArgb(255, 159, 163);//define transparent back color (light pink)
            Color transparentPink = Color.FromArgb(255, 0, 255);    //define transparent back color (pink)
            //Color transparentGray = Color.FromArgb(135, 83, 135);    //define transparent back color (gray)

            // Improvement icons
            int count = 1;  //start at 1. 0 is for no improvement.
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Improvements[count] = icons.Clone(new Rectangle((36 * col) + 343 + col, (20 * row) + 1 + row, 36, 20));
                    count++;
                    if (count == 39) break;
                }
            }

            // WondersIcons
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    Improvements[count] = icons.Clone(new Rectangle((36 * col) + 343 + col, (20 * row) + 106 + row, 36, 20));
                    count++;
                }
            }

            // Research icons
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    ResearchIcons[col, row] = icons.Clone(new Rectangle((36 * col) + 343 + col, (20 * row) + 211 + row, 36, 20));
                }
            }

            SellIcon = icons.Clone(new Rectangle(16, 320, 14, 14));
            SellIcon.ReplaceColors(transparentLightPink, Colors.Transparent);

            ViewPiece = icons.Clone(new Rectangle(199, 256, 64, 32));
            ViewPiece.ReplaceColors(transparentLightPink, Colors.Transparent);
            ViewPiece.ReplaceColors(transparentPink, Colors.Transparent);

            GridLines = icons.Clone(new Rectangle(183, 430, 64, 32));
            GridLines.ReplaceColors(transparentLightPink, Colors.Transparent);
            GridLines.ReplaceColors(transparentPink, Colors.Transparent);

            GridLinesVisible = icons.Clone(new Rectangle(248, 430, 64, 32));
            GridLinesVisible.ReplaceColors(transparentLightPink, Colors.Transparent);
            GridLinesVisible.ReplaceColors(transparentPink, Colors.Transparent);

            // Big icons in city resources
            CityHungerBig = icons.Clone(new Rectangle(1, 290, 14, 14));
            CityHungerBig.ReplaceColors(transparentLightPink, Colors.Transparent);
            CityShortageBig = icons.Clone(new Rectangle(16, 290, 14, 14));
            CityShortageBig.ReplaceColors(transparentLightPink, Colors.Transparent);
            CityCorruptBig = icons.Clone(new Rectangle(31, 290, 14, 14));
            CityCorruptBig.ReplaceColors(transparentLightPink, Colors.Transparent);
            CityFoodBig = icons.Clone(new Rectangle(1, 305, 14, 14));
            CityFoodBig.ReplaceColors(transparentLightPink, Colors.Transparent);
            CitySupportBig = icons.Clone(new Rectangle(16, 305, 14, 14));
            CitySupportBig.ReplaceColors(transparentLightPink, Colors.Transparent);
            CityTradeBig = icons.Clone(new Rectangle(31, 305, 14, 14));
            CityTradeBig.ReplaceColors(transparentLightPink, Colors.Transparent);
            CityLuxBig = icons.Clone(new Rectangle(1, 320, 14, 14));
            CityLuxBig.ReplaceColors(transparentLightPink, Colors.Transparent);
            CityTaxBig = icons.Clone(new Rectangle(16, 320, 14, 14));
            CityTaxBig.ReplaceColors(transparentLightPink, Colors.Transparent);
            CitySciBig = icons.Clone(new Rectangle(31, 320, 14, 14));
            CitySciBig.ReplaceColors(transparentLightPink, Colors.Transparent);

            // Small icons in city resources
            CityFoodSmall = icons.Clone(new Rectangle(49, 334, 10, 10));
            CityFoodSmall.ReplaceColors(transparentLightPink, Colors.Transparent);
            CitySupportSmall = icons.Clone(new Rectangle(60, 334, 10, 10));
            CitySupportSmall.ReplaceColors(transparentLightPink, Colors.Transparent);
            CityTradeSmall = icons.Clone(new Rectangle(71, 334, 10, 10));
            CityTradeSmall.ReplaceColors(transparentLightPink, Colors.Transparent);

            // Wallpaper icons
            //PanelOuterWallpaper = icons.Clone(new Rectangle(199, 322, 64, 32));
            //PanelInnerWallpaper = icons.Clone(new Rectangle(298, 190, 32, 32));

            // Icon for next/previous city (black arrow)
            NextCity = icons.Clone(new Rectangle(227, 389, 18, 24));
            NextCity.ReplaceColors(transparentLightPink, Colors.Transparent);
            PrevCity = icons.Clone(new Rectangle(246, 389, 18, 24));
            PrevCity.ReplaceColors(transparentLightPink, Colors.Transparent);
            //NextCityLarge = ModifyImage.Resize(NextCity, 27, 36);    //50% larger
            NextCityLarge = NextCity;
            //PrevCityLarge = ModifyImage.Resize(PrevCity, 27, 36);    //50% larger
            PrevCityLarge = PrevCity;

            // City window icons
            CityExit = icons.Clone(new Rectangle(1, 389, 16, 16));
            CityZoomOUT = icons.Clone(new Rectangle(18, 389, 16, 16));
            CityZoomIN = icons.Clone(new Rectangle(35, 389, 16, 16));

            // Battle sprites
            BattleAnim = new Bitmap[8];
            for (int i = 0; i < 8; i++)
            {
                BattleAnim[i] = icons.Clone(new Rectangle(1  + 33 * i, 356, 32, 32));
                BattleAnim[i].ReplaceColors(transparentLightPink, Colors.Transparent);
            }

            icons.Dispose();
        }

        public static void CityWallpaperBitmapImportFromFile()
        {
            var cityWallpaper = LoadBitmapFrom("CITY.GIF");
            CityWallpaper = cityWallpaper.CropImage(new Rectangle(0, 0, 636, 421));
        }

        private static Bitmap CreateNonIndexedImage(Image src)  //Converting GIFs to non-indexed images (required for SetPixel method)
        {
            var newBmp = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppRgba);

            //using (Graphics gfx = Graphics.FromImage(newBmp))
            //{
            //    gfx.DrawImage(src, 0, 0);
            //}

            return newBmp;
        }
    }
}
