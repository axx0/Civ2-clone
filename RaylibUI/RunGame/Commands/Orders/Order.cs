using Model;
using Model.Dialog;
using Model.Menu;

namespace RaylibUI.RunGame.Commands.Orders;

public abstract class Order(GameScreen gameScreen, Shortcut keyCombo, string id, string? name = null)
    : IGameCommand
{
    public Shortcut[] ActivationKeys { get; set; } = [keyCombo];
    protected readonly GameScreen GameScreen = gameScreen;

    public string ErrorDialog { get; set; } = "CANTDO";
    public string? Name { get; } = name;
    public DialogImageElements? ErrorImage { get; private set; }

    public MenuCommand? Command { get; set; }
    
    public string Id { get; } = id;

    public bool Checked => false;

    public abstract bool Update();

    public abstract void Action();

    public CommandStatus Status { get; private set; }


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
