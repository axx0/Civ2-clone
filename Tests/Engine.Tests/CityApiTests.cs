using System.Linq;
using Civ2engine.Scripting.ScriptObjects;
using Neo.IronLua;
using Xunit;

namespace Engine.Tests;

public class CityApiTests
{
    [Fact]
    public void CityApi_Properties()
    {
        var (game, _, civ) = ApiTestHarness.CreateGameAndAi();
        var tile = ApiTestHarness.FindEmptyTile(game);
        var city = ApiTestHarness.CreateCity(game, civ, tile, "Rome");
        var api = new CityApi(city, game);
        
        Assert.Equal(city, api.BaseCity);
    }
}
