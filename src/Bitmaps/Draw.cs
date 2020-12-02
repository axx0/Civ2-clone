using ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using civ2.Enums;
using civ2.Forms;
using civ2.Units;
using civ2.Terrains;

namespace civ2.Bitmaps
{
    public partial class Draw : BaseInstance
    {
        public static Bitmap CityWallpaper, PanelOuterWallpaper,
                             Irrigation, Farmland, Mining, Pollution, Fortified, Fortress, Airbase, AirbasePlane, 
                             Shield, ViewPiece, WallpaperStatusForm, UnitShieldShadow, GridLines, GridLinesVisible, Dither, Blank, DitherBase, 
                             SellIcon, SellIconLarge, 
                             NextCityLarge, PrevCity, PrevCityLarge, ZoomIN, ZoomOUT;
        public static Bitmap[] Desert, Plains, Grassland, ForestBase, HillsBase, MtnsBase, Tundra, Glacier, Swamp, Jungle, Ocean, River, Forest, Mountains, Hills,  RiverMouth, Road, 
                               Railroad, Units, UnitShield, NoBorderUnitShield, CityFlag, Improvements, ImprovementsLarge, ImprovementsSmall;
        public static Bitmap[,] Coast, City, CityWall, DitherBlank, DitherDots, DitherDesert, DitherPlains, DitherGrassland, DitherForest, DitherHills, 
                                DitherMountains, DitherTundra, DitherGlacier, DitherSwamp, DitherJungle, PeopleL, PeopleLshadow, ResearchIcons;
        public static int[,] unitShieldLocation = new int[63, 2];
        public static int[,,] cityFlagLoc, cityWallFlagLoc, citySizeWindowLoc, cityWallSizeWindowLoc;
        //public static int[,,] cityWallFlagLoc = new int[6, 4, 2];
        public static Icon Civ2Icon;

        public static void LoadGraphicsAssetsFromFiles(string path)
        {
            TerrainBitmapsImportFromFile(path);
            CitiesBitmapsImportFromFile(path);
            UnitsBitmapsImportFromFile(path);
            PeopleIconsBitmapsImportFromFile(path);            
            IconsBitmapsImportFromFile(path);
        }

        public static void LoadGraphicsAssetsAtIntroScreen()
        {
            ImportDLLimages();
            ImportWallpapersFromIconsFile();
        }

        //public static void CreateLoadGameGraphics()
        //{
        //    //Creates bitmaps from current folder
        //    //LoadGraphicsAssetsFromFiles();

        //    //Create graphic of each map tile
        //    for (int col = 0; col < Data.MapXdim; col++)
        //        for (int row = 0; row < Data.MapYdim; row++)
        //            Map.Tile[col, row].Graphic = TerrainBitmap(col, row);

        //    //Add cities+units graphics to each tile & create image of maps (what each civ sees + one entire visible)
        //    Game.CivsMap = new Bitmap[9];            
        //    for (int civ = 0; civ < 9; civ++) CreateWholeMapImage(civ);  //What each civ (index=0...7) sees, additionally (index=8) for entire revealed map
        //}

        //Create image of civ's world maps
        public static void CreateWholeMapImage(int civ)
        {
            //Game.CivsMap[civ] = new Bitmap(64 * Data.MapXdim + 32, 32 * Data.MapYdim + 16);

            //using (Graphics g = Graphics.FromImage(Game.CivsMap[civ]))
            //{
            //    for (int row = 0; row < Data.MapYdim; row++)
            //    {
            //        for (int col = 0; col < Data.MapXdim; col++)
            //        {
            //            //Draw only if the tile is visible for each civ (index=8...whole map visible)
            //            if ((civ < 8 && Map.Tile[col, row].Visibility[civ]) || civ == 8)
            //            {
            //                //Tiles
            //                g.DrawImage(
            //                    Map.Tile[col, row].Graphic,
            //                    64 * col + 32 * (row % 2),
            //                    16 * row);

            //                //Implement dithering in all 4 directions if necessary
            //                if (civ != 8)
            //                    for (int tileX = 0; tileX < 2; tileX++)
            //                        for (int tileY = 0; tileY < 2; tileY++)
            //                        {
            //                            int[] offset = new int[] { -1, 1};
            //                            int col_ = 2 * col + (row % 2); //real->civ2 coords
            //                            int colNew_ = col_ + offset[tileX];
            //                            int rowNew = row + offset[tileY];
            //                            int colNew = (colNew_ - rowNew % 2) / 2; //back to real coords
            //                            if (colNew >= 0 && colNew < Data.MapXdim && rowNew >= 0 && rowNew < Data.MapYdim)   //don't observe outside map limits
            //                                if (!Map.Tile[colNew, rowNew].Visibility[civ])   //surrounding tile is not visible -> dither
            //                                    g.DrawImage(DitherDots[tileX, tileY],
            //                                                64 * col + 32 * (row % 2) + 32 * tileX,
            //                                                16 * row + 16 * tileY);
            //                        }


            //                //Units
            //                int[] coords = Ext.XYciv2(new int[] { col, row });  //civ2 coords from real coords
            //                int col2 = coords[0];
            //                int row2 = coords[1];
            //                List<IUnit> unitsHere = Game.Units.Where(u => u.X == col2 && u.Y == row2).ToList();
            //                if (unitsHere.Any())
            //                {
            //                    IUnit unit = unitsHere.Last();
            //                    int zoom = 8;
            //                    if (!unit.IsInCity)
            //                    {
            //                        g.DrawImage(
            //                            CreateUnitBitmap(unit, unitsHere.Count() > 1, zoom),
            //                            32 * col2,
            //                            16 * row2 - 16);
            //                    }
            //                }

            //                //Cities
            //                City city = Game.Cities.Find(c => c.X == col2 && c.Y == row2);
            //                if (city != null)
            //                {
            //                    g.DrawImage(
            //                        CreateCityBitmap(city, true, 8),
            //                        32 * col2,
            //                        16 * row2 - 16);
            //                }
            //            }                            
            //        }
            //    }

            //    //City name text is drawn last
            //    foreach (City city in Game.Cities)
            //    {
            //        int[] ColRow = Ext.Civ2xy(new int[] { city.X, city.Y });  //real coords from civ2 coords
            //        if ((civ < 8 && Map.Tile[ColRow[0], ColRow[1]].Visibility[civ]) || civ == 8)
            //        {
            //            Bitmap cityNameBitmap = CreateCityNameBitmap(city, 8);
            //            g.DrawImage(
            //                cityNameBitmap,
            //                32 * city.X + 32 - cityNameBitmap.Width / 2,
            //                16 * city.Y + 3 * 8);
            //        }
            //    }
            //}
        }

