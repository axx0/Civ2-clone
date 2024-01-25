using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Terrains;
using Model;
using Model.Menu;
using RaylibUI.RunGame.GameModes.Orders;

namespace RaylibUI.RunGame.Commands.Orders;

public class ImprovementOrder : Order
{
    private readonly TerrainImprovement _improvement;
    private readonly Game _game;
    private readonly LocalPlayer _player;

    public ImprovementOrder(TerrainImprovement improvement, GameScreen gameScreen, Game game) :
        base(gameScreen, Shortcut.Parse(improvement.Shortcut), GetCommandName(improvement), improvement.Name)
    {
        _improvement = improvement;
        _game = game;
        _player = gameScreen.Player;
    }

    public override void Update()
    {
        if (_player.ActiveUnit == null)
        {
            SetCommandState(CommandStatus.Invalid);
            return;
        }

        if (_player.ActiveUnit.AIrole != AIroleType.Settle)
        {
            SetCommandState(CommandStatus.Invalid, errorPopupKeyword: "ONLYSETTLERS");
        }

        var canBeBuilt = TerrainImprovementFunctions.CanImprovementBeBuiltHere(_player.ActiveTile, _improvement, _player.ActiveUnit.Owner);

        SetCommandState(canBeBuilt.Enabled ? CommandStatus.Normal : CommandStatus.Disabled, canBeBuilt.CommandTitle, canBeBuilt.ErrorPopup);
    }

    public override void Action()
    {
        _player.ActiveUnit?.Build(_improvement);
        _game.CheckConstruction(_player.ActiveTile, _improvement);
        _game.ChooseNextUnit();
    }

    private static string GetCommandName(TerrainImprovement improvement)
    {
        var baseId = CommandIds.BuildImprovementOrderNormal;
        if (improvement.Negative)
        {
            baseId = CommandIds.RemoveNegativeImprovementOrder;
        }else if (improvement.Foreground)
        {
            baseId = CommandIds.BuildImprovementOrderForeground;
        }
        return baseId + "_" + improvement.Name.ToUpperInvariant();
    }
}