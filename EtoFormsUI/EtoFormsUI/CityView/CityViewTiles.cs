using Civ2engine.Enums;
using Eto.Drawing;

namespace EtoFormsUI
{
    public class CityViewTiles
    {
        public int Id { get; set; }

        public ImprovementType Type { get; set; }

        /// <summary>
        /// Source image
        /// </summary>
        public Bitmap SourceBmp { get; set; }

        /// <summary>
        /// Part of source image
        /// </summary>
        public Rectangle SourceRect { get; set; }

        /// <summary>
        /// Image drawing offset
        /// </summary>
        public Point DrawOffset { get; set; }

        /// <summary>
        /// Id of tiles to be drawn if improvement does not exist in city (forest, etc.)
        /// </summary>
        public int AlternativeTileId { get; set; }

        public CityViewTiles(int id, ImprovementType type, Bitmap sourceBmp, Rectangle sourceRect, Point drawOffset, int altTileId)
        {
            id = id;
            Type = type;
            SourceBmp = sourceBmp;
            SourceRect = sourceRect;
            DrawOffset = drawOffset;
            AlternativeTileId = altTileId;
        }
    }
}
