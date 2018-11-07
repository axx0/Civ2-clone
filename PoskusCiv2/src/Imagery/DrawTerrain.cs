using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Imagery
{
    class DrawTerrain
    {
        //public Bitmap Map;

        public Bitmap Square(int col, int row)
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

                //Draw coast & river mouth
                if (Game.Terrain[col, row].Type == TerrainType.Ocean)
                {
                    int[] land = IsLandPresent(col, row);   //Determine if land is present in 8 directions

                    //draw coast & river mouth tiles
                    //NW+N+NE tiles
                    if (land[7] == 0 && land[0] == 0 && land[1] == 0) { graphics.DrawImage(Images.Coast[0, 0], 16, 0); }
                    if (land[7] == 1 && land[0] == 0 && land[1] == 0) { graphics.DrawImage(Images.Coast[1, 0], 16, 0); }
                    if (land[7] == 0 && land[0] == 1 && land[1] == 0) { graphics.DrawImage(Images.Coast[2, 0], 16, 0); }
                    if (land[7] == 1 && land[0] == 1 && land[1] == 0) { graphics.DrawImage(Images.Coast[3, 0], 16, 0); }
                    if (land[7] == 0 && land[0] == 0 && land[1] == 1) { graphics.DrawImage(Images.Coast[4, 0], 16, 0); }
                    if (land[7] == 1 && land[0] == 0 && land[1] == 1) { graphics.DrawImage(Images.Coast[5, 0], 16, 0); }
                    if (land[7] == 0 && land[0] == 1 && land[1] == 1) { graphics.DrawImage(Images.Coast[6, 0], 16, 0); }
                    if (land[7] == 1 && land[0] == 1 && land[1] == 1) { graphics.DrawImage(Images.Coast[7, 0], 16, 0); }
                    //SW+S+SE tiles
                    if (land[3] == 0 && land[4] == 0 && land[5] == 0) { graphics.DrawImage(Images.Coast[0, 1], 16, 16); }
                    if (land[3] == 1 && land[4] == 0 && land[5] == 0) { graphics.DrawImage(Images.Coast[1, 1], 16, 16); }
                    if (land[3] == 0 && land[4] == 1 && land[5] == 0) { graphics.DrawImage(Images.Coast[2, 1], 16, 16); }
                    if (land[3] == 1 && land[4] == 1 && land[5] == 0) { graphics.DrawImage(Images.Coast[3, 1], 16, 16); }
                    if (land[3] == 0 && land[4] == 0 && land[5] == 1) { graphics.DrawImage(Images.Coast[4, 1], 16, 16); }
                    if (land[3] == 1 && land[4] == 0 && land[5] == 1) { graphics.DrawImage(Images.Coast[5, 1], 16, 16); }
                    if (land[3] == 0 && land[4] == 1 && land[5] == 1) { graphics.DrawImage(Images.Coast[6, 1], 16, 16); }
                    if (land[3] == 1 && land[4] == 1 && land[5] == 1) { graphics.DrawImage(Images.Coast[7, 1], 16, 16); }
                    //SW+W+NW tiles
                    if (land[5] == 0 && land[6] == 0 && land[7] == 0) { graphics.DrawImage(Images.Coast[0, 2], 0, 8); }
                    if (land[5] == 1 && land[6] == 0 && land[7] == 0) { graphics.DrawImage(Images.Coast[1, 2], 0, 8); }
                    if (land[5] == 0 && land[6] == 1 && land[7] == 0) { graphics.DrawImage(Images.Coast[2, 2], 0, 8); }
                    if (land[5] == 1 && land[6] == 1 && land[7] == 0) { graphics.DrawImage(Images.Coast[3, 2], 0, 8); }
                    if (land[5] == 0 && land[6] == 0 && land[7] == 1) { graphics.DrawImage(Images.Coast[4, 2], 0, 8); }
                    if (land[5] == 1 && land[6] == 0 && land[7] == 1) { graphics.DrawImage(Images.Coast[5, 2], 0, 8); }
                    if (land[5] == 0 && land[6] == 1 && land[7] == 1) { graphics.DrawImage(Images.Coast[6, 2], 0, 8); }
                    if (land[5] == 1 && land[6] == 1 && land[7] == 1) { graphics.DrawImage(Images.Coast[7, 2], 0, 8); }
                    //NE+E+SE tiles
                    if (land[1] == 0 && land[2] == 0 && land[3] == 0) { graphics.DrawImage(Images.Coast[0, 3], 32, 8); }
                    if (land[1] == 1 && land[2] == 0 && land[3] == 0) { graphics.DrawImage(Images.Coast[1, 3], 32, 8); }
                    if (land[1] == 0 && land[2] == 1 && land[3] == 0) { graphics.DrawImage(Images.Coast[2, 3], 32, 8); }
                    if (land[1] == 1 && land[2] == 1 && land[3] == 0) { graphics.DrawImage(Images.Coast[3, 3], 32, 8); }
                    if (land[1] == 0 && land[2] == 0 && land[3] == 1) { graphics.DrawImage(Images.Coast[4, 3], 32, 8); }
                    if (land[1] == 1 && land[2] == 0 && land[3] == 1) { graphics.DrawImage(Images.Coast[5, 3], 32, 8); }
                    if (land[1] == 0 && land[2] == 1 && land[3] == 1) { graphics.DrawImage(Images.Coast[6, 3], 32, 8); }
                    if (land[1] == 1 && land[2] == 1 && land[3] == 1) { graphics.DrawImage(Images.Coast[7, 3], 32, 8); }

                    //River mouth
                    //if next to ocean is river, draw river mouth on this tile                            
                    int col_ = 2 * col + row % 2; //rewrite indexes in Civ2-style
                    int Xdim = 2 * Game.Data.MapXdim;   //X=50 in markted as X=100 in Civ2
                    int Ydim = Game.Data.MapYdim;   //no need for such correction for Y
                    if (col_ + 1 < Xdim && row - 1 >= 0)    //NE there is no edge of map
                    {
                        if (land[1] == 1 && Game.Terrain[((col_ + 1) - (row - 1) % 2) / 2, row - 1].River) { graphics.DrawImage(Images.RiverMouth[0], 0, 0); }
                    }
                    if (col_ + 1 < Xdim && row + 1 < Ydim)    //SE there is no edge of map
                    {
                        if (land[3] == 1 && Game.Terrain[((col_ + 1) - (row + 1) % 2) / 2, row + 1].River) { graphics.DrawImage(Images.RiverMouth[1], 0, 0); }
                    }
                    if (col_ - 1 >= 0 && row + 1 < Ydim)    //SW there is no edge of map
                    {
                        if (land[5] == 1 && Game.Terrain[((col_ - 1) - (row + 1) % 2) / 2, row + 1].River) { graphics.DrawImage(Images.RiverMouth[2], 0, 0); }
                    }
                    if (col_ - 1 >= 0 && row - 1 >= 0)    //NW there is no edge of map
                    {
                        if (land[7] == 1 && Game.Terrain[((col_ - 1) - (row - 1) % 2) / 2, row - 1].River) { graphics.DrawImage(Images.RiverMouth[3], 0, 0); }
                    }
                }

                //Draw forests
                if (Game.Terrain[col, row].Type == TerrainType.Forest)
                {
                    int[] forestAround = IsForestAround(col, row);

                    //draw forest tiles
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 0, 0, 0 })) { graphics.DrawImage(Images.Forest[0], 0, 0); }
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 0, 0, 0 })) { graphics.DrawImage(Images.Forest[1], 0, 0); }
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 1, 0, 0 })) { graphics.DrawImage(Images.Forest[2], 0, 0); }
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 1, 0, 0 })) { graphics.DrawImage(Images.Forest[3], 0, 0); }
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 0, 1, 0 })) { graphics.DrawImage(Images.Forest[4], 0, 0); }
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 0, 1, 0 })) { graphics.DrawImage(Images.Forest[5], 0, 0); }
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 1, 1, 0 })) { graphics.DrawImage(Images.Forest[6], 0, 0); }
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 1, 1, 0 })) { graphics.DrawImage(Images.Forest[7], 0, 0); }
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 0, 0, 1 })) { graphics.DrawImage(Images.Forest[8], 0, 0); }
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 0, 0, 1 })) { graphics.DrawImage(Images.Forest[9], 0, 0); }
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 1, 0, 1 })) { graphics.DrawImage(Images.Forest[10], 0, 0); }
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 1, 0, 1 })) { graphics.DrawImage(Images.Forest[11], 0, 0); }
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 0, 1, 1 })) { graphics.DrawImage(Images.Forest[12], 0, 0); }
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 0, 1, 1 })) { graphics.DrawImage(Images.Forest[13], 0, 0); }
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 0, 1, 1, 1 })) { graphics.DrawImage(Images.Forest[14], 0, 0); }
                    if (Enumerable.SequenceEqual(forestAround, new int[4] { 1, 1, 1, 1 })) { graphics.DrawImage(Images.Forest[15], 0, 0); }
                }

                //Draw mountains
                //CORRECT THIS: IF SHIELD IS AT MOUNTAIN IT SHOULD BE Mountains[2] and Mountains[3] !!!
                if (Game.Terrain[col, row].Type == TerrainType.Mountains)
                {
                    int[] mountAround = IsMountAround(col, row);

                    //draw forest tiles
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 0, 0, 0 })) { graphics.DrawImage(Images.Mountains[0], 0, 0); }
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 0, 0, 0 })) { graphics.DrawImage(Images.Mountains[1], 0, 0); }
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 1, 0, 0 })) { graphics.DrawImage(Images.Mountains[2], 0, 0); }
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 1, 0, 0 })) { graphics.DrawImage(Images.Mountains[3], 0, 0); }
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 0, 1, 0 })) { graphics.DrawImage(Images.Mountains[4], 0, 0); }
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 0, 1, 0 })) { graphics.DrawImage(Images.Mountains[5], 0, 0); }
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 1, 1, 0 })) { graphics.DrawImage(Images.Mountains[6], 0, 0); }
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 1, 1, 0 })) { graphics.DrawImage(Images.Mountains[7], 0, 0); }
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 0, 0, 1 })) { graphics.DrawImage(Images.Mountains[8], 0, 0); }
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 0, 0, 1 })) { graphics.DrawImage(Images.Mountains[9], 0, 0); }
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 1, 0, 1 })) { graphics.DrawImage(Images.Mountains[10], 0, 0); }
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 1, 0, 1 })) { graphics.DrawImage(Images.Mountains[11], 0, 0); }
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 0, 1, 1 })) { graphics.DrawImage(Images.Mountains[12], 0, 0); }
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 0, 1, 1 })) { graphics.DrawImage(Images.Mountains[13], 0, 0); }
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 0, 1, 1, 1 })) { graphics.DrawImage(Images.Mountains[14], 0, 0); }
                    if (Enumerable.SequenceEqual(mountAround, new int[4] { 1, 1, 1, 1 })) { graphics.DrawImage(Images.Mountains[15], 0, 0); }
                }

                //Draw hills
                if (Game.Terrain[col, row].Type == TerrainType.Hills)
                {
                    int[] hillAround = IsHillAround(col, row);

                    //draw forest tiles
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 0, 0, 0 })) { graphics.DrawImage(Images.Hills[0], 0, 0); }
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 0, 0, 0 })) { graphics.DrawImage(Images.Hills[1], 0, 0); }
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 1, 0, 0 })) { graphics.DrawImage(Images.Hills[2], 0, 0); }
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 1, 0, 0 })) { graphics.DrawImage(Images.Hills[3], 0, 0); }
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 0, 1, 0 })) { graphics.DrawImage(Images.Hills[4], 0, 0); }
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 0, 1, 0 })) { graphics.DrawImage(Images.Hills[5], 0, 0); }
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 1, 1, 0 })) { graphics.DrawImage(Images.Hills[6], 0, 0); }
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 1, 1, 0 })) { graphics.DrawImage(Images.Hills[7], 0, 0); }
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 0, 0, 1 })) { graphics.DrawImage(Images.Hills[8], 0, 0); }
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 0, 0, 1 })) { graphics.DrawImage(Images.Hills[9], 0, 0); }
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 1, 0, 1 })) { graphics.DrawImage(Images.Hills[10], 0, 0); }
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 1, 0, 1 })) { graphics.DrawImage(Images.Hills[11], 0, 0); }
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 0, 1, 1 })) { graphics.DrawImage(Images.Hills[12], 0, 0); }
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 0, 1, 1 })) { graphics.DrawImage(Images.Hills[13], 0, 0); }
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 0, 1, 1, 1 })) { graphics.DrawImage(Images.Hills[14], 0, 0); }
                    if (Enumerable.SequenceEqual(hillAround, new int[4] { 1, 1, 1, 1 })) { graphics.DrawImage(Images.Hills[15], 0, 0); }
                }

                //Draw rivers
                if (Game.Terrain[col, row].River)
                {
                    int[] riverAround = IsRiverAround(col, row);

                    //draw river tiles
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 0, 0, 0 })) { graphics.DrawImage(Images.River[0], 0, 0); }
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 0, 0, 0 })) { graphics.DrawImage(Images.River[1], 0, 0); }
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 1, 0, 0 })) { graphics.DrawImage(Images.River[2], 0, 0); }
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 1, 0, 0 })) { graphics.DrawImage(Images.River[3], 0, 0); }
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 0, 1, 0 })) { graphics.DrawImage(Images.River[4], 0, 0); }
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 0, 1, 0 })) { graphics.DrawImage(Images.River[5], 0, 0); }
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 1, 1, 0 })) { graphics.DrawImage(Images.River[6], 0, 0); }
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 1, 1, 0 })) { graphics.DrawImage(Images.River[7], 0, 0); }
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 0, 0, 1 })) { graphics.DrawImage(Images.River[8], 0, 0); }
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 0, 0, 1 })) { graphics.DrawImage(Images.River[9], 0, 0); }
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 1, 0, 1 })) { graphics.DrawImage(Images.River[10], 0, 0); }
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 1, 0, 1 })) { graphics.DrawImage(Images.River[11], 0, 0); }
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 0, 1, 1 })) { graphics.DrawImage(Images.River[12], 0, 0); }
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 0, 1, 1 })) { graphics.DrawImage(Images.River[13], 0, 0); }
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 0, 1, 1, 1 })) { graphics.DrawImage(Images.River[14], 0, 0); }
                    if (Enumerable.SequenceEqual(riverAround, new int[4] { 1, 1, 1, 1 })) { graphics.DrawImage(Images.River[15], 0, 0); }
                }

                //Special Resources (only grassland)
                //Grassland shield is present in pattern 1100110011... in 1st line, in 3rd line shifted by 1 to right (01100110011...), in 5th line shifted by 1 to right (001100110011...) etc.
                //In 2nd line 00110011..., in 4th line shifted right by 1 (1001100...), in 6th line shifted by 1 to right (11001100...) etc.
                //For grassland special = 0 (no shield), special = 1 (shield).
                if (Game.Terrain[col, row].Type == TerrainType.Grassland)
                {
                    if (row % 2 == 0) //odd lines
                    {
                        if ((col + 4 - (row % 8) / 2) % 4 == 0 || (col + 4 - (row % 8) / 2) % 4 == 1) { Game.Terrain[col, row].Special = 1; }
                        else { Game.Terrain[col, row].Special = 0; }
                    }
                    else    //even lines
                    {
                        if ((col + 4 - (row % 8) / 2) % 4 == 2 || (col + 4 - (row % 8) / 2) % 4 == 3) { Game.Terrain[col, row].Special = 1; }
                        else { Game.Terrain[col, row].Special = 0; }
                    }


                    if (Game.Terrain[col, row].Special == 1) { graphics.DrawImage(Images.Shield, 0, 0); }
                }



                //Special Resources (not grassland)
                //(not yet 100% sure how this works)
                //No matter which terrain tile it is (except grassland). 2 special resources R1 & R2 (e.g. palm & oil for desert). R1 is (in x-direction) always followed by R2, then R1, R2, R1, ... First 2 (j=1,3) are special as they do not belong to other blocks described below. Next block has 7 y-coordinates (j=8,10,...20), next block has 6 (j=25,27,...35), next block 7 (j=40,42,...52), next block 6 (j=57,59,...67), ... Blocks are always 5 tiles appart in y-direction. In x-direction for j=1 the resources are 3/5/3/5 etc. tiles appart. For j=3 they are 8 tiles appart in x-direction. For the next block they are 8-8-8-(3/5/3/5)-8-8-8 tiles appart in x-direction. For the next block they are 8-(3/5/3/5)-8-8-(3/5/3/5)-8 tiles appart. Then these 4 blocks start repeating again. Starting points: For j=1 it is (0,1), for j=3 it is (6,3). The starting (x) points for the next block are x=3,6,4,2,5,3,6. For next block they are x=2,0,3,1,4,2. For the next block they are x=7,2,0,3,1,7,2. For next block they are x=6,1,7,5,3,6. These 4 patterns then start repeating again. So the next block has again pattern 3,6,4,2,5,3,6, the next block has x=2,0,3,1,4,2, etc.
                //For these tiles special=0 (no special, e.g. only desert), special=1 (special #1, e.g. oasis for desert), special=2 (special #2, e.g. oil for desert)
                int special = 0;
                int[] startx_B1 = new int[] { 3, 6, 4, 2, 5, 3, 6 };  //starting x-points for 4 blocks
                int[] startx_B2 = new int[] { 2, 0, 3, 1, 4, 2 };
                int[] startx_B3 = new int[] { 7, 2, 0, 3, 1, 7, 2 };
                int[] startx_B4 = new int[] { 6, 1, 7, 5, 3, 6 };
                if (Game.Terrain[col, row].Type != TerrainType.Grassland)
                {
                    special = 0;    //for start we presume this 
                    bool found = false;

                    if (row == 1) //prva posebna tocka
                    {
                        int novi_i = 0; //zacetna tocka pri j=1 (0,1)
                        while (novi_i < Game.Data.MapXdim)  //keep jumping in x-direction till map end
                        {
                            if (novi_i < Game.Data.MapXdim && col == novi_i) { special = 2; break; }   //tocke (3,1), (11,1), (19,1), ...
                            novi_i += 3;
                            if (novi_i < Game.Data.MapXdim && col == novi_i) { special = 1; break; }   //tocke (8,1), (16,1), (24,1), ...
                            novi_i += 5;
                        }

                    }
                    else if (row == 3)    //druga posebna tocka
                    {
                        int novi_i = 6; //zacetna tocka pri j=3 je (6,3)
                        while (novi_i < Game.Data.MapXdim)
                        {
                            if (novi_i < Game.Data.MapXdim && col == novi_i) { special = 1; break; }
                            novi_i += 8;
                            if (novi_i < Game.Data.MapXdim && col == novi_i) { special = 2; break; }
                            novi_i += 8;
                        }

                    }
                    else
                    {
                        int novi_j = 3;
                        while (novi_j < Game.Data.MapYdim)  //skakanje za 4 bloke naprej
                        {
                            if (found) break;

                            //BLOCK 1
                            int counter = 0;
                            novi_j += 5;   //jump to block beginning
                            while (novi_j < Game.Data.MapYdim && counter < 7)  //7 jumps in y-direction
                            {
                                if (found) break;

                                if (row == novi_j)    //correct y-loc found, now start looking for x
                                {
                                    int novi_i = startx_B1[counter];
                                    //set which resources will be and jumps
                                    int res1, res2;
                                    int skok_x1, skok_x2;
                                    if (counter == 3)
                                    {
                                        skok_x1 = 5;
                                        skok_x2 = 3;
                                        res1 = 2;
                                        res2 = 1;
                                    }
                                    else if (counter == 0 || counter == 1 || counter == 4)
                                    {
                                        skok_x1 = 8;
                                        skok_x2 = 8;
                                        res1 = 2;
                                        res2 = 2;
                                    }
                                    else
                                    {
                                        skok_x1 = 8;
                                        skok_x2 = 8;
                                        res1 = 1;
                                        res2 = 1;
                                    }

                                    while (novi_i < Game.Data.MapXdim)
                                    {
                                        if (novi_i < Game.Data.MapXdim && col == novi_i) { special = res1; found = true; break; }
                                        novi_i += skok_x1;
                                        if (novi_i < Game.Data.MapXdim && col == novi_i) { special = res2; found = true; break; }
                                        novi_i += skok_x2;

                                        if (found) break;
                                    }
                                    break;   //terminate search
                                }
                                novi_j += 2;
                                counter += 1;
                            }
                            if (found) break;

                            //BLOCK 2
                            counter = 0;
                            novi_j += 5;   //jump to block beginning
                            while (novi_j < Game.Data.MapYdim && counter < 6)  //6 jumps in y-direction
                            {
                                if (found) break;

                                if (row == novi_j)    //correct y-loc found, now start looking for x
                                {
                                    int novi_i = startx_B2[counter];
                                    //set which resources will be and jumps
                                    int res1, res2;
                                    int skok_x1, skok_x2;
                                    if (counter == 1)   //1st jump
                                    {
                                        skok_x1 = 5;
                                        skok_x2 = 3;
                                        res1 = 1;
                                        res2 = 2;
                                    }
                                    else if (counter == 4)  //4th jump
                                    {
                                        skok_x1 = 3;
                                        skok_x2 = 5;
                                        res1 = 2;
                                        res2 = 1;
                                    }
                                    else if (counter == 0 || counter == 3)
                                    {
                                        skok_x1 = 8;
                                        skok_x2 = 8;
                                        res1 = 2;
                                        res2 = 2;
                                    }
                                    else
                                    {
                                        skok_x1 = 8;
                                        skok_x2 = 8;
                                        res1 = 1;
                                        res2 = 1;
                                    }

                                    while (novi_i < Game.Data.MapXdim)
                                    {
                                        if (novi_i < Game.Data.MapXdim && col == novi_i) { special = res1; found = true; break; }
                                        novi_i += skok_x1;
                                        if (novi_i < Game.Data.MapXdim && col == novi_i) { special = res2; found = true; break; }
                                        novi_i += skok_x2;

                                        if (found) break;
                                    }
                                    break;   //terminate search
                                }
                                novi_j += 2;
                                counter += 1;
                            }
                            if (found) break;

                            //BLOCK 3
                            counter = 0;
                            novi_j += 5;   //jump to block beginning
                            while (novi_j < Game.Data.MapYdim && counter < 7)  //7 jumps in y-direction
                            {
                                if (found) break;

                                if (row == novi_j)    //correct y-loc found, now start looking for x
                                {
                                    int novi_i = startx_B3[counter];
                                    //set which resources will be and jumps
                                    int res1, res2;
                                    int skok_x1, skok_x2;
                                    if (counter == 3)   //3rd jump
                                    {
                                        skok_x1 = 3;
                                        skok_x2 = 5;
                                        res1 = 1;
                                        res2 = 2;
                                    }
                                    else if (counter == 0 || counter == 1 || counter == 4)
                                    {
                                        skok_x1 = 8;
                                        skok_x2 = 8;
                                        res1 = 2;
                                        res2 = 2;
                                    }
                                    else
                                    {
                                        skok_x1 = 8;
                                        skok_x2 = 8;
                                        res1 = 1;
                                        res2 = 1;
                                    }

                                    while (novi_i < Game.Data.MapXdim)
                                    {
                                        if (novi_i < Game.Data.MapXdim && col == novi_i) { special = res1; found = true; break; }
                                        novi_i += skok_x1;
                                        if (novi_i < Game.Data.MapXdim && col == novi_i) { special = res2; found = true; break; }
                                        novi_i += skok_x2;

                                        if (found) break;
                                    }
                                    break;   //terminate search
                                }
                                novi_j += 2;
                                counter += 1;
                            }
                            if (found) break;

                            //BLOCK 4
                            counter = 0;
                            novi_j += 5;   //jump to block beginning
                            while (novi_j < Game.Data.MapYdim && counter < 6)  //6 jumps in y-direction
                            {
                                if (found) break;

                                if (row == novi_j)    //correct y-loc found, now start looking for x
                                {
                                    int novi_i = startx_B4[counter];
                                    //set which resources will be and jumps
                                    int res1, res2;
                                    int skok_x1, skok_x2;
                                    if (counter == 1 || counter == 4)   //1st & 3rd jump
                                    {
                                        skok_x1 = 3;
                                        skok_x2 = 5;
                                        res1 = 2;
                                        res2 = 1;
                                    }
                                    else if (counter == 0 || counter == 3)
                                    {
                                        skok_x1 = 8;
                                        skok_x2 = 8;
                                        res1 = 2;
                                        res2 = 2;
                                    }
                                    else
                                    {
                                        skok_x1 = 8;
                                        skok_x2 = 8;
                                        res1 = 1;
                                        res2 = 1;
                                    }

                                    while (novi_i < Game.Data.MapXdim)
                                    {
                                        if (novi_i < Game.Data.MapXdim && col == novi_i) { special = res1; found = true; break; }
                                        novi_i += skok_x1;
                                        if (novi_i < Game.Data.MapXdim && col == novi_i) { special = res2; found = true; break; }
                                        novi_i += skok_x2;

                                        if (found) break;
                                    }
                                    break;   //terminate search
                                }
                                novi_j += 2;
                                counter += 1;
                            }
                            if (found) break;

                        }
                    }


                    //Ce se izkaze da je tocka special, potem jo narisi
                    if (special == 1)
                    {
                        Game.Terrain[col, row].Special = 1;
                        switch (Game.Terrain[col, row].Type)
                        {
                            case TerrainType.Desert: maptype = Images.Desert[2]; break;     //Oasis
                            case TerrainType.Forest: maptype = Images.ForestBase[2]; break; //Fazan
                            case TerrainType.Glacier: maptype = Images.Glacier[2]; break;   //Walrus
                            case TerrainType.Hills: maptype = Images.HillsBase[2]; break;   //Coal
                            case TerrainType.Jungle: maptype = Images.Jungle[2]; break;     //Gems
                            case TerrainType.Mountains: maptype = Images.MtnsBase[2]; break;//Gold
                            case TerrainType.Ocean: maptype = Images.Ocean[2]; break;       //Fish
                            case TerrainType.Plains: maptype = Images.Plains[2]; break;     //Buffalo
                            case TerrainType.Swamp: maptype = Images.Swamp[2]; break;       //Peat
                            case TerrainType.Tundra: maptype = Images.Tundra[2]; break;     //Musox
                            default: throw new ArgumentOutOfRangeException();
                        }
                        graphics.DrawImage(maptype, 0, 0);
                    }
                    else if (special == 2)
                    {
                        Game.Terrain[col, row].Special = 2;
                        switch (Game.Terrain[col, row].Type)
                        {
                            case TerrainType.Desert: maptype = Images.Desert[3]; break;     //Oil
                            case TerrainType.Forest: maptype = Images.ForestBase[3]; break; //Silk
                            case TerrainType.Glacier: maptype = Images.Glacier[3]; break;   //Oil
                            case TerrainType.Hills: maptype = Images.HillsBase[3]; break;   //Grapes
                            case TerrainType.Jungle: maptype = Images.Jungle[3]; break;     //Banana
                            case TerrainType.Mountains: maptype = Images.MtnsBase[3]; break;//Iron
                            case TerrainType.Ocean: maptype = Images.Ocean[3]; break;       //Whale
                            case TerrainType.Plains: maptype = Images.Plains[3]; break;     //Wheat
                            case TerrainType.Swamp: maptype = Images.Swamp[3]; break;       //Spice
                            case TerrainType.Tundra: maptype = Images.Tundra[3]; break;     //Furs
                            default: throw new ArgumentOutOfRangeException();
                        }
                        graphics.DrawImage(maptype, 0, 0);
                    }
                    else { Game.Terrain[col, row].Special = 0; }

                }

                //Roads (cites also act as road tiles)
                if (Game.Terrain[col, row].Road || Game.Terrain[col, row].CityPresent)
                {
                    int[] roadAround = IsRoadAround(col, row);

                    //draw roads
                    if (roadAround[0] == 1) { graphics.DrawImage(Images.Road[8], 0, 0); }  //to N
                    if (roadAround[1] == 1) { graphics.DrawImage(Images.Road[1], 0, 0); }  //to NE
                    if (roadAround[2] == 1) { graphics.DrawImage(Images.Road[2], 0, 0); }  //to E
                    if (roadAround[3] == 1) { graphics.DrawImage(Images.Road[3], 0, 0); }  //to SE
                    if (roadAround[4] == 1) { graphics.DrawImage(Images.Road[4], 0, 0); }  //to S
                    if (roadAround[5] == 1) { graphics.DrawImage(Images.Road[5], 0, 0); }  //to SW
                    if (roadAround[6] == 1) { graphics.DrawImage(Images.Road[6], 0, 0); }  //to W
                    if (roadAround[7] == 1) { graphics.DrawImage(Images.Road[7], 0, 0); }  //to NW
                    if (Enumerable.SequenceEqual(roadAround, new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 })) { graphics.DrawImage(Images.Road[0], 0, 0); }    //no road around
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
                if (Game.Terrain[col, row].Irrigation) { graphics.DrawImage(Images.Irrigation, 0, 0); }

                //Farmland
                if (Game.Terrain[col, row].Farmland) { graphics.DrawImage(Images.Farmland, 0, 0); }

                //Mining
                if (Game.Terrain[col, row].Mining) { graphics.DrawImage(Images.Mining, 0, 0); }

                //Pollution
                if (Game.Terrain[col, row].Pollution) { graphics.DrawImage(Images.Pollution, 0, 0); }

                //Fortress
                if (Game.Terrain[col, row].Fortress) { graphics.DrawImage(Images.Fortress, 0, 0); }

                //Airbase
                if (Game.Terrain[col, row].Airbase) { graphics.DrawImage(Images.Airbase, 0, 0); }



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
            if (j - 2 < 0) { land[0] = 1; }   //if N tile is out of map (black tile), we presume it is land
            else if (Game.Terrain[(i_ - (j - 2) % 2) / 2, j - 2].Type != TerrainType.Ocean) { land[0] = 1; }
            //NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) { land[1] = 1; }  //NE is black tile
            else if (Game.Terrain[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type != TerrainType.Ocean) { land[1] = 1; }    //if it is not ocean, it is land
            //E:
            if (i_ + 2 >= Xdim) { land[2] = 1; }  //E is black tile
            else if (Game.Terrain[((i_ + 2) - j % 2) / 2, j].Type != TerrainType.Ocean) { land[2] = 1; }
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) { land[3] = 1; }  //SE is black tile
            else if (Game.Terrain[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type != TerrainType.Ocean) { land[3] = 1; }
            //S:
            if (j + 2 >= Ydim) { land[4] = 1; }   //S is black tile
            else if (Game.Terrain[(i_ - (j + 2) % 2) / 2, j + 2].Type != TerrainType.Ocean) { land[4] = 1; }
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) { land[5] = 1; }  //SW is black tile
            else if (Game.Terrain[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type != TerrainType.Ocean) { land[5] = 1; }
            //W:
            if (i_ - 2 < 0) { land[6] = 1; }  //W is black tile
            else if (Game.Terrain[((i_ - 2) - j % 2) / 2, j].Type != TerrainType.Ocean) { land[6] = 1; }
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) { land[7] = 1; }  //NW is black tile
            else if (Game.Terrain[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type != TerrainType.Ocean) { land[7] = 1; }

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
            if (i_ + 1 >= Xdim || j - 1 < 0) { forestAround[0] = 0; }  //NE is black tile (we presume no forest is there)
            else if (Game.Terrain[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Forest) { forestAround[0] = 1; }
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) { forestAround[1] = 0; }  //SE is black tile
            else if (Game.Terrain[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Forest) { forestAround[1] = 1; }
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) { forestAround[2] = 0; }  //SW is black tile
            else if (Game.Terrain[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Forest) { forestAround[2] = 1; }
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) { forestAround[3] = 0; }  //NW is black tile
            else if (Game.Terrain[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Forest) { forestAround[3] = 1; }

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
            if (i_ + 1 >= Xdim || j - 1 < 0) { mountAround[0] = 0; }  //NE is black tile (we presume no mountain is there)
            else if (Game.Terrain[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Mountains) { mountAround[0] = 1; }
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) { mountAround[1] = 0; }  //SE is black tile
            else if (Game.Terrain[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Mountains) { mountAround[1] = 1; }
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) { mountAround[2] = 0; }  //SW is black tile
            else if (Game.Terrain[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Mountains) { mountAround[2] = 1; }
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) { mountAround[3] = 0; }  //NW is black tile
            else if (Game.Terrain[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Mountains) { mountAround[3] = 1; }

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
            if (i_ + 1 >= Xdim || j - 1 < 0) { hillAround[0] = 0; }  //NE is black tile (we presume no hill is there)
            else if (Game.Terrain[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Hills) { hillAround[0] = 1; }
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) { hillAround[1] = 0; }  //SE is black tile
            else if (Game.Terrain[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Hills) { hillAround[1] = 1; }
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) { hillAround[2] = 0; }  //SW is black tile
            else if (Game.Terrain[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Hills) { hillAround[2] = 1; }
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) { hillAround[3] = 0; }  //NW is black tile
            else if (Game.Terrain[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Hills) { hillAround[3] = 1; }

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
            if (i_ + 1 >= Xdim || j - 1 < 0) { riverAround[0] = 0; }  //NE is black tile (we presume no river is there)
            else if (Game.Terrain[((i_ + 1) - (j - 1) % 2) / 2, j - 1].River || Game.Terrain[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Ocean) { riverAround[0] = 1; }
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) { riverAround[1] = 0; }  //SE is black tile
            else if (Game.Terrain[((i_ + 1) - (j + 1) % 2) / 2, j + 1].River || Game.Terrain[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Ocean) { riverAround[1] = 1; }
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) { riverAround[2] = 0; }  //SW is black tile
            else if (Game.Terrain[((i_ - 1) - (j + 1) % 2) / 2, j + 1].River || Game.Terrain[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Type == TerrainType.Ocean) { riverAround[2] = 1; }
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) { riverAround[3] = 0; }  //NW is black tile
            else if (Game.Terrain[((i_ - 1) - (j - 1) % 2) / 2, j - 1].River || Game.Terrain[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Type == TerrainType.Ocean) { riverAround[3] = 1; }

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
            if (j - 2 < 0) { roadAround[0] = 0; }   //if N tile is out of map (black tile), we presume there is no road
            else if (Game.Terrain[(i_ - (j - 2) % 2) / 2, j - 2].Road || Game.Terrain[(i_ - (j - 2) % 2) / 2, j - 2].CityPresent) { roadAround[0] = 1; }
            //NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) { roadAround[1] = 0; }  //NE is black tile
            else if (Game.Terrain[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Road || Game.Terrain[((i_ + 1) - (j - 1) % 2) / 2, j - 1].CityPresent) { roadAround[1] = 1; }
            //E:
            if (i_ + 2 >= Xdim) { roadAround[2] = 0; }  //E is black tile
            else if (Game.Terrain[((i_ + 2) - j % 2) / 2, j].Road || Game.Terrain[((i_ + 2) - j % 2) / 2, j].CityPresent) { roadAround[2] = 1; }
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) { roadAround[3] = 0; }  //SE is black tile
            else if (Game.Terrain[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Road || Game.Terrain[((i_ + 1) - (j + 1) % 2) / 2, j + 1].CityPresent) { roadAround[3] = 1; }
            //S:
            if (j + 2 >= Ydim) { roadAround[4] = 0; }   //S is black tile
            else if (Game.Terrain[(i_ - (j + 2) % 2) / 2, j + 2].Road || Game.Terrain[(i_ - (j + 2) % 2) / 2, j + 2].CityPresent) { roadAround[4] = 1; }
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) { roadAround[5] = 0; }  //SW is black tile
            else if (Game.Terrain[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Road || Game.Terrain[((i_ - 1) - (j + 1) % 2) / 2, j + 1].CityPresent) { roadAround[5] = 1; }
            //W:
            if (i_ - 2 < 0) { roadAround[6] = 0; }  //W is black tile
            else if (Game.Terrain[((i_ - 2) - j % 2) / 2, j].Road || Game.Terrain[((i_ - 2) - j % 2) / 2, j].CityPresent) { roadAround[6] = 1; }
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) { roadAround[7] = 0; }  //NW is black tile
            else if (Game.Terrain[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Road || Game.Terrain[((i_ - 1) - (j - 1) % 2) / 2, j - 1].CityPresent) { roadAround[7] = 1; }

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
            if (j - 2 < 0) { railroadAround[0] = 0; }   //if N tile is out of map (black tile), we presume there is no road
            else if (Game.Terrain[(i_ - (j - 2) % 2) / 2, j - 2].Railroad || Game.Terrain[(i_ - (j - 2) % 2) / 2, j - 2].CityPresent) { railroadAround[0] = 1; }
            //NE:
            if (i_ + 1 >= Xdim || j - 1 < 0) { railroadAround[1] = 0; }  //NE is black tile
            else if (Game.Terrain[((i_ + 1) - (j - 1) % 2) / 2, j - 1].Railroad || Game.Terrain[((i_ + 1) - (j - 1) % 2) / 2, j - 1].CityPresent) { railroadAround[1] = 1; }
            //E:
            if (i_ + 2 >= Xdim) { railroadAround[2] = 0; }  //E is black tile
            else if (Game.Terrain[((i_ + 2) - j % 2) / 2, j].Railroad || Game.Terrain[((i_ + 2) - j % 2) / 2, j].CityPresent) { railroadAround[2] = 1; }
            //SE:
            if (i_ + 1 >= Xdim || j + 1 >= Ydim) { railroadAround[3] = 0; }  //SE is black tile
            else if (Game.Terrain[((i_ + 1) - (j + 1) % 2) / 2, j + 1].Railroad || Game.Terrain[((i_ + 1) - (j + 1) % 2) / 2, j + 1].CityPresent) { railroadAround[3] = 1; }
            //S:
            if (j + 2 >= Ydim) { railroadAround[4] = 0; }   //S is black tile
            else if (Game.Terrain[(i_ - (j + 2) % 2) / 2, j + 2].Railroad || Game.Terrain[(i_ - (j + 2) % 2) / 2, j + 2].CityPresent) { railroadAround[4] = 1; }
            //SW:
            if (i_ - 1 < 0 || j + 1 >= Ydim) { railroadAround[5] = 0; }  //SW is black tile
            else if (Game.Terrain[((i_ - 1) - (j + 1) % 2) / 2, j + 1].Railroad || Game.Terrain[((i_ - 1) - (j + 1) % 2) / 2, j + 1].CityPresent) { railroadAround[5] = 1; }
            //W:
            if (i_ - 2 < 0) { railroadAround[6] = 0; }  //W is black tile
            else if (Game.Terrain[((i_ - 2) - j % 2) / 2, j].Railroad || Game.Terrain[((i_ - 2) - j % 2) / 2, j].CityPresent) { railroadAround[6] = 1; }
            //NW:
            if (i_ - 1 < 0 || j - 1 < 0) { railroadAround[7] = 0; }  //NW is black tile
            else if (Game.Terrain[((i_ - 1) - (j - 1) % 2) / 2, j - 1].Railroad || Game.Terrain[((i_ - 1) - (j - 1) % 2) / 2, j - 1].CityPresent) { railroadAround[7] = 1; }

            return railroadAround;
        }
    }
}
