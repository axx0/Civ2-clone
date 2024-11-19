using Civ2.Rules;
using Civ2engine;
using Civ2engine.IO;
using Civ2engine.OriginalSaves;
using Model;
using Model.InterfaceActions;

namespace Civ2.Dialogs.FileDialogs;

public class LoadGame : FileDialogHandler
{
    public const string DialogTitle = "File-LoadGame";

    public LoadGame() : base(DialogTitle, ".sav")
    {
    }

    public override ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox?> popup)
    {
        this.Title = Labels.For(LabelIndex.SelectGameToLoad);
        return this;
    }

    protected override IInterfaceAction HandleFileSelection(string fileName,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        return Civ2engine.SaveLoad.LoadGame.LoadFrom(fileName, civ2Interface.MainApp);
    }
}