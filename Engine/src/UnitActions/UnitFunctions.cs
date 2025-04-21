using System;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Civ2engine.Units;
using Model.Core;
using Model.Core.Units;

namespace Civ2engine.UnitActions
{
    public static class UnitFunctions
    {
        public static bool CanFortifyHere(Unit unit, Tile tile)
        {
            return unit.Domain switch
            {
                UnitGas.Ground => tile.Terrain.Type != TerrainType.Ocean,
                UnitGas.Air => tile.CityHere is not null || tile.EffectsList.Any(e => e.Target == ImprovementConstants.Airbase),
                UnitGas.Sea => tile.CityHere is not null,
                UnitGas.Special => true,
                _ => true
            };
        }

        public static bool CanEnter(UnitGas domain, Tile tile)
        {
            return domain switch
            {
                UnitGas.Ground => tile.Terrain.Type != TerrainType.Ocean,
                UnitGas.Air => true,
                UnitGas.Sea => tile.Terrain.Type == TerrainType.Ocean || tile.CityHere is not null,
                UnitGas.Special => true,
            };
        }
    }
}