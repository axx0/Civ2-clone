using ExtensionMethods;
using civ2.Enums;
using civ2.Forms;
using civ2.Units;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace civ2.Bitmaps
{
    public partial class Images
    {
        public static Bitmap CityWallpaper, 
                             Irrigation, Farmland, Mining, Pollution, Fortified, Fortress, Airbase, AirbasePlane, 
                             Shield, ViewPiece, WallpaperMapForm, WallpaperStatusForm, UnitShieldShadow, GridLines, GridLinesVisible, Dither, Blank, DitherBase, 
                             SellIcon, SellIconLarge, CitymapFoodLarge, CitymapFoodLargeBigger, CitymapHungerLarge, CitymapHungerLargeBigger, CitymapFoodSmall, CitymapFoodSmallBigger, 
                             CitymapShieldLarge, CitymapShieldLargeBigger, CitymapShieldSmall, CitymapShieldSmallBigger, CitymapTradeLarge, CitymapTradeLargeBigger, CitymapTradeSmall, 
                             CitymapTradeSmallBigger, CitymapShortageLargeBigger, CitymapShortageLarge, CitymapCorruptionLarge, CitymapCorruptionLargeBigger, CitymapSupportLarge, 
                             CitymapSupportLargeBigger, CitymapLuxLarge, CitymapLuxLargeBigger, CitymapTaxLarge, CitymapTaxLargeBigger, CitymapSciLarge, CitymapSciLargeBigger, NextCity, 
                             NextCityLarge, PrevCity, PrevCityLarge, ZoomIN, ZoomOUT;
        public static Bitmap[] Desert, Plains, Grassland, ForestBase, HillsBase, MtnsBase, Tundra, Glacier, Swamp, Jungle, Ocean, River, Forest, Mountains, Hills,  RiverMouth, Road, 
                               Railroad, Units, UnitShield, NoBorderUnitShield, CityFlag, Improvements, ImprovementsLarge, ImprovementsSmall;
        public static Bitmap[,] Coast, City, CityWall, DitherBlank, DitherDots, DitherDesert, DitherPlains, DitherGrassland, DitherForest, DitherHills, 
                                DitherMountains, DitherTundra, DitherGlacier, DitherSwamp, DitherJungle, PeopleL, PeopleLshadow, ResearchIcons;
        public static int[,] unitShieldLocation = new int[63, 2];
        public static int[,,] cityFlagLoc, cityWallFlagLoc, citySizeWindowLoc, cityWallSizeWindowLoc;
        //public static int[,,] cityWallFlagLoc = new int[6, 4, 2];

        private static void LoadGraphicsAssetsFromFiles()
        {
            TerrainBitmapsImportFromFile();
            CitiesBitmapsImportFromFile();
            UnitsBitmapsImportFromFile();
            PeopleIconsBitmapsImportFromFile();
            IconsBitmapsImportFromFile();
            CityWallpaperBitmapImportFromFile();
            ImportDLLimages();
        }

        public static void CreateLoadGameGraphics()
        {
            //Creates bitmaps from current folder (CURRENTLY FROM RESOURCES, TODO: load files from disk)
            LoadGraphicsAssetsFromFiles();

            //Create graphic of each map tile
            for (int col = 0; col < Data.MapXdim; col++)
                for (int row = 0; row < Data.MapYdim; row++)
                    Game.TerrainTile[col, row].Graphic = TerrainBitmap(col, row);

            //Add cities+units graphics to each tile & create image of maps (what each civ sees + one entire visible)
            Game.CivsMap = new Bitmap[9];            
            for (int civ = 0; civ < 9; civ++) CreateWholeMapImage(civ);  //What each civ (index=0...7) sees, additionally (index=8) for entire revealed map
        }

        //Create image of civ's world maps
        public static void CreateWholeMapImage(int civ)
        {
            Game.CivsMap[civ] = new Bitmap(64 * Data.MapXdim + 32, 32 * Data.MapYdim + 16);

            using (Graphics g = Graphics.FromImage(Game.CivsMap[civ]))
            {
                for (int row = 0; row < Data.MapYdim; row++)
                {
                    for (int col = 0; col < Data.MapXdim; col++)
                    {
                        //Draw only if the tile is visible for each civ (index=8...whole map visible)
                        if ((civ < 8 && Game.TerrainTile[col, row].Visibility[civ]) || civ == 8)
                        {
                            //Tiles
                            g.DrawImage(
                                Game.TerrainTile[col, row].Graphic,
                                64 * col + 32 * (row % 2),
                                16 * row);

                            //Implement dithering in all 4 directions if necessary
                            if (civ != 8)
                                for (int tileX = 0; tileX < 2; tileX++)
                                    for (int tileY = 0; tileY < 2; tileY++)
                                    {
                                        int[] offset = new int[] { -1, 1};
                                        int col_ = 2 * col + (row % 2); //real->civ2 coords
                                        int colNew_ = col_ + offset[tileX];
                                        int rowNew = row + offset[tileY];
                                        int colNew = (colNew_ - rowNew % 2) / 2; //back to real coords
                                        if (colNew >= 0 && colNew < Data.MapXdim && rowNew >= 0 && rowNew < Data.MapYdim)   //don't observe outside map limits
                                            if (!Game.TerrainTile[colNew, rowNew].Visibility[civ])   //surrounding tile is not visible -> dither
                                                g.DrawImage(DitherDots[tileX, tileY],
                                                            64 * col + 32 * (row % 2) + 32 * tileX,
                                                            16 * row + 16 * tileY);
                                    }


                            //Units
                            int[] coords = Ext.XYciv2(new int[] { col, row });  //civ2 coords from real coords
                            int col2 = coords[0];
                            int row2 = coords[1];
                            List<IUnit> unitsHere = Game.Units.Where(u => u.X == col2 && u.Y == row2).ToList();
                            if (unitsHere.Any())
                            {
                                IUnit unit = unitsHere.Last();
                                int zoom = 8;
                                if (!unit.IsInCity)
                                {
                                    g.DrawImage(
                                        CreateUnitBitmap(unit, unitsHere.Count() > 1, zoom),
                                        32 * col2,
                                        16 * row2 - 16);
                                }
                            }

                            //Cities
                            City city = Game.Cities.Find(c => c.X == col2 && c.Y == row2);
                            if (city != null)
                            {
                                g.DrawImage(
                                    CreateCityBitmap(city, true, 8),
                                    32 * col2,
                                    16 * row2 - 16);
                            }
                        }                            
                    }
                }

                //City name text is drawn last
                foreach (City city in Game.Cities)
                {
                    int[] ColRow = Ext.Civ2xy(new int[] { city.X, city.Y });  //real coords from civ2 coords
                    if ((civ < 8 && Game.TerrainTile[ColRow[0], ColRow[1]].Visibility[civ]) || civ == 8)
                    {
                        Bitmap cityNameBitmap = CreateCityNameBitmap(city, 8);
                        g.DrawImage(
                            cityNameBitmap,
                            32 * city.X + 32 - cityNameBitmap.Width / 2,
                            16 * city.Y + 3 * 8);
                    }
                }
            }
        }

        //Redraw invisible to visible tiles in the map
        public static void RedrawMap(int[] centralCoords)
        {
            Bitmap mapPart = new Bitmap(3 * 64, 3 * 32 + 16);   //part which will be pasted on top of existing world map

            //Get coords of central tile & which squares are to be drawn
            List<int[]> coordsOffsetsToBeDrawn = new List<int[]>
            {
                new int[] {-2, -4},
                new int[] {0, -4},
                new int[] {2, -4},
                new int[] {-3, -3},
                new int[] {-1, -3},
                new int[] {1, -3},
                new int[] {3, -3},
                new int[] {-2, -2},
                new int[] {0, -2},
                new int[] {2, -2},
                new int[] {-3, -1},
                new int[] {-1, -1},
                new int[] {1, -1},
                new int[] {3, -1},
                new int[] {-2, 0},
                new int[] {0, 0},
                new int[] {2, 0},
                new int[] {-3, 1},
                new int[] {-1, 1},
                new int[] {1, 1},
                new int[] {3, 1},
                new int[] {-2, 2},
                new int[] {0, 2},
                new int[] {2, 2},
                new int[] {-3, 3},
                new int[] {-1, 3},
                new int[] {1, 3},
                new int[] {3, 3},
                new int[] {-2, 4},
                new int[] {0, 4},
                new int[] {2, 4}
            };

            int zoom = 8;

            using (Graphics g = Graphics.FromImage(mapPart))
            {
                g.FillRectangle(Brushes.Black, new Rectangle(0, 0, 3 * 64, 3 * 32 + 16));    //fill bitmap with black (necessary for correct drawing if image is on upper map edge)

                foreach (int[] coordsOffsets in coordsOffsetsToBeDrawn)
                {
                    //change coords of central offset
                    int xCiv2 = centralCoords[0] + coordsOffsets[0];
                    int yCiv2 = centralCoords[1] + coordsOffsets[1];
                    int xReal = (xCiv2 - yCiv2 % 2) / 2;
                    int yReal = yCiv2;

                    if (xCiv2 >= 0 && yCiv2 >= 0 && xCiv2 < 2 * Data.MapXdim && yCiv2 < Data.MapYdim)    //make sure you're not drawing tiles outside map bounds
                    {
                        //Tiles
                        int civId = Game.Instance.ActiveCiv.Id;
                        if ((civId < 8 && Game.TerrainTile[xReal, yReal].Visibility[civId]) || civId == 8)
                        {
                            g.DrawImage(
                                Images.TerrainBitmap(xReal, yReal),
                                32 * coordsOffsets[0] + 64,
                                16 * coordsOffsets[1] + 48);

                            //Implement dithering in all 4 directions if necessary
                            if (civId != 8)
                                for (int tileX = 0; tileX < 2; tileX++)
                                    for (int tileY = 0; tileY < 2; tileY++)
                                    {
                                        int[] offset = new int[] { -1, 1 };
                                        int xCiv2New = xCiv2 + offset[tileX];
                                        int yCiv2New = yCiv2 + offset[tileY];
                                        int xRealNew = (xCiv2New - yCiv2New % 2) / 2; //back to real coords
                                        int yRealNew = yCiv2New;
                                        if (xRealNew >= 0 && xRealNew < Data.MapXdim && yRealNew >= 0 && yRealNew < Data.MapYdim)   //don't observe outside map limits
                                            if (!Game.TerrainTile[xRealNew, yRealNew].Visibility[civId])   //surrounding tile is not visible -> dither
                                                g.DrawImage(Images.DitherDots[tileX, tileY],
                                                            32 * coordsOffsets[0] + 64 + 32 * tileX,
                                                            16 * coordsOffsets[1] + 48 + 16 * tileY);
                                    }
                        }

                        //Units
                        List<IUnit> unitsHere = Game.Units.Where(u => u.X == xCiv2 && u.Y == yCiv2).ToList();
                        //If active unit is in this list-- > remove it
                        if (unitsHere.Contains(Game.Instance.ActiveUnit))
                        {
                            unitsHere.Remove(Game.Instance.ActiveUnit);
                        }
                        if (unitsHere.Any())
                        {
                            IUnit unit;
                            //If this is not tile with active unit or viewing piece, draw last unit on stack
                            if (!unitsHere.Contains(Game.Instance.ActiveUnit))
                            {
                                unit = unitsHere.Last();
                                if (!unit.IsInCity)
                                {
                                    g.DrawImage(
                                        Images.CreateUnitBitmap(unit, unitsHere.Count() > 1, zoom),
                                        32 * coordsOffsets[0] + 64,
                                        16 * coordsOffsets[1] + 32);
                                }
                            }
                            //This tile has active unit/viewing piece
                            else
                            {
                                //Viewing pieces mode is enabled, so draw last unit on stack
                                if (MapPanel.ViewPiecesMode)
                                {
                                    unit = unitsHere.Last();
                                    if (!unit.IsInCity)
                                    {
                                        g.DrawImage(
                                            Images.CreateUnitBitmap(unit, unitsHere.Count() > 1, zoom),
                                            32 * coordsOffsets[0] + 64,
                                            16 * coordsOffsets[1] + 32);
                                    }
                                }
                            }
                        }

                        //Cities
                        City city = Game.Cities.Find(c => c.X == xCiv2 && c.Y == yCiv2);
                        if (city != null)
                        {
                            g.DrawImage(
                                Images.CreateCityBitmap(city, true, 8),
                                32 * coordsOffsets[0] + 64,
                                16 * coordsOffsets[1] + 32);
                        }
                    }
                }

                //City names
                //Add additional coords for drawing city names
                coordsOffsetsToBeDrawn.Add(new int[] { -3, -5 });
                coordsOffsetsToBeDrawn.Add(new int[] { -1, -5 });
                coordsOffsetsToBeDrawn.Add(new int[] { 1, -5 });
                coordsOffsetsToBeDrawn.Add(new int[] { 3, -5 });
                coordsOffsetsToBeDrawn.Add(new int[] { -4, -2 });
                coordsOffsetsToBeDrawn.Add(new int[] { 4, -2 });
                coordsOffsetsToBeDrawn.Add(new int[] { -4, 0 });
                coordsOffsetsToBeDrawn.Add(new int[] { 4, 0 });
                coordsOffsetsToBeDrawn.Add(new int[] { -4, 2 });
                coordsOffsetsToBeDrawn.Add(new int[] { 4, 2 });
                foreach (int[] coordsOffsets in coordsOffsetsToBeDrawn)
                {
                    //change coords of central offset
                    int x = centralCoords[0] + coordsOffsets[0];
                    int y = centralCoords[1] + coordsOffsets[1];

                    if (x >= 0 && y >= 0 && x < 2 * Data.MapXdim && y < Data.MapYdim)    //make sure you're not drawing tiles outside map bounds
                    {
                        City city = Game.Cities.Find(c => c.X == x && c.Y == y);
                        if (city != null)
                        {
                            Bitmap cityNameBitmap = Images.CreateCityNameBitmap(city, 8);
                            g.DrawImage(
                                cityNameBitmap,
                                32 * coordsOffsets[0] + 64 + 32 - cityNameBitmap.Width / 2,
                                16 * coordsOffsets[1] + 3 * 8 + 48);
                        }
                    }
                }
            }
        }

        public static Bitmap TerrainBitmap(int col, int row)
        {
            Bitmap tile = new Bitmap(64, 32); //define a bitmap for drawing in MapForm

            using (Graphics graphics = Graphics.FromImage(tile))  //Draw tiles
            {
                Bitmap maptype;
                switch (Game.TerrainTile[col, row].Type)
                {
                    case TerrainType.Desert: maptype = Desert[0]; break;
                    case TerrainType.Forest: maptype = ForestBase[0]; break;
                    case TerrainType.Glacier: maptype = Glacier[0]; break;
                    case TerrainType.Grassland: maptype = Grassland[0]; break;
                    case TerrainType.Hills: maptype = HillsBase[0]; break;
                    case TerrainType.Jungle: maptype = Jungle[0]; break;
                    case TerrainType.Mountains: maptype = MtnsBase[0]; break;
                    case TerrainType.Ocean: maptype = Ocean[0]; break;
                    case TerrainType.Plains: maptype = Plains[0]; break;
                    case TerrainType.Swamp: maptype = Swamp[0]; break;
                    case TerrainType.Tundra: maptype = Tundra[0]; break;
                    default: throw new ArgumentOutOfRangeException();
                }
                graphics.DrawImage(maptype, 0, 0);

                //Dither
                int col_ = 2 * col + row % 2;   //to civ2-style
                //First check if you are on map edge. If not, look at type of terrain in all 4 directions.
                TerrainType[,] tiletype = new TerrainType[2, 2];
                if ((col_ != 0) && (row != 0)) tiletype[0, 0] = Game.TerrainTile[((col_ - 1) - (row - 1) % 2) / 2, row - 1].Type;
                if ((col_ != 2 * Data.MapXdim - 1) && (row != 0)) tiletype[1, 0] = Game.TerrainTile[((col_ + 1) - (row - 1) % 2) / 2, row - 1].Type;
                if ((col_ != 0) && (row != Data.MapYdim - 1)) tiletype[0, 1] = Game.TerrainTile[((col_ - 1) - (row + 1) % 2) / 2, row + 1].Type;
                if ((col_ != 2 * Data.MapXdim - 1) && (row != Data.MapYdim - 1)) tiletype[1, 1] = Game.TerrainTile[((col_ + 1) - (row + 1) % 2) / 2, row + 1].Type;
                //implement dither on 4 locations in square
                for (int tileX = 0; tileX < 2; tileX++)    //for 4 directions
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

                //Draw coast & river mouth
                if (Game.TerrainTile[col, row].Type == TerrainType.Ocean)
                {
                    int[] land = IsLandPresent(col, row);   //Determine if land is present in 8 directions

                    //draw coast & river mouth tiles
                    //NW+N+NE tiles
                    if (land[7] == 0 && land[0] == 0 && land[1] == 0) graphics.DrawImage(Coast[0, 0], 16, 0);
                    if (land[7] == 1 && land[0] == 0 && land[1] == 0) graphics.DrawImage(Coast[1, 0], 16, 0);
                    if (land[7] == 0 && land[0] == 1 && land[1] == 0) graphics.DrawImage(Coast[2, 0], 16, 0);
                    if (land[7] == 1 && land[0] == 1 && land[1] == 0) graphics.DrawImage(Coast[3, 0], 16, 0);
                    if (land[7] == 0 && land[0] == 0 && land[1] == 1) graphics.DrawImage(Coast[4, 0], 16, 0);
                    if (land[7] == 1 && land[0] == 0 && land[1] == 1) graphics.DrawImage(Coast[5, 0], 16, 0);
                    if (land[7] == 0 && land[0] == 1 && land[1] == 1) graphics.DrawImage(Coast[6, 0], 16, 0);
                    if (land[7] == 1 && land[0] == 1 && land[1] == 1) graphics.DrawImage(Coast[7, 0], 16, 0);
                    //SW+S+SE tiles
                    if (land[3] == 0 && land[4] == 0 && land[5] == 0) graphics.DrawImage(Coast[0, 1], 16, 16);
                    if (land[3] == 1 && land[4] == 0 && land[5] == 0) graphics.DrawImage(Coast[1, 1], 16, 16);
                    if (land[3] == 0 && land[4] == 1 && land[5] == 0) graphics.DrawImage(Coast[2, 1], 16, 16);
                    if (land[3] == 1 && land[4] == 1 && land[5] == 0) graphics.DrawImage(Coast[3, 1], 16, 16);
                    if (land[3] == 0 && land[4] == 0 && land[5] == 1) graphics.DrawImage(Coast[4, 1], 16, 16);
                    if (land[3] == 1 && land[4] == 0 && land[5] == 1) graphics.DrawImage(Coast[5, 1], 16, 16);
                    if (land[3] == 0 && land[4] == 1 && land[5] == 1) graphics.DrawImage(Coast[6, 1], 16, 16);
                    if (land[3] == 1 && land[4] == 1 && land[5] == 1) graphics.DrawImage(Coast[7, 1], 16, 16);
                    //SW+W+NW tiles
                    if (land[5] == 0 && land[6] == 0 && land[7] == 0) graphics.DrawImage(Coast[0, 2], 0, 8);
                    if (land[5] == 1 && land[6] == 0 && land[7] == 0) graphics.DrawImage(Coast[1, 2], 0, 8);
                    if (land[5] == 0 && land[6] == 1 && land[7] == 0) graphics.DrawImage(Coast[2, 2], 0, 8);
                    if (land[5] == 1 && land[6] == 1 && land[7] == 0) graphics.DrawImage(Coast[3, 2], 0, 8);
                    if (land[5] == 0 && land[6] == 0 && land[7] == 1) graphics.DrawImage(Coast[4, 2], 0, 8);
                    if (land[5] == 1 && land[6] == 0 && land[7] == 1) graphics.DrawImage(Coast[5, 2], 0, 8);
                    if (land[5] == 0 && land[6] == 1 && land[7] == 1) graphics.DrawImage(Coast[6, 2], 0, 8);
                    if (land[5] == 1 && land[6] == 1 && land[7] == 1) graphics.DrawImage(Coast[7, 2], 0, 8);
                    //NE+E+SE tiles
                    if (land[1] == 0 && land[2] == 0 && land[3] == 0) graphics.DrawImage(Coast[0, 3], 32, 8);
                    if (land[1] == 1 && land[2] == 0 && land[3] == 0) graphics.DrawImage(Coast[1, 3], 32, 8);
                    if (land[1] == 0 && land[2] == 1 && land[3] == 0) graphics.DrawImage(Coast[2, 3], 32, 8);
                    if (land[1] == 1 && land[2] == 1 && land[3] == 0) graphics.DrawImage(Coast[3, 3], 32, 8);
                    if (land[1] == 0 && land[2] == 0 && land[3] == 1) graphics.DrawImage(Coast[4, 3], 32, 8);
                    if (land[1] == 1 && land[2] == 0 && land[3] == 1) graphics.DrawImage(Coast[5, 3], 32, 8);
                    if (land[1] == 0 && land[2] == 1 && land[3] == 1) graphics.DrawImage(Coast[6, 3], 32, 8);
                    if (land[1] == 1 && land[2] == 1 && land[3] == 1) graphics.DrawImage(Coast[7, 3], 32, 8);

                    //River mouth
                    //if next to ocean is river, draw river mouth on this tile                            
                    col_ = 2 * col + row % 2; //rewrite indexes in Civ2-style
                    int Xdim = 2 * Data.MapXdim;   //X=50 in markted as X=100 in Civ2
                    int Ydim = Data.MapYdim;   //no need for such correction for Y
                    if (col_ + 1 < Xdim && row - 1 >= 0)    //NE there is no edge of map
                    {
                        if (land[1] == 1 && Game.TerrainTile[((col_ + 1) - (row - 1) % 2) / 2, row - 1].River) graphics.DrawImage(RiverMouth[0], 0, 0);
                    }
                    if (col_ + 1 < Xdim && row + 1 < Ydim)    //SE there is no edge of map
                    {
                        if (land[3] == 1 && Game.TerrainTile[((col_ + 1) - (row + 1) % 2) / 2, row + 1].River) graphics.DrawImage(RiverMouth[1], 0, 0);
                    }
                    if (col_ - 1 >= 0 && row + 1 < Ydim)    //SW there is no edge of map
                    {
                        if (land[5] == 1 && Game.TerrainTile[((col_ - 1) - (row + 1) % 2) / 2, row + 1].River) graphics.DrawImage(RiverMouth[2], 0, 0);
                    }
                    if (col_ - 1 >= 0 && row - 1 >= 0)    //NW there is no edge of map
                    {
                        if (land[7] == 1 && Game.TerrainTile[((col_ - 1) - (row - 1) % 2) / 2, row - 1].River) graphics.DrawImage(RiverMouth[3], 0, 0);
                    }
                }

                //Draw forests
                if (Game.TerrainTile[col, row].Type == TerrainType.Forest)
                {
                    int[] forestAround = IsForestAround(col, row);

                    //draw forest tiles
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 0, 0, 0 })) graphics.DrawImage(Forest[0], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 0, 0, 0 })) graphics.DrawImage(Forest[1], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 1, 0, 0 })) graphics.DrawImage(Forest[2], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 1, 0, 0 })) graphics.DrawImage(Forest[3], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 0, 1, 0 })) graphics.DrawImage(Forest[4], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 0, 1, 0 })) graphics.DrawImage(Forest[5], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 1, 1, 0 })) graphics.DrawImage(Forest[6], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 1, 1, 0 })) graphics.DrawImage(Forest[7], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 0, 0, 1 })) graphics.DrawImage(Forest[8], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 0, 0, 1 })) graphics.DrawImage(Forest[9], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 1, 0, 1 })) graphics.DrawImage(Forest[10], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 1, 0, 1 })) graphics.DrawImage(Forest[11], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 0, 1, 1 })) graphics.DrawImage(Forest[12], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 0, 1, 1 })) graphics.DrawImage(Forest[13], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 1, 1, 1 })) graphics.DrawImage(Forest[14], 0, 0);
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 1, 1, 1 })) graphics.DrawImage(Forest[15], 0, 0);
                }

                //Draw mountains
                //CORRECT THIS: IF SHIELD IS AT MOUNTAIN IT SHOULD BE Mountains[2] and Mountains[3] !!!
                if (Game.TerrainTile[col, row].Type == TerrainType.Mountains)
                {
                    int[] mountAround = IsMountAround(col, row);

                    //draw forest tiles
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 0, 0, 0 })) graphics.DrawImage(Mountains[0], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 0, 0, 0 })) graphics.DrawImage(Mountains[1], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 1, 0, 0 })) graphics.DrawImage(Mountains[2], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 1, 0, 0 })) graphics.DrawImage(Mountains[3], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 0, 1, 0 })) graphics.DrawImage(Mountains[4], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 0, 1, 0 })) graphics.DrawImage(Mountains[5], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 1, 1, 0 })) graphics.DrawImage(Mountains[6], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 1, 1, 0 })) graphics.DrawImage(Mountains[7], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 0, 0, 1 })) graphics.DrawImage(Mountains[8], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 0, 0, 1 })) graphics.DrawImage(Mountains[9], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 1, 0, 1 })) graphics.DrawImage(Mountains[10], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 1, 0, 1 })) graphics.DrawImage(Mountains[11], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 0, 1, 1 })) graphics.DrawImage(Mountains[12], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 0, 1, 1 })) graphics.DrawImage(Mountains[13], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 1, 1, 1 })) graphics.DrawImage(Mountains[14], 0, 0);
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 1, 1, 1 })) graphics.DrawImage(Mountains[15], 0, 0);
                }

                //Draw hills
                if (Game.TerrainTile[col, row].Type == TerrainType.Hills)
                {
                    int[] hillAround = IsHillAround(col, row);

                    //draw forest tiles
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 0, 0, 0 })) graphics.DrawImage(Hills[0], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 0, 0, 0 })) graphics.DrawImage(Hills[1], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 1, 0, 0 })) graphics.DrawImage(Hills[2], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 1, 0, 0 })) graphics.DrawImage(Hills[3], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 0, 1, 0 })) graphics.DrawImage(Hills[4], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 0, 1, 0 })) graphics.DrawImage(Hills[5], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 1, 1, 0 })) graphics.DrawImage(Hills[6], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 1, 1, 0 })) graphics.DrawImage(Hills[7], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 0, 0, 1 })) graphics.DrawImage(Hills[8], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 0, 0, 1 })) graphics.DrawImage(Hills[9], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 1, 0, 1 })) graphics.DrawImage(Hills[10], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 1, 0, 1 })) graphics.DrawImage(Hills[11], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 0, 1, 1 })) graphics.DrawImage(Hills[12], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 0, 1, 1 })) graphics.DrawImage(Hills[13], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 1, 1, 1 })) graphics.DrawImage(Hills[14], 0, 0);
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 1, 1, 1 })) graphics.DrawImage(Hills[15], 0, 0);
                }

                //Draw rivers
                if (Game.TerrainTile[col, row].River)
                {
                    int[] riverAround = IsRiverAround(col, row);

                    //draw river tiles
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 0, 0, 0 })) graphics.DrawImage(River[0], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 0, 0, 0 })) graphics.DrawImage(River[1], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 1, 0, 0 })) graphics.DrawImage(River[2], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 1, 0, 0 })) graphics.DrawImage(River[3], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 0, 1, 0 })) graphics.DrawImage(River[4], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 0, 1, 0 })) graphics.DrawImage(River[5], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 1, 1, 0 })) graphics.DrawImage(River[6], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 1, 1, 0 })) graphics.DrawImage(River[7], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 0, 0, 1 })) graphics.DrawImage(River[8], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 0, 0, 1 })) graphics.DrawImage(River[9], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 1, 0, 1 })) graphics.DrawImage(River[10], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 1, 0, 1 })) graphics.DrawImage(River[11], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 0, 1, 1 })) graphics.DrawImage(River[12], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 0, 1, 1 })) graphics.DrawImage(River[13], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 1, 1, 1 })) graphics.DrawImage(River[14], 0, 0);
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 1, 1, 1 })) graphics.DrawImage(River[15], 0, 0);
                }

                //Draw special resources if they exist
                if (Game.TerrainTile[col, row].SpecType != null)
                {
                    switch (Game.TerrainTile[col, row].SpecType)
                    {
                        case SpecialType.Oasis: maptype = Desert[2]; break;
                        case SpecialType.DesertOil: maptype = Desert[3]; break;
                        case SpecialType.Buffalo: maptype = Plains[2]; break;
                        case SpecialType.Wheat: maptype = Plains[3]; break;
                        case SpecialType.GrasslandShield: maptype = Shield; break;
                        case SpecialType.Pheasant: maptype = ForestBase[2]; break;
                        case SpecialType.Silk: maptype = ForestBase[3]; break;
                        case SpecialType.Coal: maptype = HillsBase[2]; break;
                        case SpecialType.Wine: maptype = HillsBase[3]; break;
                        case SpecialType.Gold: maptype = MtnsBase[2]; break;
                        case SpecialType.Iron: maptype = MtnsBase[3]; break;
                        case SpecialType.Game: maptype = Tundra[2]; break;
                        case SpecialType.Furs: maptype = Tundra[3]; break;
                        case SpecialType.Ivory: maptype = Glacier[2]; break;
                        case SpecialType.GlacierOil: maptype = Glacier[3]; break;
                        case SpecialType.Peat: maptype = Swamp[2]; break;
                        case SpecialType.Spice: maptype = Swamp[3]; break;
                        case SpecialType.Gems: maptype = Jungle[2]; break;
                        case SpecialType.Fruit: maptype = Jungle[3]; break;
                        case SpecialType.Fish: maptype = Ocean[2]; break;
                        case SpecialType.Whales: maptype = Ocean[3]; break;
                        default: throw new ArgumentOutOfRangeException();
                    }
                    graphics.DrawImage(maptype, 0, 0);
                }

                //Roads (cites also act as road tiles)
                if (Game.TerrainTile[col, row].Road || Game.TerrainTile[col, row].CityPresent)
                {
                    int[] roadAround = IsRoadAround(col, row);

                    //draw roads
                    if (roadAround[0] == 1) graphics.DrawImage(Road[8], 0, 0);  //to N
                    if (roadAround[1] == 1) graphics.DrawImage(Road[1], 0, 0);  //to NE
                    if (roadAround[2] == 1) graphics.DrawImage(Road[2], 0, 0);  //to E
                    if (roadAround[3] == 1) graphics.DrawImage(Road[3], 0, 0);  //to SE
                    if (roadAround[4] == 1) graphics.DrawImage(Road[4], 0, 0);  //to S
                    if (roadAround[5] == 1) graphics.DrawImage(Road[5], 0, 0);  //to SW
                    if (roadAround[6] == 1) graphics.DrawImage(Road[6], 0, 0);  //to W
                    if (roadAround[7] == 1) graphics.DrawImage(Road[7], 0, 0);  //to NW
                    if (Enumerable.SequenceEqual(roadAround, new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 })) graphics.DrawImage(Road[0], 0, 0);    //no road around
                }

                // !!!! NOT AS SIMPLE AS THIS. CORRECT THIS !!!!!
                //Railroads (cites also act as railroad tiles)
                //if (Game.TerrainTile[i, j].Railroad || Game.TerrainTile[i, j].CityPresent)
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
                if (Game.TerrainTile[col, row].Irrigation) graphics.DrawImage(Irrigation, 0, 0);

                //Farmland
                if (Game.TerrainTile[col, row].Farmland) graphics.DrawImage(Farmland, 0, 0);

                //Mining
                if (Game.TerrainTile[col, row].Mining) graphics.DrawImage(Mining, 0, 0);

                //Pollution
                if (Game.TerrainTile[col, row].Pollution) graphics.DrawImage(Pollution, 0, 0);

                //Fortress
                if (Game.TerrainTile[col, row].Fortress) graphics.DrawImage(Fortress, 0, 0);

                //Airbase
                if (Game.TerrainTile[col, row].Airbase) graphics.DrawImage(Airbase, 0, 0);

            }

            return tile;
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
            else if (Game.TerrainTile[(i_ - (j - 2) % 2) / 2, j - 2].Type != TerrainType.Ocean) land[0] = 1;
            //NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) land[1] = 1;  //NE is black tile
            else if (Game.TerrainTile[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type != TerrainType.Ocean) land[1] = 1;    //if it is not ocean, it is land
            //E:
            if (i_ + 2 >= Xdim) land[2] = 1;  //E is black tile
            else if (Game.TerrainTile[((i_ + 2) - j % 2) / 2, j].Type != TerrainType.Ocean) land[2] = 1;
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) land[3] = 1;  //SE is black tile
            else if (Game.TerrainTile[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type != TerrainType.Ocean) land[3] = 1;
            //S:
            if (j + 2 >= Ydim) land[4] = 1;   //S is black tile
            else if (Game.TerrainTile[(i_ - (j + 2) % 2) / 2, j + 2].Type != TerrainType.Ocean) land[4] = 1;
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) land[5] = 1;  //SW is black tile
            else if (Game.TerrainTile[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type != TerrainType.Ocean) land[5] = 1;
            //W:
            if (i_ - 2 < 0) land[6] = 1;  //W is black tile
            else if (Game.TerrainTile[((i_ - 2) - j % 2) / 2, j].Type != TerrainType.Ocean) land[6] = 1;
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) land[7] = 1;  //NW is black tile
            else if (Game.TerrainTile[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type != TerrainType.Ocean) land[7] = 1;

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
            else if (Game.TerrainTile[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Forest) forestAround[0] = 1;
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) forestAround[1] = 0;  //SE is black tile
            else if (Game.TerrainTile[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Forest) forestAround[1] = 1;
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) forestAround[2] = 0;  //SW is black tile
            else if (Game.TerrainTile[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Forest) forestAround[2] = 1;
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) forestAround[3] = 0;  //NW is black tile
            else if (Game.TerrainTile[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Forest) forestAround[3] = 1;

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
            else if (Game.TerrainTile[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Mountains) mountAround[0] = 1;
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) mountAround[1] = 0;  //SE is black tile
            else if (Game.TerrainTile[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Mountains) mountAround[1] = 1;
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) mountAround[2] = 0;  //SW is black tile
            else if (Game.TerrainTile[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Mountains) mountAround[2] = 1;
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) mountAround[3] = 0;  //NW is black tile
            else if (Game.TerrainTile[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Mountains) mountAround[3] = 1;

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
            else if (Game.TerrainTile[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Hills) hillAround[0] = 1;
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) hillAround[1] = 0;  //SE is black tile
            else if (Game.TerrainTile[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Hills) hillAround[1] = 1;
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) hillAround[2] = 0;  //SW is black tile
            else if (Game.TerrainTile[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Hills) hillAround[2] = 1;
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) hillAround[3] = 0;  //NW is black tile
            else if (Game.TerrainTile[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Hills) hillAround[3] = 1;

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
            else if (Game.TerrainTile[((i_ + 1) - (j - 1) % 2) / 2, j - 1].River || Game.TerrainTile[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Ocean) riverAround[0] = 1;
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) riverAround[1] = 0;  //SE is black tile
            else if (Game.TerrainTile[((i_ + 1) - (j + 1) % 2) / 2, j + 1].River || Game.TerrainTile[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Ocean) riverAround[1] = 1;
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) riverAround[2] = 0;  //SW is black tile
            else if (Game.TerrainTile[((i_ - 1) - (j + 1) % 2) / 2, j + 1].River || Game.TerrainTile[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Ocean) riverAround[2] = 1;
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) riverAround[3] = 0;  //NW is black tile
            else if (Game.TerrainTile[((i_ - 1) - (j - 1) % 2) / 2, j - 1].River || Game.TerrainTile[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Ocean) riverAround[3] = 1;

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
            else if (Game.TerrainTile[(i_ - (j - 2) % 2) / 2, j - 2].Road || Game.TerrainTile[(i_ - (j - 2) % 2) / 2, j - 2].CityPresent) roadAround[0] = 1;
            //NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) roadAround[1] = 0;  //NE is black tile
            else if (Game.TerrainTile[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Road || Game.TerrainTile[((i_ + 1) - (j - 1) % 2) / 2, j - 1].CityPresent) roadAround[1] = 1;
            //E:
            if (i_ + 2 >= Xdim) roadAround[2] = 0;  //E is black tile
            else if (Game.TerrainTile[((i_ + 2) - j % 2) / 2, j].Road || Game.TerrainTile[((i_ + 2) - j % 2) / 2, j].CityPresent) roadAround[2] = 1;
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) roadAround[3] = 0;  //SE is black tile
            else if (Game.TerrainTile[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Road || Game.TerrainTile[((i_ + 1) - (j + 1) % 2) / 2, j + 1].CityPresent) roadAround[3] = 1;
            //S:
            if (j + 2 >= Ydim) roadAround[4] = 0;   //S is black tile
            else if (Game.TerrainTile[(i_ - (j + 2) % 2) / 2, j + 2].Road || Game.TerrainTile[(i_ - (j + 2) % 2) / 2, j + 2].CityPresent) roadAround[4] = 1;
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) roadAround[5] = 0;  //SW is black tile
            else if (Game.TerrainTile[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Road || Game.TerrainTile[((i_ - 1) - (j + 1) % 2) / 2, j + 1].CityPresent) roadAround[5] = 1;
            //W:
            if (i_ - 2 < 0) roadAround[6] = 0;  //W is black tile
            else if (Game.TerrainTile[((i_ - 2) - j % 2) / 2, j].Road || Game.TerrainTile[((i_ - 2) - j % 2) / 2, j].CityPresent) roadAround[6] = 1;
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) roadAround[7] = 0;  //NW is black tile
            else if (Game.TerrainTile[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Road || Game.TerrainTile[((i_ - 1) - (j - 1) % 2) / 2, j - 1].CityPresent) roadAround[7] = 1;

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
            else if (Game.TerrainTile[(i_ - (j - 2) % 2) / 2, j - 2].Railroad || Game.TerrainTile[(i_ - (j - 2) % 2) / 2, j - 2].CityPresent) railroadAround[0] = 1;
            //NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) railroadAround[1] = 0;  //NE is black tile
            else if (Game.TerrainTile[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Railroad || Game.TerrainTile[((i_ + 1) - (j - 1) % 2) / 2, j - 1].CityPresent) railroadAround[1] = 1;
            //E:
            if (i_ + 2 >= Xdim) railroadAround[2] = 0;  //E is black tile
            else if (Game.TerrainTile[((i_ + 2) - j % 2) / 2, j].Railroad || Game.TerrainTile[((i_ + 2) - j % 2) / 2, j].CityPresent) railroadAround[2] = 1;
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) railroadAround[3] = 0;  //SE is black tile
            else if (Game.TerrainTile[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Railroad || Game.TerrainTile[((i_ + 1) - (j + 1) % 2) / 2, j + 1].CityPresent) railroadAround[3] = 1;
            //S:
            if (j + 2 >= Ydim) railroadAround[4] = 0;   //S is black tile
            else if (Game.TerrainTile[(i_ - (j + 2) % 2) / 2, j + 2].Railroad || Game.TerrainTile[(i_ - (j + 2) % 2) / 2, j + 2].CityPresent) railroadAround[4] = 1;
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) railroadAround[5] = 0;  //SW is black tile
            else if (Game.TerrainTile[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Railroad || Game.TerrainTile[((i_ - 1) - (j + 1) % 2) / 2, j + 1].CityPresent) railroadAround[5] = 1;
            //W:
            if (i_ - 2 < 0) railroadAround[6] = 0;  //W is black tile
            else if (Game.TerrainTile[((i_ - 2) - j % 2) / 2, j].Railroad || Game.TerrainTile[((i_ - 2) - j % 2) / 2, j].CityPresent) railroadAround[6] = 1;
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) railroadAround[7] = 0;  //NW is black tile
            else if (Game.TerrainTile[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Railroad || Game.TerrainTile[((i_ - 1) - (j - 1) % 2) / 2, j - 1].CityPresent) railroadAround[7] = 1;

            return railroadAround;
        }

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
                    graphics.DrawImage(NoBorderUnitShield[unit.CivId], secondShieldXLoc, unitShieldLocation[(int)unit.Type, 1]);   //dark shield
                }

                //shield shadow
                graphics.DrawImage(UnitShieldShadow, unitShieldLocation[(int)unit.Type, 0] + borderShieldOffset, unitShieldLocation[(int)unit.Type, 1] - borderShieldOffset);

                //main shield
                graphics.DrawImage(UnitShield[unit.CivId], unitShieldLocation[(int)unit.Type, 0], unitShieldLocation[(int)unit.Type, 1]);

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
            if (cityStyle < 4) 
            {
                if (Array.Exists(city.Improvements, element => element.Type == ImprovementType.Palace))     //palace exists
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
            g.DrawString(city.Name, new Font("Times New Roman", fontSize), new SolidBrush(CivColors.CityTextColor[city.Owner]), new PointF(0, 0));

            return _textGraphic;
        }

        public static List<Bitmap> CreateMapAnimation(AnimationType animation)
        {
            List<Bitmap> _bitmapList = new List<Bitmap>();

            switch (animation)
            {
                case AnimationType.UnitWaiting:
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Bitmap _bitmap = new Bitmap(64, 32);
                            using (Graphics g = Graphics.FromImage(_bitmap))
                            {
                                if (MapPanel.ViewPiecesMode)
                                {

                                }
                                else    //unit is active
                                {
                                    int x = Game.Instance.ActiveUnit.X;
                                    int y = Game.Instance.ActiveUnit.Y;
                                    g.DrawImage(Game.CivsMap[Game.Instance.ActiveCiv.Id], new Rectangle(32 * x, 16 * y, 64, 48));
                                    if (i == 1) g.DrawImage(Game.Instance.ActiveUnit.GraphicMapPanel, 0, 16);
                                }
                            }
                            _bitmapList.Add(_bitmap);
                        }
                        break;
                    }
            }

            return _bitmapList;
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

        public static Bitmap DrawCitizens(City city, double scale_factor) //Draw faces in cityform
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
                PeopleType[] peoples = city.People;
                int drawIndex = 0;
                for (int i = 0; i < city.Size; i++)
                {
                    drawIndex = (int)peoples[i];
                    if (i % 2 == 1 && (drawIndex == 0 || drawIndex == 2 || drawIndex == 4 || drawIndex == 6)) drawIndex++;  //to change men/woman appearance
                    graphics.DrawImage(ModifyImage.ResizeImage(PeopleLshadow[drawIndex, 0], (int)(27 * scale_factor), (int)(30 * scale_factor)), i * spacing + 1, 1);   //shadow
                    graphics.DrawImage(ModifyImage.ResizeImage(PeopleL[drawIndex, 0], (int)(27 * scale_factor), (int)(30 * scale_factor)), i * spacing, 0);
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

        public static Bitmap DrawCityResourcesMap(City city)    //Draw terrain in city form
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
                            //TODO: correct this
                            //if (newX >= 0 && newX < 2 * Data.MapXdim && newY >= 0 && newY < Data.MapYdim) image = TerrainBitmap((newX - (newY % 2)) / 2, newY);
                            //else image = Blank;
                            image = Blank;
                            graphics.DrawImage(image, 32 * (x_ + 3), 16 * (y_ + 3));
                        }

                //Then draw city
                graphics.DrawImage(CreateCityBitmap(city, false, 8), 64 * 1 + 32 * (3 % 2) + 1, 16 * 2 + 1);

                //Then draw food/shield/trade icons around the city (21 squares around city)
                int[,] offsets = new int[21, 2] { { 0, 0 }, { -1, -3 }, { -3, -1 }, { -3, 1 }, { -1, 3 }, { 1, 3 }, { 3, 1 }, { 3, -1 }, { 1, -3 }, { -2, -2 }, { -2, 2 }, { 2, 2 },
                                                  { 2, -2 }, { 0, -2 }, { -1, -1 }, { -2, 0 }, { -1, 1 }, { 0, 2 }, { 1, 1 }, { 2, 0 }, { 1, -1 } };    //offset of squares from city square (0,0)
                int[] cityFood = city.FoodDistribution;
                int[] cityShld = city.ShieldDistribution;
                int[] cityTrad = city.TradeDistribution;
                for (int i = 0; i < 21; i++)
                    if (city.DistributionWorkers[i] == 1)
                    {
                        //First count all icons on this square to determine the spacing between icons (10 = no spacing, 15 = no spacing @ 50% scaled)
                        int spacing;
                        switch (cityFood[i] + cityShld[i] + cityTrad[i])
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
                        int x_offset = 32 - ((cityFood[i] + cityShld[i] + cityTrad[i] - 1) * spacing + 15) / 2;
                        int y_offset = 9;
                        for (int j = 0; j < cityFood[i]; j++) graphics.DrawImage(CitymapFoodSmallBigger, x_offset + (3 + offsets[i, 0]) * 32 + j * spacing, y_offset + (3 + offsets[i, 1]) * 16);
                        for (int j = 0; j < cityShld[i]; j++) graphics.DrawImage(CitymapShieldSmallBigger, x_offset + (3 + offsets[i, 0]) * 32 + (cityFood[i] + j) * spacing, y_offset + (3 + offsets[i, 1]) * 16);
                        for (int j = 0; j < cityTrad[i]; j++) graphics.DrawImage(CitymapTradeSmallBigger, x_offset + (3 + offsets[i, 0]) * 32 + (cityFood[i] + cityShld[i] + j) * spacing, y_offset + (3 + offsets[i, 1]) * 16);
                    }
            }
            return map;
        }

    }
}
 