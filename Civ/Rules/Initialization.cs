using Civ2engine;

namespace Civ.Rules;

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

}