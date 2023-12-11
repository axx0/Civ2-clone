using System.Collections.Generic;
using System.Linq;
using Civ2engine.Terrains;

namespace Civ2engine.MapObjects;

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

    public bool IsAccurate(Tile real)
    {
        if (real.CityHere == null)
        {
            if (CityHere != null)
            {
                return false;
            }
        }
        else
        {
            if (CityHere == null || CityHere.Name != real.CityHere.Name || CityHere.Size != real.CityHere.Size ||
                CityHere.OwnerId != real.CityHere.OwnerId)
            {
                return false;
            }
        }

        return real.Improvements.All(realImprovement => Improvements.Any(i => i.IsMatch(realImprovement))) &&
               Improvements.All(improvement => real.Improvements.Any(i => i.IsMatch(improvement)));
    }

    public void Update(Tile real)
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
        else
        {
            CityHere = null;
        }

        
        Improvements = real.Improvements.Select(i => new ConstructedImprovement
            { Improvement = i.Improvement, Level = i.Level, Group = i.Group }).ToList();
    }
}