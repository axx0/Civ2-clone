using Civ2engine;
using Model;
using Model.InterfaceActions;

namespace Civ2.Dialogs;

public abstract class BaseDialogHandler : ICivDialogHandler
{
    protected BaseDialogHandler(string name, double X = 0, double Y = 0)
    {
        Name = name;
        DialogPos = new Point(X, Y);
    }

    private Point DialogPos { get; }

    public string Name { get; }
    public virtual ICivDialogHandler UpdatePopupData(Dictionary<string, PopupBox?> popups)
    {
        if (popups.TryGetValue(Name, out var popup))
        {
            Dialog = new DialogElements
            {
                Dialog = popup,
                DialogPos = DialogPos
            };
        }

        return this;
    }
    
    public DialogElements Dialog { get; private set; }

    public abstract IInterfaceAction HandleDialogResult(DialogResult result,
        Dictionary<string, ICivDialogHandler> civDialogHandlers, Civ2Interface civ2Interface);

    public virtual IInterfaceAction Show(Civ2Interface activeInterface)
    {
        return new MenuAction(Dialog);
    }
}