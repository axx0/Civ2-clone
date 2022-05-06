using System.Collections.Generic;
using System.Linq;
using Civ2engine.Terrains;

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
            var Improvements = tile.Improvements;
            if (improvement.ExclusiveGroup > 0)
            {
                var previous = Improvements
                    .Where(i => i.Improvement != improvement.Id && i.Group == improvement.ExclusiveGroup).ToList();

                previous.ForEach(i =>
                {
                    tile.EffectsList.RemoveAll(e => e.Source == i.Improvement);
                    Improvements.Remove(i);
                });
            }

            var transformEffect = terrain.Effects?.FirstOrDefault(e => e.Target == ImprovementConstants.Transform);
            if (transformEffect != null)
            {
                tile.Terrain = terrains[transformEffect.Value];
                return;
            }

            BuildEffects(tile, improvement, terrain, levelToBuild);

            var existing = Improvements.FirstOrDefault(i => i.Improvement == improvement.Id);
            if (existing is not null)
            {
                existing.Level = levelToBuild;
            }
            else
            {
                Improvements.Add(new ConstructedImprovement
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
    }
}