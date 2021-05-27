using System;
using System.Collections.Generic;
using Eto.Drawing;
using System.Linq;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Terrains;
using EtoFormsUIExtensionMethods;

namespace EtoFormsUI
{
    public static partial class Draw
    {
        public static void Tile(Graphics g, int xC2, int yC2, int zoom, Point dest)
        {
            using var _tilePic = Images.MapTileGraphicC2(xC2, yC2).Resize(zoom);
            g.DrawImage(_tilePic, dest);
        }

        public static Bitmap MakeTileGraphic(ITerrain tile, int col, int row, bool flatEarth, TerrainSet terrainSet)
        {
            // EVERYTHING HERE IS IN CIV2-COORDS AND NOT IN REGULAR COORDS!!!

            // First convert regular coords to civ2-style
            col = 2 * col + row % 2; // you don't change row
            int Xdim = 2 * Map.XDim;
            int Ydim = Map.YDim;

            // Define a bitmap for drawing
            var _tilePic = new Bitmap(64, 32, PixelFormat.Format32bppRgba);

            // Draw tile
            using var g = new Graphics(_tilePic);
            
            g.DrawImage(terrainSet.BaseTiles[(int)tile.Type], 0, 0);

            // Dither
            if (tile.Type != TerrainType.Ocean)
            {
                if (flatEarth)
                {
                    // Determine type of NW tile
                    if ((col != 0) && row != 0) ApplyDither(g,Map.TileC2(col - 1, row - 1).Type, tile.Type, terrainSet.DitherMaps[0], 0, 0);
                    // Determine type of NE tile
                    if (col != Xdim - 1 && (row != 0)) ApplyDither(g, Map.TileC2(col + 1, row - 1).Type, tile.Type, terrainSet.DitherMaps[1], 32, 0);
                    // Determine type of SW tile
                    if (col != 0 && (row != Ydim - 1)) ApplyDither(g, Map.TileC2(col - 1, row + 1).Type, tile.Type, terrainSet.DitherMaps[2], 0, 16);
                    // Determine type of SE tile
                    if (col != Xdim - 1 && (row != Ydim - 1)) ApplyDither(g,Map.TileC2(col + 1, row + 1).Type, tile.Type, terrainSet.DitherMaps[3], 32, 16);
                }
                else // Round earth
                {
                    if (row != 0)
                    {
                        ApplyDither(g, Map.TileC2((col == 0 ? Xdim : col) - 1, row - 1).Type, tile.Type,
                            terrainSet.DitherMaps[0], 0, 0);

                        ApplyDither(g, Map.TileC2(col == Xdim - 1 ? 0 : col + 1, row - 1).Type, tile.Type,
                            terrainSet.DitherMaps[1], 32, 0);
                    }

                    if (row != Ydim - 1)
                    {
                        ApplyDither(g, Map.TileC2((col == 0 ? Xdim : col) - 1, row + 1).Type, tile.Type,
                            terrainSet.DitherMaps[2], 0, 16);

                        ApplyDither(g, Map.TileC2(col == Xdim - 1 ? 0 : col + 1, row + 1).Type, tile.Type,
                            terrainSet.DitherMaps[3], 32, 16);
                    }
                }
            }
            // // Determine type of terrain in all 4 directions. Be careful if you're on map edge.
            // TerrainType?[,]
            //     tiletype = new TerrainType?[2, 2] {{null, null}, {null, null}}; // null = beyond map limits
            // if (flatEarth)
            // {
            //     // Determine type of NW tile
            //     if ((col != 0) && (row != 0)) tiletype[0, 0] = Map.TileC2(col - 1, row - 1).Type;
            //     // Determine type of NE tile
            //     if ((col != Xdim - 1) && (row != 0)) tiletype[1, 0] = Map.TileC2(col + 1, row - 1).Type;
            //     // Determine type of SW tile
            //     if ((col != 0) && (row != Ydim - 1)) tiletype[0, 1] = Map.TileC2(col - 1, row + 1).Type;
            //     // Determine type of SE tile
            //     if ((col != Xdim - 1) && (row != Ydim - 1)) tiletype[1, 1] = Map.TileC2(col + 1, row + 1).Type;
            // }
            // else // Round earth
            // {
            //     // Determine type of NW tile
            //     if ((col == 0) && (row != 0)) tiletype[0, 0] = Map.TileC2(Xdim - 1, row - 1).Type;   // if on left edge take tile from other side of map
            //     else if ((col != 0) && (row != 0)) tiletype[0, 0] = Map.TileC2(col - 1, row - 1).Type;
            //     // Determine type of NE tile
            //     if ((col == Xdim - 1) && (row != 0)) tiletype[1, 0] = Map.TileC2(0, row - 1).Type;   // if on right edge take tile from other side of map
            //     else if ((col != Xdim - 1) && (row != 0)) tiletype[1, 0] = Map.TileC2(col + 1, row - 1).Type;
            //     // Determine type of SW tile
            //     if ((col == 0) && (row != Ydim - 1)) tiletype[0, 1] = Map.TileC2(Xdim - 1, row + 1).Type;   // if on left edge take tile from other side of map
            //     else if ((col != 0) && (row != Ydim - 1)) tiletype[0, 1] = Map.TileC2(col - 1, row + 1).Type;
            //     // Determine type of SE tile
            //     if ((col == Xdim - 1) && (row != Ydim - 1)) tiletype[1, 1] = Map.TileC2(0, row + 1).Type;  // if on right edge take tile from other side of map
            //     else if ((col != Xdim - 1) && (row != Ydim - 1)) tiletype[1, 1] = Map.TileC2(col + 1, row + 1).Type;
            // }
            //
            //
            // // Implement dither on 4 locations in square
            // for (int tileX = 0; tileX < 2; tileX++) // for 4 directions
            // {
            //     for (int tileY = 0; tileY < 2; tileY++)
            //     {
            //         if(tiletype[tileX, tileY] == tile.Type) continue; //Don't dither same terrain 
            //         switch (tiletype[tileX, tileY])
            //         {
            //             case TerrainType.Desert: g.DrawImage(Images.DitherDesert[tileX, tileY], 32 * tileX, 16 * tileY); break;
            //             case TerrainType.Plains: g.DrawImage(Images.DitherPlains[tileX, tileY], 32 * tileX, 16 * tileY); break;
            //             case TerrainType.Grassland: g.DrawImage(Images.DitherGrassland[tileX, tileY], 32 * tileX, 16 * tileY); break;
            //             case TerrainType.Forest: g.DrawImage(Images.DitherForest[tileX, tileY], 32 * tileX, 16 * tileY); break;
            //             case TerrainType.Hills: g.DrawImage(Images.DitherHills[tileX, tileY], 32 * tileX, 16 * tileY); break;
            //             case TerrainType.Mountains: g.DrawImage(Images.DitherMountains[tileX, tileY], 32 * tileX, 16 * tileY); break;
            //             case TerrainType.Tundra: g.DrawImage(Images.DitherTundra[tileX, tileY], 32 * tileX, 16 * tileY); break;
            //             case TerrainType.Glacier: g.DrawImage(Images.DitherGlacier[tileX, tileY], 32 * tileX, 16 * tileY); break;
            //             case TerrainType.Swamp: g.DrawImage(Images.DitherSwamp[tileX, tileY], 32 * tileX, 16 * tileY); break;
            //             case TerrainType.Jungle: g.DrawImage(Images.DitherJungle[tileX, tileY], 32 * tileX, 16 * tileY); break;
            //             case TerrainType.Ocean: g.DrawImage(Images.DitherGrassland[tileX, tileY], 32 * tileX, 16 * tileY); break;
            //         }
            //     }
            // }

            switch (tile.Type)
            {
                // Draw coast & river mouth
                case TerrainType.Ocean:
                {
                    bool[] land = IsLandAround(col, row, flatEarth); // Determine if land is present in 8 directions

                    // Draw coast & river mouth tiles
                    // NW+N+NE tiles
                    if (!land[7] && !land[0] && !land[1]) g.DrawImage(Images.Coast[0, 0], 16, 0);
                    if (land[7] && !land[0] && !land[1]) g.DrawImage(Images.Coast[1, 0], 16, 0);
                    if (!land[7] && land[0] && !land[1]) g.DrawImage(Images.Coast[2, 0], 16, 0);
                    if (land[7] && land[0] && !land[1]) g.DrawImage(Images.Coast[3, 0], 16, 0);
                    if (!land[7] && !land[0] && land[1]) g.DrawImage(Images.Coast[4, 0], 16, 0);
                    if (land[7] && !land[0] && land[1]) g.DrawImage(Images.Coast[5, 0], 16, 0);
                    if (!land[7] && land[0] && land[1]) g.DrawImage(Images.Coast[6, 0], 16, 0);
                    if (land[7] && land[0] && land[1]) g.DrawImage(Images.Coast[7, 0], 16, 0);

                    // SW+S+SE tiles
                    if (!land[3] && !land[4] && !land[5]) g.DrawImage(Images.Coast[0, 1], 16, 16);
                    if (land[3] && !land[4] && !land[5]) g.DrawImage(Images.Coast[1, 1], 16, 16);
                    if (!land[3] && land[4] && !land[5]) g.DrawImage(Images.Coast[2, 1], 16, 16);
                    if (land[3] && land[4] && !land[5]) g.DrawImage(Images.Coast[3, 1], 16, 16);
                    if (!land[3] && !land[4] && land[5]) g.DrawImage(Images.Coast[4, 1], 16, 16);
                    if (land[3] && !land[4] && land[5]) g.DrawImage(Images.Coast[5, 1], 16, 16);
                    if (!land[3] && land[4] && land[5]) g.DrawImage(Images.Coast[6, 1], 16, 16);
                    if (land[3] && land[4] && land[5]) g.DrawImage(Images.Coast[7, 1], 16, 16);

                    // SW+W+NW tiles
                    if (!land[5] && !land[6] && !land[7]) g.DrawImage(Images.Coast[0, 2], 0, 8);
                    if (land[5] && !land[6] && !land[7]) g.DrawImage(Images.Coast[1, 2], 0, 8);
                    if (!land[5] && land[6] && !land[7]) g.DrawImage(Images.Coast[2, 2], 0, 8);
                    if (land[5] && land[6] && !land[7]) g.DrawImage(Images.Coast[3, 2], 0, 8);
                    if (!land[5] && !land[6] && land[7]) g.DrawImage(Images.Coast[4, 2], 0, 8);
                    if (land[5] && !land[6] && land[7]) g.DrawImage(Images.Coast[5, 2], 0, 8);
                    if (!land[5] && land[6] && land[7]) g.DrawImage(Images.Coast[6, 2], 0, 8);
                    if (land[5] && land[6] && land[7]) g.DrawImage(Images.Coast[7, 2], 0, 8);

                    // NE+E+SE tiles
                    if (!land[1] && !land[2] && !land[3]) g.DrawImage(Images.Coast[0, 3], 32, 8);
                    if (land[1] && !land[2] && !land[3]) g.DrawImage(Images.Coast[1, 3], 32, 8);
                    if (!land[1] && land[2] && !land[3]) g.DrawImage(Images.Coast[2, 3], 32, 8);
                    if (land[1] && land[2] && !land[3]) g.DrawImage(Images.Coast[3, 3], 32, 8);
                    if (!land[1] && !land[2] && land[3]) g.DrawImage(Images.Coast[4, 3], 32, 8);
                    if (land[1] && !land[2] && land[3]) g.DrawImage(Images.Coast[5, 3], 32, 8);
                    if (!land[1] && land[2] && land[3]) g.DrawImage(Images.Coast[6, 3], 32, 8);
                    if (land[1] && land[2] && land[3]) g.DrawImage(Images.Coast[7, 3], 32, 8);

                    // River mouth
                    // If river is next to ocean, draw river mouth on this tile.
                    if (col + 1 < Xdim && row - 1 >= 0) // NE there is no edge of map
                    {
                        if (land[1] && Map.TileC2(col + 1, row - 1).River) g.DrawImage(Images.RiverMouth[0], 0, 0);
                    }

                    if (col + 1 < Xdim && row + 1 < Ydim) // SE there is no edge of map
                    {
                        if (land[3] && Map.TileC2(col + 1, row + 1).River) g.DrawImage(Images.RiverMouth[1], 0, 0);
                    }

                    if (col - 1 >= 0 && row + 1 < Ydim) // SW there is no edge of map
                    {
                        if (land[5] && Map.TileC2(col - 1, row + 1).River) g.DrawImage(Images.RiverMouth[2], 0, 0);
                    }

                    if (col - 1 >= 0 && row - 1 >= 0) // NW there is no edge of map
                    {
                        if (land[7] && Map.TileC2(col - 1, row - 1).River) g.DrawImage(Images.RiverMouth[3], 0, 0);
                    }

                    break;
                }
                // Draw forests
                case TerrainType.Forest:
                {
                    var forestIndex = IsTerrainAroundIn4directions(col, row, TerrainType.Forest, flatEarth);

                    // Draw forest tiles
                    g.DrawImage(Images.Forest[forestIndex], 0, 0);
                    break;
                }
                // Draw mountains
                case TerrainType.Mountains:
                {
                    var mountainsIndex = IsTerrainAroundIn4directions(col, row, TerrainType.Mountains, flatEarth);

                    // Draw mountain tiles
                    g.DrawImage(Images.Mountains[mountainsIndex], 0, 0);
                    break;
                }
                // Draw hills
                case TerrainType.Hills:
                {
                    var hillIndex = IsTerrainAroundIn4directions(col, row, TerrainType.Hills, flatEarth);

                    // Draw hill tiles
                    g.DrawImage(Images.Hills[hillIndex], 0, 0);
                    break;
                }

            }

            // Draw rivers
            if (tile.River)
            {
                var riverIndex = IsRiverAround(col, row, flatEarth);

                // Draw river tiles
                g.DrawImage(Images.River[riverIndex], 0, 0);
            }

            // Draw shield for grasslands
            
            if (tile.Type == TerrainType.Grassland)
            {
                if (tile.HasShield)
                {
                    g.DrawImage(Images.GrasslandShield, 0, 0);
                }
            }else if (tile.special != -1)
            {
                // Draw special resources if they exist
                g.DrawImage(terrainSet.Specials[tile.special][(int) tile.Type], 0, 0);
            }


            // Roads (cites also act as road tiles)
            if (tile.Road || tile.IsCityPresent)
            {
                bool[] isRoadAround = IsRoadAround(col, row, flatEarth);

                // Draw roads
                if (isRoadAround[0]) g.DrawImage(Images.Road[8], 0, 0);  // to N
                if (isRoadAround[1]) g.DrawImage(Images.Road[1], 0, 0);  // to NE
                if (isRoadAround[2]) g.DrawImage(Images.Road[2], 0, 0);  // to E
                if (isRoadAround[3]) g.DrawImage(Images.Road[3], 0, 0);  // to SE
                if (isRoadAround[4]) g.DrawImage(Images.Road[4], 0, 0);  // to S
                if (isRoadAround[5]) g.DrawImage(Images.Road[5], 0, 0);  // to SW
                if (isRoadAround[6]) g.DrawImage(Images.Road[6], 0, 0);  // to W
                if (isRoadAround[7]) g.DrawImage(Images.Road[7], 0, 0);  // to NW
                if (isRoadAround.SequenceEqual(new bool[8] { false, false, false, false, false, false, false, false }))
                {
                    g.DrawImage(Images.Road[0], 0, 0); // No road around
                }
            }

            // TODO: make railroad drawing logic
            // Railroads (cites also act as railroad tiles)
            //if (Map.TileC2(i, j).Railroad || Map.TileC2(i, j).CityPresent)
            //{
            //    bool[] isRailroadAround = IsRailroadAround(i, j);
            //
            //    // Draw railroads
            //    if (isRailroadAround[0]) g.DrawImage(Images.Railroad[8], 64 * i + 32 * (j % 2) + 1, 16 * j + 1);  // to N
            //    if (isRailroadAround[1]) g.DrawImage(Images.Railroad[1], 64 * i + 32 * (j % 2) + 1, 16 * j + 1);  // to NE
            //    if (isRailroadAround[2]) g.DrawImage(Images.Railroad[2], 64 * i + 32 * (j % 2) + 1, 16 * j + 1);  // to E
            //    if (isRailroadAround[3]) g.DrawImage(Images.Railroad[3], 64 * i + 32 * (j % 2) + 1, 16 * j + 1);  // to SE
            //    if (isRailroadAround[4]) g.DrawImage(Images.Railroad[4], 64 * i + 32 * (j % 2) + 1, 16 * j + 1);  // to S
            //    if (isRailroadAround[5]) g.DrawImage(Images.Railroad[5], 64 * i + 32 * (j % 2) + 1, 16 * j + 1);  // to SW
            //    if (isRailroadAround[6]) g.DrawImage(Images.Railroad[6], 64 * i + 32 * (j % 2) + 1, 16 * j + 1);  // to W
            //    if (isRailroadAround[7]) g.DrawImage(Images.Railroad[7], 64 * i + 32 * (j % 2) + 1, 16 * j + 1);  // to NW
            //    if (isRailroadAround.SequenceEqual(new bool[8] { false, false, false, false, false, false, false, false })) 
            //      g.DrawImage(Images.Railroad[0], 64 * i + 32 * (j % 2) + 1, 16 * j + 1);  // No railroad around
            //}

            // Irrigation
            if (tile.Irrigation) g.DrawImage(Images.Irrigation, 0, 0);

            // Farmland
            if (tile.Farmland) g.DrawImage(Images.Farmland, 0, 0);

            // Mining
            if (tile.Mining && !tile.Farmland) g.DrawImage(Images.Mining, 0, 0);

            // Pollution
            if (tile.Pollution) g.DrawImage(Images.Pollution, 0, 0);

            // Fortress
            if (tile.Fortress) g.DrawImage(MapImages.Specials[1], 0, 0);

            // Airbase
            else if (tile.Airbase) g.DrawImage(MapImages.Specials[tile.IsUnitPresent ? 3 : 2], 0, 0);

            return _tilePic;
        }

