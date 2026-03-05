using System.Linq;
using Civ2engine.Scripting.ScriptObjects;
using Neo.IronLua;
using Xunit;

namespace Engine.Tests;

public class CityImprovementTests
{
    [Fact]
    public void CityImprovement_Properties()
    {
        var (game, _, _) = ApiTestHarness.CreateGameAndAi();
        var imp = game.Rules.Improvements.First();
        var api = new CityImprovement(imp);
        
        Assert.Equal(imp.Effects, api.Effects);
    }

    [Fact]
    public void CityImprovement_AddTerrainEffect()
    {
        var (game, _, _) = ApiTestHarness.CreateGameAndAi();
        var imp = game.Rules.Improvements.First();
        var api = new CityImprovement(imp);
        
        using var l = new Lua();
        var g = l.CreateEnvironment();
        g["imp"] = api;
        
        g.DoChunk("imp:addTerrainEffect({ Resource = 1, Value = 2, Terrain = 3 })", "test.lua");
        
        var effect = imp.TerrainEffects.Last();
        Assert.Equal(1, effect.Resource);
        Assert.Equal(2, effect.Value);
        Assert.Equal(3, effect.Terrain);
    }
}
