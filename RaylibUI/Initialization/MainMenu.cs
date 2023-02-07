using System.Numerics;
using Model;
using Raylib_cs;

namespace RaylibUI.Initialization;

public class MainMenu : IScreen
{
    private readonly IUserInterface _activeInterface;
    private readonly Action _shutdownApp;
    private readonly List<Dialog> _dialogs = new();
    private IInterfaceAction _currentAction;
    private List<ImagePanel> _imagePanels = new();

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
            UpdateDecorations(action.MenuElement);
            _dialogs.Add(new Dialog(action.MenuElement.Dialog, new []{ HandleButtonClick}, action.MenuElement.TextBoxes));
        }
    }
    
    private void UpdateDecorations(MenuElements menu)
    {
        var existingPanels = _imagePanels.ToList();
        var newPanels = new List<ImagePanel>();
        foreach (var d in menu.Decorations)
        {
            var key = d.Image.Key;
            var existing = existingPanels.FirstOrDefault(p => p.Key == key);
            if (existing != null)
            {
                existingPanels.Remove(existing);
                newPanels.Add(existing);
                existing.Location = new Vector2(d.Location.X, d.Location.Y);
            }
            else
            {
                var panel = new ImagePanel(d.Image.Key,Images.ExtractBitmap(d.Image),new Vector2(d.Location.X, d.Location.Y));
                newPanels.Add(panel);
            }
        }
        _imagePanels = newPanels;
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

    public void Draw()
    {
        foreach (var panel in _imagePanels)
        {
            panel.Draw();
        }
        foreach (var dialog in _dialogs.ToList())
        {
            dialog.Draw();
        }
    }
}