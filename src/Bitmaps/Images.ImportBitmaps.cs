using System;
using System.Drawing;
using System.IO;

namespace civ2.Bitmaps
{
    public partial class Images
    {
        // Extract icon from civ2.exe file
        public static void ImportCiv2Icon()
        {
            try
            {
                Civ2Icon = Icon.ExtractAssociatedIcon(Settings.Civ2Path + "civ2.exe");
            }
            catch
            {
                Console.WriteLine("Civ2.exe not found!");
            }
            
        }

        public static void TerrainBitmapsImportFromFile()
        {
            Bitmap terrain1 = new Bitmap(640, 480);
            try
            {
                terrain1 = new Bitmap(Settings.Civ2Path + "TERRAIN1.GIF");
            }
            catch
            {
                Console.WriteLine("TERRAIN1.GIF not found!");
            }
            
            Bitmap terrain2 = new Bitmap(640, 480);
            try
            {
                terrain2 = new Bitmap(Settings.Civ2Path + "TERRAIN2.GIF");
            }
            catch
            {
                Console.WriteLine("TERRAIN2.GIF not found!");
            }

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
            for (int i = 0; i < 4; i++)
            {
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
                Ocean[i].MakeTransparent(transparentPink);
            }

            //4 small dither tiles
            DitherBlank = new Bitmap[2, 2];
            DitherDots = new Bitmap[2, 2];
            for (int tileX = 0; tileX < 2; tileX++)
                for (int tileY = 0; tileY < 2; tileY++)
                {
                    DitherBlank[tileX, tileY] = terrain1.Clone(new Rectangle(tileX * 32 + 1, tileY * 16 + 447, 32, 16), terrain1.PixelFormat);
                    DitherDots[tileX, tileY] = DitherBlank[tileX, tileY];
                    DitherDots[tileX, tileY].MakeTransparent(transparentGray);
                    DitherDots[tileX, tileY].MakeTransparent(transparentPink);
                }

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
            for (int tileX = 0; tileX < 2; tileX++)
            {    //for 4 directions (NE, SE, SW, NW)
                for (int tileY = 0; tileY < 2; tileY++)
                {
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
                    for (int col = 0; col < 32; col++)
                    {
                        for (int row = 0; row < 16; row++)
                        {
                            //replacementColor = DitherBlank.GetPixel(tileX * 32 + col, tileY * 16 + row);
                            replacementColor = DitherBlank[tileX, tileY].GetPixel(col, row);
                            if (replacementColor == Color.FromArgb(0, 0, 0))
                            {
                                DitherDesert[tileX, tileY].SetPixel(col, row, Desert[0].GetPixel(tileX * 32 + col, tileY * 16 + row));
                                DitherPlains[tileX, tileY].SetPixel(col, row, Plains[0].GetPixel(tileX * 32 + col, tileY * 16 + row));
                                DitherGrassland[tileX, tileY].SetPixel(col, row, Grassland[0].GetPixel(tileX * 32 + col, tileY * 16 + row));
                                DitherForest[tileX, tileY].SetPixel(col, row, ForestBase[0].GetPixel(tileX * 32 + col, tileY * 16 + row));
                                DitherHills[tileX, tileY].SetPixel(col, row, HillsBase[0].GetPixel(tileX * 32 + col, tileY * 16 + row));
                                DitherMountains[tileX, tileY].SetPixel(col, row, MtnsBase[0].GetPixel(tileX * 32 + col, tileY * 16 + row));
                                DitherTundra[tileX, tileY].SetPixel(col, row, Tundra[0].GetPixel(tileX * 32 + col, tileY * 16 + row));
                                DitherGlacier[tileX, tileY].SetPixel(col, row, Glacier[0].GetPixel(tileX * 32 + col, tileY * 16 + row));
                                DitherSwamp[tileX, tileY].SetPixel(col, row, Swamp[0].GetPixel(tileX * 32 + col, tileY * 16 + row));
                                DitherJungle[tileX, tileY].SetPixel(col, row, Jungle[0].GetPixel(tileX * 32 + col, tileY * 16 + row));
                            }
                            else
                            {
                                DitherDesert[tileX, tileY].SetPixel(col, row, Color.Transparent);
                                DitherPlains[tileX, tileY].SetPixel(col, row, Color.Transparent);
                                DitherGrassland[tileX, tileY].SetPixel(col, row, Color.Transparent);
                                DitherForest[tileX, tileY].SetPixel(col, row, Color.Transparent);
                                DitherHills[tileX, tileY].SetPixel(col, row, Color.Transparent);
                                DitherMountains[tileX, tileY].SetPixel(col, row, Color.Transparent);
                                DitherTundra[tileX, tileY].SetPixel(col, row, Color.Transparent);
                                DitherGlacier[tileX, tileY].SetPixel(col, row, Color.Transparent);
                                DitherSwamp[tileX, tileY].SetPixel(col, row, Color.Transparent);
                                DitherJungle[tileX, tileY].SetPixel(col, row, Color.Transparent);
                            }
                        }
                    }
                }
            }


            //Rivers, Forest, Mountains, Hills
            for (int i = 0; i < 16; i++)
            {
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
                Hills[i].MakeTransparent(transparentPink);
            }

            //River mouths
            for (int i = 0; i < 4; i++)
            {
                RiverMouth[i] = terrain2.Clone(new Rectangle(i + 1 + i * 64, 11 * 1 + 10 * 32, 64, 32), terrain2.PixelFormat);
                RiverMouth[i].MakeTransparent(transparentGray);
                RiverMouth[i].MakeTransparent(transparentPink);
                RiverMouth[i].MakeTransparent(transparentCyan);
            }

            //Coast
            for (int i = 0; i < 8; i++)
            {
                Coast[i, 0] = terrain2.Clone(new Rectangle(2 * i + 1 + 2 * i * 32, 429, 32, 16), terrain2.PixelFormat);  // N
                Coast[i, 0].MakeTransparent(transparentGray);
                Coast[i, 1] = terrain2.Clone(new Rectangle(2 * i + 1 + 2 * i * 32, 429 + 1 * 1 + 1 * 16, 32, 16), terrain2.PixelFormat);  // S
                Coast[i, 1].MakeTransparent(transparentGray);
                Coast[i, 2] = terrain2.Clone(new Rectangle(2 * i + 1 + 2 * i * 32, 429 + 2 * 1 + 2 * 16, 32, 16), terrain2.PixelFormat);  // W
                Coast[i, 2].MakeTransparent(transparentGray);
                Coast[i, 3] = terrain2.Clone(new Rectangle(2 * (i + 1) + (2 * i + 1) * 32, 429 + 2 * 1 + 2 * 16, 32, 16), terrain2.PixelFormat);  // E
                Coast[i, 3].MakeTransparent(transparentGray);
            }

            //Road & railorad
            for (int i = 0; i < 9; i++)
            {
                Road[i] = terrain1.Clone(new Rectangle(i + 1 + i * 64, 364, 64, 32), terrain1.PixelFormat);
                Road[i].MakeTransparent(transparentGray);
                Road[i].MakeTransparent(transparentPink);
                Railroad[i] = terrain1.Clone(new Rectangle(i + 1 + i * 64, 397, 64, 32), terrain1.PixelFormat);
                Railroad[i].MakeTransparent(transparentGray);
                Railroad[i].MakeTransparent(transparentPink);
            }

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

        public static void CitiesBitmapsImportFromFile()
        {
            Bitmap cities = new Bitmap(640, 480);
            try
            {
                cities = new Bitmap(Settings.Civ2Path + "CITIES.GIF");
            }
            catch
            {
                Console.WriteLine("CITIES.GIF not found!");
            }

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
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    City[row, col] = cities.Clone(new Rectangle(1 + 65 * col, 39 + 49 * row, 64, 48), cities.PixelFormat);
                    City[row, col].MakeTransparent(transparentGray);
                    City[row, col].MakeTransparent(transparentPink);
                    CityWall[row, col] = cities.Clone(new Rectangle(334 + 65 * col, 39 + 49 * row, 64, 48), cities.PixelFormat);
                    CityWall[row, col].MakeTransparent(transparentGray);
                    CityWall[row, col].MakeTransparent(transparentPink);
                    //determine where the city size window is located (x-y)
                    for (int ix = 0; ix < 64; ix++)
                    { //in x-direction
                        if (cities.GetPixel(65 * col + ix, 38 + 49 * row) == Color.FromArgb(0, 0, 255)) cityFlagLoc[row, col, 0] = ix;  //if pixel on border is blue
                        if (cities.GetPixel(333 + 65 * col + ix, 38 + 49 * row) == Color.FromArgb(0, 0, 255)) cityWallFlagLoc[row, col, 0] = ix;
                    } //for cities with wall
                    for (int iy = 0; iy < 48; iy++)
                    { //in y-direction
                        if (cities.GetPixel(65 * col, 38 + 49 * row + iy) == Color.FromArgb(0, 0, 255)) cityFlagLoc[row, col, 1] = iy;
                        if (cities.GetPixel(333 + 65 * col, 38 + 49 * row + iy) == Color.FromArgb(0, 0, 255)) cityWallFlagLoc[row, col, 1] = iy;
                    }
                }
            }

