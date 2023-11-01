using System.Collections.Generic;

namespace Civ2engine;

public class APlayCDtrack : IAction
{
    public int TrackNo { get; set; }
    public List<string> Strings { get; set; }
}
