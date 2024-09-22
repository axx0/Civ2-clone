using Civ2engine.MapObjects;
using Civ2engine.Terrains;

namespace Model.Core.Mapping;


public class PlayerTile
{
    public PlayerTile(Tile real)
    {
        if (real.CityHere != null)
        {
            CityHere = new CityInfo
            {
                Name = real.CityHere.Name,
                Size = real.CityHere.Size,
                OwnerId = real.CityHere.OwnerId
            };
        }


        Improvements = real.Improvements.Select(i => new ConstructedImprovement
            { Improvement = i.Improvement, Level = i.Level, Group = i.Group }).ToList();

    }

    public PlayerTile()
    {
        Improvements = new List<ConstructedImprovement>();
    }

    public CityInfo? CityHere { get; set; }
    public List<ConstructedImprovement> Improvements { get; set; }
}

