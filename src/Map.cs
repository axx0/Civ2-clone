using System.Drawing;
using civ2.Terrains;
using civ2.Bitmaps;

namespace civ2
{
    public class Map : BaseInstance
    {
        public int Xdim { get; private set; }
        public int Ydim { get; private set; }
        public int Area { get; private set; }
        public int Seed { get; private set; }
        public int LocatorXdim { get; private set; }
        public int LocatorYdim { get; private set; }
        public ITerrain[,] Tile { get; set; }
        public bool[,][] Visibility { get; set; }    // Visibility of tiles for each civ
        //public Bitmap[] BaseMapPic { get; set; }    // For normal zoom (=0)
        //public Bitmap ActiveCivMap { get; set; }   // Currently active map, for current zoom level

        public ITerrain TileC2(int xC2, int yC2) => Tile[(((xC2 + 2 * Xdim) % (2 * Xdim)) - yC2 % 2) / 2, yC2]; // Accepts tile coords in civ2-style and returns the correct Tile (you can index beyond E/W borders for drawing round world)
        public ITerrain TileC22(int xC2, int yC2) => Tile[(((xC2 + 2 * Xdim) % (2 * Xdim)) - yC2 % 2) / 2, yC2]; // Accepts tile coords in civ2-style and returns the correct Tile (you can index 
        public bool IsTileVisibleC2(int xC2, int yC2, int civ) => Visibility[( ((xC2 + 2 * Xdim) % (2 * Xdim)) - yC2 % 2 ) / 2, yC2][civ];   // Returns Visibility for civ2-style coords (you can index beyond E/W borders for drawing round world)

        // Generate first instance of terrain tiles by importing game data
        public void GenerateMap(GameData data)
        {
            Xdim = data.MapXdim;
            Ydim = data.MapYdim;
            Area = data.MapArea;
            Seed = data.MapSeed;
            LocatorXdim = data.MapLocatorXdim;
            LocatorYdim = data.MapLocatorYdim;
            Visibility = data.MapVisibilityCivs;

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
                        SpecType = data.MapSpecialType[col, row]
                    };
                }
            }

            // Make graphics for all tiles (don't do this above, you have to know surrounding tiles)
            for (int col = 0; col < Xdim; col++)
            {
                for (int row = 0; row < Ydim; row++)
                {
                    Tile[col, row].Graphic = Draw.Terrain(Tile[col, row], col, row, Game.Options.FlatEarth);
                }
            }

            // Make standard level zoom map graphics for each civ (including barbarians with id=0) + revealed map
            //BaseMapPic = new Bitmap[9];
            //for (int civId = 0; civId < 9; civId++)
            //    BaseMapPic[civId] = Draw.DrawMap(civId, Game.Options.FlatEarth);    // For std. zoom level

            // Make a map grafic for current use in MapPanel
            //SetNewActiveMapPic();
        }

        // Set the new active map pic for MapPanel
        //public void SetNewActiveMapPic()
        //{
        //    int mapId = Game.MapRevealed ? 8 : Game.ActiveCiv.Id;
        //    if (Game.Zoom == 0) // No need to resize the map
        //        ActiveCivMap = BaseMapPic[mapId];
        //    else
        //        ActiveCivMap = ModifyImage.ResizeImage(BaseMapPic[mapId], Game.Zoom);
        //}

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
