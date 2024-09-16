using Civ2engine;
using Model;
using Model.Dialog;
using Model.Images;
using Model.Menu;
using Raylib_cs;

namespace RaylibUI.RunGame.Commands;

public class Quit : IGameCommand
{
    private readonly GameScreen _gameScreen;

    public Quit(GameScreen gameScreen)
    {
        _gameScreen = gameScreen;
    }
    
    public string Id => CommandIds.QuitGame;
    public Shortcut[] ActivationKeys { get; set; } = { new(KeyboardKey.Q, ctrl: true) };
    public CommandStatus Status => CommandStatus.Normal;

    public bool Update()
    {
        return true;
    }

    public void Action()
    {
        // ReSharper disable once StringLiteralTypo
        _gameScreen.ShowPopup("REALLYQUIT", DialogClick);
    }

    private void DialogClick(string button, int option, IList<bool>? _, IDictionary<string, string>? _2)
    {
        if (button == Labels.Ok && option == 1)
        {
            _gameScreen.Main.ReloadMain();
        }
    }

    public MenuCommand? Command { get; set; }
    public string ErrorDialog { get; } = string.Empty;
    public DialogImageElements? ErrorImage { get; } = null;
    public string? Name { get; }
}