        //Redraw invisible to visible tiles in the map
        public static void RedrawMap(int[] centralCoords)
        {
            //Bitmap mapPart = new Bitmap(3 * 64, 3 * 32 + 16);   //part which will be pasted on top of existing world map

            ////Get coords of central tile & which squares are to be drawn
            //List<int[]> coordsOffsetsToBeDrawn = new List<int[]>
            //{
            //    new int[] {-2, -4},
            //    new int[] {0, -4},
            //    new int[] {2, -4},
            //    new int[] {-3, -3},
            //    new int[] {-1, -3},
            //    new int[] {1, -3},
            //    new int[] {3, -3},
            //    new int[] {-2, -2},
            //    new int[] {0, -2},
            //    new int[] {2, -2},
            //    new int[] {-3, -1},
            //    new int[] {-1, -1},
            //    new int[] {1, -1},
            //    new int[] {3, -1},
            //    new int[] {-2, 0},
            //    new int[] {0, 0},
            //    new int[] {2, 0},
            //    new int[] {-3, 1},
            //    new int[] {-1, 1},
            //    new int[] {1, 1},
            //    new int[] {3, 1},
            //    new int[] {-2, 2},
            //    new int[] {0, 2},
            //    new int[] {2, 2},
            //    new int[] {-3, 3},
            //    new int[] {-1, 3},
            //    new int[] {1, 3},
            //    new int[] {3, 3},
            //    new int[] {-2, 4},
            //    new int[] {0, 4},
            //    new int[] {2, 4}
            //};

            //int zoom = 8;

            //using (Graphics g = Graphics.FromImage(mapPart))
            //{
            //    g.FillRectangle(Brushes.Black, new Rectangle(0, 0, 3 * 64, 3 * 32 + 16));    //fill bitmap with black (necessary for correct drawing if image is on upper map edge)

            //    foreach (int[] coordsOffsets in coordsOffsetsToBeDrawn)
            //    {
            //        //change coords of central offset
            //        int xCiv2 = centralCoords[0] + coordsOffsets[0];
            //        int yCiv2 = centralCoords[1] + coordsOffsets[1];
            //        int xReal = (xCiv2 - yCiv2 % 2) / 2;
            //        int yReal = yCiv2;

            //        if (xCiv2 >= 0 && yCiv2 >= 0 && xCiv2 < 2 * Data.MapXdim && yCiv2 < Data.MapYdim)    //make sure you're not drawing tiles outside map bounds
            //        {
            //            //Tiles
            //            int civId = Game.Instance.ActiveCiv.Id;
            //            if ((civId < 8 && Map.Tile[xReal, yReal].Visibility[civId]) || civId == 8)
            //            {
            //                g.DrawImage(
            //                    Images.TerrainBitmap(xReal, yReal),
            //                    32 * coordsOffsets[0] + 64,
            //                    16 * coordsOffsets[1] + 48);

            //                //Implement dithering in all 4 directions if necessary
            //                if (civId != 8)
            //                    for (int tileX = 0; tileX < 2; tileX++)
            //                        for (int tileY = 0; tileY < 2; tileY++)
            //                        {
            //                            int[] offset = new int[] { -1, 1 };
            //                            int xCiv2New = xCiv2 + offset[tileX];
            //                            int yCiv2New = yCiv2 + offset[tileY];
            //                            int xRealNew = (xCiv2New - yCiv2New % 2) / 2; //back to real coords
            //                            int yRealNew = yCiv2New;
            //                            if (xRealNew >= 0 && xRealNew < Data.MapXdim && yRealNew >= 0 && yRealNew < Data.MapYdim)   //don't observe outside map limits
            //                                if (!Map.Tile[xRealNew, yRealNew].Visibility[civId])   //surrounding tile is not visible -> dither
            //                                    g.DrawImage(Images.DitherDots[tileX, tileY],
            //                                                32 * coordsOffsets[0] + 64 + 32 * tileX,
            //                                                16 * coordsOffsets[1] + 48 + 16 * tileY);
            //                        }
            //            }

            //            //Units
            //            List<IUnit> unitsHere = Game.Units.Where(u => u.X == xCiv2 && u.Y == yCiv2).ToList();
            //            //If active unit is in this list-- > remove it
            //            if (unitsHere.Contains(Game.Instance.ActiveUnit))
            //            {
            //                unitsHere.Remove(Game.Instance.ActiveUnit);
            //            }
            //            if (unitsHere.Any())
            //            {
            //                IUnit unit;
            //                //If this is not tile with active unit or viewing piece, draw last unit on stack
            //                if (!unitsHere.Contains(Game.Instance.ActiveUnit))
            //                {
            //                    unit = unitsHere.Last();
            //                    if (!unit.IsInCity)
            //                    {
            //                        g.DrawImage(
            //                            Images.CreateUnitBitmap(unit, unitsHere.Count() > 1, zoom),
            //                            32 * coordsOffsets[0] + 64,
            //                            16 * coordsOffsets[1] + 32);
            //                    }
            //                }
            //                //This tile has active unit/viewing piece
            //                else
            //                {
            //                    //Viewing pieces mode is enabled, so draw last unit on stack
            //                    if (MapPanel.ViewPiecesMode)
            //                    {
            //                        unit = unitsHere.Last();
            //                        if (!unit.IsInCity)
            //                        {
            //                            g.DrawImage(
            //                                Images.CreateUnitBitmap(unit, unitsHere.Count() > 1, zoom),
            //                                32 * coordsOffsets[0] + 64,
            //                                16 * coordsOffsets[1] + 32);
            //                        }
            //                    }
            //                }
            //            }

            //            //Cities
            //            City city = Game.Cities.Find(c => c.X == xCiv2 && c.Y == yCiv2);
            //            if (city != null)
            //            {
            //                g.DrawImage(
            //                    Images.CreateCityBitmap(city, true, 8),
            //                    32 * coordsOffsets[0] + 64,
            //                    16 * coordsOffsets[1] + 32);
            //            }
            //        }
            //    }

            //    //City names
            //    //Add additional coords for drawing city names
            //    coordsOffsetsToBeDrawn.Add(new int[] { -3, -5 });
            //    coordsOffsetsToBeDrawn.Add(new int[] { -1, -5 });
            //    coordsOffsetsToBeDrawn.Add(new int[] { 1, -5 });
            //    coordsOffsetsToBeDrawn.Add(new int[] { 3, -5 });
            //    coordsOffsetsToBeDrawn.Add(new int[] { -4, -2 });
            //    coordsOffsetsToBeDrawn.Add(new int[] { 4, -2 });
            //    coordsOffsetsToBeDrawn.Add(new int[] { -4, 0 });
            //    coordsOffsetsToBeDrawn.Add(new int[] { 4, 0 });
            //    coordsOffsetsToBeDrawn.Add(new int[] { -4, 2 });
            //    coordsOffsetsToBeDrawn.Add(new int[] { 4, 2 });
            //    foreach (int[] coordsOffsets in coordsOffsetsToBeDrawn)
            //    {
            //        //change coords of central offset
            //        int x = centralCoords[0] + coordsOffsets[0];
            //        int y = centralCoords[1] + coordsOffsets[1];

            //        if (x >= 0 && y >= 0 && x < 2 * Data.MapXdim && y < Data.MapYdim)    //make sure you're not drawing tiles outside map bounds
            //        {
            //            City city = Game.Cities.Find(c => c.X == x && c.Y == y);
            //            if (city != null)
            //            {
            //                Bitmap cityNameBitmap = Images.CreateCityNameBitmap(city, 8);
            //                g.DrawImage(
            //                    cityNameBitmap,
            //                    32 * coordsOffsets[0] + 64 + 32 - cityNameBitmap.Width / 2,
            //                    16 * coordsOffsets[1] + 3 * 8 + 48);
            //            }
            //        }
            //    }
            //}
        }
        
