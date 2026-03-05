using System.Linq;
using Civ2engine.Scripting.ScriptObjects;
using Neo.IronLua;
using Xunit;

namespace Engine.Tests;

public class UnitApiTests
{
    [Fact]
    public void UnitApi_BasicProperties()
    {
        var (game, _, civ) = ApiTestHarness.CreateGameAndAi();
        var tile = ApiTestHarness.FindEmptyTile(game);
        var unitType = game.Rules.UnitTypes.First();
        var unit = ApiTestHarness.CreateUnit(civ, unitType, tile);
        
        var api = new UnitApi(unit, game);
        
        Assert.Equal(unit.Id, api.id);
        Assert.Equal(unit.RemainingHitpoints, api.hitpoints);
        Assert.Equal(unit.MovePointsLost, api.moveSpent);
        Assert.Equal(unit.Veteran, api.veteran);
        Assert.Equal(tile.X, api.location.x);
        Assert.Equal(tile.Y, api.location.y);
        Assert.Equal(unitType.Name, api.type.name);
    }

    [Fact]
    public void UnitApi_Lua_PropertyAccess()
    {
        var (game, _, civ) = ApiTestHarness.CreateGameAndAi();
        var tile = ApiTestHarness.FindEmptyTile(game);
        var unitType = game.Rules.UnitTypes.First();
        var unit = ApiTestHarness.CreateUnit(civ, unitType, tile);
        var api = new UnitApi(unit, game);
        
        using var l = new Lua();
        var g = l.CreateEnvironment();
        g["unit"] = api;
        
        var result = g.DoChunk("return unit.id, unit.hitpoints, unit.type.name", "test.lua");
        
        Assert.Equal(unit.Id, (int)result[0]);
        Assert.Equal(unit.RemainingHitpoints, (int)result[1]);
        Assert.Equal(unitType.Name, (string)result[2]);
    }

    [Fact]
    public void UnitApi_ExtendedData()
    {
        var (game, _, civ) = ApiTestHarness.CreateGameAndAi();
        var tile = ApiTestHarness.FindEmptyTile(game);
        var unitType = game.Rules.UnitTypes.First();
        var unit = ApiTestHarness.CreateUnit(civ, unitType, tile);
        var api = new UnitApi(unit, game);
        
        api.SetNum("test_num", 42);
        Assert.Equal(42, api.GetNum("test_num"));
        
        api.SetString("test_str", "hello");
        Assert.Equal("hello", api.GetString("test_str"));
        
        using var l = new Lua();
        var g = l.CreateEnvironment();
        g["unit"] = api;
        
        g.DoChunk("unit:SetNum('lua_num', 123); unit:SetString('lua_str', 'world')", "test.lua");
        
        Assert.Equal(123, api.GetNum("lua_num"));
        Assert.Equal("world", api.GetString("lua_str"));
    }
}
