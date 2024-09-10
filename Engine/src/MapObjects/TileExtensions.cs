using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.Terrains;
using Civ2engine.Units;
using Model.Core;
using Model.Core.Mapping;

namespace Civ2engine.MapObjects
{
    public static class TileExtensions
    {
        public static void RemoveImprovement(this Tile tile, TerrainImprovement improvement, int levelToRemove)
        {
            var built = tile.Improvements.FirstOrDefault(i => i.Improvement == improvement.Id);
            
            if (built == null || built.Level < levelToRemove) return;
            
            tile.EffectsList.RemoveAll(i => i.Source == improvement.Id && i.Level >= levelToRemove);
            if (levelToRemove == 0)
            {
                tile.Improvements.Remove(built);
            }
            else
            {
                built.Level = levelToRemove - 1;
            }
        }

        public static void AddImprovement(this Tile tile, TerrainImprovement improvement, AllowedTerrain terrain, int levelToBuild,
            Terrain[] terrains)
        {
            var improvements = tile.Improvements;
            if (improvement.ExclusiveGroup > 0)
            {
                var previous = improvements
                    .Where(i => i.Improvement != improvement.Id && i.Group == improvement.ExclusiveGroup).ToList();

                previous.ForEach(i =>
                {
                    tile.EffectsList.RemoveAll(e => e.Source == i.Improvement);
                    improvements.Remove(i);
                });
            }

            var transformEffect = terrain.Effects?.FirstOrDefault(e => e.Target == ImprovementConstants.Transform);
            if (transformEffect != null)
            {
                tile.Terrain = terrains[transformEffect.Value];
                return;
            }

            BuildEffects(tile, improvement, terrain, levelToBuild);

            var existing = improvements.FirstOrDefault(i => i.Improvement == improvement.Id);
            if (existing is not null)
            {
                existing.Level = levelToBuild;
            }
            else
            {
                improvements.Add(new ConstructedImprovement
                    { Group = improvement.ExclusiveGroup, Improvement = improvement.Id, Level = levelToBuild });
            }
        }

        public static void BuildEffects(this Tile tile, TerrainImprovement improvement, AllowedTerrain terrain, int levelToBuild)
        {
            tile.EffectsList.RemoveAll(e => e.Source == improvement.Id);

            tile.EffectsList.AddRange(improvement.Levels
                .Take(levelToBuild + 1)
                .SelectMany((levelData, levelNo) =>
                    levelData.Effects?.Select(e => new ActiveEffect(e, improvement.Id, levelNo)) ?? Enumerable.Empty<ActiveEffect>()));

            if (terrain.Effects is { Count: > 0 })
            {
                tile.EffectsList.AddRange(terrain.Effects.Select(e => new ActiveEffect(e, improvement.Id)));
            }
        }

        public static IEnumerable<Tile> CityRadius(this Tile tile, bool nullForInvalid = false)
        {
            return tile.Map.CityRadius(tile, nullForInvalid);
        }

        public static IEnumerable<Tile> Neighbours(this Tile tile)
        {
            return tile.Map.Neighbours(tile);
        }
        
        public static void UpdatePlayer(this Tile tile,int civilizationId)
        {
            if (tile.PlayerKnowledge == null || tile.PlayerKnowledge.Length <= civilizationId)
            {
                var know = new PlayerTile[civilizationId+1];
                if (tile.PlayerKnowledge != null)
                {
                    for (int i = 0; i < tile.PlayerKnowledge.Length; i++)
                    {
                        know[i] = tile.PlayerKnowledge[i];
                    }
                }
                tile.PlayerKnowledge = know;
            }

            tile.PlayerKnowledge[civilizationId] = new PlayerTile(tile);
        }

        /// <summary>
        /// Ensure player can see everything visible to them at game start or scenario start
        ///  This shouln't be called later (need to figure out how to exclude from loaded games)
        /// </summary>
        public static void UpdateAllPlayers(this Tile tile)
        {
            var visibility = tile.Visibility;
            var playerKnowledge = new PlayerTile[visibility.Length];
            for (var i = 0; i < visibility.Length; i++)
            {
                if(tile.Map.IsCurrentlyVisible(tile, i))
                {
                    playerKnowledge[i] = new PlayerTile(tile);
                }
                else if(playerKnowledge[i] == null)
                {
                    playerKnowledge[i] = new PlayerTile();
                }
            }
            tile.PlayerKnowledge = playerKnowledge;
        }
        
        
        public static Unit GetTopUnit(this Tile tile, Func<Unit, bool>? pred = null)
        {
            var units = pred != null ? tile.UnitsHere.Where(pred) : tile.UnitsHere;
            return (tile.Terrain.Type == TerrainType.Ocean
                    ? units.OrderByDescending(u => u.Domain == UnitGas.Sea ? 1 : 0)
                    : units.OrderByDescending(u => u.Domain == UnitGas.Sea ? 0 : 1))
                .ThenBy(u => u.AttackBase).First();
        }
    }
}