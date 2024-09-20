using Civ2engine;
using Model.Dialog;
using Model.InterfaceActions;

namespace Civ2.Dialogs;

public abstract class FileDialogHandler : ICivDialogHandler
{
    private readonly string _extension;
    public string Name { get; }

    protected FileDialogHandler(string name, string extension)
    {
        _extension = extension;
        Name = name;
    }

    public abstract ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox?> popup);

    public DialogElements Dialog { get; }
    public IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface)
    {
        if (result.SelectedIndex == 0)
        {
            var fileName = result.TextValues?["FileName"];
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                return HandleFileSelection(fileName, civDialogHandlers, civ2Interface);
            }
        }

        return civDialogHandlers[MainMenu.Title].Show(civ2Interface);
    }

    protected abstract IInterfaceAction HandleFileSelection(string fileName,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface active);

    public IInterfaceAction Show(Civ2Interface activeInterface)
    {
        return new FileAction(new OpenFileInfo
        {
            Title = Title,
            Filters = new List<FileFilter> { new(_extension) }
        }, Name);
    }

    public string Title { get; protected set;  }
}