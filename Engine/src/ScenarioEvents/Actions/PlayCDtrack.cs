using System.Collections.Generic;

namespace Civ2engine;

public class PlayCDtrack : IScenarioAction
{
    public int TrackNo { get; set; }
    public List<string> Strings { get; set; }
}
