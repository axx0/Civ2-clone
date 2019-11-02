using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RTciv2.Enums;

namespace RTciv2.Imagery
{
    class Squares
    {
        public Bitmap Terrain(int col, int row)
        {
            //define a bitmap for drawing in MapForm
            Bitmap square = new Bitmap(64, 32);            

            using (Graphics graphics = Graphics.FromImage(square))
            {
                ///Draw tiles
                Bitmap maptype;
                switch (Game.Terrain[col, row].Type)
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
                if ((col_ != 0) && (row != 0)) tiletype[0, 0] = Game.Terrain[((col_ - 1) - (row - 1) % 2) / 2, row - 1].Type;
                if ((col_ != 2 * Game.Data.MapXdim - 1) && (row != 0)) tiletype[1, 0] = Game.Terrain[((col_ + 1) - (row - 1) % 2) / 2, row - 1].Type;
                if ((col_ != 0) && (row != Game.Data.MapYdim - 1)) tiletype[0, 1] = Game.Terrain[((col_ - 1) - (row + 1) % 2) / 2, row + 1].Type;
                if ((col_ != 2 * Game.Data.MapXdim - 1) && (row != Game.Data.MapYdim - 1)) tiletype[1, 1] = Game.Terrain[((col_ + 1) - (row + 1) % 2) / 2, row + 1].Type;
                //implement dither on 4 locations in square
                for (int tileX = 0; tileX < 2; tileX++)    //for 4 directions
                {
                    for (int tileY = 0; tileY < 2; tileY++)
                    {
                        switch(tiletype[tileX, tileY])
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
                    }
                }
                
                //Draw coast & river mouth
                if (Game.Terrain[col, row].Type == TerrainType.Ocean)
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
                    int Xdim = 2 * Game.Data.MapXdim;   //X=50 in markted as X=100 in Civ2
                    int Ydim = Game.Data.MapYdim;   //no need for such correction for Y
                    if (col_ + 1 < Xdim && row - 1 >= 0)    //NE there is no edge of map
                    {
                        if (land[1] == 1 && Game.Terrain[((col_ + 1) - (row - 1) % 2) / 2, row - 1].River) graphics.DrawImage(Images.RiverMouth[0], 0, 0);
                    }
                    if (col_ + 1 < Xdim && row + 1 < Ydim)    //SE there is no edge of map
                    {
                        if (land[3] == 1 && Game.Terrain[((col_ + 1) - (row + 1) % 2) / 2, row + 1].River) graphics.DrawImage(Images.RiverMouth[1], 0, 0);
                    }
                    if (col_ - 1 >= 0 && row + 1 < Ydim)    //SW there is no edge of map
                    {
                        if (land[5] == 1 && Game.Terrain[((col_ - 1) - (row + 1) % 2) / 2, row + 1].River) graphics.DrawImage(Images.RiverMouth[2], 0, 0);
                    }
                    if (col_ - 1 >= 0 && row - 1 >= 0)    //NW there is no edge of map
                    {
                        if (land[7] == 1 && Game.Terrain[((col_ - 1) - (row - 1) % 2) / 2, row - 1].River) graphics.DrawImage(Images.RiverMouth[3], 0, 0);
                    }
                }

                //Draw forests
                if (Game.Terrain[col, row].Type == TerrainType.Forest)
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
                if (Game.Terrain[col, row].Type == TerrainType.Mountains)
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
                if (Game.Terrain[col, row].Type == TerrainType.Hills)
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
                if (Game.Terrain[col, row].River)
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
                if (Game.Terrain[col, row].SpecType != null)
                {
                    switch (Game.Terrain[col, row].SpecType)
                    {
                        case SpecialType.Oasis:             maptype = Images.Desert[2]; break;
                        case SpecialType.DesertOil:         maptype = Images.Desert[3]; break;
                        case SpecialType.Buffalo:           maptype = Images.Plains[2]; break;
                        case SpecialType.Wheat:             maptype = Images.Plains[3]; break;
                        case SpecialType.GrasslandShield:   maptype = Images.Shield; break;
                        case SpecialType.Pheasant:          maptype = Images.ForestBase[2]; break;
                        case SpecialType.Silk:              maptype = Images.ForestBase[3]; break;
                        case SpecialType.Coal:              maptype = Images.HillsBase[2]; break;
                        case SpecialType.Wine:              maptype = Images.HillsBase[3]; break;
                        case SpecialType.Gold:              maptype = Images.MtnsBase[2]; break;
                        case SpecialType.Iron:              maptype = Images.MtnsBase[3]; break;
                        case SpecialType.Game:              maptype = Images.Tundra[2]; break;
                        case SpecialType.Furs:              maptype = Images.Tundra[3]; break;
                        case SpecialType.Ivory:             maptype = Images.Glacier[2]; break;
                        case SpecialType.GlacierOil:        maptype = Images.Glacier[3]; break;
                        case SpecialType.Peat:              maptype = Images.Swamp[2]; break;
                        case SpecialType.Spice:             maptype = Images.Swamp[3]; break;
                        case SpecialType.Gems:              maptype = Images.Jungle[2]; break;
                        case SpecialType.Fruit:             maptype = Images.Jungle[3]; break;
                        case SpecialType.Fish:              maptype = Images.Ocean[2]; break;
                        case SpecialType.Whales:            maptype = Images.Ocean[3]; break;
                        default: throw new ArgumentOutOfRangeException();
                    }
                    graphics.DrawImage(maptype, 0, 0);
                }

                //Roads (cites also act as road tiles)
                if (Game.Terrain[col, row].Road || Game.Terrain[col, row].CityPresent)
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
                //if (Game.Terrain[i, j].Railroad || Game.Terrain[i, j].CityPresent)
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
                if (Game.Terrain[col, row].Irrigation) graphics.DrawImage(Images.Irrigation, 0, 0);

                //Farmland
                if (Game.Terrain[col, row].Farmland) graphics.DrawImage(Images.Farmland, 0, 0);

                //Mining
                if (Game.Terrain[col, row].Mining) graphics.DrawImage(Images.Mining, 0, 0);

                //Pollution
                if (Game.Terrain[col, row].Pollution) graphics.DrawImage(Images.Pollution, 0, 0);

                //Fortress
                if (Game.Terrain[col, row].Fortress) graphics.DrawImage(Images.Fortress, 0, 0);

                //Airbase
                if (Game.Terrain[col, row].Airbase) graphics.DrawImage(Images.Airbase, 0, 0);



            }
            return square;
        }

