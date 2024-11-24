using System.Collections.Generic;
using Civ2engine.Terrains;
using Model.Core.Mapping;

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

    public List<ConstructedImprovement> I { get; set; }
    public PlayerTile?[]? P { get; set; }
    
    /// <summary>
    /// True if the tile has a river
    /// </summary>
    public bool R { get; set; }
}

