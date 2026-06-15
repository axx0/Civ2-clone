using Civ2engine;
using Model.Input;
using Model.Controls;
using Model.Core.Mapping;
using Model.Core.Units;

namespace RaylibUI.RunGame.Commands.Cheat;

public class DestroyUnitsAtCursor(GameScreen gameScreen) : AlwaysOnCommand(gameScreen, CommandIds.CheatDestroyAllUnitsAtCursor, [new Shortcut(Key.D, ctrl: true, shift: true)])
{
    public override void Action()
    {
        Tile cursorTile = ((Game)GameScreen.Game).ActiveTile;
        // Note that this can't just be:
        //   cursorTile.UnitsHere.forEach(u => u.Dead = true)
        // because this would mutate UnitsHere while we're iterating it!
        // So we have to copy into a temp list, and iterate over that.
        List<Unit> unitsToDelete = new List<Unit>();
        cursorTile.UnitsHere.ForEach(unitsToDelete.Add);
        unitsToDelete.ForEach(unit => unit.Dead = true);
        GameScreen.StatusPanel.Update();
        GameScreen.TileCache.Clear();
        GameScreen.MapControl.ForceRedraw = true;
    }
}