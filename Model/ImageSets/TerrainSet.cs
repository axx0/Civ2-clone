using Model.Core;
using Model.Images;
using Raylib_CSharp.Images;
using RaylibUI;

namespace Model.ImageSets
{
    public class TerrainSet
    {
        public TerrainSet(int tileWidth, int tileHeight)
        {
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            HalfWidth = tileWidth / 2;
            HalfHeight = tileHeight / 2;
            DiagonalCut = HalfHeight * HalfWidth;
        }

        public int DiagonalCut { get; }

        public int HalfHeight { get; }

        public int HalfWidth { get; }

        public IImageSource[] BaseTiles { get; set; }
        public IImageSource[][] Specials { get; set; }
        public IImageSource Blank { get; set; }
        public DitherMap[] DitherMaps { get; set; }
        public IImageSource[] RiverMouth { get; set; }
        public IImageSource[] River { get; set; }
        public IImageSource[] Forest { get; set; }
        public IImageSource[] Mountains { get; set; }
        public IImageSource[] Hills { get; set; }
        public IImageSource[,] Coast { get; set; }
        public IImageSource Pollution { get; set; }
        public IImageSource GrasslandShield { get; set; }

        public IImageSource Huts { get; set; }
        public Image[] DitherMask { get; set; }
        
        public Dictionary<int, ImprovementGraphic> ImprovementsMap { get; set; }

        public int TileWidth { get; }

        public int TileHeight { get; }

        public IImageSource[] ImagesFor(TerrainType terrain)
        {
            switch (terrain)
            {
                case TerrainType.Forest:
                    return Forest;
                case TerrainType.Hills:
                    return Hills;
                case TerrainType.Mountains:
                    return Mountains;
                case TerrainType.Desert:
                case TerrainType.Plains:
                case TerrainType.Grassland:
                case TerrainType.Tundra:
                case TerrainType.Glacier:
                case TerrainType.Swamp:
                case TerrainType.Jungle:
                case TerrainType.Ocean:
                default:
                    throw new ArgumentOutOfRangeException(nameof(terrain), terrain, null);
            }
        }
    }
}