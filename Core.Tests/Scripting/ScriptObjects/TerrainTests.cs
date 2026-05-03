using Civ2engine.Scripting;
using Model.Core.Mapping;

namespace Core.Tests.Scripting.ScriptObjects;

public class TerrainTests
{
    [Fact]
    public void TerrainApi_Properties()
    {
        var (game, _, _) = ApiTestHarness.CreateGameAndAi();
        var tile = ApiTestHarness.FindEmptyTile(game);
        var api = new TerrainApi(tile);

        Assert.Equal(tile.Terrain.Type == TerrainType.Ocean, api.isOcean);
    }

    [Fact]
    public void BaseTerrain_Properties()
    {
        var (game, _, _) = ApiTestHarness.CreateGameAndAi();
        var terrain = game.Rules.Terrains[0][0];
        var api = new BaseTerrain(terrain, 0);

        Assert.Equal(terrain.Type == TerrainType.Ocean, api.isOcean);
        Assert.Equal(terrain, api.Terrain);
    }
}