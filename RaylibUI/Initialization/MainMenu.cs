using Civ2engine;
using Model;
using Model.Core;
using Model.InterfaceActions;
using Raylib_cs;
using RaylibUI.Forms;
using RaylibUtils;
using Civ2;
using Model.Dialog;

namespace RaylibUI.Initialization;

public class MainMenu : BaseScreen
{
    private readonly Action _shutdownApp;
    private readonly Action<IGame> _startGame;
    private IInterfaceAction _currentAction;
    private List<ImagePanel> _imagePanels = new();
    private ScreenBackground? _background;
    
    private SoundData? _sndMenuLoop;

    public MainMenu(Main main, Action shutdownApp, Action<IGame> startGame, Sound soundManager) : base(main)
    {
        _shutdownApp = shutdownApp;
        _startGame = startGame;
        
        InterfaceChanged(soundManager);

        _currentAction = main.ActiveInterface.GetInitialAction();
        ProcessAction(_currentAction);
    }

    private void ProcessAction(IInterfaceAction action)
    {
        _currentAction = action;
        switch (action)
        {
            case StartGame start:
                _sndMenuLoop?.Stop();
                _startGame(start.Game);
                break;
            case ExitAction:
                _shutdownApp();
                break;
            case MenuAction menuAction:
            {
                var menu = menuAction.DialogElement;
                UpdateDecorations(menu);
                    
                ShowDialog(new CivDialog(MainWindow, menu.Dialog, menu.DialogPos, HandleButtonClick,
                    optionsCols: menu.OptionsCols,
                    replaceStrings: menu.ReplaceStrings,
                    replaceNumbers: menu.ReplaceNumbers, 
                    checkboxStates: menu.CheckboxStates,
                    textBoxDefs: menu.TextBoxes, 
                    initSelectedOption: menu.SelectedOption,
                    optionsIcons: menu.OptionsIcons,
                    image: menu.Image));
                break;
            }
            case FileAction fileAction:
                _imagePanels.Clear();
                
                ShowDialog(new FileDialog(MainWindow,fileAction.FileInfo.Title, Settings.Civ2Path, (fileName) =>
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

        ProcessAction(MainWindow.ActiveInterface.ProcessDialog(_currentAction.Name, res));
        return true;
    }

    private void UpdateDecorations(DialogElements dialog)
    {
        var existingPanels = _imagePanels.ToList();
        var newPanels = new List<ImagePanel>();
        foreach (var d in dialog.Decorations)
        {
            var key = d.Image.GetKey();
            var existing = existingPanels.FirstOrDefault(p => p.Key == key);
            if (existing != null)
            {
                existingPanels.Remove(existing);
                newPanels.Add(existing);
                existing.Location = d.Location;
            }
            else
            {
                var panel = new ImagePanel(MainWindow.ActiveInterface, key, d.Image, d.Location);
                newPanels.Add(panel);
            }
        }
        _imagePanels = newPanels;
    }



    private void HandleButtonClick(string button, int selectedIndex, IList<bool> checkboxStates,
        IDictionary<string, string>? textBoxValues)
    {
        ProcessAction(MainWindow.ActiveInterface.ProcessDialog(_currentAction.Name,
            new DialogResult(button, selectedIndex, checkboxStates, TextValues: textBoxValues)));

    }

    public override void Draw(bool pulse)
    {
        _sndMenuLoop?.MusicUpdateCall();
        
        var screenWidth = Raylib.GetScreenWidth();
        var screenHeight = Raylib.GetScreenHeight();

        var titleImg = MainWindow.ActiveInterface.ScenTitleImage;
        if (titleImg != null)
        {
            var titleTexture = TextureCache.GetImage(titleImg, MainWindow.ActiveInterface);
            Raylib.ClearBackground(_background.Background);
            Raylib.DrawTexture(titleTexture, (screenWidth - titleTexture.Width) / 2, (screenHeight - titleTexture.Height) / 2, Color.White);
        }
        else if (_background == null)
        {
            Raylib.ClearBackground(new Color(143, 123, 99, 255));
        }
        else
        {
            Raylib.ClearBackground(_background.Background);
            Raylib.DrawTexture(_background.CentreImage, (screenWidth- _background.CentreImage.Width)/2, (screenHeight-_background.CentreImage.Height)/2, Color.White);
        }
        foreach (var panel in _imagePanels)
        {
            if (titleImg == null)
                panel.Draw();
        }
        
        base.Draw(pulse);
    }

    public override void InterfaceChanged(Sound soundManager)
    {
        _sndMenuLoop?.Stop();
        _sndMenuLoop = soundManager.PlayCiv2DefaultSound("MENULOOP",true);

        _background = CreateBackgroundImage();
    }

    public ScreenBackground? CreateBackgroundImage()
    {
        var backGroundImage = MainWindow.ActiveInterface.PicSources["backgroundImage"][0];
        if (backGroundImage != null)
        {
            var img = Images.ExtractBitmap(backGroundImage, MainWindow.ActiveInterface);
            var colour = Raylib.GetImageColor(img, 0, 0);
            return new ScreenBackground(colour, TextureCache.GetImage(backGroundImage, MainWindow.ActiveInterface));
        }

        return null;
    }
}