using System;
using System.Drawing;
using System.Linq;
using civ2.Enums;
using civ2.Terrains;

namespace civ2.Bitmaps
{
    public partial class Draw
    {
        public static Bitmap Terrain(ITerrain tile, int col, int row, bool flatEarth)
        {
            // Define a bitmap for drawing
            Bitmap tilePic = new Bitmap(64, 32);

            // Draw tile
            using (Graphics graphics = Graphics.FromImage(tilePic))
            {
                switch (tile.Type)
                {
                    case TerrainType.Desert: graphics.DrawImage(Images.Desert[0], 0, 0); break;
                    case TerrainType.Forest: graphics.DrawImage(Images.ForestBase[0], 0, 0); break;
                    case TerrainType.Glacier: graphics.DrawImage(Images.Glacier[0], 0, 0); break;
                    case TerrainType.Grassland: graphics.DrawImage(Images.Grassland[0], 0, 0); break;
                    case TerrainType.Hills: graphics.DrawImage(Images.HillsBase[0], 0, 0); break;
                    case TerrainType.Jungle: graphics.DrawImage(Images.Jungle[0], 0, 0); break;
                    case TerrainType.Mountains: graphics.DrawImage(Images.MtnsBase[0], 0, 0); break;
                    case TerrainType.Ocean: graphics.DrawImage(Images.Ocean[0], 0, 0); break;
                    case TerrainType.Plains: graphics.DrawImage(Images.Plains[0], 0, 0); break;
                    case TerrainType.Swamp: graphics.DrawImage(Images.Swamp[0], 0, 0); break;
                    case TerrainType.Tundra: graphics.DrawImage(Images.Tundra[0], 0, 0); break;
                    default: throw new ArgumentOutOfRangeException();
                }

                // Dither
                int col_ = 2 * col + row % 2;   // to civ2-style
                // Determine type of terrain in all 4 directions. Be careful if you're on map edge.
                TerrainType?[,] tiletype = new TerrainType?[2, 2] { { null, null }, { null, null } };   // null = beyond map limits
                if (flatEarth)
                {
                    // Determine type of NW tile
                    if ((col_ != 0) && (row != 0)) tiletype[0, 0] = Map.Tile[((col_ - 1) - (row - 1) % 2) / 2, row - 1].Type;
                    // Determine type of NE tile
                    if ((col_ != 2 * Map.Xdim - 1) && (row != 0)) tiletype[1, 0] = Map.Tile[((col_ + 1) - (row - 1) % 2) / 2, row - 1].Type;
                    // Determine type of SW tile
                    if ((col_ != 0) && (row != Map.Ydim - 1)) tiletype[0, 1] = Map.Tile[((col_ - 1) - (row + 1) % 2) / 2, row + 1].Type;
                    // Determine type of SE tile
                    if ((col_ != 2 * Map.Xdim - 1) && (row != Map.Ydim - 1)) tiletype[1, 1] = Map.Tile[((col_ + 1) - (row + 1) % 2) / 2, row + 1].Type;
                }
                else    // Round earth
                {
                    // Determine type of NW tile
                    if ((col_ == 0) && (row != 0)) tiletype[0, 0] = Map.Tile[2 * Map.Xdim - 1, row - 1].Type;   // if on left edge take tile from other side of map
                    else if ((col_ != 0) && (row != 0)) tiletype[0, 0] = Map.Tile[((col_ - 1) - (row - 1) % 2) / 2, row - 1].Type;
                    // Determine type of NE tile
                    if ((col_ == 2 * Map.Xdim - 1) && (row != 0)) tiletype[1, 0] = Map.Tile[0, row - 1].Type;   // if on right edge take tile from other side of map
                    else if ((col_ != 2 * Map.Xdim - 1) && (row != 0)) tiletype[1, 0] = Map.Tile[((col_ + 1) - (row - 1) % 2) / 2, row - 1].Type;
                    // Determine type of SW tile
                    if ((col_ == 0) && (row != Map.Ydim - 1)) tiletype[0, 1] = Map.Tile[2 * Map.Xdim - 1, row + 1].Type;   // if on left edge take tile from other side of map
                    else if ((col_ != 0) && (row != Map.Ydim - 1)) tiletype[0, 1] = Map.Tile[((col_ - 1) - (row + 1) % 2) / 2, row + 1].Type;
                    // Determine type of SE tile
                    if ((col_ == 2 * Map.Xdim - 1) && (row != Map.Ydim - 1)) tiletype[1, 1] = Map.Tile[0, row + 1].Type;  // if on right edge take tile from other side of map
                    else if ((col_ != 2 * Map.Xdim - 1) && (row != Map.Ydim - 1)) tiletype[1, 1] = Map.Tile[((col_ + 1) - (row + 1) % 2) / 2, row + 1].Type;
                }
                // Implement dither on 4 locations in square
                for (int tileX = 0; tileX < 2; tileX++)    // for 4 directions
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

                // Draw coast & river mouth
                if (Map.Tile[col, row].Type == TerrainType.Ocean)
                {
                    bool[] land = IsLandPresent(col, row, flatEarth);   // Determine if land is present in 8 directions

                    // Draw coast & river mouth tiles
                    // NW+N+NE tiles
                    if (!land[7] && !land[0] && !land[1]) graphics.DrawImage(Images.Coast[0, 0], 16, 0);
                    if (land[7] && !land[0] && !land[1]) graphics.DrawImage(Images.Coast[1, 0], 16, 0);
                    if (!land[7] && land[0] && !land[1]) graphics.DrawImage(Images.Coast[2, 0], 16, 0);
                    if (land[7] && land[0] && !land[1]) graphics.DrawImage(Images.Coast[3, 0], 16, 0);
                    if (!land[7] && !land[0] && land[1]) graphics.DrawImage(Images.Coast[4, 0], 16, 0);
                    if (land[7] && !land[0] && land[1]) graphics.DrawImage(Images.Coast[5, 0], 16, 0);
                    if (!land[7] && land[0] && land[1]) graphics.DrawImage(Images.Coast[6, 0], 16, 0);
                    if (land[7] && land[0] && land[1]) graphics.DrawImage(Images.Coast[7, 0], 16, 0);
                    // SW+S+SE tiles
                    if (!land[3] && !land[4] && !land[5]) graphics.DrawImage(Images.Coast[0, 1], 16, 16);
                    if (land[3] && !land[4] && !land[5]) graphics.DrawImage(Images.Coast[1, 1], 16, 16);
                    if (!land[3] && land[4] && !land[5]) graphics.DrawImage(Images.Coast[2, 1], 16, 16);
                    if (land[3] && land[4] && !land[5]) graphics.DrawImage(Images.Coast[3, 1], 16, 16);
                    if (!land[3] && !land[4] && land[5]) graphics.DrawImage(Images.Coast[4, 1], 16, 16);
                    if (land[3] && !land[4] && land[5]) graphics.DrawImage(Images.Coast[5, 1], 16, 16);
                    if (!land[3] && land[4] && land[5]) graphics.DrawImage(Images.Coast[6, 1], 16, 16);
                    if (land[3] && land[4] && land[5]) graphics.DrawImage(Images.Coast[7, 1], 16, 16);
                    // SW+W+NW tiles
                    if (!land[5] && !land[6] && !land[7]) graphics.DrawImage(Images.Coast[0, 2], 0, 8);
                    if (land[5] && !land[6] && !land[7]) graphics.DrawImage(Images.Coast[1, 2], 0, 8);
                    if (!land[5] && land[6] && !land[7]) graphics.DrawImage(Images.Coast[2, 2], 0, 8);
                    if (land[5] && land[6] && !land[7]) graphics.DrawImage(Images.Coast[3, 2], 0, 8);
                    if (!land[5] && !land[6] && land[7]) graphics.DrawImage(Images.Coast[4, 2], 0, 8);
                    if (land[5] && !land[6] && land[7]) graphics.DrawImage(Images.Coast[5, 2], 0, 8);
                    if (!land[5] && land[6] && land[7]) graphics.DrawImage(Images.Coast[6, 2], 0, 8);
                    if (land[5] && land[6] && land[7]) graphics.DrawImage(Images.Coast[7, 2], 0, 8);
                    // NE+E+SE tiles
                    if (!land[1] && !land[2] && !land[3]) graphics.DrawImage(Images.Coast[0, 3], 32, 8);
                    if (land[1] && !land[2] && !land[3]) graphics.DrawImage(Images.Coast[1, 3], 32, 8);
                    if (!land[1] && land[2] && !land[3]) graphics.DrawImage(Images.Coast[2, 3], 32, 8);
                    if (land[1] && land[2] && !land[3]) graphics.DrawImage(Images.Coast[3, 3], 32, 8);
                    if (!land[1] && !land[2] && land[3]) graphics.DrawImage(Images.Coast[4, 3], 32, 8);
                    if (land[1] && !land[2] && land[3]) graphics.DrawImage(Images.Coast[5, 3], 32, 8);
                    if (!land[1] && land[2] && land[3]) graphics.DrawImage(Images.Coast[6, 3], 32, 8);
                    if (land[1] && land[2] && land[3]) graphics.DrawImage(Images.Coast[7, 3], 32, 8);

                    // River mouth
                    // If river is next to ocean, draw river mouth on this tile.
                    col_ = 2 * col + row % 2; // rewrite indexes in Civ2-style
                    int Xdim = 2 * Map.Xdim;    // X=50 in markted as X=100 in Civ2
                    int Ydim = Map.Ydim;        // no need for such correction for Y
                    if (col_ + 1 < Xdim && row - 1 >= 0)    // NE there is no edge of map
                    {
                        if (land[1] && Map.Tile[((col_ + 1) - (row - 1) % 2) / 2, row - 1].River) graphics.DrawImage(Images.RiverMouth[0], 0, 0);
                    }
                    if (col_ + 1 < Xdim && row + 1 < Ydim)    // SE there is no edge of map
                    {
                        if (land[3] && Map.Tile[((col_ + 1) - (row + 1) % 2) / 2, row + 1].River) graphics.DrawImage(Images.RiverMouth[1], 0, 0);
                    }
                    if (col_ - 1 >= 0 && row + 1 < Ydim)    // SW there is no edge of map
                    {
                        if (land[5] && Map.Tile[((col_ - 1) - (row + 1) % 2) / 2, row + 1].River) graphics.DrawImage(Images.RiverMouth[2], 0, 0);
                    }
                    if (col_ - 1 >= 0 && row - 1 >= 0)    // NW there is no edge of map
                    {
                        if (land[7] && Map.Tile[((col_ - 1) - (row - 1) % 2) / 2, row - 1].River) graphics.DrawImage(Images.RiverMouth[3], 0, 0);
                    }
                }

                // Draw forests
                if (Map.Tile[col, row].Type == TerrainType.Forest)
                {
                    bool[] isForestAround = IsForestAround(col, row);

                    // Draw forest tiles
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { false, false, false, false })) graphics.DrawImage(Images.Forest[0], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { true, false, false, false })) graphics.DrawImage(Images.Forest[1], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { false, true, false, false })) graphics.DrawImage(Images.Forest[2], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { true, true, false, false })) graphics.DrawImage(Images.Forest[3], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { false, false, true, false })) graphics.DrawImage(Images.Forest[4], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { true, false, true, false })) graphics.DrawImage(Images.Forest[5], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { false, true, true, false })) graphics.DrawImage(Images.Forest[6], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { true, true, true, false })) graphics.DrawImage(Images.Forest[7], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { false, false, false, true })) graphics.DrawImage(Images.Forest[8], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { true, false, false, true })) graphics.DrawImage(Images.Forest[9], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { false, true, false, true })) graphics.DrawImage(Images.Forest[10], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { true, true, false, true })) graphics.DrawImage(Images.Forest[11], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { false, false, true, true })) graphics.DrawImage(Images.Forest[12], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { true, false, true, true })) graphics.DrawImage(Images.Forest[13], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { false, true, true, true })) graphics.DrawImage(Images.Forest[14], 0, 0);
                    if (Enumerable.SequenceEqual(isForestAround, new bool[4] { true, true, true, true })) graphics.DrawImage(Images.Forest[15], 0, 0);
                }

                // Draw mountains
                // TODO: correct drawing mountains - IF SHIELD IS AT MOUNTAIN IT SHOULD BE Mountains[2] and Mountains[3] !!!
                if (Map.Tile[col, row].Type == TerrainType.Mountains)
                {
                    bool[] isMountAround = IsMountAround(col, row);

                    // Draw mountain tiles
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { false, false, false, false })) graphics.DrawImage(Images.Mountains[0], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { true, false, false, false })) graphics.DrawImage(Images.Mountains[1], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { false, true, false, false })) graphics.DrawImage(Images.Mountains[2], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { true, true, false, false })) graphics.DrawImage(Images.Mountains[3], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { false, false, true, false })) graphics.DrawImage(Images.Mountains[4], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { true, false, true, false })) graphics.DrawImage(Images.Mountains[5], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { false, true, true, false })) graphics.DrawImage(Images.Mountains[6], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { true, true, true, false })) graphics.DrawImage(Images.Mountains[7], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { false, false, false, true })) graphics.DrawImage(Images.Mountains[8], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { true, false, false, true })) graphics.DrawImage(Images.Mountains[9], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { false, true, false, true })) graphics.DrawImage(Images.Mountains[10], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { true, true, false, true })) graphics.DrawImage(Images.Mountains[11], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { false, false, true, true })) graphics.DrawImage(Images.Mountains[12], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { true, false, true, true })) graphics.DrawImage(Images.Mountains[13], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { false, true, true, true })) graphics.DrawImage(Images.Mountains[14], 0, 0);
                    if (Enumerable.SequenceEqual(isMountAround, new bool[4] { true, true, true, true })) graphics.DrawImage(Images.Mountains[15], 0, 0);
                }

                // Draw hills
                if (Map.Tile[col, row].Type == TerrainType.Hills)
                {
                    bool[] isHillAround = IsHillAround(col, row);

                    // Draw hill tiles
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { false, false, false, false })) graphics.DrawImage(Images.Hills[0], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { true, false, false, false })) graphics.DrawImage(Images.Hills[1], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { false, true, false, false })) graphics.DrawImage(Images.Hills[2], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { true, true, false, false })) graphics.DrawImage(Images.Hills[3], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { false, false, true, false })) graphics.DrawImage(Images.Hills[4], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { true, false, true, false })) graphics.DrawImage(Images.Hills[5], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { false, true, true, false })) graphics.DrawImage(Images.Hills[6], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { true, true, true, false })) graphics.DrawImage(Images.Hills[7], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { false, false, false, true })) graphics.DrawImage(Images.Hills[8], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { true, false, false, true })) graphics.DrawImage(Images.Hills[9], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { false, true, false, true })) graphics.DrawImage(Images.Hills[10], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { true, true, false, true })) graphics.DrawImage(Images.Hills[11], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { false, false, true, true })) graphics.DrawImage(Images.Hills[12], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { true, false, true, true })) graphics.DrawImage(Images.Hills[13], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { false, true, true, true })) graphics.DrawImage(Images.Hills[14], 0, 0);
                    if (Enumerable.SequenceEqual(isHillAround, new bool[4] { true, true, true, true })) graphics.DrawImage(Images.Hills[15], 0, 0);
                }

                // Draw rivers
                if (Map.Tile[col, row].River)
                {
                    bool[] isRiverAround = IsRiverAround(col, row);

                    // Draw river tiles
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { false, false, false, false })) graphics.DrawImage(Images.River[0], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { true, false, false, false })) graphics.DrawImage(Images.River[1], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { false, true, false, false })) graphics.DrawImage(Images.River[2], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { true, true, false, false })) graphics.DrawImage(Images.River[3], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { false, false, true, false })) graphics.DrawImage(Images.River[4], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { true, false, true, false })) graphics.DrawImage(Images.River[5], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { false, true, true, false })) graphics.DrawImage(Images.River[6], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { true, true, true, false })) graphics.DrawImage(Images.River[7], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { false, false, false, true })) graphics.DrawImage(Images.River[8], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { true, false, false, true })) graphics.DrawImage(Images.River[9], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { false, true, false, true })) graphics.DrawImage(Images.River[10], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { true, true, false, true })) graphics.DrawImage(Images.River[11], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { false, false, true, true })) graphics.DrawImage(Images.River[12], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { true, false, true, true })) graphics.DrawImage(Images.River[13], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { false, true, true, true })) graphics.DrawImage(Images.River[14], 0, 0);
                    if (Enumerable.SequenceEqual(isRiverAround, new bool[4] { true, true, true, true })) graphics.DrawImage(Images.River[15], 0, 0);
                }

                // Draw special resources if they exist
                Map.Tile[col, row].SpecType = null; // TODO: temporary
                if (Map.Tile[col, row].SpecType != null)
                {
                    switch (Map.Tile[col, row].SpecType)
                    {
                        case SpecialType.Oasis: graphics.DrawImage(Images.Desert[2], 0, 0); break;
                        case SpecialType.Buffalo: graphics.DrawImage(Images.Plains[2], 0, 0); break;
                        case SpecialType.Grassland: graphics.DrawImage(Images.Grassland[0], 0, 0); break;   // TODO: what is spectype for grassland?
                        case SpecialType.Pheasant: graphics.DrawImage(Images.ForestBase[2], 0, 0); break;
                        case SpecialType.Coal: graphics.DrawImage(Images.HillsBase[2], 0, 0); break;
                        case SpecialType.Gold: graphics.DrawImage(Images.MtnsBase[2], 0, 0); break;
                        case SpecialType.Game: graphics.DrawImage(Images.Tundra[2], 0, 0); break;
                        case SpecialType.Ivory: graphics.DrawImage(Images.Glacier[2], 0, 0); break;
                        case SpecialType.Peat: graphics.DrawImage(Images.Swamp[2], 0, 0); break;
                        case SpecialType.Gems: graphics.DrawImage(Images.Jungle[2], 0, 0); break;
                        case SpecialType.Fish: graphics.DrawImage(Images.Ocean[2], 0, 0); break;
                        case SpecialType.DesertOil: graphics.DrawImage(Images.Desert[3], 0, 0); break;
                        case SpecialType.Wheat: graphics.DrawImage(Images.Plains[3], 0, 0); break;
                        case SpecialType.GrasslandShield: graphics.DrawImage(Images.Shield, 0, 0); break;   // TODO: what is spectype for grassland?
                        case SpecialType.Silk: graphics.DrawImage(Images.ForestBase[3], 0, 0); break;
                        case SpecialType.Wine: graphics.DrawImage(Images.HillsBase[3], 0, 0); break;
                        case SpecialType.Iron: graphics.DrawImage(Images.MtnsBase[3], 0, 0); break;
                        case SpecialType.Furs: graphics.DrawImage(Images.Tundra[3], 0, 0); break;
                        case SpecialType.GlacierOil: graphics.DrawImage(Images.Glacier[3], 0, 0); break;
                        case SpecialType.Spice: graphics.DrawImage(Images.Swamp[3], 0, 0); break;
                        case SpecialType.Fruit: graphics.DrawImage(Images.Jungle[3], 0, 0); break;
                        case SpecialType.Whales: graphics.DrawImage(Images.Ocean[3], 0, 0); break;
                        default: throw new ArgumentOutOfRangeException();
                    }
                }

                // Roads (cites also act as road tiles)
                if (Map.Tile[col, row].Road || Map.Tile[col, row].CityPresent)
                {
                    bool[] isRoadAround = IsisRoadAround(col, row);

                    // Draw roads
                    if (isRoadAround[0]) graphics.DrawImage(Images.Road[8], 0, 0);  // to N
                    if (isRoadAround[1]) graphics.DrawImage(Images.Road[1], 0, 0);  // to NE
                    if (isRoadAround[2]) graphics.DrawImage(Images.Road[2], 0, 0);  // to E
                    if (isRoadAround[3]) graphics.DrawImage(Images.Road[3], 0, 0);  // to SE
                    if (isRoadAround[4]) graphics.DrawImage(Images.Road[4], 0, 0);  // to S
                    if (isRoadAround[5]) graphics.DrawImage(Images.Road[5], 0, 0);  // to SW
                    if (isRoadAround[6]) graphics.DrawImage(Images.Road[6], 0, 0);  // to W
                    if (isRoadAround[7]) graphics.DrawImage(Images.Road[7], 0, 0);  // to NW
                    if (Enumerable.SequenceEqual(isRoadAround, new bool[8] { false, false, false, false, false, false, false, false })) graphics.DrawImage(Images.Road[0], 0, 0);    // no road around
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
                if (Map.Tile[col, row].Irrigation) graphics.DrawImage(Images.Irrigation, 0, 0);

                // Farmland
                if (Map.Tile[col, row].Farmland) graphics.DrawImage(Images.Farmland, 0, 0);

                // Mining
                if (Map.Tile[col, row].Mining) graphics.DrawImage(Images.Mining, 0, 0);

                // Pollution
                if (Map.Tile[col, row].Pollution) graphics.DrawImage(Images.Pollution, 0, 0);

                // Fortress
                if (Map.Tile[col, row].Fortress) graphics.DrawImage(Images.Fortress, 0, 0);

                // Airbase
                if (Map.Tile[col, row].Airbase) graphics.DrawImage(Images.Airbase, 0, 0);

            }

            return tilePic;
        }

        private static bool[] IsLandPresent(int i, int j, bool flatEarth)
        {
            // In start we presume all surrounding tiles are water (land=true, water=false). Index=0 is North, follows in clockwise direction.
            bool[] land = new bool[8] { false, false, false, false, false, false, false, false };

            // Rewrite indexes in Civ2-style
            int i_ = 2 * i + j % 2;
            int Xdim = 2 * Map.Xdim;    // X=50 in markted as X=100 in Civ2
            int Ydim = Map.Ydim;        // no need for such correction for Y

            // Observe in all directions if land is present next to ocean
            // N:
            if (j - 2 < 0) land[0] = true;   // if N tile is out of map limits, we presume it is land
            else if (Map.Tile[(i_ - (j - 2) % 2) / 2, j - 2].Type != TerrainType.Ocean) land[0] = true;
            // NE:
            if (j == 0) land[1] = true;  // NE is beyond limits
            else if (i_ == Xdim - 1)    // you are on easter edge of map
            {
                if (flatEarth) land[1] = true;
                else if (Map.Tile[0, j - 1].Type != TerrainType.Ocean) land[1] = true;  // tile on mirro side of map is not ocean
            }
            else if (Map.Tile[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type != TerrainType.Ocean) land[1] = true;    // if it is not ocean, it is land
            // E:
            if (i_ + 2 >= Xdim) // you are on right edge of map
            {
                if (flatEarth) land[2] = true;
                else if (Map.Tile[((i_ + 2 - Xdim) - j % 2) / 2, j].Type != TerrainType.Ocean) land[2] = true;
            }                
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

    }
}
