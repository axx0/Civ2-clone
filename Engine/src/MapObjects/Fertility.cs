using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.Terrains;
using Model.Core;

namespace Civ2engine.MapObjects
{
    public static class Fertility
    {
        public static void CalculateFertility(this Map map, IEnumerable<Terrain> terrains, IEnumerable<Tile> land = null)
        {
            land ??= map.Islands.SelectMany(i => i.Tiles);
            
            var fertilityValues = terrains.Select(GetFertilityValue).ToArray();
            foreach (var tile in land)
            {
                SetFertility(map, tile, fertilityValues);
            }
        }

        private static void SetFertility(Map map, Tile tile, decimal[][] fertilityValues)
        {
            var coasts = map.Neighbours(tile).Where(t => t.Terrain.Type == TerrainType.Ocean).Select(t => t.Island)
                .Distinct().ToList();
            tile.Fertility = map.CityRadius(tile).Sum(
                nTile =>
                {
                    var value = fertilityValues[(int) nTile.Terrain.Type][nTile.Special + 1];
                    if (tile.River)
                    {
                        value += 1;
                    }

                    if (nTile.Type != TerrainType.Ocean || coasts.Contains(nTile.Island))
                    {
                        return value;
                    }

                    return -value;
                }) * (tile.Terrain.CanIrrigate == -1 ? tile.Terrain.Food + tile.Terrain.IrrigationBonus : tile.Terrain.Food);

            if (tile.Fertility < 0)
            {
                tile.Fertility = 0;
            }
        }

        private static decimal[] GetFertilityValue(Terrain terrain)
        {
            var baseValue = terrain.Food * 1.5m + terrain.Shields + terrain.Trade;
            var specials = terrain.Specials.Select(s => s.Food * 1.5m + s.Shields + s.Trade);
            var result = new List<decimal> {baseValue};
            result.AddRange(specials);
            return result.ToArray();
        }

        public static void AdjustFertilityForCity(this Map map, Tile location)
        {
            
            location.Fertility = -2;

            foreach (var neighbour in map.Neighbours(location).Where(n=>n.Fertility > 0))
            {
                neighbour.Fertility /= 2;
            }

            foreach (var inRadius in map.CityRadius(location).Where(n=> n.Fertility > 0))
            {
                inRadius.Fertility /= 2;
            }
        }

        public static void ReverseFertilityAdjustment(this Map map, Tile location, Terrain[] terrains)
        {
            var fertilityValues = terrains.Select(GetFertilityValue).ToArray();
            SetFertility(map, location, fertilityValues);
            foreach (var neighbour in map.Neighbours(location))
            {
                if (neighbour.CityHere != null)
                {
                    location.Fertility /= 2;
                }
                else if(neighbour.Fertility > 0)
                {
                    neighbour.Fertility *= 2;
                }
            }

            foreach (var inRadius in map.CityRadius(location))
            {
                if (inRadius.CityHere != null)
                {
                    location.Fertility /= 2;
                }else if (inRadius.Fertility > 0)
                {
                    inRadius.Fertility *= 2;
                }
            }
        }
    }
}