        private int[] IsLandPresent(int i, int j)
        {
            int[] land = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 }; //in start we presume all surrounding tiles are water (land=1, water=0). Starting 0 is North, follows in clockwise direction.

            //rewrite indexes in Civ2-style
            int i_ = 2 * i + j % 2;
            int Xdim = 2 * Game.Data.MapXdim;   //X=50 in markted as X=100 in Civ2
            int Ydim = Game.Data.MapYdim;   //no need for such correction for Y

            //observe in all directions if land is present next to ocean
            //N:
            if (j - 2 < 0) land[0] = 1;   //if N tile is out of map (black tile), we presume it is land
            else if (Game.Terrain[(i_ - (j - 2) % 2) / 2, j - 2].Type != TerrainType.Ocean) land[0] = 1;
            //NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) land[1] = 1;  //NE is black tile
            else if (Game.Terrain[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type != TerrainType.Ocean) land[1] = 1;    //if it is not ocean, it is land
            //E:
            if (i_ + 2 >= Xdim) land[2] = 1;  //E is black tile
            else if (Game.Terrain[((i_ + 2) - j % 2) / 2, j].Type != TerrainType.Ocean) land[2] = 1;
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) land[3] = 1;  //SE is black tile
            else if (Game.Terrain[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type != TerrainType.Ocean) land[3] = 1;
            //S:
            if (j + 2 >= Ydim) land[4] = 1;   //S is black tile
            else if (Game.Terrain[(i_ - (j + 2) % 2) / 2, j + 2].Type != TerrainType.Ocean) land[4] = 1;
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) land[5] = 1;  //SW is black tile
            else if (Game.Terrain[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type != TerrainType.Ocean) land[5] = 1;
            //W:
            if (i_ - 2 < 0) land[6] = 1;  //W is black tile
            else if (Game.Terrain[((i_ - 2) - j % 2) / 2, j].Type != TerrainType.Ocean) land[6] = 1;
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) land[7] = 1;  //NW is black tile
            else if (Game.Terrain[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type != TerrainType.Ocean) land[7] = 1;

            return land;
        }

