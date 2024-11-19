using Civ2.Dialogs.NewGame;
using Civ2.Dialogs.Scenario;
using Civ2.Rules;
using Civ2engine;
using Civ2engine.IO;
using Model;
using Model.InterfaceActions;
using System.Text.RegularExpressions;
using Civ2engine.OriginalSaves;

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
        GameData gameData = Read.ReadSavFile(File.ReadAllBytes(fileName));

        var activeInterface = civ2Interface.MainApp.SetActiveRulesetFromFile(root, scnDirectory, gameData.ExtendedMetadata);

        return activeInterface.HandleLoadScenario(gameData, scnName, scnDirectory);
    }
}