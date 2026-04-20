using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Model.Constants;
using Model.Core;
using Model.Core.Units;
using Moq;

namespace Core.Tests.Cities;

public class CityExtensionsTests
{
    private (Mock<IGame> game, Rules rules, Civilization civ, Map map) SetupGame()
    {
        var rules = new Rules
        {
            Governments = new[]
            {
                new Government { Level = 0, SettlersConsumption = 1, UnitTypesAlwaysFree = Array.Empty<int>(), Distance = -1 },
                new Government { Level = 1, SettlersConsumption = 0, UnitTypesAlwaysFree = Array.Empty<int>(), Distance = -1 }
            }
        };
        rules.Cosmic.FoodEatenPerTurn = 2;
        var civ = new Civilization { Id = 1, Government = 0 };
        // Use a larger map to get a reasonable ScaleFactor
        var map = new Map(true, 5) { XDim = 40, YDim = 10 }; // ScaleFactor = 400 / 4000 = 0.1
        map.Tile = new Tile[40, 10];
        for (int y = 0; y < 10; y++)
        {
            for (int x_idx = 0; x_idx < 40; x_idx++)
            {
                int x = x_idx * 2 + y % 2;
                var tile = new Tile(x, y, new Terrain { Name = "Plains", Type = TerrainType.Plains, Specials = Array.Empty<Special>(), Defense = 100 }, 0, map, x_idx + y * 40, new bool[2]);
                tile.SetVisible(civ.Id);
                map.Tile[x_idx, y] = tile;
            }
        }
        var game = new Mock<IGame>();
        game.Setup(g => g.Rules).Returns(rules);
        game.Setup(g => g.MaxDistance).Returns(100.0);
        game.Setup(g => g.AllCivilizations).Returns(new List<Civilization> { civ });
        return (game, rules, civ, map);
    }

    [Fact]
    public void GetPopulation_ReturnsCorrectValue()
    {
        var city = new City { Size = 1 };
        Assert.Equal(10000, city.GetPopulation());
        city.Size = 3;
        // 1*10000 + 2*10000 + 3*10000 = 60000
        Assert.Equal(60000, city.GetPopulation());
    }

    [Fact]
    public void GetOrganizationLevel_ReturnsCorrectValue()
    {
        var (_, rules, civ, _) = SetupGame();
        var city = new City { Owner = civ };
        
        Assert.Equal(0, city.GetOrganizationLevel(rules));
        
        city.WeLoveKingDay = true;
        Assert.Equal(1, city.GetOrganizationLevel(rules));
        
        civ.Government = 1; // Government with Level 1
        city.WeLoveKingDay = false;
        Assert.Equal(1, city.GetOrganizationLevel(rules));
        
        city.WeLoveKingDay = true;
        Assert.Equal(2, city.GetOrganizationLevel(rules));
    }

    [Fact]
    public void IsNextToOcean_ReturnsCorrectValue()
    {
        var (_, _, _, map) = SetupGame();
        var tile = map.Tile[1, 1];
        var city = new City { Location = tile };
        
        Assert.False(city.IsNextToOcean());
        
        // (0, 1) is a neighbor of (1, 1) in staggered grid
        map.Tile[0, 1].Terrain = new Terrain { Type = TerrainType.Ocean };
        Assert.True(city.IsNextToOcean());
    }

    [Fact]
    public void IsNextToRiver_ReturnsCorrectValue()
    {
        var (_, _, _, map) = SetupGame();
        var tile = map.Tile[1, 1];
        var city = new City { Location = tile };
        
        Assert.False(city.IsNextToRiver());
        
        // (0, 1) is a neighbor of (1, 1)
        map.Tile[0, 1].River = true;
        Assert.True(city.IsNextToRiver());
    }

    [Fact]
    public void SetUnitSupport_CalculatesSupportCorrectly()
    {
        var (_, rules, civ, _) = SetupGame();
        var city = new City { Owner = civ, Size = 2 };
        var govt = rules.Governments[0];
        govt.NumberOfFreeUnitsPerCity = 1;
        
        var unit1 = new Unit { Owner = civ, HomeCity = city, TypeDefinition = new UnitDefinition { Flags = new bool[20] } };
        var unit2 = new Unit { Owner = civ, HomeCity = city, TypeDefinition = new UnitDefinition { Flags = new bool[20] } };
        city.SupportedUnits.AddRange(new[] { unit1, unit2 });
        civ.Units.AddRange(new[] { unit1, unit2 });
        
        city.SetUnitSupport(govt);
        
        Assert.False(unit1.NeedsSupport); // First unit is free
        Assert.True(unit2.NeedsSupport);  // Second unit needs support
        
        govt.NumberOfFreeUnitsPerCity = -1; // -1 means city size
        city.SetUnitSupport(govt);
        Assert.False(unit1.NeedsSupport);
        Assert.False(unit2.NeedsSupport);
    }

    [Fact]
    public void GetFoodStorage_ReturnsCorrectValue()
    {
        var city = new City();
        Assert.Equal(0, city.GetFoodStorage());
        
        var storage1 = new Improvement { Type = 1, Effects = { [Effects.FoodStorage] = 50 } };
        city.AddImprovement(storage1);
        Assert.Equal(50, city.GetFoodStorage());
        
        var storage2 = new Improvement { Type = 2, Effects = { [Effects.FoodStorage] = 50 } };
        city.AddImprovement(storage2);
        Assert.Equal(100, city.GetFoodStorage());
        
        // Test with invalid total storage (> 100)
        var storage3 = new Improvement { Type = 3, Effects = { [Effects.FoodStorage] = 10 } };
        city.OrderedImprovements.Clear();
        city.AddImprovement(storage1);
        city.AddImprovement(storage2);
        city.AddImprovement(storage3);
        // Sum is 110, so it should take Max of valid ones (50)
        Assert.Equal(50, city.GetFoodStorage());
    }

