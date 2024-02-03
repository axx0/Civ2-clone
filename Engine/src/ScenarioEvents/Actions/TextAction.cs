using System.Collections.Generic;

namespace Civ2engine;

public class TextAction : IScenarioAction
{
    public bool NoBroadcast { get; set; }
    public List<string> Strings { get; set; }
}
