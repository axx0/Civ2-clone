using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Model.Core;

namespace Civ2engine.MapObjects
{
    public static class Islands
    {
        public static void NormalizeIslands(this Map map)
        {
            var allTiles = new HashSet<Tile>();

            for (var y = 0; y < map.Tile.GetLength(1); y++)
            {
                for (var x = 0; x < map.Tile.GetLength(0); x++)
                {
                    allTiles.Add(map.Tile[x, y]);
                }
            }

            var oceans = new List<IslandDetails>();
            var lands = new List<IslandDetails>();
            while (allTiles.Count > 0)
            {
                var aTile = allTiles.FirstOrDefault();
                var island = new IslandDetails
                {
                    Tiles = {aTile}
                };
                Func<Tile, bool> comparator;
                if (aTile.Type == TerrainType.Ocean)
                {
                    oceans.Add(island);
                    comparator = t => t.Type == TerrainType.Ocean;
                }
                else
                {
                    lands.Add(island);
                    comparator = t => t.Type != TerrainType.Ocean;
                }

                var edgeSet = new HashSet<Tile> {aTile};
                allTiles.Remove(aTile);
                while (edgeSet.Count > 0)
                {
                    var current = edgeSet.FirstOrDefault();
                    edgeSet.Remove(current);
                    foreach (var tile in map.Neighbours(current).Where(t => comparator(t) && allTiles.Contains(t)))
                    {
                        edgeSet.Add(tile);
                        allTiles.Remove(tile);
                        island.Tiles.Add(tile);
                    }
                }
            }

            map.Islands = lands;
            map.RenumberIslands();
            map.RenumberOceans(oceans);
        }

        public static void RenumberIslands(this Map map)
        {
            map.Islands = map.Islands.OrderByDescending(i => i.Tiles.Count).ToList();
            for (int i = 0; i < map.Islands.Count; i++)
            {
                var island = map.Islands[i];
                island.Id = i + 1;
                island.Tiles.ForEach(tile =>
                {
                    tile.Island = island.Id;
                });
            }
        }

        public static void RenumberOceans(this Map map, List<Tile> oceanTiles)
        {
            var allTiles = new HashSet<Tile>(oceanTiles);

            var oceans = new List<IslandDetails>();
            while (allTiles.Count > 0)
            {
                var aTile = allTiles.FirstOrDefault();
                var island = new IslandDetails
                {
                    Tiles = {aTile}
                };
                oceans.Add(island);
                    

                var edgeSet = new HashSet<Tile> {aTile};
                allTiles.Remove(aTile);
                while (edgeSet.Count > 0)
                {
                    var current = edgeSet.FirstOrDefault();
                    edgeSet.Remove(current);
                    foreach (var tile in map.Neighbours(current).Where(t => allTiles.Contains(t)))
                    {
                        edgeSet.Add(tile);
                        allTiles.Remove(tile);
                        island.Tiles.Add(tile);
                    }
                }
            }
            map.RenumberOceans(oceans);
        }
        
        public static void RenumberOceans(this Map map, IEnumerable<IslandDetails> oceans)
        {
            if (!oceans.Any()) return;

            var orderedOceans = oceans.OrderByDescending(i => i.Tiles.Count).ToList();

            orderedOceans[0].Id = 0;

            var lastLand = map.Islands.Max(i=>i.Id);
            
            for (var i = 1; i < orderedOceans.Count; i++)
            {
                var island = orderedOceans[i];
                island.Id = lastLand + i;
            }
        }
    }
}