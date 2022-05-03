using System;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Civ2engine.Units;

namespace Civ2engine.UnitActions
{
    public static class UnitFunctions
    {
        public static UnitActionAssessment CanFortifyHere(Unit unit, Tile tile)
        {
            return unit.Domain switch
            {
                UnitGAS.Ground => new UnitActionAssessment(tile.Terrain.Type != TerrainType.Ocean),
                UnitGAS.Air => new UnitActionAssessment(tile.CityHere is not null || tile.HasAirbase()),
                UnitGAS.Sea => new UnitActionAssessment(tile.CityHere is not null),
                UnitGAS.Special => new UnitActionAssessment(true),
                _ => new UnitActionAssessment(true)
            };
        }
    }
}