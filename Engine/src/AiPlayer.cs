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
using Model.Core;
using Model.Core.Advances;
using Model.Core.Units;
using Neo.IronLua;

namespace Civ2engine
{
    public class AiPlayer : IPlayer
    {
        private readonly Game _game;
        public AiInterface AI;
        public Civilization Civilization { get; set; }

        public int DifficultyLevel { get; }

        public AiPlayer(int difficultyLevel, Civilization civilization, Tile tile0, Game game)
        {
            _game = game;
            DifficultyLevel = difficultyLevel;
            Civilization = civilization;
            ActiveTile = civilization.Units.FirstOrDefault()?.CurrentLocation ??
                         civilization.Cities.FirstOrDefault()?.Location ?? tile0; 
            AIScript = civilization.PlayerType == PlayerType.Barbarians ? "barbarian.ai" : "default.ai";
        }

        public void WeLoveTheKingCanceled(City city)
        {
        }

        public Tile ActiveTile { get; set; }
        public Unit? ActiveUnit { get; set; }

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

        public void SelectNewAdvance(IGame game, List<Advance> researchPossibilities)
        {
            var res = AI.Call(AiEvent.ResearchComplete,
                new LuaTable { { "researchPossibilities", researchPossibilities } });
            Civilization.ReseachingAdvance = res switch
            {
                Advance advance => advance.Index,
                int index and >= 0 when index < researchPossibilities.Count => researchPossibilities[index].Index,
                _ => game.Random.ChooseFrom(researchPossibilities).Index
            };
        }

        public void CantProduce(City city, IProductionOrder? newItem)
        {
            
        }

        public void CityProductionComplete(City city)
        {
            var productionOrders = ProductionPossibilities.GetAllowedProductionOrders(city);
            AI.Call(AiEvent.CityProductionComplete, new LuaTable { { "city", city }, { "productionOrders", productionOrders } });
        }

        public IInterfaceCommands Ui { get; } = null;
        public string AIScript { get; set; }

        public void NotifyImprovementEnabled(TerrainImprovement improvement, int level)
        {
            // 
        }

        public void MapChanged(List<Tile> tiles)
        {
            //Does the AI care?
        }

        public void WaitingAtEndOfTurn(IGame game)
        {
            //Call out to AI script to finalize turn
            AI.Call(AiEvent.TurnEnd, new LuaTable());
            
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
            AI.Call(AiEvent.TurnStart, new LuaTable { { "Turn", turnNumber } });
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
                var result = AI.Call(AiEvent.UnitOrdersNeeded, new LuaTable { { "Unit", new UnitApi(unit, _game) } });
                if (result is LuaResult { Count: > 0 } luaResult)
                {
                    action = luaResult[0] switch
                    {
                        UnitAction luaAction => luaAction,
                        TileApi tile => TileToAction(tile.BaseTile, unit),
                        "B" or "b" => new BuildCityAction(unit, _game),
                        "F" or "f" => new FortifyAction(unit, _game),
                        "W" or "w" => new WaitAction(unit, _game, this),
                        _ => action
                    };
                }

                action ??= new NothingAction(unit, _game);
                action.Execute();
            }
        }

        private void CallUnitsLost(LuaTable units, Unit? by)
        {
            var args = new LuaTable { { "Units", units } };
            if (by != null)
            {
                args.Add("By", new UnitApi(by, _game));
            }

            AI.Call(AiEvent.UnitsLost, args);
        }

        public void UnitLost(Unit unit, Unit? killedBy)
        {
            var units = new LuaTable { new UnitApi(unit, _game) };
            CallUnitsLost(units, killedBy);
        }

        public void UnitsLost(List<Unit> deadUnits, Unit? killedBy)
        {
            var units = new LuaTable();
            foreach (var u in deadUnits)
            {
                units.Add(new UnitApi(u, _game));
            }
            CallUnitsLost(units, killedBy);
        }

        public void UnitMoved(Unit unit, Tile tileTo, Tile tileFrom)
        {
            AI.Call(AiEvent.UnitMoved, new LuaTable { { "Unit", new UnitApi(unit, _game) }, { "TileTo", new TileApi(tileTo, _game)}, { "TileFrom", new TileApi(tileFrom, _game)} });
        }

        public void CombatHappened(CombatEventArgs combatEventArgs)
        {
            //TODO: Does the AI care?
        }

        public void MoveBlocked(Unit unit, BlockedReason blockedReason)
        {
            //TODO: Does the AI care? Can this even happen?
        }

        private UnitAction? TileToAction(Tile tile, Unit unit)
        {
            if (tile == unit.CurrentLocation)
            {
                return new NothingAction(unit, _game);
            }

            if (!unit.CurrentLocation.Neighbours().Contains(tile))
            {
                return new GotoAction(unit, tile, _game);
            }
            
            if (tile.UnitsHere.Count > 0 && tile.UnitsHere[0].Owner.Id != unit.Owner.Id)
            {
                return new AttackAction(unit, tile, _game);
            }
            return new MoveAction(unit, tile, _game);
        }
    }
}