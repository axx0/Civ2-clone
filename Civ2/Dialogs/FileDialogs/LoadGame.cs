using Civ2engine;
using Civ2engine.IO;
using Model;
using Model.InterfaceActions;

namespace Civ2.Dialogs.FileDialogs;

public class LoadGame : FileDialogHandler
{
    public const string DialogTitle = "File-LoadGame";

    public LoadGame() : base(DialogTitle, ".sav")
    {
    }

    public override ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popup)
    {
        this.Title = Labels.For(LabelIndex.SelectGameToLoad);
        return this;
    }

    protected override IInterfaceAction HandleFileSelection(string fileName)
    {
        var savDirectory = Path.GetDirectoryName(fileName);
        var ruleSet = new Ruleset
        {
            FolderPath = savDirectory,
            Root = Settings.SearchPaths.FirstOrDefault(p => savDirectory.StartsWith(p)) ?? Settings.SearchPaths[0]
        };
        var savName = Path.GetFileName(fileName);
        

        var game = ClassicSaveLoader.LoadSave(ruleSet, savName, RulesParser.ParseRules(ruleSet));
        return new StartGame(game);
    }
}