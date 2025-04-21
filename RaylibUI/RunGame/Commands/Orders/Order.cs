using Civ2engine.MapObjects;
using Civ2engine.Units;
using Model;
using Model.Images;
using Model.Menu;
using Model.Dialog;
using Raylib_CSharp;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI.RunGame.GameModes.Orders;

public abstract class Order : IGameCommand
{
    public Shortcut[] ActivationKeys { get; set; }
    protected readonly GameScreen GameScreen;

    protected Order(GameScreen gameScreen, Shortcut keyCombo, string id, string? name = null)
    {
        ActivationKeys = new[] { keyCombo};
        Id = id;
        Name = name;
        GameScreen = gameScreen;

        ErrorDialog = "CANTDO";
    }

    public string ErrorDialog { get; set; }
    public string? Name { get; }
    public DialogImageElements? ErrorImage { get; set; }

    public MenuCommand? Command { get; set; }
    
    public string Id { get; }

    public bool Checked => false;

    public abstract bool Update();

    public abstract void Action();

    public CommandStatus Status { get; set; }


    protected bool SetCommandState(CommandStatus status = CommandStatus.Disabled, string? menuText = null,
        string? errorPopupKeyword = null, DialogImageElements? errorPopupImage = null)
    {
        if (GameScreen.ActiveMode != GameScreen.Moving)
        {
            status = CommandStatus.Invalid;
            menuText = null;
        }
        if (Command != null)
        {
            Command.MenuText = menuText;
        }

        Status = status;
        ErrorDialog = !string.IsNullOrWhiteSpace(errorPopupKeyword) ? errorPopupKeyword : "CANTDO";
        ErrorImage = errorPopupImage;

        return Status <= CommandStatus.Normal;
    }
}
