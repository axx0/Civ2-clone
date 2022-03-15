using System;
using System.Linq;
using Civ2engine.Units;
using Civ2engine.Enums;
using Civ2engine.Events;

namespace Civ2engine
{
    public partial class Game : BaseInstance
    {
        public event EventHandler<MapEventArgs> OnMapEvent;
        public event EventHandler<UnitEventArgs> OnUnitEvent;

        // Choose next unit for orders. If all units ended turn, update cities.
        public void ChooseNextUnit(bool startOfTurn = false)
        {
            Unit nextUnit = null;
            var units = _activeCiv.Units.Where(u=> !u.Dead).ToList();
            if (_activeUnit != null)
            {
                //Look for units on this square or neighbours of this square
                var activeTile = _activeUnit.Dead
                    ? _maps[_currentMap].TileC2(_activeUnit.X, _activeUnit.Y)
                    : _activeUnit.CurrentLocation;
                nextUnit = activeTile.UnitsHere.FirstOrDefault(u => u.AwaitingOrders) ?? _maps[_currentMap]
                    .Neighbours(activeTile)
                    .SelectMany(t => t.UnitsHere.Where(u => u.Owner == _activeCiv && u.AwaitingOrders))
                    .FirstOrDefault();
            }

            if (nextUnit == null)
            {
                nextUnit = units.FirstOrDefault(u => u.AwaitingOrders);
            }

            // End turn if no units awaiting orders
            if (nextUnit == null && !startOfTurn)
            {
                if (Options.AlwaysWaitAtEndOfTurn && _activeCiv.PlayerType != PlayerType.AI)
                {
                    OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.WaitAtEndOfTurn));
                }
                else
                {
                    ChoseNextCiv();
                }
            }
            // Choose next unit
            else
            {
                _activeUnit = nextUnit;
                if (_activeUnit != null)
                {
                    TriggerUnitEvent(new ActivationEventArgs(_activeUnit, false, false));
                }
            }
        }
    }
}
