using System.Linq;
using Civ2engine.Scripting.ScriptObjects;
using Neo.IronLua;
using Xunit;

namespace Engine.Tests;

public class UnitTypeTests
{
    [Fact]
    public void UnitType_Properties()
    {
        var (game, _, _) = ApiTestHarness.CreateGameAndAi();
        var def = game.Rules.UnitTypes.First();
        var api = new UnitType(def, game);
        
        Assert.Equal(def.Name, api.name);
        Assert.Equal(def.Attack, api.attack);
        Assert.Equal(def.Defense, api.defense);
        Assert.Equal(def.Cost, api.cost);
    }

    [Fact]
    public void UnitType_Lua_Access()
    {
        var (game, _, _) = ApiTestHarness.CreateGameAndAi();
        var def = game.Rules.UnitTypes.First();
        def.Name = "Phalanx";
        var api = new UnitType(def, game);
        
        using var l = new Lua();
        var g = l.CreateEnvironment();
        g["ut"] = api;
        
        var result = g.DoChunk("return ut.name, ut.attack", "test.lua");
        Assert.Equal("Phalanx", (string)result[0]);
        Assert.Equal(def.Attack, (int)result[1]);
    }
}
