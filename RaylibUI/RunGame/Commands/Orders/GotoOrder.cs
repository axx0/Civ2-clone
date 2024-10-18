using System.Collections.Generic;
using System.Linq;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.UnitActions;
using Civ2engine.Units;
using Model;
using Model.Core;
using Model.Interface;
using Model.Menu;
using Raylib_CSharp.Interact;
using Path = Civ2engine.Units.Path;

namespace RaylibUI.RunGame.GameModes.Orders;

public class GotoOrder : Order
{
    private List<City> _cities;
    private bool _allCities;

    public GotoOrder(GameScreen gameScreen) : 
        base(gameScreen, new Shortcut(KeyboardKey.G), CommandIds.GotoOrder)
    {
    }

    public override bool Update()
    {
        return SetCommandState(GameScreen.Player.ActiveUnit != null ? CommandStatus.Normal : CommandStatus.Invalid);
    }

    public override void Action()
    {
        _allCities = false;
        var activeUnit = GameScreen.Player.ActiveUnit!;
        Show(GameScreen.Player.Civilization.Cities, activeUnit);
    }

    private void HandleButtonClick(string button, int index, IList<bool>? arg3, IDictionary<string, string>? arg4)
    {
        var activeUnit = GameScreen.Player.ActiveUnit!;
        if (button == Labels.Ok)
        {
            var city = _cities[index];
            var path = Path.CalculatePathBetween(GameScreen.Game, activeUnit.CurrentLocation!, city.Location,
                activeUnit.Domain,
                activeUnit.MaxMovePoints, activeUnit.Owner, activeUnit.Alpine, activeUnit.IgnoreZonesOfControl);
            if (path != null)
            {
                activeUnit.Order = (int)OrderType.GoTo;
                activeUnit.GoToX = city.Location.X;
                activeUnit.GoToY = city.Location.Y;
                path.Follow(GameScreen.Game, activeUnit);
                if (activeUnit.MovePoints <= 0)
                {
                    GameScreen.Game.ChooseNextUnit();
                }
            }
        }
        else
        {
            _allCities = !_allCities;
            var cities = _allCities ? GameScreen.Game.AllCities : GameScreen.Player.Civilization.Cities;
            Show(cities, activeUnit);
        }
    }

    private void Show(List<City> cities, Unit activeUnit)
    {
            var islands = MovementFunctions.GetIslandsFor(activeUnit);
        _cities = cities.Where(c => c.Location != activeUnit.CurrentLocation &&
                                    islands.Contains(c.Location.Island) ||
                                    c.Location.Neighbours().Any(l => islands.Contains(l.Island))).OrderBy(c=>c.Name)
            .ToList();
        GameScreen.ShowPopup("GOTO", handleButtonClick: HandleButtonClick,
            listBox: new ListBoxDefinition
                { Vertical = true, Entries = _cities.Select(c => new ListBoxEntry { LeftText = c.Name }).ToList() });
    }
}