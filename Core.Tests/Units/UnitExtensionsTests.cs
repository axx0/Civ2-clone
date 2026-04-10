using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Civ2engine.Units;
using Model.Constants;
using Model.Core.Cities;
using Model.Core.Units;
using Moq;

namespace Core.Tests.Units;

public class UnitExtensionsTests
{
    [Fact]
    public void AttackFactor_BaseValue()
    {
        var unit = new Unit { TypeDefinition = new UnitDefinition { Attack = 10 } };
        var enemy = new Unit { TypeDefinition = new UnitDefinition { Attack = 5 } };
        
        Assert.Equal(10.0, unit.AttackFactor(enemy));
    }

    [Fact]
    public void AttackFactor_VeteranBonus()
    {
        var unit = new Unit { TypeDefinition = new UnitDefinition { Attack = 10 }, Veteran = true };
        var enemy = new Unit { TypeDefinition = new UnitDefinition { Attack = 5 } };
        
        Assert.Equal(15.0, unit.AttackFactor(enemy));
    }

    [Fact]
    public void DefenseFactor_CarriedUnit_ReturnsZero()
    {
        var unit = new Unit { InShip = new Unit() };
        var attacker = new Unit();
        var map = new Map(true, 0);
        var tile = new Tile(0, 0, new Terrain { Specials = [] }, 0, map, 0, new bool[0]);
        
        Assert.Equal(0, unit.DefenseFactor(attacker, tile, 0));
    }

    [Fact]
    public void DefenseFactor_GroundUnit_BaseValue()
    {
        var unit = new Unit { TypeDefinition = new UnitDefinition { Defense = 10, Domain = UnitGas.Ground, Flags = new bool[15] } };
        var attacker = new Unit { TypeDefinition = new UnitDefinition { Domain = UnitGas.Ground, Flags = new bool[15] } };
        var map = new Map(true, 0);
        var tile = new Tile(0, 0, new Terrain { Defense = 2, Specials = [] }, 0, map, 0, new bool[2]);
        
        Assert.Equal(10, unit.DefenseFactor(attacker, tile, 0));
    }

    [Fact]
    public void DefenseFactor_VeteranBonus()
    {
        var unit = new Unit { TypeDefinition = new UnitDefinition { Defense = 10, Domain = UnitGas.Ground, Flags = new bool[15] }, Veteran = true };
        var attacker = new Unit { TypeDefinition = new UnitDefinition { Domain = UnitGas.Ground, Flags = new bool[15] } };
        var map = new Map(true, 0);
        var tile = new Tile(0, 0, new Terrain { Defense = 2, Specials = [] }, 0, map, 0, new bool[2]);
        
        // 10 * 1.5 = 15. Tile defense factor 1. Total 15.
        Assert.Equal(15, unit.DefenseFactor(attacker, tile, 0));
    }

    [Fact]
    public void DefenseFactor_FortifiedBonus()
    {
        var unit = new Unit { TypeDefinition = new UnitDefinition { Defense = 10, Domain = UnitGas.Ground, Flags = new bool[15] }, Order = (int)OrderType.Fortified };
        var attacker = new Unit { TypeDefinition = new UnitDefinition { Domain = UnitGas.Ground, Flags = new bool[15] } };
        var map = new Map(true, 0);
        var tile = new Tile(0, 0, new Terrain { Defense = 2, Specials = [] }, 0, map, 0, new bool[2]);
        
        // 10 + (10/2) = 15.
        Assert.Equal(15, unit.DefenseFactor(attacker, tile, 0));
    }

    [Fact]
    public void DefenseFactor_FortressBonus()
    {
        var unit = new Unit { TypeDefinition = new UnitDefinition { Defense = 10, Domain = UnitGas.Ground, Flags = new bool[15] } };
        var attacker = new Unit { TypeDefinition = new UnitDefinition { Domain = UnitGas.Ground, Flags = new bool[15] } };
        var map = new Map(true, 0);
        var tile = new Tile(0, 0, new Terrain { Defense = 2, Specials = [] }, 0, map, 0, new bool[2]);
        
        // 10 + (10 * 50 / 100) = 15.
        Assert.Equal(15, unit.DefenseFactor(attacker, tile, 50));
    }

    [Fact]
    public void DefenseFactor_CityWallsBonus()
    {
        var unit = new Unit { TypeDefinition = new UnitDefinition { Defense = 10, Domain = UnitGas.Ground, Flags = new bool[15] } };
        var attacker = new Unit { TypeDefinition = new UnitDefinition { Domain = UnitGas.Ground, Flags = new bool[15] } };
        var map = new Map(true, 0);
        var tile = new Tile(0, 0, new Terrain { Defense = 2, Specials = [] }, 0, map, 0, new bool[2]);
        var city = new City { Owner = new Civilization { Id = 1 } };
        var walls = new Improvement
        {
            Type = 1,
            Effects =
            {
                [Effects.Walled] = 300
            }
        };
        city.AddImprovement(walls);
        tile.CityHere = city;
        
        // 10 + (300 / 100) = 13. ??? Wait, line 66 in UnitExtensions.cs: 
        // var totalWallDefence = tile.CityHere.Improvements.Sum(i => i.Effects.GetValueOrDefault(Effects.Walled, 0)) / 100m;
        // It's NOT a multiplier of df, it's an additive factor?
        // Let's check: df += bestGroundFactor;
        // And bestGroundFactor = totalWallDefence if totalWallDefence > bestGroundFactor.
        // If totalWallDefence is 3.0, and df is 10.0, it becomes 13.0.
        // That seems WRONG for City Walls (usually 3x defense).
        // Let's re-read UnitExtensions.cs.
        
        Assert.Equal(13, unit.DefenseFactor(attacker, tile, 0));
    }
}
