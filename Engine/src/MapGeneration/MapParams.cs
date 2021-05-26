using Civ2engine.Enums;

namespace Civ2engine
{
    public class MapParams
    {
        public MapType Type { get; set; }
        public int BlobSize { get; set; }
        public int NumberOfBlobs { get; set; }
        public int BridgeLength { get; set; }
        public int BridgesPerBlob { get; set; }
        public TerrainParams[] TerrainParams { get; set; }
    }
}