using Civ2.Dialogs.NewGame;
using Civ2.Dialogs.Scenario;
using Civ2.Rules;
using Civ2engine;
using Civ2engine.IO;
using Model;
using Model.InterfaceActions;
using System.Text.RegularExpressions;

namespace Civ2.Dialogs.FileDialogs;

public class LoadScenario : FileDialogHandler
{
    public const string DialogTitle = "File-LoadScenario";

    public LoadScenario() : base(DialogTitle, ".scn")
    {
    }

    public override ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox?> popup)
    {
        this.Title = Labels.For(LabelIndex.SelectScenarioToLoad);
        return this;
    }

    protected override IInterfaceAction HandleFileSelection(string fileName, Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        var scnDirectory = Path.GetDirectoryName(fileName);
        var root = Settings.SearchPaths.FirstOrDefault(p => scnDirectory.StartsWith(p)) ?? Settings.SearchPaths[0];
        var scnName = Path.GetFileName(fileName);
        GameData gameData = Read.ReadSAVFile(scnDirectory, scnName);
        var fallbackPath = civ2Interface.GetFallbackPath(root, gameData.GameType);

        var ruleSet = new Ruleset
        {
            FolderPath = scnDirectory,
            FallbackPath = fallbackPath,
            Root = root
        };

        Initialization.ConfigObject.RuleSet = ruleSet;

        civ2Interface.ExpectedMaps = gameData.MapNoSecondaryMaps + 1;
        Initialization.LoadGraphicsAssets(civ2Interface);

        var game = ClassicSaveLoader.LoadSave(gameData, ruleSet, Initialization.ConfigObject.Rules);

        Initialization.Start(game);

        var introFile = Regex.Replace(scnName, ".scn", ".txt", RegexOptions.IgnoreCase);
        if (Directory.EnumerateFiles(scnDirectory, introFile, new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).FirstOrDefault() != null)
        {
            var popupbox = ScenarioIntroLoader.LoadIntro(new string[] { scnDirectory }, introFile);
        }

        return civDialogHandlers[ScenarioLoaded.Title].Show(civ2Interface);
    }
}