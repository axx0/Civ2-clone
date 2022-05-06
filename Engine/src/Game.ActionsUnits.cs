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

        private OrderType[] _doNothingOrders = new[] { OrderType.Fortified, OrderType.Sleep };

        // Choose next unit for orders. If all units ended turn, update cities.
        public void ChooseNextUnit()
        {
            var units = _activeCiv.Units.Where(u => !u.Dead).ToList();

            var player = Players[_activeCiv.Id];
            
            //Look for units on this square or neighbours of this square
            
            var nextUnit = NextUnit(player, units);

            ActiveUnit = nextUnit;

            // End turn if no units awaiting orders
            if (nextUnit == null)
            {
                var anyUnitsMoved = units.Any(u => u.MovePointsLost > 0);
                if ((!anyUnitsMoved || Options.AlwaysWaitAtEndOfTurn) && _activeCiv.PlayerType != PlayerType.AI)
                {
                    OnPlayerEvent?.Invoke(null, new PlayerEventArgs(PlayerEventType.WaitingAtEndOfTurn, _activeCiv.Id));
                }
                else
                {
                    if (ProcessEndOfTurn())
                    {
                        ChoseNextCiv();
                        return;
                    }
                }
            }
        }

        private Unit NextUnit(IPlayer player, List<Unit> units)
        {
            Unit nextUnit;
            if (player.WaitingList is { Count: > 0 })
            {
                nextUnit =
                    ActiveTile.UnitsHere.FirstOrDefault(u => u.AwaitingOrders && !player.WaitingList.Contains(u)) ??
                    CurrentMap
                        .Neighbours(ActiveTile)
                        .SelectMany(
                            t => t.UnitsHere.Where(u => u.Owner == _activeCiv && u.AwaitingOrders && !player.WaitingList.Contains(u)))
                        .FirstOrDefault();

                nextUnit ??= units.FirstOrDefault(u => u.AwaitingOrders && !player.WaitingList.Contains(u));
                if (nextUnit == null && player.WaitingList.Count > 0)
                {
                    nextUnit = player.WaitingList[0];
                    player.WaitingList.Clear();
                }
            }
            else
            {
                nextUnit =
                    ActiveTile.UnitsHere.FirstOrDefault(u => u.AwaitingOrders) ??
                    CurrentMap
                        .Neighbours(ActiveTile)
                        .SelectMany(
                            t => t.UnitsHere.Where(u => u.Owner == _activeCiv && u.AwaitingOrders))
                        .FirstOrDefault();

                nextUnit ??= units.FirstOrDefault(u => u.AwaitingOrders);
            }

            return nextUnit;
        }

        public bool ProcessEndOfTurn()
        {
            foreach (var unit in _activeCiv.Units.Where(u =>
                         u.MovePoints > 0 && !_doNothingOrders.Contains(u.Order)))
            {
                if (unit.Order == OrderType.Fortify)
                {
                    unit.Order = OrderType.Fortified;
                    unit.MovePointsLost = unit.MovePoints;
                }
                else
                {
                    unit.ProcessOrder();
                    
                    if (TerrainImprovements.ContainsKey(unit.Building))
                    {
                        ActiveUnit = CheckConstruction(unit.CurrentLocation, TerrainImprovements[unit.Building])
                            .FirstOrDefault(u => u.MovePoints > 0);
                        if (ActiveUnit != null)
                        {
                            return false;
                        }
                    }
                }
            }

            return ActiveUnit == null;
        }

        public List<Unit> CheckConstruction(Tile tile, TerrainImprovement improvement)
        {
            var units = tile.UnitsHere.Where(u => u.Building == improvement.Id).ToList();
            if (units.Count <= 0) return units;
            
            var terrain = improvement.AllowedTerrains[tile.Z].FirstOrDefault(t => t.TerrainType == (int)tile.Type);
            var existingImprovement = tile.Improvements.FirstOrDefault(i => i.Improvement == improvement.Id);
            
            if (terrain == null || (improvement.Negative && existingImprovement == null) ||
                (existingImprovement?.Level == improvement.Levels.Count - 1))
            {
                //If improvement has become invalid for terrain then return the units to the user
                units.ForEach(u =>
                {
                    u.Counter = 0;
                    u.Order = OrderType.NoOrders;
                    u.Building = 0;
                });
                return units;
            }

            int levelToBuild;
            if (existingImprovement != null)
            {
                if (!improvement.Negative)
                {
                    levelToBuild = existingImprovement.Level;
                }
                else
                {
                    levelToBuild = existingImprovement.Level + 1;
                }
            }
            else
            {
                levelToBuild = 0;
            }

            var progress = units.Sum(u => u.Counter);
            var cost = terrain.BuildTime;
            if (tile.River)
            {
                var river = improvement.AllowedTerrains[tile.Z]
                    .FirstOrDefault(t => t.TerrainType == TerrainConstants.River);
                if (river != null)
                {
                    cost += river.BuildTime;
                }
            }
            if (improvement.Levels[levelToBuild].BuildCostMultiplier != 0)
            {
                cost += cost * improvement.Levels[levelToBuild].BuildCostMultiplier / 100;
            }
            if (progress < cost)
            {
                return new List<Unit>();
            }

            if (improvement.Negative)
            {
                tile.RemoveImprovement(improvement, levelToBuild);
            }
            else
            {
                tile.AddImprovement(improvement, terrain, levelToBuild, Rules.Terrains[tile.Z]);
            }

            units.ForEach(u =>
            {
                u.Counter = 0;
                u.Order = OrderType.NoOrders;
                u.Building = 0;
            });

            var tiles = new List<Tile> { tile };
            tiles.AddRange(tile.Map.Neighbours(tile));
            TriggerMapEvent(MapEventType.UpdateMap, tiles);

            return units;
        }
    }
}
