using Civ2engine;
using Model.Core.Cities;

public class CityApi
{
    public CityApi(City city, Game game)
    {
        BaseCity = city;
        _ = game;
    }

    public City BaseCity { get; }
}
