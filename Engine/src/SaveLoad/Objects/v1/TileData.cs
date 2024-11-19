namespace Civ2engine.SaveLoad;

public class TileData
{
    /// <summary>
    /// The terrain of the tile
    /// </summary>
    public int T { get; set; }

    /// <summary>
    /// Visibility of the tile by civs
    /// </summary>
    public bool[] V { get; set; }
}