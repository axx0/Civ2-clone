using System.Linq;
using Civ2engine.Scripting;
using Civ2engine.Scripting.ScriptObjects;
using Neo.IronLua;
using Xunit;

namespace Engine.Tests;

public class TribeTests
{
    [Fact]
    public void Tribe_Properties()
    {
        var (game, _, civ) = ApiTestHarness.CreateGameAndAi();
        var tribe = new Tribe(civ);
        
        Assert.Equal(civ.Id, tribe.Id);
        Assert.Equal(civ, tribe.Civ);
    }

    [Fact]
    public void Tribe_Lua_Access()
    {
        var (game, _, civ) = ApiTestHarness.CreateGameAndAi();
        civ.TribeName = "Romans";
        var tribe = new Tribe(civ);
        
        using var l = new Lua();
        var g = l.CreateEnvironment();
        g["tribe"] = tribe;
        
        var result = g.DoChunk("return tribe.Civ.TribeName", "test.lua");
        Assert.Equal("Romans", (string)result[0]);
    }
}
