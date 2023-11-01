using System.Collections.Generic;

namespace Civ2engine;

public class AGiveTech : IAction
{
    public int CivId { get; set; }
    public int TechId { get; set; }
	public List<string> Strings { get; set; }
}
