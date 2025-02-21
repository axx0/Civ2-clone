using System.Collections.Generic;

namespace Model;

public class ScenarioEvent
{
    public ITrigger? Trigger { get; set; }
    public List<IScenarioAction> Actions { get; set; }

    /// <summary>
    /// Second trigger, only present if @AND modifier is present
    /// </summary>
    public ITrigger? Trigger2 { get; set; }

    /// <summary>
    /// Modifier, so that trigger is "true" for the rest of game
    /// </summary>
    public bool Continuous { get; set; }

    /// <summary>
    /// Delay modifier
    /// </summary>
    public int? Delay { get; set; } = null;

    /// <summary>
    /// Modifier that executes the event just once
    /// </summary>
    public bool JustOnce { get; set; }

    public CheckFlag FlagTrigger { get; set; }
}
