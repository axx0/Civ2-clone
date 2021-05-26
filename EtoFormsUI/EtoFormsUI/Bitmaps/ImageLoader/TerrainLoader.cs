using System.Linq;
using Civ2engine;
using Eto.Drawing;
using EtoFormsUIExtensionMethods;

namespace EtoFormsUI.ImageLoader
{
    public static class TerrainLoader
    {
        public static void LoadTerrain(Ruleset ruleset, Rules rules)
        {
            MapImages.Terrains = new TerrainSet[rules.Maps.Length];

            for (var i = 0; i < MapImages.Terrains.Length; i++)
            {
                MapImages.Terrains[i] = LoadTerrain(ruleset, i);
            }
        }

        private static TerrainSet LoadTerrain(Ruleset ruleset, int index)
        {
            using var tileData = Common.LoadBitmapFrom($"Terrain{((index * 2) + 1)}", ruleset);
            using var overlayData = Common.LoadBitmapFrom($"Terrain{((index * 2) + 2)}", ruleset);

            // Initialize objects
            var terrain = new TerrainSet();
            // Desert = new Bitmap[4];
            // Plains = new Bitmap[4];
            // Grassland = new Bitmap[4];
            // ForestBase = new Bitmap[4];
            // HillsBase = new Bitmap[4];
            // MtnsBase = new Bitmap[4];
            // Tundra = new Bitmap[4];
            // Glacier = new Bitmap[4];
            // Swamp = new Bitmap[4];
            // Jungle = new Bitmap[4];
            // Ocean = new Bitmap[4];
            // Coast = new Bitmap[8, 4];
            // River = new Bitmap[16];
            // Forest = new Bitmap[16];
            // Mountains = new Bitmap[16];
            // Hills = new Bitmap[16];
            // RiverMouth = new Bitmap[4];
            // Road = new Bitmap[9];
            // Railroad = new Bitmap[9];

            // Define transparent colors
            var borderColour = tileData.GetPixel(0, 0);
            var transparentGray = Color.FromArgb(135, 135, 135);    // Define transparent back color (gray)
            var transparentPink = Color.FromArgb(255, 0, 255);    // Define transparent back color (pink)
            var transparentCyan = Color.FromArgb(0, 255, 255);    // Define transparent back color (cyan)

            tileData.ReplaceColors(transparentGray, Colors.Transparent);
            tileData.ReplaceColors(transparentPink, Colors.Transparent);

            terrain.BaseTiles = Enumerable.Range(0, 11)
                .Select(i => tileData.Clone(new Rectangle(1, 1 + (33*i), 64, 32))).ToArray();
            
            
            var secondSpecial = tileData.GetPixel(262, 0) == borderColour ? 261 : 196;
            var firstSpecial = secondSpecial - 65;

            terrain.Specials = new [] { Enumerable.Range(0, 11)
                .Select(i => tileData.Clone(new Rectangle(firstSpecial, 1 + (33*i), 64, 32))).ToArray(),           
             Enumerable.Range(0, 11)
                .Select(i => tileData.Clone(new Rectangle(secondSpecial, 1 + (33*i), 64, 32))).ToArray()};
            
            return terrain;

            // // 4 small dither tiles
            // DitherBlank = new Bitmap[2, 2];
            // DitherDots = new Bitmap[2, 2];
            // for (int tileX = 0; tileX < 2; tileX++)
            // {
            //     for (int tileY = 0; tileY < 2; tileY++)
            //     {
            //         DitherBlank[tileX, tileY] = tileData.Clone(new Rectangle((tileX * 32) + 1, (tileY * 16) + 447, 32, 16));
            //         DitherDots[tileX, tileY] = DitherBlank[tileX, tileY];
            //         DitherDots[tileX, tileY].ReplaceColors(transparentGray, Colors.Transparent);
            //         DitherDots[tileX, tileY].ReplaceColors(transparentPink, Colors.Transparent);
            //     }
            // }
            //
            // // Blank tile
            // Blank = tileData.Clone(new Rectangle(131, 447, 64, 32));
            // Blank.ReplaceColors(transparentGray, Colors.Transparent);
            //
            // // Dither base (only useful for grasland?)
            // DitherBase = tileData.Clone(new Rectangle(196, 447, 64, 32));
            //
            // // Replace black dither pixels with base pixels
            // DitherDesert = new Bitmap[2, 2];   // 4 dither tiles for one 64x32 map tile
            // DitherPlains = new Bitmap[2, 2];
            // DitherGrassland = new Bitmap[2, 2];
            // DitherForest = new Bitmap[2, 2];
            // DitherHills = new Bitmap[2, 2];
            // DitherMountains = new Bitmap[2, 2];
            // DitherTundra = new Bitmap[2, 2];
            // DitherGlacier = new Bitmap[2, 2];
            // DitherSwamp = new Bitmap[2, 2];
            // DitherJungle = new Bitmap[2, 2];
            // Color replacementColor;
            // for (int tileX = 0; tileX < 2; tileX++)
            // {    // For 4 directions (NE, SE, SW, NW)
            //     for (int tileY = 0; tileY < 2; tileY++)
            //     {
            //         DitherDesert[tileX, tileY] = new Bitmap(32, 16, PixelFormat.Format32bppRgba);
            //         DitherPlains[tileX, tileY] = new Bitmap(32, 16, PixelFormat.Format32bppRgba);
            //         DitherGrassland[tileX, tileY] = new Bitmap(32, 16, PixelFormat.Format32bppRgba);
            //         DitherForest[tileX, tileY] = new Bitmap(32, 16, PixelFormat.Format32bppRgba);
            //         DitherHills[tileX, tileY] = new Bitmap(32, 16, PixelFormat.Format32bppRgba);
            //         DitherMountains[tileX, tileY] = new Bitmap(32, 16, PixelFormat.Format32bppRgba);
            //         DitherTundra[tileX, tileY] = new Bitmap(32, 16, PixelFormat.Format32bppRgba);
            //         DitherGlacier[tileX, tileY] = new Bitmap(32, 16, PixelFormat.Format32bppRgba);
            //         DitherSwamp[tileX, tileY] = new Bitmap(32, 16, PixelFormat.Format32bppRgba);
            //         DitherJungle[tileX, tileY] = new Bitmap(32, 16, PixelFormat.Format32bppRgba);
            //         for (int col = 0; col < 32; col++)
            //         {
            //             for (int row = 0; row < 16; row++)
            //             {
            //                 // replacementColor = DitherBlank.GetPixel(tileX * 32 + col, tileY * 16 + row);
            //                 replacementColor = DitherBlank[tileX, tileY].GetPixel(col, row);
            //                 if (replacementColor == Color.FromArgb(0, 0, 0))
            //                 {
            //                     DitherDesert[tileX, tileY].SetPixel(col, row, Desert[0].GetPixel((tileX * 32) + col, (tileY * 16) + row));
            //                     DitherPlains[tileX, tileY].SetPixel(col, row, Plains[0].GetPixel((tileX * 32) + col, (tileY * 16) + row));
            //                     DitherGrassland[tileX, tileY].SetPixel(col, row, Grassland[0].GetPixel((tileX * 32) + col, (tileY * 16) + row));
            //                     DitherForest[tileX, tileY].SetPixel(col, row, ForestBase[0].GetPixel((tileX * 32) + col, (tileY * 16) + row));
            //                     DitherHills[tileX, tileY].SetPixel(col, row, HillsBase[0].GetPixel((tileX * 32) + col, (tileY * 16) + row));
            //                     DitherMountains[tileX, tileY].SetPixel(col, row, MtnsBase[0].GetPixel((tileX * 32) + col, (tileY * 16) + row));
            //                     DitherTundra[tileX, tileY].SetPixel(col, row, Tundra[0].GetPixel((tileX * 32) + col, (tileY * 16) + row));
            //                     DitherGlacier[tileX, tileY].SetPixel(col, row, Glacier[0].GetPixel((tileX * 32) + col, (tileY * 16) + row));
            //                     DitherSwamp[tileX, tileY].SetPixel(col, row, Swamp[0].GetPixel((tileX * 32) + col, (tileY * 16) + row));
            //                     DitherJungle[tileX, tileY].SetPixel(col, row, Jungle[0].GetPixel((tileX * 32) + col, (tileY * 16) + row));
            //                 }
            //                 else
            //                 {
            //                     DitherDesert[tileX, tileY].SetPixel(col, row, Color.FromArgb(0, 0, 0, 0));
            //                     DitherPlains[tileX, tileY].SetPixel(col, row, Color.FromArgb(0, 0, 0, 0));
            //                     DitherGrassland[tileX, tileY].SetPixel(col, row, Color.FromArgb(0, 0, 0, 0));
            //                     DitherForest[tileX, tileY].SetPixel(col, row, Color.FromArgb(0, 0, 0, 0));
            //                     DitherHills[tileX, tileY].SetPixel(col, row, Color.FromArgb(0, 0, 0, 0));
            //                     DitherMountains[tileX, tileY].SetPixel(col, row, Color.FromArgb(0, 0, 0, 0));
            //                     DitherTundra[tileX, tileY].SetPixel(col, row, Color.FromArgb(0, 0, 0, 0));
            //                     DitherGlacier[tileX, tileY].SetPixel(col, row, Color.FromArgb(0, 0, 0, 0));
            //                     DitherSwamp[tileX, tileY].SetPixel(col, row, Color.FromArgb(0, 0, 0, 0));
            //                     DitherJungle[tileX, tileY].SetPixel(col, row, Color.FromArgb(0, 0, 0, 0));
            //                 }
            //             }
            //         }
            //     }
            // }
            //
            // // Rivers, Forest, Mountains, Hills
            // for (int i = 0; i < 16; i++)
            // {
            //     River[i] = overlayData.Clone(new Rectangle((i % 8) + 1 + ((i % 8) * 64), 3 + (i / 8) + ((2 + (i / 8)) * 32), 64, 32));
            //     River[i].ReplaceColors(transparentGray, Colors.Transparent);
            //     River[i].ReplaceColors(transparentPink, Colors.Transparent);
            //     Forest[i] = overlayData.Clone(new Rectangle((i % 8) + 1 + ((i % 8) * 64), 5 + (i / 8) + ((4 + (i / 8)) * 32), 64, 32));
            //     Forest[i].ReplaceColors(transparentGray, Colors.Transparent);
            //     Forest[i].ReplaceColors(transparentPink, Colors.Transparent);
            //     Mountains[i] = overlayData.Clone(new Rectangle((i % 8) + 1 + ((i % 8) * 64), 7 + (i / 8) + ((6 + (i / 8)) * 32), 64, 32));
            //     Mountains[i].ReplaceColors(transparentGray, Colors.Transparent);
            //     Mountains[i].ReplaceColors(transparentPink, Colors.Transparent);
            //     Hills[i] = overlayData.Clone(new Rectangle((i % 8) + 1 + ((i % 8) * 64), 9 + (i / 8) + ((8 + (i / 8)) * 32), 64, 32));
            //     Hills[i].ReplaceColors(transparentGray, Colors.Transparent);
            //     Hills[i].ReplaceColors(transparentPink, Colors.Transparent);
            // }
            //
            // // River mouths
            // for (int i = 0; i < 4; i++)
            // {
            //     RiverMouth[i] = overlayData.Clone(new Rectangle(i + 1 + (i * 64), (11 * 1) + (10 * 32), 64, 32));
            //     RiverMouth[i].ReplaceColors(transparentGray, Colors.Transparent);
            //     RiverMouth[i].ReplaceColors(transparentPink, Colors.Transparent);
            //     RiverMouth[i].ReplaceColors(transparentCyan, Colors.Transparent);
            // }
            //
            // // Coast
            // for (int i = 0; i < 8; i++)
            // {
            //     Coast[i, 0] = overlayData.Clone(new Rectangle((2 * i) + 1 + (2 * i * 32), 429, 32, 16));  // N
            //     Coast[i, 0].ReplaceColors(transparentGray, Colors.Transparent);
            //     Coast[i, 1] = overlayData.Clone(new Rectangle((2 * i) + 1 + (2 * i * 32), 429 + (1 * 1) + (1 * 16), 32, 16));  // S
            //     Coast[i, 1].ReplaceColors(transparentGray, Colors.Transparent);
            //     Coast[i, 2] = overlayData.Clone(new Rectangle((2 * i) + 1 + (2 * i * 32), 429 + (2 * 1) + (2 * 16), 32, 16));  // W
            //     Coast[i, 2].ReplaceColors(transparentGray, Colors.Transparent);
            //     Coast[i, 3] = overlayData.Clone(new Rectangle((2 * (i + 1)) + (((2 * i) + 1) * 32), 429 + (2 * 1) + (2 * 16), 32, 16));  // E
            //     Coast[i, 3].ReplaceColors(transparentGray, Colors.Transparent);
            // }
            //
            // // Road & railorad
            // for (int i = 0; i < 9; i++)
            // {
            //     Road[i] = tileData.Clone(new Rectangle(i + 1 + (i * 64), 364, 64, 32));
            //     Road[i].ReplaceColors(transparentGray, Colors.Transparent);
            //     Road[i].ReplaceColors(transparentPink, Colors.Transparent);
            //     Railroad[i] = tileData.Clone(new Rectangle(i + 1 + (i * 64), 397, 64, 32));
            //     Railroad[i].ReplaceColors(transparentGray, Colors.Transparent);
            //     Railroad[i].ReplaceColors(transparentPink, Colors.Transparent);
            // }
            //
            // Irrigation = tileData.Clone(new Rectangle(456, 100, 64, 32));
            // Irrigation.ReplaceColors(transparentGray, Colors.Transparent);
            // Irrigation.ReplaceColors(transparentPink, Colors.Transparent);
            //
            // Farmland = tileData.Clone(new Rectangle(456, 133, 64, 32));
            // Farmland.ReplaceColors(transparentGray, Colors.Transparent);
            // Farmland.ReplaceColors(transparentPink, Colors.Transparent);
            //
            // Mining = tileData.Clone(new Rectangle(456, 166, 64, 32));
            // Mining.ReplaceColors(transparentGray, Colors.Transparent);
            // Mining.ReplaceColors(transparentPink, Colors.Transparent);
            //
            // Pollution = tileData.Clone(new Rectangle(456, 199, 64, 32));
            // Pollution.ReplaceColors(transparentGray, Colors.Transparent);
            // Pollution.ReplaceColors(transparentPink, Colors.Transparent);
            //
            // GrasslandShield = tileData.Clone(new Rectangle(456, 232, 64, 32));
            // GrasslandShield.ReplaceColors(transparentGray, Colors.Transparent);
            // GrasslandShield.ReplaceColors(transparentPink, Colors.Transparent);
        }
    }
}