        private static void ApplyDither(Graphics g, TerrainType neighbourType, TerrainType tileType, IReadOnlyList<Bitmap> ditherMap, int offsetX, int offsetY)    
        {
            if (neighbourType == TerrainType.Ocean)
            {
                neighbourType = TerrainType.Grassland;
            }
            if(neighbourType == tileType) return;
            g.DrawImage(ditherMap[(int)neighbourType], offsetX, offsetY);
        }

        private static bool[] IsLandAround(int col, int row, bool flatEarth)
        {
            // In start we presume all surrounding tiles are water (land=true, water=false). Index=0 is North, follows in clockwise direction.
            bool[] land = new bool[8] { false, false, false, false, false, false, false, false };
            var indicies = new int[] {0, 0, 0, 0};

            int Xdim = 2 * Map.XDim;    // X=50 in markted as X=100 in Civ2
            int Ydim = Map.YDim;        // no need for such correction for Y

            // Observe in all directions if land is present next to ocean
            // N:
            if (row - 2 < 0)
            {
                land[0] = true;   // if N tile is out of map limits, we presume it is land
                indicies[0] += 2;
            }
            else if (Map.TileC2(col, row - 2).Type != TerrainType.Ocean)
            {
                land[0] = true;
                indicies[0] += 2;
            }
            // NE:
            if (row == 0)
            {
                land[1] = true;  // NE is beyond limits
            }
            else if (col == Xdim - 1)    // you are on eastern edge of map
            {
                if (flatEarth)
                {
                    land[1] = true;
                }
                else if (Map.TileC2(0, row - 1).Type != TerrainType.Ocean)
                {
                    land[1] = true;  // tile on mirror side of map is not ocean
                }
            }
            else if (Map.TileC2(col + 1, row - 1).Type != TerrainType.Ocean)
            {
                land[1] = true;    // if it is not ocean, it is land
            }
            // E:
            if (col + 2 >= Xdim) // you are on right edge of map
            {
                if (flatEarth)
                {
                    land[2] = true;
                }
                else if (Map.TileC2(Xdim - col, row).Type != TerrainType.Ocean)
                {
                    land[2] = true;
                }
            }
            else if (Map.TileC2(col + 2, row).Type != TerrainType.Ocean)
            {
                land[2] = true;
            }
            // SE:
            if (row == Ydim - 1)
            {
                land[3] = true;  // SE is beyond limits
            }
            else if (col + 1 == Xdim)    // you are on eastern edge of map
            {
                if (flatEarth)
                {
                    land[3] = true;
                }
                else if (Map.TileC2(0, row + 1).Type != TerrainType.Ocean)
                {
                    land[3] = true;  // tile on mirror side of map is not ocean
                }
            }
            else if (Map.TileC2(col + 1, row + 1).Type != TerrainType.Ocean)
            {
                land[3] = true;
            }
            // S:
            if (row + 2 >= Ydim)
            {
                land[4] = true;   // S is beyond map limits
            }
            else if (Map.TileC2(col, row + 2).Type != TerrainType.Ocean)
            {
                land[4] = true;
            }
            // SW:
            if (row == Ydim - 1)
            {
                land[5] = true; // SW is beyond limits
            }
            else if (col == 0)    // you are on western edge of map
            {
                if (flatEarth)
                {
                    land[5] = true;
                }
                else if (Map.TileC2(Xdim - 1, row + 1).Type != TerrainType.Ocean)
                {
                    land[5] = true;
                }
            }
            else if (Map.TileC2(col - 1, row + 1).Type != TerrainType.Ocean)
            {
                land[5] = true;
            }
            // W:
            if (col - 2 < 0) // you are on left edge of map
            {
                if (flatEarth)
                {
                    land[6] = true;
                }
                else if (Map.TileC2(Xdim - 2 + col, row).Type != TerrainType.Ocean)
                {
                    land[6] = true;
                }
            }
            else if (Map.TileC2(col - 2, row).Type != TerrainType.Ocean)
            {
                land[6] = true;
            }
            // NW:
            if (row == 0)
            {
                land[7] = true;  // NW is beyond limits
            }
            else if (col == 0) // you are on western edge of map
            {
                if (flatEarth)
                {
                    land[7] = true;
                }
                else if (Map.TileC2(Xdim - 1, row - 1).Type != TerrainType.Ocean)
                {
                    land[7] = true;
                }
            }
            else if (Map.TileC2(col - 1, row - 1).Type != TerrainType.Ocean)
            {
                land[7] = true;
            }

            return land;
        }

