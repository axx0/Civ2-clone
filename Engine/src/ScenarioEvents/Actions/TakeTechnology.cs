using System.Collections.Generic;

namespace Civ2engine;

public class TakeTechnology : IScenarioAction
{
	public int TechId { get; set; }

	/// <summary>
	/// 0xFC=TRIGGERRECEIVER/TRIGGERDEFENDER, 0xFD=TRIGGERATTACKER
	/// </summary>
	public int WhomId { get; set; }
	public bool Collapse { get; set; }
	public List<string> Strings { get; set; }
}
