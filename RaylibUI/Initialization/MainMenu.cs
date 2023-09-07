using Civ2engine;
using Model;
using Model.InterfaceActions;
using Raylib_cs;
using RaylibUI.Forms;

namespace RaylibUI.Initialization;

public class MainMenu : BaseScreen
{
    private readonly Main _main;
    private readonly Action _shutdownApp;
    private readonly Action<Game> _startGame;
    private IInterfaceAction _currentAction;
    private List<ImagePanel> _imagePanels = new();
    private readonly ScreenBackground? _background;
    
    
    private readonly SoundData? _sndMenuLoop;

    public MainMenu(Main main, Action shutdownApp, Action<Game> startGame, Sound soundManager)
    {
        _sndMenuLoop =  soundManager.PlayCIV2DefaultSound("MENULOOP",true);
        _main = main;
        _shutdownApp = shutdownApp;
        _startGame = startGame;

        ImageUtils.SetLook(_main.ActiveInterface.Look);
        _background = CreateBackgroundImage();

        _currentAction = main.ActiveInterface.GetInitialAction();
        ProcessAction(_currentAction);
    }

    private void ProcessAction(IInterfaceAction action)
    {
        FormManager.Clear();
        _currentAction = action;
        switch (action)
        {
            case StartGame start:
                Images.LoadGraphicsAssetsFromFiles(start.RuleSet, start.Game.Rules);
                _sndMenuLoop?.Stop();
                _startGame(start.Game);
                break;
            case ExitAction:
                _shutdownApp();
                break;
            case MenuAction menuAction:
            {
                var menu = menuAction.MenuElement;
                UpdateDecorations(menu);

                FormManager.Add(new Dialog(menu.Dialog, menu.DialogPos, new[] { HandleButtonClick },
                    optionsCols: menu.OptionsCols,
                    replaceStrings: menu.ReplaceStrings,
                    replaceNumbers: menu.ReplaceNumbers, checkboxStates: menu.CheckboxStates, textBoxDefs: menu.TextBoxes));

                ShowDialog(new CivDialog(menu.Dialog, menu.DialogPos, HandleButtonClick,
                    optionsCols: menu.OptionsCols,
                    replaceStrings: menu.ReplaceStrings,
                    replaceNumbers: menu.ReplaceNumbers, checkboxStates: menu.CheckboxStates, textBoxDefs: menu.TextBoxes));
                break;
            }
            case FileAction fileAction:
                _imagePanels.Clear();
                
                ShowDialog(new FileDialog(fileAction.FileInfo.Title, Settings.Civ2Path, (fileName) =>
                {
                    return fileAction.FileInfo.Filters.Any(filter => filter.IsMatch(fileName));
                }, HandleFileSelection));
                break;
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

        ProcessAction(_main.ActiveInterface.ProcessDialog(_currentAction.Name, res));
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
        ProcessAction(_main.ActiveInterface.ProcessDialog(_currentAction.Name,
            new DialogResult(button, selectedIndex, checkboxStates, TextValues: textBoxValues)));

    }

    public override void Draw(bool pulse)
    {
        _sndMenuLoop?.MusicUpdateCall();
        
        var screenWidth = Raylib.GetScreenWidth();
        var screenHeight = Raylib.GetScreenHeight();

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
        var backGroundImage = _main.ActiveInterface.BackgroundImage;
        if (backGroundImage != null)
        {
            var img = Images.ExtractBitmap(backGroundImage);
            var colour = Raylib.GetImageColor(img, 0, 0);
            return new ScreenBackground(colour, TextureCache.GetImage(backGroundImage));
        }

        return null;
    }
}