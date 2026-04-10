using Civ2engine;
using Civ2engine.IO;
using Civ2engine.SaveLoad;
using Core.Tests.Mocks;
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

namespace Core.Tests.SaveLoad;

public class LoadGameTests
{
    private readonly IMain _mockMainApp;
    private readonly MockInterface _mockUi;

    public LoadGameTests()
    {
        // We need to hard code the SearchPaths here for the LoadFrom() method to work properly under test.
        _mockUi = new MockInterface();
        _mockMainApp = _mockUi.MainApp;
        var testFileDirectory = TestFileUtils.GetTestFileDirectory();
        Settings.SearchPaths = [testFileDirectory, testFileDirectory];

        // This is also needed so that the Barbarians civ can be initialised in the GameSerializer.
        // JSON save file loading fails if this isn't pre-populated.
        Labels.UpdateLabels(_mockMainApp.ActiveRuleSet);
    }

    [Fact]
    public void TestLoadFromThrowsExceptionIfPathNotFound()
    {
        // Arrange
        var saveFilePath = TestFileUtils.GetTestFilePath("pathtonowhere.sav");

        // Act
        // Assert
        Assert.Throws<FileNotFoundException>(() => LoadGame.LoadFrom(saveFilePath, _mockMainApp));

    }

    [Fact]
    public void TestLoadClassicGameGivesValue()
    {
        // Arrange
        // These are identified by having the "CIVILISE" word at the start of the file.
        var path = TestFileUtils.GetTestFilePath("test_classic.sav");

        // Act
        LoadGame.LoadFrom(path, _mockMainApp);
        var result = (Game)_mockUi.LoadedGame!;

        // Assert
        Assert.NotNull(result);
        
        Assert.Equal(1, result.TurnNumber);
        Assert.Equal(8, result.AllCivilizations.Count);
        
        var barbarians = result.AllCivilizations[0];
        Assert.Equal("Barbarians", barbarians.TribeName);
        Assert.Equal(PlayerType.Barbarians, barbarians.PlayerType);

        var player = result.AllCivilizations[1];
        Assert.Equal("Romans", player.TribeName);
        Assert.Equal(PlayerType.Local, player.PlayerType);
        Assert.Equal(0, player.Money);
    }

    [Fact]
    public void TestLoadJsonGameGivesValue()
    {
        // Arrange
        // This is the json version of the "test_classic.sav" file
        var path = TestFileUtils.GetTestFilePath("test_json.sav");

        // Act
        LoadGame.LoadFrom(path, _mockMainApp);
        var result = (Game)_mockUi.LoadedGame!;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TurnNumber);
    }
}