using Civ2engine;
using Civ2engine.Enums;
using JetBrains.Annotations;
using Model;
using Model.Controls;
using Model.Core.Cities;
using Model.Core.Units;
using Model.Input;

namespace RaylibUI.RunGame.Commands.Orders;

[UsedImplicitly]
public class SetHomeCityOrder(GameScreen gameScreen) : Order(gameScreen, new Shortcut(Key.H), "UNIT_ORDER_SET_HOME_CITY")
{
    public override bool Update()
    {
        var unit = GameScreen.Player.ActiveUnit;
        if (unit == null || unit.Dead)
        {
            return SetCommandState(CommandStatus.Invalid);
        }

        var city = GetEligibleHomeCity(unit);
        if (city == null)
        {
            return SetCommandState(CommandStatus.Disabled);
        }

        return SetCommandState(CommandStatus.Normal);
    }

    public override void Action()
    {
        var unit = GameScreen.Player.ActiveUnit;
        if (unit == null || unit.Dead)
        {
            return;
        }

        var newHomeCity = GetEligibleHomeCity(unit);
        if (newHomeCity == null)
        {
            return;
        }

        var oldHomeCity = unit.HomeCity;
        unit.HomeCity = newHomeCity;
        unit.Order = (int)OrderType.NoOrders;
        unit.WaitOrder = false;

        RecalculateSupport(oldHomeCity);
        RecalculateSupport(newHomeCity);

        GameScreen.StatusPanel.Update();
        GameScreen.ForceRedraw();
    }

    private City? GetEligibleHomeCity(Unit unit)
    {
        var city = unit.CurrentLocation.CityHere;
        if (city == null || city.Owner != unit.Owner || city == unit.HomeCity)
        {
            return null;
        }

        return city;
    }

    private void RecalculateSupport(City? city)
    {
        if (city == null)
        {
            return;
        }

        var government = GameScreen.Game.Rules.Governments[city.Owner.Government];
        city.SetUnitSupport(government);
        city.CalculateOutput(city.Owner.Government, GameScreen.Game);
    }
}
