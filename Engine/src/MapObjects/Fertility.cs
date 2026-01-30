using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.Terrains;
using Model.Core;
using Model.Core.Mapping;

namespace Civ2engine.MapObjects
{
    public static class Fertility
    {
        public static void CalculateFertility(this Map map, IEnumerable<Terrain> terrains)
        {
            var fertilityValues = terrains.Select(GetFertilityValue).ToArray();
            foreach (var island in map.Islands)
            {
                foreach (var tile in island.Tiles)
                {
                    SetFertility(map, tile, fertilityValues, island);
                }
            }
        }

        private static void SetFertility(Map map, Tile tile, decimal[][] fertilityValues, IslandDetails island)
        {
            var coasts = map.Neighbours(tile).Where(t => t.Terrain.Type == TerrainType.Ocean).Select(t => t.Island)
                .Distinct().ToList();
            tile.Fertility = map.CityRadius(tile, true).Sum(
                nTile =>
                {
                    if (nTile == null) // Being near the map edge is very bad
                    {
                        return -20;
                    }
                    var value = fertilityValues[(int) nTile.Terrain.Type][nTile.Special + 1];
                    if (tile.River)
                    {
                        value += 1;
                    }

                    if (value == 0) //Tiles with no fertility are bad
                    {
                        return -10;
                    }

                    if (nTile.Type != TerrainType.Ocean || coasts.Contains(nTile.Island))
                    {
                        return value;
                    }

                    //Ocean tiles are bad when we're not coastal
                    return -value;
                }) * (tile.Terrain.CanIrrigate == -1 ? tile.Terrain.Food + tile.Terrain.IrrigationBonus : tile.Terrain.Food);
            if (tile.Fertility < 0)
            {
                tile.Fertility = 0;
            }else if (tile.Terrain.Food > 0)
            {
                island.TotalFertile += tile.Terrain.Food;
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
                inRadius!.Fertility /= 2;
            }
        }

        public static void ReverseFertilityAdjustment(this Map map, Tile location, Terrain[] terrains)
        {
            var island = map.Islands[location.Island];
            var fertilityValues = terrains.Select(GetFertilityValue).ToArray();
            SetFertility(map, location, fertilityValues, island);
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