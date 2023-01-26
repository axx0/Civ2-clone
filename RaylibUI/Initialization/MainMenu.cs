using Model;

namespace RaylibUI.Initialization;

public class MainMenu : IScreen
{
    private readonly IUserInterface _activeInterface;
    private readonly Action _shutdownApp;
    private readonly List<Dialog> _dialogs = new();
    private IInterfaceAction _currentAction;

    public MainMenu(IUserInterface activeInterface, Action shutdownApp)
    {
        _activeInterface = activeInterface;
        _shutdownApp = shutdownApp;

        _currentAction = activeInterface.GetInitialAction();
        MakeMenuElements(_currentAction);
    }

    private void MakeMenuElements(IInterfaceAction action)
    {
        _dialogs.Clear();
        if (action.MenuElement != null)
        {
            _dialogs.Add(new Dialog(action.MenuElement.Dialog, new []{ HandleButtonClick}, action.MenuElement.TextBoxes));
        }
    }

    private void HandleButtonClick(string button, int selectedIndex, IDictionary<string ,string>? textBoxValues)
    {
        var act =_activeInterface.ProcessDialog(_currentAction.MenuElement.Dialog.Name, new DialogResult(button, selectedIndex, TextValues: textBoxValues));
        if (act.ActionType == EventType.Exit)
        {
            _shutdownApp();
        }
        else
        {
            _currentAction = act;
            MakeMenuElements(act);
        }
    }

    public IInterfaceAction CurrentAction { get; set; }
    public void Draw()
    {
        foreach (var dialog in _dialogs.ToList())
        {
            dialog.Draw();
        }
    }
}