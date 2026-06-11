using Civ2engine;
using Model.Core;
using Model.Core.GoodyHuts;
using Model.Core.GoodyHuts.Outcomes;
using Model.Core.Mapping;
using Model.Core.Units;

namespace Core.Tests.Units;

public class GoldOutcomeTests
{
    [Fact]
    public void ApplyOutcome_IncreasesOwnerMoneyBySpecifiedAmount()
    {
        // Arrange
        var initialMoney = 100;
        var goldAmount = 50;
        var owner = new Civilization { Money = initialMoney };
        var unit = new Unit { Owner = owner };
        var goldOutcome = new GoldOutcome(goldAmount);

        // Act
        goldOutcome.ApplyOutcome(unit);

        // Assert
        Assert.Equal(initialMoney + goldAmount, owner.Money);
    }

    [Fact]
    public void AbandonedVillageOutcome_ReturnsSuccessfulNoEffectResult()
    {
        var unit = CreateUnit();
        var result = new AbandonedVillageOutcome().ApplyOutcome(unit);

        Assert.True(result.Success);
        Assert.Equal("AbandonedVillage", result.OutcomeType);
        Assert.Null(result.CreatedUnit);
    }

    [Fact]
    public void ScrollsOutcome_GrantsFirstUnknownAdvance()
    {
        var unit = CreateUnit(owner: new Civilization { Advances = [true, false, false] });

        var result = new ScrollsOutcome().ApplyOutcome(unit);

        Assert.True(result.Success);
        Assert.Equal("Scrolls", result.OutcomeType);
        Assert.Equal(1, result.AdvanceIndex);
        Assert.True(unit.Owner.Advances[1]);
    }

    [Fact]
    public void ScrollsOutcome_FailsWhenNoUnknownAdvanceExists()
    {
        var unit = CreateUnit(owner: new Civilization { Advances = [true, true] });

        var result = new ScrollsOutcome().ApplyOutcome(unit);

        Assert.False(result.Success);
        Assert.Equal("Scrolls", result.OutcomeType);
        Assert.Null(result.AdvanceIndex);
    }

    [Fact]
    public void MercenariesOutcome_AddsVeteranUnitAtCurrentTile()
    {
        var owner = new Civilization { Id = 1 };
        var unit = CreateUnit(owner: owner);
        owner.Units.Add(unit);
        var mercenaryType = new UnitDefinition { Name = "Warriors", Type = 2, Flags = new bool[20] };

        var result = new MercenariesOutcome(mercenaryType).ApplyOutcome(unit);

        Assert.True(result.Success);
        Assert.Equal("Mercenaries", result.OutcomeType);
        Assert.NotNull(result.CreatedUnit);
        Assert.Same(mercenaryType, result.CreatedUnit.TypeDefinition);
        Assert.True(result.CreatedUnit.Veteran);
        Assert.False(result.CreatedUnit.NeedsSupport);
        Assert.Contains(result.CreatedUnit, owner.Units);
        Assert.Contains(result.CreatedUnit, unit.CurrentLocation.UnitsHere);
    }

    [Fact]
    public void TribeOutcome_OnNonPlainsOrGrassland_AddsNomadUnit()
    {
        var owner = new Civilization { Id = 1 };
        var unit = CreateUnit(owner: owner, terrainType: TerrainType.Hills);
        var settlerType = new UnitDefinition { Name = "Settlers", Type = 0, Flags = new bool[20] };

        var result = new TribeOutcome(settlerType).ApplyOutcome(unit);

        Assert.True(result.Success);
        Assert.Equal("Nomads", result.OutcomeType);
        Assert.NotNull(result.CreatedUnit);
        Assert.Same(settlerType, result.CreatedUnit.TypeDefinition);
        Assert.Contains(result.CreatedUnit, owner.Units);
    }

    [Fact]
    public void TribeOutcome_OnPlainsOrGrassland_ReturnsAdvancedTribeResult()
    {
        var unit = CreateUnit(terrainType: TerrainType.Plains);

        var result = new TribeOutcome().ApplyOutcome(unit);

        Assert.True(result.Success);
        Assert.Equal("AdvancedTribe", result.OutcomeType);
        Assert.Null(result.CreatedUnit);
    }

    [Fact]
    public void BarbariansOutcome_ReturnsBarbarianResultForEngineToSpawn()
    {
        var unit = CreateUnit();

        var result = new BarbariansOutcome().ApplyOutcome(unit);

        Assert.True(result.Success);
        Assert.Equal("Barbarians", result.OutcomeType);
    }

    [Fact]
    public void GoodyHut_UsesConfiguredOutcomeList()
    {
        var unit = CreateUnit();
        var hut = new GoodyHut([new GoldOutcome(25)]);

        var result = hut.Trigger(unit);

        Assert.True(result.Success);
        Assert.Equal("Gold", result.OutcomeType);
        Assert.Equal(25, unit.Owner.Money);
    }

    private static Unit CreateUnit(Civilization? owner = null, TerrainType terrainType = TerrainType.Plains)
    {
        owner ??= new Civilization();
        var map = new Map(true, 0) { XDim = 1, YDim = 1, Tile = new Tile[1, 1] };
        var terrain = new Terrain { Name = terrainType.ToString(), Type = terrainType, Specials = [] };
        var tile = new Tile(0, 0, terrain, 0, map, 0, new bool[2]);
        map.Tile[0, 0] = tile;

        return new Unit
        {
            Owner = owner,
            TypeDefinition = new UnitDefinition { Name = "Explorer", Type = 50, Flags = new bool[20] },
            CurrentLocation = tile,
            X = tile.X,
            Y = tile.Y,
            MapIndex = tile.Z
        };
    }
}
