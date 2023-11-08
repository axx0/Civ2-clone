using System.Collections.Generic;

namespace Civ2engine;

public class Scenario
{
    public List<ScenarioEvent> Events = new ();
    public bool TotalWar { get; set; }
    public bool ObjectiveVictory { get; set; }
    public bool CountWondersAsObjectives { get; set; }
    public bool ForbidGovernmentSwitching { get; set; }
    public bool ForbidTechFromConquests { get; set; }
    public bool ElliminatePollution { get; set; }
    public bool SpecialWWIIonlyAI { get; set; }
    public string Name { get; set; }
    public int TechParadigm { get; set; }
    public int TurnYearIncrement { get; set; }
    public int StartingYear { get; set; }
    public int MaxTurns { get; set; }
    public int ObjectiveProtagonist { get; set; }
    public int NoObjectivesDecisiveVictory { get; set; }
    public int NoObjectivesMarginalVictory { get; set; }
    public int NoObjectivesMarginalDefeat { get; set; }
    public int NoObjectivesDecisiveDefeat { get; set; }
}