            //Get flag bitmaps
            for (int col = 0; col < 9; col++)
            {
                CityFlag[col] = cities.Clone(new Rectangle(1 + 15 * col, 425, 14, 22), cities.PixelFormat);
                CityFlag[col].MakeTransparent(transparentGray);
            }

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

        public static void UnitsBitmapsImportFromFile()
        {
            Bitmap units = new Bitmap(640, 480);
            try
            {
                units = new Bitmap(Settings.Civ2Path + "UNITS.GIF");
            }
            catch
            {
                Console.WriteLine("UNITS.GIF not found!");
            }

            Units = new Bitmap[63];
            UnitShield = new Bitmap[8];
            NoBorderUnitShield = new Bitmap[8];

            //define transparent colors
            Color transparentGray = Color.FromArgb(135, 83, 135);    //define transparent back color (gray)
            Color transparentPink = Color.FromArgb(255, 0, 255);    //define transparent back color (pink)

            int count = 0;
            for (int row = 0; row < 7; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    Units[count] = units.Clone(new Rectangle(64 * col + 1 + col, 48 * row + 1 + row, 64, 48), units.PixelFormat);
                    Units[count].MakeTransparent(transparentGray);
                    Units[count].MakeTransparent(transparentPink);
                    //determine where the unit shield is located (x-y)
                    for (int ix = 0; ix < 64; ix++) if (units.GetPixel(65 * col + ix, 49 * row) == Color.FromArgb(0, 0, 255)) unitShieldLocation[count, 0] = ix;  //if pixel on border is blue, in x-direction
                    for (int iy = 0; iy < 48; iy++) if (units.GetPixel(65 * col, 49 * row + iy) == Color.FromArgb(0, 0, 255)) unitShieldLocation[count, 1] = iy;  //in y-direction
                    count++;
                }
            }

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

