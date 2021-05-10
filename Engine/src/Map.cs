using System;
using Civ2engine.Enums;
using Civ2engine.Terrains;

namespace Civ2engine
{
    public class Map : BaseInstance
    {
        public int Xdim { get; private set; }
        public int Ydim { get; private set; }
        public int Area { get; private set; }
        public int ResourceSeed { get; private set; }
        public int LocatorXdim { get; private set; }
        public int LocatorYdim { get; private set; }
        public bool MapRevealed { get; set; }
        public int WhichCivsMapShown { get; set; }
        public ITerrain[,] Tile { get; set; }
        public bool[,][] Visibility { get; set; } // Visibility of tiles for each civ
        
        public bool IsValidTile(int xC2, int yC2)
        {
            var x = (((xC2 + 2 * Xdim) % (2 * Xdim)) - yC2 % 2);
            return -1 < x && x < Xdim && -1 < yC2 && yC2 < Ydim;
        }
        public ITerrain TileC2(int xC2, int yC2) => Tile[(((xC2 + 2 * Xdim) % (2 * Xdim)) - yC2 % 2) / 2, yC2]; // Accepts tile coords in civ2-style and returns the correct Tile (you can index beyond E/W borders for drawing round world)
        public bool IsTileVisibleC2(int xC2, int yC2, int civ) => Visibility[( ((xC2 + 2 * Xdim) % (2 * Xdim)) - yC2 % 2 ) / 2, yC2][civ];   // Returns Visibility for civ2-style coords (you can index beyond E/W borders for drawing round world)
        public bool TileHasEnemyUnit(int xC2, int yC2, UnitType unitType) => (Game.UnitsHere(xC2, yC2).Count == 1) && (Game.UnitsHere(xC2, yC2)[0].Type == unitType);

        private int _zoom;
        public int Zoom     // -7 (min) ... 8 (max), 0=std.
        {
            get { return _zoom; }
            set
            {
                _zoom = Math.Max(Math.Min(value, 8), -7);
            }
        }
        public int Xpx => 4 * (_zoom + 8);    // Length of 1 map square in X
        public int Ypx => 2 * (_zoom + 8);    // Length of 1 map square in Y
        public int[] StartingClickedXY { get; set; }    // Last tile clicked with your mouse on the map. Gives info where the map should be centered (further calculated in MapPanel).
        private int[] _activeXY;
        public int[] ActiveXY   // Coords of either active unit or view piece
        {
            get
            {
                if (!ViewPieceMode )
                {
                    _activeXY = new int[] { Game.GetActiveUnit.X, Game.GetActiveUnit.Y };
                }
                return _activeXY;
            }
            set => _activeXY = value;
        }
        private bool _viewPieceMode;
        public bool ViewPieceMode 
        {
            get => !Game.GetActiveCiv.AnyUnitsAwaitingOrders || _viewPieceMode;
            set => _viewPieceMode = value;
        }

        // Generate first instance of terrain tiles by importing game data
        public void GenerateMap(GameData data, Rules rules)
        {
            Xdim = data.MapXdim;
            Ydim = data.MapYdim;
            Area = data.MapArea;
            ResourceSeed = data.MapResourceSeed;
            LocatorXdim = data.MapLocatorXdim;
            LocatorYdim = data.MapLocatorYdim;
            Visibility = data.MapVisibilityCivs;

            Tile = new Tile[Xdim, Ydim];
            for (int col = 0; col < Xdim; col++)
            {
                for (int row = 0; row < Ydim; row++)
                {
                    var terrain = data.MapTerrainType[col, row];
                    Tile[col, row] = new Tile(2 * col + (row % 2), row, rules.Terrains[(int) terrain], Map.ResourceSeed)
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
                        Island = data.MapIslandNo[col, row]
                    };
                }
            }
        }

        private static Map _instance;
        public static Map Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Map();
                return _instance;
            }
        }
    }
}
