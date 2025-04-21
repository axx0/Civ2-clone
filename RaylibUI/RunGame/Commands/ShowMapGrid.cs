using Civ2engine;
using Model;
using Model.Dialog;
using Model.Images;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.Commands;

public class ShowMapGrid : IGameCommand
{
    private readonly GameScreen _gameScreen;

    public ShowMapGrid(GameScreen gameScreen)
    {
        _gameScreen = gameScreen;
    }
    
    public string Id => CommandIds.ShowMapGrid;
    public Shortcut[] ActivationKeys { get; set; } = { new(KeyboardKey.G, ctrl: true) };
    public CommandStatus Status => CommandStatus.Normal;

    public bool Update()
    {
        return true;
    }

    public void Action()
    {
        _gameScreen.ShowMapGrid();
    }

    public bool Checked => _gameScreen.ShowGrid;
    public MenuCommand? Command { get; set; }
    public string ErrorDialog { get; } = string.Empty;
    public DialogImageElements? ErrorImage { get; } = null;
    public string? Name { get; }
}