        private int[] IsForestAround(int i, int j)
        {
            int[] forestAround = new int[4] { 0, 0, 0, 0 }; //in start we presume all surrounding tiles are not forest (forest=1, no forest=0). Starting 0 is NE, follows in clockwise direction.

            //rewrite indexes in Civ2-style
            int i_ = 2 * i + j % 2;
            int Xdim = 2 * Game.Data.MapXdim;   //X=50 in markted as X=100 in Civ2
            int Ydim = Game.Data.MapYdim;   //no need for such correction for Y

            //observe in all directions if forest is present
            //NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) forestAround[0] = 0;  //NE is black tile (we presume no forest is there)
            else if (Game.Terrain[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Forest) forestAround[0] = 1;
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) forestAround[1] = 0;  //SE is black tile
            else if (Game.Terrain[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Forest) forestAround[1] = 1;
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) forestAround[2] = 0;  //SW is black tile
            else if (Game.Terrain[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Forest) forestAround[2] = 1;
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) forestAround[3] = 0;  //NW is black tile
            else if (Game.Terrain[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Forest) forestAround[3] = 1;

            return forestAround;
        }
        
        private int[] IsMountAround(int i, int j)
        {
            int[] mountAround = new int[4] { 0, 0, 0, 0 }; //in start we presume all surrounding tiles are not mountains (mount=1, no mount=0). Starting 0 is NE, follows in clockwise direction.

            //rewrite indexes in Civ2-style
            int i_ = 2 * i + j % 2;
            int Xdim = 2 * Game.Data.MapXdim;   //X=50 in markted as X=100 in Civ2
            int Ydim = Game.Data.MapYdim;   //no need for such correction for Y

            //observe in all directions if mountain is present
            //NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) mountAround[0] = 0;  //NE is black tile (we presume no mountain is there)
            else if (Game.Terrain[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Mountains) mountAround[0] = 1;
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) mountAround[1] = 0;  //SE is black tile
            else if (Game.Terrain[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Mountains) mountAround[1] = 1;
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) mountAround[2] = 0;  //SW is black tile
            else if (Game.Terrain[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Mountains) mountAround[2] = 1;
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) mountAround[3] = 0;  //NW is black tile
            else if (Game.Terrain[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Mountains) mountAround[3] = 1;

            return mountAround;
        }

        private int[] IsHillAround(int i, int j)
        {
            int[] hillAround = new int[4] { 0, 0, 0, 0 }; //in start we presume all surrounding tiles are not hills (hill=1, no hill=0). Starting 0 is NE, follows in clockwise direction.

            //rewrite indexes in Civ2-style
            int i_ = 2 * i + j % 2;
            int Xdim = 2 * Game.Data.MapXdim;   //X=50 in markted as X=100 in Civ2
            int Ydim = Game.Data.MapYdim;   //no need for such correction for Y

            //observe in all directions if hill is present
            //NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) hillAround[0] = 0;  //NE is black tile (we presume no hill is there)
            else if (Game.Terrain[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Hills) hillAround[0] = 1;
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) hillAround[1] = 0;  //SE is black tile
            else if (Game.Terrain[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Hills) hillAround[1] = 1;
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) hillAround[2] = 0;  //SW is black tile
            else if (Game.Terrain[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Hills) hillAround[2] = 1;
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) hillAround[3] = 0;  //NW is black tile
            else if (Game.Terrain[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Hills) hillAround[3] = 1;

            return hillAround;
        }
        
        private int[] IsRiverAround(int i, int j)
        {
            int[] riverAround = new int[4] { 0, 0, 0, 0 }; //in start we presume all surrounding tiles are not rivers (river=1, no river=0). Starting 0 is NE, follows in clockwise direction.

            //rewrite indexes in Civ2-style
            int i_ = 2 * i + j % 2;
            int Xdim = 2 * Game.Data.MapXdim;   //X=50 in markted as X=100 in Civ2
            int Ydim = Game.Data.MapYdim;   //no need for such correction for Y

            //observe in all directions if river is present
            //NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) riverAround[0] = 0;  //NE is black tile (we presume no river is there)
            else if (Game.Terrain[((i_ + 1) - (j - 1) % 2) / 2, j - 1].River || Game.Terrain[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Ocean) riverAround[0] = 1;
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) riverAround[1] = 0;  //SE is black tile
            else if (Game.Terrain[((i_ + 1) - (j + 1) % 2) / 2, j + 1].River || Game.Terrain[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Ocean) riverAround[1] = 1;
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) riverAround[2] = 0;  //SW is black tile
            else if (Game.Terrain[((i_ - 1) - (j + 1) % 2) / 2, j + 1].River || Game.Terrain[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Ocean) riverAround[2] = 1;
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) riverAround[3] = 0;  //NW is black tile
            else if (Game.Terrain[((i_ - 1) - (j - 1) % 2) / 2, j - 1].River || Game.Terrain[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Ocean) riverAround[3] = 1;

            return riverAround;
        }

        private int[] IsRoadAround(int i, int j)
        {
            int[] roadAround = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 }; //in start we presume all surrounding tiles do not have roads. Starting 0 is NE, follows in clockwise direction.

            //rewrite indexes in Civ2-style
            int i_ = 2 * i + j % 2;
            int Xdim = 2 * Game.Data.MapXdim;   //X=50 in markted as X=100 in Civ2
            int Ydim = Game.Data.MapYdim;   //no need for such correction for Y

            //observe in all directions if road or city is present next to tile
            //N:
            if (j - 2 < 0) roadAround[0] = 0;   //if N tile is out of map (black tile), we presume there is no road
            else if (Game.Terrain[(i_ - (j - 2) % 2) / 2, j - 2].Road || Game.Terrain[(i_ - (j - 2) % 2) / 2, j - 2].CityPresent) roadAround[0] = 1;
            //NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) roadAround[1] = 0;  //NE is black tile
            else if (Game.Terrain[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Road || Game.Terrain[((i_ + 1) - (j - 1) % 2) / 2, j - 1].CityPresent) roadAround[1] = 1;
            //E:
            if (i_ + 2 >= Xdim) roadAround[2] = 0;  //E is black tile
            else if (Game.Terrain[((i_ + 2) - j % 2) / 2, j].Road || Game.Terrain[((i_ + 2) - j % 2) / 2, j].CityPresent) roadAround[2] = 1;
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) roadAround[3] = 0;  //SE is black tile
            else if (Game.Terrain[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Road || Game.Terrain[((i_ + 1) - (j + 1) % 2) / 2, j + 1].CityPresent) roadAround[3] = 1;
            //S:
            if (j + 2 >= Ydim) roadAround[4] = 0;   //S is black tile
            else if (Game.Terrain[(i_ - (j + 2) % 2) / 2, j + 2].Road || Game.Terrain[(i_ - (j + 2) % 2) / 2, j + 2].CityPresent) roadAround[4] = 1;
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) roadAround[5] = 0;  //SW is black tile
            else if (Game.Terrain[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Road || Game.Terrain[((i_ - 1) - (j + 1) % 2) / 2, j + 1].CityPresent) roadAround[5] = 1;
            //W:
            if (i_ - 2 < 0) roadAround[6] = 0;  //W is black tile
            else if (Game.Terrain[((i_ - 2) - j % 2) / 2, j].Road || Game.Terrain[((i_ - 2) - j % 2) / 2, j].CityPresent) roadAround[6] = 1;
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) roadAround[7] = 0;  //NW is black tile
            else if (Game.Terrain[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Road || Game.Terrain[((i_ - 1) - (j - 1) % 2) / 2, j - 1].CityPresent) roadAround[7] = 1;

            return roadAround;
        }


        private int[] IsRailroadAround(int i, int j)
        {
            int[] railroadAround = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 }; //in start we presume all surrounding tiles do not have railroads. Starting 0 is NE, follows in clockwise direction.

            //rewrite indexes in Civ2-style
            int i_ = 2 * i + j % 2;
            int Xdim = 2 * Game.Data.MapXdim;   //X=50 in markted as X=100 in Civ2
            int Ydim = Game.Data.MapYdim;   //no need for such correction for Y

            //observe in all directions if road or city is present next to tile
            //N:
            if (j - 2 < 0) railroadAround[0] = 0;   //if N tile is out of map (black tile), we presume there is no road
            else if (Game.Terrain[(i_ - (j - 2) % 2) / 2, j - 2].Railroad || Game.Terrain[(i_ - (j - 2) % 2) / 2, j - 2].CityPresent) railroadAround[0] = 1;
            //NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) railroadAround[1] = 0;  //NE is black tile
            else if (Game.Terrain[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Railroad || Game.Terrain[((i_ + 1) - (j - 1) % 2) / 2, j - 1].CityPresent) railroadAround[1] = 1;
            //E:
            if (i_ + 2 >= Xdim) railroadAround[2] = 0;  //E is black tile
            else if (Game.Terrain[((i_ + 2) - j % 2) / 2, j].Railroad || Game.Terrain[((i_ + 2) - j % 2) / 2, j].CityPresent) railroadAround[2] = 1;
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) railroadAround[3] = 0;  //SE is black tile
            else if (Game.Terrain[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Railroad || Game.Terrain[((i_ + 1) - (j + 1) % 2) / 2, j + 1].CityPresent) railroadAround[3] = 1;
            //S:
            if (j + 2 >= Ydim) railroadAround[4] = 0;   //S is black tile
            else if (Game.Terrain[(i_ - (j + 2) % 2) / 2, j + 2].Railroad || Game.Terrain[(i_ - (j + 2) % 2) / 2, j + 2].CityPresent) railroadAround[4] = 1;
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) railroadAround[5] = 0;  //SW is black tile
            else if (Game.Terrain[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Railroad || Game.Terrain[((i_ - 1) - (j + 1) % 2) / 2, j + 1].CityPresent) railroadAround[5] = 1;
            //W:
            if (i_ - 2 < 0) railroadAround[6] = 0;  //W is black tile
            else if (Game.Terrain[((i_ - 2) - j % 2) / 2, j].Railroad || Game.Terrain[((i_ - 2) - j % 2) / 2, j].CityPresent) railroadAround[6] = 1;
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) railroadAround[7] = 0;  //NW is black tile
            else if (Game.Terrain[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Railroad || Game.Terrain[((i_ - 1) - (j - 1) % 2) / 2, j - 1].CityPresent) railroadAround[7] = 1;

            return railroadAround;
        }
    }
}
