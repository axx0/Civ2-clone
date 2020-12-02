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
        
        // Generate first instance of terrain tiles by importing game data
        public void GenerateMap(GameData data)
        {
            Xdim = data.MapXdim;
            Ydim = data.MapYdim;
            Area = data.MapArea;
            Seed = data.MapSeed;
            LocatorYdim = data.MapLocatorXdim;
            LocatorYdim = data.MapLocatorYdim;

            Tile = new ITerrain[Xdim, Ydim];
            for (int col = 0; col < Xdim; col++)
            {
                for (int row = 0; row < Ydim; row++)
                {
                    Tile[col, row].Type = data.MapTerrainType[col, row];
                    Tile[col, row].River = data.MapRiverPresent[col, row];
                    Tile[col, row].Resource = data.MapResourcePresent[col, row];
                    Tile[col, row].UnitPresent = data.MapUnitPresent[col, row];
                    Tile[col, row].CityPresent = data.MapCityPresent[col, row];
                    Tile[col, row].Irrigation = data.MapIrrigationPresent[col, row];
                    Tile[col, row].Mining = data.MapMiningPresent[col, row];
                    Tile[col, row].Road = data.MapRoadPresent[col, row];
                    Tile[col, row].Railroad = data.MapRailroadPresent[col, row];
                    Tile[col, row].Fortress = data.MapFortressPresent[col, row];
                    Tile[col, row].Pollution = data.MapPollutionPresent[col, row];
                    Tile[col, row].Farmland = data.MapFarmlandPresent[col, row];
                    Tile[col, row].Airbase = data.MapAirbasePresent[col, row];
                    Tile[col, row].Island = data.MapIslandNo[col, row];
                    Tile[col, row].SpecType = data.MapSpecialType[col, row];
                    Tile[col, row].Visibility = data.MapVisibilityCivs[col, row];
                }
            }

            // Make graphics for all tiles
            for (int col = 0; col < Xdim; col++)
            {
                for (int row = 0; row < Ydim; row++)
                {
                    Tile[col, row].Graphic = Draw.DrawTerrain(Tile[col, row], col, row);
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