        // Check whether a certain type of terrain (mount./hills/forest) is present in 4 directions arount the tile
        private static int IsTerrainAroundIn4directions(int col, int row, TerrainType terrain, bool flatEarth)
        {
            // In start we presume all surrounding tiles are not of same type (=false). Index=0 is NE, follows in clockwise direction.
            var index = 0;

            // Rewrite indexes in Civ2-style
            int Xdim = 2 * Map.XDim;    // X=50 in markted as X=100 in Civ2
            int Ydim = Map.YDim;        // no need for such correction for Y

            // Observe in all directions if terrain is present
            // NE:
            if (row != 0)
            {
                if (col == Xdim - 1) // you are on eastern edge of map
                {
                    if (!flatEarth && Map.TileC2(0, row - 1).Type == terrain)
                    {
                        index = 1; // tile on mirror side of map
                    }
                }
                else if (Map.TileC2(col + 1, row - 1).Type == terrain)
                {
                    index = 1;
                }

                if (col == 0) // you are on western edge of map
                {
                    if (!flatEarth && Map.TileC2(Xdim - 1, row - 1).Type == terrain)
                    {
                        index += 8;
                    }
                }
                else if (Map.TileC2(col - 1, row - 1).Type == terrain)
                {
                    index += 8;
                }
            }

            if (row != Ydim - 1)
            {
                // SE:
                if (col == Xdim - 1) // you are on eastern edge of map
                {
                    if (!flatEarth && Map.TileC2(0, row + 1).Type == terrain)
                    {
                        index += 2; // tile on mirror side of map
                    }
                }
                else if (Map.TileC2(col + 1, row + 1).Type == terrain)
                {
                    index += 2;
                }

                // SW:
                if (col == 0) // you are on western edge of map
                {
                    if (!flatEarth && Map.TileC2(Xdim - 1, row + 1).Type == terrain)
                    {
                        index += 4;
                    }
                }
                else if (Map.TileC2(col - 1, row + 1).Type == terrain)
                {
                    index += 4;
                }
            }

            return index;
        }