        public static Bitmap DrawTerrain(ITerrain tile, int col, int row)
        {
            // Define a bitmap for drawing
            Bitmap tilePic = new Bitmap(64, 32);

            // Draw tile
            using (Graphics graphics = Graphics.FromImage(tilePic))
            {
                switch (tile.Type)
                {
                    case TerrainType.Desert: graphics.DrawImage(Desert[0], 0, 0); break;
                    case TerrainType.Forest: graphics.DrawImage(ForestBase[0], 0, 0); break;
                    case TerrainType.Glacier: graphics.DrawImage(Glacier[0], 0, 0); break;
                    case TerrainType.Grassland: graphics.DrawImage(Grassland[0], 0, 0); break;
                    case TerrainType.Hills: graphics.DrawImage(HillsBase[0], 0, 0); break;
                    case TerrainType.Jungle: graphics.DrawImage(Jungle[0], 0, 0); break;
                    case TerrainType.Mountains: graphics.DrawImage(MtnsBase[0], 0, 0); break;
                    case TerrainType.Ocean: graphics.DrawImage(Ocean[0], 0, 0); break;
                    case TerrainType.Plains: graphics.DrawImage(Plains[0], 0, 0); break;
                    case TerrainType.Swamp: graphics.DrawImage(Swamp[0], 0, 0); break;
                    case TerrainType.Tundra: graphics.DrawImage(Tundra[0], 0, 0); break;
                    default: throw new ArgumentOutOfRangeException();
                }

                // Dither
                int col_ = 2 * col + row % 2;   // to civ2-style
                // First check if you are on map edge. If not, look at type of terrain in all 4 directions.
                TerrainType[,] tiletype = new TerrainType[2, 2];
                if ((col_ != 0) && (row != 0)) tiletype[0, 0] = Map.Tile[((col_ - 1) - (row - 1) % 2) / 2, row - 1].Type;
                if ((col_ != 2 * Map.Xdim - 1) && (row != 0)) tiletype[1, 0] = Map.Tile[((col_ + 1) - (row - 1) % 2) / 2, row - 1].Type;
                if ((col_ != 0) && (row != Map.Ydim - 1)) tiletype[0, 1] = Map.Tile[((col_ - 1) - (row + 1) % 2) / 2, row + 1].Type;
                if ((col_ != 2 * Map.Xdim - 1) && (row != Map.Ydim - 1)) tiletype[1, 1] = Map.Tile[((col_ + 1) - (row + 1) % 2) / 2, row + 1].Type;
                // Implement dither on 4 locations in square
                for (int tileX = 0; tileX < 2; tileX++)    // for 4 directions
                    for (int tileY = 0; tileY < 2; tileY++)
                        switch (tiletype[tileX, tileY])
                        {
                            case TerrainType.Desert: graphics.DrawImage(DitherDesert[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Plains: graphics.DrawImage(DitherPlains[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Grassland: graphics.DrawImage(DitherGrassland[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Forest: graphics.DrawImage(DitherForest[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Hills: graphics.DrawImage(DitherHills[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Mountains: graphics.DrawImage(DitherMountains[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Tundra: graphics.DrawImage(DitherTundra[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Glacier: graphics.DrawImage(DitherGlacier[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Swamp: graphics.DrawImage(DitherSwamp[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Jungle: graphics.DrawImage(DitherJungle[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Ocean: graphics.DrawImage(DitherGrassland[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            default: break;
                        }

                // Draw coast & river mouth
                if (Map.Tile[col, row].Type == TerrainType.Ocean)
                {
                    bool[] land = IsLandPresent(col, row);   // Determine if land is present in 8 directions

                    // Draw coast & river mouth tiles
                    // NW+N+NE tiles
                    if (!land[7] && !land[0] && !land[1]) graphics.DrawImage(Coast[0, 0], 16, 0);
                    if (land[7] && !land[0] && !land[1]) graphics.DrawImage(Coast[1, 0], 16, 0);
                    if (!land[7] && land[0] && !land[1]) graphics.DrawImage(Coast[2, 0], 16, 0);
                    if (land[7] && land[0] && !land[1]) graphics.DrawImage(Coast[3, 0], 16, 0);
                    if (!land[7] && !land[0] && land[1]) graphics.DrawImage(Coast[4, 0], 16, 0);
                    if (land[7] && !land[0] && land[1]) graphics.DrawImage(Coast[5, 0], 16, 0);
                    if (!land[7] && land[0] && land[1]) graphics.DrawImage(Coast[6, 0], 16, 0);
                    if (land[7] && land[0] && land[1]) graphics.DrawImage(Coast[7, 0], 16, 0);
                    // SW+S+SE tiles
                    if (!land[3] && !land[4] && !land[5]) graphics.DrawImage(Coast[0, 1], 16, 16);
                    if (land[3] && !land[4] && !land[5]) graphics.DrawImage(Coast[1, 1], 16, 16);
                    if (!land[3] && land[4] && !land[5]) graphics.DrawImage(Coast[2, 1], 16, 16);
                    if (land[3] && land[4] && !land[5]) graphics.DrawImage(Coast[3, 1], 16, 16);
                    if (!land[3] && !land[4] && land[5]) graphics.DrawImage(Coast[4, 1], 16, 16);
                    if (land[3] && !land[4] && land[5]) graphics.DrawImage(Coast[5, 1], 16, 16);
                    if (!land[3] && land[4] && land[5]) graphics.DrawImage(Coast[6, 1], 16, 16);
                    if (land[3] && land[4] && land[5]) graphics.DrawImage(Coast[7, 1], 16, 16);
                    // SW+W+NW tiles
                    if (!land[5] && !land[6] && !land[7]) graphics.DrawImage(Coast[0, 2], 0, 8);
                    if (land[5] && !land[6] && !land[7]) graphics.DrawImage(Coast[1, 2], 0, 8);
                    if (!land[5] && land[6] && !land[7]) graphics.DrawImage(Coast[2, 2], 0, 8);
                    if (land[5] && land[6] && !land[7]) graphics.DrawImage(Coast[3, 2], 0, 8);
                    if (!land[5] && !land[6] && land[7]) graphics.DrawImage(Coast[4, 2], 0, 8);
                    if (land[5] && !land[6] && land[7]) graphics.DrawImage(Coast[5, 2], 0, 8);
                    if (!land[5] && land[6] && land[7]) graphics.DrawImage(Coast[6, 2], 0, 8);
                    if (land[5] && land[6] && land[7]) graphics.DrawImage(Coast[7, 2], 0, 8);
                    // NE+E+SE tiles
                    if (!land[1] && !land[2] && !land[3]) graphics.DrawImage(Coast[0, 3], 32, 8);
                    if (land[1] && !land[2] && !land[3]) graphics.DrawImage(Coast[1, 3], 32, 8);
                    if (!land[1] && land[2] && !land[3]) graphics.DrawImage(Coast[2, 3], 32, 8);
                    if (land[1] && land[2] && !land[3]) graphics.DrawImage(Coast[3, 3], 32, 8);
                    if (!land[1] && !land[2] && land[3]) graphics.DrawImage(Coast[4, 3], 32, 8);
                    if (land[1] && !land[2] && land[3]) graphics.DrawImage(Coast[5, 3], 32, 8);
                    if (!land[1] && land[2] && land[3]) graphics.DrawImage(Coast[6, 3], 32, 8);
                    if (land[1] && land[2] && land[3]) graphics.DrawImage(Coast[7, 3], 32, 8);

                    // River mouth
                    // If river is next to ocean, draw river mouth on this tile.
                    col_ = 2 * col + row % 2; // rewrite indexes in Civ2-style
                    int Xdim = 2 * Map.Xdim;    // X=50 in markted as X=100 in Civ2
                    int Ydim = Map.Ydim;        // no need for such correction for Y
                    if (col_ + 1 < Xdim && row - 1 >= 0)    // NE there is no edge of map
                    {
                        if (land[1] && Map.Tile[((col_ + 1) - (row - 1) % 2) / 2, row - 1].River) graphics.DrawImage(RiverMouth[0], 0, 0);
                    }
                    if (col_ + 1 < Xdim && row + 1 < Ydim)    // SE there is no edge of map
                    {
                        if (land[3] && Map.Tile[((col_ + 1) - (row + 1) % 2) / 2, row + 1].River) graphics.DrawImage(RiverMouth[1], 0, 0);
                    }
                    if (col_ - 1 >= 0 && row + 1 < Ydim)    // SW there is no edge of map
                    {
                        if (land[5] && Map.Tile[((col_ - 1) - (row + 1) % 2) / 2, row + 1].River) graphics.DrawImage(RiverMouth[2], 0, 0);
                    }
                    if (col_ - 1 >= 0 && row - 1 >= 0)    // NW there is no edge of map
                    {
                        if (land[7] && Map.Tile[((col_ - 1) - (row - 1) % 2) / 2, row - 1].River) graphics.DrawImage(RiverMouth[3], 0, 0);
                    }
                }

                // Draw forests
                if (Map.Tile[col, row].Type == TerrainType.Forest)
                {
                    bool[] isForestAround = IsForestAround(col, row);

                    // Draw forest tiles
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { false, false, false, false })) graphics.DrawImage(Forest[0], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { true, false, false, false })) graphics.DrawImage(Forest[1], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { false, true, false, false })) graphics.DrawImage(Forest[2], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { true, true, false, false })) graphics.DrawImage(Forest[3], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { false, false, true, false })) graphics.DrawImage(Forest[4], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { true, false, true, false })) graphics.DrawImage(Forest[5], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { false, true, true, false })) graphics.DrawImage(Forest[6], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { true, true, true, false })) graphics.DrawImage(Forest[7], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { false, false, false, true })) graphics.DrawImage(Forest[8], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { true, false, false, true })) graphics.DrawImage(Forest[9], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { false, true, false, true })) graphics.DrawImage(Forest[10], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { true, true, false, true })) graphics.DrawImage(Forest[11], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { false, false, true, true })) graphics.DrawImage(Forest[12], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { true, false, true, true })) graphics.DrawImage(Forest[13], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { false, true, true, true })) graphics.DrawImage(Forest[14], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { true, true, true, true })) graphics.DrawImage(Forest[15], 0, 0);
                }

                // Draw mountains
                // TODO: correct drawing mountains - IF SHIELD IS AT MOUNTAIN IT SHOULD BE Mountains[2] and Mountains[3] !!!
                if (Map.Tile[col, row].Type == TerrainType.Mountains)
                {
                    bool[] isMountAround = IsMountAround(col, row);

                    // Draw mountain tiles
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { false, false, false, false })) graphics.DrawImage(Mountains[0], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { true, false, false, false })) graphics.DrawImage(Mountains[1], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { false, true, false, false })) graphics.DrawImage(Mountains[2], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { true, true, false, false })) graphics.DrawImage(Mountains[3], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { false, false, true, false })) graphics.DrawImage(Mountains[4], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { true, false, true, false })) graphics.DrawImage(Mountains[5], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { false, true, true, false })) graphics.DrawImage(Mountains[6], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { true, true, true, false })) graphics.DrawImage(Mountains[7], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { false, false, false, true })) graphics.DrawImage(Mountains[8], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { true, false, false, true })) graphics.DrawImage(Mountains[9], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { false, true, false, true })) graphics.DrawImage(Mountains[10], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { true, true, false, true })) graphics.DrawImage(Mountains[11], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { false, false, true, true })) graphics.DrawImage(Mountains[12], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { true, false, true, true })) graphics.DrawImage(Mountains[13], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { false, true, true, true })) graphics.DrawImage(Mountains[14], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { true, true, true, true })) graphics.DrawImage(Mountains[15], 0, 0);
                }

                // Draw hills
                if (Map.Tile[col, row].Type == TerrainType.Hills)
                {
                    bool[] isHillAround = IsHillAround(col, row);

                    // Draw hill tiles
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { false, false, false, false })) graphics.DrawImage(Hills[0], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { true, false, false, false })) graphics.DrawImage(Hills[1], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { false, true, false, false })) graphics.DrawImage(Hills[2], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { true, true, false, false })) graphics.DrawImage(Hills[3], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { false, false, true, false })) graphics.DrawImage(Hills[4], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { true, false, true, false })) graphics.DrawImage(Hills[5], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { false, true, true, false })) graphics.DrawImage(Hills[6], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { true, true, true, false })) graphics.DrawImage(Hills[7], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { false, false, false, true })) graphics.DrawImage(Hills[8], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { true, false, false, true })) graphics.DrawImage(Hills[9], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { false, true, false, true })) graphics.DrawImage(Hills[10], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { true, true, false, true })) graphics.DrawImage(Hills[11], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { false, false, true, true })) graphics.DrawImage(Hills[12], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { true, false, true, true })) graphics.DrawImage(Hills[13], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { false, true, true, true })) graphics.DrawImage(Hills[14], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { true, true, true, true })) graphics.DrawImage(Hills[15], 0, 0);
                }

                // Draw rivers
                if (Map.Tile[col, row].River)
                {
                    bool[] isRiverAround = IsRiverAround(col, row);

                    // Draw river tiles
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { false, false, false, false })) graphics.DrawImage(River[0], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { true, false, false, false })) graphics.DrawImage(River[1], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { false, true, false, false })) graphics.DrawImage(River[2], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { true, true, false, false })) graphics.DrawImage(River[3], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { false, false, true, false })) graphics.DrawImage(River[4], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { true, false, true, false })) graphics.DrawImage(River[5], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { false, true, true, false })) graphics.DrawImage(River[6], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { true, true, true, false })) graphics.DrawImage(River[7], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { false, false, false, true })) graphics.DrawImage(River[8], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { true, false, false, true })) graphics.DrawImage(River[9], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { false, true, false, true })) graphics.DrawImage(River[10], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { true, true, false, true })) graphics.DrawImage(River[11], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { false, false, true, true })) graphics.DrawImage(River[12], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { true, false, true, true })) graphics.DrawImage(River[13], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { false, true, true, true })) graphics.DrawImage(River[14], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { true, true, true, true })) graphics.DrawImage(River[15], 0, 0);
                }

                // Draw special resources if they exist
                if (Map.Tile[col, row].SpecType != null)
                {
                    switch (Map.Tile[col, row].SpecType)
                    {
                        case SpecialType.Oasis: graphics.DrawImage(Desert[2], 0, 0); break;
                        case SpecialType.DesertOil: graphics.DrawImage(Desert[3], 0, 0); break;
                        case SpecialType.Buffalo: graphics.DrawImage(Plains[2], 0, 0); break;
                        case SpecialType.Wheat: graphics.DrawImage(Plains[3], 0, 0); break;
                        case SpecialType.GrasslandShield: graphics.DrawImage(Shield, 0, 0); break;
                        case SpecialType.Pheasant: graphics.DrawImage(ForestBase[2], 0, 0); break;
                        case SpecialType.Silk: graphics.DrawImage(ForestBase[3], 0, 0); break;
                        case SpecialType.Coal: graphics.DrawImage(HillsBase[2], 0, 0); break;
                        case SpecialType.Wine: graphics.DrawImage(HillsBase[3], 0, 0); break;
                        case SpecialType.Gold: graphics.DrawImage(MtnsBase[2], 0, 0); break;
                        case SpecialType.Iron: graphics.DrawImage(MtnsBase[3], 0, 0); break;
                        case SpecialType.Game: graphics.DrawImage(Tundra[2], 0, 0); break;
                        case SpecialType.Furs: graphics.DrawImage(Tundra[3], 0, 0); break;
                        case SpecialType.Ivory: graphics.DrawImage(Glacier[2], 0, 0); break;
                        case SpecialType.GlacierOil: graphics.DrawImage(Glacier[3], 0, 0); break;
                        case SpecialType.Peat: graphics.DrawImage(Swamp[2], 0, 0); break;
                        case SpecialType.Spice: graphics.DrawImage(Swamp[3], 0, 0); break;
                        case SpecialType.Gems: graphics.DrawImage(Jungle[2], 0, 0); break;
                        case SpecialType.Fruit: graphics.DrawImage(Jungle[3], 0, 0); break;
                        case SpecialType.Fish: graphics.DrawImage(Ocean[2], 0, 0); break;
                        case SpecialType.Whales: graphics.DrawImage(Ocean[3], 0, 0); break;
                        default: throw new ArgumentOutOfRangeException();
                    }
                    
                }

                // Roads (cites also act as road tiles)
                if (Map.Tile[col, row].Road || Map.Tile[col, row].CityPresent)
                {
                    bool[] isRoadAround = IsisRoadAround(col, row);

                    // Draw roads
                    if (isRoadAround[0]) graphics.DrawImage(Road[8], 0, 0);  // to N
                    if (isRoadAround[1]) graphics.DrawImage(Road[1], 0, 0);  // to NE
                    if (isRoadAround[2]) graphics.DrawImage(Road[2], 0, 0);  // to E
                    if (isRoadAround[3]) graphics.DrawImage(Road[3], 0, 0);  // to SE
                    if (isRoadAround[4]) graphics.DrawImage(Road[4], 0, 0);  // to S
                    if (isRoadAround[5]) graphics.DrawImage(Road[5], 0, 0);  // to SW
                    if (isRoadAround[6]) graphics.DrawImage(Road[6], 0, 0);  // to W
                    if (isRoadAround[7]) graphics.DrawImage(Road[7], 0, 0);  // to NW
                    if (Enumerable.SequenceEqual(isRoadAround, new bool[8] { false, false, false, false, false, false, false, false })) graphics.DrawImage(Road[0], 0, 0);    // no road around
                }

                // TODO: make railroad drawing logic
                // Railroads (cites also act as railroad tiles)
                //if (Map.Tile[i, j].Railroad || Map.Tile[i, j].CityPresent)
                //{
                //    bool[] isRailroadAround = IsRailroadAround(i, j);
                //
                //    // Draw railroads
                //    if (isRailroadAround[0]) { graphics.DrawImage(Images.Railroad[8], 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }  // to N
                //    if (isRailroadAround[1]) { graphics.DrawImage(Images.Railroad[1], 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }  // to NE
                //    if (isRailroadAround[2]) { graphics.DrawImage(Images.Railroad[2], 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }  // to E
                //    if (isRailroadAround[3]) { graphics.DrawImage(Images.Railroad[3], 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }  // to SE
                //    if (isRailroadAround[4]) { graphics.DrawImage(Images.Railroad[4], 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }  // to S
                //    if (isRailroadAround[5]) { graphics.DrawImage(Images.Railroad[5], 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }  // to SW
                //    if (isRailroadAround[6]) { graphics.DrawImage(Images.Railroad[6], 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }  // to W
                //    if (isRailroadAround[7]) { graphics.DrawImage(Images.Railroad[7], 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }  // to NW
                //    if (Enumerable.SequenceEqual(isRailroadAround, new bool[8] { false, false, false, false, false, false, false, false })) 
                //      graphics.DrawImage(Images.Railroad[0], 64 * i + 32 * (j % 2) + 1, 16 * j + 1);  // no railroad around
                //}

                // Irrigation
                if (Map.Tile[col, row].Irrigation) graphics.DrawImage(Irrigation, 0, 0);

                // Farmland
                if (Map.Tile[col, row].Farmland) graphics.DrawImage(Farmland, 0, 0);

                // Mining
                if (Map.Tile[col, row].Mining) graphics.DrawImage(Mining, 0, 0);

                // Pollution
                if (Map.Tile[col, row].Pollution) graphics.DrawImage(Pollution, 0, 0);

                // Fortress
                if (Map.Tile[col, row].Fortress) graphics.DrawImage(Fortress, 0, 0);

                // Airbase
                if (Map.Tile[col, row].Airbase) graphics.DrawImage(Airbase, 0, 0);

            }

            return tilePic;
        }

        private static bool[] IsLandPresent(int i, int j)
        {
            // In start we presume all surrounding tiles are water (land=true, water=false). Index=0 is North, follows in clockwise direction.
            bool[] land = new bool[8] { false, false, false, false, false, false, false, false };

            // Rewrite indexes in Civ2-style
            int i_ = 2 * i + j % 2;
            int Xdim = 2 * Map.Xdim;    // X=50 in markted as X=100 in Civ2
            int Ydim = Map.Ydim;        // no need for such correction for Y

            // Observe in all directions if land is present next to ocean
            // N:
            if (j - 2 < 0) land[0] = true;   // if N tile is out of map (black tile), we presume it is land
            else if (Map.Tile[(i_ - (j - 2) % 2) / 2, j - 2].Type != TerrainType.Ocean) land[0] = true;
            // NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) land[1] = true;  // NE is black tile
            else if (Map.Tile[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type != TerrainType.Ocean) land[1] = true;    // if it is not ocean, it is land
            // E:
            if (i_ + 2 >= Xdim) land[2] = true;  // E is black tile
            else if (Map.Tile[((i_ + 2) - j % 2) / 2, j].Type != TerrainType.Ocean) land[2] = true;
            // SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) land[3] = true;  // SE is black tile
            else if (Map.Tile[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type != TerrainType.Ocean) land[3] = true;
            // S:
            if (j + 2 >= Ydim) land[4] = true;   // S is black tile
            else if (Map.Tile[(i_ - (j + 2) % 2) / 2, j + 2].Type != TerrainType.Ocean) land[4] = true;
            // SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) land[5] = true;  // SW is black tile
            else if (Map.Tile[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type != TerrainType.Ocean) land[5] = true;
            // W:
            if (i_ - 2 < 0) land[6] = true;  // W is black tile
            else if (Map.Tile[((i_ - 2) - j % 2) / 2, j].Type != TerrainType.Ocean) land[6] = true;
            // NW:
            if (i_ - 1 < 0 || j - 1 < 0) land[7] = true;  // NW is black tile
            else if (Map.Tile[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type != TerrainType.Ocean) land[7] = true;

            return land;
        }

        private static bool[] IsForestAround(int i, int j)
        {
            // In start we presume all surrounding tiles are not forest (forest=true, no forest=false). Index=0 is NE, follows in clockwise direction.
            bool[] isForestAround = new bool[4] { false, false, false, false };

            // Rewrite indexes in Civ2-style
            int i_ = 2 * i + j % 2;
            int Xdim = 2 * Map.Xdim;    // X=50 in markted as X=100 in Civ2
            int Ydim = Map.Ydim;        // no need for such correction for Y

            // Observe in all directions if forest is present
            // NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) isForestAround[0] = false;  // NE is black tile (we presume no forest is there)
            else if (Map.Tile[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Forest) isForestAround[0] = true;
            // SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) isForestAround[1] = false;  // SE is black tile
            else if (Map.Tile[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Forest) isForestAround[1] = true;
            // SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) isForestAround[2] = false;  // SW is black tile
            else if (Map.Tile[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Forest) isForestAround[2] = true;
            // NW:
            if (i_ - 1 < 0 || j - 1 < 0) isForestAround[3] = false;  // NW is black tile
            else if (Map.Tile[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Forest) isForestAround[3] = true;

            return isForestAround;
        }

        private static bool[] IsMountAround(int i, int j)
        {
            // In start we presume all surrounding tiles are not mountains (mount=true, no mount=false). Index=0 is NE, follows in clockwise direction.
            bool[] isMountAround = new bool[4] { false, false, false, false };

            // Rewrite indexes in Civ2-style
            int i_ = 2 * i + j % 2;
            int Xdim = 2 * Map.Xdim;    // X=50 in markted as X=100 in Civ2
            int Ydim = Map.Ydim;        // no need for such correction for Y

            // Observe in all directions if mountain is present
            // NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) isMountAround[0] = false;  // NE is black tile (we presume no mountain is there)
            else if (Map.Tile[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Mountains) isMountAround[0] = true;
            // SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) isMountAround[1] = false;  // SE is black tile
            else if (Map.Tile[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Mountains) isMountAround[1] = true;
            // SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) isMountAround[2] = false;  // SW is black tile
            else if (Map.Tile[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Mountains) isMountAround[2] = true;
            // NW:
            if (i_ - 1 < 0 || j - 1 < 0) isMountAround[3] = false;  // NW is black tile
            else if (Map.Tile[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Mountains) isMountAround[3] = true;

            return isMountAround;
        }

        private static bool[] IsHillAround(int i, int j)
        {
            // In start we presume all surrounding tiles are not hills (hill=true, no hill=false). Index=0 is NE, follows in clockwise direction.
            bool[] isHillAround = new bool[4] { false, false, false, false };

            // Rewrite indexes in Civ2-style
            int i_ = 2 * i + j % 2;
            int Xdim = 2 * Map.Xdim;    // X=50 in markted as X=100 in Civ2
            int Ydim = Map.Ydim;        // no need for such correction for Y

            // observe in all directions if hill is present
            // NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) isHillAround[0] = false;  // NE is black tile (we presume no hill is there)
            else if (Map.Tile[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Hills) isHillAround[0] = true;
            // SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) isHillAround[1] = false;  // SE is black tile
            else if (Map.Tile[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Hills) isHillAround[1] = true;
            // SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) isHillAround[2] = false;  // SW is black tile
            else if (Map.Tile[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Hills) isHillAround[2] = true;
            // NW:
            if (i_ - 1 < 0 || j - 1 < 0) isHillAround[3] = false;  // NW is black tile
            else if (Map.Tile[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Hills) isHillAround[3] = true;

            return isHillAround;
        }

        private static bool[] IsRiverAround(int i, int j)
        {
            // In start we presume all surrounding tiles are not rivers (river=true, no river=false). Index=0 is NE, follows in clockwise direction.
            bool[] isRiverAround = new bool[4] { false, false, false, false };

            // Rewrite indexes in Civ2-style
            int i_ = 2 * i + j % 2;
            int Xdim = 2 * Map.Xdim;    // X=50 in markted as X=100 in Civ2
            int Ydim = Map.Ydim;        // no need for such correction for Y

            // Observe in all directions if river is present
            // NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) isRiverAround[0] = false;  // NE is black tile (we presume no river is there)
            else if (Map.Tile[((i_ + 1) - (j - 1) % 2) / 2, j - 1].River || Map.Tile[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Ocean) isRiverAround[0] = true;
            // SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) isRiverAround[1] = false;  // SE is black tile
            else if (Map.Tile[((i_ + 1) - (j + 1) % 2) / 2, j + 1].River || Map.Tile[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Ocean) isRiverAround[1] = true;
            // SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) isRiverAround[2] = false;  // SW is black tile
            else if (Map.Tile[((i_ - 1) - (j + 1) % 2) / 2, j + 1].River || Map.Tile[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Ocean) isRiverAround[2] = true;
            // NW:
            if (i_ - 1 < 0 || j - 1 < 0) isRiverAround[3] = false;  // NW is black tile
            else if (Map.Tile[((i_ - 1) - (j - 1) % 2) / 2, j - 1].River || Map.Tile[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Ocean) isRiverAround[3] = true;

            return isRiverAround;
        }

        private static bool[] IsisRoadAround(int i, int j)
        {
            // In start we presume all surrounding tiles do not have roads. Index=0 is NE, follows in clockwise direction.
            bool[] isRoadAround = new bool[8] { false, false, false, false, false, false, false, false };

            // Rewrite indexes in Civ2-style
            int i_ = 2 * i + j % 2;
            int Xdim = 2 * Map.Xdim;    // X=50 in markted as X=100 in Civ2
            int Ydim = Map.Ydim;        // no need for such correction for Y

            // Observe in all directions if road or city is present next to tile
            // N:
            if (j - 2 < 0) isRoadAround[0] = false;   // if N tile is out of map (black tile), we presume there is no road
            else if (Map.Tile[(i_ - (j - 2) % 2) / 2, j - 2].Road || Map.Tile[(i_ - (j - 2) % 2) / 2, j - 2].CityPresent) isRoadAround[0] = true;
            // NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) isRoadAround[1] = false;  // NE is black tile
            else if (Map.Tile[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Road || Map.Tile[((i_ + 1) - (j - 1) % 2) / 2, j - 1].CityPresent) isRoadAround[1] = true;
            // E:
            if (i_ + 2 >= Xdim) isRoadAround[2] = false;  // E is black tile
            else if (Map.Tile[((i_ + 2) - j % 2) / 2, j].Road || Map.Tile[((i_ + 2) - j % 2) / 2, j].CityPresent) isRoadAround[2] = true;
            // SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) isRoadAround[3] = false;  // SE is black tile
            else if (Map.Tile[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Road || Map.Tile[((i_ + 1) - (j + 1) % 2) / 2, j + 1].CityPresent) isRoadAround[3] = true;
            // S:
            if (j + 2 >= Ydim) isRoadAround[4] = false;   // S is black tile
            else if (Map.Tile[(i_ - (j + 2) % 2) / 2, j + 2].Road || Map.Tile[(i_ - (j + 2) % 2) / 2, j + 2].CityPresent) isRoadAround[4] = true;
            // SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) isRoadAround[5] = false;  // SW is black tile
            else if (Map.Tile[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Road || Map.Tile[((i_ - 1) - (j + 1) % 2) / 2, j + 1].CityPresent) isRoadAround[5] = true;
            // W:
            if (i_ - 2 < 0) isRoadAround[6] = false;  // W is black tile
            else if (Map.Tile[((i_ - 2) - j % 2) / 2, j].Road || Map.Tile[((i_ - 2) - j % 2) / 2, j].CityPresent) isRoadAround[6] = true;
            // NW:
            if (i_ - 1 < 0 || j - 1 < 0) isRoadAround[7] = false;  // NW is black tile
            else if (Map.Tile[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Road || Map.Tile[((i_ - 1) - (j - 1) % 2) / 2, j - 1].CityPresent) isRoadAround[7] = true;

            return isRoadAround;
        }

        private static bool[] IsRailroadAround(int i, int j)
        {
            // In start we presume all surrounding tiles do not have railroads. Index=0 is NE, follows in clockwise direction.
            bool[] isRailroadAround = new bool[8] { false, false, false, false, false, false, false, false };

            // Rewrite indexes in Civ2-style
            int i_ = 2 * i + j % 2;
            int Xdim = 2 * Map.Xdim;    // X=50 in markted as X=100 in Civ2
            int Ydim = Map.Ydim;        // no need for such correction for Y

            // Observe in all directions if road or city is present next to tile
            // N:
            if (j - 2 < 0) isRailroadAround[0] = false;   // if N tile is out of map (black tile), we presume there is no road
            else if (Map.Tile[(i_ - (j - 2) % 2) / 2, j - 2].Railroad || Map.Tile[(i_ - (j - 2) % 2) / 2, j - 2].CityPresent) isRailroadAround[0] = true;
            // NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) isRailroadAround[1] = false;  // NE is black tile
            else if (Map.Tile[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Railroad || Map.Tile[((i_ + 1) - (j - 1) % 2) / 2, j - 1].CityPresent) isRailroadAround[1] = true;
            // E:
            if (i_ + 2 >= Xdim) isRailroadAround[2] = false;  // E is black tile
            else if (Map.Tile[((i_ + 2) - j % 2) / 2, j].Railroad || Map.Tile[((i_ + 2) - j % 2) / 2, j].CityPresent) isRailroadAround[2] = true;
            // SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) isRailroadAround[3] = false;  // SE is black tile
            else if (Map.Tile[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Railroad || Map.Tile[((i_ + 1) - (j + 1) % 2) / 2, j + 1].CityPresent) isRailroadAround[3] = true;
            // S:
            if (j + 2 >= Ydim) isRailroadAround[4] = false;   // S is black tile
            else if (Map.Tile[(i_ - (j + 2) % 2) / 2, j + 2].Railroad || Map.Tile[(i_ - (j + 2) % 2) / 2, j + 2].CityPresent) isRailroadAround[4] = true;
            // SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) isRailroadAround[5] = false;  // SW is black tile
            else if (Map.Tile[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Railroad || Map.Tile[((i_ - 1) - (j + 1) % 2) / 2, j + 1].CityPresent) isRailroadAround[5] = true;
            // W:
            if (i_ - 2 < 0) isRailroadAround[6] = false;  // W is black tile
            else if (Map.Tile[((i_ - 2) - j % 2) / 2, j].Railroad || Map.Tile[((i_ - 2) - j % 2) / 2, j].CityPresent) isRailroadAround[6] = true;
            // NW:
            if (i_ - 1 < 0 || j - 1 < 0) isRailroadAround[7] = false;  // NW is black tile
            else if (Map.Tile[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Railroad || Map.Tile[((i_ - 1) - (j - 1) % 2) / 2, j - 1].CityPresent) isRailroadAround[7] = true;

            return isRailroadAround;
        }

        public static Bitmap CreateUnitBitmap(IUnit unit, bool drawInStack, int zoom)
        {
            Bitmap square = new Bitmap(64, 48);     //define a bitmap for drawing       

            //using (Graphics graphics = Graphics.FromImage(square))
            //{
            //    StringFormat sf = new StringFormat();
            //    sf.LineAlignment = StringAlignment.Center;
            //    sf.Alignment = StringAlignment.Center;
            //    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

            //    string shieldText;
            //    switch (unit.Order)
            //    {
            //        case OrderType.Fortify:         shieldText = "F"; break;
            //        case OrderType.Fortified:       shieldText = "F"; break;
            //        case OrderType.Sleep:           shieldText = "S"; break;
            //        case OrderType.BuildFortress:   shieldText = "F"; break;
            //        case OrderType.BuildRoad:       shieldText = "R"; break;
            //        case OrderType.BuildIrrigation: shieldText = "I"; break;
            //        case OrderType.BuildMine:       shieldText = "m"; break;
            //        case OrderType.Transform:       shieldText = "O"; break;
            //        case OrderType.CleanPollution:  shieldText = "p"; break;
            //        case OrderType.BuildAirbase:    shieldText = "E"; break;
            //        case OrderType.GoTo:            shieldText = "G"; break;
            //        case OrderType.NoOrders:        shieldText = "-"; break;
            //        default:                        shieldText = "-"; break;
            //    }

            //    //Draw unit shields. First determine if the shield is on the left or right side
            //    int firstShieldXLoc = unitShieldLocation[(int)unit.Type, 0];
            //    int secondShieldXLoc = firstShieldXLoc;
            //    int secondShieldBorderXLoc;
            //    int borderShieldOffset;
            //    if (firstShieldXLoc < 32) 
            //    {
            //        borderShieldOffset = -1;
            //        secondShieldXLoc -= 4;
            //        secondShieldBorderXLoc = secondShieldXLoc - 1; 
            //    }
            //    else 
            //    {
            //        borderShieldOffset = 1;
            //        secondShieldXLoc += 4;
            //        secondShieldBorderXLoc = secondShieldXLoc + 1; 
            //    }

            //    //Determine hitpoints bar size
            //    int hitpointsBarX = (int)Math.Floor((float)unit.HitPoints * 12 / unit.MaxHitPoints);
            //    Color hitpointsColor;
            //    if (hitpointsBarX <= 3) 
            //        hitpointsColor = Color.FromArgb(243, 0, 0);
            //    else if (hitpointsBarX >= 4 && hitpointsBarX <= 8) 
            //        hitpointsColor = Color.FromArgb(255, 223, 79);
            //    else 
            //        hitpointsColor = Color.FromArgb(87, 171, 39);

            //    //Draw shadow for unit in stack
            //    if (drawInStack)    //draw dark shield if unit is stacked on top of others
            //    {
            //        graphics.DrawImage(UnitShieldShadow, secondShieldBorderXLoc, unitShieldLocation[(int)unit.Type, 1]); //shield shadow
            //        graphics.DrawImage(NoBorderUnitShield[unit.CivId], secondShieldXLoc, unitShieldLocation[(int)unit.Type, 1]);   //dark shield
            //    }

            //    //shield shadow
            //    graphics.DrawImage(UnitShieldShadow, unitShieldLocation[(int)unit.Type, 0] + borderShieldOffset, unitShieldLocation[(int)unit.Type, 1] - borderShieldOffset);

            //    //main shield
            //    graphics.DrawImage(UnitShield[unit.CivId], unitShieldLocation[(int)unit.Type, 0], unitShieldLocation[(int)unit.Type, 1]);

            //    //Draw black background for hitpoints bar
            //    graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(unitShieldLocation[(int)unit.Type, 0], unitShieldLocation[(int)unit.Type, 1] + 2, 12, 3));

            //    //Draw hitpoints bar
            //    graphics.FillRectangle(new SolidBrush(hitpointsColor), new Rectangle(unitShieldLocation[(int)unit.Type, 0], unitShieldLocation[(int)unit.Type, 1] + 2, hitpointsBarX, 3));

            //    //Action on shield
            //    graphics.DrawString(shieldText, new Font("Arial", 6.5f), new SolidBrush(Color.Black), unitShieldLocation[(int)unit.Type, 0] + 7, unitShieldLocation[(int)unit.Type, 1] + 12, sf);

            //    if (unit.Order != OrderType.Sleep) graphics.DrawImage(Units[(int)unit.Type], 0, 0);    //draw unit
            //    else graphics.DrawImage(Units[(int)unit.Type], new Rectangle(0, 0, 64, 48), 0, 0, 64, 48, GraphicsUnit.Pixel, ModifyImage.ConvertToGray());    //draw sentry unit

            //    //draw fortification
            //    if (unit.Order == OrderType.Fortified) graphics.DrawImage(Fortified, 0, 0);

            //    sf.Dispose();
            //}

            ////Resize image if required
            //square = ModifyImage.ResizeImage(square, 8 * zoom, 6 * zoom);

            return square;
        }

        public static Bitmap CreateCityBitmap(City city, bool citySizeWindow, int zoom)
        {
            Bitmap graphic = new Bitmap(64, 48);    //define a bitmap for drawing map

            ////Determine city bitmap
            ////For everything not modern or industrial => 4 city size styles (0=sizes 1...3, 1=sizes 4...5, 2=sizes 6...7, 3=sizes >= 8)
            ////If city is capital => 3 size styles (1=sizes 1...3, 2=sizes 4...5, 3=sizes >= 6)
            //int cityStyle = Game.Civs[city.Owner].CityStyle;
            //int sizeStyle = 0;
            //if (cityStyle < 4) 
            //{
            //    if (Array.Exists(city.Improvements, element => element.Type == ImprovementType.Palace))     //palace exists
            //    { 
            //        if (city.Size <= 3) sizeStyle = 1;
            //        else if (city.Size > 3 && city.Size <= 5) sizeStyle = 2;
            //        else sizeStyle = 3; 
            //    }
            //    else 
            //    {
            //        if (city.Size <= 3) sizeStyle = 0;
            //        else if (city.Size > 3 && city.Size <= 5) sizeStyle = 1;
            //        else if (city.Size > 5 && city.Size <= 7) sizeStyle = 2;
            //        else sizeStyle = 3; 
            //    } 
            //}
            ////If city is industrial => 4 city size styles (0=sizes 1...4, 1=sizes 5...7, 2=sizes 8...10, 3=sizes >= 11)
            ////If city is capital => 3 size styles (1=sizes 1...4, 2=sizes 5...7, 3=sizes >= 8)
            //else if (cityStyle == 4) 
            //{
            //    if (Array.Exists(city.Improvements, element => element.Type == ImprovementType.Palace)) //palace exists
            //    {
            //        if (city.Size <= 4) sizeStyle = 1;
            //        else if (city.Size > 4 && city.Size <= 7) sizeStyle = 2;
            //        else sizeStyle = 3; 
            //    }
            //    else 
            //    {
            //        if (city.Size <= 4) sizeStyle = 0;
            //        else if (city.Size > 4 && city.Size <= 7) sizeStyle = 1;
            //        else if (city.Size > 7 && city.Size <= 10) sizeStyle = 2;
            //        else sizeStyle = 3; 
            //    } 
            //}
            ////If city is modern => 4 city size styles (0=sizes 1...4, 1=sizes 5...10, 2=sizes 11...18, 3=sizes >= 19)
            ////If city is capital => 3 size styles (1=sizes 1...4, 2=sizes 5...10, 3=sizes >= 11)
            //else 
            //{
            //    if (Array.Exists(city.Improvements, element => element.Type == ImprovementType.Palace)) //palace exists
            //    { 
            //        if (city.Size <= 4) sizeStyle = 1;
            //        else if (city.Size > 4 && city.Size <= 10) sizeStyle = 2;
            //        else sizeStyle = 3; 
            //    }
            //    else 
            //    {
            //        if (city.Size <= 4) sizeStyle = 0;
            //        else if (city.Size > 4 && city.Size <= 10) sizeStyle = 1;
            //        else if (city.Size > 10 && city.Size <= 18) sizeStyle = 2;
            //        else sizeStyle = 3; 
            //    } 
            //}

            ////If no units are in the city, draw no flag
            //bool flagPresent = false;
            //if (Game.Units.Any(unit => unit.X == city.X && unit.Y == city.Y)) flagPresent = true;

            //using (Graphics graphics = Graphics.FromImage(graphic))
            //{
            //    StringFormat sf = new StringFormat();
            //    sf.LineAlignment = StringAlignment.Center;
            //    sf.Alignment = StringAlignment.Center;
            //    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

            //    if (!Array.Exists(city.Improvements, element => element.Type == ImprovementType.CityWalls))  //no city walls
            //    {
            //        graphics.DrawImage(City[cityStyle, sizeStyle], 0, 0);
            //        if (citySizeWindow) //Draw city size window
            //        {
            //            graphics.DrawRectangle(new Pen(Color.Black), citySizeWindowLoc[cityStyle, sizeStyle, 0] - 1, citySizeWindowLoc[cityStyle, sizeStyle, 1] - 1, 9, 13);  //rectangle
            //            graphics.FillRectangle(new SolidBrush(CivColors.Light[city.Owner]), citySizeWindowLoc[cityStyle, sizeStyle, 0], citySizeWindowLoc[cityStyle, sizeStyle, 1], 8, 12); //filling of rectangle
            //            graphics.DrawString(city.Size.ToString(), new Font("Times New Roman", 10.0f, FontStyle.Bold), new SolidBrush(Color.Black), citySizeWindowLoc[cityStyle, sizeStyle, 0] + 4, citySizeWindowLoc[cityStyle, sizeStyle, 1] + 6, sf);    //Size text
            //        }
            //        if (flagPresent) graphics.DrawImage(CityFlag[city.Owner], cityFlagLoc[cityStyle, sizeStyle, 0] - 3, cityFlagLoc[cityStyle, sizeStyle, 1] - 17); //draw city flag
                    
            //    }
            //    else
            //    {
            //        graphics.DrawImage(CityWall[cityStyle, sizeStyle], 0, 0);
            //        if (citySizeWindow)
            //        {
            //            graphics.DrawRectangle(new Pen(Color.Black), cityWallSizeWindowLoc[cityStyle, sizeStyle, 0] - 1, cityWallSizeWindowLoc[cityStyle, sizeStyle, 1] - 1, 9, 13); //Draw city (+Wall) size window
            //            graphics.FillRectangle(new SolidBrush(CivColors.Light[city.Owner]), cityWallSizeWindowLoc[cityStyle, sizeStyle, 0], cityWallSizeWindowLoc[cityStyle, sizeStyle, 1], 8, 12); //filling of rectangle
            //            graphics.DrawString(city.Size.ToString(), new Font("Times New Roman", 10.0f, FontStyle.Bold), new SolidBrush(Color.Black), cityWallSizeWindowLoc[cityStyle, sizeStyle, 0] + 4, cityWallSizeWindowLoc[cityStyle, sizeStyle, 1] + 6, sf);    //Size text    
            //        }
            //        if (flagPresent) graphics.DrawImage(CityFlag[city.Owner], cityWallFlagLoc[cityStyle, sizeStyle, 0] - 3, cityWallFlagLoc[cityStyle, sizeStyle, 1] - 17);    //Draw city flag                    
            //    }
            //    sf.Dispose();
            //}

            //graphic = ModifyImage.ResizeImage(graphic, 8 * zoom, 6 * zoom);

            return graphic;
        }

        public static Bitmap CreateCityNameBitmap(City city, int zoom)
        {
            //Define text characteristics for zoom levels
            int shadowOffset, fontSize;
            switch (zoom)
            {
                case 1: shadowOffset = 0; fontSize = 1; break;
                case 2: shadowOffset = 0; fontSize = 3; break;
                case 3: shadowOffset = 0; fontSize = 5; break;
                case 4: shadowOffset = 1; fontSize = 7; break;
                case 5: shadowOffset = 1; fontSize = 10; break;
                case 6: shadowOffset = 1; fontSize = 11; break;
                case 7: shadowOffset = 1; fontSize = 13; break;
                case 8: shadowOffset = 2; fontSize = 14; break;
                case 9: shadowOffset = 2; fontSize = 16; break;
                case 10: shadowOffset = 2; fontSize = 17; break;
                case 11: shadowOffset = 2; fontSize = 19; break;
                case 12: shadowOffset = 2; fontSize = 21; break;
                case 13: shadowOffset = 2; fontSize = 24; break;
                case 14: shadowOffset = 2; fontSize = 25; break;
                case 15: shadowOffset = 2; fontSize = 26; break;
                case 16: shadowOffset = 2; fontSize = 28; break;
                default: shadowOffset = 2; fontSize = 14; break;
            }

            //Draw
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

        public static List<Bitmap> CreateMapAnimation(AnimationType animation)
        {
            List<Bitmap> _bitmapList = new List<Bitmap>();

            //switch (animation)
            //{
            //    case AnimationType.UnitWaiting:
            //        {
            //            for (int i = 0; i < 2; i++)
            //            {
            //                Bitmap _bitmap = new Bitmap(64, 32);
            //                using (Graphics g = Graphics.FromImage(_bitmap))
            //                {
            //                    if (MapPanel.ViewPiecesMode)
            //                    {

            //                    }
            //                    else    //unit is active
            //                    {
            //                        int x = Game.Instance.ActiveUnit.X;
            //                        int y = Game.Instance.ActiveUnit.Y;
            //                        g.DrawImage(Game.CivsMap[Game.Instance.ActiveCiv.Id], new Rectangle(32 * x, 16 * y, 64, 48));
            //                        if (i == 1) g.DrawImage(Game.Instance.ActiveUnit.GraphicMapPanel, 0, 16);
            //                    }
            //                }
            //                _bitmapList.Add(_bitmap);
            //            }
            //            break;
            //        }
            //}

            return _bitmapList;
        }


    }
}
 