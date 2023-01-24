using System.Collections.Generic;
using Raylib_cs;
using RaylibUI.ImageLoader;

namespace RaylibUI
{
    public class TerrainSet
    {
        public Image[] BaseTiles { get; set; }
        public Image[][] Specials { get; set; }
        public Image Blank { get; set; }
        public Image[][] DitherMaps { get; set; }
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
    }
}