        public static void PeopleIconsBitmapsImportFromFile()
        {
            Bitmap icons = new Bitmap(640, 480);
            try
            {
                icons = new Bitmap(Settings.Civ2Path + "PEOPLE.GIF");
            }
            catch
            {
                Console.WriteLine("PEOPLE.GIF not found!");
            }

            PeopleL = new Bitmap[11, 4];
            PeopleLshadow = new Bitmap[11, 4];

            //define transparent colors
            Color transparentPink = Color.FromArgb(255, 0, 255);    //define transparent back color (pink)

            //Make shadows of faces
            for (int row = 0; row < 4; row++)
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

            icons.Dispose();
        }

        public static void IconsBitmapsImportFromFile()
        {
            Bitmap icons = new Bitmap(640, 480);
            try
            {
                icons = new Bitmap(Settings.Civ2Path + "ICONS.GIF");
            }
            catch
            {
                Console.WriteLine("ICONS.GIF not found!");
            }

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

            ViewPiece = icons.Clone(new Rectangle(199, 256, 64, 32), icons.PixelFormat);
            ViewPiece.MakeTransparent(transparentLightPink);
            ViewPiece.MakeTransparent(transparentPink);

            GridLines = icons.Clone(new Rectangle(183, 430, 64, 32), icons.PixelFormat);
            GridLines.MakeTransparent(transparentLightPink);
            GridLines.MakeTransparent(transparentPink);

            GridLinesVisible = icons.Clone(new Rectangle(248, 430, 64, 32), icons.PixelFormat);
            GridLinesVisible.MakeTransparent(transparentLightPink);
            GridLinesVisible.MakeTransparent(transparentPink);

            PanelOuterWallpaper = icons.Clone(new Rectangle(199, 322, 64, 32), icons.PixelFormat);
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

        public static void CityWallpaperBitmapImportFromFile()
        {
            Bitmap cityWallpaper = new Bitmap(640, 480);
            try
            {
                cityWallpaper = new Bitmap(Settings.Civ2Path + "CITY.GIF");
            }
            catch
            {
                Console.WriteLine("CITY.GIF not found!");
            }

            CityWallpaper = ModifyImage.CropImage(cityWallpaper, new Rectangle(0, 0, 640, 420));
        }

        private static Bitmap CreateNonIndexedImage(Image src)  //Converting GIFs to non-indexed images (required for SetPixel method)
        {
            Bitmap newBmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics gfx = Graphics.FromImage(newBmp))
            {
                gfx.DrawImage(src, 0, 0);
            }

            return newBmp;
        }
    }
}
