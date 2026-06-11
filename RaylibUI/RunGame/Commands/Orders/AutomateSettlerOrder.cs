using Civ2engine.Enums;
using JetBrains.Annotations;
using Model;
using Model.Constants;
using Model.Controls;
using Model.Input;

namespace RaylibUI.RunGame.Commands.Orders;

[UsedImplicitly]
public class AutomateSettlerOrder(GameScreen gameScreen)
    : Order(gameScreen, new Shortcut(Key.K), CommandIds.AutomateSettlerOrder)
{
    public override bool Update()
    {
        var activeUnit = GameScreen.Player.ActiveUnit;
        if (activeUnit == null)
        {
            return SetCommandState(CommandStatus.Invalid);
        }

        if (activeUnit.AiRole != AiRoleType.Settle)
        {
            return SetCommandState(CommandStatus.Invalid, errorPopupKeyword: "ONLYSETTLERS");
        }

        return SetCommandState(CommandStatus.Normal);
    }

    public override void Action()
    {
        var activeUnit = GameScreen.Player.ActiveUnit;
        if (activeUnit == null)
        {
            return;
        }

        activeUnit.Order = (int)OrderType.Automate;
        activeUnit.WaitOrder = true;
        activeUnit.MovePointsLost = activeUnit.MovePoints;
        GameScreen.Game.ChooseNextUnit();
    }
}
