using Model.Input;
using Civ2engine;
using JetBrains.Annotations;
using Model;
using Model.Dialog;
using Model.Images;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.Commands;

[UsedImplicitly]
public class ShowMapGrid(GameScreen gameScreen) : IGameCommand
{
    public string Id => CommandIds.ShowMapGrid;
    public Shortcut[] ActivationKeys { get; set; } = { new(Key.G, ctrl: true) };
    public CommandStatus Status => CommandStatus.Normal;

    public bool Update()
    {
        return true;
    }

    public void Action()
    {
        gameScreen.ShowMapGrid();
    }

    public bool Checked => gameScreen.ShowGrid;
    public MenuCommand? Command { get; set; }
    public string ErrorDialog => string.Empty;
    public DialogImageElements? ErrorImage => null;
    public string? Name => "Show Map Grid";
}