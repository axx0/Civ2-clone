using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Civ2engine.IO
{
    public static class MapReader
    {
        public static MapData Read(string mapPath)
        {
            var data = new MapData();
            using var reader =
                new BinaryReader(File.Open(mapPath,
                    FileMode.Open));

            data.Width = reader.ReadInt16(); //bytes[0x00000000]; //	Width x 2 (e.g. 100 for a 50 x 80 map)
            data.Height =reader.ReadInt16(); //bytes[0x00000002]; //	Height
            data.Area = reader.ReadInt16();//bytes[0x00000004]; //	Surface (width x height)
            data.FlatWorld = reader.ReadInt16() != 0; //	World shape (0 = round map, 1 or any other value = flat map)
            data.ResourceSeed = reader.ReadInt16();
            //     bytes
            //         [0x00000008]; //	Resource seed. This determines the resource and goody hut patterns. There are 64 distinct patterns, 0 to 63. 64 is the same pattern again as 0, 65 is the same as 1 etc. However, if you start a game on a map with a seed of 0 or 1 (but not their equivalents 64, 65 or any other values) Civilization II will choose a random pattern instead.
            reader.ReadBytes(4); //Discard 4 bytes

            var startingXCoords = Enumerable.Range(0, 21).Select(_ => reader.ReadInt16()).ToArray();
            var startingYCoords = Enumerable.Range(0, 21).Select(_ => reader.ReadInt16()).ToArray();
            // // bytes[0x0000000A];//	width / 2 (rounded up)
            // // bytes[0x0000000C];//	height / 4 (rounded up)
            // var startOffsetX = bytes[0x0000000E];
            // // X coordinates of all 21 civilization starting points, in the same order as in the RULES.TXT. When a civilization has no starting point, the value will be 0xFFFF.
            // //
            // // NOTE: Both X and Y coordinates are in the Civ2 coordinate system, NOT the Map Editor coordinate system!
            //
            // var startOffsetY =
            //     bytes[0x00000038]; //Y coordinates of all 21 civilization starting points, in the same order as in the RULES.TXT.
            data.StartPositions = startingXCoords.Zip(startingYCoords).ToArray();
            var bytes = reader.ReadBytes(6);
            var terrainData = new List<byte>(data.Area);
            while (bytes.Length > 0)
            {
                terrainData.Add(bytes[0]);
                bytes = reader.ReadBytes(6);
            }
            data.TerrainData =terrainData.ToArray();
            return data;
        }
    }

    public class MapData
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Area { get; set; }
        public bool FlatWorld { get; set; }
        public int ResourceSeed { get; set; }
        
        public (short First, short Second)[] StartPositions { get; set; }
        public byte[] TerrainData { get; set; }
    }
}