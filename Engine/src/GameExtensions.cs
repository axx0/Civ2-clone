using System.Collections.Generic;
using System.Linq;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Civ2engine.Units;
using Model.Core;

namespace Civ2engine;

public static class GameExtensions
{
    public static void SetImprovementsForCity(this IGame game,City city)
    {
        SetImprovementsForCities(game,city.Owner, city);
    }

    public static void SetImprovementsForCities(this IGame game,Civilization civilization, params City[] cities)
    {
        var citiesToSet = cities.Length == 0 ? civilization.Cities : (IList<City>)cities;
        var improvements = game.TerrainImprovements.Values
            .Where(t => t.AllCitys)
            .Select(improvement =>
            {
                var level = -1;
                for (var i = 0; i < improvement.Levels.Count; i++)
                {
                    if (AdvanceFunctions.HasTech(civilization, improvement.Levels[i].RequiredTech))
                    {
                        level = i;
                    }
                }

                return new { improvement, level };
            }).Where(ti => ti.level != -1)
            .ToList();


        foreach (var tile in citiesToSet.Select(c => c.Location))
        {
            tile.Improvements.Clear();
            tile.EffectsList.Clear();
            var visibleTo = tile.PlayerKnowledge?.Select(((playerTile, civId) => new { playerTile, civId } ))
                .Where(arg => arg.playerTile != null).Select(arg => arg.civId).ToList();
            foreach (var can in improvements)
            {
                var terrain = can.improvement.AllowedTerrains[tile.Z]
                    .FirstOrDefault(t => t.TerrainType == (int)tile.Type);
                if (terrain is not null)
                {
                    tile.AddImprovement(can.improvement, terrain, can.level, game.Rules.Terrains[tile.Z], visibleTo);
                }
            }
        }
    }
    public static List<Unit> CheckConstruction(this IGame game, Tile tile, TerrainImprovement improvement)
    {
        var units = tile.UnitsHere.Where(u => u.Building == improvement.Id).ToList();
        if (units.Count <= 0) return units;

        var terrain = improvement.AllowedTerrains[tile.Z].FirstOrDefault(t => t.TerrainType == (int)tile.Type);
        var existingImprovement = tile.Improvements.FirstOrDefault(i => i.Improvement == improvement.Id);

        if (terrain == null || (improvement.Negative && existingImprovement == null) ||
            (existingImprovement?.Level == improvement.Levels.Count - 1))
        {
            //If improvement has become invalid for terrain then return the units to the user
            units.ForEach(u =>
            {
                u.Counter = 0;
                u.Order = (int)OrderType.NoOrders;
                u.Building = 0;
            });
            return units;
        }

        int levelToBuild;
        if (existingImprovement != null)
        {
            if (!improvement.Negative)
            {
                levelToBuild = existingImprovement.Level;
            }
            else
            {
                levelToBuild = existingImprovement.Level + 1;
            }
        }
        else
        {
            levelToBuild = 0;
        }

        var progress = units.Sum(u => u.Counter);
        var cost = terrain.BuildTime;
        if (tile.River)
        {
            var river = improvement.AllowedTerrains[tile.Z]
                .FirstOrDefault(t => t.TerrainType == TerrainConstants.River);
            if (river != null)
            {
                cost += river.BuildTime;
            }
        }

        if (improvement.Levels[levelToBuild].BuildCostMultiplier != 0)
        {
            cost += cost * improvement.Levels[levelToBuild].BuildCostMultiplier / 100;
        }

        if (progress < cost)
        {
            return new List<Unit>();
        }

        var visibleTo = tile.GetCivsVisibleTo(game);
        if (improvement.Negative)
        {
            tile.RemoveImprovement(improvement, levelToBuild, visibleTo);
        }
        else
        {
            tile.AddImprovement(improvement, terrain, levelToBuild, game.Rules.Terrains[tile.Z], visibleTo);
        }

        units.ForEach(u =>
        {
            u.Counter = 0;
            u.Order = (int)OrderType.NoOrders;
            u.Building = 0;
        });

        var tiles = new List<Tile> { tile };
        if (improvement.HasMultiTile)
        {
            tiles.AddRange(tile.Map.Neighbours(tile));
        }

        game.TriggerMapEvent(MapEventType.UpdateMap, tiles);

        return units;
    }

    public static void UpdatePlayerViewData(this IGame game)
    {
        foreach (var map in game.Maps)
        {
            for (int y = 0; y < map.Tile.GetLength(1); y++)
            {
                for (int x = 0; x < map.Tile.GetLength(0); x++)
                {
                    var tile = map.Tile[x, y];
                    tile.UpdateAllPlayers();
                    if (tile.Owner == -1)
                    {
                        if (tile.IsUnitPresent)
                        {
                            tile.Owner = tile.UnitsHere[0].Owner.Id;
                        }
                        else if (tile.CityHere != null)
                        {
                            tile.Owner = tile.CityHere.OwnerId;
                        }
                        else if (tile.WorkedBy != null)
                        {
                            tile.Owner = tile.WorkedBy.OwnerId;
                        }
                    }
                }
            }
        }
    }
}