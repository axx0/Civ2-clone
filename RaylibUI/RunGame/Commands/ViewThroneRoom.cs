using Model.Input;
using Civ2engine;
using JetBrains.Annotations;
using Model;
using Model.Controls;
using RaylibUI.RunGame.GameControls;

namespace RaylibUI.RunGame.Commands;

[UsedImplicitly]
public class ViewThroneRoom(GameScreen gameScreen) : IGameCommand
{
    public string Id => CommandIds.ViewThroneRoom;
    public Shortcut[] ActivationKeys { get; set; } = { new(Key.H, ctrl: true) };
    public CommandStatus Status => CommandStatus.Normal;

    public bool Update()
    {
        return true;
    }

    public void Action()
    {
        gameScreen.ShowDialog(new ThroneRoomWindow(gameScreen, true));
    }
    public bool Checked => false;
    public MenuCommand? Command { get; set; }
    public string ErrorDialog => string.Empty;
    public DialogImageElements? ErrorImage => null;
    public string? Name => "View Throne Room";
}