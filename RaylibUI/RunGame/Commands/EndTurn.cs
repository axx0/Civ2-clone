using JetBrains.Annotations;
using Model.Input;
using Model;
using Model.Controls;

namespace RaylibUI.RunGame.Commands;

[UsedImplicitly]
public class EndTurn(GameScreen gameScreen) : IGameCommand
{
    public string Id => CommandIds.EndTurn;
    
    public Shortcut[] ActivationKeys { get; set; } = [new (Key.Enter), new(Key.N, ctrl: true)];
    public CommandStatus Status { get; private set; }

    public bool Update()
    {
        Status = gameScreen.ActiveMode == gameScreen.ViewPiece || gameScreen.ActiveMode == gameScreen.Moving
            ? CommandStatus.Priority
            : CommandStatus.Disabled;
        return Status != CommandStatus.Disabled;
    }

    public void Action()
    {
        if (gameScreen.Game.ProcessEndOfTurn())
        {
            gameScreen.Game.ChoseNextCiv();
        }
    }

    public bool Checked => false;
    public MenuCommand? Command { get; set; }
    public string ErrorDialog => string.Empty;
    public DialogImageElements? ErrorImage => null;
    public string? Name => "End Turn";
}