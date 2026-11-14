namespace Civ2engine.Enums
{
    /// <summary>
    /// How map graphics (terrain, units, cities) look when they are scaled by zoom
    /// </summary>
    public enum MapScaling
    {
        /// <summary>
        /// Nearest-neighbour: crisp pixels, like the original game
        /// </summary>
        Pixelated = 0,

        /// <summary>
        /// Bilinear: smooth, slightly blurry
        /// </summary>
        Smooth = 1,
    }
}
