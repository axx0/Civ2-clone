using System.Collections.Generic;

namespace Civ2engine;

public class AEndGame : IAction
{
    public bool EndScreens { get; set; }
    public List<string> Strings { get; set; }
}
