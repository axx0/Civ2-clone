using System.Collections.Generic;

namespace Civ2engine;

public class TTurn : ITrigger
{
    public int Turn { get; set; }
    public List<string> Strings { get; set; }
}
