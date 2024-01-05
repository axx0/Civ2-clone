using System.Collections.Generic;

namespace Civ2engine;

public class AText : IAction
{
    public bool NoBroadcast { get; set; }
    public List<string> Strings { get; set; }
}
