using Civ2engine.MapObjects;
using Civ2engine.Units;
using Model;
using Model.Menu;
using Raylib_cs;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI.RunGame.GameModes.Orders;

public abstract class Order : IGameCommand
{
    public Shortcut KeyCombo { get; set; }
    protected readonly GameScreen _gameScreen;

    protected Order(GameScreen gameScreen, Shortcut keyCombo, string id, string? name = null)
    {
        KeyCombo = keyCombo;
        Id = id;
        Name = name;
        _gameScreen = gameScreen;

        ErrorDialog = "CANTDO";
    }

    public string ErrorDialog { get; set; }
    public string? Name { get; }

    public MenuCommand? Command { get; set; }
    
    public string Id { get; }

    public abstract void Update();

    public abstract void Action();

    public CommandStatus Status { get; set; }


    protected void SetCommandState(CommandStatus status = CommandStatus.Disabled, string? menuText = null,
        string? errorPopupKeyword = null)
    {
        if (_gameScreen.ActiveMode != _gameScreen.Moving)
        {
            status = CommandStatus.Invalid;
            menuText = null;
        }
        if (Command != null)
        {
            Command.MenuText = menuText;
            Command.Enabled = status <= CommandStatus.Normal;
        }

        Status = status;
        ErrorDialog = !string.IsNullOrWhiteSpace(errorPopupKeyword) ? errorPopupKeyword : "CANTDO";
    }
}
