using Civ2engine;
using Civ2engine.IO;
using Core.Tests.TestFiles;
using Model;
using Model.Controls;
using Model.Core;
using Model.Core.Advances;
using Model.Images;
using Model.ImageSets;
using Model.InterfaceActions;
using Raylib_CSharp.Images;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Transformations;

namespace Core.Tests.Mocks;

internal class MockInterface : IUserInterface
{
    public InterfaceStyle Look => throw new NotImplementedException();

    public string Title => throw new NotImplementedException();

    public IImageSource? ScenTitleImage
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public List<TerrainSet> TileSets => throw new NotImplementedException();

    public CityImageSet CityImages => throw new NotImplementedException();

    public UnitSet UnitImages => throw new NotImplementedException();

    public PlayerColour[] PlayerColours => throw new NotImplementedException();

    public int ExpectedMaps
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public CommonMapImageSet MapImages => throw new NotImplementedException();

    public int DefaultDialogWidth => throw new NotImplementedException();

    public bool IsButtonInOuterPanel => throw new NotImplementedException();

    public Padding DialogPadding => throw new NotImplementedException();

    public Dictionary<string, IImageSource[]> PicSources => throw new NotImplementedException();

    public IList<ResourceImage> ResourceImages => throw new NotImplementedException();

    public IMain MainApp => new MockMainApp(this);

    public IGame? LoadedGame { get; private set; }

    public int InterfaceIndex
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

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

    public void DrawBorderWallpaper(Wallpaper wallpaper, ref Image destination, int height, int width,
        Padding padding, bool statusPanel)
    {
        throw new NotImplementedException();
    }

    public void DrawButton(Texture2D texture, Rectangle bounds)
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

    public int GetCityStyleIndexFromEpoch(int cityStyle, int epoch)
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

    public ListboxLooks GetListboxLooks(ListboxType? type)
    {
        throw new NotImplementedException();
    }

    public List<CityViewTiles> GetCityViewTiles()
    {
        throw new NotImplementedException();
    }

    public List<BinaryStorage> GetCityViewAltTiles()
    {
        throw new NotImplementedException();
    }

    public CivilopediaProperties GetCivilopediaProperties(Civilopedia civilopedia)
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

    public IInterfaceAction HandleLoadGame(IGame game, Rules rules, Ruleset ruleset,
        Dictionary<string, string?> viewData)
    {
        LoadedGame = game;
        return new MockAction();
    }

    public IInterfaceAction HandleLoadScenario(IGame game, string scnName, Ruleset ruleset)
    {
        return new MockAction();
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

    internal class MockMainApp : IMain
    {
        private readonly MockInterface _ui;

        public MockMainApp(MockInterface ui)
        {
            _ui = ui;
        }

        public Ruleset[] AllRuleSets
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public Ruleset ActiveRuleSet => new Ruleset(
            "mock",
            new Dictionary<string, string>(),
            [TestFileUtils.GetTestFileDirectory()]);

        public IUserInterface SetActiveRuleSet(int ruleSetIndex)
        {
            throw new NotImplementedException();
        }

        public IUserInterface SetActiveRulesetFromFile(string root, string subDirectory,
            Dictionary<string, string> extendedMetadata)
        {
            return _ui;
        }
    }
}
