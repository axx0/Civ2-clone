using Civ2engine;
using Model.Input;
using Civ2engine.IO;
using Model.Core;
using Model.Controls;
using Model.Core.Mapping;

namespace RaylibUI.RunGame.Commands.Cheat;

// TODO: For parity with civ2, this needs a second dialog for editing terrain features (improvements, river, etc.) not just terrain type.
public class ChangeTerrainAtCursor(GameScreen gameScreen) : AlwaysOnCommand(gameScreen, CommandIds.CheatChangeTerrainAtCursor, [new Shortcut(Key.F8, shift: true)])
{
    private Tile? cursorTile;
    private CivDialog? _selectNewTerrainKind;
    private Terrain[]? terrains;

    public override void Action()
    {
        cursorTile = ((Game)GameScreen.Game).ActiveTile;
        var mapNumber = cursorTile.Map.MapIndex;
        terrains = GameScreen.Game.Rules.Terrains[mapNumber];
        var currentTerrain = cursorTile.Terrain;
        
        var terrainSelection = new PopupBox
        {
            Title = "SelectTerrainType",
            Options = terrains.Select(t => t.Type.ToString()).ToArray(),
            Default = Array.IndexOf(terrains, currentTerrain),
            Button = [Labels.Ok, Labels.Cancel]
        };
        
        _selectNewTerrainKind = new CivDialog(GameScreen.Main, new DialogElements(terrainSelection), ChangeTerrainHandler);

        GameScreen.ShowDialog(_selectNewTerrainKind);
    }

    private void ChangeTerrainHandler(string button, int selection, IList<bool>? arg3, IDictionary<string, string>? arg4)
    {
        if (button == Labels.Ok && null != cursorTile && null != terrains)
        {
            cursorTile.Terrain = terrains[selection];
            GameScreen.StatusPanel.Update();
            GameScreen.TileCache.Clear();
            GameScreen.MapControl.ForceRedraw = true;
        }
        GameScreen.CloseDialog(_selectNewTerrainKind);
    }
}