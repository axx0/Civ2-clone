using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Units;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Civ2engine.UnitActions;
using Model.Constants;
using Model.Core.Mapping;
using Model.Core.Player;
using Model.Core.Units;

namespace Civ2engine
{
    public partial class Game
    {
        /// <summary>
        /// This is now only used for lua script integration for other events raise them on the player version
        /// </summary>
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
                    Players[_activeCiv.Id].WaitingAtEndOfTurn();
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
                //TODO: determine the true values of these extra props
                OnUnitEvent?.Invoke(this, new ActivationEventArgs(unit: nextUnit, userInitiated: true, reactivation: false));
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
                        case OrderType.Automate:
                            ProcessAutomatedSettler(unit);
                            break;
                        default:
                        {
                            unit.ProcessOrder();

                            if (TerrainImprovements.TryGetValue(unit.Building, out var improvement))
                            {
                                var completedUnits = this.CheckConstruction(unit.CurrentLocation, improvement);
                                foreach (var completedUnit in completedUnits.Where(u => u.WaitOrder && u.AiRole == AiRoleType.Settle))
                                {
                                    completedUnit.Order = (int)OrderType.Automate;
                                    completedUnit.Building = 0;
                                    completedUnit.SkipTurn();
                                }

                                var activeUnit = completedUnits.FirstOrDefault(u => u.MovePoints > 0 && !u.WaitOrder);
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

        private void ProcessAutomatedSettler(Unit unit)
        {
            if (unit.AiRole != AiRoleType.Settle || unit.CurrentLocation == null)
            {
                unit.SkipTurn();
                return;
            }

            var currentTile = unit.CurrentLocation;
            if (TryAssignAutomatedSettlerJob(unit, currentTile))
            {
                return;
            }

            var nearbyCity = currentTile.CityRadius().FirstOrDefault(t => t.CityHere != null)?.CityHere;
            var workTile = FindAutomatedSettlerWorkTile(unit, currentTile, nearbyCity?.Location);
            if (workTile != null)
            {
                MovementFunctions.MoveC2(this, unit, workTile.X - currentTile.X, workTile.Y - currentTile.Y);
                if (!unit.Dead)
                {
                    unit.Order = (int)OrderType.Automate;
                }
                return;
            }

            if (nearbyCity == null && currentTile.Type != TerrainType.Ocean)
            {
                var moreFertile = MovementFunctions.GetPossibleMoves(currentTile, unit)
                    .Where(t => t.Fertility > currentTile.Fertility && t.Type != TerrainType.Ocean)
                    .OrderByDescending(t => t.Fertility)
                    .FirstOrDefault();

                if (moreFertile != null)
                {
                    MovementFunctions.MoveC2(this, unit, moreFertile.X - currentTile.X, moreFertile.Y - currentTile.Y);
                    if (!unit.Dead)
                    {
                        unit.Order = (int)OrderType.Automate;
                    }
                }
                else
                {
                    CityActions.BuildCity(unit, this, CityActions.GetCityName(unit.Owner, this));
                }
                return;
            }

            unit.SkipTurn();
        }

        private bool TryAssignAutomatedSettlerJob(Unit unit, Tile currentTile)
        {
            var preferredImprovements = new[]
            {
                ImprovementTypes.Road,
                ImprovementTypes.Irrigation
            };

            foreach (var improvementId in preferredImprovements)
            {
                if (!CanAutomatedSettlerBuild(unit, currentTile, improvementId, out var improvement))
                {
                    continue;
                }

                unit.WaitOrder = true;
                unit.Build(improvement);
                return true;
            }

            return false;
        }

        private Tile? FindAutomatedSettlerWorkTile(Unit unit, Tile currentTile, Tile? nearbyCityTile)
        {
            var cityRadius = nearbyCityTile?.CityRadius().ToHashSet();
            return MovementFunctions.GetPossibleMoves(currentTile, unit)
                .Where(t => t.Type != TerrainType.Ocean && t.CityHere == null)
                .Where(t => cityRadius == null || cityRadius.Contains(t))
                .Where(t => CanAutomatedSettlerImprove(unit, t))
                .OrderByDescending(t => AutomatedSettlerWorkScore(unit, t))
                .FirstOrDefault();
        }

        private bool CanAutomatedSettlerImprove(Unit unit, Tile tile)
        {
            return CanAutomatedSettlerBuild(unit, tile, ImprovementTypes.Road, out _) ||
                   CanAutomatedSettlerBuild(unit, tile, ImprovementTypes.Irrigation, out _);
        }

        private int AutomatedSettlerWorkScore(Unit unit, Tile tile)
        {
            var score = (int)tile.Fertility;
            if (tile.WorkedBy != null)
            {
                score += 200;
            }

            if (CanAutomatedSettlerBuild(unit, tile, ImprovementTypes.Road, out _))
            {
                score += 100;
            }

            if (CanAutomatedSettlerBuild(unit, tile, ImprovementTypes.Irrigation, out _))
            {
                score += 80;
            }

            return score;
        }

        private bool CanAutomatedSettlerBuild(Unit unit, Tile tile, int improvementId, out TerrainImprovement improvement)
        {
            if (!TerrainImprovements.TryGetValue(improvementId, out var foundImprovement))
            {
                improvement = null!;
                return false;
            }

            improvement = foundImprovement;

            var selectedImprovement = improvement;

            if (selectedImprovement.ExclusiveGroup > 0 &&
                tile.Improvements.Any(i => i.Group == selectedImprovement.ExclusiveGroup && i.Improvement != selectedImprovement.Id))
            {
                return false;
            }

            return TerrainImprovementFunctions.CanImprovementBeBuiltHere(tile, selectedImprovement, unit.Owner).Enabled;
        }
    }
}
