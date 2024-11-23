using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Model.Core;

namespace Civ2engine
{
    public class MapGenerator
    {
        public static Task<Map[]> GenerateMap(GameInitializationConfig config)
        {
            return Task.Run(() =>
            {
                var secondaryMaps = config.Rules.Maps;
                var area = config.WorldSize;
                var maps = new Map[secondaryMaps?.Length ?? 0 + 1];

                var width = area[0];
                var height = area[1];

                if (config.TerrainData != null)
                {
                    var mainMap = new Map(config.FlatWorld, 0)
                    {
                    
                        XDim = width,
                        YDim = height,
                        ResourceSeed = config.ResourceSeed ?? config.Random.Next(64),
                        Tile = new Tile[width, height]
                    };
                    var terrains = config.Rules.Terrains;
                    var index = 0;


                    var yMax = mainMap.Tile.GetLength(1) - 1;
                    var land = new List<Tile>();
                    for (int y = 0; y < mainMap.Tile.GetLength(1); y++)
                    {
                        var odd = y % 2;
                        for (int x = 0; x < mainMap.Tile.GetLength(0); x++)
                        {
                            var terra = config.TerrainData[index++];
                            var tile = new Tile(2 * x + odd, y, terrains[0][terra & 0xF], mainMap.ResourceSeed, mainMap, x,
                                new bool[config.NumberOfCivs + 1])
                            {
                                River = terra > 100
                            };
                            if (tile.Type != TerrainType.Ocean)
                            {
                                land.Add(tile);
                            }

                            mainMap.Tile[x, y] = tile;
                        }
                    }

                    if (!config.FlatWorld)
                    {
                        var ocean = terrains[0][(int) TerrainType.Ocean];
                        var arctic = terrains[0][(int) TerrainType.Glacier];
                        for (int x = 0; x < mainMap.Tile.GetLength(0); x++)
                        {
                            if (mainMap.Tile[x, 0].Terrain == ocean)
                            {
                                mainMap.Tile[x, 0].Terrain = arctic;
                            }

                            if (mainMap.Tile[x, yMax].Terrain == ocean)
                            {
                                mainMap.Tile[x, yMax].Terrain = arctic;
                            }
                        }
                    }
                    
                    mainMap.NormalizeIslands();
                    
                    mainMap.CalculateFertility(terrains[0], land);

                    maps[0] = mainMap;
                }

                for (int i = 0; i < maps.Length; i++)
                {
                    if (maps[i] == null)
                    {
                        maps[i] = GenerateMap(config, width, height, area);
                    }
                }

                return maps;
            });
        }

        private static Map GenerateMap(GameInitializationConfig config, int width, int height, int[] area)
        {
            var mainMap = new Map(config.FlatWorld, 0)
            {
                    
                XDim = width,
                YDim = height,
                ResourceSeed = config.ResourceSeed ?? config.Random.Next(64),
                Tile = new Tile[width, height]
            };
            var terrains = config.Rules.Terrains;
            var index = 0;


            var yMax = mainMap.Tile.GetLength(1) - 1;
            var land = new List<Tile>();
            var remainingTiles = new HashSet<Tile>();
            var ocean = terrains[0][(int) TerrainType.Ocean];
            var arctic = terrains[0][(int) TerrainType.Glacier];
            for (int y = 0; y < mainMap.Tile.GetLength(1); y++)
            {
                var odd = y % 2;
                var defaultTerrain = config.FlatWorld || (y > 0 && y < yMax) ? ocean : arctic;
                for (int x = 0; x < mainMap.Tile.GetLength(0); x++)
                {
                    var tile = new Tile(2 * x + odd, y, defaultTerrain, mainMap.ResourceSeed, mainMap, x, 
                        new bool[config.NumberOfCivs + 1])
                    {
                        Island = -1
                    };
                    remainingTiles.Add(tile);
                    mainMap.Tile[x, y] = tile;
                }
            }

            var landRequired = config.PropLand switch
            {
                0 => (area[0] * area[1]) / 6,
                1 => (area[0] * area[1]) / 4,
                _ => (area[0] * area[1]) / 2
            };

            var landUsed = 0;

            var minIslandSize = 3;
                    
            var maxIslandSize = 30;

            var grassland = terrains[0][(int) TerrainType.Grassland];

            var continents = 0;

            var oceans = new List<Tile>();

            var islands = new List<IslandDetails>();

            while (landUsed < landRequired && remainingTiles.Count > 0)
            {
                var candidate = config.Random.ChooseFrom<Tile>(remainingTiles);
                remainingTiles.Remove(candidate);
                land.Add(candidate);

                var edgeSet = new HashSet<Tile>();
                candidate.Terrain = grassland;
                var island = new IslandDetails {Tiles = {candidate}};
                islands.Add(island);
                        
                var size = config.Random.Next(minIslandSize, maxIslandSize);

                foreach (var tile in mainMap.DirectNeighbours(candidate))
                {
                    if (tile.Island != -1) continue;
                    edgeSet.Add(tile);
                    remainingTiles.Remove(tile);
                    tile.Island = 0;
                }

                while (island.Tiles.Count < size && edgeSet.Count > 0)
                {
                    var choice = config.Random.ChooseFrom(edgeSet);
                    island.Tiles.Add(choice);
                    land.Add(choice);

                    choice.Island = candidate.Island;
                    choice.Terrain = grassland;
                    foreach (var tile in mainMap.DirectNeighbours(choice))
                    {
                        if (tile.Island != -1 || !remainingTiles.Contains(tile)) continue;
                        edgeSet.Add(tile);
                        remainingTiles.Remove(tile);
                        tile.Island = 0;
                    }
                }

                if (edgeSet.Count > 0)
                {
                    oceans.AddRange(edgeSet);

                    foreach (var neighbour in edgeSet.SelectMany(tile =>
                                 mainMap.DirectNeighbours(tile).Where(n =>
                                     n.Island == -1 && mainMap.Neighbours(n).Any(t => t.Type != TerrainType.Ocean))))
                    {
                        remainingTiles.Remove(neighbour);
                        neighbour.Island = 0;
                        oceans.Add(neighbour);
                    }
                }

                landUsed += island.Tiles.Count;
            }
                    
            oceans.AddRange(remainingTiles);

            mainMap.Islands = islands;
            mainMap.RenumberIslands();
            mainMap.RenumberOceans(oceans);
                    
            mainMap.CalculateFertility(terrains[0], land);

            return mainMap;
        }
    }
}

