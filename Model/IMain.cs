using Civ2engine.IO;

namespace Model;

public interface IMain
{
    Ruleset SetActiveRulesetFromFile(string root, string subDirectory, Dictionary<string, string> extendedMetadata);
    
    public Ruleset[] AllRuleSets { get; set; }
    Ruleset ActiveRuleSet { get; }
    void SetActiveRuleSet(int ruleSetIndex);
}