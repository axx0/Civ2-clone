
using Civ2engine;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Civ2engine.UnitActions;
using Model.Core;
using Model.Core.Units;

namespace Engine.Tests;

public class MovementFunctionsTests
{
    private readonly MockGame _game;
    private readonly Map _map;
    private readonly Civilization _civ;
    private readonly Civilization _enemyCiv;

    public MovementFunctionsTests()
    {
        _game = CreateMockGame();
        _map = CreateTestMap();
        _civ = CreateTestCivilization(0, "TestCiv");
        _enemyCiv = CreateTestCivilization(1, "EnemyCiv");
        _game.AllCivilizations.Add(_civ);
        _game.AllCivilizations.Add(_enemyCiv);
        _game.Players = [new MockPlayer(_civ), new MockPlayer((_enemyCiv))];
        _game.Maps.Add(_map);
    }

    #region ActiveUnitCannotMove Tests

    [Fact]
    public void ActiveUnitCannotMove_NullUnit_ReturnsTrue()
    {
        // Arrange
        Unit? unit = null;

        // Act
        var result = MovementFunctions.ActiveUnitCannotMove(unit);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ActiveUnitCannotMove_DeadUnit_ReturnsTrue()
    {
        // Arrange
        var unit = CreateTestUnit(UnitGas.Ground, _civ);
        unit.Dead = true;

        // Act
        var result = MovementFunctions.ActiveUnitCannotMove(unit);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ActiveUnitCannotMove_NoMovePoints_ReturnsTrue()
    {
        // Arrange
        var unit = CreateTestUnit(UnitGas.Ground, _civ);
        unit.MovePointsLost = unit.MovePoints;
        unit.Dead = false;

        // Act
        var result = MovementFunctions.ActiveUnitCannotMove(unit);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ActiveUnitCannotMove_HasMovePoints_ReturnsFalse()
    {
        // Arrange
        var unit = CreateTestUnit(UnitGas.Ground, _civ);
        unit.Dead = false;
        unit.CurrentLocation = _map.TileC2(0, 0);

        // Act
        var result = MovementFunctions.ActiveUnitCannotMove(unit);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region MoveCost Tests

    [Fact]
    public void MoveCost_BasicMovement_ReturnsCorrectCost()
    {
        // Arrange
        var tileFrom = _map.TileC2(0, 0);
        var tileTo = _map.TileC2(1, 0);
        var cosmicRules = new CosmicRules();
        var baseMoveCost = 10;

        // Act
        var result = MovementFunctions.MoveCost(tileTo, tileFrom, baseMoveCost, cosmicRules);

        // Assert
        Assert.True(result > 0);
    }

    [Fact]
    public void MoveCost_WithRoad_ReducesCost()
    {
        // Arrange
        var tileFrom = _map.TileC2(0, 0);
        var tileTo = _map.TileC2(1, 0);
        var cosmicRules = new CosmicRules();
        var baseMoveCost = 10;

        var road = SetupRoadProperties();
        
        // Add road to both tiles
        tileTo.AddImprovement(road, new AllowedTerrain(), 1, [], [] );
        tileFrom.AddImprovement(road, new AllowedTerrain(), 1, [], []);

        // Act
        var result = MovementFunctions.MoveCost(tileTo, tileFrom, baseMoveCost, cosmicRules);

        // Assert
        // With road, cost should be reduced
        Assert.True(result <= baseMoveCost);
    }

    private static TerrainImprovement SetupRoadProperties()
    {
        var road = new TerrainImprovement()
        {
            Id = ImprovementTypes.Road,
            Levels = new List<ImprovementLevel>
            {
                new()
                {
                    RequiredTech = AdvancesConstants.Nil,
                    Effects =
                    [
                        new TerrainImprovementAction
                        {
                            Target = ImprovementConstants.Movement,
                            Action = ImprovementActions.Set,
                            Value = 3,
                        }
                    ]
                },
                new()
                {
                    RequiredTech = (int)AdvanceType.Railroad,
                    Effects =
                    [
                        new TerrainImprovementAction
                        {
                            Target = ImprovementConstants.Movement,
                            Action = ImprovementActions.Set,
                            Value = 0,
                        }
                    ],
                    BuildCostMultiplier = 50
                }
            },
            AllCitys = true,
            Shortcut = "R",
            Layer = 5,
            HasMultiTile = true
        };
        return road;
    }

    #endregion

    #region IsFriendlyTile Tests

    [Fact]
    public void IsFriendlyTile_EmptyTile_ReturnsFalse()
    {
        // Arrange
        var tile = _map.TileC2(0, 0);

        // Act
        var result = MovementFunctions.IsFriendlyTile(tile, _civ);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsFriendlyTile_FriendlyCity_ReturnsTrue()
    {
        // Arrange
        var tile = _map.TileC2(0, 0);
        var city = new City
        {
            Owner = _civ,
            Location = tile
        };
        tile.CityHere = city;

        // Act
        var result = MovementFunctions.IsFriendlyTile(tile, _civ);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsFriendlyTile_EnemyCity_ReturnsFalse()
    {
        // Arrange
        var tile = _map.TileC2(0, 0);
        var city = new City
        {
            Owner = _enemyCiv,
            Location = tile
        };
        tile.CityHere = city;

        // Act
        var result = MovementFunctions.IsFriendlyTile(tile, _civ);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsFriendlyTile_FriendlyUnit_ReturnsTrue()
    {
        // Arrange
        var tile = _map.TileC2(0, 0);
        var unit = CreateTestUnit(UnitGas.Ground, _civ);
        unit.CurrentLocation = tile;
        tile.UnitsHere.Add(unit);

        // Act
        var result = MovementFunctions.IsFriendlyTile(tile, _civ);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsFriendlyTile_EnemyUnit_ReturnsFalse()
    {
        // Arrange
        var tile = _map.TileC2(0, 0);
        var unit = CreateTestUnit(UnitGas.Ground, _enemyCiv);
        unit.CurrentLocation = tile;
        tile.UnitsHere.Add(unit);

        // Act
        var result = MovementFunctions.IsFriendlyTile(tile, _civ);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region IsNextToEnemy Tests

    [Fact]
    public void IsNextToEnemy_NoEnemies_ReturnsFalse()
    {
        // Arrange
        var tile = _map.TileC2(5, 5);

        // Act
        var result = MovementFunctions.IsNextToEnemy(tile, _civ, UnitGas.Ground);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNextToEnemy_EnemyOnAdjacentTile_ReturnsTrue()
    {
        // Arrange
        var tile = _map.TileC2(5, 5);
        var adjacentTile = tile.Neighbours().First();
        var enemyUnit = CreateTestUnit(UnitGas.Ground, _enemyCiv);
        enemyUnit.CurrentLocation = adjacentTile;
        adjacentTile.UnitsHere.Add(enemyUnit);

        // Act
        var result = MovementFunctions.IsNextToEnemy(tile, _civ, UnitGas.Ground);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region GetPossibleMoves Tests

    [Fact]
    public void GetPossibleMoves_LandUnit_ReturnsValidMoves()
    {
        // Arrange
        var (tile, _) = GetOfTypeNextToLand();
        var unit = CreateTestUnit(UnitGas.Ground, _civ);
        unit.CurrentLocation = tile;
        var count = tile.Neighbours().Count(t=>t.Terrain.Type != TerrainType.Ocean);

        // Act
        var possibleMoves = MovementFunctions.GetPossibleMoves(tile, unit).ToList();

        // Assert
        Assert.Equal(possibleMoves.Count, count);
    }
    

    [Fact]
    public void GetPossibleMoves_SeaUnit_OnlyReturnsOceanTiles()
    {
        // Arrange
        var (tile, _) = GetOfTypeNextToLand(TerrainType.Ocean);
        var unit = CreateTestUnit(UnitGas.Sea, _civ);
        unit.CurrentLocation = tile;
        unit.MovePointsLost = 0;

        // Act
        var possibleMoves = MovementFunctions.GetPossibleMoves(tile, unit).ToList();

        // Assert
        // All possible moves should be ocean tiles
        Assert.All(possibleMoves, t => Assert.Equal(TerrainType.Ocean, t.Terrain.Type));
    }
    
    [Fact]
    public void GetPossibleMoves_SeaUnit_ReturnsAllTiles_When_CarryingUnits()
    {
        // Arrange
        var (tile, _) = GetOfTypeNextToLand(TerrainType.Ocean);
        var unit = CreateTestUnit(UnitGas.Sea, _civ, 2);
        var carried = CreateTestUnit(UnitGas.Ground, _civ);
        carried.CurrentLocation = tile;
        unit.CurrentLocation = tile;
        unit.CarriedUnits.Add(carried);
        carried.InShip = unit;
        unit.MovePointsLost = 0;
        var count = tile.Neighbours().Count();

        // Act
        var possibleMoves = MovementFunctions.GetPossibleMoves(tile, unit).ToList();

        // Assert
        // All possible moves should be ocean tiles
        Assert.Equal(possibleMoves.Count, count);
    }

    #endregion

    #region GetIslandsFor Tests

    [Fact]
    public void GetIslandsFor_LandUnit_ReturnsLandIslands()
    {
        // Arrange
        var unit = CreateTestUnit(UnitGas.Ground, _civ);
        var tile = _map.TileC2(5, 5);
        tile.Island = 1;
        unit.CurrentLocation = tile;

        // Act
        var islands = MovementFunctions.GetIslandsFor(unit).ToList();

        // Assert
        Assert.NotEmpty(islands);
        Assert.Contains(1, islands);
    }

    [Fact]
    public void GetIslandsFor_SeaUnit_ReturnsSeaIslands()
    {
        // Arrange
        var unit = CreateTestUnit(UnitGas.Sea, _civ);
        var tile = _map.TileC2(5, 5);
        tile.Terrain = new Terrain { Type = TerrainType.Ocean };
        tile.Island = 0;
        unit.CurrentLocation = tile;

        // Act
        var islands = MovementFunctions.GetIslandsFor(unit).ToList();

        // Assert
        Assert.NotEmpty(islands);
        Assert.Contains(0, islands);
    }

    #endregion

    #region AttackAtTile Tests

    [Fact]
    public void AttackAtTile_EnemyUnit_ReturnsTrue()
    {
        // Arrange
        var attackingUnit = CreateTestUnit(UnitGas.Ground, _civ);
        var defendingUnit = CreateTestUnit(UnitGas.Ground, _enemyCiv);
        var (tile, from) = GetOfTypeNextToLand();
        
        attackingUnit.CurrentLocation = from;
        defendingUnit.CurrentLocation = tile;
        tile.UnitsHere.Add(defendingUnit);

        // Act
        var result = MovementFunctions.AttackAtTile(attackingUnit, _game, tile);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AttackAtTile_EmptyTile_ReturnsFalse()
    {
        // Arrange
        var attackingUnit = CreateTestUnit(UnitGas.Ground, _civ);
        var tile = _map.TileC2(5, 5);
        attackingUnit.CurrentLocation = _map.TileC2(4, 5);

        // Act
        var result = MovementFunctions.AttackAtTile(attackingUnit, _game, tile);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region UnitMoved Tests

    [Fact]
    public void UnitMoved_ValidMove_ReturnsTrue()
    {
        // Arrange
        var unit = CreateTestUnit(UnitGas.Ground, _civ);
        var (tileFrom, tileTo) = GetOfTypeNextToLand();
        unit.CurrentLocation = tileFrom;
        unit.MovePointsLost = 0;
        tileFrom.UnitsHere.Add(unit);

        // Act
        var result = MovementFunctions.UnitMoved(_game, unit, tileTo, tileFrom);

        // Assert
        Assert.True(result);
        Assert.Equal(tileTo, unit.CurrentLocation);
    }

    private Tuple<Tile, Tile> GetOfTypeNextToLand(TerrainType type = TerrainType.Plains)
    {
        for (int i = 0; i < _map.Tile.GetLength(0); i++)
        {
            for (int j = 0; j < _map.Tile.GetLength(1); j++)
            {
                if (_map.Tile[i, j].Terrain.Type == type)
                {
                    var to = _map.Tile[i, j].Neighbours().FirstOrDefault(t=>t.Terrain.Type != TerrainType.Ocean);
                    if (to != null)
                    {
                        return new Tuple<Tile, Tile>(_map.Tile[i, j], to);
                    }
                }
            }
        }
        throw new Exception("No valid tiles found");
    }
    

    #endregion

    #region Helper Methods

    private MockGame CreateMockGame()
    {
        return new MockGame
        {
            AllCivilizations = new List<Civilization>(),
            Maps = new List<Map>(),
            Rules = new Rules()
        };
    }

    private Map CreateTestMap()
    {
        var ran = new Random();
        var map = new Map(true, 0)
        {
            Tile = new Tile[10, 10],
            XDim = 10,
            YDim = 10
        };
        
        var plains = new Terrain { Type = TerrainType.Plains, Specials = []};
        var ocean = new Terrain { Type = TerrainType.Ocean, Specials = []};

        Terrain[] terrains = [plains, ocean];
        
        var seed = 1;

        // Initialize all tiles
        for (int x = 0; x < map.XDim; x++)
        {
            for (int y = 0; y < map.YDim; y++)
            {
                map.Tile[x, y] = new Tile(x, y, terrains[ran.Next(terrains.Length)], seed, map,x, [] )
                {
                    Island = 1
                };
            }
        }

        return map;
    }

    private Civilization CreateTestCivilization(int id, string name)
    {
        return new Civilization
        {
            Id = id,
            TribeName = name,
            Money = 1000
        };
    }

    private Unit CreateTestUnit(UnitGas domain, Civilization owner, int hold = 0)
    {
        return new Unit
        {
            Owner = owner,
            TypeDefinition = new UnitDefinition
            {
                Domain = domain,
                Move = 10,
                Flags = Enumerable.Repeat(false, 13).ToArray(),
                Attack = 5,
                Defense = 3,
                Hold = hold
            },
            Dead = false,
        };
    }

    #endregion
}

// Extended MockGame for testing