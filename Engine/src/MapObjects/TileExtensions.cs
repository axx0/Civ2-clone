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

        public static void CompleteConstruction(this Tile tile, TerrainImprovement improvement, AllowedTerrain terrain, int levelToBuild,
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

            var transformEffect = terrain.Effects.FirstOrDefault(e => e.Target == ImprovementConstants.Transform);
            if (transformEffect != null)
            {
                tile.Terrain = terrains[transformEffect.Value];
                return;
            }

            if (levelToBuild > 0)
            {
                var imp = Improvements.FirstOrDefault(i => i.Improvement == improvement.Id);
                if (imp != null)
                {
                    imp.Level = levelToBuild;
                    if (improvement.Levels[levelToBuild].Effects?.Count > 0)
                    {
                        tile.EffectsList.AddRange(improvement.Levels[levelToBuild].Effects.Select(e => new ActiveEffect
                            { Target = e.Target, Action = e.Action, Value = e.Value, Source = improvement.Id, Level = levelToBuild }));
                    }

                    return;
                }
            }
            if (improvement.Levels[levelToBuild].Effects?.Count > 0)
            {
                tile.EffectsList.AddRange(improvement.Levels[levelToBuild].Effects.Select(e => new ActiveEffect
                    { Target = e.Target, Action = e.Action, Value = e.Value, Source = improvement.Id, Level = 0 }));
            }

            if (terrain.Effects is { Count: > 0 })
            {
                tile.EffectsList.AddRange(terrain.Effects.Select(e => new ActiveEffect
                    { Target = e.Target, Action = e.Action, Value = e.Value, Source = improvement.Id, Level = 0}));
            }

            Improvements.Add(new ConstructedImprovement
                { Group = improvement.ExclusiveGroup, Improvement = improvement.Id, Level = levelToBuild });
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