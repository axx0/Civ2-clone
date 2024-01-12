using System.Collections.Generic;

namespace Civ2engine;

public class APlayWAV : IAction
{
    public string File { get; set; }
    public List<string> Strings { get; set; }
}
