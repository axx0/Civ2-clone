using System.Collections.Generic;
using Civ2engine.Enums;
using Model.ImageSets;
using Raylib_cs;

namespace RaylibUI
{
    public class TerrainSet
    {
        public Image[] BaseTiles { get; set; }
        public Image[][] Specials { get; set; }
        public Image Blank { get; set; }
        public DitherMap[] DitherMaps { get; set; }
        public Image[] RiverMouth { get; set; }
        public Image[] River { get; set; }
        public Image[] Forest { get; set; }
        public Image[] Mountains { get; set; }
        public Image[] Hills { get; set; }
        public Image[,] Coast { get; set; }
        public Image Pollution { get; set; }
        public Image GrasslandShield { get; set; }
        public Image[] DitherMask { get; set; }
        
        public Dictionary<int, ImprovementGraphic> ImprovementsMap { get; set; }

        public Image[] ImagesFor(TerrainType terrain)
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