using Model;
using Model.Dialog;
using Model.Menu;

namespace RaylibUI.RunGame.Commands;

public abstract class AlwaysOnCommand(GameScreen gameScreen, string commandId, Shortcut[] defaultKeys)
    : IGameCommand
{
    protected readonly GameScreen GameScreen = gameScreen;

    public string Id { get; } = commandId;

    public Shortcut[] ActivationKeys { get; set; } = defaultKeys;
    public CommandStatus Status => CommandStatus.Normal;
    public MenuCommand? Command { get; set; }
    public string ErrorDialog { get; } = string.Empty;
    public DialogImageElements? ErrorImage { get; } = null;
    public string? Name { get; }

    public bool Update()
    {
        return GameScreen.ActiveMode != GameScreen.Processing;
    }

    public abstract void Action();
}