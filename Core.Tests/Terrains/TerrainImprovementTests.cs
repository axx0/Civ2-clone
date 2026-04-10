using Civ2engine;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Model.Core;
using Moq;

namespace Core.Tests.Terrains;

public class TerrainImprovementTests
{
    [Fact]
    public void CanImprovementBeBuiltHere_CityPresent_ReturnsFalse()
    {
        var map = new Map(true, 0);
        var tile = new Tile(0, 0, new Terrain { Specials = [] }, 0, map, 0, new bool[2]);
        tile.CityHere = new City();
        var improvement = new TerrainImprovement();
        improvement.Levels = new List<ImprovementLevel> { new ImprovementLevel { RequiredTech = AdvancesConstants.Nil } };
        var civ = new Civilization();
        
        
        var result = TerrainImprovementFunctions.CanImprovementBeBuiltHere(tile, improvement, civ);
        
        Assert.False(result.Enabled);
        Assert.Equal("CANTDO", result.ErrorPopup);
    }

    [Fact]
    public void CanImprovementBeBuiltHere_NotAllowedOnTerrain_ReturnsFalse()
    {
        var map = new Map(true, 0);
        var tile = new Tile(0, 0, new Terrain { Type = TerrainType.Plains, Specials = [] }, 0, map, 0, new bool[2]);
        var improvement = new TerrainImprovement();
        improvement.Levels = new List<ImprovementLevel> { new ImprovementLevel { RequiredTech = AdvancesConstants.Nil } };
        improvement.AllowedTerrains = new List<List<AllowedTerrain>> { new List<AllowedTerrain>() };
        var civ = new Civilization();
        
        var result = TerrainImprovementFunctions.CanImprovementBeBuiltHere(tile, improvement, civ);
        
        Assert.False(result.Enabled);
        Assert.Equal("CANTIMPROVE", result.ErrorPopup);
    }

    [Fact]
    public void LabelFrom_ReturnsFormattedLabel()
    {
        Labels.Items = new string[1000];
        Labels.Items[(int)LabelIndex.Build] = "Build";
        
        var level = new ImprovementLevel { Name = "Road" };
        Assert.Equal("Build Road", TerrainImprovementFunctions.LabelFrom(level));
        
        level.BuildLabel = "Custom Label";
        Assert.Equal("Custom Label", TerrainImprovementFunctions.LabelFrom(level));
        
        level.BuildLabel = "~Build"; // Should parse enum
        Assert.Equal("Build", TerrainImprovementFunctions.LabelFrom(level));
    }
}
