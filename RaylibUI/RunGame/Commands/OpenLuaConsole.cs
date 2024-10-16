using Model;
using Model.Dialog;
using Model.Images;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.Commands;

public class OpenLuaConsole : IGameCommand
{
    private readonly GameScreen _gameScreen;
    public string Id => CommandIds.OpenLuaConsole;
    public Shortcut[] ActivationKeys { get; set; } = { new Shortcut(KeyboardKey.Nine, true, true) };
    public CommandStatus Status => CommandStatus.Normal;
    public bool Update()
    {
        return true;
    }

    public void Action()
    {
        _gameScreen.ShowDialog(new LuaConsole(_gameScreen), true);
    }

    public OpenLuaConsole(GameScreen gameScreen)
    {
        _gameScreen = gameScreen;
    }

    public MenuCommand? Command { get; set; }
    public string ErrorDialog { get; }
    public DialogImageElements? ErrorImage { get; } = null;
    public string? Name { get; }
}