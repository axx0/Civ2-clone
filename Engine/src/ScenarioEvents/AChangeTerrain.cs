using System.Collections.Generic;

namespace Civ2engine;

public class AChangeTerrain : IAction
{
    public int TerrainTypeId { get; set; }
	public int[] MapCoords { get; set; }
	public List<string> Strings { get; set; }
}
