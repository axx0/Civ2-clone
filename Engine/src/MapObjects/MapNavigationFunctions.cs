using System.Collections.Generic;
using System.Linq;

namespace Civ2engine.MapObjects;

public static class MapNavigationFunctions
{
    public static IEnumerable<Tile?> CityRadius(this Map map, Tile tile, bool nullForInvalid = false)
    {
        var odd = tile.Odd;
        var offsets = new List<int[]>
        {
            new[] { 0, -2 },
            new[] { -1 + odd, -1 },
            new[] { -1, 0 },
            new[] { -1 + odd, 1 },
            new[] { 0, 2 },
            new[] { odd, 1 },
            new[] { 1, 0 },
            new[] { odd, -1 },
            new[] { odd, 3 },
            new[] { 1 + odd, 1 },
            new[] { 1 + odd, -1 },
            new[] { odd, -3 },
            new[] { -1, -2 },
            new[] { -1, 2 },
            new[] { 1, 2 },
            new[] { 1, -2 },
            new[] { 0, 0 },
            new[] { -1 + odd, -3 },
            new[] { -2 + odd, -1 },
            new[] { -2 + odd, 1 },
            new[] { -1 + odd, 3 },
        };

        return TilesAround(map, tile, offsets, nullForInvalid);
    }


    public static IEnumerable<Tile> DirectNeighbours(this Map map, Tile candidate, bool nullForInvalid = false)
    {
        var evenOdd = candidate.Odd;
        var offsets = new List<int[]>
        {
            new[] { 0 + evenOdd, -1 }, //0
            new[] { 0 + evenOdd, 1 }, //1
            new[] { -1 + evenOdd, 1 }, //2
            new[] { -1 + evenOdd, -1 } //3

            //     new[] {-1 + evenOdd, -1}, //3
            // new[] {0 + evenOdd, -1}, //1
            // new[] {-1 + evenOdd, 1}, //2
            // new[] {0 + evenOdd, 1}, //0
        };
        return TilesAround(map, candidate, offsets, nullForInvalid);
    }

    public static IEnumerable<Tile> Neighbours(this Map map, Tile candidate, bool twoSpaces = false,
        bool nullForInvalid = false)
    {
        var odd = candidate.Odd;
        var offsets = new List<int[]>
        {
            new[] { odd, -1 },
            new[] { 1, 0 },
            new[] { odd, 1 },
            new[] { 0, 2 },
            new[] { -1 + odd, 1 },
            new[] { -1, 0 },
            new[] { -1 + odd, -1 },
            new[] { 0, -2 },
        };

        if (twoSpaces)
        {
            var extraOffsets = new List<int[]>
            {
                new[] { odd, -3 },
                new[] { 1, -2 },
                new[] { 1 + odd, -1 },
                new[] { 2, 0 },
                new[] { 1 + odd, 1 },
                new[] { 1, 2 },
                new[] { odd, 3 },
                new[] { 0, 4 },
                new[] { odd - 1, 3 },
                new[] { -1, 2 },
                new[] { -2 + odd, 1 },
                new[] { -2, 0 },
                new[] { -2 + odd, -1 },
                new[] { -1, -2 },
                new[] { odd - 1, -3 },
                new[] { 0, -4 },
            };
            offsets.AddRange(extraOffsets);
        }

        return TilesAround(map, candidate, offsets, nullForInvalid);
    }

    public static IEnumerable<Tile> SecondRing(this Map map, Tile candidate, bool nullForInvalid = false)
    {
        
        var odd = candidate.Odd;
        var offsets = new List<int[]>
        {
            new[] { odd, -3 },
            new[] { 1, -2 },
            new[] { 1 + odd, -1 },
            new[] { 2, 0 },
            new[] { 1 + odd, 1 },
            new[] { 1, 2 },
            new[] { odd, 3 },
            new[] { 0, 4 },
            new[] { odd - 1, 3 },
            new[] { -1, 2 },
            new[] { -2 + odd, 1 },
            new[] { -2, 0 },
            new[] { -2 + odd, -1 },
            new[] { -1, -2 },
            new[] { odd - 1, -3 },
            new[] { 0, -4 },
        };
        
        return TilesAround(map, candidate, offsets, nullForInvalid);
    }

    private static IEnumerable<Tile> TilesAround(Map map, Tile centre, IEnumerable<int[]> offsets,
        bool nullForInvalid = false)
    {
        var coords = new[] { (centre.X - centre.Odd) / 2, centre.Y };
        foreach (var offset in offsets)
        {
            var x = coords[0] + offset[0];
            var y = coords[1] + offset[1];

            if (y < 0 || y >= map.YDim)
            {
                if (nullForInvalid)
                {
                    yield return null;
                }

                continue;
            }

            if (x < 0 || x >= map.XDim)
            {
                if (map.Flat)
                {
                    if (nullForInvalid)
                    {
                        yield return null;
                    }

                    continue;
                }

                if (x < 0)
                {
                    x += map.XDim;
                }
                else
                {
                    x -= map.XDim;
                }
            }

            yield return map.Tile[x, y];
        }
    }

    public static void SetAsStartingLocation(this Map map, Tile tile, int ownerId)
    {
        tile.SetVisible(ownerId);

        foreach (var radiusTile in CityRadius(map,tile))
        {
            radiusTile.SetVisible(ownerId);
        }

        map.AdjustFertilityForCity(tile);
    }

    public static bool IsCurrentlyVisible(this Map map, Tile tile, int toWho)
    {
        return map.MapRevealed || tile.UnitsHere.Any(u => u.Owner.Id == toWho) ||
               Neighbours(map,tile).Any(l => l.UnitsHere.Any(u => u.Owner.Id == toWho)) ||
               CityRadius(map,tile)
                   .Any(t => t.CityHere != null && t.CityHere.Owner.Id == toWho);
    }
}