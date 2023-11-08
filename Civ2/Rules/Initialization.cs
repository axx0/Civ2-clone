using Civ2.ImageLoader;
using Civ2engine;
using Civ2engine.IO;

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

    public static void Start(Game game)
    {
        GameInstance = game;
    }
    
    public static void LoadGraphicsAssets(Civ2Interface civ2Interface)
    {
        ConfigObject.RuleSet ??= RuleSets.First();
        
        ConfigObject.Rules = RulesParser.ParseRules(ConfigObject.RuleSet);
        
        //TODO: Check is interface already hase initialized images and unload them
    
        TerrainLoader.LoadTerrain(ConfigObject.RuleSet, civ2Interface);
        CityLoader.LoadCities(ConfigObject.RuleSet, civ2Interface.CityImages, civ2Interface);
        UnitLoader.LoadUnits(ConfigObject.RuleSet, civ2Interface);
        IconLoader.LoadIcons(ConfigObject.RuleSet, civ2Interface);
        //LoadPeopleIcons(ruleset);
        //LoadCityWallpaper(ruleset);
    }
}