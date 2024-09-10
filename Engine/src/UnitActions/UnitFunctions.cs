using System;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Civ2engine.Units;
using Model.Core;

namespace Civ2engine.UnitActions
{
    public static class UnitFunctions
    {
        public static UnitActionAssessment CanFortifyHere(Unit unit, Tile tile)
        {
            return unit.Domain switch
            {
                UnitGas.Ground => new UnitActionAssessment(tile.Terrain.Type != TerrainType.Ocean),
                UnitGas.Air => new UnitActionAssessment(tile.CityHere is not null || tile.EffectsList.Any(e => e.Target == ImprovementConstants.Airbase)),
                UnitGas.Sea => new UnitActionAssessment(tile.CityHere is not null),
                UnitGas.Special => new UnitActionAssessment(true),
                _ => new UnitActionAssessment(true)
            };
        }
    }
}