using System.Collections.Generic;

namespace Civ2engine;

public class AChangeMoney : IAction
{
    public int ReceiverCivId { get; set; }
    public int Amount { get; set; }
    public List<string> Strings { get; set; }
}
