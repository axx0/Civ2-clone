using Civ2engine;
using Model;

namespace Civ2.Dialogs;

public abstract class BaseDialogHandler : ICivDialogHandler
{
    protected BaseDialogHandler(string name, int x =-1, int y =-1)
    {
        Name = name;
        DialogPos = new Point(x, y);
    }

    private Point DialogPos { get; }

    public string Name { get; }
    public virtual ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox> popups)
    {       
        Dialog = new MenuElements
        {
            Dialog = popups[Name],
            DialogPos = DialogPos
        };
        return this;
    }
    
    public MenuElements Dialog { get; private set; }

    public abstract IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers);

    public IInterfaceAction Show()
    {
        return new MenuAction(Dialog);
    }
}