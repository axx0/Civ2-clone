using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Civ2engine.Production;
using Civ2engine.Scripting;
using Civ2engine.Scripting.ScriptObjects;
using Civ2engine.Scripting.UnitActions;
using Civ2engine.UnitActions;
using Civ2engine.Units;
using Model.Constants;
using Model.Core;
using Model.Core.Advances;
using Model.Core.Cities;
using Model.Core.GoodyHuts.Outcomes;
using Model.Core.Mapping;
using Model.Core.Player;
using Model.Core.Production;
using Model.Core.Units;
using Neo.IronLua;

namespace Civ2engine
{
    public class AiPlayer(int difficultyLevel, Civilization civilization, Tile tile0, Game game, AiInterface ai)
        : IPlayer
    {
        public AiInterface Ai { get; } = ai;
        public Civilization Civilization { get; } = civilization;

        public int DifficultyLevel { get; } = difficultyLevel;

        public void WeLoveTheKingCanceled(City city)
        {
        }

        public Tile ActiveTile { get; set; } = civilization.Units.FirstOrDefault()?.CurrentLocation ??
                                               civilization.Cities.FirstOrDefault()?.Location ?? tile0;

        public Unit? ActiveUnit { get; private set; }

        public List<Unit> WaitingList { get; } = [];

        public void CivilDisorder(City city)
        {
        }

        public void OrderRestored(City city)
        {
        }

        public void WeLoveTheKingStarted(City city)
        {
        }

        public void CantMaintain(City city, Improvement cityImprovement)
        {
        }

        public void SelectNewAdvance(List<Advance> researchPossibilities)
        {
            var res = Ai.Call(AiEvent.ResearchComplete,
                new LuaTable
                {
                    {
                        "researchPossibilities",
                        LuaTable.pack(researchPossibilities.Select(object (a) => new Tech(game.Rules.Advances, a.Index))
                            .ToArray())
                    }
                });
            if (res is not null && res.Count > 0)
            {
                Civilization.ReseachingAdvance = res.Values[0] switch
                {
                    Tech tech => tech.id,
                    Advance advance => advance.Index,
                    int index and >= 0 when index < researchPossibilities.Count => researchPossibilities[index].Index,
                    _ => game.Random.ChooseFrom(researchPossibilities).Index
                };
            }
        }

        public void CantProduce(City city, IProductionOrder? newItem)
        {
        }

        public void CityProductionComplete(City city)
        {
            var productionOrders = ProductionPossibilities.GetAllowedProductionOrders(city);
            Ai.Call(AiEvent.CityProductionComplete,
                new LuaTable { { "city", city }, { "productionOrders", productionOrders } });
        }

        public IInterfaceCommands Ui { get; } = null!;
        public string AIScript { get; set; } = civilization.PlayerType == PlayerType.Barbarians ? "barbarian.ai" : "default.ai";

        public void NotifyImprovementEnabled(TerrainImprovement improvement, int level)
        {
        }

        public void MapChanged(List<Tile> tiles)
        {
        }

        public void WaitingAtEndOfTurn()
        {
            Ai.Call(AiEvent.TurnEnd, new LuaTable());
            game.ChoseNextCiv();
        }

        public void NotifyAdvanceResearched(int advance)
        {
        }

        public void FoodShortage(City city)
        {
        }

        public void CityDecrease(City city)
        {
        }

        public void TurnStart(int turnNumber)
        {
            WaitingList.Clear();
            Ai.Call(AiEvent.TurnStart, new LuaTable { { "Turn", turnNumber } });
        }

        public void SetUnitActive(Unit? unit, bool move)
        {
            ActiveUnit = unit;
            if (unit == null) return;

            if (unit.CurrentLocation != null) ActiveTile = unit.CurrentLocation;
            if (!move) return;

            var safety = 0;
            while (!unit.Dead && unit.MovePoints > 0 && safety++ < 12)
            {
                var oldTile = unit.CurrentLocation;
                var oldMovesLost = unit.MovePointsLost;
                var oldOrder = unit.Order;

                var action = GetScriptedAction(unit) ?? GetFallbackAction(unit) ?? new NothingAction(unit, game);
                action.Execute();

                if (unit.Dead || unit.MovePoints <= 0)
                {
                    break;
                }

                if (unit.CurrentLocation == oldTile && unit.MovePointsLost == oldMovesLost && unit.Order == oldOrder)
                {
                    unit.SkipTurn();
                    break;
                }
            }
        }

        private UnitAction? GetScriptedAction(Unit unit)
        {
            var result = Ai.Call(AiEvent.UnitOrdersNeeded, new LuaTable { { "Unit", new UnitApi(unit, game) } });
            if (result is not LuaResult { Count: > 0 } luaResult)
            {
                return null;
            }

            return luaResult[0] switch
            {
                UnitAction luaAction => luaAction,
                TileApi tile => TileToAction(tile.BaseTile, unit),
                "B" or "b" => new BuildCityAction(unit, game),
                "F" or "f" => new FortifyAction(unit, game),
                "W" or "w" => new WaitAction(unit, game, this),
                _ => null
            };
        }

        private UnitAction? GetFallbackAction(Unit unit)
        {
            if (unit.CurrentLocation == null || unit.Dead)
            {
                return null;
            }

            var currentTile = unit.CurrentLocation;

            if (unit.AttackBase > 0)
            {
                var adjacentEnemy = AdjacentEnemyAction(unit, currentTile);
                if (adjacentEnemy != null)
                {
                    return adjacentEnemy;
                }
            }

            if (unit.AiRole == AiRoleType.Settle || unit.TypeDefinition.IsSettler)
            {
                return SettlerAction(unit, currentTile);
            }

            if (unit.AiRole == AiRoleType.Defend && currentTile.CityHere?.Owner == unit.Owner)
            {
                return new FortifyAction(unit, game);
            }

            if (unit.AttackBase > 0)
            {
                var targetAction = MoveTowardNearestEnemy(unit, currentTile);
                if (targetAction != null)
                {
                    return targetAction;
                }
            }

            return ExploreAction(unit, currentTile);
        }

        private UnitAction? AdjacentEnemyAction(Unit unit, Tile currentTile)
        {
            var attackTile = MovementFunctions.GetPossibleMoves(currentTile, unit)
                .Where(tile => IsEnemyTile(unit, tile))
                .OrderBy(tile => tile.CityHere == null ? 1 : 0)
                .ThenByDescending(tile => tile.UnitsHere.Any(other => other.AttackBase > 0))
                .FirstOrDefault();

            return attackTile == null ? null : TileToAction(attackTile, unit);
        }

        private UnitAction? SettlerAction(Unit unit, Tile currentTile)
        {
            if (CanFoundUsefulCity(unit, currentTile) && ShouldFoundCityNow(unit, currentTile))
            {
                return new BuildCityAction(unit, game);
            }

            var destination = MovementFunctions.GetPossibleMoves(currentTile, unit)
                .Where(tile => IsSafeForNonCombatUnit(unit, tile))
                .Where(tile => tile.Type != TerrainType.Ocean && !tile.Terrain.Impassable)
                .OrderByDescending(tile => CitySiteScore(unit, tile))
                .FirstOrDefault();

            if (destination != null && CitySiteScore(unit, destination) >= CitySiteScore(unit, currentTile))
            {
                return new MoveAction(unit, destination, game);
            }

            if (CanFoundUsefulCity(unit, currentTile))
            {
                return new BuildCityAction(unit, game);
            }

            return ExploreAction(unit, currentTile, avoidEnemies: true);
        }

        private bool ShouldFoundCityNow(Unit unit, Tile tile)
        {
            if (unit.Owner.Cities.Count == 0)
            {
                return true;
            }

            if (tile.Fertility >= 6)
            {
                return true;
            }

            return !MovementFunctions.GetPossibleMoves(tile, unit)
                .Any(next => CitySiteScore(unit, next) > CitySiteScore(unit, tile) + 25);
        }

        private bool CanFoundUsefulCity(Unit unit, Tile tile)
        {
            if (tile.Type == TerrainType.Ocean || tile.Terrain.Impassable || tile.CityHere != null)
            {
                return false;
            }

            if (tile.UnitsHere.Any(other => other.Owner != unit.Owner))
            {
                return false;
            }

            return !tile.CityRadius().Any(other => other.CityHere != null && other.CityHere.Owner == unit.Owner);
        }

        private int CitySiteScore(Unit unit, Tile tile)
        {
            if (!CanFoundUsefulCity(unit, tile))
            {
                return int.MinValue / 2;
            }

            var score = (int)(tile.Fertility * 100);
            if (tile.River) score += 80;
            if (tile.Special >= 0) score += 90;
            if (tile.HasShield) score += 25;
            if (tile.Resource) score += 40;
            if (tile.HasGoodyHut) score -= 75;

            var friendlyCityDistance = unit.Owner.Cities.Count == 0
                ? 8d
                : unit.Owner.Cities.Min(city => Utilities.DistanceTo(tile, city.Location));
            score += (int)(Math.Min(friendlyCityDistance, 10d) * 8d);

            var enemyNearby = tile.Neighbours().Any(neighbour => IsEnemyTile(unit, neighbour));
            if (enemyNearby)
            {
                score -= 150;
            }

            return score;
        }

        private UnitAction? MoveTowardNearestEnemy(Unit unit, Tile currentTile)
        {
            var target = FindNearestEnemyTile(unit, currentTile);
            if (target == null)
            {
                return null;
            }

            var destination = MovementFunctions.GetPossibleMoves(currentTile, unit)
                .Where(tile => tile == target || !BlocksCombatUnit(unit, tile))
                .OrderBy(tile => Utilities.DistanceTo(tile, target))
                .ThenByDescending(tile => tile.CityHere != null && tile.CityHere.Owner != unit.Owner)
                .FirstOrDefault();

            return destination == null ? null : TileToAction(destination, unit);
        }

        private UnitAction? ExploreAction(Unit unit, Tile currentTile, bool avoidEnemies = false)
        {
            var moves = MovementFunctions.GetPossibleMoves(currentTile, unit)
                .Where(tile => unit.AttackBase > 0 || IsSafeForNonCombatUnit(unit, tile))
                .Where(tile => !avoidEnemies || !tile.Neighbours().Any(neighbour => IsEnemyTile(unit, neighbour)))
                .ToList();

            if (moves.Count == 0)
            {
                return new NothingAction(unit, game);
            }

            var destination = moves
                .OrderBy(tile => tile.Visibility.Length > unit.Owner.Id && tile.Visibility[unit.Owner.Id] ? 1 : 0)
                .ThenByDescending(tile => tile.Fertility)
                .ThenBy(_ => game.Random.Next(1000))
                .First();

            return TileToAction(destination, unit);
        }

        private Tile? FindNearestEnemyTile(Unit unit, Tile currentTile)
        {
            Tile? best = null;
            var bestDistance = double.MaxValue;
            foreach (var tile in currentTile.Map.Tile)
            {
                if (!IsEnemyTile(unit, tile))
                {
                    continue;
                }

                if (unit.Domain == UnitGas.Ground && tile.Type == TerrainType.Ocean)
                {
                    continue;
                }

                var distance = Utilities.DistanceTo(currentTile, tile);
                if (distance < bestDistance)
                {
                    best = tile;
                    bestDistance = distance;
                }
            }

            return best;
        }

        private static bool IsSafeForNonCombatUnit(Unit unit, Tile tile)
        {
            if (tile.UnitsHere.Any(other => other.Owner != unit.Owner))
            {
                return false;
            }

            if (tile.CityHere != null && tile.CityHere.Owner != unit.Owner)
            {
                return false;
            }

            return !tile.Neighbours().Any(neighbour => IsEnemyTile(unit, neighbour));
        }

        private static bool BlocksCombatUnit(Unit unit, Tile tile)
        {
            return tile.UnitsHere.Any(other => other.Owner == unit.Owner && other.InShip == null) &&
                   tile.CityHere?.Owner != unit.Owner;
        }

        private static bool IsEnemyTile(Unit unit, Tile tile)
        {
            if (tile.CityHere != null && tile.CityHere.Owner != unit.Owner)
            {
                return true;
            }

            return tile.UnitsHere.Any(other => !other.Dead && other.Owner != unit.Owner && other.InShip == null);
        }

        private void CallUnitsLost(LuaTable units, Unit? by)
        {
            var args = new LuaTable { { "Units", units } };
            if (by != null)
            {
                args.Add("By", new UnitApi(by, game));
            }

            Ai.Call(AiEvent.UnitsLost, args);
        }

        public void UnitLost(Unit unit, Unit? killedBy)
        {
            var units = new LuaTable { new UnitApi(unit, game) };
            CallUnitsLost(units, killedBy);
        }

        public void UnitsLost(List<Unit> deadUnits, Unit? killedBy)
        {
            var units = new LuaTable();
            foreach (var u in deadUnits)
            {
                units.Add(new UnitApi(u, game));
            }
            CallUnitsLost(units, killedBy);
        }

        public void UnitMoved(Unit unit, Tile tileTo, Tile tileFrom)
        {
            Ai.Call(AiEvent.UnitMoved,
                new LuaTable
                {
                    { "Unit", new UnitApi(unit, game) },
                    { "TileTo", new TileApi(tileTo, game) },
                    { "TileFrom", new TileApi(tileFrom, game) }
                });
        }

        public void CombatHappened(CombatEventArgs combatEventArgs)
        {
        }

        public void MoveBlocked(Unit unit, BlockedReason blockedReason)
        {
        }

        public void GoodyHutTriggered(Unit unit, GoodyHutOutcomeResult outcome)
        {
        }

        public void SelectTechFromConquest(List<Advance> techs)
        {
            int result;
            var luaResult = Ai.Call(AiEvent.SelectTechFromConquest,
                new LuaTable { { "Techs", techs.Select(t => new Tech(game.Rules.Advances, t.Index)).ToList() } });
            if (luaResult is { Count: > 0 })
            {
                result = luaResult[0] switch
                {
                    int index and >= 0 when index < techs.Count => techs[index].Index,
                    int index when index > techs.Count => index % game.Rules.Advances.Length,
                    Advance advance => advance.Index,
                    Tech tech => tech.id,
                    _ => ai.Random.ChooseFrom(techs).Index
                };
            }
            else
            {
                result = ai.Random.ChooseFrom(techs).Index;
            }
            game.GiveAdvance(result, Civilization);
        }

        public void CityLost(City city)
        {
            Ai.Call(AiEvent.CityLost, new LuaTable { { "city", new CityApi(city, game) } });
        }

        public void CityCaptured(City city)
        {
            Ai.Call(AiEvent.CityCaptured, new LuaTable { { "city", new CityApi(city, game) } });
        }

        private UnitAction? TileToAction(Tile tile, Unit unit)
        {
            if (tile == unit.CurrentLocation)
            {
                return new NothingAction(unit, game);
            }

            if (!unit.CurrentLocation.Neighbours().Contains(tile))
            {
                return new GotoAction(unit, tile, game);
            }

            if (tile.UnitsHere.Count > 0 && tile.UnitsHere[0].Owner.Id != unit.Owner.Id)
            {
                return new AttackAction(unit, tile, game);
            }
            return new MoveAction(unit, tile, game);
        }
    }
}
