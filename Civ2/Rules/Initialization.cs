using Civ2.ImageLoader;
using Civ2engine;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.IO;
using Model;
using Model.Core;
using RaylibUtils;

namespace Civ2.Rules;

public static class Initialization
{
    private static GameInitializationConfig? _config;
    public static GameInitializationConfig ConfigObject => _config ??= GetConfig() ;
    public static IDictionary<string,string?>? ViewData { get; set; }

    private static GameInitializationConfig GetConfig()
    {
        //TODO: Load config
        var config = new GameInitializationConfig();

        return config;
    }

    public static GameInitializationConfig ClearInitializationConfig() => _config = null;

    internal static IGame GameInstance;

    public static void Start(IGame game)
    {
        GameInstance = game;
    }
    
    public static void LoadGraphicsAssets(Civ2Interface civ2Interface)
    {
        var ruleSet = civ2Interface.MainApp.ActiveRuleSet;
        ConfigObject.Rules = RulesParser.ParseRules(ruleSet);

        Images.ClearCache();
        TerrainLoader.LoadTerrain(ruleSet, civ2Interface);
        UnitLoader.LoadUnits(ruleSet, civ2Interface);
        CityLoader.LoadCities(ruleSet, civ2Interface.CityImages, civ2Interface);
        IconLoader.LoadIcons(ruleSet, civ2Interface);
        //LoadPeopleIcons(ruleset);
        //LoadCityWallpaper(ruleset);
    }

    public static void CompleteConfig()
    {
        
        ConfigObject.GroupedTribes = ConfigObject.Rules.Leaders
            .ToLookup(g => g.Color);


        var civilizations = new List<Civilization>
        {
            Barbarians.Civilization,
            ConfigObject.PlayerCiv
        };

        if (!ConfigObject.SelectComputerOpponents)
        {
            for (var i = 1; civilizations.Count <= ConfigObject.NumberOfCivs; i++)
            {
                if (i == ConfigObject.PlayerCiv.Id) continue;

                var contains = ConfigObject.GroupedTribes.Contains(i);
                var tribes = ConfigObject.GroupedTribes.Contains(i)
                    ? ConfigObject.GroupedTribes[i].ToList()
                    : ConfigObject.Rules.Leaders
                        .Where(leader => civilizations.All(civ => civ.Adjective != leader.Adjective)).ToList();

                civilizations.Add(MakeCivilization(ConfigObject, ConfigObject.Random.ChooseFrom(tribes), false,
                    i));
            }
        }

        ConfigObject.Civilizations = civilizations;
    }

    public static Civilization MakeCivilization(GameInitializationConfig config, LeaderDefaults tribe, bool human, int id)
    {
        var titles = config.Rules.Governments.Select((g, i) => GetLeaderTitle(config, tribe, g, i)).ToArray();
        var gender = human ? config.Gender : ConfigObject.Random.Next(2);
        return new Civilization
        {
            TribeId = tribe.TribeId,
            Adjective = tribe.Adjective,
            Alive = true,
            Government = (int)GovernmentType.Despotism,
            Id = id,
            Money = 0,
            Advances = new bool[config.Rules.Advances.Length],
            CityStyle = tribe.CityStyle,
            LeaderGender = gender,
            LeaderName = gender == 0 ? tribe.NameMale : tribe.NameFemale,
            LeaderTitle = titles[(int)GovernmentType.Despotism],
            ScienceRate = 60,
            TaxRate = 40,
            TribeName = tribe.Plural,
            Titles = titles,
            PlayerType = human ? PlayerType.Local : PlayerType.Ai,
            NormalColour = tribe.Color,
            AllowedAdvanceGroups = tribe.AdvanceGroups ?? new [] { AdvanceGroupAccess.CanResearch }
        };
    }
    private static string GetLeaderTitle(GameInitializationConfig config, LeaderDefaults tribe, Government gov, int governmentType)
    {
        var govt = tribe.Titles.FirstOrDefault(t=>t.Gov == governmentType) ?? (IGovernmentTitles)gov;
        return config.Gender == 0 ? govt.TitleMale : govt.TitleFemale;
    }

    public static IGame UpdateScenarioChoices()
    {
        var config = ConfigObject;
        var instance = GameInstance;
        
        instance.SetHumanPlayer(config.ScenPlayerCivId);
        instance.DifficultyLevel = config.DifficultyLevel;
        instance.GetPlayerCiv.LeaderGender = config.Gender;
        instance.GetPlayerCiv.LeaderName = config.LeaderName;
        return instance;
    }
}