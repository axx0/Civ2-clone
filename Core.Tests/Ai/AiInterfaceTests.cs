using Civ2engine.Scripting;
using Core.Tests.Scripting.ScriptObjects;
using Neo.IronLua;

namespace Core.Tests.Ai;

public class AiInterfaceTests
{
    [Fact]
    public void AiInterface_BasicProperties()
    {
        var (game, aiPlayer, civ) = ApiTestHarness.CreateGameAndAi();
        var api = aiPlayer.AI;

        Assert.Equal(civ, api.civ);
        Assert.Equal(aiPlayer.DifficultyLevel, api.difficulty);
    }

    [Fact]
    public void AiInterface_GetNearestCity()
    {
        var (game, aiPlayer, civ) = ApiTestHarness.CreateGameAndAi();
        var api = aiPlayer.AI;
        var tile = ApiTestHarness.FindEmptyTile(game);
        var city = ApiTestHarness.CreateCity(game, civ, tile, "Rome");

        var nearest = api.GetNearestCity(new TileApi(tile, game));
        Assert.Equal(city, nearest);
    }

    [Fact]
    public void AiInterface_RandomTile()
    {
        var (game, aiPlayer, _) = ApiTestHarness.CreateGameAndAi();
        var api = aiPlayer.AI;

        var tile = api.RandomTile(new LuaTable { { "global", true } });
        Assert.NotNull(tile);
        Assert.IsType<TileApi>(tile);
    }

    [Fact]
    public void AiInterface_RegisterAndCallEvent()
    {
        var (game, aiPlayer, _) = ApiTestHarness.CreateGameAndAi();
        var api = aiPlayer.AI;

        bool called = false;
        api.RegisterEvent("test_event", (ai, args) =>
        {
            called = (bool)args["val"];
            return "success";
        });

        var result = api.Call("test_event", new LuaTable { { "val", true } });

        Assert.True(called);
        Assert.Equal("success", result);
    }
}
