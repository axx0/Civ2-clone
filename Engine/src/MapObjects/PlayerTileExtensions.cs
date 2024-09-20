using System.Collections.Generic;
using System.Linq;
using Civ2engine.Terrains;
using Model.Core.Mapping;

namespace Civ2engine.MapObjects;

public static class PlayerTileExtensions
{
    public static bool IsAccurate(this PlayerTile tile, Tile real)
    {
        if (real.CityHere == null)
        {
            if (tile.CityHere != null)
            {
                return false;
            }
        }
        else
        {
            if (tile.CityHere == null || tile.CityHere.Name != real.CityHere.Name || tile.CityHere.Size != real.CityHere.Size ||
                tile.CityHere.OwnerId != real.CityHere.OwnerId)
            {
                return false;
            }
        }

        return real.Improvements.All(realImprovement => tile.Improvements.Any(i => i.IsMatch(realImprovement))) &&
               tile.Improvements.All(improvement => real.Improvements.Any(i => i.IsMatch(improvement)));
    }

    public static void Update(this PlayerTile tile,Tile real)
    {
        if (real.CityHere != null)
        {
            tile.CityHere = new CityInfo
            {
                Name = real.CityHere.Name,
                Size = real.CityHere.Size,
                OwnerId = real.CityHere.OwnerId
            };
        }
        else
        {
            tile.CityHere = null;
        }

        
        tile.Improvements = real.Improvements.Select(i => new ConstructedImprovement
            { Improvement = i.Improvement, Level = i.Level, Group = i.Group }).ToList();
    }
}