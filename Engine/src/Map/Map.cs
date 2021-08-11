using System;
using System.Collections.Generic;
using Civ2engine.Terrains;

namespace Civ2engine
{
    public class Map
    {
        public int MapIndex { get; } = 0;
        public int XDim { get; internal set; }
        public int YDim { get; internal set; }
        public int ResourceSeed { get; internal set; }
        public int LocatorXdim { get; private set; }
        public int LocatorYdim { get; private set; }
        public bool MapRevealed { get; set; }
        public int WhichCivsMapShown { get; set; }
        public Tile[,] Tile { get; set; }
        public bool IsValidTileC2(int xC2, int yC2)
        {
            var x = (((xC2 + 2 * XDim) % (2 * XDim)) - yC2 % 2);
            return -1 < x && x < XDim && -1 < yC2 && yC2 < YDim;
        }
        public Tile TileC2(int xC2, int yC2) => Tile[(((xC2 + 2 * XDim) % (2 * XDim)) - yC2 % 2) / 2, yC2]; // Accepts tile coords in civ2-style and returns the correct Tile (you can index beyond E/W borders for drawing round world)
        public bool IsTileVisibleC2(int xC2, int yC2, int civ) => MapRevealed || Tile[( ((xC2 + 2 * XDim) % (2 * XDim)) - yC2 % 2 ) / 2, yC2].Visibility[civ];   // Returns V

        private int _zoom;
        public int Zoom     // -7 (min) ... 8 (max), 0=std.
        {
            get => _zoom;
            set => _zoom = Math.Max(Math.Min(value, 8), -7);
        }
        public int Xpx => 4 * (_zoom + 8);    // Length of 1 map square in X
        public int Ypx => 2 * (_zoom + 8);    // Length of 1 map square in Y
        public int[] StartingClickedXY { get; set; }    // Last tile clicked with your mouse on the map. Gives info where the map should be centered (further calculated in MapPanel).
        public List<IslandDetails> Islands { get; set; }

        /// <summary>
        /// Generate first instance of terrain tiles by importing game data.
        /// </summary>
        /// <param name="data">Game data.</param>
        /// <param name="rules">Game rules.</param>
        public void PopulateTilesFromGameData(GameData data, Rules rules)
        {
            XDim = data.MapXdim;
            YDim = data.MapYdim;
            ResourceSeed = data.MapResourceSeed;
            LocatorXdim = data.MapLocatorXdim;
            LocatorYdim = data.MapLocatorYdim;

            Tile = new Tile[XDim, YDim];
            for (var col = 0; col < XDim; col++)
            {
                for (var row = 0; row < YDim; row++)
                {
                    var terrain = data.MapTerrainType[col, row];
                    Tile[col, row] = new Tile(2 * col + (row % 2), row, rules.Terrains[MapIndex][(int) terrain], ResourceSeed)
                    {
                        River = data.MapRiverPresent[col, row],
                        Resource = data.MapResourcePresent[col, row],
                        //UnitPresent = data.MapUnitPresent[col, row],  // you can find this out yourself
                        //CityPresent = data.MapCityPresent[col, row],  // you can find this out yourself
                        Irrigation = data.MapIrrigationPresent[col, row],
                        Mining = data.MapMiningPresent[col, row],
                        Road = data.MapRoadPresent[col, row],
                        Railroad = data.MapRailroadPresent[col, row],
                        Fortress = data.MapFortressPresent[col, row],
                        Pollution = data.MapPollutionPresent[col, row],
                        Farmland = data.MapFarmlandPresent[col, row],
                        Airbase = data.MapAirbasePresent[col, row],
                        Island = data.MapIslandNo[col, row],
                        Visibility = data.MapVisibilityCivs[col,row]
                    };
                }
            }
        }

        public IEnumerable<Tile> CityRadius(Tile tile, bool nullForInvalid = false)
        {
            var coords = new [] { (tile.X - tile.Odd) / 2, tile.Y };
            var odd = tile.Odd;
            var offsets = new List<int[]>
            {
                    new[] {odd, -1},
                    new[] {1, 0},
                    new[] {odd, 1},
                    new[] {0, 2},
                    new[] {-1+odd, 1},
                    new[] {-1, 0},
                    new[] {-1+odd, -1},
                    new[] {0, -2},
                    new[] {1, -2},
                    new[] {1, 2},
                    new[] {-1, 2},
                    new[] {-1, -2},
                    new[] {odd, -3},
                    new[] {1+odd, -1},
                    new[] {1+odd, 1},
                    new[] {0+odd, 3},
                    new[] {-1+odd, 3},
                    new[] {-2+odd, 1},
                    new[] {-2 + odd, -1},
                    new[] {-1+odd, -3}
                };

            yield return tile;
            foreach (var offset in offsets)
            {
                var x = coords[0] + offset[0];
                var y = coords[1] + offset[1];
                if (x < 0 || x >= XDim || y < 0 || y >= YDim)
                {
                    if (nullForInvalid)
                    {
                        yield return null;
                    }
                }
                else
                {
                    yield return Tile[x, y];
                }
            }
        }


        public IEnumerable<Tile> DirectNeighbours(Tile candidate)
        {
            var evenOdd = candidate.Odd;
            var coords = new [] { (candidate.X - evenOdd) / 2, candidate.Y };
            var offsets = new List<int[]>
            {
                new[] {0 + evenOdd, -1},
                new[] {0 + evenOdd, 1},
                new[] {-1 + evenOdd, 1},
                new[] {-1 + evenOdd, -1}
            };
            foreach (var offset in offsets)
            {
                var x = coords[0] + offset[0];
                var y = coords[1] + offset[1];
                if (x < 0 || x >= XDim || y < 0 || y >= YDim) continue;
                yield return Tile[x, y];
            }
        }
        
        public IEnumerable<Tile> Neighbours(Tile candidate)
        {
            var odd = candidate.Odd;
            var coords = new [] { (candidate.X - odd) / 2, candidate.Y };
            var offsets = new List<int[]>
            {
                new[] {odd, -1},
                new[] {1, 0},
                new[] {odd, 1},
                new[] {0, 2},
                new[] {-1+odd, 1},
                new[] {-1, 0},
                new[] {-1+odd, -1},
                new[] {0, -2},
            };
            foreach (var offset in offsets)
            {
                var x = coords[0] + offset[0];
                var y = coords[1] + offset[1];
                if (x < 0 || x >= XDim || y < 0 || y >= YDim) continue;
                yield return Tile[x, y];
            }
        }

        public void SetAsStartingLocation(Tile tile, int ownerId)
        {
            tile.Visibility[ownerId] = true;
            
            foreach (var radiusTile in CityRadius(tile))
            {
                radiusTile.Visibility[ownerId] = true;
            }
            
            this.AdjustFertilityForCity(tile);
        }
    }
}
