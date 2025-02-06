using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Units;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;

namespace Civ2engine
{
    public partial class Game
    {
        public event EventHandler<MapEventArgs> OnMapEvent;
        public event EventHandler<UnitEventArgs> OnUnitEvent;
        internal event EventHandler<CivEventArgs> OnCivEvent;

        private readonly int[] _doNothingOrders = { (int)OrderType.Fortified, (int)OrderType.Sleep };

        // Choose next unit for orders. If all units ended turn, update cities.
        public void ChooseNextUnit()
        {
            var units = _activeCiv.Units.Where(u => !u.Dead).ToList();

            var player = Players[_activeCiv.Id];
            
            //Look for units on this square or neighbours of this square
            
            var nextUnit = NextUnit(player, units);
            
            // End turn if no units awaiting orders
            if (nextUnit == null)
            {
                var anyUnitsMoved = units.Any(u => u.MovePointsLost > 0);
                if ((!anyUnitsMoved || Options.AlwaysWaitAtEndOfTurn))
                {
                    Players[_activeCiv.Id].WaitingAtEndOfTurn(this);
                }
                else
                {
                    if (ProcessEndOfTurn())
                    {
                        ChoseNextCiv();
                    }
                }
            }
            else
            {
                player.SetUnitActive(nextUnit, true);
                // If the player immediately moved the unit it might be already dead or moved so choose again
                if (nextUnit.Dead || nextUnit.MovePointsLost == nextUnit.MaxMovePoints)
                {
                    ChooseNextUnit();
                }
            }
        }

        private Unit? NextUnit(IPlayer player, List<Unit> units)
        {
            if (player.WaitingList is { Count: > 0 })
            {
                return
                    ActiveTile.UnitsHere.FirstOrDefault(u => u.AwaitingOrders && !player.WaitingList.Contains(u)) ??
                    ActiveTile
                        .Neighbours()
                        .SelectMany(
                            t => t.UnitsHere.Where(u =>
                                u.Owner == _activeCiv && u.AwaitingOrders && !player.WaitingList.Contains(u)))
                        .FirstOrDefault() ??
                    units.FirstOrDefault(u => u.AwaitingOrders && !player.WaitingList.Contains(u)) ??
                    ResetWaiting(player);

            }

            return ActiveTile.UnitsHere.FirstOrDefault(u => u.AwaitingOrders) ??
                   ActiveTile
                       .Neighbours()
                       .SelectMany(
                           t => t.UnitsHere.Where(u => u.Owner == _activeCiv && u.AwaitingOrders))
                       .FirstOrDefault() ?? units.FirstOrDefault(u => u.AwaitingOrders);

        }

        private Unit ResetWaiting(IPlayer player)
        {
            var unit = player.WaitingList[0];
            player.WaitingList.Clear();
            return unit;
        }

        public bool ProcessEndOfTurn()
        {
            var player = Players[_activeCiv.Id];
            foreach (var unit in _activeCiv.Units)
            {
                if (unit is { MovePoints: > 0, CurrentLocation: not null } && !_doNothingOrders.Contains(unit.Order))
                {
                    switch ((OrderType)unit.Order)
                    {
                        case OrderType.Fortify:
                            unit.Order = (int)OrderType.Fortified;
                            unit.MovePointsLost = unit.MovePoints;
                            break;
                        case OrderType.GoTo:
                            if (unit.CurrentLocation.Map.IsValidTileC2(unit.GoToX, unit.GoToY))
                            {
                                var tile = unit.CurrentLocation.Map.TileC2(unit.GoToX, unit.GoToY);
                                var path = Path.CalculatePathBetween(this, unit.CurrentLocation, tile, unit.Domain, unit.MaxMovePoints, unit.Owner, unit.Alpine, unit.IgnoreZonesOfControl);
                                path?.Follow(this, unit);
                            }

                            if (unit.MovePoints >= 0)
                            {
                                player.SetUnitActive(unit, true);
                                return false;
                            }

                            break;
                        default:
                        {
                            unit.ProcessOrder();

                            if (TerrainImprovements.TryGetValue(unit.Building, out var improvement))
                            {
                                var activeUnit = this.CheckConstruction(unit.CurrentLocation, improvement)
                                    .FirstOrDefault(u => u.MovePoints > 0);
                                if (activeUnit != null)
                                {
                                    player.SetUnitActive(activeUnit, true);
                                    return false;
                                }
                            }

                            break;
                        }
                    }
                }
            }

            return true;
        }
    }
}
