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
using Model.Core.Advances;
using Model.Core.GoodyHuts.Outcomes;
using Model.Core.Player;
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
            Ai.Call(AiEvent.CityProductionComplete, new LuaTable { { "city", city }, { "productionOrders", productionOrders } });
        }

        public IInterfaceCommands Ui { get; } = null;
        public string AIScript { get; set; } = civilization.PlayerType == PlayerType.Barbarians ? "barbarian.ai" : "default.ai";

        public void NotifyImprovementEnabled(TerrainImprovement improvement, int level)
        {
            // 
        }

        public void MapChanged(List<Tile> tiles)
        {
            //Does the AI care?
        }

        public void WaitingAtEndOfTurn()
        {
            //Call out to AI script to finalize turn
            Ai.Call(AiEvent.TurnEnd, new LuaTable());
            
            //End turn
            game.ChoseNextCiv();
        }

        public void NotifyAdvanceResearched(int advance)
        {
            // What should the AI do??
        }

        public void FoodShortage(City city)
        {
            // What should the AI do??
        }

        public void CityDecrease(City city)
        {
            // What should the AI do??
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
            
            UnitAction? action = null;
            while (unit.MovePoints > 0)
            {
                var result = Ai.Call(AiEvent.UnitOrdersNeeded, new LuaTable { { "Unit", new UnitApi(unit, game) } });
                if (result is LuaResult { Count: > 0 } luaResult)
                {
                    action = luaResult[0] switch
                    {
                        UnitAction luaAction => luaAction,
                        TileApi tile => TileToAction(tile.BaseTile, unit),
                        "B" or "b" => new BuildCityAction(unit, game),
                        "F" or "f" => new FortifyAction(unit, game),
                        "W" or "w" => new WaitAction(unit, game, this),
                        _ => action
                    };
                }

                action ??= new NothingAction(unit, game);
                action.Execute();
            }
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
            Ai.Call(AiEvent.UnitMoved, new LuaTable { { "Unit", new UnitApi(unit, game) }, { "TileTo", new TileApi(tileTo, game)}, { "TileFrom", new TileApi(tileFrom, game)} });
        }

        public void CombatHappened(CombatEventArgs combatEventArgs)
        {
            //TODO: Does the AI care?
        }

        public void MoveBlocked(Unit unit, BlockedReason blockedReason)
        {
            //TODO: Does the AI care? Can this even happen?
        }

        public void GoodyHutTriggered(Unit unit, GoodyHutOutcomeResult outcome)
        {
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