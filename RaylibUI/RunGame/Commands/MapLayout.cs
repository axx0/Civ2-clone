using Civ2engine;
using Model;
using Model.Menu;
using Raylib_cs;

namespace RaylibUI.RunGame.Commands;

public class MapLayout : IGameCommand
{
    private readonly GameScreen _gameScreen;

    public MapLayout(GameScreen gameScreen)
    {
        _gameScreen = gameScreen;
    }
    
    public string Id => CommandIds.MapLayoutToggle;
    public Shortcut[] ActivationKeys { get; set; } = new[] { Shortcut.None };
    public CommandStatus Status => CommandStatus.Normal;

    public bool Update()
    {
        return true;
    }

    public void Action()
    {
        _gameScreen.ToggleMapLayout();
    }

    public MenuCommand? Command { get; set; }
    public string ErrorDialog { get; } = string.Empty;
    public string? Name { get; }
}