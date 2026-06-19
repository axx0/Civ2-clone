using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Model.Core;
using Model.Core.Mapping;

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
                var maps = new Map[(secondaryMaps?.Length ?? 0) + 1];

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
                    
                    mainMap.CalculateFertility(terrains[0]);

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

            var (majorLandmasses, minIslandSize, maxIslandSize, minContinentSize, maxContinentSize) = GetLandmassSettings(config, landRequired);
            var landmassIndex = 0;

            var oceans = new HashSet<Tile>();

            var islands = new List<IslandDetails>();

            while (landUsed < landRequired && remainingTiles.Count > 0)
            {
                var candidate = config.Random.ChooseFrom(remainingTiles);
                remainingTiles.Remove(candidate);
                land.Add(candidate);

                var edgeSet = new HashSet<Tile>();
                var island = new IslandDetails {Tiles = {candidate}, Id = islands.Count + 1};
                candidate.Island = island.Id;
                SetGeneratedLandTerrain(candidate, config, terrains[0], yMax);
                islands.Add(island);
                        
                var size = landmassIndex < majorLandmasses
                    ? config.Random.Next(minContinentSize, maxContinentSize + 1)
                    : config.Random.Next(minIslandSize, maxIslandSize + 1);
                size = Math.Min(size, Math.Max(1, landRequired - landUsed));
                landmassIndex++;

                foreach (var tile in mainMap.DirectNeighbours(candidate))
                {
                    if (tile.Type != TerrainType.Ocean || !remainingTiles.Contains(tile) || tile.Island != -1) continue;
                    edgeSet.Add(tile);
                    remainingTiles.Remove(tile);
                    tile.Island = 0;
                }

                while (island.Tiles.Count < size && edgeSet.Count > 0)
                {
                    var choice = config.Random.ChooseFrom(edgeSet);
                    edgeSet.Remove(choice);
                    island.Tiles.Add(choice);
                    land.Add(choice);

                    choice.Island = island.Id;
                    SetGeneratedLandTerrain(choice, config, terrains[0], yMax);
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
                    foreach (var tile in edgeSet)
                    {
                        oceans.Add(tile);
                        foreach (var neighbour in tile.Neighbours().Where(n =>
                                     n is { Island: -1, Type: TerrainType.Ocean } && remainingTiles.Contains(n) &&
                                     mainMap.DiagonalNeighbours(n).Any(t => t.Type != TerrainType.Ocean)))
                        {
                            neighbour.Island = 0;
                            remainingTiles.Remove(neighbour);
                            oceans.Add(neighbour);
                        }
                    }
                }

                landUsed += island.Tiles.Count;
            }

            if (remainingTiles.Count > 0)
            {
                foreach (var tile in remainingTiles)
                {
                    oceans.Add(tile);
                    tile.Island = 0;
                }
            }
            
            mainMap.Islands = islands;
            mainMap.RenumberIslands();
            mainMap.RenumberOceans(oceans);

            foreach (var tile in land)
            {
                tile.RefreshGoodyHut(mainMap.ResourceSeed);
            }
                    
            mainMap.CalculateFertility(terrains[0]);

            return mainMap;
        }


        private static (int MajorLandmasses, int MinIslandSize, int MaxIslandSize, int MinContinentSize, int MaxContinentSize)
            GetLandmassSettings(GameInitializationConfig config, int landRequired)
        {
            var safeLand = Math.Max(24, landRequired);
            return config.Landform switch
            {
                // Archipelago still gets one moderate starting landmass, but keeps
                // the rest of the world broken into smaller islands.
                0 => (1,
                    4,
                    Math.Max(12, safeLand / 18),
                    Math.Max(24, safeLand / 10),
                    Math.Max(36, safeLand / 6)),

                // Large land mass: Civ2-style continent-heavy maps.
                2 => (3,
                    12,
                    Math.Max(45, safeLand / 12),
                    Math.Max(90, safeLand / 5),
                    Math.Max(150, safeLand / 3)),

                // Normal: bias toward a couple of playable continents instead of
                // many tiny island clusters.
                _ => (2,
                    8,
                    Math.Max(32, safeLand / 14),
                    Math.Max(70, safeLand / 7),
                    Math.Max(120, safeLand / 4))
            };
        }

        private static void SetGeneratedLandTerrain(Tile tile, GameInitializationConfig config, Terrain[] terrains, int yMax)
        {
            tile.Terrain = SelectGeneratedTerrain(tile, config, terrains, yMax);
            tile.River = ShouldPlaceRiver(tile, config);
        }

        private static Terrain SelectGeneratedTerrain(Tile tile, GameInitializationConfig config, Terrain[] terrains, int yMax)
        {
            var latitude = yMax <= 0 ? 0d : Math.Abs((tile.Y / (double)yMax) - 0.5d) * 2d;
            var wetness = config.Climate switch
            {
                0 => -18,
                2 => 18,
                _ => 0
            };
            var warmth = config.Temperature switch
            {
                0 => -18,
                2 => 18,
                _ => 0
            };
            var ageRoughness = config.Age switch
            {
                0 => 18,
                2 => -10,
                _ => 0
            };

            if (!config.FlatWorld && latitude > 0.9d)
            {
                return terrains[(int)(config.Random.Next(100) < 70 - warmth ? TerrainType.Glacier : TerrainType.Tundra)];
            }

            var roll = config.Random.Next(100);

            if (latitude > 0.72d)
            {
                if (roll < 45 - warmth) return terrains[(int)TerrainType.Tundra];
                if (roll < 62 + ageRoughness) return terrains[(int)TerrainType.Hills];
                if (roll < 74 + ageRoughness) return terrains[(int)TerrainType.Mountains];
                if (roll < 86 + wetness) return terrains[(int)TerrainType.Forest];
                return terrains[(int)TerrainType.Plains];
            }

            if (latitude < 0.28d)
            {
                if (roll < 18 + wetness) return terrains[(int)TerrainType.Jungle];
                if (roll < 28 + wetness) return terrains[(int)TerrainType.Swamp];
                if (roll < 42 - wetness + warmth) return terrains[(int)TerrainType.Desert];
                if (roll < 58 + ageRoughness) return terrains[(int)TerrainType.Hills];
                if (roll < 68 + ageRoughness) return terrains[(int)TerrainType.Mountains];
                if (roll < 82) return terrains[(int)TerrainType.Plains];
                return terrains[(int)TerrainType.Grassland];
            }

            if (roll < 10 - wetness + warmth) return terrains[(int)TerrainType.Desert];
            if (roll < 24 + wetness) return terrains[(int)TerrainType.Forest];
            if (roll < 34 + wetness) return terrains[(int)TerrainType.Swamp];
            if (roll < 50 + ageRoughness) return terrains[(int)TerrainType.Hills];
            if (roll < 60 + ageRoughness) return terrains[(int)TerrainType.Mountains];
            if (roll < 78) return terrains[(int)TerrainType.Plains];
            return terrains[(int)TerrainType.Grassland];
        }

        private static bool ShouldPlaceRiver(Tile tile, GameInitializationConfig config)
        {
            if (tile.Type is TerrainType.Ocean or TerrainType.Glacier or TerrainType.Mountains)
            {
                return false;
            }

            var riverChance = config.Climate switch
            {
                0 => 4,
                2 => 13,
                _ => 8
            };

            if (tile.Type is TerrainType.Jungle or TerrainType.Swamp or TerrainType.Grassland)
            {
                riverChance += 4;
            }

            return config.Random.Next(100) < riverChance;
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
