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
        var root = Settings.SearchPaths.FirstOrDefault(p => savDirectory.StartsWith(p)) ?? Settings.SearchPaths[0];
        var savName = Path.GetFileName(fileName);
        GameData gameData = Read.ReadSAVFile(savDirectory, savName);
        var fallbackPath = civ2Interface.GetFallbackPath(root, gameData.GameType);

        var ruleSet = new Ruleset
        {
            FolderPath = savDirectory,
            FallbackPath = fallbackPath,
            Root = root
        };

        Initialization.ConfigObject.RuleSet = ruleSet;

        civ2Interface.ExpectedMaps = gameData.MapNoSecondaryMaps + 1;
        Initialization.LoadGraphicsAssets(civ2Interface);

        var game = ClassicSaveLoader.LoadSave(gameData, ruleSet, Initialization.ConfigObject.Rules);

        Initialization.Start(game);
        return civDialogHandlers[LoadOk.Title].Show(civ2Interface);
    }
}