using Civ2engine;
using Civ2engine.Scripting;
using Civ2engine.Scripting.ScriptObjects;
using Neo.IronLua;

namespace Core.Tests.Scripting.ScriptObjects;

public class TribeTests
{
    [Fact]
    public void Tribe_Properties()
    {
        var (game, _, civ) = ApiTestHarness.CreateGameAndAi();
        var tribe = new Tribe(civ, game);

        Assert.Equal(civ.Id, tribe.id);
        Assert.Equal(civ, tribe.Civ);
    }

    [Fact]
    public void Tribe_Lua_Access()
    {
        var (game, _, civ) = ApiTestHarness.CreateGameAndAi();
        civ.TribeName = "Romans";
        var tribe = new Tribe(civ, game);

        using var l = new Lua();
        var g = l.CreateEnvironment();
        g["tribe"] = tribe;

        var result = g.DoChunk("return tribe.name", "test.lua");
        Assert.Equal("Romans", (string)result[0]);
    }

    [Fact]
    public void Tribe_ExtendedProperties()
    {
        var (game, _, civ) = ApiTestHarness.CreateGameAndAi();
        var tribe = new Tribe(civ, game);

        // Test basic properties
        civ.Alive = true;
        Assert.True(tribe.active);

        civ.Adjective = "Roman";
        Assert.Equal("Roman", tribe.adjective);
        tribe.adjective = "Greek";
        Assert.Equal("Greek", civ.Adjective);

        civ.Government = 2;
        Assert.Equal(2, tribe.government);
        tribe.government = 3;
        Assert.Equal(3, civ.Government);

        civ.Money = 1000;
        Assert.Equal(1000, tribe.money);
        tribe.money = 500;
        Assert.Equal(500, civ.Money);

        civ.TribeName = "Greeks";
        Assert.Equal("Greeks", tribe.name);
        tribe.name = "Romans";
        Assert.Equal("Romans", civ.TribeName);

        civ.Science = 200;
        Assert.Equal(200, tribe.researchProgress);
        tribe.researchProgress = 250;
        Assert.Equal(250, civ.Science);

        civ.ScienceRate = 40;
        Assert.Equal(40, tribe.scienceRate);

        civ.TaxRate = 30;
        Assert.Equal(30, tribe.taxRate);

        Assert.NotNull(tribe.leader);
        Assert.Equal(civ.LeaderName, tribe.leader.name);

        Assert.NotNull(tribe.spaceship);
    }

    [Fact]
    public void Tribe_Betrayals()
    {
        var (game, _, civ) = ApiTestHarness.CreateGameAndAi();
        var tribe = new Tribe(civ, game);

        civ.Betrayals = 5;
        Assert.Equal(5, tribe.betrayals);

        tribe.betrayals = 3;
        Assert.Equal(3, civ.Betrayals);

        // Test negative values are clamped to 0
        tribe.betrayals = -1;
        Assert.Equal(0, civ.Betrayals);
    }

    [Fact]
    public void Tribe_FutureTechs()
    {
        var (game, _, civ) = ApiTestHarness.CreateGameAndAi();
        var tribe = new Tribe(civ, game);

        civ.FutureTechCount = 10;
        Assert.Equal(10, tribe.futureTechs);

        tribe.futureTechs = 15;
        Assert.Equal(15, civ.FutureTechCount);

        // Test negative values are clamped to 0
        tribe.futureTechs = -5;
        Assert.Equal(0, civ.FutureTechCount);
    }

    [Fact]
    public void Tribe_IsHuman()
    {
        var (game, _, civ) = ApiTestHarness.CreateGameAndAi();
        var tribe = new Tribe(civ, game);

        civ.PlayerType = PlayerType.Local;
        Assert.True(tribe.isHuman);

        civ.PlayerType = PlayerType.Remote;
        Assert.True(tribe.isHuman);

        civ.PlayerType = PlayerType.Ai;
        Assert.False(tribe.isHuman);

        tribe.isHuman = true;
        Assert.Equal(PlayerType.Local, civ.PlayerType);

        tribe.isHuman = false;
        Assert.Equal(PlayerType.Ai, civ.PlayerType);
    }

    [Fact]
    public void Tribe_Counts()
    {
        var (game, _, civ) = ApiTestHarness.CreateGameAndAi();
        var tribe = new Tribe(civ, game);

        Assert.Equal(civ.Cities.Count, tribe.numCities);
        Assert.Equal(civ.Units.Count, tribe.numUnits);
        Assert.Equal(civ.Advances.Count(a => a), tribe.numTechs);
    }

    [Fact]
    public void Tribe_Researching()
    {
        var (game, _, civ) = ApiTestHarness.CreateGameAndAi();
        var tribe = new Tribe(civ, game);

        // Test no research
        civ.ReseachingAdvance = -1;
        Assert.Null(tribe.researching);

        // Test researching a tech
        civ.ReseachingAdvance = 0;
        Assert.NotNull(tribe.researching);
        Assert.Equal(0, tribe.researching.id);

        // Test setting research
        tribe.researching = new Tech(game.Rules.Advances, 1);
        Assert.Equal(1, civ.ReseachingAdvance);

        // Test clearing research
        tribe.researching = null;
        Assert.Equal(-1, civ.ReseachingAdvance);
    }

    [Fact]
    public void Tribe_ResearchCost()
    {
        var (game, _, civ) = ApiTestHarness.CreateGameAndAi();
        var tribe = new Tribe(civ, game);

        // Research cost should be calculated
        var cost = tribe.researchCost;
        Assert.True(cost > 0);
    }

    [Fact]
    public void Tribe_Patience()
    {
        var (game, _, civ) = ApiTestHarness.CreateGameAndAi();
        var tribe = new Tribe(civ, game);

        civ.Patience = 7;
        Assert.Equal(7, tribe.patience);

        tribe.patience = 5;
        Assert.Equal(5, civ.Patience);
    }

    [Fact]
    public void Tribe_Proxies()
    {
        var (game, _, civ) = ApiTestHarness.CreateGameAndAi();
        var tribe = new Tribe(civ, game);

        Assert.NotNull(tribe.attitude);
        Assert.NotNull(tribe.reputation);
        Assert.NotNull(tribe.treaties);
    }
}