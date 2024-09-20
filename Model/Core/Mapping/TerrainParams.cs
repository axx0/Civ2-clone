using Civ2engine.Enums;
using Model.Core;

namespace Civ2engine
{
    public class TerrainParams
    {
        public TerrainType Type { get; set; }
        public int Frequency { get; set; }
        public int MinLength { get; set; }
        public int MeanLength { get; set; }
    }
}