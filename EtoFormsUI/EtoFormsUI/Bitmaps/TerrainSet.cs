using Eto.Drawing;

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
        public Bitmap[] Road { get; set; }
        public Bitmap[] Railroad { get; set; }
        public Bitmap Irrigation { get; set; }
        public Bitmap Farmland { get; set; }
        public Bitmap Mining { get; set; }
        public Bitmap Pollution { get; set; }
        public Bitmap GrasslandShield { get; set; }
        public Bitmap[] DitherMask { get; set; }
    }
}