    [Fact]
    public void CalculateOutput_SimpleCase()
    {
        var (game, rules, civ, map) = SetupGame();
        var tile = map.Tile[1, 1];
        tile.Terrain.Food = 2;
        tile.Terrain.Shields = 1;
        tile.Terrain.Trade = 1;
        tile.Terrain.Defense = 100;
        
        var city = new City { Owner = civ, Location = tile, Size = 1 };
        city.WorkedTiles.Add(tile);
        civ.Cities.Add(city);
        
        city.CalculateOutput(0, game.Object);
        
        Assert.Equal(2, city.FoodProduction);
        Assert.Equal(1, city.TotalProduction);
        Assert.Equal(1, city.Trade);
        Assert.Equal(0, city.Waste);
        Assert.Equal(0, city.Corruption);
        Assert.Equal(1, city.Production); // TotalProduction (1) - Support (0) - Waste (0)
        Assert.Equal(2, city.FoodConsumption); // Size (1) * FoodEatenPerTurn (2)
        Assert.Equal(0, city.SurplusHunger);
    }

    [Fact]
    public void CalculatePollution_Cases()
    {
        var (game, rules, civ, map) = SetupGame();
        var city = new City { Owner = civ, Size = 10, Production = 30 };
        
        // We use reflection because CalculatePollution is private static
        var calculatePollutionMethod = typeof(CityExtensions).GetMethod("CalculatePollution", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        Assert.NotNull(calculatePollutionMethod);

        // Case 1: No improvements
        // Industrial: (30 / 1) - 20 = 10.
        // Population: modifier = 0.
        // Total = 10.
        var result = (int)calculatePollutionMethod.Invoke(null, new object[] { city });
        Assert.Equal(10, result);
        
        // Case 2: With industrial pollution elimination
        city.AddImprovement(new Improvement { Type = 1, Effects = { [Effects.EliminateIndustrialPollution] = 1 } });
        result = (int)calculatePollutionMethod.Invoke(null, new object[] { city });
        Assert.Equal(0, result);
        
        // Case 3: With population pollution modifier
        city.OrderedImprovements.Clear();
        city.AddImprovement(new Improvement { Type = 1, Effects = { [Effects.EliminateIndustrialPollution] = 1 } });
        city.AddImprovement(new Improvement { Type = 2, Effects = { [Effects.PopulationPollutionModifier] = 2 } });
        // Industrial: 0.
        // Population: 10 * 2 / 4 = 5.
        // Total = 5.
        result = (int)calculatePollutionMethod.Invoke(null, new object[] { city });
        Assert.Equal(5, result);
    }

    [Fact]
    public void CalculateOutput_WithCorruptionAndWaste()
    {
        var (game, rules, civ, map) = SetupGame();
        var cityTile = map.Tile[1, 1];
        var city = new City { Owner = civ, Location = cityTile, Size = 1 };
        civ.Cities.Add(city);
        
        // Use a fixed distance for government
        rules.Governments[0].Distance = 10;
        
        cityTile.Terrain.Trade = 100;
        cityTile.Terrain.Shields = 100;
        city.WorkedTiles.Add(cityTile);
        
        // ScaleFactor = 0.1. Effective distance = 10 * 0.1 = 1.0.
        // Trade = 99 (low organization).
        // corruption = 99 * 1.0 * (15/4) / 100 = 3.7125 -> 3.
        // waste = 100 * 1.0 * (15/4) / 100 = 3.75 -> 3.
        
        city.CalculateOutput(0, game.Object);
        
        Assert.Equal(3, city.Corruption);
        Assert.Equal(3, city.Waste);
        Assert.Equal(96, city.Trade); // 99 - 3
        Assert.Equal(96, city.Production); // 99 - 3
    }

    [Fact]
    public void AutoAddDistributionWorkers_AddsCorrectTiles()
    {
        var (game, rules, civ, map) = SetupGame();
        var cityTile = map.Tile[1, 1]; // coords [1, 1], X=3, Y=1
        var city = new City { Owner = civ, Location = cityTile, Size = 2 };
        cityTile.CityHere = city;
        city.WorkedTiles.Add(cityTile);
        cityTile.WorkedBy = city;
        
        // Neighbors of (1, 1) [coords]
        // [0, 1] is one of them.
        var neighbor = map.Tile[0, 1];
        neighbor.Terrain.Food = 4; // High food
        neighbor.Terrain.Specials = Array.Empty<Special>();
        neighbor.SetVisible(civ.Id);
        
        city.AutoAddDistributionWorkers(rules);
        
        Assert.Contains(neighbor, city.WorkedTiles);
        Assert.Equal(city, neighbor.WorkedBy);
    }

    [Fact]
    public void ShrinkAndGrowCity_WorksCorrectly()
    {
        var (game, rules, civ, map) = SetupGame();
        var cityTile = map.Tile[1, 1];
        var city = new City { Owner = civ, Location = cityTile, Size = 1 };
        cityTile.CityHere = city;
        city.WorkedTiles.Add(cityTile);
        cityTile.WorkedBy = city;
        civ.Cities.Add(city);

        // High food neighbor to ensure AutoAdd works
        var neighbor = map.Tile[0, 1];
        neighbor.Terrain.Food = 4;
        neighbor.SetVisible(civ.Id);

        city.GrowCity(game.Object);
        Assert.Equal(2, city.Size);
        // Total worked tiles should be Size + 1 = 3
        Assert.Equal(3, city.WorkedTiles.Count);

        city.ShrinkCity(game.Object);
        Assert.Equal(1, city.Size);
        // Size + 1 = 2
        Assert.Equal(2, city.WorkedTiles.Count);
    }
}