        private static int IsRiverAround(int col, int row, bool flatEarth)
        {
            // In start we presume all surrounding tiles are not rivers (river=true, no river=false). Index=0 is NE, follows in clockwise direction.
            var river = 0;

            // Rewrite indexes in Civ2-style
            var xDim = 2 * Map.XDim; // X=50 in marked as X=100 in Civ2
            var yDim = Map.YDim; // no need for such correction for Y

            // Observe in all directions if river is present
            if (row != 0)
            { 
                // NE:
                if (col == xDim - 1) // you are on eastern edge of map
                {
                    if (!flatEarth && (Map.TileC2(0, row - 1).River || Map.TileC2(0, row - 1).Type == TerrainType.Ocean))
                    {
                        river = 1; // tile on mirror side of map
                    }
                }
                else if (Map.TileC2(col + 1, row - 1).River || Map.TileC2(col + 1, row - 1).Type == TerrainType.Ocean)
                {
                    river = 1;
                }
                
                
                // NW:
                if (col == 0) // you are on western edge of map
                {
                    if (!flatEarth && (Map.TileC2(xDim - 1, row - 1).River || Map.TileC2(xDim - 1, row - 1).Type == TerrainType.Ocean))
                    {
                        river += 8;
                    }
                }
                else if (Map.TileC2(col - 1, row - 1).River || Map.TileC2(col - 1, row - 1).Type == TerrainType.Ocean)
                {
                    river += 8;
                }
            }

            // SE:
            if (row != yDim - 1)
            {
                if (col == xDim - 1) // you are on eastern edge of map
                {
                    if (!flatEarth && (Map.TileC2(0, row + 1).River || Map.TileC2(0, row + 1).Type == TerrainType.Ocean))
                    {
                        // tile on mirror side of map
                        river += 2;
                    }
                }
                else if (Map.TileC2(col + 1, row + 1).River || Map.TileC2(col + 1, row + 1).Type == TerrainType.Ocean)
                {
                    river += 2;
                }

                if (col == 0) // you are on western edge of map
                {
                    if (!flatEarth && (Map.TileC2(xDim - 1, row + 1).River ||
                             Map.TileC2(xDim - 1, row + 1).Type == TerrainType.Ocean))
                    {
                        river += 4;
                    }
                }
                else if (Map.TileC2(col - 1, row + 1).River || Map.TileC2(col - 1, row + 1).Type == TerrainType.Ocean)
                {
                    river += 4;
                }
            }
            return river;
        }

