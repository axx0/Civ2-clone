using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.IO;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Civ2engine.Units;
using Raylib_cs;

namespace RaylibUI.RunGame.GameModes.Orders;

public class ImprovementOrder : Order
{
    private readonly TerrainImprovement _improvement;
    private readonly Game _game;

    public ImprovementOrder(TerrainImprovement improvement, GameScreen gameScreen, Game game, MenuElement menuElement) : 
        base(gameScreen, ParseShortCut(improvement.Shortcut), 
        menuElement?.MenuText ?? TerrainImprovementFunctions.LabelFrom(improvement.Levels[0]), improvement.MenuGroup)
    {
        _improvement = improvement;
        _game = game;
    }

    private static KeyboardKey ParseShortCut(string improvementShortcut)
    {
        var result = KeyboardKey.KEY_NULL;
        foreach (var keyString in improvementShortcut.Split("|"))
        {
            if (keyString.Length == 1)
            {
                result |= GetKeys(keyString.ToUpperInvariant());
            }
            else
            {
                result |= GetKeys(keyString);
            }
        }

        return result;
    }

    private static KeyboardKey GetKeys(string keyString)
    {
        if (Enum.TryParse(typeof(KeyboardKey), keyString, out var parsed))
        {
            return (KeyboardKey)parsed;
        }

        return KeyboardKey.KEY_NULL;
    }


    public override Order Update(Tile activeTile, Unit activeUnit)
    {
        if (activeUnit == null)
        {
            SetCommandState(OrderStatus.Illegal);
            return this;
        }

        if (activeUnit.AIrole != AIroleType.Settle)
        {
            SetCommandState(OrderStatus.Illegal, errorPopupKeyword: "ONLYSETTLERS");
            return this;
        }

        var canBeBuilt = TerrainImprovementFunctions.CanImprovementBeBuiltHere(activeTile, _improvement, activeUnit.Owner);

        SetCommandState(canBeBuilt.Enabled ? OrderStatus.Active : OrderStatus.Disabled, canBeBuilt.CommandTitle, canBeBuilt.ErrorPopup);
        return this;
    }

    protected override void Execute(LocalPlayer player)
    {
        player.ActiveUnit.Build(_improvement);
        _game.CheckConstruction(player.ActiveTile, _improvement);
        _game.ChooseNextUnit();
    }
}