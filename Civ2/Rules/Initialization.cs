using Civ2engine;

namespace Civ2.Rules;

public static class Initialization
{
    private static GameInitializationConfig? _config;
    public static GameInitializationConfig ConfigObject => _config ??= GetConfig() ;

    private static IList<Ruleset>? _ruleSets;
    public static IList<Ruleset> RuleSets => _ruleSets ??= Locator.LocateRules(Settings.SearchPaths);

    private static GameInitializationConfig GetConfig()
    {

        //TODO: Load config
        var config = new GameInitializationConfig();

        return config;
    }

    internal static Game GameInstance = null;

    public static void Start(Game game, Ruleset ruleSet)
    {
        GameInstance = game;
        SelectedRuleSet = ruleSet;
    }

    public static Ruleset SelectedRuleSet { get; set; }
}