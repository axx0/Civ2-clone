using Raylib_CSharp.Colors;
using Raylib_CSharp.Images;
using Raylib_CSharp.Transformations;

namespace RaylibUtils;

public static class DrawUtils
{
    /// <summary>
    /// Fill a rectangle with tiles. If you input tile array, the tiles are chosen randomly (TOT-style).
    /// </summary>
    /// <param name="tile">Tiles.</param>
    /// <param name="dest">Image to which you are drawing.</param>
    /// <param name="rect">Area of the image which you are tiling.</param>
    public static void TileFill(Image[] tiles, ref Image dest, Rectangle rect)
    {
        var rnd = new Random();
        var len = tiles.Length;

        int rows = (int)rect.Height / tiles[0].Height + 1;
        int columns = (int)rect.Width / tiles[0].Width + 1;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                int srcWidth = col < columns - 1 ? tiles[0].Width : (int)rect.Width - (columns - 1) * tiles[0].Width;
                int srcHeight = row < rows - 1 ? tiles[0].Height : (int)rect.Height - (rows - 1) * tiles[0].Height;
                var srcRect = new Rectangle(0, 0, srcWidth, srcHeight);

                dest.Draw(tiles[rnd.Next(len)], srcRect,
                    new Rectangle(rect.X + col * tiles[0].Width, rect.Y + row * tiles[0].Height, srcWidth, srcHeight), Color.White);
            }
        }
    }
}
