using System.Collections.Generic;
using System.Linq;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Civ2engine.Units;
using Model;
using Model.Core;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame.GameModes.Orders;

public class PillageOrder : Order
{
    private readonly IGame _game;

    public PillageOrder(GameScreen gameScreen) : 
        base(gameScreen,  new Shortcut(KeyboardKey.P, shift:true), CommandIds.PillageOrder)
    {
        _game = GameScreen.Game;
    }

    public override bool Update()
    {
        var activeTile = GameScreen.Player.ActiveTile;
        var activeUnit = GameScreen.Player.ActiveUnit;
        if (activeTile.IsCityPresent)
        {
            return SetCommandState(CommandStatus.Invalid);
        }

        if (activeUnit == null || activeUnit.AttackBase == 0)
        {
            return SetCommandState(CommandStatus.Invalid);
        }

        if (activeTile.Improvements.Count > 0 &&
            activeTile.Improvements.Any(i => !GameScreen.Game.TerrainImprovements[i.Improvement].Negative))
        {
            return SetCommandState(CommandStatus.Normal);
        }

        return SetCommandState(CommandStatus.Invalid);
    }

    public override void Action()
    {
        var improvements = GameScreen.Player.ActiveTile.Improvements.Where(i =>
            _game.TerrainImprovements.ContainsKey(i.Improvement) &&
            !_game.TerrainImprovements[i.Improvement].Negative).ToList();
        ConstructedImprovement improvementToPillage = null;
        if (improvements.Count > 1)
        {
            // TODO: implement listbox _gameScreen.ShowPopup("PILLAGEWHAT", listbox: new ListboxDefinition
                                                           //{
                                                           //    LeftText = improvements
                                                           //        .Select(i => _game.TerrainImprovements[i.Improvement].Levels[i.Level].Name).ToList()
                                                           //});
            //var popup = _mainForm.popupBoxList[];
            //var dialog = new Civ2dialog(_mainForm, popup, );
            //dialog.ShowModal();
            //if (dialog.SelectedButton == "OK")
            //{
            //    improvementToPillage = improvements[dialog.SelectedIndex];
            //}
        }
        else
        {
            improvementToPillage = improvements.FirstOrDefault();
            Pillage(improvementToPillage);
        }
    }

    private void Pillage(ConstructedImprovement? improvementToPillage)
    {
        var player = GameScreen.Player;
        
        player.ActiveUnit.MovePointsLost += _game.Rules.Cosmic.MovementMultiplier;
            
        var improvement = _game.TerrainImprovements[improvementToPillage.Improvement];
        player.ActiveTile.RemoveImprovement(improvement,improvementToPillage.Level, player.ActiveTile.GetCivsVisibleTo(_game));
        var tiles = new List<Tile> { player.ActiveTile };
        if (improvement.HasMultiTile)
        {
            tiles.AddRange(player.ActiveTile.Neighbours());
        }
        _game.TriggerMapEvent(MapEventType.UpdateMap, tiles);
        if (player.ActiveUnit.MovePoints <= 0)
        {
            _game.ChooseNextUnit();
        }
    }
}