using Civ2engine.Scripting;
using Neo.IronLua;

namespace Core.Tests.Scripting.ScriptObjects;

public class TechTests
{
    [Fact]
    public void Tech_Properties()
    {
        var (game, _, _) = ApiTestHarness.CreateGameAndAi();
        var advances = game.Rules.Advances;
        var tech = new Tech(advances, 0);

        Assert.Equal(advances[0].Name, tech.name);
        Assert.Equal(advances[0].Index, tech.id);
    }

    [Fact]
    public void Tech_Lua_Access()
    {
        var (game, _, _) = ApiTestHarness.CreateGameAndAi();
        var advances = game.Rules.Advances;
        advances[0].Name = "Pottery";
        var tech = new Tech(advances, 0);

        using var l = new Lua();
        var g = l.CreateEnvironment();
        g["tech"] = tech;

        var result = g.DoChunk("return tech.name, tech.id", "test.lua");
        Assert.Equal("Pottery", (string)result[0]);
        Assert.Equal(advances[0].Index, (int)result[1]);
    }
}