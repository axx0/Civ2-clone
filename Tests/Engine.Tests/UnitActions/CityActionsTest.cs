using Civ2engine;
using Civ2engine.UnitActions;
using Model.Core;
using Moq;

namespace Engine.Tests.UnitActions;

public class CityActionsTest
{
    private static Mock<IGame> SetupGame(int citiesToCreate, out Civilization civ)
    {
        // Given a game
        var game = new Mock<IGame>();
        // with a civilization named "TestCiv"
        civ = new Civilization();
        civ.TribeName = "TestCiv";
        game.Setup(g => g.AllCivilizations).Returns([civ]);
        // with a list of city names
        Dictionary<string, List<string>> cityNames = new Dictionary<string, List<string>>();
        cityNames["TESTCIV"] = ["City1", "City2", "City3"];
        game.Setup(g => g.CityNames).Returns(cityNames);
        // with N cities in the History
        History history = new History(game.Object);
        for (int cityCounter = 0; cityCounter < citiesToCreate; cityCounter++)
        {
            City c = new City();
            c.Name = cityNames["TESTCIV"][cityCounter];
            c.Owner = civ;
            history.CityBuilt(c);
        }
        game.Setup(g => g.History).Returns(history);
        // and N cities built by this civilization in the game data
        Dictionary<Civilization, int> citiesBuiltCount = new Dictionary<Civilization, int>();
        citiesBuiltCount[civ] = citiesToCreate;
        game.Setup(g => g.CitiesBuiltSoFar).Returns(citiesBuiltCount);
        return game;
    }

    [Fact]
    public void GetCityNameForFirstCity()
    {
        // Given a game with zero cities already created
        var game = SetupGame(0, out var civ);
        // Then GetCityName should suggest the first city name
        string suggested = CityActions.GetCityName(civ, game.Object);
        Assert.Equal("City1", suggested);
    }

    [Fact]
    public void GetCityNameForSecondCity()
    {
        // Given a game with one city already created
        var game = SetupGame(1, out var civ);
        // then GetCityName should suggest the second city name
        string suggested = CityActions.GetCityName(civ, game.Object);
        Assert.Equal("City2", suggested);
    }

    [Fact]
    public void GetCityNameWhenCitiesListExhausted()
    {
        // Given a game with three cities already created
        var game = SetupGame(3, out var civ);
        // then GetCityName should suggest Dummy Name
        string suggested = CityActions.GetCityName(civ, game.Object);
        Assert.Equal("Dummy Name", suggested);
    }
}
