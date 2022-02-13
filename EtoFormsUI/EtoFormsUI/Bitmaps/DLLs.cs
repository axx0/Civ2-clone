using System.IO;
using Civ2engine;

namespace EtoFormsUI
{
    public static class DLLs
    {
        private static byte[] _intro;
        private static byte[] _tiles;
        public static byte[] Intro => _intro ??= File.ReadAllBytes( Utils.GetFilePath("Intro.dll"));
        public static byte[] Tiles => _tiles ??= File.ReadAllBytes(Utils.GetFilePath("Tiles.dll"));
    }
}
