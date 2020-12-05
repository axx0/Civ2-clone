using System.Drawing;
using civ2.Terrains;
using civ2.Bitmaps;

namespace civ2
{
    public class Map
    {
        public int Xdim { get; private set; }
        public int Ydim { get; private set; }
        public int Area { get; private set; }
        public int Seed { get; private set; }
        public int LocatorXdim { get; private set; }
        public int LocatorYdim { get; private set; }
        public ITerrain[,] Tile { get; private set; }
        public Bitmap[] Graphic { get; set; }
        
        // Generate first instance of terrain tiles by importing game data
        public void GenerateMap(GameData data)
        {
            Xdim = data.MapXdim;
            Ydim = data.MapYdim;
            Area = data.MapArea;
            Seed = data.MapSeed;
            LocatorYdim = data.MapLocatorXdim;
            LocatorYdim = data.MapLocatorYdim;

            Tile = new Terrain[Xdim, Ydim];
            for (int col = 0; col < Xdim; col++)
            {
                for (int row = 0; row < Ydim; row++)
                {
                    Tile[col, row] = new Terrain
                    {
                        X = 2 * col + (row % 2),
                        Y = row,
                        Type = data.MapTerrainType[col, row],
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
                        SpecType = data.MapSpecialType[col, row],
                        Visibility = data.MapVisibilityCivs[col, row]
                    };
                }
            }

            // Make graphics for all tiles
            for (int col = 0; col < Xdim; col++)
            {
                for (int row = 0; row < Ydim; row++)
                {
                    Tile[col, row].Graphic = Draw.Terrain(Tile[col, row], col, row);
                }
            }

            // Make graphics of a map for each civ (including barbarians) + revealed map
            Graphic = new Bitmap[9];
            for (int civId = 0; civId < 9; civId++)
                Graphic[civId] = Draw.DrawMap(civId);
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
