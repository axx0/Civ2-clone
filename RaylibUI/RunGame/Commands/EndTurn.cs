using System.Diagnostics.CodeAnalysis;
using Model;
using Model.Menu;

namespace RaylibUI.RunGame.Commands;

public class EndTurn : IGameCommand
{
    private readonly GameScreen _gameScreen;

    public EndTurn(GameScreen gameScreen)
    {
        _gameScreen = gameScreen;
    }

    public string Id => CommandIds.EndTurn;
    public Shortcut KeyCombo { get; set; }
    public CommandStatus Status { get; private set; }

    public void Update()
    {
        Status = _gameScreen.ActiveMode == _gameScreen.ViewPiece || _gameScreen.ActiveMode == _gameScreen.Moving
            ? CommandStatus.Priority
            : CommandStatus.Disabled;
    }

    public void Action()
    {
        _gameScreen.Game.ProcessEndOfTurn();
    }

    public MenuCommand? Command { get; set; }
    public string ErrorDialog { get; } = string.Empty;
    public string? Name { get; }
}