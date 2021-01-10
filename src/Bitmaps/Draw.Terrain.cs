using System;
using System.Drawing;
using System.Linq;
using civ2.Enums;
using civ2.Terrains;

namespace civ2.Bitmaps
{
    public static partial class Draw
    {
        public static Bitmap Terrain(ITerrain tile, int col, int row, bool flatEarth)
        {
            // EVERYTHING HERE IS IN CIV2-COORDS AND NOT IN REGULAR COORDS!!!

            // First convert regular coords to civ2-style
            col = 2 * col + row % 2;   // you don't change row
            int Xdim = 2 * Map.Xdim;
            int Ydim = Map.Ydim;

            // Define a bitmap for drawing
            Bitmap tilePic = new Bitmap(64, 32);

            // Draw tile
            using (var g = Graphics.FromImage(tilePic))
            {
                switch (tile.Type)
                {
                    case TerrainType.Desert: g.DrawImage(Images.Desert[0], 0, 0); break;
                    case TerrainType.Forest: g.DrawImage(Images.ForestBase[0], 0, 0); break;
                    case TerrainType.Glacier: g.DrawImage(Images.Glacier[0], 0, 0); break;
                    case TerrainType.Grassland: g.DrawImage(Images.Grassland[0], 0, 0); break;
                    case TerrainType.Hills: g.DrawImage(Images.HillsBase[0], 0, 0); break;
                    case TerrainType.Jungle: g.DrawImage(Images.Jungle[0], 0, 0); break;
                    case TerrainType.Mountains: g.DrawImage(Images.MtnsBase[0], 0, 0); break;
                    case TerrainType.Ocean: g.DrawImage(Images.Ocean[0], 0, 0); break;
                    case TerrainType.Plains: g.DrawImage(Images.Plains[0], 0, 0); break;
                    case TerrainType.Swamp: g.DrawImage(Images.Swamp[0], 0, 0); break;
                    case TerrainType.Tundra: g.DrawImage(Images.Tundra[0], 0, 0); break;
                    default: throw new ArgumentOutOfRangeException();
                }

                // Dither
                // Determine type of terrain in all 4 directions. Be careful if you're on map edge.
                TerrainType?[,] tiletype = new TerrainType?[2, 2] { { null, null }, { null, null } };   // null = beyond map limits
                if (flatEarth)
                {
                    // Determine type of NW tile
                    if ((col != 0) && (row != 0))
                    {
                        tiletype[0, 0] = Map.TileC2(col - 1, row - 1).Type;
                    }
                    // Determine type of NE tile
                    if ((col != Xdim - 1) && (row != 0))
                    {
                        tiletype[1, 0] = Map.TileC2(col + 1, row - 1).Type;
                    }
                    // Determine type of SW tile
                    if ((col != 0) && (row != Ydim - 1))
                    {
                        tiletype[0, 1] = Map.TileC2(col - 1, row + 1).Type;
                    }
                    // Determine type of SE tile
                    if ((col != Xdim - 1) && (row != Ydim - 1))
                    {
                        tiletype[1, 1] = Map.TileC2(col + 1, row + 1).Type;
                    }
                }
                else    // Round earth
                {
                    // Determine type of NW tile
                    if ((col == 0) && (row != 0))
                    {
                        tiletype[0, 0] = Map.TileC2(Xdim - 1, row - 1).Type;   // if on left edge take tile from other side of map
                    }
                    else if ((col != 0) && (row != 0))
                    {
                        tiletype[0, 0] = Map.TileC2(col - 1, row - 1).Type;
                    }
                    // Determine type of NE tile
                    if ((col == Xdim - 1) && (row != 0))
                    {
                        tiletype[1, 0] = Map.TileC2(0, row - 1).Type;   // if on right edge take tile from other side of map
                    }
                    else if ((col != Xdim - 1) && (row != 0))
                    {
                        tiletype[1, 0] = Map.TileC2(col + 1, row - 1).Type;
                    }
                    // Determine type of SW tile
                    if ((col == 0) && (row != Ydim - 1))
                    {
                        tiletype[0, 1] = Map.TileC2(Xdim - 1, row + 1).Type;   // if on left edge take tile from other side of map
                    }
                    else if ((col != 0) && (row != Ydim - 1))
                    {
                        tiletype[0, 1] = Map.TileC2(col - 1, row + 1).Type;
                    }
                    // Determine type of SE tile
                    if ((col == Xdim - 1) && (row != Ydim - 1))
                    {
                        tiletype[1, 1] = Map.TileC2(0, row + 1).Type;  // if on right edge take tile from other side of map
                    }
                    else if ((col != Xdim - 1) && (row != Ydim - 1))
                    {
                        tiletype[1, 1] = Map.TileC2(col + 1, row + 1).Type;
                    }
                }
                // Implement dither on 4 locations in square
                for (int tileX = 0; tileX < 2; tileX++)    // for 4 directions
                {
                    for (int tileY = 0; tileY < 2; tileY++)
                    {
                        switch (tiletype[tileX, tileY])
                        {
                            case TerrainType.Desert: g.DrawImage(Images.DitherDesert[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Plains: g.DrawImage(Images.DitherPlains[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Grassland: g.DrawImage(Images.DitherGrassland[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Forest: g.DrawImage(Images.DitherForest[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Hills: g.DrawImage(Images.DitherHills[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Mountains: g.DrawImage(Images.DitherMountains[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Tundra: g.DrawImage(Images.DitherTundra[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Glacier: g.DrawImage(Images.DitherGlacier[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Swamp: g.DrawImage(Images.DitherSwamp[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Jungle: g.DrawImage(Images.DitherJungle[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            case TerrainType.Ocean: g.DrawImage(Images.DitherGrassland[tileX, tileY], 32 * tileX, 16 * tileY); break;
                            default: break;
                        }
                    }
                }

                // Draw coast & river mouth
                if (Map.TileC2(col, row).Type == TerrainType.Ocean)
                {
                    bool[] land = IsLandAround(col, row, flatEarth);   // Determine if land is present in 8 directions

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
                    if (col + 1 < Xdim && row - 1 >= 0)    // NE there is no edge of map
                    {
                        if (land[1] && Map.TileC2(col + 1, row - 1).River) g.DrawImage(Images.RiverMouth[0], 0, 0);
                    }
                    if (col + 1 < Xdim && row + 1 < Ydim)    // SE there is no edge of map
                    {
                        if (land[3] && Map.TileC2(col + 1, row + 1).River) g.DrawImage(Images.RiverMouth[1], 0, 0);
                    }
                    if (col - 1 >= 0 && row + 1 < Ydim)    // SW there is no edge of map
                    {
                        if (land[5] && Map.TileC2(col - 1, row + 1).River) g.DrawImage(Images.RiverMouth[2], 0, 0);
                    }
                    if (col - 1 >= 0 && row - 1 >= 0)    // NW there is no edge of map
                    {
                        if (land[7] && Map.TileC2(col - 1, row - 1).River) g.DrawImage(Images.RiverMouth[3], 0, 0);
                    }
                }

                // Draw forests
                if (Map.TileC2(col, row).Type == TerrainType.Forest)
                {
                    bool[] isForestAround = IsTerrainAroundIn4directions(col, row, TerrainType.Forest, flatEarth);

                    // Draw forest tiles
                    if (isForestAround.SequenceEqual(new bool[4] { false, false, false, false })) g.DrawImage(Images.Forest[0], 0, 0);
                    if (isForestAround.SequenceEqual(new bool[4] { true, false, false, false })) g.DrawImage(Images.Forest[1], 0, 0);
                    if (isForestAround.SequenceEqual(new bool[4] { false, true, false, false })) g.DrawImage(Images.Forest[2], 0, 0);
                    if (isForestAround.SequenceEqual(new bool[4] { true, true, false, false })) g.DrawImage(Images.Forest[3], 0, 0);
                    if (isForestAround.SequenceEqual(new bool[4] { false, false, true, false })) g.DrawImage(Images.Forest[4], 0, 0);
                    if (isForestAround.SequenceEqual(new bool[4] { true, false, true, false })) g.DrawImage(Images.Forest[5], 0, 0);
                    if (isForestAround.SequenceEqual(new bool[4] { false, true, true, false })) g.DrawImage(Images.Forest[6], 0, 0);
                    if (isForestAround.SequenceEqual(new bool[4] { true, true, true, false })) g.DrawImage(Images.Forest[7], 0, 0);
                    if (isForestAround.SequenceEqual(new bool[4] { false, false, false, true })) g.DrawImage(Images.Forest[8], 0, 0);
                    if (isForestAround.SequenceEqual(new bool[4] { true, false, false, true })) g.DrawImage(Images.Forest[9], 0, 0);
                    if (isForestAround.SequenceEqual(new bool[4] { false, true, false, true })) g.DrawImage(Images.Forest[10], 0, 0);
                    if (isForestAround.SequenceEqual(new bool[4] { true, true, false, true })) g.DrawImage(Images.Forest[11], 0, 0);
                    if (isForestAround.SequenceEqual(new bool[4] { false, false, true, true })) g.DrawImage(Images.Forest[12], 0, 0);
                    if (isForestAround.SequenceEqual(new bool[4] { true, false, true, true })) g.DrawImage(Images.Forest[13], 0, 0);
                    if (isForestAround.SequenceEqual(new bool[4] { false, true, true, true })) g.DrawImage(Images.Forest[14], 0, 0);
                    if (isForestAround.SequenceEqual(new bool[4] { true, true, true, true })) g.DrawImage(Images.Forest[15], 0, 0);
                }

                // Draw mountains
                // TODO: correct drawing mountains - IF SHIELD IS AT MOUNTAIN IT SHOULD BE Mountains[2] and Mountains[3] !!!
                if (Map.TileC2(col, row).Type == TerrainType.Mountains)
                {
                    bool[] isMountAround = IsTerrainAroundIn4directions(col, row, TerrainType.Mountains, flatEarth);

                    // Draw mountain tiles
                    if (isMountAround.SequenceEqual(new bool[4] { false, false, false, false })) g.DrawImage(Images.Mountains[0], 0, 0);
                    if (isMountAround.SequenceEqual(new bool[4] { true, false, false, false })) g.DrawImage(Images.Mountains[1], 0, 0);
                    if (isMountAround.SequenceEqual(new bool[4] { false, true, false, false })) g.DrawImage(Images.Mountains[2], 0, 0);
                    if (isMountAround.SequenceEqual(new bool[4] { true, true, false, false })) g.DrawImage(Images.Mountains[3], 0, 0);
                    if (isMountAround.SequenceEqual(new bool[4] { false, false, true, false })) g.DrawImage(Images.Mountains[4], 0, 0);
                    if (isMountAround.SequenceEqual(new bool[4] { true, false, true, false })) g.DrawImage(Images.Mountains[5], 0, 0);
                    if (isMountAround.SequenceEqual(new bool[4] { false, true, true, false })) g.DrawImage(Images.Mountains[6], 0, 0);
                    if (isMountAround.SequenceEqual(new bool[4] { true, true, true, false })) g.DrawImage(Images.Mountains[7], 0, 0);
                    if (isMountAround.SequenceEqual(new bool[4] { false, false, false, true })) g.DrawImage(Images.Mountains[8], 0, 0);
                    if (isMountAround.SequenceEqual(new bool[4] { true, false, false, true })) g.DrawImage(Images.Mountains[9], 0, 0);
                    if (isMountAround.SequenceEqual(new bool[4] { false, true, false, true })) g.DrawImage(Images.Mountains[10], 0, 0);
                    if (isMountAround.SequenceEqual(new bool[4] { true, true, false, true })) g.DrawImage(Images.Mountains[11], 0, 0);
                    if (isMountAround.SequenceEqual(new bool[4] { false, false, true, true })) g.DrawImage(Images.Mountains[12], 0, 0);
                    if (isMountAround.SequenceEqual(new bool[4] { true, false, true, true })) g.DrawImage(Images.Mountains[13], 0, 0);
                    if (isMountAround.SequenceEqual(new bool[4] { false, true, true, true })) g.DrawImage(Images.Mountains[14], 0, 0);
                    if (isMountAround.SequenceEqual(new bool[4] { true, true, true, true })) g.DrawImage(Images.Mountains[15], 0, 0);
                }

                // Draw hills
                if (Map.TileC2(col, row).Type == TerrainType.Hills)
                {
                    bool[] isHillAround = IsTerrainAroundIn4directions(col, row, TerrainType.Hills, flatEarth);

                    // Draw hill tiles
                    if (isHillAround.SequenceEqual(new bool[4] { false, false, false, false })) g.DrawImage(Images.Hills[0], 0, 0);
                    if (isHillAround.SequenceEqual(new bool[4] { true, false, false, false })) g.DrawImage(Images.Hills[1], 0, 0);
                    if (isHillAround.SequenceEqual(new bool[4] { false, true, false, false })) g.DrawImage(Images.Hills[2], 0, 0);
                    if (isHillAround.SequenceEqual(new bool[4] { true, true, false, false })) g.DrawImage(Images.Hills[3], 0, 0);
                    if (isHillAround.SequenceEqual(new bool[4] { false, false, true, false })) g.DrawImage(Images.Hills[4], 0, 0);
                    if (isHillAround.SequenceEqual(new bool[4] { true, false, true, false })) g.DrawImage(Images.Hills[5], 0, 0);
                    if (isHillAround.SequenceEqual(new bool[4] { false, true, true, false })) g.DrawImage(Images.Hills[6], 0, 0);
                    if (isHillAround.SequenceEqual(new bool[4] { true, true, true, false })) g.DrawImage(Images.Hills[7], 0, 0);
                    if (isHillAround.SequenceEqual(new bool[4] { false, false, false, true })) g.DrawImage(Images.Hills[8], 0, 0);
                    if (isHillAround.SequenceEqual(new bool[4] { true, false, false, true })) g.DrawImage(Images.Hills[9], 0, 0);
                    if (isHillAround.SequenceEqual(new bool[4] { false, true, false, true })) g.DrawImage(Images.Hills[10], 0, 0);
                    if (isHillAround.SequenceEqual(new bool[4] { true, true, false, true })) g.DrawImage(Images.Hills[11], 0, 0);
                    if (isHillAround.SequenceEqual(new bool[4] { false, false, true, true })) g.DrawImage(Images.Hills[12], 0, 0);
                    if (isHillAround.SequenceEqual(new bool[4] { true, false, true, true })) g.DrawImage(Images.Hills[13], 0, 0);
                    if (isHillAround.SequenceEqual(new bool[4] { false, true, true, true })) g.DrawImage(Images.Hills[14], 0, 0);
                    if (isHillAround.SequenceEqual(new bool[4] { true, true, true, true })) g.DrawImage(Images.Hills[15], 0, 0);
                }

                // Draw rivers
                if (Map.TileC2(col, row).River)
                {
                    bool[] isRiverAround = IsRiverAround(col, row, flatEarth);

                    // Draw river tiles
                    if (isRiverAround.SequenceEqual(new bool[4] { false, false, false, false })) g.DrawImage(Images.River[0], 0, 0);
                    if (isRiverAround.SequenceEqual(new bool[4] { true, false, false, false })) g.DrawImage(Images.River[1], 0, 0);
                    if (isRiverAround.SequenceEqual(new bool[4] { false, true, false, false })) g.DrawImage(Images.River[2], 0, 0);
                    if (isRiverAround.SequenceEqual(new bool[4] { true, true, false, false })) g.DrawImage(Images.River[3], 0, 0);
                    if (isRiverAround.SequenceEqual(new bool[4] { false, false, true, false })) g.DrawImage(Images.River[4], 0, 0);
                    if (isRiverAround.SequenceEqual(new bool[4] { true, false, true, false })) g.DrawImage(Images.River[5], 0, 0);
                    if (isRiverAround.SequenceEqual(new bool[4] { false, true, true, false })) g.DrawImage(Images.River[6], 0, 0);
                    if (isRiverAround.SequenceEqual(new bool[4] { true, true, true, false })) g.DrawImage(Images.River[7], 0, 0);
                    if (isRiverAround.SequenceEqual(new bool[4] { false, false, false, true })) g.DrawImage(Images.River[8], 0, 0);
                    if (isRiverAround.SequenceEqual(new bool[4] { true, false, false, true })) g.DrawImage(Images.River[9], 0, 0);
                    if (isRiverAround.SequenceEqual(new bool[4] { false, true, false, true })) g.DrawImage(Images.River[10], 0, 0);
                    if (isRiverAround.SequenceEqual(new bool[4] { true, true, false, true })) g.DrawImage(Images.River[11], 0, 0);
                    if (isRiverAround.SequenceEqual(new bool[4] { false, false, true, true })) g.DrawImage(Images.River[12], 0, 0);
                    if (isRiverAround.SequenceEqual(new bool[4] { true, false, true, true })) g.DrawImage(Images.River[13], 0, 0);
                    if (isRiverAround.SequenceEqual(new bool[4] { false, true, true, true })) g.DrawImage(Images.River[14], 0, 0);
                    if (isRiverAround.SequenceEqual(new bool[4] { true, true, true, true })) g.DrawImage(Images.River[15], 0, 0);
                }

                // Draw special resources if they exist
                Map.TileC2(col, row).SpecType = null; // TODO: temporary
                if (Map.TileC2(col, row).SpecType != null)
                {
                    switch (Map.TileC2(col, row).SpecType)
                    {
                        case SpecialType.Oasis: g.DrawImage(Images.Desert[2], 0, 0); break;
                        case SpecialType.Buffalo: g.DrawImage(Images.Plains[2], 0, 0); break;
                        case SpecialType.Grassland: g.DrawImage(Images.Grassland[0], 0, 0); break;   // TODO: what is spectype for grassland?
                        case SpecialType.Pheasant: g.DrawImage(Images.ForestBase[2], 0, 0); break;
                        case SpecialType.Coal: g.DrawImage(Images.HillsBase[2], 0, 0); break;
                        case SpecialType.Gold: g.DrawImage(Images.MtnsBase[2], 0, 0); break;
                        case SpecialType.Game: g.DrawImage(Images.Tundra[2], 0, 0); break;
                        case SpecialType.Ivory: g.DrawImage(Images.Glacier[2], 0, 0); break;
                        case SpecialType.Peat: g.DrawImage(Images.Swamp[2], 0, 0); break;
                        case SpecialType.Gems: g.DrawImage(Images.Jungle[2], 0, 0); break;
                        case SpecialType.Fish: g.DrawImage(Images.Ocean[2], 0, 0); break;
                        case SpecialType.DesertOil: g.DrawImage(Images.Desert[3], 0, 0); break;
                        case SpecialType.Wheat: g.DrawImage(Images.Plains[3], 0, 0); break;
                        case SpecialType.GrasslandShield: g.DrawImage(Images.Shield, 0, 0); break;   // TODO: what is spectype for grassland?
                        case SpecialType.Silk: g.DrawImage(Images.ForestBase[3], 0, 0); break;
                        case SpecialType.Wine: g.DrawImage(Images.HillsBase[3], 0, 0); break;
                        case SpecialType.Iron: g.DrawImage(Images.MtnsBase[3], 0, 0); break;
                        case SpecialType.Furs: g.DrawImage(Images.Tundra[3], 0, 0); break;
                        case SpecialType.GlacierOil: g.DrawImage(Images.Glacier[3], 0, 0); break;
                        case SpecialType.Spice: g.DrawImage(Images.Swamp[3], 0, 0); break;
                        case SpecialType.Fruit: g.DrawImage(Images.Jungle[3], 0, 0); break;
                        case SpecialType.Whales: g.DrawImage(Images.Ocean[3], 0, 0); break;
                        default: throw new ArgumentOutOfRangeException();
                    }
                }

                // Roads (cites also act as road tiles)
                if (Map.TileC2(col, row).Road || Map.TileC2(col, row).CityPresent)
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
                        g.DrawImage(Images.Road[0], 0, 0);    // No road around
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
                if (Map.TileC2(col, row).Irrigation) g.DrawImage(Images.Irrigation, 0, 0);

                // Farmland
                if (Map.TileC2(col, row).Farmland) g.DrawImage(Images.Farmland, 0, 0);

                // Mining
                if (Map.TileC2(col, row).Mining) g.DrawImage(Images.Mining, 0, 0);

                // Pollution
                if (Map.TileC2(col, row).Pollution) g.DrawImage(Images.Pollution, 0, 0);

                // Fortress
                if (Map.TileC2(col, row).Fortress) g.DrawImage(Images.Fortress, 0, 0);

                // Airbase
                if (Map.TileC2(col, row).Airbase) g.DrawImage(Images.Airbase, 0, 0);
            }

            return tilePic;
        }

        private static bool[] IsLandAround(int col, int row, bool flatEarth)
        {
            // In start we presume all surrounding tiles are water (land=true, water=false). Index=0 is North, follows in clockwise direction.
            bool[] land = new bool[8] { false, false, false, false, false, false, false, false };

            int Xdim = 2 * Map.Xdim;    // X=50 in markted as X=100 in Civ2
            int Ydim = Map.Ydim;        // no need for such correction for Y

            // Observe in all directions if land is present next to ocean
            // N:
            if (row - 2 < 0)
            {
                land[0] = true;   // if N tile is out of map limits, we presume it is land
            }
            else if (Map.TileC2(col, row - 2).Type != TerrainType.Ocean)
            {
                land[0] = true;
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
        private static bool[] IsTerrainAroundIn4directions(int col, int row, TerrainType terrain, bool flatEarth)
        {
            // In start we presume all surrounding tiles are not of same type (=false). Index=0 is NE, follows in clockwise direction.
            bool[] isTerrainAround = new bool[4] { false, false, false, false };

            // Rewrite indexes in Civ2-style
            int Xdim = 2 * Map.Xdim;    // X=50 in markted as X=100 in Civ2
            int Ydim = Map.Ydim;        // no need for such correction for Y

            // Observe in all directions if terrain is present
            // NE:
            if (row == 0)
            {
                isTerrainAround[0] = false;  // NE is beyond limits
            }
            else if (col == Xdim - 1)    // you are on eastern edge of map
            {
                if (flatEarth)
                {
                    isTerrainAround[0] = false;
                }
                else if (Map.TileC2(0, row - 1).Type == terrain)
                {
                    isTerrainAround[0] = true;  // tile on mirror side of map
                }
            }
            else if (Map.TileC2(col + 1, row - 1).Type == terrain)
            {
                isTerrainAround[0] = true;
            }
            // SE:
            if (row == Ydim - 1)
            {
                isTerrainAround[1] = false;  // SE is beyond limits
            }
            else if (col == Xdim - 1)    // you are on eastern edge of map
            {
                if (flatEarth)
                {
                    isTerrainAround[1] = false;
                }
                else if (Map.TileC2(0, row + 1).Type == terrain)
                {
                    isTerrainAround[1] = true;  // tile on mirror side of map
                }
            }
            else if (Map.TileC2(col + 1, row + 1).Type == terrain)
            {
                isTerrainAround[1] = true;
            }
            // SW:
            if (row == Ydim - 1)
            {
                isTerrainAround[2] = false; // SW is beyond limits
            }
            else if (col == 0)    // you are on western edge of map
            {
                if (flatEarth)
                {
                    isTerrainAround[2] = false;
                }
                else if (Map.TileC2(Xdim - 1, row + 1).Type == terrain)
                {
                    isTerrainAround[2] = true;
                }
            }
            else if (Map.TileC2(col - 1, row + 1).Type == terrain)
            {
                isTerrainAround[2] = true;
            }
            // NW:
            if (row == 0)
            {
                isTerrainAround[3] = false;  // NW is beyond limits
            }
            else if (col == 0) // you are on western edge of map
            {
                if (flatEarth)
                {
                    isTerrainAround[3] = false;
                }
                else if (Map.TileC2(Xdim - 1, row - 1).Type == terrain)
                {
                    isTerrainAround[3] = true;
                }
            }
            else if (Map.TileC2(col - 1, row - 1).Type == terrain)
            {
                isTerrainAround[3] = true;
            }

            return isTerrainAround;
        }

        private static bool[] IsRiverAround(int col, int row, bool flatEarth)
        {
            // In start we presume all surrounding tiles are not rivers (river=true, no river=false). Index=0 is NE, follows in clockwise direction.
            bool[] isRiverAround = new bool[4] { false, false, false, false };

            // Rewrite indexes in Civ2-style
            int Xdim = 2 * Map.Xdim;    // X=50 in markted as X=100 in Civ2
            int Ydim = Map.Ydim;        // no need for such correction for Y

            // Observe in all directions if river is present
            // NE:
            if (row == 0)
            {
                isRiverAround[0] = false;  // NE is beyond limits
            }
            else if (col == Xdim - 1)    // you are on eastern edge of map
            {
                if (flatEarth)
                {
                    isRiverAround[0] = false;
                }
                else if (Map.TileC2(0, row - 1).River || Map.TileC2(0, row - 1).Type == TerrainType.Ocean)
                {
                    isRiverAround[0] = true;  // tile on mirror side of map
                }
            }
            else if (Map.TileC2(col + 1, row - 1).River || Map.TileC2(col + 1, row - 1).Type == TerrainType.Ocean)
            {
                isRiverAround[0] = true;
            }
            // SE:
            if (row == Ydim - 1)
            {
                isRiverAround[1] = false;  // SE is beyond limits
            }
            else if (col == Xdim - 1)    // you are on eastern edge of map
            {
                if (flatEarth)
                {
                    isRiverAround[1] = false;
                }
                else if (Map.TileC2(0, row + 1).River || Map.TileC2(0, row + 1).Type == TerrainType.Ocean)
                {
                    isRiverAround[1] = true;  // tile on mirror side of map
                }
            }
            else if (Map.TileC2(col + 1, row + 1).River || Map.TileC2(col + 1, row + 1).Type == TerrainType.Ocean)
            {
                isRiverAround[1] = true;
            }
            // SW:
            if (row == Ydim - 1)
            {
                isRiverAround[2] = false; // SW is beyond limits
            }
            else if (col == 0)    // you are on western edge of map
            {
                if (flatEarth)
                {
                    isRiverAround[2] = false;
                }
                else if (Map.TileC2(Xdim - 1, row + 1).River || Map.TileC2(Xdim - 1, row + 1).Type == TerrainType.Ocean)
                {
                    isRiverAround[2] = true;
                }
            }
            else if (Map.TileC2(col - 1, row + 1).River || Map.TileC2(col - 1, row + 1).Type == TerrainType.Ocean)
            {
                isRiverAround[2] = true;
            }
            // NW:
            if (row == 0)
            {
                isRiverAround[3] = false;  // NW is beyond limits
            }
            else if (col == 0) // you are on western edge of map
            {
                if (flatEarth)
                {
                    isRiverAround[3] = false;
                }
                else if (Map.TileC2(Xdim - 1, row - 1).River || Map.TileC2(Xdim - 1, row - 1).Type == TerrainType.Ocean)
                {
                    isRiverAround[3] = true;
                }
            }
            else if (Map.TileC2(col - 1, row - 1).River || Map.TileC2(col - 1, row - 1).Type == TerrainType.Ocean)
            {
                isRiverAround[3] = true;
            }

            return isRiverAround;
        }

        private static bool[] IsRoadAround(int col, int row, bool flatEarth)
        {
            // In start we presume all surrounding tiles do not have roads. Index=0 is NE, follows in clockwise direction.
            bool[] isRoadAround = new bool[8] { false, false, false, false, false, false, false, false };

            // Rewrite indexes in Civ2-style
            int Xdim = 2 * Map.Xdim;    // X=50 in markted as X=100 in Civ2
            int Ydim = Map.Ydim;        // no need for such correction for Y

            // Observe in all directions if road or city is present next to tile
            // N:
            if (row - 2 < 0)
            {
                isRoadAround[0] = false;   // if N tile is out of map limits
            }
            else if (Map.TileC2(col, row - 2).Road || Map.TileC2(col, row - 2).CityPresent)
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
                else if (Map.TileC2(0, row - 1).Road || Map.TileC2(0, row - 1).CityPresent)
                {
                    isRoadAround[1] = true;  // tile on mirror side of map
                }
            }
            else if (Map.TileC2(col + 1, row - 1).Road || Map.TileC2(col + 1, row - 1).CityPresent)
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
                else if (Map.TileC2(Xdim - col, row).Road || Map.TileC2(Xdim - col, row).CityPresent)
                {
                    isRoadAround[2] = true;
                }
            }
            else if (Map.TileC2(col + 2, row).Road || Map.TileC2(col + 2, row).CityPresent)
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
                else if (Map.TileC2(0, row + 1).Road || Map.TileC2(0, row + 1).CityPresent)
                {
                    isRoadAround[3] = true;  // tile on mirror side of map
                }
            }
            else if (Map.TileC2(col + 1, row + 1).Road || Map.TileC2(col + 1, row + 1).CityPresent)
            {
                isRoadAround[3] = true;
            }
            // S:
            if (row + 2 >= Ydim)
            {
                isRoadAround[4] = false;   // S is beyond map limits
            }
            else if (Map.TileC2(col, row + 2).Road || Map.TileC2(col, row + 2).CityPresent)
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
                else if (Map.TileC2(Xdim - 1, row + 1).Road || Map.TileC2(Xdim - 1, row + 1).CityPresent)
                {
                    isRoadAround[5] = true;
                }
            }
            else if (Map.TileC2(col - 1, row + 1).Road || Map.TileC2(col - 1, row + 1).CityPresent)
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
                else if (Map.TileC2(Xdim - 2 + col, row).Road || Map.TileC2(Xdim - 2 + col, row).CityPresent)
                {
                    isRoadAround[6] = true;
                }
            }
            else if (Map.TileC2(col - 2, row).Road || Map.TileC2(col - 2, row).CityPresent)
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
                else if (Map.TileC2(Xdim - 1, row - 1).Road || Map.TileC2(Xdim - 1, row - 1).CityPresent)
                {
                    isRoadAround[7] = true;
                }
            }
            else if (Map.TileC2(col - 1, row - 1).Road || Map.TileC2(col - 1, row - 1).CityPresent)
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
            int Xdim = 2 * Map.Xdim;    // X=50 in markted as X=100 in Civ2
            int Ydim = Map.Ydim;        // no need for such correction for Y

            // Observe in all directions if railroad or city is present next to tile
            // N:
            if (row - 2 < 0)
            {
                isRailroadAround[0] = false;   // if N tile is out of map limits
            }
            else if (Map.TileC2(col, row - 2).Railroad || Map.TileC2(col, row - 2).CityPresent)
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
                else if (Map.TileC2(0, row - 1).Railroad || Map.TileC2(0, row - 1).CityPresent)
                {
                    isRailroadAround[1] = true;  // tile on mirror side of map
                }
            }
            else if (Map.TileC2(col + 1, row - 1).Railroad || Map.TileC2(col + 1, row - 1).CityPresent)
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
                else if (Map.TileC2(Xdim - col, row).Railroad || Map.TileC2(Xdim - col, row).CityPresent)
                {
                    isRailroadAround[2] = true;
                }
            }
            else if (Map.TileC2(col + 2, row).Railroad || Map.TileC2(col + 2, row).CityPresent)
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
                else if (Map.TileC2(0, row + 1).Railroad || Map.TileC2(0, row + 1).CityPresent)
                {
                    isRailroadAround[3] = true;  // tile on mirror side of map
                }
            }
            else if (Map.TileC2(col + 1, row + 1).Railroad || Map.TileC2(col + 1, row + 1).CityPresent)
            {
                isRailroadAround[3] = true;
            }
            // S:
            if (row + 2 >= Ydim)
            {
                isRailroadAround[4] = false;   // S is beyond map limits
            }
            else if (Map.TileC2(col, row + 2).Railroad || Map.TileC2(col, row + 2).CityPresent)
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
                else if (Map.TileC2(Xdim - 1, row + 1).Railroad || Map.TileC2(Xdim - 1, row + 1).CityPresent)
                {
                    isRailroadAround[5] = true;
                }
            }
            else if (Map.TileC2(col - 1, row + 1).Railroad || Map.TileC2(col - 1, row + 1).CityPresent)
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
                else if (Map.TileC2(Xdim - 2 + col, row).Railroad || Map.TileC2(Xdim - 2 + col, row).CityPresent)
                {
                    isRailroadAround[6] = true;
                }
            }
            else if (Map.TileC2(col - 2, row).Railroad || Map.TileC2(col - 2, row).CityPresent)
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
                else if (Map.TileC2(Xdim - 1, row - 1).Railroad || Map.TileC2(Xdim - 1, row - 1).CityPresent)
                {
                    isRailroadAround[7] = true;
                }
            }
            else if (Map.TileC2(col - 1, row - 1).Railroad || Map.TileC2(col - 1, row - 1).CityPresent)
            {
                isRailroadAround[7] = true;
            }

            return isRailroadAround;
        }
    }
}
