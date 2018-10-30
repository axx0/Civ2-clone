using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace PoskusCiv2.Imagery
{
    class DrawMap
    {
        public static ImportSavegame Map = new ImportSavegame();
        public static string[] terrainName = new string[138];
        public string[] terrainName1 = new string[11] { "Desert", "Plains", "Grasslands", "Forest", "Hills", "Mountains", "Tundra", "Arctic", "Swamp", "Jungle", "Ocean" }; //to merge into terrainName

        //Terrain tiles
        public Bitmap bitmap1;
        Bitmap terrain1, terrain2;
        Bitmap desert1, desert3, plains1, grasslands1, forest_base1, hills_base1, mountains_base1, tundra1, arctic1, swamp1, jungle1, ocean1;
        Rectangle RectTileDesert1, RectTileDesert2, RectTileDesert3, RectTileDesert4, RectTilePlains1, RectTilePlains3, RectTilePlains4, RectTileGrasslands1, RectTileGrasslands2, RectTileGrasslands3, RectTileGrasslands4, RectTileForest_base1, RectTileForest_base2, RectTileForest_base3, RectTileForest_base4, RectTileHills_base1, RectTileHills_base2, RectTileHills_base3, RectTileHills_base4, RectTileMountains_base1, RectTileMountains_base2, RectTileMountains_base3, RectTileMountains_base4, RectTileTundra1, RectTileTundra2, RectTileTundra3, RectTileTundra4, RectTileArctic1, RectTileArctic2, RectTileArctic3, RectTileArctic4, RectTileSwamp1, RectTileSwamp2, RectTileSwamp3, RectTileSwamp4, RectTileJungle1, RectTileJungle2, RectTileJungle3, RectTileJungle4, RectTileOcean1, RectTileOcean3, RectTileOcean4;
        Color transparentGray, transparentPink;

        //Coast tiles
        Bitmap coast0N, coast0W, coast0E, coast0S, coast1N, coast1W, coast1E, coast1S, coast2N, coast2W, coast2E, coast2S, coast3N, coast3W, coast3E, coast3S, coast4N, coast4W, coast4E, coast4S, coast5N, coast5W, coast5E, coast5S, coast6N, coast6W, coast6E, coast6S, coast7S, coast7N, coast7W, coast7E;
        Rectangle RectCoast0N, RectCoast0W, RectCoast0E, RectCoast0S, RectCoast1N, RectCoast1W, RectCoast1E, RectCoast1S, RectCoast2N, RectCoast2W, RectCoast2E, RectCoast2S, RectCoast3N, RectCoast3W, RectCoast3E, RectCoast3S, RectCoast4N, RectCoast4W, RectCoast4E, RectCoast4S, RectCoast5N, RectCoast5W, RectCoast5E, RectCoast5S, RectCoast6N, RectCoast6W, RectCoast6E, RectCoast6S, RectCoast7N, RectCoast7W, RectCoast7E, RectCoast7S;

        //River/mountains/hills/forest tiles
        Bitmap river0, river1, river2, river3, river4, river5, river6, river7, river8, river9, river10, river11, river12, river13, river14, river15, forest0, forest1, forest2, forest3, forest4, forest5, forest6, forest7, forest8, forest9, forest10, forest11, forest12, forest13, forest14, forest15, mountains0, mountains1, mountains2, mountains3, mountains4, mountains5, mountains6, mountains7, mountains8, mountains9, mountains10, mountains11, mountains12, mountains13, mountains14, mountains15, hills0, hills1, hills2, hills3, hills4, hills5, hills6, hills7, hills8, hills9, hills10, hills11, hills12, hills13, hills14, hills15, river_mouth0, river_mouth1, river_mouth2, river_mouth3;
        Rectangle RectRiver0, RectRiver1, RectRiver2, RectRiver3, RectRiver4, RectRiver5, RectRiver6, RectRiver7, RectRiver8, RectRiver9, RectRiver10, RectRiver11, RectRiver12, RectRiver13, RectRiver14, RectRiver15, RectForest0, RectForest1, RectForest2, RectForest3, RectForest4, RectForest5, RectForest6, RectForest7, RectForest8, RectForest9, RectForest10, RectForest11, RectForest12, RectForest13, RectForest14, RectForest15, RectMountains0, RectMountains1, RectMountains2, RectMountains3, RectMountains4, RectMountains5, RectMountains6, RectMountains7, RectMountains8, RectMountains9, RectMountains10, RectMountains11, RectMountains12, RectMountains13, RectMountains14, RectMountains15, RectHills0, RectHills1, RectHills2, RectHills3, RectHills4, RectHills5, RectHills6, RectHills7, RectHills8, RectHills9, RectHills10, RectHills11, RectHills12, RectHills13, RectHills14, RectHills15, RectRiver_mouth0, RectRiver_mouth1, RectRiver_mouth2, RectRiver_mouth3;

        public DrawMap()
        {
            //define terrain names
            Array.Copy(terrainName1, 0, terrainName, 0, 11);
            Array.Copy(terrainName1, 0, terrainName, 128, 10);
            terrainName[74] = "Ocean";

            terrain1 = new Bitmap(@"C:\DOS\CIV 2\Civ2\TERRAIN1.GIF");
            terrain2 = new Bitmap(@"C:\DOS\CIV 2\Civ2\TERRAIN2.GIF");

            //Define rectangles for reading images
            RectTileDesert1 = new Rectangle(1, 1, 64, 32);
            RectTileDesert3 = new Rectangle(3 * 1 + 2 * 64, 1, 64, 32);  //oasis
            RectTilePlains1 = new Rectangle(1, 1 + 1 + 32, 64, 32);
            RectTileGrasslands1 = new Rectangle(1, 3 * 1 + 2 * 32, 64, 32);
            RectTileForest_base1 = new Rectangle(1, 4 * 1 + 3 * 32, 64, 32);
            RectTileHills_base1 = new Rectangle(1, 5 * 1 + 4 * 32, 64, 32);
            RectTileMountains_base1 = new Rectangle(1, 6 * 1 + 5 * 32, 64, 32);
            RectTileTundra1 = new Rectangle(1, 7 * 1 + 6 * 32, 64, 32);
            RectTileArctic1 = new Rectangle(1, 8 * 1 + 7 * 32, 64, 32);
            RectTileSwamp1 = new Rectangle(1, 9 * 1 + 8 * 32, 64, 32);
            RectTileJungle1 = new Rectangle(1, 10 * 1 + 9 * 32, 64, 32);
            RectTileOcean1 = new Rectangle(1, 11 * 1 + 10 * 32, 64, 32);
            //Define coast rectangles
            RectCoast0N = new Rectangle(1, 429, 32, 16);
            RectCoast0S = new Rectangle(1, 429 + 1 * 1 + 1 * 16, 32, 16);
            RectCoast0W = new Rectangle(1, 429 + 2 * 1 + 2 * 16, 32, 16);
            RectCoast0E = new Rectangle(2 * 1 + 1 * 32, 429 + 2 * 1 + 2 * 16, 32, 16);
            RectCoast1N = new Rectangle(3 * 1 + 2 * 32, 429, 32, 16);
            RectCoast1S = new Rectangle(3 * 1 + 2 * 32, 429 + 1 * 1 + 1 * 16, 32, 16);
            RectCoast1W = new Rectangle(3 * 1 + 2 * 32, 429 + 2 * 1 + 2 * 16, 32, 16);
            RectCoast1E = new Rectangle(4 * 1 + 3 * 32, 429 + 2 * 1 + 2 * 16, 32, 16);
            RectCoast2N = new Rectangle(5 * 1 + 4 * 32, 429, 32, 16);
            RectCoast2S = new Rectangle(5 * 1 + 4 * 32, 429 + 1 * 1 + 1 * 16, 32, 16);
            RectCoast2W = new Rectangle(5 * 1 + 4 * 32, 429 + 2 * 1 + 2 * 16, 32, 16);
            RectCoast2E = new Rectangle(6 * 1 + 5 * 32, 429 + 2 * 1 + 2 * 16, 32, 16);
            RectCoast3N = new Rectangle(7 * 1 + 6 * 32, 429, 32, 16);
            RectCoast3S = new Rectangle(7 * 1 + 6 * 32, 429 + 1 * 1 + 1 * 16, 32, 16);
            RectCoast3W = new Rectangle(7 * 1 + 6 * 32, 429 + 2 * 1 + 2 * 16, 32, 16);
            RectCoast3E = new Rectangle(8 * 1 + 7 * 32, 429 + 2 * 1 + 2 * 16, 32, 16);
            RectCoast4N = new Rectangle(9 * 1 + 8 * 32, 429, 32, 16);
            RectCoast4S = new Rectangle(9 * 1 + 8 * 32, 429 + 1 * 1 + 1 * 16, 32, 16);
            RectCoast4W = new Rectangle(9 * 1 + 8 * 32, 429 + 2 * 1 + 2 * 16, 32, 16);
            RectCoast4E = new Rectangle(10 * 1 + 9 * 32, 429 + 2 * 1 + 2 * 16, 32, 16);
            RectCoast5N = new Rectangle(11 * 1 + 10 * 32, 429, 32, 16);
            RectCoast5S = new Rectangle(11 * 1 + 10 * 32, 429 + 1 * 1 + 1 * 16, 32, 16);
            RectCoast5W = new Rectangle(11 * 1 + 10 * 32, 429 + 2 * 1 + 2 * 16, 32, 16);
            RectCoast5E = new Rectangle(12 * 1 + 11 * 32, 429 + 2 * 1 + 2 * 16, 32, 16);
            RectCoast6N = new Rectangle(13 * 1 + 12 * 32, 429, 32, 16);
            RectCoast6S = new Rectangle(13 * 1 + 12 * 32, 429 + 1 * 1 + 1 * 16, 32, 16);
            RectCoast6W = new Rectangle(13 * 1 + 12 * 32, 429 + 2 * 1 + 2 * 16, 32, 16);
            RectCoast6E = new Rectangle(14 * 1 + 13 * 32, 429 + 2 * 1 + 2 * 16, 32, 16);
            RectCoast7N = new Rectangle(15 * 1 + 14 * 32, 429, 32, 16);
            RectCoast7S = new Rectangle(15 * 1 + 14 * 32, 429 + 1 * 1 + 1 * 16, 32, 16);
            RectCoast7W = new Rectangle(15 * 1 + 14 * 32, 429 + 2 * 1 + 2 * 16, 32, 16);
            RectCoast7E = new Rectangle(16 * 1 + 15 * 32, 429 + 2 * 1 + 2 * 16, 32, 16);
            //Define forest/river/mountain/hill rectangles
            RectRiver0 = new Rectangle(1 * 1 + 0 * 64, 3 * 1 + 2 * 32, 64, 32);
            RectRiver1 = new Rectangle(2 * 1 + 1 * 64, 3 * 1 + 2 * 32, 64, 32);
            RectRiver2 = new Rectangle(3 * 1 + 2 * 64, 3 * 1 + 2 * 32, 64, 32);
            RectRiver3 = new Rectangle(4 * 1 + 3 * 64, 3 * 1 + 2 * 32, 64, 32);
            RectRiver4 = new Rectangle(5 * 1 + 4 * 64, 3 * 1 + 2 * 32, 64, 32);
            RectRiver5 = new Rectangle(6 * 1 + 5 * 64, 3 * 1 + 2 * 32, 64, 32);
            RectRiver6 = new Rectangle(7 * 1 + 6 * 64, 3 * 1 + 2 * 32, 64, 32);
            RectRiver7 = new Rectangle(8 * 1 + 7 * 64, 3 * 1 + 2 * 32, 64, 32);
            RectRiver8 = new Rectangle(1 * 1 + 0 * 64, 4 * 1 + 3 * 32, 64, 32);
            RectRiver9 = new Rectangle(2 * 1 + 1 * 64, 4 * 1 + 3 * 32, 64, 32);
            RectRiver10 = new Rectangle(3 * 1 + 2 * 64, 4 * 1 + 3 * 32, 64, 32);
            RectRiver11 = new Rectangle(4 * 1 + 3 * 64, 4 * 1 + 3 * 32, 64, 32);
            RectRiver12 = new Rectangle(5 * 1 + 4 * 64, 4 * 1 + 3 * 32, 64, 32);
            RectRiver13 = new Rectangle(6 * 1 + 5 * 64, 4 * 1 + 3 * 32, 64, 32);
            RectRiver14 = new Rectangle(7 * 1 + 6 * 64, 4 * 1 + 3 * 32, 64, 32);
            RectRiver15 = new Rectangle(8 * 1 + 7 * 64, 4 * 1 + 3 * 32, 64, 32);
            RectForest0 = new Rectangle(1 * 1 + 0 * 64, 5 * 1 + 4 * 32, 64, 32);
            RectForest1 = new Rectangle(2 * 1 + 1 * 64, 5 * 1 + 4 * 32, 64, 32);
            RectForest2 = new Rectangle(3 * 1 + 2 * 64, 5 * 1 + 4 * 32, 64, 32);
            RectForest3 = new Rectangle(4 * 1 + 3 * 64, 5 * 1 + 4 * 32, 64, 32);
            RectForest4 = new Rectangle(5 * 1 + 4 * 64, 5 * 1 + 4 * 32, 64, 32);
            RectForest5 = new Rectangle(6 * 1 + 5 * 64, 5 * 1 + 4 * 32, 64, 32);
            RectForest6 = new Rectangle(7 * 1 + 6 * 64, 5 * 1 + 4 * 32, 64, 32);
            RectForest7 = new Rectangle(8 * 1 + 7 * 64, 5 * 1 + 4 * 32, 64, 32);
            RectForest8 = new Rectangle(1 * 1 + 0 * 64, 6 * 1 + 5 * 32, 64, 32);
            RectForest9 = new Rectangle(2 * 1 + 1 * 64, 6 * 1 + 5 * 32, 64, 32);
            RectForest10 = new Rectangle(3 * 1 + 2 * 64, 6 * 1 + 5 * 32, 64, 32);
            RectForest11 = new Rectangle(4 * 1 + 3 * 64, 6 * 1 + 5 * 32, 64, 32);
            RectForest12 = new Rectangle(5 * 1 + 4 * 64, 6 * 1 + 5 * 32, 64, 32);
            RectForest13 = new Rectangle(6 * 1 + 5 * 64, 6 * 1 + 5 * 32, 64, 32);
            RectForest14 = new Rectangle(7 * 1 + 6 * 64, 6 * 1 + 5 * 32, 64, 32);
            RectForest15 = new Rectangle(8 * 1 + 7 * 64, 6 * 1 + 5 * 32, 64, 32);
            RectMountains0 = new Rectangle(1 * 1 + 0 * 64, 7 * 1 + 6 * 32, 64, 32);
            RectMountains1 = new Rectangle(2 * 1 + 1 * 64, 7 * 1 + 6 * 32, 64, 32);
            RectMountains2 = new Rectangle(3 * 1 + 2 * 64, 7 * 1 + 6 * 32, 64, 32);
            RectMountains3 = new Rectangle(4 * 1 + 3 * 64, 7 * 1 + 6 * 32, 64, 32);
            RectMountains4 = new Rectangle(5 * 1 + 4 * 64, 7 * 1 + 6 * 32, 64, 32);
            RectMountains5 = new Rectangle(6 * 1 + 5 * 64, 7 * 1 + 6 * 32, 64, 32);
            RectMountains6 = new Rectangle(7 * 1 + 6 * 64, 7 * 1 + 6 * 32, 64, 32);
            RectMountains7 = new Rectangle(8 * 1 + 7 * 64, 7 * 1 + 6 * 32, 64, 32);
            RectMountains8 = new Rectangle(1 * 1 + 0 * 64, 8 * 1 + 7 * 32, 64, 32);
            RectMountains9 = new Rectangle(2 * 1 + 1 * 64, 8 * 1 + 7 * 32, 64, 32);
            RectMountains10 = new Rectangle(3 * 1 + 2 * 64, 8 * 1 + 7 * 32, 64, 32);
            RectMountains11 = new Rectangle(4 * 1 + 3 * 64, 8 * 1 + 7 * 32, 64, 32);
            RectMountains12 = new Rectangle(5 * 1 + 4 * 64, 8 * 1 + 7 * 32, 64, 32);
            RectMountains13 = new Rectangle(6 * 1 + 5 * 64, 8 * 1 + 7 * 32, 64, 32);
            RectMountains14 = new Rectangle(7 * 1 + 6 * 64, 8 * 1 + 7 * 32, 64, 32);
            RectMountains15 = new Rectangle(8 * 1 + 7 * 64, 8 * 1 + 7 * 32, 64, 32);
            RectHills0 = new Rectangle(1 * 1 + 0 * 64, 9 * 1 + 8 * 32, 64, 32);
            RectHills1 = new Rectangle(2 * 1 + 1 * 64, 9 * 1 + 8 * 32, 64, 32);
            RectHills2 = new Rectangle(3 * 1 + 2 * 64, 9 * 1 + 8 * 32, 64, 32);
            RectHills3 = new Rectangle(4 * 1 + 3 * 64, 9 * 1 + 8 * 32, 64, 32);
            RectHills4 = new Rectangle(5 * 1 + 4 * 64, 9 * 1 + 8 * 32, 64, 32);
            RectHills5 = new Rectangle(6 * 1 + 5 * 64, 9 * 1 + 8 * 32, 64, 32);
            RectHills6 = new Rectangle(7 * 1 + 6 * 64, 9 * 1 + 8 * 32, 64, 32);
            RectHills7 = new Rectangle(8 * 1 + 7 * 64, 9 * 1 + 8 * 32, 64, 32);
            RectHills8 = new Rectangle(1 * 1 + 0 * 64, 10 * 1 + 9 * 32, 64, 32);
            RectHills9 = new Rectangle(2 * 1 + 1 * 64, 10 * 1 + 9 * 32, 64, 32);
            RectHills10 = new Rectangle(3 * 1 + 2 * 64, 10 * 1 + 9 * 32, 64, 32);
            RectHills11 = new Rectangle(4 * 1 + 3 * 64, 10 * 1 + 9 * 32, 64, 32);
            RectHills12 = new Rectangle(5 * 1 + 4 * 64, 10 * 1 + 9 * 32, 64, 32);
            RectHills13 = new Rectangle(6 * 1 + 5 * 64, 10 * 1 + 9 * 32, 64, 32);
            RectHills14 = new Rectangle(7 * 1 + 6 * 64, 10 * 1 + 9 * 32, 64, 32);
            RectHills15 = new Rectangle(8 * 1 + 7 * 64, 10 * 1 + 9 * 32, 64, 32);
            RectRiver_mouth0 = new Rectangle(1 * 1 + 0 * 64, 11 * 1 + 10 * 32, 64, 32);
            RectRiver_mouth1 = new Rectangle(2 * 1 + 1 * 64, 11 * 1 + 10 * 32, 64, 32);
            RectRiver_mouth2 = new Rectangle(3 * 1 + 2 * 64, 11 * 1 + 10 * 32, 64, 32);
            RectRiver_mouth3 = new Rectangle(4 * 1 + 3 * 64, 11 * 1 + 10 * 32, 64, 32);

            //Define tile object
            desert1 = (Bitmap)terrain1.Clone(RectTileDesert1, terrain1.PixelFormat);
            desert3 = (Bitmap)terrain1.Clone(RectTileDesert3, terrain1.PixelFormat);
            plains1 = (Bitmap)terrain1.Clone(RectTilePlains1, terrain1.PixelFormat);
            grasslands1 = (Bitmap)terrain1.Clone(RectTileGrasslands1, terrain1.PixelFormat);
            forest_base1 = (Bitmap)terrain1.Clone(RectTileForest_base1, terrain1.PixelFormat);
            hills_base1 = (Bitmap)terrain1.Clone(RectTileHills_base1, terrain1.PixelFormat);
            mountains_base1 = (Bitmap)terrain1.Clone(RectTileMountains_base1, terrain1.PixelFormat);
            tundra1 = (Bitmap)terrain1.Clone(RectTileTundra1, terrain1.PixelFormat);
            arctic1 = (Bitmap)terrain1.Clone(RectTileArctic1, terrain1.PixelFormat);
            swamp1 = (Bitmap)terrain1.Clone(RectTileSwamp1, terrain1.PixelFormat);
            jungle1 = (Bitmap)terrain1.Clone(RectTileJungle1, terrain1.PixelFormat);
            ocean1 = (Bitmap)terrain1.Clone(RectTileOcean1, terrain1.PixelFormat);
            //define coast tile object
            coast0N = (Bitmap)terrain2.Clone(RectCoast0N, terrain2.PixelFormat);
            coast0W = (Bitmap)terrain2.Clone(RectCoast0W, terrain2.PixelFormat);
            coast0E = (Bitmap)terrain2.Clone(RectCoast0E, terrain2.PixelFormat);
            coast0S = (Bitmap)terrain2.Clone(RectCoast0S, terrain2.PixelFormat);
            coast1N = (Bitmap)terrain2.Clone(RectCoast1N, terrain2.PixelFormat);
            coast1W = (Bitmap)terrain2.Clone(RectCoast1W, terrain2.PixelFormat);
            coast1E = (Bitmap)terrain2.Clone(RectCoast1E, terrain2.PixelFormat);
            coast1S = (Bitmap)terrain2.Clone(RectCoast1S, terrain2.PixelFormat);
            coast2N = (Bitmap)terrain2.Clone(RectCoast2N, terrain2.PixelFormat);
            coast2W = (Bitmap)terrain2.Clone(RectCoast2W, terrain2.PixelFormat);
            coast2E = (Bitmap)terrain2.Clone(RectCoast2E, terrain2.PixelFormat);
            coast2S = (Bitmap)terrain2.Clone(RectCoast2S, terrain2.PixelFormat);
            coast3N = (Bitmap)terrain2.Clone(RectCoast3N, terrain2.PixelFormat);
            coast3W = (Bitmap)terrain2.Clone(RectCoast3W, terrain2.PixelFormat);
            coast3E = (Bitmap)terrain2.Clone(RectCoast3E, terrain2.PixelFormat);
            coast3S = (Bitmap)terrain2.Clone(RectCoast3S, terrain2.PixelFormat);
            coast4N = (Bitmap)terrain2.Clone(RectCoast4N, terrain2.PixelFormat);
            coast4W = (Bitmap)terrain2.Clone(RectCoast4W, terrain2.PixelFormat);
            coast4E = (Bitmap)terrain2.Clone(RectCoast4E, terrain2.PixelFormat);
            coast4S = (Bitmap)terrain2.Clone(RectCoast4S, terrain2.PixelFormat);
            coast5N = (Bitmap)terrain2.Clone(RectCoast5N, terrain2.PixelFormat);
            coast5W = (Bitmap)terrain2.Clone(RectCoast5W, terrain2.PixelFormat);
            coast5E = (Bitmap)terrain2.Clone(RectCoast5E, terrain2.PixelFormat);
            coast5S = (Bitmap)terrain2.Clone(RectCoast5S, terrain2.PixelFormat);
            coast6N = (Bitmap)terrain2.Clone(RectCoast6N, terrain2.PixelFormat);
            coast6W = (Bitmap)terrain2.Clone(RectCoast6W, terrain2.PixelFormat);
            coast6E = (Bitmap)terrain2.Clone(RectCoast6E, terrain2.PixelFormat);
            coast6S = (Bitmap)terrain2.Clone(RectCoast6S, terrain2.PixelFormat);
            coast7N = (Bitmap)terrain2.Clone(RectCoast7N, terrain2.PixelFormat);
            coast7W = (Bitmap)terrain2.Clone(RectCoast7W, terrain2.PixelFormat);
            coast7E = (Bitmap)terrain2.Clone(RectCoast7E, terrain2.PixelFormat);
            coast7S = (Bitmap)terrain2.Clone(RectCoast7S, terrain2.PixelFormat);
            //define river/hill/mountain/forest tile objects
            river0 = (Bitmap)terrain2.Clone(RectRiver0, terrain2.PixelFormat);
            river1 = (Bitmap)terrain2.Clone(RectRiver1, terrain2.PixelFormat);
            river2 = (Bitmap)terrain2.Clone(RectRiver2, terrain2.PixelFormat);
            river3 = (Bitmap)terrain2.Clone(RectRiver3, terrain2.PixelFormat);
            river4 = (Bitmap)terrain2.Clone(RectRiver4, terrain2.PixelFormat);
            river5 = (Bitmap)terrain2.Clone(RectRiver5, terrain2.PixelFormat);
            river6 = (Bitmap)terrain2.Clone(RectRiver6, terrain2.PixelFormat);
            river7 = (Bitmap)terrain2.Clone(RectRiver7, terrain2.PixelFormat);
            river8 = (Bitmap)terrain2.Clone(RectRiver8, terrain2.PixelFormat);
            river9 = (Bitmap)terrain2.Clone(RectRiver9, terrain2.PixelFormat);
            river10 = (Bitmap)terrain2.Clone(RectRiver10, terrain2.PixelFormat);
            river11 = (Bitmap)terrain2.Clone(RectRiver11, terrain2.PixelFormat);
            river12 = (Bitmap)terrain2.Clone(RectRiver12, terrain2.PixelFormat);
            river13 = (Bitmap)terrain2.Clone(RectRiver13, terrain2.PixelFormat);
            river14 = (Bitmap)terrain2.Clone(RectRiver14, terrain2.PixelFormat);
            river15 = (Bitmap)terrain2.Clone(RectRiver15, terrain2.PixelFormat);
            forest0 = (Bitmap)terrain2.Clone(RectForest0, terrain2.PixelFormat);
            forest1 = (Bitmap)terrain2.Clone(RectForest1, terrain2.PixelFormat);
            forest2 = (Bitmap)terrain2.Clone(RectForest2, terrain2.PixelFormat);
            forest3 = (Bitmap)terrain2.Clone(RectForest3, terrain2.PixelFormat);
            forest4 = (Bitmap)terrain2.Clone(RectForest4, terrain2.PixelFormat);
            forest5 = (Bitmap)terrain2.Clone(RectForest5, terrain2.PixelFormat);
            forest6 = (Bitmap)terrain2.Clone(RectForest6, terrain2.PixelFormat);
            forest7 = (Bitmap)terrain2.Clone(RectForest7, terrain2.PixelFormat);
            forest8 = (Bitmap)terrain2.Clone(RectForest8, terrain2.PixelFormat);
            forest9 = (Bitmap)terrain2.Clone(RectForest9, terrain2.PixelFormat);
            forest10 = (Bitmap)terrain2.Clone(RectForest10, terrain2.PixelFormat);
            forest11 = (Bitmap)terrain2.Clone(RectForest11, terrain2.PixelFormat);
            forest12 = (Bitmap)terrain2.Clone(RectForest12, terrain2.PixelFormat);
            forest13 = (Bitmap)terrain2.Clone(RectForest13, terrain2.PixelFormat);
            forest14 = (Bitmap)terrain2.Clone(RectForest14, terrain2.PixelFormat);
            forest15 = (Bitmap)terrain2.Clone(RectForest15, terrain2.PixelFormat);
            mountains0 = (Bitmap)terrain2.Clone(RectMountains0, terrain2.PixelFormat);
            mountains1 = (Bitmap)terrain2.Clone(RectMountains1, terrain2.PixelFormat);
            mountains2 = (Bitmap)terrain2.Clone(RectMountains2, terrain2.PixelFormat);
            mountains3 = (Bitmap)terrain2.Clone(RectMountains3, terrain2.PixelFormat);
            mountains4 = (Bitmap)terrain2.Clone(RectMountains4, terrain2.PixelFormat);
            mountains5 = (Bitmap)terrain2.Clone(RectMountains5, terrain2.PixelFormat);
            mountains6 = (Bitmap)terrain2.Clone(RectMountains6, terrain2.PixelFormat);
            mountains7 = (Bitmap)terrain2.Clone(RectMountains7, terrain2.PixelFormat);
            mountains8 = (Bitmap)terrain2.Clone(RectMountains8, terrain2.PixelFormat);
            mountains9 = (Bitmap)terrain2.Clone(RectMountains9, terrain2.PixelFormat);
            mountains10 = (Bitmap)terrain2.Clone(RectMountains10, terrain2.PixelFormat);
            mountains11 = (Bitmap)terrain2.Clone(RectMountains11, terrain2.PixelFormat);
            mountains12 = (Bitmap)terrain2.Clone(RectMountains12, terrain2.PixelFormat);
            mountains13 = (Bitmap)terrain2.Clone(RectMountains13, terrain2.PixelFormat);
            mountains14 = (Bitmap)terrain2.Clone(RectMountains14, terrain2.PixelFormat);
            mountains15 = (Bitmap)terrain2.Clone(RectMountains15, terrain2.PixelFormat);
            hills0 = (Bitmap)terrain2.Clone(RectHills0, terrain2.PixelFormat);
            hills1 = (Bitmap)terrain2.Clone(RectHills1, terrain2.PixelFormat);
            hills2 = (Bitmap)terrain2.Clone(RectHills2, terrain2.PixelFormat);
            hills3 = (Bitmap)terrain2.Clone(RectHills3, terrain2.PixelFormat);
            hills4 = (Bitmap)terrain2.Clone(RectHills4, terrain2.PixelFormat);
            hills5 = (Bitmap)terrain2.Clone(RectHills5, terrain2.PixelFormat);
            hills6 = (Bitmap)terrain2.Clone(RectHills6, terrain2.PixelFormat);
            hills7 = (Bitmap)terrain2.Clone(RectHills7, terrain2.PixelFormat);
            hills8 = (Bitmap)terrain2.Clone(RectHills8, terrain2.PixelFormat);
            hills9 = (Bitmap)terrain2.Clone(RectHills9, terrain2.PixelFormat);
            hills10 = (Bitmap)terrain2.Clone(RectHills10, terrain2.PixelFormat);
            hills11 = (Bitmap)terrain2.Clone(RectHills11, terrain2.PixelFormat);
            hills12 = (Bitmap)terrain2.Clone(RectHills12, terrain2.PixelFormat);
            hills13 = (Bitmap)terrain2.Clone(RectHills13, terrain2.PixelFormat);
            hills14 = (Bitmap)terrain2.Clone(RectHills14, terrain2.PixelFormat);
            hills15 = (Bitmap)terrain2.Clone(RectHills15, terrain2.PixelFormat);
            river_mouth0 = (Bitmap)terrain2.Clone(RectRiver_mouth0, terrain2.PixelFormat);
            river_mouth1 = (Bitmap)terrain2.Clone(RectRiver_mouth1, terrain2.PixelFormat);
            river_mouth2 = (Bitmap)terrain2.Clone(RectRiver_mouth2, terrain2.PixelFormat);
            river_mouth3 = (Bitmap)terrain2.Clone(RectRiver_mouth3, terrain2.PixelFormat);

            //define transparent colors
            transparentGray = desert1.GetPixel(1, 1);    //define transparent back color (gray)
            transparentPink = desert3.GetPixel(5, 16);    //define transparent back color (pink)
            //Make transparent background for tile images
            desert1.MakeTransparent(transparentGray);
            desert3.MakeTransparent(transparentGray);
            desert3.MakeTransparent(transparentPink);
            plains1.MakeTransparent(transparentGray);
            grasslands1.MakeTransparent(transparentGray);
            forest_base1.MakeTransparent(transparentGray);
            hills_base1.MakeTransparent(transparentGray);
            mountains_base1.MakeTransparent(transparentGray);
            tundra1.MakeTransparent(transparentGray);
            arctic1.MakeTransparent(transparentGray);
            swamp1.MakeTransparent(transparentGray);
            jungle1.MakeTransparent(transparentGray);
            ocean1.MakeTransparent(transparentGray);
            //Make transparent background for coast tile images
            coast0N.MakeTransparent(transparentGray);
            coast0W.MakeTransparent(transparentGray);
            coast0E.MakeTransparent(transparentGray);
            coast0S.MakeTransparent(transparentGray);
            coast1N.MakeTransparent(transparentGray);
            coast1W.MakeTransparent(transparentGray);
            coast1E.MakeTransparent(transparentGray);
            coast1S.MakeTransparent(transparentGray);
            coast2N.MakeTransparent(transparentGray);
            coast2W.MakeTransparent(transparentGray);
            coast2E.MakeTransparent(transparentGray);
            coast2S.MakeTransparent(transparentGray);
            coast3N.MakeTransparent(transparentGray);
            coast3W.MakeTransparent(transparentGray);
            coast3E.MakeTransparent(transparentGray);
            coast3S.MakeTransparent(transparentGray);
            coast4N.MakeTransparent(transparentGray);
            coast4W.MakeTransparent(transparentGray);
            coast4E.MakeTransparent(transparentGray);
            coast4S.MakeTransparent(transparentGray);
            coast5N.MakeTransparent(transparentGray);
            coast5W.MakeTransparent(transparentGray);
            coast5E.MakeTransparent(transparentGray);
            coast5S.MakeTransparent(transparentGray);
            coast6N.MakeTransparent(transparentGray);
            coast6W.MakeTransparent(transparentGray);
            coast6E.MakeTransparent(transparentGray);
            coast6S.MakeTransparent(transparentGray);
            coast7N.MakeTransparent(transparentGray);
            coast7W.MakeTransparent(transparentGray);
            coast7E.MakeTransparent(transparentGray);
            coast7S.MakeTransparent(transparentGray);
            //Make transparent background for river/hills/mountains/forest tile images
            river0.MakeTransparent(transparentGray);
            river0.MakeTransparent(transparentPink);
            river1.MakeTransparent(transparentGray);
            river1.MakeTransparent(transparentPink);
            river2.MakeTransparent(transparentGray);
            river2.MakeTransparent(transparentPink);
            river3.MakeTransparent(transparentGray);
            river3.MakeTransparent(transparentPink);
            river4.MakeTransparent(transparentGray);
            river4.MakeTransparent(transparentPink);
            river5.MakeTransparent(transparentGray);
            river5.MakeTransparent(transparentPink);
            river6.MakeTransparent(transparentGray);
            river6.MakeTransparent(transparentPink);
            river7.MakeTransparent(transparentGray);
            river7.MakeTransparent(transparentPink);
            river8.MakeTransparent(transparentGray);
            river8.MakeTransparent(transparentPink);
            river9.MakeTransparent(transparentGray);
            river9.MakeTransparent(transparentPink);
            river10.MakeTransparent(transparentGray);
            river10.MakeTransparent(transparentPink);
            river11.MakeTransparent(transparentGray);
            river11.MakeTransparent(transparentPink);
            river12.MakeTransparent(transparentGray);
            river12.MakeTransparent(transparentPink);
            river13.MakeTransparent(transparentGray);
            river13.MakeTransparent(transparentPink);
            river14.MakeTransparent(transparentGray);
            river14.MakeTransparent(transparentPink);
            river15.MakeTransparent(transparentGray);
            river15.MakeTransparent(transparentPink);
            forest0.MakeTransparent(transparentGray);
            forest0.MakeTransparent(transparentPink);
            forest1.MakeTransparent(transparentGray);
            forest1.MakeTransparent(transparentPink);
            forest2.MakeTransparent(transparentGray);
            forest2.MakeTransparent(transparentPink);
            forest3.MakeTransparent(transparentGray);
            forest3.MakeTransparent(transparentPink);
            forest4.MakeTransparent(transparentGray);
            forest4.MakeTransparent(transparentPink);
            forest5.MakeTransparent(transparentGray);
            forest5.MakeTransparent(transparentPink);
            forest6.MakeTransparent(transparentGray);
            forest6.MakeTransparent(transparentPink);
            forest7.MakeTransparent(transparentGray);
            forest7.MakeTransparent(transparentPink);
            forest8.MakeTransparent(transparentGray);
            forest8.MakeTransparent(transparentPink);
            forest9.MakeTransparent(transparentGray);
            forest9.MakeTransparent(transparentPink);
            forest10.MakeTransparent(transparentGray);
            forest10.MakeTransparent(transparentPink);
            forest11.MakeTransparent(transparentGray);
            forest11.MakeTransparent(transparentPink);
            forest12.MakeTransparent(transparentGray);
            forest12.MakeTransparent(transparentPink);
            forest13.MakeTransparent(transparentGray);
            forest13.MakeTransparent(transparentPink);
            forest14.MakeTransparent(transparentGray);
            forest14.MakeTransparent(transparentPink);
            forest15.MakeTransparent(transparentGray);
            forest15.MakeTransparent(transparentPink);
            mountains0.MakeTransparent(transparentGray);
            mountains0.MakeTransparent(transparentPink);
            mountains1.MakeTransparent(transparentGray);
            mountains1.MakeTransparent(transparentPink);
            mountains2.MakeTransparent(transparentGray);
            mountains2.MakeTransparent(transparentPink);
            mountains3.MakeTransparent(transparentGray);
            mountains3.MakeTransparent(transparentPink);
            mountains4.MakeTransparent(transparentGray);
            mountains4.MakeTransparent(transparentPink);
            mountains5.MakeTransparent(transparentGray);
            mountains5.MakeTransparent(transparentPink);
            mountains6.MakeTransparent(transparentGray);
            mountains6.MakeTransparent(transparentPink);
            mountains7.MakeTransparent(transparentGray);
            mountains7.MakeTransparent(transparentPink);
            mountains8.MakeTransparent(transparentGray);
            mountains8.MakeTransparent(transparentPink);
            mountains9.MakeTransparent(transparentGray);
            mountains9.MakeTransparent(transparentPink);
            mountains10.MakeTransparent(transparentGray);
            mountains10.MakeTransparent(transparentPink);
            mountains11.MakeTransparent(transparentGray);
            mountains11.MakeTransparent(transparentPink);
            mountains12.MakeTransparent(transparentGray);
            mountains12.MakeTransparent(transparentPink);
            mountains13.MakeTransparent(transparentGray);
            mountains13.MakeTransparent(transparentPink);
            mountains14.MakeTransparent(transparentGray);
            mountains14.MakeTransparent(transparentPink);
            mountains15.MakeTransparent(transparentGray);
            mountains15.MakeTransparent(transparentPink);
            hills0.MakeTransparent(transparentGray);
            hills0.MakeTransparent(transparentPink);
            hills1.MakeTransparent(transparentGray);
            hills1.MakeTransparent(transparentPink);
            hills2.MakeTransparent(transparentGray);
            hills2.MakeTransparent(transparentPink);
            hills3.MakeTransparent(transparentGray);
            hills3.MakeTransparent(transparentPink);
            hills4.MakeTransparent(transparentGray);
            hills4.MakeTransparent(transparentPink);
            hills5.MakeTransparent(transparentGray);
            hills5.MakeTransparent(transparentPink);
            hills6.MakeTransparent(transparentGray);
            hills6.MakeTransparent(transparentPink);
            hills7.MakeTransparent(transparentGray);
            hills7.MakeTransparent(transparentPink);
            hills8.MakeTransparent(transparentGray);
            hills8.MakeTransparent(transparentPink);
            hills9.MakeTransparent(transparentGray);
            hills9.MakeTransparent(transparentPink);
            hills10.MakeTransparent(transparentGray);
            hills10.MakeTransparent(transparentPink);
            hills11.MakeTransparent(transparentGray);
            hills11.MakeTransparent(transparentPink);
            hills12.MakeTransparent(transparentGray);
            hills12.MakeTransparent(transparentPink);
            hills13.MakeTransparent(transparentGray);
            hills13.MakeTransparent(transparentPink);
            hills14.MakeTransparent(transparentGray);
            hills14.MakeTransparent(transparentPink);
            hills15.MakeTransparent(transparentGray);
            hills15.MakeTransparent(transparentPink);
            river_mouth0.MakeTransparent(transparentGray);
            river_mouth0.MakeTransparent(transparentPink);
            river_mouth0.MakeTransparent(Color.Cyan);
            river_mouth1.MakeTransparent(transparentGray);
            river_mouth1.MakeTransparent(transparentPink);
            river_mouth1.MakeTransparent(Color.Cyan);
            river_mouth2.MakeTransparent(transparentGray);
            river_mouth2.MakeTransparent(transparentPink);
            river_mouth2.MakeTransparent(Color.Cyan);
            river_mouth3.MakeTransparent(transparentGray);
            river_mouth3.MakeTransparent(transparentPink);
            river_mouth3.MakeTransparent(Color.Cyan);

            //define a bitmap for drawing in MapForm
            bitmap1 = new System.Drawing.Bitmap(Map.MapXdimension * 64, Map.MapYdimension * 32);

            //Make a new offline bitmap
            Bitmap imageDrawn;
            int indeks, indeksN, indeksS, indeksE, indeksW, indeksNW, indeksNE, indeksSE, indeksSW;
            using (Graphics graphics = Graphics.FromImage(bitmap1))
            {
                for (int i = 0; i < Map.MapXdimension; i++)
                {
                    for (int j = 0; j < Map.MapYdimension - 1; j++) //POPRAVI -1 !!!!
                    {
                        //choose correct tile
                        indeks = j * Map.MapXdimension + i;
                        if ((Map.Terrain[indeks] == 0) || (Map.Terrain[indeks] == 128)) { imageDrawn = desert1; }
                        else if ((Map.Terrain[indeks] == 1) || (Map.Terrain[indeks] == 129)) { imageDrawn = plains1; }
                        else if ((Map.Terrain[indeks] == 2) || (Map.Terrain[indeks] == 130)) { imageDrawn = grasslands1; }
                        else if ((Map.Terrain[indeks] == 3) || (Map.Terrain[indeks] == 131)) { imageDrawn = forest_base1; }
                        else if ((Map.Terrain[indeks] == 4) || (Map.Terrain[indeks] == 132)) { imageDrawn = hills_base1; }
                        else if ((Map.Terrain[indeks] == 5) || (Map.Terrain[indeks] == 133)) { imageDrawn = mountains_base1; }
                        else if ((Map.Terrain[indeks] == 6) || (Map.Terrain[indeks] == 134)) { imageDrawn = tundra1; }
                        else if ((Map.Terrain[indeks] == 7) || (Map.Terrain[indeks] == 135)) { imageDrawn = arctic1; }
                        else if ((Map.Terrain[indeks] == 8) || (Map.Terrain[indeks] == 136)) { imageDrawn = swamp1; }
                        else if ((Map.Terrain[indeks] == 9) || (Map.Terrain[indeks] == 137)) { imageDrawn = jungle1; }
                        else if (Map.Terrain[indeks] == 10) { imageDrawn = ocean1; }
                        else if (Map.Terrain[indeks] == 74) { imageDrawn = ocean1; }
                        else { imageDrawn = desert3; }

                        graphics.DrawImage(imageDrawn, 64 * i + 32 * (j % 2) + 1, 16 * j + 1);   //draw map tile

                        //draw coast
                        if ((Map.Terrain[indeks] == 10) || (Map.Terrain[indeks] == 74))   //if tile is ocean
                        {
                            //define indexes of surrounding tiles
                            //N:
                            indeksN = (j - 2) * Map.MapXdimension + i;
                            //NE:
                            if ((j % 2 == 1) && (i == Map.MapXdimension - 1)) { indeksNE = indeks - Map.MapXdimension + 1; }    //if we are on map edge
                            else { indeksNE = indeks - Map.MapXdimension + (j % 2); }
                            //E:
                            if (i == Map.MapXdimension - 1) { indeksE = indeks - Map.MapXdimension + 1; }  //if we are on map edge
                            else { indeksE = indeks + 1; }
                            //SE:
                            if ((j % 2 == 1) && (i == Map.MapXdimension - 1)) { indeksSE = indeks + 1; }    //if we are on map edge
                            else { indeksSE = indeks + Map.MapXdimension + (j % 2); }
                            //S:
                            indeksS = (j + 2) * Map.MapXdimension + i;
                            //SW:
                            if ((j % 2 == 0) && (i == 0)) { indeksSW = indeks + 2 * Map.MapXdimension - 1; }    //if we are on map edge
                            else { indeksSW = indeks + Map.MapXdimension - 1 + (j % 2); }
                            //W:
                            if (i == 0) { indeksW = indeks + Map.MapXdimension - 1; }  //if we are on map edge
                            else { indeksW = indeks - 1; }
                            //NW:
                            if ((j % 2 == 0) && (i == 0)) { indeksNW = indeks - 1; }    //if we are on map edge
                            else { indeksNW = indeks - Map.MapXdimension - 1 + (j % 2); }

                            int[] land = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 }; //in start we presume all surrounding tiles are water (land=1, water=0). Starting 0 is North, follows in clockwise direction.

                            //observe in all directions if land is present next to ocean
                            //N:
                            //if (j == 1) { land[0] = 1; }   //if N tile is out of map (black tile), we presume it is land
                            if (indeksN < 0) { land[0] = 1; }
                            else if ((Map.Terrain[indeksN] != 10) && (Map.Terrain[indeksN] != 74)) { land[0] = 1; }
                            //NE:
                            if (indeksNE < 0) { land[1] = 1; }
                            else if ((Map.Terrain[indeksNE] != 10) && (Map.Terrain[indeksNE] != 74)) { land[1] = 1; }    //if it is not ocean, it is land
                            //E:
                            if ((Map.Terrain[indeksE] != 10) && (Map.Terrain[indeksE] != 74)) { land[2] = 1; }
                            //SE:
                            if ((Map.Terrain[indeksSE] != 10) && (Map.Terrain[indeksSE] != 74)) { land[3] = 1; }
                            //S:
                            if (j == Map.MapYdimension - 2) { land[4] = 1; }   //if S tile is out of map (black tile), we presume it is land
                            else if ((Map.Terrain[indeksS] != 10) && (Map.Terrain[indeksS] != 74)) { land[4] = 1; }
                            //SW:
                            if ((Map.Terrain[indeksSW] != 10) && (Map.Terrain[indeksSW] != 74)) { land[5] = 1; }
                            //W:
                            if ((Map.Terrain[indeksW] != 10) && (Map.Terrain[indeksW] != 74)) { land[6] = 1; }
                            //NW:
                            if (indeksNW < 0) { land[7] = 1; }
                            else if ((Map.Terrain[indeksNW] != 10) && (Map.Terrain[indeksNW] != 74)) { land[7] = 1; }

                            //draw coast & river mouth tiles
                            //NW+N+NE tiles
                            if ((land[7] == 1) && (land[0] == 0) && (land[1] == 0)) { graphics.DrawImage(coast1N, 64 * i + 32 * (j % 2) + 1 + 16, 16 * j + 1); }
                            if ((land[7] == 0) && (land[0] == 1) && (land[1] == 0)) { graphics.DrawImage(coast2N, 64 * i + 32 * (j % 2) + 1 + 16, 16 * j + 1); }
                            if ((land[7] == 1) && (land[0] == 1) && (land[1] == 0)) { graphics.DrawImage(coast3N, 64 * i + 32 * (j % 2) + 1 + 16, 16 * j + 1); }
                            if ((land[7] == 0) && (land[0] == 0) && (land[1] == 1)) { graphics.DrawImage(coast4N, 64 * i + 32 * (j % 2) + 1 + 16, 16 * j + 1); }
                            if ((land[7] == 1) && (land[0] == 0) && (land[1] == 1)) { graphics.DrawImage(coast5N, 64 * i + 32 * (j % 2) + 1 + 16, 16 * j + 1); }
                            if ((land[7] == 0) && (land[0] == 1) && (land[1] == 1)) { graphics.DrawImage(coast6N, 64 * i + 32 * (j % 2) + 1 + 16, 16 * j + 1); }
                            if ((land[7] == 1) && (land[0] == 1) && (land[1] == 1)) { graphics.DrawImage(coast7N, 64 * i + 32 * (j % 2) + 1 + 16, 16 * j + 1); }
                            //SW+S+SE tiles
                            if ((land[3] == 1) && (land[4] == 0) && (land[5] == 0)) { graphics.DrawImage(coast1S, 64 * i + 32 * (j % 2) + 1 + 16, 16 * j + 1 + 16); }
                            if ((land[3] == 0) && (land[4] == 1) && (land[5] == 0)) { graphics.DrawImage(coast2S, 64 * i + 32 * (j % 2) + 1 + 16, 16 * j + 1 + 16); }
                            if ((land[3] == 1) && (land[4] == 1) && (land[5] == 0)) { graphics.DrawImage(coast3S, 64 * i + 32 * (j % 2) + 1 + 16, 16 * j + 1 + 16); }
                            if ((land[3] == 0) && (land[4] == 0) && (land[5] == 1)) { graphics.DrawImage(coast4S, 64 * i + 32 * (j % 2) + 1 + 16, 16 * j + 1 + 16); }
                            if ((land[3] == 1) && (land[4] == 0) && (land[5] == 1)) { graphics.DrawImage(coast5S, 64 * i + 32 * (j % 2) + 1 + 16, 16 * j + 1 + 16); }
                            if ((land[3] == 0) && (land[4] == 1) && (land[5] == 1)) { graphics.DrawImage(coast6S, 64 * i + 32 * (j % 2) + 1 + 16, 16 * j + 1 + 16); }
                            if ((land[3] == 1) && (land[4] == 1) && (land[5] == 1)) { graphics.DrawImage(coast7S, 64 * i + 32 * (j % 2) + 1 + 16, 16 * j + 1 + 16); }
                            //SW+W+NW tiles
                            if ((land[5] == 1) && (land[6] == 0) && (land[7] == 0)) { graphics.DrawImage(coast1W, 64 * i + 32 * (j % 2) + 1, 16 * j + 1 + 8); }
                            if ((land[5] == 0) && (land[6] == 1) && (land[7] == 0)) { graphics.DrawImage(coast2W, 64 * i + 32 * (j % 2) + 1, 16 * j + 1 + 8); }
                            if ((land[5] == 1) && (land[6] == 1) && (land[7] == 0)) { graphics.DrawImage(coast3W, 64 * i + 32 * (j % 2) + 1, 16 * j + 1 + 8); }
                            if ((land[5] == 0) && (land[6] == 0) && (land[7] == 1)) { graphics.DrawImage(coast4W, 64 * i + 32 * (j % 2) + 1, 16 * j + 1 + 8); }
                            if ((land[5] == 1) && (land[6] == 0) && (land[7] == 1)) { graphics.DrawImage(coast5W, 64 * i + 32 * (j % 2) + 1, 16 * j + 1 + 8); }
                            if ((land[5] == 0) && (land[6] == 1) && (land[7] == 1)) { graphics.DrawImage(coast6W, 64 * i + 32 * (j % 2) + 1, 16 * j + 1 + 8); }
                            if ((land[5] == 1) && (land[6] == 1) && (land[7] == 1)) { graphics.DrawImage(coast7W, 64 * i + 32 * (j % 2) + 1, 16 * j + 1 + 8); }
                            //NE+E+SE tiles
                            if ((land[1] == 1) && (land[2] == 0) && (land[3] == 0)) { graphics.DrawImage(coast1E, 64 * i + 32 * (j % 2) + 1 + 32, 16 * j + 1 + 8); }
                            if ((land[1] == 0) && (land[2] == 1) && (land[3] == 0)) { graphics.DrawImage(coast2E, 64 * i + 32 * (j % 2) + 1 + 32, 16 * j + 1 + 8); }
                            if ((land[1] == 1) && (land[2] == 1) && (land[3] == 0)) { graphics.DrawImage(coast3E, 64 * i + 32 * (j % 2) + 1 + 32, 16 * j + 1 + 8); }
                            if ((land[1] == 0) && (land[2] == 0) && (land[3] == 1)) { graphics.DrawImage(coast4E, 64 * i + 32 * (j % 2) + 1 + 32, 16 * j + 1 + 8); }
                            if ((land[1] == 1) && (land[2] == 0) && (land[3] == 1)) { graphics.DrawImage(coast5E, 64 * i + 32 * (j % 2) + 1 + 32, 16 * j + 1 + 8); }
                            if ((land[1] == 0) && (land[2] == 1) && (land[3] == 1)) { graphics.DrawImage(coast6E, 64 * i + 32 * (j % 2) + 1 + 32, 16 * j + 1 + 8); }
                            if ((land[1] == 1) && (land[2] == 1) && (land[3] == 1)) { graphics.DrawImage(coast7E, 64 * i + 32 * (j % 2) + 1 + 32, 16 * j + 1 + 8); }

                            //draw river mouth
                            //if next to ocean is river, draw river mouth on this tile
                            if ((indeksNE>=0) && (Map.Terrain[indeksNE] >= 128) && (Map.Terrain[indeksNE] <= 137)) { graphics.DrawImage(river_mouth0, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if ((Map.Terrain[indeksSE] >= 128) && (Map.Terrain[indeksSE] <= 137)) { graphics.DrawImage(river_mouth1, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if ((Map.Terrain[indeksSW] >= 128) && (Map.Terrain[indeksSW] <= 137)) { graphics.DrawImage(river_mouth2, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if ((indeksNW >= 0) && (Map.Terrain[indeksNW] >= 128) && (Map.Terrain[indeksNW] <= 137)) { graphics.DrawImage(river_mouth3, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                        }

                        //draw forests
                        int[] forestAround = new int[4] { 0, 0, 0, 0 }; //in start we presume all surrounding tiles are not forest (forest=1, no forest=0). Starting 0 is NE, follows in clockwise direction.
                        if ((Map.Terrain[indeks] == 3) || (Map.Terrain[indeks] == 131))   //if tile is forest or fores+river
                        {
                            //NE:
                            if ((j % 2 == 1) && (i == Map.MapXdimension - 1)) { indeksNE = indeks - Map.MapXdimension + 1; }    //if we are on map edge
                            else { indeksNE = indeks - Map.MapXdimension + (j % 2); }
                            //SE:
                            if ((j % 2 == 1) && (i == Map.MapXdimension - 1)) { indeksSE = indeks + 1; }    //if we are on map edge
                            else { indeksSE = indeks + Map.MapXdimension + (j % 2); }
                            //SW:
                            if ((j % 2 == 0) && (i == 0)) { indeksSW = indeks + 2 * Map.MapXdimension - 1; }    //if we are on map edge
                            else { indeksSW = indeks + Map.MapXdimension - 1 + (j % 2); }
                            //NW:
                            if ((j % 2 == 0) && (i == 0)) { indeksNW = indeks - 1; }    //if we are on map edge
                            else { indeksNW = indeks - Map.MapXdimension - 1 + (j % 2); }

                            //observe in all 4 directions if forest is present
                            //NE
                            if (indeksNE < 0) { forestAround[0] = 0; }
                            else if ((Map.Terrain[indeksNE] == 3) || (Map.Terrain[indeksNE] == 131)) { forestAround[0] = 1; }    //this is forest
                            //SE
                            if ((Map.Terrain[indeksSE] == 3) || (Map.Terrain[indeksSE] == 131)) { forestAround[1] = 1; }
                            //SW
                            if ((Map.Terrain[indeksSW] == 3) || (Map.Terrain[indeksSW] == 131)) { forestAround[2] = 1; }
                            //NW
                            if (indeksNW < 0) { forestAround[3] = 0; }
                            else if ((Map.Terrain[indeksNW] == 3) || (Map.Terrain[indeksNW] == 131)) { forestAround[3] = 1; }

                            //draw forest tiles
                            int[] refArray0 = new int[4] { 0, 0, 0, 0 };
                            int[] refArray1 = new int[4] { 1, 0, 0, 0 };
                            int[] refArray2 = new int[4] { 0, 1, 0, 0 };
                            int[] refArray3 = new int[4] { 1, 1, 0, 0 };
                            int[] refArray4 = new int[4] { 0, 0, 1, 0 };
                            int[] refArray5 = new int[4] { 1, 0, 1, 0 };
                            int[] refArray6 = new int[4] { 0, 1, 1, 0 };
                            int[] refArray7 = new int[4] { 1, 1, 1, 0 };
                            int[] refArray8 = new int[4] { 0, 0, 0, 1 };
                            int[] refArray9 = new int[4] { 1, 0, 0, 1 };
                            int[] refArray10 = new int[4] { 0, 1, 0, 1 };
                            int[] refArray11 = new int[4] { 1, 1, 0, 1 };
                            int[] refArray12 = new int[4] { 0, 0, 1, 1 };
                            int[] refArray13 = new int[4] { 1, 0, 1, 1 };
                            int[] refArray14 = new int[4] { 0, 1, 1, 1 };
                            int[] refArray15 = new int[4] { 1, 1, 1, 1 };
                            if (Enumerable.SequenceEqual(forestAround, refArray0)) { graphics.DrawImage(forest0, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(forestAround, refArray1)) { graphics.DrawImage(forest1, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(forestAround, refArray2)) { graphics.DrawImage(forest2, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(forestAround, refArray3)) { graphics.DrawImage(forest3, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(forestAround, refArray4)) { graphics.DrawImage(forest4, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(forestAround, refArray5)) { graphics.DrawImage(forest5, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(forestAround, refArray6)) { graphics.DrawImage(forest6, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(forestAround, refArray7)) { graphics.DrawImage(forest7, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(forestAround, refArray8)) { graphics.DrawImage(forest8, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(forestAround, refArray9)) { graphics.DrawImage(forest9, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(forestAround, refArray10)) { graphics.DrawImage(forest10, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(forestAround, refArray11)) { graphics.DrawImage(forest11, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(forestAround, refArray12)) { graphics.DrawImage(forest12, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(forestAround, refArray13)) { graphics.DrawImage(forest13, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(forestAround, refArray14)) { graphics.DrawImage(forest14, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(forestAround, refArray15)) { graphics.DrawImage(forest15, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                        }

                        //draw mountains
                        //CORRECT: IF SHIELD IS AT MOUNTAIN IT SHOULD BE mountain15!!!
                        int[] mountainAround = new int[4] { 0, 0, 0, 0 }; //in start we presume all surrounding tiles are not mountains (mountain=1, no mountain=0). Starting 0 is NE, follows in clockwise direction.
                        if ((Map.Terrain[indeks] == 5) || (Map.Terrain[indeks] == 133))   //if tile is moountains or mountain+river
                        {
                            //NE:
                            if ((j % 2 == 1) && (i == Map.MapXdimension - 1)) { indeksNE = indeks - Map.MapXdimension + 1; }    //if we are on map edge
                            else { indeksNE = indeks - Map.MapXdimension + (j % 2); }
                            //SE:
                            if ((j % 2 == 1) && (i == Map.MapXdimension - 1)) { indeksSE = indeks + 1; }    //if we are on map edge
                            else { indeksSE = indeks + Map.MapXdimension + (j % 2); }
                            //SW:
                            if ((j % 2 == 0) && (i == 0)) { indeksSW = indeks + 2 * Map.MapXdimension - 1; }    //if we are on map edge
                            else { indeksSW = indeks + Map.MapXdimension - 1 + (j % 2); }
                            //NW:
                            if ((j % 2 == 0) && (i == 0)) { indeksNW = indeks - 1; }    //if we are on map edge
                            else { indeksNW = indeks - Map.MapXdimension - 1 + (j % 2); }

                            //observe in all 4 directions if forest is present
                            //NE
                            if ((Map.Terrain[indeksNE] == 5) || (Map.Terrain[indeksNE] == 133)) { mountainAround[0] = 1; }    //this is mountain
                            //SE
                            if ((Map.Terrain[indeksSE] == 5) || (Map.Terrain[indeksSE] == 133)) { mountainAround[1] = 1; }
                            //SW
                            if ((Map.Terrain[indeksSW] == 5) || (Map.Terrain[indeksSW] == 133)) { mountainAround[2] = 1; }
                            //NW
                            if ((Map.Terrain[indeksNW] == 5) || (Map.Terrain[indeksNW] == 133)) { mountainAround[3] = 1; }

                            //draw mountain tiles
                            int[] refArray0 = new int[4] { 0, 0, 0, 0 };
                            int[] refArray1 = new int[4] { 1, 0, 0, 0 };
                            int[] refArray2 = new int[4] { 0, 1, 0, 0 };
                            int[] refArray3 = new int[4] { 1, 1, 0, 0 };
                            int[] refArray4 = new int[4] { 0, 0, 1, 0 };
                            int[] refArray5 = new int[4] { 1, 0, 1, 0 };
                            int[] refArray6 = new int[4] { 0, 1, 1, 0 };
                            int[] refArray7 = new int[4] { 1, 1, 1, 0 };
                            int[] refArray8 = new int[4] { 0, 0, 0, 1 };
                            int[] refArray9 = new int[4] { 1, 0, 0, 1 };
                            int[] refArray10 = new int[4] { 0, 1, 0, 1 };
                            int[] refArray11 = new int[4] { 1, 1, 0, 1 };
                            int[] refArray12 = new int[4] { 0, 0, 1, 1 };
                            int[] refArray13 = new int[4] { 1, 0, 1, 1 };
                            int[] refArray14 = new int[4] { 0, 1, 1, 1 };
                            int[] refArray15 = new int[4] { 1, 1, 1, 1 };
                            if (Enumerable.SequenceEqual(mountainAround, refArray0)) { graphics.DrawImage(mountains0, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(mountainAround, refArray1)) { graphics.DrawImage(mountains1, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(mountainAround, refArray2)) { graphics.DrawImage(mountains2, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(mountainAround, refArray3)) { graphics.DrawImage(mountains3, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(mountainAround, refArray4)) { graphics.DrawImage(mountains4, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(mountainAround, refArray5)) { graphics.DrawImage(mountains5, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(mountainAround, refArray6)) { graphics.DrawImage(mountains6, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(mountainAround, refArray7)) { graphics.DrawImage(mountains7, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(mountainAround, refArray8)) { graphics.DrawImage(mountains8, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(mountainAround, refArray9)) { graphics.DrawImage(mountains9, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(mountainAround, refArray10)) { graphics.DrawImage(mountains10, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(mountainAround, refArray11)) { graphics.DrawImage(mountains11, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(mountainAround, refArray12)) { graphics.DrawImage(mountains12, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(mountainAround, refArray13)) { graphics.DrawImage(mountains13, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(mountainAround, refArray14)) { graphics.DrawImage(mountains14, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(mountainAround, refArray15)) { graphics.DrawImage(mountains15, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                        }

                        //draw hills
                        int[] hillsAround = new int[4] { 0, 0, 0, 0 }; //in start we presume all surrounding tiles are not hills (hills=1, no hills=0). Starting 0 is NE, follows in clockwise direction.
                        if ((Map.Terrain[indeks] == 4) || (Map.Terrain[indeks] == 132))   //if tile is hills or hills+river
                        {
                            //NE:
                            if ((j % 2 == 1) && (i == Map.MapXdimension - 1)) { indeksNE = indeks - Map.MapXdimension + 1; }    //if we are on map edge
                            else { indeksNE = indeks - Map.MapXdimension + (j % 2); }
                            //SE:
                            if ((j % 2 == 1) && (i == Map.MapXdimension - 1)) { indeksSE = indeks + 1; }    //if we are on map edge
                            else { indeksSE = indeks + Map.MapXdimension + (j % 2); }
                            //SW:
                            if ((j % 2 == 0) && (i == 0)) { indeksSW = indeks + 2 * Map.MapXdimension - 1; }    //if we are on map edge
                            else { indeksSW = indeks + Map.MapXdimension - 1 + (j % 2); }
                            //NW:
                            if ((j % 2 == 0) && (i == 0)) { indeksNW = indeks - 1; }    //if we are on map edge
                            else { indeksNW = indeks - Map.MapXdimension - 1 + (j % 2); }

                            //observe in all 4 directions if forest is present
                            //NE
                            if (indeksNE < 0) { hillsAround[0] = 0; }
                            else if ((Map.Terrain[indeksNE] == 4) || (Map.Terrain[indeksNE] == 132)) { hillsAround[0] = 1; }    //this is mountain
                            //SE
                            if ((Map.Terrain[indeksSE] == 4) || (Map.Terrain[indeksSE] == 132)) { hillsAround[1] = 1; }
                            //SW
                            if ((Map.Terrain[indeksSW] == 4) || (Map.Terrain[indeksSW] == 132)) { hillsAround[2] = 1; }
                            //NW
                            if (indeksNW < 0) { hillsAround[3] = 0; }
                            else if ((Map.Terrain[indeksNW] == 4) || (Map.Terrain[indeksNW] == 132)) { hillsAround[3] = 1; }

                            //draw mountain tiles
                            int[] refArray0 = new int[4] { 0, 0, 0, 0 };
                            int[] refArray1 = new int[4] { 1, 0, 0, 0 };
                            int[] refArray2 = new int[4] { 0, 1, 0, 0 };
                            int[] refArray3 = new int[4] { 1, 1, 0, 0 };
                            int[] refArray4 = new int[4] { 0, 0, 1, 0 };
                            int[] refArray5 = new int[4] { 1, 0, 1, 0 };
                            int[] refArray6 = new int[4] { 0, 1, 1, 0 };
                            int[] refArray7 = new int[4] { 1, 1, 1, 0 };
                            int[] refArray8 = new int[4] { 0, 0, 0, 1 };
                            int[] refArray9 = new int[4] { 1, 0, 0, 1 };
                            int[] refArray10 = new int[4] { 0, 1, 0, 1 };
                            int[] refArray11 = new int[4] { 1, 1, 0, 1 };
                            int[] refArray12 = new int[4] { 0, 0, 1, 1 };
                            int[] refArray13 = new int[4] { 1, 0, 1, 1 };
                            int[] refArray14 = new int[4] { 0, 1, 1, 1 };
                            int[] refArray15 = new int[4] { 1, 1, 1, 1 };
                            if (Enumerable.SequenceEqual(hillsAround, refArray0)) { graphics.DrawImage(hills0, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(hillsAround, refArray1)) { graphics.DrawImage(hills1, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(hillsAround, refArray2)) { graphics.DrawImage(hills2, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(hillsAround, refArray3)) { graphics.DrawImage(hills3, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(hillsAround, refArray4)) { graphics.DrawImage(hills4, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(hillsAround, refArray5)) { graphics.DrawImage(hills5, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(hillsAround, refArray6)) { graphics.DrawImage(hills6, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(hillsAround, refArray7)) { graphics.DrawImage(hills7, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(hillsAround, refArray8)) { graphics.DrawImage(hills8, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(hillsAround, refArray9)) { graphics.DrawImage(hills9, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(hillsAround, refArray10)) { graphics.DrawImage(hills10, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(hillsAround, refArray11)) { graphics.DrawImage(hills11, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(hillsAround, refArray12)) { graphics.DrawImage(hills12, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(hillsAround, refArray13)) { graphics.DrawImage(hills13, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(hillsAround, refArray14)) { graphics.DrawImage(hills14, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(hillsAround, refArray15)) { graphics.DrawImage(hills15, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                        }
                        
                        //draw rivers
                        int[] riverAround = new int[4] { 0, 0, 0, 0 }; //in start we presume all surrounding tiles are not river (river=1, no river=0). Starting 0 is NE, follows in clockwise direction.
                        if ((Map.Terrain[indeks] >= 128) && (Map.Terrain[indeks] <= 137))   //if tile is river
                        {
                            //NE:
                            if ((j % 2 == 1) && (i == Map.MapXdimension - 1)) { indeksNE = indeks - Map.MapXdimension + 1; }    //if we are on map edge
                            else { indeksNE = indeks - Map.MapXdimension + (j % 2); }
                            //SE:
                            if ((j % 2 == 1) && (i == Map.MapXdimension - 1)) { indeksSE = indeks + 1; }    //if we are on map edge
                            else { indeksSE = indeks + Map.MapXdimension + (j % 2); }
                            //SW:
                            if ((j % 2 == 0) && (i == 0)) { indeksSW = indeks + 2 * Map.MapXdimension - 1; }    //if we are on map edge
                            else { indeksSW = indeks + Map.MapXdimension - 1 + (j % 2); }
                            //NW:
                            if ((j % 2 == 0) && (i == 0)) { indeksNW = indeks - 1; }    //if we are on map edge
                            else { indeksNW = indeks - Map.MapXdimension - 1 + (j % 2); }

                            //observe in all 4 directions if forest is present OR if ocean is present (entry into river mouth)
                            //NE
                            if (indeksNE < 0) { riverAround[0] = 0; }
                            else if (((Map.Terrain[indeksNE] >= 128) && (Map.Terrain[indeksNE] <= 137)) || (Map.Terrain[indeksNE] == 10) || (Map.Terrain[indeksNE] == 74)) { riverAround[0] = 1; }    //this is mountain
                            //SE
                            if (((Map.Terrain[indeksSE] >= 128) && (Map.Terrain[indeksSE] <= 137)) || (Map.Terrain[indeksSE] == 10) || (Map.Terrain[indeksSE] == 74)) { riverAround[1] = 1; }
                            //SW
                            if (((Map.Terrain[indeksSW] >= 128) && (Map.Terrain[indeksSW] <= 137)) || (Map.Terrain[indeksSW] == 10) || (Map.Terrain[indeksSW] == 74)) { riverAround[2] = 1; }
                            //NW
                            if (indeksNW < 0) { riverAround[0] = 0; }
                            else if (((Map.Terrain[indeksNW] >= 128) && (Map.Terrain[indeksNW] <= 137)) || (Map.Terrain[indeksNW] == 10) || (Map.Terrain[indeksNW] == 74)) { riverAround[3] = 1; }

                            //draw mountain tiles
                            int[] refArray0 = new int[4] { 0, 0, 0, 0 };
                            int[] refArray1 = new int[4] { 1, 0, 0, 0 };
                            int[] refArray2 = new int[4] { 0, 1, 0, 0 };
                            int[] refArray3 = new int[4] { 1, 1, 0, 0 };
                            int[] refArray4 = new int[4] { 0, 0, 1, 0 };
                            int[] refArray5 = new int[4] { 1, 0, 1, 0 };
                            int[] refArray6 = new int[4] { 0, 1, 1, 0 };
                            int[] refArray7 = new int[4] { 1, 1, 1, 0 };
                            int[] refArray8 = new int[4] { 0, 0, 0, 1 };
                            int[] refArray9 = new int[4] { 1, 0, 0, 1 };
                            int[] refArray10 = new int[4] { 0, 1, 0, 1 };
                            int[] refArray11 = new int[4] { 1, 1, 0, 1 };
                            int[] refArray12 = new int[4] { 0, 0, 1, 1 };
                            int[] refArray13 = new int[4] { 1, 0, 1, 1 };
                            int[] refArray14 = new int[4] { 0, 1, 1, 1 };
                            int[] refArray15 = new int[4] { 1, 1, 1, 1 };
                            if (Enumerable.SequenceEqual(riverAround, refArray0)) { graphics.DrawImage(river0, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(riverAround, refArray1)) { graphics.DrawImage(river1, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(riverAround, refArray2)) { graphics.DrawImage(river2, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(riverAround, refArray3)) { graphics.DrawImage(river3, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(riverAround, refArray4)) { graphics.DrawImage(river4, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(riverAround, refArray5)) { graphics.DrawImage(river5, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(riverAround, refArray6)) { graphics.DrawImage(river6, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(riverAround, refArray7)) { graphics.DrawImage(river7, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(riverAround, refArray8)) { graphics.DrawImage(river8, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(riverAround, refArray9)) { graphics.DrawImage(river9, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(riverAround, refArray10)) { graphics.DrawImage(river10, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(riverAround, refArray11)) { graphics.DrawImage(river11, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(riverAround, refArray12)) { graphics.DrawImage(river12, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(riverAround, refArray13)) { graphics.DrawImage(river13, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(riverAround, refArray14)) { graphics.DrawImage(river14, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }
                            if (Enumerable.SequenceEqual(riverAround, refArray15)) { graphics.DrawImage(river15, 64 * i + 32 * (j % 2) + 1, 16 * j + 1); }

                        }


                    }
                }
            }

        }

    }
}
