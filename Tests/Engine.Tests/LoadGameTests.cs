using Civ2engine;
using Civ2engine.IO;
using Civ2engine.SaveLoad;
using Model;
using Model.Core;
using Model.Core.Advances;
using Model.Dialog;
using Model.Images;
using Model.ImageSets;
using Model.InterfaceActions;
using Raylib_CSharp.Images;
using Raylib_CSharp.Textures;

namespace Engine.Tests;

public class LoadGameTests
{
    public LoadGameTests()
    {
        // Need to hard code SearchPaths somehow to get the LoadFrom() method to work.
        // This happens to be where I have the game installed on my local.
        // TODO: Mock this properly.
        Settings.AddPath("C:\\code\\Civilization_2");
        
        // This is needed so that the Barbarians civ can be initialised in the GameSerializer. JSON save file loading fails if this isn't pre-populated.
        Labels.UpdateLabels(null);
    }

    [Fact]
    public void TestLoadFromThrowsExceptionIfPathNotFound()
    {
        // Arrange
        var path = "C:\\code\\Civilization_2\\pathtonowhere.sav";
        var mainApp = new MockMainApp();

        // Act
        // Assert
        Assert.Throws<FileNotFoundException>(() => LoadGame.LoadFrom(path, mainApp));
            
    }

    [Fact]
    public void TestLoadClassicGameGivesValue()
    {
        // Arrange
        // TODO: Should be mocked properly:
        // This one happens to live on my file system and has the "CIVILISE" identifier.
        var path = "C:\\code\\Civilization_2\\saves\\test_classic.sav";
        var mainApp = new MockMainApp();

        // Act
        var result = LoadGame.LoadFrom(path, mainApp);

        // Assert
        // TODO: Expand and validate the settings load properly from the file.
        Assert.NotNull(result);
    }

    [Fact]
    public void TestLoadJsonGameGivesValue()
    {
        // Arrange
         // TODO: Should be mocked properly:
         // This one happens to live on my file system and is the json version of the "test_classic.sav" file
        var path = "C:\\code\\Civilization_2\\saves\\test_json.sav";
        var mainApp = new MockMainApp();

        // Act
        var result = LoadGame.LoadFrom(path, mainApp);

        // Assert
        // TODO: Expand and validate the settings load properly from the file.
        Assert.NotNull(result);
    }
}

internal class MockInterface : IUserInterface
{
    public InterfaceStyle Look => throw new NotImplementedException();

    public string Title => throw new NotImplementedException();

    public IImageSource? ScenTitleImage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public List<TerrainSet> TileSets => throw new NotImplementedException();

    public CityImageSet CityImages => throw new NotImplementedException();

    public UnitSet UnitImages => throw new NotImplementedException();

    public PlayerColour[] PlayerColours => throw new NotImplementedException();

    public int ExpectedMaps { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public CommonMapImageSet MapImages => throw new NotImplementedException();

    public int DefaultDialogWidth => throw new NotImplementedException();

    public bool IsButtonInOuterPanel => throw new NotImplementedException();

    public Padding DialogPadding => throw new NotImplementedException();

    public Dictionary<string, IImageSource[]> PicSources => throw new NotImplementedException();

    public IList<ResourceImage> ResourceImages => throw new NotImplementedException();

    public IMain MainApp => new MockMainApp();

    public int InterfaceIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public bool CanDisplay(string? title)
    {
        throw new NotImplementedException();
    }

    public IList<DropdownMenuContents> ConfigureGameCommands(IList<IGameCommand> commands)
    {
        throw new NotImplementedException();
    }

    public void DrawBorderLines(ref Image destination, int height, int width, Padding padding, bool statusPanel)
    {
        throw new NotImplementedException();
    }

    public void DrawBorderWallpaper(Wallpaper wallpaper, ref Image destination, int height, int width, Padding padding, bool statusPanel)
    {
        throw new NotImplementedException();
    }

    public void DrawButton(Texture2D texture, int x, int y, int w, int h)
    {
        throw new NotImplementedException();
    }

    public IList<Ruleset> FindRuleSets(string[] searchPaths)
    {
        throw new NotImplementedException();
    }

    public IImageSource? GetAdvanceImage(Advance advance)
    {
        throw new NotImplementedException();
    }

    public int GetCityIndexForStyle(int cityStyleIndex, City city, int citySize)
    {
        throw new NotImplementedException();
    }

    public CityWindowLayout GetCityWindowDefinition()
    {
        throw new NotImplementedException();
    }

    public PopupBox? GetDialog(string dialogName)
    {
        throw new NotImplementedException();
    }

    public IImageSource? GetImprovementImage(Improvement improvement, int firstWonderIndex)
    {
        throw new NotImplementedException();
    }

    public IInterfaceAction GetInitialAction()
    {
        throw new NotImplementedException();
    }

    public Padding GetPadding(float headerLabelHeight, bool footer)
    {
        throw new NotImplementedException();
    }

    public string GetScientistName(int civilizationEpoch)
    {
        throw new NotImplementedException();
    }

    public IInterfaceAction HandleLoadClassicGame(GameData gameData)
    {
        return new MockAction();
    }

    public IInterfaceAction HandleLoadGame(IGame game, Rules rules, Ruleset ruleset)
    {
        return new MockAction();
    }

    public IInterfaceAction HandleLoadGame(IGame game, Rules rules, Ruleset ruleset, Dictionary<string, string?> viewData)
    {
        return new MockAction();
    }

    public IInterfaceAction HandleLoadScenario(GameData gameData, string scnName, string scnDirectory)
    {
        throw new NotImplementedException();
    }

    public IInterfaceAction HandleLoadScenario(IGame game, string scnName, string scnDirectory)
    {
        throw new NotImplementedException();
    }

    public void Initialize()
    {
        // This should ideally be done here instead of test class ctor.
        //Labels.UpdateLabels(null);
    }

    public IInterfaceAction InitNewGame(bool quickStart)
    {
        throw new NotImplementedException();
    }

    public void LoadPlayerColours()
    {
        throw new NotImplementedException();
    }

    public IInterfaceAction ProcessDialog(string dialogName, DialogResult dialogResult)
    {
        throw new NotImplementedException();
    }

    public UnitShield UnitShield(int unitType)
    {
        throw new NotImplementedException();
    }

    private class MockAction : IInterfaceAction
    {
        public string Name => throw new NotImplementedException();

        public EventType ActionType => throw new NotImplementedException();
    }
}

internal class MockMainApp : IMain
{
    public MockMainApp()
    {
    }

    public Ruleset[] AllRuleSets { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Ruleset ActiveRuleSet => new Ruleset(
        "mock",
        new Dictionary<string, string>(),
        ["C:\\code\\Civilization_2\\saves", "C:\\code\\Civilization_2"]);

    public IUserInterface SetActiveRuleSet(int ruleSetIndex)
    {
        throw new NotImplementedException();
    }

    public IUserInterface SetActiveRulesetFromFile(string root, string subDirectory, Dictionary<string, string> extendedMetadata)
    {
        return new MockInterface();
    }
}