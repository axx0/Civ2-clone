using System;
using System.Linq;
using Civ2engine.Units;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;

namespace Civ2engine
{
    public partial class Game
    {
        public event EventHandler<MapEventArgs> OnMapEvent;
        public event EventHandler<UnitEventArgs> OnUnitEvent;

        // Choose next unit for orders. If all units ended turn, update cities.
        public void ChooseNextUnit()
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

            nextUnit ??= units.FirstOrDefault(u => u.AwaitingOrders);

            // End turn if no units awaiting orders
            if (nextUnit == null)
            {
                var anyUnitsMoved = units.Any(u => u.MovePointsLost > 0);
                if ((!anyUnitsMoved || Options.AlwaysWaitAtEndOfTurn) && _activeCiv.PlayerType != PlayerType.AI)
                {
                    OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.WaitAtEndOfTurn));
                }
                else
                {
                    foreach (var unit in _activeCiv.Units.Where(u=>u.MovePoints > 0))
                    {
                        unit.ProcessOrder();
                        CheckConstruction(unit.CurrentLocation, unit.Order);
                    }
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

        public void CheckConstruction(Tile tile, OrderType order)
        {
            var units = tile.UnitsHere.Where(u => u.Order == order).ToList();
            if (units.Count > 0)
            {
                var progress = units.Sum(u => u.Counter);

                int timeToComplete;
                switch (order)
                {
                    case OrderType.Transform:
                        timeToComplete = Rules.Cosmic.BaseTimeEngineersTransform;
                        break;
                    case OrderType.BuildIrrigation:
                        timeToComplete = tile.Terrain.TurnsToIrrigate;
                        break;
                    case OrderType.BuildMine:
                        timeToComplete = tile.Terrain.TurnsToMine;
                        break;
                    default:
                        timeToComplete = tile.EffectiveTerrain.MoveCost * 2;
                        break;
                }

                if (progress >= timeToComplete)
                {
                    tile.CompleteConstruction(order, Rules);
                    units.ForEach(u=>
                    {
                        u.Counter = 0;
                        u.Order = OrderType.NoOrders;
                    });
                }
            }
        }
    }
}
