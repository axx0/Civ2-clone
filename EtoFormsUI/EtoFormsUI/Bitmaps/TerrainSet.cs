using System.Collections.Generic;
using Eto.Drawing;
using EtoFormsUI.ImageLoader;

namespace EtoFormsUI
{
    public class TerrainSet
    {
        public Bitmap[] BaseTiles { get; set; }
        public Bitmap[][] Specials { get; set; }
        public Bitmap Blank { get; set; }
        public Bitmap[][] DitherMaps { get; set; }
        public Bitmap[] RiverMouth { get; set; }
        public Bitmap[] River { get; set; }
        public Bitmap[] Forest { get; set; }
        public Bitmap[] Mountains { get; set; }
        public Bitmap[] Hills { get; set; }
        public Bitmap[,] Coast { get; set; }
        public Bitmap Pollution { get; set; }
        public Bitmap GrasslandShield { get; set; }
        public Bitmap[] DitherMask { get; set; }
        
        public Dictionary<int, ImprovementGraphic> ImprovementsMap { get; set; }
    }
}