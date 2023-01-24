using System.Collections.Generic;
using System.IO;
using System.Linq;
using Civ2engine;
using Civ2engine.Terrains;
using Raylib_cs;

namespace RaylibUI.ImageLoader
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
            int _frames = 0;
            var tileData = Raylib.LoadImageAnim(ruleset.Paths[0] + Path.DirectorySeparatorChar + $"Terrain{((index * 2) + 1)}.gif", out _frames);
            var overlayData = Raylib.LoadImageAnim(ruleset.Paths[0] + Path.DirectorySeparatorChar + $"Terrain{((index * 2) + 2)}.gif", out _frames);

            // Initialize objects
            var terrain = new TerrainSet();

            // Get dither tile before making it transparent
            var ditherTile = Raylib.ImageFromImage(tileData, new Rectangle(1, 447, 64, 32));
            Raylib.ImageColorReplace(ref ditherTile, Color.BLACK, Color.YELLOW);
            Raylib.ImageColorReplace(ref ditherTile, new Color(255, 0, 255, 255), Color.BLACK);
            Raylib.ImageColorReplace(ref ditherTile, new Color(135, 135, 135, 255), Color.BLACK);
            Raylib.ImageColorReplace(ref ditherTile, Color.YELLOW, Color.WHITE);

            // Transparent
            Raylib.ImageColorReplace(ref tileData, new Color(135, 135, 135, 255), new Color(135, 135, 135, 0));
            Raylib.ImageColorReplace(ref tileData, new Color(255, 0, 255, 255), new Color(255, 0, 255, 0));
            Raylib.ImageColorReplace(ref overlayData, new Color(135, 135, 135, 255), new Color(135, 135, 135, 0));
            Raylib.ImageColorReplace(ref overlayData, new Color(255, 0, 255, 255), new Color(255, 0, 255, 0));

            terrain.BaseTiles = Enumerable.Range(0, 11)
                .Select(i => Raylib.ImageFromImage(tileData, new Rectangle(1, 1 + (33 * i), 64, 32))).ToArray();


            var secondSpecial = 196;
            var firstSpecial = secondSpecial - 65;

            terrain.Specials = new[]
            {
                Enumerable.Range(0, 11)
                    .Select(i => Raylib.ImageFromImage(tileData, new Rectangle(firstSpecial, 1 + (33 * i), 64, 32))).ToArray(),
                Enumerable.Range(0, 11)
                    .Select(i => Raylib.ImageFromImage(tileData, new Rectangle(secondSpecial, 1 + (33 * i), 64, 32))).ToArray()
            };


            // Blank tile
            terrain.Blank = Raylib.ImageFromImage(tileData, new Rectangle(131, 447, 64, 32));

            // 4 small dither tiles (base mask must be B/W)
            var ditherMask = new[]
            {
                Raylib.ImageFromImage(ditherTile, new Rectangle(0, 0, 32, 16)),
                Raylib.ImageFromImage(ditherTile, new Rectangle(32, 0, 32, 16)),
                Raylib.ImageFromImage(ditherTile, new Rectangle(0, 16, 32, 16)),
                Raylib.ImageFromImage(ditherTile, new Rectangle(32, 16, 32, 16))
            };
            terrain.DitherMask = ditherMask;

            terrain.DitherMaps = new[]
            {
                BuildDitherMaps(ditherMask[0], terrain.BaseTiles, 0, 0),
                BuildDitherMaps(ditherMask[1], terrain.BaseTiles, 32, 0),
                BuildDitherMaps(ditherMask[2], terrain.BaseTiles, 0, 16),
                BuildDitherMaps(ditherMask[3], terrain.BaseTiles, 32, 16)
            };

            // Rivers, Forest, Mountains, Hills
            terrain.River = new Image[16];
            terrain.Forest = new Image[16];
            terrain.Mountains = new Image[16];
            terrain.Hills = new Image[16];
            for (var i = 0; i < 16; i++)
            {
                terrain.River[i] = Raylib.ImageFromImage(overlayData, new Rectangle((i % 8) + 1 + ((i % 8) * 64),
                    3 + (i / 8) + ((2 + (i / 8)) * 32), 64, 32));
                terrain.Forest[i] = Raylib.ImageFromImage(overlayData, new Rectangle((i % 8) + 1 + ((i % 8) * 64),
                    5 + (i / 8) + ((4 + (i / 8)) * 32), 64, 32));
                terrain.Mountains[i] = Raylib.ImageFromImage(overlayData, new Rectangle((i % 8) + 1 + ((i % 8) * 64),
                    7 + (i / 8) + ((6 + (i / 8)) * 32), 64, 32));
                terrain.Hills[i] = Raylib.ImageFromImage(overlayData, new Rectangle((i % 8) + 1 + ((i % 8) * 64),
                    9 + (i / 8) + ((8 + (i / 8)) * 32), 64, 32));
            }

            // River mouths
            terrain.RiverMouth = new Image[4];
            for (var i = 0; i < 4; i++)
            {
                terrain.RiverMouth[i] =
                    Raylib.ImageFromImage(overlayData, new Rectangle(i + 1 + (i * 64), (11 * 1) + (10 * 32), 64, 32));
            }

            // Coast
            terrain.Coast = new Image[8, 4];
            for (var i = 0; i < 8; i++)
            {
                terrain.Coast[i, 0] = Raylib.ImageFromImage(overlayData, new Rectangle((2 * i) + 1 + (2 * i * 32), 429, 32, 16)); // N
                terrain.Coast[i, 1] =
                    Raylib.ImageFromImage(overlayData, new Rectangle((2 * i) + 1 + (2 * i * 32), 429 + (1 * 1) + (1 * 16), 32, 16)); // S
                terrain.Coast[i, 2] =
                    Raylib.ImageFromImage(overlayData, new Rectangle((2 * i) + 1 + (2 * i * 32), 429 + (2 * 1) + (2 * 16), 32, 16)); // W
                terrain.Coast[i, 3] = Raylib.ImageFromImage(overlayData, new Rectangle((2 * (i + 1)) + (((2 * i) + 1) * 32),
                    429 + (2 * 1) + (2 * 16), 32, 16)); // E
            }

            // Road & railroad
            terrain.ImprovementsMap = new Dictionary<int, ImprovementGraphic>();

            var roadGraphics = new ImprovementGraphic
            {
                Levels = new Image[2, 9]
            };


            terrain.ImprovementsMap.Add(ImprovementTypes.Road, roadGraphics);

            for (var i = 0; i < 9; i++)
            {
                roadGraphics.Levels[0, i] = Raylib.ImageFromImage(tileData, new Rectangle(i + 1 + (i * 64), 364, 64, 32));
                roadGraphics.Levels[1, i] = Raylib.ImageFromImage(tileData, new Rectangle(i + 1 + (i * 64), 397, 64, 32));
            }

            terrain.ImprovementsMap.Add(ImprovementTypes.Irrigation, new ImprovementGraphic
            {
                Levels = new[,]
                {
                    { Raylib.ImageFromImage(tileData, new Rectangle(456, 100, 64, 32)) },
                    { Raylib.ImageFromImage(tileData, new Rectangle(456, 133, 64, 32)) }
                }
            });

            terrain.ImprovementsMap[ImprovementTypes.Mining] = new ImprovementGraphic
            { Levels = new[,] { { Raylib.ImageFromImage(tileData, new Rectangle(456, 166, 64, 32)) } } };

            terrain.ImprovementsMap[ImprovementTypes.Pollution] = new ImprovementGraphic
            { Levels = new[,] { { Raylib.ImageFromImage(tileData, new Rectangle(456, 199, 64, 32)) } } };

            //terrain.ImprovementsMap[ImprovementTypes.Fortress] = new ImprovementGraphic
            //{ Levels = new[,] { { MapImages.Specials[1] } } };

            //// Airbase
            //terrain.ImprovementsMap[ImprovementTypes.Airbase] = new ImprovementGraphic
            //{
            //    Levels = new[,] { { MapImages.Specials[2] } },
            //    UnitLevels = new[,] { { MapImages.Specials[3] } }
            //};


            terrain.GrasslandShield = Raylib.ImageFromImage(tileData, new Rectangle(456, 232, 64, 32));

            return terrain;
        }

        private unsafe static Image[] BuildDitherMaps(Image mask, Image[] baseTiles, int offsetX, int offsetY)
        {
            var ditherMaps = new Image[10];
            for (var i = 0; i < 10; i++)
            {
                ditherMaps[i] = Raylib.ImageFromImage(baseTiles[i], new Rectangle(offsetX, offsetY, 32, 16));
                Raylib.ImageAlphaMask(ref ditherMaps[i], mask);
            }
            
            return ditherMaps;
        }


        //private unsafe static Image[] BuildDitherMaps(Image mask, IReadOnlyList<Image> baseTiles, Image blank, int offsetX,
        //    int offsetY)
        //{
        //    var ditherMaps = Enumerable.Range(0, 10).Select((_) => new Image())
        //        .ToArray();
        
        //    for (var col = 0; col < 32; col++)
        //    {
        //        for (var row = 0; row < 16; row++)
        //        {
        //            if (!Raylib.GetImageColor(mask, col, row).Equals(Color.BLACK)) continue;
        //            // Only need to set the non transparent pixels since the default is transparent
        //            for (var i = 0; i < ditherMaps.Length; i++)
        //            {
        //                ditherMaps[i].SetPixel(col, row, baseTiles[i].GetPixel(offsetX + col, offsetY + row));
        //            }
        //        }
        //    }

        //    return ditherMaps;
        //}
    }

    public class ImprovementGraphic
    {
        public Image[,] Levels { get; set; }
        public Image[,] UnitLevels { get; set; }
    }
}   