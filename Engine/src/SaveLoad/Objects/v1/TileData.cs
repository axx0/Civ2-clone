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
    /// I the improvements at the tile
    /// </summary>
    public string? I { get; set; }
    
    /// <summary>
    /// The players that can see the tile and what they see A means all
    /// </summary>
    public IList<string> P { get; set; }
    
    /// <summary>
    /// True if the tile has a river
    /// </summary>
    public bool R { get; set; }
}

