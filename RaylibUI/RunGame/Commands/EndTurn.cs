using Model;
using Model.Dialog;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.Commands;

public class EndTurn : IGameCommand
{
    private readonly GameScreen _gameScreen;

    public EndTurn(GameScreen gameScreen)
    {
        _gameScreen = gameScreen;
    }

    public string Id => CommandIds.EndTurn;
    
    public Shortcut[] ActivationKeys { get; set; } = { new (KeyboardKey.Enter), new (KeyboardKey.KpEnter), new(KeyboardKey.N, ctrl: true)};
    public CommandStatus Status { get; private set; }

    public bool Update()
    {
        Status = _gameScreen.ActiveMode == _gameScreen.ViewPiece || _gameScreen.ActiveMode == _gameScreen.Moving
            ? CommandStatus.Priority
            : CommandStatus.Disabled;
        return Status != CommandStatus.Disabled;
    }

    public void Action()
    {
        if (_gameScreen.Game.ProcessEndOfTurn())
        {
            _gameScreen.Game.ChoseNextCiv();
        }
    }

    public MenuCommand? Command { get; set; }
    public string ErrorDialog { get; } = string.Empty;
    public DialogImageElements? ErrorImage { get; } = null;
    public string? Name { get; }
}