using Civ2engine;
using Model.Core.Cities;

public class CityApi(City city, Game game)
{
    public City BaseCity { get; } = city;
}