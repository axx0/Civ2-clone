using Civ2engine.Scripting;
using Civ2engine.Scripting.ScriptObjects;
using Neo.IronLua;

namespace Core.Tests.Scripting.ScriptObjects;

public class TileApiTests
{
    [Fact]
    public void TileApi_BasicProperties()
    {
        var (game, _, civ) = ApiTestHarness.CreateGameAndAi();
        var tile = ApiTestHarness.FindEmptyTile(game);
        var api = new TileApi(tile, game);

        Assert.Equal(tile.X, api.x);
        Assert.Equal(tile.Y, api.y);
        Assert.Equal(tile.Z, api.z);
        Assert.Equal(tile.Terrain.Type == Model.Core.TerrainType.Ocean, api.terrain.isOcean);
    }

    [Fact]
    public void TileApi_Units_ReturnsUnitApiWrappers()
    {
        var (game, _, civ) = ApiTestHarness.CreateGameAndAi();
        var tile = ApiTestHarness.FindEmptyTile(game);
        var unitType = game.Rules.UnitTypes.First();
        var unit = ApiTestHarness.CreateUnit(civ, unitType, tile);

        var api = new TileApi(tile, game);
        var unitsTable = (LuaTable)api.units;
        var units = new List<UnitApi>();
        foreach (var entry in (IEnumerable<KeyValuePair<object, object>>)unitsTable)
        {
            if (entry.Key is int && entry.Value is UnitApi u) units.Add(u);
        }

        Assert.Single(units);
        Assert.Equal(unit.Id, units[0].id);
    }

    [Fact]
    public void TileApi_City_ReturnsCity()
    {
        var (game, _, civ) = ApiTestHarness.CreateGameAndAi();
        var tile = ApiTestHarness.FindEmptyTile(game);
        var city = ApiTestHarness.CreateCity(game, civ, tile, "TestCity");

        var api = new TileApi(tile, game);
        Assert.NotNull(api.city);
        Assert.Equal("TestCity", api.city.Name);
    }

    [Fact]
    public void TileApi_Owner()
    {
        var (game, _, civ) = ApiTestHarness.CreateGameAndAi();
        var tile = ApiTestHarness.FindEmptyTile(game);
        var api = new TileApi(tile, game);

        Assert.Null(api.owner);

        api.owner = new Tribe(civ, game);
        Assert.Equal(civ.Id, tile.Owner);
        Assert.NotNull(api.owner);
        Assert.Equal(civ.Id, api.owner.Civ.Id);
    }

    [Fact]
    public void TileApi_Lua_Iteration()
    {
        var (game, _, civ) = ApiTestHarness.CreateGameAndAi();
        var tile = ApiTestHarness.FindEmptyTile(game);
        // ApiTestHarness.CreateUnit already adds to tile.UnitsHere
        ApiTestHarness.CreateUnit(civ, game.Rules.UnitTypes.First(), tile);

        var api = new TileApi(tile, game);

        using var l = new Lua();
        var g = l.CreateEnvironment();
        g["tile"] = api;

        // Iterating over LuaTable using pairs
        var result = g.DoChunk("local count = 0; for _ in ipairs(tile.units) do count = count + 1 end; return count",
            "test.lua");
        Assert.Equal(1, (int)result[0]);
    }
}