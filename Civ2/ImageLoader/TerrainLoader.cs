using Civ2engine;
using Civ2engine.Terrains;
using Model.ImageSets;
using Raylib_cs;
using RaylibUI;
using RayLibUtils;

namespace Civ2.ImageLoader
{
    public static class TerrainLoader
    {
        public static void LoadTerrain(Ruleset ruleset, Civ2Interface active)
        {
            for (var i = 0; i < active.ExpectedMaps; i++)
            {
                active.TileSets.Add(LoadTerrain(ruleset, i, active));
            }
        }

        private static TerrainSet LoadTerrain(Ruleset ruleset, int index, Civ2Interface active)
        {
            var path = Utils.GetFilePath($"Terrain{(index * 2) + 1}", ruleset.Paths, "gif", "bmp");
            Images.LoadPropertiesFromPIC(path, active.TilePICprops);
            path = Utils.GetFilePath($"Terrain{(index * 2) + 2}", ruleset.Paths, "gif", "bmp");
            Images.LoadPropertiesFromPIC(path, active.OverlayPICprops);

            // Initialize objects
            var terrain = new TerrainSet(64, 32);

            var tileProps = active.TilePICprops;
            var overlayProps = active.OverlayPICprops;

            // Get dither tile before making it transparent
            var ditherTile = tileProps["dither"][0].Image;
            Color gray;
            unsafe
            {
                // Get the gray colour (it's not always the same in MGE/TOT, unlike pink)
                var imageColours = Raylib.LoadImageColors(ditherTile);
                gray = imageColours[0];
                Raylib.UnloadImageColors(imageColours);
            }
            Raylib.ImageColorReplace(ref ditherTile, Color.BLACK, Color.WHITE);
            Raylib.ImageColorReplace(ref ditherTile, new Color(255, 0, 255, 0), Color.BLACK);
            Raylib.ImageColorReplace(ref ditherTile, gray, Color.BLACK);

            terrain.BaseTiles = tileProps["base1"].Select(t => t.Image).ToArray();

            terrain.Specials = new[]
            {
                tileProps["special1"].Select(s => s.Image).ToArray(),
                tileProps["special2"].Select(s => s.Image).ToArray(),
            };

            terrain.Blank = tileProps["blank"][0].Image;

            // 4 small dither tiles (base mask must be B/W)
            terrain.DitherMask = new[]
            {
                Raylib.ImageFromImage(ditherTile, new Rectangle(32, 0, 32, 16)),
                Raylib.ImageFromImage(ditherTile, new Rectangle(32, 16, 32, 16)),
                Raylib.ImageFromImage(ditherTile, new Rectangle(0, 16, 32, 16)),
                Raylib.ImageFromImage(ditherTile, new Rectangle(0, 0, 32, 16)),
            };

            terrain.DitherMaps = new[]
            {
                BuildDitherMaps(terrain.DitherMask[0], terrain.BaseTiles, 32, 0, terrain.Blank),
                BuildDitherMaps(terrain.DitherMask[1], terrain.BaseTiles, 32, 16, terrain.Blank),
                BuildDitherMaps(terrain.DitherMask[2], terrain.BaseTiles, 0, 16, terrain.Blank),
                BuildDitherMaps(terrain.DitherMask[3], terrain.BaseTiles, 0, 0, terrain.Blank),
            };

            terrain.River = overlayProps["river"].Select(r => r.Image).ToArray();
            terrain.Forest = overlayProps["forest"].Select(r => r.Image).ToArray();
            terrain.Mountains = overlayProps["mountain"].Select(r => r.Image).ToArray();
            terrain.Hills = overlayProps["hill"].Select(r => r.Image).ToArray();
            terrain.RiverMouth = overlayProps["riverMouth"].Select(r => r.Image).ToArray();

            terrain.Coast = new Image[8, 4];
            for (var i = 0; i < 8; i++)
            {
                terrain.Coast[i, 0] = overlayProps["coastline"][4 * i + 0].Image; // N
                terrain.Coast[i, 1] = overlayProps["coastline"][4 * i + 1].Image; // S
                terrain.Coast[i, 2] = overlayProps["coastline"][4 * i + 2].Image; // W
                terrain.Coast[i, 3] = overlayProps["coastline"][4 * i + 3].Image; // E
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
                roadGraphics.Levels[0, i] = tileProps["road"][i].Image;
                roadGraphics.Levels[1, i] = tileProps["railroad"][i].Image;
            }

            terrain.ImprovementsMap.Add(ImprovementTypes.Irrigation, new ImprovementGraphic
            {
                Levels = new[,]
                {
                    { tileProps["irrigation"][0].Image },
                    { tileProps["farmland"][0].Image }
                }
            });

            terrain.ImprovementsMap[ImprovementTypes.Mining] = new ImprovementGraphic
                { Levels = new[,] { { tileProps["mine"][0].Image } } };

            terrain.ImprovementsMap[ImprovementTypes.Pollution] = new ImprovementGraphic
                { Levels = new[,] { { tileProps["pollution"][0].Image } } };

            //Note airbase and fortress are now loaded directly by the cities loader
            terrain.GrasslandShield = tileProps["shield"][0].Image;

            return terrain;
        }

        private static DitherMap BuildDitherMaps(Image mask, Image[] baseTiles, int offsetX, int offsetY,
            Image terrainBlank)
        {
            var sampleRect = new Rectangle(offsetX, offsetY, 32, 16);
            var totalTiles = baseTiles.Length + 1;
            var ditherMaps = new Image[totalTiles];
            for (var i = 0; i < baseTiles.Length; i++)
            {
                ditherMaps[i] = Raylib.ImageFromImage(baseTiles[i], sampleRect);
                Raylib.ImageAlphaMask(ref ditherMaps[i], mask);
            }

            ditherMaps[^1] = Raylib.ImageFromImage(terrainBlank, sampleRect);
            Raylib.ImageAlphaMask(ref ditherMaps[^1], mask);

            return new DitherMap { X = offsetX, Y = offsetY, Images = ditherMaps };
        }
    }
}   