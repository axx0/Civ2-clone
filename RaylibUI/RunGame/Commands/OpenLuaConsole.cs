using JetBrains.Annotations;
using Model.Input;
using Model;
using Model.Dialog;
using Model.Menu;

namespace RaylibUI.RunGame.Commands;

[UsedImplicitly]
public class OpenLuaConsole(GameScreen gameScreen) : IGameCommand
{
    public string Id => CommandIds.OpenLuaConsole;
    public Shortcut[] ActivationKeys { get; set; } = { new Shortcut(Key.D9, true, true) };
    public CommandStatus Status => CommandStatus.Normal;
    public bool Update()
    {
        return true;
    }

    public void Action()
    {
        gameScreen.ShowDialog(new LuaConsole(gameScreen), true);
    }

    public bool Checked => false;
    public MenuCommand? Command { get; set; }
    public string ErrorDialog => "";
    public DialogImageElements? ErrorImage => null;
    public string? Name => "Open Lua Console";
}