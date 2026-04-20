using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Civ2engine.Units;
using Model.Constants;
using Model.Core;
using Model.Core.Units;
using Moq;

namespace Core.Tests.Units;

public class UnitExtensionsCombatTests
{
    private (Mock<IGame> game, Rules rules, Map map) SetupGame()
    {
        var rules = new Rules();
        var map = new Map(true, 5) { XDim = 10, YDim = 10 };
        map.Tile = new Tile[10, 10];
        for (int y = 0; y < 10; y++)
        {
            for (int x_idx = 0; x_idx < 10; x_idx++)
            {
                int x = x_idx * 2 + y % 2;
                map.Tile[x_idx, y] = new Tile(x, y, new Terrain { Name = "Plains", Type = TerrainType.Plains, Specials = Array.Empty<Special>(), Defense = 100 }, 0, map, x_idx + y * 10, new bool[2]);
            }
        }
        var game = new Mock<IGame>();
        game.Setup(g => g.Rules).Returns(rules);
        return (game, rules, map);
    }

    [Fact]
    public void AttackFactor_CalculatesCorrectly()
    {
        var (game, rules, map) = SetupGame();
        var attacker = new Unit
        {
            TypeDefinition = new UnitDefinition
            {
                Attack = 4,
                Flags = new bool[20]
            },
            Veteran = false
        };
        var defender = new Unit { TypeDefinition = new UnitDefinition { Attack = 1 } };
        
        // Base case: 4
        Assert.Equal(4.0, attacker.AttackFactor(defender));
        
        // Veteran: 4 * 1.5 = 6
        attacker.Veteran = true;
        Assert.Equal(6.0, attacker.AttackFactor(defender));
    }

    [Fact]
    public void DefenseFactor_CalculatesCorrectly()
    {
        var (game, rules, map) = SetupGame();
        var tile = map.Tile[1, 1];
        tile.Terrain.Defense = 100; // 100% bonus? No, wait.
        // Tile.Defense implementation: (River ? EffectiveTerrain.Defense + 1 : EffectiveTerrain.Defense) / 2
        // EffectiveTerrain.Defense is 100. (100 / 2) = 50.
        // Wait, why / 2? Civ2 terrain defense is usually 1.0, 1.5, etc.
        // 100 probably means 1.0.
        
        var defender = new Unit
        {
            TypeDefinition = new UnitDefinition
            {
                Defense = 2,
                Flags = new bool[20],
                Domain = UnitGas.Ground
            },
            Veteran = false
        };
        var attacker = new Unit
        {
            TypeDefinition = new UnitDefinition
            {
                Flags = new bool[20],
                Domain = UnitGas.Ground
            }
        };

        // Base case: df = 2. bestGroundFactor = 0. df = 2.
        // Terrain bonus: df * tile.Defense = 2 * 50 = 100.
        // Wait, 50?
        Assert.Equal(100, defender.DefenseFactor(attacker, tile, 0));
        
        // Veteran: 2 * 1.5 = 3. 3 * 50 = 150.
        defender.Veteran = true;
        Assert.Equal(150, defender.DefenseFactor(attacker, tile, 0));
        
        // Fortified: df = 3. fortifiedFactor = 3 / 2 = 1.5. df += 1.5 = 4.5.
        // 4.5 * 50 = 225.
        defender.Order = (int)OrderType.Fortified;
        Assert.Equal(225, defender.DefenseFactor(attacker, tile, 0));
    }

    [Fact]
    public void DefenseFactor_WithSAMBattery_CalculatesCorrectly()
    {
        var (game, rules, map) = SetupGame();
        var tile = map.Tile[1, 1];
        var city = new City { Owner = new Civilization { Id = 1 } };
        tile.CityHere = city;
        
        var defender = new Unit
        {
            TypeDefinition = new UnitDefinition
            {
                Defense = 2,
                Flags = new bool[20],
                Domain = UnitGas.Ground
            },
            CurrentLocation = tile
        };
        var attacker = new Unit
        {
            TypeDefinition = new UnitDefinition
            {
                Flags = new bool[20],
                Domain = UnitGas.Air
            }
        };

        // Add SAM Battery effect (100 means +100% = 2x total)
        city.AddImprovement(new Improvement { Type = 1, Effects = { [Effects.AirDefence] = 100 } });
        
        // Base df = 2. SAM bonus = 2 * 100 / 100 = 2. df = 4.
        // Terrain bonus: 4 * 50 = 200.
        Assert.Equal(200, defender.DefenseFactor(attacker, tile, 0));
        
        // Ground attacker does not trigger SAM
        var groundAttacker = new Unit { TypeDefinition = new UnitDefinition { Domain = UnitGas.Ground, Flags = new bool[20] } };
        Assert.Equal(100, defender.DefenseFactor(groundAttacker, tile, 0));
    }

    [Fact]
    public void DefenseFactor_WithCoastalFortress_CalculatesCorrectly()
    {
        var (game, rules, map) = SetupGame();
        var tile = map.Tile[1, 1];
        var city = new City { Owner = new Civilization { Id = 1 } };
        tile.CityHere = city;
        
        var defender = new Unit
        {
            TypeDefinition = new UnitDefinition
            {
                Defense = 2,
                Flags = new bool[20],
                Domain = UnitGas.Ground
            },
            CurrentLocation = tile
        };
        var attacker = new Unit
        {
            TypeDefinition = new UnitDefinition
            {
                Flags = new bool[20],
                Domain = UnitGas.Sea
            }
        };

        // Add Coastal Fortress effect (100 means +100% = 2x total)
        city.AddImprovement(new Improvement { Type = 1, Effects = { [Effects.SeaDefence] = 100 } });
        
        // Base df = 2. Sea defence bonus = 2 * 100 / 100 = 2. df = 4.
        // Terrain bonus: 4 * 50 = 200.
        Assert.Equal(200, defender.DefenseFactor(attacker, tile, 0));
    }
}
