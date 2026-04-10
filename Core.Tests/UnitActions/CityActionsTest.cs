using Civ2engine;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Production;
using Civ2engine.Terrains;
using Civ2engine.UnitActions;
using Model.Core;
using Model.Core.Advances;
using Model.Core.Cities;
using Model.Core.Units;
using Moq;

namespace Core.Tests.UnitActions;

public class CityActionsTest
{
    private static Mock<IGame> SetupGame(int citiesToCreate, out Civilization civ)
    {
        var game = new Mock<IGame>();
        civ = new Civilization { TribeName = "TestCiv" };
        game.Setup(g => g.AllCivilizations).Returns([civ]);
        
        var cityNames = new Dictionary<string, List<string>?>
        {
            ["TESTCIV"] = ["City1", "City2", "City3"]
        };
        game.Setup(g => g.CityNames).Returns(cityNames);
        
        var mockHistory = new Mock<IHistory>();
        game.Setup(g => g.History).Returns(mockHistory.Object);
        
        var citiesBuiltCount = new Dictionary<Civilization, int>
        {
            [civ] = citiesToCreate
        };
        game.Setup(g => g.CitiesBuiltSoFar).Returns(citiesBuiltCount);
        return game;
    }

    [Fact]
    public void GetCityName_SuggestsNameInOrder()
    {
        var game = SetupGame(0, out var civ);
        Assert.Equal("City1", CityActions.GetCityName(civ, game.Object));
        
        game.Object.CitiesBuiltSoFar[civ] = 1;
        Assert.Equal("City2", CityActions.GetCityName(civ, game.Object));
    }

    [Fact]
    public void GetCityName_ReturnsDummyName_WhenListExhausted()
    {
        var game = SetupGame(3, out var civ);
        Assert.Equal("Dummy Name", CityActions.GetCityName(civ, game.Object));
    }

    [Fact]
    public void GetCityName_UsesExtra_WhenTribeNotFound()
    {
        var game = new Mock<IGame>();
        var civ = new Civilization { TribeName = "UnknownTribe" };
        var cityNames = new Dictionary<string, List<string>?>
        {
            ["EXTRA"] = ["ExtraCity1"]
        };
        game.Setup(g => g.CityNames).Returns(cityNames);
        game.Setup(g => g.CitiesBuiltSoFar).Returns(new Dictionary<Civilization, int>());

        Assert.Equal("ExtraCity1", CityActions.GetCityName(civ, game.Object));
    }

    [Fact]
    public void BuildCity_InitializesCityCorrectly()
    {
        // Arrange
        var game = new Mock<IGame>();
        var civ = new Civilization { Id = 0, TribeName = "Romans", Government = 0, Advances = new bool[10] };
        var unitDef = new UnitDefinition { Move = 10, Flags = new bool[20], Cost = 20 };
        var unit = new Unit { Owner = civ, TypeDefinition = unitDef };
        
        var map = new Map(true, 0)
        {
            XDim = 10,
            YDim = 10,
            Tile = new Tile[10, 10]
        };
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                map.Tile[x, y] = new Tile(x * 2, y, new Terrain { Name = "Plains", Type = TerrainType.Plains, Specials = Array.Empty<Special>() }, 0, map, x, new bool[2]);
            }
        }

        var tile = map.Tile[0, 0];
        unit.CurrentLocation = tile;

        var rules = new Rules
        {
            Advances = [new Advance { Name = "None" }],
            Improvements = [],
            Cosmic =
            {
                FoodEatenPerTurn = 2
            },
            Governments = [new Government { Level = 0, SettlersConsumption = 1 }]
        };

        var mockProd = new Mock<IProductionOrder>();
        mockProd.Setup(p => p.Cost).Returns(10);
        mockProd.Setup(p => p.RequiredTech).Returns(AdvancesConstants.Nil);
        mockProd.Setup(p => p.ExpiresTech).Returns(AdvancesConstants.Nil);
        rules.ProductionOrders = [mockProd.Object];
        ProductionPossibilities.InitializeProductionLists([civ], rules.ProductionOrders);
        
        game.Setup(g => g.Rules).Returns(rules);
        game.Setup(g => g.AllCities).Returns(new List<City>());
        game.Setup(g => g.CitiesBuiltSoFar).Returns(new Dictionary<Civilization, int>());
        game.Setup(g => g.History).Returns(new Mock<IHistory>().Object);
        game.Setup(g => g.Maps).Returns(new List<Map> { map });
        game.Setup(g => g.MaxDistance).Returns(100.0);
        game.Setup(g => g.TerrainImprovements).Returns(new Dictionary<int, TerrainImprovement>());

        // Act
        var city = CityActions.BuildCity(unit, game.Object, "Rome");

        // Assert
        Assert.Equal("Rome", city.Name);
        Assert.Equal(tile, city.Location);
        Assert.Equal(civ, city.Owner);
        Assert.True(unit.Dead);
        Assert.Contains(city, game.Object.AllCities);
        Assert.Contains(city, civ.Cities);
        Assert.Equal(1, game.Object.CitiesBuiltSoFar[civ]);
        Assert.Equal(city, tile.CityHere);
        Assert.Equal(city, tile.WorkedBy);
    }
}