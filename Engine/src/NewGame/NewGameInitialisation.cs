using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Units;

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

            return Game.StartNew(maps, config, civilizations, paths);
        }

        private static Tile GetDefaultStart(GameInitializationConfig config, Civilization civilization, Map map)
        {
            var index = Array.FindIndex(config.Rules.Leaders, l => l.Adjective == civilization.Adjective);
            if (index > -1 && index < config.StartPositions.Length)
            {
                var pos = config.StartPositions[index];
                if (pos[0] != -1 && pos[1] != -1)
                {
                    var tile = map.TileC2(pos[0], pos[1]);
                    if (tile.Fertility > -1)
                    {
                        map.SetAsStartingLocation(tile, civilization.Id);
                        config.StartTiles.Add(tile);
                        return tile;
                    }
                }
            }

            return null;
        }


        private static Tile GetStartLoc(Civilization civilization, GameInitializationConfig config, Map map)
        {
            var maxFertility = 0m;
            var tiles = new HashSet<Tile>();
            for (int y = 0; y < map.Tile.GetLength(1); y++)
            {
                for (int x = 0; x < map.Tile.GetLength(0); x++)
                {
                    var tile = map.Tile[x, y];
                    if(tile.Fertility < maxFertility) continue;
                    if (tile.Fertility > maxFertility)
                    {
                        tiles.Clear();
                        maxFertility = tile.Fertility;
                    }

                    tiles.Add(tile);
                }
            }

            var selectedTile = tiles.OrderByDescending(t=> DistanceToNearestStart(config, t)).First();
            
            config.StartTiles.Add(selectedTile);
            map.SetAsStartingLocation(selectedTile, civilization.Id);
            return selectedTile;
        }

        private static double DistanceToNearestStart(GameInitializationConfig config, Tile tile)
        {
            if (config.StartTiles.Count == 0)
            {
                return config.Random.Next();
            }

            
            var minDist = Utilities.DistanceTo(config.StartTiles[0], tile);
            for (int i = 1; i < config.StartTiles.Count; i++)
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