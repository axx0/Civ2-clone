using System;
using System.Collections.Generic;
using Eto.Drawing;
using System.Linq;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Improvements;
using Civ2engine.MapObjects;
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

        public static Bitmap MakeTileGraphic(Tile tile, int arrayCol, int row, bool flatEarth, TerrainSet terrainSet)
        {
            // EVERYTHING HERE IS IN CIV2-COORDS AND NOT IN REGULAR COORDS!!!

            // First convert regular coords to civ2-style
            int col = 2 * arrayCol + row % 2; // you don't change row
            var map = Map;
            int Xdim = map.XDimMax;
            int Ydim = map.YDim;

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
                    if (col != 0 && row != 0) ApplyDither(g,map.TileC2(col - 1, row - 1).Type, tile.Type, terrainSet.DitherMaps[0], 0, 0);
                    // Determine type of NE tile
                    if (col != Xdim - 1 && (row != 0)) ApplyDither(g, map.TileC2(col + 1, row - 1).Type, tile.Type, terrainSet.DitherMaps[1], 32, 0);
                    // Determine type of SW tile
                    if (col != 0 && (row != Ydim - 1)) ApplyDither(g, map.TileC2(col - 1, row + 1).Type, tile.Type, terrainSet.DitherMaps[2], 0, 16);
                    // Determine type of SE tile
                    if (col != Xdim - 1 && (row != Ydim - 1)) ApplyDither(g,map.TileC2(col + 1, row + 1).Type, tile.Type, terrainSet.DitherMaps[3], 32, 16);
                }
                else // Round earth
                {
                    if (row != 0)
                    {
                        ApplyDither(g, map.TileC2((col == 0 ? Xdim : col) - 1, row - 1).Type, tile.Type,
                            terrainSet.DitherMaps[0], 0, 0);

                        ApplyDither(g, map.TileC2(col == Xdim - 1 ? 0 : col + 1, row - 1).Type, tile.Type,
                            terrainSet.DitherMaps[1], 32, 0);
                    }

                    if (row != Ydim - 1)
                    {
                        ApplyDither(g, map.TileC2((col == 0 ? Xdim : col) - 1, row + 1).Type, tile.Type,
                            terrainSet.DitherMaps[2], 0, 16);

                        ApplyDither(g, map.TileC2(col == Xdim - 1 ? 0 : col + 1, row + 1).Type, tile.Type,
                            terrainSet.DitherMaps[3], 32, 16);
                    }
                }
            }

            switch (tile.Type)
            {
                // Draw coast & river mouth
                case TerrainType.Ocean:
                {
                    var land = IsLandAround(col, row, flatEarth); // Determine if land is present in 8 directions

                    // Draw coast & river mouth tiles
                    // NW+N+NE tiles
                    if (!land[7] && !land[0] && !land[1]) g.DrawImage(terrainSet.Coast[0, 0], 16, 0);
                    if (land[7] && !land[0] && !land[1]) g.DrawImage(terrainSet.Coast[1, 0], 16, 0);
                    if (!land[7] && land[0] && !land[1]) g.DrawImage(terrainSet.Coast[2, 0], 16, 0);
                    if (land[7] && land[0] && !land[1]) g.DrawImage(terrainSet.Coast[3, 0], 16, 0);
                    if (!land[7] && !land[0] && land[1]) g.DrawImage(terrainSet.Coast[4, 0], 16, 0);
                    if (land[7] && !land[0] && land[1]) g.DrawImage(terrainSet.Coast[5, 0], 16, 0);
                    if (!land[7] && land[0] && land[1]) g.DrawImage(terrainSet.Coast[6, 0], 16, 0);
                    if (land[7] && land[0] && land[1]) g.DrawImage(terrainSet.Coast[7, 0], 16, 0);

                    // SW+S+SE tiles
                    if (!land[3] && !land[4] && !land[5]) g.DrawImage(terrainSet.Coast[0, 1], 16, 16);
                    if (land[3] && !land[4] && !land[5]) g.DrawImage(terrainSet.Coast[1, 1], 16, 16);
                    if (!land[3] && land[4] && !land[5]) g.DrawImage(terrainSet.Coast[2, 1], 16, 16);
                    if (land[3] && land[4] && !land[5]) g.DrawImage(terrainSet.Coast[3, 1], 16, 16);
                    if (!land[3] && !land[4] && land[5]) g.DrawImage(terrainSet.Coast[4, 1], 16, 16);
                    if (land[3] && !land[4] && land[5]) g.DrawImage(terrainSet.Coast[5, 1], 16, 16);
                    if (!land[3] && land[4] && land[5]) g.DrawImage(terrainSet.Coast[6, 1], 16, 16);
                    if (land[3] && land[4] && land[5]) g.DrawImage(terrainSet.Coast[7, 1], 16, 16);

                    // SW+W+NW tiles
                    if (!land[5] && !land[6] && !land[7]) g.DrawImage(terrainSet.Coast[0, 2], 0, 8);
                    if (land[5] && !land[6] && !land[7]) g.DrawImage(terrainSet.Coast[1, 2], 0, 8);
                    if (!land[5] && land[6] && !land[7]) g.DrawImage(terrainSet.Coast[2, 2], 0, 8);
                    if (land[5] && land[6] && !land[7]) g.DrawImage(terrainSet.Coast[3, 2], 0, 8);
                    if (!land[5] && !land[6] && land[7]) g.DrawImage(terrainSet.Coast[4, 2], 0, 8);
                    if (land[5] && !land[6] && land[7]) g.DrawImage(terrainSet.Coast[5, 2], 0, 8);
                    if (!land[5] && land[6] && land[7]) g.DrawImage(terrainSet.Coast[6, 2], 0, 8);
                    if (land[5] && land[6] && land[7]) g.DrawImage(terrainSet.Coast[7, 2], 0, 8);

                    // NE+E+SE tiles
                    if (!land[1] && !land[2] && !land[3]) g.DrawImage(terrainSet.Coast[0, 3], 32, 8);
                    if (land[1] && !land[2] && !land[3]) g.DrawImage(terrainSet.Coast[1, 3], 32, 8);
                    if (!land[1] && land[2] && !land[3]) g.DrawImage(terrainSet.Coast[2, 3], 32, 8);
                    if (land[1] && land[2] && !land[3]) g.DrawImage(terrainSet.Coast[3, 3], 32, 8);
                    if (!land[1] && !land[2] && land[3]) g.DrawImage(terrainSet.Coast[4, 3], 32, 8);
                    if (land[1] && !land[2] && land[3]) g.DrawImage(terrainSet.Coast[5, 3], 32, 8);
                    if (!land[1] && land[2] && land[3]) g.DrawImage(terrainSet.Coast[6, 3], 32, 8);
                    if (land[1] && land[2] && land[3]) g.DrawImage(terrainSet.Coast[7, 3], 32, 8);

                    // River mouth
                    // If river is next to ocean, draw river mouth on this tile.
                    if (col + 1 < Xdim && row - 1 >= 0) // NE there is no edge of map
                    {
                        if (land[1] && map.TileC2(col + 1, row - 1).River) g.DrawImage(terrainSet.RiverMouth[0], 0, 0);
                    }

                    if (col + 1 < Xdim && row + 1 < Ydim) // SE there is no edge of map
                    {
                        if (land[3] && map.TileC2(col + 1, row + 1).River) g.DrawImage(terrainSet.RiverMouth[1], 0, 0);
                    }

                    if (col - 1 >= 0 && row + 1 < Ydim) // SW there is no edge of map
                    {
                        if (land[5] && map.TileC2(col - 1, row + 1).River) g.DrawImage(terrainSet.RiverMouth[2], 0, 0);
                    }

                    if (col - 1 >= 0 && row - 1 >= 0) // NW there is no edge of map
                    {
                        if (land[7] && map.TileC2(col - 1, row - 1).River) g.DrawImage(terrainSet.RiverMouth[3], 0, 0);
                    }

                    break;
                }
                // Draw forests
                case TerrainType.Forest:
                {
                    var forestIndex = IsTerrainAroundIn4directions(col, row, TerrainType.Forest, flatEarth);

                    // Draw forest tiles
                    g.DrawImage(terrainSet.Forest[forestIndex], 0, 0);
                    break;
                }
                // Draw mountains
                case TerrainType.Mountains:
                {
                    var mountainsIndex = IsTerrainAroundIn4directions(col, row, TerrainType.Mountains, flatEarth);

                    // Draw mountain tiles
                    g.DrawImage(terrainSet.Mountains[mountainsIndex], 0, 0);
                    break;
                }
                // Draw hills
                case TerrainType.Hills:
                {
                    var hillIndex = IsTerrainAroundIn4directions(col, row, TerrainType.Hills, flatEarth);

                    // Draw hill tiles
                    g.DrawImage(terrainSet.Hills[hillIndex], 0, 0);
                    break;
                }
            }

            // Draw rivers
            if (tile.River)
            {
                var riverIndex = IsRiverAround(col, row, flatEarth);

                // Draw river tiles
                g.DrawImage(terrainSet.River[riverIndex], 0, 0);
            }

            // Draw shield for grasslands
            
            if (tile.Type == TerrainType.Grassland)
            {
                if (tile.HasShield)
                {
                    g.DrawImage(terrainSet.GrasslandShield, 0, 0);
                }
            }else if (tile.Special != -1)
            {
                // Draw special resources if they exist
                g.DrawImage(terrainSet.Specials[tile.Special][(int) tile.Type], 0, 0);
            }


            var improvements = tile.Improvements
                .Where(ci => Game.TerrainImprovements.ContainsKey(ci.Improvement))
                .OrderBy(ci => Game.TerrainImprovements[ci.Improvement].Layer).ToList();

            foreach (var construct in improvements)
            {
                var improvement = Game.TerrainImprovements[construct.Improvement];
                var graphics = terrainSet.ImprovementsMap[construct.Improvement];

                if (improvement.HasMultiTile)
                {
                    bool hasNeighbours = false;

                    foreach (var neighbour in tile.Map.Neighbours(tile))
                    {
                        var neighboringImprovement =
                            neighbour.Improvements.FirstOrDefault(i =>
                                i.Improvement == construct.Improvement);
                        if (neighboringImprovement != null)
                        {
                            var index = GetCoordsFromDifference(neighbour.X-tile.X  , neighbour.Y - tile.Y );
                            if (index != -1)
                            {
                                if (neighboringImprovement.Level < construct.Level)
                                {
                                    g.DrawImage(graphics.Levels[neighboringImprovement.Level, index],0,0);
                                }
                                else
                                {
                                    hasNeighbours = true;
                                    g.DrawImage(graphics.Levels[construct.Level, index],0,0);
                                }
                            }
                        }
                    }

                    if (!hasNeighbours)
                    {
                        g.DrawImage(graphics.Levels[construct.Level, 0],0,0);
                    }
                }
                else
                {
                    if (tile.IsUnitPresent && graphics.UnitLevels != null)
                    {
                        g.DrawImage(graphics.UnitLevels[construct.Level, 0], 0, 0);
                    }
                    else
                    {
                        g.DrawImage(graphics.Levels[construct.Level, 0], 0, 0);
                    }
                }
            }
            
            
            // Roads (cites also act as road tiles)
            // if (tile.Road || tile.IsCityPresent)
            // {
            //     bool[] isRoadAround = IsRoadAround(col, row, flatEarth);
            //
            //     // Draw roads
            //     if (isRoadAround[0]) g.DrawImage(terrainSet.Road[8], 0, 0);  // to N
            //     if (isRoadAround[1]) g.DrawImage(terrainSet.Road[1], 0, 0);  // to NE
            //     if (isRoadAround[2]) g.DrawImage(terrainSet.Road[2], 0, 0);  // to E
            //     if (isRoadAround[3]) g.DrawImage(terrainSet.Road[3], 0, 0);  // to SE
            //     if (isRoadAround[4]) g.DrawImage(terrainSet.Road[4], 0, 0);  // to S
            //     if (isRoadAround[5]) g.DrawImage(terrainSet.Road[5], 0, 0);  // to SW
            //     if (isRoadAround[6]) g.DrawImage(terrainSet.Road[6], 0, 0);  // to W
            //     if (isRoadAround[7]) g.DrawImage(terrainSet.Road[7], 0, 0);  // to NW
            //     if (isRoadAround.SequenceEqual(new bool[8] { false, false, false, false, false, false, false, false }))
            //     {
            //         g.DrawImage(terrainSet.Road[0], 0, 0); // No road around
            //     }
            // }
            //
            // // TODO: make railroad drawing logic
            // // Railroads (cites also act as railroad tiles)
            // //if (Map.TileC2(i, j).Railroad || Map.TileC2(i, j).CityPresent)
            // //{
            // //    bool[] isRailroadAround = IsRailroadAround(i, j);
            // //
            // //    // Draw railroads
            // //    if (isRailroadAround[0]) g.DrawImage(terrainSet.Railroad[8], 64 * i + 32 * (j % 2) + 1, 16 * j + 1);  // to N
            // //    if (isRailroadAround[1]) g.DrawImage(terrainSet.Railroad[1], 64 * i + 32 * (j % 2) + 1, 16 * j + 1);  // to NE
            // //    if (isRailroadAround[2]) g.DrawImage(terrainSet.Railroad[2], 64 * i + 32 * (j % 2) + 1, 16 * j + 1);  // to E
            // //    if (isRailroadAround[3]) g.DrawImage(terrainSet.Railroad[3], 64 * i + 32 * (j % 2) + 1, 16 * j + 1);  // to SE
            // //    if (isRailroadAround[4]) g.DrawImage(terrainSet.Railroad[4], 64 * i + 32 * (j % 2) + 1, 16 * j + 1);  // to S
            // //    if (isRailroadAround[5]) g.DrawImage(terrainSet.Railroad[5], 64 * i + 32 * (j % 2) + 1, 16 * j + 1);  // to SW
            // //    if (isRailroadAround[6]) g.DrawImage(terrainSet.Railroad[6], 64 * i + 32 * (j % 2) + 1, 16 * j + 1);  // to W
            // //    if (isRailroadAround[7]) g.DrawImage(terrainSet.Railroad[7], 64 * i + 32 * (j % 2) + 1, 16 * j + 1);  // to NW
            // //    if (isRailroadAround.SequenceEqual(new bool[8] { false, false, false, false, false, false, false, false })) 
            // //      g.DrawImage(terrainSet.Railroad[0], 64 * i + 32 * (j % 2) + 1, 16 * j + 1);  // No railroad around
            // //}
            //
            // // Irrigation
            // if (tile.Irrigation) g.DrawImage(terrainSet.Irrigation, 0, 0);
            //
            // // Farmland
            // if (tile.Farmland) g.DrawImage(terrainSet.Farmland, 0, 0);
            //
            // // Mining
            // if (tile.Mining && !tile.Farmland) g.DrawImage(terrainSet.Mining, 0, 0);

            // Pollution
            //if (tile.Pollution) g.DrawImage(terrainSet.Pollution, 0, 0);

            // Fortress


            return _tilePic;
        }

        private static int GetCoordsFromDifference(int deltaX, int deltaY)
        {
            return deltaX switch
            {
                1 when deltaY == -1 => 1,
                2 when deltaY == 0 => 2,
                1 when deltaY == 1 => 3,
                0 when deltaY == 2 => 4,
                -1 when deltaY == 1 => 5,
                -2 when deltaY == 0 => 6,
                -1 when deltaY == -1 => 7,
                0 when deltaY == -2 => 8,
                _ => -1
            };
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

            int Xdim = Map.XDimMax;    // X=50 in markted as X=100 in Civ2
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
            int Xdim = Map.XDimMax;    // X=50 in markted as X=100 in Civ2
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
            var xDim = Map.XDimMax; // X=50 in marked as X=100 in Civ2
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
    }
}
