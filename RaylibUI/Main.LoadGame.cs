using System;
using System.Collections.Generic;
using RaylibUI.ImageLoader;
using Civ2engine.IO;
using Civ2engine;
using Raylib_cs;
using Civ2engine.MapObjects;
using Civ2engine.Enums;
using Civ2engine.Units;

namespace RaylibUI
{
    public partial class Main
    {
        public IPlayer CurrentPlayer { get; set; }
        private string savDirectory;
        private Unit activeUnit;
        

        private void LocateStartingFiles(string savName, Func<Ruleset, string, bool> initializer)
        {
            var ruleSet = new Ruleset
            {
                FolderPath = Settings.Civ2Path,
                Root = savDirectory = Settings.Civ2Path
            };
            if (initializer(ruleSet, savName))
            {
                //Sounds.LoadSounds(ruleSet.Paths);
                Playgame();
                return;
            }
        }

        public bool LoadGameInitialization(Ruleset ruleset, string saveFileName)
        {
            var rules = RulesParser.ParseRules(ruleset);

            GameData gameData = Read.ReadSAVFile(ruleset.FolderPath, saveFileName);
            var hydrator = new LoadedGameObjects(rules, gameData);
            map = hydrator.Map;
            activeUnit = hydrator.ActiveUnit;

            Images.LoadGraphicsAssetsFromFiles(ruleset, rules);
            return true;
        }

        public void Playgame()
        {
            //Sounds.Stop();
            //Sounds.PlaySound(GameSounds.MenuOk);

            //var playerCiv = Game.GetPlayerCiv;

            StartGame();
            //Sounds.PlaySound(GameSounds.MenuOk);
        }

        public Texture2D MapTileTextureC2(int xC2, int yC2) => Images.MapTileTexture[(((xC2 + map.XDimMax) % (map.XDimMax)) - yC2 % 2) / 2, yC2];

        public void StartGame()
        {
            //SetupGameModes(Game.Instance);

            // Generate map tile graphics
            Images.MapTileGraphic = new Image[map.XDim, map.YDim];
            Images.MapTileTexture = new Texture2D[map.XDim, map.YDim];
            for (var col = 0; col < map.XDim; col++)
            {
                for (var row = 0; row < map.YDim; row++)
                {
                    Images.MapTileGraphic[col, row] = MakeTileGraphic(map.Tile[col, row], col, row, false, MapImages.Terrains[map.MapIndex]);
                    Images.MapTileTexture[col, row] = Raylib.LoadTextureFromImage(Images.MapTileGraphic[col, row]);
                    Raylib.UnloadImage(Images.MapTileGraphic[col, row]);
                }
            }
        }

