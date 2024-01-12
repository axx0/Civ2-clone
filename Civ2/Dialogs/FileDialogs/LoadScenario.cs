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
        var ruleSet = new Ruleset
        {
            FolderPath = scnDirectory,
            Root = Settings.SearchPaths.FirstOrDefault(p => scnDirectory.StartsWith(p)) ?? Settings.SearchPaths[0]
        };
        var scnName = Path.GetFileName(fileName);

        Initialization.ConfigObject.RuleSet = ruleSet;

        Initialization.LoadGraphicsAssets(civ2Interface);

        var game = ClassicSaveLoader.LoadSave(ruleSet, scnName, Initialization.ConfigObject.Rules);

        Initialization.Start(game);

        var introFile = Regex.Replace(scnName, ".scn", ".txt", RegexOptions.IgnoreCase);
        if (Directory.EnumerateFiles(scnDirectory, introFile, new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }).FirstOrDefault() != null)
        {
            var popupbox = ScenarioIntroLoader.LoadIntro(new string[] { scnDirectory }, introFile);
        }

        return civDialogHandlers[ScenarioLoaded.Title].Show(civ2Interface);
    }
}