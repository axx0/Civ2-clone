using System;
using System.Collections.Generic;
using System.Linq;

namespace Civ2engine.MapObjects
{
    public class Map
    {
        public bool Flat { get; }

        public Map(bool flat, int index)
        {
            Flat = flat;
            MapIndex = index;
        }
        public int MapIndex { get; } = 0;

        public int XDim
        {
            get => _xDim;
            set
            {
                _xDim = value;
                XDimMax = value * 2;
            }
        }

        public int XDimMax { get; private set; }
        public int YDim { get; set; }
        public int ResourceSeed { get; set; }
        public int LocatorXdim { get; set; }
        public int LocatorYdim { get; set; }
        public bool MapRevealed { get; set; }
        public int WhichCivsMapShown { get; set; }
        public Tile[,] Tile { get; set; }
        public bool IsValidTileC2(int xC2, int yC2)
        {
            var maxX = XDimMax;
            var x = (((xC2 + maxX) % maxX) - yC2 % 2);
            return -1 < x && x < maxX && -1 < yC2 && yC2 < YDim;
        }
        public Tile TileC2(int xC2, int yC2) =>
            Tile
            [((xC2 + XDimMax) % XDimMax - yC2 % 2) / 2,
                yC2]; // Accepts tile coords in civ2-style and returns the correct Tile (you can index beyond E/W borders for drawing round world)
        public bool IsTileVisibleC2(int xC2, int yC2, int civ) => MapRevealed || Tile[( (xC2 + XDimMax) % XDimMax - yC2 % 2 ) / 2, yC2].IsVisible(civ);   // Returns V

        private int _xDim;
        
        public int[] StartingClickedXy { get; set; }    // Last tile clicked with your mouse on the map. Gives info where the map should be centered (further calculated in MapPanel).
        public List<IslandDetails> Islands { get; set; }
        public double ScaleFactor => XDim * YDim / 4000d;
    }
}
