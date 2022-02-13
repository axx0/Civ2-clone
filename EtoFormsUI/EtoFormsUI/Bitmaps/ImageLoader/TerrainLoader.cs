using System.Collections.Generic;
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


        private static Color _black = Color.FromArgb(0, 0, 0);

        private static TerrainSet LoadTerrain(Ruleset ruleset, int index)
        {
            using var tileData = Common.LoadBitmapFrom($"Terrain{((index * 2) + 1)}", ruleset.Paths);
            using var overlayData = Common.LoadBitmapFrom($"Terrain{((index * 2) + 2)}", ruleset.Paths);

            // Initialize objects
            var terrain = new TerrainSet();

            // Define transparent colors
            var borderColour = tileData.GetPixel(0, 0);
            var transparentGray = Color.FromArgb(135, 135, 135); // Define transparent back color (gray)
            var transparentPink = Color.FromArgb(255, 0, 255); // Define transparent back color (pink)
            var transparentCyan = Color.FromArgb(0, 255, 255); // Define transparent back color (cyan)

            tileData.SetTransparent(new Color[] { transparentGray, transparentPink });
            overlayData.SetTransparent(new Color[] { transparentGray, transparentPink });

            terrain.BaseTiles = Enumerable.Range(0, 11)
                .Select(i => tileData.Clone(new Rectangle(1, 1 + (33 * i), 64, 32))).ToArray();


            var secondSpecial = tileData.GetPixel(262, 0) == borderColour ? 261 : 196;
            var firstSpecial = secondSpecial - 65;

            terrain.Specials = new[]
            {
                Enumerable.Range(0, 11)
                    .Select(i => tileData.Clone(new Rectangle(firstSpecial, 1 + (33 * i), 64, 32))).ToArray(),
                Enumerable.Range(0, 11)
                    .Select(i => tileData.Clone(new Rectangle(secondSpecial, 1 + (33 * i), 64, 32))).ToArray()
            };


            // Blank tile
            terrain.Blank = tileData.Clone(new Rectangle(131, 447, 64, 32));

            // 4 small dither tiles
            var ditherMask = new[]
            {
                tileData.Clone(new Rectangle(1, 447, 32, 16)),
                tileData.Clone(new Rectangle(33, 447, 32, 16)),
                tileData.Clone(new Rectangle(1, 463, 32, 16)),
                tileData.Clone(new Rectangle(33, 463, 32, 16))
            };
            terrain.DitherMask = ditherMask;

            terrain.DitherMaps = new[]
            {
                BuildDitherMaps(ditherMask[0], terrain.BaseTiles, terrain.Blank, 0, 0),
                BuildDitherMaps(ditherMask[1], terrain.BaseTiles, terrain.Blank, 32, 0),
                BuildDitherMaps(ditherMask[2], terrain.BaseTiles, terrain.Blank, 0, 16),
                BuildDitherMaps(ditherMask[3], terrain.BaseTiles, terrain.Blank, 32, 16)
            };

            // Rivers, Forest, Mountains, Hills
            terrain.River = new Bitmap[16];
            terrain.Forest = new Bitmap[16];
            terrain.Mountains = new Bitmap[16];
            terrain.Hills = new Bitmap[16];
            for (var i = 0; i < 16; i++)
            {
                terrain.River[i] = overlayData.Clone(new Rectangle((i % 8) + 1 + ((i % 8) * 64),
                    3 + (i / 8) + ((2 + (i / 8)) * 32), 64, 32));
                terrain.Forest[i] = overlayData.Clone(new Rectangle((i % 8) + 1 + ((i % 8) * 64),
                    5 + (i / 8) + ((4 + (i / 8)) * 32), 64, 32));
                terrain.Mountains[i] = overlayData.Clone(new Rectangle((i % 8) + 1 + ((i % 8) * 64),
                    7 + (i / 8) + ((6 + (i / 8)) * 32), 64, 32));
                terrain.Hills[i] = overlayData.Clone(new Rectangle((i % 8) + 1 + ((i % 8) * 64),
                    9 + (i / 8) + ((8 + (i / 8)) * 32), 64, 32));
            }

            // River mouths

            terrain.RiverMouth = new Bitmap[4];
            for (var i = 0; i < 4; i++)
            {
                terrain.RiverMouth[i] =
                    overlayData.Clone(new Rectangle(i + 1 + (i * 64), (11 * 1) + (10 * 32), 64, 32));
            }

            // Coast
            terrain.Coast = new Bitmap[8, 4];
            for (var i = 0; i < 8; i++)
            {
                terrain.Coast[i, 0] = overlayData.Clone(new Rectangle((2 * i) + 1 + (2 * i * 32), 429, 32, 16)); // N
                terrain.Coast[i, 1] =
                    overlayData.Clone(new Rectangle((2 * i) + 1 + (2 * i * 32), 429 + (1 * 1) + (1 * 16), 32, 16)); // S
                terrain.Coast[i, 2] =
                    overlayData.Clone(new Rectangle((2 * i) + 1 + (2 * i * 32), 429 + (2 * 1) + (2 * 16), 32, 16)); // W
                terrain.Coast[i, 3] = overlayData.Clone(new Rectangle((2 * (i + 1)) + (((2 * i) + 1) * 32),
                    429 + (2 * 1) + (2 * 16), 32, 16)); // E
            }

            // Road & railroad
            terrain.Road = new Bitmap[9];
            terrain.Railroad = new Bitmap[9];
            for (var i = 0; i < 9; i++)
            {
                terrain.Road[i] = tileData.Clone(new Rectangle(i + 1 + (i * 64), 364, 64, 32));
                terrain.Railroad[i] = tileData.Clone(new Rectangle(i + 1 + (i * 64), 397, 64, 32));
            }

            terrain.Irrigation = tileData.Clone(new Rectangle(456, 100, 64, 32));
            terrain.Farmland = tileData.Clone(new Rectangle(456, 133, 64, 32));
            terrain.Mining = tileData.Clone(new Rectangle(456, 166, 64, 32));
            terrain.Pollution = tileData.Clone(new Rectangle(456, 199, 64, 32));
            terrain.GrasslandShield = tileData.Clone(new Rectangle(456, 232, 64, 32));

            return terrain;
        }

        private static Bitmap[] BuildDitherMaps(Bitmap mask, IReadOnlyList<Bitmap> baseTiles, Bitmap blank, int offsetX,
            int offsetY)
        {
            var ditherMaps = Enumerable.Range(0, 10).Select((_) => new Bitmap(32, 16, PixelFormat.Format32bppRgba))
                .ToArray();
            for (var col = 0; col < 32; col++)
            {
                for (var row = 0; row < 16; row++)
                {
                    if (mask.GetPixel(col, row) != _black) continue;
                    // Only need to set the non transparent pixels since the default is transparent
                    for (var i = 0; i < ditherMaps.Length; i++)
                    {
                        ditherMaps[i].SetPixel(col, row, baseTiles[i].GetPixel(offsetX + col, offsetY + row));
                    }
                }
            }

            return ditherMaps;
        }
    }
}