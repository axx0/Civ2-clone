using System.Collections.Generic;

namespace Civ2engine;

public class TRandomTurn : ITrigger
{
    public int Denominator { get; set; }
    public List<string> Strings { get; set; }
}
