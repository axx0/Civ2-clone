using System.Collections.Generic;

namespace Civ2engine;

public class AMakeAggression : IAction
{
    public int WhomCivId { get; set; }
    public int WhoCivId { get; set; }
    public List<string> Strings { get; set; }
}
