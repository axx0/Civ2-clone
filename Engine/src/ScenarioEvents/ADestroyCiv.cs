using System.Collections.Generic;

namespace Civ2engine;

public class ADestroyCiv : IAction
{
    public int CivId { get; set; }
	public List<string> Strings { get; set; }
}
