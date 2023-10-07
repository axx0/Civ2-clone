using Civ2.Rules;
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

    protected override IInterfaceAction HandleFileSelection(string fileName,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        var savDirectory = Path.GetDirectoryName(fileName);
        var ruleSet = new Ruleset
        {
            FolderPath = savDirectory,
            Root = Settings.SearchPaths.FirstOrDefault(p => savDirectory.StartsWith(p)) ?? Settings.SearchPaths[0]
        };
        var savName = Path.GetFileName(fileName);

        Initialization.ConfigObject.RuleSet = ruleSet;
        
        Initialization.LoadGraphicsAssets(civ2Interface);

        var game = ClassicSaveLoader.LoadSave(ruleSet, savName, Initialization.ConfigObject.Rules);

        Initialization.Start(game);
        return civDialogHandlers[LoadOk.Title].Show(civ2Interface);
    }
}