using System.Collections.Generic;

namespace Civ2engine;

public class EndGame : IScenarioAction
{
    public bool EndScreens { get; set; }
    public List<string> Strings { get; set; }
}
