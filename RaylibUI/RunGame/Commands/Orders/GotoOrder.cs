using System.Collections.Generic;
using System.Linq;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.UnitActions.Move;
using Civ2engine.Units;
using Model;
using Model.Core;
using Model.Menu;
using Raylib_cs;

namespace RaylibUI.RunGame.GameModes.Orders;

public class GotoOrder : Order
{
    private readonly IGame _game;
    private readonly LocalPlayer _player;

    public GotoOrder(GameScreen gameScreen) : 
        base(gameScreen, new Shortcut(KeyboardKey.G), CommandIds.GotoOrder)
    {
        _game = GameScreen.Game;
        _player = GameScreen.Player;
    }

    public override bool Update()
    {
        return SetCommandState(_player.ActiveUnit != null ? CommandStatus.Normal : CommandStatus.Invalid);
    }

    public override void Action()
    {
        ////_mainForm.CurrentGameMode = new Goto(_game);
        //var popup = _mainForm.popupBoxList["GOTO"];
        //var activeUnit = player.ActiveUnit;
        //var islands = MovementFunctions.GetIslandsFor(activeUnit);
        //var cities = player.Civ.Cities.Where(c =>
        //        islands.Contains(c.Location.Island) || c.Location.Neighbours().Any(l => islands.Contains(l.Island)))
        //    .ToList();
        //var dialog = new Civ2dialog(_mainForm, popup, new List<string> { activeUnit.Name },
        //    listbox: new ListboxDefinition
        //    {
        //        LeftText = cities.Select(c => c.Name).ToList()
        //    });
        //dialog.ShowModal();
        //if (dialog.SelectedButton == Labels.Ok)
        //{
            
        //    var city = cities[dialog.SelectedIndex];
        //    var path = Path.CalculatePathBetween(_game, player.ActiveTile, city.Location, activeUnit.Domain,
        //        activeUnit.MaxMovePoints, activeUnit.Owner, activeUnit.Alpine, activeUnit.IgnoreZonesOfControl);
        //    if (path != null)
        //    {
        //        activeUnit.Order = OrderType.GoTo;
        //        activeUnit.GoToX = city.Location.X;
        //        activeUnit.GoToY = city.Location.Y;
        //        path.Follow(_game, activeUnit);
        //        if (activeUnit.MovePoints <= 0)
        //        {
        //            _game.ChooseNextUnit();
        //        }
        //    }
        //}
        
    }
}