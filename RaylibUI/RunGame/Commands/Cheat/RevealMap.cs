using Model.Input;
using Civ2engine;
using Civ2engine.MapObjects;
using Model.Controls;
using JetBrains.Annotations;

namespace RaylibUI.RunGame.Commands.Cheat;

[UsedImplicitly]
public class RevealMap(GameScreen gameScreen) : AlwaysOnCommand(gameScreen, CommandIds.CheatRevealMapCommand, [new Shortcut(Key.F2, shift: true)])
{
    private CivDialog? _revealMapDialog;
    private Tile? _cachedActive;

    public override void Action()
    {
        var allLabel = Labels.For(LabelIndex.EntireMap);
        var noSpecial = Labels.For(LabelIndex.NoSpecialView);
        
        _revealMapDialog = new CivDialog(GameScreen.Main, new DialogElements(new PopupBox
        {
            Title = "Reveal Map",
            Options = GameScreen.Game.AllCivilizations.Select(c=>c.TribeName).Concat<string>([noSpecial, allLabel]).ToArray(),
            Button = [Labels.Ok, Labels.Cancel]
        }), HandleButtonClick );
        
        GameScreen.ShowDialog(_revealMapDialog);
    }

    private void HandleButtonClick(string button, int selection, IList<bool>? arg3, IDictionary<string, string>? arg4)
    {
        if (button == Labels.Ok && (selection != GameScreen.VisibleCivId || GameScreen.CurrentMap.MapRevealed))
        {
            foreach (var map in GameScreen.Game.Maps)
            {
                map.MapRevealed = selection > GameScreen.Game.AllCivilizations.Count;
            }
            GameScreen.ActiveMode = GameScreen.ViewPiece;
        
            var civId = selection < GameScreen.Game.AllCivilizations.Count ? selection : GameScreen.Player.Civilization.Id;
            
            _cachedActive ??= GameScreen.Player.ActiveTile;

            if (civId == GameScreen.Player.Civilization.Id)
            {
                GameScreen.Player.ActiveTile = _cachedActive;
                _cachedActive = null;
            }
            else
            {
                GameScreen.Player.ActiveTile = GameScreen.Game.Players[civId].ActiveTile;
            }
            
            GameScreen.VisibleCivId = civId;
            GameScreen.TileCache.Clear();
            GameScreen.MapControl.ForceRedraw = true;
        }
        GameScreen.CloseDialog(_revealMapDialog);
    }
}