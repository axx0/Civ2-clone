using System.Collections.Generic;

namespace Model;

public class ChangeTerrain : IScenarioAction
{
    public int TerrainTypeId { get; set; }
	public int[,] MapCoords { get; set; }
	public int MapId { get; set; }
	public short ExceptionMask { get; set; }
	public List<string> Strings { get; set; }
}