//                  function flood_Generator(data: MapData, options: { [key: string]: string }, mapRandom: FastRandom): void {
//
//     const avaliableLand: Set<Tile> = new Set(createTiles(data, mapRandom));
//
//     const halfSize = data.height / 2;
//
//     data.regions.push({ name: "Open Sea", locations: [] })
//
//     let landUsed = 0
//     const landRequired = avaliableLand.size / 4;
//
//     let continents = 0
//
//     const minIslandSize = 3;
//
//     const maxIslandSize = 300;
//
//     while (landUsed < landRequired || avaliableLand.size > 0) {
//         const candidate: Tile = mapRandom.take(avaliableLand);
//
//         const coastSet: Set<Tile> = new Set()
//         const edgeSet: Set<Tile> = new Set();
//         candidate.continent = continents++;
//         candidate.terrain = selectTerrain(candidate, halfSize, mapRandom);
//         candidate.modifiers = selectModifier(candidate, mapRandom);
//         const islandTiles = [candidate];
//
//         const size = mapRandom.nextRange(minIslandSize, maxIslandSize)
//         let [minX, maxX, minY, maxY] = [candidate.x, candidate.x, candidate.y, candidate.y];
//
//         let xRange = (maxX - minX + 1) * 2
//         let [minYLim, maxYLim] = [minY - xRange, maxY + xRange ]
//
//         let yRange = (maxY - minY + 1) * 2
//         let [minXLim, maxXLim] = [minX - yRange, maxX + yRange]
//
//         for (const tile of neighbours(candidate, data)) {
//             if (tile && tile.continent === -1) {
//                 edgeSet.add(tile)
//                 avaliableLand.delete(tile)
//             }
//         }
//         while (islandTiles.length < size && edgeSet.size > 0) {
//             const choice = mapRandom.take(edgeSet)
//             islandTiles.push(choice)
//             if (choice.x < minX) {
//                 minX = choice.x
//                 xRange = (maxX - minX + 1) * 2;
//                 [minYLim, maxYLim] = [minY - xRange, maxY + xRange]
//             } else if (choice.x > maxX) {
//                 maxX = choice.x
//                 xRange = (maxX - minX + 1) * 2;
//                 [minYLim, maxYLim] = [minY - xRange, maxY + xRange]
//             } else if (choice.y < minY) {
//                 minY = choice.y
//                 yRange = (maxY - minY + 1) * 2;
//                 [minXLim, maxXLim] = [minX - yRange, maxX + yRange];
//             } else if (choice.y > maxY) {
//                 maxY = choice.y;
//                 yRange = (maxY - minY + 1) * 2;
//                 [minXLim, maxXLim] = [minX - yRange, maxX + yRange];
//             }
//             choice.continent = candidate.continent;
//             choice.terrain = selectTerrain(choice, halfSize, mapRandom);
//             choice.modifiers = selectModifier(choice, mapRandom);
//             for (const tile of neighbours(choice, data)) {
//                 if (tile && tile.continent === -1) {
//                     if (tile.y > minYLim && tile.y < maxYLim && tile.x < maxXLim && tile.x > minXLim) {
//                         edgeSet.add(tile);
//                     } else {
//                         coastSet.add(tile);
//                     }
//                     tile.continent = 0;
//                     avaliableLand.delete(tile);                    
//                 }
//             }
//         }
//         
//         // Add back reserved coast tiles
//         for (let coast of coastSet) {
//             edgeSet.add(coast)
//         }
//
//         if (islandTiles.length < minIslandSize) {
//             continents--;
//             for (let tile of islandTiles) {
//                 tile.continent = 0;
//                 tile.terrain = Ocean;
//                 tile.modifiers = []
//             }
//         } else {
//
//             //fill tiny lakes
//             for (let tile of edgeSet) {
//                 let scene = 0
//                 let neighbour: Tile | null = null
//                 for (let n of neighbours(tile, data)) {
//                     if (!n) continue
//                     if (n.terrain === Ocean) {
//                         neighbour = null;
//                         break;
//                     }
//                     scene++
//                     if (mapRandom.nextFloat() < 1 / scene) {
//                         neighbour = n;
//                     }
//                 }
//                 if (neighbour !== null) {
//                     tile.terrain = neighbour.terrain;
//                     tile.continent = neighbour.continent;
//                     islandTiles.push(tile)
//                     edgeSet.delete(tile)
//                 }
//             }
//         }
//         landUsed += islandTiles.length;
//
//         //reserve edge tiles
//         let extraEdge = islandTiles.length * 3 - edgeSet.size
//         while (extraEdge > 0 && edgeSet.size > 0) {
//             const tile = mapRandom.take(edgeSet)
//             for (let n of neighbours(tile, data)) {                
//                 if (n && n.continent === -1) {
//                     edgeSet.add(n);
//                     n.continent = 0
//                     avaliableLand.delete(n)
//                     extraEdge -= 1;
//                 }
//             }
//         }
//     }
// }