        public Image MakeTileGraphic(Tile tile, int arrayCol, int row, bool flatEarth, TerrainSet terrainSet)
        {
            // EVERYTHING HERE IS IN CIV2-COORDS AND NOT IN REGULAR COORDS!!!

            // First convert regular coords to civ2-style
            int col = 2 * arrayCol + row % 2; // you don't change row
            int Xdim = map.XDimMax;
            int Ydim = map.YDim;

            // Define base bitmap for drawing
            var _tilePic = Raylib.ImageCopy(terrainSet.BaseTiles[(int)tile.Type]);

            // Dither
            if (tile.Type != TerrainType.Ocean)
            {
                if (flatEarth)
                {
                    // Determine type of NW tile
                    if (col != 0 && row != 0) ApplyDither(_tilePic, map.TileC2(col - 1, row - 1).Type, tile.Type, terrainSet.DitherMaps[0], 0, 0);
                    // Determine type of NE tile
                    if (col != Xdim - 1 && (row != 0)) ApplyDither(_tilePic, map.TileC2(col + 1, row - 1).Type, tile.Type, terrainSet.DitherMaps[1], 32, 0);
                    // Determine type of SW tile
                    if (col != 0 && (row != Ydim - 1)) ApplyDither(_tilePic, map.TileC2(col - 1, row + 1).Type, tile.Type, terrainSet.DitherMaps[2], 0, 16);
                    // Determine type of SE tile
                    if (col != Xdim - 1 && (row != Ydim - 1)) ApplyDither(_tilePic, map.TileC2(col + 1, row + 1).Type, tile.Type, terrainSet.DitherMaps[3], 32, 16);
                }
                else // Round earth
                {
                    if (row != 0)
                    {
                        ApplyDither(_tilePic, map.TileC2((col == 0 ? Xdim : col) - 1, row - 1).Type, tile.Type, terrainSet.DitherMaps[0], 0, 0);

                        ApplyDither(_tilePic, map.TileC2(col == Xdim - 1 ? 0 : col + 1, row - 1).Type, tile.Type, terrainSet.DitherMaps[1], 32, 0);
                    }

                    if (row != Ydim - 1)
                    {
                        ApplyDither(_tilePic, map.TileC2((col == 0 ? Xdim : col) - 1, row + 1).Type, tile.Type, terrainSet.DitherMaps[2], 0, 16);

                        ApplyDither(_tilePic, map.TileC2(col == Xdim - 1 ? 0 : col + 1, row + 1).Type, tile.Type, terrainSet.DitherMaps[3], 32, 16);
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
                        if (!land[7] && !land[0] && !land[1]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[0, 0], new Rectangle(0, 0, 32, 16), new Rectangle(16, 0, 32, 16), Color.WHITE);
                        if (land[7] && !land[0] && !land[1]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[1, 0], new Rectangle(0, 0, 32, 16), new Rectangle(16, 0, 32, 16), Color.WHITE);
                        if (!land[7] && land[0] && !land[1]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[2, 0], new Rectangle(0, 0, 32, 16), new Rectangle(16, 0, 32, 16), Color.WHITE);
                        if (land[7] && land[0] && !land[1]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[3, 0], new Rectangle(0, 0, 32, 16), new Rectangle(16, 0, 32, 16), Color.WHITE);
                        if (!land[7] && !land[0] && land[1]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[4, 0], new Rectangle(0, 0, 32, 16), new Rectangle(16, 0, 32, 16), Color.WHITE);
                        if (land[7] && !land[0] && land[1]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[5, 0], new Rectangle(0, 0, 32, 16), new Rectangle(16, 0, 32, 16), Color.WHITE);
                        if (!land[7] && land[0] && land[1]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[6, 0], new Rectangle(0, 0, 32, 16), new Rectangle(16, 0, 32, 16), Color.WHITE);
                        if (land[7] && land[0] && land[1]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[7, 0], new Rectangle(0, 0, 32, 16), new Rectangle(16, 0, 32, 16), Color.WHITE);

                        // SW+S+SE tiles
                        if (!land[3] && !land[4] && !land[5]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[0, 1], new Rectangle(0, 0, 32, 16), new Rectangle(16, 16, 32, 16), Color.WHITE);
                        if (land[3] && !land[4] && !land[5]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[1, 1], new Rectangle(0, 0, 32, 16), new Rectangle(16, 16, 32, 16), Color.WHITE);
                        if (!land[3] && land[4] && !land[5]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[2, 1], new Rectangle(0, 0, 32, 16), new Rectangle(16, 16, 32, 16), Color.WHITE);
                        if (land[3] && land[4] && !land[5]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[3, 1], new Rectangle(0, 0, 32, 16), new Rectangle(16, 16, 32, 16), Color.WHITE);
                        if (!land[3] && !land[4] && land[5]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[4, 1], new Rectangle(0, 0, 32, 16), new Rectangle(16, 16, 32, 16), Color.WHITE);
                        if (land[3] && !land[4] && land[5]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[5, 1], new Rectangle(0, 0, 32, 16), new Rectangle(16, 16, 32, 16), Color.WHITE);
                        if (!land[3] && land[4] && land[5]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[6, 1], new Rectangle(0, 0, 32, 16), new Rectangle(16, 16, 32, 16), Color.WHITE);
                        if (land[3] && land[4] && land[5]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[7, 1], new Rectangle(0, 0, 32, 16), new Rectangle(16, 16, 32, 16), Color.WHITE);

                        // SW+W+NW tiles
                        if (!land[5] && !land[6] && !land[7]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[0, 2], new Rectangle(0, 0, 32, 16), new Rectangle(0, 8, 32, 16), Color.WHITE);
                        if (land[5] && !land[6] && !land[7]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[1, 2], new Rectangle(0, 0, 32, 16), new Rectangle(0, 8, 32, 16), Color.WHITE);
                        if (!land[5] && land[6] && !land[7]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[2, 2], new Rectangle(0, 0, 32, 16), new Rectangle(0, 8, 32, 16), Color.WHITE);
                        if (land[5] && land[6] && !land[7]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[3, 2], new Rectangle(0, 0, 32, 16), new Rectangle(0, 8, 32, 16), Color.WHITE);
                        if (!land[5] && !land[6] && land[7]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[4, 2], new Rectangle(0, 0, 32, 16), new Rectangle(0, 8, 32, 16), Color.WHITE);
                        if (land[5] && !land[6] && land[7]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[5, 2], new Rectangle(0, 0, 32, 16), new Rectangle(0, 8, 32, 16), Color.WHITE);
                        if (!land[5] && land[6] && land[7]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[6, 2], new Rectangle(0, 0, 32, 16), new Rectangle(0, 8, 32, 16), Color.WHITE);
                        if (land[5] && land[6] && land[7]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[7, 2], new Rectangle(0, 0, 32, 16), new Rectangle(0, 8, 32, 16), Color.WHITE);

                        // NE+E+SE tiles
                        if (!land[1] && !land[2] && !land[3]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[0, 3], new Rectangle(0, 0, 32, 16), new Rectangle(32, 8, 32, 16), Color.WHITE);
                        if (land[1] && !land[2] && !land[3]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[1, 3], new Rectangle(0, 0, 32, 16), new Rectangle(32, 8, 32, 16), Color.WHITE);
                        if (!land[1] && land[2] && !land[3]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[2, 3], new Rectangle(0, 0, 32, 16), new Rectangle(32, 8, 32, 16), Color.WHITE);
                        if (land[1] && land[2] && !land[3]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[3, 3], new Rectangle(0, 0, 32, 16), new Rectangle(32, 8, 32, 16), Color.WHITE);
                        if (!land[1] && !land[2] && land[3]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[4, 3], new Rectangle(0, 0, 32, 16), new Rectangle(32, 8, 32, 16), Color.WHITE);
                        if (land[1] && !land[2] && land[3]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[5, 3], new Rectangle(0, 0, 32, 16), new Rectangle(32, 8, 32, 16), Color.WHITE);
                        if (!land[1] && land[2] && land[3]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[6, 3], new Rectangle(0, 0, 32, 16), new Rectangle(32, 8, 32, 16), Color.WHITE);
                        if (land[1] && land[2] && land[3]) Raylib.ImageDraw(ref _tilePic, terrainSet.Coast[7, 3], new Rectangle(0, 0, 32, 16), new Rectangle(32, 8, 32, 16), Color.WHITE);

                        // River mouth
                        // If river is next to ocean, draw river mouth on this tile.
                        if (col + 1 < Xdim && row - 1 >= 0) // NE there is no edge of map
                        {
                            if (land[1] && map.TileC2(col + 1, row - 1).River) Raylib.ImageDraw(ref _tilePic, terrainSet.RiverMouth[0], new Rectangle(0, 0, 64, 32), new Rectangle(0, 0, 64, 32), Color.WHITE);
                        }

                        if (col + 1 < Xdim && row + 1 < Ydim) // SE there is no edge of map
                        {
                            if (land[3] && map.TileC2(col + 1, row + 1).River) Raylib.ImageDraw(ref _tilePic, terrainSet.RiverMouth[1], new Rectangle(0, 0, 64, 32), new Rectangle(0, 0, 64, 32), Color.WHITE);
                        }

                        if (col - 1 >= 0 && row + 1 < Ydim) // SW there is no edge of map
                        {
                            if (land[5] && map.TileC2(col - 1, row + 1).River) Raylib.ImageDraw(ref _tilePic, terrainSet.RiverMouth[2], new Rectangle(0, 0, 64, 32), new Rectangle(0, 0, 64, 32), Color.WHITE);
                        }

                        if (col - 1 >= 0 && row - 1 >= 0) // NW there is no edge of map
                        {
                            if (land[7] && map.TileC2(col - 1, row - 1).River) Raylib.ImageDraw(ref _tilePic, terrainSet.RiverMouth[3], new Rectangle(0, 0, 64, 32), new Rectangle(0, 0, 64, 32), Color.WHITE);
                        }

                        break;
                    }
                // Draw forests
                case TerrainType.Forest:
                    {
                        var forestIndex = IsTerrainAroundIn4directions(col, row, TerrainType.Forest, flatEarth);
                        Raylib.ImageDraw(ref _tilePic, terrainSet.Forest[forestIndex], new Rectangle(0, 0, 64, 32), new Rectangle(0, 0, 64, 32), Color.WHITE);
                        break;
                    }
                // Draw mountains
                case TerrainType.Mountains:
                    {
                        var mountainsIndex = IsTerrainAroundIn4directions(col, row, TerrainType.Mountains, flatEarth);
                        Raylib.ImageDraw(ref _tilePic, terrainSet.Mountains[mountainsIndex], new Rectangle(0, 0, 64, 32), new Rectangle(0, 0, 64, 32), Color.WHITE);
                        break;
                    }
                // Draw hills
                case TerrainType.Hills:
                    {
                        var hillIndex = IsTerrainAroundIn4directions(col, row, TerrainType.Hills, flatEarth);
                        Raylib.ImageDraw(ref _tilePic, terrainSet.Hills[hillIndex], new Rectangle(0, 0, 64, 32), new Rectangle(0, 0, 64, 32), Color.WHITE);
                        break;
                    }
            }

            // Draw rivers
            if (tile.River)
            {
                var riverIndex = IsRiverAround(col, row, flatEarth);
                Raylib.ImageDraw(ref _tilePic, terrainSet.River[riverIndex], new Rectangle(0, 0, 64, 32), new Rectangle(0, 0, 64, 32), Color.WHITE);
            }

            // Draw shield for grasslands
            if (tile.Type == TerrainType.Grassland)
            {
                if (tile.HasShield)
                {
                    Raylib.ImageDraw(ref _tilePic, terrainSet.GrasslandShield, new Rectangle(0, 0, 64, 32), new Rectangle(0, 0, 64, 32), Color.WHITE);
                }
            }
            else if (tile.Special != -1)
            {
                // Draw special resources if they exist
                Raylib.ImageDraw(ref _tilePic, terrainSet.Specials[tile.Special][(int)tile.Type], new Rectangle(0, 0, 64, 32), new Rectangle(0, 0, 64, 32), Color.WHITE); 
            }


            //        var improvements = tile.Improvements
            //            .Where(ci => Game.TerrainImprovements.ContainsKey(ci.Improvement))
            //            .OrderBy(ci => Game.TerrainImprovements[ci.Improvement].Layer).ToList();

            //        foreach (var construct in improvements)
            //        {
            //            var improvement = Game.TerrainImprovements[construct.Improvement];
            //            var graphics = terrainSet.ImprovementsMap[construct.Improvement];

            //            if (improvement.HasMultiTile)
            //            {
            //                bool hasNeighbours = false;

            //                foreach (var neighbour in tile.Map.Neighbours(tile))
            //                {
            //                    var neighboringImprovement =
            //                        neighbour.Improvements.FirstOrDefault(i =>
            //                            i.Improvement == construct.Improvement);
            //                    if (neighboringImprovement != null)
            //                    {
            //                        var index = GetCoordsFromDifference(neighbour.X - tile.X, neighbour.Y - tile.Y);
            //                        if (index != -1)
            //                        {
            //                            if (neighboringImprovement.Level < construct.Level)
            //                            {
            //                                g.DrawImage(graphics.Levels[neighboringImprovement.Level, index], 0, 0);
            //                            }
            //                            else
            //                            {
            //                                hasNeighbours = true;
            //                                g.DrawImage(graphics.Levels[construct.Level, index], 0, 0);
            //                            }
            //                        }
            //                    }
            //                }

            //                if (!hasNeighbours)
            //                {
            //                    if (tile.CityHere is null)
            //                    {
            //                        g.DrawImage(graphics.Levels[construct.Level, 0], 0, 0);
            //                    }
            //                }
            //            }
            //            else if (tile.CityHere is not null)
            //            {
            //                if (tile.Map.DirectNeighbours(tile).Any(t => t.Improvements.Any(i => i.Improvement == construct.Improvement)))
            //                {
            //                    g.DrawImage(graphics.Levels[construct.Level, 0], 0, 0);
            //                }
            //            }
            //            else
            //            {
            //                if (tile.IsUnitPresent && graphics.UnitLevels != null)
            //                {
            //                    g.DrawImage(graphics.UnitLevels[construct.Level, 0], 0, 0);
            //                }
            //                else
            //                {
            //                    g.DrawImage(graphics.Levels[construct.Level, 0], 0, 0);
            //                }
            //            }
            //        }

            return _tilePic;
        }

        private void ApplyDither(Image orig_img, TerrainType neighbourType, TerrainType tileType, IReadOnlyList<Image> ditherMap, int offsetX, int offsetY)
        {
            if (neighbourType == TerrainType.Ocean)
            {
                neighbourType = TerrainType.Grassland;
            }
            if (neighbourType == tileType) return;
            Raylib.ImageDraw(ref orig_img, ditherMap[(int)neighbourType], new Rectangle(0, 0, 32, 16), new Rectangle(offsetX, offsetY, 32, 16), Color.WHITE);
        }

        private bool[] IsLandAround(int col, int row, bool flatEarth)
        {
            // In start we presume all surrounding tiles are water (land=true, water=false). Index=0 is North, follows in clockwise direction.
            bool[] land = new bool[8] { false, false, false, false, false, false, false, false };
            var indicies = new int[] { 0, 0, 0, 0 };

            int Xdim = map.XDimMax;    // X=50 in markted as X=100 in Civ2
            int Ydim = map.YDim;        // no need for such correction for Y

            // Observe in all directions if land is present next to ocean
            // N:
            if (row - 2 < 0)
            {
                land[0] = true;   // if N tile is out of map limits, we presume it is land
                indicies[0] += 2;
            }
            else if (map.TileC2(col, row - 2).Type != TerrainType.Ocean)
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
                else if (map.TileC2(0, row - 1).Type != TerrainType.Ocean)
                {
                    land[1] = true;  // tile on mirror side of map is not ocean
                }
            }
            else if (map.TileC2(col + 1, row - 1).Type != TerrainType.Ocean)
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
                else if (map.TileC2(Xdim - col, row).Type != TerrainType.Ocean)
                {
                    land[2] = true;
                }
            }
            else if (map.TileC2(col + 2, row).Type != TerrainType.Ocean)
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
                else if (map.TileC2(0, row + 1).Type != TerrainType.Ocean)
                {
                    land[3] = true;  // tile on mirror side of map is not ocean
                }
            }
            else if (map.TileC2(col + 1, row + 1).Type != TerrainType.Ocean)
            {
                land[3] = true;
            }
            // S:
            if (row + 2 >= Ydim)
            {
                land[4] = true;   // S is beyond map limits
            }
            else if (map.TileC2(col, row + 2).Type != TerrainType.Ocean)
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
                else if (map.TileC2(Xdim - 1, row + 1).Type != TerrainType.Ocean)
                {
                    land[5] = true;
                }
            }
            else if (map.TileC2(col - 1, row + 1).Type != TerrainType.Ocean)
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
                else if (map.TileC2(Xdim - 2 + col, row).Type != TerrainType.Ocean)
                {
                    land[6] = true;
                }
            }
            else if (map.TileC2(col - 2, row).Type != TerrainType.Ocean)
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
                else if (map.TileC2(Xdim - 1, row - 1).Type != TerrainType.Ocean)
                {
                    land[7] = true;
                }
            }
            else if (map.TileC2(col - 1, row - 1).Type != TerrainType.Ocean)
            {
                land[7] = true;
            }

            return land;
        }

        // Check whether a certain type of terrain (mount./hills/forest) is present in 4 directions arount the tile
        private int IsTerrainAroundIn4directions(int col, int row, TerrainType terrain, bool flatEarth)
        {
            // In start we presume all surrounding tiles are not of same type (=false). Index=0 is NE, follows in clockwise direction.
            var index = 0;

            // Rewrite indexes in Civ2-style
            int Xdim = map.XDimMax;    // X=50 in markted as X=100 in Civ2
            int Ydim = map.YDim;        // no need for such correction for Y

            // Observe in all directions if terrain is present
            // NE:
            if (row != 0)
            {
                if (col == Xdim - 1) // you are on eastern edge of map
                {
                    if (!flatEarth && map.TileC2(0, row - 1).Type == terrain)
                    {
                        index = 1; // tile on mirror side of map
                    }
                }
                else if (map.TileC2(col + 1, row - 1).Type == terrain)
                {
                    index = 1;
                }

                if (col == 0) // you are on western edge of map
                {
                    if (!flatEarth && map.TileC2(Xdim - 1, row - 1).Type == terrain)
                    {
                        index += 8;
                    }
                }
                else if (map.TileC2(col - 1, row - 1).Type == terrain)
                {
                    index += 8;
                }
            }

            if (row != Ydim - 1)
            {
                // SE:
                if (col == Xdim - 1) // you are on eastern edge of map
                {
                    if (!flatEarth && map.TileC2(0, row + 1).Type == terrain)
                    {
                        index += 2; // tile on mirror side of map
                    }
                }
                else if (map.TileC2(col + 1, row + 1).Type == terrain)
                {
                    index += 2;
                }

                // SW:
                if (col == 0) // you are on western edge of map
                {
                    if (!flatEarth && map.TileC2(Xdim - 1, row + 1).Type == terrain)
                    {
                        index += 4;
                    }
                }
                else if (map.TileC2(col - 1, row + 1).Type == terrain)
                {
                    index += 4;
                }
            }

            return index;
        }

        private int IsRiverAround(int col, int row, bool flatEarth)
        {
            // In start we presume all surrounding tiles are not rivers (river=true, no river=false). Index=0 is NE, follows in clockwise direction.
            var river = 0;

            // Rewrite indexes in Civ2-style
            var xDim = map.XDimMax; // X=50 in marked as X=100 in Civ2
            var yDim = map.YDim; // no need for such correction for Y

            // Observe in all directions if river is present
            if (row != 0)
            {
                // NE:
                if (col == xDim - 1) // you are on eastern edge of map
                {
                    if (!flatEarth && (map.TileC2(0, row - 1).River || map.TileC2(0, row - 1).Type == TerrainType.Ocean))
                    {
                        river = 1; // tile on mirror side of map
                    }
                }
                else if (map.TileC2(col + 1, row - 1).River || map.TileC2(col + 1, row - 1).Type == TerrainType.Ocean)
                {
                    river = 1;
                }


                // NW:
                if (col == 0) // you are on western edge of map
                {
                    if (!flatEarth && (map.TileC2(xDim - 1, row - 1).River || map.TileC2(xDim - 1, row - 1).Type == TerrainType.Ocean))
                    {
                        river += 8;
                    }
                }
                else if (map.TileC2(col - 1, row - 1).River || map.TileC2(col - 1, row - 1).Type == TerrainType.Ocean)
                {
                    river += 8;
                }
            }

            // SE:
            if (row != yDim - 1)
            {
                if (col == xDim - 1) // you are on eastern edge of map
                {
                    if (!flatEarth && (map.TileC2(0, row + 1).River || map.TileC2(0, row + 1).Type == TerrainType.Ocean))
                    {
                        // tile on mirror side of map
                        river += 2;
                    }
                }
                else if (map.TileC2(col + 1, row + 1).River || map.TileC2(col + 1, row + 1).Type == TerrainType.Ocean)
                {
                    river += 2;
                }

                if (col == 0) // you are on western edge of map
                {
                    if (!flatEarth && (map.TileC2(xDim - 1, row + 1).River ||
                             map.TileC2(xDim - 1, row + 1).Type == TerrainType.Ocean))
                    {
                        river += 4;
                    }
                }
                else if (map.TileC2(col - 1, row + 1).River || map.TileC2(col - 1, row + 1).Type == TerrainType.Ocean)
                {
                    river += 4;
                }
            }
            return river;
        }
    }
}
