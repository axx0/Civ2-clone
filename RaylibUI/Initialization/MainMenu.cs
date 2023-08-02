using System.Numerics;
using Civ2engine;
using Model;
using Raylib_cs;
using RaylibUI.Forms;

namespace RaylibUI.Initialization;

public class MainMenu : BaseScreen
{
    private readonly IUserInterface _activeInterface;
    private readonly Action _shutdownApp;
    private IInterfaceAction _currentAction;
    private List<ImagePanel> _imagePanels = new();
    private readonly ScreenBackground? _background;

    public MainMenu(IUserInterface activeInterface, Action shutdownApp)
    {
        _activeInterface = activeInterface;
        _shutdownApp = shutdownApp;

        ImageUtils.SetLook(_activeInterface.Look);

        _currentAction = activeInterface.GetInitialAction();
        MakeMenuElements(_currentAction);
        _background = CreateBackgroundImage();
    }

    private void MakeMenuElements(IInterfaceAction action)
    {
        FormManager.Clear();
        
        if (action.MenuElement != null)
        {
            UpdateDecorations(action.MenuElement);

            FormManager.Add(new Dialog(action.MenuElement.Dialog, action.MenuElement.DialogPos, new[] { HandleButtonClick },
                optionsCols: action.MenuElement.OptionsCols, 
                replaceNumbers: action.MenuElement.ReplaceNumbers, checkboxStates: action.MenuElement.CheckboxStates, textBoxDefs: action.MenuElement.TextBoxes));
        
            ShowDialog(new CivDialog(action.MenuElement.Dialog, action.MenuElement.DialogPos,  HandleButtonClick,
                optionsCols: action.MenuElement.OptionsCols, 
                replaceNumbers: action.MenuElement.ReplaceNumbers, checkboxStates: action.MenuElement.CheckboxStates, textBoxDefs: action.MenuElement.TextBoxes));
            
        }

        if (action.FileInfo != null)
        {
            ShowDialog(new FileDialog(action.FileInfo.Title, Settings.Civ2Path, (fileName) =>
            {
                return action.FileInfo.Filters.Any(filter => filter.IsMatch(fileName));
            }, HandleFileSelection));

        }
    }

    private bool HandleFileSelection(string? fileName)
    {
        DialogResult res;
        if (!string.IsNullOrWhiteSpace(fileName))
        {
            res = new DialogResult("Ok", 0,
                TextValues: new Dictionary<string, string> { { "FileName", fileName } });
        }
        else
        {
            res = new DialogResult("Cancel", 1);
        }

        NextAct(_activeInterface.ProcessDialog(_currentAction.Name, res));
        return true;
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
                existing.Location = d.Location;
            }
            else
            {
                var panel = new ImagePanel(d.Image.Key,d.Image,d.Location);
                newPanels.Add(panel);
            }
        }
        _imagePanels = newPanels;
    }



    private void HandleButtonClick(string button, int selectedIndex, IList<bool> checkboxStates,
        IDictionary<string, string>? textBoxValues)
    {
        if (_currentAction.MenuElement == null) return;
        NextAct(_activeInterface.ProcessDialog(_currentAction.MenuElement.Dialog.Name,
            new DialogResult(button, selectedIndex, checkboxStates, TextValues: textBoxValues)));

    }

    private void NextAct(IInterfaceAction newAction)
    {
        if (newAction.ActionType == EventType.Exit)
        {
            _shutdownApp();
        }
        else
        {
            _currentAction = newAction;
            MakeMenuElements(newAction);
        }
    }

    public override void Draw(bool pulse)
    {
        int screenWidth = Raylib.GetScreenWidth();
        int screenHeight = Raylib.GetScreenHeight();

        if (_background == null)
        {
            Raylib.ClearBackground(new Color(143, 123, 99, 255));
        }
        else
        {
            Raylib.ClearBackground(_background.background);
            Raylib.DrawTexture(_background.CentreImage, (screenWidth- _background.CentreImage.width)/2, (screenHeight-_background.CentreImage.height)/2, Color.WHITE);
        }
        foreach (var panel in _imagePanels)
        {
            panel.Draw();
        }

        FormManager.DrawForms();
        
        base.Draw(pulse);
    }

    public ScreenBackground? CreateBackgroundImage()
    {
        var backGroundImage = _activeInterface.BackgroundImage;
        if (backGroundImage != null)
        {
            var img = Images.ExtractBitmap(backGroundImage);
            var colour = Raylib.GetImageColor(img, 0, 0);
            return new ScreenBackground(colour, TextureCache.GetImage(backGroundImage));
        }

        return null;
    }
}