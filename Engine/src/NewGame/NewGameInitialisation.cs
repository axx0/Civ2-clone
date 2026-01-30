using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Model.Core.Units;

namespace Civ2engine.NewGame
{
    public static class NewGameInitialisation
    {
        public static Game StartNewGame(GameInitializationConfig config, Map[] maps, IList<Civilization> civilizations,
            string[] paths)
        {
            var settlerType = config.Rules.UnitTypes[(int) UnitType.Settlers];

            var units = civilizations.Skip(1).Select(c => new
                {
                    Civ = c, DefaultStart = config.StartPositions != null ? GetDefaultStart(config, c, maps[0]) : null
                })
                .OrderBy(c => c.DefaultStart != null)
                .Select(c => new
                {
                    c.Civ, StartLocation = c.DefaultStart ?? GetStartLoc(c.Civ, config, maps[0])
                }).Select((c, id) => new Unit
                {
                    Counter = 0,
                    Dead = false,
                    Id = id,
                    Order = (int)OrderType.NoOrders,
                    Owner = c.Civ,
                    Veteran = false,
                    X = c.StartLocation.X,
                    Y = c.StartLocation.Y,
                    CurrentLocation = c.StartLocation,
                    TypeDefinition = settlerType
                }).ToList();
            units.ForEach(u =>
            {
                u.Owner.Units.Add(u);
            });

            maps[0].WhichCivsMapShown = config.PlayerCiv.Id;

            return new Game(maps, config.Rules, civilizations, new Options(config), paths, config.DifficultyLevel, config.BarbarianActivity);
        }

        private static Tile? GetDefaultStart(GameInitializationConfig config, Civilization civilization, Map map)
        {
            var index = Array.FindIndex(config.Rules.Leaders, l => l.Adjective == civilization.Adjective);
            if (index <= -1 || index >= config.StartPositions!.Length) return null;
            
            var pos = config.StartPositions[index];
            if (pos[0] == -1 || pos[1] == -1) return null;
            
            var tile = map.TileC2(pos[0], pos[1]);
            if (config.StartTiles.Contains(tile) || tile.IsUnitPresent ||
                tile.Neighbours().Any(n => config.StartTiles.Contains(n) || n.IsUnitPresent))
            {
                return null;
            }

            if (tile.Fertility > -1)
            {
                map.SetAsStartingLocation(tile, civilization.Id);
                config.StartTiles.Add(tile);
                return tile;
            }

            return null;
        }


        private static Tile GetStartLoc(Civilization civilization, GameInitializationConfig config, Map map)
        {
            var startTiles = config.StartTiles;
            var maxFertility = 0m;
            var tiles = new List<Tile>();
            foreach (var island in map.Islands)
            {
                var existing = startTiles.Count(t => t.Island == island.Id);
                decimal islandFertility = island.TotalFertile;
                if (existing > 0)
                {
                    islandFertility /= 4m * existing;
                }
                foreach (var tile in island.Tiles)
                {
                    var fertility = islandFertility + tile.Fertility;
                    if (fertility < maxFertility) continue;
                    if (startTiles.Contains(tile) || tile.IsUnitPresent)
                    {
                        if (maxFertility > 0)
                        {
                            continue;
                        }
                    }
                    else if (fertility > maxFertility)
                    {
                        // This code may let adjacent tiles with exactly equal fertility pass, but they will be filtered out by the distance to start checks at the end this just ensures that we will have good spacing in our options
                        if (tile.CityRadius().Any(t => startTiles.Contains(t) || t.IsUnitPresent))
                        {
                            var adjustedFertility = fertility /4m;
                            if (adjustedFertility < maxFertility)
                            {
                                continue;
                            }

                            maxFertility = adjustedFertility;
                        }
                        else
                        {
                            maxFertility = fertility;
                        }
                        tiles.Clear();
                    }

                    tiles.Add(tile);
                }
            }

            var selectedTile = tiles.Count == 1 || config.StartTiles.Count == 0
                ? tiles.First()
                : tiles.OrderByDescending(t => DistanceToNearestStart(config, t)).First();

            config.StartTiles.Add(selectedTile);
            map.SetAsStartingLocation(selectedTile, civilization.Id);
            return selectedTile;
        }

        private static double DistanceToNearestStart(GameInitializationConfig config, Tile tile)
        {
            var minDist = Utilities.DistanceTo(config.StartTiles[0], tile);
            for (var i = 1; i < config.StartTiles.Count; i++)
            {
                var dist = Utilities.DistanceTo(config.StartTiles[i], tile);
                if (dist < minDist)
                {
                    minDist = dist;
                }
            }

            return minDist;
        }
    }
}