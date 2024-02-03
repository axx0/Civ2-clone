using System.Collections.Generic;

namespace Civ2engine;

public class PlayWav : IScenarioAction
{
    public string File { get; set; }
    public List<string> Strings { get; set; }
}
