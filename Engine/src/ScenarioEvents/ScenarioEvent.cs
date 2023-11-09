using System.Collections.Generic;

namespace Civ2engine;

public class ScenarioEvent
{
    public ITrigger? Trigger { get; set; }
    public List<IAction> Actions { get; set; }
}
