using Civ2engine;
using Model;
using Model.InterfaceActions;

namespace Civ2.Dialogs.FileDialogs;

public class LoadMap : FileDialogHandler
{
    public const string DialogTitle = "File-LoadMap";

    public LoadMap() : base(DialogTitle, ".mp")
    {
    }

    public override ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popup)
    {
        this.Title = Labels.For(LabelIndex.SelectMapToLoad);
        return this;
    }

    protected override IInterfaceAction HandleFileSelection(string fileName)
    {
        throw new NotImplementedException();
    }
}