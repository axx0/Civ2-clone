using System;
using System.Linq;
using Civ2engine.Terrains;
using Model.Core;

namespace Civ2engine.MapObjects;

public static class TileResourceExtensions
{
    public static int GetFood(this Tile tile, bool lowOrganisation)
        {
            decimal food = tile.EffectiveTerrain.Food;

            var foodEffects = tile.EffectsList.Where(e => e.Target == ImprovementConstants.Food).ToList();

            food += foodEffects.Where(e => e.Action == ImprovementActions.Add).Sum(e => e.Value);

            var cityImprovements = tile.WorkedBy?.Improvements.SelectMany(ci =>
                ci.TerrainEffects?.Where(ef =>ef.Resource == ImprovementConstants.Food &&
                    (!ef.Terrain.HasValue || ef.Terrain.Value == (int)tile.Terrain.Type ) && 
                    (!ef.Improvement.HasValue 
                     || tile.Improvements.Any(i => i.Improvement == ef.Improvement && i.Level >= ef.Level))
                    ) ?? Array.Empty<CityTerrainEffect>())?? Array.Empty<CityTerrainEffect>();
            foreach (var cityTerrainEffect in cityImprovements.OrderBy(i=>i.Action))
            {
                switch (cityTerrainEffect.Action)
                {
                    case CityTerrainEffect.Add:
                        food += cityTerrainEffect.Value;
                        break;
                    case CityTerrainEffect.AddExtra:
                        if (food > 0)
                        {
                            food += cityTerrainEffect.Value;
                        }
                        break;
                    case CityTerrainEffect.Multiply:
                        food *= cityTerrainEffect.Value / 100m;
                        break;
                }
            }
            
            if (tile.CityHere != null && food < 2)
            {
                food += 1;
            }
            if (food > 0)
            {
                var multiplier = foodEffects.Where(e => e.Action == ImprovementActions.Multiply)
                    .Sum(e => e.Value);
                if (multiplier != 0)
                {
                    food += food * (multiplier / 100m);
                }
            }

            if (lowOrganisation && food >= 3)
            {
                food -= 1;
            }

            return (int)food;
        }

        public static int GetTrade(this Tile tile,int organizationLevel)
        {
            decimal trade = tile.EffectiveTerrain.Trade;

            if (tile.River)
            {
                trade += tile.Terrain.RoadBonus;
            }

            var tradeEffects = tile.EffectsList.Where(e => e.Target == ImprovementConstants.Trade).ToList();

            trade += tradeEffects.Where(e => e.Action == ImprovementActions.Add).Sum(e => e.Value);
            if (organizationLevel > 1 && trade > 0)
            {
                trade += 1;
            }

            if (trade > 0)
            {
                var multiplier = tradeEffects.Where(e => e.Action == ImprovementActions.Multiply)
                    .Sum(e => e.Value);
                if (multiplier != 0)
                {
                    trade += trade * (multiplier / 100m);
                }
            }

            if (organizationLevel == 0 && trade >= 3)
            {
                trade -= 1;
            }

            return (int)trade;
        }

        public static int GetShields(this Tile tile, bool lowOrganization)
        {
            decimal shields = tile.Type == TerrainType.Grassland && !tile.HasShield ? 0 :  tile.EffectiveTerrain.Shields;

            var productionEffects = tile.EffectsList.Where(e => e.Target == ImprovementConstants.Shields).ToList();

            if (productionEffects.Count > 0)
            {
                shields += productionEffects.Where(e => e.Action == ImprovementActions.Add).Sum(e => e.Value);
                if (shields > 0)
                {
                    var multiplier = productionEffects.Where(e => e.Action == ImprovementActions.Multiply)
                        .Sum(e => e.Value);
                    if (multiplier != 0)
                    {
                        shields += shields * (multiplier / 100m);
                    }
                }
            }

            if (lowOrganization && shields >= 3)
            {
                shields -= 1;
            }

            return (int)shields;
        }

}