        private static bool[] IsRoadAround(int col, int row, bool flatEarth)
        {
            // In start we presume all surrounding tiles do not have roads. Index=0 is NE, follows in clockwise direction.
            bool[] isRoadAround = new bool[8] { false, false, false, false, false, false, false, false };

            // Rewrite indexes in Civ2-style
            int Xdim = 2 * Map.XDim;    // X=50 in markted as X=100 in Civ2
            int Ydim = Map.YDim;        // no need for such correction for Y

            // Observe in all directions if road or city is present next to tile
            // N:
            if (row - 2 < 0)
            {
                isRoadAround[0] = false;   // if N tile is out of map limits
            }
            else if (Map.TileC2(col, row - 2).Road || Map.TileC2(col, row - 2).IsCityPresent)
            {
                isRoadAround[0] = true;
            }
            // NE:
            if (row == 0)
            {
                isRoadAround[1] = false;  // NE is beyond limits
            }
            else if (col == Xdim - 1)    // you are on eastern edge of map
            {
                if (flatEarth)
                {
                    isRoadAround[1] = false;
                }
                else if (Map.TileC2(0, row - 1).Road || Map.TileC2(0, row - 1).IsCityPresent)
                {
                    isRoadAround[1] = true;  // tile on mirror side of map
                }
            }
            else if (Map.TileC2(col + 1, row - 1).Road || Map.TileC2(col + 1, row - 1).IsCityPresent)
            {
                isRoadAround[1] = true;
            }
            // E:
            if (col + 2 >= Xdim) // you are on right edge of map
            {
                if (flatEarth)
                {
                    isRoadAround[2] = false;
                }
                else if (Map.TileC2(Xdim - col, row).Road || Map.TileC2(Xdim - col, row).IsCityPresent)
                {
                    isRoadAround[2] = true;
                }
            }
            else if (Map.TileC2(col + 2, row).Road || Map.TileC2(col + 2, row).IsCityPresent)
            {
                isRoadAround[2] = true;
            }
            // SE:
            if (row == Ydim - 1)
            {
                isRoadAround[3] = false;  // SE is beyond limits
            }
            else if (col + 1 == Xdim)    // you are on eastern edge of map
            {
                if (flatEarth)
                {
                    isRoadAround[3] = false;
                }
                else if (Map.TileC2(0, row + 1).Road || Map.TileC2(0, row + 1).IsCityPresent)
                {
                    isRoadAround[3] = true;  // tile on mirror side of map
                }
            }
            else if (Map.TileC2(col + 1, row + 1).Road || Map.TileC2(col + 1, row + 1).IsCityPresent)
            {
                isRoadAround[3] = true;
            }
            // S:
            if (row + 2 >= Ydim)
            {
                isRoadAround[4] = false;   // S is beyond map limits
            }
            else if (Map.TileC2(col, row + 2).Road || Map.TileC2(col, row + 2).IsCityPresent)
            {
                isRoadAround[4] = true;
            }
            // SW:
            if (row == Ydim - 1)
            {
                isRoadAround[5] = false; // SW is beyond limits
            }
            else if (col == 0)    // you are on western edge of map
            {
                if (flatEarth)
                {
                    isRoadAround[5] = false;
                }
                else if (Map.TileC2(Xdim - 1, row + 1).Road || Map.TileC2(Xdim - 1, row + 1).IsCityPresent)
                {
                    isRoadAround[5] = true;
                }
            }
            else if (Map.TileC2(col - 1, row + 1).Road || Map.TileC2(col - 1, row + 1).IsCityPresent)
            {
                isRoadAround[5] = true;
            }
            // W:
            if (col - 2 < 0) // you are on left edge of map
            {
                if (flatEarth)
                {
                    isRoadAround[6] = false;
                }
                else if (Map.TileC2(Xdim - 2 + col, row).Road || Map.TileC2(Xdim - 2 + col, row).IsCityPresent)
                {
                    isRoadAround[6] = true;
                }
            }
            else if (Map.TileC2(col - 2, row).Road || Map.TileC2(col - 2, row).IsCityPresent)
            {
                isRoadAround[6] = true;
            }
            // NW:
            if (row == 0)
            {
                isRoadAround[7] = false;  // NW is beyond limits
            }
            else if (col == 0) // you are on western edge of map
            {
                if (flatEarth)
                {
                    isRoadAround[7] = false;
                }
                else if (Map.TileC2(Xdim - 1, row - 1).Road || Map.TileC2(Xdim - 1, row - 1).IsCityPresent)
                {
                    isRoadAround[7] = true;
                }
            }
            else if (Map.TileC2(col - 1, row - 1).Road || Map.TileC2(col - 1, row - 1).IsCityPresent)
            {
                isRoadAround[7] = true;
            }

            return isRoadAround;
        }

