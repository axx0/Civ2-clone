using Civ2engine;
using Model;
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

    public abstract ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popup);

    public MenuElements Dialog { get; }
    public IInterfaceAction HandleDialogResult(DialogResult result, Dictionary<string, ICivDialogHandler> civDialogHandlers)
    {
        if (result.SelectedIndex == 0)
        {
            var fileName = result.TextValues?["FileName"];
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                return HandleFileSelection(fileName, civDialogHandlers);
            }
        }

        return civDialogHandlers[MainMenu.Title].Show();
    }

    protected abstract IInterfaceAction HandleFileSelection(string fileName,
        Dictionary<string, ICivDialogHandler> civDialogHandlers);

    public IInterfaceAction Show()
    {
        return new FileAction(new OpenFileInfo
        {
            Title = Title,
            Filters = new List<FileFilter> { new(_extension) }
        }, Name);
    }

    public string Title { get; protected set;  }
}