        private static bool[] IsRailroadAround(int col, int row, bool flatEarth)
        {
            // In start we presume all surrounding tiles do not have railroads. Index=0 is NE, follows in clockwise direction.
            bool[] isRailroadAround = new bool[8] { false, false, false, false, false, false, false, false };

            // Rewrite indexes in Civ2-style
            int Xdim = 2 * Map.XDim;    // X=50 in markted as X=100 in Civ2
            int Ydim = Map.YDim;        // no need for such correction for Y

            // Observe in all directions if railroad or city is present next to tile
            // N:
            if (row - 2 < 0)
            {
                isRailroadAround[0] = false;   // if N tile is out of map limits
            }
            else if (Map.TileC2(col, row - 2).Railroad || Map.TileC2(col, row - 2).IsCityPresent)
            {
                isRailroadAround[0] = true;
            }
            // NE:
            if (row == 0)
            {
                isRailroadAround[1] = false;  // NE is beyond limits
            }
            else if (col == Xdim - 1)    // you are on eastern edge of map
            {
                if (flatEarth)
                {
                    isRailroadAround[1] = false;
                }
                else if (Map.TileC2(0, row - 1).Railroad || Map.TileC2(0, row - 1).IsCityPresent)
                {
                    isRailroadAround[1] = true;  // tile on mirror side of map
                }
            }
            else if (Map.TileC2(col + 1, row - 1).Railroad || Map.TileC2(col + 1, row - 1).IsCityPresent)
            {
                isRailroadAround[1] = true;
            }
            // E:
            if (col + 2 >= Xdim) // you are on right edge of map
            {
                if (flatEarth)
                {
                    isRailroadAround[2] = false;
                }
                else if (Map.TileC2(Xdim - col, row).Railroad || Map.TileC2(Xdim - col, row).IsCityPresent)
                {
                    isRailroadAround[2] = true;
                }
            }
            else if (Map.TileC2(col + 2, row).Railroad || Map.TileC2(col + 2, row).IsCityPresent)
            {
                isRailroadAround[2] = true;
            }
            // SE:
            if (row == Ydim - 1)
            {
                isRailroadAround[3] = false;  // SE is beyond limits
            }
            else if (col + 1 == Xdim)    // you are on eastern edge of map
            {
                if (flatEarth)
                {
                    isRailroadAround[3] = false;
                }
                else if (Map.TileC2(0, row + 1).Railroad || Map.TileC2(0, row + 1).IsCityPresent)
                {
                    isRailroadAround[3] = true;  // tile on mirror side of map
                }
            }
            else if (Map.TileC2(col + 1, row + 1).Railroad || Map.TileC2(col + 1, row + 1).IsCityPresent)
            {
                isRailroadAround[3] = true;
            }
            // S:
            if (row + 2 >= Ydim)
            {
                isRailroadAround[4] = false;   // S is beyond map limits
            }
            else if (Map.TileC2(col, row + 2).Railroad || Map.TileC2(col, row + 2).IsCityPresent)
            {
                isRailroadAround[4] = true;
            }
            // SW:
            if (row == Ydim - 1)
            {
                isRailroadAround[5] = false; // SW is beyond limits
            }
            else if (col == 0)    // you are on western edge of map
            {
                if (flatEarth)
                {
                    isRailroadAround[5] = false;
                }
                else if (Map.TileC2(Xdim - 1, row + 1).Railroad || Map.TileC2(Xdim - 1, row + 1).IsCityPresent)
                {
                    isRailroadAround[5] = true;
                }
            }
            else if (Map.TileC2(col - 1, row + 1).Railroad || Map.TileC2(col - 1, row + 1).IsCityPresent)
            {
                isRailroadAround[5] = true;
            }
            // W:
            if (col - 2 < 0) // you are on left edge of map
            {
                if (flatEarth)
                {
                    isRailroadAround[6] = false;
                }
                else if (Map.TileC2(Xdim - 2 + col, row).Railroad || Map.TileC2(Xdim - 2 + col, row).IsCityPresent)
                {
                    isRailroadAround[6] = true;
                }
            }
            else if (Map.TileC2(col - 2, row).Railroad || Map.TileC2(col - 2, row).IsCityPresent)
            {
                isRailroadAround[6] = true;
            }
            // NW:
            if (row == 0)
            {
                isRailroadAround[7] = false;  // NW is beyond limits
            }
            else if (col == 0) // you are on western edge of map
            {
                if (flatEarth)
                {
                    isRailroadAround[7] = false;
                }
                else if (Map.TileC2(Xdim - 1, row - 1).Railroad || Map.TileC2(Xdim - 1, row - 1).IsCityPresent)
                {
                    isRailroadAround[7] = true;
                }
            }
            else if (Map.TileC2(col - 1, row - 1).Railroad || Map.TileC2(col - 1, row - 1).IsCityPresent)
            {
                isRailroadAround[7] = true;
            }

            return isRailroadAround;
        }
    }
}
